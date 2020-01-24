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
using System.IO;
using System.Linq;
using Apache.NBCEL.ClassFile;
using Apache.NBCEL.Generic;
using Apache.NBCEL.Verifier.Exc;
using Apache.NBCEL.Verifier.Statics;
using Type = Apache.NBCEL.Generic.Type;

namespace Apache.NBCEL.Verifier.Structurals
{
	/// <summary>
	///     This PassVerifier verifies a method of class file according to pass 3,
	///     so-called structural verification as described in The Java Virtual Machine
	///     Specification, 2nd edition.
	/// </summary>
	/// <remarks>
	///     This PassVerifier verifies a method of class file according to pass 3,
	///     so-called structural verification as described in The Java Virtual Machine
	///     Specification, 2nd edition.
	///     More detailed information is to be found at the do_verify() method's
	///     documentation.
	/// </remarks>
	/// <seealso cref="Do_verify()" />
	public sealed class Pass3bVerifier : PassVerifier
    {
        /// <summary>In DEBUG mode, the verification algorithm is not randomized.</summary>
        private const bool DEBUG = true;

        /// <summary>The method number to verify.</summary>
        private readonly int method_no;

        /// <summary>The Verifier that created this.</summary>
        private readonly Verifier myOwner;

        /// <summary>This class should only be instantiated by a Verifier.</summary>
        /// <seealso cref="Verifier" />
        public Pass3bVerifier(Verifier owner, int method_no)
        {
            // end Inner Class InstructionContextQueue
            myOwner = owner;
            this.method_no = method_no;
        }

