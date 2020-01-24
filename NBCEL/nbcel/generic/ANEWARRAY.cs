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
	/// ANEWARRAY -  Create new array of references
	/// <PRE>Stack: ..., count -&gt; ..., arrayref</PRE>
	/// </summary>
	public class ANEWARRAY : NBCEL.generic.CPInstruction, NBCEL.generic.LoadClass, NBCEL.generic.AllocationInstruction
		, NBCEL.generic.ExceptionThrower, NBCEL.generic.StackConsumer, NBCEL.generic.StackProducer
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal ANEWARRAY()
		{
		}

		public ANEWARRAY(int index)
			: base(NBCEL.Const.ANEWARRAY, index)
		{
		}

		public virtual System.Type[] GetExceptions()
		{
			return NBCEL.ExceptionConst.CreateExceptions(NBCEL.ExceptionConst.EXCS.EXCS_CLASS_AND_INTERFACE_RESOLUTION
				, NBCEL.ExceptionConst.NEGATIVE_ARRAY_SIZE_EXCEPTION);
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
			v.VisitLoadClass(this);
			v.VisitAllocationInstruction(this);
			v.VisitExceptionThrower(this);
			v.VisitStackProducer(this);
			v.VisitTypedInstruction(this);
			v.VisitCPInstruction(this);
			v.VisitANEWARRAY(this);
		}

		public virtual NBCEL.generic.ObjectType GetLoadClassType(NBCEL.generic.ConstantPoolGen
			 cpg)
		{
			NBCEL.generic.Type t = GetType(cpg);
			if (t is NBCEL.generic.ArrayType)
			{
				t = ((NBCEL.generic.ArrayType)t).GetBasicType();
			}
			return (t is NBCEL.generic.ObjectType) ? (NBCEL.generic.ObjectType)t : null;
		}
	}
}
