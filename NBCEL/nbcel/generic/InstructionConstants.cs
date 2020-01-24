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

namespace NBCEL.generic
{
	/// <summary>This interface contains shareable instruction objects.</summary>
	/// <remarks>
	///     This interface contains shareable instruction objects.
	///     In order to save memory you can use some instructions multiply,
	///     since they have an immutable state and are directly derived from
	///     Instruction.  I.e. they have no instance fields that could be
	///     changed. Since some of these instructions like ICONST_0 occur
	///     very frequently this can save a lot of time and space. This
	///     feature is an adaptation of the FlyWeight design pattern, we
	///     just use an array instead of a factory.
	///     The Instructions can also accessed directly under their names, so
	///     it's possible to write il.append(Instruction.ICONST_0);
	/// </remarks>
	[Obsolete(@"(since 6.0) Do not use. Use InstructionConst instead."
    )]
    public abstract class InstructionConstants
    {
        /// <summary>Predefined instruction objects</summary>
        public static readonly Instruction NOP = new NOP();

        public static readonly Instruction ACONST_NULL = new ACONST_NULL
            ();

        public static readonly Instruction ICONST_M1 = new ICONST(-1);

        public static readonly Instruction ICONST_0 = new ICONST(0);

        public static readonly Instruction ICONST_1 = new ICONST(1);

        public static readonly Instruction ICONST_2 = new ICONST(2);

        public static readonly Instruction ICONST_3 = new ICONST(3);

        public static readonly Instruction ICONST_4 = new ICONST(4);

        public static readonly Instruction ICONST_5 = new ICONST(5);

        public static readonly Instruction LCONST_0 = new LCONST(0);

        public static readonly Instruction LCONST_1 = new LCONST(1);

        public static readonly Instruction FCONST_0 = new FCONST(0);

        public static readonly Instruction FCONST_1 = new FCONST(1);

        public static readonly Instruction FCONST_2 = new FCONST(2);

        public static readonly Instruction DCONST_0 = new DCONST(0);

        public static readonly Instruction DCONST_1 = new DCONST(1);

        public static readonly ArrayInstruction IALOAD = new IALOAD();

        public static readonly ArrayInstruction LALOAD = new LALOAD();

        public static readonly ArrayInstruction FALOAD = new FALOAD();

        public static readonly ArrayInstruction DALOAD = new DALOAD();

        public static readonly ArrayInstruction AALOAD = new AALOAD();

        public static readonly ArrayInstruction BALOAD = new BALOAD();

        public static readonly ArrayInstruction CALOAD = new CALOAD();

        public static readonly ArrayInstruction SALOAD = new SALOAD();

        public static readonly ArrayInstruction IASTORE = new IASTORE();

        public static readonly ArrayInstruction LASTORE = new LASTORE();

        public static readonly ArrayInstruction FASTORE = new FASTORE();

        public static readonly ArrayInstruction DASTORE = new DASTORE();

        public static readonly ArrayInstruction AASTORE = new AASTORE();

        public static readonly ArrayInstruction BASTORE = new BASTORE();

        public static readonly ArrayInstruction CASTORE = new CASTORE();

        public static readonly ArrayInstruction SASTORE = new SASTORE();

        public static readonly StackInstruction POP = new POP();

        public static readonly StackInstruction POP2 = new POP2();

        public static readonly StackInstruction DUP = new DUP();

        public static readonly StackInstruction DUP_X1 = new DUP_X1();

        public static readonly StackInstruction DUP_X2 = new DUP_X2();

        public static readonly StackInstruction DUP2 = new DUP2();

        public static readonly StackInstruction DUP2_X1 = new DUP2_X1();

        public static readonly StackInstruction DUP2_X2 = new DUP2_X2();

        public static readonly StackInstruction SWAP = new SWAP();

        public static readonly ArithmeticInstruction IADD = new IADD();

        public static readonly ArithmeticInstruction LADD = new LADD();

        public static readonly ArithmeticInstruction FADD = new FADD();

        public static readonly ArithmeticInstruction DADD = new DADD();

        public static readonly ArithmeticInstruction ISUB = new ISUB();

        public static readonly ArithmeticInstruction LSUB = new LSUB();

        public static readonly ArithmeticInstruction FSUB = new FSUB();

        public static readonly ArithmeticInstruction DSUB = new DSUB();

        public static readonly ArithmeticInstruction IMUL = new IMUL();

        public static readonly ArithmeticInstruction LMUL = new LMUL();

