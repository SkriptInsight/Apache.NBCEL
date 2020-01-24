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

namespace NBCEL.generic
{
	/// <summary>Denote that a class targets InstructionHandles within an InstructionList.
	/// 	</summary>
	/// <remarks>
	/// Denote that a class targets InstructionHandles within an InstructionList. Namely
	/// the following implementers:
	/// </remarks>
	/// <seealso cref="BranchHandle"/>
	/// <seealso cref="LocalVariableGen"/>
	/// <seealso cref="CodeExceptionGen"/>
	public interface InstructionTargeter
	{
		/// <summary>Checks whether this targeter targets the specified instruction handle.</summary>
		bool ContainsTarget(NBCEL.generic.InstructionHandle ih);

		/// <summary>Replaces the target of this targeter from this old handle to the new handle.
		/// 	</summary>
		/// <param name="old_ih">the old handle</param>
		/// <param name="new_ih">the new handle</param>
		/// <exception cref="ClassGenException">if old_ih is not targeted by this object</exception>
		/// <exception cref="NBCEL.generic.ClassGenException"/>
		void UpdateTarget(NBCEL.generic.InstructionHandle old_ih, NBCEL.generic.InstructionHandle
			 new_ih);
	}
}
