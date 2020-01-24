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
	/// This abstract class provides a logic of a loading
	/// <see cref="NBCEL.classfile.JavaClass"/>
	/// objects class names via
	/// <see cref="ClassPath"/>
	/// .
	/// <p>Subclasses can choose caching strategy of the objects by implementing the abstract methods (e.g.,
	/// <see cref="StoreClass(NBCEL.classfile.JavaClass)"/>
	/// and
	/// <see cref="FindClass(string)"/>
	/// ).</p>
	/// </summary>
	/// <since>6.4.0</since>
	public abstract class AbstractClassPathRepository : NBCEL.util.Repository
	{
		private readonly NBCEL.util.ClassPath _path;

		internal AbstractClassPathRepository(NBCEL.util.ClassPath classPath)
		{
			_path = classPath;
		}

		public abstract void StoreClass(NBCEL.classfile.JavaClass javaClass);

		public abstract void RemoveClass(NBCEL.classfile.JavaClass javaClass);

		public abstract NBCEL.classfile.JavaClass FindClass(string className);

		public abstract void Clear();

		/// <summary>Finds a JavaClass object by name.</summary>
		/// <remarks>
		/// Finds a JavaClass object by name. If it is already in this Repository, the Repository version is returned.
		/// Otherwise, the Repository's classpath is searched for the class (and it is added to the Repository if found).
		/// </remarks>
		/// <param name="className">the name of the class</param>
		/// <returns>the JavaClass object</returns>
		/// <exception cref="System.TypeLoadException">if the class is not in the Repository, and could not be found on the classpath
		/// 	</exception>
		public virtual NBCEL.classfile.JavaClass LoadClass(string className)
		{
			if (className == null || (className.Length == 0))
			{
				throw new System.ArgumentException("Invalid class name " + className);
			}
			className = className.Replace('/', '.');
			// Just in case, canonical form
			NBCEL.classfile.JavaClass clazz = FindClass(className);
			if (clazz != null)
			{
				return clazz;
			}
			try
			{
				return LoadClass(_path.GetInputStream(className), className);
			}
			catch (System.IO.IOException e)
			{
				throw new System.TypeLoadException("Exception while looking for class " + className
					 + ": " + e, e);
			}
		}

		/// <summary>Finds the JavaClass object for a runtime Class object.</summary>
		/// <remarks>
		/// Finds the JavaClass object for a runtime Class object. If a class with the same name is already in this
		/// Repository, the Repository version is returned. Otherwise, getResourceAsStream() is called on the Class object to
		/// find the class's representation. If the representation is found, it is added to the Repository.
		/// </remarks>
		/// <seealso cref="System.Type{T}"/>
		/// <param name="clazz">the runtime Class object</param>
		/// <returns>JavaClass object for given runtime class</returns>
		/// <exception cref="System.TypeLoadException">if the class is not in the Repository, and its representation could not be found
		/// 	</exception>
		public virtual NBCEL.classfile.JavaClass LoadClass(System.Type clazz)
		{
			string className = clazz.FullName;
			NBCEL.classfile.JavaClass repositoryClass = FindClass(className);
			if (repositoryClass != null)
			{
				return repositoryClass;
			}
			string name = className;
			int i = name.LastIndexOf('.');
			if (i > 0)
			{
				name = Sharpen.Runtime.Substring(name, i + 1);
			}
			/*
			try
			{
				using (java.io.InputStream clsStream = clazz.GetResourceAsStream(name + ".class"))
				{
					return LoadClass(clsStream, className);
				}
			}
			catch (System.IO.IOException)
			{
				return null;
			}
		*/
			return null;
		}

		/// <exception cref="System.TypeLoadException"/>
		private NBCEL.classfile.JavaClass LoadClass(java.io.InputStream inputStream, string
			 className)
		{
			try
			{
				if (inputStream != null)
				{
					NBCEL.classfile.ClassParser parser = new NBCEL.classfile.ClassParser(inputStream, 
						className);
					NBCEL.classfile.JavaClass clazz = parser.Parse();
					StoreClass(clazz);
					return clazz;
				}
			}
			catch (System.IO.IOException e)
			{
				throw new System.TypeLoadException("Exception while looking for class " + className
					 + ": " + e, e);
			}
			finally
			{
				if (inputStream != null)
				{
					try
					{
						inputStream.Close();
					}
					catch (System.IO.IOException)
					{
					}
				}
			}
			// ignored
			throw new System.TypeLoadException("ClassRepository could not load " + className);
		}

		public virtual NBCEL.util.ClassPath GetClassPath()
		{
			return _path;
		}
	}
}
