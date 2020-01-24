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
	/// This class represents a inner class attribute, i.e., the class
	/// indices of the inner and outer classes, the name and the attributes
	/// of the inner class.
	/// </summary>
	/// <seealso cref="InnerClasses"/>
	public sealed class InnerClass : System.ICloneable, NBCEL.classfile.Node
	{
		private int inner_class_index;

		private int outer_class_index;

		private int inner_name_index;

		private int inner_access_flags;

		/// <summary>Initialize from another object.</summary>
		public InnerClass(NBCEL.classfile.InnerClass c)
			: this(c.GetInnerClassIndex(), c.GetOuterClassIndex(), c.GetInnerNameIndex(), c.GetInnerAccessFlags
				())
		{
		}

		/// <summary>Construct object from file stream.</summary>
		/// <param name="file">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		internal InnerClass(java.io.DataInput file)
			: this(file.ReadUnsignedShort(), file.ReadUnsignedShort(), file.ReadUnsignedShort
				(), file.ReadUnsignedShort())
		{
		}

		/// <param name="inner_class_index">Class index in constant pool of inner class</param>
		/// <param name="outer_class_index">Class index in constant pool of outer class</param>
		/// <param name="inner_name_index">Name index in constant pool of inner class</param>
		/// <param name="inner_access_flags">Access flags of inner class</param>
		public InnerClass(int inner_class_index, int outer_class_index, int inner_name_index
			, int inner_access_flags)
		{
			this.inner_class_index = inner_class_index;
			this.outer_class_index = outer_class_index;
			this.inner_name_index = inner_name_index;
			this.inner_access_flags = inner_access_flags;
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
			v.VisitInnerClass(this);
		}

		/// <summary>Dump inner class attribute to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public void Dump(java.io.DataOutputStream file)
		{
			file.WriteShort(inner_class_index);
			file.WriteShort(outer_class_index);
			file.WriteShort(inner_name_index);
			file.WriteShort(inner_access_flags);
		}

		/// <returns>access flags of inner class.</returns>
		public int GetInnerAccessFlags()
		{
			return inner_access_flags;
		}

		/// <returns>class index of inner class.</returns>
		public int GetInnerClassIndex()
		{
			return inner_class_index;
		}

		/// <returns>name index of inner class.</returns>
		public int GetInnerNameIndex()
		{
			return inner_name_index;
		}

		/// <returns>class index of outer class.</returns>
		public int GetOuterClassIndex()
		{
			return outer_class_index;
		}

		/// <param name="inner_access_flags">access flags for this inner class</param>
		public void SetInnerAccessFlags(int inner_access_flags)
		{
			this.inner_access_flags = inner_access_flags;
		}

		/// <param name="inner_class_index">index into the constant pool for this class</param>
		public void SetInnerClassIndex(int inner_class_index)
		{
			this.inner_class_index = inner_class_index;
		}

		/// <param name="inner_name_index">index into the constant pool for this class's name
		/// 	</param>
		public void SetInnerNameIndex(int inner_name_index)
		{
			// TODO unused
			this.inner_name_index = inner_name_index;
		}

		/// <param name="outer_class_index">index into the constant pool for the owning class
		/// 	</param>
		public void SetOuterClassIndex(int outer_class_index)
		{
			// TODO unused
			this.outer_class_index = outer_class_index;
		}

		/// <returns>String representation.</returns>
		public override string ToString()
		{
			return "InnerClass(" + inner_class_index + ", " + outer_class_index + ", " + inner_name_index
				 + ", " + inner_access_flags + ")";
		}

		/// <returns>Resolved string representation</returns>
		public string ToString(NBCEL.classfile.ConstantPool constant_pool)
		{
			string outer_class_name;
			string inner_name;
			string inner_class_name = constant_pool.GetConstantString(inner_class_index, NBCEL.Const
				.CONSTANT_Class);
			inner_class_name = NBCEL.classfile.Utility.CompactClassName(inner_class_name, false
				);
			if (outer_class_index != 0)
			{
				outer_class_name = constant_pool.GetConstantString(outer_class_index, NBCEL.Const
					.CONSTANT_Class);
				outer_class_name = " of class " + NBCEL.classfile.Utility.CompactClassName(outer_class_name
					, false);
			}
			else
			{
				outer_class_name = string.Empty;
			}
			if (inner_name_index != 0)
			{
				inner_name = ((NBCEL.classfile.ConstantUtf8)constant_pool.GetConstant(inner_name_index
					, NBCEL.Const.CONSTANT_Utf8)).GetBytes();
			}
			else
			{
				inner_name = "(anonymous)";
			}
			string access = NBCEL.classfile.Utility.AccessToString(inner_access_flags, true);
			access = (access.Length == 0) ? string.Empty : (access + " ");
			return "  " + access + inner_name + "=class " + inner_class_name + outer_class_name;
		}

		/// <returns>deep copy of this object</returns>
		public NBCEL.classfile.InnerClass Copy()
		{
			return (NBCEL.classfile.InnerClass)MemberwiseClone();
			// TODO should this throw?
			return null;
		}

		object System.ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
