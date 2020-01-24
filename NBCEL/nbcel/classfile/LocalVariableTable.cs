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
	/// This class represents colection of local variables in a
	/// method.
	/// </summary>
	/// <remarks>
	/// This class represents colection of local variables in a
	/// method. This attribute is contained in the <em>Code</em> attribute.
	/// </remarks>
	/// <seealso cref="Code"/>
	/// <seealso cref="LocalVariable"/>
	public class LocalVariableTable : NBCEL.classfile.Attribute
	{
		private NBCEL.classfile.LocalVariable[] local_variable_table;

		/// <summary>Initialize from another object.</summary>
		/// <remarks>
		/// Initialize from another object. Note that both objects use the same
		/// references (shallow copy). Use copy() for a physical copy.
		/// </remarks>
		public LocalVariableTable(NBCEL.classfile.LocalVariableTable c)
			: this(c.GetNameIndex(), c.GetLength(), c.GetLocalVariableTable(), c.GetConstantPool
				())
		{
		}

		/// <param name="name_index">Index in constant pool to `LocalVariableTable'</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="local_variable_table">Table of local variables</param>
		/// <param name="constant_pool">Array of constants</param>
		public LocalVariableTable(int name_index, int length, NBCEL.classfile.LocalVariable
			[] local_variable_table, NBCEL.classfile.ConstantPool constant_pool)
			: base(NBCEL.Const.ATTR_LOCAL_VARIABLE_TABLE, name_index, length, constant_pool)
		{
			// variables
			this.local_variable_table = local_variable_table;
		}

		/// <summary>Construct object from input stream.</summary>
		/// <param name="name_index">Index in constant pool</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="input">Input stream</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <exception cref="System.IO.IOException"/>
		internal LocalVariableTable(int name_index, int length, java.io.DataInput input, 
			NBCEL.classfile.ConstantPool constant_pool)
			: this(name_index, length, (NBCEL.classfile.LocalVariable[])null, constant_pool)
		{
			int local_variable_table_length = input.ReadUnsignedShort();
			local_variable_table = new NBCEL.classfile.LocalVariable[local_variable_table_length
				];
			for (int i = 0; i < local_variable_table_length; i++)
			{
				local_variable_table[i] = new NBCEL.classfile.LocalVariable(input, constant_pool);
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
			v.VisitLocalVariableTable(this);
		}

		/// <summary>Dump local variable table attribute to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public sealed override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteShort(local_variable_table.Length);
			foreach (NBCEL.classfile.LocalVariable variable in local_variable_table)
			{
				variable.Dump(file);
			}
		}

		/// <returns>Array of local variables of method.</returns>
		public NBCEL.classfile.LocalVariable[] GetLocalVariableTable()
		{
			return local_variable_table;
		}

		/// <param name="index">the variable slot</param>
		/// <returns>the first LocalVariable that matches the slot or null if not found</returns>
		[System.ObsoleteAttribute(@"since 5.2 because multiple variables can share the same slot, use getLocalVariable(int index, int pc) instead."
			)]
		public NBCEL.classfile.LocalVariable GetLocalVariable(int index)
		{
			foreach (NBCEL.classfile.LocalVariable variable in local_variable_table)
			{
				if (variable.GetIndex() == index)
				{
					return variable;
				}
			}
			return null;
		}

		/// <param name="index">the variable slot</param>
		/// <param name="pc">the current pc that this variable is alive</param>
		/// <returns>the LocalVariable that matches or null if not found</returns>
		public NBCEL.classfile.LocalVariable GetLocalVariable(int index, int pc)
		{
			foreach (NBCEL.classfile.LocalVariable variable in local_variable_table)
			{
				if (variable.GetIndex() == index)
				{
					int start_pc = variable.GetStartPC();
					int end_pc = start_pc + variable.GetLength();
					if ((pc >= start_pc) && (pc <= end_pc))
					{
						return variable;
					}
				}
			}
			return null;
		}

		public void SetLocalVariableTable(NBCEL.classfile.LocalVariable[] local_variable_table
			)
		{
			this.local_variable_table = local_variable_table;
		}

		/// <returns>String representation.</returns>
		public sealed override string ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			for (int i = 0; i < local_variable_table.Length; i++)
			{
				buf.Append(local_variable_table[i]);
				if (i < local_variable_table.Length - 1)
				{
					buf.Append('\n');
				}
			}
			return buf.ToString();
		}

		/// <returns>deep copy of this attribute</returns>
		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool _constant_pool
			)
		{
			NBCEL.classfile.LocalVariableTable c = (NBCEL.classfile.LocalVariableTable)Clone(
				);
			c.local_variable_table = new NBCEL.classfile.LocalVariable[local_variable_table.Length
				];
			for (int i = 0; i < local_variable_table.Length; i++)
			{
				c.local_variable_table[i] = local_variable_table[i].Copy();
			}
			c.SetConstantPool(_constant_pool);
			return c;
		}

		public int GetTableLength()
		{
			return local_variable_table == null ? 0 : local_variable_table.Length;
		}
	}
}
