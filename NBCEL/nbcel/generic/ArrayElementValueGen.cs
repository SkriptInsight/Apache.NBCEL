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
	public class ArrayElementValueGen : NBCEL.generic.ElementValueGen
	{
		private readonly System.Collections.Generic.List<NBCEL.generic.ElementValueGen> evalues;

		public ArrayElementValueGen(NBCEL.generic.ConstantPoolGen cp)
			: base(ARRAY, cp)
		{
			// J5TODO: Should we make this an array or a list? A list would be easier to
			// modify ...
			evalues = new System.Collections.Generic.List<NBCEL.generic.ElementValueGen>();
		}

		public ArrayElementValueGen(int type, NBCEL.classfile.ElementValue[] datums, NBCEL.generic.ConstantPoolGen
			 cpool)
			: base(type, cpool)
		{
			if (type != ARRAY)
			{
				throw new System.Exception("Only element values of type array can be built with this ctor - type specified: "
					 + type);
			}
			this.evalues = new System.Collections.Generic.List<NBCEL.generic.ElementValueGen>
				();
			foreach (NBCEL.classfile.ElementValue datum in datums)
			{
				evalues.Add(NBCEL.generic.ElementValueGen.Copy(datum, cpool, true));
			}
		}

		/// <summary>Return immutable variant of this ArrayElementValueGen</summary>
		public override NBCEL.classfile.ElementValue GetElementValue()
		{
			NBCEL.classfile.ElementValue[] immutableData = new NBCEL.classfile.ElementValue[evalues
				.Count];
			int i = 0;
			foreach (NBCEL.generic.ElementValueGen element in evalues)
			{
				immutableData[i++] = element.GetElementValue();
			}
			return new NBCEL.classfile.ArrayElementValue(base.GetElementValueType(), immutableData
				, GetConstantPool().GetConstantPool());
		}

		/// <param name="value"/>
		/// <param name="cpool"/>
		public ArrayElementValueGen(NBCEL.classfile.ArrayElementValue value, NBCEL.generic.ConstantPoolGen
			 cpool, bool copyPoolEntries)
			: base(ARRAY, cpool)
		{
			evalues = new System.Collections.Generic.List<NBCEL.generic.ElementValueGen>();
			NBCEL.classfile.ElementValue[] @in = value.GetElementValuesArray();
			foreach (NBCEL.classfile.ElementValue element in @in)
			{
				evalues.Add(NBCEL.generic.ElementValueGen.Copy(element, cpool, copyPoolEntries));
			}
		}

		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream dos)
		{
			dos.WriteByte(base.GetElementValueType());
			// u1 type of value (ARRAY == '[')
			dos.WriteShort(evalues.Count);
			foreach (NBCEL.generic.ElementValueGen element in evalues)
			{
				element.Dump(dos);
			}
		}

		public override string StringifyValue()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("[");
			string comma = string.Empty;
			foreach (NBCEL.generic.ElementValueGen element in evalues)
			{
				sb.Append(comma);
				comma = ",";
				sb.Append(element.StringifyValue());
			}
			sb.Append("]");
			return sb.ToString();
		}

		public virtual System.Collections.Generic.List<NBCEL.generic.ElementValueGen> GetElementValues
			()
		{
			return evalues;
		}

		public virtual int GetElementValuesSize()
		{
			return evalues.Count;
		}

		public virtual void AddElement(NBCEL.generic.ElementValueGen gen)
		{
			evalues.Add(gen);
		}
	}
}
