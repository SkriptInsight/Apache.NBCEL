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

namespace NBCEL.generic
{
	/// <summary>This class is used to build up a constant pool.</summary>
	/// <remarks>
	/// This class is used to build up a constant pool. The user adds
	/// constants via `addXXX' methods, `addString', `addClass',
	/// etc.. These methods return an index into the constant
	/// pool. Finally, `getFinalConstantPool()' returns the constant pool
	/// built up. Intermediate versions of the constant pool can be
	/// obtained with `getConstantPool()'. A constant pool has capacity for
	/// Constants.MAX_SHORT entries. Note that the first (0) is used by the
	/// JVM and that Double and Long constants need two slots.
	/// </remarks>
	/// <seealso cref="NBCEL.classfile.Constant"/>
	public class ConstantPoolGen
	{
		private const int DEFAULT_BUFFER_SIZE = 256;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal int size;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal NBCEL.classfile.Constant[] constants;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getSize()"
			)]
		protected internal int index = 1;

		private const string METHODREF_DELIM = ":";

		private const string IMETHODREF_DELIM = "#";

		private const string FIELDREF_DELIM = "&";

		private const string NAT_DELIM = "%";

		private class Index
		{
			internal readonly int index;

			internal Index(int i)
			{
				// First entry (0) used by JVM
				// Name and Type
				index = i;
			}
		}

		/// <summary>Initialize with given array of constants.</summary>
		/// <param name="cs">array of given constants, new ones will be appended</param>
		public ConstantPoolGen(NBCEL.classfile.Constant[] cs)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(DEFAULT_BUFFER_SIZE);
			size = System.Math.Max(DEFAULT_BUFFER_SIZE, cs.Length + 64);
			constants = new NBCEL.classfile.Constant[size];
			System.Array.Copy(cs, 0, constants, 0, cs.Length);
			if (cs.Length > 0)
			{
				index = cs.Length;
			}
			for (int i = 1; i < index; i++)
			{
				NBCEL.classfile.Constant c = constants[i];
				if (c is NBCEL.classfile.ConstantString)
				{
					NBCEL.classfile.ConstantString s = (NBCEL.classfile.ConstantString)c;
					NBCEL.classfile.ConstantUtf8 u8 = (NBCEL.classfile.ConstantUtf8)constants[s.GetStringIndex
						()];
					string key = u8.GetBytes();
					if (!string_table.ContainsKey(key))
					{
						Sharpen.Collections.Put(string_table, key, new NBCEL.generic.ConstantPoolGen.Index
							(i));
					}
				}
				else if (c is NBCEL.classfile.ConstantClass)
				{
					NBCEL.classfile.ConstantClass s = (NBCEL.classfile.ConstantClass)c;
					NBCEL.classfile.ConstantUtf8 u8 = (NBCEL.classfile.ConstantUtf8)constants[s.GetNameIndex
						()];
					string key = u8.GetBytes();
					if (!class_table.ContainsKey(key))
					{
						Sharpen.Collections.Put(class_table, key, new NBCEL.generic.ConstantPoolGen.Index
							(i));
					}
				}
				else if (c is NBCEL.classfile.ConstantNameAndType)
				{
					NBCEL.classfile.ConstantNameAndType n = (NBCEL.classfile.ConstantNameAndType)c;
					NBCEL.classfile.ConstantUtf8 u8 = (NBCEL.classfile.ConstantUtf8)constants[n.GetNameIndex
						()];
					NBCEL.classfile.ConstantUtf8 u8_2 = (NBCEL.classfile.ConstantUtf8)constants[n.GetSignatureIndex
						()];
					sb.Append(u8.GetBytes());
					sb.Append(NAT_DELIM);
					sb.Append(u8_2.GetBytes());
					string key = sb.ToString();
					sb.Clear();
					if (!n_a_t_table.ContainsKey(key))
					{
						Sharpen.Collections.Put(n_a_t_table, key, new NBCEL.generic.ConstantPoolGen.Index
							(i));
					}
				}
				else if (c is NBCEL.classfile.ConstantUtf8)
				{
					NBCEL.classfile.ConstantUtf8 u = (NBCEL.classfile.ConstantUtf8)c;
					string key = u.GetBytes();
					if (!utf8_table.ContainsKey(key))
					{
						Sharpen.Collections.Put(utf8_table, key, new NBCEL.generic.ConstantPoolGen.Index(
							i));
					}
				}
				else if (c is NBCEL.classfile.ConstantCP)
				{
					NBCEL.classfile.ConstantCP m = (NBCEL.classfile.ConstantCP)c;
					string class_name;
					NBCEL.classfile.ConstantUtf8 u8;
					if (c is NBCEL.classfile.ConstantInvokeDynamic)
					{
						class_name = (((NBCEL.classfile.ConstantInvokeDynamic)m).GetBootstrapMethodAttrIndex
							()).ToString();
					}
					else
					{
						// since name can't begin with digit, can  use
						// METHODREF_DELIM with out fear of duplicates.
						NBCEL.classfile.ConstantClass clazz = (NBCEL.classfile.ConstantClass)constants[m.
							GetClassIndex()];
						u8 = (NBCEL.classfile.ConstantUtf8)constants[clazz.GetNameIndex()];
						class_name = u8.GetBytes().Replace('/', '.');
					}
					NBCEL.classfile.ConstantNameAndType n = (NBCEL.classfile.ConstantNameAndType)constants
						[m.GetNameAndTypeIndex()];
					u8 = (NBCEL.classfile.ConstantUtf8)constants[n.GetNameIndex()];
					string method_name = u8.GetBytes();
					u8 = (NBCEL.classfile.ConstantUtf8)constants[n.GetSignatureIndex()];
					string signature = u8.GetBytes();
					string delim = METHODREF_DELIM;
					if (c is NBCEL.classfile.ConstantInterfaceMethodref)
					{
						delim = IMETHODREF_DELIM;
					}
					else if (c is NBCEL.classfile.ConstantFieldref)
					{
						delim = FIELDREF_DELIM;
					}
					sb.Append(class_name);
					sb.Append(delim);
					sb.Append(method_name);
					sb.Append(delim);
					sb.Append(signature);
					string key = sb.ToString();
					sb.Clear();
					if (!cp_table.ContainsKey(key))
					{
						Sharpen.Collections.Put(cp_table, key, new NBCEL.generic.ConstantPoolGen.Index(i)
							);
					}
				}
				else if (c == null)
				{
				}
				else if (c is NBCEL.classfile.ConstantInteger)
				{
				}
				else if (c is NBCEL.classfile.ConstantLong)
				{
				}
				else if (c is NBCEL.classfile.ConstantFloat)
				{
				}
				else if (c is NBCEL.classfile.ConstantDouble)
				{
				}
				else if (c is NBCEL.classfile.ConstantMethodType)
				{
				}
				else if (c is NBCEL.classfile.ConstantMethodHandle)
				{
				}
				else if (c is NBCEL.classfile.ConstantModule)
				{
				}
				else if (c is NBCEL.classfile.ConstantPackage)
				{
				}
				else
				{
					// entries may be null
					// nothing to do
					// nothing to do
					// nothing to do
					// nothing to do
					// nothing to do
					// TODO should this be handled somehow?
					// TODO should this be handled somehow?
					// TODO should this be handled somehow?
					// TODO should this be handled somehow?
					System.Diagnostics.Debug.Assert(false, "Unexpected constant type: " + c.GetType()
						.FullName);
				}
			}
		}

		/// <summary>Initialize with given constant pool.</summary>
		public ConstantPoolGen(NBCEL.classfile.ConstantPool cp)
			: this(cp.GetConstantPool())
		{
		}

		/// <summary>Create empty constant pool.</summary>
		public ConstantPoolGen()
		{
			size = DEFAULT_BUFFER_SIZE;
			constants = new NBCEL.classfile.Constant[size];
		}

		/// <summary>Resize internal array of constants.</summary>
		protected internal virtual void AdjustSize()
		{
			if (index + 3 >= size)
			{
				NBCEL.classfile.Constant[] cs = constants;
				size *= 2;
				constants = new NBCEL.classfile.Constant[size];
				System.Array.Copy(cs, 0, constants, 0, index);
			}
		}

		private readonly System.Collections.Generic.IDictionary<string, NBCEL.generic.ConstantPoolGen.Index
			> string_table = new System.Collections.Generic.Dictionary<string, NBCEL.generic.ConstantPoolGen.Index
			>();

		/// <summary>Look for ConstantString in ConstantPool containing String `str'.</summary>
		/// <param name="str">String to search for</param>
		/// <returns>index on success, -1 otherwise</returns>
		public virtual int LookupString(string str)
		{
			NBCEL.generic.ConstantPoolGen.Index index = string_table.GetOrNull(str);
			return (index != null) ? index.index : -1;
		}

		/// <summary>Add a new String constant to the ConstantPool, if it is not already in there.
		/// 	</summary>
		/// <param name="str">String to add</param>
		/// <returns>index of entry</returns>
		public virtual int AddString(string str)
		{
			int ret;
			if ((ret = LookupString(str)) != -1)
			{
				return ret;
			}
			// Already in CP
			int utf8 = AddUtf8(str);
			AdjustSize();
			NBCEL.classfile.ConstantString s = new NBCEL.classfile.ConstantString(utf8);
			ret = index;
			constants[index++] = s;
			if (!string_table.ContainsKey(str))
			{
				Sharpen.Collections.Put(string_table, str, new NBCEL.generic.ConstantPoolGen.Index
					(ret));
			}
			return ret;
		}

		private readonly System.Collections.Generic.IDictionary<string, NBCEL.generic.ConstantPoolGen.Index
			> class_table = new System.Collections.Generic.Dictionary<string, NBCEL.generic.ConstantPoolGen.Index
			>();

		/// <summary>Look for ConstantClass in ConstantPool named `str'.</summary>
		/// <param name="str">String to search for</param>
		/// <returns>index on success, -1 otherwise</returns>
		public virtual int LookupClass(string str)
		{
			NBCEL.generic.ConstantPoolGen.Index index = class_table.GetOrNull(str.Replace('.'
				, '/'));
			return (index != null) ? index.index : -1;
		}

		private int AddClass_(string clazz)
		{
			int ret;
			if ((ret = LookupClass(clazz)) != -1)
			{
				return ret;
			}
			// Already in CP
			AdjustSize();
			NBCEL.classfile.ConstantClass c = new NBCEL.classfile.ConstantClass(AddUtf8(clazz
				));
			ret = index;
			constants[index++] = c;
			if (!class_table.ContainsKey(clazz))
			{
				Sharpen.Collections.Put(class_table, clazz, new NBCEL.generic.ConstantPoolGen.Index
					(ret));
			}
			return ret;
		}

		/// <summary>Add a new Class reference to the ConstantPool, if it is not already in there.
		/// 	</summary>
		/// <param name="str">Class to add</param>
		/// <returns>index of entry</returns>
		public virtual int AddClass(string str)
		{
			return AddClass_(str.Replace('.', '/'));
		}

		/// <summary>Add a new Class reference to the ConstantPool for a given type.</summary>
		/// <param name="type">Class to add</param>
		/// <returns>index of entry</returns>
		public virtual int AddClass(NBCEL.generic.ObjectType type)
		{
			return AddClass(type.GetClassName());
		}

		/// <summary>Add a reference to an array class (e.g.</summary>
		/// <remarks>
		/// Add a reference to an array class (e.g. String[][]) as needed by MULTIANEWARRAY
		/// instruction, e.g. to the ConstantPool.
		/// </remarks>
		/// <param name="type">type of array class</param>
		/// <returns>index of entry</returns>
		public virtual int AddArrayClass(NBCEL.generic.ArrayType type)
		{
			return AddClass_(type.GetSignature());
		}

		/// <summary>Look for ConstantInteger in ConstantPool.</summary>
		/// <param name="n">integer number to look for</param>
		/// <returns>index on success, -1 otherwise</returns>
		public virtual int LookupInteger(int n)
		{
			for (int i = 1; i < index; i++)
			{
				if (constants[i] is NBCEL.classfile.ConstantInteger)
				{
					NBCEL.classfile.ConstantInteger c = (NBCEL.classfile.ConstantInteger)constants[i];
					if (c.GetBytes() == n)
					{
						return i;
					}
				}
			}
			return -1;
		}

		/// <summary>Add a new Integer constant to the ConstantPool, if it is not already in there.
		/// 	</summary>
		/// <param name="n">integer number to add</param>
		/// <returns>index of entry</returns>
		public virtual int AddInteger(int n)
		{
			int ret;
			if ((ret = LookupInteger(n)) != -1)
			{
				return ret;
			}
			// Already in CP
			AdjustSize();
			ret = index;
			constants[index++] = new NBCEL.classfile.ConstantInteger(n);
			return ret;
		}

		/// <summary>Look for ConstantFloat in ConstantPool.</summary>
		/// <param name="n">Float number to look for</param>
		/// <returns>index on success, -1 otherwise</returns>
		public virtual int LookupFloat(float n)
		{
			int bits = Sharpen.Runtime.FloatToIntBits(n);
			for (int i = 1; i < index; i++)
			{
				if (constants[i] is NBCEL.classfile.ConstantFloat)
				{
					NBCEL.classfile.ConstantFloat c = (NBCEL.classfile.ConstantFloat)constants[i];
					if (Sharpen.Runtime.FloatToIntBits(c.GetBytes()) == bits)
					{
						return i;
					}
				}
			}
			return -1;
		}

		/// <summary>Add a new Float constant to the ConstantPool, if it is not already in there.
		/// 	</summary>
		/// <param name="n">Float number to add</param>
		/// <returns>index of entry</returns>
		public virtual int AddFloat(float n)
		{
			int ret;
			if ((ret = LookupFloat(n)) != -1)
			{
				return ret;
			}
			// Already in CP
			AdjustSize();
			ret = index;
			constants[index++] = new NBCEL.classfile.ConstantFloat(n);
			return ret;
		}

		private readonly System.Collections.Generic.IDictionary<string, NBCEL.generic.ConstantPoolGen.Index
			> utf8_table = new System.Collections.Generic.Dictionary<string, NBCEL.generic.ConstantPoolGen.Index
			>();

		/// <summary>Look for ConstantUtf8 in ConstantPool.</summary>
		/// <param name="n">Utf8 string to look for</param>
		/// <returns>index on success, -1 otherwise</returns>
		public virtual int LookupUtf8(string n)
		{
			NBCEL.generic.ConstantPoolGen.Index index = utf8_table.GetOrNull(n);
			return (index != null) ? index.index : -1;
		}

		/// <summary>Add a new Utf8 constant to the ConstantPool, if it is not already in there.
		/// 	</summary>
		/// <param name="n">Utf8 string to add</param>
		/// <returns>index of entry</returns>
		public virtual int AddUtf8(string n)
		{
			int ret;
			if ((ret = LookupUtf8(n)) != -1)
			{
				return ret;
			}
			// Already in CP
			AdjustSize();
			ret = index;
			constants[index++] = new NBCEL.classfile.ConstantUtf8(n);
			if (!utf8_table.ContainsKey(n))
			{
				Sharpen.Collections.Put(utf8_table, n, new NBCEL.generic.ConstantPoolGen.Index(ret
					));
			}
			return ret;
		}

		/// <summary>Look for ConstantLong in ConstantPool.</summary>
		/// <param name="n">Long number to look for</param>
		/// <returns>index on success, -1 otherwise</returns>
		public virtual int LookupLong(long n)
		{
			for (int i = 1; i < index; i++)
			{
				if (constants[i] is NBCEL.classfile.ConstantLong)
				{
					NBCEL.classfile.ConstantLong c = (NBCEL.classfile.ConstantLong)constants[i];
					if (c.GetBytes() == n)
					{
						return i;
					}
				}
			}
			return -1;
		}

		/// <summary>Add a new long constant to the ConstantPool, if it is not already in there.
		/// 	</summary>
		/// <param name="n">Long number to add</param>
		/// <returns>index of entry</returns>
		public virtual int AddLong(long n)
		{
			int ret;
			if ((ret = LookupLong(n)) != -1)
			{
				return ret;
			}
			// Already in CP
			AdjustSize();
			ret = index;
			constants[index] = new NBCEL.classfile.ConstantLong(n);
			index += 2;
			// Wastes one entry according to spec
			return ret;
		}

		/// <summary>Look for ConstantDouble in ConstantPool.</summary>
		/// <param name="n">Double number to look for</param>
		/// <returns>index on success, -1 otherwise</returns>
		public virtual int LookupDouble(double n)
		{
			long bits = System.BitConverter.DoubleToInt64Bits(n);
			for (int i = 1; i < index; i++)
			{
				if (constants[i] is NBCEL.classfile.ConstantDouble)
				{
					NBCEL.classfile.ConstantDouble c = (NBCEL.classfile.ConstantDouble)constants[i];
					if (System.BitConverter.DoubleToInt64Bits(c.GetBytes()) == bits)
					{
						return i;
					}
				}
			}
			return -1;
		}

		/// <summary>Add a new double constant to the ConstantPool, if it is not already in there.
		/// 	</summary>
		/// <param name="n">Double number to add</param>
		/// <returns>index of entry</returns>
		public virtual int AddDouble(double n)
		{
			int ret;
			if ((ret = LookupDouble(n)) != -1)
			{
				return ret;
			}
			// Already in CP
			AdjustSize();
			ret = index;
			constants[index] = new NBCEL.classfile.ConstantDouble(n);
			index += 2;
			// Wastes one entry according to spec
			return ret;
		}

		private readonly System.Collections.Generic.IDictionary<string, NBCEL.generic.ConstantPoolGen.Index
			> n_a_t_table = new System.Collections.Generic.Dictionary<string, NBCEL.generic.ConstantPoolGen.Index
			>();

		/// <summary>Look for ConstantNameAndType in ConstantPool.</summary>
		/// <param name="name">of variable/method</param>
		/// <param name="signature">of variable/method</param>
		/// <returns>index on success, -1 otherwise</returns>
		public virtual int LookupNameAndType(string name, string signature)
		{
			NBCEL.generic.ConstantPoolGen.Index _index = n_a_t_table.GetOrNull(name + NAT_DELIM
				 + signature);
			return (_index != null) ? _index.index : -1;
		}

		/// <summary>
		/// Add a new NameAndType constant to the ConstantPool if it is not already
		/// in there.
		/// </summary>
		/// <param name="name">Name string to add</param>
		/// <param name="signature">signature string to add</param>
		/// <returns>index of entry</returns>
		public virtual int AddNameAndType(string name, string signature)
		{
			int ret;
			int name_index;
			int signature_index;
			if ((ret = LookupNameAndType(name, signature)) != -1)
			{
				return ret;
			}
			// Already in CP
			AdjustSize();
			name_index = AddUtf8(name);
			signature_index = AddUtf8(signature);
			ret = index;
			constants[index++] = new NBCEL.classfile.ConstantNameAndType(name_index, signature_index
				);
			string key = name + NAT_DELIM + signature;
			if (!n_a_t_table.ContainsKey(key))
			{
				Sharpen.Collections.Put(n_a_t_table, key, new NBCEL.generic.ConstantPoolGen.Index
					(ret));
			}
			return ret;
		}

		private readonly System.Collections.Generic.IDictionary<string, NBCEL.generic.ConstantPoolGen.Index
			> cp_table = new System.Collections.Generic.Dictionary<string, NBCEL.generic.ConstantPoolGen.Index
			>();

		/// <summary>Look for ConstantMethodref in ConstantPool.</summary>
		/// <param name="class_name">Where to find method</param>
		/// <param name="method_name">Guess what</param>
		/// <param name="signature">return and argument types</param>
		/// <returns>index on success, -1 otherwise</returns>
		public virtual int LookupMethodref(string class_name, string method_name, string 
			signature)
		{
			NBCEL.generic.ConstantPoolGen.Index index = cp_table.GetOrNull(class_name + METHODREF_DELIM
				 + method_name + METHODREF_DELIM + signature);
			return (index != null) ? index.index : -1;
		}

		public virtual int LookupMethodref(NBCEL.generic.MethodGen method)
		{
			return LookupMethodref(method.GetClassName(), method.GetName(), method.GetSignature
				());
		}

		/// <summary>
		/// Add a new Methodref constant to the ConstantPool, if it is not already
		/// in there.
		/// </summary>
		/// <param name="class_name">class name string to add</param>
		/// <param name="method_name">method name string to add</param>
		/// <param name="signature">method signature string to add</param>
		/// <returns>index of entry</returns>
		public virtual int AddMethodref(string class_name, string method_name, string signature
			)
		{
			int ret;
			int class_index;
			int name_and_type_index;
			if ((ret = LookupMethodref(class_name, method_name, signature)) != -1)
			{
				return ret;
			}
			// Already in CP
			AdjustSize();
			name_and_type_index = AddNameAndType(method_name, signature);
			class_index = AddClass(class_name);
			ret = index;
			constants[index++] = new NBCEL.classfile.ConstantMethodref(class_index, name_and_type_index
				);
			string key = class_name + METHODREF_DELIM + method_name + METHODREF_DELIM + signature;
			if (!cp_table.ContainsKey(key))
			{
				Sharpen.Collections.Put(cp_table, key, new NBCEL.generic.ConstantPoolGen.Index(ret
					));
			}
			return ret;
		}

		public virtual int AddMethodref(NBCEL.generic.MethodGen method)
		{
			return AddMethodref(method.GetClassName(), method.GetName(), method.GetSignature(
				));
		}

		/// <summary>Look for ConstantInterfaceMethodref in ConstantPool.</summary>
		/// <param name="class_name">Where to find method</param>
		/// <param name="method_name">Guess what</param>
		/// <param name="signature">return and argument types</param>
		/// <returns>index on success, -1 otherwise</returns>
		public virtual int LookupInterfaceMethodref(string class_name, string method_name
			, string signature)
		{
			NBCEL.generic.ConstantPoolGen.Index index = cp_table.GetOrNull(class_name + IMETHODREF_DELIM
				 + method_name + IMETHODREF_DELIM + signature);
			return (index != null) ? index.index : -1;
		}

		public virtual int LookupInterfaceMethodref(NBCEL.generic.MethodGen method)
		{
			return LookupInterfaceMethodref(method.GetClassName(), method.GetName(), method.GetSignature
				());
		}

		/// <summary>
		/// Add a new InterfaceMethodref constant to the ConstantPool, if it is not already
		/// in there.
		/// </summary>
		/// <param name="class_name">class name string to add</param>
		/// <param name="method_name">method name string to add</param>
		/// <param name="signature">signature string to add</param>
		/// <returns>index of entry</returns>
		public virtual int AddInterfaceMethodref(string class_name, string method_name, string
			 signature)
		{
			int ret;
			int class_index;
			int name_and_type_index;
			if ((ret = LookupInterfaceMethodref(class_name, method_name, signature)) != -1)
			{
				return ret;
			}
			// Already in CP
			AdjustSize();
			class_index = AddClass(class_name);
			name_and_type_index = AddNameAndType(method_name, signature);
			ret = index;
			constants[index++] = new NBCEL.classfile.ConstantInterfaceMethodref(class_index, 
				name_and_type_index);
			string key = class_name + IMETHODREF_DELIM + method_name + IMETHODREF_DELIM + signature;
			if (!cp_table.ContainsKey(key))
			{
				Sharpen.Collections.Put(cp_table, key, new NBCEL.generic.ConstantPoolGen.Index(ret
					));
			}
			return ret;
		}

		public virtual int AddInterfaceMethodref(NBCEL.generic.MethodGen method)
		{
			return AddInterfaceMethodref(method.GetClassName(), method.GetName(), method.GetSignature
				());
		}

		/// <summary>Look for ConstantFieldref in ConstantPool.</summary>
		/// <param name="class_name">Where to find method</param>
		/// <param name="field_name">Guess what</param>
		/// <param name="signature">return and argument types</param>
		/// <returns>index on success, -1 otherwise</returns>
		public virtual int LookupFieldref(string class_name, string field_name, string signature
			)
		{
			NBCEL.generic.ConstantPoolGen.Index index = cp_table.GetOrNull(class_name + FIELDREF_DELIM
				 + field_name + FIELDREF_DELIM + signature);
			return (index != null) ? index.index : -1;
		}

		/// <summary>
		/// Add a new Fieldref constant to the ConstantPool, if it is not already
		/// in there.
		/// </summary>
		/// <param name="class_name">class name string to add</param>
		/// <param name="field_name">field name string to add</param>
		/// <param name="signature">signature string to add</param>
		/// <returns>index of entry</returns>
		public virtual int AddFieldref(string class_name, string field_name, string signature
			)
		{
			int ret;
			int class_index;
			int name_and_type_index;
			if ((ret = LookupFieldref(class_name, field_name, signature)) != -1)
			{
				return ret;
			}
			// Already in CP
			AdjustSize();
			class_index = AddClass(class_name);
			name_and_type_index = AddNameAndType(field_name, signature);
			ret = index;
			constants[index++] = new NBCEL.classfile.ConstantFieldref(class_index, name_and_type_index
				);
			string key = class_name + FIELDREF_DELIM + field_name + FIELDREF_DELIM + signature;
			if (!cp_table.ContainsKey(key))
			{
				Sharpen.Collections.Put(cp_table, key, new NBCEL.generic.ConstantPoolGen.Index(ret
					));
			}
			return ret;
		}

		/// <param name="i">index in constant pool</param>
		/// <returns>constant pool entry at index i</returns>
		public virtual NBCEL.classfile.Constant GetConstant(int i)
		{
			return constants[i];
		}

		/// <summary>Use with care!</summary>
		/// <param name="i">index in constant pool</param>
		/// <param name="c">new constant pool entry at index i</param>
		public virtual void SetConstant(int i, NBCEL.classfile.Constant c)
		{
			constants[i] = c;
		}

		/// <returns>intermediate constant pool</returns>
		public virtual NBCEL.classfile.ConstantPool GetConstantPool()
		{
			return new NBCEL.classfile.ConstantPool(constants);
		}

		/// <returns>current size of constant pool</returns>
		public virtual int GetSize()
		{
			return index;
		}

		/// <returns>constant pool with proper length</returns>
		public virtual NBCEL.classfile.ConstantPool GetFinalConstantPool()
		{
			NBCEL.classfile.Constant[] cs = new NBCEL.classfile.Constant[index];
			System.Array.Copy(constants, 0, cs, 0, index);
			return new NBCEL.classfile.ConstantPool(cs);
		}

		/// <returns>String representation.</returns>
		public override string ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			for (int i = 1; i < index; i++)
			{
				buf.Append(i).Append(")").Append(constants[i]).Append("\n");
			}
			return buf.ToString();
		}

		/// <summary>Import constant from another ConstantPool and return new index.</summary>
		public virtual int AddConstant(NBCEL.classfile.Constant c, NBCEL.generic.ConstantPoolGen
			 cp)
		{
			NBCEL.classfile.Constant[] constants = cp.GetConstantPool().GetConstantPool();
			switch (c.GetTag())
			{
				case NBCEL.Const.CONSTANT_String:
				{
					NBCEL.classfile.ConstantString s = (NBCEL.classfile.ConstantString)c;
					NBCEL.classfile.ConstantUtf8 u8 = (NBCEL.classfile.ConstantUtf8)constants[s.GetStringIndex
						()];
					return AddString(u8.GetBytes());
				}

				case NBCEL.Const.CONSTANT_Class:
				{
					NBCEL.classfile.ConstantClass s = (NBCEL.classfile.ConstantClass)c;
					NBCEL.classfile.ConstantUtf8 u8 = (NBCEL.classfile.ConstantUtf8)constants[s.GetNameIndex
						()];
					return AddClass(u8.GetBytes());
				}

				case NBCEL.Const.CONSTANT_NameAndType:
				{
					NBCEL.classfile.ConstantNameAndType n = (NBCEL.classfile.ConstantNameAndType)c;
					NBCEL.classfile.ConstantUtf8 u8 = (NBCEL.classfile.ConstantUtf8)constants[n.GetNameIndex
						()];
					NBCEL.classfile.ConstantUtf8 u8_2 = (NBCEL.classfile.ConstantUtf8)constants[n.GetSignatureIndex
						()];
					return AddNameAndType(u8.GetBytes(), u8_2.GetBytes());
				}

				case NBCEL.Const.CONSTANT_Utf8:
				{
					return AddUtf8(((NBCEL.classfile.ConstantUtf8)c).GetBytes());
				}

				case NBCEL.Const.CONSTANT_Double:
				{
					return AddDouble(((NBCEL.classfile.ConstantDouble)c).GetBytes());
				}

				case NBCEL.Const.CONSTANT_Float:
				{
					return AddFloat(((NBCEL.classfile.ConstantFloat)c).GetBytes());
				}

				case NBCEL.Const.CONSTANT_Long:
				{
					return AddLong(((NBCEL.classfile.ConstantLong)c).GetBytes());
				}

				case NBCEL.Const.CONSTANT_Integer:
				{
					return AddInteger(((NBCEL.classfile.ConstantInteger)c).GetBytes());
				}

				case NBCEL.Const.CONSTANT_InterfaceMethodref:
				case NBCEL.Const.CONSTANT_Methodref:
				case NBCEL.Const.CONSTANT_Fieldref:
				{
					NBCEL.classfile.ConstantCP m = (NBCEL.classfile.ConstantCP)c;
					NBCEL.classfile.ConstantClass clazz = (NBCEL.classfile.ConstantClass)constants[m.
						GetClassIndex()];
					NBCEL.classfile.ConstantNameAndType n = (NBCEL.classfile.ConstantNameAndType)constants
						[m.GetNameAndTypeIndex()];
					NBCEL.classfile.ConstantUtf8 u8 = (NBCEL.classfile.ConstantUtf8)constants[clazz.GetNameIndex
						()];
					string class_name = u8.GetBytes().Replace('/', '.');
					u8 = (NBCEL.classfile.ConstantUtf8)constants[n.GetNameIndex()];
					string name = u8.GetBytes();
					u8 = (NBCEL.classfile.ConstantUtf8)constants[n.GetSignatureIndex()];
					string signature = u8.GetBytes();
					switch (c.GetTag())
					{
						case NBCEL.Const.CONSTANT_InterfaceMethodref:
						{
							return AddInterfaceMethodref(class_name, name, signature);
						}

						case NBCEL.Const.CONSTANT_Methodref:
						{
							return AddMethodref(class_name, name, signature);
						}

						case NBCEL.Const.CONSTANT_Fieldref:
						{
							return AddFieldref(class_name, name, signature);
						}

						default:
						{
							// Never reached
							throw new System.Exception("Unknown constant type " + c);
						}
					}
					goto default;
				}

				default:
				{
					// Never reached
					throw new System.Exception("Unknown constant type " + c);
				}
			}
		}
	}
}
