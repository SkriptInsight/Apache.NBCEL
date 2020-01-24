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
	///     and represents a reference to a Double object.
	/// </summary>
	/// <seealso cref="Constant" />
	public sealed class ConstantDouble : Constant, ConstantObject
    {
        private double bytes;

        /// <param name="bytes">Data</param>
        public ConstantDouble(double bytes)
            : base(Const.CONSTANT_Double)
        {
            this.bytes = bytes;
        }

        /// <summary>Initialize from another object.</summary>
        public ConstantDouble(ConstantDouble c)
            : this(c.GetBytes())
        {
        }

        /// <summary>Initialize instance from file data.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal ConstantDouble(DataInput file)
            : this(file.ReadDouble())
        {
        }

        /// <returns>Double object</returns>
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
            v.VisitConstantDouble(this);
        }

        /// <summary>Dump constant double to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            file.WriteByte(GetTag());
            file.WriteDouble(bytes);
        }

        /// <returns>data, i.e., 8 bytes.</returns>
        public double GetBytes()
        {
            return bytes;
        }

        /// <param name="bytes">the raw bytes that represent the double value</param>
        public void SetBytes(double bytes)
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