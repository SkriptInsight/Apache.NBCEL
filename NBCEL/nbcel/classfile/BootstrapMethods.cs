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
	/// <summary>This class represents a BootstrapMethods attribute.</summary>
	/// <seealso><a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.23">
	/// * The class File Format : The BootstrapMethods Attribute</a></seealso>
	/// <since>6.0</since>
	public class BootstrapMethods : NBCEL.classfile.Attribute
	{
		private NBCEL.classfile.BootstrapMethod[] bootstrap_methods;

		/// <summary>Initialize from another object.</summary>
		/// <remarks>
		/// Initialize from another object. Note that both objects use the same
		/// references (shallow copy). Use clone() for a physical copy.
		/// </remarks>
		public BootstrapMethods(NBCEL.classfile.BootstrapMethods c)
			: this(c.GetNameIndex(), c.GetLength(), c.GetBootstrapMethods(), c.GetConstantPool
				())
		{
		}

		/// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="bootstrap_methods">array of bootstrap methods</param>
		/// <param name="constant_pool">Array of constants</param>
		public BootstrapMethods(int name_index, int length, NBCEL.classfile.BootstrapMethod
			[] bootstrap_methods, NBCEL.classfile.ConstantPool constant_pool)
			: base(NBCEL.Const.ATTR_BOOTSTRAP_METHODS, name_index, length, constant_pool)
		{
			// TODO this could be made final (setter is not used)
			this.bootstrap_methods = bootstrap_methods;
		}

		/// <summary>Construct object from Input stream.</summary>
		/// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="input">Input stream</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <exception cref="System.IO.IOException"/>
		internal BootstrapMethods(int name_index, int length, java.io.DataInput input, NBCEL.classfile.ConstantPool
			 constant_pool)
			: this(name_index, length, (NBCEL.classfile.BootstrapMethod[])null, constant_pool
				)
		{
			int num_bootstrap_methods = input.ReadUnsignedShort();
			bootstrap_methods = new NBCEL.classfile.BootstrapMethod[num_bootstrap_methods];
			for (int i = 0; i < num_bootstrap_methods; i++)
			{
				bootstrap_methods[i] = new NBCEL.classfile.BootstrapMethod(input);
			}
		}

		/// <returns>array of bootstrap method "records"</returns>
		public NBCEL.classfile.BootstrapMethod[] GetBootstrapMethods()
		{
			return bootstrap_methods;
		}

		/// <param name="bootstrap_methods">the array of bootstrap methods</param>
		public void SetBootstrapMethods(NBCEL.classfile.BootstrapMethod[] bootstrap_methods
			)
		{
			this.bootstrap_methods = bootstrap_methods;
		}

		/// <param name="v">Visitor object</param>
		public override void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitBootstrapMethods(this);
		}

		/// <returns>deep copy of this attribute</returns>
		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool _constant_pool
			)
		{
			NBCEL.classfile.BootstrapMethods c = (NBCEL.classfile.BootstrapMethods)Clone();
			c.bootstrap_methods = new NBCEL.classfile.BootstrapMethod[bootstrap_methods.Length
				];
			for (int i = 0; i < bootstrap_methods.Length; i++)
			{
				c.bootstrap_methods[i] = bootstrap_methods[i].Copy();
			}
			c.SetConstantPool(_constant_pool);
			return c;
		}

		/// <summary>Dump bootstrap methods attribute to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public sealed override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteShort(bootstrap_methods.Length);
			foreach (NBCEL.classfile.BootstrapMethod bootstrap_method in bootstrap_methods)
			{
				bootstrap_method.Dump(file);
			}
		}

		/// <returns>String representation.</returns>
		public sealed override string ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			buf.Append("BootstrapMethods(");
			buf.Append(bootstrap_methods.Length);
			buf.Append("):");
			for (int i = 0; i < bootstrap_methods.Length; i++)
			{
				buf.Append("\n");
				int start = buf.Length;
				buf.Append("  ").Append(i).Append(": ");
				int indent_count = buf.Length - start;
				string[] lines = (bootstrap_methods[i].ToString(base.GetConstantPool())).Split("\\r?\\n"
					);
				buf.Append(lines[0]);
				for (int j = 1; j < lines.Length; j++)
				{
					buf.Append("\n").Append(Sharpen.Runtime.Substring("          ", 0, indent_count))
						.Append(lines[j]);
				}
			}
			return buf.ToString();
		}
	}
}
