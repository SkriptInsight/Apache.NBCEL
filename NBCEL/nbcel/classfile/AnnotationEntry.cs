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
	/// <summary>represents one annotation in the annotation table</summary>
	/// <since>6.0</since>
	public class AnnotationEntry : NBCEL.classfile.Node
	{
		private readonly int type_index;

		private readonly NBCEL.classfile.ConstantPool constant_pool;

		private readonly bool isRuntimeVisible__;

		private System.Collections.Generic.List<NBCEL.classfile.ElementValuePair> element_value_pairs;

		/*
		* Factory method to create an AnnotionEntry from a DataInput
		*
		* @param input
		* @param constant_pool
		* @param isRuntimeVisible
		* @return the entry
		* @throws IOException
		*/
		/// <exception cref="System.IO.IOException"/>
		public static NBCEL.classfile.AnnotationEntry Read(java.io.DataInput input, NBCEL.classfile.ConstantPool
			 constant_pool, bool isRuntimeVisible)
		{
			NBCEL.classfile.AnnotationEntry annotationEntry = new NBCEL.classfile.AnnotationEntry
				(input.ReadUnsignedShort(), constant_pool, isRuntimeVisible);
			int num_element_value_pairs = input.ReadUnsignedShort();
			annotationEntry.element_value_pairs = new System.Collections.Generic.List<NBCEL.classfile.ElementValuePair
				>();
			for (int i = 0; i < num_element_value_pairs; i++)
			{
				annotationEntry.element_value_pairs.Add(new NBCEL.classfile.ElementValuePair(input
					.ReadUnsignedShort(), NBCEL.classfile.ElementValue.ReadElementValue(input, constant_pool
					), constant_pool));
			}
			return annotationEntry;
		}

		public AnnotationEntry(int type_index, NBCEL.classfile.ConstantPool constant_pool
			, bool isRuntimeVisible)
		{
			this.type_index = type_index;
			this.constant_pool = constant_pool;
			this.isRuntimeVisible__ = isRuntimeVisible;
		}

		public virtual int GetTypeIndex()
		{
			return type_index;
		}

		public virtual NBCEL.classfile.ConstantPool GetConstantPool()
		{
			return constant_pool;
		}

		public virtual bool IsRuntimeVisible()
		{
			return isRuntimeVisible__;
		}

		/// <summary>Called by objects that are traversing the nodes of the tree implicitely defined by the contents of a Java class.
		/// 	</summary>
		/// <remarks>
		/// Called by objects that are traversing the nodes of the tree implicitely defined by the contents of a Java class.
		/// I.e., the hierarchy of methods, fields, attributes, etc. spawns a tree of objects.
		/// </remarks>
		/// <param name="v">Visitor object</param>
		public virtual void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitAnnotationEntry(this);
		}

		/// <returns>the annotation type name</returns>
		public virtual string GetAnnotationType()
		{
			NBCEL.classfile.ConstantUtf8 c = (NBCEL.classfile.ConstantUtf8)constant_pool.GetConstant
				(type_index, NBCEL.Const.CONSTANT_Utf8);
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
		public virtual NBCEL.classfile.ElementValuePair[] GetElementValuePairs()
		{
			// TODO return List
			return Sharpen.Collections.ToArray(element_value_pairs, new NBCEL.classfile.ElementValuePair
				[element_value_pairs.Count]);
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void Dump(java.io.DataOutputStream dos)
		{
			dos.WriteShort(type_index);
			// u2 index of type name in cpool
			dos.WriteShort(element_value_pairs.Count);
			// u2 element_value pair
			// count
			foreach (NBCEL.classfile.ElementValuePair envp in element_value_pairs)
			{
				envp.Dump(dos);
			}
		}

		public virtual void AddElementNameValuePair(NBCEL.classfile.ElementValuePair elementNameValuePair
			)
		{
			element_value_pairs.Add(elementNameValuePair);
		}

		public virtual string ToShortString()
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder();
			result.Append("@");
			result.Append(GetAnnotationType());
			NBCEL.classfile.ElementValuePair[] evPairs = GetElementValuePairs();
			if (evPairs.Length > 0)
			{
				result.Append("(");
				foreach (NBCEL.classfile.ElementValuePair element in evPairs)
				{
					result.Append(element.ToShortString());
				}
				result.Append(")");
			}
			return result.ToString();
		}

		public override string ToString()
		{
			return ToShortString();
		}

		public static NBCEL.classfile.AnnotationEntry[] CreateAnnotationEntries(NBCEL.classfile.Attribute
			[] attrs)
		{
			// Find attributes that contain annotation data
			System.Collections.Generic.List<NBCEL.classfile.AnnotationEntry> accumulatedAnnotations
				 = new System.Collections.Generic.List<NBCEL.classfile.AnnotationEntry>(attrs.Length
				);
			foreach (NBCEL.classfile.Attribute attribute in attrs)
			{
				if (attribute is NBCEL.classfile.Annotations)
				{
					NBCEL.classfile.Annotations runtimeAnnotations = (NBCEL.classfile.Annotations)attribute;
					Sharpen.Collections.AddAll(accumulatedAnnotations, runtimeAnnotations.GetAnnotationEntries
						());
				}
			}
			return Sharpen.Collections.ToArray(accumulatedAnnotations, new NBCEL.classfile.AnnotationEntry
				[accumulatedAnnotations.Count]);
		}
	}
}
