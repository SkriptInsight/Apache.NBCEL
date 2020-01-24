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

namespace Apache.NBCEL.Generic
{
    /// <summary>Equality of instructions isn't clearly to be defined.</summary>
    /// <remarks>
    ///     Equality of instructions isn't clearly to be defined. You might
    ///     wish, for example, to compare whether instructions have the same
    ///     meaning. E.g., whether two INVOKEVIRTUALs describe the same
    ///     call.
    ///     <p>
    ///         The DEFAULT comparator however, considers two instructions
    ///         to be equal if they have same opcode and point to the same indexes
    ///         (if any) in the constant pool or the same local variable index. Branch
    ///         instructions must have the same target.
    ///     </p>
    /// </remarks>
    /// <seealso cref="Instruction" />
    public abstract class InstructionComparator
    {
        public static InstructionComparator DEFAULT = new DefaultInstructionComparatorImpl();

        public abstract bool Equals(Instruction i1, Instruction
            i2);
    }

    internal class DefaultInstructionComparatorImpl : InstructionComparator
    {
        public override bool Equals(Instruction i1, Instruction i2)
        {
            if (i1.GetOpcode() == i2.GetOpcode())
            {
                if (i1 is BranchInstruction)
                    // BIs are never equal to make targeters work correctly (BCEL-195)
                    return false;
                if (i1 is BaseConstantPushInstruction)
                    //                } else if (i1 == i2) { TODO consider adding this shortcut
                    //                    return true; // this must be AFTER the BI test
                    return ((BaseConstantPushInstruction) i1).GetValue().Equals(
                        ((BaseConstantPushInstruction
                            ) i2).GetValue());
                if (i1 is IndexedInstruction)
                    return ((IndexedInstruction) i1).GetIndex() == ((IndexedInstruction
                               ) i2).GetIndex();
                if (i1 is NEWARRAY)
                    return ((NEWARRAY) i1).GetTypecode() == ((NEWARRAY) i2)
                           .GetTypecode();
                return true;
            }

            return false;
        }
    }
}