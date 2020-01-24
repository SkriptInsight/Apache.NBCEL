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

namespace NBCEL.generic
{
	/// <since>6.0</since>
	public class ElementValuePairGen
	{
		private int nameIdx;

		private readonly NBCEL.generic.ElementValueGen value;

		private readonly NBCEL.generic.ConstantPoolGen cpool;

		public ElementValuePairGen(NBCEL.classfile.ElementValuePair nvp, NBCEL.generic.ConstantPoolGen
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
			{
				nameIdx = cpool.AddUtf8(nvp.GetNameString());
			}
			else
			{
				nameIdx = nvp.GetNameIndex();
			}
			value = NBCEL.generic.ElementValueGen.Copy(nvp.GetValue(), cpool, copyPoolEntries
				);
		}

		/// <summary>Retrieve an immutable version of this ElementNameValuePairGen</summary>
		public virtual NBCEL.classfile.ElementValuePair GetElementNameValuePair()
		{
			NBCEL.classfile.ElementValue immutableValue = value.GetElementValue();
			return new NBCEL.classfile.ElementValuePair(nameIdx, immutableValue, cpool.GetConstantPool
				());
		}

		protected internal ElementValuePairGen(int idx, NBCEL.generic.ElementValueGen value
			, NBCEL.generic.ConstantPoolGen cpool)
		{
			this.nameIdx = idx;
			this.value = value;
			this.cpool = cpool;
		}

		public ElementValuePairGen(string name, NBCEL.generic.ElementValueGen value, NBCEL.generic.ConstantPoolGen
			 cpool)
		{
			this.nameIdx = cpool.AddUtf8(name);
			this.value = value;
			this.cpool = cpool;
		}

		/// <exception cref="System.IO.IOException"/>
		protected internal virtual void Dump(java.io.DataOutputStream dos)
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
			return ((NBCEL.classfile.ConstantUtf8)cpool.GetConstant(nameIdx)).GetBytes();
		}

		public NBCEL.generic.ElementValueGen GetValue()
		{
			return value;
		}

		public override string ToString()
		{
			return "ElementValuePair:[" + GetNameString() + "=" + value.StringifyValue() + "]";
		}
	}
}
