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
	/// FCMPL - Compare floats: value1 &lt; value2
	/// <PRE>Stack: ..., value1, value2 -&gt; ..., result</PRE>
	/// </summary>
	public class FCMPL : NBCEL.generic.Instruction, NBCEL.generic.TypedInstruction, NBCEL.generic.StackProducer
		, NBCEL.generic.StackConsumer
	{
		public FCMPL()
			: base(NBCEL.Const.FCMPL, (short)1)
		{
		}

		/// <returns>Type.FLOAT</returns>
		public virtual NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cp)
		{
			return NBCEL.generic.Type.FLOAT;
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
			v.VisitTypedInstruction(this);
			v.VisitStackProducer(this);
			v.VisitStackConsumer(this);
			v.VisitFCMPL(this);
		}
	}
}