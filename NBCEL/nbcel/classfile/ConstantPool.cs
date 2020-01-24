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
	/// This class represents the constant pool, i.e., a table of constants, of
	/// a parsed classfile.
	/// </summary>
	/// <remarks>
	/// This class represents the constant pool, i.e., a table of constants, of
	/// a parsed classfile. It may contain null references, due to the JVM
	/// specification that skips an entry after an 8-byte constant (double,
	/// long) entry.  Those interested in generating constant pools
	/// programatically should see <a href="../generic/ConstantPoolGen.html">
	/// ConstantPoolGen</a>.
	/// </remarks>
	/// <seealso cref="Constant"/>
	/// <seealso cref="NBCEL.generic.ConstantPoolGen"/>
	public class ConstantPool : System.ICloneable, NBCEL.classfile.Node
	{
		private NBCEL.classfile.Constant[] constant_pool;

		/// <param name="constant_pool">Array of constants</param>
		public ConstantPool(NBCEL.classfile.Constant[] constant_pool)
		{
			this.constant_pool = constant_pool;
		}

		/// <summary>Reads constants from given input stream.</summary>
		/// <param name="input">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public ConstantPool(java.io.DataInput input)
		{
			byte tag;
			int constant_pool_count = input.ReadUnsignedShort();
			constant_pool = new NBCEL.classfile.Constant[constant_pool_count];
			/* constant_pool[0] is unused by the compiler and may be used freely
			* by the implementation.
			*/
			for (int i = 1; i < constant_pool_count; i++)
			{
				constant_pool[i] = NBCEL.classfile.Constant.ReadConstant(input);
				/* Quote from the JVM specification:
				* "All eight byte constants take up two spots in the constant pool.
				* If this is the n'th byte in the constant pool, then the next item
				* will be numbered n+2"
				*
				* Thus we have to increment the index counter.
				*/
				tag = constant_pool[i].GetTag();
				if ((tag == NBCEL.Const.CONSTANT_Double) || (tag == NBCEL.Const.CONSTANT_Long))
				{
					i++;
				}
			}
		}

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
		public virtual void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitConstantPool(this);
		}

		/// <summary>Resolves constant to a string representation.</summary>
		/// <param name="c">Constant to be printed</param>
		/// <returns>String representation</returns>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public virtual string ConstantToString(NBCEL.classfile.Constant c)
		{
			string str;
			int i;
			byte tag = c.GetTag();
			switch (tag)
			{
				case NBCEL.Const.CONSTANT_Class:
				{
					i = ((NBCEL.classfile.ConstantClass)c).GetNameIndex();
					c = GetConstant(i, NBCEL.Const.CONSTANT_Utf8);
					str = NBCEL.classfile.Utility.CompactClassName(((NBCEL.classfile.ConstantUtf8)c).
						GetBytes(), false);
					break;
				}

				case NBCEL.Const.CONSTANT_String:
				{
					i = ((NBCEL.classfile.ConstantString)c).GetStringIndex();
					c = GetConstant(i, NBCEL.Const.CONSTANT_Utf8);
					str = "\"" + Escape(((NBCEL.classfile.ConstantUtf8)c).GetBytes()) + "\"";
					break;
				}

				case NBCEL.Const.CONSTANT_Utf8:
				{
					str = ((NBCEL.classfile.ConstantUtf8)c).GetBytes();
					break;
				}

				case NBCEL.Const.CONSTANT_Double:
				{
					str = ((NBCEL.classfile.ConstantDouble)c).GetBytes().ToString();
					break;
				}

				case NBCEL.Const.CONSTANT_Float:
				{
					str = ((NBCEL.classfile.ConstantFloat)c).GetBytes().ToString();
					break;
				}

				case NBCEL.Const.CONSTANT_Long:
				{
					str = ((NBCEL.classfile.ConstantLong)c).GetBytes().ToString();
					break;
				}

				case NBCEL.Const.CONSTANT_Integer:
				{
					str = ((NBCEL.classfile.ConstantInteger)c).GetBytes().ToString();
					break;
				}

				case NBCEL.Const.CONSTANT_NameAndType:
				{
					str = ConstantToString(((NBCEL.classfile.ConstantNameAndType)c).GetNameIndex(), NBCEL.Const
						.CONSTANT_Utf8) + " " + ConstantToString(((NBCEL.classfile.ConstantNameAndType)c
						).GetSignatureIndex(), NBCEL.Const.CONSTANT_Utf8);
					break;
				}

				case NBCEL.Const.CONSTANT_InterfaceMethodref:
				case NBCEL.Const.CONSTANT_Methodref:
				case NBCEL.Const.CONSTANT_Fieldref:
				{
					str = ConstantToString(((NBCEL.classfile.ConstantCP)c).GetClassIndex(), NBCEL.Const
						.CONSTANT_Class) + "." + ConstantToString(((NBCEL.classfile.ConstantCP)c).GetNameAndTypeIndex
						(), NBCEL.Const.CONSTANT_NameAndType);
					break;
				}

				case NBCEL.Const.CONSTANT_MethodHandle:
				{
					// Note that the ReferenceIndex may point to a Fieldref, Methodref or
					// InterfaceMethodref - so we need to peek ahead to get the actual type.
					NBCEL.classfile.ConstantMethodHandle cmh = (NBCEL.classfile.ConstantMethodHandle)
						c;
					str = NBCEL.Const.GetMethodHandleName(cmh.GetReferenceKind()) + " " + ConstantToString
						(cmh.GetReferenceIndex(), GetConstant(cmh.GetReferenceIndex()).GetTag());
					break;
				}

				case NBCEL.Const.CONSTANT_MethodType:
				{
					NBCEL.classfile.ConstantMethodType cmt = (NBCEL.classfile.ConstantMethodType)c;
					str = ConstantToString(cmt.GetDescriptorIndex(), NBCEL.Const.CONSTANT_Utf8);
					break;
				}

				case NBCEL.Const.CONSTANT_InvokeDynamic:
				{
					NBCEL.classfile.ConstantInvokeDynamic cid = (NBCEL.classfile.ConstantInvokeDynamic
						)c;
					str = cid.GetBootstrapMethodAttrIndex() + ":" + ConstantToString(cid.GetNameAndTypeIndex
						(), NBCEL.Const.CONSTANT_NameAndType);
					break;
				}

				case NBCEL.Const.CONSTANT_Module:
				{
					i = ((NBCEL.classfile.ConstantModule)c).GetNameIndex();
					c = GetConstant(i, NBCEL.Const.CONSTANT_Utf8);
					str = NBCEL.classfile.Utility.CompactClassName(((NBCEL.classfile.ConstantUtf8)c).
						GetBytes(), false);
					break;
				}

				case NBCEL.Const.CONSTANT_Package:
				{
					i = ((NBCEL.classfile.ConstantPackage)c).GetNameIndex();
					c = GetConstant(i, NBCEL.Const.CONSTANT_Utf8);
					str = NBCEL.classfile.Utility.CompactClassName(((NBCEL.classfile.ConstantUtf8)c).
						GetBytes(), false);
					break;
				}

				default:
				{
					// Never reached
					throw new System.Exception("Unknown constant type " + tag);
				}
			}
			return str;
		}

		private static string Escape(string str)
		{
			int len = str.Length;
			System.Text.StringBuilder buf = new System.Text.StringBuilder(len + 5);
			char[] ch = str.ToCharArray();
			for (int i = 0; i < len; i++)
			{
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
			}
			return buf.ToString();
		}

		/// <summary>
		/// Retrieves constant at `index' from constant pool and resolve it to
		/// a string representation.
		/// </summary>
		/// <param name="index">of constant in constant pool</param>
		/// <param name="tag">expected type</param>
		/// <returns>String representation</returns>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public virtual string ConstantToString(int index, byte tag)
		{
			NBCEL.classfile.Constant c = GetConstant(index, tag);
			return ConstantToString(c);
		}

		/// <summary>Dump constant pool to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public virtual void Dump(java.io.DataOutputStream file)
		{
			file.WriteShort(constant_pool.Length);
			for (int i = 1; i < constant_pool.Length; i++)
			{
				if (constant_pool[i] != null)
				{
					constant_pool[i].Dump(file);
				}
			}
		}

		/// <summary>Gets constant from constant pool.</summary>
		/// <param name="index">Index in constant pool</param>
		/// <returns>Constant value</returns>
		/// <seealso cref="Constant"/>
		public virtual NBCEL.classfile.Constant GetConstant(int index)
		{
			if (index >= constant_pool.Length || index < 0)
			{
				throw new NBCEL.classfile.ClassFormatException("Invalid constant pool reference: "
					 + index + ". Constant pool size is: " + constant_pool.Length);
			}
			return constant_pool[index];
		}

		/// <summary>
		/// Gets constant from constant pool and check whether it has the
		/// expected type.
		/// </summary>
		/// <param name="index">Index in constant pool</param>
		/// <param name="tag">Tag of expected constant, i.e., its type</param>
		/// <returns>Constant value</returns>
		/// <seealso cref="Constant"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public virtual NBCEL.classfile.Constant GetConstant(int index, byte tag)
		{
			NBCEL.classfile.Constant c;
			c = GetConstant(index);
			if (c == null)
			{
				throw new NBCEL.classfile.ClassFormatException("Constant pool at index " + index 
					+ " is null.");
			}
			if (c.GetTag() != tag)
			{
				throw new NBCEL.classfile.ClassFormatException("Expected class `" + NBCEL.Const.GetConstantName
					(tag) + "' at index " + index + " and got " + c);
			}
			return c;
		}

		/// <returns>Array of constants.</returns>
		/// <seealso cref="Constant"/>
		public virtual NBCEL.classfile.Constant[] GetConstantPool()
		{
			return constant_pool;
		}

		/// <summary>
		/// Gets string from constant pool and bypass the indirection of
		/// `ConstantClass' and `ConstantString' objects.
		/// </summary>
		/// <remarks>
		/// Gets string from constant pool and bypass the indirection of
		/// `ConstantClass' and `ConstantString' objects. I.e. these classes have
		/// an index field that points to another entry of the constant pool of
		/// type `ConstantUtf8' which contains the real data.
		/// </remarks>
		/// <param name="index">Index in constant pool</param>
		/// <param name="tag">Tag of expected constant, either ConstantClass or ConstantString
		/// 	</param>
		/// <returns>Contents of string reference</returns>
		/// <seealso cref="ConstantClass"/>
		/// <seealso cref="ConstantString"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public virtual string GetConstantString(int index, byte tag)
		{
			NBCEL.classfile.Constant c;
			int i;
			c = GetConstant(index, tag);
			switch (tag)
			{
				case NBCEL.Const.CONSTANT_Class:
				{
					/* This switch() is not that elegant, since the four classes have the
					* same contents, they just differ in the name of the index
					* field variable.
					* But we want to stick to the JVM naming conventions closely though
					* we could have solved these more elegantly by using the same
					* variable name or by subclassing.
					*/
					i = ((NBCEL.classfile.ConstantClass)c).GetNameIndex();
					break;
				}

				case NBCEL.Const.CONSTANT_String:
				{
					i = ((NBCEL.classfile.ConstantString)c).GetStringIndex();
					break;
				}

				case NBCEL.Const.CONSTANT_Module:
				{
					i = ((NBCEL.classfile.ConstantModule)c).GetNameIndex();
					break;
				}

				case NBCEL.Const.CONSTANT_Package:
				{
					i = ((NBCEL.classfile.ConstantPackage)c).GetNameIndex();
					break;
				}

				default:
				{
					throw new System.Exception("getConstantString called with illegal tag " + tag);
				}
			}
			// Finally get the string from the constant pool
			c = GetConstant(i, NBCEL.Const.CONSTANT_Utf8);
			return ((NBCEL.classfile.ConstantUtf8)c).GetBytes();
		}

		/// <returns>Length of constant pool.</returns>
		public virtual int GetLength()
		{
			return constant_pool == null ? 0 : constant_pool.Length;
		}

		/// <param name="constant">Constant to set</param>
		public virtual void SetConstant(int index, NBCEL.classfile.Constant constant)
		{
			constant_pool[index] = constant;
		}

		/// <param name="constant_pool"/>
		public virtual void SetConstantPool(NBCEL.classfile.Constant[] constant_pool)
		{
			this.constant_pool = constant_pool;
		}

		/// <returns>String representation.</returns>
		public override string ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			for (int i = 1; i < constant_pool.Length; i++)
			{
				buf.Append(i).Append(")").Append(constant_pool[i]).Append("\n");
			}
			return buf.ToString();
		}

		/// <returns>deep copy of this constant pool</returns>
		public virtual NBCEL.classfile.ConstantPool Copy()
		{
			NBCEL.classfile.ConstantPool c = null;
			c = (NBCEL.classfile.ConstantPool)MemberwiseClone();
			c.constant_pool = new NBCEL.classfile.Constant[constant_pool.Length];
			for (int i = 1; i < constant_pool.Length; i++)
			{
				if (constant_pool[i] != null)
				{
					c.constant_pool[i] = constant_pool[i].Copy();
				}
			}
			// TODO should this throw?
			return c;
		}

		object System.ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
