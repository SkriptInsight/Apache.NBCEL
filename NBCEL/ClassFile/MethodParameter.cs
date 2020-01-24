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
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>Entry of the parameters table.</summary>
	/// <seealso>
	///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.24">
	///         * The class File Format : The MethodParameters Attribute
	///     </a>
	/// </seealso>
	/// <since>6.0</since>
	public class MethodParameter : ICloneable
    {
        /// <summary>The access flags</summary>
        private int access_flags;

        /// <summary>
        ///     Index of the CONSTANT_Utf8_info structure in the constant_pool table representing the name of the parameter
        /// </summary>
        private int name_index;

        public MethodParameter()
        {
        }

        /// <summary>Construct object from input stream.</summary>
        /// <param name="input">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="ClassFormatException" />
        internal MethodParameter(DataInput input)
        {
            name_index = input.ReadUnsignedShort();
            access_flags = input.ReadUnsignedShort();
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        public virtual int NameIndex
        {
            get => name_index;
            set => this.name_index = value;
        }

        /// <summary>Returns the name of the parameter.</summary>
        public virtual string GetParameterName(ConstantPool constant_pool
        )
        {
            if (name_index == 0) return null;
            return ((ConstantUtf8) constant_pool.GetConstant(name_index, Const
                .CONSTANT_Utf8)).GetBytes();
        }

        public virtual int AccessFlags
        {
            get => access_flags;
            set => this.access_flags = value;
        }

        public virtual bool IsFinal => (access_flags & Const.ACC_FINAL) != 0;

        public virtual bool IsSynthetic => (access_flags & Const.ACC_SYNTHETIC) != 0;

        public virtual bool IsMandated => (access_flags & Const.ACC_MANDATED) != 0;

        public virtual void Accept(Visitor v)
        {
            v.VisitMethodParameter(this);
        }

        /// <summary>Dump object to file stream on binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public void Dump(DataOutputStream file)
        {
            file.WriteShort(name_index);
            file.WriteShort(access_flags);
        }

        /// <returns>deep copy of this object</returns>
        public virtual MethodParameter Copy()
        {
            return (MethodParameter) MemberwiseClone();
            // TODO should this throw?
            return null;
        }
    }
}