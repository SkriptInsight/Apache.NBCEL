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
	/// <summary>JSR - Jump to subroutine</summary>
	public class JSR : NBCEL.generic.JsrInstruction, NBCEL.generic.VariableLengthInstruction
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal JSR()
		{
		}

		public JSR(NBCEL.generic.InstructionHandle target)
			: base(NBCEL.Const.JSR, target)
		{
		}

		/// <summary>Dump instruction as byte code to stream out.</summary>
		/// <param name="out">Output stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream @out)
		{
			base.SetIndex(GetTargetOffset());
			if (base.GetOpcode() == NBCEL.Const.JSR)
			{
				base.Dump(@out);
			}
			else
			{
				// JSR_W
				base.SetIndex(GetTargetOffset());
				@out.WriteByte(base.GetOpcode());
				@out.WriteInt(base.GetIndex());
			}
		}

		protected internal override int UpdatePosition(int offset, int max_offset)
		{
			int i = GetTargetOffset();
			// Depending on old position value
			SetPosition(GetPosition() + offset);
			// Position may be shifted by preceding expansions
			if (System.Math.Abs(i) >= (short.MaxValue - max_offset))
			{
				// to large for short (estimate)
				base.SetOpcode(NBCEL.Const.JSR_W);
				short old_length = (short)base.GetLength();
				base.SetLength(5);
				return base.GetLength() - old_length;
			}
			return 0;
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
			v.VisitVariableLengthInstruction(this);
			v.VisitBranchInstruction(this);
			v.VisitJsrInstruction(this);
			v.VisitJSR(this);
		}
	}
}
