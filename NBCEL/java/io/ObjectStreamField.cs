/*
* Copyright (c) 1996, 2013, Oracle and/or its affiliates. All rights reserved.
* ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*/

using System;
using System.Reflection;
using System.Text;

namespace java.io
{
    /// <summary>A description of a Serializable field from a Serializable class.</summary>
    /// <remarks>
    ///     A description of a Serializable field from a Serializable class.  An array
    ///     of ObjectStreamFields is used to declare the Serializable fields of a class.
    /// </remarks>
    /// <author>Mike Warres</author>
    /// <author>Roger Riggs</author>
    /// <seealso cref="ObjectStreamClass" />
    /// <since>1.2</since>
    public class ObjectStreamField : IComparable<object>
    {
        /// <summary>corresponding reflective field object, if any</summary>
        private readonly FieldInfo field;

        /// <summary>field name</summary>
        private readonly string name;

        /// <summary>canonical JVM signature of field type</summary>
        private readonly string signature;

        /// <summary>field type (Object.class if unknown non-primitive type)</summary>
        private readonly Type type;

        /// <summary>whether or not to (de)serialize field values as unshared</summary>
        private readonly bool unshared;

        /// <summary>offset of field value in enclosing field group</summary>
        private int offset;

        /// <summary>Create a Serializable field with the specified type.</summary>
        /// <remarks>
        ///     Create a Serializable field with the specified type.  This field should
        ///     be documented with a <code>serialField</code> tag.
        /// </remarks>
        /// <param name="name">the name of the serializable field</param>
        /// <param name="type">the <code>Class</code> object of the serializable field</param>
        public ObjectStreamField(string name, Type type)
            : this(name, type, false)
        {
        }

        /// <summary>
        ///     Creates an ObjectStreamField representing a serializable field with the
        ///     given name and type.
        /// </summary>
        /// <remarks>
        ///     Creates an ObjectStreamField representing a serializable field with the
        ///     given name and type.  If unshared is false, values of the represented
        ///     field are serialized and deserialized in the default manner--if the
        ///     field is non-primitive, object values are serialized and deserialized as
        ///     if they had been written and read by calls to writeObject and
        ///     readObject.  If unshared is true, values of the represented field are
        ///     serialized and deserialized as if they had been written and read by
        ///     calls to writeUnshared and readUnshared.
        /// </remarks>
        /// <param name="name">field name</param>
        /// <param name="type">field type</param>
        /// <param name="unshared">
        ///     if false, write/read field values in the same manner
        ///     as writeObject/readObject; if true, write/read in the same
        ///     manner as writeUnshared/readUnshared
        /// </param>
        /// <since>1.4</since>
        public ObjectStreamField(string name, Type type, bool unshared)
        {
            if (name == null) throw new ArgumentNullException();
            this.name = name;
            this.type = type;
            this.unshared = unshared;
            signature = string.Intern(GetClassSignature(type));
            field = null;
        }

        /// <summary>
        ///     Creates an ObjectStreamField representing a field with the given name,
        ///     signature and unshared setting.
        /// </summary>
        internal ObjectStreamField(string name, string signature, bool unshared)
        {
            if (name == null) throw new ArgumentNullException();
            this.name = name;
            this.signature = string.Intern(signature);
            this.unshared = unshared;
            field = null;
            switch (signature[0])
            {
                case 'Z':
                {
                    type = typeof(bool);
                    break;
                }

                case 'B':
                {
                    type = typeof(byte);
                    break;
                }

                case 'C':
                {
                    type = typeof(char);
                    break;
                }

                case 'S':
                {
                    type = typeof(short);
                    break;
                }

                case 'I':
                {
                    type = typeof(int);
                    break;
                }

                case 'J':
                {
                    type = typeof(long);
                    break;
                }

                case 'F':
                {
                    type = typeof(float);
                    break;
                }

                case 'D':
                {
                    type = typeof(double);
                    break;
                }

                case 'L':
                case '[':
                {
                    type = typeof(object);
                    break;
                }

                default:
                {
                    throw new ArgumentException("illegal signature");
                }
            }
        }

        /// <summary>
        ///     Creates an ObjectStreamField representing the given field with the
        ///     specified unshared setting.
        /// </summary>
        /// <remarks>
        ///     Creates an ObjectStreamField representing the given field with the
        ///     specified unshared setting.  For compatibility with the behavior of
        ///     earlier serialization implementations, a "showType" parameter is
        ///     necessary to govern whether or not a getType() call on this
        ///     ObjectStreamField (if non-primitive) will return Object.class (as
        ///     opposed to a more specific reference type).
        /// </remarks>
        internal ObjectStreamField(FieldInfo field, bool unshared, bool showType)
        {
            this.field = field;
            this.unshared = unshared;
            name = field.Name;
            var ftype = field.GetType();
            type = showType || ftype.IsPrimitive ? ftype : typeof(object);
            signature = string.Intern(GetClassSignature(ftype));
        }

