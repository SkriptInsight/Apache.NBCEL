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
using Apache.NBCEL.Generic;
using Apache.NBCEL.Java.IO;
using Apache.NBCEL.Java.Nio;
using Apache.NBCEL.Util;
using Type = Apache.NBCEL.Generic.Type;

namespace Apache.NBCEL.ClassFile
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
    /// <seealso cref="ClassGen" />
    public class JavaClass : AccessFlags, ICloneable, Node
        , IComparable<JavaClass>
    {
        public const byte Heap = 1;

        public const byte File = 2;

        public const byte Zip = 3;

        private static readonly bool Debug = System.GetBoolean("JavaClass.debug");

        private static BCELComparator _bcelComparator = new BcelComparator76(
        );

        private AnnotationEntry[] _annotations;

        private Attribute[] _attributes;

        private string _className;

        private int _classNameIndex;

        private bool _computedNestedTypeStatus;

        private ConstantPool _constantPool;

        private Field[] _fields;
        private string _fileName;

        private string[] _interfaceNames;

        private int[] _interfaces;

        private bool _isAnonymous;

        private bool _isNested;

        private int _major;

        private Method[] _methods;

        private int _minor;

        private readonly string _packageName;

        /// <summary>
        ///     In cases where we go ahead and create something,
        ///     use the default SyntheticRepository, because we
        ///     don't know any better.
        /// </summary>
        [NonSerialized] private Util.Repository _repository = SyntheticRepository.GetInstance
            ();

        private readonly byte _source = Heap;

        private string _sourceFileName = "<Unknown>";

        private string _superclassName;

        private int _superclassNameIndex;

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
        public JavaClass(int classNameIndex, int superclassNameIndex, string fileName
            , int major, int minor, int accessFlags, ConstantPool constantPool
            , int[] interfaces, Field[] fields, Method[] methods
            , Attribute[] attributes, byte source)
            : base(accessFlags)
        {
            if (interfaces == null) interfaces = new int[0];
            if (attributes == null) attributes = new Attribute[0];
            if (fields == null) fields = new Field[0];
            if (methods == null) methods = new Method[0];
            this._classNameIndex = classNameIndex;
            this._superclassNameIndex = superclassNameIndex;
            this._fileName = fileName;
            this._major = major;
            this._minor = minor;
            this._constantPool = constantPool;
            this._interfaces = interfaces;
            this._fields = fields;
            this._methods = methods;
            this._attributes = attributes;
            this._source = source;
            // Get source file name if available
            foreach (var attribute in attributes)
                if (attribute is SourceFile)
                {
                    _sourceFileName = ((SourceFile) attribute).GetSourceFileName();
                    break;
                }

            /* According to the specification the following entries must be of type
            * `ConstantClass' but we check that anyway via the
            * `ConstPool.getConstant' method.
            */
            _className = constantPool.GetConstantString(classNameIndex, Const.CONSTANT_Class
            );
            _className = Utility.CompactClassName(_className, false);
            var index = _className.LastIndexOf('.');
            if (index < 0)
                _packageName = string.Empty;
            else
                _packageName = Runtime.Substring(_className, 0, index);
            if (superclassNameIndex > 0)
            {
                // May be zero -> class is java.lang.Object
                _superclassName = constantPool.GetConstantString(superclassNameIndex, Const
                    .CONSTANT_Class);
                _superclassName = Utility.CompactClassName(_superclassName, false
                );
            }
            else
            {
                _superclassName = "java.lang.Object";
            }

            _interfaceNames = new string[interfaces.Length];
            for (var i = 0; i < interfaces.Length; i++)
            {
                var str = constantPool.GetConstantString(interfaces[i], Const.CONSTANT_Class
                );
                _interfaceNames[i] = Utility.CompactClassName(str, false);
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
        public JavaClass(int classNameIndex, int superclassNameIndex, string fileName
            , int major, int minor, int accessFlags, ConstantPool constantPool
            , int[] interfaces, Field[] fields, Method[] methods
            , Attribute[] attributes)
            : this(classNameIndex, superclassNameIndex, fileName, major, minor, accessFlags
                , constantPool, interfaces, fields, methods, attributes, Heap)
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
        internal static void DebugLog(string str)
        {
            if (Debug) Console.Out.WriteLine(str);
        }

        /// <summary>Dump class to a file.</summary>
        /// <param name="file">Output file</param>
        /// <exception cref="IOException" />
        public virtual void Dump(FileInfo file)
        {
            var parent = file.Directory;
            if (parent != null) Directory.CreateDirectory(parent.FullName);
            using (var dos = new DataOutputStream(global::System.IO.File.ReadAllBytes(file.FullName).ToOutputStream()))
            {
                Dump(dos);
            }
        }

        /// <summary>Dump class to a file named file_name.</summary>
        /// <param name="_file_name">Output file name</param>
        /// <exception cref="IOException" />
        public virtual void Dump(string fileName)
        {
            Dump(new FileInfo(fileName));
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
            file.WriteShort(_minor);
            file.WriteShort(_major);
            _constantPool.Dump(file);
            file.WriteShort(GetAccessFlags());
            file.WriteShort(_classNameIndex);
            file.WriteShort(_superclassNameIndex);
            file.WriteShort(_interfaces.Length);
            foreach (var interface1 in _interfaces) file.WriteShort(interface1);
            file.WriteShort(_fields.Length);
            foreach (var field in _fields) field.Dump(file);
            file.WriteShort(_methods.Length);
            foreach (var method in _methods) method.Dump(file);
            if (_attributes != null)
            {
                file.WriteShort(_attributes.Length);
                foreach (var attribute in _attributes) attribute.Dump(file);
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
            return _attributes;
        }

        /// <returns>Annotations on the class</returns>
        /// <since>6.0</since>
        public virtual AnnotationEntry[] GetAnnotationEntries()
        {
            if (_annotations == null)
                _annotations = AnnotationEntry.CreateAnnotationEntries(GetAttributes
                    ());
            return _annotations;
        }

        /// <returns>Class name.</returns>
        public virtual string GetClassName()
        {
            return _className;
        }

        /// <returns>Package name.</returns>
        public virtual string GetPackageName()
        {
            return _packageName;
        }

        /// <returns>Class name index.</returns>
        public virtual int GetClassNameIndex()
        {
            return _classNameIndex;
        }

        /// <returns>Constant pool.</returns>
        public virtual ConstantPool GetConstantPool()
        {
            return _constantPool;
        }

        /// <returns>
        ///     Fields, i.e., variables of the class. Like the JVM spec
        ///     mandates for the classfile format, these fields are those specific to
        ///     this class, and not those of the superclass or superinterfaces.
        /// </returns>
        public virtual Field[] GetFields()
        {
            return _fields;
        }

        /// <returns>File name of class, aka SourceFile attribute value</returns>
        public virtual string GetFileName()
        {
            return _fileName;
        }

        /// <returns>Names of implemented interfaces.</returns>
        public virtual string[] GetInterfaceNames()
        {
            return _interfaceNames;
        }

        /// <returns>Indices in constant pool of implemented interfaces.</returns>
        public virtual int[] GetInterfaceIndices()
        {
            return _interfaces;
        }

        /// <returns>Major number of class file version.</returns>
        public virtual int GetMajor()
        {
            return _major;
        }

        /// <returns>Methods of the class.</returns>
        public virtual Method[] GetMethods()
        {
            return _methods;
        }

        /// <returns>
        ///     A
        ///     <see cref="Method" />
        ///     corresponding to
        ///     java.lang.reflect.Method if any
        /// </returns>
        public virtual Method GetMethod(MethodInfo m)
        {
            foreach (var method in _methods)
                if (m.Name.Equals(method.GetName()) &&
                    Type.GetSignature(m).Equals(method.GetSignature()))
                    return method;
            return null;
        }

        /// <returns>Minor number of class file version.</returns>
        public virtual int GetMinor()
        {
            return _minor;
        }

        /// <returns>sbsolute path to file where this class was read from</returns>
        public virtual string GetSourceFileName()
        {
            return _sourceFileName;
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
            return _superclassName;
        }

        /// <returns>Class name index.</returns>
        public virtual int GetSuperclassNameIndex()
        {
            return _superclassNameIndex;
        }

        /// <param name="attributes">.</param>
        public virtual void SetAttributes(Attribute[] attributes)
        {
            this._attributes = attributes;
        }

        /// <param name="class_name">.</param>
        public virtual void SetClassName(string className)
        {
            this._className = className;
        }

        /// <param name="class_name_index">.</param>
        public virtual void SetClassNameIndex(int classNameIndex)
        {
            this._classNameIndex = classNameIndex;
        }

        /// <param name="constant_pool">.</param>
        public virtual void SetConstantPool(ConstantPool constantPool)
        {
            this._constantPool = constantPool;
        }

        /// <param name="fields">.</param>
        public virtual void SetFields(Field[] fields)
        {
            this._fields = fields;
        }

        /// <summary>Set File name of class, aka SourceFile attribute value</summary>
        public virtual void SetFileName(string fileName)
        {
            this._fileName = fileName;
        }

        /// <param name="interface_names">.</param>
        public virtual void SetInterfaceNames(string[] interfaceNames)
        {
            this._interfaceNames = interfaceNames;
        }

        /// <param name="interfaces">.</param>
        public virtual void SetInterfaces(int[] interfaces)
        {
            this._interfaces = interfaces;
        }

        /// <param name="major">.</param>
        public virtual void SetMajor(int major)
        {
            this._major = major;
        }

        /// <param name="methods">.</param>
        public virtual void SetMethods(Method[] methods)
        {
            this._methods = methods;
        }

        /// <param name="minor">.</param>
        public virtual void SetMinor(int minor)
        {
            this._minor = minor;
        }

        /// <summary>Set absolute path to file this class was read from.</summary>
        public virtual void SetSourceFileName(string sourceFileName)
        {
            this._sourceFileName = sourceFileName;
        }

        /// <param name="superclass_name">.</param>
        public virtual void SetSuperclassName(string superclassName)
        {
            this._superclassName = superclassName;
        }

        /// <param name="superclass_name_index">.</param>
        public virtual void SetSuperclassNameIndex(int superclassNameIndex)
        {
            this._superclassNameIndex = superclassNameIndex;
        }

        /// <returns>String representing class contents.</returns>
        public override string ToString()
        {
            var access = Utility.AccessToString(GetAccessFlags(), true
            );
            access = access.Length == 0 ? string.Empty : access + " ";
            var buf = new StringBuilder(128);
            buf.Append(access).Append(Utility.ClassOrInterface(GetAccessFlags
                ())).Append(" ").Append(_className).Append(" extends ").Append(Utility
                .CompactClassName(_superclassName, false)).Append('\n');
            var size = _interfaces.Length;
            if (size > 0)
            {
                buf.Append("implements\t\t");
                for (var i = 0; i < size; i++)
                {
                    buf.Append(_interfaceNames[i]);
                    if (i < size - 1) buf.Append(", ");
                }

                buf.Append('\n');
            }

            buf.Append("file name\t\t").Append(_fileName).Append('\n');
            buf.Append("compiled from\t\t").Append(_sourceFileName).Append('\n');
            buf.Append("compiler version\t").Append(_major).Append(".").Append(_minor).Append('\n'
            );
            buf.Append("access flags\t\t").Append(GetAccessFlags()).Append('\n');
            buf.Append("constant pool\t\t").Append(_constantPool.GetLength()).Append(" entries\n"
            );
            buf.Append("ACC_SUPER flag\t\t").Append(IsSuper()).Append("\n");
            if (_attributes.Length > 0)
            {
                buf.Append("\nAttribute(s):\n");
                foreach (var attribute in _attributes) buf.Append(Indent(attribute));
            }

            var annotations = GetAnnotationEntries();
            if (annotations != null && annotations.Length > 0)
            {
                buf.Append("\nAnnotation(s):\n");
                foreach (var annotation in annotations) buf.Append(Indent(annotation));
            }

            if (_fields.Length > 0)
            {
                buf.Append("\n").Append(_fields.Length).Append(" fields:\n");
                foreach (var field in _fields) buf.Append("\t").Append(field).Append('\n');
            }

            if (_methods.Length > 0)
            {
                buf.Append("\n").Append(_methods.Length).Append(" methods:\n");
                foreach (var method in _methods) buf.Append("\t").Append(method).Append('\n');
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
            c._constantPool = _constantPool.Copy();
            c._interfaces = (int[]) _interfaces.Clone();
            c._interfaceNames = (string[]) _interfaceNames.Clone();
            c._fields = new Field[_fields.Length];
            for (var i = 0; i < _fields.Length; i++) c._fields[i] = _fields[i].Copy(c._constantPool);
            c._methods = new Method[_methods.Length];
            for (var i = 0; i < _methods.Length; i++) c._methods[i] = _methods[i].Copy(c._constantPool);
            c._attributes = new Attribute[_attributes.Length];
            for (var i = 0; i < _attributes.Length; i++) c._attributes[i] = _attributes[i].Copy(c._constantPool);
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
            return _isAnonymous;
        }

        /// <since>6.0</since>
        public bool IsNested()
        {
            ComputeNestedTypeStatus();
            return _isNested;
        }

        private void ComputeNestedTypeStatus()
        {
            if (_computedNestedTypeStatus) return;
            foreach (var attribute in _attributes)
                if (attribute is InnerClasses)
                {
                    var innerClasses = ((InnerClasses) attribute
                        ).GetInnerClasses();
                    foreach (var innerClasse in innerClasses)
                    {
                        var innerClassAttributeRefersToMe = false;
                        var innerClassName = _constantPool.GetConstantString(innerClasse.GetInnerClassIndex
                            (), Const.CONSTANT_Class);
                        innerClassName = Utility.CompactClassName(innerClassName, false
                        );
                        if (innerClassName.Equals(GetClassName())) innerClassAttributeRefersToMe = true;
                        if (innerClassAttributeRefersToMe)
                        {
                            _isNested = true;
                            if (innerClasse.GetInnerNameIndex() == 0) _isAnonymous = true;
                        }
                    }
                }

            _computedNestedTypeStatus = true;
        }

        /// <returns>returns either HEAP (generated), FILE, or ZIP</returns>
        public byte GetSource()
        {
            return _source;
        }

        /// <summary>Gets the ClassRepository which holds its definition.</summary>
        /// <remarks>
        ///     Gets the ClassRepository which holds its definition. By default
        ///     this is the same as SyntheticRepository.getInstance();
        /// </remarks>
        public virtual Util.Repository GetRepository()
        {
            return _repository;
        }

        /// <summary>Sets the ClassRepository which loaded the JavaClass.</summary>
        /// <remarks>
        ///     Sets the ClassRepository which loaded the JavaClass.
        ///     Should be called immediately after parsing is done.
        /// </remarks>
        public virtual void SetRepository(Util.Repository repository)
        {
            // TODO make protected?
            this._repository = repository;
        }

        /// <summary>Equivalent to runtime "instanceof" operator.</summary>
        /// <returns>true if this JavaClass is derived from the super class</returns>
        /// <exception cref="TypeLoadException">
        ///     if superclasses or superinterfaces
        ///     of this object can't be found
        /// </exception>
        public bool InstanceOf(JavaClass superClass)
        {
            if (Equals(superClass)) return true;
            var superClasses = GetSuperClasses();
            foreach (var superClasse in superClasses)
                if (superClasse.Equals(superClass))
                    return true;
            if (superClass.IsInterface()) return ImplementationOf(superClass);
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
            var superInterfaces = GetAllInterfaces();
            foreach (var superInterface in superInterfaces)
                if (superInterface.Equals(inter))
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
            return _repository.LoadClass(GetSuperclassName());
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
            var interfaces = GetInterfaceNames();
            var classes = new JavaClass[interfaces.Length
            ];
            for (var i = 0; i < interfaces.Length; i++) classes[i] = _repository.LoadClass(interfaces[i]);
            return classes;
        }

        public byte[] Bytes => GetBytes();

        public Attribute[] Attributes
        {
            get => GetAttributes();
            set => SetAttributes(value);
        }

        public AnnotationEntry[] AnnotationEntries => GetAnnotationEntries();

        public string ClassName
        {
            get => GetClassName();
            set => SetClassName(value);
        }

        public string PackageName => GetPackageName();

        public int ClassNameIndex
        {
            get => GetClassNameIndex();
            set => SetClassNameIndex(value);
        }

        public ConstantPool ConstantPool
        {
            get => GetConstantPool();
            set => SetConstantPool(value);
        }

        public Field[] Fields
        {
            get => GetFields();
            set => SetFields(value);
        }

        public string FileName
        {
            get => GetFileName();
            set => SetFileName(value);
        }

        public string[] InterfaceNames
        {
            get => GetInterfaceNames();
            set => SetInterfaceNames(value);
        }

        public int[] InterfaceIndices => GetInterfaceIndices();

        public int Major
        {
            get => GetMajor();
            set => SetMajor(value);
        }

        public Method[] Methods
        {
            get => GetMethods();
            set => SetMethods(value);
        }

        public int Minor
        {
            get => GetMinor();
            set => SetMinor(value);
        }

        public string SourceFileName
        {
            get => GetSourceFileName();
            set => SetSourceFileName(value);
        }

        public string SuperclassName
        {
            get => GetSuperclassName();
            set => SetSuperclassName(value);
        }

        public int SuperclassNameIndex
        {
            get => GetSuperclassNameIndex();
            set => SetSuperclassNameIndex(value);
        }

        public Util.Repository Repository
        {
            get => GetRepository();
            set => SetRepository(value);
        }

        public JavaClass SuperClass => GetSuperClass();

        public JavaClass[] SuperClasses => GetSuperClasses();

        public JavaClass[] Interfaces => GetInterfaces();

        public JavaClass[] AllInterfaces => GetAllInterfaces();


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
                var interfaces = clazz.GetInterfaces();
                if (clazz.IsInterface())
                    allInterfaces.Add(clazz);
                else if (souper != null) queue.Enqueue(souper);
                foreach (var @interface in interfaces) queue.Enqueue(@interface);
            }

            return Collections.ToArray(allInterfaces, new JavaClass[allInterfaces
                .Count]);
        }

        /// <returns>Comparison strategy object</returns>
        public static BCELComparator GetComparator()
        {
            return _bcelComparator;
        }

        /// <param name="comparator">Comparison strategy object</param>
        public static void SetComparator(BCELComparator comparator)
        {
            _bcelComparator = comparator;
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
            return _bcelComparator.Equals(this, obj);
        }

        /// <summary>Return value as defined by given BCELComparator strategy.</summary>
        /// <remarks>
        ///     Return value as defined by given BCELComparator strategy.
        ///     By default return the hashcode of the class name.
        /// </remarks>
        /// <seealso cref="object.GetHashCode()" />
        public override int GetHashCode()
        {
            return _bcelComparator.HashCode(this);
        }

        private sealed class BcelComparator76 : BCELComparator
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
                var @this = (JavaClass) o1;
                var that = (JavaClass) o2;
                return System.Equals(@this.GetClassName(), that.GetClassName());
            }

            public int HashCode(object o)
            {
                var @this = (JavaClass) o;
                return @this.GetClassName().GetHashCode();
            }
        }
    }
}