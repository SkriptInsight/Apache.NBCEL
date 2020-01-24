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

namespace NBCEL.verifier.statics
{
	/// <summary>
	/// This PassVerifier verifies a class file according to
	/// pass 3, static part as described in The Java Virtual
	/// Machine Specification, 2nd edition.
	/// </summary>
	/// <remarks>
	/// This PassVerifier verifies a class file according to
	/// pass 3, static part as described in The Java Virtual
	/// Machine Specification, 2nd edition.
	/// More detailed information is to be found at the do_verify()
	/// method's documentation.
	/// </remarks>
	/// <seealso cref="Do_verify()"/>
	public sealed class Pass3aVerifier : NBCEL.verifier.PassVerifier
	{
		/// <summary>The Verifier that created this.</summary>
		private readonly NBCEL.verifier.Verifier myOwner;

		/// <summary>The method number to verify.</summary>
		/// <remarks>
		/// The method number to verify.
		/// This is the index in the array returned
		/// by JavaClass.getMethods().
		/// </remarks>
		private readonly int method_no;

		/// <summary>The one and only InstructionList object used by an instance of this class.
		/// 	</summary>
		/// <remarks>
		/// The one and only InstructionList object used by an instance of this class.
		/// It's here for performance reasons by do_verify() and its callees.
		/// </remarks>
		private NBCEL.generic.InstructionList instructionList;

		/// <summary>The one and only Code object used by an instance of this class.</summary>
		/// <remarks>
		/// The one and only Code object used by an instance of this class.
		/// It's here for performance reasons by do_verify() and its callees.
		/// </remarks>
		private NBCEL.classfile.Code code;

		/// <summary>Should only be instantiated by a Verifier.</summary>
		public Pass3aVerifier(NBCEL.verifier.Verifier owner, int method_no)
		{
			myOwner = owner;
			this.method_no = method_no;
		}

