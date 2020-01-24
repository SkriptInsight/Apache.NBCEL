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

using java.io;
using NBCEL.util;
using Sharpen;

namespace NBCEL.generic
{
	/// <summary>LOOKUPSWITCH - Switch with unordered set of values</summary>
	/// <seealso cref="SWITCH" />
	public class LOOKUPSWITCH : Select
    {
	    /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
	    /// <remarks>
	    ///     Empty constructor needed for Instruction.readInstruction.
	    ///     Not to be used otherwise.
	    /// </remarks>
	    internal LOOKUPSWITCH()
        {
        }

        public LOOKUPSWITCH(int[] match, InstructionHandle[] targets, InstructionHandle
            defaultTarget)
            : base(Const.LOOKUPSWITCH, match, targets, defaultTarget)
        {
            /* alignment remainder assumed 0 here, until dump time. */
            var _length = (short) (9 + GetMatch_length() * 8);
            SetLength(_length);
            SetFixed_length(_length);
        }

        /// <summary>Dump instruction as byte code to stream out.</summary>
        /// <param name="out">Output stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream @out)
        {
            base.Dump(@out);
            var _match_length = GetMatch_length();
            @out.WriteInt(_match_length);
            // npairs
            for (var i = 0; i < _match_length; i++)
            {
                @out.WriteInt(GetMatch(i));
                // match-offset pairs
                @out.WriteInt(SetIndices(i, GetTargetOffset(GetTarget(i))));
            }
        }

        /// <summary>Read needed data (e.g.</summary>
        /// <remarks>Read needed data (e.g. index) from file.</remarks>
        /// <exception cref="System.IO.IOException" />
        protected internal override void InitFromFile(ByteSequence bytes, bool
            wide)
        {
            base.InitFromFile(bytes, wide);
            // reads padding
            var _match_length = bytes.ReadInt();
            SetMatch_length(_match_length);
            var _fixed_length = (short) (9 + _match_length * 8);
            SetFixed_length(_fixed_length);
            var _length = (short) (_match_length + GetPadding());
            SetLength(_length);
            SetMatches(new int[_match_length]);
            SetIndices(new int[_match_length]);
            SetTargets(new InstructionHandle[_match_length]);
            for (var i = 0; i < _match_length; i++)
            {
                SetMatch(i, bytes.ReadInt());
                SetIndices(i, bytes.ReadInt());
            }
        }

        /// <summary>Call corresponding visitor method(s).</summary>
        /// <remarks>
        ///     Call corresponding visitor method(s). The order is:
        ///     Call visitor methods of implemented interfaces first, then
        ///     call methods according to the class hierarchy in descending order,
        ///     i.e., the most specific visitXXX() call comes last.
        /// </remarks>
        /// <param name="v">Visitor object</param>
        public override void Accept(Visitor v)
        {
            v.VisitVariableLengthInstruction(this);
            v.VisitStackConsumer(this);
            v.VisitBranchInstruction(this);
            v.VisitSelect(this);
            v.VisitLOOKUPSWITCH(this);
        }
    }
}