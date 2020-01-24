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

using System;
using System.Text;
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	///     This class represents a bootstrap method attribute, i.e., the bootstrap
	///     method ref, the number of bootstrap arguments and an array of the
	///     bootstrap arguments.
	/// </summary>
	/// <seealso>
	///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.23">
	///         * The class File Format : The BootstrapMethods Attribute
	///     </a>
	/// </seealso>
	/// <since>6.0</since>
	public class BootstrapMethod : ICloneable
    {
        /// <summary>Array of references to the constant_pool table</summary>
        private int[] bootstrap_arguments;

        /// <summary>
        ///     Index of the CONSTANT_MethodHandle_info structure in the constant_pool table
        /// </summary>
        private int bootstrap_method_ref;

        /// <summary>Initialize from another object.</summary>
        public BootstrapMethod(BootstrapMethod c)
            : this(c.GetBootstrapMethodRef(), c.GetBootstrapArguments())
        {
        }

        /// <summary>Construct object from input stream.</summary>
        /// <param name="input">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal BootstrapMethod(DataInput input)
            : this(input.ReadUnsignedShort(), input.ReadUnsignedShort())
        {
            for (var i = 0; i < bootstrap_arguments.Length; i++)
            {
                bootstrap_arguments[i] = input.ReadUnsignedShort();
            }
        }

        private BootstrapMethod(int bootstrap_method_ref, int num_bootstrap_arguments)
            : this(bootstrap_method_ref, new int[num_bootstrap_arguments])
        {
        }

        /// <param name="bootstrap_method_ref">
        ///     int index into constant_pool of CONSTANT_MethodHandle
        /// </param>
        /// <param name="bootstrap_arguments">
        ///     int[] indices into constant_pool of CONSTANT_[type]_info
        /// </param>
        public BootstrapMethod(int bootstrap_method_ref, int[] bootstrap_arguments)
        {
            // helper method
            this.bootstrap_method_ref = bootstrap_method_ref;
            this.bootstrap_arguments = bootstrap_arguments;
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        /// <returns>index into constant_pool of bootstrap_method</returns>
        public virtual int GetBootstrapMethodRef()
        {
            return bootstrap_method_ref;
        }

        /// <param name="bootstrap_method_ref">
        ///     int index into constant_pool of CONSTANT_MethodHandle
        /// </param>
        public virtual void SetBootstrapMethodRef(int bootstrap_method_ref)
        {
            this.bootstrap_method_ref = bootstrap_method_ref;
        }

        /// <returns>
        ///     int[] of bootstrap_method indices into constant_pool of CONSTANT_[type]_info
        /// </returns>
        public virtual int[] GetBootstrapArguments()
        {
            return bootstrap_arguments;
        }

        /// <returns>count of number of boostrap arguments</returns>
        public virtual int GetNumBootstrapArguments()
        {
            return bootstrap_arguments.Length;
        }

        /// <param name="bootstrap_arguments">
        ///     int[] indices into constant_pool of CONSTANT_[type]_info
        /// </param>
        public virtual void SetBootstrapArguments(int[] bootstrap_arguments)
        {
            this.bootstrap_arguments = bootstrap_arguments;
        }

        /// <returns>String representation.</returns>
        public sealed override string ToString()
        {
            return "BootstrapMethod(" + bootstrap_method_ref + ", " + bootstrap_arguments.Length
                   + ", " + Arrays.ToString(bootstrap_arguments) + ")";
        }

        /// <returns>Resolved string representation</returns>
        public string ToString(ConstantPool constant_pool)
        {
            var buf = new StringBuilder();
            string bootstrap_method_name;
            bootstrap_method_name = constant_pool.ConstantToString(bootstrap_method_ref, Const
                .CONSTANT_MethodHandle);
            buf.Append(Utility.CompactClassName(bootstrap_method_name, false)
            );
            var num_bootstrap_arguments = bootstrap_arguments.Length;
            if (num_bootstrap_arguments > 0)
            {
                buf.Append("\nMethod Arguments:");
                for (var i = 0; i < num_bootstrap_arguments; i++)
                {
                    buf.Append("\n  ").Append(i).Append(": ");
                    buf.Append(constant_pool.ConstantToString(constant_pool.GetConstant(bootstrap_arguments
                        [i])));
                }
            }

            return buf.ToString();
        }

        /// <summary>Dump object to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public void Dump(DataOutputStream file)
        {
            file.WriteShort(bootstrap_method_ref);
            file.WriteShort(bootstrap_arguments.Length);
            foreach (var bootstrap_argument in bootstrap_arguments)
            {
                file.WriteShort(bootstrap_argument);
            }
        }

        /// <returns>deep copy of this object</returns>
        public virtual BootstrapMethod Copy()
        {
            return (BootstrapMethod) MemberwiseClone();
            // TODO should this throw?
            return null;
        }
    }
}