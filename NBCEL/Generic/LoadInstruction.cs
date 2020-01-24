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
	///     Denotes an unparameterized instruction to load a value from a local
	///     variable, e.g.
	/// </summary>
	/// <remarks>
	///     Denotes an unparameterized instruction to load a value from a local
	///     variable, e.g. ILOAD.
	/// </remarks>
	public abstract class LoadInstruction : LocalVariableInstruction, PushInstruction
    {
	    /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
	    /// <remarks>
	    ///     Empty constructor needed for Instruction.readInstruction.
	    ///     Not to be used otherwise.
	    ///     tag and length are defined in readInstruction and initFromFile, respectively.
	    /// </remarks>
	    internal LoadInstruction(short canon_tag, short c_tag)
            : base(canon_tag, c_tag)
        {
        }

	    /// <param name="opcode">Instruction opcode</param>
	    /// <param name="c_tag">Instruction number for compact version, ALOAD_0, e.g.</param>
	    /// <param name="n">local variable index (unsigned short)</param>
	    protected internal LoadInstruction(short opcode, short c_tag, int n)
            : base(opcode, c_tag, n)
        {
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
            v.VisitStackProducer(this);
            v.VisitPushInstruction(this);
            v.VisitTypedInstruction(this);
            v.VisitLocalVariableInstruction(this);
            v.VisitLoadInstruction(this);
        }
    }
}