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
	public class EnumElementValue : NBCEL.classfile.ElementValue
	{
		private readonly int typeIdx;

		private readonly int valueIdx;

		public EnumElementValue(int type, int typeIdx, int valueIdx, NBCEL.classfile.ConstantPool
			 cpool)
			: base(type, cpool)
		{
			// For enum types, these two indices point to the type and value
			if (type != ENUM_CONSTANT)
			{
				throw new System.Exception("Only element values of type enum can be built with this ctor - type specified: "
					 + type);
			}
			this.typeIdx = typeIdx;
			this.valueIdx = valueIdx;
		}

		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream dos)
		{
			dos.WriteByte(base.GetType());
			// u1 type of value (ENUM_CONSTANT == 'e')
			dos.WriteShort(typeIdx);
			// u2
			dos.WriteShort(valueIdx);
		}

		// u2
		public override string StringifyValue()
		{
			NBCEL.classfile.ConstantUtf8 cu8 = (NBCEL.classfile.ConstantUtf8)base.GetConstantPool
				().GetConstant(valueIdx, NBCEL.Const.CONSTANT_Utf8);
			return cu8.GetBytes();
		}

		public virtual string GetEnumTypeString()
		{
			NBCEL.classfile.ConstantUtf8 cu8 = (NBCEL.classfile.ConstantUtf8)base.GetConstantPool
				().GetConstant(typeIdx, NBCEL.Const.CONSTANT_Utf8);
			return cu8.GetBytes();
		}

		// Utility.signatureToString(cu8.getBytes());
		public virtual string GetEnumValueString()
		{
			NBCEL.classfile.ConstantUtf8 cu8 = (NBCEL.classfile.ConstantUtf8)base.GetConstantPool
				().GetConstant(valueIdx, NBCEL.Const.CONSTANT_Utf8);
			return cu8.GetBytes();
		}

		public virtual int GetValueIndex()
		{
			return valueIdx;
		}

		public virtual int GetTypeIndex()
		{
			return typeIdx;
		}
	}
}
