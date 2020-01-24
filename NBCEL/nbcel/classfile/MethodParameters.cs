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
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>This class represents a MethodParameters attribute.</summary>
	/// <seealso><a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.24">
	/// * The class File Format : The MethodParameters Attribute</a></seealso>
	/// <since>6.0</since>
	public class MethodParameters : NBCEL.classfile.Attribute
	{
		private NBCEL.classfile.MethodParameter[] parameters = new NBCEL.classfile.MethodParameter
			[0];

		/// <exception cref="System.IO.IOException"/>
		internal MethodParameters(int name_index, int length, java.io.DataInput input, NBCEL.classfile.ConstantPool
			 constant_pool)
			: base(NBCEL.Const.ATTR_METHOD_PARAMETERS, name_index, length, constant_pool)
		{
			int parameters_count = input.ReadUnsignedByte();
			parameters = new NBCEL.classfile.MethodParameter[parameters_count];
			for (int i = 0; i < parameters_count; i++)
			{
				parameters[i] = new NBCEL.classfile.MethodParameter(input);
			}
		}

		public virtual NBCEL.classfile.MethodParameter[] GetParameters()
		{
			return parameters;
		}

		public virtual void SetParameters(NBCEL.classfile.MethodParameter[] parameters)
		{
			this.parameters = parameters;
		}

		public override void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitMethodParameters(this);
		}

		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool _constant_pool
			)
		{
			NBCEL.classfile.MethodParameters c = (NBCEL.classfile.MethodParameters)Clone();
			c.parameters = new NBCEL.classfile.MethodParameter[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				c.parameters[i] = parameters[i].Copy();
			}
			c.SetConstantPool(_constant_pool);
			return c;
		}

		/// <summary>Dump method parameters attribute to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteByte(parameters.Length);
			foreach (NBCEL.classfile.MethodParameter parameter in parameters)
			{
				parameter.Dump(file);
			}
		}
	}
}
