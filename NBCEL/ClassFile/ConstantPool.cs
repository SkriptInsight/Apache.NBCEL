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
using System.Text;
using Apache.NBCEL.Generic;
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>
	///     This class represents the constant pool, i.e., a table of constants, of
	///     a parsed classfile.
	/// </summary>
	/// <remarks>
	///     This class represents the constant pool, i.e., a table of constants, of
	///     a parsed classfile. It may contain null references, due to the JVM
	///     specification that skips an entry after an 8-byte constant (double,
	///     long) entry.  Those interested in generating constant pools
	///     programatically should see
	///     <a href="../generic/ConstantPoolGen.html">
	///         ConstantPoolGen
	///     </a>
	///     .
	/// </remarks>
	/// <seealso cref="Constant" />
	/// <seealso cref="ConstantPoolGen" />
	public class ConstantPool : ICloneable, Node
    {
        private Constant[] constant_pool;

        /// <param name="constant_pool">Array of constants</param>
        public ConstantPool(Constant[] constant_pool)
        {
            this.constant_pool = constant_pool;
        }

        /// <summary>Reads constants from given input stream.</summary>
        /// <param name="input">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="ClassFormatException" />
        /// <exception cref="NBCEL.classfile.ClassFormatException" />
        public ConstantPool(DataInput input)
        {
            byte tag;
            var constant_pool_count = input.ReadUnsignedShort();
            constant_pool = new Constant[constant_pool_count];
            /* constant_pool[0] is unused by the compiler and may be used freely
            * by the implementation.
            */
            for (var i = 1; i < constant_pool_count; i++)
            {
                constant_pool[i] = Constant.ReadConstant(input);
                /* Quote from the JVM specification:
                * "All eight byte constants take up two spots in the constant pool.
                * If this is the n'th byte in the constant pool, then the next item
                * will be numbered n+2"
                *
                * Thus we have to increment the index counter.
                */
                tag = constant_pool[i].GetTag();
                if (tag == Const.CONSTANT_Double || tag == Const.CONSTANT_Long) i++;
            }
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
        public virtual void Accept(Visitor v)
        {
            v.VisitConstantPool(this);
        }

        /// <summary>Resolves constant to a string representation.</summary>
        /// <param name="c">Constant to be printed</param>
        /// <returns>String representation</returns>
        /// <exception cref="ClassFormatException" />
        public virtual string ConstantToString(Constant c)
        {
            string str;
            int i;
            var tag = c.GetTag();
            switch (tag)
            {
                case Const.CONSTANT_Class:
                {
                    i = ((ConstantClass) c).GetNameIndex();
                    c = GetConstant(i, Const.CONSTANT_Utf8);
                    str = Utility.CompactClassName(((ConstantUtf8) c).GetBytes(), false);
                    break;
                }

                case Const.CONSTANT_String:
                {
                    i = ((ConstantString) c).GetStringIndex();
                    c = GetConstant(i, Const.CONSTANT_Utf8);
                    str = "\"" + Escape(((ConstantUtf8) c).GetBytes()) + "\"";
                    break;
                }

                case Const.CONSTANT_Utf8:
                {
                    str = ((ConstantUtf8) c).GetBytes();
                    break;
                }

                case Const.CONSTANT_Double:
                {
                    str = ((ConstantDouble) c).GetBytes().ToString();
                    break;
                }

                case Const.CONSTANT_Float:
                {
                    str = ((ConstantFloat) c).GetBytes().ToString();
                    break;
                }

                case Const.CONSTANT_Long:
                {
                    str = ((ConstantLong) c).GetBytes().ToString();
                    break;
                }

                case Const.CONSTANT_Integer:
                {
                    str = ((ConstantInteger) c).GetBytes().ToString();
                    break;
                }

                case Const.CONSTANT_NameAndType:
                {
                    str = ConstantToString(((ConstantNameAndType) c).GetNameIndex(), Const
                              .CONSTANT_Utf8) + " " + ConstantToString(((ConstantNameAndType) c
                              ).GetSignatureIndex(), Const.CONSTANT_Utf8);
                    break;
                }

                case Const.CONSTANT_InterfaceMethodref:
                case Const.CONSTANT_Methodref:
                case Const.CONSTANT_Fieldref:
                {
                    str = ConstantToString(((ConstantCP) c).GetClassIndex(), Const
                              .CONSTANT_Class) + "." + ConstantToString(((ConstantCP) c).GetNameAndTypeIndex
                              (), Const.CONSTANT_NameAndType);
                    break;
                }

                case Const.CONSTANT_MethodHandle:
                {
                    // Note that the ReferenceIndex may point to a Fieldref, Methodref or
                    // InterfaceMethodref - so we need to peek ahead to get the actual type.
                    var cmh = (ConstantMethodHandle)
                        c;
                    str = Const.GetMethodHandleName(cmh.GetReferenceKind()) + " " + ConstantToString
                              (cmh.GetReferenceIndex(), GetConstant(cmh.GetReferenceIndex()).GetTag());
                    break;
                }

                case Const.CONSTANT_MethodType:
                {
                    var cmt = (ConstantMethodType) c;
                    str = ConstantToString(cmt.GetDescriptorIndex(), Const.CONSTANT_Utf8);
                    break;
                }

                case Const.CONSTANT_InvokeDynamic:
                {
                    var cid = (ConstantInvokeDynamic
                        ) c;
                    str = cid.GetBootstrapMethodAttrIndex() + ":" + ConstantToString(cid.GetNameAndTypeIndex
                              (), Const.CONSTANT_NameAndType);
                    break;
                }

                case Const.CONSTANT_Module:
                {
                    i = ((ConstantModule) c).GetNameIndex();
                    c = GetConstant(i, Const.CONSTANT_Utf8);
                    str = Utility.CompactClassName(((ConstantUtf8) c).GetBytes(), false);
                    break;
                }

                case Const.CONSTANT_Package:
                {
                    i = ((ConstantPackage) c).GetNameIndex();
                    c = GetConstant(i, Const.CONSTANT_Utf8);
                    str = Utility.CompactClassName(((ConstantUtf8) c).GetBytes(), false);
                    break;
                }

                default:
                {
                    // Never reached
                    throw new Exception("Unknown constant type " + tag);
                }
            }

            return str;
        }

        private static string Escape(string str)
        {
            var len = str.Length;
            var buf = new StringBuilder(len + 5);
            var ch = str.ToCharArray();
            for (var i = 0; i < len; i++)
                switch (ch[i])
                {
                    case '\n':
                    {
                        buf.Append("\\n");
                        break;
                    }

                    case '\r':
                    {
                        buf.Append("\\r");
                        break;
                    }

                    case '\t':
                    {
                        buf.Append("\\t");
                        break;
                    }

                    case '\b':
                    {
                        buf.Append("\\b");
                        break;
                    }

                    case '"':
                    {
                        buf.Append("\\\"");
                        break;
                    }

                    default:
                    {
                        buf.Append(ch[i]);
                        break;
                    }
                }

            return buf.ToString();
        }

        /// <summary>
        ///     Retrieves constant at `index' from constant pool and resolve it to
        ///     a string representation.
        /// </summary>
        /// <param name="index">of constant in constant pool</param>
        /// <param name="tag">expected type</param>
        /// <returns>String representation</returns>
        /// <exception cref="ClassFormatException" />
        public virtual string ConstantToString(int index, byte tag)
        {
            var c = GetConstant(index, tag);
            return ConstantToString(c);
        }

        /// <summary>Dump constant pool to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public virtual void Dump(DataOutputStream file)
        {
            file.WriteShort(constant_pool.Length);
            for (var i = 1; i < constant_pool.Length; i++)
                if (constant_pool[i] != null)
                    constant_pool[i].Dump(file);
        }

        /// <summary>Gets constant from constant pool.</summary>
        /// <param name="index">Index in constant pool</param>
        /// <returns>Constant value</returns>
        /// <seealso cref="Constant" />
        public virtual Constant GetConstant(int index)
        {
            if (index >= constant_pool.Length || index < 0)
                throw new ClassFormatException("Invalid constant pool reference: "
                                               + index + ". Constant pool size is: " + constant_pool.Length);
            return constant_pool[index];
        }

        /// <summary>
        ///     Gets constant from constant pool and check whether it has the
        ///     expected type.
        /// </summary>
        /// <param name="index">Index in constant pool</param>
        /// <param name="tag">Tag of expected constant, i.e., its type</param>
        /// <returns>Constant value</returns>
        /// <seealso cref="Constant" />
        /// <exception cref="ClassFormatException" />
        /// <exception cref="ClassFormatException" />
        public virtual Constant GetConstant(int index, byte tag)
        {
            Constant c;
            c = GetConstant(index);
            if (c == null)
                throw new ClassFormatException("Constant pool at index " + index
                                                                         + " is null.");
            if (c.GetTag() != tag)
                throw new ClassFormatException("Expected class `" + Const.GetConstantName
                                                   (tag) + "' at index " + index + " and got " + c);
            return c;
        }

        /// <returns>Array of constants.</returns>
        /// <seealso cref="Constant" />
        public virtual Constant[] GetConstantPool()
        {
            return constant_pool;
        }

        /// <summary>
        ///     Gets string from constant pool and bypass the indirection of
        ///     `ConstantClass' and `ConstantString' objects.
        /// </summary>
        /// <remarks>
        ///     Gets string from constant pool and bypass the indirection of
        ///     `ConstantClass' and `ConstantString' objects. I.e. these classes have
        ///     an index field that points to another entry of the constant pool of
        ///     type `ConstantUtf8' which contains the real data.
        /// </remarks>
        /// <param name="index">Index in constant pool</param>
        /// <param name="tag">
        ///     Tag of expected constant, either ConstantClass or ConstantString
        /// </param>
        /// <returns>Contents of string reference</returns>
        /// <seealso cref="ConstantClass" />
        /// <seealso cref="ConstantString" />
        /// <exception cref="ClassFormatException" />
        /// <exception cref="ClassFormatException" />
        public virtual string GetConstantString(int index, byte tag)
        {
            Constant c;
            int i;
            c = GetConstant(index, tag);
            switch (tag)
            {
                case Const.CONSTANT_Class:
                {
                    /* This switch() is not that elegant, since the four classes have the
                    * same contents, they just differ in the name of the index
                    * field variable.
                    * But we want to stick to the JVM naming conventions closely though
                    * we could have solved these more elegantly by using the same
                    * variable name or by subclassing.
                    */
                    i = ((ConstantClass) c).GetNameIndex();
                    break;
                }

                case Const.CONSTANT_String:
                {
                    i = ((ConstantString) c).GetStringIndex();
                    break;
                }

                case Const.CONSTANT_Module:
                {
                    i = ((ConstantModule) c).GetNameIndex();
                    break;
                }

                case Const.CONSTANT_Package:
                {
                    i = ((ConstantPackage) c).GetNameIndex();
                    break;
                }

                default:
                {
                    throw new Exception("getConstantString called with illegal tag " + tag);
                }
            }

            // Finally get the string from the constant pool
            c = GetConstant(i, Const.CONSTANT_Utf8);
            return ((ConstantUtf8) c).GetBytes();
        }

        /// <returns>Length of constant pool.</returns>
        public virtual int GetLength()
        {
            return constant_pool == null ? 0 : constant_pool.Length;
        }

        /// <param name="constant">Constant to set</param>
        public virtual void SetConstant(int index, Constant constant)
        {
            constant_pool[index] = constant;
        }

        /// <param name="constant_pool" />
        public virtual void SetConstantPool(Constant[] constant_pool)
        {
            this.constant_pool = constant_pool;
        }

        /// <returns>String representation.</returns>
        public override string ToString()
        {
            var buf = new StringBuilder();
            for (var i = 1; i < constant_pool.Length; i++)
                buf.Append(i).Append(")").Append(constant_pool[i]).Append("\n");
            return buf.ToString();
        }

        /// <returns>deep copy of this constant pool</returns>
        public virtual ConstantPool Copy()
        {
            ConstantPool c = null;
            c = (ConstantPool) MemberwiseClone();
            c.constant_pool = new Constant[constant_pool.Length];
            for (var i = 1; i < constant_pool.Length; i++)
                if (constant_pool[i] != null)
                    c.constant_pool[i] = constant_pool[i].Copy();
            // TODO should this throw?
            return c;
        }
    }
}