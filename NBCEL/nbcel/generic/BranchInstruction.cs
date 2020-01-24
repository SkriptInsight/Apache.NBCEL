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
using java.io;
using NBCEL.util;
using Sharpen;

namespace NBCEL.generic
{
	/// <summary>Abstract super class for branching instructions like GOTO, IFEQ, etc..</summary>
	/// <remarks>
	///     Abstract super class for branching instructions like GOTO, IFEQ, etc..
	///     Branch instructions may have a variable length, namely GOTO, JSR,
	///     LOOKUPSWITCH and TABLESWITCH.
	/// </remarks>
	/// <seealso cref="InstructionList" />
	public abstract class BranchInstruction : Instruction, InstructionTargeter
    {
        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal int index;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal int position;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal InstructionHandle target;

        /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
        /// <remarks>
        ///     Empty constructor needed for Instruction.readInstruction.
        ///     Not to be used otherwise.
        /// </remarks>
        internal BranchInstruction()
        {
        }

        /// <summary>Common super constructor</summary>
        /// <param name="opcode">Instruction opcode</param>
        /// <param name="target">instruction to branch to</param>
        protected internal BranchInstruction(short opcode, InstructionHandle
            target)
            : base(opcode, 3)
        {
            // Branch target relative to this instruction
            // Target object in instruction list
            // Byte code offset
            SetTarget(target);
        }

        /// <param name="old_ih">old target</param>
        /// <param name="new_ih">new target</param>
        public virtual void UpdateTarget(InstructionHandle old_ih, InstructionHandle
            new_ih)
        {
            if (target == old_ih)
                SetTarget(new_ih);
            else
                throw new ClassGenException("Not targeting " + old_ih + ", but " +
                                            target);
        }

        /// <returns>true, if ih is target of this instruction</returns>
        public virtual bool ContainsTarget(InstructionHandle ih)
        {
            return target == ih;
        }

        /// <summary>Dump instruction as byte code to stream out.</summary>
        /// <param name="out">Output stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream @out)
        {
            @out.WriteByte(base.GetOpcode());
            index = GetTargetOffset();
            if (!IsValidShort(index))
                throw new ClassGenException("Branch target offset too large for short: "
                                            + index);
            @out.WriteShort(index);
        }

        // May be negative, i.e., point backwards
        /// <param name="_target">branch target</param>
        /// <returns>the offset to  `target' relative to this instruction</returns>
        protected internal virtual int GetTargetOffset(InstructionHandle _target
        )
        {
            if (_target == null)
                throw new ClassGenException("Target of " + base.ToString(true) + " is invalid null handle"
                );
            var t = _target.GetPosition();
            if (t < 0)
                throw new ClassGenException("Invalid branch target position offset for "
                                            + base.ToString(true) + ":" + t + ":" + _target);
            return t - position;
        }

        /// <returns>the offset to this instruction's target</returns>
        protected internal virtual int GetTargetOffset()
        {
            return GetTargetOffset(target);
        }

        /// <summary>
        ///     Called by InstructionList.setPositions when setting the position for every
        ///     instruction.
        /// </summary>
        /// <remarks>
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
        protected internal virtual int UpdatePosition(int offset, int max_offset)
        {
            position += offset;
            return 0;
        }

        /// <summary>
        ///     Long output format:
        ///     &lt;position in byte code&gt;
        ///     &lt;name of opcode&gt; "["&lt;opcode number&gt;"]"
        ///     "("&lt;length of instruction&gt;")"
        ///     "&lt;"&lt;target instruction&gt;"&gt;" "@"&lt;branch target offset&gt;
        /// </summary>
        /// <param name="verbose">long/short format switch</param>
        /// <returns>mnemonic for instruction</returns>
        public override string ToString(bool verbose)
        {
            var s = base.ToString(verbose);
            var t = "null";
            if (verbose)
            {
                if (target != null)
                {
                    if (target.GetInstruction() == this)
                        t = "<points to itself>";
                    else if (target.GetInstruction() == null)
                        t = "<null instruction!!!?>";
                    else
                        // I'm more interested in the address of the target then
                        // the instruction located there.
                        //t = target.getInstruction().toString(false); // Avoid circles
                        t = string.Empty + target.GetPosition();
                }
            }
            else if (target != null)
            {
                index = target.GetPosition();
                // index = getTargetOffset();  crashes if positions haven't been set
                // t = "" + (index + position);
                t = string.Empty + index;
            }

            return s + " -> " + t;
        }

        /// <summary>Read needed data (e.g.</summary>
        /// <remarks>
        ///     Read needed data (e.g. index) from file. Conversion to a InstructionHandle
        ///     is done in InstructionList(byte[]).
        /// </remarks>
        /// <param name="bytes">input stream</param>
        /// <param name="wide">wide prefix?</param>
        /// <seealso cref="InstructionList" />
        /// <exception cref="System.IO.IOException" />
        protected internal override void InitFromFile(ByteSequence bytes, bool
            wide)
        {
            SetLength(3);
            index = bytes.ReadShort();
        }

        /// <returns>target offset in byte code</returns>
        public int GetIndex()
        {
            return index;
        }

        /// <returns>target of branch instruction</returns>
        public virtual InstructionHandle GetTarget()
        {
            return target;
        }

        /// <summary>Set branch target</summary>
        /// <param name="target">branch target</param>
        public virtual void SetTarget(InstructionHandle target)
        {
            NotifyTarget(this.target, target, this);
            this.target = target;
        }

        /// <summary>
        ///     Used by BranchInstruction, LocalVariableGen, CodeExceptionGen, LineNumberGen
        /// </summary>
        internal static void NotifyTarget(InstructionHandle old_ih, InstructionHandle
            new_ih, InstructionTargeter t)
        {
            if (old_ih != null) old_ih.RemoveTargeter(t);
            if (new_ih != null) new_ih.AddTargeter(t);
        }

        /// <summary>Inform target that it's not targeted anymore.</summary>
        internal override void Dispose()
        {
            SetTarget(null);
            index = -1;
            position = -1;
        }

        /// <returns>the position</returns>
        /// <since>6.0</since>
        protected internal virtual int GetPosition()
        {
            return position;
        }

        /// <param name="position">the position to set</param>
        /// <since>6.0</since>
        protected internal virtual void SetPosition(int position)
        {
            this.position = position;
        }

        /// <param name="index">the index to set</param>
        /// <since>6.0</since>
        protected internal virtual void SetIndex(int index)
        {
            this.index = index;
        }
    }
}