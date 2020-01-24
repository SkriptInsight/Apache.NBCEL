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
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>an annotation's element value pair</summary>
	/// <since>6.0</since>
	public class ElementValuePair
	{
		private readonly NBCEL.classfile.ElementValue elementValue;

		private readonly NBCEL.classfile.ConstantPool constantPool;

		private readonly int elementNameIndex;

		public ElementValuePair(int elementNameIndex, NBCEL.classfile.ElementValue elementValue
			, NBCEL.classfile.ConstantPool constantPool)
		{
			this.elementValue = elementValue;
			this.elementNameIndex = elementNameIndex;
			this.constantPool = constantPool;
		}

		public virtual string GetNameString()
		{
			NBCEL.classfile.ConstantUtf8 c = (NBCEL.classfile.ConstantUtf8)constantPool.GetConstant
				(elementNameIndex, NBCEL.Const.CONSTANT_Utf8);
			return c.GetBytes();
		}

		public NBCEL.classfile.ElementValue GetValue()
		{
			return elementValue;
		}

		public virtual int GetNameIndex()
		{
			return elementNameIndex;
		}

		public virtual string ToShortString()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append(GetNameString()).Append("=").Append(GetValue().ToShortString());
			return result.ToString();
		}

		/// <exception cref="System.IO.IOException"/>
		protected internal virtual void Dump(java.io.DataOutputStream dos)
		{
			dos.WriteShort(elementNameIndex);
			// u2 name of the element
			elementValue.Dump(dos);
		}
	}
}
