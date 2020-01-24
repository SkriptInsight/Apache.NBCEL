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
using System.Text;
using Apache.NBCEL.Generic;
using Apache.NBCEL.Verifier.Exc;

namespace Apache.NBCEL.Verifier.Structurals
{
	/// <summary>
	///     Instances of this class contain information about the subroutines
	///     found in a code array of a method.
	/// </summary>
	/// <remarks>
	///     Instances of this class contain information about the subroutines
	///     found in a code array of a method.
	///     This implementation considers the top-level (the instructions
	///     reachable without a JSR or JSR_W starting off from the first
	///     instruction in a code array of a method) being a special subroutine;
	///     see getTopLevel() for that.
	///     Please note that the definition of subroutines in the Java Virtual
	///     Machine Specification, Second Edition is somewhat incomplete.
	///     Therefore, JustIce uses an own, more rigid notion.
	///     Basically, a subroutine is a piece of code that starts at the target
	///     of a JSR of JSR_W instruction and ends at a corresponding RET
	///     instruction. Note also that the control flow of a subroutine
	///     may be complex and non-linear; and that subroutines may be nested.
	///     JustIce also mandates subroutines not to be protected by exception
	///     handling code (for the sake of control flow predictability).
	///     To understand JustIce's notion of subroutines, please read
	///     TODO: refer to the paper.
	/// </remarks>
	/// <seealso cref="GetTopLevel()" />
	public class Subroutines
    {
	    /// <summary>The map containing the subroutines found.</summary>
	    /// <remarks>
	    ///     The map containing the subroutines found.
	    ///     Key: InstructionHandle of the leader of the subroutine.
	    ///     Elements: SubroutineImpl objects.
	    /// </remarks>
	    private readonly IDictionary<InstructionHandle
            , Subroutine> subroutines = new Dictionary
            <InstructionHandle, Subroutine>();

	    /// <summary>
	    ///     This is referring to a special subroutine, namely the
	    ///     top level.
	    /// </summary>
	    /// <remarks>
	    ///     This is referring to a special subroutine, namely the
	    ///     top level. This is not really a subroutine but we use
	    ///     it to distinguish between top level instructions and
	    ///     unreachable instructions.
	    /// </remarks>
	    public readonly Subroutine TOPLEVEL;

	    /// <summary>Constructor.</summary>
	    /// <param name="mg">
	    ///     A MethodGen object representing method to
	    ///     create the Subroutine objects of.
	    ///     Assumes that JustIce strict checks are needed.
	    /// </param>
	    public Subroutines(MethodGen mg)
            : this(mg, true)
        {
        }

