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
using System.Text;
using java.io;
using NBCEL.util;
using Sharpen;

namespace NBCEL.generic
{
	/// <summary>
	///     Select - Abstract super class for LOOKUPSWITCH and TABLESWITCH instructions.
	/// </summary>
	/// <remarks>
	///     Select - Abstract super class for LOOKUPSWITCH and TABLESWITCH instructions.
	///     <p>We use our super's <code>target</code> property as the default target.
	/// </remarks>
	/// <seealso cref="LOOKUPSWITCH" />
	/// <seealso cref="TABLESWITCH" />
	/// <seealso cref="InstructionList" />
	public abstract class Select : BranchInstruction, VariableLengthInstruction
        , StackConsumer, StackProducer
    {
        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal int fixed_length;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal int[] indices;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal int[] match;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal int match_length;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal int padding;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal InstructionHandle[] targets;

        /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
        /// <remarks>
        ///     Empty constructor needed for Instruction.readInstruction.
        ///     Not to be used otherwise.
        /// </remarks>
        internal Select()
        {
        }

        /// <summary>(Match, target) pairs for switch.</summary>
        /// <remarks>
        ///     (Match, target) pairs for switch.
        ///     `Match' and `targets' must have the same length of course.
        /// </remarks>
        /// <param name="match">array of matching values</param>
        /// <param name="targets">instruction targets</param>
        /// <param name="defaultTarget">default instruction target</param>
        internal Select(short opcode, int[] match, InstructionHandle[] targets
            , InstructionHandle defaultTarget)
            : base(opcode, null)
        {
            /* @since 6.0 */
            // matches, i.e., case 1: ... TODO could be package-protected?
            // target offsets TODO could be package-protected?
            // target objects in instruction list TODO could be package-protected?
            // fixed length defined by subclasses TODO could be package-protected?
            // number of cases TODO could be package-protected?
            // number of pad bytes for alignment TODO could be package-protected?
            // don't set default target before instuction is built
            this.match = match;
            this.targets = targets;
            // now it's safe to set default target
            SetTarget(defaultTarget);
            foreach (var target2 in targets) NotifyTarget(null, target2, this);
            if ((match_length = match.Length) != targets.Length)
                throw new ClassGenException("Match and target array have not the same length: Match length: "
                                            + match.Length + " Target length: " + targets.Length);
            indices = new int[match_length];
        }

        /// <summary>
        ///     Since this is a variable length instruction, it may shift the following
        ///     instructions which then need to update their position.
        /// </summary>
        /// <remarks>
        ///     Since this is a variable length instruction, it may shift the following
        ///     instructions which then need to update their position.
        ///     Called by InstructionList.setPositions when setting the position for every
        ///     instruction. In the presence of variable length instructions `setPositions'
        ///     performs multiple passes over the instruction list to calculate the
        ///     correct (byte) positions and offsets by calling this function.
        /// </remarks>
        /// <param name="offset">
        ///     additional offset caused by preceding (variable length) instructions
        /// </param>
        /// <param name="max_offset">
        ///     the maximum offset that may be caused by these instructions
        /// </param>
        /// <returns>
        ///     additional offset caused by possible change of this instruction's length
        /// </returns>
        protected internal override int UpdatePosition(int offset, int max_offset)
        {
            SetPosition(GetPosition() + offset);
            // Additional offset caused by preceding SWITCHs, GOTOs, etc.
            var old_length = (short) base.GetLength();
            /* Alignment on 4-byte-boundary, + 1, because of tag byte.
            */
            padding = (4 - (GetPosition() + 1) % 4) % 4;
            SetLength((short) (fixed_length + padding));
            // Update length
            return base.GetLength() - old_length;
        }

        /// <summary>Dump instruction as byte code to stream out.</summary>
        /// <param name="out">Output stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream @out)
        {
            @out.WriteByte(base.GetOpcode());
            for (var i = 0; i < padding; i++) @out.WriteByte(0);
            base.SetIndex(GetTargetOffset());
            // Write default target offset
            @out.WriteInt(GetIndex());
        }

        /// <summary>Read needed data (e.g.</summary>
        /// <remarks>Read needed data (e.g. index) from file.</remarks>
        /// <exception cref="System.IO.IOException" />
        protected internal override void InitFromFile(ByteSequence bytes, bool
            wide)
        {
            padding = (4 - bytes.GetIndex() % 4) % 4;
            // Compute number of pad bytes
            for (var i = 0; i < padding; i++) bytes.ReadByte();
            // Default branch target common for both cases (TABLESWITCH, LOOKUPSWITCH)
            base.SetIndex(bytes.ReadInt());
        }

        /// <returns>mnemonic for instruction</returns>
        public override string ToString(bool verbose)
        {
            var buf = new StringBuilder(base.ToString(verbose
            ));
            if (verbose)
                for (var i = 0; i < match_length; i++)
                {
                    var s = "null";
                    if (targets[i] != null) s = targets[i].GetInstruction().ToString();
                    buf.Append("(").Append(match[i]).Append(", ").Append(s).Append(" = {").Append(indices
                        [i]).Append("})");
                }
            else
                buf.Append(" ...");

            return buf.ToString();
        }

        /// <summary>Set branch target for `i'th case</summary>
        public virtual void SetTarget(int i, InstructionHandle target)
        {
            // TODO could be package-protected?
            NotifyTarget(targets[i], target, this);
            targets[i] = target;
        }

        /// <param name="old_ih">old target</param>
        /// <param name="new_ih">new target</param>
        public override void UpdateTarget(InstructionHandle old_ih, InstructionHandle
            new_ih)
        {
            var targeted = false;
            if (base.GetTarget() == old_ih)
            {
                targeted = true;
                SetTarget(new_ih);
            }

            for (var i = 0; i < targets.Length; i++)
                if (targets[i] == old_ih)
                {
                    targeted = true;
                    SetTarget(i, new_ih);
                }

            if (!targeted) throw new ClassGenException("Not targeting " + old_ih);
        }

        /// <returns>true, if ih is target of this instruction</returns>
        public override bool ContainsTarget(InstructionHandle ih)
        {
            if (base.GetTarget() == ih) return true;
            foreach (var target2 in targets)
                if (target2 == ih)
                    return true;
            return false;
        }

        /// <exception cref="java.lang.CloneNotSupportedException" />
        protected internal virtual object Clone()
        {
            var copy = (Select) MemberwiseClone();
            copy.match = (int[]) match.Clone();
            copy.indices = (int[]) indices.Clone();
            copy.targets = (InstructionHandle[]) targets.Clone();
            return copy;
        }

        /// <summary>Inform targets that they're not targeted anymore.</summary>
        internal override void Dispose()
        {
            base.Dispose();
            foreach (var target2 in targets) target2.RemoveTargeter(this);
        }

        /// <returns>array of match indices</returns>
        public virtual int[] GetMatchs()
        {
            return match;
        }

        /// <returns>array of match target offsets</returns>
        public virtual int[] GetIndices()
        {
            return indices;
        }

        /// <returns>array of match targets</returns>
        public virtual InstructionHandle[] GetTargets()
        {
            return targets;
        }

        /// <returns>match entry</returns>
        /// <since>6.0</since>
        internal int GetMatch(int index)
        {
            return match[index];
        }

        /// <returns>index entry from indices</returns>
        /// <since>6.0</since>
        internal int GetIndices(int index)
        {
            return indices[index];
        }

        /// <returns>target entry</returns>
        /// <since>6.0</since>
        internal InstructionHandle GetTarget(int index)
        {
            return targets[index];
        }

        /// <returns>the fixed_length</returns>
        /// <since>6.0</since>
        internal int GetFixed_length()
        {
            return fixed_length;
        }

        /// <param name="fixed_length">the fixed_length to set</param>
        /// <since>6.0</since>
        internal void SetFixed_length(int fixed_length)
        {
            this.fixed_length = fixed_length;
        }

        /// <returns>the match_length</returns>
        /// <since>6.0</since>
        internal int GetMatch_length()
        {
            return match_length;
        }

        /// <param name="match_length">the match_length to set</param>
        /// <since>6.0</since>
        internal int SetMatch_length(int match_length)
        {
            this.match_length = match_length;
            return match_length;
        }

        /// <param name="index" />
        /// <param name="value" />
        /// <since>6.0</since>
        internal void SetMatch(int index, int value)
        {
            match[index] = value;
        }

        /// <param name="array" />
        /// <since>6.0</since>
        internal void SetIndices(int[] array)
        {
            indices = array;
        }

        /// <param name="array" />
        /// <since>6.0</since>
        internal void SetMatches(int[] array)
        {
            match = array;
        }

        /// <param name="array" />
        /// <since>6.0</since>
        internal void SetTargets(InstructionHandle[] array)
        {
            targets = array;
        }

        /// <returns>the padding</returns>
        /// <since>6.0</since>
        internal int GetPadding()
        {
            return padding;
        }

        /// <since>6.0</since>
        internal int SetIndices(int i, int value)
        {
            indices[i] = value;
            return value;
        }

        // Allow use in nested calls
    }
}