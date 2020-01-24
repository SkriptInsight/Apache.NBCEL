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
	/// specified in the given class path. By default, this is the value returned by ClassPath.getClassPath().
	/// <p>
	/// This repository uses a factory design, allowing it to maintain a collection of different classpaths, and as such It is designed to be used as a singleton per
	/// classpath.
	/// </p>
	/// </remarks>
	/// <seealso cref="NBCEL.Repository"/>
	public class SyntheticRepository : NBCEL.util.MemorySensitiveClassPathRepository
	{
		private static readonly System.Collections.Generic.IDictionary<NBCEL.util.ClassPath
			, NBCEL.util.SyntheticRepository> instances = new System.Collections.Generic.Dictionary
			<NBCEL.util.ClassPath, NBCEL.util.SyntheticRepository>();

		private SyntheticRepository(NBCEL.util.ClassPath path)
			: base(path)
		{
		}

		// private static final String DEFAULT_PATH = ClassPath.getClassPath();
		// CLASSPATH X REPOSITORY
		public static NBCEL.util.SyntheticRepository GetInstance()
		{
			return GetInstance(NBCEL.util.ClassPath.SYSTEM_CLASS_PATH);
		}

		public static NBCEL.util.SyntheticRepository GetInstance(NBCEL.util.ClassPath classPath
			)
		{
			NBCEL.util.SyntheticRepository rep = instances.GetOrNull(classPath);
			if (rep == null)
			{
				rep = new NBCEL.util.SyntheticRepository(classPath);
				Sharpen.Collections.Put(instances, classPath, rep);
			}
			return rep;
		}
	}
}
