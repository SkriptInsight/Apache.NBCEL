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
using System.Collections.Generic;
using System.Text;
using java.io;
using NBCEL.classfile;
using Sharpen;

namespace NBCEL.generic
{
    /// <since>6.0</since>
    public class ArrayElementValueGen : ElementValueGen
    {
        private readonly List<ElementValueGen> evalues;

        public ArrayElementValueGen(ConstantPoolGen cp)
            : base(ARRAY, cp)
        {
            // J5TODO: Should we make this an array or a list? A list would be easier to
            // modify ...
            evalues = new List<ElementValueGen>();
        }

        public ArrayElementValueGen(int type, ElementValue[] datums, ConstantPoolGen
            cpool)
            : base(type, cpool)
        {
            if (type != ARRAY)
                throw new Exception("Only element values of type array can be built with this ctor - type specified: "
                                    + type);
            evalues = new List<ElementValueGen>
                ();
            foreach (var datum in datums) evalues.Add(Copy(datum, cpool, true));
        }

        /// <param name="value" />
        /// <param name="cpool" />
        public ArrayElementValueGen(ArrayElementValue value, ConstantPoolGen
            cpool, bool copyPoolEntries)
            : base(ARRAY, cpool)
        {
            evalues = new List<ElementValueGen>();
            var @in = value.GetElementValuesArray();
            foreach (var element in @in) evalues.Add(Copy(element, cpool, copyPoolEntries));
        }

        /// <summary>Return immutable variant of this ArrayElementValueGen</summary>
        public override ElementValue GetElementValue()
        {
            var immutableData = new ElementValue[evalues
                .Count];
            var i = 0;
            foreach (var element in evalues) immutableData[i++] = element.GetElementValue();
            return new ArrayElementValue(base.GetElementValueType(), immutableData
                , GetConstantPool().GetConstantPool());
        }

        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream dos)
        {
            dos.WriteByte(base.GetElementValueType());
            // u1 type of value (ARRAY == '[')
            dos.WriteShort(evalues.Count);
            foreach (var element in evalues) element.Dump(dos);
        }

        public override string StringifyValue()
        {
            var sb = new StringBuilder();
            sb.Append("[");
            var comma = string.Empty;
            foreach (var element in evalues)
            {
                sb.Append(comma);
                comma = ",";
                sb.Append(element.StringifyValue());
            }

            sb.Append("]");
            return sb.ToString();
        }

        public virtual List<ElementValueGen> GetElementValues
            ()
        {
            return evalues;
        }

        public virtual int GetElementValuesSize()
        {
            return evalues.Count;
        }

        public virtual void AddElement(ElementValueGen gen)
        {
            evalues.Add(gen);
        }
    }
}