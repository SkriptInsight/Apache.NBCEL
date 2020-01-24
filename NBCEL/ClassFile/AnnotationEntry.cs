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

using System.Collections.Generic;
using System.Text;
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>represents one annotation in the annotation table</summary>
	/// <since>6.0</since>
	public class AnnotationEntry : Node
    {
        private readonly ConstantPool constant_pool;

        private readonly bool isRuntimeVisible__;
        private readonly int type_index;

        private List<ElementValuePair> element_value_pairs;

        public AnnotationEntry(int type_index, ConstantPool constant_pool
            , bool isRuntimeVisible)
        {
            this.type_index = type_index;
            this.constant_pool = constant_pool;
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
        public virtual void Accept(Visitor v)
        {
            v.VisitAnnotationEntry(this);
        }

        /*
        * Factory method to create an AnnotionEntry from a DataInput
        *
        * @param input
        * @param constant_pool
        * @param isRuntimeVisible
        * @return the entry
        * @throws IOException
        */
        /// <exception cref="System.IO.IOException" />
        public static AnnotationEntry Read(DataInput input, ConstantPool
            constant_pool, bool isRuntimeVisible)
        {
            var annotationEntry = new AnnotationEntry
                (input.ReadUnsignedShort(), constant_pool, isRuntimeVisible);
            var num_element_value_pairs = input.ReadUnsignedShort();
            annotationEntry.element_value_pairs = new List<ElementValuePair
            >();
            for (var i = 0; i < num_element_value_pairs; i++)
                annotationEntry.element_value_pairs.Add(new ElementValuePair(input
                    .ReadUnsignedShort(), ElementValue.ReadElementValue(input, constant_pool
                ), constant_pool));
            return annotationEntry;
        }

        public virtual int GetTypeIndex()
        {
            return type_index;
        }

        public virtual ConstantPool GetConstantPool()
        {
            return constant_pool;
        }

        public virtual bool IsRuntimeVisible()
        {
            return isRuntimeVisible__;
        }

        /// <returns>the annotation type name</returns>
        public virtual string GetAnnotationType()
        {
            var c = (ConstantUtf8) constant_pool.GetConstant
                (type_index, Const.CONSTANT_Utf8);
            return c.GetBytes();
        }

        /// <returns>the annotation type index</returns>
        public virtual int GetAnnotationTypeIndex()
        {
            return type_index;
        }

        /// <returns>the number of element value pairs in this annotation entry</returns>
        public int GetNumElementValuePairs()
        {
            return element_value_pairs.Count;
        }

        /// <returns>the element value pairs in this annotation entry</returns>
        public virtual ElementValuePair[] GetElementValuePairs()
        {
            // TODO return List
            return Collections.ToArray(element_value_pairs, new ElementValuePair
                [element_value_pairs.Count]);
        }

        /// <exception cref="System.IO.IOException" />
        public virtual void Dump(DataOutputStream dos)
        {
            dos.WriteShort(type_index);
            // u2 index of type name in cpool
            dos.WriteShort(element_value_pairs.Count);
            // u2 element_value pair
            // count
            foreach (var envp in element_value_pairs) envp.Dump(dos);
        }

        public virtual void AddElementNameValuePair(ElementValuePair elementNameValuePair
        )
        {
            element_value_pairs.Add(elementNameValuePair);
        }

        public virtual string ToShortString()
        {
            var result = new StringBuilder();
            result.Append("@");
            result.Append(GetAnnotationType());
            var evPairs = GetElementValuePairs();
            if (evPairs.Length > 0)
            {
                result.Append("(");
                foreach (var element in evPairs) result.Append(element.ToShortString());
                result.Append(")");
            }

            return result.ToString();
        }

        public override string ToString()
        {
            return ToShortString();
        }

        public static AnnotationEntry[] CreateAnnotationEntries(Attribute
            [] attrs)
        {
            // Find attributes that contain annotation data
            var accumulatedAnnotations
                = new List<AnnotationEntry>(attrs.Length
                );
            foreach (var attribute in attrs)
                if (attribute is Annotations)
                {
                    var runtimeAnnotations = (Annotations) attribute;
                    Collections.AddAll(accumulatedAnnotations, runtimeAnnotations.GetAnnotationEntries
                        ());
                }

            return Collections.ToArray(accumulatedAnnotations, new AnnotationEntry
                [accumulatedAnnotations.Count]);
        }
    }
}