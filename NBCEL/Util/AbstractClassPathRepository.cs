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

using System;
using System.IO;
using Apache.NBCEL.ClassFile;
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.Util
{
	/// <summary>
	///     This abstract class provides a logic of a loading
	///     <see cref="JavaClass" />
	///     objects class names via
	///     <see cref="ClassPath" />
	///     .
	///     <p>
	///         Subclasses can choose caching strategy of the objects by implementing the abstract methods (e.g.,
	///         <see cref="StoreClass" />
	///         and
	///         <see cref="FindClass(string)" />
	///         ).
	///     </p>
	/// </summary>
	/// <since>6.4.0</since>
	public abstract class AbstractClassPathRepository : Repository
    {
        private readonly ClassPath _path;

        internal AbstractClassPathRepository(ClassPath classPath)
        {
            _path = classPath;
        }

        public abstract void StoreClass(JavaClass javaClass);

        public abstract void RemoveClass(JavaClass javaClass);

        public abstract JavaClass FindClass(string className);

        public abstract void Clear();

        /// <summary>Finds a JavaClass object by name.</summary>
        /// <remarks>
        ///     Finds a JavaClass object by name. If it is already in this Repository, the Repository version is returned.
        ///     Otherwise, the Repository's classpath is searched for the class (and it is added to the Repository if found).
        /// </remarks>
        /// <param name="className">the name of the class</param>
        /// <returns>the JavaClass object</returns>
        /// <exception cref="System.TypeLoadException">
        ///     if the class is not in the Repository, and could not be found on the classpath
        /// </exception>
        public virtual JavaClass LoadClass(string className)
        {
            if (className == null || className.Length == 0)
                throw new ArgumentException("Invalid class name " + className);
            className = className.Replace('/', '.');
            // Just in case, canonical form
            var clazz = FindClass(className);
            if (clazz != null) return clazz;
            try
            {
                return LoadClass(_path.GetInputStream(className), className);
            }
            catch (IOException e)
            {
                throw new TypeLoadException("Exception while looking for class " + className
                                                                                 + ": " + e, e);
            }
        }

        /// <summary>Finds the JavaClass object for a runtime Class object.</summary>
        /// <remarks>
        ///     Finds the JavaClass object for a runtime Class object. If a class with the same name is already in this
        ///     Repository, the Repository version is returned. Otherwise, getResourceAsStream() is called on the Class object to
        ///     find the class's representation. If the representation is found, it is added to the Repository.
        /// </remarks>
        /// <seealso cref="System.Type{T}" />
        /// <param name="clazz">the runtime Class object</param>
        /// <returns>JavaClass object for given runtime class</returns>
        /// <exception cref="System.TypeLoadException">
        ///     if the class is not in the Repository, and its representation could not be found
        /// </exception>
        public virtual JavaClass LoadClass(Type clazz)
        {
            var className = clazz.FullName;
            var repositoryClass = FindClass(className);
            if (repositoryClass != null) return repositoryClass;
            var name = className;
            var i = name.LastIndexOf('.');
            if (i > 0) name = Runtime.Substring(name, i + 1);
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

        public virtual ClassPath GetClassPath()
        {
            return _path;
        }

        /// <exception cref="System.TypeLoadException" />
        private JavaClass LoadClass(InputStream inputStream, string
            className)
        {
            try
            {
                if (inputStream != null)
                {
                    var parser = new ClassParser(inputStream,
                        className);
                    var clazz = parser.Parse();
                    StoreClass(clazz);
                    return clazz;
                }
            }
            catch (IOException e)
            {
                throw new TypeLoadException("Exception while looking for class " + className
                                                                                 + ": " + e, e);
            }
            finally
            {
                if (inputStream != null)
                    try
                    {
                        inputStream.Close();
                    }
                    catch (IOException)
                    {
                    }
            }

            // ignored
            throw new TypeLoadException("ClassRepository could not load " + className);
        }
    }
}