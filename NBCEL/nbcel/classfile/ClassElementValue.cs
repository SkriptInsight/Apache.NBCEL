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
using Sharpen;

namespace NBCEL.classfile
{
    /// <since>6.0</since>
    public class ClassElementValue : ElementValue
    {
        private readonly int idx;

        public ClassElementValue(int type, int idx, ConstantPool cpool)
            : base(type, cpool)
        {
            // For primitive types and string type, this points to the value entry in
            // the cpool
            // For 'class' this points to the class entry in the cpool
            this.idx = idx;
        }

        public virtual int GetIndex()
        {
            return idx;
        }

        public virtual string GetClassString()
        {
            var c = (ConstantUtf8) GetConstantPool
                ().GetConstant(idx, Const.CONSTANT_Utf8);
            return c.GetBytes();
        }

        public override string StringifyValue()
        {
            var cu8 = (ConstantUtf8) GetConstantPool
                ().GetConstant(idx, Const.CONSTANT_Utf8);
            return cu8.GetBytes();
        }

        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream dos)
        {
            dos.WriteByte(GetType());
            // u1 kind of value
            dos.WriteShort(idx);
        }
    }
}