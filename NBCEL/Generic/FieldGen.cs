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
using System.Collections.Generic;
using System.Text;
using Apache.NBCEL.ClassFile;
using Apache.NBCEL.Util;

namespace Apache.NBCEL.Generic
{
	/// <summary>Template class for building up a field.</summary>
	/// <remarks>
	///     Template class for building up a field.  The only extraordinary thing
	///     one can do is to add a constant value attribute to a field (which must of
	///     course be compatible with to the declared type).
	/// </remarks>
	/// <seealso cref="Field" />
	public class FieldGen : FieldGenOrMethodGen
    {
        private static BCELComparator bcelComparator = new _BCELComparator_46(
        );

        private List<FieldObserver> observers;
        private object value;

        /// <summary>Declare a field.</summary>
        /// <remarks>
        ///     Declare a field. If it is static (isStatic() == true) and has a
        ///     basic type like int or String it may have an initial value
        ///     associated with it as defined by setInitValue().
        /// </remarks>
        /// <param name="access_flags">access qualifiers</param>
        /// <param name="type">field type</param>
        /// <param name="name">field name</param>
        /// <param name="cp">constant pool</param>
        public FieldGen(int access_flags, Type type, string name, ConstantPoolGen
            cp)
            : base(access_flags)
        {
            SetType(type);
            SetName(name);
            SetConstantPool(cp);
        }

        /// <summary>Instantiate from existing field.</summary>
        /// <param name="field">Field object</param>
        /// <param name="cp">
        ///     constant pool (must contain the same entries as the field's constant pool)
        /// </param>
        public FieldGen(Field field, ConstantPoolGen cp)
            : this(field.GetAccessFlags(), Type.GetType(field.GetSignature()),
                field.GetName(), cp)
        {
            var attrs = field.GetAttributes();
            foreach (var attr in attrs)
                if (attr is ConstantValue)
                {
                    SetValue(((ConstantValue) attr).GetConstantValueIndex());
                }
                else if (attr is Annotations)
                {
                    var runtimeAnnotations = (Annotations) attr;
                    var annotationEntries = runtimeAnnotations.GetAnnotationEntries
                        ();
                    foreach (var element in annotationEntries)
                        AddAnnotationEntry(new AnnotationEntryGen(element, cp, false));
                }
                else
                {
                    AddAttribute(attr);
                }
        }

        private void SetValue(int index)
        {
            var cp = base.GetConstantPool().GetConstantPool();
            var c = cp.GetConstant(index);
            value = ((ConstantObject) c).GetConstantValue(cp);
        }

        /// <summary>
        ///     Set (optional) initial value of field, otherwise it will be set to null/0/false
        ///     by the JVM automatically.
        /// </summary>
        public virtual void SetInitValue(string str)
        {
            CheckType(ObjectType.GetInstance("java.lang.String"));
            if (str != null) value = str;
        }

        public virtual void SetInitValue(long l)
        {
            CheckType(Type.LONG);
            if (l != 0L) value = l;
        }

        public virtual void SetInitValue(int i)
        {
            CheckType(Type.INT);
            if (i != 0) value = i;
        }

        public virtual void SetInitValue(short s)
        {
            CheckType(Type.SHORT);
            if (s != 0) value = s;
        }

        public virtual void SetInitValue(char c)
        {
            CheckType(Type.CHAR);
            if (c != 0) value = c;
        }

        public virtual void SetInitValue(byte b)
        {
            CheckType(Type.BYTE);
            if (b != 0) value = b;
        }

        public virtual void SetInitValue(bool b)
        {
            CheckType(Type.BOOLEAN);
            if (b) value = 1;
        }

        public virtual void SetInitValue(float f)
        {
            CheckType(Type.FLOAT);
            if (Math.Abs(f) > float.Epsilon) value = f;
        }

        public virtual void SetInitValue(double d)
        {
            CheckType(Type.DOUBLE);
            if (Math.Abs(d) > float.Epsilon) value = d;
        }

        /// <summary>Remove any initial value.</summary>
        public virtual void CancelInitValue()
        {
            value = null;
        }

        private void CheckType(Type atype)
        {
            var superType = base.GetType();
            if (superType == null)
                throw new ClassGenException("You haven't defined the type of the field yet"
                );
            if (!IsFinal())
                throw new ClassGenException("Only final fields may have an initial value!"
                );
            if (!superType.Equals(atype))
                throw new ClassGenException("Types are not compatible: " + superType
                                                                         + " vs. " + atype);
        }

        /// <summary>Get field object after having set up all necessary values.</summary>
        public virtual Field GetField()
        {
            var signature = GetSignature();
            var name_index = base.GetConstantPool().AddUtf8(base.GetName());
            var signature_index = base.GetConstantPool().AddUtf8(signature);
            if (value != null)
            {
                CheckType(base.GetType());
                var index = AddConstant();
                AddAttribute(new ConstantValue(base.GetConstantPool().AddUtf8("ConstantValue"
                ), 2, index, base.GetConstantPool().GetConstantPool()));
            }

            // sic
            AddAnnotationsAsAttribute(base.GetConstantPool());
            return new Field(GetAccessFlags(), name_index, signature_index
                , GetAttributes(), base.GetConstantPool().GetConstantPool());
        }

