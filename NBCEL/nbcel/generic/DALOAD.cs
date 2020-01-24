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
	/// DALOAD - Load double from array
	/// <PRE>Stack: ..., arrayref, index -&gt; ..., result.word1, result.word2</PRE>
	/// </summary>
	public class DALOAD : NBCEL.generic.ArrayInstruction, NBCEL.generic.StackProducer
	{
		/// <summary>Load double from array</summary>
		public DALOAD()
			: base(NBCEL.Const.DALOAD)
		{
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
			v.VisitStackProducer(this);
			v.VisitExceptionThrower(this);
			v.VisitTypedInstruction(this);
			v.VisitArrayInstruction(this);
			v.VisitDALOAD(this);
		}
	}
}