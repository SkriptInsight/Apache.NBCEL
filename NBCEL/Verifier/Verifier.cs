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
using Apache.NBCEL.Verifier.Statics;
using Apache.NBCEL.Verifier.Structurals;

namespace Apache.NBCEL.Verifier
{
	/// <summary>
	///     A Verifier instance is there to verify a class file according to The Java Virtual
	///     Machine Specification, 2nd Edition.
	/// </summary>
	/// <remarks>
	///     A Verifier instance is there to verify a class file according to The Java Virtual
	///     Machine Specification, 2nd Edition.
	///     Pass-3b-verification includes pass-3a-verification;
	///     pass-3a-verification includes pass-2-verification;
	///     pass-2-verification includes pass-1-verification.
	///     A Verifier creates PassVerifier instances to perform the actual verification.
	///     Verifier instances are usually generated by the VerifierFactory.
	/// </remarks>
	/// <seealso cref="VerifierFactory" />
	/// <seealso cref="PassVerifier" />
	public class Verifier
    {
        /// <summary>The name of the class this verifier operates on.</summary>
        private readonly string classname;

        /// <summary>The Pass3aVerifiers for this Verifier instance.</summary>
        /// <remarks>
        ///     The Pass3aVerifiers for this Verifier instance. Key: Interned string specifying the method number.
        /// </remarks>
        private readonly IDictionary<string, Pass3aVerifier
        > p3avs = new Dictionary<string, Pass3aVerifier
        >();

        /// <summary>The Pass3bVerifiers for this Verifier instance.</summary>
        /// <remarks>
        ///     The Pass3bVerifiers for this Verifier instance. Key: Interned string specifying the method number.
        /// </remarks>
        private readonly IDictionary<string, Pass3bVerifier
        > p3bvs = new Dictionary<string, Pass3bVerifier
        >();

        /// <summary>A Pass1Verifier for this Verifier instance.</summary>
        private Pass1Verifier p1v;

        /// <summary>A Pass2Verifier for this Verifier instance.</summary>
        private Pass2Verifier p2v;

        /// <summary>Instantiation is done by the VerifierFactory.</summary>
        /// <seealso cref="VerifierFactory" />
        internal Verifier(string fully_qualified_classname)
        {
            classname = fully_qualified_classname;
            Flush();
        }

        /// <summary>Returns the VerificationResult for the given pass.</summary>
        public virtual VerificationResult DoPass1()
        {
            if (p1v == null) p1v = new Pass1Verifier(this);
            return p1v.Verify();
        }

        /// <summary>Returns the VerificationResult for the given pass.</summary>
        public virtual VerificationResult DoPass2()
        {
            if (p2v == null) p2v = new Pass2Verifier(this);
            return p2v.Verify();
        }

        /// <summary>Returns the VerificationResult for the given pass.</summary>
        public virtual VerificationResult DoPass3a(int method_no)
        {
            var key = method_no.ToString();
            Pass3aVerifier p3av;
            p3av = p3avs.GetOrNull(key);
            if (p3avs.GetOrNull(key) == null)
            {
                p3av = new Pass3aVerifier(this, method_no);
                Collections.Put(p3avs, key, p3av);
            }

            return p3av.Verify();
        }

        /// <summary>Returns the VerificationResult for the given pass.</summary>
        public virtual VerificationResult DoPass3b(int method_no)
        {
            var key = method_no.ToString();
            Pass3bVerifier p3bv;
            p3bv = p3bvs.GetOrNull(key);
            if (p3bvs.GetOrNull(key) == null)
            {
                p3bv = new Pass3bVerifier(this, method_no);
                Collections.Put(p3bvs, key, p3bv);
            }

            return p3bv.Verify();
        }

        /// <summary>Returns the name of the class this verifier operates on.</summary>
        /// <remarks>
        ///     Returns the name of the class this verifier operates on.
        ///     This is particularly interesting when this verifier was created
        ///     recursively by another Verifier and you got a reference to this
        ///     Verifier by the getVerifiers() method of the VerifierFactory.
        /// </remarks>
        /// <seealso cref="VerifierFactory" />
        public string GetClassName()
        {
            return classname;
        }

        /// <summary>
        ///     Forget everything known about the class file; that means, really
        ///     start a new verification of a possibly different class file from
        ///     BCEL's repository.
        /// </summary>
        public virtual void Flush()
        {
            p1v = null;
            p2v = null;
            p3avs.Clear();
            p3bvs.Clear();
        }

