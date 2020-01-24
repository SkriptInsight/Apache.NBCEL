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

using System.Linq;
using Sharpen;

namespace NBCEL.verifier.structurals
{
	/// <summary>This class implements a stack used for symbolic JVM stack simulation.</summary>
	/// <remarks>
	/// This class implements a stack used for symbolic JVM stack simulation.
	/// [It's used an an operand stack substitute.]
	/// Elements of this stack are
	/// <see cref="NBCEL.generic.Type"/>
	/// objects.
	/// </remarks>
	public class OperandStack : System.ICloneable
	{
		/// <summary>We hold the stack information here.</summary>
		private System.Collections.Generic.List<NBCEL.generic.Type> stack = new System.Collections.Generic.List
			<NBCEL.generic.Type>();

		/// <summary>The maximum number of stack slots this OperandStack instance may hold.</summary>
		private readonly int maxStack__;

		/// <summary>Creates an empty stack with a maximum of maxStack slots.</summary>
		public OperandStack(int maxStack)
		{
			this.maxStack__ = maxStack;
		}

		/// <summary>
		/// Creates an otherwise empty stack with a maximum of maxStack slots and
		/// the ObjectType 'obj' at the top.
		/// </summary>
		public OperandStack(int maxStack, NBCEL.generic.ObjectType obj)
		{
			this.maxStack__ = maxStack;
			this.Push(obj);
		}

		/// <summary>
		/// Returns a deep copy of this object; that means, the clone operates
		/// on a new stack.
		/// </summary>
		/// <remarks>
		/// Returns a deep copy of this object; that means, the clone operates
		/// on a new stack. However, the Type objects on the stack are
		/// shared.
		/// </remarks>
		public virtual object Clone()
		{
			NBCEL.verifier.structurals.OperandStack newstack = new NBCEL.verifier.structurals.OperandStack
				(this.maxStack__);
			System.Collections.Generic.List<NBCEL.generic.Type> clone = (System.Collections.Generic.List
				<NBCEL.generic.Type>) this.stack.ToList();
			// OK because this.stack is the same type
			newstack.stack = clone;
			return newstack;
		}

		/// <summary>Clears the stack.</summary>
		public virtual void Clear()
		{
			stack = new System.Collections.Generic.List<NBCEL.generic.Type>();
		}

		/// <returns>a hash code value for the object.</returns>
		public override int GetHashCode()
		{
			return stack.GetHashCode();
		}

		/// <summary>
		/// Returns true if and only if this OperandStack
		/// equals another, meaning equal lengths and equal
		/// objects on the stacks.
		/// </summary>
		public override bool Equals(object o)
		{
			if (!(o is NBCEL.verifier.structurals.OperandStack))
			{
				return false;
			}
			NBCEL.verifier.structurals.OperandStack s = (NBCEL.verifier.structurals.OperandStack
				)o;
			return this.stack.Equals(s.stack);
		}

		/// <summary>Returns a (typed!) clone of this.</summary>
		/// <seealso cref="Clone()"/>
		public virtual NBCEL.verifier.structurals.OperandStack GetClone()
		{
			return (NBCEL.verifier.structurals.OperandStack)this.Clone();
		}

		/// <summary>Returns true IFF this OperandStack is empty.</summary>
		public virtual bool IsEmpty()
		{
			return (stack.Count == 0);
		}

		/// <summary>Returns the number of stack slots this stack can hold.</summary>
		public virtual int MaxStack()
		{
			return this.maxStack__;
		}

		/// <summary>Returns the element on top of the stack.</summary>
		/// <remarks>Returns the element on top of the stack. The element is not popped off the stack!
		/// 	</remarks>
		public virtual NBCEL.generic.Type Peek()
		{
			return Peek(0);
		}

		/// <summary>
		/// Returns the element that's i elements below the top element; that means,
		/// iff i==0 the top element is returned.
		/// </summary>
		/// <remarks>
		/// Returns the element that's i elements below the top element; that means,
		/// iff i==0 the top element is returned. The element is not popped off the stack!
		/// </remarks>
		public virtual NBCEL.generic.Type Peek(int i)
		{
			return stack[Size() - i - 1];
		}

		/// <summary>Returns the element on top of the stack.</summary>
		/// <remarks>Returns the element on top of the stack. The element is popped off the stack.
		/// 	</remarks>
		public virtual NBCEL.generic.Type Pop()
		{
			NBCEL.generic.Type e = stack.RemoveAtReturningValue(Size() - 1);
			return e;
		}

		/// <summary>Pops i elements off the stack.</summary>
		/// <remarks>Pops i elements off the stack. ALWAYS RETURNS "null"!!!</remarks>
		public virtual NBCEL.generic.Type Pop(int i)
		{
			for (int j = 0; j < i; j++)
			{
				Pop();
			}
			return null;
		}