	    /// <summary>Constructor.</summary>
	    /// <param name="mg">
	    ///     A MethodGen object representing method to
	    ///     create the Subroutine objects of.
	    /// </param>
	    /// <param name="enableJustIceCheck">whether to enable additional JustIce checks</param>
	    /// <since>6.0</since>
	    public Subroutines(MethodGen mg, bool enableJustIceCheck)
        {
            // CHECKSTYLE:OFF
            // TODO can this be made private?
            // CHECKSTYLE:ON
            var all = mg.GetInstructionList().GetInstructionHandles
                ();
            var handlers = mg.GetExceptionHandlers();
            // Define our "Toplevel" fake subroutine.
            TOPLEVEL = new SubroutineImpl(this);
            // Calculate "real" subroutines.
            var sub_leaders =
                new HashSet<InstructionHandle>();
            // Elements: InstructionHandle
            foreach (var element in all)
            {
                var inst = element.GetInstruction();
                if (inst is JsrInstruction) sub_leaders.Add(((JsrInstruction) inst).GetTarget());
            }

            // Build up the database.
            foreach (var astore in sub_leaders)
            {
                var sr = new SubroutineImpl
                    (this);
                sr.SetLocalVariable(((ASTORE) astore.GetInstruction()).GetIndex());
                Collections.Put(subroutines, astore, sr);
            }

            // Fake it a bit. We want a virtual "TopLevel" subroutine.
            Collections.Put(subroutines, all[0], TOPLEVEL);
            sub_leaders.Add(all[0]);
            // Tell the subroutines about their JsrInstructions.
            // Note that there cannot be a JSR targeting the top-level
            // since "Jsr 0" is disallowed in Pass 3a.
            // Instructions shared by a subroutine and the toplevel are
            // disallowed and checked below, after the BFS.
            foreach (var element in all)
            {
                var inst = element.GetInstruction();
                if (inst is JsrInstruction)
                {
                    var leader = ((JsrInstruction) inst).GetTarget
                        ();
                    ((SubroutineImpl) GetSubroutine(leader)).AddEnteringJsrInstruction
                        (element);
                }
            }

            // Now do a BFS from every subroutine leader to find all the
            // instructions that belong to a subroutine.
            // we don't want to assign an instruction to two or more Subroutine objects.
            var instructions_assigned
                = new HashSet<InstructionHandle>();
            //Graph colouring. Key: InstructionHandle, Value: ColourConstants enum .
            IDictionary<InstructionHandle, ColourConstants
            > colors = new Dictionary<InstructionHandle
                , ColourConstants>();
            var Q = new List
                <InstructionHandle>();
            foreach (var actual in sub_leaders)
            {
                // Do some BFS with "actual" as the root of the graph.
                // Init colors
                foreach (var element in all)
                    Collections.Put(colors, element, ColourConstants
                        .WHITE);
                Collections.Put(colors, actual, ColourConstants
                    .GRAY);
                // Init Queue
                Q.Clear();
                Q.Add(actual);
                // add(Obj) adds to the end, remove(0) removes from the start.
                /*
                * BFS ALGORITHM MODIFICATION:
                * Start out with multiple "root" nodes, as exception handlers are starting points of top-level code, too.
                * [why top-level?
                * TODO: Refer to the special JustIce notion of subroutines.]
                */
                if (actual == all[0])
                    foreach (var handler in handlers)
                    {
                        Collections.Put(colors, handler.GetHandlerPC(), ColourConstants
                            .GRAY);
                        Q.Add(handler.GetHandlerPC());
                    }

                /* CONTINUE NORMAL BFS ALGORITHM */
                // Loop until Queue is empty
                while (Q.Count != 0)
                {
                    var u = Q.RemoveAtReturningValue(0);
                    var successors = GetSuccessors(u);
                    foreach (var successor in successors)
                        if (colors.GetOrNull(successor) == ColourConstants
                                .WHITE)
                        {
                            Collections.Put(colors, successor, ColourConstants
                                .GRAY);
                            Q.Add(successor);
                        }

                    Collections.Put(colors, u, ColourConstants
                        .BLACK);
                }

                // BFS ended above.
                foreach (var element in all)
                    if (colors.GetOrNull(element) == ColourConstants
                            .BLACK)
                    {
                        ((SubroutineImpl) (actual == all[0]
                            ? GetTopLevel
                                ()
                            : GetSubroutine(actual))).AddInstruction(element);
                        if (instructions_assigned.Contains(element))
                            throw new StructuralCodeConstraintException("Instruction '" +
                                                                        element +
                                                                        "' is part of more than one subroutine (or of the top level and a subroutine)."
                            );
                        instructions_assigned.Add(element);
                    }

                if (actual != all[0])
                    // If we don't deal with the top-level 'subroutine'
                    ((SubroutineImpl) GetSubroutine(actual)).SetLeavingRET
                        ();
            }

            if (enableJustIceCheck)
                // Now make sure no instruction of a Subroutine is protected by exception handling code
                // as is mandated by JustIces notion of subroutines.
                foreach (var handler in handlers)
                {
                    var _protected = handler.GetStartPC();
                    while (_protected != handler.GetEndPC().GetNext())
                    {
                        // Note the inclusive/inclusive notation of "generic API" exception handlers!
                        foreach (var sub in subroutines.Values)
                            if (sub != subroutines.GetOrNull(all[0]))
                                // We don't want to forbid top-level exception handlers.
                                if (sub.Contains(_protected))
                                    throw new StructuralCodeConstraintException("Subroutine instruction '"
                                                                                + _protected +
                                                                                "' is protected by an exception handler, '" +
                                                                                handler +
                                                                                "'. This is forbidden by the JustIce verifier due to its clear definition of subroutines."
                                    );
                        _protected = _protected.GetNext();
                    }
                }

            // Now make sure no subroutine is calling a subroutine
            // that uses the same local variable for the RET as themselves
            // (recursively).
            // This includes that subroutines may not call themselves
            // recursively, even not through intermediate calls to other
            // subroutines.
            NoRecursiveCalls(GetTopLevel(), new HashSet<int>());
        }

