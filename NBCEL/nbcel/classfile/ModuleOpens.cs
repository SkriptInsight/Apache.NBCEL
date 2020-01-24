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

using System;
using System.Text;
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	///     This class represents an entry in the opens table of the Module attribute.
	/// </summary>
	/// <remarks>
	///     This class represents an entry in the opens table of the Module attribute.
	///     Each entry describes a package which the parent module opens.
	/// </remarks>
	/// <seealso cref="Module" />
	/// <since>6.4.0</since>
	public sealed class ModuleOpens : ICloneable, Node
    {
        private readonly int opens_flags;
        private readonly int opens_index;

        private readonly int opens_to_count;

        private readonly int[] opens_to_index;

        /// <summary>Construct object from file stream.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException">
        ///     if an I/O Exception occurs in readUnsignedShort
        /// </exception>
        internal ModuleOpens(DataInput file)
        {
            // points to CONSTANT_Package_info
            // points to CONSTANT_Module_info
            opens_index = file.ReadUnsignedShort();
            opens_flags = file.ReadUnsignedShort();
            opens_to_count = file.ReadUnsignedShort();
            opens_to_index = new int[opens_to_count];
            for (var i = 0; i < opens_to_count; i++) opens_to_index[i] = file.ReadUnsignedShort();
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
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
        public void Accept(Visitor v)
        {
            v.VisitModuleOpens(this);
        }

        // TODO add more getters and setters?
        /// <summary>Dump table entry to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException">if an I/O Exception occurs in writeShort</exception>
        public void Dump(DataOutputStream file)
        {
            file.WriteShort(opens_index);
            file.WriteShort(opens_flags);
            file.WriteShort(opens_to_count);
            foreach (var entry in opens_to_index) file.WriteShort(entry);
        }

        /// <returns>String representation</returns>
        public override string ToString()
        {
            return "opens(" + opens_index + ", " + opens_flags + ", " + opens_to_count + ", ...)";
        }

        /// <returns>Resolved string representation</returns>
        public string ToString(ConstantPool constant_pool)
        {
            var buf = new StringBuilder();
            var package_name = constant_pool.ConstantToString(opens_index, Const.CONSTANT_Package
            );
            buf.Append(Utility.CompactClassName(package_name, false));
            buf.Append(", ").Append(string.Format("%04x", opens_flags));
            buf.Append(", to(").Append(opens_to_count).Append("):\n");
            foreach (var index in opens_to_index)
            {
                var module_name = constant_pool.GetConstantString(index, Const.CONSTANT_Module
                );
                buf.Append("      ").Append(Utility.CompactClassName(module_name,
                    false)).Append("\n");
            }

            return buf.Substring(0, buf.Length - 1);
        }

        // remove the last newline
        /// <returns>deep copy of this object</returns>
        public ModuleOpens Copy()
        {
            return (ModuleOpens) MemberwiseClone();
            // TODO should this throw?
            return null;
        }
    }
}