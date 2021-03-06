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

namespace Apache.NBCEL.Generic
{
	/// <summary>
	///     SWITCH - Branch depending on int value, generates either LOOKUPSWITCH or
	///     TABLESWITCH instruction, depending on whether the match values (int[]) can be
	///     sorted with no gaps between the numbers.
	/// </summary>
	public sealed class SWITCH : CompoundInstruction
    {
        private readonly Select instruction;
        private int[] match;

        private readonly int match_length;

        private InstructionHandle[] targets;

        /// <summary>Template for switch() constructs.</summary>
        /// <remarks>
        ///     Template for switch() constructs. If the match array can be
        ///     sorted in ascending order with gaps no larger than max_gap
        ///     between the numbers, a TABLESWITCH instruction is generated, and
        ///     a LOOKUPSWITCH otherwise. The former may be more efficient, but
        ///     needs more space.
        ///     Note, that the key array always will be sorted, though we leave
        ///     the original arrays unaltered.
        /// </remarks>
        /// <param name="match">array of match values (case 2: ... case 7: ..., etc.)</param>
        /// <param name="targets">the instructions to be branched to for each case</param>
        /// <param name="target">the default target</param>
        /// <param name="max_gap">maximum gap that may between case branches</param>
        public SWITCH(int[] match, InstructionHandle[] targets, InstructionHandle
            target, int max_gap)
        {
            this.match = (int[]) match.Clone();
            this.targets = (InstructionHandle[]) targets.Clone();
            if ((match_length = match.Length) < 2)
            {
                instruction = new TABLESWITCH(match, targets, target);
            }
            else
            {
                Sort(0, match_length - 1);
                if (MatchIsOrdered(max_gap))
                {
                    Fillup(max_gap, target);
                    instruction = new TABLESWITCH(this.match, this.targets, target);
                }
                else
                {
                    instruction = new LOOKUPSWITCH(this.match, this.targets, target);
                }
            }
        }

        public SWITCH(int[] match, InstructionHandle[] targets, InstructionHandle
            target)
            : this(match, targets, target, 1)
        {
        }

        public InstructionList GetInstructionList()
        {
            return new InstructionList(instruction);
        }

        private void Fillup(int max_gap, InstructionHandle target)
        {
            var max_size = match_length + match_length * max_gap;
            var m_vec = new int[max_size];
            var t_vec = new InstructionHandle[max_size
            ];
            var count = 1;
            m_vec[0] = match[0];
            t_vec[0] = targets[0];
            for (var i = 1; i < match_length; i++)
            {
                var prev = match[i - 1];
                var gap = match[i] - prev;
                for (var j = 1; j < gap; j++)
                {
                    m_vec[count] = prev + j;
                    t_vec[count] = target;
                    count++;
                }

                m_vec[count] = match[i];
                t_vec[count] = targets[i];
                count++;
            }

            match = new int[count];
            targets = new InstructionHandle[count];
            Array.Copy(m_vec, 0, match, 0, count);
            Array.Copy(t_vec, 0, targets, 0, count);
        }

        /// <summary>Sort match and targets array with QuickSort.</summary>
        private void Sort(int l, int r)
        {
            var i = l;
            var j = r;
            int h;
            var m = match[(int) ((uint) (l + r) >> 1)];
            InstructionHandle h2;
            do
            {
                while (match[i] < m) i++;
                while (m < match[j]) j--;
                if (i <= j)
                {
                    h = match[i];
                    match[i] = match[j];
                    match[j] = h;
                    // Swap elements
                    h2 = targets[i];
                    targets[i] = targets[j];
                    targets[j] = h2;
                    // Swap instructions, too
                    i++;
                    j--;
                }
            } while (i <= j);

            if (l < j) Sort(l, j);
            if (i < r) Sort(i, r);
        }

        /// <returns>match is sorted in ascending order with no gap bigger than max_gap?</returns>
        private bool MatchIsOrdered(int max_gap)
        {
            for (var i = 1; i < match_length; i++)
                if (match[i] - match[i - 1] > max_gap)
                    return false;
            return true;
        }

        public Instruction GetInstruction()
        {
            return instruction;
        }
    }
}