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
	///     This class is derived from <em>Attribute</em> and records the nest host of the nest
	///     to which the current class or interface claims to belong.
	/// </summary>
	/// <remarks>
	///     This class is derived from <em>Attribute</em> and records the nest host of the nest
	///     to which the current class or interface claims to belong.
	///     There may be at most one NestHost attribute in a ClassFile structure.
	/// </remarks>
	/// <seealso cref="Attribute" />
	public sealed class NestHost : Attribute
    {
        private int host_class_index;

        /// <summary>Initializes from another object.</summary>
        /// <remarks>
        ///     Initializes from another object. Note that both objects use the same
        ///     references (shallow copy). Use copy() for a physical copy.
        /// </remarks>
        public NestHost(NestHost c)
            : this(c.GetNameIndex(), c.GetLength(), c.GetHostClassIndex(), c.GetConstantPool(
            ))
        {
        }

        /// <param name="name_index">Index in constant pool</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="host_class_index">Host class index</param>
        /// <param name="constant_pool">Array of constants</param>
        public NestHost(int name_index, int length, int host_class_index, ConstantPool
            constant_pool)
            : base(Const.ATTR_NEST_MEMBERS, name_index, length, constant_pool)
        {
            this.host_class_index = host_class_index;
        }

        /// <summary>Constructs object from input stream.</summary>
        /// <param name="name_index">Index in constant pool</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal NestHost(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : this(name_index, length, 0, constant_pool)
        {
            host_class_index = input.ReadUnsignedShort();
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
            v.VisitNestHost(this);
        }

        /// <summary>Dumps NestHost attribute to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException">if an I/O error occurs.</exception>
        public override void Dump(DataOutputStream file)
        {
            base.Dump(file);
            file.WriteShort(host_class_index);
        }

        /// <returns>index into constant pool of host class name.</returns>
        public int GetHostClassIndex()
        {
            return host_class_index;
        }

        /// <param name="host_class_index">the host class index</param>
        public void SetHostClassIndex(int host_class_index)
        {
            this.host_class_index = host_class_index;
        }

        /// <returns>String representation</returns>
        public override string ToString()
        {
            var buf = new StringBuilder();
            buf.Append("NestHost: ");
            var class_name = GetConstantPool().GetConstantString(host_class_index, Const
                .CONSTANT_Class);
            buf.Append(Utility.CompactClassName(class_name, false));
            return buf.ToString();
        }

        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            var c = (NestHost) Clone();
            c.SetConstantPool(_constant_pool);
            return c;
        }
    }
}