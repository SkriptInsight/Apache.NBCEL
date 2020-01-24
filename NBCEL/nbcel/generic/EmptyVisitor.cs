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

namespace NBCEL.generic
{
	/// <summary>Supplies empty method bodies to be overridden by subclasses.</summary>
	public abstract class EmptyVisitor : NBCEL.generic.Visitor
	{
		public virtual void VisitStackInstruction(NBCEL.generic.StackInstruction obj)
		{
		}

		public virtual void VisitLocalVariableInstruction(NBCEL.generic.LocalVariableInstruction
			 obj)
		{
		}

		public virtual void VisitBranchInstruction(NBCEL.generic.BranchInstruction obj)
		{
		}

		public virtual void VisitLoadClass(NBCEL.generic.LoadClass obj)
		{
		}

		public virtual void VisitFieldInstruction(NBCEL.generic.FieldInstruction obj)
		{
		}

		public virtual void VisitIfInstruction(NBCEL.generic.IfInstruction obj)
		{
		}

		public virtual void VisitConversionInstruction(NBCEL.generic.ConversionInstruction
			 obj)
		{
		}

		public virtual void VisitPopInstruction(NBCEL.generic.PopInstruction obj)
		{
		}

		public virtual void VisitJsrInstruction(NBCEL.generic.JsrInstruction obj)
		{
		}

		public virtual void VisitGotoInstruction(NBCEL.generic.GotoInstruction obj)
		{
		}

		public virtual void VisitStoreInstruction(NBCEL.generic.StoreInstruction obj)
		{
		}

		public virtual void VisitTypedInstruction(NBCEL.generic.TypedInstruction obj)
		{
		}

		public virtual void VisitSelect(NBCEL.generic.Select obj)
		{
		}

		public virtual void VisitUnconditionalBranch(NBCEL.generic.UnconditionalBranch obj
			)
		{
		}

		public virtual void VisitPushInstruction(NBCEL.generic.PushInstruction obj)
		{
		}

		public virtual void VisitArithmeticInstruction(NBCEL.generic.ArithmeticInstruction
			 obj)
		{
		}

		public virtual void VisitCPInstruction(NBCEL.generic.CPInstruction obj)
		{
		}

		public virtual void VisitInvokeInstruction(NBCEL.generic.InvokeInstruction obj)
		{
		}

		public virtual void VisitArrayInstruction(NBCEL.generic.ArrayInstruction obj)
		{
		}

		public virtual void VisitAllocationInstruction(NBCEL.generic.AllocationInstruction
			 obj)
		{
		}

		public virtual void VisitReturnInstruction(NBCEL.generic.ReturnInstruction obj)
		{
		}

		public virtual void VisitFieldOrMethod(NBCEL.generic.FieldOrMethod obj)
		{
		}

		public virtual void VisitConstantPushInstruction<T>(NBCEL.generic.ConstantPushInstruction<T>
			 obj)
		{
		}

		public virtual void VisitExceptionThrower(NBCEL.generic.ExceptionThrower obj)
		{
		}

		public virtual void VisitLoadInstruction(NBCEL.generic.LoadInstruction obj)
		{
		}

		public virtual void VisitVariableLengthInstruction(NBCEL.generic.VariableLengthInstruction
			 obj)
		{
		}

		public virtual void VisitStackProducer(NBCEL.generic.StackProducer obj)
		{
		}

		public virtual void VisitStackConsumer(NBCEL.generic.StackConsumer obj)
		{
		}

		public virtual void VisitACONST_NULL(NBCEL.generic.ACONST_NULL obj)
		{
		}

		public virtual void VisitGETSTATIC(NBCEL.generic.GETSTATIC obj)
		{
		}

		public virtual void VisitIF_ICMPLT(NBCEL.generic.IF_ICMPLT obj)
		{
		}

		public virtual void VisitMONITOREXIT(NBCEL.generic.MONITOREXIT obj)
		{
		}

		public virtual void VisitIFLT(NBCEL.generic.IFLT obj)
		{
		}

		public virtual void VisitLSTORE(NBCEL.generic.LSTORE obj)
		{
		}

		public virtual void VisitPOP2(NBCEL.generic.POP2 obj)
		{
		}

		public virtual void VisitBASTORE(NBCEL.generic.BASTORE obj)
		{
		}

		public virtual void VisitISTORE(NBCEL.generic.ISTORE obj)
		{
		}

		public virtual void VisitCHECKCAST(NBCEL.generic.CHECKCAST obj)
		{
		}

		public virtual void VisitFCMPG(NBCEL.generic.FCMPG obj)
		{
		}

		public virtual void VisitI2F(NBCEL.generic.I2F obj)
		{
		}

