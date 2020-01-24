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
	/// <summary>Template class for building up a java class.</summary>
	/// <remarks>
	/// Template class for building up a java class. May be initialized with an
	/// existing java class (file).
	/// </remarks>
	/// <seealso cref="NBCEL.classfile.JavaClass"/>
	public class ClassGen : NBCEL.classfile.AccessFlags, System.ICloneable
	{
		private string class_name;

		private string super_class_name;

		private readonly string file_name;

		private int class_name_index = -1;

		private int superclass_name_index = -1;

		private int major = NBCEL.Const.MAJOR_1_1;

		private int minor = NBCEL.Const.MINOR_1_1;

		private NBCEL.generic.ConstantPoolGen cp;

		private readonly System.Collections.Generic.List<NBCEL.classfile.Field> field_vec
			 = new System.Collections.Generic.List<NBCEL.classfile.Field>();

		private readonly System.Collections.Generic.List<NBCEL.classfile.Method> method_vec
			 = new System.Collections.Generic.List<NBCEL.classfile.Method>();

		private readonly System.Collections.Generic.List<NBCEL.classfile.Attribute> attribute_vec
			 = new System.Collections.Generic.List<NBCEL.classfile.Attribute>();

		private readonly System.Collections.Generic.List<string> interface_vec = new System.Collections.Generic.List
			<string>();

		private readonly System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen
			> annotation_vec = new System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen
			>();

		private sealed class _BCELComparator_63 : NBCEL.util.BCELComparator
		{
			public _BCELComparator_63()
			{
			}

			/* Corresponds to the fields found in a JavaClass object.
			*/
			// Template for building up constant pool
			// ArrayLists instead of arrays to gather fields, methods, etc.
			public bool Equals(object o1, object o2)
			{
				NBCEL.generic.ClassGen THIS = (NBCEL.generic.ClassGen)o1;
				NBCEL.generic.ClassGen THAT = (NBCEL.generic.ClassGen)o2;
				return Sharpen.System.Equals(THIS.GetClassName(), THAT.GetClassName());
			}

			public int HashCode(object o)
			{
				NBCEL.generic.ClassGen THIS = (NBCEL.generic.ClassGen)o;
				return THIS.GetClassName().GetHashCode();
			}
		}

		private static NBCEL.util.BCELComparator _cmp = new _BCELComparator_63();

		/// <summary>Convenience constructor to set up some important values initially.</summary>
		/// <param name="class_name">fully qualified class name</param>
		/// <param name="super_class_name">fully qualified superclass name</param>
		/// <param name="file_name">source file name</param>
		/// <param name="access_flags">access qualifiers</param>
		/// <param name="interfaces">implemented interfaces</param>
		/// <param name="cp">constant pool to use</param>
		public ClassGen(string class_name, string super_class_name, string file_name, int
			 access_flags, string[] interfaces, NBCEL.generic.ConstantPoolGen cp)
			: base(access_flags)
		{
			this.class_name = class_name;
			this.super_class_name = super_class_name;
			this.file_name = file_name;
			this.cp = cp;
			// Put everything needed by default into the constant pool and the vectors
			if (file_name != null)
			{
				AddAttribute(new NBCEL.classfile.SourceFile(cp.AddUtf8("SourceFile"), 2, cp.AddUtf8
					(file_name), cp.GetConstantPool()));
			}
			class_name_index = cp.AddClass(class_name);
			superclass_name_index = cp.AddClass(super_class_name);
			if (interfaces != null)
			{
				foreach (string interface1 in interfaces)
				{
					AddInterface(interface1);
				}
			}
		}

		/// <summary>Convenience constructor to set up some important values initially.</summary>
		/// <param name="class_name">fully qualified class name</param>
		/// <param name="super_class_name">fully qualified superclass name</param>
		/// <param name="file_name">source file name</param>
		/// <param name="access_flags">access qualifiers</param>
		/// <param name="interfaces">implemented interfaces</param>
		public ClassGen(string class_name, string super_class_name, string file_name, int
			 access_flags, string[] interfaces)
			: this(class_name, super_class_name, file_name, access_flags, interfaces, new NBCEL.generic.ConstantPoolGen
				())
		{
		}

		/// <summary>Initialize with existing class.</summary>
		/// <param name="clazz">JavaClass object (e.g. read from file)</param>
		public ClassGen(NBCEL.classfile.JavaClass clazz)
			: base(clazz.GetAccessFlags())
		{
			class_name_index = clazz.GetClassNameIndex();
			superclass_name_index = clazz.GetSuperclassNameIndex();
			class_name = clazz.GetClassName();
			super_class_name = clazz.GetSuperclassName();
			file_name = clazz.GetSourceFileName();
			cp = new NBCEL.generic.ConstantPoolGen(clazz.GetConstantPool());
			major = clazz.GetMajor();
			minor = clazz.GetMinor();
			NBCEL.classfile.Attribute[] attributes = clazz.GetAttributes();
			// J5TODO: Could make unpacking lazy, done on first reference
			NBCEL.generic.AnnotationEntryGen[] annotations = UnpackAnnotations(attributes);
			NBCEL.classfile.Method[] methods = clazz.GetMethods();
			NBCEL.classfile.Field[] fields = clazz.GetFields();
			string[] interfaces = clazz.GetInterfaceNames();
			foreach (string interface1 in interfaces)
			{
				AddInterface(interface1);
			}
			foreach (NBCEL.classfile.Attribute attribute in attributes)
			{
				if (!(attribute is NBCEL.classfile.Annotations))
				{
					AddAttribute(attribute);
				}
			}
			foreach (NBCEL.generic.AnnotationEntryGen annotation in annotations)
			{
				AddAnnotationEntry(annotation);
			}
			foreach (NBCEL.classfile.Method method in methods)
			{
				AddMethod(method);
			}
			foreach (NBCEL.classfile.Field field in fields)
			{
				AddField(field);
			}
		}

		/// <summary>Look for attributes representing annotations and unpack them.</summary>
		private NBCEL.generic.AnnotationEntryGen[] UnpackAnnotations(NBCEL.classfile.Attribute
			[] attrs)
		{
			System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen> annotationGenObjs
				 = new System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen>();
			foreach (NBCEL.classfile.Attribute attr in attrs)
			{
				if (attr is NBCEL.classfile.RuntimeVisibleAnnotations)
				{
					NBCEL.classfile.RuntimeVisibleAnnotations rva = (NBCEL.classfile.RuntimeVisibleAnnotations
						)attr;
					NBCEL.classfile.AnnotationEntry[] annos = rva.GetAnnotationEntries();
					foreach (NBCEL.classfile.AnnotationEntry a in annos)
					{
						annotationGenObjs.Add(new NBCEL.generic.AnnotationEntryGen(a, GetConstantPool(), 
							false));
					}
				}
				else if (attr is NBCEL.classfile.RuntimeInvisibleAnnotations)
				{
					NBCEL.classfile.RuntimeInvisibleAnnotations ria = (NBCEL.classfile.RuntimeInvisibleAnnotations
						)attr;
					NBCEL.classfile.AnnotationEntry[] annos = ria.GetAnnotationEntries();
					foreach (NBCEL.classfile.AnnotationEntry a in annos)
					{
						annotationGenObjs.Add(new NBCEL.generic.AnnotationEntryGen(a, GetConstantPool(), 
							false));
					}
				}
			}
			return Sharpen.Collections.ToArray(annotationGenObjs, new NBCEL.generic.AnnotationEntryGen
				[annotationGenObjs.Count]);
		}

		/// <returns>the (finally) built up Java class object.</returns>
		public virtual NBCEL.classfile.JavaClass GetJavaClass()
		{
			int[] interfaces = GetInterfaces();
			NBCEL.classfile.Field[] fields = GetFields();
			NBCEL.classfile.Method[] methods = GetMethods();
			NBCEL.classfile.Attribute[] attributes = null;
			if ((annotation_vec.Count == 0))
			{
				attributes = GetAttributes();
			}
			else
			{
				// TODO: Sometime later, trash any attributes called 'RuntimeVisibleAnnotations' or 'RuntimeInvisibleAnnotations'
				NBCEL.classfile.Attribute[] annAttributes = NBCEL.generic.AnnotationEntryGen.GetAnnotationAttributes
					(cp, GetAnnotationEntries());
				attributes = new NBCEL.classfile.Attribute[attribute_vec.Count + annAttributes.Length
					];
				Sharpen.Collections.ToArray(attribute_vec, attributes);
				System.Array.Copy(annAttributes, 0, attributes, attribute_vec.Count, annAttributes
					.Length);
			}
			// Must be last since the above calls may still add something to it
			NBCEL.classfile.ConstantPool _cp = this.cp.GetFinalConstantPool();
			return new NBCEL.classfile.JavaClass(class_name_index, superclass_name_index, file_name
				, major, minor, base.GetAccessFlags(), _cp, interfaces, fields, methods, attributes
				);
		}

		/// <summary>Add an interface to this class, i.e., this class has to implement it.</summary>
		/// <param name="name">interface to implement (fully qualified class name)</param>
		public virtual void AddInterface(string name)
		{
			interface_vec.Add(name);
		}

		/// <summary>Remove an interface from this class.</summary>
		/// <param name="name">interface to remove (fully qualified name)</param>
		public virtual void RemoveInterface(string name)
		{
			interface_vec.Remove(name);
		}

		/// <returns>major version number of class file</returns>
		public virtual int GetMajor()
		{
			return major;
		}

		/// <summary>Set major version number of class file, default value is 45 (JDK 1.1)</summary>
		/// <param name="major">major version number</param>
		public virtual void SetMajor(int major)
		{
			// TODO could be package-protected - only called by test code
			this.major = major;
		}

		/// <summary>Set minor version number of class file, default value is 3 (JDK 1.1)</summary>
		/// <param name="minor">minor version number</param>
		public virtual void SetMinor(int minor)
		{
			// TODO could be package-protected - only called by test code
			this.minor = minor;
		}

		/// <returns>minor version number of class file</returns>
		public virtual int GetMinor()
		{
			return minor;
		}

		/// <summary>Add an attribute to this class.</summary>
		/// <param name="a">attribute to add</param>
		public virtual void AddAttribute(NBCEL.classfile.Attribute a)
		{
			attribute_vec.Add(a);
		}

		public virtual void AddAnnotationEntry(NBCEL.generic.AnnotationEntryGen a)
		{
			annotation_vec.Add(a);
		}

		/// <summary>Add a method to this class.</summary>
		/// <param name="m">method to add</param>
		public virtual void AddMethod(NBCEL.classfile.Method m)
		{
			method_vec.Add(m);
		}

		/// <summary>Convenience method.</summary>
		/// <remarks>
		/// Convenience method.
		/// Add an empty constructor to this class that does nothing but calling super().
		/// </remarks>
		/// <param name="access_flags">rights for constructor</param>
		public virtual void AddEmptyConstructor(int access_flags)
		{
			NBCEL.generic.InstructionList il = new NBCEL.generic.InstructionList();
			il.Append(NBCEL.generic.InstructionConst.THIS);
			// Push `this'
			il.Append(new NBCEL.generic.INVOKESPECIAL(cp.AddMethodref(super_class_name, "<init>"
				, "()V")));
			il.Append(NBCEL.generic.InstructionConst.RETURN);
			NBCEL.generic.MethodGen mg = new NBCEL.generic.MethodGen(access_flags, NBCEL.generic.Type
				.VOID, NBCEL.generic.Type.NO_ARGS, null, "<init>", class_name, il, cp);
			mg.SetMaxStack(1);
			AddMethod(mg.GetMethod());
		}

		/// <summary>Add a field to this class.</summary>
		/// <param name="f">field to add</param>
		public virtual void AddField(NBCEL.classfile.Field f)
		{
			field_vec.Add(f);
		}

		public virtual bool ContainsField(NBCEL.classfile.Field f)
		{
			return field_vec.Contains(f);
		}

		/// <returns>field object with given name, or null</returns>
		public virtual NBCEL.classfile.Field ContainsField(string name)
		{
			foreach (NBCEL.classfile.Field f in field_vec)
			{
				if (f.GetName().Equals(name))
				{
					return f;
				}
			}
			return null;
		}

		/// <returns>method object with given name and signature, or null</returns>
		public virtual NBCEL.classfile.Method ContainsMethod(string name, string signature
			)
		{
			foreach (NBCEL.classfile.Method m in method_vec)
			{
				if (m.GetName().Equals(name) && m.GetSignature().Equals(signature))
				{
					return m;
				}
			}
			return null;
		}

		/// <summary>Remove an attribute from this class.</summary>
		/// <param name="a">attribute to remove</param>
		public virtual void RemoveAttribute(NBCEL.classfile.Attribute a)
		{
			attribute_vec.Remove(a);
		}

		/// <summary>Remove a method from this class.</summary>
		/// <param name="m">method to remove</param>
		public virtual void RemoveMethod(NBCEL.classfile.Method m)
		{
			method_vec.Remove(m);
		}

		/// <summary>Replace given method with new one.</summary>
		/// <remarks>
		/// Replace given method with new one. If the old one does not exist
		/// add the new_ method to the class anyway.
		/// </remarks>
		public virtual void ReplaceMethod(NBCEL.classfile.Method old, NBCEL.classfile.Method
			 new_)
		{
			if (new_ == null)
			{
				throw new NBCEL.generic.ClassGenException("Replacement method must not be null");
			}
			int i = method_vec.IndexOf(old);
			if (i < 0)
			{
				method_vec.Add(new_);
			}
			else
			{
				method_vec[i] = new_;
			}
		}

		/// <summary>Replace given field with new one.</summary>
		/// <remarks>
		/// Replace given field with new one. If the old one does not exist
		/// add the new_ field to the class anyway.
		/// </remarks>
		public virtual void ReplaceField(NBCEL.classfile.Field old, NBCEL.classfile.Field
			 new_)
		{
			if (new_ == null)
			{
				throw new NBCEL.generic.ClassGenException("Replacement method must not be null");
			}
			int i = field_vec.IndexOf(old);
			if (i < 0)
			{
				field_vec.Add(new_);
			}
			else
			{
				field_vec[i] = new_;
			}
		}

		/// <summary>Remove a field to this class.</summary>
		/// <param name="f">field to remove</param>
		public virtual void RemoveField(NBCEL.classfile.Field f)
		{
			field_vec.Remove(f);
		}

		public virtual string GetClassName()
		{
			return class_name;
		}

		public virtual string GetSuperclassName()
		{
			return super_class_name;
		}

		public virtual string GetFileName()
		{
			return file_name;
		}

		public virtual void SetClassName(string name)
		{
			class_name = name.Replace('/', '.');
			class_name_index = cp.AddClass(name);
		}

		public virtual void SetSuperclassName(string name)
		{
			super_class_name = name.Replace('/', '.');
			superclass_name_index = cp.AddClass(name);
		}

		public virtual NBCEL.classfile.Method[] GetMethods()
		{
			return Sharpen.Collections.ToArray(method_vec, new NBCEL.classfile.Method[method_vec
				.Count]);
		}

		public virtual void SetMethods(NBCEL.classfile.Method[] methods)
		{
			method_vec.Clear();
			foreach (NBCEL.classfile.Method method in methods)
			{
				AddMethod(method);
			}
		}

		public virtual void SetMethodAt(NBCEL.classfile.Method method, int pos)
		{
			method_vec[pos] = method;
		}

		public virtual NBCEL.classfile.Method GetMethodAt(int pos)
		{
			return method_vec[pos];
		}

		public virtual string[] GetInterfaceNames()
		{
			int size = interface_vec.Count;
			string[] interfaces = new string[size];
			Sharpen.Collections.ToArray(interface_vec, interfaces);
			return interfaces;
		}

		public virtual int[] GetInterfaces()
		{
			int size = interface_vec.Count;
			int[] interfaces = new int[size];
			for (int i = 0; i < size; i++)
			{
				interfaces[i] = cp.AddClass(interface_vec[i]);
			}
			return interfaces;
		}

		public virtual NBCEL.classfile.Field[] GetFields()
		{
			return Sharpen.Collections.ToArray(field_vec, new NBCEL.classfile.Field[field_vec
				.Count]);
		}

		public virtual NBCEL.classfile.Attribute[] GetAttributes()
		{
			return Sharpen.Collections.ToArray(attribute_vec, new NBCEL.classfile.Attribute[attribute_vec
				.Count]);
		}

		//  J5TODO: Should we make calling unpackAnnotations() lazy and put it in here?
		public virtual NBCEL.generic.AnnotationEntryGen[] GetAnnotationEntries()
		{
			return Sharpen.Collections.ToArray(annotation_vec, new NBCEL.generic.AnnotationEntryGen
				[annotation_vec.Count]);
		}

		public virtual NBCEL.generic.ConstantPoolGen GetConstantPool()
		{
			return cp;
		}

		public virtual void SetConstantPool(NBCEL.generic.ConstantPoolGen constant_pool)
		{
			cp = constant_pool;
		}

		public virtual void SetClassNameIndex(int class_name_index)
		{
			this.class_name_index = class_name_index;
			class_name = cp.GetConstantPool().GetConstantString(class_name_index, NBCEL.Const
				.CONSTANT_Class).Replace('/', '.');
		}

		public virtual void SetSuperclassNameIndex(int superclass_name_index)
		{
			this.superclass_name_index = superclass_name_index;
			super_class_name = cp.GetConstantPool().GetConstantString(superclass_name_index, 
				NBCEL.Const.CONSTANT_Class).Replace('/', '.');
		}

		public virtual int GetSuperclassNameIndex()
		{
			return superclass_name_index;
		}

		public virtual int GetClassNameIndex()
		{
			return class_name_index;
		}

		private System.Collections.Generic.List<NBCEL.generic.ClassObserver> observers;

		/// <summary>Add observer for this object.</summary>
		public virtual void AddObserver(NBCEL.generic.ClassObserver o)
		{
			if (observers == null)
			{
				observers = new System.Collections.Generic.List<NBCEL.generic.ClassObserver>();
			}
			observers.Add(o);
		}

		/// <summary>Remove observer for this object.</summary>
		public virtual void RemoveObserver(NBCEL.generic.ClassObserver o)
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
				foreach (NBCEL.generic.ClassObserver observer in observers)
				{
					observer.Notify(this);
				}
			}
		}

		public virtual object Clone()
		{
			return base.MemberwiseClone();
		}

		// never happens
		/// <returns>Comparison strategy object</returns>
		public static NBCEL.util.BCELComparator GetComparator()
		{
			return _cmp;
		}

		/// <param name="comparator">Comparison strategy object</param>
		public static void SetComparator(NBCEL.util.BCELComparator comparator)
		{
			_cmp = comparator;
		}

		/// <summary>Return value as defined by given BCELComparator strategy.</summary>
		/// <remarks>
		/// Return value as defined by given BCELComparator strategy.
		/// By default two ClassGen objects are said to be equal when
		/// their class names are equal.
		/// </remarks>
		/// <seealso cref="object.Equals(object)"/>
		public override bool Equals(object obj)
		{
			return _cmp.Equals(this, obj);
		}

		/// <summary>Return value as defined by given BCELComparator strategy.</summary>
		/// <remarks>
		/// Return value as defined by given BCELComparator strategy.
		/// By default return the hashcode of the class name.
		/// </remarks>
		/// <seealso cref="object.GetHashCode()"/>
		public override int GetHashCode()
		{
			return _cmp.HashCode(this);
		}

		object System.ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
