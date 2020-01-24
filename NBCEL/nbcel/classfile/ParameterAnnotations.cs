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
	/// <summary>base class for parameter annotations</summary>
	/// <since>6.0</since>
	public abstract class ParameterAnnotations : Attribute
    {
        /// <summary>Table of parameter annotations</summary>
        private ParameterAnnotationEntry[] parameter_annotation_table;

        /// <param name="parameter_annotation_type">
        ///     the subclass type of the parameter annotation
        /// </param>
        /// <param name="name_index">Index pointing to the name <em>Code</em></param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal ParameterAnnotations(byte parameter_annotation_type, int name_index, int
            length, DataInput input, ConstantPool constant_pool)
            : this(parameter_annotation_type, name_index, length, (ParameterAnnotationEntry
                []) null, constant_pool)
        {
            var num_parameters = input.ReadUnsignedByte();
            parameter_annotation_table = new ParameterAnnotationEntry[num_parameters
            ];
            for (var i = 0; i < num_parameters; i++)
                parameter_annotation_table[i] = new ParameterAnnotationEntry(input
                    , constant_pool);
        }

        /// <param name="parameter_annotation_type">
        ///     the subclass type of the parameter annotation
        /// </param>
        /// <param name="name_index">Index pointing to the name <em>Code</em></param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="parameter_annotation_table">the actual parameter annotations</param>
        /// <param name="constant_pool">Array of constants</param>
        public ParameterAnnotations(byte parameter_annotation_type, int name_index, int length
            , ParameterAnnotationEntry[] parameter_annotation_table, ConstantPool
                constant_pool)
            : base(parameter_annotation_type, name_index, length, constant_pool)
        {
            this.parameter_annotation_table = parameter_annotation_table;
        }

        /// <summary>
        ///     Called by objects that are traversing the nodes of the tree implicitely
        ///     defined by the contents of a Java class.
        /// </summary>
        /// <remarks>
        ///     Called by objects that are traversing the nodes of the tree implicitely
        ///     defined by the contents of a Java class. I.e., the hierarchy of methods,
        ///     fields, attributes, etc. spawns a tree of objects.
        /// </remarks>
        /// <param name="v">Visitor object</param>
        public override void Accept(Visitor v)
        {
            v.VisitParameterAnnotation(this);
        }

        /// <param name="parameter_annotation_table">
        ///     the entries to set in this parameter annotation
        /// </param>
        public void SetParameterAnnotationTable(ParameterAnnotationEntry[]
            parameter_annotation_table)
        {
            this.parameter_annotation_table = parameter_annotation_table;
        }

        /// <returns>the parameter annotation entry table</returns>
        public ParameterAnnotationEntry[] GetParameterAnnotationTable()
        {
            return parameter_annotation_table;
        }

        /// <summary>
        ///     returns the array of parameter annotation entries in this parameter annotation
        /// </summary>
        public virtual ParameterAnnotationEntry[] GetParameterAnnotationEntries
            ()
        {
            return parameter_annotation_table;
        }

        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream dos)
        {
            base.Dump(dos);
            dos.WriteByte(parameter_annotation_table.Length);
            foreach (var element in parameter_annotation_table) element.Dump(dos);
        }

        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool constant_pool
        )
        {
            return (Attribute) Clone();
        }
    }
}