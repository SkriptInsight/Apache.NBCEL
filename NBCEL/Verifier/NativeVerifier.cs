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

namespace Apache.NBCEL.Verifier
{
	/// <summary>
	///     The NativeVerifier class implements a main(String[] args) method that's
	///     roughly compatible to the one in the Verifier class, but that uses the
	///     JVM's internal verifier for its class file verification.
	/// </summary>
	/// <remarks>
	///     The NativeVerifier class implements a main(String[] args) method that's
	///     roughly compatible to the one in the Verifier class, but that uses the
	///     JVM's internal verifier for its class file verification.
	///     This can be used for comparison runs between the JVM-internal verifier
	///     and JustIce.
	/// </remarks>
	public abstract class NativeVerifier
    {
        /// <summary>This class must not be instantiated.</summary>
        private NativeVerifier()
        {
        }

        /// <summary>Works only on the first argument.</summary>
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Out.WriteLine("Verifier front-end: need exactly one argument.");
                Environment.Exit(1);
            }

            var dotclasspos = args[0].LastIndexOf(".class");
            if (dotclasspos != -1) args[0] = Runtime.Substring(args[0], 0, dotclasspos);
            args[0] = args[0].Replace('/', '.');
            //System.out.println(args[0]);
            try
            {
                Runtime.GetType(args[0]);
            }
            catch
            {
                // OK to catch Throwable here as we call exit.
                Console.Out.WriteLine("NativeVerifier: Unspecified verification error on '"
                                      + args[0] + "'.");
                Environment.Exit(1);
            }

            Console.Out.WriteLine("NativeVerifier: Class file '" + args[0] + "' seems to be okay."
            );
            Environment.Exit(0);
        }
    }
}