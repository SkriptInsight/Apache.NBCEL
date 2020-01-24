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
    /// <summary>Supplies empty method bodies to be overridden by subclasses.</summary>
    public abstract class EmptyVisitor : Visitor
    {
        public virtual void VisitStackInstruction(StackInstruction obj)
        {
        }

        public virtual void VisitLocalVariableInstruction(LocalVariableInstruction
            obj)
        {
        }

        public virtual void VisitBranchInstruction(BranchInstruction obj)
        {
        }

        public virtual void VisitLoadClass(LoadClass obj)
        {
        }

        public virtual void VisitFieldInstruction(FieldInstruction obj)
        {
        }

        public virtual void VisitIfInstruction(IfInstruction obj)
        {
        }

        public virtual void VisitConversionInstruction(ConversionInstruction
            obj)
        {
        }

        public virtual void VisitPopInstruction(PopInstruction obj)
        {
        }

        public virtual void VisitJsrInstruction(JsrInstruction obj)
        {
        }

        public virtual void VisitGotoInstruction(GotoInstruction obj)
        {
        }

        public virtual void VisitStoreInstruction(StoreInstruction obj)
        {
        }

        public virtual void VisitTypedInstruction(TypedInstruction obj)
        {
        }

        public virtual void VisitSelect(Select obj)
        {
        }

        public virtual void VisitUnconditionalBranch(UnconditionalBranch obj
        )
        {
        }

        public virtual void VisitPushInstruction(PushInstruction obj)
        {
        }

        public virtual void VisitArithmeticInstruction(ArithmeticInstruction
            obj)
        {
        }

        public virtual void VisitCPInstruction(CPInstruction obj)
        {
        }

        public virtual void VisitInvokeInstruction(InvokeInstruction obj)
        {
        }

        public virtual void VisitArrayInstruction(ArrayInstruction obj)
        {
        }

        public virtual void VisitAllocationInstruction(AllocationInstruction
            obj)
        {
        }

        public virtual void VisitReturnInstruction(ReturnInstruction obj)
        {
        }

        public virtual void VisitFieldOrMethod(FieldOrMethod obj)
        {
        }

        public virtual void VisitConstantPushInstruction<T>(ConstantPushInstruction<T>
            obj)
        {
        }

        public virtual void VisitExceptionThrower(ExceptionThrower obj)
        {
        }

        public virtual void VisitLoadInstruction(LoadInstruction obj)
        {
        }

        public virtual void VisitVariableLengthInstruction(VariableLengthInstruction
            obj)
        {
        }

        public virtual void VisitStackProducer(StackProducer obj)
        {
        }

        public virtual void VisitStackConsumer(StackConsumer obj)
        {
        }

        public virtual void VisitACONST_NULL(ACONST_NULL obj)
        {
        }

        public virtual void VisitGETSTATIC(GETSTATIC obj)
        {
        }

        public virtual void VisitIF_ICMPLT(IF_ICMPLT obj)
        {
        }

        public virtual void VisitMONITOREXIT(MONITOREXIT obj)
        {
        }

        public virtual void VisitIFLT(IFLT obj)
        {
        }

        public virtual void VisitLSTORE(LSTORE obj)
        {
        }

        public virtual void VisitPOP2(POP2 obj)
        {
        }

        public virtual void VisitBASTORE(BASTORE obj)
        {
        }

        public virtual void VisitISTORE(ISTORE obj)
        {
        }

        public virtual void VisitCHECKCAST(CHECKCAST obj)
        {
        }

        public virtual void VisitFCMPG(FCMPG obj)
        {
        }

        public virtual void VisitI2F(I2F obj)
        {
        }

        public virtual void VisitATHROW(ATHROW obj)
        {
        }

        public virtual void VisitDCMPL(DCMPL obj)
        {
        }

        public virtual void VisitARRAYLENGTH(ARRAYLENGTH obj)
        {
        }

        public virtual void VisitDUP(DUP obj)
        {
        }

        public virtual void VisitINVOKESTATIC(INVOKESTATIC obj)
        {
        }

        public virtual void VisitLCONST(LCONST obj)
        {
        }

        public virtual void VisitDREM(DREM obj)
        {
        }

        public virtual void VisitIFGE(IFGE obj)
        {
        }

        public virtual void VisitCALOAD(CALOAD obj)
        {
        }

        public virtual void VisitLASTORE(LASTORE obj)
        {
        }

        public virtual void VisitI2D(I2D obj)
        {
        }

        public virtual void VisitDADD(DADD obj)
        {
        }

        public virtual void VisitINVOKESPECIAL(INVOKESPECIAL obj)
        {
        }

        public virtual void VisitIAND(IAND obj)
        {
        }

        public virtual void VisitPUTFIELD(PUTFIELD obj)
        {
        }

        public virtual void VisitILOAD(ILOAD obj)
        {
        }

        public virtual void VisitDLOAD(DLOAD obj)
        {
        }

        public virtual void VisitDCONST(DCONST obj)
        {
        }

        public virtual void VisitNEW(NEW obj)
        {
        }

        public virtual void VisitIFNULL(IFNULL obj)
        {
        }

        public virtual void VisitLSUB(LSUB obj)
        {
        }

        public virtual void VisitL2I(L2I obj)
        {
        }

        public virtual void VisitISHR(ISHR obj)
        {
        }

        public virtual void VisitTABLESWITCH(TABLESWITCH obj)
        {
        }

        public virtual void VisitIINC(IINC obj)
        {
        }

        public virtual void VisitDRETURN(DRETURN obj)
        {
        }

        public virtual void VisitFSTORE(FSTORE obj)
        {
        }

        public virtual void VisitDASTORE(DASTORE obj)
        {
        }

        public virtual void VisitIALOAD(IALOAD obj)
        {
        }

        public virtual void VisitDDIV(DDIV obj)
        {
        }

        public virtual void VisitIF_ICMPGE(IF_ICMPGE obj)
        {
        }

        public virtual void VisitLAND(LAND obj)
        {
        }

        public virtual void VisitIDIV(IDIV obj)
        {
        }

        public virtual void VisitLOR(LOR obj)
        {
        }

        public virtual void VisitCASTORE(CASTORE obj)
        {
        }

        public virtual void VisitFREM(FREM obj)
        {
        }

        public virtual void VisitLDC(LDC obj)
        {
        }

        public virtual void VisitBIPUSH(BIPUSH obj)
        {
        }

        public virtual void VisitDSTORE(DSTORE obj)
        {
        }

        public virtual void VisitF2L(F2L obj)
        {
        }

        public virtual void VisitFMUL(FMUL obj)
        {
        }

        public virtual void VisitLLOAD(LLOAD obj)
        {
        }

        public virtual void VisitJSR(JSR obj)
        {
        }

        public virtual void VisitFSUB(FSUB obj)
        {
        }

        public virtual void VisitSASTORE(SASTORE obj)
        {
        }

        public virtual void VisitALOAD(ALOAD obj)
        {
        }

        public virtual void VisitDUP2_X2(DUP2_X2 obj)
        {
        }

        public virtual void VisitRETURN(RETURN obj)
        {
        }

        public virtual void VisitDALOAD(DALOAD obj)
        {
        }

        public virtual void VisitSIPUSH(SIPUSH obj)
        {
        }

        public virtual void VisitDSUB(DSUB obj)
        {
        }

        public virtual void VisitL2F(L2F obj)
        {
        }

        public virtual void VisitIF_ICMPGT(IF_ICMPGT obj)
        {
        }

        public virtual void VisitF2D(F2D obj)
        {
        }

        public virtual void VisitI2L(I2L obj)
        {
        }

        public virtual void VisitIF_ACMPNE(IF_ACMPNE obj)
        {
        }

        public virtual void VisitPOP(POP obj)
        {
        }

        public virtual void VisitI2S(I2S obj)
        {
        }

        public virtual void VisitIFEQ(IFEQ obj)
        {
        }

        public virtual void VisitSWAP(SWAP obj)
        {
        }

        public virtual void VisitIOR(IOR obj)
        {
        }

        public virtual void VisitIREM(IREM obj)
        {
        }

        public virtual void VisitIASTORE(IASTORE obj)
        {
        }

        public virtual void VisitNEWARRAY(NEWARRAY obj)
        {
        }

        public virtual void VisitINVOKEINTERFACE(INVOKEINTERFACE obj)
        {
        }

        public virtual void VisitINEG(INEG obj)
        {
        }

        public virtual void VisitLCMP(LCMP obj)
        {
        }

        public virtual void VisitJSR_W(JSR_W obj)
        {
        }

        public virtual void VisitMULTIANEWARRAY(MULTIANEWARRAY obj)
        {
        }

        public virtual void VisitDUP_X2(DUP_X2 obj)
        {
        }

        public virtual void VisitSALOAD(SALOAD obj)
        {
        }

        public virtual void VisitIFNONNULL(IFNONNULL obj)
        {
        }

        public virtual void VisitDMUL(DMUL obj)
        {
        }

        public virtual void VisitIFNE(IFNE obj)
        {
        }

        public virtual void VisitIF_ICMPLE(IF_ICMPLE obj)
        {
        }

        public virtual void VisitLDC2_W(LDC2_W obj)
        {
        }

        public virtual void VisitGETFIELD(GETFIELD obj)
        {
        }

        public virtual void VisitLADD(LADD obj)
        {
        }

        public virtual void VisitNOP(NOP obj)
        {
        }

        public virtual void VisitFALOAD(FALOAD obj)
        {
        }

        public virtual void VisitINSTANCEOF(INSTANCEOF obj)
        {
        }

        public virtual void VisitIFLE(IFLE obj)
        {
        }

        public virtual void VisitLXOR(LXOR obj)
        {
        }

        public virtual void VisitLRETURN(LRETURN obj)
        {
        }

        public virtual void VisitFCONST(FCONST obj)
        {
        }

        public virtual void VisitIUSHR(IUSHR obj)
        {
        }

        public virtual void VisitBALOAD(BALOAD obj)
        {
        }

        public virtual void VisitDUP2(DUP2 obj)
        {
        }

        public virtual void VisitIF_ACMPEQ(IF_ACMPEQ obj)
        {
        }

        public virtual void VisitIMPDEP1(IMPDEP1 obj)
        {
        }

        public virtual void VisitMONITORENTER(MONITORENTER obj)
        {
        }

        public virtual void VisitLSHL(LSHL obj)
        {
        }

        public virtual void VisitDCMPG(DCMPG obj)
        {
        }

        public virtual void VisitD2L(D2L obj)
        {
        }

        public virtual void VisitIMPDEP2(IMPDEP2 obj)
        {
        }

        public virtual void VisitL2D(L2D obj)
        {
        }

        public virtual void VisitRET(RET obj)
        {
        }

        public virtual void VisitIFGT(IFGT obj)
        {
        }

        public virtual void VisitIXOR(IXOR obj)
        {
        }

        public virtual void VisitINVOKEVIRTUAL(INVOKEVIRTUAL obj)
        {
        }

        public virtual void VisitFASTORE(FASTORE obj)
        {
        }

        public virtual void VisitIRETURN(IRETURN obj)
        {
        }

        public virtual void VisitIF_ICMPNE(IF_ICMPNE obj)
        {
        }

        public virtual void VisitFLOAD(FLOAD obj)
        {
        }

        public virtual void VisitLDIV(LDIV obj)
        {
        }

        public virtual void VisitPUTSTATIC(PUTSTATIC obj)
        {
        }

        public virtual void VisitAALOAD(AALOAD obj)
        {
        }

        public virtual void VisitD2I(D2I obj)
        {
        }

        public virtual void VisitIF_ICMPEQ(IF_ICMPEQ obj)
        {
        }

        public virtual void VisitAASTORE(AASTORE obj)
        {
        }

        public virtual void VisitARETURN(ARETURN obj)
        {
        }

        public virtual void VisitDUP2_X1(DUP2_X1 obj)
        {
        }

        public virtual void VisitFNEG(FNEG obj)
        {
        }

        public virtual void VisitGOTO_W(GOTO_W obj)
        {
        }

        public virtual void VisitD2F(D2F obj)
        {
        }

        public virtual void VisitGOTO(GOTO obj)
        {
        }

        public virtual void VisitISUB(ISUB obj)
        {
        }

        public virtual void VisitF2I(F2I obj)
        {
        }

        public virtual void VisitDNEG(DNEG obj)
        {
        }

        public virtual void VisitICONST(ICONST obj)
        {
        }

        public virtual void VisitFDIV(FDIV obj)
        {
        }

        public virtual void VisitI2B(I2B obj)
        {
        }

        public virtual void VisitLNEG(LNEG obj)
        {
        }

        public virtual void VisitLREM(LREM obj)
        {
        }

        public virtual void VisitIMUL(IMUL obj)
        {
        }

        public virtual void VisitIADD(IADD obj)
        {
        }

        public virtual void VisitLSHR(LSHR obj)
        {
        }

        public virtual void VisitLOOKUPSWITCH(LOOKUPSWITCH obj)
        {
        }

        public virtual void VisitDUP_X1(DUP_X1 obj)
        {
        }

        public virtual void VisitFCMPL(FCMPL obj)
        {
        }

        public virtual void VisitI2C(I2C obj)
        {
        }

        public virtual void VisitLMUL(LMUL obj)
        {
        }

        public virtual void VisitLUSHR(LUSHR obj)
        {
        }

        public virtual void VisitISHL(ISHL obj)
        {
        }

        public virtual void VisitLALOAD(LALOAD obj)
        {
        }

        public virtual void VisitASTORE(ASTORE obj)
        {
        }

        public virtual void VisitANEWARRAY(ANEWARRAY obj)
        {
        }

        public virtual void VisitFRETURN(FRETURN obj)
        {
        }

        public virtual void VisitFADD(FADD obj)
        {
        }

        public virtual void VisitBREAKPOINT(BREAKPOINT obj)
        {
        }

        /// <since>6.0</since>
        public virtual void VisitINVOKEDYNAMIC(INVOKEDYNAMIC obj)
        {
        }
    }
}