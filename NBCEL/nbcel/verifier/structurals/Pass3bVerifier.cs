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
using Sharpen;

namespace NBCEL.verifier.structurals
{
	/// <summary>
	/// This PassVerifier verifies a method of class file according to pass 3,
	/// so-called structural verification as described in The Java Virtual Machine
	/// Specification, 2nd edition.
	/// </summary>
	/// <remarks>
	/// This PassVerifier verifies a method of class file according to pass 3,
	/// so-called structural verification as described in The Java Virtual Machine
	/// Specification, 2nd edition.
	/// More detailed information is to be found at the do_verify() method's
	/// documentation.
	/// </remarks>
	/// <seealso cref="Do_verify()"/>
	public sealed class Pass3bVerifier : NBCEL.verifier.PassVerifier
	{
		/// <summary>
		/// An InstructionContextQueue is a utility class that holds
		/// (InstructionContext, ArrayList) pairs in a Queue data structure.
		/// </summary>
		/// <remarks>
		/// An InstructionContextQueue is a utility class that holds
		/// (InstructionContext, ArrayList) pairs in a Queue data structure.
		/// This is used to hold information about InstructionContext objects
		/// externally --- i.e. that information is not saved inside the
		/// InstructionContext object itself. This is useful to save the
		/// execution path of the symbolic execution of the
		/// Pass3bVerifier - this is not information
		/// that belongs into the InstructionContext object itself.
		/// Only at "execute()"ing
		/// time, an InstructionContext object will get the current information
		/// we have about its symbolic execution predecessors.
		/// </remarks>
		private sealed class InstructionContextQueue
		{
			private readonly System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
				> ics = new List<NBCEL.verifier.structurals.InstructionContext>();

			private readonly System.Collections.Generic.List<System.Collections.Generic.List<
				NBCEL.verifier.structurals.InstructionContext>> ecs = new System.Collections.Generic.List<System.Collections.Generic.List<
				NBCEL.verifier.structurals.InstructionContext>>();

			/* TODO:    Throughout pass 3b, upper halves of LONG and DOUBLE
			are represented by Type.UNKNOWN. This should be changed
			in favour of LONG_Upper and DOUBLE_Upper as in pass 2. */
			public void Add(NBCEL.verifier.structurals.InstructionContext ic, System.Collections.Generic.List
				<NBCEL.verifier.structurals.InstructionContext> executionChain)
			{
				ics.Add(ic);
				ecs.Add(executionChain);
			}

			public bool IsEmpty()
			{
				return (ics.Count == 0);
			}

			public void Remove(int i)
			{
				ics.RemoveAtReturningValue(i);
				ecs.RemoveAtReturningValue(i);
			}

			public NBCEL.verifier.structurals.InstructionContext GetIC(int i)
			{
				return ics[i];
			}

			public System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
				> GetEC(int i)
			{
				return ecs[i];
			}

			public int Size()
			{
				return ics.Count;
			}
		}

		/// <summary>In DEBUG mode, the verification algorithm is not randomized.</summary>
		private const bool DEBUG = true;

		/// <summary>The Verifier that created this.</summary>
		private readonly NBCEL.verifier.Verifier myOwner;

		/// <summary>The method number to verify.</summary>
		private readonly int method_no;

		/// <summary>This class should only be instantiated by a Verifier.</summary>
		/// <seealso cref="NBCEL.verifier.Verifier"/>
		public Pass3bVerifier(NBCEL.verifier.Verifier owner, int method_no)
		{
			// end Inner Class InstructionContextQueue
			myOwner = owner;
			this.method_no = method_no;
		}

