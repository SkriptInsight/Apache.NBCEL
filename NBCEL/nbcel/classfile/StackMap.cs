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
	/// This class represents a stack map attribute used for
	/// preverification of Java classes for the &lt;a
	/// href="http://java.sun.com/j2me/"&gt; Java 2 Micro Edition</a>
	/// (J2ME).
	/// </summary>
	/// <remarks>
	/// This class represents a stack map attribute used for
	/// preverification of Java classes for the &lt;a
	/// href="http://java.sun.com/j2me/"&gt; Java 2 Micro Edition</a>
	/// (J2ME). This attribute is used by the &lt;a
	/// href="http://java.sun.com/products/cldc/"&gt;KVM</a> and contained
	/// within the Code attribute of a method. See CLDC specification
	/// �5.3.1.2
	/// </remarks>
	/// <seealso cref="Code"/>
	/// <seealso cref="StackMapEntry"/>
	/// <seealso cref="StackMapType"/>
	public sealed class StackMap : NBCEL.classfile.Attribute
	{
		private NBCEL.classfile.StackMapEntry[] map;

		public StackMap(int name_index, int length, NBCEL.classfile.StackMapEntry[] map, 
			NBCEL.classfile.ConstantPool constant_pool)
			: base(NBCEL.Const.ATTR_STACK_MAP, name_index, length, constant_pool)
		{
			// Table of stack map entries
			/*
			* @param name_index Index of name
			* @param length Content length in bytes
			* @param map Table of stack map entries
			* @param constant_pool Array of constants
			*/
			this.map = map;
		}

		/// <summary>Construct object from input stream.</summary>
		/// <param name="name_index">Index of name</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="input">Input stream</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <exception cref="System.IO.IOException"/>
		internal StackMap(int name_index, int length, java.io.DataInput input, NBCEL.classfile.ConstantPool
			 constant_pool)
			: this(name_index, length, (NBCEL.classfile.StackMapEntry[])null, constant_pool)
		{
			int map_length = input.ReadUnsignedShort();
			map = new NBCEL.classfile.StackMapEntry[map_length];
			for (int i = 0; i < map_length; i++)
			{
				map[i] = new NBCEL.classfile.StackMapEntry(input, constant_pool);
			}
		}

		/// <summary>Dump stack map table attribute to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteShort(map.Length);
			foreach (NBCEL.classfile.StackMapEntry entry in map)
			{
				entry.Dump(file);
			}
		}

		/// <returns>Array of stack map entries</returns>
		public NBCEL.classfile.StackMapEntry[] GetStackMap()
		{
			return map;
		}

		/// <param name="map">Array of stack map entries</param>
		public void SetStackMap(NBCEL.classfile.StackMapEntry[] map)
		{
			this.map = map;
			int len = 2;
			// Length of 'number_of_entries' field prior to the array of stack maps
			foreach (NBCEL.classfile.StackMapEntry element in map)
			{
				len += element.GetMapEntrySize();
			}
			SetLength(len);
		}

		/// <returns>String representation.</returns>
		public override string ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder("StackMap(");
			for (int i = 0; i < map.Length; i++)
			{
				buf.Append(map[i]);
				if (i < map.Length - 1)
				{
					buf.Append(", ");
				}
			}
			buf.Append(')');
			return buf.ToString();
		}

		/// <returns>deep copy of this attribute</returns>
		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool _constant_pool
			)
		{
			NBCEL.classfile.StackMap c = (NBCEL.classfile.StackMap)Clone();
			c.map = new NBCEL.classfile.StackMapEntry[map.Length];
			for (int i = 0; i < map.Length; i++)
			{
				c.map[i] = map[i].Copy();
			}
			c.SetConstantPool(_constant_pool);
			return c;
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
		public override void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitStackMap(this);
		}

		public int GetMapLength()
		{
			return map == null ? 0 : map.Length;
		}
	}
}
