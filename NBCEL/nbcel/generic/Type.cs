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
using System.Reflection;
using System.Text;
using System.Threading;
using NBCEL.classfile;
using Sharpen;

namespace NBCEL.generic
{
	/// <summary>
	///     Abstract super class for all possible java types, namely basic types
	///     such as int, object types like String and array types, e.g.
	/// </summary>
	/// <remarks>
	///     Abstract super class for all possible java types, namely basic types
	///     such as int, object types like String and array types, e.g. int[]
	/// </remarks>
	public abstract class Type
    {
        /// <summary>Predefined constants</summary>
        public static readonly BasicType VOID = new BasicType
            (Const.T_VOID);

        public static readonly BasicType BOOLEAN = new BasicType
            (Const.T_BOOLEAN);

        public static readonly BasicType INT = new BasicType(
            Const.T_INT);

        public static readonly BasicType SHORT = new BasicType
            (Const.T_SHORT);

        public static readonly BasicType BYTE = new BasicType
            (Const.T_BYTE);

        public static readonly BasicType LONG = new BasicType
            (Const.T_LONG);

        public static readonly BasicType DOUBLE = new BasicType
            (Const.T_DOUBLE);

        public static readonly BasicType FLOAT = new BasicType
            (Const.T_FLOAT);

        public static readonly BasicType CHAR = new BasicType
            (Const.T_CHAR);

        public static readonly ObjectType OBJECT = new ObjectType
            ("java.lang.Object");

        public static readonly ObjectType CLASS = new ObjectType
            ("java.lang.Class");

        public static readonly ObjectType STRING = new ObjectType
            ("java.lang.String");

        public static readonly ObjectType STRINGBUFFER = new ObjectType
            ("java.lang.StringBuffer");

        public static readonly ObjectType THROWABLE = new ObjectType
            ("java.lang.Throwable");

        public static readonly Type[] NO_ARGS = new Type[0];

        public static readonly ReferenceType NULL = new _ReferenceType_62();

        public static readonly Type UNKNOWN = new _Type_64(Const.T_UNKNOWN
            , "<unknown object>");

        private static readonly ThreadLocal<int> consumed_chars = new _ThreadLocal_170
            ();

