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
	///     and represents a reference to a method handle.
	/// </summary>
	/// <seealso cref="Constant" />
	/// <since>6.0</since>
	public sealed class ConstantMethodHandle : Constant
    {
        private int reference_index;
        private int reference_kind;

        /// <summary>Initialize from another object.</summary>
        public ConstantMethodHandle(ConstantMethodHandle c)
            : this(c.GetReferenceKind(), c.GetReferenceIndex())
        {
        }

        /// <summary>Initialize instance from file data.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal ConstantMethodHandle(DataInput file)
            : this(file.ReadUnsignedByte(), file.ReadUnsignedShort())
        {
        }

        public ConstantMethodHandle(int reference_kind, int reference_index)
            : base(Const.CONSTANT_MethodHandle)
        {
            this.reference_kind = reference_kind;
            this.reference_index = reference_index;
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
            v.VisitConstantMethodHandle(this);
        }

        /// <summary>Dump method kind and index to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            file.WriteByte(GetTag());
            file.WriteByte(reference_kind);
            file.WriteShort(reference_index);
        }

        public int GetReferenceKind()
        {
            return reference_kind;
        }

        public void SetReferenceKind(int reference_kind)
        {
            this.reference_kind = reference_kind;
        }

        public int GetReferenceIndex()
        {
            return reference_index;
        }

        public void SetReferenceIndex(int reference_index)
        {
            this.reference_index = reference_index;
        }

        /// <returns>String representation</returns>
        public override string ToString()
        {
            return base.ToString() + "(reference_kind = " + reference_kind + ", reference_index = "
                   + reference_index + ")";
        }
    }
}