	    /// <summary>
	    ///     This (recursive) utility method makes sure that
	    ///     no subroutine is calling a subroutine
	    ///     that uses the same local variable for the RET as themselves
	    ///     (recursively).
	    /// </summary>
	    /// <remarks>
	    ///     This (recursive) utility method makes sure that
	    ///     no subroutine is calling a subroutine
	    ///     that uses the same local variable for the RET as themselves
	    ///     (recursively).
	    ///     This includes that subroutines may not call themselves
	    ///     recursively, even not through intermediate calls to other
	    ///     subroutines.
	    /// </remarks>
	    /// <exception cref="StructuralCodeConstraintException">
	    ///     if the above constraint is not satisfied.
	    /// </exception>
	    private void NoRecursiveCalls(Subroutine sub, HashSet
            <int> set)
        {
            var subs = sub.SubSubs();
            foreach (var sub2 in subs)
            {
                var index = ((RET) sub2.GetLeavingRET().GetInstruction()).GetIndex
                    ();
                if (!set.Add(index))
                {
                    // Don't use toString() here because of possibly infinite recursive subSubs() calls then.
                    var si = (SubroutineImpl
                        ) sub2;
                    throw new StructuralCodeConstraintException("Subroutine with local variable '"
                                                                + si.localVariable + "', JSRs '" + si.theJSRs +
                                                                "', RET '" + si.theRET +
                                                                "' is called by a subroutine which uses the same local variable index as itself; maybe even a recursive call?"
                                                                + " JustIce's clean definition of a subroutine forbids both.");
                }

                NoRecursiveCalls(sub2, set);
                set.Remove(index);
            }
        }

	    /// <summary>
	    ///     Returns the Subroutine object associated with the given
	    ///     leader (that is, the first instruction of the subroutine).
	    /// </summary>
	    /// <remarks>
	    ///     Returns the Subroutine object associated with the given
	    ///     leader (that is, the first instruction of the subroutine).
	    ///     You must not use this to get the top-level instructions
	    ///     modeled as a Subroutine object.
	    /// </remarks>
	    /// <seealso cref="GetTopLevel()" />
	    public virtual Subroutine GetSubroutine(InstructionHandle
            leader)
        {
            var ret = subroutines.GetOrNull(leader);
            if (ret == null)
                throw new AssertionViolatedException(
                    "Subroutine requested for an InstructionHandle that is not a leader of a subroutine."
                );
            if (ret == TOPLEVEL)
                throw new AssertionViolatedException("TOPLEVEL special subroutine requested; use getTopLevel()."
                );
            return ret;
        }

	    /// <summary>
	    ///     Returns the subroutine object associated with the
	    ///     given instruction.
	    /// </summary>
	    /// <remarks>
	    ///     Returns the subroutine object associated with the
	    ///     given instruction. This is a costly operation, you
	    ///     should consider using getSubroutine(InstructionHandle).
	    ///     Returns 'null' if the given InstructionHandle lies
	    ///     in so-called 'dead code', i.e. code that can never
	    ///     be executed.
	    /// </remarks>
	    /// <seealso cref="GetSubroutine" />
	    /// <seealso cref="GetTopLevel()" />
	    public virtual Subroutine SubroutineOf(InstructionHandle
            any)
        {
            foreach (var s in subroutines.Values)
                if (s.Contains(any))
                    return s;
            Console.Error.WriteLine("DEBUG: Please verify '" + any.ToString(true) + "' lies in dead code."
            );
            return null;
        }

        //throw new AssertionViolatedException("No subroutine for InstructionHandle found (DEAD CODE?).");
        /// <summary>
        ///     For easy handling, the piece of code that is <B>not</B> a
        ///     subroutine, the top-level, is also modeled as a Subroutine
        ///     object.
        /// </summary>
        /// <remarks>
        ///     For easy handling, the piece of code that is <B>not</B> a
        ///     subroutine, the top-level, is also modeled as a Subroutine
        ///     object.
        ///     It is a special Subroutine object where
        ///     <B>
        ///         you must not invoke
        ///         getEnteringJsrInstructions() or getLeavingRET()
        ///     </B>
        ///     .
        /// </remarks>
        /// <seealso cref="Subroutine.GetEnteringJsrInstructions()" />
        /// <seealso cref="Subroutine.GetLeavingRET()" />
        public virtual Subroutine GetTopLevel()
        {
            return TOPLEVEL;
        }