        /// <summary>This returns all the (warning) messages collected during verification.</summary>
        /// <remarks>
        ///     This returns all the (warning) messages collected during verification.
        ///     A prefix shows from which verifying pass a message originates.
        /// </remarks>
        /// <exception cref="System.TypeLoadException" />
        public virtual string[] GetMessages()
        {
            var messages = new List
                <string>();
            if (p1v != null)
            {
                var p1m = p1v.GetMessages();
                foreach (var element in p1m) messages.Add("Pass 1: " + element);
            }

            if (p2v != null)
            {
                var p2m = p2v.GetMessages();
                foreach (var element in p2m) messages.Add("Pass 2: " + element);
            }

            foreach (var pv in p3avs.Values)
            {
                var p3am = pv.GetMessages();
                var meth = pv.GetMethodNo();
                foreach (var element in p3am)
                    messages.Add("Pass 3a, method " + meth + " ('" + Repository.LookupClass(classname
                                 ).GetMethods()[meth] + "'): " + element);
            }

            foreach (var pv in p3bvs.Values)
            {
                var p3bm = pv.GetMessages();
                var meth = pv.GetMethodNo();
                foreach (var element in p3bm)
                    messages.Add("Pass 3b, method " + meth + " ('" + Repository.LookupClass(classname
                                 ).GetMethods()[meth] + "'): " + element);
            }

            return Collections.ToArray(messages, new string[messages.Count]);
        }

        /// <summary>Verifies class files.</summary>
        /// <remarks>
        ///     Verifies class files.
        ///     This is a simple demonstration of how the API of BCEL's
        ///     class file verifier "JustIce" may be used.
        ///     You should supply command-line arguments which are
        ///     fully qualified namea of the classes to verify. These class files
        ///     must be somewhere in your CLASSPATH (refer to Sun's
        ///     documentation for questions about this) or you must have put the classes
        ///     into the BCEL Repository yourself (via 'addClass(JavaClass)').
        /// </remarks>
        public static void Main(string[] args)
        {
            Console.Out.WriteLine(
                "JustIce by Enver Haase, (C) 2001-2002.\n<http://bcel.sourceforge.net>\n<https://commons.apache.org/bcel>\n"
            );
            for (var index = 0; index < args.Length; index++)
                try
                {
                    if (args[index].EndsWith(".class"))
                    {
                        var dotclasspos = args[index].LastIndexOf(".class");
                        if (dotclasspos != -1) args[index] = Runtime.Substring(args[index], 0, dotclasspos);
                    }

                    args[index] = args[index].Replace('/', '.');
                    Console.Out.WriteLine("Now verifying: " + args[index] + "\n");
                    VerifyType(args[index]);
                    Repository.ClearCache();
                    GC.Collect();
                }
                catch (TypeLoadException e)
                {
                    Runtime.PrintStackTrace(e);
                }
        }

        /// <exception cref="System.TypeLoadException" />
        internal static void VerifyType(string fullyQualifiedClassName)
        {
            var verifier = VerifierFactory.GetVerifier(fullyQualifiedClassName
            );
            VerificationResult verificationResult;
            verificationResult = verifier.DoPass1();
            Console.Out.WriteLine("Pass 1:\n" + verificationResult);
            verificationResult = verifier.DoPass2();
            Console.Out.WriteLine("Pass 2:\n" + verificationResult);
            if (verificationResult == VerificationResult.VR_OK)
            {
                var jc = Repository.LookupClass(fullyQualifiedClassName
                );
                for (var i = 0; i < jc.GetMethods().Length; i++)
                {
                    verificationResult = verifier.DoPass3a(i);
                    Console.Out.WriteLine("Pass 3a, method number " + i + " ['" + jc.GetMethods
                                              ()[i] + "']:\n" + verificationResult);
                    verificationResult = verifier.DoPass3b(i);
                    Console.Out.WriteLine("Pass 3b, method number " + i + " ['" + jc.GetMethods
                                              ()[i] + "']:\n" + verificationResult);
                }
            }

            Console.Out.WriteLine("Warnings:");
            var warnings = verifier.GetMessages();
            if (warnings.Length == 0) Console.Out.WriteLine("<none>");
            foreach (var warning in warnings) Console.Out.WriteLine(warning);
            Console.Out.WriteLine("\n");
            // avoid swapping.
            verifier.Flush();
        }
    }
}