        [ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal string signature;

        [ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal byte type;

        protected internal Type(byte t, string s)
        {
            // TODO should be final (and private)
            // signature for the type TODO should be private
            // EMPTY, so immutable
            type = t;
            signature = s;
        }

        /// <returns>hashcode of Type</returns>
        public override int GetHashCode()
        {
            return type ^ signature.GetHashCode();
        }

        /// <returns>whether the Types are equal</returns>
        public override bool Equals(object o)
        {
            if (o is Type)
            {
                var t = (Type) o;
                return type == t.type && signature.Equals(t.signature);
            }

            return false;
        }

        /// <returns>signature for given type.</returns>
        public virtual string GetSignature()
        {
            return signature;
        }

        /// <returns>type as defined in Constants</returns>
        public virtual byte GetType()
        {
            return type;
        }

        /// <summary>
        ///     boolean, short and char variable are considered as int in the stack or local variable area.
        /// </summary>
        /// <remarks>
        ///     boolean, short and char variable are considered as int in the stack or local variable area.
        ///     Returns
        ///     <see cref="INT" />
        ///     for
        ///     <see cref="BOOLEAN" />
        ///     ,
        ///     <see cref="SHORT" />
        ///     or
        ///     <see cref="CHAR" />
        ///     , otherwise
        ///     returns the given type.
        /// </remarks>
        /// <since>6.0</since>
        public virtual Type NormalizeForStackOrLocal()
        {
            if (this == BOOLEAN || this == BYTE || this
                == SHORT || this == CHAR)
                return INT;
            return this;
        }

        /// <returns>
        ///     stack size of this type (2 for long and double, 0 for void, 1 otherwise)
        /// </returns>
        public virtual int GetSize()
        {
            switch (type)
            {
                case Const.T_DOUBLE:
                case Const.T_LONG:
                {
                    return 2;
                }

                case Const.T_VOID:
                {
                    return 0;
                }

                default:
                {
                    return 1;
                }
            }
        }

        /// <returns>Type string, e.g. `int[]'</returns>
        public override string ToString()
        {
            return Equals(NULL) || type >= Const.T_UNKNOWN ? signature : Utility.SignatureToString(signature, false);
        }

        /// <summary>Convert type to Java method signature, e.g.</summary>
        /// <remarks>
        ///     Convert type to Java method signature, e.g. int[] f(java.lang.String x)
        ///     becomes (Ljava/lang/String;)[I
        /// </remarks>
        /// <param name="return_type">what the method returns</param>
        /// <param name="arg_types">what are the argument types</param>
        /// <returns>method signature for given type(s).</returns>
        public static string GetMethodSignature(Type return_type, Type
            [] arg_types)
        {
            var buf = new StringBuilder("(");
            if (arg_types != null)
                foreach (var arg_type in arg_types)
                    buf.Append(arg_type.GetSignature());
            buf.Append(')');
            buf.Append(return_type.GetSignature());
            return buf.ToString();
        }

        //int consumed_chars=0; // Remember position in string, see getArgumentTypes
        private static int Unwrap(ThreadLocal<int> tl)
        {
            return tl.Value;
        }

        private static void Wrap(ThreadLocal<int> tl, int value)
        {
            tl.Value = value;
        }

        /// <summary>Convert signature to a Type object.</summary>
        /// <param name="signature">signature string such as Ljava/lang/String;</param>
        /// <returns>type object</returns>
        /// <exception cref="java.lang.StringIndexOutOfBoundsException" />
        public static Type GetType(string signature)
        {
            // @since 6.0 no longer final
            var type = Utility.TypeOfSignature(signature);
            if ((sbyte) type <= Const.T_VOID)
            {
                //corrected concurrent private static field acess
                Wrap(consumed_chars, 1);
                return BasicType.GetType(type);
            }

            if (type == Const.T_ARRAY)
            {
                var dim = 0;
                do
                {
                    // Count dimensions
                    dim++;
                } while (signature[dim] == '[');

                // Recurse, but just once, if the signature is ok
                var t = GetType(Runtime.Substring(signature, dim));
                //corrected concurrent private static field acess
                //  consumed_chars += dim; // update counter - is replaced by
                var _temp = Unwrap(consumed_chars) + dim;
                Wrap(consumed_chars, _temp);
                return new ArrayType(t, dim);
            }

            // type == T_REFERENCE
            // Utility.typeSignatureToString understands how to parse generic types.
            var parsedSignature = Utility.TypeSignatureToString(signature,
                false);
            Wrap(consumed_chars, parsedSignature.Length + 2);
            // "Lblabla;" `L' and `;' are removed
            return ObjectType.GetInstance(parsedSignature.Replace('/', '.'));
        }

        /// <summary>Convert return value of a method (signature) to a Type object.</summary>
        /// <param name="signature">signature string such as (Ljava/lang/String;)V</param>
        /// <returns>return type</returns>
        public static Type GetReturnType(string signature)
        {
            try
            {
                // Read return type after `)'
                var index = signature.LastIndexOf(')') + 1;
                return GetType(Runtime.Substring(signature, index));
            }
            catch (Exception e)
            {
                // Should never occur
                throw new ClassFormatException("Invalid method signature: " + signature
                    , e);
            }
        }

        /// <summary>Convert arguments of a method (signature) to an array of Type objects.</summary>
        /// <param name="signature">signature string such as (Ljava/lang/String;)V</param>
        /// <returns>array of argument types</returns>
        public static Type[] GetArgumentTypes(string signature)
        {
            var vec = new List
                <Type>();
            int index;
            Type[] types;
            try
            {
                // Skip any type arguments to read argument declarations between `(' and `)'
                index = signature.IndexOf('(') + 1;
                if (index <= 0)
                    throw new ClassFormatException("Invalid method signature: " + signature
                    );
                while (signature[index] != ')')
                {
                    vec.Add(GetType(Runtime.Substring(signature, index)));
                    //corrected concurrent private static field acess
                    index += Unwrap(consumed_chars);
                }
            }
            catch (Exception e)
            {
                // update position
                // Should never occur
                throw new ClassFormatException("Invalid method signature: " + signature
                    , e);
            }

            types = new Type[vec.Count];
            Collections.ToArray(vec, types);
            return types;
        }

        /// <summary>Convert runtime java.lang.Class to BCEL Type object.</summary>
        /// <param name="cl">Java class</param>
        /// <returns>corresponding Type object</returns>
        public static Type GetType(System.Type cl)
        {
            if (cl == null) throw new ArgumentException("Class must not be null");
            /* That's an amzingly easy case, because getName() returns
            * the signature. That's what we would have liked anyway.
            */
            if (cl.IsArray) return GetType(cl.FullName);

            if (cl.IsPrimitive)
            {
                if (cl == typeof(int))
                    return INT;
                if (cl == typeof(void))
                    return VOID;
                if (cl == typeof(double))
                    return DOUBLE;
                if (cl == typeof(float))
                    return FLOAT;
                if (cl == typeof(bool))
                    return BOOLEAN;
                if (cl == typeof(byte))
                    return BYTE;
                if (cl == typeof(short))
                    return SHORT;
                if (cl == typeof(byte))
                    return BYTE;
                if (cl == typeof(long))
                    return LONG;
                if (cl == typeof(char))
                    return CHAR;
                throw new InvalidOperationException("Ooops, what primitive type is " + cl);
            }

            // "Real" class
            return ObjectType.GetInstance(cl.FullName);
        }

        /// <summary>Convert runtime java.lang.Class[] to BCEL Type objects.</summary>
        /// <param name="classes">an array of runtime class objects</param>
        /// <returns>array of corresponding Type objects</returns>
        public static Type[] GetTypes(System.Type[] classes)
        {
            var ret = new Type[classes.Length];
            for (var i = 0; i < ret.Length; i++) ret[i] = GetType(classes[i]);
            return ret;
        }

        public static string GetSignature(MethodInfo meth)
        {
            var sb = new StringBuilder("(");
            var @params = meth.GetParameterTypes();
            // avoid clone
            foreach (var param in @params) sb.Append(GetType(param).GetSignature());
            sb.Append(")");
            sb.Append(GetType(meth.ReturnType).GetSignature());
            return sb.ToString();
        }

        internal static int Size(int coded)
        {
            return coded & 3;
        }

        internal static int Consumed(int coded)
        {
            return coded >> 2;
        }

        internal static int Encode(int size, int consumed)
        {
            return (consumed << 2) | size;
        }

        internal static int GetArgumentTypesSize(string signature)
        {
            var res = 0;
            int index;
            try
            {
                // Skip any type arguments to read argument declarations between `(' and `)'
                index = signature.IndexOf('(') + 1;
                if (index <= 0)
                    throw new ClassFormatException("Invalid method signature: " + signature
                    );
                while (signature[index] != ')')
                {
                    var coded = GetTypeSize(Runtime.Substring(signature, index));
                    res += Size(coded);
                    index += Consumed(coded);
                }
            }
            catch (Exception e)
            {
                // Should never occur
                throw new ClassFormatException("Invalid method signature: " + signature
                    , e);
            }

            return res;
        }

        /// <exception cref="java.lang.StringIndexOutOfBoundsException" />
        internal static int GetTypeSize(string signature)
        {
            var type = Utility.TypeOfSignature(signature);
            if ((sbyte) type <= Const.T_VOID) return Encode(BasicType.GetType(type).GetSize(), 1);

            if (type == Const.T_ARRAY)
            {
                var dim = 0;
                do
                {
                    // Count dimensions
                    dim++;
                } while (signature[dim] == '[');

                // Recurse, but just once, if the signature is ok
                var consumed = Consumed(GetTypeSize(Runtime.Substring(signature, dim)));
                return Encode(1, dim + consumed);
            }

            // type == T_REFERENCE
            var index = signature.IndexOf(';');
            // Look for closing `;'
            if (index < 0) throw new ClassFormatException("Invalid signature: " + signature);
            return Encode(1, index + 1);
        }

        internal static int GetReturnTypeSize(string signature)
        {
            var index = signature.LastIndexOf(')') + 1;
            return Size(GetTypeSize(Runtime.Substring(signature, index
            )));
        }

        /*
        * Currently only used by the ArrayType constructor.
        * The signature has a complicated dependency on other parameter
        * so it's tricky to do it in a call to the super ctor.
        */
        internal virtual void SetSignature(string signature)
        {
            this.signature = signature;
        }

        private sealed class _ReferenceType_62 : ReferenceType
        {
        }

        private sealed class _Type_64 : Type
        {
            public _Type_64(byte baseArg1, string baseArg2)
                : base(baseArg1, baseArg2)
            {
            }
        }

        private sealed class _ThreadLocal_170 : ThreadLocal<int>
        {
            public _ThreadLocal_170() : base(() => 0)
            {
            }
        }
    }
}