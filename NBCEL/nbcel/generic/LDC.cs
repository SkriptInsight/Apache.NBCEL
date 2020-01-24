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
	/// <summary>LDC - Push item from constant pool.</summary>
	/// <remarks>
	/// LDC - Push item from constant pool.
	/// <PRE>Stack: ... -&gt; ..., item</PRE>
	/// </remarks>
	public class LDC : NBCEL.generic.CPInstruction, NBCEL.generic.PushInstruction, NBCEL.generic.ExceptionThrower
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal LDC()
		{
		}

		public LDC(int index)
			: base(NBCEL.Const.LDC_W, index)
		{
			SetSize();
		}

		// Adjust to proper size
		protected internal void SetSize()
		{
			if (base.GetIndex() <= NBCEL.Const.MAX_BYTE)
			{
				// Fits in one byte?
				base.SetOpcode(NBCEL.Const.LDC);
				base.SetLength(2);
			}
			else
			{
				base.SetOpcode(NBCEL.Const.LDC_W);
				base.SetLength(3);
			}
		}

		/// <summary>Dump instruction as byte code to stream out.</summary>
		/// <param name="out">Output stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream @out)
		{
			@out.WriteByte(base.GetOpcode());
			if (base.GetLength() == 2)
			{
				// TODO useless check?
				@out.WriteByte(base.GetIndex());
			}
			else
			{
				@out.WriteShort(base.GetIndex());
			}
		}

		/// <summary>Set the index to constant pool and adjust size.</summary>
		public sealed override void SetIndex(int index)
		{
			base.SetIndex(index);
			SetSize();
		}

		/// <summary>Read needed data (e.g.</summary>
		/// <remarks>Read needed data (e.g. index) from file.</remarks>
		/// <exception cref="System.IO.IOException"/>
		protected internal override void InitFromFile(NBCEL.util.ByteSequence bytes, bool
			 wide)
		{
			base.SetLength(2);
			base.SetIndex(bytes.ReadUnsignedByte());
		}

		public virtual object GetValue(NBCEL.generic.ConstantPoolGen cpg)
		{
			NBCEL.classfile.Constant c = cpg.GetConstantPool().GetConstant(base.GetIndex());
			switch (c.GetTag())
			{
				case NBCEL.Const.CONSTANT_String:
				{
					int i = ((NBCEL.classfile.ConstantString)c).GetStringIndex();
					c = cpg.GetConstantPool().GetConstant(i);
					return ((NBCEL.classfile.ConstantUtf8)c).GetBytes();
				}

				case NBCEL.Const.CONSTANT_Float:
				{
					return ((NBCEL.classfile.ConstantFloat)c).GetBytes();
				}

				case NBCEL.Const.CONSTANT_Integer:
				{
					return ((NBCEL.classfile.ConstantInteger)c).GetBytes();
				}

				case NBCEL.Const.CONSTANT_Class:
				{
					int nameIndex = ((NBCEL.classfile.ConstantClass)c).GetNameIndex();
					c = cpg.GetConstantPool().GetConstant(nameIndex);
					return new NBCEL.generic.ObjectType(((NBCEL.classfile.ConstantUtf8)c).GetBytes());
				}

				default:
				{
					// Never reached
					throw new System.Exception("Unknown or invalid constant type at " + base.GetIndex
						());
				}
			}
		}

		public override NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cpg)
		{
			switch (cpg.GetConstantPool().GetConstant(base.GetIndex()).GetTag())
			{
				case NBCEL.Const.CONSTANT_String:
				{
					return NBCEL.generic.Type.STRING;
				}

				case NBCEL.Const.CONSTANT_Float:
				{
					return NBCEL.generic.Type.FLOAT;
				}

				case NBCEL.Const.CONSTANT_Integer:
				{
					return NBCEL.generic.Type.INT;
				}

				case NBCEL.Const.CONSTANT_Class:
				{
					return NBCEL.generic.Type.CLASS;
				}

				default:
				{
					// Never reached
					throw new System.Exception("Unknown or invalid constant type at " + base.GetIndex
						());
				}
			}
		}

		public virtual System.Type[] GetExceptions()
		{
			return NBCEL.ExceptionConst.CreateExceptions(NBCEL.ExceptionConst.EXCS.EXCS_STRING_RESOLUTION
				);
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
			v.VisitExceptionThrower(this);
			v.VisitTypedInstruction(this);
			v.VisitCPInstruction(this);
			v.VisitLDC(this);
		}
	}
}
