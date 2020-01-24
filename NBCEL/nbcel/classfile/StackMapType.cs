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
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	///     This class represents the type of a local variable or item on stack
	///     used in the StackMap entries.
	/// </summary>
	/// <seealso cref="StackMapEntry" />
	/// <seealso cref="StackMap" />
	/// <seealso cref="NBCEL.Const" />
	public sealed class StackMapType : ICloneable
    {
        private ConstantPool constant_pool;

        private int index = -1;
        private byte type;

        /// <summary>Construct object from file stream.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal StackMapType(DataInput file, ConstantPool constant_pool
        )
            : this(file.ReadByte(), -1, constant_pool)
        {
            // Index to CONSTANT_Class or offset
            if (HasIndex()) index = file.ReadShort();
            this.constant_pool = constant_pool;
        }

        /// <param name="type">type tag as defined in the Constants interface</param>
        /// <param name="index">index to constant pool, or byte code offset</param>
        public StackMapType(byte type, int index, ConstantPool constant_pool
        )
        {
            if ((sbyte) type < Const.ITEM_Bogus || type > Const.ITEM_NewObject)
                throw new Exception("Illegal type for StackMapType: " + type);
            this.type = type;
            this.index = index;
            this.constant_pool = constant_pool;
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        public void SetType(byte t)
        {
            if ((sbyte) t < Const.ITEM_Bogus || t > Const.ITEM_NewObject)
                throw new Exception("Illegal type for StackMapType: " + t);
            type = t;
        }

        public byte GetType()
        {
            return type;
        }

        public void SetIndex(int t)
        {
            index = t;
        }

        /// <returns>
        ///     index to constant pool if type == ITEM_Object, or offset
        ///     in byte code, if type == ITEM_NewObject, and -1 otherwise
        /// </returns>
        public int GetIndex()
        {
            return index;
        }

        /// <summary>Dump type entries to file.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public void Dump(DataOutputStream file)
        {
            file.WriteByte(type);
            if (HasIndex()) file.WriteShort(GetIndex());
        }

        /// <returns>true, if type is either ITEM_Object or ITEM_NewObject</returns>
        public bool HasIndex()
        {
            return type == Const.ITEM_Object || type == Const.ITEM_NewObject;
        }

        private string PrintIndex()
        {
            if (type == Const.ITEM_Object)
            {
                if (index < 0) return ", class=<unknown>";
                return ", class=" + constant_pool.ConstantToString(index, Const.CONSTANT_Class
                       );
            }

            if (type == Const.ITEM_NewObject)
                return ", offset=" + index;
            return string.Empty;
        }

        /// <returns>String representation</returns>
        public override string ToString()
        {
            return "(type=" + Const.GetItemName(type) + PrintIndex() + ")";
        }

        /// <returns>deep copy of this object</returns>
        public StackMapType Copy()
        {
            return (StackMapType) MemberwiseClone();
            // TODO should this throw?
            return null;
        }

        /// <returns>Constant pool used by this object.</returns>
        public ConstantPool GetConstantPool()
        {
            return constant_pool;
        }

        /// <param name="constant_pool">Constant pool to be used for this object.</param>
        public void SetConstantPool(ConstantPool constant_pool)
        {
            this.constant_pool = constant_pool;
        }
    }
}