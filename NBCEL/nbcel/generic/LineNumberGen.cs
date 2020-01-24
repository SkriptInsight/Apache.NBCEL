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
	/// <summary>
	/// This class represents a line number within a method, i.e., give an instruction
	/// a line number corresponding to the source code line.
	/// </summary>
	/// <seealso cref="NBCEL.classfile.LineNumber"/>
	/// <seealso cref="MethodGen"/>
	public class LineNumberGen : NBCEL.generic.InstructionTargeter, System.ICloneable
	{
		private NBCEL.generic.InstructionHandle ih;

		private int src_line;

		/// <summary>Create a line number.</summary>
		/// <param name="ih">instruction handle to reference</param>
		public LineNumberGen(NBCEL.generic.InstructionHandle ih, int src_line)
		{
			SetInstruction(ih);
			SetSourceLine(src_line);
		}

		/// <returns>true, if ih is target of this line number</returns>
		public virtual bool ContainsTarget(NBCEL.generic.InstructionHandle ih)
		{
			return this.ih == ih;
		}

		/// <param name="old_ih">old target</param>
		/// <param name="new_ih">new target</param>
		public virtual void UpdateTarget(NBCEL.generic.InstructionHandle old_ih, NBCEL.generic.InstructionHandle
			 new_ih)
		{
			if (old_ih != ih)
			{
				throw new NBCEL.generic.ClassGenException("Not targeting " + old_ih + ", but " + 
					ih + "}");
			}
			SetInstruction(new_ih);
		}

		/// <summary>Get LineNumber attribute .</summary>
		/// <remarks>
		/// Get LineNumber attribute .
		/// This relies on that the instruction list has already been dumped to byte code or
		/// or that the `setPositions' methods has been called for the instruction list.
		/// </remarks>
		public virtual NBCEL.classfile.LineNumber GetLineNumber()
		{
			return new NBCEL.classfile.LineNumber(ih.GetPosition(), src_line);
		}

		public virtual void SetInstruction(NBCEL.generic.InstructionHandle ih)
		{
			// TODO could be package-protected?
			if (ih == null)
			{
				throw new System.ArgumentNullException("InstructionHandle may not be null");
			}
			NBCEL.generic.BranchInstruction.NotifyTarget(this.ih, ih, this);
			this.ih = ih;
		}

		public virtual object Clone()
		{
			return base.MemberwiseClone();
		}

		// never happens
		public virtual NBCEL.generic.InstructionHandle GetInstruction()
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

		object System.ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
