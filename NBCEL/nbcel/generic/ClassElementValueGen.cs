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

using java.io;
using NBCEL.classfile;
using Sharpen;

namespace NBCEL.generic
{
    /// <since>6.0</since>
    public class ClassElementValueGen : ElementValueGen
    {
        private readonly int idx;

        protected internal ClassElementValueGen(int typeIdx, ConstantPoolGen
            cpool)
            : base(CLASS, cpool)
        {
            // For primitive types and string type, this points to the value entry in
            // the cpool
            // For 'class' this points to the class entry in the cpool
            idx = typeIdx;
        }

        public ClassElementValueGen(ObjectType t, ConstantPoolGen
            cpool)
            : base(CLASS, cpool)
        {
            // this.idx = cpool.addClass(t);
            idx = cpool.AddUtf8(t.GetSignature());
        }

        public ClassElementValueGen(ClassElementValue value, ConstantPoolGen
            cpool, bool copyPoolEntries)
            : base(CLASS, cpool)
        {
            if (copyPoolEntries)
            {
                // idx = cpool.addClass(value.getClassString());
                idx = cpool.AddUtf8(value.GetClassString());
            }
            else
            {
                idx = value.GetIndex();
            }
        }

        /// <summary>Return immutable variant of this ClassElementValueGen</summary>
        public override ElementValue GetElementValue()
        {
            return new ClassElementValue(base.GetElementValueType(), idx, GetConstantPool
                ().GetConstantPool());
        }

        public virtual int GetIndex()
        {
            return idx;
        }

        public virtual string GetClassString()
        {
            var cu8 = (ConstantUtf8) GetConstantPool(
            ).GetConstant(idx);
            return cu8.GetBytes();
        }

        // ConstantClass c = (ConstantClass)getConstantPool().getConstant(idx);
        // ConstantUtf8 utf8 =
        // (ConstantUtf8)getConstantPool().getConstant(c.getNameIndex());
        // return utf8.getBytes();
        public override string StringifyValue()
        {
            return GetClassString();
        }

        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream dos)
        {
            dos.WriteByte(base.GetElementValueType());
            // u1 kind of value
            dos.WriteShort(idx);
        }
    }
}