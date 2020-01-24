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

namespace NBCEL.generic
{
	/// <summary>
	///     DCONST - Push 0.0 or 1.0, other values cause an exception
	///     <PRE>Stack: ...
	/// </summary>
	/// <remarks>
	///     DCONST - Push 0.0 or 1.0, other values cause an exception
	///     <PRE>Stack: ... -&gt; ..., </PRE>
	/// </remarks>
	public class DCONST : Instruction, ConstantPushInstruction<double>
    {
        private readonly double value;

        /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
        /// <remarks>
        ///     Empty constructor needed for Instruction.readInstruction.
        ///     Not to be used otherwise.
        /// </remarks>
        internal DCONST()
        {
        }

        public DCONST(double f)
            : base(Const.DCONST_0, 1)
        {
            if (f == 0.0)
                SetOpcode(Const.DCONST_0);
            else if (f == 1.0)
                SetOpcode(Const.DCONST_1);
            else
                throw new ClassGenException("DCONST can be used only for 0.0 and 1.0: "
                                            + f);
            value = f;
        }

        public virtual double GetValue()
        {
            return value;
        }

        /// <returns>Type.DOUBLE</returns>
        public virtual Type GetType(ConstantPoolGen cp)
        {
            return Type.DOUBLE;
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
            v.VisitDCONST(this);
        }
    }
}