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

namespace NBCEL.generic
{
	/// <summary>Thrown on internal errors.</summary>
	/// <remarks>
	///     Thrown on internal errors. Extends RuntimeException so it hasn't to be declared
	///     in the throws clause every time.
	/// </remarks>
	[Serializable]
    public class ClassGenException : Exception
    {
        private const long serialVersionUID = 7247369755051242791L;

        public ClassGenException()
        {
        }

        public ClassGenException(string s)
            : base(s)
        {
        }

        public ClassGenException(string s, Exception initCause)
            : base(s, initCause)
        {
        }
    }
}