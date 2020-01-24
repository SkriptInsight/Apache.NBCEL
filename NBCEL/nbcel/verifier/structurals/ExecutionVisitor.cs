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

using NBCEL.classfile;
using NBCEL.generic;
using EmptyVisitor = NBCEL.generic.EmptyVisitor;

namespace NBCEL.verifier.structurals
{
	/// <summary>
	///     This Visitor class may be used for a type-based Java Virtual Machine
	///     simulation.
	/// </summary>
	/// <remarks>
	///     This Visitor class may be used for a type-based Java Virtual Machine
	///     simulation.
	///     <p>
	///         It does not check for correct types on the OperandStack or in the
	///         LocalVariables; nor does it check their sizes are sufficiently big.
	///         Thus, to use this Visitor for bytecode verifying, you have to make sure
	///         externally that the type constraints of the Java Virtual Machine instructions
	///         are satisfied. An InstConstraintVisitor may be used for this.
	///         Anyway, this Visitor does not mandate it. For example, when you
	///         visitIADD(IADD o), then there are two stack slots popped and one
	///         stack slot containing a Type.INT is pushed (where you could also
	///         pop only one slot if you know there are two Type.INT on top of the
	///         stack). Monitor-specific behavior is not simulated.
	///     </p>
	///     <b>Conventions:</b>
	///     <p>
	///         Type.VOID will never be pushed onto the stack. Type.DOUBLE and Type.LONG
	///         that would normally take up two stack slots (like Double_HIGH and
	///         Double_LOW) are represented by a simple single Type.DOUBLE or Type.LONG
	///         object on the stack here.
	///     </p>
	///     <p>
	///         If a two-slot type is stored into a local variable, the next variable
	///         is given the type Type.UNKNOWN.
	///     </p>
	/// </remarks>
	/// <seealso cref="VisitDSTORE(NBCEL.generic.DSTORE)" />
	/// <seealso cref="InstConstraintVisitor" />
	public class ExecutionVisitor : EmptyVisitor
    {
	    /// <summary>The ConstantPoolGen we're working with.</summary>
	    /// <seealso cref="SetConstantPoolGen(NBCEL.generic.ConstantPoolGen)" />
	    private ConstantPoolGen cpg;

        /// <summary>The executionframe we're operating on.</summary>
        private Frame frame;

        // CHECKSTYLE:OFF (there are lots of references!)
        //CHECKSTYLE:ON
        /// <summary>The OperandStack from the current Frame we're operating on.</summary>
        /// <seealso cref="SetFrame(Frame)" />
        private OperandStack Stack()
        {
            return frame.GetStack();
        }

        /// <summary>The LocalVariables from the current Frame we're operating on.</summary>
        /// <seealso cref="SetFrame(Frame)" />
        private LocalVariables Locals()
        {
            return frame.GetLocals();
        }

        /// <summary>Sets the ConstantPoolGen needed for symbolic execution.</summary>
        public virtual void SetConstantPoolGen(ConstantPoolGen cpg)
        {
            // TODO could be package-protected?
            this.cpg = cpg;
        }

