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
	/// BIPUSH - Push byte on stack
	/// <PRE>Stack: ...
	/// </summary>
	/// <remarks>
	/// BIPUSH - Push byte on stack
	/// <PRE>Stack: ... -&gt; ..., value</PRE>
	/// </remarks>
	public class BIPUSH : NBCEL.generic.Instruction, NBCEL.generic.ConstantPushInstruction<byte>
	{
		private byte b;

		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal BIPUSH()
		{
		}

		/// <summary>Push byte on stack</summary>
		public BIPUSH(byte b)
			: base(NBCEL.Const.BIPUSH, (short)2)
		{
			this.b = b;
		}

		/// <summary>Dump instruction as byte code to stream out.</summary>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream @out)
		{
			base.Dump(@out);
			@out.WriteByte(b);
		}

		/// <returns>mnemonic for instruction</returns>
		public override string ToString(bool verbose)
		{
			return base.ToString(verbose) + " " + b;
		}

		/// <summary>Read needed data (e.g.</summary>
		/// <remarks>Read needed data (e.g. index) from file.</remarks>
		/// <exception cref="System.IO.IOException"/>
		protected internal override void InitFromFile(NBCEL.util.ByteSequence bytes, bool
			 wide)
		{
			base.SetLength(2);
			b = bytes.ReadByte();
		}

		public virtual byte GetValue()
		{
			return b;
		}

		/// <returns>Type.BYTE</returns>
		public virtual NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cp)
		{
			return NBCEL.generic.Type.BYTE;
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
			v.VisitBIPUSH(this);
		}
	}
}
