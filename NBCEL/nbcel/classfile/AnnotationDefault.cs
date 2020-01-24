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
	/// <summary>Represents the default value of a annotation for a method info</summary>
	/// <since>6.0</since>
	public class AnnotationDefault : Attribute
    {
        private ElementValue default_value;

        /// <param name="name_index">Index pointing to the name <em>Code</em></param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal AnnotationDefault(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : this(name_index, length, (ElementValue) null, constant_pool)
        {
            default_value = ElementValue.ReadElementValue(input, constant_pool
            );
        }

        /// <param name="name_index">Index pointing to the name <em>Code</em></param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="defaultValue">the annotation's default value</param>
        /// <param name="constant_pool">Array of constants</param>
        public AnnotationDefault(int name_index, int length, ElementValue
            defaultValue, ConstantPool constant_pool)
            : base(Const.ATTR_ANNOTATION_DEFAULT, name_index, length, constant_pool)
        {
            default_value = defaultValue;
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
            v.VisitAnnotationDefault(this);
        }

        /// <param name="defaultValue">the default value of this methodinfo's annotation</param>
        public void SetDefaultValue(ElementValue defaultValue)
        {
            default_value = defaultValue;
        }

        /// <returns>the default value</returns>
        public ElementValue GetDefaultValue()
        {
            return default_value;
        }

        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            return (Attribute) Clone();
        }

        /// <exception cref="System.IO.IOException" />
        public sealed override void Dump(DataOutputStream dos)
        {
            base.Dump(dos);
            default_value.Dump(dos);
        }
    }
}