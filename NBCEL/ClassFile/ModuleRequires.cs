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
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>
	///     This class represents an entry in the requires table of the Module attribute.
	/// </summary>
	/// <remarks>
	///     This class represents an entry in the requires table of the Module attribute.
	///     Each entry describes a module on which the parent module depends.
	/// </remarks>
	/// <seealso cref="Module" />
	/// <since>6.4.0</since>
	public sealed class ModuleRequires : ICloneable, Node
    {
        private readonly int requires_flags;
        private readonly int requires_index;

        private readonly int requires_version_index;

        /// <summary>Construct object from file stream.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException">
        ///     if an I/O Exception occurs in readUnsignedShort
        /// </exception>
        internal ModuleRequires(DataInput file)
        {
            // points to CONSTANT_Module_info
            // either 0 or points to CONSTANT_Utf8_info
            requires_index = file.ReadUnsignedShort();
            requires_flags = file.ReadUnsignedShort();
            requires_version_index = file.ReadUnsignedShort();
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
            v.VisitModuleRequires(this);
        }

        // TODO add more getters and setters?
        /// <summary>Dump table entry to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException">if an I/O Exception occurs in writeShort</exception>
        public void Dump(DataOutputStream file)
        {
            file.WriteShort(requires_index);
            file.WriteShort(requires_flags);
            file.WriteShort(requires_version_index);
        }

        /// <returns>String representation</returns>
        public override string ToString()
        {
            return "requires(" + requires_index + ", " + string.Format("%04x", requires_flags
                   ) + ", " + requires_version_index + ")";
        }

        /// <returns>Resolved string representation</returns>
        public string ToString(ConstantPool constant_pool)
        {
            var buf = new StringBuilder();
            var module_name = constant_pool.ConstantToString(requires_index, Const.CONSTANT_Module
            );
            buf.Append(Utility.CompactClassName(module_name, false));
            buf.Append(", ").Append(string.Format("%04x", requires_flags));
            var version = requires_version_index == 0
                ? "0"
                : constant_pool.GetConstantString
                    (requires_version_index, Const.CONSTANT_Utf8);
            buf.Append(", ").Append(version);
            return buf.ToString();
        }

        /// <returns>deep copy of this object</returns>
        public ModuleRequires Copy()
        {
            return (ModuleRequires) MemberwiseClone();
            // TODO should this throw?
            return null;
        }
    }
}