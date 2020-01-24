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

namespace Apache.NBCEL.Generic
{
	/// <summary>Interface implementing the Visitor pattern programming style.</summary>
	/// <remarks>
	///     Interface implementing the Visitor pattern programming style.
	///     I.e., a class that implements this interface can handle all types of
	///     instructions with the properly typed methods just by calling the accept()
	///     method.
	/// </remarks>
	public interface Visitor
    {
        void VisitStackInstruction(StackInstruction obj);

        void VisitLocalVariableInstruction(LocalVariableInstruction obj);

        void VisitBranchInstruction(BranchInstruction obj);

        void VisitLoadClass(LoadClass obj);

        void VisitFieldInstruction(FieldInstruction obj);

        void VisitIfInstruction(IfInstruction obj);

        void VisitConversionInstruction(ConversionInstruction obj);

        void VisitPopInstruction(PopInstruction obj);

        void VisitStoreInstruction(StoreInstruction obj);

        void VisitTypedInstruction(TypedInstruction obj);

        void VisitSelect(Select obj);

        void VisitJsrInstruction(JsrInstruction obj);

        void VisitGotoInstruction(GotoInstruction obj);

        void VisitUnconditionalBranch(UnconditionalBranch obj);

        void VisitPushInstruction(PushInstruction obj);

        void VisitArithmeticInstruction(ArithmeticInstruction obj);

        void VisitCPInstruction(CPInstruction obj);

        void VisitInvokeInstruction(InvokeInstruction obj);

        void VisitArrayInstruction(ArrayInstruction obj);

        void VisitAllocationInstruction(AllocationInstruction obj);

        void VisitReturnInstruction(ReturnInstruction obj);

        void VisitFieldOrMethod(FieldOrMethod obj);

        void VisitConstantPushInstruction<T>(ConstantPushInstruction<T> obj);

        void VisitExceptionThrower(ExceptionThrower obj);

        void VisitLoadInstruction(LoadInstruction obj);

        void VisitVariableLengthInstruction(VariableLengthInstruction obj);

        void VisitStackProducer(StackProducer obj);

        void VisitStackConsumer(StackConsumer obj);

        void VisitACONST_NULL(ACONST_NULL obj);

        void VisitGETSTATIC(GETSTATIC obj);

        void VisitIF_ICMPLT(IF_ICMPLT obj);

        void VisitMONITOREXIT(MONITOREXIT obj);

        void VisitIFLT(IFLT obj);

        void VisitLSTORE(LSTORE obj);

        void VisitPOP2(POP2 obj);

        void VisitBASTORE(BASTORE obj);

        void VisitISTORE(ISTORE obj);

        void VisitCHECKCAST(CHECKCAST obj);

        void VisitFCMPG(FCMPG obj);

        void VisitI2F(I2F obj);

        void VisitATHROW(ATHROW obj);

        void VisitDCMPL(DCMPL obj);

        void VisitARRAYLENGTH(ARRAYLENGTH obj);

        void VisitDUP(DUP obj);

        void VisitINVOKESTATIC(INVOKESTATIC obj);

        void VisitLCONST(LCONST obj);

        void VisitDREM(DREM obj);

        void VisitIFGE(IFGE obj);

        void VisitCALOAD(CALOAD obj);

        void VisitLASTORE(LASTORE obj);

        void VisitI2D(I2D obj);

        void VisitDADD(DADD obj);

        void VisitINVOKESPECIAL(INVOKESPECIAL obj);

        void VisitIAND(IAND obj);

        void VisitPUTFIELD(PUTFIELD obj);

        void VisitILOAD(ILOAD obj);

        void VisitDLOAD(DLOAD obj);

        void VisitDCONST(DCONST obj);

        void VisitNEW(NEW obj);

        void VisitIFNULL(IFNULL obj);

        void VisitLSUB(LSUB obj);

        void VisitL2I(L2I obj);

        void VisitISHR(ISHR obj);

        void VisitTABLESWITCH(TABLESWITCH obj);

        void VisitIINC(IINC obj);

        void VisitDRETURN(DRETURN obj);

        void VisitFSTORE(FSTORE obj);

        void VisitDASTORE(DASTORE obj);

        void VisitIALOAD(IALOAD obj);

        void VisitDDIV(DDIV obj);

        void VisitIF_ICMPGE(IF_ICMPGE obj);

        void VisitLAND(LAND obj);

        void VisitIDIV(IDIV obj);

        void VisitLOR(LOR obj);

        void VisitCASTORE(CASTORE obj);

        void VisitFREM(FREM obj);

        void VisitLDC(LDC obj);

        void VisitBIPUSH(BIPUSH obj);

        void VisitDSTORE(DSTORE obj);

        void VisitF2L(F2L obj);

        void VisitFMUL(FMUL obj);

        void VisitLLOAD(LLOAD obj);

        void VisitJSR(JSR obj);

        void VisitFSUB(FSUB obj);

        void VisitSASTORE(SASTORE obj);

