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
	/// LSTORE - Store long into local variable
	/// <PRE>Stack: ..., value.word1, value.word2 -&gt; ...
	/// </summary>
	/// <remarks>
	/// LSTORE - Store long into local variable
	/// <PRE>Stack: ..., value.word1, value.word2 -&gt; ... </PRE>
	/// </remarks>
	public class LSTORE : NBCEL.generic.StoreInstruction
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal LSTORE()
			: base(NBCEL.Const.LSTORE, NBCEL.Const.LSTORE_0)
		{
		}

		public LSTORE(int n)
			: base(NBCEL.Const.LSTORE, NBCEL.Const.LSTORE_0, n)
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
			base.Accept(v);
			v.VisitLSTORE(this);
		}
	}
}
