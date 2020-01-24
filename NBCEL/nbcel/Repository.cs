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
using NBCEL.classfile;
using NBCEL.util;
using Sharpen;

namespace NBCEL
{
	/// <summary>
	///     The repository maintains informations about class interdependencies, e.g.,
	///     whether a class is a sub-class of another.
	/// </summary>
	/// <remarks>
	///     The repository maintains informations about class interdependencies, e.g.,
	///     whether a class is a sub-class of another. Delegates actual class loading
	///     to SyntheticRepository with current class path by default.
	/// </remarks>
	/// <seealso cref="NBCEL.util.Repository" />
	/// <seealso cref="NBCEL.util.SyntheticRepository" />
	public abstract class Repository
    {
        private static util.Repository repository = SyntheticRepository.GetInstance();

        /// <returns>currently used repository instance</returns>
        public static util.Repository GetRepository()
        {
            return repository;
        }

        /// <summary>Sets repository instance to be used for class loading</summary>
        public static void SetRepository(util.Repository rep)
        {
            repository = rep;
        }

        /// <summary>
        ///     Lookups class somewhere found on your CLASSPATH, or whereever the
        ///     repository instance looks for it.
        /// </summary>
        /// <returns>class object for given fully qualified class name</returns>
        /// <exception cref="System.TypeLoadException">
        ///     if the class could not be found or
        ///     parsed correctly
        /// </exception>
        public static JavaClass LookupClass(string class_name)
        {
            return repository.LoadClass(class_name);
        }

        /// <summary>Tries to find class source using the internal repository instance.</summary>
        /// <seealso cref="System.Type{T}" />
        /// <returns>JavaClass object for given runtime class</returns>
        /// <exception cref="System.TypeLoadException">
        ///     if the class could not be found or
        ///     parsed correctly
        /// </exception>
        public static JavaClass LookupClass(Type clazz)
        {
            return repository.LoadClass(clazz);
        }

        /// <returns>
        ///     class file object for given Java class by looking on the
        ///     system class path; returns null if the class file can't be
        ///     found
        /// </returns>
        public static ClassPath.ClassFile LookupClassFile(string class_name)
        {
            try
            {
                var path = repository.GetClassPath();
                if (path == null)
                {
                    return null;
                }

                return path.GetClassFile(class_name);
            }
            catch (IOException)
            {
                return null;
            }
        }

        /// <summary>Clears the repository.</summary>
        public static void ClearCache()
        {
            repository.Clear();
        }

        /// <summary>
        ///     Adds clazz to repository if there isn't an equally named class already in there.
        /// </summary>
        /// <returns>old entry in repository</returns>
        public static JavaClass AddClass(JavaClass clazz)
        {
            var old = repository.FindClass(clazz.GetClassName());
            repository.StoreClass(clazz);
            return old;
        }

        /// <summary>Removes class with given (fully qualified) name from repository.</summary>
        public static void RemoveClass(string clazz)
        {
            repository.RemoveClass(repository.FindClass(clazz));
        }

        /// <summary>Removes given class from repository.</summary>
        public static void RemoveClass(JavaClass clazz)
        {
            repository.RemoveClass(clazz);
        }

        /// <returns>
        ///     list of super classes of clazz in ascending order, i.e.,
        ///     Object is always the last element
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        ///     if any of the superclasses can't be found
        /// </exception>
        public static JavaClass[] GetSuperClasses(JavaClass
            clazz)
        {
            return clazz.GetSuperClasses();
        }

        /// <returns>
        ///     list of super classes of clazz in ascending order, i.e.,
        ///     Object is always the last element.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        ///     if the named class or any of its
        ///     superclasses can't be found
        /// </exception>
        public static JavaClass[] GetSuperClasses(string class_name)
        {
            var jc = LookupClass(class_name);
            return GetSuperClasses(jc);
        }

        /// <returns>
        ///     all interfaces implemented by class and its super
        ///     classes and the interfaces that those interfaces extend, and so on.
        ///     (Some people call this a transitive hull).
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        ///     if any of the class's
        ///     superclasses or superinterfaces can't be found
        /// </exception>
        public static JavaClass[] GetInterfaces(JavaClass
            clazz)
        {
            return clazz.GetAllInterfaces();
        }

        /// <returns>
        ///     all interfaces implemented by class and its super
        ///     classes and the interfaces that extend those interfaces, and so on
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        ///     if the named class can't be found,
        ///     or if any of its superclasses or superinterfaces can't be found
        /// </exception>
        public static JavaClass[] GetInterfaces(string class_name)
        {
            return GetInterfaces(LookupClass(class_name));
        }

        /// <summary>Equivalent to runtime "instanceof" operator.</summary>
        /// <returns>true, if clazz is an instance of super_class</returns>
        /// <exception cref="System.TypeLoadException">
        ///     if any superclasses or superinterfaces
        ///     of clazz can't be found
        /// </exception>
        public static bool InstanceOf(JavaClass clazz, JavaClass
            super_class)
        {
            return clazz.InstanceOf(super_class);
        }

        /// <returns>true, if clazz is an instance of super_class</returns>
        /// <exception cref="System.TypeLoadException">
        ///     if either clazz or super_class
        ///     can't be found
        /// </exception>
        public static bool InstanceOf(string clazz, string super_class)
        {
            return InstanceOf(LookupClass(clazz), LookupClass(super_class));
        }

        /// <returns>true, if clazz is an instance of super_class</returns>
        /// <exception cref="System.TypeLoadException">if super_class can't be found</exception>
        public static bool InstanceOf(JavaClass clazz, string super_class
        )
        {
            return InstanceOf(clazz, LookupClass(super_class));
        }

        /// <returns>true, if clazz is an instance of super_class</returns>
        /// <exception cref="System.TypeLoadException">if clazz can't be found</exception>
        public static bool InstanceOf(string clazz, JavaClass super_class
        )
        {
            return InstanceOf(LookupClass(clazz), super_class);
        }

        /// <returns>true, if clazz is an implementation of interface inter</returns>
        /// <exception cref="System.TypeLoadException">
        ///     if any superclasses or superinterfaces
        ///     of clazz can't be found
        /// </exception>
        public static bool ImplementationOf(JavaClass clazz, JavaClass
            inter)
        {
            return clazz.ImplementationOf(inter);
        }

        /// <returns>true, if clazz is an implementation of interface inter</returns>
        /// <exception cref="System.TypeLoadException">
        ///     if clazz, inter, or any superclasses
        ///     or superinterfaces of clazz can't be found
        /// </exception>
        public static bool ImplementationOf(string clazz, string inter)
        {
            return ImplementationOf(LookupClass(clazz), LookupClass(inter));
        }

        /// <returns>true, if clazz is an implementation of interface inter</returns>
        /// <exception cref="System.TypeLoadException">
        ///     if inter or any superclasses
        ///     or superinterfaces of clazz can't be found
        /// </exception>
        public static bool ImplementationOf(JavaClass clazz, string inter
        )
        {
            return ImplementationOf(clazz, LookupClass(inter));
        }

        /// <returns>true, if clazz is an implementation of interface inter</returns>
        /// <exception cref="System.TypeLoadException">
        ///     if clazz or any superclasses or
        ///     superinterfaces of clazz can't be found
        /// </exception>
        public static bool ImplementationOf(string clazz, JavaClass inter
        )
        {
            return ImplementationOf(LookupClass(clazz), inter);
        }
    }
}