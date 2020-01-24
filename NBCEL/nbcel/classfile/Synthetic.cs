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

using System;
using System.Text;
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	///     This class is derived from <em>Attribute</em> and declares this class as
	///     `synthetic', i.e., it needs special handling.
	/// </summary>
	/// <remarks>
	///     This class is derived from <em>Attribute</em> and declares this class as
	///     `synthetic', i.e., it needs special handling.  The JVM specification
	///     states "A class member that does not appear in the source code must be
	///     marked using a Synthetic attribute."  It may appear in the ClassFile
	///     attribute table, a field_info table or a method_info table.  This class
	///     is intended to be instantiated from the
	///     <em>Attribute.readAttribute()</em> method.
	/// </remarks>
	/// <seealso cref="Attribute" />
	public sealed class Synthetic : Attribute
    {
        private byte[] bytes;

        /// <summary>Initialize from another object.</summary>
        /// <remarks>
        ///     Initialize from another object. Note that both objects use the same
        ///     references (shallow copy). Use copy() for a physical copy.
        /// </remarks>
        public Synthetic(Synthetic c)
            : this(c.GetNameIndex(), c.GetLength(), c.GetBytes(), c.GetConstantPool())
        {
        }

        /// <param name="name_index">
        ///     Index in constant pool to CONSTANT_Utf8, which
        ///     should represent the string "Synthetic".
        /// </param>
        /// <param name="length">Content length in bytes - should be zero.</param>
        /// <param name="bytes">Attribute contents</param>
        /// <param name="constant_pool">
        ///     The constant pool this attribute is associated
        ///     with.
        /// </param>
        public Synthetic(int name_index, int length, byte[] bytes, ConstantPool
            constant_pool)
            : base(Const.ATTR_SYNTHETIC, name_index, length, constant_pool)
        {
            this.bytes = bytes;
        }

        /// <summary>Construct object from input stream.</summary>
        /// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal Synthetic(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : this(name_index, length, (byte[]) null, constant_pool)
        {
            if (length > 0)
            {
                bytes = new byte[length];
                input.ReadFully(bytes);
                Println("Synthetic attribute with length > 0");
            }
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
            v.VisitSynthetic(this);
        }

        /// <summary>Dump source file attribute to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            base.Dump(file);
            if (GetLength() > 0) file.Write(bytes, 0, GetLength());
        }

        /// <returns>data bytes.</returns>
        public byte[] GetBytes()
        {
            return bytes;
        }

        /// <param name="bytes" />
        public void SetBytes(byte[] bytes)
        {
            this.bytes = bytes;
        }

        /// <returns>String representation.</returns>
        public override string ToString()
        {
            var buf = new StringBuilder("Synthetic");
            if (GetLength() > 0) buf.Append(" ").Append(Utility.ToHexString(bytes));
            return buf.ToString();
        }

        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            var c = (Synthetic) Clone();
            if (bytes != null)
            {
                c.bytes = new byte[bytes.Length];
                Array.Copy(bytes, 0, c.bytes, 0, bytes.Length);
            }

            c.SetConstantPool(_constant_pool);
            return c;
        }
    }
}