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
	/// Represents a parameter annotation that is represented in the class file
	/// but is not provided to the JVM.
	/// </summary>
	/// <since>6.0</since>
	public class RuntimeInvisibleParameterAnnotations : NBCEL.classfile.ParameterAnnotations
	{
		/// <param name="name_index">Index pointing to the name <em>Code</em></param>
		/// <param name="length">Content length in bytes</param>
		/// <param name="input">Input stream</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <exception cref="System.IO.IOException"/>
		public RuntimeInvisibleParameterAnnotations(int name_index, int length, java.io.DataInput
			 input, NBCEL.classfile.ConstantPool constant_pool)
			: base(NBCEL.Const.ATTR_RUNTIME_INVISIBLE_PARAMETER_ANNOTATIONS, name_index, length
				, input, constant_pool)
		{
		}
	}
}
