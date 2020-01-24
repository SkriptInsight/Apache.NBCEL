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
	/// <summary>Super class for the family of arithmetic instructions.</summary>
	public abstract class ArithmeticInstruction : NBCEL.generic.Instruction, NBCEL.generic.TypedInstruction
		, NBCEL.generic.StackProducer, NBCEL.generic.StackConsumer
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal ArithmeticInstruction()
		{
		}

		/// <param name="opcode">of instruction</param>
		protected internal ArithmeticInstruction(short opcode)
			: base(opcode, (short)1)
		{
		}

		/// <returns>type associated with the instruction</returns>
		public virtual NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cp)
		{
			short _opcode = base.GetOpcode();
			switch (_opcode)
			{
				case NBCEL.Const.DADD:
				case NBCEL.Const.DDIV:
				case NBCEL.Const.DMUL:
				case NBCEL.Const.DNEG:
				case NBCEL.Const.DREM:
				case NBCEL.Const.DSUB:
				{
					return NBCEL.generic.Type.DOUBLE;
				}

				case NBCEL.Const.FADD:
				case NBCEL.Const.FDIV:
				case NBCEL.Const.FMUL:
				case NBCEL.Const.FNEG:
				case NBCEL.Const.FREM:
				case NBCEL.Const.FSUB:
				{
					return NBCEL.generic.Type.FLOAT;
				}

				case NBCEL.Const.IADD:
				case NBCEL.Const.IAND:
				case NBCEL.Const.IDIV:
				case NBCEL.Const.IMUL:
				case NBCEL.Const.INEG:
				case NBCEL.Const.IOR:
				case NBCEL.Const.IREM:
				case NBCEL.Const.ISHL:
				case NBCEL.Const.ISHR:
				case NBCEL.Const.ISUB:
				case NBCEL.Const.IUSHR:
				case NBCEL.Const.IXOR:
				{
					return NBCEL.generic.Type.INT;
				}

				case NBCEL.Const.LADD:
				case NBCEL.Const.LAND:
				case NBCEL.Const.LDIV:
				case NBCEL.Const.LMUL:
				case NBCEL.Const.LNEG:
				case NBCEL.Const.LOR:
				case NBCEL.Const.LREM:
				case NBCEL.Const.LSHL:
				case NBCEL.Const.LSHR:
				case NBCEL.Const.LSUB:
				case NBCEL.Const.LUSHR:
				case NBCEL.Const.LXOR:
				{
					return NBCEL.generic.Type.LONG;
				}

				default:
				{
					// Never reached
					throw new NBCEL.generic.ClassGenException("Unknown type " + _opcode);
				}
			}
		}
	}
}
