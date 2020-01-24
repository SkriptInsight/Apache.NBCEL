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

namespace NBCEL.generic
{
	/// <summary>
	///     FCONST - Push 0.0, 1.0 or 2.0, other values cause an exception
	///     <PRE>Stack: ...
	/// </summary>
	/// <remarks>
	///     FCONST - Push 0.0, 1.0 or 2.0, other values cause an exception
	///     <PRE>Stack: ... -&gt; ..., </PRE>
	/// </remarks>
	public class FCONST : Instruction, ConstantPushInstruction<float>
    {
        private readonly float value;

        /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
        /// <remarks>
        ///     Empty constructor needed for Instruction.readInstruction.
        ///     Not to be used otherwise.
        /// </remarks>
        internal FCONST()
        {
        }

        public FCONST(float f)
            : base(Const.FCONST_0, 1)
        {
            if (Math.Abs(f) < float.Epsilon)
                SetOpcode(Const.FCONST_0);
            else if (Math.Abs(f - 1.0) < float.Epsilon)
                SetOpcode(Const.FCONST_1);
            else if (Math.Abs(f - 2.0) < float.Epsilon)
                SetOpcode(Const.FCONST_2);
            else
                throw new ClassGenException("FCONST can be used only for 0.0, 1.0 and 2.0: "
                                            + f);
            value = f;
        }

        public virtual float GetValue()
        {
            return value;
        }

        /// <returns>Type.FLOAT</returns>
        public virtual Type GetType(ConstantPoolGen cp)
        {
            return Type.FLOAT;
        }

        object BaseConstantPushInstruction.GetValue()
        {
            return GetValue();
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
            v.VisitFCONST(this);
        }
    }
}