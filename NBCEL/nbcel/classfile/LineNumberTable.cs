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
	/// This class represents a table of line numbers for debugging
	/// purposes.
	/// </summary>
	/// <remarks>
	/// This class represents a table of line numbers for debugging
	/// purposes. This attribute is used by the <em>Code</em> attribute. It
	/// contains pairs of PCs and line numbers.
	/// </remarks>
	/// <seealso cref="Code"/>
	/// <seealso cref="LineNumber"/>
	public sealed class LineNumberTable : NBCEL.classfile.Attribute
	{
		private const int MAX_LINE_LENGTH = 72;

		private NBCEL.classfile.LineNumber[] line_number_table;

		public LineNumberTable(NBCEL.classfile.LineNumberTable c)
			: this(c.GetNameIndex(), c.GetLength(), c.GetLineNumberTable(), c.GetConstantPool
				())
		{
		}

		public LineNumberTable(int name_index, int length, NBCEL.classfile.LineNumber[] line_number_table
			, NBCEL.classfile.ConstantPool constant_pool)
			: base(NBCEL.Const.ATTR_LINE_NUMBER_TABLE, name_index, length, constant_pool)
		{
			// Table of line/numbers pairs
			/*
			* Initialize from another object. Note that both objects use the same
			* references (shallow copy). Use copy() for a physical copy.
			*/
			/*
			* @param name_index Index of name
			* @param length Content length in bytes
			* @param line_number_table Table of line/numbers pairs
			* @param constant_pool Array of constants
			*/
			this.line_number_table = line_number_table;
		}

		/// <summary>Construct object from input stream.</summary>
		/// <param name="name_index">Index of name</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="input">Input stream</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <exception cref="System.IO.IOException">if an I/O Exception occurs in readUnsignedShort
		/// 	</exception>
		internal LineNumberTable(int name_index, int length, java.io.DataInput input, NBCEL.classfile.ConstantPool
			 constant_pool)
			: this(name_index, length, (NBCEL.classfile.LineNumber[])null, constant_pool)
		{
			int line_number_table_length = input.ReadUnsignedShort();
			line_number_table = new NBCEL.classfile.LineNumber[line_number_table_length];
			for (int i = 0; i < line_number_table_length; i++)
			{
				line_number_table[i] = new NBCEL.classfile.LineNumber(input);
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
			v.VisitLineNumberTable(this);
		}

		/// <summary>Dump line number table attribute to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException">if an I/O Exception occurs in writeShort</exception>
		public override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteShort(line_number_table.Length);
			foreach (NBCEL.classfile.LineNumber lineNumber in line_number_table)
			{
				lineNumber.Dump(file);
			}
		}

		/// <returns>Array of (pc offset, line number) pairs.</returns>
		public NBCEL.classfile.LineNumber[] GetLineNumberTable()
		{
			return line_number_table;
		}

		/// <param name="line_number_table">the line number entries for this table</param>
		public void SetLineNumberTable(NBCEL.classfile.LineNumber[] line_number_table)
		{
			this.line_number_table = line_number_table;
		}

		/// <returns>String representation.</returns>
		public override string ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			System.Text.StringBuilder line = new System.Text.StringBuilder();
			string newLine = Sharpen.Runtime.GetProperty("line.separator", "\n");
			for (int i = 0; i < line_number_table.Length; i++)
			{
				line.Append(line_number_table[i].ToString());
				if (i < line_number_table.Length - 1)
				{
					line.Append(", ");
				}
				if ((line.Length > MAX_LINE_LENGTH) && (i < line_number_table.Length - 1))
				{
					line.Append(newLine);
					buf.Append(line);
					line.Length = 0;
				}
			}
			buf.Append(line);
			return buf.ToString();
		}

		/// <summary>Map byte code positions to source code lines.</summary>
		/// <param name="pos">byte code offset</param>
		/// <returns>corresponding line in source code</returns>
		public int GetSourceLine(int pos)
		{
			int l = 0;
			int r = line_number_table.Length - 1;
			if (r < 0)
			{
				return -1;
			}
			int min_index = -1;
			int min = -1;
			do
			{
				/* Do a binary search since the array is ordered.
				*/
				int i = (int)(((uint)(l + r)) >> 1);
				int j = line_number_table[i].GetStartPC();
				if (j == pos)
				{
					return line_number_table[i].GetLineNumber();
				}
				else if (pos < j)
				{
					r = i - 1;
				}
				else
				{
					l = i + 1;
				}
				/* If exact match can't be found (which is the most common case)
				* return the line number that corresponds to the greatest index less
				* than pos.
				*/
				if (j < pos && j > min)
				{
					min = j;
					min_index = i;
				}
			}
			while (l <= r);
			/* It's possible that we did not find any valid entry for the bytecode
			* offset we were looking for.
			*/
			if (min_index < 0)
			{
				return -1;
			}
			return line_number_table[min_index].GetLineNumber();
		}

		/// <returns>deep copy of this attribute</returns>
		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool _constant_pool
			)
		{
			// TODO could use the lower level constructor and thereby allow
			// line_number_table to be made final
			NBCEL.classfile.LineNumberTable c = (NBCEL.classfile.LineNumberTable)Clone();
			c.line_number_table = new NBCEL.classfile.LineNumber[line_number_table.Length];
			for (int i = 0; i < line_number_table.Length; i++)
			{
				c.line_number_table[i] = line_number_table[i].Copy();
			}
			c.SetConstantPool(_constant_pool);
			return c;
		}

		public int GetTableLength()
		{
			return line_number_table == null ? 0 : line_number_table.Length;
		}
	}
}
