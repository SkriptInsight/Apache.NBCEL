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
	/// <since>6.0</since>
	public class ArrayElementValue : NBCEL.classfile.ElementValue
	{
		private readonly NBCEL.classfile.ElementValue[] evalues;

		// For array types, this is the array
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("{");
			for (int i = 0; i < evalues.Length; i++)
			{
				sb.Append(evalues[i]);
				if ((i + 1) < evalues.Length)
				{
					sb.Append(",");
				}
			}
			sb.Append("}");
			return sb.ToString();
		}

		public ArrayElementValue(int type, NBCEL.classfile.ElementValue[] datums, NBCEL.classfile.ConstantPool
			 cpool)
			: base(type, cpool)
		{
			if (type != ARRAY)
			{
				throw new System.Exception("Only element values of type array can be built with this ctor - type specified: "
					 + type);
			}
			this.evalues = datums;
		}

		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream dos)
		{
			dos.WriteByte(base.GetType());
			// u1 type of value (ARRAY == '[')
			dos.WriteShort(evalues.Length);
			foreach (NBCEL.classfile.ElementValue evalue in evalues)
			{
				evalue.Dump(dos);
			}
		}

		public override string StringifyValue()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("[");
			for (int i = 0; i < evalues.Length; i++)
			{
				sb.Append(evalues[i].StringifyValue());
				if ((i + 1) < evalues.Length)
				{
					sb.Append(",");
				}
			}
			sb.Append("]");
			return sb.ToString();
		}

		public virtual NBCEL.classfile.ElementValue[] GetElementValuesArray()
		{
			return evalues;
		}

		public virtual int GetElementValuesArraySize()
		{
			return evalues.Length;
		}
	}
}
