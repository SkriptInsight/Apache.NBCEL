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
	/// <summary>
	///     Instances of this class are thrown by BCEL's class file verifier "JustIce"
	///     when a class file to verify does not pass the verification pass 2 as described
	///     in the Java Virtual Machine specification, 2nd edition.
	/// </summary>
	[Serializable]
    public class ClassConstraintException : VerificationException
    {
        private const long serialVersionUID = -4745598983569128296L;

        /// <summary>
        ///     Constructs a new ClassConstraintException with null as its error message string.
        /// </summary>
        public ClassConstraintException()
        {
        }

        /// <summary>
        ///     Constructs a new ClassConstraintException with the specified error message.
        /// </summary>
        public ClassConstraintException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Constructs a new ClassConstraintException with the specified error message and cause
        /// </summary>
        /// <since>6.0</since>
        public ClassConstraintException(string message, Exception initCause)
            : base(message, initCause)
        {
        }
    }
}