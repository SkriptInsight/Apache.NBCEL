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
using Sharpen;

namespace NBCEL.generic
{
	/// <summary>
	/// ICONST - Push value between -1, ..., 5, other values cause an exception
	/// <PRE>Stack: ...
	/// </summary>
	/// <remarks>
	/// ICONST - Push value between -1, ..., 5, other values cause an exception
	/// <PRE>Stack: ... -&gt; ..., </PRE>
	/// </remarks>
	public class ICONST : NBCEL.generic.Instruction, NBCEL.generic.ConstantPushInstruction<int>
	{
		private int value;

		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal ICONST()
		{
		}

		public ICONST(int i)
			: base(NBCEL.Const.ICONST_0, (short)1)
		{
			if ((i >= -1) && (i <= 5))
			{
				base.SetOpcode((short)(NBCEL.Const.ICONST_0 + i));
			}
			else
			{
				// Even works for i == -1
				throw new NBCEL.generic.ClassGenException("ICONST can be used only for value between -1 and 5: "
					 + i);
			}
			value = i;
		}

		public virtual int GetValue()
		{
			return value;
		}

		/// <returns>Type.INT</returns>
		public virtual NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cp)
		{
			return NBCEL.generic.Type.INT;
		}

		object BaseConstantPushInstruction.GetValue()
		{
			return GetValue();
		}

		/// <summary>Call corresponding visitor method(s).</summary>
		/// <remarks>
		/// Call corresponding visitor method(s). The order is:
		/// Call visitor methods of implemented interfaces first, then
		/// call methods according to the class hierarchy in descending order,
		/// i.e., the most specific visitXXX() call comes last.
		/// </remarks>
		/// <param name="v">Visitor object</param>
		public override void Accept(NBCEL.generic.Visitor v)
		{
			v.VisitPushInstruction(this);
			v.VisitStackProducer(this);
			v.VisitTypedInstruction(this);
			v.VisitConstantPushInstruction(this);
			v.VisitICONST(this);
		}
	}
}