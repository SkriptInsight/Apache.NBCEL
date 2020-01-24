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
	/// This class is derived from <em>Attribute</em> and denotes that this is a
	/// deprecated method.
	/// </summary>
	/// <remarks>
	/// This class is derived from <em>Attribute</em> and denotes that this is a
	/// deprecated method.
	/// It is instantiated from the <em>Attribute.readAttribute()</em> method.
	/// </remarks>
	/// <seealso cref="Attribute"/>
	public sealed class Deprecated : NBCEL.classfile.Attribute
	{
		private byte[] bytes;

		/// <summary>Initialize from another object.</summary>
		/// <remarks>
		/// Initialize from another object. Note that both objects use the same
		/// references (shallow copy). Use clone() for a physical copy.
		/// </remarks>
		public Deprecated(NBCEL.classfile.Deprecated c)
			: this(c.GetNameIndex(), c.GetLength(), c.GetBytes(), c.GetConstantPool())
		{
		}

		/// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="bytes">Attribute contents</param>
		/// <param name="constant_pool">Array of constants</param>
		public Deprecated(int name_index, int length, byte[] bytes, NBCEL.classfile.ConstantPool
			 constant_pool)
			: base(NBCEL.Const.ATTR_DEPRECATED, name_index, length, constant_pool)
		{
			this.bytes = bytes;
		}

		/// <summary>Construct object from input stream.</summary>
		/// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="input">Input stream</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <exception cref="System.IO.IOException"/>
		internal Deprecated(int name_index, int length, java.io.DataInput input, NBCEL.classfile.ConstantPool
			 constant_pool)
			: this(name_index, length, (byte[])null, constant_pool)
		{
			if (length > 0)
			{
				bytes = new byte[length];
				input.ReadFully(bytes);
				Println("Deprecated attribute with length > 0");
			}
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
			v.VisitDeprecated(this);
		}

		/// <summary>Dump source file attribute to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			if (base.GetLength() > 0)
			{
				file.Write(bytes, 0, base.GetLength());
			}
		}

		/// <returns>data bytes.</returns>
		public byte[] GetBytes()
		{
			return bytes;
		}

		/// <param name="bytes">the raw bytes that represents this byte array</param>
		public void SetBytes(byte[] bytes)
		{
			this.bytes = bytes;
		}

		/// <returns>attribute name</returns>
		public override string ToString()
		{
			return NBCEL.Const.GetAttributeName(NBCEL.Const.ATTR_DEPRECATED);
		}

		/// <returns>deep copy of this attribute</returns>
		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool _constant_pool
			)
		{
			NBCEL.classfile.Deprecated c = (NBCEL.classfile.Deprecated)Clone();
			if (bytes != null)
			{
				c.bytes = new byte[bytes.Length];
				System.Array.Copy(bytes, 0, c.bytes, 0, bytes.Length);
			}
			c.SetConstantPool(_constant_pool);
			return c;
		}
	}
}
