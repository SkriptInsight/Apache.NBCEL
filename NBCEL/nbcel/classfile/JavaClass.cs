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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using java.io;
using NBCEL.util;
using ObjectWeb.Misc.Java.Nio;
using Sharpen;
using Type = NBCEL.generic.Type;

namespace NBCEL.classfile
{
	/// <summary>
	///     Represents a Java class, i.e., the data structures, constant pool,
	///     fields, methods and commands contained in a Java .class file.
	/// </summary>
	/// <remarks>
	///     Represents a Java class, i.e., the data structures, constant pool,
	///     fields, methods and commands contained in a Java .class file.
	///     See <a href="http://docs.oracle.com/javase/specs/">JVM specification</a> for details.
	///     The intent of this class is to represent a parsed or otherwise existing
	///     class file.  Those interested in programatically generating classes
	///     should see the <a href="../generic/ClassGen.html">ClassGen</a> class.
	/// </remarks>
	/// <seealso cref="NBCEL.generic.ClassGen" />
	public class JavaClass : AccessFlags, ICloneable, Node
        , IComparable<JavaClass>
    {
        public const byte HEAP = 1;

        public const byte FILE = 2;

        public const byte ZIP = 3;

        private static readonly bool debug = Sharpen.System.GetBoolean("JavaClass.debug");

        private static BCELComparator bcelComparator = new _BCELComparator_76(
        );

        private AnnotationEntry[] annotations;

        private Attribute[] attributes;

        private string class_name;

        private int class_name_index;

        private bool computedNestedTypeStatus;

        private ConstantPool constant_pool;

        private Field[] fields;
        private string file_name;

        private string[] interface_names;

        private int[] interfaces;

        private bool isAnonymous__;

        private bool isNested__;

        private int major;

        private Method[] methods;

        private int minor;

        private readonly string package_name;

        /// <summary>
        ///     In cases where we go ahead and create something,
        ///     use the default SyntheticRepository, because we
        ///     don't know any better.
        /// </summary>
        [NonSerialized] private util.Repository repository = SyntheticRepository.GetInstance
            ();

        private readonly byte source = HEAP;

        private string source_file_name = "<Unknown>";

        private string superclass_name;

        private int superclass_name_index;

        /// <summary>Constructor gets all contents as arguments.</summary>
        /// <param name="class_name_index">
        ///     Index into constant pool referencing a
        ///     ConstantClass that represents this class.
        /// </param>
        /// <param name="superclass_name_index">
        ///     Index into constant pool referencing a
        ///     ConstantClass that represents this class's superclass.
        /// </param>
        /// <param name="file_name">File name</param>
        /// <param name="major">Major compiler version</param>
        /// <param name="minor">Minor compiler version</param>
        /// <param name="access_flags">Access rights defined by bit flags</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <param name="interfaces">Implemented interfaces</param>
        /// <param name="fields">Class fields</param>
        /// <param name="methods">Class methods</param>
        /// <param name="attributes">Class attributes</param>
        /// <param name="source">Read from file or generated in memory?</param>
        public JavaClass(int class_name_index, int superclass_name_index, string file_name
            , int major, int minor, int access_flags, ConstantPool constant_pool
            , int[] interfaces, Field[] fields, Method[] methods
            , Attribute[] attributes, byte source)
            : base(access_flags)
        {
            if (interfaces == null) interfaces = new int[0];
            if (attributes == null) attributes = new Attribute[0];
            if (fields == null) fields = new Field[0];
            if (methods == null) methods = new Method[0];
            this.class_name_index = class_name_index;
            this.superclass_name_index = superclass_name_index;
            this.file_name = file_name;
            this.major = major;
            this.minor = minor;
            this.constant_pool = constant_pool;
            this.interfaces = interfaces;
            this.fields = fields;
            this.methods = methods;
            this.attributes = attributes;
            this.source = source;
            // Get source file name if available
            foreach (var attribute in attributes)
                if (attribute is SourceFile)
                {
                    source_file_name = ((SourceFile) attribute).GetSourceFileName();
                    break;
                }

            /* According to the specification the following entries must be of type
            * `ConstantClass' but we check that anyway via the
            * `ConstPool.getConstant' method.
            */
            class_name = constant_pool.GetConstantString(class_name_index, Const.CONSTANT_Class
            );
            class_name = Utility.CompactClassName(class_name, false);
            var index = class_name.LastIndexOf('.');
            if (index < 0)
                package_name = string.Empty;
            else
                package_name = Runtime.Substring(class_name, 0, index);
            if (superclass_name_index > 0)
            {
                // May be zero -> class is java.lang.Object
                superclass_name = constant_pool.GetConstantString(superclass_name_index, Const
                    .CONSTANT_Class);
                superclass_name = Utility.CompactClassName(superclass_name, false
                );
            }
            else
            {
                superclass_name = "java.lang.Object";
            }

            interface_names = new string[interfaces.Length];
            for (var i = 0; i < interfaces.Length; i++)
            {
                var str = constant_pool.GetConstantString(interfaces[i], Const.CONSTANT_Class
                );
                interface_names[i] = Utility.CompactClassName(str, false);
            }
        }

        /// <summary>Constructor gets all contents as arguments.</summary>
        /// <param name="class_name_index">Class name</param>
        /// <param name="superclass_name_index">Superclass name</param>
        /// <param name="file_name">File name</param>
        /// <param name="major">Major compiler version</param>
        /// <param name="minor">Minor compiler version</param>
        /// <param name="access_flags">Access rights defined by bit flags</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <param name="interfaces">Implemented interfaces</param>
        /// <param name="fields">Class fields</param>
        /// <param name="methods">Class methods</param>
        /// <param name="attributes">Class attributes</param>
        public JavaClass(int class_name_index, int superclass_name_index, string file_name
            , int major, int minor, int access_flags, ConstantPool constant_pool
            , int[] interfaces, Field[] fields, Method[] methods
            , Attribute[] attributes)
            : this(class_name_index, superclass_name_index, file_name, major, minor, access_flags
                , constant_pool, interfaces, fields, methods, attributes, HEAP)
        {
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>Return the natural ordering of two JavaClasses.</summary>
        /// <remarks>
        ///     Return the natural ordering of two JavaClasses.
        ///     This ordering is based on the class name
        /// </remarks>
        /// <since>6.0</since>
        public virtual int CompareTo(JavaClass obj)
        {
            return string.CompareOrdinal(GetClassName(), obj.GetClassName());
        }

        /// <summary>
        ///     Called by objects that are traversing the nodes of the tree implicitely
        ///     defined by the contents of a Java class.
        /// </summary>
        /// <remarks>
        ///     Called by objects that are traversing the nodes of the tree implicitely
        ///     defined by the contents of a Java class. I.e., the hierarchy of methods,
        ///     fields, attributes, etc. spawns a tree of objects.
        /// </remarks>
        /// <param name="v">Visitor object</param>
        public virtual void Accept(Visitor v)
        {
            v.VisitJavaClass(this);
        }

        /* Print debug information depending on `JavaClass.debug'
        */
        internal static void Debug(string str)
        {
            if (debug) Console.Out.WriteLine(str);
        }

        /// <summary>Dump class to a file.</summary>
        /// <param name="file">Output file</param>
        /// <exception cref="IOException" />
        public virtual void Dump(FileInfo file)
        {
            var parent = file.Directory;
            if (parent != null) Directory.CreateDirectory(parent.FullName);
            using (var dos = new DataOutputStream(File.ReadAllBytes(file.FullName).ToOutputStream()))
            {
                Dump(dos);
            }
        }

        /// <summary>Dump class to a file named file_name.</summary>
        /// <param name="_file_name">Output file name</param>
        /// <exception cref="IOException" />
        public virtual void Dump(string _file_name)
        {
            Dump(new FileInfo(_file_name));
        }

        /// <returns>class in binary format</returns>
        public virtual byte[] GetBytes()
        {
            var stream = new MemoryStream();
            var s = new MemoryOutputStream(stream);
            var ds = new DataOutputStream(s);
            try
            {
                Dump(ds);
            }
            catch (IOException e)
            {
                Runtime.PrintStackTrace(e);
            }
            finally
            {
                try
                {
                    ds.Close();
                }
                catch (IOException e2)
                {
                    Runtime.PrintStackTrace(e2, Console.Error);
                }
            }

            return stream.ToArray();
        }

        /// <summary>Dump Java class to output stream in binary format.</summary>
        /// <param name="file">Output stream</param>
        /// <exception cref="IOException" />
        public virtual void Dump(OutputStream file)
        {
            Dump(new DataOutputStream(file));
        }

        /// <summary>Dump Java class to output stream in binary format.</summary>
        /// <param name="file">Output stream</param>
        /// <exception cref="IOException" />
        public virtual void Dump(DataOutputStream file)
        {
            file.WriteInt(Const.JVM_CLASSFILE_MAGIC);
            file.WriteShort(minor);
            file.WriteShort(major);
            constant_pool.Dump(file);
            file.WriteShort(GetAccessFlags());
            file.WriteShort(class_name_index);
            file.WriteShort(superclass_name_index);
            file.WriteShort(interfaces.Length);
            foreach (var interface1 in interfaces) file.WriteShort(interface1);
            file.WriteShort(fields.Length);
            foreach (var field in fields) field.Dump(file);
            file.WriteShort(methods.Length);
            foreach (var method in methods) method.Dump(file);
            if (attributes != null)
            {
                file.WriteShort(attributes.Length);
                foreach (var attribute in attributes) attribute.Dump(file);
            }
            else
            {
                file.WriteShort(0);
            }

            file.Flush();
        }

        /// <returns>Attributes of the class.</returns>
        public virtual Attribute[] GetAttributes()
        {
            return attributes;
        }

        /// <returns>Annotations on the class</returns>
        /// <since>6.0</since>
        public virtual AnnotationEntry[] GetAnnotationEntries()
        {
            if (annotations == null)
                annotations = AnnotationEntry.CreateAnnotationEntries(GetAttributes
                    ());
            return annotations;
        }

        /// <returns>Class name.</returns>
        public virtual string GetClassName()
        {
            return class_name;
        }

        /// <returns>Package name.</returns>
        public virtual string GetPackageName()
        {
            return package_name;
        }

        /// <returns>Class name index.</returns>
        public virtual int GetClassNameIndex()
        {
            return class_name_index;
        }

        /// <returns>Constant pool.</returns>
        public virtual ConstantPool GetConstantPool()
        {
            return constant_pool;
        }

        /// <returns>
        ///     Fields, i.e., variables of the class. Like the JVM spec
        ///     mandates for the classfile format, these fields are those specific to
        ///     this class, and not those of the superclass or superinterfaces.
        /// </returns>
        public virtual Field[] GetFields()
        {
            return fields;
        }

        /// <returns>File name of class, aka SourceFile attribute value</returns>
        public virtual string GetFileName()
        {
            return file_name;
        }

        /// <returns>Names of implemented interfaces.</returns>
        public virtual string[] GetInterfaceNames()
        {
            return interface_names;
        }

        /// <returns>Indices in constant pool of implemented interfaces.</returns>
        public virtual int[] GetInterfaceIndices()
        {
            return interfaces;
        }

        /// <returns>Major number of class file version.</returns>
        public virtual int GetMajor()
        {
            return major;
        }

        /// <returns>Methods of the class.</returns>
        public virtual Method[] GetMethods()
        {
            return methods;
        }

        /// <returns>
        ///     A
        ///     <see cref="Method" />
        ///     corresponding to
        ///     java.lang.reflect.Method if any
        /// </returns>
        public virtual Method GetMethod(MethodInfo m)
        {
            foreach (var method in methods)
                if (m.Name.Equals(method.GetName()) &&
                    Type.GetSignature(m).Equals(method.GetSignature()))
                    return method;
            return null;
        }

        /// <returns>Minor number of class file version.</returns>
        public virtual int GetMinor()
        {
            return minor;
        }

        /// <returns>sbsolute path to file where this class was read from</returns>
        public virtual string GetSourceFileName()
        {
            return source_file_name;
        }

        /// <summary>returns the super class name of this class.</summary>
        /// <remarks>
        ///     returns the super class name of this class. In the case that this class is
        ///     java.lang.Object, it will return itself (java.lang.Object). This is probably incorrect
        ///     but isn't fixed at this time to not break existing clients.
        /// </remarks>
        /// <returns>Superclass name.</returns>
        public virtual string GetSuperclassName()
        {
            return superclass_name;
        }

        /// <returns>Class name index.</returns>
        public virtual int GetSuperclassNameIndex()
        {
            return superclass_name_index;
        }

        /// <param name="attributes">.</param>
        public virtual void SetAttributes(Attribute[] attributes)
        {
            this.attributes = attributes;
        }

        /// <param name="class_name">.</param>
        public virtual void SetClassName(string class_name)
        {
            this.class_name = class_name;
        }

        /// <param name="class_name_index">.</param>
        public virtual void SetClassNameIndex(int class_name_index)
        {
            this.class_name_index = class_name_index;
        }

        /// <param name="constant_pool">.</param>
        public virtual void SetConstantPool(ConstantPool constant_pool)
        {
            this.constant_pool = constant_pool;
        }

        /// <param name="fields">.</param>
        public virtual void SetFields(Field[] fields)
        {
            this.fields = fields;
        }

        /// <summary>Set File name of class, aka SourceFile attribute value</summary>
        public virtual void SetFileName(string file_name)
        {
            this.file_name = file_name;
        }

        /// <param name="interface_names">.</param>
        public virtual void SetInterfaceNames(string[] interface_names)
        {
            this.interface_names = interface_names;
        }

        /// <param name="interfaces">.</param>
        public virtual void SetInterfaces(int[] interfaces)
        {
            this.interfaces = interfaces;
        }

        /// <param name="major">.</param>
        public virtual void SetMajor(int major)
        {
            this.major = major;
        }

        /// <param name="methods">.</param>
        public virtual void SetMethods(Method[] methods)
        {
            this.methods = methods;
        }

        /// <param name="minor">.</param>
        public virtual void SetMinor(int minor)
        {
            this.minor = minor;
        }

        /// <summary>Set absolute path to file this class was read from.</summary>
        public virtual void SetSourceFileName(string source_file_name)
        {
            this.source_file_name = source_file_name;
        }

        /// <param name="superclass_name">.</param>
        public virtual void SetSuperclassName(string superclass_name)
        {
            this.superclass_name = superclass_name;
        }

        /// <param name="superclass_name_index">.</param>
        public virtual void SetSuperclassNameIndex(int superclass_name_index)
        {
            this.superclass_name_index = superclass_name_index;
        }

        /// <returns>String representing class contents.</returns>
        public override string ToString()
        {
            var access = Utility.AccessToString(GetAccessFlags(), true
            );
            access = access.Length == 0 ? string.Empty : access + " ";
            var buf = new StringBuilder(128);
            buf.Append(access).Append(Utility.ClassOrInterface(GetAccessFlags
                ())).Append(" ").Append(class_name).Append(" extends ").Append(Utility
                .CompactClassName(superclass_name, false)).Append('\n');
            var size = interfaces.Length;
            if (size > 0)
            {
                buf.Append("implements\t\t");
                for (var i = 0; i < size; i++)
                {
                    buf.Append(interface_names[i]);
                    if (i < size - 1) buf.Append(", ");
                }

                buf.Append('\n');
            }

            buf.Append("file name\t\t").Append(file_name).Append('\n');
            buf.Append("compiled from\t\t").Append(source_file_name).Append('\n');
            buf.Append("compiler version\t").Append(major).Append(".").Append(minor).Append('\n'
            );
            buf.Append("access flags\t\t").Append(GetAccessFlags()).Append('\n');
            buf.Append("constant pool\t\t").Append(constant_pool.GetLength()).Append(" entries\n"
            );
            buf.Append("ACC_SUPER flag\t\t").Append(IsSuper()).Append("\n");
            if (attributes.Length > 0)
            {
                buf.Append("\nAttribute(s):\n");
                foreach (var attribute in attributes) buf.Append(Indent(attribute));
            }

            var annotations = GetAnnotationEntries();
            if (annotations != null && annotations.Length > 0)
            {
                buf.Append("\nAnnotation(s):\n");
                foreach (var annotation in annotations) buf.Append(Indent(annotation));
            }

            if (fields.Length > 0)
            {
                buf.Append("\n").Append(fields.Length).Append(" fields:\n");
                foreach (var field in fields) buf.Append("\t").Append(field).Append('\n');
            }

            if (methods.Length > 0)
            {
                buf.Append("\n").Append(methods.Length).Append(" methods:\n");
                foreach (var method in methods) buf.Append("\t").Append(method).Append('\n');
            }

            return buf.ToString();
        }

        private static string Indent(object obj)
        {
            return string.Join("", obj.ToString().Split('\n').Select(c => '\t' + c + '\n'));
        }

        /// <returns>deep copy of this class</returns>
        public virtual JavaClass Copy()
        {
            JavaClass c = null;
            c = (JavaClass) MemberwiseClone();
            c.constant_pool = constant_pool.Copy();
            c.interfaces = (int[]) interfaces.Clone();
            c.interface_names = (string[]) interface_names.Clone();
            c.fields = new Field[fields.Length];
            for (var i = 0; i < fields.Length; i++) c.fields[i] = fields[i].Copy(c.constant_pool);
            c.methods = new Method[methods.Length];
            for (var i = 0; i < methods.Length; i++) c.methods[i] = methods[i].Copy(c.constant_pool);
            c.attributes = new Attribute[attributes.Length];
            for (var i = 0; i < attributes.Length; i++) c.attributes[i] = attributes[i].Copy(c.constant_pool);
            // TODO should this throw?
            return c;
        }

        public bool IsSuper()
        {
            return (GetAccessFlags() & Const.ACC_SUPER) != 0;
        }

        public bool IsClass()
        {
            return (GetAccessFlags() & Const.ACC_INTERFACE) == 0;
        }

        /// <since>6.0</since>
        public bool IsAnonymous()
        {
            ComputeNestedTypeStatus();
            return isAnonymous__;
        }

        /// <since>6.0</since>
        public bool IsNested()
        {
            ComputeNestedTypeStatus();
            return isNested__;
        }

        private void ComputeNestedTypeStatus()
        {
            if (computedNestedTypeStatus) return;
            foreach (var attribute in attributes)
                if (attribute is InnerClasses)
                {
                    var innerClasses = ((InnerClasses) attribute
                        ).GetInnerClasses();
                    foreach (var innerClasse in innerClasses)
                    {
                        var innerClassAttributeRefersToMe = false;
                        var inner_class_name = constant_pool.GetConstantString(innerClasse.GetInnerClassIndex
                            (), Const.CONSTANT_Class);
                        inner_class_name = Utility.CompactClassName(inner_class_name, false
                        );
                        if (inner_class_name.Equals(GetClassName())) innerClassAttributeRefersToMe = true;
                        if (innerClassAttributeRefersToMe)
                        {
                            isNested__ = true;
                            if (innerClasse.GetInnerNameIndex() == 0) isAnonymous__ = true;
                        }
                    }
                }

            computedNestedTypeStatus = true;
        }

        /// <returns>returns either HEAP (generated), FILE, or ZIP</returns>
        public byte GetSource()
        {
            return source;
        }

        /// <summary>Gets the ClassRepository which holds its definition.</summary>
        /// <remarks>
        ///     Gets the ClassRepository which holds its definition. By default
        ///     this is the same as SyntheticRepository.getInstance();
        /// </remarks>
        public virtual util.Repository GetRepository()
        {
            return repository;
        }

        /// <summary>Sets the ClassRepository which loaded the JavaClass.</summary>
        /// <remarks>
        ///     Sets the ClassRepository which loaded the JavaClass.
        ///     Should be called immediately after parsing is done.
        /// </remarks>
        public virtual void SetRepository(util.Repository repository)
        {
            // TODO make protected?
            this.repository = repository;
        }

        /// <summary>Equivalent to runtime "instanceof" operator.</summary>
        /// <returns>true if this JavaClass is derived from the super class</returns>
        /// <exception cref="TypeLoadException">
        ///     if superclasses or superinterfaces
        ///     of this object can't be found
        /// </exception>
        public bool InstanceOf(JavaClass super_class)
        {
            if (Equals(super_class)) return true;
            var super_classes = GetSuperClasses();
            foreach (var super_classe in super_classes)
                if (super_classe.Equals(super_class))
                    return true;
            if (super_class.IsInterface()) return ImplementationOf(super_class);
            return false;
        }

        /// <returns>true, if this class is an implementation of interface inter</returns>
        /// <exception cref="TypeLoadException">
        ///     if superclasses or superinterfaces
        ///     of this class can't be found
        /// </exception>
        public virtual bool ImplementationOf(JavaClass inter)
        {
            if (!inter.IsInterface()) throw new ArgumentException(inter.GetClassName() + " is no interface");
            if (Equals(inter)) return true;
            var super_interfaces = GetAllInterfaces();
            foreach (var super_interface in super_interfaces)
                if (super_interface.Equals(inter))
                    return true;
            return false;
        }

        /// <returns>
        ///     the superclass for this JavaClass object, or null if this
        ///     is java.lang.Object
        /// </returns>
        /// <exception cref="TypeLoadException">if the superclass can't be found</exception>
        public virtual JavaClass GetSuperClass()
        {
            if ("java.lang.Object".Equals(GetClassName())) return null;
            return repository.LoadClass(GetSuperclassName());
        }

        /// <returns>
        ///     list of super classes of this class in ascending order, i.e.,
        ///     java.lang.Object is always the last element
        /// </returns>
        /// <exception cref="TypeLoadException">
        ///     if any of the superclasses can't be found
        /// </exception>
        public virtual JavaClass[] GetSuperClasses()
        {
            var clazz = this;
            var allSuperClasses = new
                List<JavaClass>();
            for (clazz = clazz.GetSuperClass(); clazz != null; clazz = clazz.GetSuperClass())
                allSuperClasses.Add(clazz);
            return Collections.ToArray(allSuperClasses, new JavaClass
                [allSuperClasses.Count]);
        }

        /// <summary>Get interfaces directly implemented by this JavaClass.</summary>
        /// <exception cref="TypeLoadException" />
        public virtual JavaClass[] GetInterfaces()
        {
            var _interfaces = GetInterfaceNames();
            var classes = new JavaClass[_interfaces.Length
            ];
            for (var i = 0; i < _interfaces.Length; i++) classes[i] = repository.LoadClass(_interfaces[i]);
            return classes;
        }

        /// <summary>Get all interfaces implemented by this JavaClass (transitively).</summary>
        /// <exception cref="TypeLoadException" />
        public virtual JavaClass[] GetAllInterfaces()
        {
            var queue = new ClassQueue();
            var allInterfaces = new
                HashSet<JavaClass>();
            queue.Enqueue(this);
            while (!queue.Empty())
            {
                var clazz = queue.Dequeue();
                var souper = clazz.GetSuperClass();
                var _interfaces = clazz.GetInterfaces();
                if (clazz.IsInterface())
                    allInterfaces.Add(clazz);
                else if (souper != null) queue.Enqueue(souper);
                foreach (var _interface in _interfaces) queue.Enqueue(_interface);
            }

            return Collections.ToArray(allInterfaces, new JavaClass[allInterfaces
                .Count]);
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
        ///     By default two JavaClass objects are said to be equal when
        ///     their class names are equal.
        /// </remarks>
        /// <seealso cref="object.Equals(object)" />
        public override bool Equals(object obj)
        {
            return bcelComparator.Equals(this, obj);
        }

        /// <summary>Return value as defined by given BCELComparator strategy.</summary>
        /// <remarks>
        ///     Return value as defined by given BCELComparator strategy.
        ///     By default return the hashcode of the class name.
        /// </remarks>
        /// <seealso cref="object.GetHashCode()" />
        public override int GetHashCode()
        {
            return bcelComparator.HashCode(this);
        }

        private sealed class _BCELComparator_76 : BCELComparator
        {
            // Compiler version
            // Constant pool
            // implemented interfaces
            // Fields, i.e., variables of class
            // methods defined in the class
            // attributes defined in the class
            // annotations defined on the class
            // Generated in memory
            // Debugging on/off
            public bool Equals(object o1, object o2)
            {
                var THIS = (JavaClass) o1;
                var THAT = (JavaClass) o2;
                return Sharpen.System.Equals(THIS.GetClassName(), THAT.GetClassName());
            }

            public int HashCode(object o)
            {
                var THIS = (JavaClass) o;
                return THIS.GetClassName().GetHashCode();
            }
        }
    }
}