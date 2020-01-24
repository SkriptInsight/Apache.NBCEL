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

namespace Apache.NBCEL
{
    /// <summary>Constants for the project, mostly defined in the JVM specification.</summary>
    [Obsolete(@"(since 6.0) DO NOT USE - use Const instead")]
    public abstract class Constants
    {
	    /// <summary>Major version number of class files for Java 1.1.</summary>
	    /// <seealso cref="MINOR_1_1" />
	    public const short MAJOR_1_1 = 45;

	    /// <summary>Minor version number of class files for Java 1.1.</summary>
	    /// <seealso cref="MAJOR_1_1" />
	    public const short MINOR_1_1 = 3;

	    /// <summary>Major version number of class files for Java 1.2.</summary>
	    /// <seealso cref="MINOR_1_2" />
	    public const short MAJOR_1_2 = 46;

	    /// <summary>Minor version number of class files for Java 1.2.</summary>
	    /// <seealso cref="MAJOR_1_2" />
	    public const short MINOR_1_2 = 0;

	    /// <summary>Major version number of class files for Java 1.2.</summary>
	    /// <seealso cref="MINOR_1_2" />
	    public const short MAJOR_1_3 = 47;

	    /// <summary>Minor version number of class files for Java 1.3.</summary>
	    /// <seealso cref="MAJOR_1_3" />
	    public const short MINOR_1_3 = 0;

	    /// <summary>Major version number of class files for Java 1.3.</summary>
	    /// <seealso cref="MINOR_1_3" />
	    public const short MAJOR_1_4 = 48;

	    /// <summary>Minor version number of class files for Java 1.4.</summary>
	    /// <seealso cref="MAJOR_1_4" />
	    public const short MINOR_1_4 = 0;

	    /// <summary>Major version number of class files for Java 1.4.</summary>
	    /// <seealso cref="MINOR_1_4" />
	    public const short MAJOR_1_5 = 49;

	    /// <summary>Minor version number of class files for Java 1.5.</summary>
	    /// <seealso cref="MAJOR_1_5" />
	    public const short MINOR_1_5 = 0;

	    /// <summary>Default major version number.</summary>
	    /// <remarks>Default major version number.  Class file is for Java 1.1.</remarks>
	    /// <seealso cref="MAJOR_1_1" />
	    public const short MAJOR = MAJOR_1_1;

	    /// <summary>Default major version number.</summary>
	    /// <remarks>Default major version number.  Class file is for Java 1.1.</remarks>
	    /// <seealso cref="MAJOR_1_1" />
	    public const short MINOR = MINOR_1_1;

        /// <summary>Maximum value for an unsigned short.</summary>
        public const int MAX_SHORT = 65535;

        /// <summary>Maximum value for an unsigned byte.</summary>
        public const int MAX_BYTE = 255;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso>
        ///     "
        ///     <a href='http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.5'>
        ///         Flag definitions for Fields in the Java Virtual Machine Specification (Java SE 8 Edition).
        ///     </a>
        ///     "
        /// </seealso>
        /// <seealso>
        ///     "
        ///     <a href='http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.6'>
        ///         Flag definitions for Methods in the Java Virtual Machine Specification (Java SE 8 Edition).
        ///     </a>
        ///     "
        /// </seealso>
        /// <seealso>
        ///     "
        ///     <a href='http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.6-300-D.1-D.1'>
        ///         Flag definitions for Classes in the Java Virtual Machine Specification (Java SE 8 Edition).
        ///     </a>
        ///     "
        /// </seealso>
        public const short ACC_PUBLIC = 0x0001;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_PRIVATE = 0x0002;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_PROTECTED = 0x0004;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_STATIC = 0x0008;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_FINAL = 0x0010;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_SYNCHRONIZED = 0x0020;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_VOLATILE = 0x0040;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_BRIDGE = 0x0040;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_TRANSIENT = 0x0080;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_VARARGS = 0x0080;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_NATIVE = 0x0100;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_INTERFACE = 0x0200;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_ABSTRACT = 0x0400;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_STRICT = 0x0800;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_SYNTHETIC = 0x1000;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_ANNOTATION = 0x2000;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_ENUM = 0x4000;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short ACC_SUPER = 0x0020;

        /// <summary>One of the access flags for fields, methods, or classes.</summary>
        /// <seealso cref="ACC_PUBLIC" />
        public const short MAX_ACC_FLAG = ACC_ENUM;

        /// <summary>Marks a constant pool entry as type UTF-8.</summary>
        public const byte CONSTANT_Utf8 = 1;

        /// <summary>Marks a constant pool entry as type Integer.</summary>
        public const byte CONSTANT_Integer = 3;

        /// <summary>Marks a constant pool entry as type Float.</summary>
        public const byte CONSTANT_Float = 4;

        /// <summary>Marks a constant pool entry as type Long.</summary>
        public const byte CONSTANT_Long = 5;

        /// <summary>Marks a constant pool entry as type Double.</summary>
        public const byte CONSTANT_Double = 6;

        /// <summary>Marks a constant pool entry as a Class.</summary>
        public const byte CONSTANT_Class = 7;

        /// <summary>Marks a constant pool entry as a Field Reference.</summary>
        public const byte CONSTANT_Fieldref = 9;

        /// <summary>Marks a constant pool entry as type String.</summary>
        public const byte CONSTANT_String = 8;

        /// <summary>Marks a constant pool entry as a Method Reference.</summary>
        public const byte CONSTANT_Methodref = 10;

        /// <summary>Marks a constant pool entry as an Interface Method Reference.</summary>
        public const byte CONSTANT_InterfaceMethodref = 11;

        /// <summary>Marks a constant pool entry as a name and type.</summary>
        public const byte CONSTANT_NameAndType = 12;

        /// <summary>
        ///     The name of the static initializer, also called &quot;class
        ///     initialization method&quot; or &quot;interface initialization
        ///     method&quot;.
        /// </summary>
        /// <remarks>
        ///     The name of the static initializer, also called &quot;class
        ///     initialization method&quot; or &quot;interface initialization
        ///     method&quot;. This is &quot;&lt;clinit&gt;&quot;.
        /// </remarks>
        public const string STATIC_INITIALIZER_NAME = "<clinit>";

        /// <summary>
        ///     The name of every constructor method in a class, also called
        ///     &quot;instance initialization method&quot;.
        /// </summary>
        /// <remarks>
        ///     The name of every constructor method in a class, also called
        ///     &quot;instance initialization method&quot;. This is &quot;&lt;init&gt;&quot;.
        /// </remarks>
        public const string CONSTRUCTOR_NAME = "<init>";

