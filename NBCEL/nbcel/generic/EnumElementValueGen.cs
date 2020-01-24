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
using NBCEL.classfile;
using Sharpen;

namespace NBCEL.generic
{
    /// <since>6.0</since>
    public class EnumElementValueGen : ElementValueGen
    {
        private readonly int typeIdx;

        private readonly int valueIdx;

        /// <summary>
        ///     This ctor assumes the constant pool already contains the right type and
        ///     value - as indicated by typeIdx and valueIdx.
        /// </summary>
        /// <remarks>
        ///     This ctor assumes the constant pool already contains the right type and
        ///     value - as indicated by typeIdx and valueIdx. This ctor is used for
        ///     deserialization
        /// </remarks>
        protected internal EnumElementValueGen(int typeIdx, int valueIdx, ConstantPoolGen
            cpool)
            : base(ENUM_CONSTANT, cpool)
        {
            // For enum types, these two indices point to the type and value
            if (base.GetElementValueType() != ENUM_CONSTANT)
                throw new Exception("Only element values of type enum can be built with this ctor - type specified: "
                                    + base.GetElementValueType());
            this.typeIdx = typeIdx;
            this.valueIdx = valueIdx;
        }

        public EnumElementValueGen(ObjectType t, string value, ConstantPoolGen
            cpool)
            : base(ENUM_CONSTANT, cpool)
        {
            typeIdx = cpool.AddUtf8(t.GetSignature());
            // was addClass(t);
            valueIdx = cpool.AddUtf8(value);
        }

        public EnumElementValueGen(EnumElementValue value, ConstantPoolGen
            cpool, bool copyPoolEntries)
            : base(ENUM_CONSTANT, cpool)
        {
            // was addString(value);
            if (copyPoolEntries)
            {
                typeIdx = cpool.AddUtf8(value.GetEnumTypeString());
                // was
                // addClass(value.getEnumTypeString());
                valueIdx = cpool.AddUtf8(value.GetEnumValueString());
            }
            else
            {
                // was
                // addString(value.getEnumValueString());
                typeIdx = value.GetTypeIndex();
                valueIdx = value.GetValueIndex();
            }
        }

        /// <summary>Return immutable variant of this EnumElementValue</summary>
        public override ElementValue GetElementValue()
        {
            Console.Error.WriteLine("Duplicating value: " + GetEnumTypeString() + ":"
                                    + GetEnumValueString());
            return new EnumElementValue(base.GetElementValueType(), typeIdx,
                valueIdx, GetConstantPool().GetConstantPool());
        }

        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream dos)
        {
            dos.WriteByte(base.GetElementValueType());
            // u1 type of value (ENUM_CONSTANT == 'e')
            dos.WriteShort(typeIdx);
            // u2
            dos.WriteShort(valueIdx);
        }

        // u2
        public override string StringifyValue()
        {
            var cu8 = (ConstantUtf8) GetConstantPool(
            ).GetConstant(valueIdx);
            return cu8.GetBytes();
        }

        // ConstantString cu8 =
        // (ConstantString)getConstantPool().getConstant(valueIdx);
        // return
        // ((ConstantUtf8)getConstantPool().getConstant(cu8.getStringIndex())).getBytes();
        // BCELBUG: Should we need to call utility.signatureToString() on the output
        // here?
        public virtual string GetEnumTypeString()
        {
            // Constant cc = getConstantPool().getConstant(typeIdx);
            // ConstantClass cu8 =
            // (ConstantClass)getConstantPool().getConstant(typeIdx);
            // return
            // ((ConstantUtf8)getConstantPool().getConstant(cu8.getNameIndex())).getBytes();
            return ((ConstantUtf8) GetConstantPool().GetConstant(typeIdx)).GetBytes
                ();
        }

        // return Utility.signatureToString(cu8.getBytes());
        public virtual string GetEnumValueString()
        {
            return ((ConstantUtf8) GetConstantPool().GetConstant(valueIdx)).GetBytes
                ();
        }

        // ConstantString cu8 =
        // (ConstantString)getConstantPool().getConstant(valueIdx);
        // return
        // ((ConstantUtf8)getConstantPool().getConstant(cu8.getStringIndex())).getBytes();
        public virtual int GetValueIndex()
        {
            return valueIdx;
        }

        public virtual int GetTypeIndex()
        {
            return typeIdx;
        }
    }
}