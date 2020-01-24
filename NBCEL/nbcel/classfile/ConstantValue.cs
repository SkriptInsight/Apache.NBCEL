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
	/// This class is derived from <em>Attribute</em> and represents a constant
	/// value, i.e., a default value for initializing a class field.
	/// </summary>
	/// <remarks>
	/// This class is derived from <em>Attribute</em> and represents a constant
	/// value, i.e., a default value for initializing a class field.
	/// This class is instantiated by the <em>Attribute.readAttribute()</em> method.
	/// </remarks>
	/// <seealso cref="Attribute"/>
	public sealed class ConstantValue : NBCEL.classfile.Attribute
	{
		private int constantvalue_index;

		/// <summary>Initialize from another object.</summary>
		/// <remarks>
		/// Initialize from another object. Note that both objects use the same
		/// references (shallow copy). Use clone() for a physical copy.
		/// </remarks>
		public ConstantValue(NBCEL.classfile.ConstantValue c)
			: this(c.GetNameIndex(), c.GetLength(), c.GetConstantValueIndex(), c.GetConstantPool
				())
		{
		}

		/// <summary>Construct object from input stream.</summary>
		/// <param name="name_index">Name index in constant pool</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="input">Input stream</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <exception cref="System.IO.IOException"/>
		internal ConstantValue(int name_index, int length, java.io.DataInput input, NBCEL.classfile.ConstantPool
			 constant_pool)
			: this(name_index, length, input.ReadUnsignedShort(), constant_pool)
		{
		}

		/// <param name="name_index">Name index in constant pool</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="constantvalue_index">Index in constant pool</param>
		/// <param name="constant_pool">Array of constants</param>
		public ConstantValue(int name_index, int length, int constantvalue_index, NBCEL.classfile.ConstantPool
			 constant_pool)
			: base(NBCEL.Const.ATTR_CONSTANT_VALUE, name_index, length, constant_pool)
		{
			this.constantvalue_index = constantvalue_index;
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
			v.VisitConstantValue(this);
		}

		/// <summary>Dump constant value attribute to file stream on binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteShort(constantvalue_index);
		}

		/// <returns>Index in constant pool of constant value.</returns>
		public int GetConstantValueIndex()
		{
			return constantvalue_index;
		}

		/// <param name="constantvalue_index">the index info the constant pool of this constant value
		/// 	</param>
		public void SetConstantValueIndex(int constantvalue_index)
		{
			this.constantvalue_index = constantvalue_index;
		}

		/// <returns>String representation of constant value.</returns>
		public override string ToString()
		{
			NBCEL.classfile.Constant c = base.GetConstantPool().GetConstant(constantvalue_index
				);
			string buf;
			int i;
			switch (c.GetTag())
			{
				case NBCEL.Const.CONSTANT_Long:
				{
					// Print constant to string depending on its type
					buf = ((NBCEL.classfile.ConstantLong)c).GetBytes().ToString();
					break;
				}

				case NBCEL.Const.CONSTANT_Float:
				{
					buf = ((NBCEL.classfile.ConstantFloat)c).GetBytes().ToString();
					break;
				}

				case NBCEL.Const.CONSTANT_Double:
				{
					buf = ((NBCEL.classfile.ConstantDouble)c).GetBytes().ToString();
					break;
				}

				case NBCEL.Const.CONSTANT_Integer:
				{
					buf = ((NBCEL.classfile.ConstantInteger)c).GetBytes().ToString();
					break;
				}

				case NBCEL.Const.CONSTANT_String:
				{
					i = ((NBCEL.classfile.ConstantString)c).GetStringIndex();
					c = base.GetConstantPool().GetConstant(i, NBCEL.Const.CONSTANT_Utf8);
					buf = "\"" + NBCEL.classfile.Utility.ConvertString(((NBCEL.classfile.ConstantUtf8
						)c).GetBytes()) + "\"";
					break;
				}

				default:
				{
					throw new System.InvalidOperationException("Type of ConstValue invalid: " + c);
				}
			}
			return buf;
		}

		/// <returns>deep copy of this attribute</returns>
		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool _constant_pool
			)
		{
			NBCEL.classfile.ConstantValue c = (NBCEL.classfile.ConstantValue)Clone();
			c.SetConstantPool(_constant_pool);
			return c;
		}
	}
}