        // sic
        private void AddAnnotationsAsAttribute(ConstantPoolGen cp)
        {
            var attrs = AnnotationEntryGen.GetAnnotationAttributes
                (cp, base.GetAnnotationEntries());
            foreach (var attr in attrs) AddAttribute(attr);
        }

        private int AddConstant()
        {
            switch (base.GetType().GetType())
            {
                case Const.T_INT:
                case Const.T_CHAR:
                case Const.T_BYTE:
                case Const.T_BOOLEAN:
                case Const.T_SHORT:
                {
                    // sic
                    return base.GetConstantPool().AddInteger((int) value);
                }

                case Const.T_FLOAT:
                {
                    return base.GetConstantPool().AddFloat((float) value);
                }

                case Const.T_DOUBLE:
                {
                    return base.GetConstantPool().AddDouble((double) value);
                }

                case Const.T_LONG:
                {
                    return base.GetConstantPool().AddLong((long) value);
                }

                case Const.T_REFERENCE:
                {
                    return base.GetConstantPool().AddString((string) value);
                }

                default:
                {
                    throw new Exception("Oops: Unhandled : " + base.GetType().GetType());
                }
            }
        }

        // sic
        public override string GetSignature()
        {
            return base.GetType().GetSignature();
        }

        /// <summary>Add observer for this object.</summary>
        public virtual void AddObserver(FieldObserver o)
        {
            if (observers == null) observers = new List<FieldObserver>();
            observers.Add(o);
        }

        /// <summary>Remove observer for this object.</summary>
        public virtual void RemoveObserver(FieldObserver o)
        {
            if (observers != null) observers.Remove(o);
        }

        /// <summary>Call notify() method on all observers.</summary>
        /// <remarks>
        ///     Call notify() method on all observers. This method is not called
        ///     automatically whenever the state has changed, but has to be
        ///     called by the user after he has finished editing the object.
        /// </remarks>
        public virtual void Update()
        {
            if (observers != null)
                foreach (var observer in observers)
                    observer.Notify(this);
        }

        public virtual string GetInitValue()
        {
            if (value != null) return value.ToString();
            return null;
        }

        /// <summary>
        ///     Return string representation close to declaration format,
        ///     `public static final short MAX = 100', e.g..
        /// </summary>
        /// <returns>String representation of field</returns>
        public sealed override string ToString()
        {
            string name;
            string signature;
            string access;
            // Short cuts to constant pool
            access = Utility.AccessToString(GetAccessFlags());
            access = access.Length == 0 ? string.Empty : access + " ";
            signature = base.GetType().ToString();
            name = GetName();
            var buf = new StringBuilder(32);
            // CHECKSTYLE IGNORE MagicNumber
            buf.Append(access).Append(signature).Append(" ").Append(name);
            var value = GetInitValue();
            if (value != null) buf.Append(" = ").Append(value);
            return buf.ToString();
        }

        /// <returns>deep copy of this field</returns>
        public virtual FieldGen Copy(ConstantPoolGen cp)
        {
            var fg = (FieldGen) Clone();
            fg.SetConstantPool(cp);
            return fg;
        }

        /// <returns>Comparison strategy object</returns>
        public static BCELComparator GetComparator()
        {
            return bcelComparator;
        }

        /// <param name="comparator">Comparison strategy object</param>
        public static void SetComparator(BCELComparator comparator)
        {
            bcelComparator = comparator;
        }

        /// <summary>Return value as defined by given BCELComparator strategy.</summary>
        /// <remarks>
        ///     Return value as defined by given BCELComparator strategy.
        ///     By default two FieldGen objects are said to be equal when
        ///     their names and signatures are equal.
        /// </remarks>
        /// <seealso cref="object.Equals(object)" />
        public override bool Equals(object obj)
        {
            return bcelComparator.Equals(this, obj);
        }

        /// <summary>Return value as defined by given BCELComparator strategy.</summary>
        /// <remarks>
        ///     Return value as defined by given BCELComparator strategy.
        ///     By default return the hashcode of the field's name XOR signature.
        /// </remarks>
        /// <seealso cref="object.GetHashCode()" />
        public override int GetHashCode()
        {
            return bcelComparator.HashCode(this);
        }

        private sealed class _BCELComparator_46 : BCELComparator
        {
            public bool Equals(object o1, object o2)
            {
                var THIS = (FieldGen) o1;
                var THAT = (FieldGen) o2;
                return System.Equals(THIS.GetName(), THAT.GetName()) && System
                           .Equals(THIS.GetSignature(), THAT.GetSignature());
            }

            public int HashCode(object o)
            {
                var THIS = (FieldGen) o;
                return THIS.GetSignature().GetHashCode() ^ THIS.GetName().GetHashCode();
            }
        }
    }
}