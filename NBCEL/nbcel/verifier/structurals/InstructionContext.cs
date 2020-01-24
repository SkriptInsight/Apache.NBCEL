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
	/// An InstructionContext offers convenient access
	/// to information like control flow successors and
	/// such.
	/// </summary>
	public interface InstructionContext
	{
		/// <summary>
		/// The getTag and setTag methods may be used for
		/// temporary flagging, such as graph colouring.
		/// </summary>
		/// <remarks>
		/// The getTag and setTag methods may be used for
		/// temporary flagging, such as graph colouring.
		/// Nothing in the InstructionContext object depends
		/// on the value of the tag. JustIce does not use it.
		/// </remarks>
		/// <seealso cref="SetTag(int)"/>
		int GetTag();

		/// <summary>
		/// The getTag and setTag methods may be used for
		/// temporary flagging, such as graph colouring.
		/// </summary>
		/// <remarks>
		/// The getTag and setTag methods may be used for
		/// temporary flagging, such as graph colouring.
		/// Nothing in the InstructionContext object depends
		/// on the value of the tag. JustIce does not use it.
		/// </remarks>
		/// <seealso cref="GetTag()"/>
		void SetTag(int tag);

		/// <summary>
		/// This method symbolically executes the Instruction
		/// held in the InstructionContext.
		/// </summary>
		/// <remarks>
		/// This method symbolically executes the Instruction
		/// held in the InstructionContext.
		/// It "merges in" the incoming execution frame situation
		/// (see The Java Virtual Machine Specification, 2nd
		/// edition, page 146).
		/// By so doing, the outgoing execution frame situation
		/// is calculated.
		/// This method is JustIce-specific and is usually of
		/// no sense for users of the ControlFlowGraph class.
		/// They should use getInstruction().accept(Visitor),
		/// possibly in conjunction with the ExecutionVisitor.
		/// </remarks>
		/// <seealso cref="ControlFlowGraph"/>
		/// <seealso cref="ExecutionVisitor"/>
		/// <seealso cref="GetOutFrame(System.Collections.Generic.List{E})"/>
		/// <returns>
		/// true -  if and only if the "outgoing" frame situation
		/// changed from the one before execute()ing.
		/// </returns>
		bool Execute(NBCEL.verifier.structurals.Frame inFrame, System.Collections.Generic.List
			<NBCEL.verifier.structurals.InstructionContext> executionPredecessors, NBCEL.verifier.structurals.InstConstraintVisitor
			 icv, NBCEL.verifier.structurals.ExecutionVisitor ev);

		NBCEL.verifier.structurals.Frame GetInFrame();

		/// <summary>
		/// This method returns the outgoing execution frame situation;
		/// therefore <B>it has to be calculated by execute(Frame, ArrayList)
		/// first.</B>
		/// </summary>
		/// <seealso cref="Execute(Frame, System.Collections.Generic.List{E}, InstConstraintVisitor, ExecutionVisitor)
		/// 	"/>
		NBCEL.verifier.structurals.Frame GetOutFrame(System.Collections.Generic.List<NBCEL.verifier.structurals.InstructionContext
			> executionPredecessors);

		/// <summary>Returns the InstructionHandle this InstructionContext is wrapped around.
		/// 	</summary>
		/// <returns>The InstructionHandle this InstructionContext is wrapped around.</returns>
		NBCEL.generic.InstructionHandle GetInstruction();

		/// <summary>Returns the usual control flow successors.</summary>
		/// <seealso cref="GetExceptionHandlers()"/>
		NBCEL.verifier.structurals.InstructionContext[] GetSuccessors();

		/// <summary>Returns the exception handlers that protect this instruction.</summary>
		/// <remarks>
		/// Returns the exception handlers that protect this instruction.
		/// They are special control flow successors.
		/// </remarks>
		NBCEL.verifier.structurals.ExceptionHandler[] GetExceptionHandlers();
	}
}
