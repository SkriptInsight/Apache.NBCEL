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
using Sharpen;

namespace NBCEL.verifier.exc
{
	/// <summary>
	///     A utility class providing convenience methods concerning Throwable instances.
	/// </summary>
	/// <seealso cref="System.Exception" />
	public sealed class Utility
    {
        /// <summary>This class is not instantiable.</summary>
        private Utility()
        {
        }

        /// <summary>
        ///     This method returns the stack trace of a Throwable instance as a String.
        /// </summary>
        public static string GetStackTrace(Exception t)
        {
            var sw = new StringWriter();
            TextWriter pw = sw;
            Runtime.PrintStackTrace(t, pw);
            return sw.ToString();
        }
    }
}