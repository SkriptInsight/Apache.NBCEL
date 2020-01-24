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
	/// <summary>
	/// This class represents an exception handler; that is, an ObjectType
	/// representing a subclass of java.lang.Throwable and the instruction
	/// the handler starts off (represented by an InstructionContext).
	/// </summary>
	public class ExceptionHandler
	{
		/// <summary>The type of the exception to catch.</summary>
		/// <remarks>The type of the exception to catch. NULL means ANY.</remarks>
		private readonly NBCEL.generic.ObjectType catchtype;

		/// <summary>The InstructionHandle where the handling begins.</summary>
		private readonly NBCEL.generic.InstructionHandle handlerpc;

		/// <summary>Leave instance creation to JustIce.</summary>
		internal ExceptionHandler(NBCEL.generic.ObjectType catch_type, NBCEL.generic.InstructionHandle
			 handler_pc)
		{
			catchtype = catch_type;
			handlerpc = handler_pc;
		}

		/// <summary>Returns the type of the exception that's handled.</summary>
		/// <remarks>Returns the type of the exception that's handled. <B>'null' means 'ANY'.</B>
		/// 	</remarks>
		public virtual NBCEL.generic.ObjectType GetExceptionType()
		{
			return catchtype;
		}

		/// <summary>Returns the InstructionHandle where the handler starts off.</summary>
		public virtual NBCEL.generic.InstructionHandle GetHandlerStart()
		{
			return handlerpc;
		}
	}
}
