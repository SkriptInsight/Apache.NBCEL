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
	/// <summary>Interface implementing the Visitor pattern programming style.</summary>
	/// <remarks>
	/// Interface implementing the Visitor pattern programming style.
	/// I.e., a class that implements this interface can handle all types of
	/// instructions with the properly typed methods just by calling the accept()
	/// method.
	/// </remarks>
	public interface Visitor
	{
		void VisitStackInstruction(NBCEL.generic.StackInstruction obj);

		void VisitLocalVariableInstruction(NBCEL.generic.LocalVariableInstruction obj);

		void VisitBranchInstruction(NBCEL.generic.BranchInstruction obj);

		void VisitLoadClass(NBCEL.generic.LoadClass obj);

		void VisitFieldInstruction(NBCEL.generic.FieldInstruction obj);

		void VisitIfInstruction(NBCEL.generic.IfInstruction obj);

		void VisitConversionInstruction(NBCEL.generic.ConversionInstruction obj);

		void VisitPopInstruction(NBCEL.generic.PopInstruction obj);

		void VisitStoreInstruction(NBCEL.generic.StoreInstruction obj);

		void VisitTypedInstruction(NBCEL.generic.TypedInstruction obj);

		void VisitSelect(NBCEL.generic.Select obj);

		void VisitJsrInstruction(NBCEL.generic.JsrInstruction obj);

		void VisitGotoInstruction(NBCEL.generic.GotoInstruction obj);

		void VisitUnconditionalBranch(NBCEL.generic.UnconditionalBranch obj);

		void VisitPushInstruction(NBCEL.generic.PushInstruction obj);

		void VisitArithmeticInstruction(NBCEL.generic.ArithmeticInstruction obj);

		void VisitCPInstruction(NBCEL.generic.CPInstruction obj);

		void VisitInvokeInstruction(NBCEL.generic.InvokeInstruction obj);

		void VisitArrayInstruction(NBCEL.generic.ArrayInstruction obj);

		void VisitAllocationInstruction(NBCEL.generic.AllocationInstruction obj);

		void VisitReturnInstruction(NBCEL.generic.ReturnInstruction obj);

		void VisitFieldOrMethod(NBCEL.generic.FieldOrMethod obj);

		void VisitConstantPushInstruction<T>(NBCEL.generic.ConstantPushInstruction<T> obj);

		void VisitExceptionThrower(NBCEL.generic.ExceptionThrower obj);

		void VisitLoadInstruction(NBCEL.generic.LoadInstruction obj);

		void VisitVariableLengthInstruction(NBCEL.generic.VariableLengthInstruction obj);

		void VisitStackProducer(NBCEL.generic.StackProducer obj);

		void VisitStackConsumer(NBCEL.generic.StackConsumer obj);

		void VisitACONST_NULL(NBCEL.generic.ACONST_NULL obj);

		void VisitGETSTATIC(NBCEL.generic.GETSTATIC obj);

		void VisitIF_ICMPLT(NBCEL.generic.IF_ICMPLT obj);

		void VisitMONITOREXIT(NBCEL.generic.MONITOREXIT obj);

		void VisitIFLT(NBCEL.generic.IFLT obj);

		void VisitLSTORE(NBCEL.generic.LSTORE obj);

		void VisitPOP2(NBCEL.generic.POP2 obj);

		void VisitBASTORE(NBCEL.generic.BASTORE obj);

		void VisitISTORE(NBCEL.generic.ISTORE obj);

		void VisitCHECKCAST(NBCEL.generic.CHECKCAST obj);

		void VisitFCMPG(NBCEL.generic.FCMPG obj);

		void VisitI2F(NBCEL.generic.I2F obj);

		void VisitATHROW(NBCEL.generic.ATHROW obj);

		void VisitDCMPL(NBCEL.generic.DCMPL obj);

		void VisitARRAYLENGTH(NBCEL.generic.ARRAYLENGTH obj);

		void VisitDUP(NBCEL.generic.DUP obj);

		void VisitINVOKESTATIC(NBCEL.generic.INVOKESTATIC obj);

		void VisitLCONST(NBCEL.generic.LCONST obj);

		void VisitDREM(NBCEL.generic.DREM obj);

		void VisitIFGE(NBCEL.generic.IFGE obj);

		void VisitCALOAD(NBCEL.generic.CALOAD obj);

		void VisitLASTORE(NBCEL.generic.LASTORE obj);

		void VisitI2D(NBCEL.generic.I2D obj);

		void VisitDADD(NBCEL.generic.DADD obj);

		void VisitINVOKESPECIAL(NBCEL.generic.INVOKESPECIAL obj);

		void VisitIAND(NBCEL.generic.IAND obj);

		void VisitPUTFIELD(NBCEL.generic.PUTFIELD obj);

		void VisitILOAD(NBCEL.generic.ILOAD obj);

		void VisitDLOAD(NBCEL.generic.DLOAD obj);

		void VisitDCONST(NBCEL.generic.DCONST obj);

		void VisitNEW(NBCEL.generic.NEW obj);

		void VisitIFNULL(NBCEL.generic.IFNULL obj);

		void VisitLSUB(NBCEL.generic.LSUB obj);

		void VisitL2I(NBCEL.generic.L2I obj);

		void VisitISHR(NBCEL.generic.ISHR obj);

		void VisitTABLESWITCH(NBCEL.generic.TABLESWITCH obj);

		void VisitIINC(NBCEL.generic.IINC obj);

		void VisitDRETURN(NBCEL.generic.DRETURN obj);

		void VisitFSTORE(NBCEL.generic.FSTORE obj);

		void VisitDASTORE(NBCEL.generic.DASTORE obj);

		void VisitIALOAD(NBCEL.generic.IALOAD obj);

		void VisitDDIV(NBCEL.generic.DDIV obj);

		void VisitIF_ICMPGE(NBCEL.generic.IF_ICMPGE obj);

		void VisitLAND(NBCEL.generic.LAND obj);

		void VisitIDIV(NBCEL.generic.IDIV obj);

		void VisitLOR(NBCEL.generic.LOR obj);

		void VisitCASTORE(NBCEL.generic.CASTORE obj);

		void VisitFREM(NBCEL.generic.FREM obj);

		void VisitLDC(NBCEL.generic.LDC obj);

		void VisitBIPUSH(NBCEL.generic.BIPUSH obj);

		void VisitDSTORE(NBCEL.generic.DSTORE obj);

		void VisitF2L(NBCEL.generic.F2L obj);

		void VisitFMUL(NBCEL.generic.FMUL obj);

		void VisitLLOAD(NBCEL.generic.LLOAD obj);

		void VisitJSR(NBCEL.generic.JSR obj);

		void VisitFSUB(NBCEL.generic.FSUB obj);

		void VisitSASTORE(NBCEL.generic.SASTORE obj);

		void VisitALOAD(NBCEL.generic.ALOAD obj);

		void VisitDUP2_X2(NBCEL.generic.DUP2_X2 obj);

		void VisitRETURN(NBCEL.generic.RETURN obj);

		void VisitDALOAD(NBCEL.generic.DALOAD obj);

		void VisitSIPUSH(NBCEL.generic.SIPUSH obj);

		void VisitDSUB(NBCEL.generic.DSUB obj);

		void VisitL2F(NBCEL.generic.L2F obj);

		void VisitIF_ICMPGT(NBCEL.generic.IF_ICMPGT obj);

		void VisitF2D(NBCEL.generic.F2D obj);

		void VisitI2L(NBCEL.generic.I2L obj);

		void VisitIF_ACMPNE(NBCEL.generic.IF_ACMPNE obj);

		void VisitPOP(NBCEL.generic.POP obj);

		void VisitI2S(NBCEL.generic.I2S obj);

		void VisitIFEQ(NBCEL.generic.IFEQ obj);

		void VisitSWAP(NBCEL.generic.SWAP obj);

		void VisitIOR(NBCEL.generic.IOR obj);

		void VisitIREM(NBCEL.generic.IREM obj);

		void VisitIASTORE(NBCEL.generic.IASTORE obj);

		void VisitNEWARRAY(NBCEL.generic.NEWARRAY obj);

		void VisitINVOKEINTERFACE(NBCEL.generic.INVOKEINTERFACE obj);

		void VisitINEG(NBCEL.generic.INEG obj);

		void VisitLCMP(NBCEL.generic.LCMP obj);

		void VisitJSR_W(NBCEL.generic.JSR_W obj);

		void VisitMULTIANEWARRAY(NBCEL.generic.MULTIANEWARRAY obj);

		void VisitDUP_X2(NBCEL.generic.DUP_X2 obj);

		void VisitSALOAD(NBCEL.generic.SALOAD obj);

		void VisitIFNONNULL(NBCEL.generic.IFNONNULL obj);

		void VisitDMUL(NBCEL.generic.DMUL obj);

		void VisitIFNE(NBCEL.generic.IFNE obj);

		void VisitIF_ICMPLE(NBCEL.generic.IF_ICMPLE obj);

		void VisitLDC2_W(NBCEL.generic.LDC2_W obj);

		void VisitGETFIELD(NBCEL.generic.GETFIELD obj);

		void VisitLADD(NBCEL.generic.LADD obj);

		void VisitNOP(NBCEL.generic.NOP obj);

		void VisitFALOAD(NBCEL.generic.FALOAD obj);

		void VisitINSTANCEOF(NBCEL.generic.INSTANCEOF obj);

		void VisitIFLE(NBCEL.generic.IFLE obj);

		void VisitLXOR(NBCEL.generic.LXOR obj);

		void VisitLRETURN(NBCEL.generic.LRETURN obj);

		void VisitFCONST(NBCEL.generic.FCONST obj);

		void VisitIUSHR(NBCEL.generic.IUSHR obj);

		void VisitBALOAD(NBCEL.generic.BALOAD obj);

		void VisitDUP2(NBCEL.generic.DUP2 obj);

		void VisitIF_ACMPEQ(NBCEL.generic.IF_ACMPEQ obj);

		void VisitIMPDEP1(NBCEL.generic.IMPDEP1 obj);

		void VisitMONITORENTER(NBCEL.generic.MONITORENTER obj);

		void VisitLSHL(NBCEL.generic.LSHL obj);

		void VisitDCMPG(NBCEL.generic.DCMPG obj);

		void VisitD2L(NBCEL.generic.D2L obj);

		void VisitIMPDEP2(NBCEL.generic.IMPDEP2 obj);

		void VisitL2D(NBCEL.generic.L2D obj);

		void VisitRET(NBCEL.generic.RET obj);

		void VisitIFGT(NBCEL.generic.IFGT obj);

		void VisitIXOR(NBCEL.generic.IXOR obj);

		void VisitINVOKEVIRTUAL(NBCEL.generic.INVOKEVIRTUAL obj);

		/// <since>6.0</since>
		void VisitINVOKEDYNAMIC(NBCEL.generic.INVOKEDYNAMIC obj);

		void VisitFASTORE(NBCEL.generic.FASTORE obj);

		void VisitIRETURN(NBCEL.generic.IRETURN obj);

		void VisitIF_ICMPNE(NBCEL.generic.IF_ICMPNE obj);

		void VisitFLOAD(NBCEL.generic.FLOAD obj);

		void VisitLDIV(NBCEL.generic.LDIV obj);

		void VisitPUTSTATIC(NBCEL.generic.PUTSTATIC obj);

		void VisitAALOAD(NBCEL.generic.AALOAD obj);

		void VisitD2I(NBCEL.generic.D2I obj);

		void VisitIF_ICMPEQ(NBCEL.generic.IF_ICMPEQ obj);

		void VisitAASTORE(NBCEL.generic.AASTORE obj);

		void VisitARETURN(NBCEL.generic.ARETURN obj);

		void VisitDUP2_X1(NBCEL.generic.DUP2_X1 obj);

		void VisitFNEG(NBCEL.generic.FNEG obj);

		void VisitGOTO_W(NBCEL.generic.GOTO_W obj);

		void VisitD2F(NBCEL.generic.D2F obj);

		void VisitGOTO(NBCEL.generic.GOTO obj);

		void VisitISUB(NBCEL.generic.ISUB obj);

		void VisitF2I(NBCEL.generic.F2I obj);

		void VisitDNEG(NBCEL.generic.DNEG obj);

		void VisitICONST(NBCEL.generic.ICONST obj);

		void VisitFDIV(NBCEL.generic.FDIV obj);

		void VisitI2B(NBCEL.generic.I2B obj);

		void VisitLNEG(NBCEL.generic.LNEG obj);

		void VisitLREM(NBCEL.generic.LREM obj);

		void VisitIMUL(NBCEL.generic.IMUL obj);

		void VisitIADD(NBCEL.generic.IADD obj);

		void VisitLSHR(NBCEL.generic.LSHR obj);

		void VisitLOOKUPSWITCH(NBCEL.generic.LOOKUPSWITCH obj);

		void VisitDUP_X1(NBCEL.generic.DUP_X1 obj);

		void VisitFCMPL(NBCEL.generic.FCMPL obj);

		void VisitI2C(NBCEL.generic.I2C obj);

		void VisitLMUL(NBCEL.generic.LMUL obj);

		void VisitLUSHR(NBCEL.generic.LUSHR obj);

		void VisitISHL(NBCEL.generic.ISHL obj);

		void VisitLALOAD(NBCEL.generic.LALOAD obj);

		void VisitASTORE(NBCEL.generic.ASTORE obj);

		void VisitANEWARRAY(NBCEL.generic.ANEWARRAY obj);

		void VisitFRETURN(NBCEL.generic.FRETURN obj);

		void VisitFADD(NBCEL.generic.FADD obj);

		void VisitBREAKPOINT(NBCEL.generic.BREAKPOINT obj);
	}
}
