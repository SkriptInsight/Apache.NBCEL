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
	/// <summary>Super class for stack operations like DUP and POP.</summary>
	public abstract class StackInstruction : NBCEL.generic.Instruction
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal StackInstruction()
		{
		}

		/// <param name="opcode">instruction opcode</param>
		protected internal StackInstruction(short opcode)
			: base(opcode, (short)1)
		{
		}

		/// <returns>Type.UNKNOWN</returns>
		public virtual NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cp)
		{
			return NBCEL.generic.Type.UNKNOWN;
		}
	}
}