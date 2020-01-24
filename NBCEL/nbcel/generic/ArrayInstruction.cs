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
	/// <summary>Super class for instructions dealing with array access such as IALOAD.</summary>
	public abstract class ArrayInstruction : NBCEL.generic.Instruction, NBCEL.generic.ExceptionThrower
		, NBCEL.generic.TypedInstruction
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal ArrayInstruction()
		{
		}

		/// <param name="opcode">of instruction</param>
		protected internal ArrayInstruction(short opcode)
			: base(opcode, (short)1)
		{
		}

		public virtual System.Type[] GetExceptions()
		{
			return NBCEL.ExceptionConst.CreateExceptions(NBCEL.ExceptionConst.EXCS.EXCS_ARRAY_EXCEPTION
				);
		}

		/// <returns>type associated with the instruction</returns>
		public virtual NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cp)
		{
			short _opcode = base.GetOpcode();
			switch (_opcode)
			{
				case NBCEL.Const.IALOAD:
				case NBCEL.Const.IASTORE:
				{
					return NBCEL.generic.Type.INT;
				}

				case NBCEL.Const.CALOAD:
				case NBCEL.Const.CASTORE:
				{
					return NBCEL.generic.Type.CHAR;
				}

				case NBCEL.Const.BALOAD:
				case NBCEL.Const.BASTORE:
				{
					return NBCEL.generic.Type.BYTE;
				}

				case NBCEL.Const.SALOAD:
				case NBCEL.Const.SASTORE:
				{
					return NBCEL.generic.Type.SHORT;
				}

				case NBCEL.Const.LALOAD:
				case NBCEL.Const.LASTORE:
				{
					return NBCEL.generic.Type.LONG;
				}

				case NBCEL.Const.DALOAD:
				case NBCEL.Const.DASTORE:
				{
					return NBCEL.generic.Type.DOUBLE;
				}

				case NBCEL.Const.FALOAD:
				case NBCEL.Const.FASTORE:
				{
					return NBCEL.generic.Type.FLOAT;
				}

				case NBCEL.Const.AALOAD:
				case NBCEL.Const.AASTORE:
				{
					return NBCEL.generic.Type.OBJECT;
				}

				default:
				{
					throw new NBCEL.generic.ClassGenException("Oops: unknown case in switch" + _opcode
						);
				}
			}
		}
	}
}
