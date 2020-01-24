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
using System.Diagnostics;
using System.Text;
using Apache.NBCEL.ClassFile;

namespace Apache.NBCEL.Generic
{
	/// <summary>This class is used to build up a constant pool.</summary>
	/// <remarks>
	///     This class is used to build up a constant pool. The user adds
	///     constants via `addXXX' methods, `addString', `addClass',
	///     etc.. These methods return an index into the constant
	///     pool. Finally, `getFinalConstantPool()' returns the constant pool
	///     built up. Intermediate versions of the constant pool can be
	///     obtained with `getConstantPool()'. A constant pool has capacity for
	///     Constants.MAX_SHORT entries. Note that the first (0) is used by the
	///     JVM and that Double and Long constants need two slots.
	/// </remarks>
	/// <seealso cref="Constant" />
	public class ConstantPoolGen
    {
        private const int DEFAULT_BUFFER_SIZE = 256;

        private const string METHODREF_DELIM = ":";

        private const string IMETHODREF_DELIM = "#";

        private const string FIELDREF_DELIM = "&";

        private const string NAT_DELIM = "%";

        private readonly IDictionary<string, Index
        > class_table = new Dictionary<string, Index
        >();

        private readonly IDictionary<string, Index
        > cp_table = new Dictionary<string, Index
        >();

        private readonly IDictionary<string, Index
        > n_a_t_table = new Dictionary<string, Index
        >();

        private readonly IDictionary<string, Index
        > string_table = new Dictionary<string, Index
        >();

