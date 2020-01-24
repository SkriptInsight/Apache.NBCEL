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
	/// <summary>
	///     BranchHandle is returned by specialized InstructionList.append() whenever a
	///     BranchInstruction is appended.
	/// </summary>
	/// <remarks>
	///     BranchHandle is returned by specialized InstructionList.append() whenever a
	///     BranchInstruction is appended. This is useful when the target of this
	///     instruction is not known at time of creation and must be set later
	///     via setTarget().
	/// </remarks>
	/// <seealso cref="InstructionHandle" />
	/// <seealso cref="Instruction" />
	/// <seealso cref="InstructionList" />
	public sealed class BranchHandle : InstructionHandle
    {
        private BranchInstruction bi;

        private BranchHandle(BranchInstruction i)
            : base(i)
        {
            // This is also a cache in case the InstructionHandle#swapInstruction() method is used
            // See BCEL-273
            // An alias in fact, but saves lots of casts
            bi = i;
        }

        /// <summary>Factory method.</summary>
        internal static BranchHandle GetBranchHandle(BranchInstruction
            i)
        {
            return new BranchHandle(i);
        }

        /* Override InstructionHandle methods: delegate to branch instruction.
        * Through this overriding all access to the private i_position field should
        * be prevented.
        */
        public override int GetPosition()
        {
            return bi.GetPosition();
        }

        internal override void SetPosition(int pos)
        {
            // Original code: i_position = bi.position = pos;
            bi.SetPosition(pos);
            base.SetPosition(pos);
        }

        protected internal override int UpdatePosition(int offset, int max_offset)
        {
            var x = bi.UpdatePosition(offset, max_offset);
            base.SetPosition(bi.GetPosition());
            return x;
        }

        /// <summary>Pass new target to instruction.</summary>
        public void SetTarget(InstructionHandle ih)
        {
            bi.SetTarget(ih);
        }

        /// <summary>Update target of instruction.</summary>
        public void UpdateTarget(InstructionHandle old_ih, InstructionHandle
            new_ih)
        {
            bi.UpdateTarget(old_ih, new_ih);
        }

        /// <returns>target of instruction.</returns>
        public InstructionHandle GetTarget()
        {
            return bi.GetTarget();
        }

        /// <summary>Set new contents.</summary>
        /// <remarks>
        ///     Set new contents. Old instruction is disposed and may not be used anymore.
        /// </remarks>
        public override void SetInstruction(Instruction i)
        {
            // This is only done in order to apply the additional type check; could be merged with super impl.
            // TODO could be package-protected?
            base.SetInstruction(i);
            if (!(i is BranchInstruction))
                throw new ClassGenException("Assigning " + i + " to branch handle which is not a branch instruction"
                );
            bi = (BranchInstruction) i;
        }
    }
}