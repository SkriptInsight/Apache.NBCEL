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
	/// <summary>This repository is used in situations where a Class is created outside the realm of a ClassLoader.
	/// 	</summary>
	/// <remarks>
	/// This repository is used in situations where a Class is created outside the realm of a ClassLoader. Classes are loaded from the file systems using the paths
	/// specified in the given class path. By default, this is the value returned by ClassPath.getClassPath(). This repository holds onto classes with
	/// SoftReferences, and will reload as needed, in cases where memory sizes are important.
	/// </remarks>
	/// <seealso cref="NBCEL.Repository"/>
	public class MemorySensitiveClassPathRepository : NBCEL.util.AbstractClassPathRepository
	{
		private readonly System.Collections.Generic.IDictionary<string, NBCEL.classfile.JavaClass> _loadedClasses = new System.Collections.Generic.Dictionary
			<string, NBCEL.classfile.JavaClass>();

		public MemorySensitiveClassPathRepository(NBCEL.util.ClassPath path)
			: base(path)
		{
		}

		// CLASSNAME X JAVACLASS
		/// <summary>Store a new JavaClass instance into this Repository.</summary>
		public override void StoreClass(NBCEL.classfile.JavaClass clazz)
		{
			// Not calling super.storeClass because this subclass maintains the mapping.
			Sharpen.Collections.Put(_loadedClasses, clazz.GetClassName(), (clazz));
			clazz.SetRepository(this);
		}

		/// <summary>Remove class from repository</summary>
		public override void RemoveClass(NBCEL.classfile.JavaClass clazz)
		{
			Sharpen.Collections.Remove(_loadedClasses, clazz.GetClassName());
		}

		/// <summary>Find an already defined (cached) JavaClass object by name.</summary>
		public override NBCEL.classfile.JavaClass FindClass(string className)
		{
			var @ref = _loadedClasses.GetOrNull(className);

			return @ref;
		}

		/// <summary>Clear all entries from cache.</summary>
		public override void Clear()
		{
			_loadedClasses.Clear();
		}
	}
}
