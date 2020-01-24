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

using System;
using Sharpen;

namespace NBCEL.generic
{
	/// <summary>Template class for building up a field.</summary>
	/// <remarks>
	/// Template class for building up a field.  The only extraordinary thing
	/// one can do is to add a constant value attribute to a field (which must of
	/// course be compatible with to the declared type).
	/// </remarks>
	/// <seealso cref="NBCEL.classfile.Field"/>
	public class FieldGen : NBCEL.generic.FieldGenOrMethodGen
	{
		private object value = null;

		private sealed class _BCELComparator_46 : NBCEL.util.BCELComparator
		{
			public _BCELComparator_46()
			{
			}

			public bool Equals(object o1, object o2)
			{
				NBCEL.generic.FieldGen THIS = (NBCEL.generic.FieldGen)o1;
				NBCEL.generic.FieldGen THAT = (NBCEL.generic.FieldGen)o2;
				return Sharpen.System.Equals(THIS.GetName(), THAT.GetName()) && Sharpen.System
					.Equals(THIS.GetSignature(), THAT.GetSignature());
			}

			public int HashCode(object o)
			{
				NBCEL.generic.FieldGen THIS = (NBCEL.generic.FieldGen)o;
				return THIS.GetSignature().GetHashCode() ^ THIS.GetName().GetHashCode();
			}
		}

		private static NBCEL.util.BCELComparator bcelComparator = new _BCELComparator_46(
			);

		/// <summary>Declare a field.</summary>
		/// <remarks>
		/// Declare a field. If it is static (isStatic() == true) and has a
		/// basic type like int or String it may have an initial value
		/// associated with it as defined by setInitValue().
		/// </remarks>
		/// <param name="access_flags">access qualifiers</param>
		/// <param name="type">field type</param>
		/// <param name="name">field name</param>
		/// <param name="cp">constant pool</param>
		public FieldGen(int access_flags, NBCEL.generic.Type type, string name, NBCEL.generic.ConstantPoolGen
			 cp)
			: base(access_flags)
		{
			SetType(type);
			SetName(name);
			SetConstantPool(cp);
		}

		/// <summary>Instantiate from existing field.</summary>
		/// <param name="field">Field object</param>
		/// <param name="cp">constant pool (must contain the same entries as the field's constant pool)
		/// 	</param>
		public FieldGen(NBCEL.classfile.Field field, NBCEL.generic.ConstantPoolGen cp)
			: this(field.GetAccessFlags(), NBCEL.generic.Type.GetType(field.GetSignature()), 
				field.GetName(), cp)
		{
			NBCEL.classfile.Attribute[] attrs = field.GetAttributes();
			foreach (NBCEL.classfile.Attribute attr in attrs)
			{
				if (attr is NBCEL.classfile.ConstantValue)
				{
					SetValue(((NBCEL.classfile.ConstantValue)attr).GetConstantValueIndex());
				}
				else if (attr is NBCEL.classfile.Annotations)
				{
					NBCEL.classfile.Annotations runtimeAnnotations = (NBCEL.classfile.Annotations)attr;
					NBCEL.classfile.AnnotationEntry[] annotationEntries = runtimeAnnotations.GetAnnotationEntries
						();
					foreach (NBCEL.classfile.AnnotationEntry element in annotationEntries)
					{
						AddAnnotationEntry(new NBCEL.generic.AnnotationEntryGen(element, cp, false));
					}
				}
				else
				{
					AddAttribute(attr);
				}
			}
		}

		private void SetValue(int index)
		{
			NBCEL.classfile.ConstantPool cp = base.GetConstantPool().GetConstantPool();
			NBCEL.classfile.Constant c = cp.GetConstant(index);
			value = ((NBCEL.classfile.ConstantObject)c).GetConstantValue(cp);
		}

		/// <summary>
		/// Set (optional) initial value of field, otherwise it will be set to null/0/false
		/// by the JVM automatically.
		/// </summary>
		public virtual void SetInitValue(string str)
		{
			CheckType(NBCEL.generic.ObjectType.GetInstance("java.lang.String"));
			if (str != null)
			{
				value = str;
			}
		}

		public virtual void SetInitValue(long l)
		{
			CheckType(NBCEL.generic.Type.LONG);
			if (l != 0L)
			{
				value = l;
			}
		}

		public virtual void SetInitValue(int i)
		{
			CheckType(NBCEL.generic.Type.INT);
			if (i != 0)
			{
				value = i;
			}
		}

		public virtual void SetInitValue(short s)
		{
			CheckType(NBCEL.generic.Type.SHORT);
			if (s != 0)
			{
				value = s;
			}
		}

		public virtual void SetInitValue(char c)
		{
			CheckType(NBCEL.generic.Type.CHAR);
			if (c != 0)
			{
				value = c;
			}
		}

		public virtual void SetInitValue(byte b)
		{
			CheckType(NBCEL.generic.Type.BYTE);
			if (b != 0)
			{
				value = b;
			}
		}

		public virtual void SetInitValue(bool b)
		{
			CheckType(NBCEL.generic.Type.BOOLEAN);
			if (b)
			{
				value = 1;
			}
		}

		public virtual void SetInitValue(float f)
		{
			CheckType(NBCEL.generic.Type.FLOAT);
			if (Math.Abs(f) > float.Epsilon)
			{
				value = f;
			}
		}

		public virtual void SetInitValue(double d)
		{
			CheckType(NBCEL.generic.Type.DOUBLE);
			if (Math.Abs(d) > float.Epsilon)
			{
				value = d;
			}
		}