		/// <summary>Pushes a Type object onto the stack.</summary>
		public virtual void Push(NBCEL.generic.Type type)
		{
			if (type == null)
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("Cannot push NULL onto OperandStack."
					);
			}
			if (type == NBCEL.generic.Type.BOOLEAN || type == NBCEL.generic.Type.CHAR || type
				 == NBCEL.generic.Type.BYTE || type == NBCEL.generic.Type.SHORT)
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("The OperandStack does not know about '"
					 + type + "'; use Type.INT instead.");
			}
			if (SlotsUsed() >= maxStack__)
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("OperandStack too small, should have thrown proper Exception elsewhere. Stack: "
					 + this);
			}
			stack.Add(type);
		}

		/// <summary>Returns the size of this OperandStack; that means, how many Type objects there are.
		/// 	</summary>
		public virtual int Size()
		{
			return stack.Count;
		}

		/// <summary>Returns the number of stack slots used.</summary>
		/// <seealso cref="MaxStack()"/>
		public virtual int SlotsUsed()
		{
			/*  XXX change this to a better implementation using a variable
			that keeps track of the actual slotsUsed()-value monitoring
			all push()es and pop()s.
			*/
			int slots = 0;
			for (int i = 0; i < stack.Count; i++)
			{
				slots += Peek(i).GetSize();
			}
			return slots;
		}

		/// <summary>Returns a String representation of this OperandStack instance.</summary>
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("Slots used: ");
			sb.Append(SlotsUsed());
			sb.Append(" MaxStack: ");
			sb.Append(maxStack__);
			sb.Append(".\n");
			for (int i = 0; i < Size(); i++)
			{
				sb.Append(Peek(i));
				sb.Append(" (Size: ");
				sb.Append(Peek(i).GetSize().ToString());
				sb.Append(")\n");
			}
			return sb.ToString();
		}

		/// <summary>Merges another stack state into this instance's stack state.</summary>
		/// <remarks>
		/// Merges another stack state into this instance's stack state.
		/// See the Java Virtual Machine Specification, Second Edition, page 146: 4.9.2
		/// for details.
		/// </remarks>
		public virtual void Merge(NBCEL.verifier.structurals.OperandStack s)
		{
			try
			{
				if ((SlotsUsed() != s.SlotsUsed()) || (Size() != s.Size()))
				{
					throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Cannot merge stacks of different size:\nOperandStack A:\n"
						 + this + "\nOperandStack B:\n" + s);
				}
				for (int i = 0; i < Size(); i++)
				{
					// If the object _was_ initialized and we're supposed to merge
					// in some uninitialized object, we reject the code (see vmspec2, 4.9.4, last paragraph).
					if ((!(stack[i] is NBCEL.verifier.structurals.UninitializedObjectType)) && (s.stack
						[i] is NBCEL.verifier.structurals.UninitializedObjectType))
					{
						throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Backwards branch with an uninitialized object on the stack detected."
							);
					}
					// Even harder, we're not initialized but are supposed to broaden
					// the known object type
					if ((!(stack[i].Equals(s.stack[i]))) && (stack[i] is NBCEL.verifier.structurals.UninitializedObjectType
						) && (!(s.stack[i] is NBCEL.verifier.structurals.UninitializedObjectType)))
					{
						throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Backwards branch with an uninitialized object on the stack detected."
							);
					}
					// on the other hand...
					if (stack[i] is NBCEL.verifier.structurals.UninitializedObjectType)
					{
						//if we have an uninitialized object here
						if (!(s.stack[i] is NBCEL.verifier.structurals.UninitializedObjectType))
						{
							//that has been initialized by now
							stack[i] = ((NBCEL.verifier.structurals.UninitializedObjectType)(stack[i])).GetInitialized
								();
						}
					}
					//note that.
					if (!stack[i].Equals(s.stack[i]))
					{
						if ((stack[i] is NBCEL.generic.ReferenceType) && (s.stack[i] is NBCEL.generic.ReferenceType
							))
						{
							stack[i] = ((NBCEL.generic.ReferenceType)stack[i]).GetFirstCommonSuperclass((NBCEL.generic.ReferenceType
								)(s.stack[i]));
						}
						else
						{
							throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Cannot merge stacks of different types:\nStack A:\n"
								 + this + "\nStack B:\n" + s);
						}
					}
				}
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: maybe not the best way to handle this
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>
		/// Replaces all occurences of u in this OperandStack instance
		/// with an "initialized" ObjectType.
		/// </summary>
		public virtual void InitializeObject(NBCEL.verifier.structurals.UninitializedObjectType
			 u)
		{
			for (int i = 0; i < stack.Count; i++)
			{
				if (stack[i] == u)
				{
					stack[i] = u.GetInitialized();
				}
			}
		}

		object System.ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
