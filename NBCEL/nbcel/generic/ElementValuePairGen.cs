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
    public class ElementValuePairGen
    {
        private readonly ConstantPoolGen cpool;

        private readonly ElementValueGen value;
        private readonly int nameIdx;

        public ElementValuePairGen(ElementValuePair nvp, ConstantPoolGen
            cpool, bool copyPoolEntries)
        {
            this.cpool = cpool;
            // J5ASSERT:
            // Could assert nvp.getNameString() points to the same thing as
            // cpool.getConstant(nvp.getNameIndex())
            // if
            // (!nvp.getNameString().equals(((ConstantUtf8)cpool.getConstant(nvp.getNameIndex())).getBytes()))
            // {
            // throw new RuntimeException("envp buggered");
            // }
            if (copyPoolEntries)
                nameIdx = cpool.AddUtf8(nvp.GetNameString());
            else
                nameIdx = nvp.GetNameIndex();
            value = ElementValueGen.Copy(nvp.GetValue(), cpool, copyPoolEntries
            );
        }

        protected internal ElementValuePairGen(int idx, ElementValueGen value
            , ConstantPoolGen cpool)
        {
            nameIdx = idx;
            this.value = value;
            this.cpool = cpool;
        }

        public ElementValuePairGen(string name, ElementValueGen value, ConstantPoolGen
            cpool)
        {
            nameIdx = cpool.AddUtf8(name);
            this.value = value;
            this.cpool = cpool;
        }

        /// <summary>Retrieve an immutable version of this ElementNameValuePairGen</summary>
        public virtual ElementValuePair GetElementNameValuePair()
        {
            var immutableValue = value.GetElementValue();
            return new ElementValuePair(nameIdx, immutableValue, cpool.GetConstantPool
                ());
        }

        /// <exception cref="System.IO.IOException" />
        protected internal virtual void Dump(DataOutputStream dos)
        {
            dos.WriteShort(nameIdx);
            // u2 name of the element
            value.Dump(dos);
        }

        public virtual int GetNameIndex()
        {
            return nameIdx;
        }

        public string GetNameString()
        {
            // ConstantString cu8 = (ConstantString)cpool.getConstant(nameIdx);
            return ((ConstantUtf8) cpool.GetConstant(nameIdx)).GetBytes();
        }

        public ElementValueGen GetValue()
        {
            return value;
        }

        public override string ToString()
        {
            return "ElementValuePair:[" + GetNameString() + "=" + value.StringifyValue() + "]";
        }
    }
}