        /// <summary>
        ///     A utility method that calculates the successors of a given InstructionHandle
        ///     <B>in the same subroutine</B>.
        /// </summary>
        /// <remarks>
        ///     A utility method that calculates the successors of a given InstructionHandle
        ///     <B>in the same subroutine</B>. That means, a RET does not have any successors
        ///     as defined here. A JsrInstruction has its physical successor as its successor
        ///     (opposed to its target) as defined here.
        /// </remarks>
        private static InstructionHandle[] GetSuccessors(InstructionHandle
            instruction)
        {
            var empty = new InstructionHandle[0];
            var single = new InstructionHandle[1]
                ;
            var inst = instruction.GetInstruction();
            if (inst is RET) return empty;
            // Terminates method normally.
            if (inst is ReturnInstruction) return empty;
            // Terminates method abnormally, because JustIce mandates
            // subroutines not to be protected by exception handlers.
            if (inst is ATHROW) return empty;
            // See method comment.
            if (inst is JsrInstruction)
            {
                single[0] = instruction.GetNext();
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
                pair[0] = instruction.GetNext();
                pair[1] = ((BranchInstruction) inst).GetTarget();
                return pair;
            }

            // default case: Fall through.
            single[0] = instruction.GetNext();
            return single;
        }

        /// <summary>
        ///     Returns a String representation of this object; merely for debugging puposes.
        /// </summary>
        public override string ToString()
        {
            return "---\n" + subroutines + "\n---\n";
        }

        /// <summary>This inner class implements the Subroutine interface.</summary>
        private class SubroutineImpl : Subroutine
        {
	        /// <summary>
	        ///     UNSET, a symbol for an uninitialized localVariable
	        ///     field.
	        /// </summary>
	        /// <remarks>
	        ///     UNSET, a symbol for an uninitialized localVariable
	        ///     field. This is used for the "top-level" Subroutine;
	        ///     i.e. no subroutine.
	        /// </remarks>
	        private const int UNSET = -1;

            private readonly Subroutines _enclosing;

            /// <summary>The instructions that belong to this subroutine.</summary>
            private readonly HashSet<InstructionHandle
            > instructions = new HashSet<InstructionHandle
            >();

            /// <summary>
            ///     The JSR or JSR_W instructions that define this
            ///     subroutine by targeting it.
            /// </summary>
            internal readonly HashSet<InstructionHandle
            > theJSRs = new HashSet<InstructionHandle
            >();

            /// <summary>
            ///     The Local Variable slot where the first
            ///     instruction of this subroutine (an ASTORE) stores
            ///     the JsrInstruction's ReturnAddress in and
            ///     the RET of this subroutine operates on.
            /// </summary>
            internal int localVariable = UNSET;

            /// <summary>The RET instruction that leaves this subroutine.</summary>
            internal InstructionHandle theRET;

            /// <summary>The default constructor.</summary>
            public SubroutineImpl(Subroutines _enclosing)
            {
                this._enclosing = _enclosing;
            }

            // Elements: InstructionHandle
            /*
            * Refer to the Subroutine interface for documentation.
            */
            public virtual bool Contains(InstructionHandle inst)
            {
                return instructions.Contains(inst);
            }

            /*
            * Refer to the Subroutine interface for documentation.
            */
            public virtual InstructionHandle[] GetEnteringJsrInstructions()
            {
                if (this == _enclosing.GetTopLevel())
                    throw new AssertionViolatedException("getLeavingRET() called on top level pseudo-subroutine."
                    );
                var jsrs = new InstructionHandle[theJSRs.Count];
                return Collections.ToArray(theJSRs, jsrs);
            }

            /*
            * Refer to the Subroutine interface for documentation.
            */
            public virtual InstructionHandle GetLeavingRET()
            {
                if (this == _enclosing.GetTopLevel())
                    throw new AssertionViolatedException("getLeavingRET() called on top level pseudo-subroutine."
                    );
                return theRET;
            }

