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
	/// ARRAYLENGTH -  Get length of array
	/// <PRE>Stack: ..., arrayref -&gt; ..., length</PRE>
	/// </summary>
	public class ARRAYLENGTH : NBCEL.generic.Instruction, NBCEL.generic.ExceptionThrower
		, NBCEL.generic.StackProducer, NBCEL.generic.StackConsumer
	{
		/// <summary>Get length of array</summary>
		public ARRAYLENGTH()
			: base(NBCEL.Const.ARRAYLENGTH, (short)1)
		{
		}

		/* since 6.0 */
		/// <returns>exceptions this instruction may cause</returns>
		public virtual System.Type[] GetExceptions()
		{
			return new System.Type[] { NBCEL.ExceptionConst.NULL_POINTER_EXCEPTION };
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
			v.VisitStackProducer(this);
			v.VisitARRAYLENGTH(this);
		}
	}
}
