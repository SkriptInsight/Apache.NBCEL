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

using System;

namespace NBCEL.generic
{
    /// <summary>Super class for JSR - Jump to subroutine</summary>
    public abstract class JsrInstruction : BranchInstruction, UnconditionalBranch
        , TypedInstruction, StackProducer
    {
        internal JsrInstruction(short opcode, InstructionHandle target)
            : base(opcode, target)
        {
        }

        /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
        /// <remarks>
        ///     Empty constructor needed for Instruction.readInstruction.
        ///     Not to be used otherwise.
        /// </remarks>
        internal JsrInstruction()
        {
        }

        /// <returns>return address type</returns>
        public virtual Type GetType(ConstantPoolGen cp)
        {
            return new ReturnaddressType(PhysicalSuccessor());
        }

        /// <summary>
        ///     Returns an InstructionHandle to the physical successor
        ///     of this JsrInstruction.
        /// </summary>
        /// <remarks>
        ///     Returns an InstructionHandle to the physical successor
        ///     of this JsrInstruction.
        ///     <B>
        ///         For this method to work,
        ///         this JsrInstruction object must not be shared between
        ///         multiple InstructionHandle objects!
        ///     </B>
        ///     Formally, there must not be InstructionHandle objects
        ///     i, j where i != j and i.getInstruction() == this ==
        ///     j.getInstruction().
        /// </remarks>
        /// <returns>
        ///     an InstructionHandle to the "next" instruction that
        ///     will be executed when RETurned from a subroutine.
        /// </returns>
        public virtual InstructionHandle PhysicalSuccessor()
        {
            var ih = base.GetTarget();
            // Rewind!
            while (ih.GetPrev() != null) ih = ih.GetPrev();
            // Find the handle for "this" JsrInstruction object.
            while (ih.GetInstruction() != this) ih = ih.GetNext();
            var toThis = ih;
            while (ih != null)
            {
                ih = ih.GetNext();
                if (ih != null && ih.GetInstruction() == this)
                    throw new Exception("physicalSuccessor() called on a shared JsrInstruction."
                    );
            }

            // Return the physical successor
            return toThis.GetNext();
        }
    }
}