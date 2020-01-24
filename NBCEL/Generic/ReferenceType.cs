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
using Apache.NBCEL.ClassFile;

namespace Apache.NBCEL.Generic
{
    /// <summary>Super class for object and array types.</summary>
    public abstract class ReferenceType : Type
    {
        protected internal ReferenceType(byte t, string s)
            : base(t, s)
        {
        }

        /// <summary>Class is non-abstract but not instantiable from the outside</summary>
        internal ReferenceType()
            : base(Const.T_OBJECT, "<null object>")
        {
        }

        /// <summary>
        ///     Return true iff this type is castable to another type t as defined in
        ///     the JVM specification.
        /// </summary>
        /// <remarks>
        ///     Return true iff this type is castable to another type t as defined in
        ///     the JVM specification.  The case where this is Type.NULL is not
        ///     defined (see the CHECKCAST definition in the JVM specification).
        ///     However, because e.g. CHECKCAST doesn't throw a
        ///     ClassCastException when casting a null reference to any Object,
        ///     true is returned in this case.
        /// </remarks>
        /// <exception cref="System.TypeLoadException">
        ///     if any classes or interfaces required
        ///     to determine assignment compatibility can't be found
        /// </exception>
        public virtual bool IsCastableTo(Type t)
        {
            if (Equals(NULL)) return t is ReferenceType;
            // If this is ever changed in isAssignmentCompatible()
            return IsAssignmentCompatibleWith(t);
        }

        /* Yes, it's true: It's the same definition.
        * See vmspec2 AASTORE / CHECKCAST definitions.
        */
        /// <summary>
        ///     Return true iff this is assignment compatible with another type t
        ///     as defined in the JVM specification; see the AASTORE definition
        ///     there.
        /// </summary>
        /// <exception cref="System.TypeLoadException">
        ///     if any classes or interfaces required
        ///     to determine assignment compatibility can't be found
        /// </exception>
        public virtual bool IsAssignmentCompatibleWith(Type t)
        {
            if (!(t is ReferenceType)) return false;
            var T = (ReferenceType) t;
            if (Equals(NULL)) return true;
            // This is not explicitely stated, but clear. Isn't it?
            /* If this is a class type then
            */
            if (this is ObjectType && ((ObjectType) this).ReferencesClassExact
                    ())
            {
                /* If T is a class type, then this must be the same class as T,
                or this must be a subclass of T;
                */
                if (T is ObjectType && ((ObjectType) T).ReferencesClassExact
                        ())
                {
                    if (Equals(T)) return true;
                    if (Repository.InstanceOf(((ObjectType) this).GetClassName(),
                        ((ObjectType) T).GetClassName()))
                        return true;
                }

                /* If T is an interface type, this must implement interface T.
                */
                if (T is ObjectType && ((ObjectType) T).ReferencesInterfaceExact
                        ())
                    if (Repository.ImplementationOf(((ObjectType) this).GetClassName
                        (), ((ObjectType) T).GetClassName()))
                        return true;
            }

            /* If this is an interface type, then:
            */
            if (this is ObjectType && ((ObjectType) this).ReferencesInterfaceExact
                    ())
            {
                /* If T is a class type, then T must be Object (�2.4.7).
                */
                if (T is ObjectType && ((ObjectType) T).ReferencesClassExact
                        ())
                    if (T.Equals(OBJECT))
                        return true;
                /* If T is an interface type, then T must be the same interface
                * as this or a superinterface of this (�2.13.2).
                */
                if (T is ObjectType && ((ObjectType) T).ReferencesInterfaceExact
                        ())
                {
                    if (Equals(T)) return true;
                    if (Repository.ImplementationOf(((ObjectType) this).GetClassName
                        (), ((ObjectType) T).GetClassName()))
                        return true;
                }
            }

            /* If this is an array type, namely, the type SC[], that is, an
            * array of components of type SC, then:
            */
            if (this is ArrayType)
            {
                /* If T is a class type, then T must be Object (�2.4.7).
                */
                if (T is ObjectType && ((ObjectType) T).ReferencesClassExact
                        ())
                    if (T.Equals(OBJECT))
                        return true;
                /* If T is an array type TC[], that is, an array of components
                * of type TC, then one of the following must be true:
                */
                if (T is ArrayType)
                {
                    /* TC and SC are the same primitive type (�2.4.1).
                    */
                    var sc = ((ArrayType) this).GetElementType();
                    var tc = ((ArrayType) T).GetElementType();
                    if (sc is BasicType && tc is BasicType && sc.Equals(tc
                        ))
                        return true;
                    /* TC and SC are reference types (�2.4.6), and type SC is
                    * assignable to TC by these runtime rules.
                    */
                    if (tc is ReferenceType && sc is ReferenceType && ((ReferenceType
                            ) sc).IsAssignmentCompatibleWith(tc))
                        return true;
                }

                /* If T is an interface type, T must be one of the interfaces implemented by arrays (�2.15). */
                // TODO: Check if this is still valid or find a way to dynamically find out which
                // interfaces arrays implement. However, as of the JVM specification edition 2, there
                // are at least two different pages where assignment compatibility is defined and
                // on one of them "interfaces implemented by arrays" is exchanged with "'Cloneable' or
                // 'java.io.Serializable'"
                if (T is ObjectType && ((ObjectType) T).ReferencesInterfaceExact
                        ())
                    foreach (var element in Const.GetInterfacesImplementedByArrays())
                        if (T.Equals(ObjectType.GetInstance(element)))
                            return true;
            }

            return false;
        }

