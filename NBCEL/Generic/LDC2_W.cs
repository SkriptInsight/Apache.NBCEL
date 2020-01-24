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
using Apache.NBCEL.ClassFile;

namespace Apache.NBCEL.Generic
{
	/// <summary>
	///     LDC2_W - Push long or double from constant pool
	///     <PRE>Stack: ...
	/// </summary>
	/// <remarks>
	///     LDC2_W - Push long or double from constant pool
	///     <PRE>Stack: ... -&gt; ..., item.word1, item.word2</PRE>
	/// </remarks>
	public class LDC2_W : CPInstruction, PushInstruction
    {
	    /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
	    /// <remarks>
	    ///     Empty constructor needed for Instruction.readInstruction.
	    ///     Not to be used otherwise.
	    /// </remarks>
	    internal LDC2_W()
        {
        }

        public LDC2_W(int index)
            : base(Const.LDC2_W, index)
        {
        }

        public override Type GetType(ConstantPoolGen cpg)
        {
            switch (cpg.GetConstantPool().GetConstant(GetIndex()).GetTag())
            {
                case Const.CONSTANT_Long:
                {
                    return Type.LONG;
                }

                case Const.CONSTANT_Double:
                {
                    return Type.DOUBLE;
                }

                default:
                {
                    // Never reached
                    throw new Exception("Unknown constant type " + base.GetOpcode());
                }
            }
        }

        public virtual object GetValue(ConstantPoolGen cpg)
        {
            var c = cpg.GetConstantPool().GetConstant(GetIndex());
            switch (c.GetTag())
            {
                case Const.CONSTANT_Long:
                {
                    return ((ConstantLong) c).GetBytes();
                }

                case Const.CONSTANT_Double:
                {
                    return ((ConstantDouble) c).GetBytes();
                }

                default:
                {
                    // Never reached
                    throw new Exception("Unknown or invalid constant type at " + GetIndex
                                            ());
                }
            }
        }

        /// <summary>Call corresponding visitor method(s).</summary>
        /// <remarks>
        ///     Call corresponding visitor method(s). The order is:
        ///     Call visitor methods of implemented interfaces first, then
        ///     call methods according to the class hierarchy in descending order,
        ///     i.e., the most specific visitXXX() call comes last.
        /// </remarks>
        /// <param name="v">Visitor object</param>
        public override void Accept(Visitor v)
        {
            v.VisitStackProducer(this);
            v.VisitPushInstruction(this);
            v.VisitTypedInstruction(this);
            v.VisitCPInstruction(this);
            v.VisitLDC2_W(this);
        }
    }
}