        void VisitALOAD(ALOAD obj);

        void VisitDUP2_X2(DUP2_X2 obj);

        void VisitRETURN(RETURN obj);

        void VisitDALOAD(DALOAD obj);

        void VisitSIPUSH(SIPUSH obj);

        void VisitDSUB(DSUB obj);

        void VisitL2F(L2F obj);

        void VisitIF_ICMPGT(IF_ICMPGT obj);

        void VisitF2D(F2D obj);

        void VisitI2L(I2L obj);

        void VisitIF_ACMPNE(IF_ACMPNE obj);

        void VisitPOP(POP obj);

        void VisitI2S(I2S obj);

        void VisitIFEQ(IFEQ obj);

        void VisitSWAP(SWAP obj);

        void VisitIOR(IOR obj);

        void VisitIREM(IREM obj);

        void VisitIASTORE(IASTORE obj);

        void VisitNEWARRAY(NEWARRAY obj);

        void VisitINVOKEINTERFACE(INVOKEINTERFACE obj);

        void VisitINEG(INEG obj);

        void VisitLCMP(LCMP obj);

        void VisitJSR_W(JSR_W obj);

        void VisitMULTIANEWARRAY(MULTIANEWARRAY obj);

        void VisitDUP_X2(DUP_X2 obj);

        void VisitSALOAD(SALOAD obj);

        void VisitIFNONNULL(IFNONNULL obj);

        void VisitDMUL(DMUL obj);

        void VisitIFNE(IFNE obj);

        void VisitIF_ICMPLE(IF_ICMPLE obj);

        void VisitLDC2_W(LDC2_W obj);

        void VisitGETFIELD(GETFIELD obj);

        void VisitLADD(LADD obj);

        void VisitNOP(NOP obj);

        void VisitFALOAD(FALOAD obj);

        void VisitINSTANCEOF(INSTANCEOF obj);

        void VisitIFLE(IFLE obj);

        void VisitLXOR(LXOR obj);

        void VisitLRETURN(LRETURN obj);

        void VisitFCONST(FCONST obj);

        void VisitIUSHR(IUSHR obj);

        void VisitBALOAD(BALOAD obj);

        void VisitDUP2(DUP2 obj);

        void VisitIF_ACMPEQ(IF_ACMPEQ obj);

        void VisitIMPDEP1(IMPDEP1 obj);

        void VisitMONITORENTER(MONITORENTER obj);

        void VisitLSHL(LSHL obj);

        void VisitDCMPG(DCMPG obj);

        void VisitD2L(D2L obj);

        void VisitIMPDEP2(IMPDEP2 obj);

        void VisitL2D(L2D obj);

        void VisitRET(RET obj);

        void VisitIFGT(IFGT obj);

        void VisitIXOR(IXOR obj);

        void VisitINVOKEVIRTUAL(INVOKEVIRTUAL obj);

        /// <since>6.0</since>
        void VisitINVOKEDYNAMIC(INVOKEDYNAMIC obj);

        void VisitFASTORE(FASTORE obj);

        void VisitIRETURN(IRETURN obj);

        void VisitIF_ICMPNE(IF_ICMPNE obj);

        void VisitFLOAD(FLOAD obj);

        void VisitLDIV(LDIV obj);

        void VisitPUTSTATIC(PUTSTATIC obj);

        void VisitAALOAD(AALOAD obj);

        void VisitD2I(D2I obj);

        void VisitIF_ICMPEQ(IF_ICMPEQ obj);

        void VisitAASTORE(AASTORE obj);

        void VisitARETURN(ARETURN obj);

        void VisitDUP2_X1(DUP2_X1 obj);

        void VisitFNEG(FNEG obj);

        void VisitGOTO_W(GOTO_W obj);

        void VisitD2F(D2F obj);

        void VisitGOTO(GOTO obj);

        void VisitISUB(ISUB obj);

        void VisitF2I(F2I obj);

        void VisitDNEG(DNEG obj);

        void VisitICONST(ICONST obj);

        void VisitFDIV(FDIV obj);

        void VisitI2B(I2B obj);

        void VisitLNEG(LNEG obj);

        void VisitLREM(LREM obj);

        void VisitIMUL(IMUL obj);

        void VisitIADD(IADD obj);

        void VisitLSHR(LSHR obj);

        void VisitLOOKUPSWITCH(LOOKUPSWITCH obj);

        void VisitDUP_X1(DUP_X1 obj);

        void VisitFCMPL(FCMPL obj);

        void VisitI2C(I2C obj);

        void VisitLMUL(LMUL obj);

        void VisitLUSHR(LUSHR obj);

        void VisitISHL(ISHL obj);

        void VisitLALOAD(LALOAD obj);

        void VisitASTORE(ASTORE obj);

        void VisitANEWARRAY(ANEWARRAY obj);

        void VisitFRETURN(FRETURN obj);

        void VisitFADD(FADD obj);

        void VisitBREAKPOINT(BREAKPOINT obj);
    }
}