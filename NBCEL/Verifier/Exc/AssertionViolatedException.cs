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

namespace Apache.NBCEL.Verifier.Exc
{
	/// <summary>Instances of this class should never be thrown.</summary>
	/// <remarks>
	///     Instances of this class should never be thrown. When such an instance is thrown,
	///     this is due to an INTERNAL ERROR of BCEL's class file verifier &quot;JustIce&quot;.
	/// </remarks>
	[Serializable]
    public sealed class AssertionViolatedException : Exception
    {
        private const long serialVersionUID = -129822266349567409L;

        /// <summary>The error message.</summary>
        private string detailMessage;

        /// <summary>
        ///     Constructs a new AssertionViolatedException with null as its error message string.
        /// </summary>
        public AssertionViolatedException()
        {
        }

        /// <summary>
        ///     Constructs a new AssertionViolatedException with the specified error message preceded
        ///     by &quot;INTERNAL ERROR: &quot;.
        /// </summary>
        public AssertionViolatedException(string message)
            : base(message = "INTERNAL ERROR: " + message)
        {
            // Thanks to Java, the constructor call here must be first.
            detailMessage = message;
        }

        /// <summary>
        ///     Constructs a new AssertionViolationException with the specified error message and initial cause
        /// </summary>
        /// <since>6.0</since>
        public AssertionViolatedException(string message, Exception initCause)
            : base(message = "INTERNAL ERROR: " + message, initCause)
        {
            detailMessage = message;
        }

        /// <summary>
        ///     Returns the error message string of this AssertionViolatedException object.
        /// </summary>
        /// <returns>the error message string of this AssertionViolatedException.</returns>
        public override string Message => detailMessage;

        /// <summary>
        ///     Extends the error message with a string before ("pre") and after ("post") the
        ///     'old' error message.
        /// </summary>
        /// <remarks>
        ///     Extends the error message with a string before ("pre") and after ("post") the
        ///     'old' error message. All of these three strings are allowed to be null, and null
        ///     is always replaced by the empty string (""). In particular, after invoking this
        ///     method, the error message of this object can no longer be null.
        /// </remarks>
        public void ExtendMessage(string pre, string post)
        {
            if (pre == null) pre = string.Empty;
            if (detailMessage == null) detailMessage = string.Empty;
            if (post == null) post = string.Empty;
            detailMessage = pre + detailMessage + post;
        }

        /// <summary>DO NOT USE.</summary>
        /// <remarks>DO NOT USE. It's for experimental testing during development only.</remarks>
        public static void Main(string[] args)
        {
            var ave = new AssertionViolatedException
                ("Oops!");
            ave.ExtendMessage("\nFOUND:\n\t", "\nExiting!!\n");
            throw ave;
        }
    }
}