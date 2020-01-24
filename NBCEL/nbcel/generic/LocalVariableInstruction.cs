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
	/// <summary>Abstract super class for instructions dealing with local variables.</summary>
	public abstract class LocalVariableInstruction : NBCEL.generic.Instruction, NBCEL.generic.TypedInstruction
		, NBCEL.generic.IndexedInstruction
	{
		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal int n = -1;

		private short c_tag = -1;

		private short canon_tag = -1;

		// index of referenced variable
		// compact version, such as ILOAD_0
		// canonical tag such as ILOAD
		private bool Wide()
		{
			return n > NBCEL.Const.MAX_BYTE;
		}

		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// tag and length are defined in readInstruction and initFromFile, respectively.
		/// </remarks>
		internal LocalVariableInstruction(short canon_tag, short c_tag)
			: base()
		{
			this.canon_tag = canon_tag;
			this.c_tag = c_tag;
		}

		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Also used by IINC()!
		/// </remarks>
		internal LocalVariableInstruction()
		{
		}

		/// <param name="opcode">Instruction opcode</param>
		/// <param name="c_tag">Instruction number for compact version, ALOAD_0, e.g.</param>
		/// <param name="n">local variable index (unsigned short)</param>
		protected internal LocalVariableInstruction(short opcode, short c_tag, int n)
			: base(opcode, (short)2)
		{
			this.c_tag = c_tag;
			canon_tag = opcode;
			SetIndex(n);
		}

		/// <summary>Dump instruction as byte code to stream out.</summary>
		/// <param name="out">Output stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream @out)
		{
			if (Wide())
			{
				@out.WriteByte(NBCEL.Const.WIDE);
			}
			@out.WriteByte(base.GetOpcode());
			if (base.GetLength() > 1)
			{
				// Otherwise ILOAD_n, instruction, e.g.
				if (Wide())
				{
					@out.WriteShort(n);
				}
				else
				{
					@out.WriteByte(n);
				}
			}
		}

		/// <summary>
		/// Long output format:
		/// &lt;name of opcode&gt; "["&lt;opcode number&gt;"]"
		/// "("&lt;length of instruction&gt;")" "&lt;"&lt; local variable index&gt;"&gt;"
		/// </summary>
		/// <param name="verbose">long/short format switch</param>
		/// <returns>mnemonic for instruction</returns>
		public override string ToString(bool verbose)
		{
			short _opcode = base.GetOpcode();
			if (((_opcode >= NBCEL.Const.ILOAD_0) && (_opcode <= NBCEL.Const.ALOAD_3)) || ((_opcode
				 >= NBCEL.Const.ISTORE_0) && (_opcode <= NBCEL.Const.ASTORE_3)))
			{
				return base.ToString(verbose);
			}
			return base.ToString(verbose) + " " + n;
		}

		/// <summary>Read needed data (e.g.</summary>
		/// <remarks>
		/// Read needed data (e.g. index) from file.
		/// <pre>
		/// (ILOAD &lt;= tag &lt;= ALOAD_3) || (ISTORE &lt;= tag &lt;= ASTORE_3)
		/// </pre>
		/// </remarks>
		/// <exception cref="System.IO.IOException"/>
		protected internal override void InitFromFile(NBCEL.util.ByteSequence bytes, bool
			 wide)
		{
			if (wide)
			{
				n = bytes.ReadUnsignedShort();
				base.SetLength(4);
			}
			else
			{
				short _opcode = base.GetOpcode();
				if (((_opcode >= NBCEL.Const.ILOAD) && (_opcode <= NBCEL.Const.ALOAD)) || ((_opcode
					 >= NBCEL.Const.ISTORE) && (_opcode <= NBCEL.Const.ASTORE)))
				{
					n = bytes.ReadUnsignedByte();
					base.SetLength(2);
				}
				else if (_opcode <= NBCEL.Const.ALOAD_3)
				{
					// compact load instruction such as ILOAD_2
					n = (_opcode - NBCEL.Const.ILOAD_0) % 4;
					base.SetLength(1);
				}
				else
				{
					// Assert ISTORE_0 <= tag <= ASTORE_3
					n = (_opcode - NBCEL.Const.ISTORE_0) % 4;
					base.SetLength(1);
				}
			}
		}

		/// <returns>local variable index (n) referred by this instruction.</returns>
		public int GetIndex()
		{
			return n;
		}

		/// <summary>Set the local variable index.</summary>
		/// <remarks>
		/// Set the local variable index.
		/// also updates opcode and length
		/// TODO Why?
		/// </remarks>
		/// <seealso cref="SetIndexOnly(int)"/>
		public virtual void SetIndex(int n)
		{
			// TODO could be package-protected?
			if ((n < 0) || (n > NBCEL.Const.MAX_SHORT))
			{
				throw new NBCEL.generic.ClassGenException("Illegal value: " + n);
			}
			this.n = n;
			// Cannot be < 0 as this is checked above
			if (n <= 3)
			{
				// Use more compact instruction xLOAD_n
				base.SetOpcode((short)(c_tag + n));
				base.SetLength(1);
			}
			else
			{
				base.SetOpcode(canon_tag);
				if (Wide())
				{
					base.SetLength(4);
				}
				else
				{
					base.SetLength(2);
				}
			}
		}

		/// <returns>canonical tag for instruction, e.g., ALOAD for ALOAD_0</returns>
		public virtual short GetCanonicalTag()
		{
			return canon_tag;
		}

		/// <summary>
		/// Returns the type associated with the instruction -
		/// in case of ALOAD or ASTORE Type.OBJECT is returned.
		/// </summary>
		/// <remarks>
		/// Returns the type associated with the instruction -
		/// in case of ALOAD or ASTORE Type.OBJECT is returned.
		/// This is just a bit incorrect, because ALOAD and ASTORE
		/// may work on every ReferenceType (including Type.NULL) and
		/// ASTORE may even work on a ReturnaddressType .
		/// </remarks>
		/// <returns>type associated with the instruction</returns>
		public virtual NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cp)
		{
			switch (canon_tag)
			{
				case NBCEL.Const.ILOAD:
				case NBCEL.Const.ISTORE:
				{
					return NBCEL.generic.Type.INT;
				}

				case NBCEL.Const.LLOAD:
				case NBCEL.Const.LSTORE:
				{
					return NBCEL.generic.Type.LONG;
				}

				case NBCEL.Const.DLOAD:
				case NBCEL.Const.DSTORE:
				{
					return NBCEL.generic.Type.DOUBLE;
				}

				case NBCEL.Const.FLOAD:
				case NBCEL.Const.FSTORE:
				{
					return NBCEL.generic.Type.FLOAT;
				}

				case NBCEL.Const.ALOAD:
				case NBCEL.Const.ASTORE:
				{
					return NBCEL.generic.Type.OBJECT;
				}

				default:
				{
					throw new NBCEL.generic.ClassGenException("Oops: unknown case in switch" + canon_tag
						);
				}
			}
		}

		/// <summary>Sets the index of the referenced variable (n) only</summary>
		/// <since>6.0</since>
		/// <seealso cref="SetIndex(int)"/>
		internal void SetIndexOnly(int n)
		{
			this.n = n;
		}
	}
}