        // default.
        /// <summary>
        ///     This commutative operation returns the first common superclass (narrowest ReferenceType
        ///     referencing a class, not an interface).
        /// </summary>
        /// <remarks>
        ///     This commutative operation returns the first common superclass (narrowest ReferenceType
        ///     referencing a class, not an interface).
        ///     If one of the types is a superclass of the other, the former is returned.
        ///     If "this" is Type.NULL, then t is returned.
        ///     If t is Type.NULL, then "this" is returned.
        ///     If "this" equals t ['this.equals(t)'] "this" is returned.
        ///     If "this" or t is an ArrayType, then Type.OBJECT is returned;
        ///     unless their dimensions match. Then an ArrayType of the same
        ///     number of dimensions is returned, with its basic type being the
        ///     first common super class of the basic types of "this" and t.
        ///     If "this" or t is a ReferenceType referencing an interface, then Type.OBJECT is returned.
        ///     If not all of the two classes' superclasses cannot be found, "null" is returned.
        ///     See the JVM specification edition 2, "�4.9.2 The Bytecode Verifier".
        /// </remarks>
        /// <exception cref="System.TypeLoadException">
        ///     on failure to find superclasses of this
        ///     type, or the type passed as a parameter
        /// </exception>
        public virtual ReferenceType GetFirstCommonSuperclass(ReferenceType
            t)
        {
            if (Equals(NULL)) return t;
            if (t.Equals(NULL)) return this;
            if (Equals(t)) return this;
            /*
            * TODO: Above sounds a little arbitrary. On the other hand, there is
            * no object referenced by Type.NULL so we can also say all the objects
            * referenced by Type.NULL were derived from java.lang.Object.
            * However, the Java Language's "instanceof" operator proves us wrong:
            * "null" is not referring to an instance of java.lang.Object :)
            */
            /* This code is from a bug report by Konstantin Shagin <konst@cs.technion.ac.il> */
            if (this is ArrayType && t is ArrayType)
            {
                var arrType1 = (ArrayType) this;
                var arrType2 = (ArrayType) t;
                if (arrType1.GetDimensions() == arrType2.GetDimensions() && arrType1.GetBasicType
                        () is ObjectType && arrType2.GetBasicType() is ObjectType)
                    return new ArrayType(((ObjectType) arrType1.GetBasicType
                            ()).GetFirstCommonSuperclass((ObjectType) arrType2.GetBasicType()),
                        arrType1.GetDimensions());
            }

            if (this is ArrayType || t is ArrayType) return OBJECT;
            // TODO: Is there a proof of OBJECT being the direct ancestor of every ArrayType?
            if (this is ObjectType && ((ObjectType) this).ReferencesInterfaceExact
                    () || t is ObjectType && ((ObjectType) t).ReferencesInterfaceExact
                    ())
                return OBJECT;
            // TODO: The above line is correct comparing to the vmspec2. But one could
            // make class file verification a bit stronger here by using the notion of
            // superinterfaces or even castability or assignment compatibility.
            // this and t are ObjectTypes, see above.
            var thiz = (ObjectType) this;
            var other = (ObjectType) t;
            var thiz_sups = Repository.GetSuperClasses(thiz.GetClassName
                ());
            var other_sups = Repository.GetSuperClasses(other.GetClassName
                ());
            if (thiz_sups == null || other_sups == null) return null;
            // Waaahh...
            var this_sups = new JavaClass[thiz_sups.Length
                                          + 1];
            var t_sups = new JavaClass[other_sups.Length
                                       + 1];
            Array.Copy(thiz_sups, 0, this_sups, 1, thiz_sups.Length);
            Array.Copy(other_sups, 0, t_sups, 1, other_sups.Length);
            this_sups[0] = Repository.LookupClass(thiz.GetClassName());
            t_sups[0] = Repository.LookupClass(other.GetClassName());
            foreach (var t_sup in t_sups)
            foreach (var this_sup in this_sups)
                if (this_sup.Equals(t_sup))
                    return ObjectType.GetInstance(this_sup.GetClassName());
            // Huh? Did you ask for Type.OBJECT's superclass??
            return null;
        }