            /*
            * Refer to the Subroutine interface for documentation.
            */
            public virtual InstructionHandle[] GetInstructions()
            {
                var ret = new InstructionHandle[instructions.Count];
                return Collections.ToArray(instructions, ret);
            }

            /* Satisfies Subroutine.getRecursivelyAccessedLocalsIndices(). */
            public virtual int[] GetRecursivelyAccessedLocalsIndices()
            {
                var s = new HashSet
                    <int>();
                var lvs = GetAccessedLocalsIndices();
                foreach (var lv in lvs) s.Add(lv);
                _getRecursivelyAccessedLocalsIndicesHelper(s, SubSubs());
                var ret = new int[s.Count];
                var j = -1;
                foreach (var index in s)
                {
                    j++;
                    ret[j] = index;
                }

                return ret;
            }

            /*
            * Satisfies Subroutine.getAccessedLocalIndices().
            */
            public virtual int[] GetAccessedLocalsIndices()
            {
                //TODO: Implement caching.
                var acc = new HashSet
                    <int>();
                if (theRET == null && this != _enclosing.GetTopLevel())
                    throw new AssertionViolatedException(
                        "This subroutine object must be built up completely before calculating accessed locals."
                    );
                {
                    foreach (var ih in instructions)
                        // RET is not a LocalVariableInstruction in the current version of BCEL.
                        if (ih.GetInstruction() is LocalVariableInstruction || ih.GetInstruction
                                () is RET)
                        {
                            var idx = ((IndexedInstruction) ih.GetInstruction()).GetIndex();
                            acc.Add(idx);
                            // LONG? DOUBLE?.
                            try
                            {
                                // LocalVariableInstruction instances are typed without the need to look into
                                // the constant pool.
                                if (ih.GetInstruction() is LocalVariableInstruction)
                                {
                                    var s = ((LocalVariableInstruction) ih.GetInstruction()).GetType(null
                                    ).GetSize();
                                    if (s == 2) acc.Add(idx + 1);
                                }
                            }
                            catch (Exception re)
                            {
                                throw new AssertionViolatedException(
                                    "Oops. BCEL did not like NULL as a ConstantPoolGen object."
                                    , re);
                            }
                        }
                }
                {
                    var ret = new int[acc.Count];
                    var j = -1;
                    foreach (var accessedLocal in acc)
                    {
                        j++;
                        ret[j] = accessedLocal;
                    }

                    return ret;
                }
            }

            /*
            * Satisfies Subroutine.subSubs().
            */
            public virtual Subroutine[] SubSubs()
            {
                var h = new
                    HashSet<Subroutine>();
                foreach (var ih in instructions)
                {
                    var inst = ih.GetInstruction();
                    if (inst is JsrInstruction)
                    {
                        var targ = ((JsrInstruction) inst).GetTarget
                            ();
                        h.Add(_enclosing.GetSubroutine(targ));
                    }
                }

                var ret = new Subroutine
                    [h.Count];
                return Collections.ToArray(h, ret);
            }

            /// <summary>
            ///     Returns a String representation of this object, merely
            ///     for debugging purposes.
            /// </summary>
            /// <remarks>
            ///     Returns a String representation of this object, merely
            ///     for debugging purposes.
            ///     (Internal) Warning: Verbosity on a problematic subroutine may cause
            ///     stack overflow errors due to recursive subSubs() calls.
            ///     Don't use this, then.
            /// </remarks>
            public override string ToString()
            {
                var ret = new StringBuilder();
                ret.Append("Subroutine: Local variable is '").Append(localVariable);
                ret.Append("', JSRs are '").Append(theJSRs);
                ret.Append("', RET is '").Append(theRET);
                ret.Append("', Instructions: '").Append(instructions).Append("'.");
                ret.Append(" Accessed local variable slots: '");
                var alv = GetAccessedLocalsIndices();
                foreach (var element in alv)
                {
                    ret.Append(element);
                    ret.Append(" ");
                }

                ret.Append("'.");
                ret.Append(" Recursively (via subsub...routines) accessed local variable slots: '"
                );
                alv = GetRecursivelyAccessedLocalsIndices();
                foreach (var element in alv)
                {
                    ret.Append(element);
                    ret.Append(" ");
                }

                ret.Append("'.");
                return ret.ToString();
            }

