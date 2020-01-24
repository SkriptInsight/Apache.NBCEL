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
	/// and represents a reference to a dynamically computed constant.
	/// </summary>
	/// <seealso cref="Constant"/>
	/// <seealso><a href="https://bugs.openjdk.java.net/secure/attachment/74618/constant-dynamic.html">
	/// * Change request for JEP 309</a></seealso>
	/// <since>6.3</since>
	public sealed class ConstantDynamic : NBCEL.classfile.ConstantCP
	{
		/// <summary>Initialize from another object.</summary>
		public ConstantDynamic(NBCEL.classfile.ConstantDynamic c)
			: this(c.GetBootstrapMethodAttrIndex(), c.GetNameAndTypeIndex())
		{
		}

		/// <summary>Initialize instance from file data.</summary>
		/// <param name="file">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		internal ConstantDynamic(java.io.DataInput file)
			: this(file.ReadShort(), file.ReadShort())
		{
		}

		public ConstantDynamic(int bootstrap_method_attr_index, int name_and_type_index)
			: base(NBCEL.Const.CONSTANT_Dynamic, bootstrap_method_attr_index, name_and_type_index
				)
		{
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
			v.VisitConstantDynamic(this);
		}

		/// <returns>
		/// Reference (index) to bootstrap method this constant refers to.
		/// Note that this method is a functional duplicate of getClassIndex
		/// for use by ConstantInvokeDynamic.
		/// </returns>
		/// <since>6.0</since>
		public int GetBootstrapMethodAttrIndex()
		{
			return base.GetClassIndex();
		}

		// AKA bootstrap_method_attr_index
		/// <returns>String representation</returns>
		public override string ToString()
		{
			return base.ToString().Replace("class_index", "bootstrap_method_attr_index");
		}
	}
}
