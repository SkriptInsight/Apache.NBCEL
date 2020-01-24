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
	/// <summary>This interface contains shareable instruction objects.</summary>
	/// <remarks>
	/// This interface contains shareable instruction objects.
	/// In order to save memory you can use some instructions multiply,
	/// since they have an immutable state and are directly derived from
	/// Instruction.  I.e. they have no instance fields that could be
	/// changed. Since some of these instructions like ICONST_0 occur
	/// very frequently this can save a lot of time and space. This
	/// feature is an adaptation of the FlyWeight design pattern, we
	/// just use an array instead of a factory.
	/// The Instructions can also accessed directly under their names, so
	/// it's possible to write il.append(Instruction.ICONST_0);
	/// </remarks>
	[System.ObsoleteAttribute(@"(since 6.0) Do not use. Use InstructionConst instead."
		)]
	public abstract class InstructionConstants
	{
		/// <summary>Predefined instruction objects</summary>
		public static readonly NBCEL.generic.Instruction NOP = new NBCEL.generic.NOP();

		public static readonly NBCEL.generic.Instruction ACONST_NULL = new NBCEL.generic.ACONST_NULL
			();

		public static readonly NBCEL.generic.Instruction ICONST_M1 = new NBCEL.generic.ICONST(-1);

		public static readonly NBCEL.generic.Instruction ICONST_0 = new NBCEL.generic.ICONST(0);

		public static readonly NBCEL.generic.Instruction ICONST_1 = new NBCEL.generic.ICONST(1);

		public static readonly NBCEL.generic.Instruction ICONST_2 = new NBCEL.generic.ICONST(2);

		public static readonly NBCEL.generic.Instruction ICONST_3 = new NBCEL.generic.ICONST(3);

		public static readonly NBCEL.generic.Instruction ICONST_4 = new NBCEL.generic.ICONST(4);

		public static readonly NBCEL.generic.Instruction ICONST_5 = new NBCEL.generic.ICONST(5);

		public static readonly NBCEL.generic.Instruction LCONST_0 = new NBCEL.generic.LCONST(0);

		public static readonly NBCEL.generic.Instruction LCONST_1 = new NBCEL.generic.LCONST(1);

		public static readonly NBCEL.generic.Instruction FCONST_0 = new NBCEL.generic.FCONST(0);

		public static readonly NBCEL.generic.Instruction FCONST_1 = new NBCEL.generic.FCONST(1);

		public static readonly NBCEL.generic.Instruction FCONST_2 = new NBCEL.generic.FCONST(2);

		public static readonly NBCEL.generic.Instruction DCONST_0 = new NBCEL.generic.DCONST(0);

		public static readonly NBCEL.generic.Instruction DCONST_1 = new NBCEL.generic.DCONST(1);

		public static readonly NBCEL.generic.ArrayInstruction IALOAD = new NBCEL.generic.IALOAD();

		public static readonly NBCEL.generic.ArrayInstruction LALOAD = new NBCEL.generic.LALOAD();

		public static readonly NBCEL.generic.ArrayInstruction FALOAD = new NBCEL.generic.FALOAD();

		public static readonly NBCEL.generic.ArrayInstruction DALOAD = new NBCEL.generic.DALOAD();

		public static readonly NBCEL.generic.ArrayInstruction AALOAD = new NBCEL.generic.AALOAD();

		public static readonly NBCEL.generic.ArrayInstruction BALOAD = new NBCEL.generic.BALOAD();

		public static readonly NBCEL.generic.ArrayInstruction CALOAD = new NBCEL.generic.CALOAD();

		public static readonly NBCEL.generic.ArrayInstruction SALOAD = new NBCEL.generic.SALOAD();

		public static readonly NBCEL.generic.ArrayInstruction IASTORE = new NBCEL.generic.IASTORE();

		public static readonly NBCEL.generic.ArrayInstruction LASTORE = new NBCEL.generic.LASTORE();

		public static readonly NBCEL.generic.ArrayInstruction FASTORE = new NBCEL.generic.FASTORE();

		public static readonly NBCEL.generic.ArrayInstruction DASTORE = new NBCEL.generic.DASTORE();

		public static readonly NBCEL.generic.ArrayInstruction AASTORE = new NBCEL.generic.AASTORE();

		public static readonly NBCEL.generic.ArrayInstruction BASTORE = new NBCEL.generic.BASTORE();

		public static readonly NBCEL.generic.ArrayInstruction CASTORE = new NBCEL.generic.CASTORE();

		public static readonly NBCEL.generic.ArrayInstruction SASTORE = new NBCEL.generic.SASTORE();

		public static readonly NBCEL.generic.StackInstruction POP = new NBCEL.generic.POP();

		public static readonly NBCEL.generic.StackInstruction POP2 = new NBCEL.generic.POP2();

		public static readonly NBCEL.generic.StackInstruction DUP = new NBCEL.generic.DUP();

		public static readonly NBCEL.generic.StackInstruction DUP_X1 = new NBCEL.generic.DUP_X1();

		public static readonly NBCEL.generic.StackInstruction DUP_X2 = new NBCEL.generic.DUP_X2();

		public static readonly NBCEL.generic.StackInstruction DUP2 = new NBCEL.generic.DUP2();

		public static readonly NBCEL.generic.StackInstruction DUP2_X1 = new NBCEL.generic.DUP2_X1();

		public static readonly NBCEL.generic.StackInstruction DUP2_X2 = new NBCEL.generic.DUP2_X2();

		public static readonly NBCEL.generic.StackInstruction SWAP = new NBCEL.generic.SWAP();

		public static readonly NBCEL.generic.ArithmeticInstruction IADD = new NBCEL.generic.IADD();

		public static readonly NBCEL.generic.ArithmeticInstruction LADD = new NBCEL.generic.LADD();

		public static readonly NBCEL.generic.ArithmeticInstruction FADD = new NBCEL.generic.FADD();

		public static readonly NBCEL.generic.ArithmeticInstruction DADD = new NBCEL.generic.DADD();

		public static readonly NBCEL.generic.ArithmeticInstruction ISUB = new NBCEL.generic.ISUB();

		public static readonly NBCEL.generic.ArithmeticInstruction LSUB = new NBCEL.generic.LSUB();

		public static readonly NBCEL.generic.ArithmeticInstruction FSUB = new NBCEL.generic.FSUB();

		public static readonly NBCEL.generic.ArithmeticInstruction DSUB = new NBCEL.generic.DSUB();

		public static readonly NBCEL.generic.ArithmeticInstruction IMUL = new NBCEL.generic.IMUL();

		public static readonly NBCEL.generic.ArithmeticInstruction LMUL = new NBCEL.generic.LMUL();

		public static readonly NBCEL.generic.ArithmeticInstruction FMUL = new NBCEL.generic.FMUL();

		public static readonly NBCEL.generic.ArithmeticInstruction DMUL = new NBCEL.generic.DMUL();

		public static readonly NBCEL.generic.ArithmeticInstruction IDIV = new NBCEL.generic.IDIV();

		public static readonly NBCEL.generic.ArithmeticInstruction LDIV = new NBCEL.generic.LDIV();

		public static readonly NBCEL.generic.ArithmeticInstruction FDIV = new NBCEL.generic.FDIV();

		public static readonly NBCEL.generic.ArithmeticInstruction DDIV = new NBCEL.generic.DDIV();

		public static readonly NBCEL.generic.ArithmeticInstruction IREM = new NBCEL.generic.IREM();

		public static readonly NBCEL.generic.ArithmeticInstruction LREM = new NBCEL.generic.LREM();

		public static readonly NBCEL.generic.ArithmeticInstruction FREM = new NBCEL.generic.FREM();

		public static readonly NBCEL.generic.ArithmeticInstruction DREM = new NBCEL.generic.DREM();

		public static readonly NBCEL.generic.ArithmeticInstruction INEG = new NBCEL.generic.INEG();

		public static readonly NBCEL.generic.ArithmeticInstruction LNEG = new NBCEL.generic.LNEG();

		public static readonly NBCEL.generic.ArithmeticInstruction FNEG = new NBCEL.generic.FNEG();

		public static readonly NBCEL.generic.ArithmeticInstruction DNEG = new NBCEL.generic.DNEG();

		public static readonly NBCEL.generic.ArithmeticInstruction ISHL = new NBCEL.generic.ISHL();

		public static readonly NBCEL.generic.ArithmeticInstruction LSHL = new NBCEL.generic.LSHL();

		public static readonly NBCEL.generic.ArithmeticInstruction ISHR = new NBCEL.generic.ISHR();

		public static readonly NBCEL.generic.ArithmeticInstruction LSHR = new NBCEL.generic.LSHR();

		public static readonly NBCEL.generic.ArithmeticInstruction IUSHR = new NBCEL.generic.IUSHR(
			);

		public static readonly NBCEL.generic.ArithmeticInstruction LUSHR = new NBCEL.generic.LUSHR(
			);

		public static readonly NBCEL.generic.ArithmeticInstruction IAND = new NBCEL.generic.IAND();

		public static readonly NBCEL.generic.ArithmeticInstruction LAND = new NBCEL.generic.LAND();

		public static readonly NBCEL.generic.ArithmeticInstruction IOR = new NBCEL.generic.IOR();

		public static readonly NBCEL.generic.ArithmeticInstruction LOR = new NBCEL.generic.LOR();

		public static readonly NBCEL.generic.ArithmeticInstruction IXOR = new NBCEL.generic.IXOR();

		public static readonly NBCEL.generic.ArithmeticInstruction LXOR = new NBCEL.generic.LXOR();

		public static readonly NBCEL.generic.ConversionInstruction I2L = new NBCEL.generic.I2L();

		public static readonly NBCEL.generic.ConversionInstruction I2F = new NBCEL.generic.I2F();

		public static readonly NBCEL.generic.ConversionInstruction I2D = new NBCEL.generic.I2D();

		public static readonly NBCEL.generic.ConversionInstruction L2I = new NBCEL.generic.L2I();

		public static readonly NBCEL.generic.ConversionInstruction L2F = new NBCEL.generic.L2F();

		public static readonly NBCEL.generic.ConversionInstruction L2D = new NBCEL.generic.L2D();

		public static readonly NBCEL.generic.ConversionInstruction F2I = new NBCEL.generic.F2I();

		public static readonly NBCEL.generic.ConversionInstruction F2L = new NBCEL.generic.F2L();

		public static readonly NBCEL.generic.ConversionInstruction F2D = new NBCEL.generic.F2D();

		public static readonly NBCEL.generic.ConversionInstruction D2I = new NBCEL.generic.D2I();

		public static readonly NBCEL.generic.ConversionInstruction D2L = new NBCEL.generic.D2L();

		public static readonly NBCEL.generic.ConversionInstruction D2F = new NBCEL.generic.D2F();

		public static readonly NBCEL.generic.ConversionInstruction I2B = new NBCEL.generic.I2B();

		public static readonly NBCEL.generic.ConversionInstruction I2C = new NBCEL.generic.I2C();

		public static readonly NBCEL.generic.ConversionInstruction I2S = new NBCEL.generic.I2S();

		public static readonly NBCEL.generic.Instruction LCMP = new NBCEL.generic.LCMP();

		public static readonly NBCEL.generic.Instruction FCMPL = new NBCEL.generic.FCMPL();

		public static readonly NBCEL.generic.Instruction FCMPG = new NBCEL.generic.FCMPG();

		public static readonly NBCEL.generic.Instruction DCMPL = new NBCEL.generic.DCMPL();

		public static readonly NBCEL.generic.Instruction DCMPG = new NBCEL.generic.DCMPG();

		public static readonly NBCEL.generic.ReturnInstruction IRETURN = new NBCEL.generic.IRETURN(
			);

		public static readonly NBCEL.generic.ReturnInstruction LRETURN = new NBCEL.generic.LRETURN(
			);

		public static readonly NBCEL.generic.ReturnInstruction FRETURN = new NBCEL.generic.FRETURN(
			);

		public static readonly NBCEL.generic.ReturnInstruction DRETURN = new NBCEL.generic.DRETURN(
			);

		public static readonly NBCEL.generic.ReturnInstruction ARETURN = new NBCEL.generic.ARETURN(
			);

		public static readonly NBCEL.generic.ReturnInstruction RETURN = new NBCEL.generic.RETURN();

		public static readonly NBCEL.generic.Instruction ARRAYLENGTH = new NBCEL.generic.ARRAYLENGTH
			();

		public static readonly NBCEL.generic.Instruction ATHROW = new NBCEL.generic.ATHROW();

		public static readonly NBCEL.generic.Instruction MONITORENTER = new NBCEL.generic.MONITORENTER
			();

		public static readonly NBCEL.generic.Instruction MONITOREXIT = new NBCEL.generic.MONITOREXIT
			();

		/// <summary>
		/// You can use these static readonlyants in multiple places safely, if you can guarantee
		/// that you will never alter their internal values, e.g.
		/// </summary>
		/// <remarks>
		/// You can use these static readonlyants in multiple places safely, if you can guarantee
		/// that you will never alter their internal values, e.g. call setIndex().
		/// </remarks>
		public static readonly NBCEL.generic.LocalVariableInstruction THIS = new NBCEL.generic.ALOAD
			(0);

		public static readonly NBCEL.generic.LocalVariableInstruction ALOAD_0 = THIS;

		public static readonly NBCEL.generic.LocalVariableInstruction ALOAD_1 = new NBCEL.generic.ALOAD
			(1);

		public static readonly NBCEL.generic.LocalVariableInstruction ALOAD_2 = new NBCEL.generic.ALOAD
			(2);

		public static readonly NBCEL.generic.LocalVariableInstruction ILOAD_0 = new NBCEL.generic.ILOAD
			(0);

		public static readonly NBCEL.generic.LocalVariableInstruction ILOAD_1 = new NBCEL.generic.ILOAD
			(1);

		public static readonly NBCEL.generic.LocalVariableInstruction ILOAD_2 = new NBCEL.generic.ILOAD
			(2);

		public static readonly NBCEL.generic.LocalVariableInstruction ASTORE_0 = new NBCEL.generic.ASTORE
			(0);

		public static readonly NBCEL.generic.LocalVariableInstruction ASTORE_1 = new NBCEL.generic.ASTORE
			(1);

		public static readonly NBCEL.generic.LocalVariableInstruction ASTORE_2 = new NBCEL.generic.ASTORE
			(2);

		public static readonly NBCEL.generic.LocalVariableInstruction ISTORE_0 = new NBCEL.generic.ISTORE
			(0);

		public static readonly NBCEL.generic.LocalVariableInstruction ISTORE_1 = new NBCEL.generic.ISTORE
			(1);

		public static readonly NBCEL.generic.LocalVariableInstruction ISTORE_2 = new NBCEL.generic.ISTORE
			(2);

		/// <summary>
		/// Get object via its opcode, for immutable instructions like
		/// branch instructions entries are set to null.
		/// </summary>
		public static readonly NBCEL.generic.Instruction[] INSTRUCTIONS = new NBCEL.generic.Instruction
			[256];

		/// <summary>
		/// Interfaces may have no static initializers, so we simulate this
		/// with an inner class.
		/// </summary>
		public static readonly NBCEL.generic.InstructionConstants.Clinit bla = new NBCEL.generic.InstructionConstants.Clinit
			();

		public class Clinit
		{
			internal Clinit()
			{
				/*
				* NOTE these are not currently immutable, because Instruction
				* has mutable protected fields opcode and length.
				*/
				INSTRUCTIONS[NBCEL.Const.NOP] = NOP;
				INSTRUCTIONS[NBCEL.Const.ACONST_NULL] = ACONST_NULL;
				INSTRUCTIONS[NBCEL.Const.ICONST_M1] = ICONST_M1;
				INSTRUCTIONS[NBCEL.Const.ICONST_0] = ICONST_0;
				INSTRUCTIONS[NBCEL.Const.ICONST_1] = ICONST_1;
				INSTRUCTIONS[NBCEL.Const.ICONST_2] = ICONST_2;
				INSTRUCTIONS[NBCEL.Const.ICONST_3] = ICONST_3;
				INSTRUCTIONS[NBCEL.Const.ICONST_4] = ICONST_4;
				INSTRUCTIONS[NBCEL.Const.ICONST_5] = ICONST_5;
				INSTRUCTIONS[NBCEL.Const.LCONST_0] = LCONST_0;
				INSTRUCTIONS[NBCEL.Const.LCONST_1] = LCONST_1;
				INSTRUCTIONS[NBCEL.Const.FCONST_0] = FCONST_0;
				INSTRUCTIONS[NBCEL.Const.FCONST_1] = FCONST_1;
				INSTRUCTIONS[NBCEL.Const.FCONST_2] = FCONST_2;
				INSTRUCTIONS[NBCEL.Const.DCONST_0] = DCONST_0;
				INSTRUCTIONS[NBCEL.Const.DCONST_1] = DCONST_1;
				INSTRUCTIONS[NBCEL.Const.IALOAD] = IALOAD;
				INSTRUCTIONS[NBCEL.Const.LALOAD] = LALOAD;
				INSTRUCTIONS[NBCEL.Const.FALOAD] = FALOAD;
				INSTRUCTIONS[NBCEL.Const.DALOAD] = DALOAD;
				INSTRUCTIONS[NBCEL.Const.AALOAD] = AALOAD;
				INSTRUCTIONS[NBCEL.Const.BALOAD] = BALOAD;
				INSTRUCTIONS[NBCEL.Const.CALOAD] = CALOAD;
				INSTRUCTIONS[NBCEL.Const.SALOAD] = SALOAD;
				INSTRUCTIONS[NBCEL.Const.IASTORE] = IASTORE;
				INSTRUCTIONS[NBCEL.Const.LASTORE] = LASTORE;
				INSTRUCTIONS[NBCEL.Const.FASTORE] = FASTORE;
				INSTRUCTIONS[NBCEL.Const.DASTORE] = DASTORE;
				INSTRUCTIONS[NBCEL.Const.AASTORE] = AASTORE;
				INSTRUCTIONS[NBCEL.Const.BASTORE] = BASTORE;
				INSTRUCTIONS[NBCEL.Const.CASTORE] = CASTORE;
				INSTRUCTIONS[NBCEL.Const.SASTORE] = SASTORE;
				INSTRUCTIONS[NBCEL.Const.POP] = POP;
				INSTRUCTIONS[NBCEL.Const.POP2] = POP2;
				INSTRUCTIONS[NBCEL.Const.DUP] = DUP;
				INSTRUCTIONS[NBCEL.Const.DUP_X1] = DUP_X1;
				INSTRUCTIONS[NBCEL.Const.DUP_X2] = DUP_X2;
				INSTRUCTIONS[NBCEL.Const.DUP2] = DUP2;
				INSTRUCTIONS[NBCEL.Const.DUP2_X1] = DUP2_X1;
				INSTRUCTIONS[NBCEL.Const.DUP2_X2] = DUP2_X2;
				INSTRUCTIONS[NBCEL.Const.SWAP] = SWAP;
				INSTRUCTIONS[NBCEL.Const.IADD] = IADD;
				INSTRUCTIONS[NBCEL.Const.LADD] = LADD;
				INSTRUCTIONS[NBCEL.Const.FADD] = FADD;
				INSTRUCTIONS[NBCEL.Const.DADD] = DADD;
				INSTRUCTIONS[NBCEL.Const.ISUB] = ISUB;
				INSTRUCTIONS[NBCEL.Const.LSUB] = LSUB;
				INSTRUCTIONS[NBCEL.Const.FSUB] = FSUB;
				INSTRUCTIONS[NBCEL.Const.DSUB] = DSUB;
				INSTRUCTIONS[NBCEL.Const.IMUL] = IMUL;
				INSTRUCTIONS[NBCEL.Const.LMUL] = LMUL;
				INSTRUCTIONS[NBCEL.Const.FMUL] = FMUL;
				INSTRUCTIONS[NBCEL.Const.DMUL] = DMUL;
				INSTRUCTIONS[NBCEL.Const.IDIV] = IDIV;
				INSTRUCTIONS[NBCEL.Const.LDIV] = LDIV;
				INSTRUCTIONS[NBCEL.Const.FDIV] = FDIV;
				INSTRUCTIONS[NBCEL.Const.DDIV] = DDIV;
				INSTRUCTIONS[NBCEL.Const.IREM] = IREM;
				INSTRUCTIONS[NBCEL.Const.LREM] = LREM;
				INSTRUCTIONS[NBCEL.Const.FREM] = FREM;
				INSTRUCTIONS[NBCEL.Const.DREM] = DREM;
				INSTRUCTIONS[NBCEL.Const.INEG] = INEG;
				INSTRUCTIONS[NBCEL.Const.LNEG] = LNEG;
				INSTRUCTIONS[NBCEL.Const.FNEG] = FNEG;
				INSTRUCTIONS[NBCEL.Const.DNEG] = DNEG;
				INSTRUCTIONS[NBCEL.Const.ISHL] = ISHL;
				INSTRUCTIONS[NBCEL.Const.LSHL] = LSHL;
				INSTRUCTIONS[NBCEL.Const.ISHR] = ISHR;
				INSTRUCTIONS[NBCEL.Const.LSHR] = LSHR;
				INSTRUCTIONS[NBCEL.Const.IUSHR] = IUSHR;
				INSTRUCTIONS[NBCEL.Const.LUSHR] = LUSHR;
				INSTRUCTIONS[NBCEL.Const.IAND] = IAND;
				INSTRUCTIONS[NBCEL.Const.LAND] = LAND;
				INSTRUCTIONS[NBCEL.Const.IOR] = IOR;
				INSTRUCTIONS[NBCEL.Const.LOR] = LOR;
				INSTRUCTIONS[NBCEL.Const.IXOR] = IXOR;
				INSTRUCTIONS[NBCEL.Const.LXOR] = LXOR;
				INSTRUCTIONS[NBCEL.Const.I2L] = I2L;
				INSTRUCTIONS[NBCEL.Const.I2F] = I2F;
				INSTRUCTIONS[NBCEL.Const.I2D] = I2D;
				INSTRUCTIONS[NBCEL.Const.L2I] = L2I;
				INSTRUCTIONS[NBCEL.Const.L2F] = L2F;
				INSTRUCTIONS[NBCEL.Const.L2D] = L2D;
				INSTRUCTIONS[NBCEL.Const.F2I] = F2I;
				INSTRUCTIONS[NBCEL.Const.F2L] = F2L;
				INSTRUCTIONS[NBCEL.Const.F2D] = F2D;
				INSTRUCTIONS[NBCEL.Const.D2I] = D2I;
				INSTRUCTIONS[NBCEL.Const.D2L] = D2L;
				INSTRUCTIONS[NBCEL.Const.D2F] = D2F;
				INSTRUCTIONS[NBCEL.Const.I2B] = I2B;
				INSTRUCTIONS[NBCEL.Const.I2C] = I2C;
				INSTRUCTIONS[NBCEL.Const.I2S] = I2S;
				INSTRUCTIONS[NBCEL.Const.LCMP] = LCMP;
				INSTRUCTIONS[NBCEL.Const.FCMPL] = FCMPL;
				INSTRUCTIONS[NBCEL.Const.FCMPG] = FCMPG;
				INSTRUCTIONS[NBCEL.Const.DCMPL] = DCMPL;
				INSTRUCTIONS[NBCEL.Const.DCMPG] = DCMPG;
				INSTRUCTIONS[NBCEL.Const.IRETURN] = IRETURN;
				INSTRUCTIONS[NBCEL.Const.LRETURN] = LRETURN;
				INSTRUCTIONS[NBCEL.Const.FRETURN] = FRETURN;
				INSTRUCTIONS[NBCEL.Const.DRETURN] = DRETURN;
				INSTRUCTIONS[NBCEL.Const.ARETURN] = ARETURN;
				INSTRUCTIONS[NBCEL.Const.RETURN] = RETURN;
				INSTRUCTIONS[NBCEL.Const.ARRAYLENGTH] = ARRAYLENGTH;
				INSTRUCTIONS[NBCEL.Const.ATHROW] = ATHROW;
				INSTRUCTIONS[NBCEL.Const.MONITORENTER] = MONITORENTER;
				INSTRUCTIONS[NBCEL.Const.MONITOREXIT] = MONITOREXIT;
			}
		}
	}

	public static class InstructionConstantsConstants
	{
	}
}
