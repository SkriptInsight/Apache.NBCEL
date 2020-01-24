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
using Sharpen;

namespace NBCEL.generic
{
	/// <summary>
	/// Wrapper class for push operations, which are implemented either as BIPUSH,
	/// LDC or xCONST_n instructions.
	/// </summary>
	public sealed class PUSH : NBCEL.generic.CompoundInstruction, NBCEL.generic.VariableLengthInstruction
	{
		private NBCEL.generic.Instruction instruction;

		/// <summary>This constructor also applies for values of type short, char, byte</summary>
		/// <param name="cp">Constant pool</param>
		/// <param name="value">to be pushed</param>
		public PUSH(NBCEL.generic.ConstantPoolGen cp, int value)
		{
			if ((value >= -1) && (value <= 5))
			{
				instruction = NBCEL.generic.InstructionConst.GetInstruction(NBCEL.Const.ICONST_0 
					+ value);
			}
			else if (NBCEL.generic.Instruction.IsValidByte(value))
			{
				instruction = new NBCEL.generic.BIPUSH(unchecked((byte)value));
			}
			else if (NBCEL.generic.Instruction.IsValidShort(value))
			{
				instruction = new NBCEL.generic.SIPUSH((short)value);
			}
			else
			{
				instruction = new NBCEL.generic.LDC(cp.AddInteger(value));
			}
		}

		/// <param name="cp">Constant pool</param>
		/// <param name="value">to be pushed</param>
		public PUSH(NBCEL.generic.ConstantPoolGen cp, bool value)
		{
			instruction = NBCEL.generic.InstructionConst.GetInstruction(NBCEL.Const.ICONST_0 
				+ (value ? 1 : 0));
		}

		/// <param name="cp">Constant pool</param>
		/// <param name="value">to be pushed</param>
		public PUSH(NBCEL.generic.ConstantPoolGen cp, float value)
		{
			if (Math.Abs(value) < float.Epsilon)
			{
				instruction = NBCEL.generic.InstructionConst.FCONST_0;
			}
			else if (Math.Abs(value - 1.0) < float.Epsilon)
			{
				instruction = NBCEL.generic.InstructionConst.FCONST_1;
			}
			else if (Math.Abs(value - 2.0) < float.Epsilon)
			{
				instruction = NBCEL.generic.InstructionConst.FCONST_2;
			}
			else
			{
				instruction = new NBCEL.generic.LDC(cp.AddFloat(value));
			}
		}

		/// <param name="cp">Constant pool</param>
		/// <param name="value">to be pushed</param>
		public PUSH(NBCEL.generic.ConstantPoolGen cp, long value)
		{
			if (value == 0)
			{
				instruction = NBCEL.generic.InstructionConst.LCONST_0;
			}
			else if (value == 1)
			{
				instruction = NBCEL.generic.InstructionConst.LCONST_1;
			}
			else
			{
				instruction = new NBCEL.generic.LDC2_W(cp.AddLong(value));
			}
		}

		/// <param name="cp">Constant pool</param>
		/// <param name="value">to be pushed</param>
		public PUSH(NBCEL.generic.ConstantPoolGen cp, double value)
		{
			if (value == 0.0)
			{
				instruction = NBCEL.generic.InstructionConst.DCONST_0;
			}
			else if (value == 1.0)
			{
				instruction = NBCEL.generic.InstructionConst.DCONST_1;
			}
			else
			{
				instruction = new NBCEL.generic.LDC2_W(cp.AddDouble(value));
			}
		}

		/// <param name="cp">Constant pool</param>
		/// <param name="value">to be pushed</param>
		public PUSH(NBCEL.generic.ConstantPoolGen cp, string value)
		{
			if (value == null)
			{
				instruction = NBCEL.generic.InstructionConst.ACONST_NULL;
			}
			else
			{
				instruction = new NBCEL.generic.LDC(cp.AddString(value));
			}
		}

		/// <param name="cp"/>
		/// <param name="value"/>
		/// <since>6.0</since>
		public PUSH(NBCEL.generic.ConstantPoolGen cp, NBCEL.generic.ObjectType value)
		{
			if (value == null)
			{
				instruction = NBCEL.generic.InstructionConst.ACONST_NULL;
			}
			else
			{
				instruction = new NBCEL.generic.LDC(cp.AddClass(value));
			}
		}

		/// <param name="cp">Constant pool</param>
		/// <param name="value">to be pushed</param>
		public PUSH(NBCEL.generic.ConstantPoolGen cp, object value, bool numberOnly)
		{
			if ((value is int) || (value is short) || (value is byte))
			{
				instruction = new NBCEL.generic.PUSH(cp, (int)value).instruction;
			}
			else if (value is double)
			{
				instruction = new NBCEL.generic.PUSH(cp, (double)value).instruction;
			}
			else if (value is float)
			{
				instruction = new NBCEL.generic.PUSH(cp, (float)value).instruction;
			}
			else if (value is long)
			{
				instruction = new NBCEL.generic.PUSH(cp, (long) value).instruction;
			}
			else
			{
				throw new NBCEL.generic.ClassGenException("What's this: " + value);
			}
		}

		/// <summary>creates a push object from a Character value.</summary>
		/// <remarks>
		/// creates a push object from a Character value. Warning: Make sure not to attempt to allow
		/// autoboxing to create this value parameter, as an alternative constructor will be called
		/// </remarks>
		/// <param name="cp">Constant pool</param>
		/// <param name="value">to be pushed</param>
		public PUSH(NBCEL.generic.ConstantPoolGen cp, char value)
			: this(cp, (int)value)
		{
		}
		public NBCEL.generic.InstructionList GetInstructionList()
		{
			return new NBCEL.generic.InstructionList(instruction);
		}

		public NBCEL.generic.Instruction GetInstruction()
		{
			return instruction;
		}

		/// <returns>mnemonic for instruction</returns>
		public override string ToString()
		{
			return instruction + " (PUSH)";
		}
	}
}
