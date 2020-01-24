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
using NBCEL.classfile;
using Sharpen;

namespace NBCEL.util
{
	/// <summary>Abstract definition of a class repository.</summary>
	/// <remarks>
	///     Abstract definition of a class repository. Instances may be used to load classes from different sources and may be
	///     used in the Repository.setRepository method.
	/// </remarks>
	/// <seealso cref="NBCEL.Repository" />
	public interface Repository
    {
        /// <summary>Stores the provided class under "clazz.getClassName()"</summary>
        void StoreClass(JavaClass clazz);

        /// <summary>Removes class from repository</summary>
        void RemoveClass(JavaClass clazz);

        /// <summary>
        ///     Finds the class with the name provided, if the class isn't there, return NULL.
        /// </summary>
        JavaClass FindClass(string className);

        /// <summary>
        ///     Finds the class with the name provided, if the class isn't there, make an attempt to load it.
        /// </summary>
        /// <exception cref="System.TypeLoadException" />
        JavaClass LoadClass(string className);

        /// <summary>Finds the JavaClass instance for the given run-time class object</summary>
        /// <exception cref="System.TypeLoadException" />
        JavaClass LoadClass(Type clazz);

        /// <summary>Clears all entries from cache.</summary>
        void Clear();

        /// <summary>Gets the ClassPath associated with this Repository</summary>
        ClassPath GetClassPath();
    }
}