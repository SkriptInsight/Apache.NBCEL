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
	/// LDIV - Divide longs
	/// <PRE>Stack: ..., value1.word1, value1.word2, value2.word1, value2.word2 -&gt;</PRE>
	/// ..., result.word1, result.word2
	/// </summary>
	public class LDIV : NBCEL.generic.ArithmeticInstruction, NBCEL.generic.ExceptionThrower
	{
		public LDIV()
			: base(NBCEL.Const.LDIV)
		{
		}

		public virtual System.Type[] GetExceptions()
		{
			return new System.Type[] { NBCEL.ExceptionConst.ARITHMETIC_EXCEPTION };
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
			v.VisitExceptionThrower(this);
			v.VisitTypedInstruction(this);
			v.VisitStackProducer(this);
			v.VisitStackConsumer(this);
			v.VisitArithmeticInstruction(this);
			v.VisitLDIV(this);
		}
	}
}
