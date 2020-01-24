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

namespace NBCEL.generic
{
	/// <since>6.0</since>
	public class SimpleElementValueGen : NBCEL.generic.ElementValueGen
	{
		private int idx;

		/// <summary>
		/// Protected ctor used for deserialization, doesn't *put* an entry in the
		/// constant pool, assumes the one at the supplied index is correct.
		/// </summary>
		protected internal SimpleElementValueGen(int type, int idx, NBCEL.generic.ConstantPoolGen
			 cpGen)
			: base(type, cpGen)
		{
			// For primitive types and string type, this points to the value entry in
			// the cpGen
			// For 'class' this points to the class entry in the cpGen
			// ctors for each supported type... type could be inferred but for now lets
			// force it to be passed
			this.idx = idx;
		}

		public SimpleElementValueGen(int type, NBCEL.generic.ConstantPoolGen cpGen, int value
			)
			: base(type, cpGen)
		{
			idx = GetConstantPool().AddInteger(value);
		}

		public SimpleElementValueGen(int type, NBCEL.generic.ConstantPoolGen cpGen, long 
			value)
			: base(type, cpGen)
		{
			idx = GetConstantPool().AddLong(value);
		}

		public SimpleElementValueGen(int type, NBCEL.generic.ConstantPoolGen cpGen, double
			 value)
			: base(type, cpGen)
		{
			idx = GetConstantPool().AddDouble(value);
		}

		public SimpleElementValueGen(int type, NBCEL.generic.ConstantPoolGen cpGen, float
			 value)
			: base(type, cpGen)
		{
			idx = GetConstantPool().AddFloat(value);
		}

		public SimpleElementValueGen(int type, NBCEL.generic.ConstantPoolGen cpGen, short
			 value)
			: base(type, cpGen)
		{
			idx = GetConstantPool().AddInteger(value);
		}

		public SimpleElementValueGen(int type, NBCEL.generic.ConstantPoolGen cpGen, byte 
			value)
			: base(type, cpGen)
		{
			idx = GetConstantPool().AddInteger(value);
		}

		public SimpleElementValueGen(int type, NBCEL.generic.ConstantPoolGen cpGen, char 
			value)
			: base(type, cpGen)
		{
			idx = GetConstantPool().AddInteger(value);
		}

		public SimpleElementValueGen(int type, NBCEL.generic.ConstantPoolGen cpGen, bool 
			value)
			: base(type, cpGen)
		{
			if (value)
			{
				idx = GetConstantPool().AddInteger(1);
			}
			else
			{
				idx = GetConstantPool().AddInteger(0);
			}
		}

		public SimpleElementValueGen(int type, NBCEL.generic.ConstantPoolGen cpGen, string
			 value)
			: base(type, cpGen)
		{
			idx = GetConstantPool().AddUtf8(value);
		}

		/// <summary>
		/// The boolean controls whether we copy info from the 'old' constant pool to
		/// the 'new'.
		/// </summary>
		/// <remarks>
		/// The boolean controls whether we copy info from the 'old' constant pool to
		/// the 'new'. You need to use this ctor if the annotation is being copied
		/// from one file to another.
		/// </remarks>
		public SimpleElementValueGen(NBCEL.classfile.SimpleElementValue value, NBCEL.generic.ConstantPoolGen
			 cpool, bool copyPoolEntries)
			: base(value.GetElementValueType(), cpool)
		{
			if (!copyPoolEntries)
			{
				// J5ASSERT: Could assert value.stringifyValue() is the same as
				// cpool.getConstant(SimpleElementValuevalue.getIndex())
				idx = value.GetIndex();
			}
			else
			{
				switch (value.GetElementValueType())
				{
					case STRING:
					{
						idx = cpool.AddUtf8(value.GetValueString());
						break;
					}

					case PRIMITIVE_INT:
					{
						idx = cpool.AddInteger(value.GetValueInt());
						break;
					}

					case PRIMITIVE_BYTE:
					{
						idx = cpool.AddInteger(value.GetValueByte());
						break;
					}

					case PRIMITIVE_CHAR:
					{
						idx = cpool.AddInteger(value.GetValueChar());
						break;
					}

					case PRIMITIVE_LONG:
					{
						idx = cpool.AddLong(value.GetValueLong());
						break;
					}

					case PRIMITIVE_FLOAT:
					{
						idx = cpool.AddFloat(value.GetValueFloat());
						break;
					}

					case PRIMITIVE_DOUBLE:
					{
						idx = cpool.AddDouble(value.GetValueDouble());
						break;
					}

					case PRIMITIVE_BOOLEAN:
					{
						if (value.GetValueBoolean())
						{
							idx = cpool.AddInteger(1);
						}
						else
						{
							idx = cpool.AddInteger(0);
						}
						break;
					}

					case PRIMITIVE_SHORT:
					{
						idx = cpool.AddInteger(value.GetValueShort());
						break;
					}

					default:
					{
						throw new System.Exception("SimpleElementValueGen class does not know how to copy this type "
							 + base.GetElementValueType());
					}
				}
			}
		}

