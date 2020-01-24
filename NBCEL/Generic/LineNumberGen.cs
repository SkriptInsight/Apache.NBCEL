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
using Apache.NBCEL.ClassFile;

namespace Apache.NBCEL.Generic
{
	/// <summary>
	///     This class represents a line number within a method, i.e., give an instruction
	///     a line number corresponding to the source code line.
	/// </summary>
	/// <seealso cref="LineNumber" />
	/// <seealso cref="MethodGen" />
	public class LineNumberGen : InstructionTargeter, ICloneable
    {
        private InstructionHandle ih;

        private int src_line;

        /// <summary>Create a line number.</summary>
        /// <param name="ih">instruction handle to reference</param>
        public LineNumberGen(InstructionHandle ih, int src_line)
        {
            SetInstruction(ih);
            SetSourceLine(src_line);
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        /// <returns>true, if ih is target of this line number</returns>
        public virtual bool ContainsTarget(InstructionHandle ih)
        {
            return this.ih == ih;
        }

        /// <param name="old_ih">old target</param>
        /// <param name="new_ih">new target</param>
        public virtual void UpdateTarget(InstructionHandle old_ih, InstructionHandle
            new_ih)
        {
            if (old_ih != ih)
                throw new ClassGenException("Not targeting " + old_ih + ", but " +
                                            ih + "}");
            SetInstruction(new_ih);
        }

        /// <summary>Get LineNumber attribute .</summary>
        /// <remarks>
        ///     Get LineNumber attribute .
        ///     This relies on that the instruction list has already been dumped to byte code or
        ///     or that the `setPositions' methods has been called for the instruction list.
        /// </remarks>
        public virtual LineNumber GetLineNumber()
        {
            return new LineNumber(ih.GetPosition(), src_line);
        }

        public virtual void SetInstruction(InstructionHandle ih)
        {
            // TODO could be package-protected?
            if (ih == null) throw new ArgumentNullException("InstructionHandle may not be null");
            BranchInstruction.NotifyTarget(this.ih, ih, this);
            this.ih = ih;
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        // never happens
        public virtual InstructionHandle GetInstruction()
        {
            return ih;
        }

        public virtual void SetSourceLine(int src_line)
        {
            // TODO could be package-protected?
            this.src_line = src_line;
        }

        public virtual int GetSourceLine()
        {
            return src_line;
        }
    }
}