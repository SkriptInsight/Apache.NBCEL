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
using NBCEL.classfile;

namespace NBCEL.util
{
	/// <summary>Utility class implementing a (typesafe) stack of JavaClass objects.</summary>
	/// <seealso cref="Sharpen.System.Collections.Stack{E}" />
	public class ClassStack
    {
        private readonly Stack<JavaClass> stack = new Stack
            <JavaClass>();

        public virtual void Push(JavaClass clazz)
        {
            stack.Push(clazz);
        }

        public virtual JavaClass Pop()
        {
            return stack.Pop();
        }

        public virtual JavaClass Top()
        {
            return stack.Peek();
        }

        public virtual bool Empty()
        {
            return stack.Count == 0;
        }
    }
}