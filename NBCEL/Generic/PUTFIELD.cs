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

namespace Apache.NBCEL.Generic
{
	/// <summary>
	///     PUTFIELD - Put field in object
	///     <PRE>Stack: ..., objectref, value -&gt; ...</PRE>
	///     OR
	///     <PRE>Stack: ..., objectref, value.word1, value.word2 -&gt; ...</PRE>
	/// </summary>
	public class PUTFIELD : FieldInstruction, PopInstruction
        , ExceptionThrower
    {
	    /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
	    /// <remarks>
	    ///     Empty constructor needed for Instruction.readInstruction.
	    ///     Not to be used otherwise.
	    /// </remarks>
	    internal PUTFIELD()
        {
        }

        public PUTFIELD(int index)
            : base(Const.PUTFIELD, index)
        {
        }

        public virtual global::System.Type[] GetExceptions()
        {
            return ExceptionConst.CreateExceptions(ExceptionConst.EXCS.EXCS_FIELD_AND_METHOD_RESOLUTION
                , ExceptionConst.NULL_POINTER_EXCEPTION, ExceptionConst.INCOMPATIBLE_CLASS_CHANGE_ERROR
            );
        }

        public override int ConsumeStack(ConstantPoolGen cpg)
        {
            return GetFieldSize(cpg) + 1;
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
            v.VisitExceptionThrower(this);
            v.VisitStackConsumer(this);
            v.VisitPopInstruction(this);
            v.VisitTypedInstruction(this);
            v.VisitLoadClass(this);
            v.VisitCPInstruction(this);
            v.VisitFieldOrMethod(this);
            v.VisitFieldInstruction(this);
            v.VisitPUTFIELD(this);
        }
    }
}