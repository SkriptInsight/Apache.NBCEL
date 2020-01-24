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
    /// <summary>JSR_W - Jump to subroutine</summary>
    public class JSR_W : JsrInstruction
    {
	    /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
	    /// <remarks>
	    ///     Empty constructor needed for Instruction.readInstruction.
	    ///     Not to be used otherwise.
	    /// </remarks>
	    internal JSR_W()
        {
        }

        public JSR_W(InstructionHandle target)
            : base(Const.JSR_W, target)
        {
            SetLength(5);
        }

        /// <summary>Dump instruction as byte code to stream out.</summary>
        /// <param name="out">Output stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream @out)
        {
            base.SetIndex(GetTargetOffset());
            @out.WriteByte(base.GetOpcode());
            @out.WriteInt(GetIndex());
        }

        /// <summary>Read needed data (e.g.</summary>
        /// <remarks>Read needed data (e.g. index) from file.</remarks>
        /// <exception cref="System.IO.IOException" />
        protected internal override void InitFromFile(ByteSequence bytes, bool
            wide)
        {
            base.SetIndex(bytes.ReadInt());
            SetLength(5);
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
            v.VisitBranchInstruction(this);
            v.VisitJsrInstruction(this);
            v.VisitJSR_W(this);
        }
    }
}