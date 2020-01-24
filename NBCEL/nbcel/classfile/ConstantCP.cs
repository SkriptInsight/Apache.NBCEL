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
	/// Abstract super class for Fieldref, Methodref, InterfaceMethodref and
	/// InvokeDynamic constants.
	/// </summary>
	/// <seealso cref="ConstantFieldref"/>
	/// <seealso cref="ConstantMethodref"/>
	/// <seealso cref="ConstantInterfaceMethodref"/>
	/// <seealso cref="ConstantInvokeDynamic"/>
	public abstract class ConstantCP : NBCEL.classfile.Constant
	{
		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal int class_index;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal int name_and_type_index;

		/// <summary>Initialize from another object.</summary>
		public ConstantCP(NBCEL.classfile.ConstantCP c)
			: this(c.GetTag(), c.GetClassIndex(), c.GetNameAndTypeIndex())
		{
		}

		/// <summary>Initialize instance from file data.</summary>
		/// <param name="tag">Constant type tag</param>
		/// <param name="file">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		internal ConstantCP(byte tag, java.io.DataInput file)
			: this(tag, file.ReadUnsignedShort(), file.ReadUnsignedShort())
		{
		}

		/// <param name="class_index">Reference to the class containing the field</param>
		/// <param name="name_and_type_index">and the field signature</param>
		protected internal ConstantCP(byte tag, int class_index, int name_and_type_index)
			: base(tag)
		{
			// Note that this field is used to store the
			// bootstrap_method_attr_index of a ConstantInvokeDynamic.
			// TODO make private (has getter & setter)
			// This field has the same meaning for all subclasses.
			// TODO make private (has getter & setter)
			this.class_index = class_index;
			this.name_and_type_index = name_and_type_index;
		}

		/// <summary>Dump constant field reference to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public sealed override void Dump(java.io.DataOutputStream file)
		{
			file.WriteByte(base.GetTag());
			file.WriteShort(class_index);
			file.WriteShort(name_and_type_index);
		}

		/// <returns>Reference (index) to class this constant refers to.</returns>
		public int GetClassIndex()
		{
			return class_index;
		}

		/// <param name="class_index">points to Constant_class</param>
		public void SetClassIndex(int class_index)
		{
			this.class_index = class_index;
		}

		/// <returns>Reference (index) to signature of the field.</returns>
		public int GetNameAndTypeIndex()
		{
			return name_and_type_index;
		}

		/// <param name="name_and_type_index">points to Constant_NameAndType</param>
		public void SetNameAndTypeIndex(int name_and_type_index)
		{
			this.name_and_type_index = name_and_type_index;
		}

		/// <returns>Class this field belongs to.</returns>
		public virtual string GetClass(NBCEL.classfile.ConstantPool cp)
		{
			return cp.ConstantToString(class_index, NBCEL.Const.CONSTANT_Class);
		}

		/// <returns>
		/// String representation.
		/// not final as ConstantInvokeDynamic needs to modify
		/// </returns>
		public override string ToString()
		{
			return base.ToString() + "(class_index = " + class_index + ", name_and_type_index = "
				 + name_and_type_index + ")";
		}
	}
}