		/// <summary>
		/// Pass 3a is the verification of static constraints of
		/// JVM code (such as legal targets of branch instructions).
		/// </summary>
		/// <remarks>
		/// Pass 3a is the verification of static constraints of
		/// JVM code (such as legal targets of branch instructions).
		/// This is the part of pass 3 where you do not need data
		/// flow analysis.
		/// JustIce also delays the checks for a correct exception
		/// table of a Code attribute and correct line number entries
		/// in a LineNumberTable attribute of a Code attribute (which
		/// conceptually belong to pass 2) to this pass. Also, most
		/// of the check for valid local variable entries in a
		/// LocalVariableTable attribute of a Code attribute is
		/// delayed until this pass.
		/// All these checks need access to the code array of the
		/// Code attribute.
		/// </remarks>
		/// <exception cref="NBCEL.verifier.exc.InvalidMethodException">if the method to verify does not exist.
		/// 	</exception>
		public override NBCEL.verifier.VerificationResult Do_verify()
		{
			try
			{
				if (myOwner.DoPass2().Equals(NBCEL.verifier.VerificationResult.VR_OK))
				{
					// Okay, class file was loaded correctly by Pass 1
					// and satisfies static constraints of Pass 2.
					NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(myOwner.GetClassName(
						));
					NBCEL.classfile.Method[] methods = jc.GetMethods();
					if (method_no >= methods.Length)
					{
						throw new NBCEL.verifier.exc.InvalidMethodException("METHOD DOES NOT EXIST!");
					}
					NBCEL.classfile.Method method = methods[method_no];
					code = method.GetCode();
					// No Code? Nothing to verify!
					if (method.IsAbstract() || method.IsNative())
					{
						// IF mg HAS NO CODE (static constraint of Pass 2)
						return NBCEL.verifier.VerificationResult.VR_OK;
					}
					// TODO:
					// We want a very sophisticated code examination here with good explanations
					// on where to look for an illegal instruction or such.
					// Only after that we should try to build an InstructionList and throw an
					// AssertionViolatedException if after our examination InstructionList building
					// still fails.
					// That examination should be implemented in a byte-oriented way, i.e. look for
					// an instruction, make sure its validity, count its length, find the next
					// instruction and so on.
					try
					{
						instructionList = new NBCEL.generic.InstructionList(method.GetCode().GetCode());
					}
					catch (System.Exception)
					{
						return new NBCEL.verifier.VerificationResult(NBCEL.verifier.VerificationResult.VERIFIED_REJECTED
							, "Bad bytecode in the code array of the Code attribute of method '" + method + 
							"'.");
					}
					instructionList.SetPositions(true);
					// Start verification.
					NBCEL.verifier.VerificationResult vr = NBCEL.verifier.VerificationResult.VR_OK;
					//default
					try
					{
						DelayedPass2Checks();
					}
					catch (NBCEL.verifier.exc.ClassConstraintException cce)
					{
						vr = new NBCEL.verifier.VerificationResult(NBCEL.verifier.VerificationResult.VERIFIED_REJECTED
							, cce.Message);
						return vr;
					}
					try
					{
						Pass3StaticInstructionChecks();
						Pass3StaticInstructionOperandsChecks();
					}
					catch (NBCEL.verifier.exc.StaticCodeConstraintException scce)
					{
						vr = new NBCEL.verifier.VerificationResult(NBCEL.verifier.VerificationResult.VERIFIED_REJECTED
							, scce.Message);
					}
					catch (System.InvalidCastException cce)
					{
						vr = new NBCEL.verifier.VerificationResult(NBCEL.verifier.VerificationResult.VERIFIED_REJECTED
							, "Class Cast Exception: " + cce.Message);
					}
					return vr;
				}
				//did not pass Pass 2.
				return NBCEL.verifier.VerificationResult.VR_NOTYET;
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: maybe not the best way to handle this
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>
		/// These are the checks that could be done in pass 2 but are delayed to pass 3
		/// for performance reasons.
		/// </summary>
		/// <remarks>
		/// These are the checks that could be done in pass 2 but are delayed to pass 3
		/// for performance reasons. Also, these checks need access to the code array
		/// of the Code attribute of a Method so it's okay to perform them here.
		/// Also see the description of the do_verify() method.
		/// </remarks>
		/// <exception cref="NBCEL.verifier.exc.ClassConstraintException">if the verification fails.
		/// 	</exception>
		/// <seealso cref="Do_verify()"/>
		private void DelayedPass2Checks()
		{
			int[] instructionPositions = instructionList.GetInstructionPositions();
			int codeLength = code.GetCode().Length;
			/////////////////////
			// LineNumberTable //
			/////////////////////
			NBCEL.classfile.LineNumberTable lnt = code.GetLineNumberTable();
			if (lnt != null)
			{
				NBCEL.classfile.LineNumber[] lineNumbers = lnt.GetLineNumberTable();
				NBCEL.verifier.statics.IntList offsets = new NBCEL.verifier.statics.IntList();
				
				lineNumber_loop_continue: 
				foreach (NBCEL.classfile.LineNumber lineNumber in lineNumbers)
				{
					// may appear in any order.
					foreach (int instructionPosition in instructionPositions)
					{
						// TODO: Make this a binary search! The instructionPositions array is naturally ordered!
						int offset = lineNumber.GetStartPC();
						if (instructionPosition == offset)
						{
							if (offsets.Contains(offset))
							{
								AddMessage("LineNumberTable attribute '" + code.GetLineNumberTable() + "' refers to the same code offset ('"
									 + offset + "') more than once" + " which is violating the semantics [but is sometimes produced by IBM's 'jikes' compiler]."
									);
							}
							else
							{
								offsets.Add(offset);
							}
							goto lineNumber_loop_continue;
						}
					}
					throw new NBCEL.verifier.exc.ClassConstraintException("Code attribute '" + code +
						 "' has a LineNumberTable attribute '" + code.GetLineNumberTable() + "' referring to a code offset ('"
						 + lineNumber.GetStartPC() + "') that does not exist.");
				}
lineNumber_loop_break: ;
			}
			///////////////////////////
			// LocalVariableTable(s) //
			///////////////////////////
			/* We cannot use code.getLocalVariableTable() because there could be more
			than only one. This is a bug in BCEL. */
			NBCEL.classfile.Attribute[] atts = code.GetAttributes();
			foreach (NBCEL.classfile.Attribute att in atts)
			{
				if (att is NBCEL.classfile.LocalVariableTable)
				{
					NBCEL.classfile.LocalVariableTable lvt = (NBCEL.classfile.LocalVariableTable)att;
					NBCEL.classfile.LocalVariable[] localVariables = lvt.GetLocalVariableTable();
					foreach (NBCEL.classfile.LocalVariable localVariable in localVariables)
					{
						int startpc = localVariable.GetStartPC();
						int length = localVariable.GetLength();
						if (!Contains(instructionPositions, startpc))
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Code attribute '" + code +
								 "' has a LocalVariableTable attribute '" + code.GetLocalVariableTable() + "' referring to a code offset ('"
								 + startpc + "') that does not exist.");
						}
						if ((!Contains(instructionPositions, startpc + length)) && (startpc + length != codeLength
							))
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Code attribute '" + code +
								 "' has a LocalVariableTable attribute '" + code.GetLocalVariableTable() + "' referring to a code offset start_pc+length ('"
								 + (startpc + length) + "') that does not exist.");
						}
					}
				}
			}
			////////////////////
			// ExceptionTable //
			////////////////////
			// In BCEL's "classfile" API, the startPC/endPC-notation is
			// inclusive/exclusive as in the Java Virtual Machine Specification.
			// WARNING: This is not true for BCEL's "generic" API.
			NBCEL.classfile.CodeException[] exceptionTable = code.GetExceptionTable();
			foreach (NBCEL.classfile.CodeException element in exceptionTable)
			{
				int startpc = element.GetStartPC();
				int endpc = element.GetEndPC();
				int handlerpc = element.GetHandlerPC();
				if (startpc >= endpc)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Code attribute '" + code +
						 "' has an exception_table entry '" + element + "' that has its start_pc ('" + startpc
						 + "') not smaller than its end_pc ('" + endpc + "').");
				}
				if (!Contains(instructionPositions, startpc))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Code attribute '" + code +
						 "' has an exception_table entry '" + element + "' that has a non-existant bytecode offset as its start_pc ('"
						 + startpc + "').");
				}
				if ((!Contains(instructionPositions, endpc)) && (endpc != codeLength))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Code attribute '" + code +
						 "' has an exception_table entry '" + element + "' that has a non-existant bytecode offset as its end_pc ('"
						 + startpc + "') [that is also not equal to code_length ('" + codeLength + "')]."
						);
				}
				if (!Contains(instructionPositions, handlerpc))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Code attribute '" + code +
						 "' has an exception_table entry '" + element + "' that has a non-existant bytecode offset as its handler_pc ('"
						 + handlerpc + "').");
				}
			}
		}

		/// <summary>
		/// These are the checks if constraints are satisfied which are described in the
		/// Java Virtual Machine Specification, Second Edition as Static Constraints on
		/// the instructions of Java Virtual Machine Code (chapter 4.8.1).
		/// </summary>
		/// <exception cref="NBCEL.verifier.exc.StaticCodeConstraintException">if the verification fails.
		/// 	</exception>
		private void Pass3StaticInstructionChecks()
		{
			// Code array must not be empty:
			// Enforced in pass 2 (also stated in the static constraints of the Code
			// array in vmspec2), together with pass 1 (reading code_length bytes and
			// interpreting them as code[]). So this must not be checked again here.
			if (code.GetCode().Length >= NBCEL.Const.MAX_CODE_SIZE)
			{
				// length must be LESS than the max
				throw new NBCEL.verifier.exc.StaticCodeInstructionConstraintException("Code array in code attribute '"
					 + code + "' too big: must be smaller than " + NBCEL.Const.MAX_CODE_SIZE + "65536 bytes."
					);
			}
			// First opcode at offset 0: okay, that's clear. Nothing to do.
			// Only instances of the instructions documented in Section 6.4 may appear in
			// the code array.
			// For BCEL's sake, we cannot handle WIDE stuff, but hopefully BCEL does its job right :)
			// The last byte of the last instruction in the code array must be the byte at index
			// code_length-1 : See the do_verify() comments. We actually don't iterate through the
			// byte array, but use an InstructionList so we cannot check for this. But BCEL does
			// things right, so it's implicitly okay.
			// TODO: Check how BCEL handles (and will handle) instructions like IMPDEP1, IMPDEP2,
			//       BREAKPOINT... that BCEL knows about but which are illegal anyway.
			//       We currently go the safe way here.
			NBCEL.generic.InstructionHandle ih = instructionList.GetStart();
			while (ih != null)
			{
				NBCEL.generic.Instruction i = ih.GetInstruction();
				if (i is NBCEL.generic.IMPDEP1)
				{
					throw new NBCEL.verifier.exc.StaticCodeInstructionConstraintException("IMPDEP1 must not be in the code, it is an illegal instruction for _internal_ JVM use!"
						);
				}
				if (i is NBCEL.generic.IMPDEP2)
				{
					throw new NBCEL.verifier.exc.StaticCodeInstructionConstraintException("IMPDEP2 must not be in the code, it is an illegal instruction for _internal_ JVM use!"
						);
				}
				if (i is NBCEL.generic.BREAKPOINT)
				{
					throw new NBCEL.verifier.exc.StaticCodeInstructionConstraintException("BREAKPOINT must not be in the code, it is an illegal instruction for _internal_ JVM use!"
						);
				}
				ih = ih.GetNext();
			}
			// The original verifier seems to do this check here, too.
			// An unreachable last instruction may also not fall through the
			// end of the code, which is stupid -- but with the original
			// verifier's subroutine semantics one cannot predict reachability.
			NBCEL.generic.Instruction last = instructionList.GetEnd().GetInstruction();
			if (!((last is NBCEL.generic.ReturnInstruction) || (last is NBCEL.generic.RET) ||
				 (last is NBCEL.generic.GotoInstruction) || (last is NBCEL.generic.ATHROW)))
			{
				throw new NBCEL.verifier.exc.StaticCodeInstructionConstraintException("Execution must not fall off the bottom of the code array."
					 + " This constraint is enforced statically as some existing verifiers do" + " - so it may be a false alarm if the last instruction is not reachable."
					);
			}
		}

		/// <summary>
		/// These are the checks for the satisfaction of constraints which are described in the
		/// Java Virtual Machine Specification, Second Edition as Static Constraints on
		/// the operands of instructions of Java Virtual Machine Code (chapter 4.8.1).
		/// </summary>
		/// <remarks>
		/// These are the checks for the satisfaction of constraints which are described in the
		/// Java Virtual Machine Specification, Second Edition as Static Constraints on
		/// the operands of instructions of Java Virtual Machine Code (chapter 4.8.1).
		/// BCEL parses the code array to create an InstructionList and therefore has to check
		/// some of these constraints. Additional checks are also implemented here.
		/// </remarks>
		/// <exception cref="NBCEL.verifier.exc.StaticCodeConstraintException">if the verification fails.
		/// 	</exception>
		private void Pass3StaticInstructionOperandsChecks()
		{
			try
			{
				// When building up the InstructionList, BCEL has already done all those checks
				// mentioned in The Java Virtual Machine Specification, Second Edition, as
				// "static constraints on the operands of instructions in the code array".
				// TODO: see the do_verify() comments. Maybe we should really work on the
				//       byte array first to give more comprehensive messages.
				// TODO: Review Exception API, possibly build in some "offending instruction" thing
				//       when we're ready to insulate the offending instruction by doing the
				//       above thing.
				// TODO: Implement as much as possible here. BCEL does _not_ check everything.
				NBCEL.generic.ConstantPoolGen cpg = new NBCEL.generic.ConstantPoolGen(NBCEL.Repository
					.LookupClass(myOwner.GetClassName()).GetConstantPool());
				NBCEL.verifier.statics.Pass3aVerifier.InstOperandConstraintVisitor v = new NBCEL.verifier.statics.Pass3aVerifier.InstOperandConstraintVisitor
					(this, cpg);
				// Checks for the things BCEL does _not_ handle itself.
				NBCEL.generic.InstructionHandle ih = instructionList.GetStart();
				while (ih != null)
				{
					NBCEL.generic.Instruction i = ih.GetInstruction();
					// An "own" constraint, due to JustIce's new definition of what "subroutine" means.
					if (i is NBCEL.generic.JsrInstruction)
					{
						NBCEL.generic.InstructionHandle target = ((NBCEL.generic.JsrInstruction)i).GetTarget
							();
						if (target == instructionList.GetStart())
						{
							throw new NBCEL.verifier.exc.StaticCodeInstructionOperandConstraintException("Due to JustIce's clear definition of subroutines, no JSR or JSR_W may have a top-level instruction"
								 + " (such as the very first instruction, which is targeted by instruction '" + 
								ih + "' as its target.");
						}
						if (!(target.GetInstruction() is NBCEL.generic.ASTORE))
						{
							throw new NBCEL.verifier.exc.StaticCodeInstructionOperandConstraintException("Due to JustIce's clear definition of subroutines, no JSR or JSR_W may target anything else"
								 + " than an ASTORE instruction. Instruction '" + ih + "' targets '" + target + 
								"'.");
						}
					}
					// vmspec2, page 134-137
					ih.Accept(v);
					ih = ih.GetNext();
				}
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: maybe not the best way to handle this
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>A small utility method returning if a given int i is in the given int[] ints.
		/// 	</summary>
		private static bool Contains(int[] ints, int i)
		{
			foreach (int k in ints)
			{
				if (k == i)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Returns the method number as supplied when instantiating.</summary>
		public int GetMethodNo()
		{
			return method_no;
		}

		/// <summary>
		/// This visitor class does the actual checking for the instruction
		/// operand's constraints.
		/// </summary>
		private class InstOperandConstraintVisitor : NBCEL.generic.EmptyVisitor
		{
			/// <summary>The ConstantPoolGen instance this Visitor operates on.</summary>
			private readonly NBCEL.generic.ConstantPoolGen constantPoolGen;

			/// <summary>The only Constructor.</summary>
			internal InstOperandConstraintVisitor(Pass3aVerifier _enclosing, NBCEL.generic.ConstantPoolGen
				 constantPoolGen)
			{
				this._enclosing = _enclosing;
				this.constantPoolGen = constantPoolGen;
			}

			/// <summary>
			/// Utility method to return the max_locals value of the method verified
			/// by the surrounding Pass3aVerifier instance.
			/// </summary>
			private int Max_locals()
			{
				try
				{
					return NBCEL.Repository.LookupClass(this._enclosing.myOwner.GetClassName()).GetMethods
						()[this._enclosing.method_no].GetCode().GetMaxLocals();
				}
				catch (System.TypeLoadException e)
				{
					// FIXME: maybe not the best way to handle this
					throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
				}
			}

			/// <summary>A utility method to always raise an exeption.</summary>
			private void ConstraintViolated(NBCEL.generic.Instruction i, string message)
			{
				throw new NBCEL.verifier.exc.StaticCodeInstructionOperandConstraintException("Instruction "
					 + i + " constraint violated: " + message);
			}

			/// <summary>
			/// A utility method to raise an exception if the index is not
			/// a valid constant pool index.
			/// </summary>
			private void IndexValid(NBCEL.generic.Instruction i, int idx)
			{
				if (idx < 0 || idx >= this.constantPoolGen.GetSize())
				{
					this.ConstraintViolated(i, "Illegal constant pool index '" + idx + "'.");
				}
			}

			///////////////////////////////////////////////////////////
			// The Java Virtual Machine Specification, pages 134-137 //
			///////////////////////////////////////////////////////////
			/// <summary>Assures the generic preconditions of a LoadClass instance.</summary>
			/// <remarks>
			/// Assures the generic preconditions of a LoadClass instance.
			/// The referenced class is loaded and pass2-verified.
			/// </remarks>
			public override void VisitLoadClass(NBCEL.generic.LoadClass loadClass)
			{
				NBCEL.generic.ObjectType t = loadClass.GetLoadClassType(this.constantPoolGen);
				if (t != null)
				{
					// null means "no class is loaded"
					NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(t.GetClassName
						());
					NBCEL.verifier.VerificationResult vr = v.DoPass1();
					if (vr.GetStatus() != NBCEL.verifier.VerificationResult.VERIFIED_OK)
					{
						this.ConstraintViolated((NBCEL.generic.Instruction)loadClass, "Class '" + loadClass
							.GetLoadClassType(this.constantPoolGen).GetClassName() + "' is referenced, but cannot be loaded: '"
							 + vr + "'.");
					}
				}
			}

			// The target of each jump and branch instruction [...] must be the opcode [...]
			// BCEL _DOES_ handle this.
			// tableswitch: BCEL will do it, supposedly.
			// lookupswitch: BCEL will do it, supposedly.
			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitLDC(NBCEL.generic.LDC ldc)
			{
				// LDC and LDC_W (LDC_W is a subclass of LDC in BCEL's model)
				this.IndexValid(ldc, ldc.GetIndex());
				NBCEL.classfile.Constant c = this.constantPoolGen.GetConstant(ldc.GetIndex());
				if (c is NBCEL.classfile.ConstantClass)
				{
					this._enclosing.AddMessage("Operand of LDC or LDC_W is CONSTANT_Class '" + c + "' - this is only supported in JDK 1.5 and higher."
						);
				}
				else if (!((c is NBCEL.classfile.ConstantInteger) || (c is NBCEL.classfile.ConstantFloat
					) || (c is NBCEL.classfile.ConstantString)))
				{
					this.ConstraintViolated(ldc, "Operand of LDC or LDC_W must be one of CONSTANT_Integer, CONSTANT_Float or CONSTANT_String, but is '"
						 + c + "'.");
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitLDC2_W(NBCEL.generic.LDC2_W o)
			{
				// LDC2_W
				this.IndexValid(o, o.GetIndex());
				NBCEL.classfile.Constant c = this.constantPoolGen.GetConstant(o.GetIndex());
				if (!((c is NBCEL.classfile.ConstantLong) || (c is NBCEL.classfile.ConstantDouble
					)))
				{
					this.ConstraintViolated(o, "Operand of LDC2_W must be CONSTANT_Long or CONSTANT_Double, but is '"
						 + c + "'.");
				}
				try
				{
					this.IndexValid(o, o.GetIndex() + 1);
				}
				catch (NBCEL.verifier.exc.StaticCodeInstructionOperandConstraintException e)
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("OOPS: Does not BCEL handle that? LDC2_W operand has a problem."
						, e);
				}
			}

			private NBCEL.generic.ObjectType GetObjectType(NBCEL.generic.FieldInstruction o)
			{
				NBCEL.generic.ReferenceType rt = o.GetReferenceType(this.constantPoolGen);
				if (rt is NBCEL.generic.ObjectType)
				{
					return (NBCEL.generic.ObjectType)rt;
				}
				this.ConstraintViolated(o, "expecting ObjectType but got " + rt);
				return null;
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitFieldInstruction(NBCEL.generic.FieldInstruction o)
			{
				//getfield, putfield, getstatic, putstatic
				try
				{
					this.IndexValid(o, o.GetIndex());
					NBCEL.classfile.Constant c = this.constantPoolGen.GetConstant(o.GetIndex());
					if (!(c is NBCEL.classfile.ConstantFieldref))
					{
						this.ConstraintViolated(o, "Indexing a constant that's not a CONSTANT_Fieldref but a '"
							 + c + "'.");
					}
					string field_name = o.GetFieldName(this.constantPoolGen);
					NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(this.GetObjectType(o)
						.GetClassName());
					NBCEL.classfile.Field[] fields = jc.GetFields();
					NBCEL.classfile.Field f = null;
					foreach (NBCEL.classfile.Field field in fields)
					{
						if (field.GetName().Equals(field_name))
						{
							NBCEL.generic.Type f_type = NBCEL.generic.Type.GetType(field.GetSignature());
							NBCEL.generic.Type o_type = o.GetType(this.constantPoolGen);
							/* TODO: Check if assignment compatibility is sufficient.
							* What does Sun do?
							*/
							if (f_type.Equals(o_type))
							{
								f = field;
								break;
							}
						}
					}
					if (f == null)
					{
						NBCEL.classfile.JavaClass[] superclasses = jc.GetSuperClasses();
						foreach (NBCEL.classfile.JavaClass superclass in superclasses)
						{
							fields = superclass.GetFields();
							foreach (NBCEL.classfile.Field field in fields)
							{
								if (field.GetName().Equals(field_name))
								{
									NBCEL.generic.Type f_type = NBCEL.generic.Type.GetType(field.GetSignature());
									NBCEL.generic.Type o_type = o.GetType(this.constantPoolGen);
									if (f_type.Equals(o_type))
									{
										f = field;
										if ((f.GetAccessFlags() & (NBCEL.Const.ACC_PUBLIC | NBCEL.Const.ACC_PROTECTED)) ==
											 0)
										{
											f = null;
										}
										goto outer_break;
									}
								}
							}
						}
outer_break: ;
						if (f == null)
						{
							this.ConstraintViolated(o, "Referenced field '" + field_name + "' does not exist in class '"
								 + jc.GetClassName() + "'.");
						}
					}
					else
					{
						/* TODO: Check if assignment compatibility is sufficient.
						What does Sun do? */
						NBCEL.generic.Type.GetType(f.GetSignature());
						o.GetType(this.constantPoolGen);
					}
				}
				catch (System.TypeLoadException e)
				{
					//                Type f_type = Type.getType(f.getSignature());
					//                Type o_type = o.getType(cpg);
					// Argh. Sun's implementation allows us to have multiple fields of
					// the same name but with a different signature.
					//if (! f_type.equals(o_type)) {
					//    constraintViolated(o,
					//        "Referenced field '"+field_name+"' has type '"+f_type+"' instead of '"+o_type+"' as expected.");
					//}
					/* TODO: Check for access modifiers here. */
					// FIXME: maybe not the best way to handle this
					throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitInvokeInstruction(NBCEL.generic.InvokeInstruction o)
			{
				this.IndexValid(o, o.GetIndex());
				if ((o is NBCEL.generic.INVOKEVIRTUAL) || (o is NBCEL.generic.INVOKESPECIAL) || (
					o is NBCEL.generic.INVOKESTATIC))
				{
					NBCEL.classfile.Constant c = this.constantPoolGen.GetConstant(o.GetIndex());
					if (!(c is NBCEL.classfile.ConstantMethodref))
					{
						this.ConstraintViolated(o, "Indexing a constant that's not a CONSTANT_Methodref but a '"
							 + c + "'.");
					}
					else
					{
						// Constants are okay due to pass2.
						NBCEL.classfile.ConstantNameAndType cnat = (NBCEL.classfile.ConstantNameAndType)(
							this.constantPoolGen.GetConstant(((NBCEL.classfile.ConstantMethodref)c).GetNameAndTypeIndex
							()));
						NBCEL.classfile.ConstantUtf8 cutf8 = (NBCEL.classfile.ConstantUtf8)(this.constantPoolGen
							.GetConstant(cnat.GetNameIndex()));
						if (cutf8.GetBytes().Equals(NBCEL.Const.CONSTRUCTOR_NAME) && (!(o is NBCEL.generic.INVOKESPECIAL
							)))
						{
							this.ConstraintViolated(o, "Only INVOKESPECIAL is allowed to invoke instance initialization methods."
								);
						}
						if ((!(cutf8.GetBytes().Equals(NBCEL.Const.CONSTRUCTOR_NAME))) && (cutf8.GetBytes
							().StartsWith("<")))
						{
							this.ConstraintViolated(o, "No method with a name beginning with '<' other than the instance initialization methods"
								 + " may be called by the method invocation instructions.");
						}
					}
				}
				else
				{
					//if (o instanceof INVOKEINTERFACE) {
					NBCEL.classfile.Constant c = this.constantPoolGen.GetConstant(o.GetIndex());
					if (!(c is NBCEL.classfile.ConstantInterfaceMethodref))
					{
						this.ConstraintViolated(o, "Indexing a constant that's not a CONSTANT_InterfaceMethodref but a '"
							 + c + "'.");
					}
					// TODO: From time to time check if BCEL allows to detect if the
					// 'count' operand is consistent with the information in the
					// CONSTANT_InterfaceMethodref and if the last operand is zero.
					// By now, BCEL hides those two operands because they're superfluous.
					// Invoked method must not be <init> or <clinit>
					NBCEL.classfile.ConstantNameAndType cnat = (NBCEL.classfile.ConstantNameAndType)(
						this.constantPoolGen.GetConstant(((NBCEL.classfile.ConstantInterfaceMethodref)c)
						.GetNameAndTypeIndex()));
					string name = ((NBCEL.classfile.ConstantUtf8)(this.constantPoolGen.GetConstant(cnat
						.GetNameIndex()))).GetBytes();
					if (name.Equals(NBCEL.Const.CONSTRUCTOR_NAME))
					{
						this.ConstraintViolated(o, "Method to invoke must not be '" + NBCEL.Const.CONSTRUCTOR_NAME
							 + "'.");
					}
					if (name.Equals(NBCEL.Const.STATIC_INITIALIZER_NAME))
					{
						this.ConstraintViolated(o, "Method to invoke must not be '" + NBCEL.Const.STATIC_INITIALIZER_NAME
							 + "'.");
					}
				}
				// The LoadClassType is the method-declaring class, so we have to check the other types.
				NBCEL.generic.Type t = o.GetReturnType(this.constantPoolGen);
				if (t is NBCEL.generic.ArrayType)
				{
					t = ((NBCEL.generic.ArrayType)t).GetBasicType();
				}
				if (t is NBCEL.generic.ObjectType)
				{
					NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(((NBCEL.generic.ObjectType
						)t).GetClassName());
					NBCEL.verifier.VerificationResult vr = v.DoPass2();
					if (vr.GetStatus() != NBCEL.verifier.VerificationResult.VERIFIED_OK)
					{
						this.ConstraintViolated(o, "Return type class/interface could not be verified successfully: '"
							 + vr.GetMessage() + "'.");
					}
				}
				NBCEL.generic.Type[] ts = o.GetArgumentTypes(this.constantPoolGen);
				foreach (NBCEL.generic.Type element in ts)
				{
					t = element;
					if (t is NBCEL.generic.ArrayType)
					{
						t = ((NBCEL.generic.ArrayType)t).GetBasicType();
					}
					if (t is NBCEL.generic.ObjectType)
					{
						NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(((NBCEL.generic.ObjectType
							)t).GetClassName());
						NBCEL.verifier.VerificationResult vr = v.DoPass2();
						if (vr.GetStatus() != NBCEL.verifier.VerificationResult.VERIFIED_OK)
						{
							this.ConstraintViolated(o, "Argument type class/interface could not be verified successfully: '"
								 + vr.GetMessage() + "'.");
						}
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitINSTANCEOF(NBCEL.generic.INSTANCEOF o)
			{
				this.IndexValid(o, o.GetIndex());
				NBCEL.classfile.Constant c = this.constantPoolGen.GetConstant(o.GetIndex());
				if (!(c is NBCEL.classfile.ConstantClass))
				{
					this.ConstraintViolated(o, "Expecting a CONSTANT_Class operand, but found a '" + 
						c + "'.");
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitCHECKCAST(NBCEL.generic.CHECKCAST o)
			{
				this.IndexValid(o, o.GetIndex());
				NBCEL.classfile.Constant c = this.constantPoolGen.GetConstant(o.GetIndex());
				if (!(c is NBCEL.classfile.ConstantClass))
				{
					this.ConstraintViolated(o, "Expecting a CONSTANT_Class operand, but found a '" + 
						c + "'.");
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitNEW(NBCEL.generic.NEW o)
			{
				this.IndexValid(o, o.GetIndex());
				NBCEL.classfile.Constant c = this.constantPoolGen.GetConstant(o.GetIndex());
				if (!(c is NBCEL.classfile.ConstantClass))
				{
					this.ConstraintViolated(o, "Expecting a CONSTANT_Class operand, but found a '" + 
						c + "'.");
				}
				else
				{
					NBCEL.classfile.ConstantUtf8 cutf8 = (NBCEL.classfile.ConstantUtf8)(this.constantPoolGen
						.GetConstant(((NBCEL.classfile.ConstantClass)c).GetNameIndex()));
					NBCEL.generic.Type t = NBCEL.generic.Type.GetType("L" + cutf8.GetBytes() + ";");
					if (t is NBCEL.generic.ArrayType)
					{
						this.ConstraintViolated(o, "NEW must not be used to create an array.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitMULTIANEWARRAY(NBCEL.generic.MULTIANEWARRAY o)
			{
				this.IndexValid(o, o.GetIndex());
				NBCEL.classfile.Constant c = this.constantPoolGen.GetConstant(o.GetIndex());
				if (!(c is NBCEL.classfile.ConstantClass))
				{
					this.ConstraintViolated(o, "Expecting a CONSTANT_Class operand, but found a '" + 
						c + "'.");
				}
				int dimensions2create = o.GetDimensions();
				if (dimensions2create < 1)
				{
					this.ConstraintViolated(o, "Number of dimensions to create must be greater than zero."
						);
				}
				NBCEL.generic.Type t = o.GetType(this.constantPoolGen);
				if (t is NBCEL.generic.ArrayType)
				{
					int dimensions = ((NBCEL.generic.ArrayType)t).GetDimensions();
					if (dimensions < dimensions2create)
					{
						this.ConstraintViolated(o, "Not allowed to create array with more dimensions ('" 
							+ dimensions2create + "') than the one referenced by the CONSTANT_Class '" + t +
							 "'.");
					}
				}
				else
				{
					this.ConstraintViolated(o, "Expecting a CONSTANT_Class referencing an array type."
						 + " [Constraint not found in The Java Virtual Machine Specification, Second Edition, 4.8.1]"
						);
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitANEWARRAY(NBCEL.generic.ANEWARRAY o)
			{
				this.IndexValid(o, o.GetIndex());
				NBCEL.classfile.Constant c = this.constantPoolGen.GetConstant(o.GetIndex());
				if (!(c is NBCEL.classfile.ConstantClass))
				{
					this.ConstraintViolated(o, "Expecting a CONSTANT_Class operand, but found a '" + 
						c + "'.");
				}
				NBCEL.generic.Type t = o.GetType(this.constantPoolGen);
				if (t is NBCEL.generic.ArrayType)
				{
					int dimensions = ((NBCEL.generic.ArrayType)t).GetDimensions();
					if (dimensions > NBCEL.Const.MAX_ARRAY_DIMENSIONS)
					{
						this.ConstraintViolated(o, "Not allowed to create an array with more than " + NBCEL.Const
							.MAX_ARRAY_DIMENSIONS + " dimensions;" + " actual: " + dimensions);
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitNEWARRAY(NBCEL.generic.NEWARRAY o)
			{
				byte t = o.GetTypecode();
				if (!((t == NBCEL.Const.T_BOOLEAN) || (t == NBCEL.Const.T_CHAR) || (t == NBCEL.Const
					.T_FLOAT) || (t == NBCEL.Const.T_DOUBLE) || (t == NBCEL.Const.T_BYTE) || (t == NBCEL.Const
					.T_SHORT) || (t == NBCEL.Const.T_INT) || (t == NBCEL.Const.T_LONG)))
				{
					this.ConstraintViolated(o, "Illegal type code '+t+' for 'atype' operand.");
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitILOAD(NBCEL.generic.ILOAD o)
			{
				int idx = o.GetIndex();
				if (idx < 0)
				{
					this.ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
				}
				else
				{
					int maxminus1 = this.Max_locals() - 1;
					if (idx > maxminus1)
					{
						this.ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
							 + maxminus1 + "'.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitFLOAD(NBCEL.generic.FLOAD o)
			{
				int idx = o.GetIndex();
				if (idx < 0)
				{
					this.ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
				}
				else
				{
					int maxminus1 = this.Max_locals() - 1;
					if (idx > maxminus1)
					{
						this.ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
							 + maxminus1 + "'.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitALOAD(NBCEL.generic.ALOAD o)
			{
				int idx = o.GetIndex();
				if (idx < 0)
				{
					this.ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
				}
				else
				{
					int maxminus1 = this.Max_locals() - 1;
					if (idx > maxminus1)
					{
						this.ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
							 + maxminus1 + "'.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitISTORE(NBCEL.generic.ISTORE o)
			{
				int idx = o.GetIndex();
				if (idx < 0)
				{
					this.ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
				}
				else
				{
					int maxminus1 = this.Max_locals() - 1;
					if (idx > maxminus1)
					{
						this.ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
							 + maxminus1 + "'.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitFSTORE(NBCEL.generic.FSTORE o)
			{
				int idx = o.GetIndex();
				if (idx < 0)
				{
					this.ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
				}
				else
				{
					int maxminus1 = this.Max_locals() - 1;
					if (idx > maxminus1)
					{
						this.ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
							 + maxminus1 + "'.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitASTORE(NBCEL.generic.ASTORE o)
			{
				int idx = o.GetIndex();
				if (idx < 0)
				{
					this.ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
				}
				else
				{
					int maxminus1 = this.Max_locals() - 1;
					if (idx > maxminus1)
					{
						this.ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
							 + maxminus1 + "'.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitIINC(NBCEL.generic.IINC o)
			{
				int idx = o.GetIndex();
				if (idx < 0)
				{
					this.ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
				}
				else
				{
					int maxminus1 = this.Max_locals() - 1;
					if (idx > maxminus1)
					{
						this.ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
							 + maxminus1 + "'.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitRET(NBCEL.generic.RET o)
			{
				int idx = o.GetIndex();
				if (idx < 0)
				{
					this.ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
				}
				else
				{
					int maxminus1 = this.Max_locals() - 1;
					if (idx > maxminus1)
					{
						this.ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
							 + maxminus1 + "'.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitLLOAD(NBCEL.generic.LLOAD o)
			{
				int idx = o.GetIndex();
				if (idx < 0)
				{
					this.ConstraintViolated(o, "Index '" + idx + "' must be non-negative." + " [Constraint by JustIce as an analogon to the single-slot xLOAD/xSTORE instructions; may not happen anyway.]"
						);
				}
				else
				{
					int maxminus2 = this.Max_locals() - 2;
					if (idx > maxminus2)
					{
						this.ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-2 '"
							 + maxminus2 + "'.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitDLOAD(NBCEL.generic.DLOAD o)
			{
				int idx = o.GetIndex();
				if (idx < 0)
				{
					this.ConstraintViolated(o, "Index '" + idx + "' must be non-negative." + " [Constraint by JustIce as an analogon to the single-slot xLOAD/xSTORE instructions; may not happen anyway.]"
						);
				}
				else
				{
					int maxminus2 = this.Max_locals() - 2;
					if (idx > maxminus2)
					{
						this.ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-2 '"
							 + maxminus2 + "'.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitLSTORE(NBCEL.generic.LSTORE o)
			{
				int idx = o.GetIndex();
				if (idx < 0)
				{
					this.ConstraintViolated(o, "Index '" + idx + "' must be non-negative." + " [Constraint by JustIce as an analogon to the single-slot xLOAD/xSTORE instructions; may not happen anyway.]"
						);
				}
				else
				{
					int maxminus2 = this.Max_locals() - 2;
					if (idx > maxminus2)
					{
						this.ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-2 '"
							 + maxminus2 + "'.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitDSTORE(NBCEL.generic.DSTORE o)
			{
				int idx = o.GetIndex();
				if (idx < 0)
				{
					this.ConstraintViolated(o, "Index '" + idx + "' must be non-negative." + " [Constraint by JustIce as an analogon to the single-slot xLOAD/xSTORE instructions; may not happen anyway.]"
						);
				}
				else
				{
					int maxminus2 = this.Max_locals() - 2;
					if (idx > maxminus2)
					{
						this.ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-2 '"
							 + maxminus2 + "'.");
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitLOOKUPSWITCH(NBCEL.generic.LOOKUPSWITCH o)
			{
				int[] matchs = o.GetMatchs();
				int max = int.MinValue;
				for (int i = 0; i < matchs.Length; i++)
				{
					if (matchs[i] == max && i != 0)
					{
						this.ConstraintViolated(o, "Match '" + matchs[i] + "' occurs more than once.");
					}
					if (matchs[i] < max)
					{
						this.ConstraintViolated(o, "Lookup table must be sorted but isn't.");
					}
					else
					{
						max = matchs[i];
					}
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitTABLESWITCH(NBCEL.generic.TABLESWITCH o)
			{
			}

			// "high" must be >= "low". We cannot check this, as BCEL hides
			// it from us.
			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitPUTSTATIC(NBCEL.generic.PUTSTATIC o)
			{
				try
				{
					string field_name = o.GetFieldName(this.constantPoolGen);
					NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(this.GetObjectType(o)
						.GetClassName());
					NBCEL.classfile.Field[] fields = jc.GetFields();
					NBCEL.classfile.Field f = null;
					foreach (NBCEL.classfile.Field field in fields)
					{
						if (field.GetName().Equals(field_name))
						{
							f = field;
							break;
						}
					}
					if (f == null)
					{
						throw new NBCEL.verifier.exc.AssertionViolatedException("Field '" + field_name + 
							"' not found in " + jc.GetClassName());
					}
					if (f.IsFinal())
					{
						if (!(this._enclosing.myOwner.GetClassName().Equals(this.GetObjectType(o).GetClassName
							())))
						{
							this.ConstraintViolated(o, "Referenced field '" + f + "' is final and must therefore be declared in the current class '"
								 + this._enclosing.myOwner.GetClassName() + "' which is not the case: it is declared in '"
								 + o.GetReferenceType(this.constantPoolGen) + "'.");
						}
					}
					if (!(f.IsStatic()))
					{
						this.ConstraintViolated(o, "Referenced field '" + f + "' is not static which it should be."
							);
					}
					string meth_name = NBCEL.Repository.LookupClass(this._enclosing.myOwner.GetClassName
						()).GetMethods()[this._enclosing.method_no].GetName();
					// If it's an interface, it can be set only in <clinit>.
					if ((!(jc.IsClass())) && (!(meth_name.Equals(NBCEL.Const.STATIC_INITIALIZER_NAME)
						)))
					{
						this.ConstraintViolated(o, "Interface field '" + f + "' must be set in a '" + NBCEL.Const
							.STATIC_INITIALIZER_NAME + "' method.");
					}
				}
				catch (System.TypeLoadException e)
				{
					// FIXME: maybe not the best way to handle this
					throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitGETSTATIC(NBCEL.generic.GETSTATIC o)
			{
				try
				{
					string field_name = o.GetFieldName(this.constantPoolGen);
					NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(this.GetObjectType(o)
						.GetClassName());
					NBCEL.classfile.Field[] fields = jc.GetFields();
					NBCEL.classfile.Field f = null;
					foreach (NBCEL.classfile.Field field in fields)
					{
						if (field.GetName().Equals(field_name))
						{
							f = field;
							break;
						}
					}
					if (f == null)
					{
						throw new NBCEL.verifier.exc.AssertionViolatedException("Field '" + field_name + 
							"' not found in " + jc.GetClassName());
					}
					if (!(f.IsStatic()))
					{
						this.ConstraintViolated(o, "Referenced field '" + f + "' is not static which it should be."
							);
					}
				}
				catch (System.TypeLoadException e)
				{
					// FIXME: maybe not the best way to handle this
					throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
				}
			}

			/* Checks if the constraints of operands of the said instruction(s) are satisfied. */
			//public void visitPUTFIELD(PUTFIELD o) {
			// for performance reasons done in Pass 3b
			//}
			/* Checks if the constraints of operands of the said instruction(s) are satisfied. */
			//public void visitGETFIELD(GETFIELD o) {
			// for performance reasons done in Pass 3b
			//}
			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitINVOKEDYNAMIC(NBCEL.generic.INVOKEDYNAMIC o)
			{
				throw new System.Exception("INVOKEDYNAMIC instruction is not supported at this time"
					);
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitINVOKEINTERFACE(NBCEL.generic.INVOKEINTERFACE o)
			{
				try
				{
					// INVOKEINTERFACE is a LoadClass; the Class where the referenced method is declared in,
					// is therefore resolved/verified.
					// INVOKEINTERFACE is an InvokeInstruction, the argument and return types are resolved/verified,
					// too. So are the allowed method names.
					string classname = o.GetClassName(this.constantPoolGen);
					NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(classname);
					NBCEL.classfile.Method m = this.GetMethodRecursive(jc, o);
					if (m == null)
					{
						this.ConstraintViolated(o, "Referenced method '" + o.GetMethodName(this.constantPoolGen
							) + "' with expected signature '" + o.GetSignature(this.constantPoolGen) + "' not found in class '"
							 + jc.GetClassName() + "'.");
					}
					if (jc.IsClass())
					{
						this.ConstraintViolated(o, "Referenced class '" + jc.GetClassName() + "' is a class, but not an interface as expected."
							);
					}
				}
				catch (System.TypeLoadException e)
				{
					// FIXME: maybe not the best way to handle this
					throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
				}
			}

			/// <summary>
			/// Looks for the method referenced by the given invoke instruction in the given class
			/// or its super classes and super interfaces.
			/// </summary>
			/// <param name="jc">the class that defines the referenced method</param>
			/// <param name="invoke">the instruction that references the method</param>
			/// <returns>the referenced method or null if not found.</returns>
			/// <exception cref="System.TypeLoadException"/>
			private NBCEL.classfile.Method GetMethodRecursive(NBCEL.classfile.JavaClass jc, NBCEL.generic.InvokeInstruction
				 invoke)
			{
				NBCEL.classfile.Method m;
				//look in the given class
				m = this.GetMethod(jc, invoke);
				if (m != null)
				{
					//method found in given class
					return m;
				}
				//method not found, look in super classes
				foreach (NBCEL.classfile.JavaClass superclass in jc.GetSuperClasses())
				{
					m = this.GetMethod(superclass, invoke);
					if (m != null)
					{
						//method found in super class
						return m;
					}
				}
				//method not found, look in super interfaces
				foreach (NBCEL.classfile.JavaClass superclass in jc.GetInterfaces())
				{
					m = this.GetMethod(superclass, invoke);
					if (m != null)
					{
						//method found in super interface
						return m;
					}
				}
				//method not found in the hierarchy
				return null;
			}

			/// <summary>Looks for the method referenced by the given invoke instruction in the given class.
			/// 	</summary>
			/// <param name="jc">the class that defines the referenced method</param>
			/// <param name="invoke">the instruction that references the method</param>
			/// <returns>the referenced method or null if not found.</returns>
			private NBCEL.classfile.Method GetMethod(NBCEL.classfile.JavaClass jc, NBCEL.generic.InvokeInstruction
				 invoke)
			{
				NBCEL.classfile.Method[] ms = jc.GetMethods();
				foreach (NBCEL.classfile.Method element in ms)
				{
					if ((element.GetName().Equals(invoke.GetMethodName(this.constantPoolGen))) && (NBCEL.generic.Type
						.GetReturnType(element.GetSignature()).Equals(invoke.GetReturnType(this.constantPoolGen
						))) && (this.Objarrayequals(NBCEL.generic.Type.GetArgumentTypes(element.GetSignature
						()), invoke.GetArgumentTypes(this.constantPoolGen))))
					{
						return element;
					}
				}
				return null;
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitINVOKESPECIAL(NBCEL.generic.INVOKESPECIAL o)
			{
				try
				{
					// INVOKESPECIAL is a LoadClass; the Class where the referenced method is declared in,
					// is therefore resolved/verified.
					// INVOKESPECIAL is an InvokeInstruction, the argument and return types are resolved/verified,
					// too. So are the allowed method names.
					string classname = o.GetClassName(this.constantPoolGen);
					NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(classname);
					NBCEL.classfile.Method m = this.GetMethodRecursive(jc, o);
					if (m == null)
					{
						this.ConstraintViolated(o, "Referenced method '" + o.GetMethodName(this.constantPoolGen
							) + "' with expected signature '" + o.GetSignature(this.constantPoolGen) + "' not found in class '"
							 + jc.GetClassName() + "'.");
					}
					NBCEL.classfile.JavaClass current = NBCEL.Repository.LookupClass(this._enclosing.
						myOwner.GetClassName());
					if (current.IsSuper())
					{
						if ((NBCEL.Repository.InstanceOf(current, jc)) && (!current.Equals(jc)))
						{
							if (!(o.GetMethodName(this.constantPoolGen).Equals(NBCEL.Const.CONSTRUCTOR_NAME)))
							{
								// Special lookup procedure for ACC_SUPER classes.
								int supidx = -1;
								NBCEL.classfile.Method meth = null;
								while (supidx != 0)
								{
									supidx = current.GetSuperclassNameIndex();
									current = NBCEL.Repository.LookupClass(current.GetSuperclassName());
									NBCEL.classfile.Method[] meths = current.GetMethods();
									foreach (NBCEL.classfile.Method meth2 in meths)
									{
										if ((meth2.GetName().Equals(o.GetMethodName(this.constantPoolGen))) && (NBCEL.generic.Type
											.GetReturnType(meth2.GetSignature()).Equals(o.GetReturnType(this.constantPoolGen
											))) && (this.Objarrayequals(NBCEL.generic.Type.GetArgumentTypes(meth2.GetSignature
											()), o.GetArgumentTypes(this.constantPoolGen))))
										{
											meth = meth2;
											break;
										}
									}
									if (meth != null)
									{
										break;
									}
								}
								if (meth == null)
								{
									this.ConstraintViolated(o, "ACC_SUPER special lookup procedure not successful: method '"
										 + o.GetMethodName(this.constantPoolGen) + "' with proper signature not declared in superclass hierarchy."
										);
								}
							}
						}
					}
				}
				catch (System.TypeLoadException e)
				{
					// FIXME: maybe not the best way to handle this
					throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitINVOKESTATIC(NBCEL.generic.INVOKESTATIC o)
			{
				try
				{
					// INVOKESTATIC is a LoadClass; the Class where the referenced method is declared in,
					// is therefore resolved/verified.
					// INVOKESTATIC is an InvokeInstruction, the argument and return types are resolved/verified,
					// too. So are the allowed method names.
					string classname = o.GetClassName(this.constantPoolGen);
					NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(classname);
					NBCEL.classfile.Method m = this.GetMethodRecursive(jc, o);
					if (m == null)
					{
						this.ConstraintViolated(o, "Referenced method '" + o.GetMethodName(this.constantPoolGen
							) + "' with expected signature '" + o.GetSignature(this.constantPoolGen) + "' not found in class '"
							 + jc.GetClassName() + "'.");
					}
					else if (!(m.IsStatic()))
					{
						// implies it's not abstract, verified in pass 2.
						this.ConstraintViolated(o, "Referenced method '" + o.GetMethodName(this.constantPoolGen
							) + "' has ACC_STATIC unset.");
					}
				}
				catch (System.TypeLoadException e)
				{
					// FIXME: maybe not the best way to handle this
					throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
				}
			}

			/// <summary>Checks if the constraints of operands of the said instruction(s) are satisfied.
			/// 	</summary>
			public override void VisitINVOKEVIRTUAL(NBCEL.generic.INVOKEVIRTUAL o)
			{
				try
				{
					// INVOKEVIRTUAL is a LoadClass; the Class where the referenced method is declared in,
					// is therefore resolved/verified.
					// INVOKEVIRTUAL is an InvokeInstruction, the argument and return types are resolved/verified,
					// too. So are the allowed method names.
					string classname = o.GetClassName(this.constantPoolGen);
					NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(classname);
					NBCEL.classfile.Method m = this.GetMethodRecursive(jc, o);
					if (m == null)
					{
						this.ConstraintViolated(o, "Referenced method '" + o.GetMethodName(this.constantPoolGen
							) + "' with expected signature '" + o.GetSignature(this.constantPoolGen) + "' not found in class '"
							 + jc.GetClassName() + "'.");
					}
					if (!(jc.IsClass()))
					{
						this.ConstraintViolated(o, "Referenced class '" + jc.GetClassName() + "' is an interface, but not a class as expected."
							);
					}
				}
				catch (System.TypeLoadException e)
				{
					// FIXME: maybe not the best way to handle this
					throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
				}
			}

			// WIDE stuff is BCEL-internal and cannot be checked here.
			/// <summary>A utility method like equals(Object) for arrays.</summary>
			/// <remarks>
			/// A utility method like equals(Object) for arrays.
			/// The equality of the elements is based on their equals(Object)
			/// method instead of their object identity.
			/// </remarks>
			private bool Objarrayequals(object[] o, object[] p)
			{
				if (o.Length != p.Length)
				{
					return false;
				}
				for (int i = 0; i < o.Length; i++)
				{
					if (!(o[i].Equals(p[i])))
					{
						return false;
					}
				}
				return true;
			}

			private readonly Pass3aVerifier _enclosing;
		}
	}
}
