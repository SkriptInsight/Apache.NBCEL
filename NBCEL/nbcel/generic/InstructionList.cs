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
using System.Collections;
using ObjectWeb.Misc.Java.Nio;
using Sharpen;

namespace NBCEL.generic
{
	/// <summary>This class is a container for a list of <a href="Instruction.html">Instruction</a> objects.
	/// 	</summary>
	/// <remarks>
	/// This class is a container for a list of <a href="Instruction.html">Instruction</a> objects. Instructions can be appended, inserted, moved, deleted, etc..
	/// Instructions are being wrapped into <a href="InstructionHandle.html">InstructionHandles</a> objects that are returned upon append/insert operations. They
	/// give the user (read only) access to the list structure, such that it can be traversed and manipulated in a controlled way.
	/// A list is finally dumped to a byte code array with <a href="#getByteCode()">getByteCode</a>.
	/// </remarks>
	/// <seealso cref="Instruction"/>
	/// <seealso cref="InstructionHandle"/>
	/// <seealso cref="BranchHandle"/>
	public class InstructionList : System.Collections.Generic.IEnumerable<NBCEL.generic.InstructionHandle
		>
	{
		private NBCEL.generic.InstructionHandle start = null;

		private NBCEL.generic.InstructionHandle end = null;

		private int length = 0;

		private int[] byte_positions;

		/// <summary>Create (empty) instruction list.</summary>
		public InstructionList()
		{
		}

		/// <summary>Create instruction list containing one instruction.</summary>
		/// <param name="i">initial instruction</param>
		public InstructionList(NBCEL.generic.Instruction i)
		{
			// number of elements in list
			// byte code offsets corresponding to instructions
			Append(i);
		}

		/// <summary>Create instruction list containing one instruction.</summary>
		/// <param name="i">initial instruction</param>
		public InstructionList(NBCEL.generic.BranchInstruction i)
		{
			Append(i);
		}

		/// <summary>Initialize list with (nonnull) compound instruction.</summary>
		/// <remarks>Initialize list with (nonnull) compound instruction. Consumes argument list, i.e., it becomes empty.
		/// 	</remarks>
		/// <param name="c">compound instruction (list)</param>
		public InstructionList(NBCEL.generic.CompoundInstruction c)
		{
			Append(c.GetInstructionList());
		}

		/// <summary>Test for empty list.</summary>
		public virtual bool IsEmpty()
		{
			return start == null;
		}

		// && end == null
		/// <summary>Find the target instruction (handle) that corresponds to the given target position (byte code offset).
		/// 	</summary>
		/// <param name="ihs">array of instruction handles, i.e. il.getInstructionHandles()</param>
		/// <param name="pos">array of positions corresponding to ihs, i.e. il.getInstructionPositions()
		/// 	</param>
		/// <param name="count">length of arrays</param>
		/// <param name="target">target position to search for</param>
		/// <returns>target position's instruction handle if available</returns>
		public static NBCEL.generic.InstructionHandle FindHandle(NBCEL.generic.InstructionHandle
			[] ihs, int[] pos, int count, int target)
		{
			int l = 0;
			int r = count - 1;
			do
			{
				/*
				* Do a binary search since the pos array is orderd.
				*/
				int i = (int)(((uint)(l + r)) >> 1);
				int j = pos[i];
				if (j == target)
				{
					return ihs[i];
				}
				else if (target < j)
				{
					r = i - 1;
				}
				else
				{
					l = i + 1;
				}
			}
			while (l <= r);
			return null;
		}

		/// <summary>Get instruction handle for instruction at byte code position pos.</summary>
		/// <remarks>
		/// Get instruction handle for instruction at byte code position pos. This only works properly, if the list is freshly initialized from a byte array or
		/// setPositions() has been called before this method.
		/// </remarks>
		/// <param name="pos">byte code position to search for</param>
		/// <returns>target position's instruction handle if available</returns>
		public virtual NBCEL.generic.InstructionHandle FindHandle(int pos)
		{
			int[] positions = byte_positions;
			NBCEL.generic.InstructionHandle ih = start;
			for (int i = 0; i < length; i++)
			{
				if (positions[i] == pos)
				{
					return ih;
				}
				ih = ih.GetNext();
			}
			return null;
		}

		/// <summary>Initialize instruction list from byte array.</summary>
		/// <param name="code">byte array containing the instructions</param>
		public InstructionList(byte[] code)
		{
			int count = 0;
			// Contains actual length
			int[] pos;
			NBCEL.generic.InstructionHandle[] ihs;
			try
			{
				using (NBCEL.util.ByteSequence bytes = new NBCEL.util.ByteSequence(code))
				{
					ihs = new NBCEL.generic.InstructionHandle[code.Length];
					pos = new int[code.Length];
					// Can't be more than that
					/*
					* Pass 1: Create an object for each byte code and append them to the list.
					*/
					while (bytes.Available() > 0)
					{
						// Remember byte offset and associate it with the instruction
						int off = bytes.GetIndex();
						pos[count] = off;
						/*
						* Read one instruction from the byte stream, the byte position is set accordingly.
						*/
						NBCEL.generic.Instruction i = NBCEL.generic.Instruction.ReadInstruction(bytes);
						NBCEL.generic.InstructionHandle ih;
						if (i is NBCEL.generic.BranchInstruction)
						{
							ih = Append((NBCEL.generic.BranchInstruction)i);
						}
						else
						{
							ih = Append(i);
						}
						ih.SetPosition(off);
						ihs[count] = ih;
						count++;
					}
				}
			}
			catch (System.IO.IOException e)
			{
				throw new NBCEL.generic.ClassGenException(e.ToString(), e);
			}
			byte_positions = new int[count];
			// Trim to proper size
			System.Array.Copy(pos, 0, byte_positions, 0, count);
			/*
			* Pass 2: Look for BranchInstruction and update their targets, i.e., convert offsets to instruction handles.
			*/
			for (int i = 0; i < count; i++)
			{
				if (ihs[i] is NBCEL.generic.BranchHandle)
				{
					NBCEL.generic.BranchInstruction bi = (NBCEL.generic.BranchInstruction)ihs[i].GetInstruction
						();
					int target = bi.GetPosition() + bi.GetIndex();
					/*
					* Byte code position: relative -> absolute.
					*/
					// Search for target position
					NBCEL.generic.InstructionHandle ih = FindHandle(ihs, pos, count, target);
					if (ih == null)
					{
						throw new NBCEL.generic.ClassGenException("Couldn't find target for branch: " + bi
							);
					}
					bi.SetTarget(ih);
					// Update target
					// If it is a Select instruction, update all branch targets
					if (bi is NBCEL.generic.Select)
					{
						// Either LOOKUPSWITCH or TABLESWITCH
						NBCEL.generic.Select s = (NBCEL.generic.Select)bi;
						int[] indices = s.GetIndices();
						for (int j = 0; j < indices.Length; j++)
						{
							target = bi.GetPosition() + indices[j];
							ih = FindHandle(ihs, pos, count, target);
							if (ih == null)
							{
								throw new NBCEL.generic.ClassGenException("Couldn't find target for switch: " + bi
									);
							}
							s.SetTarget(j, ih);
						}
					}
				}
			}
		}

		// Update target
		/// <summary>Append another list after instruction (handle) ih contained in this list.
		/// 	</summary>
		/// <remarks>Append another list after instruction (handle) ih contained in this list. Consumes argument list, i.e., it becomes empty.
		/// 	</remarks>
		/// <param name="ih">where to append the instruction list</param>
		/// <param name="il">Instruction list to append to this one</param>
		/// <returns>instruction handle pointing to the <B>first</B> appended instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Append(NBCEL.generic.InstructionHandle
			 ih, NBCEL.generic.InstructionList il)
		{
			if (il == null)
			{
				throw new NBCEL.generic.ClassGenException("Appending null InstructionList");
			}
			if (il.IsEmpty())
			{
				return ih;
			}
			NBCEL.generic.InstructionHandle next = ih.GetNext();
			NBCEL.generic.InstructionHandle ret = il.start;
			ih.SetNext(il.start);
			il.start.SetPrev(ih);
			il.end.SetNext(next);
			if (next != null)
			{
				next.SetPrev(il.end);
			}
			else
			{
				end = il.end;
			}
			// Update end ...
			length += il.length;
			// Update length
			il.Clear();
			return ret;
		}

