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
using Apache.NBCEL.Java.IO;
using Apache.NBCEL.Util;

namespace Apache.NBCEL.Generic
{
	/// <summary>LDC - Push item from constant pool.</summary>
	/// <remarks>
	///     LDC - Push item from constant pool.
	///     <PRE>Stack: ... -&gt; ..., item</PRE>
	/// </remarks>
	public class LDC : CPInstruction, PushInstruction, ExceptionThrower
    {
	    /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
	    /// <remarks>
	    ///     Empty constructor needed for Instruction.readInstruction.
	    ///     Not to be used otherwise.
	    /// </remarks>
	    internal LDC()
        {
        }

        public LDC(int index)
            : base(Const.LDC_W, index)
        {
            SetSize();
        }

        public virtual global::System.Type[] GetExceptions()
        {
            return ExceptionConst.CreateExceptions(ExceptionConst.EXCS.EXCS_STRING_RESOLUTION
            );
        }

        // Adjust to proper size
        protected internal void SetSize()
        {
            if (GetIndex() <= Const.MAX_BYTE)
            {
                // Fits in one byte?
                SetOpcode(Const.LDC);
                SetLength(2);
            }
            else
            {
                SetOpcode(Const.LDC_W);
                SetLength(3);
            }
        }

        /// <summary>Dump instruction as byte code to stream out.</summary>
        /// <param name="out">Output stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream @out)
        {
            @out.WriteByte(base.GetOpcode());
            if (base.GetLength() == 2)
                // TODO useless check?
                @out.WriteByte(GetIndex());
            else
                @out.WriteShort(GetIndex());
        }

        /// <summary>Set the index to constant pool and adjust size.</summary>
        public sealed override void SetIndex(int index)
        {
            base.SetIndex(index);
            SetSize();
        }

        /// <summary>Read needed data (e.g.</summary>
        /// <remarks>Read needed data (e.g. index) from file.</remarks>
        /// <exception cref="System.IO.IOException" />
        protected internal override void InitFromFile(ByteSequence bytes, bool
            wide)
        {
            SetLength(2);
            base.SetIndex(bytes.ReadUnsignedByte());
        }

        public virtual object GetValue(ConstantPoolGen cpg)
        {
            var c = cpg.GetConstantPool().GetConstant(GetIndex());
            switch (c.GetTag())
            {
                case Const.CONSTANT_String:
                {
                    var i = ((ConstantString) c).GetStringIndex();
                    c = cpg.GetConstantPool().GetConstant(i);
                    return ((ConstantUtf8) c).GetBytes();
                }

                case Const.CONSTANT_Float:
                {
                    return ((ConstantFloat) c).GetBytes();
                }

                case Const.CONSTANT_Integer:
                {
                    return ((ConstantInteger) c).GetBytes();
                }

                case Const.CONSTANT_Class:
                {
                    var nameIndex = ((ConstantClass) c).GetNameIndex();
                    c = cpg.GetConstantPool().GetConstant(nameIndex);
                    return new ObjectType(((ConstantUtf8) c).GetBytes());
                }

                default:
                {
                    // Never reached
                    throw new Exception("Unknown or invalid constant type at " + GetIndex
                                            ());
                }
            }
        }

        public override Type GetType(ConstantPoolGen cpg)
        {
            switch (cpg.GetConstantPool().GetConstant(GetIndex()).GetTag())
            {
                case Const.CONSTANT_String:
                {
                    return Type.STRING;
                }

                case Const.CONSTANT_Float:
                {
                    return Type.FLOAT;
                }

                case Const.CONSTANT_Integer:
                {
                    return Type.INT;
                }

                case Const.CONSTANT_Class:
                {
                    return Type.CLASS;
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
            v.VisitExceptionThrower(this);
            v.VisitTypedInstruction(this);
            v.VisitCPInstruction(this);
            v.VisitLDC(this);
        }
    }
}