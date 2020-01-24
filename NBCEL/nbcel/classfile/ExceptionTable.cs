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
	/// <summary>
	/// This class represents the table of exceptions that are thrown by a
	/// method.
	/// </summary>
	/// <remarks>
	/// This class represents the table of exceptions that are thrown by a
	/// method. This attribute may be used once per method.  The name of
	/// this class is <em>ExceptionTable</em> for historical reasons; The
	/// Java Virtual Machine Specification, Second Edition defines this
	/// attribute using the name <em>Exceptions</em> (which is inconsistent
	/// with the other classes).
	/// </remarks>
	/// <seealso cref="Code"/>
	public sealed class ExceptionTable : NBCEL.classfile.Attribute
	{
		private int[] exception_index_table;

		/// <summary>Initialize from another object.</summary>
		/// <remarks>
		/// Initialize from another object. Note that both objects use the same
		/// references (shallow copy). Use copy() for a physical copy.
		/// </remarks>
		public ExceptionTable(NBCEL.classfile.ExceptionTable c)
			: this(c.GetNameIndex(), c.GetLength(), c.GetExceptionIndexTable(), c.GetConstantPool
				())
		{
		}

		/// <param name="name_index">Index in constant pool</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="exception_index_table">Table of indices in constant pool</param>
		/// <param name="constant_pool">Array of constants</param>
		public ExceptionTable(int name_index, int length, int[] exception_index_table, NBCEL.classfile.ConstantPool
			 constant_pool)
			: base(NBCEL.Const.ATTR_EXCEPTIONS, name_index, length, constant_pool)
		{
			// constant pool
			this.exception_index_table = exception_index_table != null ? exception_index_table
				 : new int[0];
		}

		/// <summary>Construct object from input stream.</summary>
		/// <param name="name_index">Index in constant pool</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="input">Input stream</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <exception cref="System.IO.IOException"/>
		internal ExceptionTable(int name_index, int length, java.io.DataInput input, NBCEL.classfile.ConstantPool
			 constant_pool)
			: this(name_index, length, (int[])null, constant_pool)
		{
			int number_of_exceptions = input.ReadUnsignedShort();
			exception_index_table = new int[number_of_exceptions];
			for (int i = 0; i < number_of_exceptions; i++)
			{
				exception_index_table[i] = input.ReadUnsignedShort();
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
			v.VisitExceptionTable(this);
		}

		/// <summary>Dump exceptions attribute to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteShort(exception_index_table.Length);
			foreach (int index in exception_index_table)
			{
				file.WriteShort(index);
			}
		}

		/// <returns>Array of indices into constant pool of thrown exceptions.</returns>
		public int[] GetExceptionIndexTable()
		{
			return exception_index_table;
		}

		/// <returns>Length of exception table.</returns>
		public int GetNumberOfExceptions()
		{
			return exception_index_table == null ? 0 : exception_index_table.Length;
		}

		/// <returns>class names of thrown exceptions</returns>
		public string[] GetExceptionNames()
		{
			string[] names = new string[exception_index_table.Length];
			for (int i = 0; i < exception_index_table.Length; i++)
			{
				names[i] = base.GetConstantPool().GetConstantString(exception_index_table[i], NBCEL.Const
					.CONSTANT_Class).Replace('/', '.');
			}
			return names;
		}

		/// <param name="exception_index_table">
		/// the list of exception indexes
		/// Also redefines number_of_exceptions according to table length.
		/// </param>
		public void SetExceptionIndexTable(int[] exception_index_table)
		{
			this.exception_index_table = exception_index_table != null ? exception_index_table
				 : new int[0];
		}

		/// <returns>String representation, i.e., a list of thrown exceptions.</returns>
		public override string ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			string str;
			buf.Append("Exceptions: ");
			for (int i = 0; i < exception_index_table.Length; i++)
			{
				str = base.GetConstantPool().GetConstantString(exception_index_table[i], NBCEL.Const
					.CONSTANT_Class);
				buf.Append(NBCEL.classfile.Utility.CompactClassName(str, false));
				if (i < exception_index_table.Length - 1)
				{
					buf.Append(", ");
				}
			}
			return buf.ToString();
		}

		/// <returns>deep copy of this attribute</returns>
		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool _constant_pool
			)
		{
			NBCEL.classfile.ExceptionTable c = (NBCEL.classfile.ExceptionTable)Clone();
			if (exception_index_table != null)
			{
				c.exception_index_table = new int[exception_index_table.Length];
				System.Array.Copy(exception_index_table, 0, c.exception_index_table, 0, exception_index_table
					.Length);
			}
			c.SetConstantPool(_constant_pool);
			return c;
		}
	}
}
