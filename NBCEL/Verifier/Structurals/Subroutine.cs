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

using Apache.NBCEL.Generic;

namespace Apache.NBCEL.Verifier.Structurals
{
	/// <summary>This interface defines properties of JVM bytecode subroutines.</summary>
	/// <remarks>
	///     This interface defines properties of JVM bytecode subroutines. Note that it is 'abused' to maintain the top-level
	///     code in a consistent fashion, too.
	/// </remarks>
	public interface Subroutine
    {
	    /// <summary>
	    ///     Returns all the JsrInstructions that have the first instruction of this subroutine as their target.
	    /// </summary>
	    /// <remarks>
	    ///     Returns all the JsrInstructions that have the first instruction of this subroutine as their target.
	    ///     <B>
	    ///         Must not
	    ///         be invoked on the 'top-level subroutine'.
	    ///     </B>
	    /// </remarks>
	    /// <returns>
	    ///     The JsrInstructions that have the first instruction of this subroutine as their target.
	    /// </returns>
	    InstructionHandle[] GetEnteringJsrInstructions();

	    /// <summary>Returns the one and only RET that leaves the subroutine.</summary>
	    /// <remarks>
	    ///     Returns the one and only RET that leaves the subroutine. Note that JustIce has a pretty rigid notion of a
	    ///     subroutine. <B>Must not be invoked on the 'top-level subroutine'.</B>
	    /// </remarks>
	    /// <returns>The one and only RET that leaves the subroutine.</returns>
	    /// <seealso cref="Subroutines" />
	    InstructionHandle GetLeavingRET();

	    /// <summary>Returns all instructions that together form this subroutine.</summary>
	    /// <remarks>
	    ///     Returns all instructions that together form this subroutine. Note that an instruction is part of exactly one
	    ///     subroutine (the top-level code is considered to be a special subroutine) - else it is not reachable at all (dead
	    ///     code).
	    /// </remarks>
	    /// <returns>All instructions that together form this subroutine.</returns>
	    InstructionHandle[] GetInstructions();

	    /// <summary>
	    ///     Returns if the given InstructionHandle refers to an instruction that is part of this subroutine.
	    /// </summary>
	    /// <remarks>
	    ///     Returns if the given InstructionHandle refers to an instruction that is part of this subroutine. This is a
	    ///     convenience method that saves iteration over the InstructionHandle objects returned by getInstructions().
	    /// </remarks>
	    /// <param name="inst">The InstructionHandle to test.</param>
	    /// <returns>
	    ///     Whether the given InstructionHandle refers to an instruction that is part of this subroutine.
	    /// </returns>
	    /// <seealso cref="GetInstructions()" />
	    bool Contains(InstructionHandle inst);

	    /// <summary>
	    ///     Returns an int[] containing the indices of the local variable slots accessed by this Subroutine (read-accessed,
	    ///     write-accessed or both); local variables referenced by subroutines of this subroutine are not included.
	    /// </summary>
	    /// <returns>An int[] containing the indices of the local variable slots.</returns>
	    /// <seealso cref="GetRecursivelyAccessedLocalsIndices()" />
	    int[] GetAccessedLocalsIndices();

	    /// <summary>
	    ///     Returns an int[] containing the indices of the local variable slots accessed by this Subroutine (read-accessed,
	    ///     write-accessed or both); local variables referenced by subroutines of this subroutine are included.
	    /// </summary>
	    /// <returns>An int[] containing the indices of the local variable slots.</returns>
	    /// <seealso cref="GetAccessedLocalsIndices()" />
	    int[] GetRecursivelyAccessedLocalsIndices();

	    /// <summary>Returns the subroutines that are directly called from this subroutine.</summary>
	    /// <returns>The subroutines that are directly called from this subroutine.</returns>
	    Subroutine[] SubSubs();
    }
}