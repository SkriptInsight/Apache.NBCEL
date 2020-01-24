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
using Sharpen;

namespace NBCEL.generic
{
	/// <summary>
	/// Abstract super class for all possible java types, namely basic types
	/// such as int, object types like String and array types, e.g.
	/// </summary>
	/// <remarks>
	/// Abstract super class for all possible java types, namely basic types
	/// such as int, object types like String and array types, e.g. int[]
	/// </remarks>
	public abstract class Type
	{
		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal byte type;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal string signature;

		/// <summary>Predefined constants</summary>
		public static readonly NBCEL.generic.BasicType VOID = new NBCEL.generic.BasicType
			(NBCEL.Const.T_VOID);

		public static readonly NBCEL.generic.BasicType BOOLEAN = new NBCEL.generic.BasicType
			(NBCEL.Const.T_BOOLEAN);

		public static readonly NBCEL.generic.BasicType INT = new NBCEL.generic.BasicType(
			NBCEL.Const.T_INT);

		public static readonly NBCEL.generic.BasicType SHORT = new NBCEL.generic.BasicType
			(NBCEL.Const.T_SHORT);

		public static readonly NBCEL.generic.BasicType BYTE = new NBCEL.generic.BasicType
			(NBCEL.Const.T_BYTE);

		public static readonly NBCEL.generic.BasicType LONG = new NBCEL.generic.BasicType
			(NBCEL.Const.T_LONG);

		public static readonly NBCEL.generic.BasicType DOUBLE = new NBCEL.generic.BasicType
			(NBCEL.Const.T_DOUBLE);

		public static readonly NBCEL.generic.BasicType FLOAT = new NBCEL.generic.BasicType
			(NBCEL.Const.T_FLOAT);

		public static readonly NBCEL.generic.BasicType CHAR = new NBCEL.generic.BasicType
			(NBCEL.Const.T_CHAR);

		public static readonly NBCEL.generic.ObjectType OBJECT = new NBCEL.generic.ObjectType
			("java.lang.Object");

		public static readonly NBCEL.generic.ObjectType CLASS = new NBCEL.generic.ObjectType
			("java.lang.Class");

		public static readonly NBCEL.generic.ObjectType STRING = new NBCEL.generic.ObjectType
			("java.lang.String");

		public static readonly NBCEL.generic.ObjectType STRINGBUFFER = new NBCEL.generic.ObjectType
			("java.lang.StringBuffer");

		public static readonly NBCEL.generic.ObjectType THROWABLE = new NBCEL.generic.ObjectType
			("java.lang.Throwable");

		public static readonly NBCEL.generic.Type[] NO_ARGS = new NBCEL.generic.Type[0];

		private sealed class _ReferenceType_62 : NBCEL.generic.ReferenceType
		{
			public _ReferenceType_62()
			{
			}
		}

		public static readonly NBCEL.generic.ReferenceType NULL = new _ReferenceType_62();

		private sealed class _Type_64 : NBCEL.generic.Type
		{
			public _Type_64(byte baseArg1, string baseArg2)
				: base(baseArg1, baseArg2)
			{
			}
		}

