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
    /// <summary>Equality of instructions isn't clearly to be defined.</summary>
    /// <remarks>
    /// Equality of instructions isn't clearly to be defined. You might
    /// wish, for example, to compare whether instructions have the same
    /// meaning. E.g., whether two INVOKEVIRTUALs describe the same
    /// call.
    /// <p>
    /// The DEFAULT comparator however, considers two instructions
    /// to be equal if they have same opcode and point to the same indexes
    /// (if any) in the constant pool or the same local variable index. Branch
    /// instructions must have the same target.
    /// </p>
    /// </remarks>
    /// <seealso cref="Instruction"/>
    public abstract class InstructionComparator
    {
        public static NBCEL.generic.InstructionComparator DEFAULT = new DefaultInstructionComparatorImpl();

        public abstract bool Equals(NBCEL.generic.Instruction i1, NBCEL.generic.Instruction
            i2);
    }

    class DefaultInstructionComparatorImpl : InstructionComparator
    {
        public override bool Equals(Instruction i1, Instruction i2)
        {
            if (i1.GetOpcode() == i2.GetOpcode())
            {
                if (i1 is NBCEL.generic.BranchInstruction)
                {
                    // BIs are never equal to make targeters work correctly (BCEL-195)
                    return false;
                }
                else if (i1 is NBCEL.generic.BaseConstantPushInstruction)
                {
                    //                } else if (i1 == i2) { TODO consider adding this shortcut
                    //                    return true; // this must be AFTER the BI test
                    return ((NBCEL.generic.BaseConstantPushInstruction) i1).GetValue().Equals(
                        ((NBCEL.generic.BaseConstantPushInstruction
                            ) i2).GetValue());
                }
                else if (i1 is NBCEL.generic.IndexedInstruction)
                {
                    return ((NBCEL.generic.IndexedInstruction) i1).GetIndex() == ((NBCEL.generic.IndexedInstruction
                               ) i2).GetIndex();
                }
                else if (i1 is NBCEL.generic.NEWARRAY)
                {
                    return ((NBCEL.generic.NEWARRAY) i1).GetTypecode() == ((NBCEL.generic.NEWARRAY) i2)
                           .GetTypecode();
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}