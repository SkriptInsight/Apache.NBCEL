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

using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	///     represents an annotation that is represented in the class file and is
	///     provided to the JVM.
	/// </summary>
	/// <since>6.0</since>
	public class RuntimeVisibleAnnotations : Annotations
    {
	    /// <param name="name_index">Index pointing to the name <em>Code</em></param>
	    /// <param name="length">Content length in bytes</param>
	    /// <param name="input">Input stream</param>
	    /// <param name="constant_pool">Array of constants</param>
	    /// <exception cref="System.IO.IOException" />
	    public RuntimeVisibleAnnotations(int name_index, int length, DataInput input
            , ConstantPool constant_pool)
            : base(Const.ATTR_RUNTIME_VISIBLE_ANNOTATIONS, name_index, length, input, constant_pool
                , true)
        {
        }

        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool constant_pool
        )
        {
            return (Attribute) Clone();
        }

        /// <exception cref="System.IO.IOException" />
        public sealed override void Dump(DataOutputStream dos)
        {
            base.Dump(dos);
            WriteAnnotations(dos);
        }
    }
}