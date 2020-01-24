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

using System.Text;
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>an annotation's element value pair</summary>
	/// <since>6.0</since>
	public class ElementValuePair
    {
        private readonly ConstantPool constantPool;

        private readonly int elementNameIndex;
        private readonly ElementValue elementValue;

        public ElementValuePair(int elementNameIndex, ElementValue elementValue
            , ConstantPool constantPool)
        {
            this.elementValue = elementValue;
            this.elementNameIndex = elementNameIndex;
            this.constantPool = constantPool;
        }

        public virtual string GetNameString()
        {
            var c = (ConstantUtf8) constantPool.GetConstant
                (elementNameIndex, Const.CONSTANT_Utf8);
            return c.GetBytes();
        }

        public ElementValue GetValue()
        {
            return elementValue;
        }

        public virtual int GetNameIndex()
        {
            return elementNameIndex;
        }

        public virtual string ToShortString()
        {
            var result = new StringBuilder();
            result.Append(GetNameString()).Append("=").Append(GetValue().ToShortString());
            return result.ToString();
        }

        /// <exception cref="System.IO.IOException" />
        protected internal virtual void Dump(DataOutputStream dos)
        {
            dos.WriteShort(elementNameIndex);
            // u2 name of the element
            elementValue.Dump(dos);
        }
    }
}