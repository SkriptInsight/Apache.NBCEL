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
	///     and represents a reference to a float object.
	/// </summary>
	/// <seealso cref="Constant" />
	public sealed class ConstantFloat : Constant, ConstantObject
    {
        private float bytes;

        /// <param name="bytes">Data</param>
        public ConstantFloat(float bytes)
            : base(Const.CONSTANT_Float)
        {
            this.bytes = bytes;
        }

        /// <summary>Initialize from another object.</summary>
        /// <remarks>
        ///     Initialize from another object. Note that both objects use the same
        ///     references (shallow copy). Use clone() for a physical copy.
        /// </remarks>
        public ConstantFloat(ConstantFloat c)
            : this(c.GetBytes())
        {
        }

        /// <summary>Initialize instance from file data.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal ConstantFloat(DataInput file)
            : this(file.ReadFloat())
        {
        }

        /// <returns>Float object</returns>
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
            v.VisitConstantFloat(this);
        }

        /// <summary>Dump constant float to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            file.WriteByte(GetTag());
            file.WriteFloat(bytes);
        }

        /// <returns>data, i.e., 4 bytes.</returns>
        public float GetBytes()
        {
            return bytes;
        }

        /// <param name="bytes">the raw bytes that represent this float</param>
        public void SetBytes(float bytes)
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