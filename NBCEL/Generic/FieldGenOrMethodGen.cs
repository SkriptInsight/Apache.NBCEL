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
using Apache.NBCEL.ClassFile;
using Attribute = Apache.NBCEL.ClassFile.Attribute;

namespace Apache.NBCEL.Generic
{
	/// <summary>
	///     Super class for FieldGen and MethodGen objects, since they have
	///     some methods in common!
	/// </summary>
	public abstract class FieldGenOrMethodGen : AccessFlags, NamedAndTyped
        , ICloneable
    {
        private readonly List<AnnotationEntryGen
        > annotation_vec = new List<AnnotationEntryGen
        >();

        private readonly List<Attribute> attribute_vec
            = new List<Attribute>();

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal ConstantPoolGen cp;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal string name;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal Type type;

        protected internal FieldGenOrMethodGen()
        {
        }

        /// <since>6.0</since>
        protected internal FieldGenOrMethodGen(int access_flags)
            : base(access_flags)
        {
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        // @since 6.0
        // TODO could this be package protected?
        public virtual void SetType(Type type)
        {
            // TODO could be package-protected?
            if (type.GetType() == Const.T_ADDRESS) throw new ArgumentException("Type can not be " + type);
            this.type = type;
        }

        public virtual Type GetType()
        {
            return type;
        }

        /// <returns>name of method/field.</returns>
        public virtual string GetName()
        {
            return name;
        }

        public virtual void SetName(string name)
        {
            // TODO could be package-protected?
            this.name = name;
        }

        public virtual ConstantPoolGen GetConstantPool()
        {
            return cp;
        }

        public virtual void SetConstantPool(ConstantPoolGen cp)
        {
            // TODO could be package-protected?
            this.cp = cp;
        }

        /// <summary>Add an attribute to this method.</summary>
        /// <remarks>
        ///     Add an attribute to this method. Currently, the JVM knows about
        ///     the `Code', `ConstantValue', `Synthetic' and `Exceptions'
        ///     attributes. Other attributes will be ignored by the JVM but do no
        ///     harm.
        /// </remarks>
        /// <param name="a">attribute to be added</param>
        public virtual void AddAttribute(Attribute a)
        {
            attribute_vec.Add(a);
        }

        /// <since>6.0</since>
        protected internal virtual void AddAnnotationEntry(AnnotationEntryGen
            ag)
        {
            // TODO could this be package protected?
            annotation_vec.Add(ag);
        }

        /// <summary>Remove an attribute.</summary>
        public virtual void RemoveAttribute(Attribute a)
        {
            attribute_vec.Remove(a);
        }

        /// <since>6.0</since>
        protected internal virtual void RemoveAnnotationEntry(AnnotationEntryGen
            ag)
        {
            // TODO could this be package protected?
            annotation_vec.Remove(ag);
        }

        /// <summary>Remove all attributes.</summary>
        public virtual void RemoveAttributes()
        {
            attribute_vec.Clear();
        }

        /// <since>6.0</since>
        protected internal virtual void RemoveAnnotationEntries()
        {
            // TODO could this be package protected?
            annotation_vec.Clear();
        }

        /// <returns>all attributes of this method.</returns>
        public virtual Attribute[] GetAttributes()
        {
            var attributes = new Attribute[attribute_vec
                .Count];
            Collections.ToArray(attribute_vec, attributes);
            return attributes;
        }

        public virtual AnnotationEntryGen[] GetAnnotationEntries()
        {
            var annotations = new AnnotationEntryGen
                [annotation_vec.Count];
            Collections.ToArray(annotation_vec, annotations);
            return annotations;
        }

        /// <returns>signature of method/field.</returns>
        public abstract string GetSignature();

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        // never happens
    }
}