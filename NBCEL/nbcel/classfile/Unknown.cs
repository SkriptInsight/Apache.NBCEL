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
using System.Collections.Generic;
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	///     This class represents a reference to an unknown (i.e.,
	///     application-specific) attribute of a class.
	/// </summary>
	/// <remarks>
	///     This class represents a reference to an unknown (i.e.,
	///     application-specific) attribute of a class.  It is instantiated from the
	///     <see cref="Attribute.ReadAttribute(java.io.DataInput, ConstantPool)" />
	///     method.
	///     Applications that need to read in application-specific attributes should create an
	///     <see cref="UnknownAttributeReader" />
	///     implementation and attach it via
	///     <see cref="Attribute.AddAttributeReader(string, UnknownAttributeReader)" />
	///     .
	/// </remarks>
	/// <seealso cref="Attribute" />
	/// <seealso cref="UnknownAttributeReader" />
	public sealed class Unknown : Attribute
    {
        private static readonly IDictionary<string, Unknown
        > unknown_attributes = new Dictionary<string, Unknown
        >();

        private readonly string name;
        private byte[] bytes;

        /// <summary>Initialize from another object.</summary>
        /// <remarks>
        ///     Initialize from another object. Note that both objects use the same
        ///     references (shallow copy). Use clone() for a physical copy.
        /// </remarks>
        public Unknown(Unknown c)
            : this(c.GetNameIndex(), c.GetLength(), c.GetBytes(), c.GetConstantPool())
        {
        }

        /// <summary>Create a non-standard attribute.</summary>
        /// <param name="name_index">Index in constant pool</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="bytes">Attribute contents</param>
        /// <param name="constant_pool">Array of constants</param>
        public Unknown(int name_index, int length, byte[] bytes, ConstantPool
            constant_pool)
            : base(Const.ATTR_UNKNOWN, name_index, length, constant_pool)
        {
            this.bytes = bytes;
            name = ((ConstantUtf8) constant_pool.GetConstant(name_index, Const
                .CONSTANT_Utf8)).GetBytes();
            Collections.Put(unknown_attributes, name, this);
        }

        /// <summary>Construct object from input stream.</summary>
        /// <param name="name_index">Index in constant pool</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal Unknown(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : this(name_index, length, (byte[]) null, constant_pool)
        {
            if (length > 0)
            {
                bytes = new byte[length];
                input.ReadFully(bytes);
            }
        }

        /// <returns>array of unknown attributes, but just one for each kind.</returns>
        internal static Unknown[] GetUnknownAttributes()
        {
            var unknowns = new Unknown[unknown_attributes
                .Count];
            Collections.ToArray(unknown_attributes.Values, unknowns);
            unknown_attributes.Clear();
            return unknowns;
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
            v.VisitUnknown(this);
        }

        /// <summary>Dump unknown bytes to file stream.</summary>
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

        /// <returns>name of attribute.</returns>
        public override string GetName()
        {
            return name;
        }

        /// <param name="bytes">the bytes to set</param>
        public void SetBytes(byte[] bytes)
        {
            this.bytes = bytes;
        }

        /// <returns>String representation.</returns>
        public override string ToString()
        {
            if (GetLength() == 0 || bytes == null) return "(Unknown attribute " + name + ")";
            string hex;
            if (GetLength() > 10)
            {
                var tmp = new byte[10];
                Array.Copy(bytes, 0, tmp, 0, 10);
                hex = Utility.ToHexString(tmp) + "... (truncated)";
            }
            else
            {
                hex = Utility.ToHexString(bytes);
            }

            return "(Unknown attribute " + name + ": " + hex + ")";
        }

        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            var c = (Unknown) Clone();
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