		public static readonly NBCEL.generic.Type UNKNOWN = new _Type_64(NBCEL.Const.T_UNKNOWN
			, "<unknown object>");

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
			if (o is NBCEL.generic.Type)
			{
				NBCEL.generic.Type t = (NBCEL.generic.Type)o;
				return (type == t.type) && signature.Equals(t.signature);
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

		/// <summary>boolean, short and char variable are considered as int in the stack or local variable area.
		/// 	</summary>
		/// <remarks>
		/// boolean, short and char variable are considered as int in the stack or local variable area.
		/// Returns
		/// <see cref="INT"/>
		/// for
		/// <see cref="BOOLEAN"/>
		/// ,
		/// <see cref="SHORT"/>
		/// or
		/// <see cref="CHAR"/>
		/// , otherwise
		/// returns the given type.
		/// </remarks>
		/// <since>6.0</since>
		public virtual NBCEL.generic.Type NormalizeForStackOrLocal()
		{
			if (this == NBCEL.generic.Type.BOOLEAN || this == NBCEL.generic.Type.BYTE || this
				 == NBCEL.generic.Type.SHORT || this == NBCEL.generic.Type.CHAR)
			{
				return NBCEL.generic.Type.INT;
			}
			return this;
		}

		/// <returns>stack size of this type (2 for long and double, 0 for void, 1 otherwise)
		/// 	</returns>
		public virtual int GetSize()
		{
			switch (type)
			{
				case NBCEL.Const.T_DOUBLE:
				case NBCEL.Const.T_LONG:
				{
					return 2;
				}

				case NBCEL.Const.T_VOID:
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
			return ((this.Equals(NBCEL.generic.Type.NULL) || (type >= NBCEL.Const.T_UNKNOWN))
				) ? signature : NBCEL.classfile.Utility.SignatureToString(signature, false);
		}

		/// <summary>Convert type to Java method signature, e.g.</summary>
		/// <remarks>
		/// Convert type to Java method signature, e.g. int[] f(java.lang.String x)
		/// becomes (Ljava/lang/String;)[I
		/// </remarks>
		/// <param name="return_type">what the method returns</param>
		/// <param name="arg_types">what are the argument types</param>
		/// <returns>method signature for given type(s).</returns>
		public static string GetMethodSignature(NBCEL.generic.Type return_type, NBCEL.generic.Type
			[] arg_types)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder("(");
			if (arg_types != null)
			{
				foreach (NBCEL.generic.Type arg_type in arg_types)
				{
					buf.Append(arg_type.GetSignature());
				}
			}
			buf.Append(')');
			buf.Append(return_type.GetSignature());
			return buf.ToString();
		}

		private sealed class _ThreadLocal_170 : System.Threading.ThreadLocal<int>
		{
			public _ThreadLocal_170() : base(() => 0)
			{
			}
		}

		private static readonly System.Threading.ThreadLocal<int> consumed_chars = new _ThreadLocal_170
			();

		//int consumed_chars=0; // Remember position in string, see getArgumentTypes
		private static int Unwrap(System.Threading.ThreadLocal<int> tl)
		{
			return tl.Value;
		}

		private static void Wrap(System.Threading.ThreadLocal<int> tl, int value)
		{
			tl.Value = value;
		}

		/// <summary>Convert signature to a Type object.</summary>
		/// <param name="signature">signature string such as Ljava/lang/String;</param>
		/// <returns>type object</returns>
		/// <exception cref="java.lang.StringIndexOutOfBoundsException"/>
		public static NBCEL.generic.Type GetType(string signature)
		{
			// @since 6.0 no longer final
			byte type = NBCEL.classfile.Utility.TypeOfSignature(signature);
			if (((sbyte)type) <= NBCEL.Const.T_VOID)
			{
				//corrected concurrent private static field acess
				Wrap(consumed_chars, 1);
				return NBCEL.generic.BasicType.GetType(type);
			}
			else if (type == NBCEL.Const.T_ARRAY)
			{
				int dim = 0;
				do
				{
					// Count dimensions
					dim++;
				}
				while (signature[dim] == '[');
				// Recurse, but just once, if the signature is ok
				NBCEL.generic.Type t = GetType(Sharpen.Runtime.Substring(signature, dim));
				//corrected concurrent private static field acess
				//  consumed_chars += dim; // update counter - is replaced by
				int _temp = Unwrap(consumed_chars) + dim;
				Wrap(consumed_chars, _temp);
				return new NBCEL.generic.ArrayType(t, dim);
			}
			else
			{
				// type == T_REFERENCE
				// Utility.typeSignatureToString understands how to parse generic types.
				string parsedSignature = NBCEL.classfile.Utility.TypeSignatureToString(signature, 
					false);
				Wrap(consumed_chars, parsedSignature.Length + 2);
				// "Lblabla;" `L' and `;' are removed
				return NBCEL.generic.ObjectType.GetInstance(parsedSignature.Replace('/', '.'));
			}
		}

		/// <summary>Convert return value of a method (signature) to a Type object.</summary>
		/// <param name="signature">signature string such as (Ljava/lang/String;)V</param>
		/// <returns>return type</returns>
		public static NBCEL.generic.Type GetReturnType(string signature)
		{
			try
			{
				// Read return type after `)'
				int index = signature.LastIndexOf(')') + 1;
				return GetType(Sharpen.Runtime.Substring(signature, index));
			}
			catch (Exception e)
			{
				// Should never occur
				throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
					, e);
			}
		}

