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

using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	///     This class is derived from the abstract
	///     <see cref="Constant" />
	///     and represents a reference to an int object.
	/// </summary>
	/// <seealso cref="Constant" />
	public sealed class ConstantInteger : Constant, ConstantObject
    {
        private int bytes;

        /// <param name="bytes">Data</param>
        public ConstantInteger(int bytes)
            : base(Const.CONSTANT_Integer)
        {
            this.bytes = bytes;
        }

        /// <summary>Initialize from another object.</summary>
        public ConstantInteger(ConstantInteger c)
            : this(c.GetBytes())
        {
        }

        /// <summary>Initialize instance from file data.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal ConstantInteger(DataInput file)
            : this(file.ReadInt())
        {
        }

        /// <returns>Integer object</returns>
        public object GetConstantValue(ConstantPool cp)
        {
            return bytes;
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
            v.VisitConstantInteger(this);
        }

        /// <summary>Dump constant integer to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            file.WriteByte(GetTag());
            file.WriteInt(bytes);
        }

        /// <returns>data, i.e., 4 bytes.</returns>
        public int GetBytes()
        {
            return bytes;
        }

        /// <param name="bytes">the raw bytes that represent this integer</param>
        public void SetBytes(int bytes)
        {
            this.bytes = bytes;
        }

        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return base.ToString() + "(bytes = " + bytes + ")";
        }
    }
}