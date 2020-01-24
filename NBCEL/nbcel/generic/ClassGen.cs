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
using NBCEL.classfile;
using NBCEL.util;
using Sharpen;
using Attribute = NBCEL.classfile.Attribute;

namespace NBCEL.generic
{
	/// <summary>Template class for building up a java class.</summary>
	/// <remarks>
	///     Template class for building up a java class. May be initialized with an
	///     existing java class (file).
	/// </remarks>
	/// <seealso cref="NBCEL.classfile.JavaClass" />
	public class ClassGen : AccessFlags, ICloneable
    {
        private static BCELComparator _cmp = new _BCELComparator_63();

        private readonly List<AnnotationEntryGen
        > annotation_vec = new List<AnnotationEntryGen
        >();

        private readonly List<Attribute> attribute_vec
            = new List<Attribute>();

        private readonly List<Field> field_vec
            = new List<Field>();

        private readonly string file_name;

        private readonly List<string> interface_vec = new List
            <string>();

        private readonly List<Method> method_vec
            = new List<Method>();

        private string class_name;

        private int class_name_index = -1;

        private ConstantPoolGen cp;

        private int major = Const.MAJOR_1_1;

        private int minor = Const.MINOR_1_1;

        private List<ClassObserver> observers;

        private string super_class_name;

        private int superclass_name_index = -1;

        /// <summary>Convenience constructor to set up some important values initially.</summary>
        /// <param name="class_name">fully qualified class name</param>
        /// <param name="super_class_name">fully qualified superclass name</param>
        /// <param name="file_name">source file name</param>
        /// <param name="access_flags">access qualifiers</param>
        /// <param name="interfaces">implemented interfaces</param>
        /// <param name="cp">constant pool to use</param>
        public ClassGen(string class_name, string super_class_name, string file_name, int
            access_flags, string[] interfaces, ConstantPoolGen cp)
            : base(access_flags)
        {
            this.class_name = class_name;
            this.super_class_name = super_class_name;
            this.file_name = file_name;
            this.cp = cp;
            // Put everything needed by default into the constant pool and the vectors
            if (file_name != null)
                AddAttribute(new SourceFile(cp.AddUtf8("SourceFile"), 2, cp.AddUtf8
                    (file_name), cp.GetConstantPool()));
            class_name_index = cp.AddClass(class_name);
            superclass_name_index = cp.AddClass(super_class_name);
            if (interfaces != null)
                foreach (var interface1 in interfaces)
                    AddInterface(interface1);
        }

        /// <summary>Convenience constructor to set up some important values initially.</summary>
        /// <param name="class_name">fully qualified class name</param>
        /// <param name="super_class_name">fully qualified superclass name</param>
        /// <param name="file_name">source file name</param>
        /// <param name="access_flags">access qualifiers</param>
        /// <param name="interfaces">implemented interfaces</param>
        public ClassGen(string class_name, string super_class_name, string file_name, int
            access_flags, string[] interfaces)
            : this(class_name, super_class_name, file_name, access_flags, interfaces, new ConstantPoolGen
                ())
        {
        }

