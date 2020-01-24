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
	///     and represents a reference to a String object.
	/// </summary>
	/// <seealso cref="Constant" />
	public sealed class ConstantString : Constant, ConstantObject
    {
        private int string_index;

        /// <summary>Initialize from another object.</summary>
        public ConstantString(ConstantString c)
            : this(c.GetStringIndex())
        {
        }

        /// <summary>Initialize instance from file data.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal ConstantString(DataInput file)
            : this(file.ReadUnsignedShort())
        {
        }

        /// <param name="string_index">Index of Constant_Utf8 in constant pool</param>
        public ConstantString(int string_index)
            : base(Const.CONSTANT_String)
        {
            // Identical to ConstantClass except for this name
            this.string_index = string_index;
        }

        /// <returns>String object</returns>
        public object GetConstantValue(ConstantPool cp)
        {
            var c = cp.GetConstant(string_index, Const.CONSTANT_Utf8
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
            v.VisitConstantString(this);
        }

        /// <summary>Dump constant field reference to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            file.WriteByte(GetTag());
            file.WriteShort(string_index);
        }

        /// <returns>Index in constant pool of the string (ConstantUtf8).</returns>
        public int GetStringIndex()
        {
            return string_index;
        }

        /// <param name="string_index">the index into the constant of the string value</param>
        public void SetStringIndex(int string_index)
        {
            this.string_index = string_index;
        }

        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return base.ToString() + "(string_index = " + string_index + ")";
        }

        /// <returns>dereferenced string</returns>
        public string GetBytes(ConstantPool cp)
        {
            return (string) GetConstantValue(cp);
        }
    }
}