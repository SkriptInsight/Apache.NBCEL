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

namespace Apache.NBCEL.Generic
{
	/// <summary>
	///     INVOKESPECIAL - Invoke instance method; special handling for superclass, private
	///     and instance initialization method invocations
	///     <PRE>Stack: ..., objectref, [arg1, [arg2 ...]] -&gt; ...</PRE>
	/// </summary>
	/// <seealso>
	///     *
	///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5.invokespecial">
	///         * The invokespecial instruction in The Java Virtual Machine Specification
	///     </a>
	/// </seealso>
	public class INVOKESPECIAL : InvokeInstruction
    {
	    /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
	    /// <remarks>
	    ///     Empty constructor needed for Instruction.readInstruction.
	    ///     Not to be used otherwise.
	    /// </remarks>
	    internal INVOKESPECIAL()
        {
        }

        public INVOKESPECIAL(int index)
            : base(Const.INVOKESPECIAL, index)
        {
        }

        /// <summary>Dump instruction as byte code to stream out.</summary>
        /// <param name="out">Output stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream @out)
        {
            @out.WriteByte(base.GetOpcode());
            @out.WriteShort(GetIndex());
        }

        public override global::System.Type[] GetExceptions()
        {
            return ExceptionConst.CreateExceptions(ExceptionConst.EXCS.EXCS_FIELD_AND_METHOD_RESOLUTION
                , ExceptionConst.NULL_POINTER_EXCEPTION, ExceptionConst.INCOMPATIBLE_CLASS_CHANGE_ERROR
                , ExceptionConst.ABSTRACT_METHOD_ERROR, ExceptionConst.UNSATISFIED_LINK_ERROR
            );
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
            v.VisitTypedInstruction(this);
            v.VisitStackConsumer(this);
            v.VisitStackProducer(this);
            v.VisitLoadClass(this);
            v.VisitCPInstruction(this);
            v.VisitFieldOrMethod(this);
            v.VisitInvokeInstruction(this);
            v.VisitINVOKESPECIAL(this);
        }
    }
}