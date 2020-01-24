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
	/// This Visitor class may be used for a type-based Java Virtual Machine
	/// simulation.
	/// </summary>
	/// <remarks>
	/// This Visitor class may be used for a type-based Java Virtual Machine
	/// simulation.
	/// <p>It does not check for correct types on the OperandStack or in the
	/// LocalVariables; nor does it check their sizes are sufficiently big.
	/// Thus, to use this Visitor for bytecode verifying, you have to make sure
	/// externally that the type constraints of the Java Virtual Machine instructions
	/// are satisfied. An InstConstraintVisitor may be used for this.
	/// Anyway, this Visitor does not mandate it. For example, when you
	/// visitIADD(IADD o), then there are two stack slots popped and one
	/// stack slot containing a Type.INT is pushed (where you could also
	/// pop only one slot if you know there are two Type.INT on top of the
	/// stack). Monitor-specific behavior is not simulated.</p>
	/// <b>Conventions:</b>
	/// <p>Type.VOID will never be pushed onto the stack. Type.DOUBLE and Type.LONG
	/// that would normally take up two stack slots (like Double_HIGH and
	/// Double_LOW) are represented by a simple single Type.DOUBLE or Type.LONG
	/// object on the stack here.</p>
	/// <p>If a two-slot type is stored into a local variable, the next variable
	/// is given the type Type.UNKNOWN.</p>
	/// </remarks>
	/// <seealso cref="VisitDSTORE(NBCEL.generic.DSTORE)"/>
	/// <seealso cref="InstConstraintVisitor"/>
	public class ExecutionVisitor : NBCEL.generic.EmptyVisitor
	{
		/// <summary>The executionframe we're operating on.</summary>
		private NBCEL.verifier.structurals.Frame frame = null;

		/// <summary>The ConstantPoolGen we're working with.</summary>
		/// <seealso cref="SetConstantPoolGen(NBCEL.generic.ConstantPoolGen)"/>
		private NBCEL.generic.ConstantPoolGen cpg = null;

		/// <summary>Constructor.</summary>
		/// <remarks>Constructor. Constructs a new instance of this class.</remarks>
		public ExecutionVisitor()
		{
		}

		// CHECKSTYLE:OFF (there are lots of references!)
		//CHECKSTYLE:ON
		/// <summary>The OperandStack from the current Frame we're operating on.</summary>
		/// <seealso cref="SetFrame(Frame)"/>
		private NBCEL.verifier.structurals.OperandStack Stack()
		{
			return frame.GetStack();
		}

		/// <summary>The LocalVariables from the current Frame we're operating on.</summary>
		/// <seealso cref="SetFrame(Frame)"/>
		private NBCEL.verifier.structurals.LocalVariables Locals()
		{
			return frame.GetLocals();
		}

		/// <summary>Sets the ConstantPoolGen needed for symbolic execution.</summary>
		public virtual void SetConstantPoolGen(NBCEL.generic.ConstantPoolGen cpg)
		{
			// TODO could be package-protected?
			this.cpg = cpg;
		}

		/// <summary>
		/// The only method granting access to the single instance of
		/// the ExecutionVisitor class.
		/// </summary>
		/// <remarks>
		/// The only method granting access to the single instance of
		/// the ExecutionVisitor class. Before actively using this
		/// instance, <B>SET THE ConstantPoolGen FIRST</B>.
		/// </remarks>
		/// <seealso cref="SetConstantPoolGen(NBCEL.generic.ConstantPoolGen)"/>
		public virtual void SetFrame(NBCEL.verifier.structurals.Frame f)
		{
			// TODO could be package-protected?
			this.frame = f;
		}

		///** Symbolically executes the corresponding Java Virtual Machine instruction. */
		//public void visitWIDE(WIDE o) {
		// The WIDE instruction is modelled as a flag
		// of the embedded instructions in BCEL.
		// Therefore BCEL checks for possible errors
		// when parsing in the .class file: We don't
		// have even the possibilty to care for WIDE
		// here.
		//}
		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitAALOAD(NBCEL.generic.AALOAD o)
		{
			Stack().Pop();
			// pop the index int
			//System.out.print(stack().peek());
			NBCEL.generic.Type t = Stack().Pop();
			// Pop Array type
			if (t == NBCEL.generic.Type.NULL)
			{
				Stack().Push(NBCEL.generic.Type.NULL);
			}
			else
			{
				// Do nothing stackwise --- a NullPointerException is thrown at Run-Time
				NBCEL.generic.ArrayType at = (NBCEL.generic.ArrayType)t;
				Stack().Push(at.GetElementType());
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitAASTORE(NBCEL.generic.AASTORE o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitACONST_NULL(NBCEL.generic.ACONST_NULL o)
		{
			Stack().Push(NBCEL.generic.Type.NULL);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitALOAD(NBCEL.generic.ALOAD o)
		{
			Stack().Push(Locals().Get(o.GetIndex()));
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitANEWARRAY(NBCEL.generic.ANEWARRAY o)
		{
			Stack().Pop();
			//count
			Stack().Push(new NBCEL.generic.ArrayType(o.GetType(cpg), 1));
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitARETURN(NBCEL.generic.ARETURN o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitARRAYLENGTH(NBCEL.generic.ARRAYLENGTH o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitASTORE(NBCEL.generic.ASTORE o)
		{
			Locals().Set(o.GetIndex(), Stack().Pop());
		}

		//System.err.println("TODO-DEBUG:    set LV '"+o.getIndex()+"' to '"+locals().get(o.getIndex())+"'.");
		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitATHROW(NBCEL.generic.ATHROW o)
		{
			NBCEL.generic.Type t = Stack().Pop();
			Stack().Clear();
			if (t.Equals(NBCEL.generic.Type.NULL))
			{
				Stack().Push(NBCEL.generic.Type.GetType("Ljava/lang/NullPointerException;"));
			}
			else
			{
				Stack().Push(t);
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitBALOAD(NBCEL.generic.BALOAD o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitBASTORE(NBCEL.generic.BASTORE o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitBIPUSH(NBCEL.generic.BIPUSH o)
		{
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitCALOAD(NBCEL.generic.CALOAD o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitCASTORE(NBCEL.generic.CASTORE o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitCHECKCAST(NBCEL.generic.CHECKCAST o)
		{
			// It's possibly wrong to do so, but SUN's
			// ByteCode verifier seems to do (only) this, too.
			// TODO: One could use a sophisticated analysis here to check
			//       if a type cannot possibly be cated to another and by
			//       so doing predict the ClassCastException at run-time.
			Stack().Pop();
			Stack().Push(o.GetType(cpg));
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitD2F(NBCEL.generic.D2F o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.FLOAT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitD2I(NBCEL.generic.D2I o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitD2L(NBCEL.generic.D2L o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDADD(NBCEL.generic.DADD o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.DOUBLE);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDALOAD(NBCEL.generic.DALOAD o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.DOUBLE);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDASTORE(NBCEL.generic.DASTORE o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDCMPG(NBCEL.generic.DCMPG o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDCMPL(NBCEL.generic.DCMPL o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDCONST(NBCEL.generic.DCONST o)
		{
			Stack().Push(NBCEL.generic.Type.DOUBLE);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDDIV(NBCEL.generic.DDIV o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.DOUBLE);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDLOAD(NBCEL.generic.DLOAD o)
		{
			Stack().Push(NBCEL.generic.Type.DOUBLE);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDMUL(NBCEL.generic.DMUL o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.DOUBLE);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDNEG(NBCEL.generic.DNEG o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.DOUBLE);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDREM(NBCEL.generic.DREM o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.DOUBLE);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDRETURN(NBCEL.generic.DRETURN o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDSTORE(NBCEL.generic.DSTORE o)
		{
			Locals().Set(o.GetIndex(), Stack().Pop());
			Locals().Set(o.GetIndex() + 1, NBCEL.generic.Type.UNKNOWN);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDSUB(NBCEL.generic.DSUB o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.DOUBLE);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDUP(NBCEL.generic.DUP o)
		{
			NBCEL.generic.Type t = Stack().Pop();
			Stack().Push(t);
			Stack().Push(t);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDUP_X1(NBCEL.generic.DUP_X1 o)
		{
			NBCEL.generic.Type w1 = Stack().Pop();
			NBCEL.generic.Type w2 = Stack().Pop();
			Stack().Push(w1);
			Stack().Push(w2);
			Stack().Push(w1);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDUP_X2(NBCEL.generic.DUP_X2 o)
		{
			NBCEL.generic.Type w1 = Stack().Pop();
			NBCEL.generic.Type w2 = Stack().Pop();
			if (w2.GetSize() == 2)
			{
				Stack().Push(w1);
				Stack().Push(w2);
				Stack().Push(w1);
			}
			else
			{
				NBCEL.generic.Type w3 = Stack().Pop();
				Stack().Push(w1);
				Stack().Push(w3);
				Stack().Push(w2);
				Stack().Push(w1);
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDUP2(NBCEL.generic.DUP2 o)
		{
			NBCEL.generic.Type t = Stack().Pop();
			if (t.GetSize() == 2)
			{
				Stack().Push(t);
				Stack().Push(t);
			}
			else
			{
				// t.getSize() is 1
				NBCEL.generic.Type u = Stack().Pop();
				Stack().Push(u);
				Stack().Push(t);
				Stack().Push(u);
				Stack().Push(t);
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDUP2_X1(NBCEL.generic.DUP2_X1 o)
		{
			NBCEL.generic.Type t = Stack().Pop();
			if (t.GetSize() == 2)
			{
				NBCEL.generic.Type u = Stack().Pop();
				Stack().Push(t);
				Stack().Push(u);
				Stack().Push(t);
			}
			else
			{
				//t.getSize() is1
				NBCEL.generic.Type u = Stack().Pop();
				NBCEL.generic.Type v = Stack().Pop();
				Stack().Push(u);
				Stack().Push(t);
				Stack().Push(v);
				Stack().Push(u);
				Stack().Push(t);
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitDUP2_X2(NBCEL.generic.DUP2_X2 o)
		{
			NBCEL.generic.Type t = Stack().Pop();
			if (t.GetSize() == 2)
			{
				NBCEL.generic.Type u = Stack().Pop();
				if (u.GetSize() == 2)
				{
					Stack().Push(t);
					Stack().Push(u);
					Stack().Push(t);
				}
				else
				{
					NBCEL.generic.Type v = Stack().Pop();
					Stack().Push(t);
					Stack().Push(v);
					Stack().Push(u);
					Stack().Push(t);
				}
			}
			else
			{
				//t.getSize() is 1
				NBCEL.generic.Type u = Stack().Pop();
				NBCEL.generic.Type v = Stack().Pop();
				if (v.GetSize() == 2)
				{
					Stack().Push(u);
					Stack().Push(t);
					Stack().Push(v);
					Stack().Push(u);
					Stack().Push(t);
				}
				else
				{
					NBCEL.generic.Type w = Stack().Pop();
					Stack().Push(u);
					Stack().Push(t);
					Stack().Push(w);
					Stack().Push(v);
					Stack().Push(u);
					Stack().Push(t);
				}
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitF2D(NBCEL.generic.F2D o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.DOUBLE);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitF2I(NBCEL.generic.F2I o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitF2L(NBCEL.generic.F2L o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFADD(NBCEL.generic.FADD o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.FLOAT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFALOAD(NBCEL.generic.FALOAD o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.FLOAT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFASTORE(NBCEL.generic.FASTORE o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFCMPG(NBCEL.generic.FCMPG o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFCMPL(NBCEL.generic.FCMPL o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFCONST(NBCEL.generic.FCONST o)
		{
			Stack().Push(NBCEL.generic.Type.FLOAT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFDIV(NBCEL.generic.FDIV o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.FLOAT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFLOAD(NBCEL.generic.FLOAD o)
		{
			Stack().Push(NBCEL.generic.Type.FLOAT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFMUL(NBCEL.generic.FMUL o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.FLOAT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFNEG(NBCEL.generic.FNEG o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.FLOAT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFREM(NBCEL.generic.FREM o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.FLOAT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFRETURN(NBCEL.generic.FRETURN o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFSTORE(NBCEL.generic.FSTORE o)
		{
			Locals().Set(o.GetIndex(), Stack().Pop());
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitFSUB(NBCEL.generic.FSUB o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.FLOAT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitGETFIELD(NBCEL.generic.GETFIELD o)
		{
			Stack().Pop();
			NBCEL.generic.Type t = o.GetFieldType(cpg);
			if (t.Equals(NBCEL.generic.Type.BOOLEAN) || t.Equals(NBCEL.generic.Type.CHAR) || 
				t.Equals(NBCEL.generic.Type.BYTE) || t.Equals(NBCEL.generic.Type.SHORT))
			{
				t = NBCEL.generic.Type.INT;
			}
			Stack().Push(t);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitGETSTATIC(NBCEL.generic.GETSTATIC o)
		{
			NBCEL.generic.Type t = o.GetFieldType(cpg);
			if (t.Equals(NBCEL.generic.Type.BOOLEAN) || t.Equals(NBCEL.generic.Type.CHAR) || 
				t.Equals(NBCEL.generic.Type.BYTE) || t.Equals(NBCEL.generic.Type.SHORT))
			{
				t = NBCEL.generic.Type.INT;
			}
			Stack().Push(t);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitGOTO(NBCEL.generic.GOTO o)
		{
		}

		// no stack changes.
		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitGOTO_W(NBCEL.generic.GOTO_W o)
		{
		}

		// no stack changes.
		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitI2B(NBCEL.generic.I2B o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitI2C(NBCEL.generic.I2C o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitI2D(NBCEL.generic.I2D o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.DOUBLE);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitI2F(NBCEL.generic.I2F o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.FLOAT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitI2L(NBCEL.generic.I2L o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitI2S(NBCEL.generic.I2S o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIADD(NBCEL.generic.IADD o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIALOAD(NBCEL.generic.IALOAD o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIAND(NBCEL.generic.IAND o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIASTORE(NBCEL.generic.IASTORE o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitICONST(NBCEL.generic.ICONST o)
		{
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIDIV(NBCEL.generic.IDIV o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIF_ACMPEQ(NBCEL.generic.IF_ACMPEQ o)
		{
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIF_ACMPNE(NBCEL.generic.IF_ACMPNE o)
		{
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIF_ICMPEQ(NBCEL.generic.IF_ICMPEQ o)
		{
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIF_ICMPGE(NBCEL.generic.IF_ICMPGE o)
		{
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIF_ICMPGT(NBCEL.generic.IF_ICMPGT o)
		{
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIF_ICMPLE(NBCEL.generic.IF_ICMPLE o)
		{
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIF_ICMPLT(NBCEL.generic.IF_ICMPLT o)
		{
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIF_ICMPNE(NBCEL.generic.IF_ICMPNE o)
		{
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIFEQ(NBCEL.generic.IFEQ o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIFGE(NBCEL.generic.IFGE o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIFGT(NBCEL.generic.IFGT o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIFLE(NBCEL.generic.IFLE o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIFLT(NBCEL.generic.IFLT o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIFNE(NBCEL.generic.IFNE o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIFNONNULL(NBCEL.generic.IFNONNULL o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIFNULL(NBCEL.generic.IFNULL o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIINC(NBCEL.generic.IINC o)
		{
		}

		// stack is not changed.
		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitILOAD(NBCEL.generic.ILOAD o)
		{
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIMUL(NBCEL.generic.IMUL o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitINEG(NBCEL.generic.INEG o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitINSTANCEOF(NBCEL.generic.INSTANCEOF o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		/// <since>6.0</since>
		public override void VisitINVOKEDYNAMIC(NBCEL.generic.INVOKEDYNAMIC o)
		{
			for (int i = 0; i < o.GetArgumentTypes(cpg).Length; i++)
			{
				Stack().Pop();
			}
			// We are sure the invoked method will xRETURN eventually
			// We simulate xRETURNs functionality here because we
			// don't really "jump into" and simulate the invoked
			// method.
			if (o.GetReturnType(cpg) != NBCEL.generic.Type.VOID)
			{
				NBCEL.generic.Type t = o.GetReturnType(cpg);
				if (t.Equals(NBCEL.generic.Type.BOOLEAN) || t.Equals(NBCEL.generic.Type.CHAR) || 
					t.Equals(NBCEL.generic.Type.BYTE) || t.Equals(NBCEL.generic.Type.SHORT))
				{
					t = NBCEL.generic.Type.INT;
				}
				Stack().Push(t);
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitINVOKEINTERFACE(NBCEL.generic.INVOKEINTERFACE o)
		{
			Stack().Pop();
			//objectref
			for (int i = 0; i < o.GetArgumentTypes(cpg).Length; i++)
			{
				Stack().Pop();
			}
			// We are sure the invoked method will xRETURN eventually
			// We simulate xRETURNs functionality here because we
			// don't really "jump into" and simulate the invoked
			// method.
			if (o.GetReturnType(cpg) != NBCEL.generic.Type.VOID)
			{
				NBCEL.generic.Type t = o.GetReturnType(cpg);
				if (t.Equals(NBCEL.generic.Type.BOOLEAN) || t.Equals(NBCEL.generic.Type.CHAR) || 
					t.Equals(NBCEL.generic.Type.BYTE) || t.Equals(NBCEL.generic.Type.SHORT))
				{
					t = NBCEL.generic.Type.INT;
				}
				Stack().Push(t);
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitINVOKESPECIAL(NBCEL.generic.INVOKESPECIAL o)
		{
			if (o.GetMethodName(cpg).Equals(NBCEL.Const.CONSTRUCTOR_NAME))
			{
				NBCEL.verifier.structurals.UninitializedObjectType t = (NBCEL.verifier.structurals.UninitializedObjectType
					)Stack().Peek(o.GetArgumentTypes(cpg).Length);
				if (t == NBCEL.verifier.structurals.Frame.GetThis())
				{
					NBCEL.verifier.structurals.Frame.SetThis(null);
				}
				Stack().InitializeObject(t);
				Locals().InitializeObject(t);
			}
			Stack().Pop();
			//objectref
			for (int i = 0; i < o.GetArgumentTypes(cpg).Length; i++)
			{
				Stack().Pop();
			}
			// We are sure the invoked method will xRETURN eventually
			// We simulate xRETURNs functionality here because we
			// don't really "jump into" and simulate the invoked
			// method.
			if (o.GetReturnType(cpg) != NBCEL.generic.Type.VOID)
			{
				NBCEL.generic.Type t = o.GetReturnType(cpg);
				if (t.Equals(NBCEL.generic.Type.BOOLEAN) || t.Equals(NBCEL.generic.Type.CHAR) || 
					t.Equals(NBCEL.generic.Type.BYTE) || t.Equals(NBCEL.generic.Type.SHORT))
				{
					t = NBCEL.generic.Type.INT;
				}
				Stack().Push(t);
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitINVOKESTATIC(NBCEL.generic.INVOKESTATIC o)
		{
			for (int i = 0; i < o.GetArgumentTypes(cpg).Length; i++)
			{
				Stack().Pop();
			}
			// We are sure the invoked method will xRETURN eventually
			// We simulate xRETURNs functionality here because we
			// don't really "jump into" and simulate the invoked
			// method.
			if (o.GetReturnType(cpg) != NBCEL.generic.Type.VOID)
			{
				NBCEL.generic.Type t = o.GetReturnType(cpg);
				if (t.Equals(NBCEL.generic.Type.BOOLEAN) || t.Equals(NBCEL.generic.Type.CHAR) || 
					t.Equals(NBCEL.generic.Type.BYTE) || t.Equals(NBCEL.generic.Type.SHORT))
				{
					t = NBCEL.generic.Type.INT;
				}
				Stack().Push(t);
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitINVOKEVIRTUAL(NBCEL.generic.INVOKEVIRTUAL o)
		{
			Stack().Pop();
			//objectref
			for (int i = 0; i < o.GetArgumentTypes(cpg).Length; i++)
			{
				Stack().Pop();
			}
			// We are sure the invoked method will xRETURN eventually
			// We simulate xRETURNs functionality here because we
			// don't really "jump into" and simulate the invoked
			// method.
			if (o.GetReturnType(cpg) != NBCEL.generic.Type.VOID)
			{
				NBCEL.generic.Type t = o.GetReturnType(cpg);
				if (t.Equals(NBCEL.generic.Type.BOOLEAN) || t.Equals(NBCEL.generic.Type.CHAR) || 
					t.Equals(NBCEL.generic.Type.BYTE) || t.Equals(NBCEL.generic.Type.SHORT))
				{
					t = NBCEL.generic.Type.INT;
				}
				Stack().Push(t);
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIOR(NBCEL.generic.IOR o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIREM(NBCEL.generic.IREM o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIRETURN(NBCEL.generic.IRETURN o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitISHL(NBCEL.generic.ISHL o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitISHR(NBCEL.generic.ISHR o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitISTORE(NBCEL.generic.ISTORE o)
		{
			Locals().Set(o.GetIndex(), Stack().Pop());
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitISUB(NBCEL.generic.ISUB o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIUSHR(NBCEL.generic.IUSHR o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitIXOR(NBCEL.generic.IXOR o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitJSR(NBCEL.generic.JSR o)
		{
			Stack().Push(new NBCEL.generic.ReturnaddressType(o.PhysicalSuccessor()));
		}

		//System.err.println("TODO-----------:"+o.physicalSuccessor());
		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitJSR_W(NBCEL.generic.JSR_W o)
		{
			Stack().Push(new NBCEL.generic.ReturnaddressType(o.PhysicalSuccessor()));
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitL2D(NBCEL.generic.L2D o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.DOUBLE);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitL2F(NBCEL.generic.L2F o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.FLOAT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitL2I(NBCEL.generic.L2I o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLADD(NBCEL.generic.LADD o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLALOAD(NBCEL.generic.LALOAD o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLAND(NBCEL.generic.LAND o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLASTORE(NBCEL.generic.LASTORE o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLCMP(NBCEL.generic.LCMP o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLCONST(NBCEL.generic.LCONST o)
		{
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLDC(NBCEL.generic.LDC o)
		{
			NBCEL.classfile.Constant c = cpg.GetConstant(o.GetIndex());
			if (c is NBCEL.classfile.ConstantInteger)
			{
				Stack().Push(NBCEL.generic.Type.INT);
			}
			if (c is NBCEL.classfile.ConstantFloat)
			{
				Stack().Push(NBCEL.generic.Type.FLOAT);
			}
			if (c is NBCEL.classfile.ConstantString)
			{
				Stack().Push(NBCEL.generic.Type.STRING);
			}
			if (c is NBCEL.classfile.ConstantClass)
			{
				Stack().Push(NBCEL.generic.Type.CLASS);
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public virtual void VisitLDC_W(NBCEL.generic.LDC_W o)
		{
			NBCEL.classfile.Constant c = cpg.GetConstant(o.GetIndex());
			if (c is NBCEL.classfile.ConstantInteger)
			{
				Stack().Push(NBCEL.generic.Type.INT);
			}
			if (c is NBCEL.classfile.ConstantFloat)
			{
				Stack().Push(NBCEL.generic.Type.FLOAT);
			}
			if (c is NBCEL.classfile.ConstantString)
			{
				Stack().Push(NBCEL.generic.Type.STRING);
			}
			if (c is NBCEL.classfile.ConstantClass)
			{
				Stack().Push(NBCEL.generic.Type.CLASS);
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLDC2_W(NBCEL.generic.LDC2_W o)
		{
			NBCEL.classfile.Constant c = cpg.GetConstant(o.GetIndex());
			if (c is NBCEL.classfile.ConstantLong)
			{
				Stack().Push(NBCEL.generic.Type.LONG);
			}
			if (c is NBCEL.classfile.ConstantDouble)
			{
				Stack().Push(NBCEL.generic.Type.DOUBLE);
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLDIV(NBCEL.generic.LDIV o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLLOAD(NBCEL.generic.LLOAD o)
		{
			Stack().Push(Locals().Get(o.GetIndex()));
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLMUL(NBCEL.generic.LMUL o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLNEG(NBCEL.generic.LNEG o)
		{
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLOOKUPSWITCH(NBCEL.generic.LOOKUPSWITCH o)
		{
			Stack().Pop();
		}

		//key
		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLOR(NBCEL.generic.LOR o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLREM(NBCEL.generic.LREM o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLRETURN(NBCEL.generic.LRETURN o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLSHL(NBCEL.generic.LSHL o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLSHR(NBCEL.generic.LSHR o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLSTORE(NBCEL.generic.LSTORE o)
		{
			Locals().Set(o.GetIndex(), Stack().Pop());
			Locals().Set(o.GetIndex() + 1, NBCEL.generic.Type.UNKNOWN);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLSUB(NBCEL.generic.LSUB o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLUSHR(NBCEL.generic.LUSHR o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitLXOR(NBCEL.generic.LXOR o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.LONG);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitMONITORENTER(NBCEL.generic.MONITORENTER o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitMONITOREXIT(NBCEL.generic.MONITOREXIT o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitMULTIANEWARRAY(NBCEL.generic.MULTIANEWARRAY o)
		{
			for (int i = 0; i < o.GetDimensions(); i++)
			{
				Stack().Pop();
			}
			Stack().Push(o.GetType(cpg));
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitNEW(NBCEL.generic.NEW o)
		{
			Stack().Push(new NBCEL.verifier.structurals.UninitializedObjectType((NBCEL.generic.ObjectType
				)(o.GetType(cpg))));
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitNEWARRAY(NBCEL.generic.NEWARRAY o)
		{
			Stack().Pop();
			Stack().Push(o.GetType());
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitNOP(NBCEL.generic.NOP o)
		{
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitPOP(NBCEL.generic.POP o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitPOP2(NBCEL.generic.POP2 o)
		{
			NBCEL.generic.Type t = Stack().Pop();
			if (t.GetSize() == 1)
			{
				Stack().Pop();
			}
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitPUTFIELD(NBCEL.generic.PUTFIELD o)
		{
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitPUTSTATIC(NBCEL.generic.PUTSTATIC o)
		{
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitRET(NBCEL.generic.RET o)
		{
		}

		// do nothing, return address
		// is in in the local variables.
		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitRETURN(NBCEL.generic.RETURN o)
		{
		}

		// do nothing.
		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitSALOAD(NBCEL.generic.SALOAD o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitSASTORE(NBCEL.generic.SASTORE o)
		{
			Stack().Pop();
			Stack().Pop();
			Stack().Pop();
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitSIPUSH(NBCEL.generic.SIPUSH o)
		{
			Stack().Push(NBCEL.generic.Type.INT);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitSWAP(NBCEL.generic.SWAP o)
		{
			NBCEL.generic.Type t = Stack().Pop();
			NBCEL.generic.Type u = Stack().Pop();
			Stack().Push(t);
			Stack().Push(u);
		}

		/// <summary>Symbolically executes the corresponding Java Virtual Machine instruction.
		/// 	</summary>
		public override void VisitTABLESWITCH(NBCEL.generic.TABLESWITCH o)
		{
			Stack().Pop();
		}
	}
}
