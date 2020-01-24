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
*/

using System;
using System.Text;
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>
	///     This class is derived from <em>Attribute</em> and records the classes and interfaces that
	///     are authorized to claim membership in the nest hosted by the current class or interface.
	/// </summary>
	/// <remarks>
	///     This class is derived from <em>Attribute</em> and records the classes and interfaces that
	///     are authorized to claim membership in the nest hosted by the current class or interface.
	///     There may be at most one NestMembers attribute in a ClassFile structure.
	/// </remarks>
	/// <seealso cref="Attribute" />
	public sealed class NestMembers : Attribute
    {
        private int[] classes;

        /// <summary>Initialize from another object.</summary>
        /// <remarks>
        ///     Initialize from another object. Note that both objects use the same
        ///     references (shallow copy). Use copy() for a physical copy.
        /// </remarks>
        public NestMembers(NestMembers c)
            : this(c.GetNameIndex(), c.GetLength(), c.GetClasses(), c.GetConstantPool())
        {
        }

        /// <param name="name_index">Index in constant pool</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="classes">Table of indices in constant pool</param>
        /// <param name="constant_pool">Array of constants</param>
        public NestMembers(int name_index, int length, int[] classes, ConstantPool
            constant_pool)
            : base(Const.ATTR_NEST_MEMBERS, name_index, length, constant_pool)
        {
            this.classes = classes != null ? classes : new int[0];
        }

        /// <summary>Construct object from input stream.</summary>
        /// <param name="name_index">Index in constant pool</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal NestMembers(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : this(name_index, length, (int[]) null, constant_pool)
        {
            var number_of_classes = input.ReadUnsignedShort();
            classes = new int[number_of_classes];
            for (var i = 0; i < number_of_classes; i++) classes[i] = input.ReadUnsignedShort();
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
        public override void Accept(Visitor v)
        {
            v.VisitNestMembers(this);
        }

        /// <summary>Dump NestMembers attribute to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            base.Dump(file);
            file.WriteShort(classes.Length);
            foreach (var index in classes) file.WriteShort(index);
        }

        /// <returns>array of indices into constant pool of class names.</returns>
        public int[] GetClasses()
        {
            return classes;
        }

        /// <returns>Length of classes table.</returns>
        public int GetNumberClasses()
        {
            return classes == null ? 0 : classes.Length;
        }

        /// <returns>string array of class names</returns>
        public string[] GetClassNames()
        {
            var names = new string[classes.Length];
            for (var i = 0; i < classes.Length; i++)
                names[i] = GetConstantPool().GetConstantString(classes[i], Const.CONSTANT_Class
                ).Replace('/', '.');
            return names;
        }

        /// <param name="classes">
        ///     the list of class indexes
        ///     Also redefines number_of_classes according to table length.
        /// </param>
        public void SetClasses(int[] classes)
        {
            this.classes = classes != null ? classes : new int[0];
        }

        /// <returns>String representation, i.e., a list of classes.</returns>
        public override string ToString()
        {
            var buf = new StringBuilder();
            buf.Append("NestMembers(");
            buf.Append(classes.Length);
            buf.Append("):\n");
            foreach (var index in classes)
            {
                var class_name = GetConstantPool().GetConstantString(index, Const.CONSTANT_Class
                );
                buf.Append("  ").Append(Utility.CompactClassName(class_name, false
                )).Append("\n");
            }

            return buf.Substring(0, buf.Length - 1);
        }

        // remove the last newline
        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            var c = (NestMembers) Clone();
            if (classes != null)
            {
                c.classes = new int[classes.Length];
                Array.Copy(classes, 0, c.classes, 0, classes.Length);
            }

            c.SetConstantPool(_constant_pool);
            return c;
        }
    }
}