		public virtual void VisitATHROW(NBCEL.generic.ATHROW obj)
		{
		}

		public virtual void VisitDCMPL(NBCEL.generic.DCMPL obj)
		{
		}

		public virtual void VisitARRAYLENGTH(NBCEL.generic.ARRAYLENGTH obj)
		{
		}

		public virtual void VisitDUP(NBCEL.generic.DUP obj)
		{
		}

		public virtual void VisitINVOKESTATIC(NBCEL.generic.INVOKESTATIC obj)
		{
		}

		public virtual void VisitLCONST(NBCEL.generic.LCONST obj)
		{
		}

		public virtual void VisitDREM(NBCEL.generic.DREM obj)
		{
		}

		public virtual void VisitIFGE(NBCEL.generic.IFGE obj)
		{
		}

		public virtual void VisitCALOAD(NBCEL.generic.CALOAD obj)
		{
		}

		public virtual void VisitLASTORE(NBCEL.generic.LASTORE obj)
		{
		}

		public virtual void VisitI2D(NBCEL.generic.I2D obj)
		{
		}

		public virtual void VisitDADD(NBCEL.generic.DADD obj)
		{
		}

		public virtual void VisitINVOKESPECIAL(NBCEL.generic.INVOKESPECIAL obj)
		{
		}

		public virtual void VisitIAND(NBCEL.generic.IAND obj)
		{
		}

		public virtual void VisitPUTFIELD(NBCEL.generic.PUTFIELD obj)
		{
		}

		public virtual void VisitILOAD(NBCEL.generic.ILOAD obj)
		{
		}

		public virtual void VisitDLOAD(NBCEL.generic.DLOAD obj)
		{
		}

		public virtual void VisitDCONST(NBCEL.generic.DCONST obj)
		{
		}

		public virtual void VisitNEW(NBCEL.generic.NEW obj)
		{
		}

		public virtual void VisitIFNULL(NBCEL.generic.IFNULL obj)
		{
		}

		public virtual void VisitLSUB(NBCEL.generic.LSUB obj)
		{
		}

		public virtual void VisitL2I(NBCEL.generic.L2I obj)
		{
		}

		public virtual void VisitISHR(NBCEL.generic.ISHR obj)
		{
		}

		public virtual void VisitTABLESWITCH(NBCEL.generic.TABLESWITCH obj)
		{
		}

		public virtual void VisitIINC(NBCEL.generic.IINC obj)
		{
		}

		public virtual void VisitDRETURN(NBCEL.generic.DRETURN obj)
		{
		}

		public virtual void VisitFSTORE(NBCEL.generic.FSTORE obj)
		{
		}

		public virtual void VisitDASTORE(NBCEL.generic.DASTORE obj)
		{
		}

		public virtual void VisitIALOAD(NBCEL.generic.IALOAD obj)
		{
		}

		public virtual void VisitDDIV(NBCEL.generic.DDIV obj)
		{
		}

		public virtual void VisitIF_ICMPGE(NBCEL.generic.IF_ICMPGE obj)
		{
		}

		public virtual void VisitLAND(NBCEL.generic.LAND obj)
		{
		}

		public virtual void VisitIDIV(NBCEL.generic.IDIV obj)
		{
		}

		public virtual void VisitLOR(NBCEL.generic.LOR obj)
		{
		}

		public virtual void VisitCASTORE(NBCEL.generic.CASTORE obj)
		{
		}

		public virtual void VisitFREM(NBCEL.generic.FREM obj)
		{
		}

		public virtual void VisitLDC(NBCEL.generic.LDC obj)
		{
		}

		public virtual void VisitBIPUSH(NBCEL.generic.BIPUSH obj)
		{
		}

		public virtual void VisitDSTORE(NBCEL.generic.DSTORE obj)
		{
		}

		public virtual void VisitF2L(NBCEL.generic.F2L obj)
		{
		}

		public virtual void VisitFMUL(NBCEL.generic.FMUL obj)
		{
		}

		public virtual void VisitLLOAD(NBCEL.generic.LLOAD obj)
		{
		}

		public virtual void VisitJSR(NBCEL.generic.JSR obj)
		{
		}

		public virtual void VisitFSUB(NBCEL.generic.FSUB obj)
		{
		}

		public virtual void VisitSASTORE(NBCEL.generic.SASTORE obj)
		{
		}

		public virtual void VisitALOAD(NBCEL.generic.ALOAD obj)
		{
		}

		public virtual void VisitDUP2_X2(NBCEL.generic.DUP2_X2 obj)
		{
		}

		public virtual void VisitRETURN(NBCEL.generic.RETURN obj)
		{
		}

