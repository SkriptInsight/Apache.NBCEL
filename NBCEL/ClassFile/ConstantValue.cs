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
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>
	///     This class is derived from <em>Attribute</em> and represents a constant
	///     value, i.e., a default value for initializing a class field.
	/// </summary>
	/// <remarks>
	///     This class is derived from <em>Attribute</em> and represents a constant
	///     value, i.e., a default value for initializing a class field.
	///     This class is instantiated by the <em>Attribute.readAttribute()</em> method.
	/// </remarks>
	/// <seealso cref="Attribute" />
	public sealed class ConstantValue : Attribute
    {
        private int constantvalue_index;

        /// <summary>Initialize from another object.</summary>
        /// <remarks>
        ///     Initialize from another object. Note that both objects use the same
        ///     references (shallow copy). Use clone() for a physical copy.
        /// </remarks>
        public ConstantValue(ConstantValue c)
            : this(c.GetNameIndex(), c.GetLength(), c.GetConstantValueIndex(), c.GetConstantPool
                ())
        {
        }

        /// <summary>Construct object from input stream.</summary>
        /// <param name="name_index">Name index in constant pool</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal ConstantValue(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : this(name_index, length, input.ReadUnsignedShort(), constant_pool)
        {
        }

        /// <param name="name_index">Name index in constant pool</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="constantvalue_index">Index in constant pool</param>
        /// <param name="constant_pool">Array of constants</param>
        public ConstantValue(int name_index, int length, int constantvalue_index, ConstantPool
            constant_pool)
            : base(Const.ATTR_CONSTANT_VALUE, name_index, length, constant_pool)
        {
            this.constantvalue_index = constantvalue_index;
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
            v.VisitConstantValue(this);
        }

        /// <summary>Dump constant value attribute to file stream on binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            base.Dump(file);
            file.WriteShort(constantvalue_index);
        }

        /// <returns>Index in constant pool of constant value.</returns>
        public int GetConstantValueIndex()
        {
            return constantvalue_index;
        }

        /// <param name="constantvalue_index">
        ///     the index info the constant pool of this constant value
        /// </param>
        public void SetConstantValueIndex(int constantvalue_index)
        {
            this.constantvalue_index = constantvalue_index;
        }

        /// <returns>String representation of constant value.</returns>
        public override string ToString()
        {
            var c = GetConstantPool().GetConstant(constantvalue_index
            );
            string buf;
            int i;
            switch (c.GetTag())
            {
                case Const.CONSTANT_Long:
                {
                    // Print constant to string depending on its type
                    buf = ((ConstantLong) c).GetBytes().ToString();
                    break;
                }

                case Const.CONSTANT_Float:
                {
                    buf = ((ConstantFloat) c).GetBytes().ToString();
                    break;
                }

                case Const.CONSTANT_Double:
                {
                    buf = ((ConstantDouble) c).GetBytes().ToString();
                    break;
                }

                case Const.CONSTANT_Integer:
                {
                    buf = ((ConstantInteger) c).GetBytes().ToString();
                    break;
                }

                case Const.CONSTANT_String:
                {
                    i = ((ConstantString) c).GetStringIndex();
                    c = GetConstantPool().GetConstant(i, Const.CONSTANT_Utf8);
                    buf = "\"" + Utility.ConvertString(((ConstantUtf8
                              ) c).GetBytes()) + "\"";
                    break;
                }

                default:
                {
                    throw new InvalidOperationException("Type of ConstValue invalid: " + c);
                }
            }

            return buf;
        }

        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            var c = (ConstantValue) Clone();
            c.SetConstantPool(_constant_pool);
            return c;
        }
    }
}