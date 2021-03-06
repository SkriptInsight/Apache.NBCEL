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

using System.Text;
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>
	///     This class is derived from <em>Attribute</em> and denotes that this class
	///     is an Inner class of another.
	/// </summary>
	/// <remarks>
	///     This class is derived from <em>Attribute</em> and denotes that this class
	///     is an Inner class of another.
	///     to the source file of this class.
	///     It is instantiated from the <em>Attribute.readAttribute()</em> method.
	/// </remarks>
	/// <seealso cref="Attribute" />
	public sealed class InnerClasses : Attribute
    {
        private InnerClass[] inner_classes;

        /// <summary>Initialize from another object.</summary>
        /// <remarks>
        ///     Initialize from another object. Note that both objects use the same
        ///     references (shallow copy). Use clone() for a physical copy.
        /// </remarks>
        public InnerClasses(InnerClasses c)
            : this(c.GetNameIndex(), c.GetLength(), c.GetInnerClasses(), c.GetConstantPool())
        {
        }

        /// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="inner_classes">array of inner classes attributes</param>
        /// <param name="constant_pool">Array of constants</param>
        public InnerClasses(int name_index, int length, InnerClass[] inner_classes
            , ConstantPool constant_pool)
            : base(Const.ATTR_INNER_CLASSES, name_index, length, constant_pool)
        {
            this.inner_classes = inner_classes != null
                ? inner_classes
                : new InnerClass
                    [0];
        }

        /// <summary>Construct object from input stream.</summary>
        /// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal InnerClasses(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : this(name_index, length, (InnerClass[]) null, constant_pool)
        {
            var number_of_classes = input.ReadUnsignedShort();
            inner_classes = new InnerClass[number_of_classes];
            for (var i = 0; i < number_of_classes; i++) inner_classes[i] = new InnerClass(input);
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
            v.VisitInnerClasses(this);
        }

        /// <summary>Dump source file attribute to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            base.Dump(file);
            file.WriteShort(inner_classes.Length);
            foreach (var inner_class in inner_classes) inner_class.Dump(file);
        }

        /// <returns>array of inner class "records"</returns>
        public InnerClass[] GetInnerClasses()
        {
            return inner_classes;
        }

        /// <param name="inner_classes">the array of inner classes</param>
        public void SetInnerClasses(InnerClass[] inner_classes)
        {
            this.inner_classes = inner_classes != null
                ? inner_classes
                : new InnerClass
                    [0];
        }

        /// <returns>String representation.</returns>
        public override string ToString()
        {
            var buf = new StringBuilder();
            buf.Append("InnerClasses(");
            buf.Append(inner_classes.Length);
            buf.Append("):\n");
            foreach (var inner_class in inner_classes) buf.Append(inner_class.ToString(GetConstantPool())).Append("\n");
            return buf.Substring(0, buf.Length - 1);
        }

        // remove the last newline
        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            // TODO this could be recoded to use a lower level constructor after creating a copy of the inner classes
            var c = (InnerClasses) Clone();
            c.inner_classes = new InnerClass[inner_classes.Length];
            for (var i = 0; i < inner_classes.Length; i++) c.inner_classes[i] = inner_classes[i].Copy();
            c.SetConstantPool(_constant_pool);
            return c;
        }
    }
}