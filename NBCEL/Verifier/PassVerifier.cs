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

namespace Apache.NBCEL.Verifier
{
	/// <summary>
	///     A PassVerifier actually verifies a class file; it is instantiated
	///     by a Verifier.
	/// </summary>
	/// <remarks>
	///     A PassVerifier actually verifies a class file; it is instantiated
	///     by a Verifier.
	///     The verification should conform with a certain pass as described
	///     in The Java Virtual Machine Specification, 2nd edition.
	///     This book describes four passes. Pass one means loading the
	///     class and verifying a few static constraints. Pass two actually
	///     verifies some other constraints that could enforce loading in
	///     referenced class files. Pass three is the first pass that actually
	///     checks constraints in the code array of a method in the class file;
	///     it has two parts with the first verifying static constraints and
	///     the second part verifying structural constraints (where a data flow
	///     analysis is used for). The fourth pass, finally, performs checks
	///     that can only be done at run-time.
	///     JustIce does not have a run-time pass, but certain constraints that
	///     are usually delayed until run-time for performance reasons are also
	///     checked during the second part of pass three.
	///     PassVerifier instances perform caching.
	///     That means, if you really want a new verification run of a certain
	///     pass you must use a new instance of a given PassVerifier.
	/// </remarks>
	/// <seealso cref="Verifier" />
	/// <seealso cref="Verify()" />
	public abstract class PassVerifier
    {
        /// <summary>The (warning) messages.</summary>
        private readonly List<string> messages = new List
            <string>();

        /// <summary>The VerificationResult cache.</summary>
        private VerificationResult verificationResult;

        /// <summary>
        ///     This method runs a verification pass conforming to the
        ///     Java Virtual Machine Specification, 2nd edition, on a
        ///     class file.
        /// </summary>
        /// <remarks>
        ///     This method runs a verification pass conforming to the
        ///     Java Virtual Machine Specification, 2nd edition, on a
        ///     class file.
        ///     PassVerifier instances perform caching;
        ///     i.e. if the verify() method once determined a VerificationResult,
        ///     then this result may be returned after every invocation of this
        ///     method instead of running the verification pass anew; likewise with
        ///     the result of getMessages().
        /// </remarks>
        /// <seealso cref="GetMessages()" />
        /// <seealso cref="AddMessage(string)" />
        public virtual VerificationResult Verify()
        {
            if (verificationResult == null) verificationResult = Do_verify();
            return verificationResult;
        }

        /// <summary>Does the real verification work, uncached.</summary>
        public abstract VerificationResult Do_verify();

        /// <summary>
        ///     This method adds a (warning) message to the message pool of this
        ///     PassVerifier.
        /// </summary>
        /// <remarks>
        ///     This method adds a (warning) message to the message pool of this
        ///     PassVerifier. This method is normally only internally used by
        ///     BCEL's class file verifier "JustIce" and should not be used from
        ///     the outside.
        /// </remarks>
        /// <seealso cref="GetMessages()" />
        public virtual void AddMessage(string message)
        {
            messages.Add(message);
        }

        /// <summary>
        ///     Returns the (warning) messages that this PassVerifier accumulated
        ///     during its do_verify()ing work.
        /// </summary>
        /// <seealso cref="AddMessage(string)" />
        /// <seealso cref="Do_verify()" />
        public virtual string[] GetMessages()
        {
            Verify();
            // create messages if not already done (cached!)
            return Collections.ToArray(messages, new string[messages.Count]);
        }
    }
}