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

using Apache.NBCEL.Java.IO;
using Apache.NBCEL.Util;

namespace Apache.NBCEL.Generic
{
	/// <summary>TABLESWITCH - Switch within given range of values, i.e., low..high</summary>
	/// <seealso cref="SWITCH" />
	public class TABLESWITCH : Select
    {
	    /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
	    /// <remarks>
	    ///     Empty constructor needed for Instruction.readInstruction.
	    ///     Not to be used otherwise.
	    /// </remarks>
	    internal TABLESWITCH()
        {
        }

	    /// <param name="match">
	    ///     sorted array of match values, match[0] must be low value,
	    ///     match[match_length - 1] high value
	    /// </param>
	    /// <param name="targets">where to branch for matched values</param>
	    /// <param name="defaultTarget">default branch</param>
	    public TABLESWITCH(int[] match, InstructionHandle[] targets, InstructionHandle
            defaultTarget)
            : base(Const.TABLESWITCH, match, targets, defaultTarget)
        {
            /* Alignment remainder assumed 0 here, until dump time */
            var _length = (short) (13 + GetMatch_length() * 4);
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
            var low = _match_length > 0 ? GetMatch(0) : 0;
            @out.WriteInt(low);
            var high = _match_length > 0 ? GetMatch(_match_length - 1) : 0;
            @out.WriteInt(high);
            for (var i = 0; i < _match_length; i++) @out.WriteInt(SetIndices(i, GetTargetOffset(GetTarget(i))));
        }

	    /// <summary>Read needed data (e.g.</summary>
	    /// <remarks>Read needed data (e.g. index) from file.</remarks>
	    /// <exception cref="System.IO.IOException" />
	    protected internal override void InitFromFile(ByteSequence bytes, bool
            wide)
        {
            base.InitFromFile(bytes, wide);
            var low = bytes.ReadInt();
            var high = bytes.ReadInt();
            var _match_length = high - low + 1;
            SetMatch_length(_match_length);
            var _fixed_length = (short) (13 + _match_length * 4);
            SetFixed_length(_fixed_length);
            SetLength((short) (_fixed_length + GetPadding()));
            SetMatches(new int[_match_length]);
            SetIndices(new int[_match_length]);
            SetTargets(new InstructionHandle[_match_length]);
            for (var i = 0; i < _match_length; i++)
            {
                SetMatch(i, low + i);
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
            v.VisitTABLESWITCH(this);
        }
    }
}