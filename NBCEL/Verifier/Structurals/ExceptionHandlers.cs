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
using Apache.NBCEL.Generic;

namespace Apache.NBCEL.Verifier.Structurals
{
    /// <summary>This class allows easy access to ExceptionHandler objects.</summary>
    public class ExceptionHandlers
    {
	    /// <summary>The ExceptionHandler instances.</summary>
	    /// <remarks>
	    ///     The ExceptionHandler instances.
	    ///     Key: InstructionHandle objects, Values: HashSet<ExceptionHandler> instances.
	    /// </remarks>
	    private readonly IDictionary<InstructionHandle
            , HashSet<ExceptionHandler
            >> exceptionhandlers;

	    /// <summary>Constructor.</summary>
	    /// <remarks>Constructor. Creates a new ExceptionHandlers instance.</remarks>
	    public ExceptionHandlers(MethodGen mg)
        {
            exceptionhandlers = new Dictionary<InstructionHandle
                , HashSet<ExceptionHandler
                >>();
            var cegs = mg.GetExceptionHandlers();
            foreach (var ceg in cegs)
            {
                var eh = new ExceptionHandler
                    (ceg.GetCatchType(), ceg.GetHandlerPC());
                for (var ih = ceg.GetStartPC(); ih != ceg.GetEndPC().GetNext(); ih = ih.GetNext())
                {
                    HashSet<ExceptionHandler> hs;
                    hs = exceptionhandlers.GetOrNull(ih);
                    if (hs == null)
                    {
                        hs = new HashSet<ExceptionHandler
                        >();
                        Collections.Put(exceptionhandlers, ih, hs);
                    }

                    hs.Add(eh);
                }
            }
        }

	    /// <summary>
	    ///     Returns all the ExceptionHandler instances representing exception
	    ///     handlers that protect the instruction ih.
	    /// </summary>
	    public virtual ExceptionHandler[] GetExceptionHandlers
            (InstructionHandle ih)
        {
            var hsSet
                = exceptionhandlers.GetOrNull(ih);
            if (hsSet == null) return new ExceptionHandler[0];
            return Collections.ToArray(hsSet, new ExceptionHandler
                [hsSet.Count]);
        }
    }
}