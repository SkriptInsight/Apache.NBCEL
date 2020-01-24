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
	///     and represents a reference to a (external) class.
	/// </summary>
	/// <seealso cref="Constant" />
	public sealed class ConstantClass : Constant, ConstantObject
    {
        private int name_index;

        /// <summary>Initialize from another object.</summary>
        public ConstantClass(ConstantClass c)
            : this(c.GetNameIndex())
        {
        }

        /// <summary>Constructs an instance from file data.</summary>
        /// <param name="dataInput">Input stream</param>
        /// <exception cref="System.IO.IOException">
        ///     if an I/O error occurs reading from the given
        ///     <paramref name="dataInput" />
        ///     .
        /// </exception>
        internal ConstantClass(DataInput dataInput)
            : this(dataInput.ReadUnsignedShort())
        {
        }

        /// <param name="name_index">
        ///     Name index in constant pool.  Should refer to a
        ///     ConstantUtf8.
        /// </param>
        public ConstantClass(int name_index)
            : base(Const.CONSTANT_Class)
        {
            // Identical to ConstantString except for the name
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
            v.VisitConstantClass(this);
        }

        /// <summary>Dumps constant class to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException">
        ///     if an I/O error occurs writing to the DataOutputStream.
        /// </exception>
        public override void Dump(DataOutputStream file)
        {
            file.WriteByte(GetTag());
            file.WriteShort(name_index);
        }

        /// <returns>Name index in constant pool of class name.</returns>
        public int GetNameIndex()
        {
            return name_index;
        }

        /// <param name="name_index">
        ///     the name index in the constant pool of this Constant Class
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