		/// <summary>Append another list after instruction i contained in this list.</summary>
		/// <remarks>Append another list after instruction i contained in this list. Consumes argument list, i.e., it becomes empty.
		/// 	</remarks>
		/// <param name="i">where to append the instruction list</param>
		/// <param name="il">Instruction list to append to this one</param>
		/// <returns>instruction handle pointing to the <B>first</B> appended instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Append(NBCEL.generic.Instruction i
			, NBCEL.generic.InstructionList il)
		{
			NBCEL.generic.InstructionHandle ih;
			if ((ih = FindInstruction2(i)) == null)
			{
				throw new NBCEL.generic.ClassGenException("Instruction " + i + " is not contained in this list."
					);
			}
			return Append(ih, il);
		}

		/// <summary>Append another list to this one.</summary>
		/// <remarks>Append another list to this one. Consumes argument list, i.e., it becomes empty.
		/// 	</remarks>
		/// <param name="il">list to append to end of this list</param>
		/// <returns>instruction handle of the <B>first</B> appended instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Append(NBCEL.generic.InstructionList
			 il)
		{
			if (il == null)
			{
				throw new NBCEL.generic.ClassGenException("Appending null InstructionList");
			}
			if (il.IsEmpty())
			{
				return null;
			}
			if (IsEmpty())
			{
				start = il.start;
				end = il.end;
				length = il.length;
				il.Clear();
				return start;
			}
			return Append(end, il);
		}

		// was end.instruction
		/// <summary>Append an instruction to the end of this list.</summary>
		/// <param name="ih">instruction to append</param>
		private void Append(NBCEL.generic.InstructionHandle ih)
		{
			if (IsEmpty())
			{
				start = end = ih;
				ih.SetNext(ih.SetPrev(null));
			}
			else
			{
				end.SetNext(ih);
				ih.SetPrev(end);
				ih.SetNext(null);
				end = ih;
			}
			length++;
		}

		// Update length
		/// <summary>Append an instruction to the end of this list.</summary>
		/// <param name="i">instruction to append</param>
		/// <returns>instruction handle of the appended instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Append(NBCEL.generic.Instruction i
			)
		{
			NBCEL.generic.InstructionHandle ih = NBCEL.generic.InstructionHandle.GetInstructionHandle
				(i);
			Append(ih);
			return ih;
		}

		/// <summary>Append a branch instruction to the end of this list.</summary>
		/// <param name="i">branch instruction to append</param>
		/// <returns>branch instruction handle of the appended instruction</returns>
		public virtual NBCEL.generic.BranchHandle Append(NBCEL.generic.BranchInstruction 
			i)
		{
			NBCEL.generic.BranchHandle ih = NBCEL.generic.BranchHandle.GetBranchHandle(i);
			Append(ih);
			return ih;
		}

		/// <summary>Append a single instruction j after another instruction i, which must be in this list of course!
		/// 	</summary>
		/// <param name="i">Instruction in list</param>
		/// <param name="j">Instruction to append after i in list</param>
		/// <returns>instruction handle of the first appended instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Append(NBCEL.generic.Instruction i
			, NBCEL.generic.Instruction j)
		{
			return Append(i, new NBCEL.generic.InstructionList(j));
		}

		/// <summary>Append a compound instruction, after instruction i.</summary>
		/// <param name="i">Instruction in list</param>
		/// <param name="c">The composite instruction (containing an InstructionList)</param>
		/// <returns>instruction handle of the first appended instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Append(NBCEL.generic.Instruction i
			, NBCEL.generic.CompoundInstruction c)
		{
			return Append(i, c.GetInstructionList());
		}

		/// <summary>Append a compound instruction.</summary>
		/// <param name="c">The composite instruction (containing an InstructionList)</param>
		/// <returns>instruction handle of the first appended instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Append(NBCEL.generic.CompoundInstruction
			 c)
		{
			return Append(c.GetInstructionList());
		}

		/// <summary>Append a compound instruction.</summary>
		/// <param name="ih">where to append the instruction list</param>
		/// <param name="c">The composite instruction (containing an InstructionList)</param>
		/// <returns>instruction handle of the first appended instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Append(NBCEL.generic.InstructionHandle
			 ih, NBCEL.generic.CompoundInstruction c)
		{
			return Append(ih, c.GetInstructionList());
		}

		/// <summary>Append an instruction after instruction (handle) ih contained in this list.
		/// 	</summary>
		/// <param name="ih">where to append the instruction list</param>
		/// <param name="i">Instruction to append</param>
		/// <returns>instruction handle pointing to the <B>first</B> appended instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Append(NBCEL.generic.InstructionHandle
			 ih, NBCEL.generic.Instruction i)
		{
			return Append(ih, new NBCEL.generic.InstructionList(i));
		}

		/// <summary>Append an instruction after instruction (handle) ih contained in this list.
		/// 	</summary>
		/// <param name="ih">where to append the instruction list</param>
		/// <param name="i">Instruction to append</param>
		/// <returns>instruction handle pointing to the <B>first</B> appended instruction</returns>
		public virtual NBCEL.generic.BranchHandle Append(NBCEL.generic.InstructionHandle 
			ih, NBCEL.generic.BranchInstruction i)
		{
			NBCEL.generic.BranchHandle bh = NBCEL.generic.BranchHandle.GetBranchHandle(i);
			NBCEL.generic.InstructionList il = new NBCEL.generic.InstructionList();
			il.Append(bh);
			Append(ih, il);
			return bh;
		}

		/// <summary>Insert another list before Instruction handle ih contained in this list.
		/// 	</summary>
		/// <remarks>Insert another list before Instruction handle ih contained in this list. Consumes argument list, i.e., it becomes empty.
		/// 	</remarks>
		/// <param name="ih">where to append the instruction list</param>
		/// <param name="il">Instruction list to insert</param>
		/// <returns>instruction handle of the first inserted instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Insert(NBCEL.generic.InstructionHandle
			 ih, NBCEL.generic.InstructionList il)
		{
			if (il == null)
			{
				throw new NBCEL.generic.ClassGenException("Inserting null InstructionList");
			}
			if (il.IsEmpty())
			{
				return ih;
			}
			NBCEL.generic.InstructionHandle prev = ih.GetPrev();
			NBCEL.generic.InstructionHandle ret = il.start;
			ih.SetPrev(il.end);
			il.end.SetNext(ih);
			il.start.SetPrev(prev);
			if (prev != null)
			{
				prev.SetNext(il.start);
			}
			else
			{
				start = il.start;
			}
			// Update start ...
			length += il.length;
			// Update length
			il.Clear();
			return ret;
		}

		/// <summary>Insert another list.</summary>
		/// <param name="il">list to insert before start of this list</param>
		/// <returns>instruction handle of the first inserted instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Insert(NBCEL.generic.InstructionList
			 il)
		{
			if (IsEmpty())
			{
				Append(il);
				// Code is identical for this case
				return start;
			}
			return Insert(start, il);
		}

		/// <summary>Insert an instruction at start of this list.</summary>
		/// <param name="ih">instruction to insert</param>
		private void Insert(NBCEL.generic.InstructionHandle ih)
		{
			if (IsEmpty())
			{
				start = end = ih;
				ih.SetNext(ih.SetPrev(null));
			}
			else
			{
				start.SetPrev(ih);
				ih.SetNext(start);
				ih.SetPrev(null);
				start = ih;
			}
			length++;
		}

		/// <summary>Insert another list before Instruction i contained in this list.</summary>
		/// <remarks>Insert another list before Instruction i contained in this list. Consumes argument list, i.e., it becomes empty.
		/// 	</remarks>
		/// <param name="i">where to append the instruction list</param>
		/// <param name="il">Instruction list to insert</param>
		/// <returns>instruction handle pointing to the first inserted instruction, i.e., il.getStart()
		/// 	</returns>
		public virtual NBCEL.generic.InstructionHandle Insert(NBCEL.generic.Instruction i
			, NBCEL.generic.InstructionList il)
		{
			NBCEL.generic.InstructionHandle ih;
			if ((ih = FindInstruction1(i)) == null)
			{
				throw new NBCEL.generic.ClassGenException("Instruction " + i + " is not contained in this list."
					);
			}
			return Insert(ih, il);
		}

		/// <summary>Insert an instruction at start of this list.</summary>
		/// <param name="i">instruction to insert</param>
		/// <returns>instruction handle of the inserted instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Insert(NBCEL.generic.Instruction i
			)
		{
			NBCEL.generic.InstructionHandle ih = NBCEL.generic.InstructionHandle.GetInstructionHandle
				(i);
			Insert(ih);
			return ih;
		}

		/// <summary>Insert a branch instruction at start of this list.</summary>
		/// <param name="i">branch instruction to insert</param>
		/// <returns>branch instruction handle of the appended instruction</returns>
		public virtual NBCEL.generic.BranchHandle Insert(NBCEL.generic.BranchInstruction 
			i)
		{
			NBCEL.generic.BranchHandle ih = NBCEL.generic.BranchHandle.GetBranchHandle(i);
			Insert(ih);
			return ih;
		}

		/// <summary>Insert a single instruction j before another instruction i, which must be in this list of course!
		/// 	</summary>
		/// <param name="i">Instruction in list</param>
		/// <param name="j">Instruction to insert before i in list</param>
		/// <returns>instruction handle of the first inserted instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Insert(NBCEL.generic.Instruction i
			, NBCEL.generic.Instruction j)
		{
			return Insert(i, new NBCEL.generic.InstructionList(j));
		}

		/// <summary>Insert a compound instruction before instruction i.</summary>
		/// <param name="i">Instruction in list</param>
		/// <param name="c">The composite instruction (containing an InstructionList)</param>
		/// <returns>instruction handle of the first inserted instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Insert(NBCEL.generic.Instruction i
			, NBCEL.generic.CompoundInstruction c)
		{
			return Insert(i, c.GetInstructionList());
		}

		/// <summary>Insert a compound instruction.</summary>
		/// <param name="c">The composite instruction (containing an InstructionList)</param>
		/// <returns>instruction handle of the first inserted instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Insert(NBCEL.generic.CompoundInstruction
			 c)
		{
			return Insert(c.GetInstructionList());
		}

		/// <summary>Insert an instruction before instruction (handle) ih contained in this list.
		/// 	</summary>
		/// <param name="ih">where to insert to the instruction list</param>
		/// <param name="i">Instruction to insert</param>
		/// <returns>instruction handle of the first inserted instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Insert(NBCEL.generic.InstructionHandle
			 ih, NBCEL.generic.Instruction i)
		{
			return Insert(ih, new NBCEL.generic.InstructionList(i));
		}

		/// <summary>Insert a compound instruction.</summary>
		/// <param name="ih">where to insert the instruction list</param>
		/// <param name="c">The composite instruction (containing an InstructionList)</param>
		/// <returns>instruction handle of the first inserted instruction</returns>
		public virtual NBCEL.generic.InstructionHandle Insert(NBCEL.generic.InstructionHandle
			 ih, NBCEL.generic.CompoundInstruction c)
		{
			return Insert(ih, c.GetInstructionList());
		}

		/// <summary>Insert an instruction before instruction (handle) ih contained in this list.
		/// 	</summary>
		/// <param name="ih">where to insert to the instruction list</param>
		/// <param name="i">Instruction to insert</param>
		/// <returns>instruction handle of the first inserted instruction</returns>
		public virtual NBCEL.generic.BranchHandle Insert(NBCEL.generic.InstructionHandle 
			ih, NBCEL.generic.BranchInstruction i)
		{
			NBCEL.generic.BranchHandle bh = NBCEL.generic.BranchHandle.GetBranchHandle(i);
			NBCEL.generic.InstructionList il = new NBCEL.generic.InstructionList();
			il.Append(bh);
			Insert(ih, il);
			return bh;
		}

		/// <summary>Take all instructions (handles) from "start" to "end" and append them after the new location "target".
		/// 	</summary>
		/// <remarks>
		/// Take all instructions (handles) from "start" to "end" and append them after the new location "target". Of course, "end" must be after "start" and target
		/// must not be located withing this range. If you want to move something to the start of the list use null as value for target.
		/// <p>
		/// Any instruction targeters pointing to handles within the block, keep their targets.
		/// </p>
		/// </remarks>
		/// <param name="start">of moved block</param>
		/// <param name="end">of moved block</param>
		/// <param name="target">of moved block</param>
		public virtual void Move(NBCEL.generic.InstructionHandle start, NBCEL.generic.InstructionHandle
			 end, NBCEL.generic.InstructionHandle target)
		{
			// Step 1: Check constraints
			if ((start == null) || (end == null))
			{
				throw new NBCEL.generic.ClassGenException("Invalid null handle: From " + start + 
					" to " + end);
			}
			if ((target == start) || (target == end))
			{
				throw new NBCEL.generic.ClassGenException("Invalid range: From " + start + " to "
					 + end + " contains target " + target);
			}
			for (NBCEL.generic.InstructionHandle ih = start; ih != end.GetNext(); ih = ih.GetNext
				())
			{
				if (ih == null)
				{
					throw new NBCEL.generic.ClassGenException("Invalid range: From " + start + " to "
						 + end);
				}
				else if (ih == target)
				{
					throw new NBCEL.generic.ClassGenException("Invalid range: From " + start + " to "
						 + end + " contains target " + target);
				}
			}
			// Step 2: Temporarily remove the given instructions from the list
			NBCEL.generic.InstructionHandle prev = start.GetPrev();
			NBCEL.generic.InstructionHandle next = end.GetNext();
			if (prev != null)
			{
				prev.SetNext(next);
			}
			else
			{
				this.start = next;
			}
			if (next != null)
			{
				next.SetPrev(prev);
			}
			else
			{
				this.end = prev;
			}
			start.SetPrev(end.SetNext(null));
			// Step 3: append after target
			if (target == null)
			{
				// append to start of list
				if (this.start != null)
				{
					this.start.SetPrev(end);
				}
				end.SetNext(this.start);
				this.start = start;
			}
			else
			{
				next = target.GetNext();
				target.SetNext(start);
				start.SetPrev(target);
				end.SetNext(next);
				if (next != null)
				{
					next.SetPrev(end);
				}
				else
				{
					this.end = end;
				}
			}
		}

		/// <summary>Move a single instruction (handle) to a new location.</summary>
		/// <param name="ih">moved instruction</param>
		/// <param name="target">new location of moved instruction</param>
		public virtual void Move(NBCEL.generic.InstructionHandle ih, NBCEL.generic.InstructionHandle
			 target)
		{
			Move(ih, ih, target);
		}

		/// <summary>Remove from instruction `prev' to instruction `next' both contained in this list.
		/// 	</summary>
		/// <remarks>
		/// Remove from instruction `prev' to instruction `next' both contained in this list. Throws TargetLostException when one of the removed instruction handles
		/// is still being targeted.
		/// </remarks>
		/// <param name="prev">where to start deleting (predecessor, exclusive)</param>
		/// <param name="next">where to end deleting (successor, exclusive)</param>
		/// <exception cref="NBCEL.generic.TargetLostException"/>
		private void Remove(NBCEL.generic.InstructionHandle prev, NBCEL.generic.InstructionHandle
			 next)
		{
			NBCEL.generic.InstructionHandle first;
			NBCEL.generic.InstructionHandle last;
			// First and last deleted instruction
			if ((prev == null) && (next == null))
			{
				first = start;
				last = end;
				start = end = null;
			}
			else
			{
				if (prev == null)
				{
					// At start of list
					first = start;
					start = next;
				}
				else
				{
					first = prev.GetNext();
					prev.SetNext(next);
				}
				if (next == null)
				{
					// At end of list
					last = end;
					end = prev;
				}
				else
				{
					last = next.GetPrev();
					next.SetPrev(prev);
				}
			}
			first.SetPrev(null);
			// Completely separated from rest of list
			last.SetNext(null);
			System.Collections.Generic.List<NBCEL.generic.InstructionHandle> target_vec = new 
				System.Collections.Generic.List<NBCEL.generic.InstructionHandle>();
			for (NBCEL.generic.InstructionHandle ih = first; ih != null; ih = ih.GetNext())
			{
				ih.GetInstruction().Dispose();
			}
			// e.g. BranchInstructions release their targets
			System.Text.StringBuilder buf = new System.Text.StringBuilder("{ ");
			for (NBCEL.generic.InstructionHandle ih = first; ih != null; ih = next)
			{
				next = ih.GetNext();
				length--;
				if (ih.HasTargeters())
				{
					// Still got targeters?
					target_vec.Add(ih);
					buf.Append(ih.ToString(true)).Append(" ");
					ih.SetNext(ih.SetPrev(null));
				}
				else
				{
					ih.Dispose();
				}
			}
			buf.Append("}");
			if (!(target_vec.Count == 0))
			{
				NBCEL.generic.InstructionHandle[] targeted = new NBCEL.generic.InstructionHandle[
					target_vec.Count];
				Sharpen.Collections.ToArray(target_vec, targeted);
				throw new NBCEL.generic.TargetLostException(targeted, buf.ToString());
			}
		}

		/// <summary>Remove instruction from this list.</summary>
		/// <remarks>Remove instruction from this list. The corresponding Instruction handles must not be reused!
		/// 	</remarks>
		/// <param name="ih">instruction (handle) to remove</param>
		/// <exception cref="NBCEL.generic.TargetLostException"/>
		public virtual void Delete(NBCEL.generic.InstructionHandle ih)
		{
			Remove(ih.GetPrev(), ih.GetNext());
		}

		/// <summary>Remove instruction from this list.</summary>
		/// <remarks>Remove instruction from this list. The corresponding Instruction handles must not be reused!
		/// 	</remarks>
		/// <param name="i">instruction to remove</param>
		/// <exception cref="NBCEL.generic.TargetLostException"/>
		public virtual void Delete(NBCEL.generic.Instruction i)
		{
			NBCEL.generic.InstructionHandle ih;
			if ((ih = FindInstruction1(i)) == null)
			{
				throw new NBCEL.generic.ClassGenException("Instruction " + i + " is not contained in this list."
					);
			}
			Delete(ih);
		}

		/// <summary>Remove instructions from instruction `from' to instruction `to' contained in this list.
		/// 	</summary>
		/// <remarks>
		/// Remove instructions from instruction `from' to instruction `to' contained in this list. The user must ensure that `from' is an instruction before `to',
		/// or risk havoc. The corresponding Instruction handles must not be reused!
		/// </remarks>
		/// <param name="from">where to start deleting (inclusive)</param>
		/// <param name="to">where to end deleting (inclusive)</param>
		/// <exception cref="NBCEL.generic.TargetLostException"/>
		public virtual void Delete(NBCEL.generic.InstructionHandle from, NBCEL.generic.InstructionHandle
			 to)
		{
			Remove(from.GetPrev(), to.GetNext());
		}

		/// <summary>Remove instructions from instruction `from' to instruction `to' contained in this list.
		/// 	</summary>
		/// <remarks>
		/// Remove instructions from instruction `from' to instruction `to' contained in this list. The user must ensure that `from' is an instruction before `to',
		/// or risk havoc. The corresponding Instruction handles must not be reused!
		/// </remarks>
		/// <param name="from">where to start deleting (inclusive)</param>
		/// <param name="to">where to end deleting (inclusive)</param>
		/// <exception cref="NBCEL.generic.TargetLostException"/>
		public virtual void Delete(NBCEL.generic.Instruction from, NBCEL.generic.Instruction
			 to)
		{
			NBCEL.generic.InstructionHandle from_ih;
			NBCEL.generic.InstructionHandle to_ih;
			if ((from_ih = FindInstruction1(from)) == null)
			{
				throw new NBCEL.generic.ClassGenException("Instruction " + from + " is not contained in this list."
					);
			}
			if ((to_ih = FindInstruction2(to)) == null)
			{
				throw new NBCEL.generic.ClassGenException("Instruction " + to + " is not contained in this list."
					);
			}
			Delete(from_ih, to_ih);
		}

		/// <summary>Search for given Instruction reference, start at beginning of list.</summary>
		/// <param name="i">instruction to search for</param>
		/// <returns>instruction found on success, null otherwise</returns>
		private NBCEL.generic.InstructionHandle FindInstruction1(NBCEL.generic.Instruction
			 i)
		{
			for (NBCEL.generic.InstructionHandle ih = start; ih != null; ih = ih.GetNext())
			{
				if (ih.GetInstruction() == i)
				{
					return ih;
				}
			}
			return null;
		}

		/// <summary>Search for given Instruction reference, start at end of list</summary>
		/// <param name="i">instruction to search for</param>
		/// <returns>instruction found on success, null otherwise</returns>
		private NBCEL.generic.InstructionHandle FindInstruction2(NBCEL.generic.Instruction
			 i)
		{
			for (NBCEL.generic.InstructionHandle ih = end; ih != null; ih = ih.GetPrev())
			{
				if (ih.GetInstruction() == i)
				{
					return ih;
				}
			}
			return null;
		}

		public virtual bool Contains(NBCEL.generic.InstructionHandle i)
		{
			if (i == null)
			{
				return false;
			}
			for (NBCEL.generic.InstructionHandle ih = start; ih != null; ih = ih.GetNext())
			{
				if (ih == i)
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool Contains(NBCEL.generic.Instruction i)
		{
			return FindInstruction1(i) != null;
		}

		public virtual void SetPositions()
		{
			// TODO could be package-protected? (some test code would need to be repackaged)
			SetPositions(false);
		}

		/// <summary>Give all instructions their position number (offset in byte stream), i.e., make the list ready to be dumped.
		/// 	</summary>
		/// <param name="check">Perform sanity checks, e.g. if all targeted instructions really belong to this list
		/// 	</param>
		public virtual void SetPositions(bool check)
		{
			// called by code in other packages
			int max_additional_bytes = 0;
			int additional_bytes = 0;
			int index = 0;
			int count = 0;
			int[] pos = new int[length];
			/*
			* Pass 0: Sanity checks
			*/
			if (check)
			{
				for (NBCEL.generic.InstructionHandle ih = start; ih != null; ih = ih.GetNext())
				{
					NBCEL.generic.Instruction i = ih.GetInstruction();
					if (i is NBCEL.generic.BranchInstruction)
					{
						// target instruction within list?
						NBCEL.generic.Instruction inst = ((NBCEL.generic.BranchInstruction)i).GetTarget()
							.GetInstruction();
						if (!Contains(inst))
						{
							throw new NBCEL.generic.ClassGenException("Branch target of " + NBCEL.Const.GetOpcodeName
								(i.GetOpcode()) + ":" + inst + " not in instruction list");
						}
						if (i is NBCEL.generic.Select)
						{
							NBCEL.generic.InstructionHandle[] targets = ((NBCEL.generic.Select)i).GetTargets(
								);
							foreach (NBCEL.generic.InstructionHandle target in targets)
							{
								inst = target.GetInstruction();
								if (!Contains(inst))
								{
									throw new NBCEL.generic.ClassGenException("Branch target of " + NBCEL.Const.GetOpcodeName
										(i.GetOpcode()) + ":" + inst + " not in instruction list");
								}
							}
						}
						if (!(ih is NBCEL.generic.BranchHandle))
						{
							throw new NBCEL.generic.ClassGenException("Branch instruction " + NBCEL.Const.GetOpcodeName
								(i.GetOpcode()) + ":" + inst + " not contained in BranchHandle.");
						}
					}
				}
			}
			/*
			* Pass 1: Set position numbers and sum up the maximum number of bytes an instruction may be shifted.
			*/
			for (NBCEL.generic.InstructionHandle ih = start; ih != null; ih = ih.GetNext())
			{
				NBCEL.generic.Instruction i = ih.GetInstruction();
				ih.SetPosition(index);
				pos[count++] = index;
				switch (i.GetOpcode())
				{
					case NBCEL.Const.JSR:
					case NBCEL.Const.GOTO:
					{
						/*
						* Get an estimate about how many additional bytes may be added, because BranchInstructions may have variable length depending on the target offset
						* (short vs. int) or alignment issues (TABLESWITCH and LOOKUPSWITCH).
						*/
						max_additional_bytes += 2;
						break;
					}

					case NBCEL.Const.TABLESWITCH:
					case NBCEL.Const.LOOKUPSWITCH:
					{
						max_additional_bytes += 3;
						break;
					}
				}
				index += i.GetLength();
			}
			/*
			* Pass 2: Expand the variable-length (Branch)Instructions depending on the target offset (short or int) and ensure that branch targets are within this
			* list.
			*/
			for (NBCEL.generic.InstructionHandle ih = start; ih != null; ih = ih.GetNext())
			{
				additional_bytes += ih.UpdatePosition(additional_bytes, max_additional_bytes);
			}
			/*
			* Pass 3: Update position numbers (which may have changed due to the preceding expansions), like pass 1.
			*/
			index = count = 0;
			for (NBCEL.generic.InstructionHandle ih = start; ih != null; ih = ih.GetNext())
			{
				NBCEL.generic.Instruction i = ih.GetInstruction();
				ih.SetPosition(index);
				pos[count++] = index;
				index += i.GetLength();
			}
			byte_positions = new int[count];
			// Trim to proper size
			System.Array.Copy(pos, 0, byte_positions, 0, count);
		}

		/// <summary>When everything is finished, use this method to convert the instruction list into an array of bytes.
		/// 	</summary>
		/// <returns>the byte code ready to be dumped</returns>
		public virtual byte[] GetByteCode()
		{
			// Update position indices of instructions
			SetPositions();
			System.IO.MemoryStream b = new System.IO.MemoryStream();
			java.io.DataOutputStream @out = new java.io.DataOutputStream(b.ToOutputStream());
			try
			{
				for (NBCEL.generic.InstructionHandle ih = start; ih != null; ih = ih.GetNext())
				{
					NBCEL.generic.Instruction i = ih.GetInstruction();
					i.Dump(@out);
				}
				// Traverse list
				@out.Flush();
			}
			catch (System.IO.IOException e)
			{
				System.Console.Error.WriteLine(e);
				return new byte[0];
			}
			return b.ToArray();
		}

		/// <returns>an array of instructions without target information for branch instructions.
		/// 	</returns>
		public virtual NBCEL.generic.Instruction[] GetInstructions()
		{
			System.Collections.Generic.List<NBCEL.generic.Instruction> instructions = new System.Collections.Generic.List
				<NBCEL.generic.Instruction>();
			try
			{
				using (NBCEL.util.ByteSequence bytes = new NBCEL.util.ByteSequence(GetByteCode()))
				{
					while (bytes.Available() > 0)
					{
						instructions.Add(NBCEL.generic.Instruction.ReadInstruction(bytes));
					}
				}
			}
			catch (System.IO.IOException e)
			{
				throw new NBCEL.generic.ClassGenException(e.ToString(), e);
			}
			return Sharpen.Collections.ToArray(instructions, new NBCEL.generic.Instruction[instructions
				.Count]);
		}

		public override string ToString()
		{
			return ToString(true);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <param name="verbose">toggle output format</param>
		/// <returns>String containing all instructions in this list.</returns>
		public virtual string ToString(bool verbose)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			for (NBCEL.generic.InstructionHandle ih = start; ih != null; ih = ih.GetNext())
			{
				buf.Append(ih.ToString(verbose)).Append("\n");
			}
			return buf.ToString();
		}

		/// <returns>iterator that lists all instructions (handles)</returns>
		public System.Collections.Generic.IEnumerator<NBCEL.generic.InstructionHandle
			> GetEnumerator()
		{
			return new _IEnumerator_991(this);
		}

		private sealed class _IEnumerator_991 : System.Collections.Generic.IEnumerator<NBCEL.generic.InstructionHandle
			>
		{
			private readonly InstructionList _enclosing;

			public _IEnumerator_991(InstructionList _enclosing)
			{
				this._enclosing = _enclosing;
				this.ih = this._enclosing.start;
			}

			private NBCEL.generic.InstructionHandle ih;

			/// <exception cref="java.util.NoSuchElementException"/>
			public NBCEL.generic.InstructionHandle Next()
			{
				if (this.ih == null)
				{
					throw new Exception("NoSuchElement");
				}
				NBCEL.generic.InstructionHandle i = this.ih;
				return i;
			}

			public bool MoveNext()
			{
				this.ih = this.ih.GetNext();
				return this.ih != null;
			}

			public void Reset()
			{
				
			}

			public InstructionHandle Current => this.ih;

			object IEnumerator.Current => Current;

			public void Dispose()
			{
			}
		}

		/// <returns>array containing all instructions (handles)</returns>
		public virtual NBCEL.generic.InstructionHandle[] GetInstructionHandles()
		{
			NBCEL.generic.InstructionHandle[] ihs = new NBCEL.generic.InstructionHandle[length
				];
			NBCEL.generic.InstructionHandle ih = start;
			for (int i = 0; i < length; i++)
			{
				ihs[i] = ih;
				ih = ih.GetNext();
			}
			return ihs;
		}

		/// <summary>Get positions (offsets) of all instructions in the list.</summary>
		/// <remarks>
		/// Get positions (offsets) of all instructions in the list. This relies on that the list has been freshly created from an byte code array, or that
		/// setPositions() has been called. Otherwise this may be inaccurate.
		/// </remarks>
		/// <returns>array containing all instruction's offset in byte code</returns>
		public virtual int[] GetInstructionPositions()
		{
			return byte_positions;
		}

		/// <returns>complete, i.e., deep copy of this list</returns>
		public virtual NBCEL.generic.InstructionList Copy()
		{
			System.Collections.Generic.IDictionary<NBCEL.generic.InstructionHandle, NBCEL.generic.InstructionHandle
				> map = new System.Collections.Generic.Dictionary<NBCEL.generic.InstructionHandle
				, NBCEL.generic.InstructionHandle>();
			NBCEL.generic.InstructionList il = new NBCEL.generic.InstructionList();
			/*
			* Pass 1: Make copies of all instructions, append them to the new list and associate old instruction references with the new ones, i.e., a 1:1 mapping.
			*/
			for (NBCEL.generic.InstructionHandle ih = start; ih != null; ih = ih.GetNext())
			{
				NBCEL.generic.Instruction i = ih.GetInstruction();
				NBCEL.generic.Instruction c = i.Copy();
				// Use clone for shallow copy
				if (c is NBCEL.generic.BranchInstruction)
				{
					Sharpen.Collections.Put(map, ih, il.Append((NBCEL.generic.BranchInstruction)c));
				}
				else
				{
					Sharpen.Collections.Put(map, ih, il.Append(c));
				}
			}
			/*
			* Pass 2: Update branch targets.
			*/
			NBCEL.generic.InstructionHandle ih_1 = start;
			NBCEL.generic.InstructionHandle ch = il.start;
			while (ih_1 != null)
			{
				NBCEL.generic.Instruction i = ih_1.GetInstruction();
				NBCEL.generic.Instruction c = ch.GetInstruction();
				if (i is NBCEL.generic.BranchInstruction)
				{
					NBCEL.generic.BranchInstruction bi = (NBCEL.generic.BranchInstruction)i;
					NBCEL.generic.BranchInstruction bc = (NBCEL.generic.BranchInstruction)c;
					NBCEL.generic.InstructionHandle itarget = bi.GetTarget();
					// old target
					// New target is in hash map
					bc.SetTarget(map.GetOrNull(itarget));
					if (bi is NBCEL.generic.Select)
					{
						// Either LOOKUPSWITCH or TABLESWITCH
						NBCEL.generic.InstructionHandle[] itargets = ((NBCEL.generic.Select)bi).GetTargets
							();
						NBCEL.generic.InstructionHandle[] ctargets = ((NBCEL.generic.Select)bc).GetTargets
							();
						for (int j = 0; j < itargets.Length; j++)
						{
							// Update all targets
							ctargets[j] = map.GetOrNull(itargets[j]);
						}
					}
				}
				ih_1 = ih_1.GetNext();
				ch = ch.GetNext();
			}
			return il;
		}

		/// <summary>Replace all references to the old constant pool with references to the new constant pool
		/// 	</summary>
		public virtual void ReplaceConstantPool(NBCEL.generic.ConstantPoolGen old_cp, NBCEL.generic.ConstantPoolGen
			 new_cp)
		{
			for (NBCEL.generic.InstructionHandle ih = start; ih != null; ih = ih.GetNext())
			{
				NBCEL.generic.Instruction i = ih.GetInstruction();
				if (i is NBCEL.generic.CPInstruction)
				{
					NBCEL.generic.CPInstruction ci = (NBCEL.generic.CPInstruction)i;
					NBCEL.classfile.Constant c = old_cp.GetConstant(ci.GetIndex());
					ci.SetIndex(new_cp.AddConstant(c, old_cp));
				}
			}
		}

		private void Clear()
		{
			start = end = null;
			length = 0;
		}

		/// <summary>Delete contents of list.</summary>
		/// <remarks>
		/// Delete contents of list. Provides better memory utilization, because the system then may reuse the instruction handles. This method is typically called
		/// right after
		/// <see cref="MethodGen.GetMethod()"/>
		/// .
		/// </remarks>
		public virtual void Dispose()
		{
			// Traverse in reverse order, because ih.next is overwritten
			for (NBCEL.generic.InstructionHandle ih = end; ih != null; ih = ih.GetPrev())
			{
				/*
				* Causes BranchInstructions to release target and targeters, because it calls dispose() on the contained instruction.
				*/
				ih.Dispose();
			}
			Clear();
		}

		/// <returns>start of list</returns>
		public virtual NBCEL.generic.InstructionHandle GetStart()
		{
			return start;
		}

		/// <returns>end of list</returns>
		public virtual NBCEL.generic.InstructionHandle GetEnd()
		{
			return end;
		}

		/// <returns>length of list (Number of instructions, not bytes)</returns>
		public virtual int GetLength()
		{
			return length;
		}

		/// <returns>length of list (Number of instructions, not bytes)</returns>
		public virtual int Size()
		{
			return length;
		}

		/// <summary>Redirect all references from old_target to new_target, i.e., update targets of branch instructions.
		/// 	</summary>
		/// <param name="old_target">the old target instruction handle</param>
		/// <param name="new_target">the new target instruction handle</param>
		public virtual void RedirectBranches(NBCEL.generic.InstructionHandle old_target, 
			NBCEL.generic.InstructionHandle new_target)
		{
			for (NBCEL.generic.InstructionHandle ih = start; ih != null; ih = ih.GetNext())
			{
				NBCEL.generic.Instruction i = ih.GetInstruction();
				if (i is NBCEL.generic.BranchInstruction)
				{
					NBCEL.generic.BranchInstruction b = (NBCEL.generic.BranchInstruction)i;
					NBCEL.generic.InstructionHandle target = b.GetTarget();
					if (target == old_target)
					{
						b.SetTarget(new_target);
					}
					if (b is NBCEL.generic.Select)
					{
						// Either LOOKUPSWITCH or TABLESWITCH
						NBCEL.generic.InstructionHandle[] targets = ((NBCEL.generic.Select)b).GetTargets(
							);
						for (int j = 0; j < targets.Length; j++)
						{
							if (targets[j] == old_target)
							{
								((NBCEL.generic.Select)b).SetTarget(j, new_target);
							}
						}
					}
				}
			}
		}

		/// <summary>Redirect all references of local variables from old_target to new_target.
		/// 	</summary>
		/// <param name="lg">array of local variables</param>
		/// <param name="old_target">the old target instruction handle</param>
		/// <param name="new_target">the new target instruction handle</param>
		/// <seealso cref="MethodGen"/>
		public virtual void RedirectLocalVariables(NBCEL.generic.LocalVariableGen[] lg, NBCEL.generic.InstructionHandle
			 old_target, NBCEL.generic.InstructionHandle new_target)
		{
			foreach (NBCEL.generic.LocalVariableGen element in lg)
			{
				NBCEL.generic.InstructionHandle start = element.GetStart();
				NBCEL.generic.InstructionHandle end = element.GetEnd();
				if (start == old_target)
				{
					element.SetStart(new_target);
				}
				if (end == old_target)
				{
					element.SetEnd(new_target);
				}
			}
		}

		/// <summary>Redirect all references of exception handlers from old_target to new_target.
		/// 	</summary>
		/// <param name="exceptions">array of exception handlers</param>
		/// <param name="old_target">the old target instruction handle</param>
		/// <param name="new_target">the new target instruction handle</param>
		/// <seealso cref="MethodGen"/>
		public virtual void RedirectExceptionHandlers(NBCEL.generic.CodeExceptionGen[] exceptions
			, NBCEL.generic.InstructionHandle old_target, NBCEL.generic.InstructionHandle new_target
			)
		{
			foreach (NBCEL.generic.CodeExceptionGen exception in exceptions)
			{
				if (exception.GetStartPC() == old_target)
				{
					exception.SetStartPC(new_target);
				}
				if (exception.GetEndPC() == old_target)
				{
					exception.SetEndPC(new_target);
				}
				if (exception.GetHandlerPC() == old_target)
				{
					exception.SetHandlerPC(new_target);
				}
			}
		}

		private System.Collections.Generic.List<NBCEL.generic.InstructionListObserver> observers;

		/// <summary>Add observer for this object.</summary>
		public virtual void AddObserver(NBCEL.generic.InstructionListObserver o)
		{
			if (observers == null)
			{
				observers = new System.Collections.Generic.List<NBCEL.generic.InstructionListObserver
					>();
			}
			observers.Add(o);
		}

		/// <summary>Remove observer for this object.</summary>
		public virtual void RemoveObserver(NBCEL.generic.InstructionListObserver o)
		{
			if (observers != null)
			{
				observers.Remove(o);
			}
		}

		/// <summary>Call notify() method on all observers.</summary>
		/// <remarks>
		/// Call notify() method on all observers. This method is not called automatically whenever the state has changed, but has to be called by the user after he
		/// has finished editing the object.
		/// </remarks>
		public virtual void Update()
		{
			if (observers != null)
			{
				foreach (NBCEL.generic.InstructionListObserver observer in observers)
				{
					observer.Notify(this);
				}
			}
		}
	}
}
