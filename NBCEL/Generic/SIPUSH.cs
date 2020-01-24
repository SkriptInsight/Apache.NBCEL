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
	///     SIPUSH - Push short
	///     <PRE>Stack: ...
	/// </summary>
	/// <remarks>
	///     SIPUSH - Push short
	///     <PRE>Stack: ... -&gt; ..., value</PRE>
	/// </remarks>
	public class SIPUSH : Instruction, ConstantPushInstruction<short>
    {
        private short b;

        /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
        /// <remarks>
        ///     Empty constructor needed for Instruction.readInstruction.
        ///     Not to be used otherwise.
        /// </remarks>
        internal SIPUSH()
        {
        }

        public SIPUSH(short b)
            : base(Const.SIPUSH, 3)
        {
            this.b = b;
        }

        public virtual short GetValue()
        {
            return b;
        }

        /// <returns>Type.SHORT</returns>
        public virtual Type GetType(ConstantPoolGen cp)
        {
            return Type.SHORT;
        }

        object BaseConstantPushInstruction.GetValue()
        {
            return GetValue();
        }

        /// <summary>Dump instruction as short code to stream out.</summary>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream @out)
        {
            base.Dump(@out);
            @out.WriteShort(b);
        }

        /// <returns>mnemonic for instruction</returns>
        public override string ToString(bool verbose)
        {
            return base.ToString(verbose) + " " + b;
        }

        /// <summary>Read needed data (e.g.</summary>
        /// <remarks>Read needed data (e.g. index) from file.</remarks>
        /// <exception cref="System.IO.IOException" />
        protected internal override void InitFromFile(ByteSequence bytes, bool
            wide)
        {
            SetLength(3);
            b = bytes.ReadShort();
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
            v.VisitPushInstruction(this);
            v.VisitStackProducer(this);
            v.VisitTypedInstruction(this);
            v.VisitConstantPushInstruction(this);
            v.VisitSIPUSH(this);
        }
    }
}