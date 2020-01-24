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

namespace NBCEL.verifier.exc
{
	/// <summary>
	/// Instances of this class are thrown by BCEL's class file verifier "JustIce"
	/// whenever
	/// verification proves that some constraint of a class file (as stated in the
	/// Java Virtual Machine Specification, Edition 2) is violated.
	/// </summary>
	/// <remarks>
	/// Instances of this class are thrown by BCEL's class file verifier "JustIce"
	/// whenever
	/// verification proves that some constraint of a class file (as stated in the
	/// Java Virtual Machine Specification, Edition 2) is violated.
	/// This is roughly equivalent to the VerifyError the JVM-internal verifiers
	/// throw.
	/// </remarks>
	[System.Serializable]
	public abstract class VerifierConstraintViolatedException : System.Exception
	{
		private const long serialVersionUID = 2946136970490179465L;

		/// <summary>The specified error message.</summary>
		private string detailMessage;

		/// <summary>Constructs a new VerifierConstraintViolatedException with null as its error message string.
		/// 	</summary>
		internal VerifierConstraintViolatedException()
			: base()
		{
		}

		/// <summary>Constructs a new VerifierConstraintViolatedException with the specified error message.
		/// 	</summary>
		internal VerifierConstraintViolatedException(string message)
			: base(message)
		{
			// /** The name of the offending class that did not pass the verifier. */
			// String name_of_offending_class;
			// Not that important
			detailMessage = message;
		}

		/// <summary>Constructs a new VerifierConstraintViolationException with the specified error message and cause
		/// 	</summary>
		internal VerifierConstraintViolatedException(string message, System.Exception initCause
			)
			: base(message, initCause)
		{
			detailMessage = message;
		}

		/// <summary>
		/// Extends the error message with a string before ("pre") and after ("post") the
		/// 'old' error message.
		/// </summary>
		/// <remarks>
		/// Extends the error message with a string before ("pre") and after ("post") the
		/// 'old' error message. All of these three strings are allowed to be null, and null
		/// is always replaced by the empty string (""). In particular, after invoking this
		/// method, the error message of this object can no longer be null.
		/// </remarks>
		public virtual void ExtendMessage(string pre, string post)
		{
			if (pre == null)
			{
				pre = string.Empty;
			}
			if (detailMessage == null)
			{
				detailMessage = string.Empty;
			}
			if (post == null)
			{
				post = string.Empty;
			}
			detailMessage = pre + detailMessage + post;
		}

		/// <summary>Returns the error message string of this VerifierConstraintViolatedException object.
		/// 	</summary>
		/// <returns>the error message string of this VerifierConstraintViolatedException.</returns>
		public override string Message
		{
			get
			{
				return detailMessage;
			}
		}
	}
}
