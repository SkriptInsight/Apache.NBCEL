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

using System.Collections.Generic;
using Apache.NBCEL.ClassFile;

namespace Apache.NBCEL.Util
{
	/// <summary>Utility class implementing a (typesafe) set of JavaClass objects.</summary>
	/// <remarks>
	///     Utility class implementing a (typesafe) set of JavaClass objects.
	///     Since JavaClass has no equals() method, the name of the class is
	///     used for comparison.
	/// </remarks>
	/// <seealso cref="ClassStack" />
	public class ClassSet
    {
        private readonly IDictionary<string, JavaClass
        > map = new Dictionary<string, JavaClass
        >();

        public virtual bool Add(JavaClass clazz)
        {
            var result = false;
            if (!map.ContainsKey(clazz.GetClassName()))
            {
                result = true;
                Collections.Put(map, clazz.GetClassName(), clazz);
            }

            return result;
        }

        public virtual void Remove(JavaClass clazz)
        {
            Collections.Remove(map, clazz.GetClassName());
        }

        public virtual bool Empty()
        {
            return map.Count == 0;
        }

        public virtual JavaClass[] ToArray()
        {
            var values = map.Values;
            var classes = new JavaClass[values.Count]
                ;
            Collections.ToArray(values, classes);
            return classes;
        }

        public virtual string[] GetClassNames()
        {
            return Collections.ToArray(map.Keys, new string[map.Count]);
        }
    }
}