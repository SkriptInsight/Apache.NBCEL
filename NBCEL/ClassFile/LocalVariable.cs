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
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>This class represents a local variable within a method.</summary>
	/// <remarks>
	///     This class represents a local variable within a method. It contains its
	///     scope, name, signature and index on the method's frame.  It is used both
	///     to represent an element of the LocalVariableTable as well as an element
	///     of the LocalVariableTypeTable.  The nomenclature used here may be a bit confusing;
	///     while the two items have the same layout in a class file, a LocalVariableTable
	///     attribute contains a descriptor_index, not a signature_index.  The
	///     LocalVariableTypeTable attribute does have a signature_index.
	/// </remarks>
	/// <seealso cref="Utility">for more details on the difference.</seealso>
	/// <seealso cref="LocalVariableTable" />
	/// <seealso cref="LocalVariableTypeTable" />
	public sealed class LocalVariable : ICloneable, Node
    {
        private ConstantPool constant_pool;

        private int index;

        private int length;

        private int name_index;

        private readonly int orig_index;

        private int signature_index;
        private int start_pc;

        /// <summary>Initializes from another LocalVariable.</summary>
        /// <remarks>
        ///     Initializes from another LocalVariable. Note that both objects use the same
        ///     references (shallow copy). Use copy() for a physical copy.
        /// </remarks>
        /// <param name="localVariable">Another LocalVariable.</param>
        public LocalVariable(LocalVariable localVariable)
            : this(localVariable.GetStartPC(), localVariable.GetLength(), localVariable.GetNameIndex
                (), localVariable.GetSignatureIndex(), localVariable.GetIndex(), localVariable.GetConstantPool
                ())
        {
            // Range in which the variable is valid
            // Index in constant pool of variable name
            // Technically, a decscriptor_index for a local variable table entry
            // and a signature_index for a local variable type table entry.
            // Index of variable signature
            /* Variable is index'th local variable on
            * this method's frame.
            */
            // never changes; used to match up with LocalVariableTypeTable entries
            orig_index = localVariable.GetOrigIndex();
        }

        /// <summary>Constructs object from file stream.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal LocalVariable(DataInput file, ConstantPool constant_pool
        )
            : this(file.ReadUnsignedShort(), file.ReadUnsignedShort(), file.ReadUnsignedShort
                (), file.ReadUnsignedShort(), file.ReadUnsignedShort(), constant_pool)
        {
        }

        /// <param name="start_pc">Range in which the variable</param>
        /// <param name="length">... is valid</param>
        /// <param name="name_index">Index in constant pool of variable name</param>
        /// <param name="signature_index">Index of variable's signature</param>
        /// <param name="index">Variable is `index'th local variable on the method's frame</param>
        /// <param name="constant_pool">Array of constants</param>
        public LocalVariable(int start_pc, int length, int name_index, int signature_index
            , int index, ConstantPool constant_pool)
        {
            this.start_pc = start_pc;
            this.length = length;
            this.name_index = name_index;
            this.signature_index = signature_index;
            this.index = index;
            this.constant_pool = constant_pool;
            orig_index = index;
        }

        /// <param name="start_pc">Range in which the variable</param>
        /// <param name="length">... is valid</param>
        /// <param name="name_index">Index in constant pool of variable name</param>
        /// <param name="signature_index">Index of variable's signature</param>
        /// <param name="index">Variable is `index'th local variable on the method's frame</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <param name="orig_index">
        ///     Variable is `index'th local variable on the method's frame prior to any changes
        /// </param>
        public LocalVariable(int start_pc, int length, int name_index, int signature_index
            , int index, ConstantPool constant_pool, int orig_index)
        {
            this.start_pc = start_pc;
            this.length = length;
            this.name_index = name_index;
            this.signature_index = signature_index;
            this.index = index;
            this.constant_pool = constant_pool;
            this.orig_index = orig_index;
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
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
        public void Accept(Visitor v)
        {
            v.VisitLocalVariable(this);
        }

        /// <summary>Dumps local variable to file stream in binary format.</summary>
        /// <param name="dataOutputStream">Output file stream</param>
        /// <exception>
        ///     IOException
        ///     if an I/O error occurs.
        /// </exception>
        /// <seealso cref="Java.IO.FilterOutputStream#out" />
        /// <exception cref="System.IO.IOException" />
        public void Dump(DataOutputStream dataOutputStream)
        {
            dataOutputStream.WriteShort(start_pc);
            dataOutputStream.WriteShort(length);
            dataOutputStream.WriteShort(name_index);
            dataOutputStream.WriteShort(signature_index);
            dataOutputStream.WriteShort(index);
        }

        /// <returns>Constant pool used by this object.</returns>
        public ConstantPool GetConstantPool()
        {
            return constant_pool;
        }

        /// <returns>Variable is valid within getStartPC() .. getStartPC()+getLength()</returns>
        public int GetLength()
        {
            return length;
        }

        /// <returns>Variable name.</returns>
        public string GetName()
        {
            ConstantUtf8 c;
            c = (ConstantUtf8) constant_pool.GetConstant(name_index, Const
                .CONSTANT_Utf8);
            return c.GetBytes();
        }

        /// <returns>Index in constant pool of variable name.</returns>
        public int GetNameIndex()
        {
            return name_index;
        }

        /// <returns>Signature.</returns>
        public string GetSignature()
        {
            ConstantUtf8 c;
            c = (ConstantUtf8) constant_pool.GetConstant(signature_index, Const
                .CONSTANT_Utf8);
            return c.GetBytes();
        }

        /// <returns>Index in constant pool of variable signature.</returns>
        public int GetSignatureIndex()
        {
            return signature_index;
        }

        /// <returns>index of register where variable is stored</returns>
        public int GetIndex()
        {
            return index;
        }

        /// <returns>index of register where variable was originally stored</returns>
        public int GetOrigIndex()
        {
            return orig_index;
        }

        /// <returns>Start of range where the variable is valid</returns>
        public int GetStartPC()
        {
            return start_pc;
        }

        /*
        * Helper method shared with LocalVariableTypeTable
        */
        internal string ToStringShared(bool typeTable)
        {
            var name = GetName();
            var signature = Utility.SignatureToString(GetSignature(), false
            );
            var label = "LocalVariable" + (typeTable ? "Types" : string.Empty);
            return label + "(start_pc = " + start_pc + ", length = " + length + ", index = "
                   + index + ":" + signature + " " + name + ")";
        }

        /// <param name="constant_pool">Constant pool to be used for this object.</param>
        public void SetConstantPool(ConstantPool constant_pool)
        {
            this.constant_pool = constant_pool;
        }

        /// <param name="length">the length of this local variable</param>
        public void SetLength(int length)
        {
            this.length = length;
        }

        /// <param name="name_index">
        ///     the index into the constant pool for the name of this variable
        /// </param>
        public void SetNameIndex(int name_index)
        {
            // TODO unused
            this.name_index = name_index;
        }

        /// <param name="signature_index">
        ///     the index into the constant pool for the signature of this variable
        /// </param>
        public void SetSignatureIndex(int signature_index)
        {
            // TODO unused
            this.signature_index = signature_index;
        }

        /// <param name="index">the index in the local variable table of this variable</param>
        public void SetIndex(int index)
        {
            // TODO unused
            this.index = index;
        }

        /// <param name="start_pc">Specify range where the local variable is valid.</param>
        public void SetStartPC(int start_pc)
        {
            // TODO unused
            this.start_pc = start_pc;
        }

        /// <returns>string representation.</returns>
        public override string ToString()
        {
            return ToStringShared(false);
        }

        /// <returns>deep copy of this object</returns>
        public LocalVariable Copy()
        {
            return (LocalVariable) MemberwiseClone();
            // TODO should this throw?
            return null;
        }
    }
}