		public virtual void VisitDALOAD(NBCEL.generic.DALOAD obj)
		{
		}

		public virtual void VisitSIPUSH(NBCEL.generic.SIPUSH obj)
		{
		}

		public virtual void VisitDSUB(NBCEL.generic.DSUB obj)
		{
		}

		public virtual void VisitL2F(NBCEL.generic.L2F obj)
		{
		}

		public virtual void VisitIF_ICMPGT(NBCEL.generic.IF_ICMPGT obj)
		{
		}

		public virtual void VisitF2D(NBCEL.generic.F2D obj)
		{
		}

		public virtual void VisitI2L(NBCEL.generic.I2L obj)
		{
		}

		public virtual void VisitIF_ACMPNE(NBCEL.generic.IF_ACMPNE obj)
		{
		}

		public virtual void VisitPOP(NBCEL.generic.POP obj)
		{
		}

		public virtual void VisitI2S(NBCEL.generic.I2S obj)
		{
		}

		public virtual void VisitIFEQ(NBCEL.generic.IFEQ obj)
		{
		}

		public virtual void VisitSWAP(NBCEL.generic.SWAP obj)
		{
		}

		public virtual void VisitIOR(NBCEL.generic.IOR obj)
		{
		}

		public virtual void VisitIREM(NBCEL.generic.IREM obj)
		{
		}

		public virtual void VisitIASTORE(NBCEL.generic.IASTORE obj)
		{
		}

		public virtual void VisitNEWARRAY(NBCEL.generic.NEWARRAY obj)
		{
		}

		public virtual void VisitINVOKEINTERFACE(NBCEL.generic.INVOKEINTERFACE obj)
		{
		}

		public virtual void VisitINEG(NBCEL.generic.INEG obj)
		{
		}

		public virtual void VisitLCMP(NBCEL.generic.LCMP obj)
		{
		}

		public virtual void VisitJSR_W(NBCEL.generic.JSR_W obj)
		{
		}

		public virtual void VisitMULTIANEWARRAY(NBCEL.generic.MULTIANEWARRAY obj)
		{
		}

		public virtual void VisitDUP_X2(NBCEL.generic.DUP_X2 obj)
		{
		}

		public virtual void VisitSALOAD(NBCEL.generic.SALOAD obj)
		{
		}

		public virtual void VisitIFNONNULL(NBCEL.generic.IFNONNULL obj)
		{
		}

		public virtual void VisitDMUL(NBCEL.generic.DMUL obj)
		{
		}

		public virtual void VisitIFNE(NBCEL.generic.IFNE obj)
		{
		}

		public virtual void VisitIF_ICMPLE(NBCEL.generic.IF_ICMPLE obj)
		{
		}

		public virtual void VisitLDC2_W(NBCEL.generic.LDC2_W obj)
		{
		}

		public virtual void VisitGETFIELD(NBCEL.generic.GETFIELD obj)
		{
		}

		public virtual void VisitLADD(NBCEL.generic.LADD obj)
		{
		}

		public virtual void VisitNOP(NBCEL.generic.NOP obj)
		{
		}

		public virtual void VisitFALOAD(NBCEL.generic.FALOAD obj)
		{
		}

		public virtual void VisitINSTANCEOF(NBCEL.generic.INSTANCEOF obj)
		{
		}

		public virtual void VisitIFLE(NBCEL.generic.IFLE obj)
		{
		}

		public virtual void VisitLXOR(NBCEL.generic.LXOR obj)
		{
		}

		public virtual void VisitLRETURN(NBCEL.generic.LRETURN obj)
		{
		}

		public virtual void VisitFCONST(NBCEL.generic.FCONST obj)
		{
		}

		public virtual void VisitIUSHR(NBCEL.generic.IUSHR obj)
		{
		}

		public virtual void VisitBALOAD(NBCEL.generic.BALOAD obj)
		{
		}

		public virtual void VisitDUP2(NBCEL.generic.DUP2 obj)
		{
		}

		public virtual void VisitIF_ACMPEQ(NBCEL.generic.IF_ACMPEQ obj)
		{
		}

		public virtual void VisitIMPDEP1(NBCEL.generic.IMPDEP1 obj)
		{
		}

		public virtual void VisitMONITORENTER(NBCEL.generic.MONITORENTER obj)
		{
		}

		public virtual void VisitLSHL(NBCEL.generic.LSHL obj)
		{
		}

		public virtual void VisitDCMPG(NBCEL.generic.DCMPG obj)
		{
		}

		public virtual void VisitD2L(NBCEL.generic.D2L obj)
		{
		}

