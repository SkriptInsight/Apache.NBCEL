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

using System.Linq;
using Sharpen;

namespace NBCEL.verifier.structurals
{
	/// <summary>This class represents a control flow graph of a method.</summary>
	public class ControlFlowGraph
	{
		/// <summary>Objects of this class represent a node in a ControlFlowGraph.</summary>
		/// <remarks>
		/// Objects of this class represent a node in a ControlFlowGraph.
		/// These nodes are instructions, not basic blocks.
		/// </remarks>
		private class InstructionContextImpl : NBCEL.verifier.structurals.InstructionContext
		{
			/// <summary>
			/// The TAG field is here for external temporary flagging, such
			/// as graph colouring.
			/// </summary>
			/// <seealso cref="GetTag()"/>
			/// <seealso cref="SetTag(int)"/>
			private int TAG;

			/// <summary>The InstructionHandle this InstructionContext is wrapped around.</summary>
			private readonly NBCEL.generic.InstructionHandle instruction;

			/// <summary>The 'incoming' execution Frames.</summary>
			private readonly System.Collections.Generic.IDictionary<NBCEL.verifier.structurals.InstructionContext
				, NBCEL.verifier.structurals.Frame> inFrames;

			/// <summary>The 'outgoing' execution Frames.</summary>
			private readonly System.Collections.Generic.IDictionary<NBCEL.verifier.structurals.InstructionContext
				, NBCEL.verifier.structurals.Frame> outFrames;

			/// <summary>
			/// The 'execution predecessors' - a list of type InstructionContext
			/// of those instances that have been execute()d before in that order.
			/// </summary>
			private System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
				> executionPredecessors = null;

			/// <summary>Creates an InstructionHandleImpl object from an InstructionHandle.</summary>
			/// <remarks>
			/// Creates an InstructionHandleImpl object from an InstructionHandle.
			/// Creation of one per InstructionHandle suffices. Don't create more.
			/// </remarks>
			public InstructionContextImpl(ControlFlowGraph _enclosing, NBCEL.generic.InstructionHandle
				 inst)
			{
				this._enclosing = _enclosing;
				// key: the last-executed JSR
				// key: the last-executed JSR
				// Type: InstructionContext
				if (inst == null)
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("Cannot instantiate InstructionContextImpl from NULL."
						);
				}
				this.instruction = inst;
				this.inFrames = new System.Collections.Generic.Dictionary<NBCEL.verifier.structurals.InstructionContext
					, NBCEL.verifier.structurals.Frame>();
				this.outFrames = new System.Collections.Generic.Dictionary<NBCEL.verifier.structurals.InstructionContext
					, NBCEL.verifier.structurals.Frame>();
			}

			/* Satisfies InstructionContext.getTag(). */
			public virtual int GetTag()
			{
				return this.TAG;
			}

			/* Satisfies InstructionContext.setTag(int). */
			public virtual void SetTag(int tag)
			{
				// part of InstructionContext interface
				this.TAG = tag;
			}

			/// <summary>Returns the exception handlers of this instruction.</summary>
			public virtual NBCEL.verifier.structurals.ExceptionHandler[] GetExceptionHandlers
				()
			{
				return this._enclosing.exceptionhandlers.GetExceptionHandlers(this.GetInstruction
					());
			}

			/// <summary>Returns a clone of the "outgoing" frame situation with respect to the given ExecutionChain.
			/// 	</summary>
			public virtual NBCEL.verifier.structurals.Frame GetOutFrame(System.Collections.Generic.List
				<NBCEL.verifier.structurals.InstructionContext> execChain)
			{
				this.executionPredecessors = execChain;
				NBCEL.verifier.structurals.Frame org;
				NBCEL.verifier.structurals.InstructionContext jsr = this.LastExecutionJSR();
				org = this.outFrames.GetOrNull(jsr);
				if (org == null)
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("outFrame not set! This:\n"
						 + this + "\nExecutionChain: " + this.GetExecutionChain() + "\nOutFrames: '" + this
						.outFrames + "'.");
				}
				return org.GetClone();
			}

