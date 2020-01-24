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
	/// <summary>
	/// This class represents the field info structure, i.e., the representation
	/// for a variable in the class.
	/// </summary>
	/// <remarks>
	/// This class represents the field info structure, i.e., the representation
	/// for a variable in the class. See JVM specification for details.
	/// </remarks>
	public sealed class Field : NBCEL.classfile.FieldOrMethod
	{
		private sealed class _BCELComparator_35 : NBCEL.util.BCELComparator
		{
			public _BCELComparator_35()
			{
			}

			public bool Equals(object o1, object o2)
			{
				NBCEL.classfile.Field THIS = (NBCEL.classfile.Field)o1;
				NBCEL.classfile.Field THAT = (NBCEL.classfile.Field)o2;
				return Sharpen.System.Equals(THIS.GetName(), THAT.GetName()) && Sharpen.System
					.Equals(THIS.GetSignature(), THAT.GetSignature());
			}

			public int HashCode(object o)
			{
				NBCEL.classfile.Field THIS = (NBCEL.classfile.Field)o;
				return THIS.GetSignature().GetHashCode() ^ THIS.GetName().GetHashCode();
			}
		}

		private static NBCEL.util.BCELComparator bcelComparator = new _BCELComparator_35(
			);

		/// <summary>Initialize from another object.</summary>
		/// <remarks>
		/// Initialize from another object. Note that both objects use the same
		/// references (shallow copy). Use clone() for a physical copy.
		/// </remarks>
		public Field(NBCEL.classfile.Field c)
			: base(c)
		{
		}

		/// <summary>Construct object from file stream.</summary>
		/// <param name="file">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		internal Field(java.io.DataInput file, NBCEL.classfile.ConstantPool constant_pool
			)
			: base(file, constant_pool)
		{
		}

		/// <param name="access_flags">Access rights of field</param>
		/// <param name="name_index">Points to field name in constant pool</param>
		/// <param name="signature_index">Points to encoded signature</param>
		/// <param name="attributes">Collection of attributes</param>
		/// <param name="constant_pool">Array of constants</param>
		public Field(int access_flags, int name_index, int signature_index, NBCEL.classfile.Attribute
			[] attributes, NBCEL.classfile.ConstantPool constant_pool)
			: base(access_flags, name_index, signature_index, attributes, constant_pool)
		{
		}

		/// <summary>
		/// Called by objects that are traversing the nodes of the tree implicitely
		/// defined by the contents of a Java class.
		/// </summary>
		/// <remarks>
		/// Called by objects that are traversing the nodes of the tree implicitely
		/// defined by the contents of a Java class. I.e., the hierarchy of methods,
		/// fields, attributes, etc. spawns a tree of objects.
		/// </remarks>
		/// <param name="v">Visitor object</param>
		public override void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitField(this);
		}

		/// <returns>constant value associated with this field (may be null)</returns>
		public NBCEL.classfile.ConstantValue GetConstantValue()
		{
			foreach (NBCEL.classfile.Attribute attribute in base.GetAttributes())
			{
				if (attribute.GetTag() == NBCEL.Const.ATTR_CONSTANT_VALUE)
				{
					return (NBCEL.classfile.ConstantValue)attribute;
				}
			}
			return null;
		}

		/// <summary>
		/// Return string representation close to declaration format,
		/// `public static final short MAX = 100', e.g..
		/// </summary>
		/// <returns>String representation of field, including the signature.</returns>
		public override string ToString()
		{
			string name;
			string signature;
			string access;
			// Short cuts to constant pool
			// Get names from constant pool
			access = NBCEL.classfile.Utility.AccessToString(base.GetAccessFlags());
			access = (access.Length == 0) ? string.Empty : (access + " ");
			signature = NBCEL.classfile.Utility.SignatureToString(GetSignature());
			name = GetName();
			System.Text.StringBuilder buf = new System.Text.StringBuilder(64);
			// CHECKSTYLE IGNORE MagicNumber
			buf.Append(access).Append(signature).Append(" ").Append(name);
			NBCEL.classfile.ConstantValue cv = GetConstantValue();
			if (cv != null)
			{
				buf.Append(" = ").Append(cv);
			}
			foreach (NBCEL.classfile.Attribute attribute in base.GetAttributes())
			{
				if (!(attribute is NBCEL.classfile.ConstantValue))
				{
					buf.Append(" [").Append(attribute).Append("]");
				}
			}
			return buf.ToString();
		}

		/// <returns>deep copy of this field</returns>
		public NBCEL.classfile.Field Copy(NBCEL.classfile.ConstantPool _constant_pool)
		{
			return (NBCEL.classfile.Field)Copy_(_constant_pool);
		}

		/// <returns>type of field</returns>
		public NBCEL.generic.Type GetType()
		{
			return NBCEL.generic.Type.GetReturnType(GetSignature());
		}

		/// <returns>Comparison strategy object</returns>
		public static NBCEL.util.BCELComparator GetComparator()
		{
			return bcelComparator;
		}

		/// <param name="comparator">Comparison strategy object</param>
		public static void SetComparator(NBCEL.util.BCELComparator comparator)
		{
			bcelComparator = comparator;
		}

		/// <summary>Return value as defined by given BCELComparator strategy.</summary>
		/// <remarks>
		/// Return value as defined by given BCELComparator strategy.
		/// By default two Field objects are said to be equal when
		/// their names and signatures are equal.
		/// </remarks>
		/// <seealso cref="object.Equals(object)"/>
		public override bool Equals(object obj)
		{
			return bcelComparator.Equals(this, obj);
		}

		/// <summary>Return value as defined by given BCELComparator strategy.</summary>
		/// <remarks>
		/// Return value as defined by given BCELComparator strategy.
		/// By default return the hashcode of the field's name XOR signature.
		/// </remarks>
		/// <seealso cref="object.GetHashCode()"/>
		public override int GetHashCode()
		{
			return bcelComparator.HashCode(this);
		}
	}
}