            /// <summary>Sets the leaving RET instruction.</summary>
            /// <remarks>
            ///     Sets the leaving RET instruction. Must be invoked after all instructions are added.
            ///     Must not be invoked for top-level 'subroutine'.
            /// </remarks>
            internal virtual void SetLeavingRET()
            {
                if (localVariable == UNSET)
                    throw new AssertionViolatedException(
                        "setLeavingRET() called for top-level 'subroutine' or forgot to set local variable first."
                    );
                InstructionHandle ret = null;
                foreach (var actual in instructions)
                    if (actual.GetInstruction() is RET)
                    {
                        if (ret != null)
                            throw new StructuralCodeConstraintException("Subroutine with more then one RET detected: '"
                                                                        + ret + "' and '" + actual + "'.");
                        ret = actual;
                    }

                if (ret == null)
                    throw new StructuralCodeConstraintException("Subroutine without a RET detected."
                    );
                if (((RET) ret.GetInstruction()).GetIndex() != localVariable)
                    throw new StructuralCodeConstraintException("Subroutine uses '"
                                                                + ret +
                                                                "' which does not match the correct local variable '" +
                                                                localVariable
                                                                + "'.");
                theRET = ret;
            }

            /// <summary>Adds a new JSR or JSR_W that has this subroutine as its target.</summary>
            public virtual void AddEnteringJsrInstruction(InstructionHandle jsrInst
            )
            {
                if (jsrInst == null || !(jsrInst.GetInstruction() is JsrInstruction
                        ))
                    throw new AssertionViolatedException("Expecting JsrInstruction InstructionHandle."
                    );
                if (localVariable == UNSET)
                    throw new AssertionViolatedException("Set the localVariable first!"
                    );
                // Something is wrong when an ASTORE is targeted that does not operate on the same local variable than the rest of the
                // JsrInstruction-targets and the RET.
                // (We don't know out leader here so we cannot check if we're really targeted!)
                if (localVariable != ((ASTORE) ((JsrInstruction)
                        jsrInst.GetInstruction()).GetTarget().GetInstruction()).GetIndex())
                    throw new AssertionViolatedException("Setting a wrong JsrInstruction."
                    );
                theJSRs.Add(jsrInst);
            }

            /*
            * Adds an instruction to this subroutine.
            * All instructions must have been added before invoking setLeavingRET().
            * @see #setLeavingRET
            */
            internal virtual void AddInstruction(InstructionHandle ih)
            {
                if (theRET != null)
                    throw new AssertionViolatedException(
                        "All instructions must have been added before invoking setLeavingRET()."
                    );
                instructions.Add(ih);
            }

            /// <summary>A recursive helper method for getRecursivelyAccessedLocalsIndices().</summary>
            /// <seealso cref="GetRecursivelyAccessedLocalsIndices()" />
            private void _getRecursivelyAccessedLocalsIndicesHelper(HashSet
                <int> s, Subroutine[] subs)
            {
                foreach (var sub in subs)
                {
                    var lvs = sub.GetAccessedLocalsIndices();
                    foreach (var lv in lvs) s.Add(lv);
                    if (sub.SubSubs().Length != 0) _getRecursivelyAccessedLocalsIndicesHelper(s, sub.SubSubs());
                }
            }

            /*
            * Sets the local variable slot the ASTORE that is targeted
            * by the JsrInstructions of this subroutine operates on.
            * This subroutine's RET operates on that same local variable
            * slot, of course.
            */
            internal virtual void SetLocalVariable(int i)
            {
                if (localVariable != UNSET)
                    throw new AssertionViolatedException("localVariable set twice."
                    );
                localVariable = i;
            }

            // end Inner Class SubrouteImpl
        }

        [Serializable]
        private sealed class ColourConstants : EnumBase
        {
            public static readonly ColourConstants WHITE
                = new ColourConstants(0, "WHITE");

            public static readonly ColourConstants GRAY
                = new ColourConstants(1, "GRAY");

            public static readonly ColourConstants BLACK
                = new ColourConstants(2, "BLACK");

            static ColourConstants()
            {
                RegisterValues<ColourConstants>(Values());
            }

            private ColourConstants(int ordinal, string name)
                : base(ordinal, name)
            {
            }

            public static ColourConstants[] Values()
            {
                return new[] {WHITE, GRAY, BLACK};
            }

            //Node coloring constants
        }
    }
}