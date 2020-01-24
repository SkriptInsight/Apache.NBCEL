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
	///     and represents a reference to the name and signature
	///     of a field or method.
	/// </summary>
	/// <seealso cref="Constant" />
	public sealed class ConstantNameAndType : Constant
    {
        private int name_index;

        private int signature_index;

        /// <summary>Initialize from another object.</summary>
        public ConstantNameAndType(ConstantNameAndType c)
            : this(c.GetNameIndex(), c.GetSignatureIndex())
        {
        }

        /// <summary>Initialize instance from file data.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal ConstantNameAndType(DataInput file)
            : this(file.ReadUnsignedShort(), file.ReadUnsignedShort())
        {
        }

        /// <param name="name_index">Name of field/method</param>
        /// <param name="signature_index">and its signature</param>
        public ConstantNameAndType(int name_index, int signature_index)
            : base(Const.CONSTANT_NameAndType)
        {
            // Name of field/method
            // and its signature.
            this.name_index = name_index;
            this.signature_index = signature_index;
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
            v.VisitConstantNameAndType(this);
        }

        /// <summary>Dump name and signature index to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            file.WriteByte(GetTag());
            file.WriteShort(name_index);
            file.WriteShort(signature_index);
        }

        /// <returns>Name index in constant pool of field/method name.</returns>
        public int GetNameIndex()
        {
            return name_index;
        }

        /// <returns>name</returns>
        public string GetName(ConstantPool cp)
        {
            return cp.ConstantToString(GetNameIndex(), Const.CONSTANT_Utf8);
        }

        /// <returns>Index in constant pool of field/method signature.</returns>
        public int GetSignatureIndex()
        {
            return signature_index;
        }

        /// <returns>signature</returns>
        public string GetSignature(ConstantPool cp)
        {
            return cp.ConstantToString(GetSignatureIndex(), Const.CONSTANT_Utf8);
        }

        /// <param name="name_index">the name index of this constant</param>
        public void SetNameIndex(int name_index)
        {
            this.name_index = name_index;
        }

        /// <param name="signature_index">
        ///     the signature index in the constant pool of this type
        /// </param>
        public void SetSignatureIndex(int signature_index)
        {
            this.signature_index = signature_index;
        }

        /// <returns>String representation</returns>
        public override string ToString()
        {
            return base.ToString() + "(name_index = " + name_index + ", signature_index = " +
                   signature_index + ")";
        }
    }
}