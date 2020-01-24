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
using Apache.NBCEL.Generic;
using Apache.NBCEL.Java.IO;
using Apache.NBCEL.Util;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>
	///     This class represents the method info structure, i.e., the representation
	///     for a method in the class.
	/// </summary>
	/// <remarks>
	///     This class represents the method info structure, i.e., the representation
	///     for a method in the class. See JVM specification for details.
	///     A method has access flags, a name, a signature and a number of attributes.
	/// </remarks>
	public sealed class Method : FieldOrMethod
    {
        private static BCELComparator bcelComparator = new _BCELComparator_36(
        );

        private ParameterAnnotationEntry[] parameterAnnotationEntries;

        /// <summary>
        ///     Empty constructor, all attributes have to be defined via `setXXX'
        ///     methods.
        /// </summary>
        /// <remarks>
        ///     Empty constructor, all attributes have to be defined via `setXXX'
        ///     methods. Use at your own risk.
        /// </remarks>
        public Method()
        {
        }

        /// <summary>Initialize from another object.</summary>
        /// <remarks>
        ///     Initialize from another object. Note that both objects use the same
        ///     references (shallow copy). Use clone() for a physical copy.
        /// </remarks>
        public Method(Method c)
            : base(c)
        {
        }

        /// <summary>Construct object from file stream.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="ClassFormatException" />
        /// <exception cref="NBCEL.classfile.ClassFormatException" />
        internal Method(DataInput file, ConstantPool constant_pool
        )
            : base(file, constant_pool)
        {
        }

        /// <param name="access_flags">Access rights of method</param>
        /// <param name="name_index">Points to field name in constant pool</param>
        /// <param name="signature_index">Points to encoded signature</param>
        /// <param name="attributes">Collection of attributes</param>
        /// <param name="constant_pool">Array of constants</param>
        public Method(int access_flags, int name_index, int signature_index, Attribute
            [] attributes, ConstantPool constant_pool)
            : base(access_flags, name_index, signature_index, attributes, constant_pool)
        {
        }

        // annotations defined on the parameters of a method
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
            v.VisitMethod(this);
        }

        /// <returns>Code attribute of method, if any</returns>
        public Code GetCode()
        {
            foreach (var attribute in GetAttributes())
                if (attribute is Code)
                    return (Code) attribute;
            return null;
        }

        /// <returns>
        ///     ExceptionTable attribute of method, if any, i.e., list all
        ///     exceptions the method may throw not exception handlers!
        /// </returns>
        public ExceptionTable GetExceptionTable()
        {
            foreach (var attribute in GetAttributes())
                if (attribute is ExceptionTable)
                    return (ExceptionTable) attribute;
            return null;
        }

        /// <returns>
        ///     LocalVariableTable of code attribute if any, i.e. the call is forwarded
        ///     to the Code atribute.
        /// </returns>
        public LocalVariableTable GetLocalVariableTable()
        {
            var code = GetCode();
            if (code == null) return null;
            return code.GetLocalVariableTable();
        }

        /// <returns>
        ///     LineNumberTable of code attribute if any, i.e. the call is forwarded
        ///     to the Code atribute.
        /// </returns>
        public LineNumberTable GetLineNumberTable()
        {
            var code = GetCode();
            if (code == null) return null;
            return code.GetLineNumberTable();
        }

        /// <summary>
        ///     Return string representation close to declaration format,
        ///     `public static void main(String[] args) throws IOException', e.g.
        /// </summary>
        /// <returns>String representation of the method.</returns>
        public override string ToString()
        {
            var access = Utility.AccessToString(GetAccessFlags());
            // Get name and signature from constant pool
            var c = (ConstantUtf8) GetConstantPool
                ().GetConstant(GetSignatureIndex(), Const.CONSTANT_Utf8);
            var signature = c.GetBytes();
            c = (ConstantUtf8) GetConstantPool().GetConstant(GetNameIndex
                (), Const.CONSTANT_Utf8);
            var name = c.GetBytes();
            signature = Utility.MethodSignatureToString(signature, name, access
                , true, GetLocalVariableTable());
            var buf = new StringBuilder(signature);
            foreach (var attribute in GetAttributes())
                if (!(attribute is Code || attribute is ExceptionTable))
                    buf.Append(" [").Append(attribute).Append("]");
            var e = GetExceptionTable();
            if (e != null)
            {
                var str = e.ToString();
                if (!(str.Length == 0)) buf.Append("\n\t\tthrows ").Append(str);
            }

            return buf.ToString();
        }

        /// <returns>deep copy of this method</returns>
        public Method Copy(ConstantPool _constant_pool)
        {
            return (Method) Copy_(_constant_pool);
        }

        /// <returns>return type of method</returns>
        public Type GetReturnType()
        {
            return Type.GetReturnType(GetSignature());
        }

        /// <returns>array of method argument types</returns>
        public Type[] GetArgumentTypes()
        {
            return Type.GetArgumentTypes(GetSignature());
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
        ///     By default two method objects are said to be equal when
        ///     their names and signatures are equal.
        /// </remarks>
        /// <seealso cref="object.Equals(object)" />
        public override bool Equals(object obj)
        {
            return bcelComparator.Equals(this, obj);
        }

        /// <summary>Return value as defined by given BCELComparator strategy.</summary>
        /// <remarks>
        ///     Return value as defined by given BCELComparator strategy.
        ///     By default return the hashcode of the method's name XOR signature.
        /// </remarks>
        /// <seealso cref="object.GetHashCode()" />
        public override int GetHashCode()
        {
            return bcelComparator.HashCode(this);
        }

        /// <returns>Annotations on the parameters of a method</returns>
        /// <since>6.0</since>
        public ParameterAnnotationEntry[] GetParameterAnnotationEntries()
        {
            if (parameterAnnotationEntries == null)
                parameterAnnotationEntries = ParameterAnnotationEntry.CreateParameterAnnotationEntries
                    (GetAttributes());
            return parameterAnnotationEntries;
        }

        private sealed class _BCELComparator_36 : BCELComparator
        {
            public bool Equals(object o1, object o2)
            {
                var THIS = (Method) o1;
                var THAT = (Method) o2;
                return System.Equals(THIS.GetName(), THAT.GetName()) && System
                           .Equals(THIS.GetSignature(), THAT.GetSignature());
            }

            public int HashCode(object o)
            {
                var THIS = (Method) o;
                return THIS.GetSignature().GetHashCode() ^ THIS.GetName().GetHashCode();
            }
        }
        
        public Code Code => GetCode();

        public ExceptionTable ExceptionTable => GetExceptionTable();

        public LocalVariableTable LocalVariableTable => GetLocalVariableTable();

        public LineNumberTable LineNumberTable => GetLineNumberTable();

        public Type ReturnType => GetReturnType();

        public Type[] ArgumentTypes => GetArgumentTypes();

        public static BCELComparator Comparator {
            get => GetComparator();
            set => SetComparator(value);
        }

        public int HashCode => GetHashCode();

        public ParameterAnnotationEntry[] ParameterAnnotationEntries => GetParameterAnnotationEntries();
    }
}