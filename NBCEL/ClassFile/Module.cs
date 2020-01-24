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

using System.Text;
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>
	///     This class is derived from <em>Attribute</em> and represents the list of modules required, exported, opened or
	///     provided by a module.
	/// </summary>
	/// <remarks>
	///     This class is derived from <em>Attribute</em> and represents the list of modules required, exported, opened or
	///     provided by a module.
	///     There may be at most one Module attribute in a ClassFile structure.
	/// </remarks>
	/// <seealso cref="Attribute" />
	/// <since>6.4.0</since>
	public sealed class Module : Attribute
    {
        private readonly int module_flags;
        private readonly int module_name_index;

        private readonly int module_version_index;

        private readonly int uses_count;

        private readonly int[] uses_index;

        private ModuleExports[] exports_table;

        private ModuleOpens[] opens_table;

        private ModuleProvides[] provides_table;

        private ModuleRequires[] requires_table;

        /// <summary>Construct object from input stream.</summary>
        /// <param name="name_index">Index in constant pool</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal Module(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : base(Const.ATTR_MODULE, name_index, length, constant_pool)
        {
            module_name_index = input.ReadUnsignedShort();
            module_flags = input.ReadUnsignedShort();
            module_version_index = input.ReadUnsignedShort();
            var requires_count = input.ReadUnsignedShort();
            requires_table = new ModuleRequires[requires_count];
            for (var i = 0; i < requires_count; i++) requires_table[i] = new ModuleRequires(input);
            var exports_count = input.ReadUnsignedShort();
            exports_table = new ModuleExports[exports_count];
            for (var i = 0; i < exports_count; i++) exports_table[i] = new ModuleExports(input);
            var opens_count = input.ReadUnsignedShort();
            opens_table = new ModuleOpens[opens_count];
            for (var i = 0; i < opens_count; i++) opens_table[i] = new ModuleOpens(input);
            uses_count = input.ReadUnsignedShort();
            uses_index = new int[uses_count];
            for (var i = 0; i < uses_count; i++) uses_index[i] = input.ReadUnsignedShort();
            var provides_count = input.ReadUnsignedShort();
            provides_table = new ModuleProvides[provides_count];
            for (var i = 0; i < provides_count; i++) provides_table[i] = new ModuleProvides(input);
        }

        /// <summary>
        ///     Called by objects that are traversing the nodes of the tree implicitely
        ///     defined by the contents of a Java class.
        /// </summary>
        /// <remarks>
        ///     Called by objects that are traversing the nodes of the tree implicitely
        ///     defined by the contents of a Java class. I.e., the hierarchy of methods,
        ///     fields, attributes, etc. spawns a tree of objects.
        /// </remarks>
        /// <param name="v">Visitor object</param>
        public override void Accept(Visitor v)
        {
            v.VisitModule(this);
        }

        // TODO add more getters and setters?
        /// <returns>table of required modules</returns>
        /// <seealso cref="ModuleRequires" />
        public ModuleRequires[] GetRequiresTable()
        {
            return requires_table;
        }

        /// <returns>table of exported interfaces</returns>
        /// <seealso cref="ModuleExports" />
        public ModuleExports[] GetExportsTable()
        {
            return exports_table;
        }

        /// <returns>table of provided interfaces</returns>
        /// <seealso cref="ModuleOpens" />
        public ModuleOpens[] GetOpensTable()
        {
            return opens_table;
        }

        /// <returns>table of provided interfaces</returns>
        /// <seealso cref="ModuleProvides" />
        public ModuleProvides[] GetProvidesTable()
        {
            return provides_table;
        }

        /// <summary>Dump Module attribute to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            base.Dump(file);
            file.WriteShort(module_name_index);
            file.WriteShort(module_flags);
            file.WriteShort(module_version_index);
            file.WriteShort(requires_table.Length);
            foreach (var entry in requires_table) entry.Dump(file);
            file.WriteShort(exports_table.Length);
            foreach (var entry in exports_table) entry.Dump(file);
            file.WriteShort(opens_table.Length);
            foreach (var entry in opens_table) entry.Dump(file);
            file.WriteShort(uses_index.Length);
            foreach (var entry in uses_index) file.WriteShort(entry);
            file.WriteShort(provides_table.Length);
            foreach (var entry in provides_table) entry.Dump(file);
        }

        /// <returns>String representation, i.e., a list of packages.</returns>
        public override string ToString()
        {
            var cp = GetConstantPool();
            var buf = new StringBuilder();
            buf.Append("Module:\n");
            buf.Append("  name:    ").Append(cp.GetConstantString(module_name_index, Const
                .CONSTANT_Module).Replace('/', '.')).Append("\n");
            buf.Append("  flags:   ").Append(string.Format("%04x", module_flags)).Append("\n"
            );
            var version = module_version_index == 0
                ? "0"
                : cp.GetConstantString(module_version_index
                    , Const.CONSTANT_Utf8);
            buf.Append("  version: ").Append(version).Append("\n");
            buf.Append("  requires(").Append(requires_table.Length).Append("):\n");
            foreach (var module in requires_table) buf.Append("    ").Append(module.ToString(cp)).Append("\n");
            buf.Append("  exports(").Append(exports_table.Length).Append("):\n");
            foreach (var module in exports_table) buf.Append("    ").Append(module.ToString(cp)).Append("\n");
            buf.Append("  opens(").Append(opens_table.Length).Append("):\n");
            foreach (var module in opens_table) buf.Append("    ").Append(module.ToString(cp)).Append("\n");
            buf.Append("  uses(").Append(uses_index.Length).Append("):\n");
            foreach (var index in uses_index)
            {
                var class_name = cp.GetConstantString(index, Const.CONSTANT_Class);
                buf.Append("    ").Append(Utility.CompactClassName(class_name, false
                )).Append("\n");
            }

            buf.Append("  provides(").Append(provides_table.Length).Append("):\n");
            foreach (var module in provides_table) buf.Append("    ").Append(module.ToString(cp)).Append("\n");
            return buf.Substring(0, buf.Length - 1);
        }

        // remove the last newline
        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            var c = (Module) Clone();
            c.requires_table = new ModuleRequires[requires_table.Length];
            for (var i = 0; i < requires_table.Length; i++) c.requires_table[i] = requires_table[i].Copy();
            c.exports_table = new ModuleExports[exports_table.Length];
            for (var i = 0; i < exports_table.Length; i++) c.exports_table[i] = exports_table[i].Copy();
            c.opens_table = new ModuleOpens[opens_table.Length];
            for (var i = 0; i < opens_table.Length; i++) c.opens_table[i] = opens_table[i].Copy();
            c.provides_table = new ModuleProvides[provides_table.Length];
            for (var i = 0; i < provides_table.Length; i++) c.provides_table[i] = provides_table[i].Copy();
            c.SetConstantPool(_constant_pool);
            return c;
        }
    }
}