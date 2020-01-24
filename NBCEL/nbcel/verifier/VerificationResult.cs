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
	/// <summary>
	/// A VerificationResult is what a PassVerifier returns
	/// after verifying.
	/// </summary>
	public class VerificationResult
	{
		/// <summary>Constant to indicate verification has not been tried yet.</summary>
		/// <remarks>
		/// Constant to indicate verification has not been tried yet.
		/// This happens if some earlier verification pass did not return VERIFIED_OK.
		/// </remarks>
		public const int VERIFIED_NOTYET = 0;

		/// <summary>Constant to indicate verification was passed.</summary>
		public const int VERIFIED_OK = 1;

		/// <summary>Constant to indicate verfication failed.</summary>
		public const int VERIFIED_REJECTED = 2;

		/// <summary>This string is the canonical message for verifications that have not been tried yet.
		/// 	</summary>
		/// <remarks>
		/// This string is the canonical message for verifications that have not been tried yet.
		/// This happens if some earlier verification pass did not return
		/// <see cref="VERIFIED_OK"/>
		/// .
		/// </remarks>
		private const string VERIFIED_NOTYET_MSG = "Not yet verified.";

		/// <summary>This string is the canonical message for passed verification passes.</summary>
		private const string VERIFIED_OK_MSG = "Passed verification.";

		/// <summary>Canonical VerificationResult for not-yet-tried verifications.</summary>
		/// <remarks>
		/// Canonical VerificationResult for not-yet-tried verifications.
		/// This happens if some earlier verification pass did not return
		/// <see cref="VERIFIED_OK"/>
		/// .
		/// </remarks>
		public static readonly NBCEL.verifier.VerificationResult VR_NOTYET = new NBCEL.verifier.VerificationResult
			(VERIFIED_NOTYET, VERIFIED_NOTYET_MSG);

		/// <summary>Canonical VerificationResult for passed verifications.</summary>
		public static readonly NBCEL.verifier.VerificationResult VR_OK = new NBCEL.verifier.VerificationResult
			(VERIFIED_OK, VERIFIED_OK_MSG);

		/// <summary>The numeric status.</summary>
		private readonly int numeric;

		/// <summary>The detailed message.</summary>
		private readonly string detailMessage;

		/// <summary>The usual constructor.</summary>
		public VerificationResult(int status, string message)
		{
			numeric = status;
			detailMessage = message;
		}

		/// <summary>
		/// Returns one of the
		/// <see cref="VERIFIED_OK"/>
		/// ,
		/// <see cref="VERIFIED_NOTYET"/>
		/// ,
		/// <see cref="VERIFIED_REJECTED"/>
		/// constants.
		/// </summary>
		public virtual int GetStatus()
		{
			return numeric;
		}

		/// <summary>Returns a detailed message.</summary>
		public virtual string GetMessage()
		{
			return detailMessage;
		}

		/// <returns>a hash code value for the object.</returns>
		public override int GetHashCode()
		{
			return numeric ^ detailMessage.GetHashCode();
		}

		/// <summary>Returns if two VerificationResult instances are equal.</summary>
		public override bool Equals(object o)
		{
			if (!(o is NBCEL.verifier.VerificationResult))
			{
				return false;
			}
			NBCEL.verifier.VerificationResult other = (NBCEL.verifier.VerificationResult)o;
			return (other.numeric == this.numeric) && other.detailMessage.Equals(this.detailMessage
				);
		}

		/// <summary>Returns a String representation of the VerificationResult.</summary>
		public override string ToString()
		{
			string ret = string.Empty;
			if (numeric == VERIFIED_NOTYET)
			{
				ret = "VERIFIED_NOTYET";
			}
			if (numeric == VERIFIED_OK)
			{
				ret = "VERIFIED_OK";
			}
			if (numeric == VERIFIED_REJECTED)
			{
				ret = "VERIFIED_REJECTED";
			}
			ret += "\n" + detailMessage + "\n";
			return ret;
		}
	}
}
