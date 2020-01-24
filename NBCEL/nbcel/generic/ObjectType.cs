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
	/// <summary>Denotes reference such as java.lang.String.</summary>
	public class ObjectType : NBCEL.generic.ReferenceType
	{
		private readonly string class_name;

		// Class name of type
		/// <since>6.0</since>
		public static NBCEL.generic.ObjectType GetInstance(string class_name)
		{
			return new NBCEL.generic.ObjectType(class_name);
		}

		/// <param name="class_name">fully qualified class name, e.g. java.lang.String</param>
		public ObjectType(string class_name)
			: base(NBCEL.Const.T_REFERENCE, "L" + class_name.Replace('.', '/') + ";")
		{
			this.class_name = class_name.Replace('/', '.');
		}

		/// <returns>name of referenced class</returns>
		public virtual string GetClassName()
		{
			return class_name;
		}

		/// <returns>a hash code value for the object.</returns>
		public override int GetHashCode()
		{
			return class_name.GetHashCode();
		}

		/// <returns>true if both type objects refer to the same class.</returns>
		public override bool Equals(object type)
		{
			return (type is NBCEL.generic.ObjectType) ? ((NBCEL.generic.ObjectType)type).class_name
				.Equals(class_name) : false;
		}

		/// <summary>
		/// If "this" doesn't reference a class, it references an interface
		/// or a non-existant entity.
		/// </summary>
		[System.ObsoleteAttribute(@"(since 6.0) this method returns an inaccurate result if the class or interface referenced cannot be found: use referencesClassExact() instead"
			)]
		public virtual bool ReferencesClass()
		{
			try
			{
				NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(class_name);
				return jc.IsClass();
			}
			catch (System.TypeLoadException)
			{
				return false;
			}
		}

		/// <summary>
		/// If "this" doesn't reference an interface, it references a class
		/// or a non-existant entity.
		/// </summary>
		[System.ObsoleteAttribute(@"(since 6.0) this method returns an inaccurate result if the class or interface referenced cannot be found: use referencesInterfaceExact() instead"
			)]
		public virtual bool ReferencesInterface()
		{
			try
			{
				NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(class_name);
				return !jc.IsClass();
			}
			catch (System.TypeLoadException)
			{
				return false;
			}
		}

		/// <summary>
		/// Return true if this type references a class,
		/// false if it references an interface.
		/// </summary>
		/// <returns>
		/// true if the type references a class, false if
		/// it references an interface
		/// </returns>
		/// <exception cref="System.TypeLoadException">
		/// if the class or interface
		/// referenced by this type can't be found
		/// </exception>
		public virtual bool ReferencesClassExact()
		{
			NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(class_name);
			return jc.IsClass();
		}

		/// <summary>
		/// Return true if this type references an interface,
		/// false if it references a class.
		/// </summary>
		/// <returns>
		/// true if the type references an interface, false if
		/// it references a class
		/// </returns>
		/// <exception cref="System.TypeLoadException">
		/// if the class or interface
		/// referenced by this type can't be found
		/// </exception>
		public virtual bool ReferencesInterfaceExact()
		{
			NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(class_name);
			return !jc.IsClass();
		}

		/// <summary>Return true if this type is a subclass of given ObjectType.</summary>
		/// <exception cref="System.TypeLoadException">
		/// if any of this class's superclasses
		/// can't be found
		/// </exception>
		public virtual bool SubclassOf(NBCEL.generic.ObjectType superclass)
		{
			if (this.ReferencesInterfaceExact() || superclass.ReferencesInterfaceExact())
			{
				return false;
			}
			return NBCEL.Repository.InstanceOf(this.class_name, superclass.class_name);
		}

		/// <summary>Java Virtual Machine Specification edition 2, � 5.4.4 Access Control</summary>
		/// <exception cref="System.TypeLoadException">
		/// if the class referenced by this type
		/// can't be found
		/// </exception>
		public virtual bool AccessibleTo(NBCEL.generic.ObjectType accessor)
		{
			NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(class_name);
			if (jc.IsPublic())
			{
				return true;
			}
			NBCEL.classfile.JavaClass acc = NBCEL.Repository.LookupClass(accessor.class_name);
			return acc.GetPackageName().Equals(jc.GetPackageName());
		}
	}
}
