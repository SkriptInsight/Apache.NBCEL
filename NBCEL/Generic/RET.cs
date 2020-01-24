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

using Apache.NBCEL.Java.IO;
using Apache.NBCEL.Util;

namespace Apache.NBCEL.Generic
{
	/// <summary>
	///     RET - Return from subroutine
	///     <PRE>Stack: ...
	/// </summary>
	/// <remarks>
	///     RET - Return from subroutine
	///     <PRE>Stack: ... -&gt; ...</PRE>
	/// </remarks>
	public class RET : Instruction, IndexedInstruction, TypedInstruction
    {
        private int index;
        private bool wide;

        /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
        /// <remarks>
        ///     Empty constructor needed for Instruction.readInstruction.
        ///     Not to be used otherwise.
        /// </remarks>
        internal RET()
        {
        }

        public RET(int index)
            : base(Const.RET, 2)
        {
            // index to local variable containg the return address
            SetIndex(index);
        }

        /// <returns>index of local variable containg the return address</returns>
        public int GetIndex()
        {
            return index;
        }

        /// <summary>Set index of local variable containg the return address</summary>
        public void SetIndex(int n)
        {
            if (n < 0) throw new ClassGenException("Negative index value: " + n);
            index = n;
            SetWide();
        }

        /// <returns>return address type</returns>
        public virtual Type GetType(ConstantPoolGen cp)
        {
            return ReturnaddressType.NO_TARGET;
        }

        // May set wide as side effect
        /// <summary>Dump instruction as byte code to stream out.</summary>
        /// <param name="out">Output stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream @out)
        {
            if (wide) @out.WriteByte(Const.WIDE);
            @out.WriteByte(base.GetOpcode());
            if (wide)
                @out.WriteShort(index);
            else
                @out.WriteByte(index);
        }

        private void SetWide()
        {
            wide = index > Const.MAX_BYTE;
            if (wide)
                SetLength(4);
            else
                // Including the wide byte
                SetLength(2);
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
                index = bytes.ReadUnsignedShort();
                SetLength(4);
            }
            else
            {
                index = bytes.ReadUnsignedByte();
                SetLength(2);
            }
        }

        /// <returns>mnemonic for instruction</returns>
        public override string ToString(bool verbose)
        {
            return base.ToString(verbose) + " " + index;
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
            v.VisitRET(this);
        }
    }
}