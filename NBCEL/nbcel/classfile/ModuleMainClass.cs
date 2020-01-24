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
	/// <summary>This class is derived from <em>Attribute</em> and indicates the main class of a module.
	/// 	</summary>
	/// <remarks>
	/// This class is derived from <em>Attribute</em> and indicates the main class of a module.
	/// There may be at most one ModuleMainClass attribute in a ClassFile structure.
	/// </remarks>
	/// <seealso cref="Attribute"/>
	public sealed class ModuleMainClass : NBCEL.classfile.Attribute
	{
		private int main_class_index;

		/// <summary>Initialize from another object.</summary>
		/// <remarks>
		/// Initialize from another object. Note that both objects use the same
		/// references (shallow copy). Use copy() for a physical copy.
		/// </remarks>
		public ModuleMainClass(NBCEL.classfile.ModuleMainClass c)
			: this(c.GetNameIndex(), c.GetLength(), c.GetHostClassIndex(), c.GetConstantPool(
				))
		{
		}

		/// <param name="name_index">Index in constant pool</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="main_class_index">Host class index</param>
		/// <param name="constant_pool">Array of constants</param>
		public ModuleMainClass(int name_index, int length, int main_class_index, NBCEL.classfile.ConstantPool
			 constant_pool)
			: base(NBCEL.Const.ATTR_NEST_MEMBERS, name_index, length, constant_pool)
		{
			this.main_class_index = main_class_index;
		}

		/// <summary>Construct object from input stream.</summary>
		/// <param name="name_index">Index in constant pool</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="input">Input stream</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <exception cref="System.IO.IOException"/>
		internal ModuleMainClass(int name_index, int length, java.io.DataInput input, NBCEL.classfile.ConstantPool
			 constant_pool)
			: this(name_index, length, 0, constant_pool)
		{
			main_class_index = input.ReadUnsignedShort();
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
			v.VisitModuleMainClass(this);
		}

		/// <summary>Dump ModuleMainClass attribute to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException">if an I/O error occurs.</exception>
		public override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteShort(main_class_index);
		}

		/// <returns>index into constant pool of host class name.</returns>
		public int GetHostClassIndex()
		{
			return main_class_index;
		}

		/// <param name="main_class_index">the host class index</param>
		public void SetHostClassIndex(int main_class_index)
		{
			this.main_class_index = main_class_index;
		}

		/// <returns>String representation</returns>
		public override string ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			buf.Append("ModuleMainClass: ");
			string class_name = base.GetConstantPool().GetConstantString(main_class_index, NBCEL.Const
				.CONSTANT_Class);
			buf.Append(NBCEL.classfile.Utility.CompactClassName(class_name, false));
			return buf.ToString();
		}

		/// <returns>deep copy of this attribute</returns>
		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool _constant_pool
			)
		{
			NBCEL.classfile.ModuleMainClass c = (NBCEL.classfile.ModuleMainClass)Clone();
			c.SetConstantPool(_constant_pool);
			return c;
		}
	}
}
