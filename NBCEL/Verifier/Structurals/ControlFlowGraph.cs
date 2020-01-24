/*
* Licensed to the Apache Software Foundation (ASF) under one or more
* contributor license agreements.  See the NOTICE file distributed with
* this work for additional information regarding copyright ownership.
* The ASF licenses this file to You under the Apache License, Version 2.0
* (the "License"); you may not use this file except in compliance with
* the License.  You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
*  Unless required by applicable law or agreed to in writing, software
*  distributed under the License is distributed on an "AS IS" BASIS,
*  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
*  See the License for the specific language governing permissions and
*  limitations under the License.
*
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Apache.NBCEL.Generic;
using Apache.NBCEL.Verifier.Exc;

namespace Apache.NBCEL.Verifier.Structurals
{
    /// <summary>This class represents a control flow graph of a method.</summary>
    public class ControlFlowGraph
    {
	    /// <summary>
	    ///     The ExceptionHandlers object for the method whose control flow is represented by this ControlFlowGraph.
	    /// </summary>
	    private readonly ExceptionHandlers exceptionhandlers;

        /// <summary>All InstructionContext instances of this ControlFlowGraph.</summary>
        private readonly IDictionary<InstructionHandle
            , InstructionContext> instructionContexts = new Dictionary
            <InstructionHandle, InstructionContext>
            ();

        /// <summary>
        ///     The Subroutines object for the method whose control flow is represented by this ControlFlowGraph.
        /// </summary>
        private readonly Subroutines subroutines;

        /// <summary>A Control Flow Graph; with additional JustIce checks</summary>
        /// <param name="method_gen">the method generator instance</param>
        public ControlFlowGraph(MethodGen method_gen)
            : this(method_gen, true)
        {
        }

        /// <summary>A Control Flow Graph.</summary>
        /// <param name="method_gen">the method generator instance</param>
        /// <param name="enableJustIceCheck">
        ///     if true, additional JustIce checks are performed
        /// </param>
        /// <since>6.0</since>
        public ControlFlowGraph(MethodGen method_gen, bool enableJustIceCheck
        )
        {
            // End Inner InstructionContextImpl Class.
            ///** The MethodGen object we're working on. */
            //private final MethodGen method_gen;
            subroutines = new Subroutines(method_gen, enableJustIceCheck
            );
            exceptionhandlers = new ExceptionHandlers(method_gen);
            var instructionhandles = method_gen.GetInstructionList
                ().GetInstructionHandles();
            foreach (var instructionhandle in instructionhandles)
                Collections.Put(instructionContexts, instructionhandle, new InstructionContextImpl
                    (this, instructionhandle));
        }

        //this.method_gen = method_gen;
        /// <summary>Returns the InstructionContext of a given instruction.</summary>
        public virtual InstructionContext ContextOf(InstructionHandle
            inst)
        {
            var ic = instructionContexts.GetOrNull(
                inst);
            if (ic == null)
                throw new AssertionViolatedException(
                    "InstructionContext requested for an InstructionHandle that's not known!"
                );
            return ic;
        }

        /// <summary>
        ///     Returns the InstructionContext[] of a given InstructionHandle[],
        ///     in a naturally ordered manner.
        /// </summary>
        public virtual InstructionContext[] ContextsOf(InstructionHandle
            [] insts)
        {
            var ret = new InstructionContext
                [insts.Length];
            for (var i = 0; i < insts.Length; i++) ret[i] = ContextOf(insts[i]);
            return ret;
        }

        /// <summary>
        ///     Returns an InstructionContext[] with all the InstructionContext instances
        ///     for the method whose control flow is represented by this ControlFlowGraph
        ///     <B>(NOT ORDERED!)</B>.
        /// </summary>
        public virtual InstructionContext[] GetInstructionContexts
            ()
        {
            var ret = new InstructionContext
                [instructionContexts.Values.Count];
            return Collections.ToArray(instructionContexts.Values, ret);
        }

        /// <summary>
        ///     Returns true, if and only if the said instruction is not reachable; that means,
        ///     if it is not part of this ControlFlowGraph.
        /// </summary>
        public virtual bool IsDead(InstructionHandle i)
        {
            return subroutines.SubroutineOf(i) == null;
        }

        /// <summary>Objects of this class represent a node in a ControlFlowGraph.</summary>
        /// <remarks>
        ///     Objects of this class represent a node in a ControlFlowGraph.
        ///     These nodes are instructions, not basic blocks.
        /// </remarks>
        private class InstructionContextImpl : InstructionContext
        {
            private readonly ControlFlowGraph _enclosing;

            /// <summary>The 'incoming' execution Frames.</summary>
            private readonly IDictionary<InstructionContext
                , Frame> inFrames;

            /// <summary>The InstructionHandle this InstructionContext is wrapped around.</summary>
            private readonly InstructionHandle instruction;

            /// <summary>The 'outgoing' execution Frames.</summary>
            private readonly IDictionary<InstructionContext
                , Frame> outFrames;

            /// <summary>
            ///     The 'execution predecessors' - a list of type InstructionContext
            ///     of those instances that have been execute()d before in that order.
            /// </summary>
            private List<InstructionContext
            > executionPredecessors;

            /// <summary>
            ///     The TAG field is here for external temporary flagging, such
            ///     as graph colouring.
            /// </summary>
            /// <seealso cref="GetTag()" />
            /// <seealso cref="SetTag(int)" />
            private int TAG;

            /// <summary>Creates an InstructionHandleImpl object from an InstructionHandle.</summary>
            /// <remarks>
            ///     Creates an InstructionHandleImpl object from an InstructionHandle.
            ///     Creation of one per InstructionHandle suffices. Don't create more.
            /// </remarks>
            public InstructionContextImpl(ControlFlowGraph _enclosing, InstructionHandle
                inst)
            {
                this._enclosing = _enclosing;
                // key: the last-executed JSR
                // key: the last-executed JSR
                // Type: InstructionContext
                if (inst == null)
                    throw new AssertionViolatedException("Cannot instantiate InstructionContextImpl from NULL."
                    );
                instruction = inst;
                inFrames = new Dictionary<InstructionContext
                    , Frame>();
                outFrames = new Dictionary<InstructionContext
                    , Frame>();
            }

            /* Satisfies InstructionContext.getTag(). */
            public virtual int GetTag()
            {
                return TAG;
            }

            /* Satisfies InstructionContext.setTag(int). */
            public virtual void SetTag(int tag)
            {
                // part of InstructionContext interface
                TAG = tag;
            }

            /// <summary>Returns the exception handlers of this instruction.</summary>
            public virtual ExceptionHandler[] GetExceptionHandlers
                ()
            {
                return _enclosing.exceptionhandlers.GetExceptionHandlers(GetInstruction
                    ());
            }

            /// <summary>
            ///     Returns a clone of the "outgoing" frame situation with respect to the given ExecutionChain.
            /// </summary>
            public virtual Frame GetOutFrame(List
                <InstructionContext> execChain)
            {
                executionPredecessors = execChain;
                Frame org;
                InstructionContext jsr = LastExecutionJSR();
                org = outFrames.GetOrNull(jsr);
                if (org == null)
                    throw new AssertionViolatedException("outFrame not set! This:\n"
                                                         + this + "\nExecutionChain: " + GetExecutionChain() +
                                                         "\nOutFrames: '" + outFrames + "'.");
                return org.GetClone();
            }

            public virtual Frame GetInFrame()
            {
                Frame org;
                InstructionContext jsr = LastExecutionJSR();
                org = inFrames.GetOrNull(jsr);
                if (org == null)
                    throw new AssertionViolatedException("inFrame not set! This:\n"
                                                         + this + "\nInFrames: '" + inFrames + "'.");
                return org.GetClone();
            }

            /// <summary>
            ///     "Merges in" (vmspec2, page 146) the "incoming" frame situation;
            ///     executes the instructions symbolically
            ///     and therefore calculates the "outgoing" frame situation.
            /// </summary>
            /// <remarks>
            ///     "Merges in" (vmspec2, page 146) the "incoming" frame situation;
            ///     executes the instructions symbolically
            ///     and therefore calculates the "outgoing" frame situation.
            ///     Returns: True iff the "incoming" frame situation changed after
            ///     merging with "inFrame".
            ///     The execPreds ArrayList must contain the InstructionContext
            ///     objects executed so far in the correct order. This is just
            ///     one execution path [out of many]. This is needed to correctly
            ///     "merge" in the special case of a RET's successor.
            ///     <B>
            ///         The InstConstraintVisitor and ExecutionVisitor instances
            ///         must be set up correctly.
            ///     </B>
            /// </remarks>
            /// <returns>
            ///     true - if and only if the "outgoing" frame situation
            ///     changed from the one before execute()ing.
            /// </returns>
            public virtual bool Execute(Frame inFrame, List
                <InstructionContext> execPreds, InstConstraintVisitor
                icv, ExecutionVisitor ev)
            {
                var clone
                    = execPreds.ToList();
                // OK because execPreds is compatible type
                executionPredecessors = clone;
                //sanity check
                if (LastExecutionJSR() == null && _enclosing.subroutines.SubroutineOf
                        (GetInstruction()) != _enclosing.subroutines.GetTopLevel())
                    throw new AssertionViolatedException("Huh?! Am I '" + this + "' part of a subroutine or not?"
                    );
                if (LastExecutionJSR() != null && _enclosing.subroutines.SubroutineOf
                        (GetInstruction()) == _enclosing.subroutines.GetTopLevel())
                    throw new AssertionViolatedException("Huh?! Am I '" + this + "' part of a subroutine or not?"
                    );
                var inF = inFrames.GetOrNull(LastExecutionJSR
                    ());
                if (inF == null)
                {
                    // no incoming frame was set, so set it.
                    Collections.Put(inFrames, LastExecutionJSR(), inFrame);
                    inF = inFrame;
                }
                else
                {
                    // if there was an "old" inFrame
                    if (inF.Equals(inFrame))
                        //shortcut: no need to merge equal frames.
                        return false;
                    if (!MergeInFrames(inFrame)) return false;
                }

                // Now we're sure the inFrame has changed!
                // new inFrame is already merged in, see above.
                var workingFrame = inF.GetClone();
                try
                {
                    // This verifies the InstructionConstraint for the current
                    // instruction, but does not modify the workingFrame object.
                    //InstConstraintVisitor icv = InstConstraintVisitor.getInstance(VerifierFactory.getVerifier(method_gen.getClassName()));
                    icv.SetFrame(workingFrame);
                    GetInstruction().Accept(icv);
                }
                catch (StructuralCodeConstraintException ce)
                {
                    ce.ExtendMessage(string.Empty, "\nInstructionHandle: " + GetInstruction() +
                                                   "\n");
                    ce.ExtendMessage(string.Empty, "\nExecution Frame:\n" + workingFrame);
                    ExtendMessageWithFlow(ce);
                    throw;
                }

                // This executes the Instruction.
                // Therefore the workingFrame object is modified.
                //ExecutionVisitor ev = ExecutionVisitor.getInstance(VerifierFactory.getVerifier(method_gen.getClassName()));
                ev.SetFrame(workingFrame);
                GetInstruction().Accept(ev);
                //getInstruction().accept(ExecutionVisitor.withFrame(workingFrame));
                Collections.Put(outFrames, LastExecutionJSR(), workingFrame);
                return true;
            }

            /*
            * Fulfils the contract of InstructionContext.getInstruction().
            */
            public virtual InstructionHandle GetInstruction()
            {
                return instruction;
            }

            /* Satisfies InstructionContext.getSuccessors(). */
            public virtual InstructionContext[] GetSuccessors()
            {
                return _enclosing.ContextsOf(_getSuccessors());
            }

            // new inFrame was different from old inFrame so merging them
            // yielded a different this.inFrame.
            /// <summary>Returns a simple String representation of this InstructionContext.</summary>
            public override string ToString()
            {
                //TODO: Put information in the brackets, e.g.
                //      Is this an ExceptionHandler? Is this a RET? Is this the start of
                //      a subroutine?
                var ret = GetInstruction().ToString(false) + "\t[InstructionContext]";
                return ret;
            }

            /// <summary>Does the actual merging (vmspec2, page 146).</summary>
            /// <remarks>
            ///     Does the actual merging (vmspec2, page 146).
            ///     Returns true IFF this.inFrame was changed in course of merging with inFrame.
            /// </remarks>
            private bool MergeInFrames(Frame inFrame)
            {
                // TODO: Can be performance-improved.
                var inF = inFrames.GetOrNull(LastExecutionJSR
                    ());
                var oldstack = inF.GetStack().GetClone();
                var oldlocals = inF.GetLocals().GetClone();
                try
                {
                    inF.GetStack().Merge(inFrame.GetStack());
                    inF.GetLocals().Merge(inFrame.GetLocals());
                }
                catch (StructuralCodeConstraintException sce)
                {
                    ExtendMessageWithFlow(sce);
                    throw;
                }

                return !(oldstack.Equals(inF.GetStack()) && oldlocals.Equals(inF.GetLocals()));
            }

            /// <summary>Returns the control flow execution chain.</summary>
            /// <remarks>
            ///     Returns the control flow execution chain. This is built
            ///     while execute(Frame, ArrayList)-ing the code represented
            ///     by the surrounding ControlFlowGraph.
            /// </remarks>
            private string GetExecutionChain()
            {
                var s = ToString();
                for (var i = executionPredecessors.Count - 1; i >= 0; i--) s = executionPredecessors[i] + "\n" + s;
                return s;
            }

            /// <summary>
            ///     Extends the StructuralCodeConstraintException ("e") object with an at-the-end-extended message.
            /// </summary>
            /// <remarks>
            ///     Extends the StructuralCodeConstraintException ("e") object with an at-the-end-extended message.
            ///     This extended message will then reflect the execution flow needed to get to the constraint
            ///     violation that triggered the throwing of the "e" object.
            /// </remarks>
            private void ExtendMessageWithFlow(StructuralCodeConstraintException
                e)
            {
                var s = "Execution flow:\n";
                e.ExtendMessage(string.Empty, s + GetExecutionChain());
            }

            /// <summary>
            ///     Returns the InstructionContextImpl with an JSR/JSR_W
            ///     that was last in the ExecutionChain, without
            ///     a corresponding RET, i.e.
            /// </summary>
            /// <remarks>
            ///     Returns the InstructionContextImpl with an JSR/JSR_W
            ///     that was last in the ExecutionChain, without
            ///     a corresponding RET, i.e.
            ///     we were called by this one.
            ///     Returns null if we were called from the top level.
            /// </remarks>
            private InstructionContextImpl LastExecutionJSR
                ()
            {
                var size = executionPredecessors.Count;
                var retcount = 0;
                for (var i = size - 1; i >= 0; i--)
                {
                    var current = (InstructionContextImpl
                        ) executionPredecessors[i];
                    var currentlast = current.GetInstruction().GetInstruction();
                    if (currentlast is RET) retcount++;
                    if (currentlast is JsrInstruction)
                    {
                        retcount--;
                        if (retcount == -1) return current;
                    }
                }

                return null;
            }

            /// <summary>
            ///     A utility method that calculates the successors of a given InstructionHandle
            ///     That means, a RET does have successors as defined here.
            /// </summary>
            /// <remarks>
            ///     A utility method that calculates the successors of a given InstructionHandle
            ///     That means, a RET does have successors as defined here.
            ///     A JsrInstruction has its target as its successor
            ///     (opposed to its physical successor) as defined here.
            /// </remarks>
            private InstructionHandle[] _getSuccessors()
            {
                // TODO: implement caching!
                var empty = new InstructionHandle[0];
                var single = new InstructionHandle[1]
                    ;
                var inst = GetInstruction().GetInstruction();
                if (inst is RET)
                {
                    var s = _enclosing.subroutines.SubroutineOf
                        (GetInstruction());
                    if (s == null)
                        //return empty;
                        // RET in dead code. "empty" would be the correct answer, but we know something about the surrounding project...
                        throw new AssertionViolatedException("Asking for successors of a RET in dead code?!"
                        );
                    //TODO: remove. Only JustIce must not use it, but foreign users of the ControlFlowGraph
                    //      will want it. Thanks Johannes Wust.
                    //throw new AssertionViolatedException("DID YOU REALLY WANT TO ASK FOR RET'S SUCCS?");
                    var jsrs = s.GetEnteringJsrInstructions();
                    var ret = new InstructionHandle[jsrs.Length];
                    for (var i = 0; i < jsrs.Length; i++) ret[i] = jsrs[i].GetNext();
                    return ret;
                }

                // Terminates method normally.
                if (inst is ReturnInstruction) return empty;
                // Terminates method abnormally, because JustIce mandates
                // subroutines not to be protected by exception handlers.
                if (inst is ATHROW) return empty;
                // See method comment.
                if (inst is JsrInstruction)
                {
                    single[0] = ((JsrInstruction) inst).GetTarget();
                    return single;
                }

                if (inst is GotoInstruction)
                {
                    single[0] = ((GotoInstruction) inst).GetTarget();
                    return single;
                }

                if (inst is BranchInstruction)
                {
                    if (inst is Select)
                    {
                        // BCEL's getTargets() returns only the non-default targets,
                        // thanks to Eli Tilevich for reporting.
                        var matchTargets = ((Select) inst).GetTargets
                            ();
                        var ret = new InstructionHandle[matchTargets
                                                            .Length + 1];
                        ret[0] = ((Select) inst).GetTarget();
                        Array.Copy(matchTargets, 0, ret, 1, matchTargets.Length);
                        return ret;
                    }

                    var pair = new InstructionHandle[2];
                    pair[0] = GetInstruction().GetNext();
                    pair[1] = ((BranchInstruction) inst).GetTarget();
                    return pair;
                }

                // default case: Fall through.
                single[0] = GetInstruction().GetNext();
                return single;
            }
        }
    }
}