		/// <summary>
		/// Whenever the outgoing frame
		/// situation of an InstructionContext changes, all its successors are
		/// put [back] into the queue [as if they were unvisited].
		/// </summary>
		/// <remarks>
		/// Whenever the outgoing frame
		/// situation of an InstructionContext changes, all its successors are
		/// put [back] into the queue [as if they were unvisited].
		/// The proof of termination is about the existence of a
		/// fix point of frame merging.
		/// </remarks>
		private void CirculationPump(NBCEL.generic.MethodGen m, NBCEL.verifier.structurals.ControlFlowGraph
			 cfg, NBCEL.verifier.structurals.InstructionContext start, NBCEL.verifier.structurals.Frame
			 vanillaFrame, NBCEL.verifier.structurals.InstConstraintVisitor icv, NBCEL.verifier.structurals.ExecutionVisitor
			 ev)
		{
			Random random = new Random();
			NBCEL.verifier.structurals.Pass3bVerifier.InstructionContextQueue icq = new NBCEL.verifier.structurals.Pass3bVerifier.InstructionContextQueue
				();
			start.Execute(vanillaFrame, new System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
				>(), icv, ev);
			// new ArrayList() <=>    no Instruction was executed before
			//                                    => Top-Level routine (no jsr call before)
			icq.Add(start, new System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
				>());
			// LOOP!
			while (!icq.IsEmpty())
			{
				NBCEL.verifier.structurals.InstructionContext u;
				System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext> ec;
				u = icq.GetIC(0);
				ec = icq.GetEC(0);
				icq.Remove(0);
				System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext> oldchain
					 = (System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
					>)(ec.ToList());
				// ec is of type ArrayList<InstructionContext>
				System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext> newchain
					 = (System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
					>)(ec.ToList());
				// ec is of type ArrayList<InstructionContext>
				newchain.Add(u);
				if ((u.GetInstruction().GetInstruction()) is NBCEL.generic.RET)
				{
					//System.err.println(u);
					// We can only follow _one_ successor, the one after the
					// JSR that was recently executed.
					NBCEL.generic.RET ret = (NBCEL.generic.RET)(u.GetInstruction().GetInstruction());
					NBCEL.generic.ReturnaddressType t = (NBCEL.generic.ReturnaddressType)u.GetOutFrame
						(oldchain).GetLocals().Get(ret.GetIndex());
					NBCEL.verifier.structurals.InstructionContext theSuccessor = cfg.ContextOf(t.GetTarget
						());
					// Sanity check
					NBCEL.verifier.structurals.InstructionContext lastJSR = null;
					int skip_jsr = 0;
					for (int ss = oldchain.Count - 1; ss >= 0; ss--)
					{
						if (skip_jsr < 0)
						{
							throw new NBCEL.verifier.exc.AssertionViolatedException("More RET than JSR in execution chain?!"
								);
						}
						//System.err.println("+"+oldchain.get(ss));
						if ((oldchain[ss]).GetInstruction().GetInstruction() is NBCEL.generic.JsrInstruction)
						{
							if (skip_jsr == 0)
							{
								lastJSR = oldchain[ss];
								break;
							}
							skip_jsr--;
						}
						if ((oldchain[ss]).GetInstruction().GetInstruction() is NBCEL.generic.RET)
						{
							skip_jsr++;
						}
					}
					if (lastJSR == null)
					{
						throw new NBCEL.verifier.exc.AssertionViolatedException("RET without a JSR before in ExecutionChain?! EC: '"
							 + oldchain + "'.");
					}
					NBCEL.generic.JsrInstruction jsr = (NBCEL.generic.JsrInstruction)(lastJSR.GetInstruction
						().GetInstruction());
					if (theSuccessor != (cfg.ContextOf(jsr.PhysicalSuccessor())))
					{
						throw new NBCEL.verifier.exc.AssertionViolatedException("RET '" + u.GetInstruction
							() + "' info inconsistent: jump back to '" + theSuccessor + "' or '" + cfg.ContextOf
							(jsr.PhysicalSuccessor()) + "'?");
					}
					if (theSuccessor.Execute(u.GetOutFrame(oldchain), newchain, icv, ev))
					{
						System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext> newchainClone
							 = (System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
							>)newchain.ToList();
						// newchain is already of type ArrayList<InstructionContext>
						icq.Add(theSuccessor, newchainClone);
					}
				}
				else
				{
					// "not a ret"
					// Normal successors. Add them to the queue of successors.
					NBCEL.verifier.structurals.InstructionContext[] succs = u.GetSuccessors();
					foreach (NBCEL.verifier.structurals.InstructionContext v in succs)
					{
						if (v.Execute(u.GetOutFrame(oldchain), newchain, icv, ev))
						{
							System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext> newchainClone
								 = (System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
								>)newchain.ToList();
							// newchain is already of type ArrayList<InstructionContext>
							icq.Add(v, newchainClone);
						}
					}
				}
				// end "not a ret"
				// Exception Handlers. Add them to the queue of successors.
				// [subroutines are never protected; mandated by JustIce]
				NBCEL.verifier.structurals.ExceptionHandler[] exc_hds = u.GetExceptionHandlers();
				foreach (NBCEL.verifier.structurals.ExceptionHandler exc_hd in exc_hds)
				{
					NBCEL.verifier.structurals.InstructionContext v = cfg.ContextOf(exc_hd.GetHandlerStart
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
					if (v.Execute(new NBCEL.verifier.structurals.Frame(u.GetOutFrame(oldchain).GetLocals
						(), new NBCEL.verifier.structurals.OperandStack(u.GetOutFrame(oldchain).GetStack
						().MaxStack(), exc_hd.GetExceptionType() == null ? NBCEL.generic.Type.THROWABLE : 
						exc_hd.GetExceptionType())), new System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
						>(), icv, ev))
					{
						icq.Add(v, new System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
							>());
					}
				}
			}
			// while (!icq.isEmpty()) END
			NBCEL.generic.InstructionHandle ih = start.GetInstruction();
			do
			{
				if ((ih.GetInstruction() is NBCEL.generic.ReturnInstruction) && (!(cfg.IsDead(ih)
					)))
				{
					NBCEL.verifier.structurals.InstructionContext ic = cfg.ContextOf(ih);
					// TODO: This is buggy, we check only the top-level return instructions this way.
					// Maybe some maniac returns from a method when in a subroutine?
					NBCEL.verifier.structurals.Frame f = ic.GetOutFrame(new System.Collections.Generic.List
						<NBCEL.verifier.structurals.InstructionContext>());
					NBCEL.verifier.structurals.LocalVariables lvs = f.GetLocals();
					for (int i = 0; i < lvs.MaxLocals(); i++)
					{
						if (lvs.Get(i) is NBCEL.verifier.structurals.UninitializedObjectType)
						{
							this.AddMessage("Warning: ReturnInstruction '" + ic + "' may leave method with an uninitialized object in the local variables array '"
								 + lvs + "'.");
						}
					}
					NBCEL.verifier.structurals.OperandStack os = f.GetStack();
					for (int i = 0; i < os.Size(); i++)
					{
						if (os.Peek(i) is NBCEL.verifier.structurals.UninitializedObjectType)
						{
							this.AddMessage("Warning: ReturnInstruction '" + ic + "' may leave method with an uninitialized object on the operand stack '"
								 + os + "'.");
						}
					}
					//see JVM $4.8.2
					NBCEL.generic.Type returnedType = null;
					NBCEL.verifier.structurals.OperandStack inStack = ic.GetInFrame().GetStack();
					if (inStack.Size() >= 1)
					{
						returnedType = inStack.Peek();
					}
					else
					{
						returnedType = NBCEL.generic.Type.VOID;
					}
					if (returnedType != null)
					{
						if (returnedType is NBCEL.generic.ReferenceType)
						{
							try
							{
								if (!((NBCEL.generic.ReferenceType)returnedType).IsCastableTo(m.GetReturnType()))
								{
									InvalidReturnTypeError(returnedType, m);
								}
							}
							catch (System.TypeLoadException e)
							{
								// Don't know what do do now, so raise RuntimeException
								throw new System.Exception(e.Message);
							}
						}
						else if (!returnedType.Equals(m.GetReturnType().NormalizeForStackOrLocal()))
						{
							InvalidReturnTypeError(returnedType, m);
						}
					}
				}
			}
			while ((ih = ih.GetNext()) != null);
		}

		/// <summary>Throws an exception indicating the returned type is not compatible with the return type of the given method
		/// 	</summary>
		/// <exception cref="NBCEL.verifier.exc.StructuralCodeConstraintException">always</exception>
		/// <since>6.0</since>
		public void InvalidReturnTypeError(NBCEL.generic.Type returnedType, NBCEL.generic.MethodGen
			 m)
		{
			throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Returned type " +
				 returnedType + " does not match Method's return type " + m.GetReturnType());
		}

