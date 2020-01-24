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
using Sharpen;

namespace NBCEL.verifier
{
	/// <summary>
	/// This class has a main method implementing a demonstration program
	/// of how to use the VerifierFactoryObserver.
	/// </summary>
	/// <remarks>
	/// This class has a main method implementing a demonstration program
	/// of how to use the VerifierFactoryObserver. It transitively verifies
	/// all class files encountered; this may take up a lot of time and,
	/// more notably, memory.
	/// </remarks>
	public class TransitiveHull : NBCEL.verifier.VerifierFactoryObserver
	{
		/// <summary>Used for indentation.</summary>
		private int indent = 0;

		/// <summary>Not publicly instantiable.</summary>
		private TransitiveHull()
		{
		}

		/* Implementing VerifierFactoryObserver. */
		public virtual void Update(string classname)
		{
			GC.Collect();
			// avoid swapping if possible.
			for (int i = 0; i < indent; i++)
			{
				System.Console.Out.Write(" ");
			}
			System.Console.Out.WriteLine(classname);
			indent += 1;
			NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(classname);
			NBCEL.verifier.VerificationResult vr;
			vr = v.DoPass1();
			if (vr != NBCEL.verifier.VerificationResult.VR_OK)
			{
				System.Console.Out.WriteLine("Pass 1:\n" + vr);
			}
			vr = v.DoPass2();
			if (vr != NBCEL.verifier.VerificationResult.VR_OK)
			{
				System.Console.Out.WriteLine("Pass 2:\n" + vr);
			}
			if (vr == NBCEL.verifier.VerificationResult.VR_OK)
			{
				try
				{
					NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(v.GetClassName());
					for (int i = 0; i < jc.GetMethods().Length; i++)
					{
						vr = v.DoPass3a(i);
						if (vr != NBCEL.verifier.VerificationResult.VR_OK)
						{
							System.Console.Out.WriteLine(v.GetClassName() + ", Pass 3a, method " + i + " ['" 
								+ jc.GetMethods()[i] + "']:\n" + vr);
						}
						vr = v.DoPass3b(i);
						if (vr != NBCEL.verifier.VerificationResult.VR_OK)
						{
							System.Console.Out.WriteLine(v.GetClassName() + ", Pass 3b, method " + i + " ['" 
								+ jc.GetMethods()[i] + "']:\n" + vr);
						}
					}
				}
				catch (System.TypeLoadException)
				{
					System.Console.Error.WriteLine("Could not find class " + v.GetClassName() + " in Repository"
						);
				}
			}
			indent -= 1;
		}

		/// <summary>
		/// This method implements a demonstration program
		/// of how to use the VerifierFactoryObserver.
		/// </summary>
		/// <remarks>
		/// This method implements a demonstration program
		/// of how to use the VerifierFactoryObserver. It transitively verifies
		/// all class files encountered; this may take up a lot of time and,
		/// more notably, memory.
		/// </remarks>
		public static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				System.Console.Out.WriteLine("Need exactly one argument: The root class to verify."
					);
				System.Environment.Exit(1);
			}
			int dotclasspos = args[0].LastIndexOf(".class");
			if (dotclasspos != -1)
			{
				args[0] = Sharpen.Runtime.Substring(args[0], 0, dotclasspos);
			}
			args[0] = args[0].Replace('/', '.');
			NBCEL.verifier.TransitiveHull th = new NBCEL.verifier.TransitiveHull();
			NBCEL.verifier.VerifierFactory.Attach(th);
			NBCEL.verifier.VerifierFactory.GetVerifier(args[0]);
			// the observer is called back and does the actual trick.
			NBCEL.verifier.VerifierFactory.Detach(th);
		}
	}
}