        public static readonly ArithmeticInstruction FMUL = new FMUL();

        public static readonly ArithmeticInstruction DMUL = new DMUL();

        public static readonly ArithmeticInstruction IDIV = new IDIV();

        public static readonly ArithmeticInstruction LDIV = new LDIV();

        public static readonly ArithmeticInstruction FDIV = new FDIV();

        public static readonly ArithmeticInstruction DDIV = new DDIV();

        public static readonly ArithmeticInstruction IREM = new IREM();

        public static readonly ArithmeticInstruction LREM = new LREM();

        public static readonly ArithmeticInstruction FREM = new FREM();

        public static readonly ArithmeticInstruction DREM = new DREM();

        public static readonly ArithmeticInstruction INEG = new INEG();

        public static readonly ArithmeticInstruction LNEG = new LNEG();

        public static readonly ArithmeticInstruction FNEG = new FNEG();

        public static readonly ArithmeticInstruction DNEG = new DNEG();

        public static readonly ArithmeticInstruction ISHL = new ISHL();

        public static readonly ArithmeticInstruction LSHL = new LSHL();

        public static readonly ArithmeticInstruction ISHR = new ISHR();

        public static readonly ArithmeticInstruction LSHR = new LSHR();

        public static readonly ArithmeticInstruction IUSHR = new IUSHR(
        );

        public static readonly ArithmeticInstruction LUSHR = new LUSHR(
        );

        public static readonly ArithmeticInstruction IAND = new IAND();

        public static readonly ArithmeticInstruction LAND = new LAND();

        public static readonly ArithmeticInstruction IOR = new IOR();

        public static readonly ArithmeticInstruction LOR = new LOR();

        public static readonly ArithmeticInstruction IXOR = new IXOR();

        public static readonly ArithmeticInstruction LXOR = new LXOR();

        public static readonly ConversionInstruction I2L = new I2L();

        public static readonly ConversionInstruction I2F = new I2F();

        public static readonly ConversionInstruction I2D = new I2D();

        public static readonly ConversionInstruction L2I = new L2I();

        public static readonly ConversionInstruction L2F = new L2F();

        public static readonly ConversionInstruction L2D = new L2D();

        public static readonly ConversionInstruction F2I = new F2I();

        public static readonly ConversionInstruction F2L = new F2L();

        public static readonly ConversionInstruction F2D = new F2D();

        public static readonly ConversionInstruction D2I = new D2I();

        public static readonly ConversionInstruction D2L = new D2L();

        public static readonly ConversionInstruction D2F = new D2F();

        public static readonly ConversionInstruction I2B = new I2B();

        public static readonly ConversionInstruction I2C = new I2C();

        public static readonly ConversionInstruction I2S = new I2S();

        public static readonly Instruction LCMP = new LCMP();

        public static readonly Instruction FCMPL = new FCMPL();

        public static readonly Instruction FCMPG = new FCMPG();

        public static readonly Instruction DCMPL = new DCMPL();

        public static readonly Instruction DCMPG = new DCMPG();

        public static readonly ReturnInstruction IRETURN = new IRETURN(
        );

        public static readonly ReturnInstruction LRETURN = new LRETURN(
        );

        public static readonly ReturnInstruction FRETURN = new FRETURN(
        );

        public static readonly ReturnInstruction DRETURN = new DRETURN(
        );

        public static readonly ReturnInstruction ARETURN = new ARETURN(
        );

        public static readonly ReturnInstruction RETURN = new RETURN();

        public static readonly Instruction ARRAYLENGTH = new ARRAYLENGTH
            ();

        public static readonly Instruction ATHROW = new ATHROW();

        public static readonly Instruction MONITORENTER = new MONITORENTER
            ();

        public static readonly Instruction MONITOREXIT = new MONITOREXIT
            ();

        /// <summary>
        ///     You can use these static readonlyants in multiple places safely, if you can guarantee
        ///     that you will never alter their internal values, e.g.
        /// </summary>
        /// <remarks>
        ///     You can use these static readonlyants in multiple places safely, if you can guarantee
        ///     that you will never alter their internal values, e.g. call setIndex().
        /// </remarks>
        public static readonly LocalVariableInstruction THIS = new ALOAD
            (0);

        public static readonly LocalVariableInstruction ALOAD_0 = THIS;

        public static readonly LocalVariableInstruction ALOAD_1 = new ALOAD
            (1);

        public static readonly LocalVariableInstruction ALOAD_2 = new ALOAD
            (2);

