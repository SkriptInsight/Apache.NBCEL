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

using java.io;
using NBCEL.util;
using Sharpen;

namespace NBCEL.generic
{
    /// <summary>IINC - Increment local variable by constant</summary>
    public class IINC : LocalVariableInstruction
    {
        private int c;
        private bool wide;

        /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
        /// <remarks>
        ///     Empty constructor needed for Instruction.readInstruction.
        ///     Not to be used otherwise.
        /// </remarks>
        internal IINC()
        {
        }

        /// <param name="n">index of local variable</param>
        /// <param name="c">increment factor</param>
        public IINC(int n, int c)
        {
            // Default behavior of LocalVariableInstruction causes error
            SetOpcode(Const.IINC);
            SetLength(3);
            SetIndex(n);
            // May set wide as side effect
            SetIncrement(c);
        }

        /// <summary>Dump instruction as byte code to stream out.</summary>
        /// <param name="out">Output stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream @out)
        {
            if (wide) @out.WriteByte(Const.WIDE);
            @out.WriteByte(base.GetOpcode());
            if (wide)
            {
                @out.WriteShort(GetIndex());
                @out.WriteShort(c);
            }
            else
            {
                @out.WriteByte(GetIndex());
                @out.WriteByte(c);
            }
        }

        private void SetWide()
        {
            wide = GetIndex() > Const.MAX_BYTE;
            if (c > 0)
                wide = wide || c > byte.MaxValue;
            else
                wide = wide || c < byte.MinValue;
            if (wide)
                SetLength(6);
            else
                // wide byte included
                SetLength(3);
        }

        /// <summary>Read needed data (e.g.</summary>
        /// <remarks>Read needed data (e.g. index) from file.</remarks>
        /// <exception cref="System.IO.IOException" />
        protected internal override void InitFromFile(ByteSequence bytes, bool
            wide)
        {
            this.wide = wide;
            if (wide)
            {
                SetLength(6);
                SetIndexOnly(bytes.ReadUnsignedShort());
                c = bytes.ReadShort();
            }
            else
            {
                SetLength(3);
                SetIndexOnly(bytes.ReadUnsignedByte());
                c = bytes.ReadByte();
            }
        }

        /// <returns>mnemonic for instruction</returns>
        public override string ToString(bool verbose)
        {
            return base.ToString(verbose) + " " + c;
        }

        /// <summary>Set index of local variable.</summary>
        public sealed override void SetIndex(int n)
        {
            if (n < 0) throw new ClassGenException("Negative index value: " + n);
            SetIndexOnly(n);
            SetWide();
        }

        /// <returns>increment factor</returns>
        public int GetIncrement()
        {
            return c;
        }

        /// <summary>Set increment factor.</summary>
        public void SetIncrement(int c)
        {
            this.c = c;
            SetWide();
        }

        /// <returns>int type</returns>
        public override Type GetType(ConstantPoolGen cp)
        {
            return Type.INT;
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
            v.VisitLocalVariableInstruction(this);
            v.VisitIINC(this);
        }
    }
}