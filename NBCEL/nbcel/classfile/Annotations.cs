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
	/// <summary>base class for annotations</summary>
	/// <since>6.0</since>
	public abstract class Annotations : Attribute
    {
        private readonly bool isRuntimeVisible__;
        private AnnotationEntry[] annotation_table;

        /// <param name="annotation_type">the subclass type of the annotation</param>
        /// <param name="name_index">Index pointing to the name <em>Code</em></param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal Annotations(byte annotation_type, int name_index, int length, DataInput
            input, ConstantPool constant_pool, bool isRuntimeVisible)
            : this(annotation_type, name_index, length, (AnnotationEntry[]) null
                , constant_pool, isRuntimeVisible)
        {
            var annotation_table_length = input.ReadUnsignedShort();
            annotation_table = new AnnotationEntry[annotation_table_length];
            for (var i = 0; i < annotation_table_length; i++)
                annotation_table[i] = AnnotationEntry.Read(input, constant_pool,
                    isRuntimeVisible);
        }

        /// <param name="annotation_type">the subclass type of the annotation</param>
        /// <param name="name_index">Index pointing to the name <em>Code</em></param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="annotation_table">the actual annotations</param>
        /// <param name="constant_pool">Array of constants</param>
        public Annotations(byte annotation_type, int name_index, int length, AnnotationEntry
                [] annotation_table, ConstantPool constant_pool, bool isRuntimeVisible
        )
            : base(annotation_type, name_index, length, constant_pool)
        {
            this.annotation_table = annotation_table;
            isRuntimeVisible__ = isRuntimeVisible;
        }

        /// <summary>
        ///     Called by objects that are traversing the nodes of the tree implicitely defined by the contents of a Java class.
        /// </summary>
        /// <remarks>
        ///     Called by objects that are traversing the nodes of the tree implicitely defined by the contents of a Java class.
        ///     I.e., the hierarchy of methods, fields, attributes, etc. spawns a tree of objects.
        /// </remarks>
        /// <param name="v">Visitor object</param>
        public override void Accept(Visitor v)
        {
            v.VisitAnnotation(this);
        }

        /// <param name="annotation_table">the entries to set in this annotation</param>
        public void SetAnnotationTable(AnnotationEntry[] annotation_table
        )
        {
            this.annotation_table = annotation_table;
        }

        /// <summary>returns the array of annotation entries in this annotation</summary>
        public virtual AnnotationEntry[] GetAnnotationEntries()
        {
            return annotation_table;
        }

        /// <returns>the number of annotation entries in this annotation</returns>
        public int GetNumAnnotations()
        {
            if (annotation_table == null) return 0;
            return annotation_table.Length;
        }

        public virtual bool IsRuntimeVisible()
        {
            return isRuntimeVisible__;
        }

        /// <exception cref="System.IO.IOException" />
        protected internal virtual void WriteAnnotations(DataOutputStream dos)
        {
            if (annotation_table == null) return;
            dos.WriteShort(annotation_table.Length);
            foreach (var element in annotation_table) element.Dump(dos);
        }
    }
}