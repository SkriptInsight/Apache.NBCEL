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

namespace NBCEL.generic
{
    /// <summary>Super class for the x2y family of instructions.</summary>
    public abstract class ConversionInstruction : Instruction, TypedInstruction
        , StackProducer, StackConsumer
    {
	    /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
	    /// <remarks>
	    ///     Empty constructor needed for Instruction.readInstruction.
	    ///     Not to be used otherwise.
	    /// </remarks>
	    internal ConversionInstruction()
        {
        }

        /// <param name="opcode">opcode of instruction</param>
        protected internal ConversionInstruction(short opcode)
            : base(opcode, 1)
        {
        }

        /// <returns>type associated with the instruction</returns>
        public virtual Type GetType(ConstantPoolGen cp)
        {
            var _opcode = base.GetOpcode();
            switch (_opcode)
            {
                case Const.D2I:
                case Const.F2I:
                case Const.L2I:
                {
                    return Type.INT;
                }

                case Const.D2F:
                case Const.I2F:
                case Const.L2F:
                {
                    return Type.FLOAT;
                }

                case Const.D2L:
                case Const.F2L:
                case Const.I2L:
                {
                    return Type.LONG;
                }

                case Const.F2D:
                case Const.I2D:
                case Const.L2D:
                {
                    return Type.DOUBLE;
                }

                case Const.I2B:
                {
                    return Type.BYTE;
                }

                case Const.I2C:
                {
                    return Type.CHAR;
                }

                case Const.I2S:
                {
                    return Type.SHORT;
                }

                default:
                {
                    // Never reached
                    throw new ClassGenException("Unknown type " + _opcode);
                }
            }
        }
    }
}