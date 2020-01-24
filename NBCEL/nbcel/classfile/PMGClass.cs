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
	/// This class is derived from <em>Attribute</em> and represents a reference
	/// to a PMG attribute.
	/// </summary>
	/// <seealso cref="Attribute"/>
	public sealed class PMGClass : NBCEL.classfile.Attribute
	{
		private int pmg_class_index;

		private int pmg_index;

		/// <summary>Initialize from another object.</summary>
		/// <remarks>
		/// Initialize from another object. Note that both objects use the same
		/// references (shallow copy). Use copy() for a physical copy.
		/// </remarks>
		public PMGClass(NBCEL.classfile.PMGClass c)
			: this(c.GetNameIndex(), c.GetLength(), c.GetPMGIndex(), c.GetPMGClassIndex(), c.
				GetConstantPool())
		{
		}

		/// <summary>Construct object from input stream.</summary>
		/// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="input">Input stream</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <exception cref="System.IO.IOException"/>
		internal PMGClass(int name_index, int length, java.io.DataInput input, NBCEL.classfile.ConstantPool
			 constant_pool)
			: this(name_index, length, input.ReadUnsignedShort(), input.ReadUnsignedShort(), 
				constant_pool)
		{
		}

		/// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="pmg_index">index in constant pool for source file name</param>
		/// <param name="pmg_class_index">Index in constant pool to CONSTANT_Utf8</param>
		/// <param name="constant_pool">Array of constants</param>
		public PMGClass(int name_index, int length, int pmg_index, int pmg_class_index, NBCEL.classfile.ConstantPool
			 constant_pool)
			: base(NBCEL.Const.ATTR_PMG, name_index, length, constant_pool)
		{
			this.pmg_index = pmg_index;
			this.pmg_class_index = pmg_class_index;
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
			Println("Visiting non-standard PMGClass object");
		}

		/// <summary>Dump source file attribute to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteShort(pmg_index);
			file.WriteShort(pmg_class_index);
		}

		/// <returns>Index in constant pool of source file name.</returns>
		public int GetPMGClassIndex()
		{
			return pmg_class_index;
		}

		/// <param name="pmg_class_index"/>
		public void SetPMGClassIndex(int pmg_class_index)
		{
			this.pmg_class_index = pmg_class_index;
		}

		/// <returns>Index in constant pool of source file name.</returns>
		public int GetPMGIndex()
		{
			return pmg_index;
		}

		/// <param name="pmg_index"/>
		public void SetPMGIndex(int pmg_index)
		{
			this.pmg_index = pmg_index;
		}

		/// <returns>PMG name.</returns>
		public string GetPMGName()
		{
			NBCEL.classfile.ConstantUtf8 c = (NBCEL.classfile.ConstantUtf8)base.GetConstantPool
				().GetConstant(pmg_index, NBCEL.Const.CONSTANT_Utf8);
			return c.GetBytes();
		}

		/// <returns>PMG class name.</returns>
		public string GetPMGClassName()
		{
			NBCEL.classfile.ConstantUtf8 c = (NBCEL.classfile.ConstantUtf8)base.GetConstantPool
				().GetConstant(pmg_class_index, NBCEL.Const.CONSTANT_Utf8);
			return c.GetBytes();
		}

		/// <returns>String representation</returns>
		public override string ToString()
		{
			return "PMGClass(" + GetPMGName() + ", " + GetPMGClassName() + ")";
		}

		/// <returns>deep copy of this attribute</returns>
		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool _constant_pool
			)
		{
			return (NBCEL.classfile.Attribute)Clone();
		}
	}
}
