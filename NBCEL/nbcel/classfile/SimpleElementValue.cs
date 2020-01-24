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

using System.Globalization;
using Sharpen;

namespace NBCEL.classfile
{
	/// <since>6.0</since>
	public class SimpleElementValue : NBCEL.classfile.ElementValue
	{
		private int index;

		public SimpleElementValue(int type, int index, NBCEL.classfile.ConstantPool cpool
			)
			: base(type, cpool)
		{
			this.index = index;
		}

		/// <returns>Value entry index in the cpool</returns>
		public virtual int GetIndex()
		{
			return index;
		}

		public virtual void SetIndex(int index)
		{
			this.index = index;
		}

		public virtual string GetValueString()
		{
			if (base.GetType() != STRING)
			{
				throw new System.Exception("Dont call getValueString() on a non STRING ElementValue"
					);
			}
			NBCEL.classfile.ConstantUtf8 c = (NBCEL.classfile.ConstantUtf8)base.GetConstantPool
				().GetConstant(GetIndex(), NBCEL.Const.CONSTANT_Utf8);
			return c.GetBytes();
		}

		public virtual int GetValueInt()
		{
			if (base.GetType() != PRIMITIVE_INT)
			{
				throw new System.Exception("Dont call getValueString() on a non STRING ElementValue"
					);
			}
			NBCEL.classfile.ConstantInteger c = (NBCEL.classfile.ConstantInteger)base.GetConstantPool
				().GetConstant(GetIndex(), NBCEL.Const.CONSTANT_Integer);
			return c.GetBytes();
		}

		public virtual byte GetValueByte()
		{
			if (base.GetType() != PRIMITIVE_BYTE)
			{
				throw new System.Exception("Dont call getValueByte() on a non BYTE ElementValue");
			}
			NBCEL.classfile.ConstantInteger c = (NBCEL.classfile.ConstantInteger)base.GetConstantPool
				().GetConstant(GetIndex(), NBCEL.Const.CONSTANT_Integer);
			return unchecked((byte)c.GetBytes());
		}

		public virtual char GetValueChar()
		{
			if (base.GetType() != PRIMITIVE_CHAR)
			{
				throw new System.Exception("Dont call getValueChar() on a non CHAR ElementValue");
			}
			NBCEL.classfile.ConstantInteger c = (NBCEL.classfile.ConstantInteger)base.GetConstantPool
				().GetConstant(GetIndex(), NBCEL.Const.CONSTANT_Integer);
			return (char)c.GetBytes();
		}

		public virtual long GetValueLong()
		{
			if (base.GetType() != PRIMITIVE_LONG)
			{
				throw new System.Exception("Dont call getValueLong() on a non LONG ElementValue");
			}
			NBCEL.classfile.ConstantLong j = (NBCEL.classfile.ConstantLong)base.GetConstantPool
				().GetConstant(GetIndex());
			return j.GetBytes();
		}

		public virtual float GetValueFloat()
		{
			if (base.GetType() != PRIMITIVE_FLOAT)
			{
				throw new System.Exception("Dont call getValueFloat() on a non FLOAT ElementValue"
					);
			}
			NBCEL.classfile.ConstantFloat f = (NBCEL.classfile.ConstantFloat)base.GetConstantPool
				().GetConstant(GetIndex());
			return f.GetBytes();
		}

		public virtual double GetValueDouble()
		{
			if (base.GetType() != PRIMITIVE_DOUBLE)
			{
				throw new System.Exception("Dont call getValueDouble() on a non DOUBLE ElementValue"
					);
			}
			NBCEL.classfile.ConstantDouble d = (NBCEL.classfile.ConstantDouble)base.GetConstantPool
				().GetConstant(GetIndex());
			return d.GetBytes();
		}

		public virtual bool GetValueBoolean()
		{
			if (base.GetType() != PRIMITIVE_BOOLEAN)
			{
				throw new System.Exception("Dont call getValueBoolean() on a non BOOLEAN ElementValue"
					);
			}
			NBCEL.classfile.ConstantInteger bo = (NBCEL.classfile.ConstantInteger)base.GetConstantPool
				().GetConstant(GetIndex());
			return bo.GetBytes() != 0;
		}