		public virtual void VisitIMPDEP2(NBCEL.generic.IMPDEP2 obj)
		{
		}

		public virtual void VisitL2D(NBCEL.generic.L2D obj)
		{
		}

		public virtual void VisitRET(NBCEL.generic.RET obj)
		{
		}

		public virtual void VisitIFGT(NBCEL.generic.IFGT obj)
		{
		}

		public virtual void VisitIXOR(NBCEL.generic.IXOR obj)
		{
		}

		public virtual void VisitINVOKEVIRTUAL(NBCEL.generic.INVOKEVIRTUAL obj)
		{
		}

		public virtual void VisitFASTORE(NBCEL.generic.FASTORE obj)
		{
		}

		public virtual void VisitIRETURN(NBCEL.generic.IRETURN obj)
		{
		}

		public virtual void VisitIF_ICMPNE(NBCEL.generic.IF_ICMPNE obj)
		{
		}

		public virtual void VisitFLOAD(NBCEL.generic.FLOAD obj)
		{
		}

		public virtual void VisitLDIV(NBCEL.generic.LDIV obj)
		{
		}

		public virtual void VisitPUTSTATIC(NBCEL.generic.PUTSTATIC obj)
		{
		}

		public virtual void VisitAALOAD(NBCEL.generic.AALOAD obj)
		{
		}

		public virtual void VisitD2I(NBCEL.generic.D2I obj)
		{
		}

		public virtual void VisitIF_ICMPEQ(NBCEL.generic.IF_ICMPEQ obj)
		{
		}

		public virtual void VisitAASTORE(NBCEL.generic.AASTORE obj)
		{
		}

		public virtual void VisitARETURN(NBCEL.generic.ARETURN obj)
		{
		}

		public virtual void VisitDUP2_X1(NBCEL.generic.DUP2_X1 obj)
		{
		}

		public virtual void VisitFNEG(NBCEL.generic.FNEG obj)
		{
		}

		public virtual void VisitGOTO_W(NBCEL.generic.GOTO_W obj)
		{
		}

		public virtual void VisitD2F(NBCEL.generic.D2F obj)
		{
		}

		public virtual void VisitGOTO(NBCEL.generic.GOTO obj)
		{
		}

		public virtual void VisitISUB(NBCEL.generic.ISUB obj)
		{
		}

		public virtual void VisitF2I(NBCEL.generic.F2I obj)
		{
		}

		public virtual void VisitDNEG(NBCEL.generic.DNEG obj)
		{
		}

		public virtual void VisitICONST(NBCEL.generic.ICONST obj)
		{
		}

		public virtual void VisitFDIV(NBCEL.generic.FDIV obj)
		{
		}

		public virtual void VisitI2B(NBCEL.generic.I2B obj)
		{
		}

		public virtual void VisitLNEG(NBCEL.generic.LNEG obj)
		{
		}

		public virtual void VisitLREM(NBCEL.generic.LREM obj)
		{
		}

		public virtual void VisitIMUL(NBCEL.generic.IMUL obj)
		{
		}

		public virtual void VisitIADD(NBCEL.generic.IADD obj)
		{
		}

		public virtual void VisitLSHR(NBCEL.generic.LSHR obj)
		{
		}

		public virtual void VisitLOOKUPSWITCH(NBCEL.generic.LOOKUPSWITCH obj)
		{
		}

		public virtual void VisitDUP_X1(NBCEL.generic.DUP_X1 obj)
		{
		}

		public virtual void VisitFCMPL(NBCEL.generic.FCMPL obj)
		{
		}

		public virtual void VisitI2C(NBCEL.generic.I2C obj)
		{
		}

		public virtual void VisitLMUL(NBCEL.generic.LMUL obj)
		{
		}

		public virtual void VisitLUSHR(NBCEL.generic.LUSHR obj)
		{
		}

		public virtual void VisitISHL(NBCEL.generic.ISHL obj)
		{
		}

		public virtual void VisitLALOAD(NBCEL.generic.LALOAD obj)
		{
		}

		public virtual void VisitASTORE(NBCEL.generic.ASTORE obj)
		{
		}

		public virtual void VisitANEWARRAY(NBCEL.generic.ANEWARRAY obj)
		{
		}

		public virtual void VisitFRETURN(NBCEL.generic.FRETURN obj)
		{
		}

		public virtual void VisitFADD(NBCEL.generic.FADD obj)
		{
		}

		public virtual void VisitBREAKPOINT(NBCEL.generic.BREAKPOINT obj)
		{
		}

		/// <since>6.0</since>
		public virtual void VisitINVOKEDYNAMIC(NBCEL.generic.INVOKEDYNAMIC obj)
		{
		}
	}
}
