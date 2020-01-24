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
*/
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	/// This class represents a stack map entry recording the types of
	/// local variables and the the of stack items at a given byte code offset.
	/// </summary>
	/// <remarks>
	/// This class represents a stack map entry recording the types of
	/// local variables and the the of stack items at a given byte code offset.
	/// See CLDC specification 5.3.1.2
	/// </remarks>
	/// <seealso cref="StackMap"/>
	/// <seealso cref="StackMapType"/>
	public sealed class StackMapEntry : NBCEL.classfile.Node, System.ICloneable
	{
		private int frame_type;

		private int byte_code_offset;

		private NBCEL.classfile.StackMapType[] types_of_locals;

		private NBCEL.classfile.StackMapType[] types_of_stack_items;

		private NBCEL.classfile.ConstantPool constant_pool;

		/// <summary>Construct object from input stream.</summary>
		/// <param name="input">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		internal StackMapEntry(java.io.DataInput input, NBCEL.classfile.ConstantPool constantPool
			)
			: this(input.ReadByte() & unchecked((int)(0xFF)), -1, null, null, constantPool)
		{
			if (frame_type >= NBCEL.Const.SAME_FRAME && frame_type <= NBCEL.Const.SAME_FRAME_MAX)
			{
				byte_code_offset = frame_type - NBCEL.Const.SAME_FRAME;
			}
			else if (frame_type >= NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME && frame_type <=
				 NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_MAX)
			{
				byte_code_offset = frame_type - NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME;
				types_of_stack_items = new NBCEL.classfile.StackMapType[1];
				types_of_stack_items[0] = new NBCEL.classfile.StackMapType(input, constantPool);
			}
			else if (frame_type == NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED)
			{
				byte_code_offset = input.ReadShort();
				types_of_stack_items = new NBCEL.classfile.StackMapType[1];
				types_of_stack_items[0] = new NBCEL.classfile.StackMapType(input, constantPool);
			}
			else if (frame_type >= NBCEL.Const.CHOP_FRAME && frame_type <= NBCEL.Const.CHOP_FRAME_MAX)
			{
				byte_code_offset = input.ReadShort();
			}
			else if (frame_type == NBCEL.Const.SAME_FRAME_EXTENDED)
			{
				byte_code_offset = input.ReadShort();
			}
			else if (frame_type >= NBCEL.Const.APPEND_FRAME && frame_type <= NBCEL.Const.APPEND_FRAME_MAX)
			{
				byte_code_offset = input.ReadShort();
				int number_of_locals = frame_type - 251;
				types_of_locals = new NBCEL.classfile.StackMapType[number_of_locals];
				for (int i = 0; i < number_of_locals; i++)
				{
					types_of_locals[i] = new NBCEL.classfile.StackMapType(input, constantPool);
				}
			}
			else if (frame_type == NBCEL.Const.FULL_FRAME)
			{
				byte_code_offset = input.ReadShort();
				int number_of_locals = input.ReadShort();
				types_of_locals = new NBCEL.classfile.StackMapType[number_of_locals];
				for (int i = 0; i < number_of_locals; i++)
				{
					types_of_locals[i] = new NBCEL.classfile.StackMapType(input, constantPool);
				}
				int number_of_stack_items = input.ReadShort();
				types_of_stack_items = new NBCEL.classfile.StackMapType[number_of_stack_items];
				for (int i = 0; i < number_of_stack_items; i++)
				{
					types_of_stack_items[i] = new NBCEL.classfile.StackMapType(input, constantPool);
				}
			}
			else
			{
				/* Can't happen */
				throw new NBCEL.classfile.ClassFormatException("Invalid frame type found while parsing stack map table: "
					 + frame_type);
			}
		}

		/// <summary>DO NOT USE</summary>
		/// <param name="byteCodeOffset"/>
		/// <param name="numberOfLocals">NOT USED</param>
		/// <param name="typesOfLocals">
		/// array of
		/// <see cref="StackMapType"/>
		/// s of locals
		/// </param>
		/// <param name="numberOfStackItems">NOT USED</param>
		/// <param name="typesOfStackItems">
		/// array ot
		/// <see cref="StackMapType"/>
		/// s of stack items
		/// </param>
		/// <param name="constantPool">the constant pool</param>
		[System.ObsoleteAttribute(@"Since 6.0, use StackMapEntry(int, int, StackMapType[], StackMapType[], ConstantPool) instead"
			)]
		public StackMapEntry(int byteCodeOffset, int numberOfLocals, NBCEL.classfile.StackMapType
			[] typesOfLocals, int numberOfStackItems, NBCEL.classfile.StackMapType[] typesOfStackItems
			, NBCEL.classfile.ConstantPool constantPool)
		{
			this.byte_code_offset = byteCodeOffset;
			this.types_of_locals = typesOfLocals != null ? typesOfLocals : new NBCEL.classfile.StackMapType
				[0];
			this.types_of_stack_items = typesOfStackItems != null ? typesOfStackItems : new NBCEL.classfile.StackMapType
				[0];
			this.constant_pool = constantPool;
		}

		/// <summary>Create an instance</summary>
		/// <param name="tag">the frame_type to use</param>
		/// <param name="byteCodeOffset"/>
		/// <param name="typesOfLocals">
		/// array of
		/// <see cref="StackMapType"/>
		/// s of locals
		/// </param>
		/// <param name="typesOfStackItems">
		/// array ot
		/// <see cref="StackMapType"/>
		/// s of stack items
		/// </param>
		/// <param name="constantPool">the constant pool</param>
		public StackMapEntry(int tag, int byteCodeOffset, NBCEL.classfile.StackMapType[] 
			typesOfLocals, NBCEL.classfile.StackMapType[] typesOfStackItems, NBCEL.classfile.ConstantPool
			 constantPool)
		{
			this.frame_type = tag;
			this.byte_code_offset = byteCodeOffset;
			this.types_of_locals = typesOfLocals != null ? typesOfLocals : new NBCEL.classfile.StackMapType
				[0];
			this.types_of_stack_items = typesOfStackItems != null ? typesOfStackItems : new NBCEL.classfile.StackMapType
				[0];
			this.constant_pool = constantPool;
		}

		/// <summary>Dump stack map entry</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public void Dump(java.io.DataOutputStream file)
		{
			file.Write(frame_type);
			if (frame_type >= NBCEL.Const.SAME_FRAME && frame_type <= NBCEL.Const.SAME_FRAME_MAX)
			{
			}
			else if (frame_type >= NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME && frame_type <=
				 NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_MAX)
			{
				// nothing to be done
				types_of_stack_items[0].Dump(file);
			}
			else if (frame_type == NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED)
			{
				file.WriteShort(byte_code_offset);
				types_of_stack_items[0].Dump(file);
			}
			else if (frame_type >= NBCEL.Const.CHOP_FRAME && frame_type <= NBCEL.Const.CHOP_FRAME_MAX)
			{
				file.WriteShort(byte_code_offset);
			}
			else if (frame_type == NBCEL.Const.SAME_FRAME_EXTENDED)
			{
				file.WriteShort(byte_code_offset);
			}
			else if (frame_type >= NBCEL.Const.APPEND_FRAME && frame_type <= NBCEL.Const.APPEND_FRAME_MAX)
			{
				file.WriteShort(byte_code_offset);
				foreach (NBCEL.classfile.StackMapType type in types_of_locals)
				{
					type.Dump(file);
				}
			}
			else if (frame_type == NBCEL.Const.FULL_FRAME)
			{
				file.WriteShort(byte_code_offset);
				file.WriteShort(types_of_locals.Length);
				foreach (NBCEL.classfile.StackMapType type in types_of_locals)
				{
					type.Dump(file);
				}
				file.WriteShort(types_of_stack_items.Length);
				foreach (NBCEL.classfile.StackMapType type in types_of_stack_items)
				{
					type.Dump(file);
				}
			}
			else
			{
				/* Can't happen */
				throw new NBCEL.classfile.ClassFormatException("Invalid Stack map table tag: " + 
					frame_type);
			}
		}

		/// <returns>String representation.</returns>
		public override string ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder(64);
			buf.Append("(");
			if (frame_type >= NBCEL.Const.SAME_FRAME && frame_type <= NBCEL.Const.SAME_FRAME_MAX)
			{
				buf.Append("SAME");
			}
			else if (frame_type >= NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME && frame_type <=
				 NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_MAX)
			{
				buf.Append("SAME_LOCALS_1_STACK");
			}
			else if (frame_type == NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED)
			{
				buf.Append("SAME_LOCALS_1_STACK_EXTENDED");
			}
			else if (frame_type >= NBCEL.Const.CHOP_FRAME && frame_type <= NBCEL.Const.CHOP_FRAME_MAX)
			{
				buf.Append("CHOP ").Append((251 - frame_type).ToString());
			}
			else if (frame_type == NBCEL.Const.SAME_FRAME_EXTENDED)
			{
				buf.Append("SAME_EXTENDED");
			}
			else if (frame_type >= NBCEL.Const.APPEND_FRAME && frame_type <= NBCEL.Const.APPEND_FRAME_MAX)
			{
				buf.Append("APPEND ").Append((frame_type - 251).ToString());
			}
			else if (frame_type == NBCEL.Const.FULL_FRAME)
			{
				buf.Append("FULL");
			}
			else
			{
				buf.Append("UNKNOWN (").Append(frame_type).Append(")");
			}
			buf.Append(", offset delta=").Append(byte_code_offset);
			if (types_of_locals.Length > 0)
			{
				buf.Append(", locals={");
				for (int i = 0; i < types_of_locals.Length; i++)
				{
					buf.Append(types_of_locals[i]);
					if (i < types_of_locals.Length - 1)
					{
						buf.Append(", ");
					}
				}
				buf.Append("}");
			}
			if (types_of_stack_items.Length > 0)
			{
				buf.Append(", stack items={");
				for (int i = 0; i < types_of_stack_items.Length; i++)
				{
					buf.Append(types_of_stack_items[i]);
					if (i < types_of_stack_items.Length - 1)
					{
						buf.Append(", ");
					}
				}
				buf.Append("}");
			}
			buf.Append(")");
			return buf.ToString();
		}

		/// <summary>Calculate stack map entry size</summary>
		internal int GetMapEntrySize()
		{
			if (frame_type >= NBCEL.Const.SAME_FRAME && frame_type <= NBCEL.Const.SAME_FRAME_MAX)
			{
				return 1;
			}
			else if (frame_type >= NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME && frame_type <=
				 NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_MAX)
			{
				return 1 + (types_of_stack_items[0].HasIndex() ? 3 : 1);
			}
			else if (frame_type == NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED)
			{
				return 3 + (types_of_stack_items[0].HasIndex() ? 3 : 1);
			}
			else if (frame_type >= NBCEL.Const.CHOP_FRAME && frame_type <= NBCEL.Const.CHOP_FRAME_MAX)
			{
				return 3;
			}
			else if (frame_type == NBCEL.Const.SAME_FRAME_EXTENDED)
			{
				return 3;
			}
			else if (frame_type >= NBCEL.Const.APPEND_FRAME && frame_type <= NBCEL.Const.APPEND_FRAME_MAX)
			{
				int len = 3;
				foreach (NBCEL.classfile.StackMapType types_of_local in types_of_locals)
				{
					len += types_of_local.HasIndex() ? 3 : 1;
				}
				return len;
			}
			else if (frame_type == NBCEL.Const.FULL_FRAME)
			{
				int len = 7;
				foreach (NBCEL.classfile.StackMapType types_of_local in types_of_locals)
				{
					len += types_of_local.HasIndex() ? 3 : 1;
				}
				foreach (NBCEL.classfile.StackMapType types_of_stack_item in types_of_stack_items)
				{
					len += types_of_stack_item.HasIndex() ? 3 : 1;
				}
				return len;
			}
			else
			{
				throw new System.Exception("Invalid StackMap frame_type: " + frame_type);
			}
		}

		public void SetFrameType(int f)
		{
			if (f >= NBCEL.Const.SAME_FRAME && f <= NBCEL.Const.SAME_FRAME_MAX)
			{
				byte_code_offset = f - NBCEL.Const.SAME_FRAME;
			}
			else if (f >= NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME && f <= NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_MAX)
			{
				byte_code_offset = f - NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME;
			}
			else if (f == NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED)
			{
			}
			else if (f >= NBCEL.Const.CHOP_FRAME && f <= NBCEL.Const.CHOP_FRAME_MAX)
			{
			}
			else if (f == NBCEL.Const.SAME_FRAME_EXTENDED)
			{
			}
			else if (f >= NBCEL.Const.APPEND_FRAME && f <= NBCEL.Const.APPEND_FRAME_MAX)
			{
			}
			else if (f == NBCEL.Const.FULL_FRAME)
			{
			}
			else
			{
				// CHECKSTYLE IGNORE EmptyBlock
				// CHECKSTYLE IGNORE EmptyBlock
				// CHECKSTYLE IGNORE EmptyBlock
				// CHECKSTYLE IGNORE EmptyBlock
				// CHECKSTYLE IGNORE EmptyBlock
				throw new System.Exception("Invalid StackMap frame_type");
			}
			frame_type = f;
		}

		public int GetFrameType()
		{
			return frame_type;
		}

		public void SetByteCodeOffset(int new_offset)
		{
			if (new_offset < 0 || new_offset > 32767)
			{
				throw new System.Exception("Invalid StackMap offset: " + new_offset);
			}
			if (frame_type >= NBCEL.Const.SAME_FRAME && frame_type <= NBCEL.Const.SAME_FRAME_MAX)
			{
				if (new_offset > NBCEL.Const.SAME_FRAME_MAX)
				{
					frame_type = NBCEL.Const.SAME_FRAME_EXTENDED;
				}
				else
				{
					frame_type = new_offset;
				}
			}
			else if (frame_type >= NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME && frame_type <=
				 NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_MAX)
			{
				if (new_offset > NBCEL.Const.SAME_FRAME_MAX)
				{
					frame_type = NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED;
				}
				else
				{
					frame_type = NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME + new_offset;
				}
			}
			else if (frame_type == NBCEL.Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED)
			{
			}
			else if (frame_type >= NBCEL.Const.CHOP_FRAME && frame_type <= NBCEL.Const.CHOP_FRAME_MAX)
			{
			}
			else if (frame_type == NBCEL.Const.SAME_FRAME_EXTENDED)
			{
			}
			else if (frame_type >= NBCEL.Const.APPEND_FRAME && frame_type <= NBCEL.Const.APPEND_FRAME_MAX)
			{
			}
			else if (frame_type == NBCEL.Const.FULL_FRAME)
			{
			}
			else
			{
				// CHECKSTYLE IGNORE EmptyBlock
				// CHECKSTYLE IGNORE EmptyBlock
				// CHECKSTYLE IGNORE EmptyBlock
				// CHECKSTYLE IGNORE EmptyBlock
				// CHECKSTYLE IGNORE EmptyBlock
				throw new System.Exception("Invalid StackMap frame_type: " + frame_type);
			}
			byte_code_offset = new_offset;
		}

		/// <summary>
		/// Update the distance (as an offset delta) from this StackMap
		/// entry to the next.
		/// </summary>
		/// <remarks>
		/// Update the distance (as an offset delta) from this StackMap
		/// entry to the next.  Note that this might cause the the
		/// frame type to change.  Note also that delta may be negative.
		/// </remarks>
		/// <param name="delta">offset delta</param>
		public void UpdateByteCodeOffset(int delta)
		{
			SetByteCodeOffset(byte_code_offset + delta);
		}

		public int GetByteCodeOffset()
		{
			return byte_code_offset;
		}

		[System.ObsoleteAttribute(@"since 6.0")]
		public void SetNumberOfLocals(int n)
		{
		}

		// TODO unused
		public int GetNumberOfLocals()
		{
			return types_of_locals.Length;
		}

		public void SetTypesOfLocals(NBCEL.classfile.StackMapType[] types)
		{
			types_of_locals = types != null ? types : new NBCEL.classfile.StackMapType[0];
		}

		public NBCEL.classfile.StackMapType[] GetTypesOfLocals()
		{
			return types_of_locals;
		}

		[System.ObsoleteAttribute(@"since 6.0")]
		public void SetNumberOfStackItems(int n)
		{
		}

		// TODO unused
		public int GetNumberOfStackItems()
		{
			return types_of_stack_items.Length;
		}

		public void SetTypesOfStackItems(NBCEL.classfile.StackMapType[] types)
		{
			types_of_stack_items = types != null ? types : new NBCEL.classfile.StackMapType[0
				];
		}

		public NBCEL.classfile.StackMapType[] GetTypesOfStackItems()
		{
			return types_of_stack_items;
		}

		/// <returns>deep copy of this object</returns>
		public NBCEL.classfile.StackMapEntry Copy()
		{
			NBCEL.classfile.StackMapEntry e;
			e = (NBCEL.classfile.StackMapEntry)MemberwiseClone();
			e.types_of_locals = new NBCEL.classfile.StackMapType[types_of_locals.Length];
			for (int i = 0; i < types_of_locals.Length; i++)
			{
				e.types_of_locals[i] = types_of_locals[i].Copy();
			}
			e.types_of_stack_items = new NBCEL.classfile.StackMapType[types_of_stack_items.Length
				];
			for (int i = 0; i < types_of_stack_items.Length; i++)
			{
				e.types_of_stack_items[i] = types_of_stack_items[i].Copy();
			}
			return e;
		}

		/// <summary>
		/// Called by objects that are traversing the nodes of the tree implicitely
		/// defined by the contents of a Java class.
		/// </summary>
		/// <remarks>
		/// Called by objects that are traversing the nodes of the tree implicitely
		/// defined by the contents of a Java class. I.e., the hierarchy of methods,
		/// fields, attributes, etc. spawns a tree of objects.
		/// </remarks>
		/// <param name="v">Visitor object</param>
		public void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitStackMapEntry(this);
		}

		/// <returns>Constant pool used by this object.</returns>
		public NBCEL.classfile.ConstantPool GetConstantPool()
		{
			return constant_pool;
		}

		/// <param name="constant_pool">Constant pool to be used for this object.</param>
		public void SetConstantPool(NBCEL.classfile.ConstantPool constant_pool)
		{
			this.constant_pool = constant_pool;
		}

		object System.ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