        private readonly IDictionary<string, Index
        > utf8_table = new Dictionary<string, Index
        >();

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal Constant[] constants;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getSize()"
        )]
        protected internal int index = 1;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal int size;

        /// <summary>Initialize with given array of constants.</summary>
        /// <param name="cs">array of given constants, new ones will be appended</param>
        public ConstantPoolGen(Constant[] cs)
        {
            var sb = new StringBuilder(DEFAULT_BUFFER_SIZE);
            size = Math.Max(DEFAULT_BUFFER_SIZE, cs.Length + 64);
            constants = new Constant[size];
            Array.Copy(cs, 0, constants, 0, cs.Length);
            if (cs.Length > 0) index = cs.Length;
            for (var i = 1; i < index; i++)
            {
                var c = constants[i];
                if (c is ConstantString)
                {
                    var s = (ConstantString) c;
                    var u8 = (ConstantUtf8) constants[s.GetStringIndex
                        ()];
                    var key = u8.GetBytes();
                    if (!string_table.ContainsKey(key))
                        Collections.Put(string_table, key, new Index
                            (i));
                }
                else if (c is ConstantClass)
                {
                    var s = (ConstantClass) c;
                    var u8 = (ConstantUtf8) constants[s.GetNameIndex
                        ()];
                    var key = u8.GetBytes();
                    if (!class_table.ContainsKey(key))
                        Collections.Put(class_table, key, new Index
                            (i));
                }
                else if (c is ConstantNameAndType)
                {
                    var n = (ConstantNameAndType) c;
                    var u8 = (ConstantUtf8) constants[n.GetNameIndex
                        ()];
                    var u8_2 = (ConstantUtf8) constants[n.GetSignatureIndex
                        ()];
                    sb.Append(u8.GetBytes());
                    sb.Append(NAT_DELIM);
                    sb.Append(u8_2.GetBytes());
                    var key = sb.ToString();
                    sb.Clear();
                    if (!n_a_t_table.ContainsKey(key))
                        Collections.Put(n_a_t_table, key, new Index
                            (i));
                }
                else if (c is ConstantUtf8)
                {
                    var u = (ConstantUtf8) c;
                    var key = u.GetBytes();
                    if (!utf8_table.ContainsKey(key))
                        Collections.Put(utf8_table, key, new Index(
                            i));
                }
                else if (c is ConstantCP)
                {
                    var m = (ConstantCP) c;
                    string class_name;
                    ConstantUtf8 u8;
                    if (c is ConstantInvokeDynamic)
                    {
                        class_name = ((ConstantInvokeDynamic) m).GetBootstrapMethodAttrIndex
                            ().ToString();
                    }
                    else
                    {
                        // since name can't begin with digit, can  use
                        // METHODREF_DELIM with out fear of duplicates.
                        var clazz = (ConstantClass) constants[m.GetClassIndex()];
                        u8 = (ConstantUtf8) constants[clazz.GetNameIndex()];
                        class_name = u8.GetBytes().Replace('/', '.');
                    }

                    var n = (ConstantNameAndType) constants
                        [m.GetNameAndTypeIndex()];
                    u8 = (ConstantUtf8) constants[n.GetNameIndex()];
                    var method_name = u8.GetBytes();
                    u8 = (ConstantUtf8) constants[n.GetSignatureIndex()];
                    var signature = u8.GetBytes();
                    var delim = METHODREF_DELIM;
                    if (c is ConstantInterfaceMethodref)
                        delim = IMETHODREF_DELIM;
                    else if (c is ConstantFieldref) delim = FIELDREF_DELIM;
                    sb.Append(class_name);
                    sb.Append(delim);
                    sb.Append(method_name);
                    sb.Append(delim);
                    sb.Append(signature);
                    var key = sb.ToString();
                    sb.Clear();
                    if (!cp_table.ContainsKey(key))
                        Collections.Put(cp_table, key, new Index(i)
                        );
                }
                else if (c == null)
                {
                }
                else if (c is ConstantInteger)
                {
                }
                else if (c is ConstantLong)
                {
                }
                else if (c is ConstantFloat)
                {
                }
                else if (c is ConstantDouble)
                {
                }
                else if (c is ConstantMethodType)
                {
                }
                else if (c is ConstantMethodHandle)
                {
                }
                else if (c is ConstantModule)
                {
                }
                else if (c is ConstantPackage)
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
                    Debug.Assert(false, "Unexpected constant type: " + c.GetType()
                                            .FullName);
                }
            }
        }

        /// <summary>Initialize with given constant pool.</summary>
        public ConstantPoolGen(ConstantPool cp)
            : this(cp.GetConstantPool())
        {
        }

        /// <summary>Create empty constant pool.</summary>
        public ConstantPoolGen()
        {
            size = DEFAULT_BUFFER_SIZE;
            constants = new Constant[size];
        }

        /// <summary>Resize internal array of constants.</summary>
        protected internal virtual void AdjustSize()
        {
            if (index + 3 >= size)
            {
                var cs = constants;
                size *= 2;
                constants = new Constant[size];
                Array.Copy(cs, 0, constants, 0, index);
            }
        }

        /// <summary>Look for ConstantString in ConstantPool containing String `str'.</summary>
        /// <param name="str">String to search for</param>
        /// <returns>index on success, -1 otherwise</returns>
        public virtual int LookupString(string str)
        {
            var index = string_table.GetOrNull(str);
            return index != null ? index.index : -1;
        }

        /// <summary>
        ///     Add a new String constant to the ConstantPool, if it is not already in there.
        /// </summary>
        /// <param name="str">String to add</param>
        /// <returns>index of entry</returns>
        public virtual int AddString(string str)
        {
            int ret;
            if ((ret = LookupString(str)) != -1) return ret;
            // Already in CP
            var utf8 = AddUtf8(str);
            AdjustSize();
            var s = new ConstantString(utf8);
            ret = index;
            constants[index++] = s;
            if (!string_table.ContainsKey(str))
                Collections.Put(string_table, str, new Index
                    (ret));
            return ret;
        }

        /// <summary>Look for ConstantClass in ConstantPool named `str'.</summary>
        /// <param name="str">String to search for</param>
        /// <returns>index on success, -1 otherwise</returns>
        public virtual int LookupClass(string str)
        {
            var index = class_table.GetOrNull(str.Replace('.'
                , '/'));
            return index != null ? index.index : -1;
        }

        private int AddClass_(string clazz)
        {
            int ret;
            if ((ret = LookupClass(clazz)) != -1) return ret;
            // Already in CP
            AdjustSize();
            var c = new ConstantClass(AddUtf8(clazz
            ));
            ret = index;
            constants[index++] = c;
            if (!class_table.ContainsKey(clazz))
                Collections.Put(class_table, clazz, new Index
                    (ret));
            return ret;
        }

        /// <summary>
        ///     Add a new Class reference to the ConstantPool, if it is not already in there.
        /// </summary>
        /// <param name="str">Class to add</param>
        /// <returns>index of entry</returns>
        public virtual int AddClass(string str)
        {
            return AddClass_(str.Replace('.', '/'));
        }

        /// <summary>Add a new Class reference to the ConstantPool for a given type.</summary>
        /// <param name="type">Class to add</param>
        /// <returns>index of entry</returns>
        public virtual int AddClass(ObjectType type)
        {
            return AddClass(type.GetClassName());
        }

        /// <summary>Add a reference to an array class (e.g.</summary>
        /// <remarks>
        ///     Add a reference to an array class (e.g. String[][]) as needed by MULTIANEWARRAY
        ///     instruction, e.g. to the ConstantPool.
        /// </remarks>
        /// <param name="type">type of array class</param>
        /// <returns>index of entry</returns>
        public virtual int AddArrayClass(ArrayType type)
        {
            return AddClass_(type.GetSignature());
        }

        /// <summary>Look for ConstantInteger in ConstantPool.</summary>
        /// <param name="n">integer number to look for</param>
        /// <returns>index on success, -1 otherwise</returns>
        public virtual int LookupInteger(int n)
        {
            for (var i = 1; i < index; i++)
                if (constants[i] is ConstantInteger)
                {
                    var c = (ConstantInteger) constants[i];
                    if (c.GetBytes() == n) return i;
                }

            return -1;
        }

        /// <summary>
        ///     Add a new Integer constant to the ConstantPool, if it is not already in there.
        /// </summary>
        /// <param name="n">integer number to add</param>
        /// <returns>index of entry</returns>
        public virtual int AddInteger(int n)
        {
            int ret;
            if ((ret = LookupInteger(n)) != -1) return ret;
            // Already in CP
            AdjustSize();
            ret = index;
            constants[index++] = new ConstantInteger(n);
            return ret;
        }

        /// <summary>Look for ConstantFloat in ConstantPool.</summary>
        /// <param name="n">Float number to look for</param>
        /// <returns>index on success, -1 otherwise</returns>
        public virtual int LookupFloat(float n)
        {
            var bits = Runtime.FloatToIntBits(n);
            for (var i = 1; i < index; i++)
                if (constants[i] is ConstantFloat)
                {
                    var c = (ConstantFloat) constants[i];
                    if (Runtime.FloatToIntBits(c.GetBytes()) == bits) return i;
                }

            return -1;
        }

        /// <summary>
        ///     Add a new Float constant to the ConstantPool, if it is not already in there.
        /// </summary>
        /// <param name="n">Float number to add</param>
        /// <returns>index of entry</returns>
        public virtual int AddFloat(float n)
        {
            int ret;
            if ((ret = LookupFloat(n)) != -1) return ret;
            // Already in CP
            AdjustSize();
            ret = index;
            constants[index++] = new ConstantFloat(n);
            return ret;
        }

        /// <summary>Look for ConstantUtf8 in ConstantPool.</summary>
        /// <param name="n">Utf8 string to look for</param>
        /// <returns>index on success, -1 otherwise</returns>
        public virtual int LookupUtf8(string n)
        {
            var index = utf8_table.GetOrNull(n);
            return index != null ? index.index : -1;
        }

        /// <summary>
        ///     Add a new Utf8 constant to the ConstantPool, if it is not already in there.
        /// </summary>
        /// <param name="n">Utf8 string to add</param>
        /// <returns>index of entry</returns>
        public virtual int AddUtf8(string n)
        {
            int ret;
            if ((ret = LookupUtf8(n)) != -1) return ret;
            // Already in CP
            AdjustSize();
            ret = index;
            constants[index++] = new ConstantUtf8(n);
            if (!utf8_table.ContainsKey(n))
                Collections.Put(utf8_table, n, new Index(ret
                ));
            return ret;
        }

        /// <summary>Look for ConstantLong in ConstantPool.</summary>
        /// <param name="n">Long number to look for</param>
        /// <returns>index on success, -1 otherwise</returns>
        public virtual int LookupLong(long n)
        {
            for (var i = 1; i < index; i++)
                if (constants[i] is ConstantLong)
                {
                    var c = (ConstantLong) constants[i];
                    if (c.GetBytes() == n) return i;
                }

            return -1;
        }

        /// <summary>
        ///     Add a new long constant to the ConstantPool, if it is not already in there.
        /// </summary>
        /// <param name="n">Long number to add</param>
        /// <returns>index of entry</returns>
        public virtual int AddLong(long n)
        {
            int ret;
            if ((ret = LookupLong(n)) != -1) return ret;
            // Already in CP
            AdjustSize();
            ret = index;
            constants[index] = new ConstantLong(n);
            index += 2;
            // Wastes one entry according to spec
            return ret;
        }

        /// <summary>Look for ConstantDouble in ConstantPool.</summary>
        /// <param name="n">Double number to look for</param>
        /// <returns>index on success, -1 otherwise</returns>
        public virtual int LookupDouble(double n)
        {
            var bits = BitConverter.DoubleToInt64Bits(n);
            for (var i = 1; i < index; i++)
                if (constants[i] is ConstantDouble)
                {
                    var c = (ConstantDouble) constants[i];
                    if (BitConverter.DoubleToInt64Bits(c.GetBytes()) == bits) return i;
                }

            return -1;
        }

        /// <summary>
        ///     Add a new double constant to the ConstantPool, if it is not already in there.
        /// </summary>
        /// <param name="n">Double number to add</param>
        /// <returns>index of entry</returns>
        public virtual int AddDouble(double n)
        {
            int ret;
            if ((ret = LookupDouble(n)) != -1) return ret;
            // Already in CP
            AdjustSize();
            ret = index;
            constants[index] = new ConstantDouble(n);
            index += 2;
            // Wastes one entry according to spec
            return ret;
        }

        /// <summary>Look for ConstantNameAndType in ConstantPool.</summary>
        /// <param name="name">of variable/method</param>
        /// <param name="signature">of variable/method</param>
        /// <returns>index on success, -1 otherwise</returns>
        public virtual int LookupNameAndType(string name, string signature)
        {
            var _index = n_a_t_table.GetOrNull(name + NAT_DELIM
                                                    + signature);
            return _index != null ? _index.index : -1;
        }

        /// <summary>
        ///     Add a new NameAndType constant to the ConstantPool if it is not already
        ///     in there.
        /// </summary>
        /// <param name="name">Name string to add</param>
        /// <param name="signature">signature string to add</param>
        /// <returns>index of entry</returns>
        public virtual int AddNameAndType(string name, string signature)
        {
            int ret;
            int name_index;
            int signature_index;
            if ((ret = LookupNameAndType(name, signature)) != -1) return ret;
            // Already in CP
            AdjustSize();
            name_index = AddUtf8(name);
            signature_index = AddUtf8(signature);
            ret = index;
            constants[index++] = new ConstantNameAndType(name_index, signature_index
            );
            var key = name + NAT_DELIM + signature;
            if (!n_a_t_table.ContainsKey(key))
                Collections.Put(n_a_t_table, key, new Index
                    (ret));
            return ret;
        }

        /// <summary>Look for ConstantMethodref in ConstantPool.</summary>
        /// <param name="class_name">Where to find method</param>
        /// <param name="method_name">Guess what</param>
        /// <param name="signature">return and argument types</param>
        /// <returns>index on success, -1 otherwise</returns>
        public virtual int LookupMethodref(string class_name, string method_name, string
            signature)
        {
            var index = cp_table.GetOrNull(class_name + METHODREF_DELIM
                                                      + method_name + METHODREF_DELIM + signature);
            return index != null ? index.index : -1;
        }

        public virtual int LookupMethodref(MethodGen method)
        {
            return LookupMethodref(method.GetClassName(), method.GetName(), method.GetSignature
                ());
        }

        /// <summary>
        ///     Add a new Methodref constant to the ConstantPool, if it is not already
        ///     in there.
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
            if ((ret = LookupMethodref(class_name, method_name, signature)) != -1) return ret;
            // Already in CP
            AdjustSize();
            name_and_type_index = AddNameAndType(method_name, signature);
            class_index = AddClass(class_name);
            ret = index;
            constants[index++] = new ConstantMethodref(class_index, name_and_type_index
            );
            var key = class_name + METHODREF_DELIM + method_name + METHODREF_DELIM + signature;
            if (!cp_table.ContainsKey(key))
                Collections.Put(cp_table, key, new Index(ret
                ));
            return ret;
        }

        public virtual int AddMethodref(MethodGen method)
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
            var index = cp_table.GetOrNull(class_name + IMETHODREF_DELIM
                                                      + method_name + IMETHODREF_DELIM + signature);
            return index != null ? index.index : -1;
        }

        public virtual int LookupInterfaceMethodref(MethodGen method)
        {
            return LookupInterfaceMethodref(method.GetClassName(), method.GetName(), method.GetSignature
                ());
        }

        /// <summary>
        ///     Add a new InterfaceMethodref constant to the ConstantPool, if it is not already
        ///     in there.
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
            if ((ret = LookupInterfaceMethodref(class_name, method_name, signature)) != -1) return ret;
            // Already in CP
            AdjustSize();
            class_index = AddClass(class_name);
            name_and_type_index = AddNameAndType(method_name, signature);
            ret = index;
            constants[index++] = new ConstantInterfaceMethodref(class_index,
                name_and_type_index);
            var key = class_name + IMETHODREF_DELIM + method_name + IMETHODREF_DELIM + signature;
            if (!cp_table.ContainsKey(key))
                Collections.Put(cp_table, key, new Index(ret
                ));
            return ret;
        }

        public virtual int AddInterfaceMethodref(MethodGen method)
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
            var index = cp_table.GetOrNull(class_name + FIELDREF_DELIM
                                                      + field_name + FIELDREF_DELIM + signature);
            return index != null ? index.index : -1;
        }

        /// <summary>
        ///     Add a new Fieldref constant to the ConstantPool, if it is not already
        ///     in there.
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
            if ((ret = LookupFieldref(class_name, field_name, signature)) != -1) return ret;
            // Already in CP
            AdjustSize();
            class_index = AddClass(class_name);
            name_and_type_index = AddNameAndType(field_name, signature);
            ret = index;
            constants[index++] = new ConstantFieldref(class_index, name_and_type_index
            );
            var key = class_name + FIELDREF_DELIM + field_name + FIELDREF_DELIM + signature;
            if (!cp_table.ContainsKey(key))
                Collections.Put(cp_table, key, new Index(ret
                ));
            return ret;
        }

        /// <param name="i">index in constant pool</param>
        /// <returns>constant pool entry at index i</returns>
        public virtual Constant GetConstant(int i)
        {
            return constants[i];
        }

        /// <summary>Use with care!</summary>
        /// <param name="i">index in constant pool</param>
        /// <param name="c">new constant pool entry at index i</param>
        public virtual void SetConstant(int i, Constant c)
        {
            constants[i] = c;
        }

        /// <returns>intermediate constant pool</returns>
        public virtual ConstantPool GetConstantPool()
        {
            return new ConstantPool(constants);
        }

        /// <returns>current size of constant pool</returns>
        public virtual int GetSize()
        {
            return index;
        }

        /// <returns>constant pool with proper length</returns>
        public virtual ConstantPool GetFinalConstantPool()
        {
            var cs = new Constant[index];
            Array.Copy(constants, 0, cs, 0, index);
            return new ConstantPool(cs);
        }

        /// <returns>String representation.</returns>
        public override string ToString()
        {
            var buf = new StringBuilder();
            for (var i = 1; i < index; i++) buf.Append(i).Append(")").Append(constants[i]).Append("\n");
            return buf.ToString();
        }

        /// <summary>Import constant from another ConstantPool and return new index.</summary>
        public virtual int AddConstant(Constant c, ConstantPoolGen
            cp)
        {
            var constants = cp.GetConstantPool().GetConstantPool();
            switch (c.GetTag())
            {
                case Const.CONSTANT_String:
                {
                    var s = (ConstantString) c;
                    var u8 = (ConstantUtf8) constants[s.GetStringIndex
                        ()];
                    return AddString(u8.GetBytes());
                }

                case Const.CONSTANT_Class:
                {
                    var s = (ConstantClass) c;
                    var u8 = (ConstantUtf8) constants[s.GetNameIndex
                        ()];
                    return AddClass(u8.GetBytes());
                }

                case Const.CONSTANT_NameAndType:
                {
                    var n = (ConstantNameAndType) c;
                    var u8 = (ConstantUtf8) constants[n.GetNameIndex
                        ()];
                    var u8_2 = (ConstantUtf8) constants[n.GetSignatureIndex
                        ()];
                    return AddNameAndType(u8.GetBytes(), u8_2.GetBytes());
                }

                case Const.CONSTANT_Utf8:
                {
                    return AddUtf8(((ConstantUtf8) c).GetBytes());
                }

                case Const.CONSTANT_Double:
                {
                    return AddDouble(((ConstantDouble) c).GetBytes());
                }

                case Const.CONSTANT_Float:
                {
                    return AddFloat(((ConstantFloat) c).GetBytes());
                }

                case Const.CONSTANT_Long:
                {
                    return AddLong(((ConstantLong) c).GetBytes());
                }

                case Const.CONSTANT_Integer:
                {
                    return AddInteger(((ConstantInteger) c).GetBytes());
                }

                case Const.CONSTANT_InterfaceMethodref:
                case Const.CONSTANT_Methodref:
                case Const.CONSTANT_Fieldref:
                {
                    var m = (ConstantCP) c;
                    var clazz = (ConstantClass) constants[m.GetClassIndex()];
                    var n = (ConstantNameAndType) constants
                        [m.GetNameAndTypeIndex()];
                    var u8 = (ConstantUtf8) constants[clazz.GetNameIndex
                        ()];
                    var class_name = u8.GetBytes().Replace('/', '.');
                    u8 = (ConstantUtf8) constants[n.GetNameIndex()];
                    var name = u8.GetBytes();
                    u8 = (ConstantUtf8) constants[n.GetSignatureIndex()];
                    var signature = u8.GetBytes();
                    switch (c.GetTag())
                    {
                        case Const.CONSTANT_InterfaceMethodref:
                        {
                            return AddInterfaceMethodref(class_name, name, signature);
                        }

                        case Const.CONSTANT_Methodref:
                        {
                            return AddMethodref(class_name, name, signature);
                        }

                        case Const.CONSTANT_Fieldref:
                        {
                            return AddFieldref(class_name, name, signature);
                        }

                        default:
                        {
                            // Never reached
                            throw new Exception("Unknown constant type " + c);
                        }
                    }

                    goto default;
                }

                default:
                {
                    // Never reached
                    throw new Exception("Unknown constant type " + c);
                }
            }
        }

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
    }
}