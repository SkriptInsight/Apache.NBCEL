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

using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	///     This class is derived from <em>Attribute</em> and represents a reference
	///     to the source file of this class.
	/// </summary>
	/// <remarks>
	///     This class is derived from <em>Attribute</em> and represents a reference
	///     to the source file of this class.  At most one SourceFile attribute
	///     should appear per classfile.  The intention of this class is that it is
	///     instantiated from the <em>Attribute.readAttribute()</em> method.
	/// </remarks>
	/// <seealso cref="Attribute" />
	public sealed class SourceFile : Attribute
    {
        private int sourcefile_index;

        /// <summary>Initialize from another object.</summary>
        /// <remarks>
        ///     Initialize from another object. Note that both objects use the same
        ///     references (shallow copy). Use clone() for a physical copy.
        /// </remarks>
        public SourceFile(SourceFile c)
            : this(c.GetNameIndex(), c.GetLength(), c.GetSourceFileIndex(), c.GetConstantPool
                ())
        {
        }

        /// <summary>Construct object from input stream.</summary>
        /// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal SourceFile(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : this(name_index, length, input.ReadUnsignedShort(), constant_pool)
        {
        }

        /// <param name="name_index">
        ///     Index in constant pool to CONSTANT_Utf8, which
        ///     should represent the string "SourceFile".
        /// </param>
        /// <param name="length">Content length in bytes, the value should be 2.</param>
        /// <param name="constant_pool">
        ///     The constant pool that this attribute is
        ///     associated with.
        /// </param>
        /// <param name="sourcefile_index">
        ///     Index in constant pool to CONSTANT_Utf8.  This
        ///     string will be interpreted as the name of the file from which this
        ///     class was compiled.  It will not be interpreted as indicating the name
        ///     of the directory contqining the file or an absolute path; this
        ///     information has to be supplied the consumer of this attribute - in
        ///     many cases, the JVM.
        /// </param>
        public SourceFile(int name_index, int length, int sourcefile_index, ConstantPool
            constant_pool)
            : base(Const.ATTR_SOURCE_FILE, name_index, length, constant_pool)
        {
            this.sourcefile_index = sourcefile_index;
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
            v.VisitSourceFile(this);
        }

        /// <summary>Dump source file attribute to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            base.Dump(file);
            file.WriteShort(sourcefile_index);
        }

        /// <returns>Index in constant pool of source file name.</returns>
        public int GetSourceFileIndex()
        {
            return sourcefile_index;
        }

        /// <param name="sourcefile_index" />
        public void SetSourceFileIndex(int sourcefile_index)
        {
            this.sourcefile_index = sourcefile_index;
        }

        /// <returns>Source file name.</returns>
        public string GetSourceFileName()
        {
            var c = (ConstantUtf8) GetConstantPool
                ().GetConstant(sourcefile_index, Const.CONSTANT_Utf8);
            return c.GetBytes();
        }

        /// <returns>String representation</returns>
        public override string ToString()
        {
            return "SourceFile: " + GetSourceFileName();
        }

        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            return (Attribute) Clone();
        }
    }
}