			public virtual NBCEL.verifier.structurals.Frame GetInFrame()
			{
				NBCEL.verifier.structurals.Frame org;
				NBCEL.verifier.structurals.InstructionContext jsr = this.LastExecutionJSR();
				org = this.inFrames.GetOrNull(jsr);
				if (org == null)
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("inFrame not set! This:\n"
						 + this + "\nInFrames: '" + this.inFrames + "'.");
				}
				return org.GetClone();
			}

			/// <summary>
			/// "Merges in" (vmspec2, page 146) the "incoming" frame situation;
			/// executes the instructions symbolically
			/// and therefore calculates the "outgoing" frame situation.
			/// </summary>
			/// <remarks>
			/// "Merges in" (vmspec2, page 146) the "incoming" frame situation;
			/// executes the instructions symbolically
			/// and therefore calculates the "outgoing" frame situation.
			/// Returns: True iff the "incoming" frame situation changed after
			/// merging with "inFrame".
			/// The execPreds ArrayList must contain the InstructionContext
			/// objects executed so far in the correct order. This is just
			/// one execution path [out of many]. This is needed to correctly
			/// "merge" in the special case of a RET's successor.
			/// <B>The InstConstraintVisitor and ExecutionVisitor instances
			/// must be set up correctly.</B>
			/// </remarks>
			/// <returns>
			/// true - if and only if the "outgoing" frame situation
			/// changed from the one before execute()ing.
			/// </returns>
			public virtual bool Execute(NBCEL.verifier.structurals.Frame inFrame, System.Collections.Generic.List
				<NBCEL.verifier.structurals.InstructionContext> execPreds, NBCEL.verifier.structurals.InstConstraintVisitor
				 icv, NBCEL.verifier.structurals.ExecutionVisitor ev)
			{
				System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext> clone
					= (System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
					>) execPreds.ToList();
				// OK because execPreds is compatible type
				this.executionPredecessors = clone;
				//sanity check
				if ((this.LastExecutionJSR() == null) && (this._enclosing.subroutines.SubroutineOf
					(this.GetInstruction()) != this._enclosing.subroutines.GetTopLevel()))
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("Huh?! Am I '" + this + "' part of a subroutine or not?"
						);
				}
				if ((this.LastExecutionJSR() != null) && (this._enclosing.subroutines.SubroutineOf
					(this.GetInstruction()) == this._enclosing.subroutines.GetTopLevel()))
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("Huh?! Am I '" + this + "' part of a subroutine or not?"
						);
				}
				NBCEL.verifier.structurals.Frame inF = this.inFrames.GetOrNull(this.LastExecutionJSR
					());
				if (inF == null)
				{
					// no incoming frame was set, so set it.
					Sharpen.Collections.Put(this.inFrames, this.LastExecutionJSR(), inFrame);
					inF = inFrame;
				}
				else
				{
					// if there was an "old" inFrame
					if (inF.Equals(inFrame))
					{
						//shortcut: no need to merge equal frames.
						return false;
					}
					if (!this.MergeInFrames(inFrame))
					{
						return false;
					}
				}
				// Now we're sure the inFrame has changed!
				// new inFrame is already merged in, see above.
				NBCEL.verifier.structurals.Frame workingFrame = inF.GetClone();
				try
				{
					// This verifies the InstructionConstraint for the current
					// instruction, but does not modify the workingFrame object.
					//InstConstraintVisitor icv = InstConstraintVisitor.getInstance(VerifierFactory.getVerifier(method_gen.getClassName()));
					icv.SetFrame(workingFrame);
					this.GetInstruction().Accept(icv);
				}
				catch (NBCEL.verifier.exc.StructuralCodeConstraintException ce)
				{
					ce.ExtendMessage(string.Empty, "\nInstructionHandle: " + this.GetInstruction() + 
						"\n");
					ce.ExtendMessage(string.Empty, "\nExecution Frame:\n" + workingFrame);
					this.ExtendMessageWithFlow(ce);
					throw;
				}
				// This executes the Instruction.
				// Therefore the workingFrame object is modified.
				//ExecutionVisitor ev = ExecutionVisitor.getInstance(VerifierFactory.getVerifier(method_gen.getClassName()));
				ev.SetFrame(workingFrame);
				this.GetInstruction().Accept(ev);
				//getInstruction().accept(ExecutionVisitor.withFrame(workingFrame));
				Sharpen.Collections.Put(this.outFrames, this.LastExecutionJSR(), workingFrame);
				return true;
			}

			// new inFrame was different from old inFrame so merging them
			// yielded a different this.inFrame.
			/// <summary>Returns a simple String representation of this InstructionContext.</summary>
			public override string ToString()
			{
				//TODO: Put information in the brackets, e.g.
				//      Is this an ExceptionHandler? Is this a RET? Is this the start of
				//      a subroutine?
				string ret = this.GetInstruction().ToString(false) + "\t[InstructionContext]";
				return ret;
			}

			/// <summary>Does the actual merging (vmspec2, page 146).</summary>
			/// <remarks>
			/// Does the actual merging (vmspec2, page 146).
			/// Returns true IFF this.inFrame was changed in course of merging with inFrame.
			/// </remarks>
			private bool MergeInFrames(NBCEL.verifier.structurals.Frame inFrame)
			{
				// TODO: Can be performance-improved.
				NBCEL.verifier.structurals.Frame inF = this.inFrames.GetOrNull(this.LastExecutionJSR
					());
				NBCEL.verifier.structurals.OperandStack oldstack = inF.GetStack().GetClone();
				NBCEL.verifier.structurals.LocalVariables oldlocals = inF.GetLocals().GetClone();
				try
				{
					inF.GetStack().Merge(inFrame.GetStack());
					inF.GetLocals().Merge(inFrame.GetLocals());
				}
				catch (NBCEL.verifier.exc.StructuralCodeConstraintException sce)
				{
					this.ExtendMessageWithFlow(sce);
					throw;
				}
				return !(oldstack.Equals(inF.GetStack()) && oldlocals.Equals(inF.GetLocals()));
			}

			/// <summary>Returns the control flow execution chain.</summary>
			/// <remarks>
			/// Returns the control flow execution chain. This is built
			/// while execute(Frame, ArrayList)-ing the code represented
			/// by the surrounding ControlFlowGraph.
			/// </remarks>
			private string GetExecutionChain()
			{
				string s = this.ToString();
				for (int i = this.executionPredecessors.Count - 1; i >= 0; i--)
				{
					s = this.executionPredecessors[i] + "\n" + s;
				}
				return s;
			}

			/// <summary>Extends the StructuralCodeConstraintException ("e") object with an at-the-end-extended message.
			/// 	</summary>
			/// <remarks>
			/// Extends the StructuralCodeConstraintException ("e") object with an at-the-end-extended message.
			/// This extended message will then reflect the execution flow needed to get to the constraint
			/// violation that triggered the throwing of the "e" object.
			/// </remarks>
			private void ExtendMessageWithFlow(NBCEL.verifier.exc.StructuralCodeConstraintException
				 e)
			{
				string s = "Execution flow:\n";
				e.ExtendMessage(string.Empty, s + this.GetExecutionChain());
			}

			/*
			* Fulfils the contract of InstructionContext.getInstruction().
			*/
			public virtual NBCEL.generic.InstructionHandle GetInstruction()
			{
				return this.instruction;
			}

			/// <summary>
			/// Returns the InstructionContextImpl with an JSR/JSR_W
			/// that was last in the ExecutionChain, without
			/// a corresponding RET, i.e.
			/// </summary>
			/// <remarks>
			/// Returns the InstructionContextImpl with an JSR/JSR_W
			/// that was last in the ExecutionChain, without
			/// a corresponding RET, i.e.
			/// we were called by this one.
			/// Returns null if we were called from the top level.
			/// </remarks>
			private NBCEL.verifier.structurals.ControlFlowGraph.InstructionContextImpl LastExecutionJSR
				()
			{
				int size = this.executionPredecessors.Count;
				int retcount = 0;
				for (int i = size - 1; i >= 0; i--)
				{
					NBCEL.verifier.structurals.ControlFlowGraph.InstructionContextImpl current = (NBCEL.verifier.structurals.ControlFlowGraph.InstructionContextImpl
						)(this.executionPredecessors[i]);
					NBCEL.generic.Instruction currentlast = current.GetInstruction().GetInstruction();
					if (currentlast is NBCEL.generic.RET)
					{
						retcount++;
					}
					if (currentlast is NBCEL.generic.JsrInstruction)
					{
						retcount--;
						if (retcount == -1)
						{
							return current;
						}
					}
				}
				return null;
			}

			/* Satisfies InstructionContext.getSuccessors(). */
			public virtual NBCEL.verifier.structurals.InstructionContext[] GetSuccessors()
			{
				return this._enclosing.ContextsOf(this._getSuccessors());
			}

			/// <summary>
			/// A utility method that calculates the successors of a given InstructionHandle
			/// That means, a RET does have successors as defined here.
			/// </summary>
			/// <remarks>
			/// A utility method that calculates the successors of a given InstructionHandle
			/// That means, a RET does have successors as defined here.
			/// A JsrInstruction has its target as its successor
			/// (opposed to its physical successor) as defined here.
			/// </remarks>
			private NBCEL.generic.InstructionHandle[] _getSuccessors()
			{
				// TODO: implement caching!
				NBCEL.generic.InstructionHandle[] empty = new NBCEL.generic.InstructionHandle[0];
				NBCEL.generic.InstructionHandle[] single = new NBCEL.generic.InstructionHandle[1]
					;
				NBCEL.generic.Instruction inst = this.GetInstruction().GetInstruction();
				if (inst is NBCEL.generic.RET)
				{
					NBCEL.verifier.structurals.Subroutine s = this._enclosing.subroutines.SubroutineOf
						(this.GetInstruction());
					if (s == null)
					{
						//return empty;
						// RET in dead code. "empty" would be the correct answer, but we know something about the surrounding project...
						throw new NBCEL.verifier.exc.AssertionViolatedException("Asking for successors of a RET in dead code?!"
							);
					}
					//TODO: remove. Only JustIce must not use it, but foreign users of the ControlFlowGraph
					//      will want it. Thanks Johannes Wust.
					//throw new AssertionViolatedException("DID YOU REALLY WANT TO ASK FOR RET'S SUCCS?");
					NBCEL.generic.InstructionHandle[] jsrs = s.GetEnteringJsrInstructions();
					NBCEL.generic.InstructionHandle[] ret = new NBCEL.generic.InstructionHandle[jsrs.
						Length];
					for (int i = 0; i < jsrs.Length; i++)
					{
						ret[i] = jsrs[i].GetNext();
					}
					return ret;
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
					single[0] = ((NBCEL.generic.JsrInstruction)inst).GetTarget();
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
					pair[0] = this.GetInstruction().GetNext();
					pair[1] = ((NBCEL.generic.BranchInstruction)inst).GetTarget();
					return pair;
				}
				// default case: Fall through.
				single[0] = this.GetInstruction().GetNext();
				return single;
			}

			private readonly ControlFlowGraph _enclosing;
		}

		/// <summary>The Subroutines object for the method whose control flow is represented by this ControlFlowGraph.
		/// 	</summary>
		private readonly NBCEL.verifier.structurals.Subroutines subroutines;

		/// <summary>The ExceptionHandlers object for the method whose control flow is represented by this ControlFlowGraph.
		/// 	</summary>
		private readonly NBCEL.verifier.structurals.ExceptionHandlers exceptionhandlers;

		/// <summary>All InstructionContext instances of this ControlFlowGraph.</summary>
		private readonly System.Collections.Generic.IDictionary<NBCEL.generic.InstructionHandle
			, NBCEL.verifier.structurals.InstructionContext> instructionContexts = new System.Collections.Generic.Dictionary
			<NBCEL.generic.InstructionHandle, NBCEL.verifier.structurals.InstructionContext>
			();

		/// <summary>A Control Flow Graph; with additional JustIce checks</summary>
		/// <param name="method_gen">the method generator instance</param>
		public ControlFlowGraph(NBCEL.generic.MethodGen method_gen)
			: this(method_gen, true)
		{
		}

		/// <summary>A Control Flow Graph.</summary>
		/// <param name="method_gen">the method generator instance</param>
		/// <param name="enableJustIceCheck">if true, additional JustIce checks are performed
		/// 	</param>
		/// <since>6.0</since>
		public ControlFlowGraph(NBCEL.generic.MethodGen method_gen, bool enableJustIceCheck
			)
		{
			// End Inner InstructionContextImpl Class.
			///** The MethodGen object we're working on. */
			//private final MethodGen method_gen;
			subroutines = new NBCEL.verifier.structurals.Subroutines(method_gen, enableJustIceCheck
				);
			exceptionhandlers = new NBCEL.verifier.structurals.ExceptionHandlers(method_gen);
			NBCEL.generic.InstructionHandle[] instructionhandles = method_gen.GetInstructionList
				().GetInstructionHandles();
			foreach (NBCEL.generic.InstructionHandle instructionhandle in instructionhandles)
			{
				Sharpen.Collections.Put(instructionContexts, instructionhandle, new NBCEL.verifier.structurals.ControlFlowGraph.InstructionContextImpl
					(this, instructionhandle));
			}
		}

		//this.method_gen = method_gen;
		/// <summary>Returns the InstructionContext of a given instruction.</summary>
		public virtual NBCEL.verifier.structurals.InstructionContext ContextOf(NBCEL.generic.InstructionHandle
			 inst)
		{
			NBCEL.verifier.structurals.InstructionContext ic = instructionContexts.GetOrNull(
				inst);
			if (ic == null)
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("InstructionContext requested for an InstructionHandle that's not known!"
					);
			}
			return ic;
		}

		/// <summary>
		/// Returns the InstructionContext[] of a given InstructionHandle[],
		/// in a naturally ordered manner.
		/// </summary>
		public virtual NBCEL.verifier.structurals.InstructionContext[] ContextsOf(NBCEL.generic.InstructionHandle
			[] insts)
		{
			NBCEL.verifier.structurals.InstructionContext[] ret = new NBCEL.verifier.structurals.InstructionContext
				[insts.Length];
			for (int i = 0; i < insts.Length; i++)
			{
				ret[i] = ContextOf(insts[i]);
			}
			return ret;
		}

		/// <summary>
		/// Returns an InstructionContext[] with all the InstructionContext instances
		/// for the method whose control flow is represented by this ControlFlowGraph
		/// <B>(NOT ORDERED!)</B>.
		/// </summary>
		public virtual NBCEL.verifier.structurals.InstructionContext[] GetInstructionContexts
			()
		{
			NBCEL.verifier.structurals.InstructionContext[] ret = new NBCEL.verifier.structurals.InstructionContext
				[instructionContexts.Values.Count];
			return Sharpen.Collections.ToArray(instructionContexts.Values, ret);
		}

		/// <summary>
		/// Returns true, if and only if the said instruction is not reachable; that means,
		/// if it is not part of this ControlFlowGraph.
		/// </summary>
		public virtual bool IsDead(NBCEL.generic.InstructionHandle i)
		{
			return subroutines.SubroutineOf(i) == null;
		}
	}
}