        /// <summary>
        ///     Whenever the outgoing frame
        ///     situation of an InstructionContext changes, all its successors are
        ///     put [back] into the queue [as if they were unvisited].
        /// </summary>
        /// <remarks>
        ///     Whenever the outgoing frame
        ///     situation of an InstructionContext changes, all its successors are
        ///     put [back] into the queue [as if they were unvisited].
        ///     The proof of termination is about the existence of a
        ///     fix point of frame merging.
        /// </remarks>
        private void CirculationPump(MethodGen m, ControlFlowGraph
            cfg, InstructionContext start, Frame
            vanillaFrame, InstConstraintVisitor icv, ExecutionVisitor
            ev)
        {
            var random = new Random();
            var icq = new InstructionContextQueue
                ();
            start.Execute(vanillaFrame, new List<InstructionContext
            >(), icv, ev);
            // new ArrayList() <=>    no Instruction was executed before
            //                                    => Top-Level routine (no jsr call before)
            icq.Add(start, new List<InstructionContext
            >());
            // LOOP!
            while (!icq.IsEmpty())
            {
                InstructionContext u;
                List<InstructionContext> ec;
                u = icq.GetIC(0);
                ec = icq.GetEC(0);
                icq.Remove(0);
                var oldchain
                    = ec.ToList();
                // ec is of type ArrayList<InstructionContext>
                var newchain
                    = ec.ToList();
                // ec is of type ArrayList<InstructionContext>
                newchain.Add(u);
                if (u.GetInstruction().GetInstruction() is RET)
                {
                    //System.err.println(u);
                    // We can only follow _one_ successor, the one after the
                    // JSR that was recently executed.
                    var ret = (RET) u.GetInstruction().GetInstruction();
                    var t = (ReturnaddressType) u.GetOutFrame
                        (oldchain).GetLocals().Get(ret.GetIndex());
                    var theSuccessor = cfg.ContextOf(t.GetTarget
                        ());
                    // Sanity check
                    InstructionContext lastJSR = null;
                    var skip_jsr = 0;
                    for (var ss = oldchain.Count - 1; ss >= 0; ss--)
                    {
                        if (skip_jsr < 0)
                            throw new AssertionViolatedException("More RET than JSR in execution chain?!"
                            );
                        //System.err.println("+"+oldchain.get(ss));
                        if (oldchain[ss].GetInstruction().GetInstruction() is JsrInstruction)
                        {
                            if (skip_jsr == 0)
                            {
                                lastJSR = oldchain[ss];
                                break;
                            }

                            skip_jsr--;
                        }

                        if (oldchain[ss].GetInstruction().GetInstruction() is RET) skip_jsr++;
                    }

                    if (lastJSR == null)
                        throw new AssertionViolatedException("RET without a JSR before in ExecutionChain?! EC: '"
                                                             + oldchain + "'.");
                    var jsr = (JsrInstruction) lastJSR.GetInstruction
                        ().GetInstruction();
                    if (theSuccessor != cfg.ContextOf(jsr.PhysicalSuccessor()))
                        throw new AssertionViolatedException("RET '" + u.GetInstruction
                                                                 () + "' info inconsistent: jump back to '" +
                                                             theSuccessor + "' or '" + cfg.ContextOf
                                                                 (jsr.PhysicalSuccessor()) + "'?");
                    if (theSuccessor.Execute(u.GetOutFrame(oldchain), newchain, icv, ev))
                    {
                        var newchainClone
                            = newchain.ToList();
                        // newchain is already of type ArrayList<InstructionContext>
                        icq.Add(theSuccessor, newchainClone);
                    }
                }
                else
                {
                    // "not a ret"
                    // Normal successors. Add them to the queue of successors.
                    var succs = u.GetSuccessors();
                    foreach (var v in succs)
                        if (v.Execute(u.GetOutFrame(oldchain), newchain, icv, ev))
                        {
                            var newchainClone
                                = newchain.ToList();
                            // newchain is already of type ArrayList<InstructionContext>
                            icq.Add(v, newchainClone);
                        }
                }

                // end "not a ret"
                // Exception Handlers. Add them to the queue of successors.
                // [subroutines are never protected; mandated by JustIce]
                var exc_hds = u.GetExceptionHandlers();
                foreach (var exc_hd in exc_hds)
                {
                    var v = cfg.ContextOf(exc_hd.GetHandlerStart
                        ());
                    // TODO: the "oldchain" and "newchain" is used to determine the subroutine
                    // we're in (by searching for the last JSR) by the InstructionContext
                    // implementation. Therefore, we should not use this chain mechanism
                    // when dealing with exception handlers.
                    // Example: a JSR with an exception handler as its successor does not
                    // mean we're in a subroutine if we go to the exception handler.
                    // We should address this problem later; by now we simply "cut" the chain
                    // by using an empty chain for the exception handlers.
                    //if (v.execute(new Frame(u.getOutFrame(oldchain).getLocals(),
                    // new OperandStack (u.getOutFrame().getStack().maxStack(),
                    // (exc_hds[s].getExceptionType()==null? Type.THROWABLE : exc_hds[s].getExceptionType())) ), newchain), icv, ev) {
                    //icq.add(v, (ArrayList) newchain.clone());
                    if (v.Execute(new Frame(u.GetOutFrame(oldchain).GetLocals
                            (), new OperandStack(u.GetOutFrame(oldchain).GetStack
                                ().MaxStack(),
                            exc_hd.GetExceptionType() == null ? Type.THROWABLE : exc_hd.GetExceptionType())),
                        new List<InstructionContext
                        >(), icv, ev))
                        icq.Add(v, new List<InstructionContext
                        >());
                }
            }

            // while (!icq.isEmpty()) END
            var ih = start.GetInstruction();
            do
            {
                if (ih.GetInstruction() is ReturnInstruction && !cfg.IsDead(ih))
                {
                    var ic = cfg.ContextOf(ih);
                    // TODO: This is buggy, we check only the top-level return instructions this way.
                    // Maybe some maniac returns from a method when in a subroutine?
                    var f = ic.GetOutFrame(new List
                        <InstructionContext>());
                    var lvs = f.GetLocals();
                    for (var i = 0; i < lvs.MaxLocals(); i++)
                        if (lvs.Get(i) is UninitializedObjectType)
                            AddMessage("Warning: ReturnInstruction '" + ic +
                                       "' may leave method with an uninitialized object in the local variables array '"
                                       + lvs + "'.");
                    var os = f.GetStack();
                    for (var i = 0; i < os.Size(); i++)
                        if (os.Peek(i) is UninitializedObjectType)
                            AddMessage("Warning: ReturnInstruction '" + ic +
                                       "' may leave method with an uninitialized object on the operand stack '"
                                       + os + "'.");
                    //see JVM $4.8.2
                    Type returnedType = null;
                    var inStack = ic.GetInFrame().GetStack();
                    if (inStack.Size() >= 1)
                        returnedType = inStack.Peek();
                    else
                        returnedType = Type.VOID;
                    if (returnedType != null)
                    {
                        if (returnedType is ReferenceType)
                            try
                            {
                                if (!((ReferenceType) returnedType).IsCastableTo(m.GetReturnType()))
                                    InvalidReturnTypeError(returnedType, m);
                            }
                            catch (TypeLoadException e)
                            {
                                // Don't know what do do now, so raise RuntimeException
                                throw new Exception(e.Message);
                            }
                        else if (!returnedType.Equals(m.GetReturnType().NormalizeForStackOrLocal()))
                            InvalidReturnTypeError(returnedType, m);
                    }
                }
            } while ((ih = ih.GetNext()) != null);
        }