        /// <summary>
        ///     The only method granting access to the single instance of
        ///     the ExecutionVisitor class.
        /// </summary>
        /// <remarks>
        ///     The only method granting access to the single instance of
        ///     the ExecutionVisitor class. Before actively using this
        ///     instance, <B>SET THE ConstantPoolGen FIRST</B>.
        /// </remarks>
        /// <seealso cref="SetConstantPoolGen(NBCEL.generic.ConstantPoolGen)" />
        public virtual void SetFrame(Frame f)
        {
            // TODO could be package-protected?
            frame = f;
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
        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitAALOAD(AALOAD o)
        {
            Stack().Pop();
            // pop the index int
            //System.out.print(stack().peek());
            var t = Stack().Pop();
            // Pop Array type
            if (t == Type.NULL)
            {
                Stack().Push(Type.NULL);
            }
            else
            {
                // Do nothing stackwise --- a NullPointerException is thrown at Run-Time
                var at = (ArrayType) t;
                Stack().Push(at.GetElementType());
            }
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitAASTORE(AASTORE o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitACONST_NULL(ACONST_NULL o)
        {
            Stack().Push(Type.NULL);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitALOAD(ALOAD o)
        {
            Stack().Push(Locals().Get(o.GetIndex()));
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitANEWARRAY(ANEWARRAY o)
        {
            Stack().Pop();
            //count
            Stack().Push(new ArrayType(o.GetType(cpg), 1));
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitARETURN(ARETURN o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitARRAYLENGTH(ARRAYLENGTH o)
        {
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitASTORE(ASTORE o)
        {
            Locals().Set(o.GetIndex(), Stack().Pop());
        }

        //System.err.println("TODO-DEBUG:    set LV '"+o.getIndex()+"' to '"+locals().get(o.getIndex())+"'.");
        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitATHROW(ATHROW o)
        {
            var t = Stack().Pop();
            Stack().Clear();
            if (t.Equals(Type.NULL))
                Stack().Push(Type.GetType("Ljava/lang/NullPointerException;"));
            else
                Stack().Push(t);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitBALOAD(BALOAD o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitBASTORE(BASTORE o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitBIPUSH(BIPUSH o)
        {
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitCALOAD(CALOAD o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitCASTORE(CASTORE o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitCHECKCAST(CHECKCAST o)
        {
            // It's possibly wrong to do so, but SUN's
            // ByteCode verifier seems to do (only) this, too.
            // TODO: One could use a sophisticated analysis here to check
            //       if a type cannot possibly be cated to another and by
            //       so doing predict the ClassCastException at run-time.
            Stack().Pop();
            Stack().Push(o.GetType(cpg));
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitD2F(D2F o)
        {
            Stack().Pop();
            Stack().Push(Type.FLOAT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitD2I(D2I o)
        {
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitD2L(D2L o)
        {
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDADD(DADD o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDALOAD(DALOAD o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDASTORE(DASTORE o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDCMPG(DCMPG o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDCMPL(DCMPL o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDCONST(DCONST o)
        {
            Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDDIV(DDIV o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDLOAD(DLOAD o)
        {
            Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDMUL(DMUL o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDNEG(DNEG o)
        {
            Stack().Pop();
            Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDREM(DREM o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDRETURN(DRETURN o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDSTORE(DSTORE o)
        {
            Locals().Set(o.GetIndex(), Stack().Pop());
            Locals().Set(o.GetIndex() + 1, Type.UNKNOWN);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDSUB(DSUB o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDUP(DUP o)
        {
            var t = Stack().Pop();
            Stack().Push(t);
            Stack().Push(t);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDUP_X1(DUP_X1 o)
        {
            var w1 = Stack().Pop();
            var w2 = Stack().Pop();
            Stack().Push(w1);
            Stack().Push(w2);
            Stack().Push(w1);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDUP_X2(DUP_X2 o)
        {
            var w1 = Stack().Pop();
            var w2 = Stack().Pop();
            if (w2.GetSize() == 2)
            {
                Stack().Push(w1);
                Stack().Push(w2);
                Stack().Push(w1);
            }
            else
            {
                var w3 = Stack().Pop();
                Stack().Push(w1);
                Stack().Push(w3);
                Stack().Push(w2);
                Stack().Push(w1);
            }
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDUP2(DUP2 o)
        {
            var t = Stack().Pop();
            if (t.GetSize() == 2)
            {
                Stack().Push(t);
                Stack().Push(t);
            }
            else
            {
                // t.getSize() is 1
                var u = Stack().Pop();
                Stack().Push(u);
                Stack().Push(t);
                Stack().Push(u);
                Stack().Push(t);
            }
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDUP2_X1(DUP2_X1 o)
        {
            var t = Stack().Pop();
            if (t.GetSize() == 2)
            {
                var u = Stack().Pop();
                Stack().Push(t);
                Stack().Push(u);
                Stack().Push(t);
            }
            else
            {
                //t.getSize() is1
                var u = Stack().Pop();
                var v = Stack().Pop();
                Stack().Push(u);
                Stack().Push(t);
                Stack().Push(v);
                Stack().Push(u);
                Stack().Push(t);
            }
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitDUP2_X2(DUP2_X2 o)
        {
            var t = Stack().Pop();
            if (t.GetSize() == 2)
            {
                var u = Stack().Pop();
                if (u.GetSize() == 2)
                {
                    Stack().Push(t);
                    Stack().Push(u);
                    Stack().Push(t);
                }
                else
                {
                    var v = Stack().Pop();
                    Stack().Push(t);
                    Stack().Push(v);
                    Stack().Push(u);
                    Stack().Push(t);
                }
            }
            else
            {
                //t.getSize() is 1
                var u = Stack().Pop();
                var v = Stack().Pop();
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
                    var w = Stack().Pop();
                    Stack().Push(u);
                    Stack().Push(t);
                    Stack().Push(w);
                    Stack().Push(v);
                    Stack().Push(u);
                    Stack().Push(t);
                }
            }
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitF2D(F2D o)
        {
            Stack().Pop();
            Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitF2I(F2I o)
        {
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitF2L(F2L o)
        {
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFADD(FADD o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.FLOAT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFALOAD(FALOAD o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.FLOAT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFASTORE(FASTORE o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFCMPG(FCMPG o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFCMPL(FCMPL o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFCONST(FCONST o)
        {
            Stack().Push(Type.FLOAT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFDIV(FDIV o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.FLOAT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFLOAD(FLOAD o)
        {
            Stack().Push(Type.FLOAT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFMUL(FMUL o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.FLOAT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFNEG(FNEG o)
        {
            Stack().Pop();
            Stack().Push(Type.FLOAT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFREM(FREM o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.FLOAT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFRETURN(FRETURN o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFSTORE(FSTORE o)
        {
            Locals().Set(o.GetIndex(), Stack().Pop());
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitFSUB(FSUB o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.FLOAT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitGETFIELD(GETFIELD o)
        {
            Stack().Pop();
            var t = o.GetFieldType(cpg);
            if (t.Equals(Type.BOOLEAN) || t.Equals(Type.CHAR) ||
                t.Equals(Type.BYTE) || t.Equals(Type.SHORT))
                t = Type.INT;
            Stack().Push(t);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitGETSTATIC(GETSTATIC o)
        {
            var t = o.GetFieldType(cpg);
            if (t.Equals(Type.BOOLEAN) || t.Equals(Type.CHAR) ||
                t.Equals(Type.BYTE) || t.Equals(Type.SHORT))
                t = Type.INT;
            Stack().Push(t);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitGOTO(GOTO o)
        {
        }

        // no stack changes.
        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitGOTO_W(GOTO_W o)
        {
        }

        // no stack changes.
        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitI2B(I2B o)
        {
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitI2C(I2C o)
        {
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitI2D(I2D o)
        {
            Stack().Pop();
            Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitI2F(I2F o)
        {
            Stack().Pop();
            Stack().Push(Type.FLOAT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitI2L(I2L o)
        {
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitI2S(I2S o)
        {
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIADD(IADD o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIALOAD(IALOAD o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIAND(IAND o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIASTORE(IASTORE o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitICONST(ICONST o)
        {
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIDIV(IDIV o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIF_ACMPEQ(IF_ACMPEQ o)
        {
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIF_ACMPNE(IF_ACMPNE o)
        {
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIF_ICMPEQ(IF_ICMPEQ o)
        {
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIF_ICMPGE(IF_ICMPGE o)
        {
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIF_ICMPGT(IF_ICMPGT o)
        {
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIF_ICMPLE(IF_ICMPLE o)
        {
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIF_ICMPLT(IF_ICMPLT o)
        {
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIF_ICMPNE(IF_ICMPNE o)
        {
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIFEQ(IFEQ o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIFGE(IFGE o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIFGT(IFGT o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIFLE(IFLE o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIFLT(IFLT o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIFNE(IFNE o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIFNONNULL(IFNONNULL o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIFNULL(IFNULL o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIINC(IINC o)
        {
        }

        // stack is not changed.
        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitILOAD(ILOAD o)
        {
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIMUL(IMUL o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitINEG(INEG o)
        {
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitINSTANCEOF(INSTANCEOF o)
        {
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        /// <since>6.0</since>
        public override void VisitINVOKEDYNAMIC(INVOKEDYNAMIC o)
        {
            for (var i = 0; i < o.GetArgumentTypes(cpg).Length; i++) Stack().Pop();
            // We are sure the invoked method will xRETURN eventually
            // We simulate xRETURNs functionality here because we
            // don't really "jump into" and simulate the invoked
            // method.
            if (o.GetReturnType(cpg) != Type.VOID)
            {
                var t = o.GetReturnType(cpg);
                if (t.Equals(Type.BOOLEAN) || t.Equals(Type.CHAR) ||
                    t.Equals(Type.BYTE) || t.Equals(Type.SHORT))
                    t = Type.INT;
                Stack().Push(t);
            }
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitINVOKEINTERFACE(INVOKEINTERFACE o)
        {
            Stack().Pop();
            //objectref
            for (var i = 0; i < o.GetArgumentTypes(cpg).Length; i++) Stack().Pop();
            // We are sure the invoked method will xRETURN eventually
            // We simulate xRETURNs functionality here because we
            // don't really "jump into" and simulate the invoked
            // method.
            if (o.GetReturnType(cpg) != Type.VOID)
            {
                var t = o.GetReturnType(cpg);
                if (t.Equals(Type.BOOLEAN) || t.Equals(Type.CHAR) ||
                    t.Equals(Type.BYTE) || t.Equals(Type.SHORT))
                    t = Type.INT;
                Stack().Push(t);
            }
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitINVOKESPECIAL(INVOKESPECIAL o)
        {
            if (o.GetMethodName(cpg).Equals(Const.CONSTRUCTOR_NAME))
            {
                var t = (UninitializedObjectType
                    ) Stack().Peek(o.GetArgumentTypes(cpg).Length);
                if (t == Frame.GetThis()) Frame.SetThis(null);
                Stack().InitializeObject(t);
                Locals().InitializeObject(t);
            }

            Stack().Pop();
            //objectref
            for (var i = 0; i < o.GetArgumentTypes(cpg).Length; i++) Stack().Pop();
            // We are sure the invoked method will xRETURN eventually
            // We simulate xRETURNs functionality here because we
            // don't really "jump into" and simulate the invoked
            // method.
            if (o.GetReturnType(cpg) != Type.VOID)
            {
                var t = o.GetReturnType(cpg);
                if (t.Equals(Type.BOOLEAN) || t.Equals(Type.CHAR) ||
                    t.Equals(Type.BYTE) || t.Equals(Type.SHORT))
                    t = Type.INT;
                Stack().Push(t);
            }
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitINVOKESTATIC(INVOKESTATIC o)
        {
            for (var i = 0; i < o.GetArgumentTypes(cpg).Length; i++) Stack().Pop();
            // We are sure the invoked method will xRETURN eventually
            // We simulate xRETURNs functionality here because we
            // don't really "jump into" and simulate the invoked
            // method.
            if (o.GetReturnType(cpg) != Type.VOID)
            {
                var t = o.GetReturnType(cpg);
                if (t.Equals(Type.BOOLEAN) || t.Equals(Type.CHAR) ||
                    t.Equals(Type.BYTE) || t.Equals(Type.SHORT))
                    t = Type.INT;
                Stack().Push(t);
            }
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitINVOKEVIRTUAL(INVOKEVIRTUAL o)
        {
            Stack().Pop();
            //objectref
            for (var i = 0; i < o.GetArgumentTypes(cpg).Length; i++) Stack().Pop();
            // We are sure the invoked method will xRETURN eventually
            // We simulate xRETURNs functionality here because we
            // don't really "jump into" and simulate the invoked
            // method.
            if (o.GetReturnType(cpg) != Type.VOID)
            {
                var t = o.GetReturnType(cpg);
                if (t.Equals(Type.BOOLEAN) || t.Equals(Type.CHAR) ||
                    t.Equals(Type.BYTE) || t.Equals(Type.SHORT))
                    t = Type.INT;
                Stack().Push(t);
            }
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIOR(IOR o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIREM(IREM o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIRETURN(IRETURN o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitISHL(ISHL o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitISHR(ISHR o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitISTORE(ISTORE o)
        {
            Locals().Set(o.GetIndex(), Stack().Pop());
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitISUB(ISUB o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIUSHR(IUSHR o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitIXOR(IXOR o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitJSR(JSR o)
        {
            Stack().Push(new ReturnaddressType(o.PhysicalSuccessor()));
        }

        //System.err.println("TODO-----------:"+o.physicalSuccessor());
        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitJSR_W(JSR_W o)
        {
            Stack().Push(new ReturnaddressType(o.PhysicalSuccessor()));
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitL2D(L2D o)
        {
            Stack().Pop();
            Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitL2F(L2F o)
        {
            Stack().Pop();
            Stack().Push(Type.FLOAT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitL2I(L2I o)
        {
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLADD(LADD o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLALOAD(LALOAD o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLAND(LAND o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLASTORE(LASTORE o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLCMP(LCMP o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLCONST(LCONST o)
        {
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLDC(LDC o)
        {
            var c = cpg.GetConstant(o.GetIndex());
            if (c is ConstantInteger) Stack().Push(Type.INT);
            if (c is ConstantFloat) Stack().Push(Type.FLOAT);
            if (c is ConstantString) Stack().Push(Type.STRING);
            if (c is ConstantClass) Stack().Push(Type.CLASS);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public virtual void VisitLDC_W(LDC_W o)
        {
            var c = cpg.GetConstant(o.GetIndex());
            if (c is ConstantInteger) Stack().Push(Type.INT);
            if (c is ConstantFloat) Stack().Push(Type.FLOAT);
            if (c is ConstantString) Stack().Push(Type.STRING);
            if (c is ConstantClass) Stack().Push(Type.CLASS);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLDC2_W(LDC2_W o)
        {
            var c = cpg.GetConstant(o.GetIndex());
            if (c is ConstantLong) Stack().Push(Type.LONG);
            if (c is ConstantDouble) Stack().Push(Type.DOUBLE);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLDIV(LDIV o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLLOAD(LLOAD o)
        {
            Stack().Push(Locals().Get(o.GetIndex()));
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLMUL(LMUL o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLNEG(LNEG o)
        {
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLOOKUPSWITCH(LOOKUPSWITCH o)
        {
            Stack().Pop();
        }

        //key
        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLOR(LOR o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLREM(LREM o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLRETURN(LRETURN o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLSHL(LSHL o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLSHR(LSHR o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLSTORE(LSTORE o)
        {
            Locals().Set(o.GetIndex(), Stack().Pop());
            Locals().Set(o.GetIndex() + 1, Type.UNKNOWN);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLSUB(LSUB o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLUSHR(LUSHR o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitLXOR(LXOR o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.LONG);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitMONITORENTER(MONITORENTER o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitMONITOREXIT(MONITOREXIT o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitMULTIANEWARRAY(MULTIANEWARRAY o)
        {
            for (var i = 0; i < o.GetDimensions(); i++) Stack().Pop();
            Stack().Push(o.GetType(cpg));
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitNEW(NEW o)
        {
            Stack().Push(new UninitializedObjectType((ObjectType
                ) o.GetType(cpg)));
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitNEWARRAY(NEWARRAY o)
        {
            Stack().Pop();
            Stack().Push(o.GetType());
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitNOP(NOP o)
        {
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitPOP(POP o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitPOP2(POP2 o)
        {
            var t = Stack().Pop();
            if (t.GetSize() == 1) Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitPUTFIELD(PUTFIELD o)
        {
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitPUTSTATIC(PUTSTATIC o)
        {
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitRET(RET o)
        {
        }

        // do nothing, return address
        // is in in the local variables.
        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitRETURN(RETURN o)
        {
        }

        // do nothing.
        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitSALOAD(SALOAD o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitSASTORE(SASTORE o)
        {
            Stack().Pop();
            Stack().Pop();
            Stack().Pop();
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitSIPUSH(SIPUSH o)
        {
            Stack().Push(Type.INT);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitSWAP(SWAP o)
        {
            var t = Stack().Pop();
            var u = Stack().Pop();
            Stack().Push(t);
            Stack().Push(u);
        }

        /// <summary>
        ///     Symbolically executes the corresponding Java Virtual Machine instruction.
        /// </summary>
        public override void VisitTABLESWITCH(TABLESWITCH o)
        {
            Stack().Pop();
        }
    }
}