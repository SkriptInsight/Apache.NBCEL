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
	/// <summary>Super class for the x2y family of instructions.</summary>
	public abstract class ConversionInstruction : NBCEL.generic.Instruction, NBCEL.generic.TypedInstruction
		, NBCEL.generic.StackProducer, NBCEL.generic.StackConsumer
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal ConversionInstruction()
		{
		}

		/// <param name="opcode">opcode of instruction</param>
		protected internal ConversionInstruction(short opcode)
			: base(opcode, (short)1)
		{
		}

		/// <returns>type associated with the instruction</returns>
		public virtual NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cp)
		{
			short _opcode = base.GetOpcode();
			switch (_opcode)
			{
				case NBCEL.Const.D2I:
				case NBCEL.Const.F2I:
				case NBCEL.Const.L2I:
				{
					return NBCEL.generic.Type.INT;
				}

				case NBCEL.Const.D2F:
				case NBCEL.Const.I2F:
				case NBCEL.Const.L2F:
				{
					return NBCEL.generic.Type.FLOAT;
				}

				case NBCEL.Const.D2L:
				case NBCEL.Const.F2L:
				case NBCEL.Const.I2L:
				{
					return NBCEL.generic.Type.LONG;
				}

				case NBCEL.Const.F2D:
				case NBCEL.Const.I2D:
				case NBCEL.Const.L2D:
				{
					return NBCEL.generic.Type.DOUBLE;
				}

				case NBCEL.Const.I2B:
				{
					return NBCEL.generic.Type.BYTE;
				}

				case NBCEL.Const.I2C:
				{
					return NBCEL.generic.Type.CHAR;
				}

				case NBCEL.Const.I2S:
				{
					return NBCEL.generic.Type.SHORT;
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
