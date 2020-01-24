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
	/// Instances of this class give users a handle to the instructions contained in
	/// an InstructionList.
	/// </summary>
	/// <remarks>
	/// Instances of this class give users a handle to the instructions contained in
	/// an InstructionList. Instruction objects may be used more than once within a
	/// list, this is useful because it saves memory and may be much faster.
	/// Within an InstructionList an InstructionHandle object is wrapped
	/// around all instructions, i.e., it implements a cell in a
	/// doubly-linked list. From the outside only the next and the
	/// previous instruction (handle) are accessible. One
	/// can traverse the list via an Enumeration returned by
	/// InstructionList.elements().
	/// </remarks>
	/// <seealso cref="Instruction"/>
	/// <seealso cref="BranchHandle"/>
	/// <seealso cref="InstructionList"/>
	public class InstructionHandle
	{
		private NBCEL.generic.InstructionHandle next;

		private NBCEL.generic.InstructionHandle prev;

		private NBCEL.generic.Instruction instruction;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal int i_position = -1;

		private System.Collections.Generic.HashSet<NBCEL.generic.InstructionTargeter> targeters;

		private System.Collections.Generic.IDictionary<object, object> attributes;

		// byte code offset of instruction
		/// <summary>Does nothing.</summary>
		[System.ObsoleteAttribute(@"Does nothing as of 6.3.1.")]
		protected internal virtual void AddHandle()
		{
		}

		// noop
		public NBCEL.generic.InstructionHandle GetNext()
		{
			return next;
		}

		public NBCEL.generic.InstructionHandle GetPrev()
		{
			return prev;
		}

		public NBCEL.generic.Instruction GetInstruction()
		{
			return instruction;
		}

		/// <summary>Replace current instruction contained in this handle.</summary>
		/// <remarks>
		/// Replace current instruction contained in this handle.
		/// Old instruction is disposed using Instruction.dispose().
		/// </remarks>
		public virtual void SetInstruction(NBCEL.generic.Instruction i)
		{
			// Overridden in BranchHandle TODO could be package-protected?
			if (i == null)
			{
				throw new NBCEL.generic.ClassGenException("Assigning null to handle");
			}
			if ((this.GetType() != typeof(NBCEL.generic.BranchHandle)) && (i is NBCEL.generic.BranchInstruction
				))
			{
				throw new NBCEL.generic.ClassGenException("Assigning branch instruction " + i + " to plain handle"
					);
			}
			if (instruction != null)
			{
				instruction.Dispose();
			}
			instruction = i;
		}

		/// <summary>
		/// Temporarily swap the current instruction, without disturbing
		/// anything.
		/// </summary>
		/// <remarks>
		/// Temporarily swap the current instruction, without disturbing
		/// anything. Meant to be used by a debugger, implementing
		/// breakpoints. Current instruction is returned.
		/// <p>
		/// Warning: if this is used on a BranchHandle then some methods such as
		/// getPosition() will still refer to the original cached instruction, whereas
		/// other BH methods may affect the cache and the replacement instruction.
		/// </remarks>
		public virtual NBCEL.generic.Instruction SwapInstruction(NBCEL.generic.Instruction
			 i)
		{
			// See BCEL-273
			// TODO remove this method in any redesign of BCEL
			NBCEL.generic.Instruction oldInstruction = instruction;
			instruction = i;
			return oldInstruction;
		}

		protected internal InstructionHandle(NBCEL.generic.Instruction i)
		{
			/*private*/
			SetInstruction(i);
		}

		/// <summary>Factory method.</summary>
		internal static NBCEL.generic.InstructionHandle GetInstructionHandle(NBCEL.generic.Instruction
			 i)
		{
			return new NBCEL.generic.InstructionHandle(i);
		}

		/// <summary>
		/// Called by InstructionList.setPositions when setting the position for every
		/// instruction.
		/// </summary>
		/// <remarks>
		/// Called by InstructionList.setPositions when setting the position for every
		/// instruction. In the presence of variable length instructions `setPositions()'
		/// performs multiple passes over the instruction list to calculate the
		/// correct (byte) positions and offsets by calling this function.
		/// </remarks>
		/// <param name="offset">additional offset caused by preceding (variable length) instructions
		/// 	</param>
		/// <param name="max_offset">the maximum offset that may be caused by these instructions
		/// 	</param>
		/// <returns>additional offset caused by possible change of this instruction's length
		/// 	</returns>
		protected internal virtual int UpdatePosition(int offset, int max_offset)
		{
			i_position += offset;
			return 0;
		}

		/// <returns>
		/// the position, i.e., the byte code offset of the contained
		/// instruction. This is accurate only after
		/// InstructionList.setPositions() has been called.
		/// </returns>
		public virtual int GetPosition()
		{
			return i_position;
		}

		/// <summary>
		/// Set the position, i.e., the byte code offset of the contained
		/// instruction.
		/// </summary>
		internal virtual void SetPosition(int pos)
		{
			i_position = pos;
		}

		/// <summary>Delete contents, i.e., remove user access.</summary>
		internal virtual void Dispose()
		{
			next = prev = null;
			instruction.Dispose();
			instruction = null;
			i_position = -1;
			attributes = null;
			RemoveAllTargeters();
		}

		/// <summary>Remove all targeters, if any.</summary>
		public virtual void RemoveAllTargeters()
		{
			if (targeters != null)
			{
				targeters.Clear();
			}
		}

		/// <summary>Denote this handle isn't referenced anymore by t.</summary>
		public virtual void RemoveTargeter(NBCEL.generic.InstructionTargeter t)
		{
			if (targeters != null)
			{
				targeters.Remove(t);
			}
		}

		/// <summary>Denote this handle is being referenced by t.</summary>
		public virtual void AddTargeter(NBCEL.generic.InstructionTargeter t)
		{
			if (targeters == null)
			{
				targeters = new System.Collections.Generic.HashSet<NBCEL.generic.InstructionTargeter
					>();
			}
			//if(!targeters.contains(t))
			targeters.Add(t);
		}

		public virtual bool HasTargeters()
		{
			return (targeters != null) && (targeters.Count > 0);
		}

		/// <returns>null, if there are no targeters</returns>
		public virtual NBCEL.generic.InstructionTargeter[] GetTargeters()
		{
			if (!HasTargeters())
			{
				return new NBCEL.generic.InstructionTargeter[0];
			}
			NBCEL.generic.InstructionTargeter[] t = new NBCEL.generic.InstructionTargeter[targeters
				.Count];
			Sharpen.Collections.ToArray(targeters, t);
			return t;
		}

		/// <returns>a (verbose) string representation of the contained instruction.</returns>
		public virtual string ToString(bool verbose)
		{
			return NBCEL.classfile.Utility.Format(i_position, 4, false, ' ') + ": " + instruction
				.ToString(verbose);
		}

		/// <returns>a string representation of the contained instruction.</returns>
		public override string ToString()
		{
			return ToString(true);
		}

		/// <summary>Add an attribute to an instruction handle.</summary>
		/// <param name="key">the key object to store/retrieve the attribute</param>
		/// <param name="attr">the attribute to associate with this handle</param>
		public virtual void AddAttribute(object key, object attr)
		{
			if (attributes == null)
			{
				attributes = new System.Collections.Generic.Dictionary<object, object>(3);
			}
			Sharpen.Collections.Put(attributes, key, attr);
		}

		/// <summary>Delete an attribute of an instruction handle.</summary>
		/// <param name="key">the key object to retrieve the attribute</param>
		public virtual void RemoveAttribute(object key)
		{
			if (attributes != null)
			{
				Sharpen.Collections.Remove(attributes, key);
			}
		}

		/// <summary>Get attribute of an instruction handle.</summary>
		/// <param name="key">the key object to store/retrieve the attribute</param>
		public virtual object GetAttribute(object key)
		{
			if (attributes != null)
			{
				return attributes.GetOrNull(key);
			}
			return null;
		}

		/// <returns>all attributes associated with this handle</returns>
		public virtual System.Collections.Generic.ICollection<object> GetAttributes()
		{
			if (attributes == null)
			{
				attributes = new System.Collections.Generic.Dictionary<object, object>(3);
			}
			return attributes.Values;
		}

		/// <summary>Convenience method, simply calls accept() on the contained instruction.</summary>
		/// <param name="v">Visitor object</param>
		public virtual void Accept(NBCEL.generic.Visitor v)
		{
			instruction.Accept(v);
		}

		/// <param name="next">the next to set</param>
		/// <>since 6.0</>
		internal NBCEL.generic.InstructionHandle SetNext(NBCEL.generic.InstructionHandle 
			next)
		{
			this.next = next;
			return next;
		}

		/// <param name="prev">the prev to set</param>
		/// <>since 6.0</>
		internal NBCEL.generic.InstructionHandle SetPrev(NBCEL.generic.InstructionHandle 
			prev)
		{
			this.prev = prev;
			return prev;
		}
	}
}