		public virtual short GetValueShort()
		{
			if (base.GetType() != PRIMITIVE_SHORT)
			{
				throw new System.Exception("Dont call getValueShort() on a non SHORT ElementValue"
					);
			}
			NBCEL.classfile.ConstantInteger s = (NBCEL.classfile.ConstantInteger)base.GetConstantPool
				().GetConstant(GetIndex());
			return (short)s.GetBytes();
		}

		public override string ToString()
		{
			return StringifyValue();
		}

		// Whatever kind of value it is, return it as a string
		public override string StringifyValue()
		{
			NBCEL.classfile.ConstantPool cpool = base.GetConstantPool();
			int _type = base.GetType();
			switch (_type)
			{
				case PRIMITIVE_INT:
				{
					NBCEL.classfile.ConstantInteger c = (NBCEL.classfile.ConstantInteger)cpool.GetConstant
						(GetIndex(), NBCEL.Const.CONSTANT_Integer);
					return c.GetBytes().ToString();
				}

				case PRIMITIVE_LONG:
				{
					NBCEL.classfile.ConstantLong j = (NBCEL.classfile.ConstantLong)cpool.GetConstant(
						GetIndex(), NBCEL.Const.CONSTANT_Long);
					return System.Convert.ToString(j.GetBytes());
				}

				case PRIMITIVE_DOUBLE:
				{
					NBCEL.classfile.ConstantDouble d = (NBCEL.classfile.ConstantDouble)cpool.GetConstant
						(GetIndex(), NBCEL.Const.CONSTANT_Double);
					return System.Convert.ToString(d.GetBytes(), CultureInfo.InvariantCulture);
				}

				case PRIMITIVE_FLOAT:
				{
					NBCEL.classfile.ConstantFloat f = (NBCEL.classfile.ConstantFloat)cpool.GetConstant
						(GetIndex(), NBCEL.Const.CONSTANT_Float);
					return System.Convert.ToString(f.GetBytes(), CultureInfo.InvariantCulture);
				}

				case PRIMITIVE_SHORT:
				{
					NBCEL.classfile.ConstantInteger s = (NBCEL.classfile.ConstantInteger)cpool.GetConstant
						(GetIndex(), NBCEL.Const.CONSTANT_Integer);
					return System.Convert.ToString(s.GetBytes());
				}

				case PRIMITIVE_BYTE:
				{
					NBCEL.classfile.ConstantInteger b = (NBCEL.classfile.ConstantInteger)cpool.GetConstant
						(GetIndex(), NBCEL.Const.CONSTANT_Integer);
					return System.Convert.ToString(b.GetBytes());
				}

				case PRIMITIVE_CHAR:
				{
					NBCEL.classfile.ConstantInteger ch = (NBCEL.classfile.ConstantInteger)cpool.GetConstant
						(GetIndex(), NBCEL.Const.CONSTANT_Integer);
					return ((char) ch.GetBytes()).ToString();
				}

				case PRIMITIVE_BOOLEAN:
				{
					NBCEL.classfile.ConstantInteger bo = (NBCEL.classfile.ConstantInteger)cpool.GetConstant
						(GetIndex(), NBCEL.Const.CONSTANT_Integer);
					if (bo.GetBytes() == 0)
					{
						return "false";
					}
					return "true";
				}

				case STRING:
				{
					NBCEL.classfile.ConstantUtf8 cu8 = (NBCEL.classfile.ConstantUtf8)cpool.GetConstant
						(GetIndex(), NBCEL.Const.CONSTANT_Utf8);
					return cu8.GetBytes();
				}

				default:
				{
					throw new System.Exception("SimpleElementValue class does not know how to stringify type "
						 + _type);
				}
			}
		}

		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream dos)
		{
			int _type = base.GetType();
			dos.WriteByte(_type);
			switch (_type)
			{
				case PRIMITIVE_INT:
				case PRIMITIVE_BYTE:
				case PRIMITIVE_CHAR:
				case PRIMITIVE_FLOAT:
				case PRIMITIVE_LONG:
				case PRIMITIVE_BOOLEAN:
				case PRIMITIVE_SHORT:
				case PRIMITIVE_DOUBLE:
				case STRING:
				{
					// u1 kind of value
					dos.WriteShort(GetIndex());
					break;
				}

				default:
				{
					throw new System.Exception("SimpleElementValue doesnt know how to write out type "
						 + _type);
				}
			}
		}
	}
}
