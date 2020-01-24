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

using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>This class represents a MethodParameters attribute.</summary>
	/// <seealso>
	///     <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.24">
	///         * The class File Format : The MethodParameters Attribute
	///     </a>
	/// </seealso>
	/// <since>6.0</since>
	public class MethodParameters : Attribute
    {
        private MethodParameter[] parameters = new MethodParameter
            [0];

        /// <exception cref="System.IO.IOException" />
        internal MethodParameters(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : base(Const.ATTR_METHOD_PARAMETERS, name_index, length, constant_pool)
        {
            var parameters_count = input.ReadUnsignedByte();
            parameters = new MethodParameter[parameters_count];
            for (var i = 0; i < parameters_count; i++) parameters[i] = new MethodParameter(input);
        }

        public virtual MethodParameter[] GetParameters()
        {
            return parameters;
        }

        public virtual void SetParameters(MethodParameter[] parameters)
        {
            this.parameters = parameters;
        }

        public override void Accept(Visitor v)
        {
            v.VisitMethodParameters(this);
        }

        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            var c = (MethodParameters) Clone();
            c.parameters = new MethodParameter[parameters.Length];
            for (var i = 0; i < parameters.Length; i++) c.parameters[i] = parameters[i].Copy();
            c.SetConstantPool(_constant_pool);
            return c;
        }

        /// <summary>Dump method parameters attribute to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            base.Dump(file);
            file.WriteByte(parameters.Length);
            foreach (var parameter in parameters) parameter.Dump(file);
        }
    }
}