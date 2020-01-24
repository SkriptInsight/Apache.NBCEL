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
	///     This class is derived from <em>Attribute</em> and represents the list of packages that are exported or opened by
	///     the Module attribute.
	/// </summary>
	/// <remarks>
	///     This class is derived from <em>Attribute</em> and represents the list of packages that are exported or opened by
	///     the Module attribute.
	///     There may be at most one ModulePackages attribute in a ClassFile structure.
	/// </remarks>
	/// <seealso cref="Attribute" />
	public sealed class ModulePackages : Attribute
    {
        private int[] package_index_table;

        /// <summary>Initialize from another object.</summary>
        /// <remarks>
        ///     Initialize from another object. Note that both objects use the same
        ///     references (shallow copy). Use copy() for a physical copy.
        /// </remarks>
        public ModulePackages(ModulePackages c)
            : this(c.GetNameIndex(), c.GetLength(), c.GetPackageIndexTable(), c.GetConstantPool
                ())
        {
        }

        /// <param name="name_index">Index in constant pool</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="package_index_table">Table of indices in constant pool</param>
        /// <param name="constant_pool">Array of constants</param>
        public ModulePackages(int name_index, int length, int[] package_index_table, ConstantPool
            constant_pool)
            : base(Const.ATTR_MODULE_PACKAGES, name_index, length, constant_pool)
        {
            this.package_index_table = package_index_table != null
                ? package_index_table
                : new
                    int[0];
        }

        /// <summary>Construct object from input stream.</summary>
        /// <param name="name_index">Index in constant pool</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal ModulePackages(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : this(name_index, length, (int[]) null, constant_pool)
        {
            var number_of_packages = input.ReadUnsignedShort();
            package_index_table = new int[number_of_packages];
            for (var i = 0; i < number_of_packages; i++) package_index_table[i] = input.ReadUnsignedShort();
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
            v.VisitModulePackages(this);
        }

        /// <summary>Dump ModulePackages attribute to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            base.Dump(file);
            file.WriteShort(package_index_table.Length);
            foreach (var index in package_index_table) file.WriteShort(index);
        }

        /// <returns>array of indices into constant pool of package names.</returns>
        public int[] GetPackageIndexTable()
        {
            return package_index_table;
        }

        /// <returns>Length of package table.</returns>
        public int GetNumberOfPackages()
        {
            return package_index_table == null ? 0 : package_index_table.Length;
        }

        /// <returns>string array of package names</returns>
        public string[] GetPackageNames()
        {
            var names = new string[package_index_table.Length];
            for (var i = 0; i < package_index_table.Length; i++)
                names[i] = GetConstantPool().GetConstantString(package_index_table[i], Const
                    .CONSTANT_Package).Replace('/', '.');
            return names;
        }

        /// <param name="package_index_table">
        ///     the list of package indexes
        ///     Also redefines number_of_packages according to table length.
        /// </param>
        public void SetPackageIndexTable(int[] package_index_table)
        {
            this.package_index_table = package_index_table != null
                ? package_index_table
                : new
                    int[0];
        }

        /// <returns>String representation, i.e., a list of packages.</returns>
        public override string ToString()
        {
            var buf = new StringBuilder();
            buf.Append("ModulePackages(");
            buf.Append(package_index_table.Length);
            buf.Append("):\n");
            foreach (var index in package_index_table)
            {
                var package_name = GetConstantPool().GetConstantString(index, Const
                    .CONSTANT_Package);
                buf.Append("  ").Append(Utility.CompactClassName(package_name, false
                )).Append("\n");
            }

            return buf.Substring(0, buf.Length - 1);
        }

        // remove the last newline
        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            var c = (ModulePackages) Clone();
            if (package_index_table != null)
            {
                c.package_index_table = new int[package_index_table.Length];
                Array.Copy(package_index_table, 0, c.package_index_table, 0, package_index_table
                    .Length);
            }

            c.SetConstantPool(_constant_pool);
            return c;
        }
    }
}