		/// <summary>Convert arguments of a method (signature) to an array of Type objects.</summary>
		/// <param name="signature">signature string such as (Ljava/lang/String;)V</param>
		/// <returns>array of argument types</returns>
		public static NBCEL.generic.Type[] GetArgumentTypes(string signature)
		{
			System.Collections.Generic.List<NBCEL.generic.Type> vec = new System.Collections.Generic.List
				<NBCEL.generic.Type>();
			int index;
			NBCEL.generic.Type[] types;
			try
			{
				// Skip any type arguments to read argument declarations between `(' and `)'
				index = signature.IndexOf('(') + 1;
				if (index <= 0)
				{
					throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
						);
				}
				while (signature[index] != ')')
				{
					vec.Add(GetType(Sharpen.Runtime.Substring(signature, index)));
					//corrected concurrent private static field acess
					index += Unwrap(consumed_chars);
				}
			}
			catch (Exception e)
			{
				// update position
				// Should never occur
				throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
					, e);
			}
			types = new NBCEL.generic.Type[vec.Count];
			Sharpen.Collections.ToArray(vec, types);
			return types;
		}

		/// <summary>Convert runtime java.lang.Class to BCEL Type object.</summary>
		/// <param name="cl">Java class</param>
		/// <returns>corresponding Type object</returns>
		public static NBCEL.generic.Type GetType(System.Type cl)
		{
			if (cl == null)
			{
				throw new System.ArgumentException("Class must not be null");
			}
			/* That's an amzingly easy case, because getName() returns
			* the signature. That's what we would have liked anyway.
			*/
			if (cl.IsArray)
			{
				return GetType(cl.FullName);
			}
			else if (cl.IsPrimitive)
			{
				if (cl == typeof(int))
				{
					return INT;
				}
				else if (cl == typeof(void))
				{
					return VOID;
				}
				else if (cl == typeof(double))
				{
					return DOUBLE;
				}
				else if (cl == typeof(float))
				{
					return FLOAT;
				}
				else if (cl == typeof(bool))
				{
					return BOOLEAN;
				}
				else if (cl == typeof(byte))
				{
					return BYTE;
				}
				else if (cl == typeof(short))
				{
					return SHORT;
				}
				else if (cl == typeof(byte))
				{
					return BYTE;
				}
				else if (cl == typeof(long))
				{
					return LONG;
				}
				else if (cl == typeof(char))
				{
					return CHAR;
				}
				else
				{
					throw new System.InvalidOperationException("Ooops, what primitive type is " + cl);
				}
			}
			else
			{
				// "Real" class
				return NBCEL.generic.ObjectType.GetInstance(cl.FullName);
			}
		}

		/// <summary>Convert runtime java.lang.Class[] to BCEL Type objects.</summary>
		/// <param name="classes">an array of runtime class objects</param>
		/// <returns>array of corresponding Type objects</returns>
		public static NBCEL.generic.Type[] GetTypes(System.Type[] classes)
		{
			NBCEL.generic.Type[] ret = new NBCEL.generic.Type[classes.Length];
			for (int i = 0; i < ret.Length; i++)
			{
				ret[i] = GetType(classes[i]);
			}
			return ret;
		}

		public static string GetSignature(System.Reflection.MethodInfo meth)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder("(");
			var @params = meth.GetParameterTypes();
			// avoid clone
			foreach (var param in @params)
			{
				sb.Append(GetType(param).GetSignature());
			}
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
			return consumed << 2 | size;
		}

		internal static int GetArgumentTypesSize(string signature)
		{
			int res = 0;
			int index;
			try
			{
				// Skip any type arguments to read argument declarations between `(' and `)'
				index = signature.IndexOf('(') + 1;
				if (index <= 0)
				{
					throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
						);
				}
				while (signature[index] != ')')
				{
					int coded = GetTypeSize(Sharpen.Runtime.Substring(signature, index));
					res += Size(coded);
					index += Consumed(coded);
				}
			}
			catch (Exception e)
			{
				// Should never occur
				throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
					, e);
			}
			return res;
		}

		/// <exception cref="java.lang.StringIndexOutOfBoundsException"/>
		internal static int GetTypeSize(string signature)
		{
			byte type = NBCEL.classfile.Utility.TypeOfSignature(signature);
			if (((sbyte)type) <= NBCEL.Const.T_VOID)
			{
				return Encode(NBCEL.generic.BasicType.GetType(type).GetSize(), 1);
			}
			else if (type == NBCEL.Const.T_ARRAY)
			{
				int dim = 0;
				do
				{
					// Count dimensions
					dim++;
				}
				while (signature[dim] == '[');
				// Recurse, but just once, if the signature is ok
				int consumed = Consumed(GetTypeSize(Sharpen.Runtime.Substring(signature, dim)));
				return Encode(1, dim + consumed);
			}
			else
			{
				// type == T_REFERENCE
				int index = signature.IndexOf(';');
				// Look for closing `;'
				if (index < 0)
				{
					throw new NBCEL.classfile.ClassFormatException("Invalid signature: " + signature);
				}
				return Encode(1, index + 1);
			}
		}

		internal static int GetReturnTypeSize(string signature)
		{
			int index = signature.LastIndexOf(')') + 1;
			return NBCEL.generic.Type.Size(GetTypeSize(Sharpen.Runtime.Substring(signature, index
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
	}
}
