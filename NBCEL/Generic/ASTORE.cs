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
	///     ASTORE - Store reference into local variable
	///     <PRE>Stack ..., objectref -&gt; ...
	/// </summary>
	/// <remarks>
	///     ASTORE - Store reference into local variable
	///     <PRE>Stack ..., objectref -&gt; ... </PRE>
	/// </remarks>
	public class ASTORE : StoreInstruction
    {
	    /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
	    /// <remarks>
	    ///     Empty constructor needed for Instruction.readInstruction.
	    ///     Not to be used otherwise.
	    /// </remarks>
	    internal ASTORE()
            : base(Const.ASTORE, Const.ASTORE_0)
        {
        }

	    /// <summary>Store reference into local variable</summary>
	    /// <param name="n">index of local variable</param>
	    public ASTORE(int n)
            : base(Const.ASTORE, Const.ASTORE_0, n)
        {
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
            base.Accept(v);
            v.VisitASTORE(this);
        }
    }
}