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
using java.io;
using NBCEL.util;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	///     Abstract superclass for classes to represent the different constant types
	///     in the constant pool of a class file.
	/// </summary>
	/// <remarks>
	///     Abstract superclass for classes to represent the different constant types
	///     in the constant pool of a class file. The classes keep closely to
	///     the JVM specification.
	/// </remarks>
	public abstract class Constant : ICloneable, Node
    {
        private static BCELComparator bcelComparator = new _BCELComparator_35(
        );

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal byte tag;

        internal Constant(byte tag)
        {
            /* In fact this tag is redundant since we can distinguish different
            * `Constant' objects by their type, i.e., via `instanceof'. In some
            * places we will use the tag for switch()es anyway.
            *
            * First, we want match the specification as closely as possible. Second we
            * need the tag as an index to select the corresponding class name from the
            * `CONSTANT_NAMES' array.
            */
            // TODO should be private & final
            this.tag = tag;
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

        /// <exception cref="System.IO.IOException" />
        public abstract void Dump(DataOutputStream file);

        /// <returns>
        ///     Tag of constant, i.e., its type. No setTag() method to avoid
        ///     confusion.
        /// </returns>
        public byte GetTag()
        {
            return tag;
        }

        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return Const.GetConstantName(tag) + "[" + tag + "]";
        }

        /// <returns>deep copy of this constant</returns>
        public virtual Constant Copy()
        {
            return (Constant) MemberwiseClone();
            // TODO should this throw?
            return null;
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        // never happens
        /// <summary>
        ///     Reads one constant from the given input, the type depends on a tag byte.
        /// </summary>
        /// <param name="dataInput">Input stream</param>
        /// <returns>Constant object</returns>
        /// <exception cref="System.IO.IOException">
        ///     if an I/O error occurs reading from the given
        ///     <paramref name="dataInput" />
        ///     .
        /// </exception>
        /// <exception cref="ClassFormatException">if the next byte is not recognized</exception>
        /// <since>6.0 made public</since>
        /// <exception cref="NBCEL.classfile.ClassFormatException" />
        public static Constant ReadConstant(DataInput dataInput)
        {
            var b = dataInput.ReadByte();
            switch (b)
            {
                case Const.CONSTANT_Class:
                {
                    // Read tag byte
                    return new ConstantClass(dataInput);
                }

                case Const.CONSTANT_Fieldref:
                {
                    return new ConstantFieldref(dataInput);
                }

                case Const.CONSTANT_Methodref:
                {
                    return new ConstantMethodref(dataInput);
                }

                case Const.CONSTANT_InterfaceMethodref:
                {
                    return new ConstantInterfaceMethodref(dataInput);
                }

                case Const.CONSTANT_String:
                {
                    return new ConstantString(dataInput);
                }

                case Const.CONSTANT_Integer:
                {
                    return new ConstantInteger(dataInput);
                }

                case Const.CONSTANT_Float:
                {
                    return new ConstantFloat(dataInput);
                }

                case Const.CONSTANT_Long:
                {
                    return new ConstantLong(dataInput);
                }

                case Const.CONSTANT_Double:
                {
                    return new ConstantDouble(dataInput);
                }

                case Const.CONSTANT_NameAndType:
                {
                    return new ConstantNameAndType(dataInput);
                }

                case Const.CONSTANT_Utf8:
                {
                    return ConstantUtf8.GetInstance(dataInput);
                }

                case Const.CONSTANT_MethodHandle:
                {
                    return new ConstantMethodHandle(dataInput);
                }

                case Const.CONSTANT_MethodType:
                {
                    return new ConstantMethodType(dataInput);
                }

                case Const.CONSTANT_Dynamic:
                {
                    return new ConstantDynamic(dataInput);
                }

                case Const.CONSTANT_InvokeDynamic:
                {
                    return new ConstantInvokeDynamic(dataInput);
                }

                case Const.CONSTANT_Module:
                {
                    return new ConstantModule(dataInput);
                }

                case Const.CONSTANT_Package:
                {
                    return new ConstantPackage(dataInput);
                }

                default:
                {
                    throw new ClassFormatException("Invalid byte tag in constant pool: "
                                                   + b);
                }
            }
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

        /// <summary>Returns value as defined by given BCELComparator strategy.</summary>
        /// <remarks>
        ///     Returns value as defined by given BCELComparator strategy.
        ///     By default two Constant objects are said to be equal when
        ///     the result of toString() is equal.
        /// </remarks>
        /// <seealso cref="object.Equals(object)" />
        public override bool Equals(object obj)
        {
            return bcelComparator.Equals(this, obj);
        }

        /// <summary>Returns value as defined by given BCELComparator strategy.</summary>
        /// <remarks>
        ///     Returns value as defined by given BCELComparator strategy.
        ///     By default return the hashcode of the result of toString().
        /// </remarks>
        /// <seealso cref="object.GetHashCode()" />
        public override int GetHashCode()
        {
            return bcelComparator.HashCode(this);
        }

        private sealed class _BCELComparator_35 : BCELComparator
        {
            public bool Equals(object o1, object o2)
            {
                var THIS = (Constant) o1;
                var THAT = (Constant) o2;
                return Sharpen.System.Equals(THIS.ToString(), THAT.ToString());
            }

            public int HashCode(object o)
            {
                var THIS = (Constant) o;
                return THIS.ToString().GetHashCode();
            }
        }
    }
}