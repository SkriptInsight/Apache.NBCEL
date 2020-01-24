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
	/// <summary>
	///     Wrapper class for push operations, which are implemented either as BIPUSH,
	///     LDC or xCONST_n instructions.
	/// </summary>
	public sealed class PUSH : CompoundInstruction, VariableLengthInstruction
    {
        private readonly Instruction instruction;

        /// <summary>This constructor also applies for values of type short, char, byte</summary>
        /// <param name="cp">Constant pool</param>
        /// <param name="value">to be pushed</param>
        public PUSH(ConstantPoolGen cp, int value)
        {
            if (value >= -1 && value <= 5)
                instruction = InstructionConst.GetInstruction(Const.ICONST_0
                                                              + value);
            else if (Instruction.IsValidByte(value))
                instruction = new BIPUSH(unchecked((byte) value));
            else if (Instruction.IsValidShort(value))
                instruction = new SIPUSH((short) value);
            else
                instruction = new LDC(cp.AddInteger(value));
        }

        /// <param name="cp">Constant pool</param>
        /// <param name="value">to be pushed</param>
        public PUSH(ConstantPoolGen cp, bool value)
        {
            instruction = InstructionConst.GetInstruction(Const.ICONST_0
                                                          + (value ? 1 : 0));
        }

        /// <param name="cp">Constant pool</param>
        /// <param name="value">to be pushed</param>
        public PUSH(ConstantPoolGen cp, float value)
        {
            if (Math.Abs(value) < float.Epsilon)
                instruction = InstructionConst.FCONST_0;
            else if (Math.Abs(value - 1.0) < float.Epsilon)
                instruction = InstructionConst.FCONST_1;
            else if (Math.Abs(value - 2.0) < float.Epsilon)
                instruction = InstructionConst.FCONST_2;
            else
                instruction = new LDC(cp.AddFloat(value));
        }

        /// <param name="cp">Constant pool</param>
        /// <param name="value">to be pushed</param>
        public PUSH(ConstantPoolGen cp, long value)
        {
            if (value == 0)
                instruction = InstructionConst.LCONST_0;
            else if (value == 1)
                instruction = InstructionConst.LCONST_1;
            else
                instruction = new LDC2_W(cp.AddLong(value));
        }

        /// <param name="cp">Constant pool</param>
        /// <param name="value">to be pushed</param>
        public PUSH(ConstantPoolGen cp, double value)
        {
            if (value == 0.0)
                instruction = InstructionConst.DCONST_0;
            else if (value == 1.0)
                instruction = InstructionConst.DCONST_1;
            else
                instruction = new LDC2_W(cp.AddDouble(value));
        }

        /// <param name="cp">Constant pool</param>
        /// <param name="value">to be pushed</param>
        public PUSH(ConstantPoolGen cp, string value)
        {
            if (value == null)
                instruction = InstructionConst.ACONST_NULL;
            else
                instruction = new LDC(cp.AddString(value));
        }

        /// <param name="cp" />
        /// <param name="value" />
        /// <since>6.0</since>
        public PUSH(ConstantPoolGen cp, ObjectType value)
        {
            if (value == null)
                instruction = InstructionConst.ACONST_NULL;
            else
                instruction = new LDC(cp.AddClass(value));
        }

        /// <param name="cp">Constant pool</param>
        /// <param name="value">to be pushed</param>
        public PUSH(ConstantPoolGen cp, object value, bool numberOnly)
        {
            if (value is int || value is short || value is byte)
                instruction = new PUSH(cp, (int) value).instruction;
            else if (value is double)
                instruction = new PUSH(cp, (double) value).instruction;
            else if (value is float)
                instruction = new PUSH(cp, (float) value).instruction;
            else if (value is long)
                instruction = new PUSH(cp, (long) value).instruction;
            else
                throw new ClassGenException("What's this: " + value);
        }

        /// <summary>creates a push object from a Character value.</summary>
        /// <remarks>
        ///     creates a push object from a Character value. Warning: Make sure not to attempt to allow
        ///     autoboxing to create this value parameter, as an alternative constructor will be called
        /// </remarks>
        /// <param name="cp">Constant pool</param>
        /// <param name="value">to be pushed</param>
        public PUSH(ConstantPoolGen cp, char value)
            : this(cp, (int) value)
        {
        }

        public InstructionList GetInstructionList()
        {
            return new InstructionList(instruction);
        }

        public Instruction GetInstruction()
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