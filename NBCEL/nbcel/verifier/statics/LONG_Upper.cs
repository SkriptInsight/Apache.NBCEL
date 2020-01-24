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

namespace NBCEL.verifier.statics
{
	/// <summary>This class represents the upper half of a LONG variable.</summary>
	public sealed class LONG_Upper : NBCEL.generic.Type
	{
		/// <summary>The one and only instance of this class.</summary>
		private static readonly NBCEL.verifier.statics.LONG_Upper singleton = new NBCEL.verifier.statics.LONG_Upper
			();

		/// <summary>The constructor; this class must not be instantiated from the outside.</summary>
		private LONG_Upper()
			: base(NBCEL.Const.T_UNKNOWN, "Long_Upper")
		{
		}

		/// <summary>Gets the single instance of this class.</summary>
		/// <returns>the single instance of this class.</returns>
		public static NBCEL.verifier.statics.LONG_Upper TheInstance()
		{
			return singleton;
		}
	}
}
