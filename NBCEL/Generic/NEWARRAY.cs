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
	///     NEWARRAY -  Create new array of basic type (int, short, ...)
	///     <PRE>Stack: ..., count -&gt; ..., arrayref</PRE>
	///     type must be one of T_INT, T_SHORT, ...
	/// </summary>
	public class NEWARRAY : Instruction, AllocationInstruction
        , ExceptionThrower, StackProducer
    {
        private byte type;

        /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
        /// <remarks>
        ///     Empty constructor needed for Instruction.readInstruction.
        ///     Not to be used otherwise.
        /// </remarks>
        internal NEWARRAY()
        {
        }

        public NEWARRAY(byte type)
            : base(Const.NEWARRAY, 2)
        {
            this.type = type;
        }

        public NEWARRAY(BasicType type)
            : this(type.GetType())
        {
        }

        public virtual global::System.Type[] GetExceptions()
        {
            return new[] {ExceptionConst.NEGATIVE_ARRAY_SIZE_EXCEPTION};
        }

        /// <summary>Dump instruction as byte code to stream out.</summary>
        /// <param name="out">Output stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream @out)
        {
            @out.WriteByte(base.GetOpcode());
            @out.WriteByte(type);
        }

        /// <returns>numeric code for basic element type</returns>
        public byte GetTypecode()
        {
            return type;
        }

        /// <returns>type of constructed array</returns>
        public Type GetType()
        {
            return new ArrayType(BasicType.GetType(type), 1);
        }

        /// <returns>mnemonic for instruction</returns>
        public override string ToString(bool verbose)
        {
            return base.ToString(verbose) + " " + Const.GetTypeName(type);
        }

        /// <summary>Read needed data (e.g.</summary>
        /// <remarks>Read needed data (e.g. index) from file.</remarks>
        /// <exception cref="System.IO.IOException" />
        protected internal override void InitFromFile(ByteSequence bytes, bool
            wide)
        {
            type = bytes.ReadByte();
            SetLength(2);
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
            v.VisitAllocationInstruction(this);
            v.VisitExceptionThrower(this);
            v.VisitStackProducer(this);
            v.VisitNEWARRAY(this);
        }
    }
}