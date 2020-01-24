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
	///     This class represents an entry in the provides table of the Module attribute.
	/// </summary>
	/// <remarks>
	///     This class represents an entry in the provides table of the Module attribute.
	///     Each entry describes a service implementation that the parent module provides.
	/// </remarks>
	/// <seealso cref="Module" />
	/// <since>6.4.0</since>
	public sealed class ModuleProvides : ICloneable, Node
    {
        private readonly int provides_index;

        private readonly int provides_with_count;

        private readonly int[] provides_with_index;

        /// <summary>Construct object from file stream.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException">
        ///     if an I/O Exception occurs in readUnsignedShort
        /// </exception>
        internal ModuleProvides(DataInput file)
        {
            // points to CONSTANT_Class_info
            // points to CONSTANT_Class_info
            provides_index = file.ReadUnsignedShort();
            provides_with_count = file.ReadUnsignedShort();
            provides_with_index = new int[provides_with_count];
            for (var i = 0; i < provides_with_count; i++) provides_with_index[i] = file.ReadUnsignedShort();
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
            v.VisitModuleProvides(this);
        }

        // TODO add more getters and setters?
        /// <summary>Dump table entry to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException">if an I/O Exception occurs in writeShort</exception>
        public void Dump(DataOutputStream file)
        {
            file.WriteShort(provides_index);
            file.WriteShort(provides_with_count);
            foreach (var entry in provides_with_index) file.WriteShort(entry);
        }

        /// <returns>String representation</returns>
        public override string ToString()
        {
            return "provides(" + provides_index + ", " + provides_with_count + ", ...)";
        }

        /// <returns>Resolved string representation</returns>
        public string ToString(ConstantPool constant_pool)
        {
            var buf = new StringBuilder();
            var interface_name = constant_pool.ConstantToString(provides_index, Const
                .CONSTANT_Class);
            buf.Append(Utility.CompactClassName(interface_name, false));
            buf.Append(", with(").Append(provides_with_count).Append("):\n");
            foreach (var index in provides_with_index)
            {
                var class_name = constant_pool.GetConstantString(index, Const.CONSTANT_Class
                );
                buf.Append("      ").Append(Utility.CompactClassName(class_name,
                    false)).Append("\n");
            }

            return buf.Substring(0, buf.Length - 1);
        }

        // remove the last newline
        /// <returns>deep copy of this object</returns>
        public ModuleProvides Copy()
        {
            return (ModuleProvides) MemberwiseClone();
            // TODO should this throw?
            return null;
        }
    }
}