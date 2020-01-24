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
	/// <summary>Super class for the xRETURN family of instructions.</summary>
	public abstract class ReturnInstruction : NBCEL.generic.Instruction, NBCEL.generic.ExceptionThrower
		, NBCEL.generic.TypedInstruction, NBCEL.generic.StackConsumer
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal ReturnInstruction()
		{
		}

		/// <param name="opcode">of instruction</param>
		protected internal ReturnInstruction(short opcode)
			: base(opcode, (short)1)
		{
		}

		public virtual NBCEL.generic.Type GetType()
		{
			short _opcode = base.GetOpcode();
			switch (_opcode)
			{
				case NBCEL.Const.IRETURN:
				{
					return NBCEL.generic.Type.INT;
				}

				case NBCEL.Const.LRETURN:
				{
					return NBCEL.generic.Type.LONG;
				}

				case NBCEL.Const.FRETURN:
				{
					return NBCEL.generic.Type.FLOAT;
				}

				case NBCEL.Const.DRETURN:
				{
					return NBCEL.generic.Type.DOUBLE;
				}

				case NBCEL.Const.ARETURN:
				{
					return NBCEL.generic.Type.OBJECT;
				}

				case NBCEL.Const.RETURN:
				{
					return NBCEL.generic.Type.VOID;
				}

				default:
				{
					// Never reached
					throw new NBCEL.generic.ClassGenException("Unknown type " + _opcode);
				}
			}
		}

		public virtual System.Type[] GetExceptions()
		{
			return new System.Type[] { NBCEL.ExceptionConst.ILLEGAL_MONITOR_STATE };
		}

		/// <returns>type associated with the instruction</returns>
		public virtual NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cp)
		{
			return GetType();
		}
	}
}
