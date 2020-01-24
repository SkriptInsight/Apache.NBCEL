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
	public abstract class ElementValueGen
	{
		[System.ObsoleteAttribute(@"(since 6.0) will be made private and final; do not access directly, use getter"
			)]
		protected internal int type;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private and final; do not access directly, use getter"
			)]
		protected internal NBCEL.generic.ConstantPoolGen cpGen;

		protected internal ElementValueGen(int type, NBCEL.generic.ConstantPoolGen cpGen)
		{
			this.type = type;
			this.cpGen = cpGen;
		}

		/// <summary>Subtypes return an immutable variant of the ElementValueGen</summary>
		public abstract NBCEL.classfile.ElementValue GetElementValue();

		public virtual int GetElementValueType()
		{
			return type;
		}

		public abstract string StringifyValue();

		/// <exception cref="System.IO.IOException"/>
		public abstract void Dump(java.io.DataOutputStream dos);

		public const int STRING = 's';

		public const int ENUM_CONSTANT = 'e';

		public const int CLASS = 'c';

		public const int ANNOTATION = '@';

		public const int ARRAY = '[';

		public const int PRIMITIVE_INT = 'I';

		public const int PRIMITIVE_BYTE = 'B';

		public const int PRIMITIVE_CHAR = 'C';

		public const int PRIMITIVE_DOUBLE = 'D';

		public const int PRIMITIVE_FLOAT = 'F';

		public const int PRIMITIVE_LONG = 'J';

		public const int PRIMITIVE_SHORT = 'S';

		public const int PRIMITIVE_BOOLEAN = 'Z';

		/// <exception cref="System.IO.IOException"/>
		public static NBCEL.generic.ElementValueGen ReadElementValue(java.io.DataInput dis
			, NBCEL.generic.ConstantPoolGen cpGen)
		{
			int type = dis.ReadUnsignedByte();
			switch (type)
			{
				case 'B':
				{
					// byte
					return new NBCEL.generic.SimpleElementValueGen(PRIMITIVE_BYTE, dis.ReadUnsignedShort
						(), cpGen);
				}

				case 'C':
				{
					// char
					return new NBCEL.generic.SimpleElementValueGen(PRIMITIVE_CHAR, dis.ReadUnsignedShort
						(), cpGen);
				}

				case 'D':
				{
					// double
					return new NBCEL.generic.SimpleElementValueGen(PRIMITIVE_DOUBLE, dis.ReadUnsignedShort
						(), cpGen);
				}

				case 'F':
				{
					// float
					return new NBCEL.generic.SimpleElementValueGen(PRIMITIVE_FLOAT, dis.ReadUnsignedShort
						(), cpGen);
				}

				case 'I':
				{
					// int
					return new NBCEL.generic.SimpleElementValueGen(PRIMITIVE_INT, dis.ReadUnsignedShort
						(), cpGen);
				}

				case 'J':
				{
					// long
					return new NBCEL.generic.SimpleElementValueGen(PRIMITIVE_LONG, dis.ReadUnsignedShort
						(), cpGen);
				}

				case 'S':
				{
					// short
					return new NBCEL.generic.SimpleElementValueGen(PRIMITIVE_SHORT, dis.ReadUnsignedShort
						(), cpGen);
				}

				case 'Z':
				{
					// boolean
					return new NBCEL.generic.SimpleElementValueGen(PRIMITIVE_BOOLEAN, dis.ReadUnsignedShort
						(), cpGen);
				}

				case 's':
				{
					// String
					return new NBCEL.generic.SimpleElementValueGen(STRING, dis.ReadUnsignedShort(), cpGen
						);
				}

				case 'e':
				{
					// Enum constant
					return new NBCEL.generic.EnumElementValueGen(dis.ReadUnsignedShort(), dis.ReadUnsignedShort
						(), cpGen);
				}

				case 'c':
				{
					// Class
					return new NBCEL.generic.ClassElementValueGen(dis.ReadUnsignedShort(), cpGen);
				}

				case '@':
				{
					// Annotation
					// TODO: isRuntimeVisible ??????????
					// FIXME
					return new NBCEL.generic.AnnotationElementValueGen(ANNOTATION, new NBCEL.generic.AnnotationEntryGen
						(NBCEL.classfile.AnnotationEntry.Read(dis, cpGen.GetConstantPool(), true), cpGen
						, false), cpGen);
				}

				case '[':
				{
					// Array
					int numArrayVals = dis.ReadUnsignedShort();
					NBCEL.classfile.ElementValue[] evalues = new NBCEL.classfile.ElementValue[numArrayVals
						];
					for (int j = 0; j < numArrayVals; j++)
					{
						evalues[j] = NBCEL.classfile.ElementValue.ReadElementValue(dis, cpGen.GetConstantPool
							());
					}
					return new NBCEL.generic.ArrayElementValueGen(ARRAY, evalues, cpGen);
				}

				default:
				{
					throw new System.Exception("Unexpected element value kind in annotation: " + type
						);
				}
			}
		}

		protected internal virtual NBCEL.generic.ConstantPoolGen GetConstantPool()
		{
			return cpGen;
		}

		/// <summary>
		/// Creates an (modifiable) ElementValueGen copy of an (immutable)
		/// ElementValue - constant pool is assumed correct.
		/// </summary>
		public static NBCEL.generic.ElementValueGen Copy(NBCEL.classfile.ElementValue value
			, NBCEL.generic.ConstantPoolGen cpool, bool copyPoolEntries)
		{
			switch (value.GetElementValueType())
			{
				case 'B':
				case 'C':
				case 'D':
				case 'F':
				case 'I':
				case 'J':
				case 'S':
				case 'Z':
				case 's':
				{
					// byte
					// char
					// double
					// float
					// int
					// long
					// short
					// boolean
					// String
					return new NBCEL.generic.SimpleElementValueGen((NBCEL.classfile.SimpleElementValue
						)value, cpool, copyPoolEntries);
				}

				case 'e':
				{
					// Enum constant
					return new NBCEL.generic.EnumElementValueGen((NBCEL.classfile.EnumElementValue)value
						, cpool, copyPoolEntries);
				}

				case '@':
				{
					// Annotation
					return new NBCEL.generic.AnnotationElementValueGen((NBCEL.classfile.AnnotationElementValue
						)value, cpool, copyPoolEntries);
				}

				case '[':
				{
					// Array
					return new NBCEL.generic.ArrayElementValueGen((NBCEL.classfile.ArrayElementValue)
						value, cpool, copyPoolEntries);
				}

				case 'c':
				{
					// Class
					return new NBCEL.generic.ClassElementValueGen((NBCEL.classfile.ClassElementValue)
						value, cpool, copyPoolEntries);
				}

				default:
				{
					throw new System.Exception("Not implemented yet! (" + value.GetElementValueType()
						 + ")");
				}
			}
		}
	}
}
