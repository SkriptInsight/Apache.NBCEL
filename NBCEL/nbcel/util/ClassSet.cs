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
	/// <summary>Utility class implementing a (typesafe) set of JavaClass objects.</summary>
	/// <remarks>
	/// Utility class implementing a (typesafe) set of JavaClass objects.
	/// Since JavaClass has no equals() method, the name of the class is
	/// used for comparison.
	/// </remarks>
	/// <seealso cref="ClassStack"/>
	public class ClassSet
	{
		private readonly System.Collections.Generic.IDictionary<string, NBCEL.classfile.JavaClass
			> map = new System.Collections.Generic.Dictionary<string, NBCEL.classfile.JavaClass
			>();

		public virtual bool Add(NBCEL.classfile.JavaClass clazz)
		{
			bool result = false;
			if (!map.ContainsKey(clazz.GetClassName()))
			{
				result = true;
				Sharpen.Collections.Put(map, clazz.GetClassName(), clazz);
			}
			return result;
		}

		public virtual void Remove(NBCEL.classfile.JavaClass clazz)
		{
			Sharpen.Collections.Remove(map, clazz.GetClassName());
		}

		public virtual bool Empty()
		{
			return (map.Count == 0);
		}

		public virtual NBCEL.classfile.JavaClass[] ToArray()
		{
			System.Collections.Generic.ICollection<NBCEL.classfile.JavaClass> values = map.Values;
			NBCEL.classfile.JavaClass[] classes = new NBCEL.classfile.JavaClass[values.Count]
				;
			Sharpen.Collections.ToArray(values, classes);
			return classes;
		}

		public virtual string[] GetClassNames()
		{
			return Sharpen.Collections.ToArray(map.Keys, new string[map.Count]);
		}
	}
}