        /// <summary>Compare this field with another <code>ObjectStreamField</code>.</summary>
        /// <remarks>
        ///     Compare this field with another <code>ObjectStreamField</code>.  Return
        ///     -1 if this is smaller, 0 if equal, 1 if greater.  Types that are
        ///     primitives are "smaller" than object types.  If equal, the field names
        ///     are compared.
        /// </remarks>
        public virtual int CompareTo(object obj)
        {
            // REMIND: deprecate?
            var other = (ObjectStreamField) obj;
            var isPrim = IsPrimitive();
            if (isPrim != other.IsPrimitive()) return isPrim ? -1 : 1;
            return string.CompareOrdinal(name, other.name);
        }

        /// <summary>Get the name of this field.</summary>
        /// <returns>
        ///     a <code>String</code> representing the name of the serializable
        ///     field
        /// </returns>
        public virtual string GetName()
        {
            return name;
        }

        /// <summary>Get the type of the field.</summary>
        /// <remarks>
        ///     Get the type of the field.  If the type is non-primitive and this
        ///     <code>ObjectStreamField</code> was obtained from a deserialized
        ///     <see cref="ObjectStreamClass" />
        ///     instance, then <code>Object.class</code> is returned.
        ///     Otherwise, the <code>Class</code> object for the type of the field is
        ///     returned.
        /// </remarks>
        /// <returns>
        ///     a <code>Class</code> object representing the type of the
        ///     serializable field
        /// </returns>
        public virtual Type GetType()
        {
            return type;
        }

        /// <summary>Returns character encoding of field type.</summary>
        /// <remarks>
        ///     Returns character encoding of field type.  The encoding is as follows:
        ///     <blockquote>
        ///         <pre>
        ///             B            byte
        ///             C            char
        ///             D            double
        ///             F            float
        ///             I            int
        ///             J            long
        ///             L            class or interface
        ///             S            short
        ///             Z            boolean
        ///             [            array
        ///         </pre>
        ///     </blockquote>
        /// </remarks>
        /// <returns>the typecode of the serializable field</returns>
        public virtual char GetTypeCode()
        {
            // REMIND: deprecate?
            return signature[0];
        }

        /// <summary>Return the JVM type signature.</summary>
        /// <returns>null if this field has a primitive type.</returns>
        public virtual string GetTypeString()
        {
            // REMIND: deprecate?
            return IsPrimitive() ? null : signature;
        }

        /// <summary>Offset of field within instance data.</summary>
        /// <returns>the offset of this field</returns>
        /// <seealso cref="SetOffset(int)" />
        public virtual int GetOffset()
        {
            // REMIND: deprecate?
            return offset;
        }

        /// <summary>Offset within instance data.</summary>
        /// <param name="offset">the offset of the field</param>
        /// <seealso cref="GetOffset()" />
        protected internal virtual void SetOffset(int offset)
        {
            // REMIND: deprecate?
            this.offset = offset;
        }

        /// <summary>Return true if this field has a primitive type.</summary>
        /// <returns>true if and only if this field corresponds to a primitive type</returns>
        public virtual bool IsPrimitive()
        {
            // REMIND: deprecate?
            var tcode = signature[0];
            return tcode != 'L' && tcode != '[';
        }

        /// <summary>
        ///     Returns boolean value indicating whether or not the serializable field
        ///     represented by this ObjectStreamField instance is unshared.
        /// </summary>
        /// <returns>
        ///     <see langword="true" />
        ///     if this field is unshared
        /// </returns>
        /// <since>1.4</since>
        public virtual bool IsUnshared()
        {
            return unshared;
        }

        /// <summary>Return a string that describes this field.</summary>
        public override string ToString()
        {
            return signature + ' ' + name;
        }

        /// <summary>
        ///     Returns field represented by this ObjectStreamField, or null if
        ///     ObjectStreamField is not associated with an actual field.
        /// </summary>
        internal virtual FieldInfo GetField()
        {
            return field;
        }

        /// <summary>
        ///     Returns JVM type signature of field (similar to getTypeString, except
        ///     that signature strings are returned for primitive fields as well).
        /// </summary>
        internal virtual string GetSignature()
        {
            return signature;
        }

        /// <summary>Returns JVM type signature for given class.</summary>
        private static string GetClassSignature(Type cl)
        {
            var sbuf = new StringBuilder();
            while (cl.IsArray)
            {
                sbuf.Append('[');
                cl = cl.GetElementType();
            }

            if (cl.IsPrimitive)
            {
                if (cl == typeof(int))
                    sbuf.Append('I');
                else if (cl == typeof(byte))
                    sbuf.Append('B');
                else if (cl == typeof(long))
                    sbuf.Append('J');
                else if (cl == typeof(float))
                    sbuf.Append('F');
                else if (cl == typeof(double))
                    sbuf.Append('D');
                else if (cl == typeof(short))
                    sbuf.Append('S');
                else if (cl == typeof(char))
                    sbuf.Append('C');
                else if (cl == typeof(bool))
                    sbuf.Append('Z');
                else if (cl == typeof(void))
                    sbuf.Append('V');
                else
                    throw new InternalError();
            }
            else
            {
                sbuf.Append('L' + cl.FullName.Replace('.', '/') + ';');
            }

            return sbuf.ToString();
        }
    }
}