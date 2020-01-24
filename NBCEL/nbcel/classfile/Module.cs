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
	/// <summary>This class is derived from <em>Attribute</em> and represents the list of modules required, exported, opened or provided by a module.
	/// 	</summary>
	/// <remarks>
	/// This class is derived from <em>Attribute</em> and represents the list of modules required, exported, opened or provided by a module.
	/// There may be at most one Module attribute in a ClassFile structure.
	/// </remarks>
	/// <seealso cref="Attribute"/>
	/// <since>6.4.0</since>
	public sealed class Module : NBCEL.classfile.Attribute
	{
		private readonly int module_name_index;

		private readonly int module_flags;

		private readonly int module_version_index;

		private NBCEL.classfile.ModuleRequires[] requires_table;

		private NBCEL.classfile.ModuleExports[] exports_table;

		private NBCEL.classfile.ModuleOpens[] opens_table;

		private readonly int uses_count;

		private readonly int[] uses_index;

		private NBCEL.classfile.ModuleProvides[] provides_table;

		/// <summary>Construct object from input stream.</summary>
		/// <param name="name_index">Index in constant pool</param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="input">Input stream</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <exception cref="System.IO.IOException"/>
		internal Module(int name_index, int length, java.io.DataInput input, NBCEL.classfile.ConstantPool
			 constant_pool)
			: base(NBCEL.Const.ATTR_MODULE, name_index, length, constant_pool)
		{
			module_name_index = input.ReadUnsignedShort();
			module_flags = input.ReadUnsignedShort();
			module_version_index = input.ReadUnsignedShort();
			int requires_count = input.ReadUnsignedShort();
			requires_table = new NBCEL.classfile.ModuleRequires[requires_count];
			for (int i = 0; i < requires_count; i++)
			{
				requires_table[i] = new NBCEL.classfile.ModuleRequires(input);
			}
			int exports_count = input.ReadUnsignedShort();
			exports_table = new NBCEL.classfile.ModuleExports[exports_count];
			for (int i = 0; i < exports_count; i++)
			{
				exports_table[i] = new NBCEL.classfile.ModuleExports(input);
			}
			int opens_count = input.ReadUnsignedShort();
			opens_table = new NBCEL.classfile.ModuleOpens[opens_count];
			for (int i = 0; i < opens_count; i++)
			{
				opens_table[i] = new NBCEL.classfile.ModuleOpens(input);
			}
			uses_count = input.ReadUnsignedShort();
			uses_index = new int[uses_count];
			for (int i = 0; i < uses_count; i++)
			{
				uses_index[i] = input.ReadUnsignedShort();
			}
			int provides_count = input.ReadUnsignedShort();
			provides_table = new NBCEL.classfile.ModuleProvides[provides_count];
			for (int i = 0; i < provides_count; i++)
			{
				provides_table[i] = new NBCEL.classfile.ModuleProvides(input);
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
			v.VisitModule(this);
		}

		// TODO add more getters and setters?
		/// <returns>table of required modules</returns>
		/// <seealso cref="ModuleRequires"/>
		public NBCEL.classfile.ModuleRequires[] GetRequiresTable()
		{
			return requires_table;
		}

		/// <returns>table of exported interfaces</returns>
		/// <seealso cref="ModuleExports"/>
		public NBCEL.classfile.ModuleExports[] GetExportsTable()
		{
			return exports_table;
		}

		/// <returns>table of provided interfaces</returns>
		/// <seealso cref="ModuleOpens"/>
		public NBCEL.classfile.ModuleOpens[] GetOpensTable()
		{
			return opens_table;
		}

		/// <returns>table of provided interfaces</returns>
		/// <seealso cref="ModuleProvides"/>
		public NBCEL.classfile.ModuleProvides[] GetProvidesTable()
		{
			return provides_table;
		}

		/// <summary>Dump Module attribute to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteShort(module_name_index);
			file.WriteShort(module_flags);
			file.WriteShort(module_version_index);
			file.WriteShort(requires_table.Length);
			foreach (NBCEL.classfile.ModuleRequires entry in requires_table)
			{
				entry.Dump(file);
			}
			file.WriteShort(exports_table.Length);
			foreach (NBCEL.classfile.ModuleExports entry in exports_table)
			{
				entry.Dump(file);
			}
			file.WriteShort(opens_table.Length);
			foreach (NBCEL.classfile.ModuleOpens entry in opens_table)
			{
				entry.Dump(file);
			}
			file.WriteShort(uses_index.Length);
			foreach (int entry in uses_index)
			{
				file.WriteShort(entry);
			}
			file.WriteShort(provides_table.Length);
			foreach (NBCEL.classfile.ModuleProvides entry in provides_table)
			{
				entry.Dump(file);
			}
		}

		/// <returns>String representation, i.e., a list of packages.</returns>
		public override string ToString()
		{
			NBCEL.classfile.ConstantPool cp = base.GetConstantPool();
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			buf.Append("Module:\n");
			buf.Append("  name:    ").Append(cp.GetConstantString(module_name_index, NBCEL.Const
				.CONSTANT_Module).Replace('/', '.')).Append("\n");
			buf.Append("  flags:   ").Append(string.Format("%04x", module_flags)).Append("\n"
				);
			string version = module_version_index == 0 ? "0" : cp.GetConstantString(module_version_index
				, NBCEL.Const.CONSTANT_Utf8);
			buf.Append("  version: ").Append(version).Append("\n");
			buf.Append("  requires(").Append(requires_table.Length).Append("):\n");
			foreach (NBCEL.classfile.ModuleRequires module in requires_table)
			{
				buf.Append("    ").Append(module.ToString(cp)).Append("\n");
			}
			buf.Append("  exports(").Append(exports_table.Length).Append("):\n");
			foreach (NBCEL.classfile.ModuleExports module in exports_table)
			{
				buf.Append("    ").Append(module.ToString(cp)).Append("\n");
			}
			buf.Append("  opens(").Append(opens_table.Length).Append("):\n");
			foreach (NBCEL.classfile.ModuleOpens module in opens_table)
			{
				buf.Append("    ").Append(module.ToString(cp)).Append("\n");
			}
			buf.Append("  uses(").Append(uses_index.Length).Append("):\n");
			foreach (int index in uses_index)
			{
				string class_name = cp.GetConstantString(index, NBCEL.Const.CONSTANT_Class);
				buf.Append("    ").Append(NBCEL.classfile.Utility.CompactClassName(class_name, false
					)).Append("\n");
			}
			buf.Append("  provides(").Append(provides_table.Length).Append("):\n");
			foreach (NBCEL.classfile.ModuleProvides module in provides_table)
			{
				buf.Append("    ").Append(module.ToString(cp)).Append("\n");
			}
			return Runtime.Substring(buf,0, buf.Length - 1);
		}

		// remove the last newline
		/// <returns>deep copy of this attribute</returns>
		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool _constant_pool
			)
		{
			NBCEL.classfile.Module c = (NBCEL.classfile.Module)Clone();
			c.requires_table = new NBCEL.classfile.ModuleRequires[requires_table.Length];
			for (int i = 0; i < requires_table.Length; i++)
			{
				c.requires_table[i] = requires_table[i].Copy();
			}
			c.exports_table = new NBCEL.classfile.ModuleExports[exports_table.Length];
			for (int i = 0; i < exports_table.Length; i++)
			{
				c.exports_table[i] = exports_table[i].Copy();
			}
			c.opens_table = new NBCEL.classfile.ModuleOpens[opens_table.Length];
			for (int i = 0; i < opens_table.Length; i++)
			{
				c.opens_table[i] = opens_table[i].Copy();
			}
			c.provides_table = new NBCEL.classfile.ModuleProvides[provides_table.Length];
			for (int i = 0; i < provides_table.Length; i++)
			{
				c.provides_table[i] = provides_table[i].Copy();
			}
			c.SetConstantPool(_constant_pool);
			return c;
		}
	}
}
