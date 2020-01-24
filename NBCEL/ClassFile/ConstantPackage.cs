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

using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>
	///     This class is derived from the abstract
	///     <see cref="Constant" />
	///     and represents a reference to a package.
	///     <p>Note: Early access Java 9 support- currently subject to change</p>
	/// </summary>
	/// <seealso cref="Constant" />
	/// <since>6.1</since>
	public sealed class ConstantPackage : Constant, ConstantObject
    {
        private int name_index;

        /// <summary>Initialize from another object.</summary>
        public ConstantPackage(ConstantPackage c)
            : this(c.GetNameIndex())
        {
        }

        /// <summary>Initialize instance from file data.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal ConstantPackage(DataInput file)
            : this(file.ReadUnsignedShort())
        {
        }

        /// <param name="name_index">
        ///     Name index in constant pool.  Should refer to a
        ///     ConstantUtf8.
        /// </param>
        public ConstantPackage(int name_index)
            : base(Const.CONSTANT_Package)
        {
            this.name_index = name_index;
        }

        /// <returns>String object</returns>
        public object GetConstantValue(ConstantPool cp)
        {
            var c = cp.GetConstant(name_index, Const.CONSTANT_Utf8
            );
            return ((ConstantUtf8) c).GetBytes();
        }

        /// <summary>
        ///     Called by objects that are traversing the nodes of the tree implicitly
        ///     defined by the contents of a Java class.
        /// </summary>
        /// <remarks>
        ///     Called by objects that are traversing the nodes of the tree implicitly
        ///     defined by the contents of a Java class. I.e., the hierarchy of methods,
        ///     fields, attributes, etc. spawns a tree of objects.
        /// </remarks>
        /// <param name="v">Visitor object</param>
        public override void Accept(Visitor v)
        {
            v.VisitConstantPackage(this);
        }

        /// <summary>Dump constant package to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            file.WriteByte(GetTag());
            file.WriteShort(name_index);
        }

        /// <returns>Name index in constant pool of package name.</returns>
        public int GetNameIndex()
        {
            return name_index;
        }

        /// <param name="name_index">
        ///     the name index in the constant pool of this Constant Package
        /// </param>
        public void SetNameIndex(int name_index)
        {
            this.name_index = name_index;
        }

        /// <returns>dereferenced string</returns>
        public string GetBytes(ConstantPool cp)
        {
            return (string) GetConstantValue(cp);
        }

        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return base.ToString() + "(name_index = " + name_index + ")";
        }
    }
}