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
*/

using System.Text;
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>This class represents a BootstrapMethods attribute.</summary>
	/// <seealso>
	///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.23">
	///         * The class File Format : The BootstrapMethods Attribute
	///     </a>
	/// </seealso>
	/// <since>6.0</since>
	public class BootstrapMethods : Attribute
    {
        private BootstrapMethod[] bootstrap_methods;

        /// <summary>Initialize from another object.</summary>
        /// <remarks>
        ///     Initialize from another object. Note that both objects use the same
        ///     references (shallow copy). Use clone() for a physical copy.
        /// </remarks>
        public BootstrapMethods(BootstrapMethods c)
            : this(c.GetNameIndex(), c.GetLength(), c.GetBootstrapMethods(), c.GetConstantPool
                ())
        {
        }

        /// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="bootstrap_methods">array of bootstrap methods</param>
        /// <param name="constant_pool">Array of constants</param>
        public BootstrapMethods(int name_index, int length, BootstrapMethod
            [] bootstrap_methods, ConstantPool constant_pool)
            : base(Const.ATTR_BOOTSTRAP_METHODS, name_index, length, constant_pool)
        {
            // TODO this could be made final (setter is not used)
            this.bootstrap_methods = bootstrap_methods;
        }

        /// <summary>Construct object from Input stream.</summary>
        /// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal BootstrapMethods(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : this(name_index, length, (BootstrapMethod[]) null, constant_pool
            )
        {
            var num_bootstrap_methods = input.ReadUnsignedShort();
            bootstrap_methods = new BootstrapMethod[num_bootstrap_methods];
            for (var i = 0; i < num_bootstrap_methods; i++) bootstrap_methods[i] = new BootstrapMethod(input);
        }

        /// <returns>array of bootstrap method "records"</returns>
        public BootstrapMethod[] GetBootstrapMethods()
        {
            return bootstrap_methods;
        }

        /// <param name="bootstrap_methods">the array of bootstrap methods</param>
        public void SetBootstrapMethods(BootstrapMethod[] bootstrap_methods
        )
        {
            this.bootstrap_methods = bootstrap_methods;
        }

        /// <param name="v">Visitor object</param>
        public override void Accept(Visitor v)
        {
            v.VisitBootstrapMethods(this);
        }

        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            var c = (BootstrapMethods) Clone();
            c.bootstrap_methods = new BootstrapMethod[bootstrap_methods.Length
            ];
            for (var i = 0; i < bootstrap_methods.Length; i++) c.bootstrap_methods[i] = bootstrap_methods[i].Copy();
            c.SetConstantPool(_constant_pool);
            return c;
        }

        /// <summary>Dump bootstrap methods attribute to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public sealed override void Dump(DataOutputStream file)
        {
            base.Dump(file);
            file.WriteShort(bootstrap_methods.Length);
            foreach (var bootstrap_method in bootstrap_methods) bootstrap_method.Dump(file);
        }

        /// <returns>String representation.</returns>
        public sealed override string ToString()
        {
            var buf = new StringBuilder();
            buf.Append("BootstrapMethods(");
            buf.Append(bootstrap_methods.Length);
            buf.Append("):");
            for (var i = 0; i < bootstrap_methods.Length; i++)
            {
                buf.Append("\n");
                var start = buf.Length;
                buf.Append("  ").Append(i).Append(": ");
                var indent_count = buf.Length - start;
                var lines = bootstrap_methods[i].ToString(GetConstantPool()).Split("\\r?\\n"
                );
                buf.Append(lines[0]);
                for (var j = 1; j < lines.Length; j++)
                    buf.Append("\n").Append(Runtime.Substring("          ", 0, indent_count))
                        .Append(lines[j]);
            }

            return buf.ToString();
        }
    }
}