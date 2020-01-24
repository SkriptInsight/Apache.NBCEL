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
	///     Utility class implementing a (typesafe) queue of JavaClass
	///     objects.
	/// </summary>
	public class ClassQueue
    {
        [Obsolete(@"(since 6.0) will be made private; do not access")]
        protected internal LinkedList<JavaClass
        > vec = new LinkedList<JavaClass>();

        // TODO not used externally
        public virtual void Enqueue(JavaClass clazz)
        {
            vec.AddLast(clazz);
        }

        public virtual JavaClass Dequeue()
        {
            return Collections.RemoveFirst(vec);
        }

        public virtual bool Empty()
        {
            return vec.Count == 0;
        }

        public override string ToString()
        {
            return vec.ToString();
        }
    }
}