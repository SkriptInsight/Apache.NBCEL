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
using System.Collections.Generic;
using Apache.NBCEL.ClassFile;

namespace Apache.NBCEL.Util
{
	/// <summary>
	///     Utility class implementing a (typesafe) collection of JavaClass
	///     objects.
	/// </summary>
	/// <remarks>
	///     Utility class implementing a (typesafe) collection of JavaClass
	///     objects. Contains the most important methods of a Vector.
	/// </remarks>
	[Serializable]
    [Obsolete(@"as of 5.1.1 - 7/17/2005")]
    public class ClassVector
    {
        private const long serialVersionUID = 5600397075672780806L;

        [Obsolete] protected internal List<JavaClass> vec
            = new List<JavaClass>();

        public virtual void AddElement(JavaClass clazz)
        {
            vec.Add(clazz);
        }

        public virtual JavaClass ElementAt(int index)
        {
            return vec[index];
        }

        public virtual void RemoveElementAt(int index)
        {
            vec.RemoveAtReturningValue(index);
        }

        public virtual JavaClass[] ToArray()
        {
            var classes = new JavaClass[vec.Count];
            Collections.ToArray(vec, classes);
            return classes;
        }
    }
}