        /// <summary>
        ///     Throws an exception indicating the returned type is not compatible with the return type of the given method
        /// </summary>
        /// <exception cref="StructuralCodeConstraintException">always</exception>
        /// <since>6.0</since>
        public void InvalidReturnTypeError(Type returnedType, MethodGen
            m)
        {
            throw new StructuralCodeConstraintException("Returned type " +
                                                        returnedType + " does not match Method's return type " +
                                                        m.GetReturnType());
        }

        /// <summary>
        ///     Pass 3b implements the data flow analysis as described in the Java Virtual
        ///     Machine Specification, Second Edition.
        /// </summary>
        /// <remarks>
        ///     Pass 3b implements the data flow analysis as described in the Java Virtual
        ///     Machine Specification, Second Edition.
        ///     Later versions will use LocalVariablesInfo objects to verify if the
        ///     verifier-inferred types and the class file's debug information (LocalVariables
        ///     attributes) match [TODO].
        /// </remarks>
        /// <seealso cref="LocalVariablesInfo" />
        /// <seealso cref="Pass2Verifier.GetLocalVariablesInfo" />
        public override VerificationResult Do_verify()
        {
            if (!myOwner.DoPass3a(method_no).Equals(VerificationResult.VR_OK)) return VerificationResult.VR_NOTYET;
            // Pass 3a ran before, so it's safe to assume the JavaClass object is
            // in the BCEL repository.
            JavaClass jc;
            try
            {
                jc = Repository.LookupClass(myOwner.GetClassName());
            }
            catch (TypeLoadException e)
            {
                // FIXME: maybe not the best way to handle this
                throw new AssertionViolatedException("Missing class: " + e, e);
            }

            var constantPoolGen = new ConstantPoolGen
                (jc.GetConstantPool());
            // Init Visitors
            var icv = new InstConstraintVisitor
                ();
            icv.SetConstantPoolGen(constantPoolGen);
            var ev = new ExecutionVisitor
                ();
            ev.SetConstantPoolGen(constantPoolGen);
            var methods = jc.GetMethods();
            // Method no "method_no" exists, we ran Pass3a before on it!
            try
            {
                var mg = new MethodGen(methods[method_no], myOwner
                    .GetClassName(), constantPoolGen);
                icv.SetMethodGen(mg);
                ////////////// DFA BEGINS HERE ////////////////
                if (!(mg.IsAbstract() || mg.IsNative()))
                {
                    // IF mg HAS CODE (See pass 2)
                    var cfg = new ControlFlowGraph
                        (mg);
                    // Build the initial frame situation for this method.
                    var f = new Frame(mg.GetMaxLocals
                        (), mg.GetMaxStack());
                    if (!mg.IsStatic())
                    {
                        if (mg.GetName().Equals(Const.CONSTRUCTOR_NAME))
                        {
                            Frame.SetThis(new UninitializedObjectType
                                (ObjectType.GetInstance(jc.GetClassName())));
                            f.GetLocals().Set(0, Frame.GetThis());
                        }
                        else
                        {
                            Frame.SetThis(null);
                            f.GetLocals().Set(0, ObjectType.GetInstance(jc.GetClassName()));
                        }
                    }

                    var argtypes = mg.GetArgumentTypes();
                    var twoslotoffset = 0;
                    for (var j = 0; j < argtypes.Length; j++)
                    {
                        if (argtypes[j] == Type.SHORT || argtypes[j] == Type.BYTE || argtypes[j] == Type.CHAR ||
                            argtypes[j] == Type
                                .BOOLEAN)
                            argtypes[j] = Type.INT;
                        f.GetLocals().Set(twoslotoffset + j + (mg.IsStatic() ? 0 : 1), argtypes[j]);
                        if (argtypes[j].GetSize() == 2)
                        {
                            twoslotoffset++;
                            f.GetLocals().Set(twoslotoffset + j + (mg.IsStatic() ? 0 : 1), Type
                                .UNKNOWN);
                        }
                    }

                    CirculationPump(mg, cfg, cfg.ContextOf(mg.GetInstructionList().GetStart()), f, icv
                        , ev);
                }
            }
            catch (VerifierConstraintViolatedException ce)
            {
                ce.ExtendMessage("Constraint violated in method '" + methods[method_no] + "':\n",
                    string.Empty);
                return new VerificationResult(VerificationResult.VERIFIED_REJECTED
                    , ce.Message);
            }
            catch (Exception re)
            {
                // These are internal errors
                var sw = new StringWriter();
                TextWriter pw = sw;
                Runtime.PrintStackTrace(re, pw);
                throw new AssertionViolatedException("Some RuntimeException occured while verify()ing class '"
                                                     + jc.GetClassName() + "', method '" + methods[method_no] +
                                                     "'. Original RuntimeException's stack trace:\n---\n"
                                                     + sw + "---\n", re);
            }

            return VerificationResult.VR_OK;
        }