        /// <summary>One of the limitations of the Java Virtual Machine.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.11">
        ///         * The Java Virtual Machine Specification, Second Edition, page 152, chapter 4.10.
        ///     </a>
        /// </seealso>
        public const int MAX_CP_ENTRIES = 65535;

        /// <summary>One of the limitations of the Java Virtual Machine.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.11">
        ///         * The Java Virtual Machine Specification, Second Edition, page 152, chapter 4.10.
        ///     </a>
        /// </seealso>
        public const int MAX_CODE_SIZE = 65536;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short NOP = 0;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ACONST_NULL = 1;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ICONST_M1 = 2;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ICONST_0 = 3;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ICONST_1 = 4;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ICONST_2 = 5;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ICONST_3 = 6;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ICONST_4 = 7;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ICONST_5 = 8;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LCONST_0 = 9;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LCONST_1 = 10;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FCONST_0 = 11;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FCONST_1 = 12;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FCONST_2 = 13;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DCONST_0 = 14;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DCONST_1 = 15;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short BIPUSH = 16;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short SIPUSH = 17;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LDC = 18;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LDC_W = 19;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LDC2_W = 20;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ILOAD = 21;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LLOAD = 22;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FLOAD = 23;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DLOAD = 24;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ALOAD = 25;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ILOAD_0 = 26;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ILOAD_1 = 27;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ILOAD_2 = 28;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ILOAD_3 = 29;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LLOAD_0 = 30;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LLOAD_1 = 31;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LLOAD_2 = 32;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LLOAD_3 = 33;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FLOAD_0 = 34;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FLOAD_1 = 35;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FLOAD_2 = 36;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FLOAD_3 = 37;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DLOAD_0 = 38;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DLOAD_1 = 39;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DLOAD_2 = 40;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DLOAD_3 = 41;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ALOAD_0 = 42;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ALOAD_1 = 43;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ALOAD_2 = 44;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ALOAD_3 = 45;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IALOAD = 46;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LALOAD = 47;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FALOAD = 48;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DALOAD = 49;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short AALOAD = 50;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short BALOAD = 51;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short CALOAD = 52;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short SALOAD = 53;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ISTORE = 54;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LSTORE = 55;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FSTORE = 56;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DSTORE = 57;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ASTORE = 58;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ISTORE_0 = 59;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ISTORE_1 = 60;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ISTORE_2 = 61;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ISTORE_3 = 62;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LSTORE_0 = 63;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LSTORE_1 = 64;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LSTORE_2 = 65;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LSTORE_3 = 66;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FSTORE_0 = 67;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FSTORE_1 = 68;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FSTORE_2 = 69;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FSTORE_3 = 70;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DSTORE_0 = 71;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DSTORE_1 = 72;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DSTORE_2 = 73;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DSTORE_3 = 74;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ASTORE_0 = 75;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ASTORE_1 = 76;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ASTORE_2 = 77;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ASTORE_3 = 78;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IASTORE = 79;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LASTORE = 80;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FASTORE = 81;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DASTORE = 82;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short AASTORE = 83;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short BASTORE = 84;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short CASTORE = 85;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short SASTORE = 86;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short POP = 87;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short POP2 = 88;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DUP = 89;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DUP_X1 = 90;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DUP_X2 = 91;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DUP2 = 92;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DUP2_X1 = 93;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DUP2_X2 = 94;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short SWAP = 95;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IADD = 96;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LADD = 97;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FADD = 98;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DADD = 99;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ISUB = 100;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LSUB = 101;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FSUB = 102;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DSUB = 103;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IMUL = 104;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LMUL = 105;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FMUL = 106;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DMUL = 107;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IDIV = 108;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LDIV = 109;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FDIV = 110;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DDIV = 111;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IREM = 112;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LREM = 113;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FREM = 114;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DREM = 115;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short INEG = 116;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LNEG = 117;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FNEG = 118;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DNEG = 119;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ISHL = 120;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LSHL = 121;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ISHR = 122;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LSHR = 123;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IUSHR = 124;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LUSHR = 125;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IAND = 126;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LAND = 127;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IOR = 128;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LOR = 129;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IXOR = 130;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LXOR = 131;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IINC = 132;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short I2L = 133;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short I2F = 134;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short I2D = 135;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short L2I = 136;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short L2F = 137;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short L2D = 138;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short F2I = 139;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short F2L = 140;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short F2D = 141;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short D2I = 142;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short D2L = 143;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short D2F = 144;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short I2B = 145;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short INT2BYTE = 145;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short I2C = 146;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short INT2CHAR = 146;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short I2S = 147;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short INT2SHORT = 147;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LCMP = 148;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FCMPL = 149;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FCMPG = 150;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DCMPL = 151;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DCMPG = 152;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IFEQ = 153;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IFNE = 154;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IFLT = 155;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IFGE = 156;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IFGT = 157;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IFLE = 158;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IF_ICMPEQ = 159;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IF_ICMPNE = 160;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IF_ICMPLT = 161;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IF_ICMPGE = 162;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IF_ICMPGT = 163;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IF_ICMPLE = 164;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IF_ACMPEQ = 165;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IF_ACMPNE = 166;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short GOTO = 167;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short JSR = 168;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short RET = 169;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short TABLESWITCH = 170;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LOOKUPSWITCH = 171;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IRETURN = 172;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short LRETURN = 173;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short FRETURN = 174;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short DRETURN = 175;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ARETURN = 176;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short RETURN = 177;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short GETSTATIC = 178;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short PUTSTATIC = 179;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short GETFIELD = 180;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short PUTFIELD = 181;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short INVOKEVIRTUAL = 182;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short INVOKESPECIAL = 183;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short INVOKENONVIRTUAL = 183;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short INVOKESTATIC = 184;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short INVOKEINTERFACE = 185;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short INVOKEDYNAMIC = 186;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short NEW = 187;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short NEWARRAY = 188;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ANEWARRAY = 189;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ARRAYLENGTH = 190;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short ATHROW = 191;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short CHECKCAST = 192;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short INSTANCEOF = 193;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short MONITORENTER = 194;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short MONITOREXIT = 195;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short WIDE = 196;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short MULTIANEWARRAY = 197;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IFNULL = 198;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IFNONNULL = 199;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short GOTO_W = 200;

