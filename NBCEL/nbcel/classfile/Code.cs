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
	/// This class represents a chunk of Java byte code contained in a
	/// method.
	/// </summary>
	/// <remarks>
	/// This class represents a chunk of Java byte code contained in a
	/// method. It is instantiated by the
	/// <em>Attribute.readAttribute()</em> method. A <em>Code</em>
	/// attribute contains informations about operand stack, local
	/// variables, byte code and the exceptions handled within this
	/// method.
	/// This attribute has attributes itself, namely <em>LineNumberTable</em> which
	/// is used for debugging purposes and <em>LocalVariableTable</em> which
	/// contains information about the local variables.
	/// </remarks>
	/// <seealso cref="Attribute"/>
	/// <seealso cref="CodeException"/>
	/// <seealso cref="LineNumberTable"/>
	/// <seealso cref="LocalVariableTable"/>
	public sealed class Code : NBCEL.classfile.Attribute
	{
		private int max_stack;

		private int max_locals;

		private byte[] code;

		private NBCEL.classfile.CodeException[] exception_table;

		private NBCEL.classfile.Attribute[] attributes;

		/// <summary>Initialize from another object.</summary>
		/// <remarks>
		/// Initialize from another object. Note that both objects use the same
		/// references (shallow copy). Use copy() for a physical copy.
		/// </remarks>
		public Code(NBCEL.classfile.Code c)
			: this(c.GetNameIndex(), c.GetLength(), c.GetMaxStack(), c.GetMaxLocals(), c.GetCode
				(), c.GetExceptionTable(), c.GetAttributes(), c.GetConstantPool())
		{
		}

		/// <param name="name_index">Index pointing to the name <em>Code</em></param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="file">Input stream</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <exception cref="System.IO.IOException"/>
		internal Code(int name_index, int length, java.io.DataInput file, NBCEL.classfile.ConstantPool
			 constant_pool)
			: this(name_index, length, file.ReadUnsignedShort(), file.ReadUnsignedShort(), (byte
				[])null, (NBCEL.classfile.CodeException[])null, (NBCEL.classfile.Attribute[])null
				, constant_pool)
		{
			// Maximum size of stack used by this method  // TODO this could be made final (setter is not used)
			// Number of local variables  // TODO this could be made final (setter is not used)
			// Actual byte code
			// Table of handled exceptions
			// or LocalVariable
			// Initialize with some default values which will be overwritten later
			int code_length = file.ReadInt();
			code = new byte[code_length];
			// Read byte code
			file.ReadFully(code);
			/* Read exception table that contains all regions where an exception
			* handler is active, i.e., a try { ... } catch() block.
			*/
			int exception_table_length = file.ReadUnsignedShort();
			exception_table = new NBCEL.classfile.CodeException[exception_table_length];
			for (int i = 0; i < exception_table_length; i++)
			{
				exception_table[i] = new NBCEL.classfile.CodeException(file);
			}
			/* Read all attributes, currently `LineNumberTable' and
			* `LocalVariableTable'
			*/
			int attributes_count = file.ReadUnsignedShort();
			attributes = new NBCEL.classfile.Attribute[attributes_count];
			for (int i = 0; i < attributes_count; i++)
			{
				attributes[i] = NBCEL.classfile.Attribute.ReadAttribute(file, constant_pool);
			}
			/* Adjust length, because of setAttributes in this(), s.b.  length
			* is incorrect, because it didn't take the internal attributes
			* into account yet! Very subtle bug, fixed in 3.1.1.
			*/
			base.SetLength(length);
		}

		/// <param name="name_index">Index pointing to the name <em>Code</em></param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="max_stack">Maximum size of stack</param>
		/// <param name="max_locals">Number of local variables</param>
		/// <param name="code">Actual byte code</param>
		/// <param name="exception_table">Table of handled exceptions</param>
		/// <param name="attributes">Attributes of code: LineNumber or LocalVariable</param>
		/// <param name="constant_pool">Array of constants</param>
		public Code(int name_index, int length, int max_stack, int max_locals, byte[] code
			, NBCEL.classfile.CodeException[] exception_table, NBCEL.classfile.Attribute[] attributes
			, NBCEL.classfile.ConstantPool constant_pool)
			: base(NBCEL.Const.ATTR_CODE, name_index, length, constant_pool)
		{
			this.max_stack = max_stack;
			this.max_locals = max_locals;
			this.code = code != null ? code : new byte[0];
			this.exception_table = exception_table != null ? exception_table : new NBCEL.classfile.CodeException
				[0];
			this.attributes = attributes != null ? attributes : new NBCEL.classfile.Attribute
				[0];
			base.SetLength(CalculateLength());
		}

		// Adjust length
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
			v.VisitCode(this);
		}

		/// <summary>Dump code attribute to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteShort(max_stack);
			file.WriteShort(max_locals);
			file.WriteInt(code.Length);
			file.Write(code, 0, code.Length);
			file.WriteShort(exception_table.Length);
			foreach (NBCEL.classfile.CodeException exception in exception_table)
			{
				exception.Dump(file);
			}
			file.WriteShort(attributes.Length);
			foreach (NBCEL.classfile.Attribute attribute in attributes)
			{
				attribute.Dump(file);
			}
		}

		/// <returns>Collection of code attributes.</returns>
		/// <seealso cref="Attribute"/>
		public NBCEL.classfile.Attribute[] GetAttributes()
		{
			return attributes;
		}

		/// <returns>LineNumberTable of Code, if it has one</returns>
		public NBCEL.classfile.LineNumberTable GetLineNumberTable()
		{
			foreach (NBCEL.classfile.Attribute attribute in attributes)
			{
				if (attribute is NBCEL.classfile.LineNumberTable)
				{
					return (NBCEL.classfile.LineNumberTable)attribute;
				}
			}
			return null;
		}

		/// <returns>LocalVariableTable of Code, if it has one</returns>
		public NBCEL.classfile.LocalVariableTable GetLocalVariableTable()
		{
			foreach (NBCEL.classfile.Attribute attribute in attributes)
			{
				if (attribute is NBCEL.classfile.LocalVariableTable)
				{
					return (NBCEL.classfile.LocalVariableTable)attribute;
				}
			}
			return null;
		}

		/// <returns>Actual byte code of the method.</returns>
		public byte[] GetCode()
		{
			return code;
		}

		/// <returns>Table of handled exceptions.</returns>
		/// <seealso cref="CodeException"/>
		public NBCEL.classfile.CodeException[] GetExceptionTable()
		{
			return exception_table;
		}

		/// <returns>Number of local variables.</returns>
		public int GetMaxLocals()
		{
			return max_locals;
		}

		/// <returns>Maximum size of stack used by this method.</returns>
		public int GetMaxStack()
		{
			return max_stack;
		}

		/// <returns>
		/// the internal length of this code attribute (minus the first 6 bytes)
		/// and excluding all its attributes
		/// </returns>
		private int GetInternalLength()
		{
			return 2 + 2 + 4 + code.Length + 2 + 8 * (exception_table == null ? 0 : exception_table
				.Length) + 2;
		}

		/*max_stack*/
		/*max_locals*/
		/*code length*/
		/*byte-code*/
		/*exception-table length*/
		/* exception table */
		/* attributes count */
		/// <returns>
		/// the full size of this code attribute, minus its first 6 bytes,
		/// including the size of all its contained attributes
		/// </returns>
		private int CalculateLength()
		{
			int len = 0;
			if (attributes != null)
			{
				foreach (NBCEL.classfile.Attribute attribute in attributes)
				{
					len += attribute.GetLength() + 6;
				}
			}
			/*attribute header size*/
			return len + GetInternalLength();
		}

		/// <param name="attributes">the attributes to set for this Code</param>
		public void SetAttributes(NBCEL.classfile.Attribute[] attributes)
		{
			this.attributes = attributes != null ? attributes : new NBCEL.classfile.Attribute
				[0];
			base.SetLength(CalculateLength());
		}

		// Adjust length
		/// <param name="code">byte code</param>
		public void SetCode(byte[] code)
		{
			this.code = code != null ? code : new byte[0];
			base.SetLength(CalculateLength());
		}

		// Adjust length
		/// <param name="exception_table">exception table</param>
		public void SetExceptionTable(NBCEL.classfile.CodeException[] exception_table)
		{
			this.exception_table = exception_table != null ? exception_table : new NBCEL.classfile.CodeException
				[0];
			base.SetLength(CalculateLength());
		}

		// Adjust length
		/// <param name="max_locals">maximum number of local variables</param>
		public void SetMaxLocals(int max_locals)
		{
			this.max_locals = max_locals;
		}

		/// <param name="max_stack">maximum stack size</param>
		public void SetMaxStack(int max_stack)
		{
			this.max_stack = max_stack;
		}

		/// <returns>String representation of code chunk.</returns>
		public string ToString(bool verbose)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder(100);
			// CHECKSTYLE IGNORE MagicNumber
			buf.Append("Code(max_stack = ").Append(max_stack).Append(", max_locals = ").Append
				(max_locals).Append(", code_length = ").Append(code.Length).Append(")\n").Append
				(NBCEL.classfile.Utility.CodeToString(code, base.GetConstantPool(), 0, -1, verbose
				));
			if (exception_table.Length > 0)
			{
				buf.Append("\nException handler(s) = \n").Append("From\tTo\tHandler\tType\n");
				foreach (NBCEL.classfile.CodeException exception in exception_table)
				{
					buf.Append(exception.ToString(base.GetConstantPool(), verbose)).Append("\n");
				}
			}
			if (attributes.Length > 0)
			{
				buf.Append("\nAttribute(s) = ");
				foreach (NBCEL.classfile.Attribute attribute in attributes)
				{
					buf.Append("\n").Append(attribute);
				}
			}
			return buf.ToString();
		}

		/// <returns>String representation of code chunk.</returns>
		public override string ToString()
		{
			return ToString(true);
		}

		/// <returns>deep copy of this attribute</returns>
		/// <param name="_constant_pool">the constant pool to duplicate</param>
		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool _constant_pool
			)
		{
			NBCEL.classfile.Code c = (NBCEL.classfile.Code)Clone();
			if (code != null)
			{
				c.code = new byte[code.Length];
				System.Array.Copy(code, 0, c.code, 0, code.Length);
			}
			c.SetConstantPool(_constant_pool);
			c.exception_table = new NBCEL.classfile.CodeException[exception_table.Length];
			for (int i = 0; i < exception_table.Length; i++)
			{
				c.exception_table[i] = exception_table[i].Copy();
			}
			c.attributes = new NBCEL.classfile.Attribute[attributes.Length];
			for (int i = 0; i < attributes.Length; i++)
			{
				c.attributes[i] = attributes[i].Copy(_constant_pool);
			}
			return c;
		}
	}
}
