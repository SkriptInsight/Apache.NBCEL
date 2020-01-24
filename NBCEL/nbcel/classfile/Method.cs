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
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	/// This class represents the method info structure, i.e., the representation
	/// for a method in the class.
	/// </summary>
	/// <remarks>
	/// This class represents the method info structure, i.e., the representation
	/// for a method in the class. See JVM specification for details.
	/// A method has access flags, a name, a signature and a number of attributes.
	/// </remarks>
	public sealed class Method : NBCEL.classfile.FieldOrMethod
	{
		private sealed class _BCELComparator_36 : NBCEL.util.BCELComparator
		{
			public _BCELComparator_36()
			{
			}

			public bool Equals(object o1, object o2)
			{
				NBCEL.classfile.Method THIS = (NBCEL.classfile.Method)o1;
				NBCEL.classfile.Method THAT = (NBCEL.classfile.Method)o2;
				return Sharpen.System.Equals(THIS.GetName(), THAT.GetName()) && Sharpen.System
					.Equals(THIS.GetSignature(), THAT.GetSignature());
			}

			public int HashCode(object o)
			{
				NBCEL.classfile.Method THIS = (NBCEL.classfile.Method)o;
				return THIS.GetSignature().GetHashCode() ^ THIS.GetName().GetHashCode();
			}
		}

		private static NBCEL.util.BCELComparator bcelComparator = new _BCELComparator_36(
			);

		private NBCEL.classfile.ParameterAnnotationEntry[] parameterAnnotationEntries;

		/// <summary>
		/// Empty constructor, all attributes have to be defined via `setXXX'
		/// methods.
		/// </summary>
		/// <remarks>
		/// Empty constructor, all attributes have to be defined via `setXXX'
		/// methods. Use at your own risk.
		/// </remarks>
		public Method()
		{
		}

		/// <summary>Initialize from another object.</summary>
		/// <remarks>
		/// Initialize from another object. Note that both objects use the same
		/// references (shallow copy). Use clone() for a physical copy.
		/// </remarks>
		public Method(NBCEL.classfile.Method c)
			: base(c)
		{
		}

		/// <summary>Construct object from file stream.</summary>
		/// <param name="file">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		internal Method(java.io.DataInput file, NBCEL.classfile.ConstantPool constant_pool
			)
			: base(file, constant_pool)
		{
		}

		/// <param name="access_flags">Access rights of method</param>
		/// <param name="name_index">Points to field name in constant pool</param>
		/// <param name="signature_index">Points to encoded signature</param>
		/// <param name="attributes">Collection of attributes</param>
		/// <param name="constant_pool">Array of constants</param>
		public Method(int access_flags, int name_index, int signature_index, NBCEL.classfile.Attribute
			[] attributes, NBCEL.classfile.ConstantPool constant_pool)
			: base(access_flags, name_index, signature_index, attributes, constant_pool)
		{
		}

		// annotations defined on the parameters of a method
		/// <summary>
		/// Called by objects that are traversing the nodes of the tree implicitely
		/// defined by the contents of a Java class.
		/// </summary>
		/// <remarks>
		/// Called by objects that are traversing the nodes of the tree implicitely
		/// defined by the contents of a Java class. I.e., the hierarchy of methods,
		/// fields, attributes, etc. spawns a tree of objects.
		/// </remarks>
		/// <param name="v">Visitor object</param>
		public override void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitMethod(this);
		}

		/// <returns>Code attribute of method, if any</returns>
		public NBCEL.classfile.Code GetCode()
		{
			foreach (NBCEL.classfile.Attribute attribute in base.GetAttributes())
			{
				if (attribute is NBCEL.classfile.Code)
				{
					return (NBCEL.classfile.Code)attribute;
				}
			}
			return null;
		}

		/// <returns>
		/// ExceptionTable attribute of method, if any, i.e., list all
		/// exceptions the method may throw not exception handlers!
		/// </returns>
		public NBCEL.classfile.ExceptionTable GetExceptionTable()
		{
			foreach (NBCEL.classfile.Attribute attribute in base.GetAttributes())
			{
				if (attribute is NBCEL.classfile.ExceptionTable)
				{
					return (NBCEL.classfile.ExceptionTable)attribute;
				}
			}
			return null;
		}

		/// <returns>
		/// LocalVariableTable of code attribute if any, i.e. the call is forwarded
		/// to the Code atribute.
		/// </returns>
		public NBCEL.classfile.LocalVariableTable GetLocalVariableTable()
		{
			NBCEL.classfile.Code code = GetCode();
			if (code == null)
			{
				return null;
			}
			return code.GetLocalVariableTable();
		}

		/// <returns>
		/// LineNumberTable of code attribute if any, i.e. the call is forwarded
		/// to the Code atribute.
		/// </returns>
		public NBCEL.classfile.LineNumberTable GetLineNumberTable()
		{
			NBCEL.classfile.Code code = GetCode();
			if (code == null)
			{
				return null;
			}
			return code.GetLineNumberTable();
		}

		/// <summary>
		/// Return string representation close to declaration format,
		/// `public static void main(String[] args) throws IOException', e.g.
		/// </summary>
		/// <returns>String representation of the method.</returns>
		public override string ToString()
		{
			string access = NBCEL.classfile.Utility.AccessToString(base.GetAccessFlags());
			// Get name and signature from constant pool
			NBCEL.classfile.ConstantUtf8 c = (NBCEL.classfile.ConstantUtf8)base.GetConstantPool
				().GetConstant(base.GetSignatureIndex(), NBCEL.Const.CONSTANT_Utf8);
			string signature = c.GetBytes();
			c = (NBCEL.classfile.ConstantUtf8)base.GetConstantPool().GetConstant(base.GetNameIndex
				(), NBCEL.Const.CONSTANT_Utf8);
			string name = c.GetBytes();
			signature = NBCEL.classfile.Utility.MethodSignatureToString(signature, name, access
				, true, GetLocalVariableTable());
			System.Text.StringBuilder buf = new System.Text.StringBuilder(signature);
			foreach (NBCEL.classfile.Attribute attribute in base.GetAttributes())
			{
				if (!((attribute is NBCEL.classfile.Code) || (attribute is NBCEL.classfile.ExceptionTable
					)))
				{
					buf.Append(" [").Append(attribute).Append("]");
				}
			}
			NBCEL.classfile.ExceptionTable e = GetExceptionTable();
			if (e != null)
			{
				string str = e.ToString();
				if (!(str.Length == 0))
				{
					buf.Append("\n\t\tthrows ").Append(str);
				}
			}
			return buf.ToString();
		}

		/// <returns>deep copy of this method</returns>
		public NBCEL.classfile.Method Copy(NBCEL.classfile.ConstantPool _constant_pool)
		{
			return (NBCEL.classfile.Method)Copy_(_constant_pool);
		}

		/// <returns>return type of method</returns>
		public NBCEL.generic.Type GetReturnType()
		{
			return NBCEL.generic.Type.GetReturnType(GetSignature());
		}

		/// <returns>array of method argument types</returns>
		public NBCEL.generic.Type[] GetArgumentTypes()
		{
			return NBCEL.generic.Type.GetArgumentTypes(GetSignature());
		}

		/// <returns>Comparison strategy object</returns>
		public static NBCEL.util.BCELComparator GetComparator()
		{
			return bcelComparator;
		}

		/// <param name="comparator">Comparison strategy object</param>
		public static void SetComparator(NBCEL.util.BCELComparator comparator)
		{
			bcelComparator = comparator;
		}

		/// <summary>Return value as defined by given BCELComparator strategy.</summary>
		/// <remarks>
		/// Return value as defined by given BCELComparator strategy.
		/// By default two method objects are said to be equal when
		/// their names and signatures are equal.
		/// </remarks>
		/// <seealso cref="object.Equals(object)"/>
		public override bool Equals(object obj)
		{
			return bcelComparator.Equals(this, obj);
		}

		/// <summary>Return value as defined by given BCELComparator strategy.</summary>
		/// <remarks>
		/// Return value as defined by given BCELComparator strategy.
		/// By default return the hashcode of the method's name XOR signature.
		/// </remarks>
		/// <seealso cref="object.GetHashCode()"/>
		public override int GetHashCode()
		{
			return bcelComparator.HashCode(this);
		}

		/// <returns>Annotations on the parameters of a method</returns>
		/// <since>6.0</since>
		public NBCEL.classfile.ParameterAnnotationEntry[] GetParameterAnnotationEntries()
		{
			if (parameterAnnotationEntries == null)
			{
				parameterAnnotationEntries = NBCEL.classfile.ParameterAnnotationEntry.CreateParameterAnnotationEntries
					(GetAttributes());
			}
			return parameterAnnotationEntries;
		}
	}
}
