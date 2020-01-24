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
	///     and represents a reference to a method type.
	/// </summary>
	/// <seealso cref="Constant" />
	/// <since>6.0</since>
	public sealed class ConstantMethodType : Constant
    {
        private int descriptor_index;

        /// <summary>Initialize from another object.</summary>
        public ConstantMethodType(ConstantMethodType c)
            : this(c.GetDescriptorIndex())
        {
        }

        /// <summary>Initialize instance from file data.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal ConstantMethodType(DataInput file)
            : this(file.ReadUnsignedShort())
        {
        }

        public ConstantMethodType(int descriptor_index)
            : base(Const.CONSTANT_MethodType)
        {
            this.descriptor_index = descriptor_index;
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
            v.VisitConstantMethodType(this);
        }

        /// <summary>Dump name and signature index to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            file.WriteByte(GetTag());
            file.WriteShort(descriptor_index);
        }

        public int GetDescriptorIndex()
        {
            return descriptor_index;
        }

        public void SetDescriptorIndex(int descriptor_index)
        {
            this.descriptor_index = descriptor_index;
        }

        /// <returns>String representation</returns>
        public override string ToString()
        {
            return base.ToString() + "(descriptor_index = " + descriptor_index + ")";
        }
    }
}