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
	/// This class is derived from the abstract
	/// <see cref="Constant"/>
	/// and represents a reference to a long object.
	/// </summary>
	/// <seealso cref="Constant"/>
	public sealed class ConstantLong : NBCEL.classfile.Constant, NBCEL.classfile.ConstantObject
	{
		private long bytes;

		/// <param name="bytes">Data</param>
		public ConstantLong(long bytes)
			: base(NBCEL.Const.CONSTANT_Long)
		{
			this.bytes = bytes;
		}

		/// <summary>Initialize from another object.</summary>
		public ConstantLong(NBCEL.classfile.ConstantLong c)
			: this(c.GetBytes())
		{
		}

		/// <summary>Initialize instance from file data.</summary>
		/// <param name="file">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		internal ConstantLong(java.io.DataInput file)
			: this(file.ReadLong())
		{
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
			v.VisitConstantLong(this);
		}

		/// <summary>Dump constant long to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream file)
		{
			file.WriteByte(base.GetTag());
			file.WriteLong(bytes);
		}

		/// <returns>data, i.e., 8 bytes.</returns>
		public long GetBytes()
		{
			return bytes;
		}

		/// <param name="bytes">the raw bytes that represent this long</param>
		public void SetBytes(long bytes)
		{
			this.bytes = bytes;
		}

		/// <returns>String representation.</returns>
		public override string ToString()
		{
			return base.ToString() + "(bytes = " + bytes + ")";
		}

		/// <returns>Long object</returns>
		public object GetConstantValue(NBCEL.classfile.ConstantPool cp)
		{
			return bytes;
		}
	}
}