		/// <summary>
		/// Pass 3b implements the data flow analysis as described in the Java Virtual
		/// Machine Specification, Second Edition.
		/// </summary>
		/// <remarks>
		/// Pass 3b implements the data flow analysis as described in the Java Virtual
		/// Machine Specification, Second Edition.
		/// Later versions will use LocalVariablesInfo objects to verify if the
		/// verifier-inferred types and the class file's debug information (LocalVariables
		/// attributes) match [TODO].
		/// </remarks>
		/// <seealso cref="NBCEL.verifier.statics.LocalVariablesInfo"/>
		/// <seealso cref="NBCEL.verifier.statics.Pass2Verifier.GetLocalVariablesInfo(int)"/>
		public override NBCEL.verifier.VerificationResult Do_verify()
		{
			if (!myOwner.DoPass3a(method_no).Equals(NBCEL.verifier.VerificationResult.VR_OK))
			{
				return NBCEL.verifier.VerificationResult.VR_NOTYET;
			}
			// Pass 3a ran before, so it's safe to assume the JavaClass object is
			// in the BCEL repository.
			NBCEL.classfile.JavaClass jc;
			try
			{
				jc = NBCEL.Repository.LookupClass(myOwner.GetClassName());
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: maybe not the best way to handle this
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
			NBCEL.generic.ConstantPoolGen constantPoolGen = new NBCEL.generic.ConstantPoolGen
				(jc.GetConstantPool());
			// Init Visitors
			NBCEL.verifier.structurals.InstConstraintVisitor icv = new NBCEL.verifier.structurals.InstConstraintVisitor
				();
			icv.SetConstantPoolGen(constantPoolGen);
			NBCEL.verifier.structurals.ExecutionVisitor ev = new NBCEL.verifier.structurals.ExecutionVisitor
				();
			ev.SetConstantPoolGen(constantPoolGen);
			NBCEL.classfile.Method[] methods = jc.GetMethods();
			// Method no "method_no" exists, we ran Pass3a before on it!
			try
			{
				NBCEL.generic.MethodGen mg = new NBCEL.generic.MethodGen(methods[method_no], myOwner
					.GetClassName(), constantPoolGen);
				icv.SetMethodGen(mg);
				////////////// DFA BEGINS HERE ////////////////
				if (!(mg.IsAbstract() || mg.IsNative()))
				{
					// IF mg HAS CODE (See pass 2)
					NBCEL.verifier.structurals.ControlFlowGraph cfg = new NBCEL.verifier.structurals.ControlFlowGraph
						(mg);
					// Build the initial frame situation for this method.
					NBCEL.verifier.structurals.Frame f = new NBCEL.verifier.structurals.Frame(mg.GetMaxLocals
						(), mg.GetMaxStack());
					if (!mg.IsStatic())
					{
						if (mg.GetName().Equals(NBCEL.Const.CONSTRUCTOR_NAME))
						{
							NBCEL.verifier.structurals.Frame.SetThis(new NBCEL.verifier.structurals.UninitializedObjectType
								(NBCEL.generic.ObjectType.GetInstance(jc.GetClassName())));
							f.GetLocals().Set(0, NBCEL.verifier.structurals.Frame.GetThis());
						}
						else
						{
							NBCEL.verifier.structurals.Frame.SetThis(null);
							f.GetLocals().Set(0, NBCEL.generic.ObjectType.GetInstance(jc.GetClassName()));
						}
					}
					NBCEL.generic.Type[] argtypes = mg.GetArgumentTypes();
					int twoslotoffset = 0;
					for (int j = 0; j < argtypes.Length; j++)
					{
						if (argtypes[j] == NBCEL.generic.Type.SHORT || argtypes[j] == NBCEL.generic.Type.
							BYTE || argtypes[j] == NBCEL.generic.Type.CHAR || argtypes[j] == NBCEL.generic.Type
							.BOOLEAN)
						{
							argtypes[j] = NBCEL.generic.Type.INT;
						}
						f.GetLocals().Set(twoslotoffset + j + (mg.IsStatic() ? 0 : 1), argtypes[j]);
						if (argtypes[j].GetSize() == 2)
						{
							twoslotoffset++;
							f.GetLocals().Set(twoslotoffset + j + (mg.IsStatic() ? 0 : 1), NBCEL.generic.Type
								.UNKNOWN);
						}
					}
					CirculationPump(mg, cfg, cfg.ContextOf(mg.GetInstructionList().GetStart()), f, icv
						, ev);
				}
			}
			catch (NBCEL.verifier.exc.VerifierConstraintViolatedException ce)
			{
				ce.ExtendMessage("Constraint violated in method '" + methods[method_no] + "':\n", 
					string.Empty);
				return new NBCEL.verifier.VerificationResult(NBCEL.verifier.VerificationResult.VERIFIED_REJECTED
					, ce.Message);
			}
			catch (System.Exception re)
			{
				// These are internal errors
				System.IO.StringWriter sw = new System.IO.StringWriter();
				System.IO.TextWriter pw = sw;
				Sharpen.Runtime.PrintStackTrace(re, pw);
				throw new NBCEL.verifier.exc.AssertionViolatedException("Some RuntimeException occured while verify()ing class '"
					 + jc.GetClassName() + "', method '" + methods[method_no] + "'. Original RuntimeException's stack trace:\n---\n"
					 + sw + "---\n", re);
			}
			return NBCEL.verifier.VerificationResult.VR_OK;
		}

		/// <summary>Returns the method number as supplied when instantiating.</summary>
		public int GetMethodNo()
		{
			return method_no;
		}
	}
}
