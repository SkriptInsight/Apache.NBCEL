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
	/// <summary>IINC - Increment local variable by constant</summary>
	public class IINC : NBCEL.generic.LocalVariableInstruction
	{
		private bool wide;

		private int c;

		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal IINC()
		{
		}

		/// <param name="n">index of local variable</param>
		/// <param name="c">increment factor</param>
		public IINC(int n, int c)
			: base()
		{
			// Default behavior of LocalVariableInstruction causes error
			base.SetOpcode(NBCEL.Const.IINC);
			base.SetLength((short)3);
			SetIndex(n);
			// May set wide as side effect
			SetIncrement(c);
		}

		/// <summary>Dump instruction as byte code to stream out.</summary>
		/// <param name="out">Output stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream @out)
		{
			if (wide)
			{
				@out.WriteByte(NBCEL.Const.WIDE);
			}
			@out.WriteByte(base.GetOpcode());
			if (wide)
			{
				@out.WriteShort(base.GetIndex());
				@out.WriteShort(c);
			}
			else
			{
				@out.WriteByte(base.GetIndex());
				@out.WriteByte(c);
			}
		}

		private void SetWide()
		{
			wide = base.GetIndex() > NBCEL.Const.MAX_BYTE;
			if (c > 0)
			{
				wide = wide || (c > byte.MaxValue);
			}
			else
			{
				wide = wide || (c < byte.MinValue);
			}
			if (wide)
			{
				base.SetLength(6);
			}
			else
			{
				// wide byte included
				base.SetLength(3);
			}
		}

		/// <summary>Read needed data (e.g.</summary>
		/// <remarks>Read needed data (e.g. index) from file.</remarks>
		/// <exception cref="System.IO.IOException"/>
		protected internal override void InitFromFile(NBCEL.util.ByteSequence bytes, bool
			 wide)
		{
			this.wide = wide;
			if (wide)
			{
				base.SetLength(6);
				base.SetIndexOnly(bytes.ReadUnsignedShort());
				c = bytes.ReadShort();
			}
			else
			{
				base.SetLength(3);
				base.SetIndexOnly(bytes.ReadUnsignedByte());
				c = bytes.ReadByte();
			}
		}

		/// <returns>mnemonic for instruction</returns>
		public override string ToString(bool verbose)
		{
			return base.ToString(verbose) + " " + c;
		}

		/// <summary>Set index of local variable.</summary>
		public sealed override void SetIndex(int n)
		{
			if (n < 0)
			{
				throw new NBCEL.generic.ClassGenException("Negative index value: " + n);
			}
			base.SetIndexOnly(n);
			SetWide();
		}

		/// <returns>increment factor</returns>
		public int GetIncrement()
		{
			return c;
		}

		/// <summary>Set increment factor.</summary>
		public void SetIncrement(int c)
		{
			this.c = c;
			SetWide();
		}

		/// <returns>int type</returns>
		public override NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cp)
		{
			return NBCEL.generic.Type.INT;
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
			v.VisitLocalVariableInstruction(this);
			v.VisitIINC(this);
		}
	}
}