        public static readonly LocalVariableInstruction ILOAD_0 = new ILOAD
            (0);

        public static readonly LocalVariableInstruction ILOAD_1 = new ILOAD
            (1);

        public static readonly LocalVariableInstruction ILOAD_2 = new ILOAD
            (2);

        public static readonly LocalVariableInstruction ASTORE_0 = new ASTORE
            (0);

        public static readonly LocalVariableInstruction ASTORE_1 = new ASTORE
            (1);

        public static readonly LocalVariableInstruction ASTORE_2 = new ASTORE
            (2);

        public static readonly LocalVariableInstruction ISTORE_0 = new ISTORE
            (0);

        public static readonly LocalVariableInstruction ISTORE_1 = new ISTORE
            (1);

        public static readonly LocalVariableInstruction ISTORE_2 = new ISTORE
            (2);

        /// <summary>
        ///     Get object via its opcode, for immutable instructions like
        ///     branch instructions entries are set to null.
        /// </summary>
        public static readonly Instruction[] INSTRUCTIONS = new Instruction
            [256];

        /// <summary>
        ///     Interfaces may have no static initializers, so we simulate this
        ///     with an inner class.
        /// </summary>
        public static readonly Clinit bla = new Clinit
            ();

        public class Clinit
        {
            internal Clinit()
            {
                /*
                * NOTE these are not currently immutable, because Instruction
                * has mutable protected fields opcode and length.
                */
                INSTRUCTIONS[Const.NOP] = NOP;
                INSTRUCTIONS[Const.ACONST_NULL] = ACONST_NULL;
                INSTRUCTIONS[Const.ICONST_M1] = ICONST_M1;
                INSTRUCTIONS[Const.ICONST_0] = ICONST_0;
                INSTRUCTIONS[Const.ICONST_1] = ICONST_1;
                INSTRUCTIONS[Const.ICONST_2] = ICONST_2;
                INSTRUCTIONS[Const.ICONST_3] = ICONST_3;
                INSTRUCTIONS[Const.ICONST_4] = ICONST_4;
                INSTRUCTIONS[Const.ICONST_5] = ICONST_5;
                INSTRUCTIONS[Const.LCONST_0] = LCONST_0;
                INSTRUCTIONS[Const.LCONST_1] = LCONST_1;
                INSTRUCTIONS[Const.FCONST_0] = FCONST_0;
                INSTRUCTIONS[Const.FCONST_1] = FCONST_1;
                INSTRUCTIONS[Const.FCONST_2] = FCONST_2;
                INSTRUCTIONS[Const.DCONST_0] = DCONST_0;
                INSTRUCTIONS[Const.DCONST_1] = DCONST_1;
                INSTRUCTIONS[Const.IALOAD] = IALOAD;
                INSTRUCTIONS[Const.LALOAD] = LALOAD;
                INSTRUCTIONS[Const.FALOAD] = FALOAD;
                INSTRUCTIONS[Const.DALOAD] = DALOAD;
                INSTRUCTIONS[Const.AALOAD] = AALOAD;
                INSTRUCTIONS[Const.BALOAD] = BALOAD;
                INSTRUCTIONS[Const.CALOAD] = CALOAD;
                INSTRUCTIONS[Const.SALOAD] = SALOAD;
                INSTRUCTIONS[Const.IASTORE] = IASTORE;
                INSTRUCTIONS[Const.LASTORE] = LASTORE;
                INSTRUCTIONS[Const.FASTORE] = FASTORE;
                INSTRUCTIONS[Const.DASTORE] = DASTORE;
                INSTRUCTIONS[Const.AASTORE] = AASTORE;
                INSTRUCTIONS[Const.BASTORE] = BASTORE;
                INSTRUCTIONS[Const.CASTORE] = CASTORE;
                INSTRUCTIONS[Const.SASTORE] = SASTORE;
                INSTRUCTIONS[Const.POP] = POP;
                INSTRUCTIONS[Const.POP2] = POP2;
                INSTRUCTIONS[Const.DUP] = DUP;
                INSTRUCTIONS[Const.DUP_X1] = DUP_X1;
                INSTRUCTIONS[Const.DUP_X2] = DUP_X2;
                INSTRUCTIONS[Const.DUP2] = DUP2;
                INSTRUCTIONS[Const.DUP2_X1] = DUP2_X1;
                INSTRUCTIONS[Const.DUP2_X2] = DUP2_X2;
                INSTRUCTIONS[Const.SWAP] = SWAP;
                INSTRUCTIONS[Const.IADD] = IADD;
                INSTRUCTIONS[Const.LADD] = LADD;
                INSTRUCTIONS[Const.FADD] = FADD;
                INSTRUCTIONS[Const.DADD] = DADD;
                INSTRUCTIONS[Const.ISUB] = ISUB;
                INSTRUCTIONS[Const.LSUB] = LSUB;
                INSTRUCTIONS[Const.FSUB] = FSUB;
                INSTRUCTIONS[Const.DSUB] = DSUB;
                INSTRUCTIONS[Const.IMUL] = IMUL;
                INSTRUCTIONS[Const.LMUL] = LMUL;
                INSTRUCTIONS[Const.FMUL] = FMUL;
                INSTRUCTIONS[Const.DMUL] = DMUL;
                INSTRUCTIONS[Const.IDIV] = IDIV;
                INSTRUCTIONS[Const.LDIV] = LDIV;
                INSTRUCTIONS[Const.FDIV] = FDIV;
                INSTRUCTIONS[Const.DDIV] = DDIV;
                INSTRUCTIONS[Const.IREM] = IREM;
                INSTRUCTIONS[Const.LREM] = LREM;
                INSTRUCTIONS[Const.FREM] = FREM;
                INSTRUCTIONS[Const.DREM] = DREM;
                INSTRUCTIONS[Const.INEG] = INEG;
                INSTRUCTIONS[Const.LNEG] = LNEG;
                INSTRUCTIONS[Const.FNEG] = FNEG;
                INSTRUCTIONS[Const.DNEG] = DNEG;
                INSTRUCTIONS[Const.ISHL] = ISHL;
                INSTRUCTIONS[Const.LSHL] = LSHL;
                INSTRUCTIONS[Const.ISHR] = ISHR;
                INSTRUCTIONS[Const.LSHR] = LSHR;
                INSTRUCTIONS[Const.IUSHR] = IUSHR;
                INSTRUCTIONS[Const.LUSHR] = LUSHR;
                INSTRUCTIONS[Const.IAND] = IAND;
                INSTRUCTIONS[Const.LAND] = LAND;
                INSTRUCTIONS[Const.IOR] = IOR;
                INSTRUCTIONS[Const.LOR] = LOR;
                INSTRUCTIONS[Const.IXOR] = IXOR;
                INSTRUCTIONS[Const.LXOR] = LXOR;
                INSTRUCTIONS[Const.I2L] = I2L;
                INSTRUCTIONS[Const.I2F] = I2F;
                INSTRUCTIONS[Const.I2D] = I2D;
                INSTRUCTIONS[Const.L2I] = L2I;
                INSTRUCTIONS[Const.L2F] = L2F;
                INSTRUCTIONS[Const.L2D] = L2D;
                INSTRUCTIONS[Const.F2I] = F2I;
                INSTRUCTIONS[Const.F2L] = F2L;
                INSTRUCTIONS[Const.F2D] = F2D;
                INSTRUCTIONS[Const.D2I] = D2I;
                INSTRUCTIONS[Const.D2L] = D2L;
                INSTRUCTIONS[Const.D2F] = D2F;
                INSTRUCTIONS[Const.I2B] = I2B;
                INSTRUCTIONS[Const.I2C] = I2C;
                INSTRUCTIONS[Const.I2S] = I2S;
                INSTRUCTIONS[Const.LCMP] = LCMP;
                INSTRUCTIONS[Const.FCMPL] = FCMPL;
                INSTRUCTIONS[Const.FCMPG] = FCMPG;
                INSTRUCTIONS[Const.DCMPL] = DCMPL;
                INSTRUCTIONS[Const.DCMPG] = DCMPG;
                INSTRUCTIONS[Const.IRETURN] = IRETURN;
                INSTRUCTIONS[Const.LRETURN] = LRETURN;
                INSTRUCTIONS[Const.FRETURN] = FRETURN;
                INSTRUCTIONS[Const.DRETURN] = DRETURN;
                INSTRUCTIONS[Const.ARETURN] = ARETURN;
                INSTRUCTIONS[Const.RETURN] = RETURN;
                INSTRUCTIONS[Const.ARRAYLENGTH] = ARRAYLENGTH;
                INSTRUCTIONS[Const.ATHROW] = ATHROW;
                INSTRUCTIONS[Const.MONITORENTER] = MONITORENTER;
                INSTRUCTIONS[Const.MONITOREXIT] = MONITOREXIT;
            }
        }
    }

    public static class InstructionConstantsConstants
    {
    }
}