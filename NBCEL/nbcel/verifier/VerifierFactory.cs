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
using Sharpen;

namespace NBCEL.verifier
{
	/// <summary>This class produces instances of the Verifier class.</summary>
	/// <remarks>
	/// This class produces instances of the Verifier class. Its purpose is to make
	/// sure that they are singleton instances with respect to the class name they
	/// operate on. That means, for every class (represented by a unique fully qualified
	/// class name) there is exactly one Verifier.
	/// </remarks>
	/// <seealso cref="Verifier"/>
	public class VerifierFactory
	{
		/// <summary>The HashMap that holds the data about the already-constructed Verifier instances.
		/// 	</summary>
		private static readonly System.Collections.Generic.IDictionary<string, NBCEL.verifier.Verifier
			> hashMap = new System.Collections.Generic.Dictionary<string, NBCEL.verifier.Verifier
			>();

		/// <summary>The VerifierFactoryObserver instances that observe the VerifierFactory.</summary>
		private static readonly System.Collections.Generic.List<NBCEL.verifier.VerifierFactoryObserver
			> observers = new System.Collections.Generic.List<NBCEL.verifier.VerifierFactoryObserver
		>();

		/// <summary>The VerifierFactory is not instantiable.</summary>
		private VerifierFactory()
		{
		}

		/// <summary>Returns the (only) verifier responsible for the class with the given name.
		/// 	</summary>
		/// <remarks>
		/// Returns the (only) verifier responsible for the class with the given name.
		/// Possibly a new Verifier object is transparently created.
		/// </remarks>
		/// <returns>the (only) verifier responsible for the class with the given name.</returns>
		public static NBCEL.verifier.Verifier GetVerifier(string fullyQualifiedClassName)
		{
			NBCEL.verifier.Verifier v = hashMap.GetOrNull(fullyQualifiedClassName);
			if (v == null)
			{
				v = new NBCEL.verifier.Verifier(fullyQualifiedClassName);
				Sharpen.Collections.Put(hashMap, fullyQualifiedClassName, v);
				Notify(fullyQualifiedClassName);
			}
			return v;
		}

		/// <summary>Notifies the observers of a newly generated Verifier.</summary>
		private static void Notify(string fullyQualifiedClassName)
		{
			// notify the observers
			foreach (NBCEL.verifier.VerifierFactoryObserver vfo in observers)
			{
				vfo.Update(fullyQualifiedClassName);
			}
		}

		/// <summary>Returns all Verifier instances created so far.</summary>
		/// <remarks>
		/// Returns all Verifier instances created so far.
		/// This is useful when a Verifier recursively lets
		/// the VerifierFactory create other Verifier instances
		/// and if you want to verify the transitive hull of
		/// referenced class files.
		/// </remarks>
		public static NBCEL.verifier.Verifier[] GetVerifiers()
		{
			NBCEL.verifier.Verifier[] vs = new NBCEL.verifier.Verifier[hashMap.Values.Count];
			return Sharpen.Collections.ToArray(hashMap.Values, vs);
		}

		// Because vs is big enough, vs is used to store the values into and returned!
		/// <summary>Adds the VerifierFactoryObserver o to the list of observers.</summary>
		public static void Attach(NBCEL.verifier.VerifierFactoryObserver o)
		{
			observers.Add(o);
		}

		/// <summary>Removes the VerifierFactoryObserver o from the list of observers.</summary>
		public static void Detach(NBCEL.verifier.VerifierFactoryObserver o)
		{
			observers.Remove(o);
		}
	}
}