        /// <summary>Java VM opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5">
        ///         * Opcode definitions in The Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short JSR_W = 201;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.2">
        ///         * Reserved opcodes in the Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short BREAKPOINT = 202;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short LDC_QUICK = 203;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short LDC_W_QUICK = 204;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short LDC2_W_QUICK = 205;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short GETFIELD_QUICK = 206;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short PUTFIELD_QUICK = 207;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short GETFIELD2_QUICK = 208;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short PUTFIELD2_QUICK = 209;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short GETSTATIC_QUICK = 210;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short PUTSTATIC_QUICK = 211;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short GETSTATIC2_QUICK = 212;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short PUTSTATIC2_QUICK = 213;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short INVOKEVIRTUAL_QUICK = 214;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short INVOKENONVIRTUAL_QUICK = 215;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short INVOKESUPER_QUICK = 216;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short INVOKESTATIC_QUICK = 217;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short INVOKEINTERFACE_QUICK = 218;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short INVOKEVIRTUALOBJECT_QUICK = 219;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short NEW_QUICK = 221;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short ANEWARRAY_QUICK = 222;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short MULTIANEWARRAY_QUICK = 223;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short CHECKCAST_QUICK = 224;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short INSTANCEOF_QUICK = 225;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short INVOKEVIRTUAL_QUICK_W = 226;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short GETFIELD_QUICK_W = 227;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a
        ///         href="https://web.archive.org/web/20120108031230/http://java.sun.com/docs/books/jvms/first_edition/html/Quick.doc.html">
        ///         * Specification of _quick opcodes in the Java Virtual Machine Specification (version 1)
        ///     </a>
        /// </seealso>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se5.0/html/ChangesAppendix.doc.html#448885">
        ///         * Why the _quick opcodes were removed from the second version of the Java Virtual Machine Specification.
        ///     </a>
        /// </seealso>
        public const short PUTFIELD_QUICK_W = 228;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.2">
        ///         * Reserved opcodes in the Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IMPDEP1 = 254;

        /// <summary>JVM internal opcode.</summary>
        /// <seealso>
        ///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.2">
        ///         * Reserved opcodes in the Java Virtual Machine Specification
        ///     </a>
        /// </seealso>
        public const short IMPDEP2 = 255;

        /// <summary>
        ///     BCEL virtual instruction for pushing an arbitrary data type onto the stack.
        /// </summary>
        /// <remarks>
        ///     BCEL virtual instruction for pushing an arbitrary data type onto the stack.  Will be converted to the appropriate
        ///     JVM
        ///     opcode when the class is dumped.
        /// </remarks>
        public const short PUSH = 4711;

        /// <summary>BCEL virtual instruction for either LOOKUPSWITCH or TABLESWITCH.</summary>
        /// <remarks>
        ///     BCEL virtual instruction for either LOOKUPSWITCH or TABLESWITCH.  Will be converted to the appropriate JVM
        ///     opcode when the class is dumped.
        /// </remarks>
        public const short SWITCH = 4712;

        /// <summary>Illegal opcode.</summary>
        public const short UNDEFINED = -1;

        /// <summary>Illegal opcode.</summary>
        public const short UNPREDICTABLE = -2;

        /// <summary>Illegal opcode.</summary>
        public const short RESERVED = -3;

        /// <summary>Mnemonic for an illegal opcode.</summary>
        public const string ILLEGAL_OPCODE = "<illegal opcode>";

        /// <summary>Mnemonic for an illegal type.</summary>
        public const string ILLEGAL_TYPE = "<illegal type>";

        /// <summary>Boolean data type.</summary>
        public const byte T_BOOLEAN = 4;

        /// <summary>Char data type.</summary>
        public const byte T_CHAR = 5;

        /// <summary>Float data type.</summary>
        public const byte T_FLOAT = 6;

        /// <summary>Double data type.</summary>
        public const byte T_DOUBLE = 7;

        /// <summary>Byte data type.</summary>
        public const byte T_BYTE = 8;

        /// <summary>Short data type.</summary>
        public const byte T_SHORT = 9;

        /// <summary>Int data type.</summary>
        public const byte T_INT = 10;

        /// <summary>Long data type.</summary>
        public const byte T_LONG = 11;

        /// <summary>Void data type (non-standard).</summary>
        public const byte T_VOID = 12;

        /// <summary>Array data type.</summary>
        public const byte T_ARRAY = 13;

        /// <summary>Object data type.</summary>
        public const byte T_OBJECT = 14;

        /// <summary>Reference data type (deprecated).</summary>
        public const byte T_REFERENCE = 14;

        /// <summary>Unknown data type.</summary>
        public const byte T_UNKNOWN = 15;

        /// <summary>Address data type.</summary>
        public const byte T_ADDRESS = 16;

        /// <summary>Attributes and their corresponding names.</summary>
        public const byte ATTR_UNKNOWN = unchecked((byte) -1);

        public const byte ATTR_SOURCE_FILE = 0;

        public const byte ATTR_CONSTANT_VALUE = 1;

        public const byte ATTR_CODE = 2;

        public const byte ATTR_EXCEPTIONS = 3;

        public const byte ATTR_LINE_NUMBER_TABLE = 4;

        public const byte ATTR_LOCAL_VARIABLE_TABLE = 5;

        public const byte ATTR_INNER_CLASSES = 6;

        public const byte ATTR_SYNTHETIC = 7;

        public const byte ATTR_DEPRECATED = 8;

        public const byte ATTR_PMG = 9;

        public const byte ATTR_SIGNATURE = 10;

        public const byte ATTR_STACK_MAP = 11;

        public const byte ATTR_RUNTIMEVISIBLE_ANNOTATIONS = 12;

        public const byte ATTR_RUNTIMEINVISIBLE_ANNOTATIONS = 13;

        public const byte ATTR_RUNTIMEVISIBLE_PARAMETER_ANNOTATIONS = 14;

        public const byte ATTR_RUNTIMEINVISIBLE_PARAMETER_ANNOTATIONS = 15;

        public const byte ATTR_ANNOTATION_DEFAULT = 16;

        public const short KNOWN_ATTRIBUTES = 12;

        /// <summary>Constants used in the StackMap attribute.</summary>
        public const byte ITEM_Bogus = 0;

        public const byte ITEM_Integer = 1;

        public const byte ITEM_Float = 2;

        public const byte ITEM_Double = 3;

        public const byte ITEM_Long = 4;

        public const byte ITEM_Null = 5;

        public const byte ITEM_InitObject = 6;

        public const byte ITEM_Object = 7;

        public const byte ITEM_NewObject = 8;

        /// <summary>The names of the access flags.</summary>
        public static readonly string[] ACCESS_NAMES =
        {
            "public", "private", "protected", "static", "final", "synchronized", "volatile", "transient", "native",
            "interface", "abstract", "strictfp", "synthetic", "annotation", "enum"
        };

