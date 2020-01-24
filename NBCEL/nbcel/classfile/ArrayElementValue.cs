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
using System.Text;
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
    /// <since>6.0</since>
    public class ArrayElementValue : ElementValue
    {
        private readonly ElementValue[] evalues;

        public ArrayElementValue(int type, ElementValue[] datums, ConstantPool
            cpool)
            : base(type, cpool)
        {
            if (type != ARRAY)
                throw new Exception("Only element values of type array can be built with this ctor - type specified: "
                                    + type);
            evalues = datums;
        }

        // For array types, this is the array
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            for (var i = 0; i < evalues.Length; i++)
            {
                sb.Append(evalues[i]);
                if (i + 1 < evalues.Length) sb.Append(",");
            }

            sb.Append("}");
            return sb.ToString();
        }

        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream dos)
        {
            dos.WriteByte(GetType());
            // u1 type of value (ARRAY == '[')
            dos.WriteShort(evalues.Length);
            foreach (var evalue in evalues) evalue.Dump(dos);
        }

        public override string StringifyValue()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            for (var i = 0; i < evalues.Length; i++)
            {
                sb.Append(evalues[i].StringifyValue());
                if (i + 1 < evalues.Length) sb.Append(",");
            }

            sb.Append("]");
            return sb.ToString();
        }

        public virtual ElementValue[] GetElementValuesArray()
        {
            return evalues;
        }

        public virtual int GetElementValuesArraySize()
        {
            return evalues.Length;
        }
    }
}