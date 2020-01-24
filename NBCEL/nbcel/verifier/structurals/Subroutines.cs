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
using Sharpen;

namespace NBCEL.verifier.structurals
{
	/// <summary>
	/// Instances of this class contain information about the subroutines
	/// found in a code array of a method.
	/// </summary>
	/// <remarks>
	/// Instances of this class contain information about the subroutines
	/// found in a code array of a method.
	/// This implementation considers the top-level (the instructions
	/// reachable without a JSR or JSR_W starting off from the first
	/// instruction in a code array of a method) being a special subroutine;
	/// see getTopLevel() for that.
	/// Please note that the definition of subroutines in the Java Virtual
	/// Machine Specification, Second Edition is somewhat incomplete.
	/// Therefore, JustIce uses an own, more rigid notion.
	/// Basically, a subroutine is a piece of code that starts at the target
	/// of a JSR of JSR_W instruction and ends at a corresponding RET
	/// instruction. Note also that the control flow of a subroutine
	/// may be complex and non-linear; and that subroutines may be nested.
	/// JustIce also mandates subroutines not to be protected by exception
	/// handling code (for the sake of control flow predictability).
	/// To understand JustIce's notion of subroutines, please read
	/// TODO: refer to the paper.
	/// </remarks>
	/// <seealso cref="GetTopLevel()"/>
	public class Subroutines
	{
		/// <summary>This inner class implements the Subroutine interface.</summary>
		private class SubroutineImpl : NBCEL.verifier.structurals.Subroutine
		{
			/// <summary>
			/// UNSET, a symbol for an uninitialized localVariable
			/// field.
			/// </summary>
			/// <remarks>
			/// UNSET, a symbol for an uninitialized localVariable
			/// field. This is used for the "top-level" Subroutine;
			/// i.e. no subroutine.
			/// </remarks>
			private const int UNSET = -1;

			/// <summary>
			/// The Local Variable slot where the first
			/// instruction of this subroutine (an ASTORE) stores
			/// the JsrInstruction's ReturnAddress in and
			/// the RET of this subroutine operates on.
			/// </summary>
			internal int localVariable = NBCEL.verifier.structurals.Subroutines.SubroutineImpl
				.UNSET;

			/// <summary>The instructions that belong to this subroutine.</summary>
			private readonly System.Collections.Generic.HashSet<NBCEL.generic.InstructionHandle
				> instructions = new System.Collections.Generic.HashSet<NBCEL.generic.InstructionHandle
				>();

			// Elements: InstructionHandle
			/*
			* Refer to the Subroutine interface for documentation.
			*/
			public virtual bool Contains(NBCEL.generic.InstructionHandle inst)
			{
				return this.instructions.Contains(inst);
			}

			/// <summary>
			/// The JSR or JSR_W instructions that define this
			/// subroutine by targeting it.
			/// </summary>
			internal readonly System.Collections.Generic.HashSet<NBCEL.generic.InstructionHandle
				> theJSRs = new System.Collections.Generic.HashSet<NBCEL.generic.InstructionHandle
				>();

			/// <summary>The RET instruction that leaves this subroutine.</summary>
			internal NBCEL.generic.InstructionHandle theRET;

			/// <summary>
			/// Returns a String representation of this object, merely
			/// for debugging purposes.
			/// </summary>
			/// <remarks>
			/// Returns a String representation of this object, merely
			/// for debugging purposes.
			/// (Internal) Warning: Verbosity on a problematic subroutine may cause
			/// stack overflow errors due to recursive subSubs() calls.
			/// Don't use this, then.
			/// </remarks>
			public override string ToString()
			{
				System.Text.StringBuilder ret = new System.Text.StringBuilder();
				ret.Append("Subroutine: Local variable is '").Append(this.localVariable);
				ret.Append("', JSRs are '").Append(this.theJSRs);
				ret.Append("', RET is '").Append(this.theRET);
				ret.Append("', Instructions: '").Append(this.instructions).Append("'.");
				ret.Append(" Accessed local variable slots: '");
				int[] alv = this.GetAccessedLocalsIndices();
				foreach (int element in alv)
				{
					ret.Append(element);
					ret.Append(" ");
				}
				ret.Append("'.");
				ret.Append(" Recursively (via subsub...routines) accessed local variable slots: '"
					);
				alv = this.GetRecursivelyAccessedLocalsIndices();
				foreach (int element in alv)
				{
					ret.Append(element);
					ret.Append(" ");
				}
				ret.Append("'.");
				return ret.ToString();
			}

