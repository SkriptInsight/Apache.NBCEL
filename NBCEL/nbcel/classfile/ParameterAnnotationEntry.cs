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
	/// <summary>represents one parameter annotation in the parameter annotation table</summary>
	/// <since>6.0</since>
	public class ParameterAnnotationEntry : NBCEL.classfile.Node
	{
		private readonly NBCEL.classfile.AnnotationEntry[] annotation_table;

		/// <summary>Construct object from input stream.</summary>
		/// <param name="input">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		internal ParameterAnnotationEntry(java.io.DataInput input, NBCEL.classfile.ConstantPool
			 constant_pool)
		{
			int annotation_table_length = input.ReadUnsignedShort();
			annotation_table = new NBCEL.classfile.AnnotationEntry[annotation_table_length];
			for (int i = 0; i < annotation_table_length; i++)
			{
				// TODO isRuntimeVisible
				annotation_table[i] = NBCEL.classfile.AnnotationEntry.Read(input, constant_pool, 
					false);
			}
		}

		/// <summary>
		/// Called by objects that are traversing the nodes of the tree implicitely
		/// defined by the contents of a Java class.
		/// </summary>
		/// <remarks>
		/// Called by objects that are traversing the nodes of the tree implicitely
		/// defined by the contents of a Java class. I.e., the hierarchy of methods,
		/// fields, attributes, etc. spawns a tree of objects.
		/// </remarks>
		/// <param name="v">Visitor object</param>
		public virtual void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitParameterAnnotationEntry(this);
		}

		/// <summary>returns the array of annotation entries in this annotation</summary>
		public virtual NBCEL.classfile.AnnotationEntry[] GetAnnotationEntries()
		{
			return annotation_table;
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void Dump(java.io.DataOutputStream dos)
		{
			dos.WriteShort(annotation_table.Length);
			foreach (NBCEL.classfile.AnnotationEntry entry in annotation_table)
			{
				entry.Dump(dos);
			}
		}

		public static NBCEL.classfile.ParameterAnnotationEntry[] CreateParameterAnnotationEntries
			(NBCEL.classfile.Attribute[] attrs)
		{
			// Find attributes that contain parameter annotation data
			System.Collections.Generic.List<NBCEL.classfile.ParameterAnnotationEntry> accumulatedAnnotations
				 = new System.Collections.Generic.List<NBCEL.classfile.ParameterAnnotationEntry>
				(attrs.Length);
			foreach (NBCEL.classfile.Attribute attribute in attrs)
			{
				if (attribute is NBCEL.classfile.ParameterAnnotations)
				{
					NBCEL.classfile.ParameterAnnotations runtimeAnnotations = (NBCEL.classfile.ParameterAnnotations
						)attribute;
					Sharpen.Collections.AddAll(accumulatedAnnotations, runtimeAnnotations.GetParameterAnnotationEntries
						());
				}
			}
			return Sharpen.Collections.ToArray(accumulatedAnnotations, new NBCEL.classfile.ParameterAnnotationEntry
				[accumulatedAnnotations.Count]);
		}
	}
}