        /// <summary>Returns the method number as supplied when instantiating.</summary>
        public int GetMethodNo()
        {
            return method_no;
        }

        /// <summary>
        ///     An InstructionContextQueue is a utility class that holds
        ///     (InstructionContext, ArrayList) pairs in a Queue data structure.
        /// </summary>
        /// <remarks>
        ///     An InstructionContextQueue is a utility class that holds
        ///     (InstructionContext, ArrayList) pairs in a Queue data structure.
        ///     This is used to hold information about InstructionContext objects
        ///     externally --- i.e. that information is not saved inside the
        ///     InstructionContext object itself. This is useful to save the
        ///     execution path of the symbolic execution of the
        ///     Pass3bVerifier - this is not information
        ///     that belongs into the InstructionContext object itself.
        ///     Only at "execute()"ing
        ///     time, an InstructionContext object will get the current information
        ///     we have about its symbolic execution predecessors.
        /// </remarks>
        private sealed class InstructionContextQueue
        {
            private readonly List<List<
                InstructionContext>> ecs = new List<List<
                InstructionContext>>();

            private readonly List<InstructionContext
            > ics = new List<InstructionContext>();

            /* TODO:    Throughout pass 3b, upper halves of LONG and DOUBLE
            are represented by Type.UNKNOWN. This should be changed
            in favour of LONG_Upper and DOUBLE_Upper as in pass 2. */
            public void Add(InstructionContext ic, List
                <InstructionContext> executionChain)
            {
                ics.Add(ic);
                ecs.Add(executionChain);
            }

            public bool IsEmpty()
            {
                return ics.Count == 0;
            }

            public void Remove(int i)
            {
                ics.RemoveAtReturningValue(i);
                ecs.RemoveAtReturningValue(i);
            }

            public InstructionContext GetIC(int i)
            {
                return ics[i];
            }

            public List<InstructionContext
            > GetEC(int i)
            {
                return ecs[i];
            }

            public int Size()
            {
                return ics.Count;
            }
        }
    }
}