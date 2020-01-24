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
    /// <summary>Super class for the xRETURN family of instructions.</summary>
    public abstract class ReturnInstruction : Instruction, ExceptionThrower
        , TypedInstruction, StackConsumer
    {
	    /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
	    /// <remarks>
	    ///     Empty constructor needed for Instruction.readInstruction.
	    ///     Not to be used otherwise.
	    /// </remarks>
	    internal ReturnInstruction()
        {
        }

        /// <param name="opcode">of instruction</param>
        protected internal ReturnInstruction(short opcode)
            : base(opcode, 1)
        {
        }

        public virtual System.Type[] GetExceptions()
        {
            return new[] {ExceptionConst.ILLEGAL_MONITOR_STATE};
        }

        /// <returns>type associated with the instruction</returns>
        public virtual Type GetType(ConstantPoolGen cp)
        {
            return GetType();
        }

        public virtual Type GetType()
        {
            var _opcode = base.GetOpcode();
            switch (_opcode)
            {
                case Const.IRETURN:
                {
                    return Type.INT;
                }

                case Const.LRETURN:
                {
                    return Type.LONG;
                }

                case Const.FRETURN:
                {
                    return Type.FLOAT;
                }

                case Const.DRETURN:
                {
                    return Type.DOUBLE;
                }

                case Const.ARETURN:
                {
                    return Type.OBJECT;
                }

                case Const.RETURN:
                {
                    return Type.VOID;
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