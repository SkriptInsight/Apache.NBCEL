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

namespace NBCEL.util
{
	/// <summary>
	/// Utility class implementing a (typesafe) collection of JavaClass
	/// objects.
	/// </summary>
	/// <remarks>
	/// Utility class implementing a (typesafe) collection of JavaClass
	/// objects. Contains the most important methods of a Vector.
	/// </remarks>
	[System.Serializable]
	[System.ObsoleteAttribute(@"as of 5.1.1 - 7/17/2005")]
	public class ClassVector
	{
		private const long serialVersionUID = 5600397075672780806L;

		[System.Obsolete]
		protected internal System.Collections.Generic.List<NBCEL.classfile.JavaClass> vec
			 = new System.Collections.Generic.List<NBCEL.classfile.JavaClass>();

		public virtual void AddElement(NBCEL.classfile.JavaClass clazz)
		{
			vec.Add(clazz);
		}

		public virtual NBCEL.classfile.JavaClass ElementAt(int index)
		{
			return vec[index];
		}

		public virtual void RemoveElementAt(int index)
		{
			vec.RemoveAtReturningValue(index);
		}

		public virtual NBCEL.classfile.JavaClass[] ToArray()
		{
			NBCEL.classfile.JavaClass[] classes = new NBCEL.classfile.JavaClass[vec.Count];
			Sharpen.Collections.ToArray(vec, classes);
			return classes;
		}
	}
}
