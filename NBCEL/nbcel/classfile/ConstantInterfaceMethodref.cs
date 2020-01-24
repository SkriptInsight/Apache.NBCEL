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
	/// <summary>This class represents a constant pool reference to an interface method.</summary>
	public sealed class ConstantInterfaceMethodref : NBCEL.classfile.ConstantCP
	{
		/// <summary>Initialize from another object.</summary>
		public ConstantInterfaceMethodref(NBCEL.classfile.ConstantInterfaceMethodref c)
			: base(NBCEL.Const.CONSTANT_InterfaceMethodref, c.GetClassIndex(), c.GetNameAndTypeIndex
				())
		{
		}

		/// <summary>Initialize instance from input data.</summary>
		/// <param name="input">input stream</param>
		/// <exception cref="System.IO.IOException"/>
		internal ConstantInterfaceMethodref(java.io.DataInput input)
			: base(NBCEL.Const.CONSTANT_InterfaceMethodref, input)
		{
		}

		/// <param name="class_index">Reference to the class containing the method</param>
		/// <param name="name_and_type_index">and the method signature</param>
		public ConstantInterfaceMethodref(int class_index, int name_and_type_index)
			: base(NBCEL.Const.CONSTANT_InterfaceMethodref, class_index, name_and_type_index)
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
			v.VisitConstantInterfaceMethodref(this);
		}
	}
}
