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
	/// LDC2_W - Push long or double from constant pool
	/// <PRE>Stack: ...
	/// </summary>
	/// <remarks>
	/// LDC2_W - Push long or double from constant pool
	/// <PRE>Stack: ... -&gt; ..., item.word1, item.word2</PRE>
	/// </remarks>
	public class LDC2_W : NBCEL.generic.CPInstruction, NBCEL.generic.PushInstruction
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal LDC2_W()
		{
		}

		public LDC2_W(int index)
			: base(NBCEL.Const.LDC2_W, index)
		{
		}

		public override NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cpg)
		{
			switch (cpg.GetConstantPool().GetConstant(base.GetIndex()).GetTag())
			{
				case NBCEL.Const.CONSTANT_Long:
				{
					return NBCEL.generic.Type.LONG;
				}

				case NBCEL.Const.CONSTANT_Double:
				{
					return NBCEL.generic.Type.DOUBLE;
				}

				default:
				{
					// Never reached
					throw new System.Exception("Unknown constant type " + base.GetOpcode());
				}
			}
		}

		public virtual object GetValue(NBCEL.generic.ConstantPoolGen cpg)
		{
			NBCEL.classfile.Constant c = cpg.GetConstantPool().GetConstant(base.GetIndex());
			switch (c.GetTag())
			{
				case NBCEL.Const.CONSTANT_Long:
				{
					return ((NBCEL.classfile.ConstantLong)c).GetBytes();
				}

				case NBCEL.Const.CONSTANT_Double:
				{
					return (object)((NBCEL.classfile.ConstantDouble)c).GetBytes();
				}

				default:
				{
					// Never reached
					throw new System.Exception("Unknown or invalid constant type at " + base.GetIndex
						());
				}
			}
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
			v.VisitPushInstruction(this);
			v.VisitTypedInstruction(this);
			v.VisitCPInstruction(this);
			v.VisitLDC2_W(this);
		}
	}
}