        /// <summary>Initialize with existing class.</summary>
        /// <param name="clazz">JavaClass object (e.g. read from file)</param>
        public ClassGen(JavaClass clazz)
            : base(clazz.GetAccessFlags())
        {
            class_name_index = clazz.GetClassNameIndex();
            superclass_name_index = clazz.GetSuperclassNameIndex();
            class_name = clazz.GetClassName();
            super_class_name = clazz.GetSuperclassName();
            file_name = clazz.GetSourceFileName();
            cp = new ConstantPoolGen(clazz.GetConstantPool());
            major = clazz.GetMajor();
            minor = clazz.GetMinor();
            var attributes = clazz.GetAttributes();
            // J5TODO: Could make unpacking lazy, done on first reference
            var annotations = UnpackAnnotations(attributes);
            var methods = clazz.GetMethods();
            var fields = clazz.GetFields();
            var interfaces = clazz.GetInterfaceNames();
            foreach (var interface1 in interfaces) AddInterface(interface1);
            foreach (var attribute in attributes)
                if (!(attribute is Annotations))
                    AddAttribute(attribute);
            foreach (var annotation in annotations) AddAnnotationEntry(annotation);
            foreach (var method in methods) AddMethod(method);
            foreach (var field in fields) AddField(field);
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>Look for attributes representing annotations and unpack them.</summary>
        private AnnotationEntryGen[] UnpackAnnotations(Attribute
            [] attrs)
        {
            var annotationGenObjs
                = new List<AnnotationEntryGen>();
            foreach (var attr in attrs)
                if (attr is RuntimeVisibleAnnotations)
                {
                    var rva = (RuntimeVisibleAnnotations
                        ) attr;
                    var annos = rva.GetAnnotationEntries();
                    foreach (var a in annos)
                        annotationGenObjs.Add(new AnnotationEntryGen(a, GetConstantPool(),
                            false));
                }
                else if (attr is RuntimeInvisibleAnnotations)
                {
                    var ria = (RuntimeInvisibleAnnotations
                        ) attr;
                    var annos = ria.GetAnnotationEntries();
                    foreach (var a in annos)
                        annotationGenObjs.Add(new AnnotationEntryGen(a, GetConstantPool(),
                            false));
                }

            return Collections.ToArray(annotationGenObjs, new AnnotationEntryGen
                [annotationGenObjs.Count]);
        }

        /// <returns>the (finally) built up Java class object.</returns>
        public virtual JavaClass GetJavaClass()
        {
            var interfaces = GetInterfaces();
            var fields = GetFields();
            var methods = GetMethods();
            Attribute[] attributes = null;
            if (annotation_vec.Count == 0)
            {
                attributes = GetAttributes();
            }
            else
            {
                // TODO: Sometime later, trash any attributes called 'RuntimeVisibleAnnotations' or 'RuntimeInvisibleAnnotations'
                var annAttributes = AnnotationEntryGen.GetAnnotationAttributes
                    (cp, GetAnnotationEntries());
                attributes = new Attribute[attribute_vec.Count + annAttributes.Length
                ];
                Collections.ToArray(attribute_vec, attributes);
                Array.Copy(annAttributes, 0, attributes, attribute_vec.Count, annAttributes
                    .Length);
            }

            // Must be last since the above calls may still add something to it
            var _cp = cp.GetFinalConstantPool();
            return new JavaClass(class_name_index, superclass_name_index, file_name
                , major, minor, GetAccessFlags(), _cp, interfaces, fields, methods, attributes
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
        public virtual void AddAttribute(Attribute a)
        {
            attribute_vec.Add(a);
        }

        public virtual void AddAnnotationEntry(AnnotationEntryGen a)
        {
            annotation_vec.Add(a);
        }

        /// <summary>Add a method to this class.</summary>
        /// <param name="m">method to add</param>
        public virtual void AddMethod(Method m)
        {
            method_vec.Add(m);
        }

        /// <summary>Convenience method.</summary>
        /// <remarks>
        ///     Convenience method.
        ///     Add an empty constructor to this class that does nothing but calling super().
        /// </remarks>
        /// <param name="access_flags">rights for constructor</param>
        public virtual void AddEmptyConstructor(int access_flags)
        {
            var il = new InstructionList();
            il.Append(InstructionConst.THIS);
            // Push `this'
            il.Append(new INVOKESPECIAL(cp.AddMethodref(super_class_name, "<init>"
                , "()V")));
            il.Append(InstructionConst.RETURN);
            var mg = new MethodGen(access_flags, Type
                .VOID, Type.NO_ARGS, null, "<init>", class_name, il, cp);
            mg.SetMaxStack(1);
            AddMethod(mg.GetMethod());
        }

        /// <summary>Add a field to this class.</summary>
        /// <param name="f">field to add</param>
        public virtual void AddField(Field f)
        {
            field_vec.Add(f);
        }

        public virtual bool ContainsField(Field f)
        {
            return field_vec.Contains(f);
        }

        /// <returns>field object with given name, or null</returns>
        public virtual Field ContainsField(string name)
        {
            foreach (var f in field_vec)
                if (f.GetName().Equals(name))
                    return f;
            return null;
        }

        /// <returns>method object with given name and signature, or null</returns>
        public virtual Method ContainsMethod(string name, string signature
        )
        {
            foreach (var m in method_vec)
                if (m.GetName().Equals(name) && m.GetSignature().Equals(signature))
                    return m;
            return null;
        }

        /// <summary>Remove an attribute from this class.</summary>
        /// <param name="a">attribute to remove</param>
        public virtual void RemoveAttribute(Attribute a)
        {
            attribute_vec.Remove(a);
        }

        /// <summary>Remove a method from this class.</summary>
        /// <param name="m">method to remove</param>
        public virtual void RemoveMethod(Method m)
        {
            method_vec.Remove(m);
        }

        /// <summary>Replace given method with new one.</summary>
        /// <remarks>
        ///     Replace given method with new one. If the old one does not exist
        ///     add the new_ method to the class anyway.
        /// </remarks>
        public virtual void ReplaceMethod(Method old, Method
            new_)
        {
            if (new_ == null) throw new ClassGenException("Replacement method must not be null");
            var i = method_vec.IndexOf(old);
            if (i < 0)
                method_vec.Add(new_);
            else
                method_vec[i] = new_;
        }

        /// <summary>Replace given field with new one.</summary>
        /// <remarks>
        ///     Replace given field with new one. If the old one does not exist
        ///     add the new_ field to the class anyway.
        /// </remarks>
        public virtual void ReplaceField(Field old, Field
            new_)
        {
            if (new_ == null) throw new ClassGenException("Replacement method must not be null");
            var i = field_vec.IndexOf(old);
            if (i < 0)
                field_vec.Add(new_);
            else
                field_vec[i] = new_;
        }

        /// <summary>Remove a field to this class.</summary>
        /// <param name="f">field to remove</param>
        public virtual void RemoveField(Field f)
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

        public virtual Method[] GetMethods()
        {
            return Collections.ToArray(method_vec, new Method[method_vec
                .Count]);
        }

        public virtual void SetMethods(Method[] methods)
        {
            method_vec.Clear();
            foreach (var method in methods) AddMethod(method);
        }

        public virtual void SetMethodAt(Method method, int pos)
        {
            method_vec[pos] = method;
        }

        public virtual Method GetMethodAt(int pos)
        {
            return method_vec[pos];
        }

        public virtual string[] GetInterfaceNames()
        {
            var size = interface_vec.Count;
            var interfaces = new string[size];
            Collections.ToArray(interface_vec, interfaces);
            return interfaces;
        }

        public virtual int[] GetInterfaces()
        {
            var size = interface_vec.Count;
            var interfaces = new int[size];
            for (var i = 0; i < size; i++) interfaces[i] = cp.AddClass(interface_vec[i]);
            return interfaces;
        }

        public virtual Field[] GetFields()
        {
            return Collections.ToArray(field_vec, new Field[field_vec
                .Count]);
        }

        public virtual Attribute[] GetAttributes()
        {
            return Collections.ToArray(attribute_vec, new Attribute[attribute_vec
                .Count]);
        }

        //  J5TODO: Should we make calling unpackAnnotations() lazy and put it in here?
        public virtual AnnotationEntryGen[] GetAnnotationEntries()
        {
            return Collections.ToArray(annotation_vec, new AnnotationEntryGen
                [annotation_vec.Count]);
        }

        public virtual ConstantPoolGen GetConstantPool()
        {
            return cp;
        }

        public virtual void SetConstantPool(ConstantPoolGen constant_pool)
        {
            cp = constant_pool;
        }

        public virtual void SetClassNameIndex(int class_name_index)
        {
            this.class_name_index = class_name_index;
            class_name = cp.GetConstantPool().GetConstantString(class_name_index, Const
                .CONSTANT_Class).Replace('/', '.');
        }

        public virtual void SetSuperclassNameIndex(int superclass_name_index)
        {
            this.superclass_name_index = superclass_name_index;
            super_class_name = cp.GetConstantPool().GetConstantString(superclass_name_index,
                Const.CONSTANT_Class).Replace('/', '.');
        }

        public virtual int GetSuperclassNameIndex()
        {
            return superclass_name_index;
        }

        public virtual int GetClassNameIndex()
        {
            return class_name_index;
        }

        /// <summary>Add observer for this object.</summary>
        public virtual void AddObserver(ClassObserver o)
        {
            if (observers == null) observers = new List<ClassObserver>();
            observers.Add(o);
        }

        /// <summary>Remove observer for this object.</summary>
        public virtual void RemoveObserver(ClassObserver o)
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

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        // never happens
        /// <returns>Comparison strategy object</returns>
        public static BCELComparator GetComparator()
        {
            return _cmp;
        }

        /// <param name="comparator">Comparison strategy object</param>
        public static void SetComparator(BCELComparator comparator)
        {
            _cmp = comparator;
        }

        /// <summary>Return value as defined by given BCELComparator strategy.</summary>
        /// <remarks>
        ///     Return value as defined by given BCELComparator strategy.
        ///     By default two ClassGen objects are said to be equal when
        ///     their class names are equal.
        /// </remarks>
        /// <seealso cref="object.Equals(object)" />
        public override bool Equals(object obj)
        {
            return _cmp.Equals(this, obj);
        }

        /// <summary>Return value as defined by given BCELComparator strategy.</summary>
        /// <remarks>
        ///     Return value as defined by given BCELComparator strategy.
        ///     By default return the hashcode of the class name.
        /// </remarks>
        /// <seealso cref="object.GetHashCode()" />
        public override int GetHashCode()
        {
            return _cmp.HashCode(this);
        }

        private sealed class _BCELComparator_63 : BCELComparator
        {
            /* Corresponds to the fields found in a JavaClass object.
            */
            // Template for building up constant pool
            // ArrayLists instead of arrays to gather fields, methods, etc.
            public bool Equals(object o1, object o2)
            {
                var THIS = (ClassGen) o1;
                var THAT = (ClassGen) o2;
                return Sharpen.System.Equals(THIS.GetClassName(), THAT.GetClassName());
            }

            public int HashCode(object o)
            {
                var THIS = (ClassGen) o;
                return THIS.GetClassName().GetHashCode();
            }
        }
    }
}