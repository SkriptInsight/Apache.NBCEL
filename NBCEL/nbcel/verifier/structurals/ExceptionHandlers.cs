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

namespace NBCEL.verifier.structurals
{
	/// <summary>This class allows easy access to ExceptionHandler objects.</summary>
	public class ExceptionHandlers
	{
		/// <summary>The ExceptionHandler instances.</summary>
		/// <remarks>
		/// The ExceptionHandler instances.
		/// Key: InstructionHandle objects, Values: HashSet<ExceptionHandler> instances.
		/// </remarks>
		private readonly System.Collections.Generic.IDictionary<NBCEL.generic.InstructionHandle
			, System.Collections.Generic.HashSet<NBCEL.verifier.structurals.ExceptionHandler
			>> exceptionhandlers;

		/// <summary>Constructor.</summary>
		/// <remarks>Constructor. Creates a new ExceptionHandlers instance.</remarks>
		public ExceptionHandlers(NBCEL.generic.MethodGen mg)
		{
			exceptionhandlers = new System.Collections.Generic.Dictionary<NBCEL.generic.InstructionHandle
				, System.Collections.Generic.HashSet<NBCEL.verifier.structurals.ExceptionHandler
				>>();
			NBCEL.generic.CodeExceptionGen[] cegs = mg.GetExceptionHandlers();
			foreach (NBCEL.generic.CodeExceptionGen ceg in cegs)
			{
				NBCEL.verifier.structurals.ExceptionHandler eh = new NBCEL.verifier.structurals.ExceptionHandler
					(ceg.GetCatchType(), ceg.GetHandlerPC());
				for (NBCEL.generic.InstructionHandle ih = ceg.GetStartPC(); ih != ceg.GetEndPC().
					GetNext(); ih = ih.GetNext())
				{
					System.Collections.Generic.HashSet<NBCEL.verifier.structurals.ExceptionHandler> hs;
					hs = exceptionhandlers.GetOrNull(ih);
					if (hs == null)
					{
						hs = new System.Collections.Generic.HashSet<NBCEL.verifier.structurals.ExceptionHandler
							>();
						Sharpen.Collections.Put(exceptionhandlers, ih, hs);
					}
					hs.Add(eh);
				}
			}
		}

		/// <summary>
		/// Returns all the ExceptionHandler instances representing exception
		/// handlers that protect the instruction ih.
		/// </summary>
		public virtual NBCEL.verifier.structurals.ExceptionHandler[] GetExceptionHandlers
			(NBCEL.generic.InstructionHandle ih)
		{
			System.Collections.Generic.HashSet<NBCEL.verifier.structurals.ExceptionHandler> hsSet
				 = exceptionhandlers.GetOrNull(ih);
			if (hsSet == null)
			{
				return new NBCEL.verifier.structurals.ExceptionHandler[0];
			}
			return Sharpen.Collections.ToArray(hsSet, new NBCEL.verifier.structurals.ExceptionHandler
				[hsSet.Count]);
		}
	}
}