			/// <summary>Sets the leaving RET instruction.</summary>
			/// <remarks>
			/// Sets the leaving RET instruction. Must be invoked after all instructions are added.
			/// Must not be invoked for top-level 'subroutine'.
			/// </remarks>
			internal virtual void SetLeavingRET()
			{
				if (this.localVariable == NBCEL.verifier.structurals.Subroutines.SubroutineImpl.UNSET)
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("setLeavingRET() called for top-level 'subroutine' or forgot to set local variable first."
						);
				}
				NBCEL.generic.InstructionHandle ret = null;
				foreach (NBCEL.generic.InstructionHandle actual in this.instructions)
				{
					if (actual.GetInstruction() is NBCEL.generic.RET)
					{
						if (ret != null)
						{
							throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Subroutine with more then one RET detected: '"
								 + ret + "' and '" + actual + "'.");
						}
						ret = actual;
					}
				}
				if (ret == null)
				{
					throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Subroutine without a RET detected."
						);
				}
				if (((NBCEL.generic.RET)ret.GetInstruction()).GetIndex() != this.localVariable)
				{
					throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Subroutine uses '"
						 + ret + "' which does not match the correct local variable '" + this.localVariable
						 + "'.");
				}
				this.theRET = ret;
			}

			/*
			* Refer to the Subroutine interface for documentation.
			*/
			public virtual NBCEL.generic.InstructionHandle[] GetEnteringJsrInstructions()
			{
				if (this == this._enclosing.GetTopLevel())
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("getLeavingRET() called on top level pseudo-subroutine."
						);
				}
				NBCEL.generic.InstructionHandle[] jsrs = new NBCEL.generic.InstructionHandle[this
					.theJSRs.Count];
				return Sharpen.Collections.ToArray(this.theJSRs, jsrs);
			}

			/// <summary>Adds a new JSR or JSR_W that has this subroutine as its target.</summary>
			public virtual void AddEnteringJsrInstruction(NBCEL.generic.InstructionHandle jsrInst
				)
			{
				if ((jsrInst == null) || (!(jsrInst.GetInstruction() is NBCEL.generic.JsrInstruction
					)))
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("Expecting JsrInstruction InstructionHandle."
						);
				}
				if (this.localVariable == NBCEL.verifier.structurals.Subroutines.SubroutineImpl.UNSET)
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("Set the localVariable first!"
						);
				}
				// Something is wrong when an ASTORE is targeted that does not operate on the same local variable than the rest of the
				// JsrInstruction-targets and the RET.
				// (We don't know out leader here so we cannot check if we're really targeted!)
				if (this.localVariable != ((NBCEL.generic.ASTORE)(((NBCEL.generic.JsrInstruction)
					jsrInst.GetInstruction()).GetTarget().GetInstruction())).GetIndex())
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("Setting a wrong JsrInstruction."
						);
				}
				this.theJSRs.Add(jsrInst);
			}

			/*
			* Refer to the Subroutine interface for documentation.
			*/
			public virtual NBCEL.generic.InstructionHandle GetLeavingRET()
			{
				if (this == this._enclosing.GetTopLevel())
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("getLeavingRET() called on top level pseudo-subroutine."
						);
				}
				return this.theRET;
			}

			/*
			* Refer to the Subroutine interface for documentation.
			*/
			public virtual NBCEL.generic.InstructionHandle[] GetInstructions()
			{
				NBCEL.generic.InstructionHandle[] ret = new NBCEL.generic.InstructionHandle[this.
					instructions.Count];
				return Sharpen.Collections.ToArray(this.instructions, ret);
			}

			/*
			* Adds an instruction to this subroutine.
			* All instructions must have been added before invoking setLeavingRET().
			* @see #setLeavingRET
			*/
			internal virtual void AddInstruction(NBCEL.generic.InstructionHandle ih)
			{
				if (this.theRET != null)
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("All instructions must have been added before invoking setLeavingRET()."
						);
				}
				this.instructions.Add(ih);
			}

			/* Satisfies Subroutine.getRecursivelyAccessedLocalsIndices(). */
			public virtual int[] GetRecursivelyAccessedLocalsIndices()
			{
				System.Collections.Generic.HashSet<int> s = new System.Collections.Generic.HashSet
					<int>();
				int[] lvs = this.GetAccessedLocalsIndices();
				foreach (int lv in lvs)
				{
					s.Add(lv);
				}
				this._getRecursivelyAccessedLocalsIndicesHelper(s, this.SubSubs());
				int[] ret = new int[s.Count];
				int j = -1;
				foreach (int index in s)
				{
					j++;
					ret[j] = index;
				}
				return ret;
			}

			/// <summary>A recursive helper method for getRecursivelyAccessedLocalsIndices().</summary>
			/// <seealso cref="GetRecursivelyAccessedLocalsIndices()"/>
			private void _getRecursivelyAccessedLocalsIndicesHelper(System.Collections.Generic.HashSet
				<int> s, NBCEL.verifier.structurals.Subroutine[] subs)
			{
				foreach (NBCEL.verifier.structurals.Subroutine sub in subs)
				{
					int[] lvs = sub.GetAccessedLocalsIndices();
					foreach (int lv in lvs)
					{
						s.Add(lv);
					}
					if (sub.SubSubs().Length != 0)
					{
						this._getRecursivelyAccessedLocalsIndicesHelper(s, sub.SubSubs());
					}
				}
			}

			/*
			* Satisfies Subroutine.getAccessedLocalIndices().
			*/
			public virtual int[] GetAccessedLocalsIndices()
			{
				//TODO: Implement caching.
				System.Collections.Generic.HashSet<int> acc = new System.Collections.Generic.HashSet
					<int>();
				if (this.theRET == null && this != this._enclosing.GetTopLevel())
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("This subroutine object must be built up completely before calculating accessed locals."
						);
				}
				{
					foreach (NBCEL.generic.InstructionHandle ih in this.instructions)
					{
						// RET is not a LocalVariableInstruction in the current version of BCEL.
						if (ih.GetInstruction() is NBCEL.generic.LocalVariableInstruction || ih.GetInstruction
							() is NBCEL.generic.RET)
						{
							int idx = ((NBCEL.generic.IndexedInstruction)(ih.GetInstruction())).GetIndex();
							acc.Add(idx);
							// LONG? DOUBLE?.
							try
							{
								// LocalVariableInstruction instances are typed without the need to look into
								// the constant pool.
								if (ih.GetInstruction() is NBCEL.generic.LocalVariableInstruction)
								{
									int s = ((NBCEL.generic.LocalVariableInstruction)ih.GetInstruction()).GetType(null
										).GetSize();
									if (s == 2)
									{
										acc.Add(idx + 1);
									}
								}
							}
							catch (System.Exception re)
							{
								throw new NBCEL.verifier.exc.AssertionViolatedException("Oops. BCEL did not like NULL as a ConstantPoolGen object."
									, re);
							}
						}
					}
				}
				{
					int[] ret = new int[acc.Count];
					int j = -1;
					foreach (int accessedLocal in acc)
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
			public virtual NBCEL.verifier.structurals.Subroutine[] SubSubs()
			{
				System.Collections.Generic.HashSet<NBCEL.verifier.structurals.Subroutine> h = new 
					System.Collections.Generic.HashSet<NBCEL.verifier.structurals.Subroutine>();
				foreach (NBCEL.generic.InstructionHandle ih in this.instructions)
				{
					NBCEL.generic.Instruction inst = ih.GetInstruction();
					if (inst is NBCEL.generic.JsrInstruction)
					{
						NBCEL.generic.InstructionHandle targ = ((NBCEL.generic.JsrInstruction)inst).GetTarget
							();
						h.Add(this._enclosing.GetSubroutine(targ));
					}
				}
				NBCEL.verifier.structurals.Subroutine[] ret = new NBCEL.verifier.structurals.Subroutine
					[h.Count];
				return Sharpen.Collections.ToArray(h, ret);
			}

			/*
			* Sets the local variable slot the ASTORE that is targeted
			* by the JsrInstructions of this subroutine operates on.
			* This subroutine's RET operates on that same local variable
			* slot, of course.
			*/
			internal virtual void SetLocalVariable(int i)
			{
				if (this.localVariable != NBCEL.verifier.structurals.Subroutines.SubroutineImpl.UNSET)
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("localVariable set twice."
						);
				}
				this.localVariable = i;
			}

			/// <summary>The default constructor.</summary>
			public SubroutineImpl(Subroutines _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly Subroutines _enclosing;
			// end Inner Class SubrouteImpl
		}

		[System.Serializable]
		private sealed class ColourConstants : Sharpen.EnumBase
		{
			public static readonly NBCEL.verifier.structurals.Subroutines.ColourConstants WHITE
				 = new NBCEL.verifier.structurals.Subroutines.ColourConstants(0, "WHITE");

			public static readonly NBCEL.verifier.structurals.Subroutines.ColourConstants GRAY
				 = new NBCEL.verifier.structurals.Subroutines.ColourConstants(1, "GRAY");

			public static readonly NBCEL.verifier.structurals.Subroutines.ColourConstants BLACK
				 = new NBCEL.verifier.structurals.Subroutines.ColourConstants(2, "BLACK");

			private ColourConstants(int ordinal, string name)
				: base(ordinal, name)
			{
			}

			public static ColourConstants[] Values()
			{
				return new ColourConstants[] { WHITE, GRAY, BLACK };
			}

			static ColourConstants()
			{
				RegisterValues<ColourConstants>(Values());
			}
			//Node coloring constants
		}

		/// <summary>The map containing the subroutines found.</summary>
		/// <remarks>
		/// The map containing the subroutines found.
		/// Key: InstructionHandle of the leader of the subroutine.
		/// Elements: SubroutineImpl objects.
		/// </remarks>
		private readonly System.Collections.Generic.IDictionary<NBCEL.generic.InstructionHandle
			, NBCEL.verifier.structurals.Subroutine> subroutines = new System.Collections.Generic.Dictionary
			<NBCEL.generic.InstructionHandle, NBCEL.verifier.structurals.Subroutine>();

		/// <summary>
		/// This is referring to a special subroutine, namely the
		/// top level.
		/// </summary>
		/// <remarks>
		/// This is referring to a special subroutine, namely the
		/// top level. This is not really a subroutine but we use
		/// it to distinguish between top level instructions and
		/// unreachable instructions.
		/// </remarks>
		public readonly NBCEL.verifier.structurals.Subroutine TOPLEVEL;

		/// <summary>Constructor.</summary>
		/// <param name="mg">
		/// A MethodGen object representing method to
		/// create the Subroutine objects of.
		/// Assumes that JustIce strict checks are needed.
		/// </param>
		public Subroutines(NBCEL.generic.MethodGen mg)
			: this(mg, true)
		{
		}

		/// <summary>Constructor.</summary>
		/// <param name="mg">
		/// A MethodGen object representing method to
		/// create the Subroutine objects of.
		/// </param>
		/// <param name="enableJustIceCheck">whether to enable additional JustIce checks</param>
		/// <since>6.0</since>
		public Subroutines(NBCEL.generic.MethodGen mg, bool enableJustIceCheck)
		{
			// CHECKSTYLE:OFF
			// TODO can this be made private?
			// CHECKSTYLE:ON
			NBCEL.generic.InstructionHandle[] all = mg.GetInstructionList().GetInstructionHandles
				();
			NBCEL.generic.CodeExceptionGen[] handlers = mg.GetExceptionHandlers();
			// Define our "Toplevel" fake subroutine.
			TOPLEVEL = new NBCEL.verifier.structurals.Subroutines.SubroutineImpl(this);
			// Calculate "real" subroutines.
			System.Collections.Generic.HashSet<NBCEL.generic.InstructionHandle> sub_leaders = 
				new System.Collections.Generic.HashSet<NBCEL.generic.InstructionHandle>();
			// Elements: InstructionHandle
			foreach (NBCEL.generic.InstructionHandle element in all)
			{
				NBCEL.generic.Instruction inst = element.GetInstruction();
				if (inst is NBCEL.generic.JsrInstruction)
				{
					sub_leaders.Add(((NBCEL.generic.JsrInstruction)inst).GetTarget());
				}
			}
			// Build up the database.
			foreach (NBCEL.generic.InstructionHandle astore in sub_leaders)
			{
				NBCEL.verifier.structurals.Subroutines.SubroutineImpl sr = new NBCEL.verifier.structurals.Subroutines.SubroutineImpl
					(this);
				sr.SetLocalVariable(((NBCEL.generic.ASTORE)(astore.GetInstruction())).GetIndex());
				Sharpen.Collections.Put(subroutines, astore, sr);
			}
			// Fake it a bit. We want a virtual "TopLevel" subroutine.
			Sharpen.Collections.Put(subroutines, all[0], TOPLEVEL);
			sub_leaders.Add(all[0]);
			// Tell the subroutines about their JsrInstructions.
			// Note that there cannot be a JSR targeting the top-level
			// since "Jsr 0" is disallowed in Pass 3a.
			// Instructions shared by a subroutine and the toplevel are
			// disallowed and checked below, after the BFS.
			foreach (NBCEL.generic.InstructionHandle element in all)
			{
				NBCEL.generic.Instruction inst = element.GetInstruction();
				if (inst is NBCEL.generic.JsrInstruction)
				{
					NBCEL.generic.InstructionHandle leader = ((NBCEL.generic.JsrInstruction)inst).GetTarget
						();
					((NBCEL.verifier.structurals.Subroutines.SubroutineImpl)GetSubroutine(leader)).AddEnteringJsrInstruction
						(element);
				}
			}
			// Now do a BFS from every subroutine leader to find all the
			// instructions that belong to a subroutine.
			// we don't want to assign an instruction to two or more Subroutine objects.
			System.Collections.Generic.HashSet<NBCEL.generic.InstructionHandle> instructions_assigned
				 = new System.Collections.Generic.HashSet<NBCEL.generic.InstructionHandle>();
			//Graph colouring. Key: InstructionHandle, Value: ColourConstants enum .
			System.Collections.Generic.IDictionary<NBCEL.generic.InstructionHandle, NBCEL.verifier.structurals.Subroutines.ColourConstants
				> colors = new System.Collections.Generic.Dictionary<NBCEL.generic.InstructionHandle
				, NBCEL.verifier.structurals.Subroutines.ColourConstants>();
			System.Collections.Generic.List<NBCEL.generic.InstructionHandle> Q = new System.Collections.Generic.List
				<NBCEL.generic.InstructionHandle>();
			foreach (NBCEL.generic.InstructionHandle actual in sub_leaders)
			{
				// Do some BFS with "actual" as the root of the graph.
				// Init colors
				foreach (NBCEL.generic.InstructionHandle element in all)
				{
					Sharpen.Collections.Put(colors, element, NBCEL.verifier.structurals.Subroutines.ColourConstants
						.WHITE);
				}
				Sharpen.Collections.Put(colors, actual, NBCEL.verifier.structurals.Subroutines.ColourConstants
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
				{
					foreach (NBCEL.generic.CodeExceptionGen handler in handlers)
					{
						Sharpen.Collections.Put(colors, handler.GetHandlerPC(), NBCEL.verifier.structurals.Subroutines.ColourConstants
							.GRAY);
						Q.Add(handler.GetHandlerPC());
					}
				}
				/* CONTINUE NORMAL BFS ALGORITHM */
				// Loop until Queue is empty
				while (Q.Count != 0)
				{
					NBCEL.generic.InstructionHandle u = Q.RemoveAtReturningValue(0);
					NBCEL.generic.InstructionHandle[] successors = GetSuccessors(u);
					foreach (NBCEL.generic.InstructionHandle successor in successors)
					{
						if (colors.GetOrNull(successor) == NBCEL.verifier.structurals.Subroutines.ColourConstants
							.WHITE)
						{
							Sharpen.Collections.Put(colors, successor, NBCEL.verifier.structurals.Subroutines.ColourConstants
								.GRAY);
							Q.Add(successor);
						}
					}
					Sharpen.Collections.Put(colors, u, NBCEL.verifier.structurals.Subroutines.ColourConstants
						.BLACK);
				}
				// BFS ended above.
				foreach (NBCEL.generic.InstructionHandle element in all)
				{
					if (colors.GetOrNull(element) == NBCEL.verifier.structurals.Subroutines.ColourConstants
						.BLACK)
					{
						((NBCEL.verifier.structurals.Subroutines.SubroutineImpl)(actual == all[0] ? GetTopLevel
							() : GetSubroutine(actual))).AddInstruction(element);
						if (instructions_assigned.Contains(element))
						{
							throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Instruction '" + 
								element + "' is part of more than one subroutine (or of the top level and a subroutine)."
								);
						}
						instructions_assigned.Add(element);
					}
				}
				if (actual != all[0])
				{
					// If we don't deal with the top-level 'subroutine'
					((NBCEL.verifier.structurals.Subroutines.SubroutineImpl)GetSubroutine(actual)).SetLeavingRET
						();
				}
			}
			if (enableJustIceCheck)
			{
				// Now make sure no instruction of a Subroutine is protected by exception handling code
				// as is mandated by JustIces notion of subroutines.
				foreach (NBCEL.generic.CodeExceptionGen handler in handlers)
				{
					NBCEL.generic.InstructionHandle _protected = handler.GetStartPC();
					while (_protected != handler.GetEndPC().GetNext())
					{
						// Note the inclusive/inclusive notation of "generic API" exception handlers!
						foreach (NBCEL.verifier.structurals.Subroutine sub in subroutines.Values)
						{
							if (sub != subroutines.GetOrNull(all[0]))
							{
								// We don't want to forbid top-level exception handlers.
								if (sub.Contains(_protected))
								{
									throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Subroutine instruction '"
										 + _protected + "' is protected by an exception handler, '" + handler + "'. This is forbidden by the JustIce verifier due to its clear definition of subroutines."
										);
								}
							}
						}
						_protected = _protected.GetNext();
					}
				}
			}
			// Now make sure no subroutine is calling a subroutine
			// that uses the same local variable for the RET as themselves
			// (recursively).
			// This includes that subroutines may not call themselves
			// recursively, even not through intermediate calls to other
			// subroutines.
			NoRecursiveCalls(GetTopLevel(), new System.Collections.Generic.HashSet<int>());
		}

		/// <summary>
		/// This (recursive) utility method makes sure that
		/// no subroutine is calling a subroutine
		/// that uses the same local variable for the RET as themselves
		/// (recursively).
		/// </summary>
		/// <remarks>
		/// This (recursive) utility method makes sure that
		/// no subroutine is calling a subroutine
		/// that uses the same local variable for the RET as themselves
		/// (recursively).
		/// This includes that subroutines may not call themselves
		/// recursively, even not through intermediate calls to other
		/// subroutines.
		/// </remarks>
		/// <exception cref="NBCEL.verifier.exc.StructuralCodeConstraintException">if the above constraint is not satisfied.
		/// 	</exception>
		private void NoRecursiveCalls(NBCEL.verifier.structurals.Subroutine sub, System.Collections.Generic.HashSet
			<int> set)
		{
			NBCEL.verifier.structurals.Subroutine[] subs = sub.SubSubs();
			foreach (NBCEL.verifier.structurals.Subroutine sub2 in subs)
			{
				int index = ((NBCEL.generic.RET)(sub2.GetLeavingRET().GetInstruction())).GetIndex
					();
				if (!set.Add(index))
				{
					// Don't use toString() here because of possibly infinite recursive subSubs() calls then.
					NBCEL.verifier.structurals.Subroutines.SubroutineImpl si = (NBCEL.verifier.structurals.Subroutines.SubroutineImpl
						)sub2;
					throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Subroutine with local variable '"
						 + si.localVariable + "', JSRs '" + si.theJSRs + "', RET '" + si.theRET + "' is called by a subroutine which uses the same local variable index as itself; maybe even a recursive call?"
						 + " JustIce's clean definition of a subroutine forbids both.");
				}
				NoRecursiveCalls(sub2, set);
				set.Remove(index);
			}
		}

		/// <summary>
		/// Returns the Subroutine object associated with the given
		/// leader (that is, the first instruction of the subroutine).
		/// </summary>
		/// <remarks>
		/// Returns the Subroutine object associated with the given
		/// leader (that is, the first instruction of the subroutine).
		/// You must not use this to get the top-level instructions
		/// modeled as a Subroutine object.
		/// </remarks>
		/// <seealso cref="GetTopLevel()"/>
		public virtual NBCEL.verifier.structurals.Subroutine GetSubroutine(NBCEL.generic.InstructionHandle
			 leader)
		{
			NBCEL.verifier.structurals.Subroutine ret = subroutines.GetOrNull(leader);
			if (ret == null)
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("Subroutine requested for an InstructionHandle that is not a leader of a subroutine."
					);
			}
			if (ret == TOPLEVEL)
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("TOPLEVEL special subroutine requested; use getTopLevel()."
					);
			}
			return ret;
		}

		/// <summary>
		/// Returns the subroutine object associated with the
		/// given instruction.
		/// </summary>
		/// <remarks>
		/// Returns the subroutine object associated with the
		/// given instruction. This is a costly operation, you
		/// should consider using getSubroutine(InstructionHandle).
		/// Returns 'null' if the given InstructionHandle lies
		/// in so-called 'dead code', i.e. code that can never
		/// be executed.
		/// </remarks>
		/// <seealso cref="GetSubroutine(NBCEL.generic.InstructionHandle)"/>
		/// <seealso cref="GetTopLevel()"/>
		public virtual NBCEL.verifier.structurals.Subroutine SubroutineOf(NBCEL.generic.InstructionHandle
			 any)
		{
			foreach (NBCEL.verifier.structurals.Subroutine s in subroutines.Values)
			{
				if (s.Contains(any))
				{
					return s;
				}
			}
			System.Console.Error.WriteLine("DEBUG: Please verify '" + any.ToString(true) + "' lies in dead code."
				);
			return null;
		}

		//throw new AssertionViolatedException("No subroutine for InstructionHandle found (DEAD CODE?).");
		/// <summary>
		/// For easy handling, the piece of code that is <B>not</B> a
		/// subroutine, the top-level, is also modeled as a Subroutine
		/// object.
		/// </summary>
		/// <remarks>
		/// For easy handling, the piece of code that is <B>not</B> a
		/// subroutine, the top-level, is also modeled as a Subroutine
		/// object.
		/// It is a special Subroutine object where <B>you must not invoke
		/// getEnteringJsrInstructions() or getLeavingRET()</B>.
		/// </remarks>
		/// <seealso cref="Subroutine.GetEnteringJsrInstructions()"/>
		/// <seealso cref="Subroutine.GetLeavingRET()"/>
		public virtual NBCEL.verifier.structurals.Subroutine GetTopLevel()
		{
			return TOPLEVEL;
		}

		/// <summary>
		/// A utility method that calculates the successors of a given InstructionHandle
		/// <B>in the same subroutine</B>.
		/// </summary>
		/// <remarks>
		/// A utility method that calculates the successors of a given InstructionHandle
		/// <B>in the same subroutine</B>. That means, a RET does not have any successors
		/// as defined here. A JsrInstruction has its physical successor as its successor
		/// (opposed to its target) as defined here.
		/// </remarks>
		private static NBCEL.generic.InstructionHandle[] GetSuccessors(NBCEL.generic.InstructionHandle
			 instruction)
		{
			NBCEL.generic.InstructionHandle[] empty = new NBCEL.generic.InstructionHandle[0];
			NBCEL.generic.InstructionHandle[] single = new NBCEL.generic.InstructionHandle[1]
				;
			NBCEL.generic.Instruction inst = instruction.GetInstruction();
			if (inst is NBCEL.generic.RET)
			{
				return empty;
			}
			// Terminates method normally.
			if (inst is NBCEL.generic.ReturnInstruction)
			{
				return empty;
			}
			// Terminates method abnormally, because JustIce mandates
			// subroutines not to be protected by exception handlers.
			if (inst is NBCEL.generic.ATHROW)
			{
				return empty;
			}
			// See method comment.
			if (inst is NBCEL.generic.JsrInstruction)
			{
				single[0] = instruction.GetNext();
				return single;
			}
			if (inst is NBCEL.generic.GotoInstruction)
			{
				single[0] = ((NBCEL.generic.GotoInstruction)inst).GetTarget();
				return single;
			}
			if (inst is NBCEL.generic.BranchInstruction)
			{
				if (inst is NBCEL.generic.Select)
				{
					// BCEL's getTargets() returns only the non-default targets,
					// thanks to Eli Tilevich for reporting.
					NBCEL.generic.InstructionHandle[] matchTargets = ((NBCEL.generic.Select)inst).GetTargets
						();
					NBCEL.generic.InstructionHandle[] ret = new NBCEL.generic.InstructionHandle[matchTargets
						.Length + 1];
					ret[0] = ((NBCEL.generic.Select)inst).GetTarget();
					System.Array.Copy(matchTargets, 0, ret, 1, matchTargets.Length);
					return ret;
				}
				NBCEL.generic.InstructionHandle[] pair = new NBCEL.generic.InstructionHandle[2];
				pair[0] = instruction.GetNext();
				pair[1] = ((NBCEL.generic.BranchInstruction)inst).GetTarget();
				return pair;
			}
			// default case: Fall through.
			single[0] = instruction.GetNext();
			return single;
		}

		/// <summary>Returns a String representation of this object; merely for debugging puposes.
		/// 	</summary>
		public override string ToString()
		{
			return "---\n" + subroutines + "\n---\n";
		}
	}
}