        /// <summary>
        ///     This commutative operation returns the first common superclass (narrowest ReferenceType
        ///     referencing a class, not an interface).
        /// </summary>
        /// <remarks>
        ///     This commutative operation returns the first common superclass (narrowest ReferenceType
        ///     referencing a class, not an interface).
        ///     If one of the types is a superclass of the other, the former is returned.
        ///     If "this" is Type.NULL, then t is returned.
        ///     If t is Type.NULL, then "this" is returned.
        ///     If "this" equals t ['this.equals(t)'] "this" is returned.
        ///     If "this" or t is an ArrayType, then Type.OBJECT is returned.
        ///     If "this" or t is a ReferenceType referencing an interface, then Type.OBJECT is returned.
        ///     If not all of the two classes' superclasses cannot be found, "null" is returned.
        ///     See the JVM specification edition 2, "�4.9.2 The Bytecode Verifier".
        /// </remarks>
        /// <exception cref="System.TypeLoadException">
        ///     on failure to find superclasses of this
        ///     type, or the type passed as a parameter
        /// </exception>
        [Obsolete(@"use getFirstCommonSuperclass(ReferenceType t) which has slightly changed semantics."
        )]
        public virtual ReferenceType FirstCommonSuperclass(ReferenceType
            t)
        {
            if (Equals(NULL)) return t;
            if (t.Equals(NULL)) return this;
            if (Equals(t)) return this;
            /*
            * TODO: Above sounds a little arbitrary. On the other hand, there is
            * no object referenced by Type.NULL so we can also say all the objects
            * referenced by Type.NULL were derived from java.lang.Object.
            * However, the Java Language's "instanceof" operator proves us wrong:
            * "null" is not referring to an instance of java.lang.Object :)
            */
            if (this is ArrayType || t is ArrayType) return OBJECT;
            // TODO: Is there a proof of OBJECT being the direct ancestor of every ArrayType?
            if (this is ObjectType && ((ObjectType) this).ReferencesInterface
                    () || t is ObjectType && ((ObjectType) t).ReferencesInterface
                    ())
                return OBJECT;
            // TODO: The above line is correct comparing to the vmspec2. But one could
            // make class file verification a bit stronger here by using the notion of
            // superinterfaces or even castability or assignment compatibility.
            // this and t are ObjectTypes, see above.
            var thiz = (ObjectType) this;
            var other = (ObjectType) t;
            var thiz_sups = Repository.GetSuperClasses(thiz.GetClassName
                ());
            var other_sups = Repository.GetSuperClasses(other.GetClassName
                ());
            if (thiz_sups == null || other_sups == null) return null;
            // Waaahh...
            var this_sups = new JavaClass[thiz_sups.Length
                                          + 1];
            var t_sups = new JavaClass[other_sups.Length
                                       + 1];
            Array.Copy(thiz_sups, 0, this_sups, 1, thiz_sups.Length);
            Array.Copy(other_sups, 0, t_sups, 1, other_sups.Length);
            this_sups[0] = Repository.LookupClass(thiz.GetClassName());
            t_sups[0] = Repository.LookupClass(other.GetClassName());
            foreach (var t_sup in t_sups)
            foreach (var this_sup in this_sups)
                if (this_sup.Equals(t_sup))
                    return ObjectType.GetInstance(this_sup.GetClassName());
            // Huh? Did you ask for Type.OBJECT's superclass??
            return null;
        }
    }
}