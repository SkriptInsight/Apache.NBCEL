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

namespace NBCEL.classfile
{
	/// <summary>
	/// This class represents the type of a local variable or item on stack
	/// used in the StackMap entries.
	/// </summary>
	/// <seealso cref="StackMapEntry"/>
	/// <seealso cref="StackMap"/>
	/// <seealso cref="NBCEL.Const"/>
	public sealed class StackMapType : System.ICloneable
	{
		private byte type;

		private int index = -1;

		private NBCEL.classfile.ConstantPool constant_pool;

		/// <summary>Construct object from file stream.</summary>
		/// <param name="file">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		internal StackMapType(java.io.DataInput file, NBCEL.classfile.ConstantPool constant_pool
			)
			: this(file.ReadByte(), -1, constant_pool)
		{
			// Index to CONSTANT_Class or offset
			if (HasIndex())
			{
				this.index = file.ReadShort();
			}
			this.constant_pool = constant_pool;
		}

		/// <param name="type">type tag as defined in the Constants interface</param>
		/// <param name="index">index to constant pool, or byte code offset</param>
		public StackMapType(byte type, int index, NBCEL.classfile.ConstantPool constant_pool
			)
		{
			if ((((sbyte)type) < NBCEL.Const.ITEM_Bogus) || (type > NBCEL.Const.ITEM_NewObject
				))
			{
				throw new System.Exception("Illegal type for StackMapType: " + type);
			}
			this.type = type;
			this.index = index;
			this.constant_pool = constant_pool;
		}

		public void SetType(byte t)
		{
			if ((((sbyte)t) < NBCEL.Const.ITEM_Bogus) || (t > NBCEL.Const.ITEM_NewObject))
			{
				throw new System.Exception("Illegal type for StackMapType: " + t);
			}
			type = t;
		}

		public byte GetType()
		{
			return type;
		}

		public void SetIndex(int t)
		{
			index = t;
		}

		/// <returns>
		/// index to constant pool if type == ITEM_Object, or offset
		/// in byte code, if type == ITEM_NewObject, and -1 otherwise
		/// </returns>
		public int GetIndex()
		{
			return index;
		}

		/// <summary>Dump type entries to file.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public void Dump(java.io.DataOutputStream file)
		{
			file.WriteByte(type);
			if (HasIndex())
			{
				file.WriteShort(GetIndex());
			}
		}

		/// <returns>true, if type is either ITEM_Object or ITEM_NewObject</returns>
		public bool HasIndex()
		{
			return type == NBCEL.Const.ITEM_Object || type == NBCEL.Const.ITEM_NewObject;
		}

		private string PrintIndex()
		{
			if (type == NBCEL.Const.ITEM_Object)
			{
				if (index < 0)
				{
					return ", class=<unknown>";
				}
				return ", class=" + constant_pool.ConstantToString(index, NBCEL.Const.CONSTANT_Class
					);
			}
			else if (type == NBCEL.Const.ITEM_NewObject)
			{
				return ", offset=" + index;
			}
			else
			{
				return string.Empty;
			}
		}

		/// <returns>String representation</returns>
		public override string ToString()
		{
			return "(type=" + NBCEL.Const.GetItemName(type) + PrintIndex() + ")";
		}

		/// <returns>deep copy of this object</returns>
		public NBCEL.classfile.StackMapType Copy()
		{
			return (NBCEL.classfile.StackMapType)MemberwiseClone();
			// TODO should this throw?
			return null;
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