        /// <summary>The names of the types of entries in a constant pool.</summary>
        public static readonly string[] CONSTANT_NAMES =
        {
            string.Empty, "CONSTANT_Utf8", string.Empty, "CONSTANT_Integer", "CONSTANT_Float", "CONSTANT_Long",
            "CONSTANT_Double", "CONSTANT_Class", "CONSTANT_String", "CONSTANT_Fieldref", "CONSTANT_Methodref",
            "CONSTANT_InterfaceMethodref", "CONSTANT_NameAndType"
        };

        /// <summary>The names of the interfaces implemented by arrays</summary>
        public static readonly string[] INTERFACES_IMPLEMENTED_BY_ARRAYS =
            {"java.lang.Cloneable", "java.io.Serializable"};

        /// <summary>
        ///     The primitive type names corresponding to the T_XX constants,
        ///     e.g., TYPE_NAMES[T_INT] = "int"
        /// </summary>
        public static readonly string[] TYPE_NAMES =
        {
            ILLEGAL_TYPE, ILLEGAL_TYPE, ILLEGAL_TYPE, ILLEGAL_TYPE, "boolean", "char", "float", "double", "byte",
            "short", "int", "long", "void", "array", "object", "unknown", "address"
        };

        /// <summary>
        ///     The primitive class names corresponding to the T_XX constants,
        ///     e.g., CLASS_TYPE_NAMES[T_INT] = "java.lang.Integer"
        /// </summary>
        public static readonly string[] CLASS_TYPE_NAMES =
        {
            ILLEGAL_TYPE, ILLEGAL_TYPE, ILLEGAL_TYPE, ILLEGAL_TYPE, "java.lang.Boolean", "java.lang.Character",
            "java.lang.Float", "java.lang.Double", "java.lang.Byte", "java.lang.Short", "java.lang.Integer",
            "java.lang.Long", "java.lang.Void", ILLEGAL_TYPE, ILLEGAL_TYPE, ILLEGAL_TYPE, ILLEGAL_TYPE
        };

        /// <summary>
        ///     The signature characters corresponding to primitive types,
        ///     e.g., SHORT_TYPE_NAMES[T_INT] = "I"
        /// </summary>
        public static readonly string[] SHORT_TYPE_NAMES =
        {
            ILLEGAL_TYPE, ILLEGAL_TYPE, ILLEGAL_TYPE, ILLEGAL_TYPE, "Z", "C", "F", "D", "B", "S", "I", "J", "V",
            ILLEGAL_TYPE, ILLEGAL_TYPE, ILLEGAL_TYPE
        };

        /// <summary>
        ///     Number of byte code operands for each opcode, i.e., number of bytes after the tag byte
        ///     itself.
        /// </summary>
        /// <remarks>
        ///     Number of byte code operands for each opcode, i.e., number of bytes after the tag byte
        ///     itself.  Indexed by opcode, so NO_OF_OPERANDS[BIPUSH] = the number of operands for a bipush
        ///     instruction.
        /// </remarks>
        public static readonly short[] NO_OF_OPERANDS =
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 1, 2, 1, 2, 2, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, UNPREDICTABLE, UNPREDICTABLE, 0,
            0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 4, 4, 2, 1, 2, 0, 0, 2,
            2, 0, 0, UNPREDICTABLE, 3, 2, 2, 4, 4, 0, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, RESERVED,
            RESERVED
        };

