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
	/// <summary>Super class for the IFxxx family of instructions.</summary>
	public abstract class IfInstruction : NBCEL.generic.BranchInstruction, NBCEL.generic.StackConsumer
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal IfInstruction()
		{
		}

		/// <param name="opcode">opcode of instruction</param>
		/// <param name="target">Target instruction to branch to</param>
		protected internal IfInstruction(short opcode, NBCEL.generic.InstructionHandle target
			)
			: base(opcode, target)
		{
		}

		/// <returns>negation of instruction, e.g. IFEQ.negate() == IFNE</returns>
		public abstract NBCEL.generic.IfInstruction Negate();
	}
}