		/// <summary>Return immutable variant</summary>
		public override NBCEL.classfile.ElementValue GetElementValue()
		{
			return new NBCEL.classfile.SimpleElementValue(base.GetElementValueType(), idx, GetConstantPool
				().GetConstantPool());
		}

		public virtual int GetIndex()
		{
			return idx;
		}

		public virtual string GetValueString()
		{
			if (base.GetElementValueType() != STRING)
			{
				throw new System.Exception("Dont call getValueString() on a non STRING ElementValue"
					);
			}
			NBCEL.classfile.ConstantUtf8 c = (NBCEL.classfile.ConstantUtf8)GetConstantPool().
				GetConstant(idx);
			return c.GetBytes();
		}

		public virtual int GetValueInt()
		{
			if (base.GetElementValueType() != PRIMITIVE_INT)
			{
				throw new System.Exception("Dont call getValueString() on a non STRING ElementValue"
					);
			}
			NBCEL.classfile.ConstantInteger c = (NBCEL.classfile.ConstantInteger)GetConstantPool
				().GetConstant(idx);
			return c.GetBytes();
		}

		// Whatever kind of value it is, return it as a string
		public override string StringifyValue()
		{
			switch (base.GetElementValueType())
			{
				case PRIMITIVE_INT:
				{
					NBCEL.classfile.ConstantInteger c = (NBCEL.classfile.ConstantInteger)GetConstantPool
						().GetConstant(idx);
					return c.GetBytes().ToString();
				}

				case PRIMITIVE_LONG:
				{
					NBCEL.classfile.ConstantLong j = (NBCEL.classfile.ConstantLong)GetConstantPool().
						GetConstant(idx);
					return System.Convert.ToString(j.GetBytes());
				}

				case PRIMITIVE_DOUBLE:
				{
					NBCEL.classfile.ConstantDouble d = (NBCEL.classfile.ConstantDouble)GetConstantPool
						().GetConstant(idx);
					return System.Convert.ToString(d.GetBytes(), CultureInfo.InvariantCulture);
				}

				case PRIMITIVE_FLOAT:
				{
					NBCEL.classfile.ConstantFloat f = (NBCEL.classfile.ConstantFloat)GetConstantPool(
						).GetConstant(idx);
					return System.Convert.ToString(f.GetBytes(), CultureInfo.InvariantCulture);
				}

				case PRIMITIVE_SHORT:
				{
					NBCEL.classfile.ConstantInteger s = (NBCEL.classfile.ConstantInteger)GetConstantPool
						().GetConstant(idx);
					return System.Convert.ToString(s.GetBytes());
				}

				case PRIMITIVE_BYTE:
				{
					NBCEL.classfile.ConstantInteger b = (NBCEL.classfile.ConstantInteger)GetConstantPool
						().GetConstant(idx);
					return System.Convert.ToString(b.GetBytes());
				}

				case PRIMITIVE_CHAR:
				{
					NBCEL.classfile.ConstantInteger ch = (NBCEL.classfile.ConstantInteger)GetConstantPool
						().GetConstant(idx);
					return System.Convert.ToString(ch.GetBytes());
				}

				case PRIMITIVE_BOOLEAN:
				{
					NBCEL.classfile.ConstantInteger bo = (NBCEL.classfile.ConstantInteger)GetConstantPool
						().GetConstant(idx);
					if (bo.GetBytes() == 0)
					{
						return "false";
					}
					return "true";
				}

				case STRING:
				{
					NBCEL.classfile.ConstantUtf8 cu8 = (NBCEL.classfile.ConstantUtf8)GetConstantPool(
						).GetConstant(idx);
					return cu8.GetBytes();
				}

				default:
				{
					throw new System.Exception("SimpleElementValueGen class does not know how to stringify type "
						 + base.GetElementValueType());
				}
			}
		}

		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream dos)
		{
			dos.WriteByte(base.GetElementValueType());
			switch (base.GetElementValueType())
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
					dos.WriteShort(idx);
					break;
				}

				default:
				{
					throw new System.Exception("SimpleElementValueGen doesnt know how to write out type "
						 + base.GetElementValueType());
				}
			}
		}
	}
}
