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
	/// and represents a reference to a module.
	/// <p>Note: Early access Java 9 support- currently subject to change</p>
	/// </summary>
	/// <seealso cref="Constant"/>
	/// <since>6.1</since>
	public sealed class ConstantModule : NBCEL.classfile.Constant, NBCEL.classfile.ConstantObject
	{
		private int name_index;

		/// <summary>Initialize from another object.</summary>
		public ConstantModule(NBCEL.classfile.ConstantModule c)
			: this(c.GetNameIndex())
		{
		}

		/// <summary>Initialize instance from file data.</summary>
		/// <param name="file">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		internal ConstantModule(java.io.DataInput file)
			: this(file.ReadUnsignedShort())
		{
		}

		/// <param name="name_index">
		/// Name index in constant pool.  Should refer to a
		/// ConstantUtf8.
		/// </param>
		public ConstantModule(int name_index)
			: base(NBCEL.Const.CONSTANT_Module)
		{
			this.name_index = name_index;
		}

		/// <summary>
		/// Called by objects that are traversing the nodes of the tree implicitly
		/// defined by the contents of a Java class.
		/// </summary>
		/// <remarks>
		/// Called by objects that are traversing the nodes of the tree implicitly
		/// defined by the contents of a Java class. I.e., the hierarchy of methods,
		/// fields, attributes, etc. spawns a tree of objects.
		/// </remarks>
		/// <param name="v">Visitor object</param>
		public override void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitConstantModule(this);
		}

		/// <summary>Dump constant module to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream file)
		{
			file.WriteByte(base.GetTag());
			file.WriteShort(name_index);
		}

		/// <returns>Name index in constant pool of module name.</returns>
		public int GetNameIndex()
		{
			return name_index;
		}

		/// <param name="name_index">the name index in the constant pool of this Constant Module
		/// 	</param>
		public void SetNameIndex(int name_index)
		{
			this.name_index = name_index;
		}

		/// <returns>String object</returns>
		public object GetConstantValue(NBCEL.classfile.ConstantPool cp)
		{
			NBCEL.classfile.Constant c = cp.GetConstant(name_index, NBCEL.Const.CONSTANT_Utf8
				);
			return ((NBCEL.classfile.ConstantUtf8)c).GetBytes();
		}

		/// <returns>dereferenced string</returns>
		public string GetBytes(NBCEL.classfile.ConstantPool cp)
		{
			return (string)GetConstantValue(cp);
		}

		/// <returns>String representation.</returns>
		public override string ToString()
		{
			return base.ToString() + "(name_index = " + name_index + ")";
		}
	}
}