        /// <summary>How the byte code operands are to be interpreted for each opcode.</summary>
        /// <remarks>
        ///     How the byte code operands are to be interpreted for each opcode.
        ///     Indexed by opcode.  TYPE_OF_OPERANDS[ILOAD] = an array of shorts
        ///     describing the data types for the instruction.
        /// </remarks>
        public static readonly short[][] TYPE_OF_OPERANDS =
        {
            new short[] { }, new short
                [] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new short
                [] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] {T_BYTE}, new short[] {T_SHORT}, new short[]
            {
                T_BYTE
            },
            new short[] {T_SHORT}, new short[] {T_SHORT}, new short[]
            {
                T_BYTE
            },
            new short[] {T_BYTE}, new short[] {T_BYTE}, new short[] {T_BYTE}, new
                short[] {T_BYTE},
            new short[] { }, new short[] { }, new short[] { }, new short
                [] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new short
                [] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new short
                [] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] { }, new short[] {T_BYTE}, new short[]
            {
                T_BYTE
            },
            new short[] {T_BYTE}, new short[] {T_BYTE}, new short[] {T_BYTE}, new
                short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[]
                { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new
                short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[]
                { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new
                short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[]
                { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new
                short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[]
                { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new
                short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[]
                { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new
                short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[]
                { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new
                short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[]
                { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new
                short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[]
                { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new
                short[] { },
            new short[] {T_BYTE, T_BYTE}, new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] { }, new short
                [] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new short
                [] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] {T_SHORT}, new short[] {T_SHORT}, new short[] {T_SHORT}, new short
                [] {T_SHORT},
            new short[] {T_SHORT}, new short[] {T_SHORT}, new short[]
            {
                T_SHORT
            },
            new short[] {T_SHORT}, new short[] {T_SHORT}, new short[]
            {
                T_SHORT
            },
            new short[] {T_SHORT}, new short[] {T_SHORT}, new short[] {T_SHORT}, new
                short[] {T_SHORT},
            new short[] {T_SHORT}, new short[] {T_SHORT}, new short
                [] {T_BYTE},
            new short[] { }, new short[] { }, new short[] { }, new short[]
                { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new
                short[] {T_SHORT},
            new short[] {T_SHORT}, new short[] {T_SHORT}, new short
                [] {T_SHORT},
            new short[] {T_SHORT}, new short[] {T_SHORT}, new short[]
            {
                T_SHORT
            },
            new short[] {T_SHORT, T_BYTE, T_BYTE}, new short[] {T_SHORT, T_BYTE, T_BYTE}, new short[] {T_SHORT},
            new short[] {T_BYTE}, new short[]
            {
                T_SHORT
            },
            new short[] { }, new short[] { }, new short[] {T_SHORT}, new short[]
            {
                T_SHORT
            },
            new short[] { }, new short[] { }, new short[] {T_BYTE}, new short[] {T_SHORT, T_BYTE},
            new short[] {T_SHORT}, new short[] {T_SHORT}, new short[]
            {
                T_INT
            },
            new short[] {T_INT}, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new short
                [] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new short
                [] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new short
                [] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new short
                [] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new short
                [] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { },
            new short[] { }, new short[] { }, new short[] { }, new short[] { }, new short
                [] { },
            new short[] { }
        };

        /// <summary>Names of opcodes.</summary>
        /// <remarks>Names of opcodes.  Indexed by opcode.  OPCODE_NAMES[ALOAD] = "aload".</remarks>
        public static readonly string[] OPCODE_NAMES =
        {
            "nop", "aconst_null", "iconst_m1", "iconst_0", "iconst_1", "iconst_2", "iconst_3", "iconst_4", "iconst_5",
            "lconst_0", "lconst_1", "fconst_0", "fconst_1", "fconst_2", "dconst_0", "dconst_1", "bipush", "sipush",
            "ldc", "ldc_w", "ldc2_w", "iload", "lload", "fload", "dload", "aload", "iload_0", "iload_1", "iload_2",
            "iload_3", "lload_0", "lload_1", "lload_2", "lload_3", "fload_0", "fload_1", "fload_2", "fload_3",
            "dload_0", "dload_1", "dload_2", "dload_3", "aload_0", "aload_1", "aload_2", "aload_3", "iaload", "laload",
            "faload", "daload", "aaload", "baload", "caload", "saload", "istore", "lstore", "fstore", "dstore",
            "astore", "istore_0", "istore_1", "istore_2", "istore_3", "lstore_0", "lstore_1", "lstore_2", "lstore_3",
            "fstore_0", "fstore_1", "fstore_2", "fstore_3", "dstore_0", "dstore_1", "dstore_2", "dstore_3", "astore_0",
            "astore_1", "astore_2", "astore_3", "iastore", "lastore", "fastore", "dastore", "aastore", "bastore",
            "castore", "sastore", "pop", "pop2", "dup", "dup_x1", "dup_x2", "dup2", "dup2_x1", "dup2_x2", "swap",
            "iadd", "ladd", "fadd", "dadd", "isub", "lsub", "fsub", "dsub", "imul", "lmul", "fmul", "dmul", "idiv",
            "ldiv", "fdiv", "ddiv", "irem", "lrem", "frem", "drem", "ineg", "lneg", "fneg", "dneg", "ishl", "lshl",
            "ishr", "lshr", "iushr", "lushr", "iand", "land", "ior", "lor", "ixor", "lxor", "iinc", "i2l", "i2f", "i2d",
            "l2i", "l2f", "l2d", "f2i", "f2l", "f2d", "d2i", "d2l", "d2f", "i2b", "i2c", "i2s", "lcmp", "fcmpl",
            "fcmpg", "dcmpl", "dcmpg", "ifeq", "ifne", "iflt", "ifge", "ifgt", "ifle", "if_icmpeq", "if_icmpne",
            "if_icmplt", "if_icmpge", "if_icmpgt", "if_icmple",
            "if_acmpeq", "if_acmpne", "goto", "jsr", "ret", "tableswitch", "lookupswitch", "ireturn", "lreturn",
            "freturn", "dreturn", "areturn", "return", "getstatic", "putstatic", "getfield", "putfield",
            "invokevirtual", "invokespecial", "invokestatic", "invokeinterface", "invokedynamic", "new", "newarray",
            "anewarray", "arraylength", "athrow", "checkcast", "instanceof", "monitorenter", "monitorexit", "wide",
            "multianewarray", "ifnull", "ifnonnull", "goto_w", "jsr_w", "breakpoint", ILLEGAL_OPCODE, ILLEGAL_OPCODE,
            ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE,
            ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE,
            ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE,
            ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE,
            ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE,
            ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE,
            ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE,
            ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE,
            ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE,
            ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, ILLEGAL_OPCODE, "impdep1", "impdep2"
        };

        /// <summary>Number of words consumed on operand stack by instructions.</summary>
        /// <remarks>
        ///     Number of words consumed on operand stack by instructions.
        ///     Indexed by opcode.  CONSUME_STACK[FALOAD] = number of words
        ///     consumed from the stack by a faload instruction.
        /// </remarks>
        public static readonly int[] CONSUME_STACK =
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 2,
            2, 2, 1, 1, 1, 1, 2, 2, 2, 2, 1, 1, 1, 1, 3, 4, 3, 4, 3, 3, 3, 3, 1, 2, 1, 2, 3,
            2, 3, 4, 2, 2, 4, 2, 4, 2, 4, 2, 4, 2, 4, 2, 4, 2, 4, 2, 4, 2, 4, 2, 4, 1, 2, 1,
            2, 2, 3, 2, 3, 2, 3, 2, 4, 2, 4, 2, 4, 0, 1, 1, 1, 2, 2, 2, 1, 1, 1, 2, 2, 2, 1,
            1, 1, 4, 2, 2, 4, 4, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 1, 1, 1,
            2, 1, 2, 1, 0, 0, UNPREDICTABLE, 1, UNPREDICTABLE, UNPREDICTABLE, UNPREDICTABLE,
            UNPREDICTABLE, UNPREDICTABLE, UNPREDICTABLE, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, UNPREDICTABLE, 1, 1, 0, 0, 0,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNPREDICTABLE, UNPREDICTABLE
        };

        /// <summary>Number of words produced onto operand stack by instructions.</summary>
        /// <remarks>
        ///     Number of words produced onto operand stack by instructions.
        ///     Indexed by opcode.  CONSUME_STACK[DALOAD] = number of words
        ///     consumed from the stack by a daload instruction.
        /// </remarks>
        public static readonly int[] PRODUCE_STACK =
        {
            0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 1,
            1, 1,
            2, 2, 2, 2, 1, 1, 1, 1, 1, 2, 1, 2, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 4,
            4, 5, 6, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1,
            2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 0, 2, 1, 2, 1, 1, 2, 1, 2, 2, 1, 2, 1, 1,
            1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0,
            0, 0, 0, 0, 0, UNPREDICTABLE, 0, UNPREDICTABLE, 0, UNPREDICTABLE, UNPREDICTABLE,
            UNPREDICTABLE, UNPREDICTABLE, UNPREDICTABLE, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0,
            0, 0, 1, 0, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED,
            UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNDEFINED, UNPREDICTABLE, UNPREDICTABLE
        };

        public static readonly string[] ATTRIBUTE_NAMES =
        {
            "SourceFile", "ConstantValue", "Code", "Exceptions", "LineNumberTable", "LocalVariableTable",
            "InnerClasses",
            "Synthetic", "Deprecated", "PMGClass", "Signature", "StackMap", "RuntimeVisibleAnnotations",
            "RuntimeInvisibleAnnotations", "RuntimeVisibleParameterAnnotations", "RuntimeInvisibleParameterAnnotations",
            "AnnotationDefault"
        };

        public static readonly string[] ITEM_NAMES =
            {"Bogus", "Integer", "Float", "Double", "Long", "Null", "InitObject", "Object", "NewObject"};

        // 2^16 - 1
        // 2^8 - 1
        // Applies to classes compiled by new compilers only
        //bytes
        // Old notion
        // Old notion
        // Old notion
        // Old name in JDK 1.0
        // Non-standard
        // Deprecated
        /*nop*/
        /*aconst_null*/
        /*iconst_m1*/
        /*iconst_0*/
        /*iconst_1*/
        /*iconst_2*/
        /*iconst_3*/
        /*iconst_4*/
        /*iconst_5*/
        /*lconst_0*/
        /*lconst_1*/
        /*fconst_0*/
        /*fconst_1*/
        /*fconst_2*/
        /*dconst_0*/
        /*dconst_1*/
        /*bipush*/
        /*sipush*/
        /*ldc*/
        /*ldc_w*/
        /*ldc2_w*/
        /*iload*/
        /*lload*/
        /*fload*/
        /*dload*/
        /*aload*/
        /*iload_0*/
        /*iload_1*/
        /*iload_2*/
        /*iload_3*/
        /*lload_0*/
        /*lload_1*/
        /*lload_2*/
        /*lload_3*/
        /*fload_0*/
        /*fload_1*/
        /*fload_2*/
        /*fload_3*/
        /*dload_0*/
        /*dload_1*/
        /*dload_2*/
        /*dload_3*/
        /*aload_0*/
        /*aload_1*/
        /*aload_2*/
        /*aload_3*/
        /*iaload*/
        /*laload*/
        /*faload*/
        /*daload*/
        /*aaload*/
        /*baload*/
        /*caload*/
        /*saload*/
        /*istore*/
        /*lstore*/
        /*fstore*/
        /*dstore*/
        /*astore*/
        /*istore_0*/
        /*istore_1*/
        /*istore_2*/
        /*istore_3*/
        /*lstore_0*/
        /*lstore_1*/
        /*lstore_2*/
        /*lstore_3*/
        /*fstore_0*/
        /*fstore_1*/
        /*fstore_2*/
        /*fstore_3*/
        /*dstore_0*/
        /*dstore_1*/
        /*dstore_2*/
        /*dstore_3*/
        /*astore_0*/
        /*astore_1*/
        /*astore_2*/
        /*astore_3*/
        /*iastore*/
        /*lastore*/
        /*fastore*/
        /*dastore*/
        /*aastore*/
        /*bastore*/
        /*castore*/
        /*sastore*/
        /*pop*/
        /*pop2*/
        /*dup*/
        /*dup_x1*/
        /*dup_x2*/
        /*dup2*/
        /*dup2_x1*/
        /*dup2_x2*/
        /*swap*/
        /*iadd*/
        /*ladd*/
        /*fadd*/
        /*dadd*/
        /*isub*/
        /*lsub*/
        /*fsub*/
        /*dsub*/
        /*imul*/
        /*lmul*/
        /*fmul*/
        /*dmul*/
        /*idiv*/
        /*ldiv*/
        /*fdiv*/
        /*ddiv*/
        /*irem*/
        /*lrem*/
        /*frem*/
        /*drem*/
        /*ineg*/
        /*lneg*/
        /*fneg*/
        /*dneg*/
        /*ishl*/
        /*lshl*/
        /*ishr*/
        /*lshr*/
        /*iushr*/
        /*lushr*/
        /*iand*/
        /*land*/
        /*ior*/
        /*lor*/
        /*ixor*/
        /*lxor*/
        /*iinc*/
        /*i2l*/
        /*i2f*/
        /*i2d*/
        /*l2i*/
        /*l2f*/
        /*l2d*/
        /*f2i*/
        /*f2l*/
        /*f2d*/
        /*d2i*/
        /*d2l*/
        /*d2f*/
        /*i2b*/
        /*i2c*/
        /*i2s*/
        /*lcmp*/
        /*fcmpl*/
        /*fcmpg*/
        /*dcmpl*/
        /*dcmpg*/
        /*ifeq*/
        /*ifne*/
        /*iflt*/
        /*ifge*/
        /*ifgt*/
        /*ifle*/
        /*if_icmpeq*/
        /*if_icmpne*/
        /*if_icmplt*/
        /*if_icmpge*/
        /*if_icmpgt*/
        /*if_icmple*/
        /*if_acmpeq*/
        /*if_acmpne*/
        /*goto*/
        /*jsr*/
        /*ret*/
        /*tableswitch*/
        /*lookupswitch*/
        /*ireturn*/
        /*lreturn*/
        /*freturn*/
        /*dreturn*/
        /*areturn*/
        /*return*/
        /*getstatic*/
        /*putstatic*/
        /*getfield*/
        /*putfield*/
        /*invokevirtual*/
        /*invokespecial*/
        /*invokestatic*/
        /*invokeinterface*/
        /*invokedynamic*/
        /*new*/
        /*newarray*/
        /*anewarray*/
        /*arraylength*/
        /*athrow*/
        /*checkcast*/
        /*instanceof*/
        /*monitorenter*/
        /*monitorexit*/
        /*wide*/
        /*multianewarray*/
        /*ifnull*/
        /*ifnonnull*/
        /*goto_w*/
        /*jsr_w*/
        /*breakpoint*/
        /*impdep1*/
        /*impdep2*/
        /*nop*/
        /*aconst_null*/
        /*iconst_m1*/
        /*iconst_0*/
        /*iconst_1*/
        /*iconst_2*/
        /*iconst_3*/
        /*iconst_4*/
        /*iconst_5*/
        /*lconst_0*/
        /*lconst_1*/
        /*fconst_0*/
        /*fconst_1*/
        /*fconst_2*/
        /*dconst_0*/
        /*dconst_1*/
        /*bipush*/
        /*sipush*/
        /*ldc*/
        /*ldc_w*/
        /*ldc2_w*/
        /*iload*/
        /*lload*/
        /*fload*/
        /*dload*/
        /*aload*/
        /*iload_0*/
        /*iload_1*/
        /*iload_2*/
        /*iload_3*/
        /*lload_0*/
        /*lload_1*/
        /*lload_2*/
        /*lload_3*/
        /*fload_0*/
        /*fload_1*/
        /*fload_2*/
        /*fload_3*/
        /*dload_0*/
        /*dload_1*/
        /*dload_2*/
        /*dload_3*/
        /*aload_0*/
        /*aload_1*/
        /*aload_2*/
        /*aload_3*/
        /*iaload*/
        /*laload*/
        /*faload*/
        /*daload*/
        /*aaload*/
        /*baload*/
        /*caload*/
        /*saload*/
        /*istore*/
        /*lstore*/
        /*fstore*/
        /*dstore*/
        /*astore*/
        /*istore_0*/
        /*istore_1*/
        /*istore_2*/
        /*istore_3*/
        /*lstore_0*/
        /*lstore_1*/
        /*lstore_2*/
        /*lstore_3*/
        /*fstore_0*/
        /*fstore_1*/
        /*fstore_2*/
        /*fstore_3*/
        /*dstore_0*/
        /*dstore_1*/
        /*dstore_2*/
        /*dstore_3*/
        /*astore_0*/
        /*astore_1*/
        /*astore_2*/
        /*astore_3*/
        /*iastore*/
        /*lastore*/
        /*fastore*/
        /*dastore*/
        /*aastore*/
        /*bastore*/
        /*castore*/
        /*sastore*/
        /*pop*/
        /*pop2*/
        /*dup*/
        /*dup_x1*/
        /*dup_x2*/
        /*dup2*/
        /*dup2_x1*/
        /*dup2_x2*/
        /*swap*/
        /*iadd*/
        /*ladd*/
        /*fadd*/
        /*dadd*/
        /*isub*/
        /*lsub*/
        /*fsub*/
        /*dsub*/
        /*imul*/
        /*lmul*/
        /*fmul*/
        /*dmul*/
        /*idiv*/
        /*ldiv*/
        /*fdiv*/
        /*ddiv*/
        /*irem*/
        /*lrem*/
        /*frem*/
        /*drem*/
        /*ineg*/
        /*lneg*/
        /*fneg*/
        /*dneg*/
        /*ishl*/
        /*lshl*/
        /*ishr*/
        /*lshr*/
        /*iushr*/
        /*lushr*/
        /*iand*/
        /*land*/
        /*ior*/
        /*lor*/
        /*ixor*/
        /*lxor*/
        /*iinc*/
        /*i2l*/
        /*i2f*/
        /*i2d*/
        /*l2i*/
        /*l2f*/
        /*l2d*/
        /*f2i*/
        /*f2l*/
        /*f2d*/
        /*d2i*/
        /*d2l*/
        /*d2f*/
        /*i2b*/
        /*i2c*/
        /*i2s*/
        /*lcmp*/
        /*fcmpl*/
        /*fcmpg*/
        /*dcmpl*/
        /*dcmpg*/
        /*ifeq*/
        /*ifne*/
        /*iflt*/
        /*ifge*/
        /*ifgt*/
        /*ifle*/
        /*if_icmpeq*/
        /*if_icmpne*/
        /*if_icmplt*/
        /*if_icmpge*/
        /*if_icmpgt*/
        /*if_icmple*/
        /*if_acmpeq*/
        /*if_acmpne*/
        /*goto*/
        /*jsr*/
        /*ret*/
        /*tableswitch*/
        /*lookupswitch*/
        /*ireturn*/
        /*lreturn*/
        /*freturn*/
        /*dreturn*/
        /*areturn*/
        /*return*/
        /*getstatic*/
        /*putstatic*/
        /*getfield*/
        /*putfield*/
        /*invokevirtual*/
        /*invokespecial*/
        /*invokestatic*/
        /*invokeinterface*/
        /*invokedynamic*/
        /*new*/
        /*newarray*/
        /*anewarray*/
        /*arraylength*/
        /*athrow*/
        /*checkcast*/
        /*instanceof*/
        /*monitorenter*/
        /*monitorexit*/
        /*wide*/
        /*multianewarray*/
        /*ifnull*/
        /*ifnonnull*/
        /*goto_w*/
        /*jsr_w*/
        /*breakpoint*/
        /*impdep1*/
        /*impdep2*/
        /*nop*/
        /*aconst_null*/
        /*iconst_m1*/
        /*iconst_0*/
        /*iconst_1*/
        /*iconst_2*/
        /*iconst_3*/
        /*iconst_4*/
        /*iconst_5*/
        /*lconst_0*/
        /*lconst_1*/
        /*fconst_0*/
        /*fconst_1*/
        /*fconst_2*/
        /*dconst_0*/
        /*dconst_1*/
        /*bipush*/
        /*sipush*/
        /*ldc*/
        /*ldc_w*/
        /*ldc2_w*/
        /*iload*/
        /*lload*/
        /*fload*/
        /*dload*/
        /*aload*/
        /*iload_0*/
        /*iload_1*/
        /*iload_2*/
        /*iload_3*/
        /*lload_0*/
        /*lload_1*/
        /*lload_2*/
        /*lload_3*/
        /*fload_0*/
        /*fload_1*/
        /*fload_2*/
        /*fload_3*/
        /*dload_0*/
        /*dload_1*/
        /*dload_2*/
        /*dload_3*/
        /*aload_0*/
        /*aload_1*/
        /*aload_2*/
        /*aload_3*/
        /*iaload*/
        /*laload*/
        /*faload*/
        /*daload*/
        /*aaload*/
        /*baload*/
        /*caload*/
        /*saload*/
        /*istore*/
        /*lstore*/
        /*fstore*/
        /*dstore*/
        /*astore*/
        /*istore_0*/
        /*istore_1*/
        /*istore_2*/
        /*istore_3*/
        /*lstore_0*/
        /*lstore_1*/
        /*lstore_2*/
        /*lstore_3*/
        /*fstore_0*/
        /*fstore_1*/
        /*fstore_2*/
        /*fstore_3*/
        /*dstore_0*/
        /*dstore_1*/
        /*dstore_2*/
        /*dstore_3*/
        /*astore_0*/
        /*astore_1*/
        /*astore_2*/
        /*astore_3*/
        /*iastore*/
        /*lastore*/
        /*fastore*/
        /*dastore*/
        /*aastore*/
        /*bastore*/
        /*castore*/
        /*sastore*/
        /*pop*/
        /*pop2*/
        /*dup*/
        /*dup_x1*/
        /*dup_x2*/
        /*dup2*/
        /*dup2_x1*/
        /*dup2_x2*/
        /*swap*/
        /*iadd*/
        /*ladd*/
        /*fadd*/
        /*dadd*/
        /*isub*/
        /*lsub*/
        /*fsub*/
        /*dsub*/
        /*imul*/
        /*lmul*/
        /*fmul*/
        /*dmul*/
        /*idiv*/
        /*ldiv*/
        /*fdiv*/
        /*ddiv*/
        /*irem*/
        /*lrem*/
        /*frem*/
        /*drem*/
        /*ineg*/
        /*lneg*/
        /*fneg*/
        /*dneg*/
        /*ishl*/
        /*lshl*/
        /*ishr*/
        /*lshr*/
        /*iushr*/
        /*lushr*/
        /*iand*/
        /*land*/
        /*ior*/
        /*lor*/
        /*ixor*/
        /*lxor*/
        /*iinc*/
        /*i2l*/
        /*i2f*/
        /*i2d*/
        /*l2i*/
        /*l2f*/
        /*l2d*/
        /*f2i*/
        /*f2l*/
        /*f2d*/
        /*d2i*/
        /*d2l*/
        /*d2f*/
        /*i2b*/
        /*i2c*/
        /*i2s*/
        /*lcmp*/
        /*fcmpl*/
        /*fcmpg*/
        /*dcmpl*/
        /*dcmpg*/
        /*ifeq*/
        /*ifne*/
        /*iflt*/
        /*ifge*/
        /*ifgt*/
        /*ifle*/
        /*if_icmpeq*/
        /*if_icmpne*/
        /*if_icmplt*/
        /*if_icmpge*/
        /*if_icmpgt*/
        /*if_icmple*/
        /*if_acmpeq*/
        /*if_acmpne*/
        /*goto*/
        /*jsr*/
        /*ret*/
        /*tableswitch*/
        /*lookupswitch*/
        /*ireturn*/
        /*lreturn*/
        /*freturn*/
        /*dreturn*/
        /*areturn*/
        /*return*/
        /*getstatic*/
        /*putstatic*/
        /*getfield*/
        /*putfield*/
        /*invokevirtual*/
        /*invokespecial*/
        /*invokestatic*/
        /*invokeinterface*/
        /*invokedynamic*/
        /*new*/
        /*newarray*/
        /*anewarray*/
        /*arraylength*/
        /*athrow*/
        /*checkcast*/
        /*instanceof*/
        /*monitorenter*/
        /*monitorexit*/
        /*wide*/
        /*multianewarray*/
        /*ifnull*/
        /*ifnonnull*/
        /*goto_w*/
        /*jsr_w*/
        /*breakpoint*/
        /*impdep1*/
        /*impdep2*/
        /*nop*/
        /*aconst_null*/
        /*iconst_m1*/
        /*iconst_0*/
        /*iconst_1*/
        /*iconst_2*/
        /*iconst_3*/
        /*iconst_4*/
        /*iconst_5*/
        /*lconst_0*/
        /*lconst_1*/
        /*fconst_0*/
        /*fconst_1*/
        /*fconst_2*/
        /*dconst_0*/
        /*dconst_1*/
        /*bipush*/
        /*sipush*/
        /*ldc*/
        /*ldc_w*/
        /*ldc2_w*/
        /*iload*/
        /*lload*/
        /*fload*/
        /*dload*/
        /*aload*/
        /*iload_0*/
        /*iload_1*/
        /*iload_2*/
        /*iload_3*/
        /*lload_0*/
        /*lload_1*/
        /*lload_2*/
        /*lload_3*/
        /*fload_0*/
        /*fload_1*/
        /*fload_2*/
        /*fload_3*/
        /*dload_0*/
        /*dload_1*/
        /*dload_2*/
        /*dload_3*/
        /*aload_0*/
        /*aload_1*/
        /*aload_2*/
        /*aload_3*/
        /*iaload*/
        /*laload*/
        /*faload*/
        /*daload*/
        /*aaload*/
        /*baload*/
        /*caload*/
        /*saload*/
        /*istore*/
        /*lstore*/
        /*fstore*/
        /*dstore*/
        /*astore*/
        /*istore_0*/
        /*istore_1*/
        /*istore_2*/
        /*istore_3*/
        /*lstore_0*/
        /*lstore_1*/
        /*lstore_2*/
        /*lstore_3*/
        /*fstore_0*/
        /*fstore_1*/
        /*fstore_2*/
        /*fstore_3*/
        /*dstore_0*/
        /*dstore_1*/
        /*dstore_2*/
        /*dstore_3*/
        /*astore_0*/
        /*astore_1*/
        /*astore_2*/
        /*astore_3*/
        /*iastore*/
        /*lastore*/
        /*fastore*/
        /*dastore*/
        /*aastore*/
        /*bastore*/
        /*castore*/
        /*sastore*/
        /*pop*/
        /*pop2*/
        /*dup*/
        /*dup_x1*/
        /*dup_x2*/
        /*dup2*/
        /*dup2_x1*/
        /*dup2_x2*/
        /*swap*/
        /*iadd*/
        /*ladd*/
        /*fadd*/
        /*dadd*/
        /*isub*/
        /*lsub*/
        /*fsub*/
        /*dsub*/
        /*imul*/
        /*lmul*/
        /*fmul*/
        /*dmul*/
        /*idiv*/
        /*ldiv*/
        /*fdiv*/
        /*ddiv*/
        /*irem*/
        /*lrem*/
        /*frem*/
        /*drem*/
        /*ineg*/
        /*lneg*/
        /*fneg*/
        /*dneg*/
        /*ishl*/
        /*lshl*/
        /*ishr*/
        /*lshr*/
        /*iushr*/
        /*lushr*/
        /*iand*/
        /*land*/
        /*ior*/
        /*lor*/
        /*ixor*/
        /*lxor*/
        /*iinc*/
        /*i2l*/
        /*i2f*/
        /*i2d*/
        /*l2i*/
        /*l2f*/
        /*l2d*/
        /*f2i*/
        /*f2l*/
        /*f2d*/
        /*d2i*/
        /*d2l*/
        /*d2f*/
        /*i2b*/
        /*i2c*/
        /*i2s*/
        /*lcmp*/
        /*fcmpl*/
        /*fcmpg*/
        /*dcmpl*/
        /*dcmpg*/
        /*ifeq*/
        /*ifne*/
        /*iflt*/
        /*ifge*/
        /*ifgt*/
        /*ifle*/
        /*if_icmpeq*/
        /*if_icmpne*/
        /*if_icmplt*/
        /*if_icmpge*/
        /*if_icmpgt*/
        /*if_icmple*/
        /*if_acmpeq*/
        /*if_acmpne*/
        /*goto*/
        /*jsr*/
        /*ret*/
        /*tableswitch*/
        /*lookupswitch*/
        /*ireturn*/
        /*lreturn*/
        /*freturn*/
        /*dreturn*/
        /*areturn*/
        /*return*/
        /*getstatic*/
        /*putstatic*/
        /*getfield*/
        /*putfield*/
        /*invokevirtual*/
        /*invokespecial*/
        /*invokestatic*/
        /*invokeinterface*/
        /*invokedynamic*/
        /*new*/
        /*newarray*/
        /*anewarray*/
        /*arraylength*/
        /*athrow*/
        /*checkcast*/
        /*instanceof*/
        /*monitorenter*/
        /*monitorexit*/
        /*wide*/
        /*multianewarray*/
        /*ifnull*/
        /*ifnonnull*/
        /*goto_w*/
        /*jsr_w*/
        /*breakpoint*/
        /*impdep1*/
        /*impdep2*/
        //should be 17
        // TODO: mutable public array!!
    }

    public static class ConstantsConstants
    {
    }
}