		/// <summary>Remove any initial value.</summary>
		public virtual void CancelInitValue()
		{
			value = null;
		}

		private void CheckType(NBCEL.generic.Type atype)
		{
			NBCEL.generic.Type superType = base.GetType();
			if (superType == null)
			{
				throw new NBCEL.generic.ClassGenException("You haven't defined the type of the field yet"
					);
			}
			if (!IsFinal())
			{
				throw new NBCEL.generic.ClassGenException("Only final fields may have an initial value!"
					);
			}
			if (!superType.Equals(atype))
			{
				throw new NBCEL.generic.ClassGenException("Types are not compatible: " + superType
					 + " vs. " + atype);
			}
		}

		/// <summary>Get field object after having set up all necessary values.</summary>
		public virtual NBCEL.classfile.Field GetField()
		{
			string signature = GetSignature();
			int name_index = base.GetConstantPool().AddUtf8(base.GetName());
			int signature_index = base.GetConstantPool().AddUtf8(signature);
			if (value != null)
			{
				CheckType(base.GetType());
				int index = AddConstant();
				AddAttribute(new NBCEL.classfile.ConstantValue(base.GetConstantPool().AddUtf8("ConstantValue"
					), 2, index, base.GetConstantPool().GetConstantPool()));
			}
			// sic
			AddAnnotationsAsAttribute(base.GetConstantPool());
			return new NBCEL.classfile.Field(base.GetAccessFlags(), name_index, signature_index
				, GetAttributes(), base.GetConstantPool().GetConstantPool());
		}

		// sic
		private void AddAnnotationsAsAttribute(NBCEL.generic.ConstantPoolGen cp)
		{
			NBCEL.classfile.Attribute[] attrs = NBCEL.generic.AnnotationEntryGen.GetAnnotationAttributes
				(cp, base.GetAnnotationEntries());
			foreach (NBCEL.classfile.Attribute attr in attrs)
			{
				AddAttribute(attr);
			}
		}

		private int AddConstant()
		{
			switch (base.GetType().GetType())
			{
				case NBCEL.Const.T_INT:
				case NBCEL.Const.T_CHAR:
				case NBCEL.Const.T_BYTE:
				case NBCEL.Const.T_BOOLEAN:
				case NBCEL.Const.T_SHORT:
				{
					// sic
					return base.GetConstantPool().AddInteger(((int)value));
				}

				case NBCEL.Const.T_FLOAT:
				{
					return base.GetConstantPool().AddFloat(((float)value));
				}

				case NBCEL.Const.T_DOUBLE:
				{
					return base.GetConstantPool().AddDouble(((double)value));
				}

				case NBCEL.Const.T_LONG:
				{
					return base.GetConstantPool().AddLong(((long)value));
				}

				case NBCEL.Const.T_REFERENCE:
				{
					return base.GetConstantPool().AddString((string)value);
				}

				default:
				{
					throw new System.Exception("Oops: Unhandled : " + base.GetType().GetType());
				}
			}
		}

		// sic
		public override string GetSignature()
		{
			return base.GetType().GetSignature();
		}

		private System.Collections.Generic.List<NBCEL.generic.FieldObserver> observers;

		/// <summary>Add observer for this object.</summary>
		public virtual void AddObserver(NBCEL.generic.FieldObserver o)
		{
			if (observers == null)
			{
				observers = new System.Collections.Generic.List<NBCEL.generic.FieldObserver>();
			}
			observers.Add(o);
		}

		/// <summary>Remove observer for this object.</summary>
		public virtual void RemoveObserver(NBCEL.generic.FieldObserver o)
		{
			if (observers != null)
			{
				observers.Remove(o);
			}
		}

		/// <summary>Call notify() method on all observers.</summary>
		/// <remarks>
		/// Call notify() method on all observers. This method is not called
		/// automatically whenever the state has changed, but has to be
		/// called by the user after he has finished editing the object.
		/// </remarks>
		public virtual void Update()
		{
			if (observers != null)
			{
				foreach (NBCEL.generic.FieldObserver observer in observers)
				{
					observer.Notify(this);
				}
			}
		}

		public virtual string GetInitValue()
		{
			if (value != null)
			{
				return value.ToString();
			}
			return null;
		}

		/// <summary>
		/// Return string representation close to declaration format,
		/// `public static final short MAX = 100', e.g..
		/// </summary>
		/// <returns>String representation of field</returns>
		public sealed override string ToString()
		{
			string name;
			string signature;
			string access;
			// Short cuts to constant pool
			access = NBCEL.classfile.Utility.AccessToString(base.GetAccessFlags());
			access = (access.Length == 0) ? string.Empty : (access + " ");
			signature = base.GetType().ToString();
			name = GetName();
			System.Text.StringBuilder buf = new System.Text.StringBuilder(32);
			// CHECKSTYLE IGNORE MagicNumber
			buf.Append(access).Append(signature).Append(" ").Append(name);
			string value = GetInitValue();
			if (value != null)
			{
				buf.Append(" = ").Append(value);
			}
			return buf.ToString();
		}

		/// <returns>deep copy of this field</returns>
		public virtual NBCEL.generic.FieldGen Copy(NBCEL.generic.ConstantPoolGen cp)
		{
			NBCEL.generic.FieldGen fg = (NBCEL.generic.FieldGen)Clone();
			fg.SetConstantPool(cp);
			return fg;
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
		/// By default two FieldGen objects are said to be equal when
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
