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
	/// <summary>
	/// Super class for FieldGen and MethodGen objects, since they have
	/// some methods in common!
	/// </summary>
	public abstract class FieldGenOrMethodGen : NBCEL.classfile.AccessFlags, NBCEL.generic.NamedAndTyped
		, System.ICloneable
	{
		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal string name;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal NBCEL.generic.Type type;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal NBCEL.generic.ConstantPoolGen cp;

		private readonly System.Collections.Generic.List<NBCEL.classfile.Attribute> attribute_vec
			 = new System.Collections.Generic.List<NBCEL.classfile.Attribute>();

		private readonly System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen
			> annotation_vec = new System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen
			>();

		protected internal FieldGenOrMethodGen()
		{
		}

		/// <since>6.0</since>
		protected internal FieldGenOrMethodGen(int access_flags)
			: base(access_flags)
		{
		}

		// @since 6.0
		// TODO could this be package protected?
		public virtual void SetType(NBCEL.generic.Type type)
		{
			// TODO could be package-protected?
			if (type.GetType() == NBCEL.Const.T_ADDRESS)
			{
				throw new System.ArgumentException("Type can not be " + type);
			}
			this.type = type;
		}

		public virtual NBCEL.generic.Type GetType()
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

		public virtual NBCEL.generic.ConstantPoolGen GetConstantPool()
		{
			return cp;
		}

		public virtual void SetConstantPool(NBCEL.generic.ConstantPoolGen cp)
		{
			// TODO could be package-protected?
			this.cp = cp;
		}

		/// <summary>Add an attribute to this method.</summary>
		/// <remarks>
		/// Add an attribute to this method. Currently, the JVM knows about
		/// the `Code', `ConstantValue', `Synthetic' and `Exceptions'
		/// attributes. Other attributes will be ignored by the JVM but do no
		/// harm.
		/// </remarks>
		/// <param name="a">attribute to be added</param>
		public virtual void AddAttribute(NBCEL.classfile.Attribute a)
		{
			attribute_vec.Add(a);
		}

		/// <since>6.0</since>
		protected internal virtual void AddAnnotationEntry(NBCEL.generic.AnnotationEntryGen
			 ag)
		{
			// TODO could this be package protected?
			annotation_vec.Add(ag);
		}

		/// <summary>Remove an attribute.</summary>
		public virtual void RemoveAttribute(NBCEL.classfile.Attribute a)
		{
			attribute_vec.Remove(a);
		}

		/// <since>6.0</since>
		protected internal virtual void RemoveAnnotationEntry(NBCEL.generic.AnnotationEntryGen
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
		public virtual NBCEL.classfile.Attribute[] GetAttributes()
		{
			NBCEL.classfile.Attribute[] attributes = new NBCEL.classfile.Attribute[attribute_vec
				.Count];
			Sharpen.Collections.ToArray(attribute_vec, attributes);
			return attributes;
		}

		public virtual NBCEL.generic.AnnotationEntryGen[] GetAnnotationEntries()
		{
			NBCEL.generic.AnnotationEntryGen[] annotations = new NBCEL.generic.AnnotationEntryGen
				[annotation_vec.Count];
			Sharpen.Collections.ToArray(annotation_vec, annotations);
			return annotations;
		}

		/// <returns>signature of method/field.</returns>
		public abstract string GetSignature();

		public virtual object Clone()
		{
			return base.MemberwiseClone();
		}

		object System.ICloneable.Clone()
		{
			return MemberwiseClone();
		}
		// never happens
	}
}
