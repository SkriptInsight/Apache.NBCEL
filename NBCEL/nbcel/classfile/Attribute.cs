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
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>Abstract super class for <em>Attribute</em> objects.</summary>
	/// <remarks>
	///     Abstract super class for <em>Attribute</em> objects. Currently the
	///     <em>ConstantValue</em>, <em>SourceFile</em>, <em>Code</em>,
	///     <em>Exceptiontable</em>, <em>LineNumberTable</em>,
	///     <em>LocalVariableTable</em>, <em>InnerClasses</em> and
	///     <em>Synthetic</em> attributes are supported. The <em>Unknown</em>
	///     attribute stands for non-standard-attributes.
	/// </remarks>
	/// <seealso cref="ConstantValue" />
	/// <seealso cref="SourceFile" />
	/// <seealso cref="Code" />
	/// <seealso cref="Unknown" />
	/// <seealso cref="ExceptionTable" />
	/// <seealso cref="LineNumberTable" />
	/// <seealso cref="LocalVariableTable" />
	/// <seealso cref="InnerClasses" />
	/// <seealso cref="Synthetic" />
	/// <seealso cref="Deprecated" />
	/// <seealso cref="Signature" />
	public abstract class Attribute : ICloneable, Node
    {
        private static readonly bool debug = Sharpen.System.GetBoolean(typeof(Attribute
                                                                       ).Name + ".debug");

        private static readonly IDictionary<string, object> readers
            = new Dictionary<string, object>();

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal ConstantPool constant_pool;

        [Obsolete(@"(since 6.0) (since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal int length;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal int name_index;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal byte tag;

        protected internal Attribute(byte tag, int name_index, int length, ConstantPool
            constant_pool)
        {
            // Points to attribute name in constant pool TODO make private (has getter & setter)
            // Content length of attribute field TODO make private (has getter & setter)
            // Tag to distinguish subclasses TODO make private & final; supposed to be immutable
            // TODO make private (has getter & setter)
            this.tag = tag;
            this.name_index = name_index;
            this.length = length;
            this.constant_pool = constant_pool;
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
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
        public abstract void Accept(Visitor v);

        // Debugging on/off
        /// <summary>
        ///     Add an Attribute reader capable of parsing (user-defined) attributes
        ///     named "name".
        /// </summary>
        /// <remarks>
        ///     Add an Attribute reader capable of parsing (user-defined) attributes
        ///     named "name". You should not add readers for the standard attributes such
        ///     as "LineNumberTable", because those are handled internally.
        /// </remarks>
        /// <param name="name">the name of the attribute as stored in the class file</param>
        /// <param name="r">the reader object</param>
        [Obsolete(@"(6.0) Use AddAttributeReader(string, UnknownAttributeReader) instead"
        )]
        public static void AddAttributeReader(string name, AttributeReader
            r)
        {
            Collections.Put(readers, name, r);
        }

        /// <summary>
        ///     Add an Attribute reader capable of parsing (user-defined) attributes
        ///     named "name".
        /// </summary>
        /// <remarks>
        ///     Add an Attribute reader capable of parsing (user-defined) attributes
        ///     named "name". You should not add readers for the standard attributes such
        ///     as "LineNumberTable", because those are handled internally.
        /// </remarks>
        /// <param name="name">the name of the attribute as stored in the class file</param>
        /// <param name="r">the reader object</param>
        public static void AddAttributeReader(string name, UnknownAttributeReader
            r)
        {
            Collections.Put(readers, name, r);
        }

        protected internal static void Println(string msg)
        {
            if (debug) Console.Error.WriteLine(msg);
        }

        /// <summary>Class method reads one attribute from the input data stream.</summary>
        /// <remarks>
        ///     Class method reads one attribute from the input data stream. This method
        ///     must not be accessible from the outside. It is called by the Field and
        ///     Method constructor methods.
        /// </remarks>
        /// <seealso cref="Field" />
        /// <seealso cref="Method" />
        /// <param name="file">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <returns>Attribute</returns>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="ClassFormatException" />
        /// <since>6.0</since>
        /// <exception cref="NBCEL.classfile.ClassFormatException" />
        public static Attribute ReadAttribute(DataInput file, ConstantPool
            constant_pool)
        {
            var tag = Const.ATTR_UNKNOWN;
            // Unknown attribute
            // Get class name from constant pool via `name_index' indirection
            var name_index = file.ReadUnsignedShort();
            var c = (ConstantUtf8) constant_pool.GetConstant
                (name_index, Const.CONSTANT_Utf8);
            var name = c.GetBytes();
            // Length of data in bytes
            var length = file.ReadInt();
            // Compare strings to find known attribute
            for (byte i = 0; (sbyte) i < Const.KNOWN_ATTRIBUTES; i++)
                if (name.Equals(Const.GetAttributeName(i)))
                {
                    tag = i;
                    // found!
                    break;
                }

            switch (tag)
            {
                case Const.ATTR_UNKNOWN:
                {
                    // Call proper constructor, depending on `tag'
                    var r = readers.GetOrNull(name);
                    if (r is UnknownAttributeReader)
                        return ((UnknownAttributeReader) r).CreateAttribute(name_index, length
                            , file, constant_pool);
                    return new Unknown(name_index, length, file, constant_pool);
                }

                case Const.ATTR_CONSTANT_VALUE:
                {
                    return new ConstantValue(name_index, length, file, constant_pool);
                }

                case Const.ATTR_SOURCE_FILE:
                {
                    return new SourceFile(name_index, length, file, constant_pool);
                }

                case Const.ATTR_CODE:
                {
                    return new Code(name_index, length, file, constant_pool);
                }

                case Const.ATTR_EXCEPTIONS:
                {
                    return new ExceptionTable(name_index, length, file, constant_pool
                    );
                }

                case Const.ATTR_LINE_NUMBER_TABLE:
                {
                    return new LineNumberTable(name_index, length, file, constant_pool
                    );
                }

                case Const.ATTR_LOCAL_VARIABLE_TABLE:
                {
                    return new LocalVariableTable(name_index, length, file, constant_pool
                    );
                }

                case Const.ATTR_INNER_CLASSES:
                {
                    return new InnerClasses(name_index, length, file, constant_pool);
                }

                case Const.ATTR_SYNTHETIC:
                {
                    return new Synthetic(name_index, length, file, constant_pool);
                }

                case Const.ATTR_DEPRECATED:
                {
                    return new Deprecated(name_index, length, file, constant_pool);
                }

                case Const.ATTR_PMG:
                {
                    return new PMGClass(name_index, length, file, constant_pool);
                }

                case Const.ATTR_SIGNATURE:
                {
                    return new Signature(name_index, length, file, constant_pool);
                }

                case Const.ATTR_STACK_MAP:
                {
                    // old style stack map: unneeded for JDK5 and below;
                    // illegal(?) for JDK6 and above.  So just delete with a warning.
                    Println("Warning: Obsolete StackMap attribute ignored.");
                    return new Unknown(name_index, length, file, constant_pool);
                }

                case Const.ATTR_RUNTIME_VISIBLE_ANNOTATIONS:
                {
                    return new RuntimeVisibleAnnotations(name_index, length, file, constant_pool
                    );
                }

                case Const.ATTR_RUNTIME_INVISIBLE_ANNOTATIONS:
                {
                    return new RuntimeInvisibleAnnotations(name_index, length, file,
                        constant_pool);
                }

                case Const.ATTR_RUNTIME_VISIBLE_PARAMETER_ANNOTATIONS:
                {
                    return new RuntimeVisibleParameterAnnotations(name_index, length,
                        file, constant_pool);
                }

                case Const.ATTR_RUNTIME_INVISIBLE_PARAMETER_ANNOTATIONS:
                {
                    return new RuntimeInvisibleParameterAnnotations(name_index, length
                        , file, constant_pool);
                }

                case Const.ATTR_ANNOTATION_DEFAULT:
                {
                    return new AnnotationDefault(name_index, length, file, constant_pool
                    );
                }

                case Const.ATTR_LOCAL_VARIABLE_TYPE_TABLE:
                {
                    return new LocalVariableTypeTable(name_index, length, file, constant_pool
                    );
                }

                case Const.ATTR_ENCLOSING_METHOD:
                {
                    return new EnclosingMethod(name_index, length, file, constant_pool
                    );
                }

                case Const.ATTR_STACK_MAP_TABLE:
                {
                    // read new style stack map: StackMapTable.  The rest of the code
                    // calls this a StackMap for historical reasons.
                    return new StackMap(name_index, length, file, constant_pool);
                }

                case Const.ATTR_BOOTSTRAP_METHODS:
                {
                    return new BootstrapMethods(name_index, length, file, constant_pool
                    );
                }

                case Const.ATTR_METHOD_PARAMETERS:
                {
                    return new MethodParameters(name_index, length, file, constant_pool
                    );
                }

                case Const.ATTR_MODULE:
                {
                    return new Module(name_index, length, file, constant_pool);
                }

                case Const.ATTR_MODULE_PACKAGES:
                {
                    return new ModulePackages(name_index, length, file, constant_pool
                    );
                }

                case Const.ATTR_MODULE_MAIN_CLASS:
                {
                    return new ModuleMainClass(name_index, length, file, constant_pool
                    );
                }

                case Const.ATTR_NEST_HOST:
                {
                    return new NestHost(name_index, length, file, constant_pool);
                }

                case Const.ATTR_NEST_MEMBERS:
                {
                    return new NestMembers(name_index, length, file, constant_pool);
                }

                default:
                {
                    // Never reached
                    throw new InvalidOperationException("Unrecognized attribute type tag parsed: "
                                                        + tag);
                }
            }
        }

        /// <summary>Class method reads one attribute from the input data stream.</summary>
        /// <remarks>
        ///     Class method reads one attribute from the input data stream. This method
        ///     must not be accessible from the outside. It is called by the Field and
        ///     Method constructor methods.
        /// </remarks>
        /// <seealso cref="Field" />
        /// <seealso cref="Method" />
        /// <param name="file">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <returns>Attribute</returns>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="ClassFormatException" />
        /// <exception cref="NBCEL.classfile.ClassFormatException" />
        public static Attribute ReadAttribute(DataInputStream file
            , ConstantPool constant_pool)
        {
            return ReadAttribute((DataInput) file, constant_pool);
        }

        /// <summary>Remove attribute reader</summary>
        /// <param name="name">the name of the attribute as stored in the class file</param>
        public static void RemoveAttributeReader(string name)
        {
            Collections.Remove(readers, name);
        }

        /// <summary>
        ///     Use copy() if you want to have a deep copy(), i.e., with all references
        ///     copied correctly.
        /// </summary>
        /// <returns>shallow copy of this attribute</returns>
        public virtual object Clone()
        {
            Attribute attr = null;
            attr = (Attribute) MemberwiseClone();
            // never happens
            return attr;
        }

        /// <returns>deep copy of this attribute</returns>
        public abstract Attribute Copy(ConstantPool _constant_pool
        );

        /// <summary>Dump attribute to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public virtual void Dump(DataOutputStream file)
        {
            file.WriteShort(name_index);
            file.WriteInt(length);
        }

        /// <returns>Constant pool used by this object.</returns>
        /// <seealso cref="ConstantPool" />
        public ConstantPool GetConstantPool()
        {
            return constant_pool;
        }

        /// <returns>Length of attribute field in bytes.</returns>
        public int GetLength()
        {
            return length;
        }

        /// <returns>Name of attribute</returns>
        /// <since>6.0</since>
        public virtual string GetName()
        {
            var c = (ConstantUtf8) constant_pool.GetConstant
                (name_index, Const.CONSTANT_Utf8);
            return c.GetBytes();
        }

        /// <returns>Name index in constant pool of attribute name.</returns>
        public int GetNameIndex()
        {
            return name_index;
        }

        /// <returns>
        ///     Tag of attribute, i.e., its type. Value may not be altered, thus there is no setTag() method.
        /// </returns>
        public byte GetTag()
        {
            return tag;
        }

        /// <param name="constant_pool">Constant pool to be used for this object.</param>
        /// <seealso cref="ConstantPool" />
        public void SetConstantPool(ConstantPool constant_pool)
        {
            this.constant_pool = constant_pool;
        }

        /// <param name="length">length in bytes.</param>
        public void SetLength(int length)
        {
            this.length = length;
        }

        /// <param name="name_index">of attribute.</param>
        public void SetNameIndex(int name_index)
        {
            this.name_index = name_index;
        }

        /// <returns>attribute name.</returns>
        public override string ToString()
        {
            return Const.GetAttributeName(tag);
        }
    }
}