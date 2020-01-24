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

namespace NBCEL.verifier.structurals
{
	/// <summary>
	/// This class represents a JVM execution frame; that means,
	/// a local variable array and an operand stack.
	/// </summary>
	public class Frame
	{
		/// <summary>
		/// For instance initialization methods, it is important to remember
		/// which instance it is that is not initialized yet.
		/// </summary>
		/// <remarks>
		/// For instance initialization methods, it is important to remember
		/// which instance it is that is not initialized yet. It will be
		/// initialized invoking another constructor later.
		/// NULL means the instance already *is* initialized.
		/// </remarks>
		[System.ObsoleteAttribute(@"Use the getter/setter to access the field as it may be made private in a later release"
			)]
		protected internal static NBCEL.verifier.structurals.UninitializedObjectType _this;

		private readonly NBCEL.verifier.structurals.LocalVariables locals;

		private readonly NBCEL.verifier.structurals.OperandStack stack;

		public Frame(int maxLocals, int maxStack)
		{
			locals = new NBCEL.verifier.structurals.LocalVariables(maxLocals);
			stack = new NBCEL.verifier.structurals.OperandStack(maxStack);
		}

		public Frame(NBCEL.verifier.structurals.LocalVariables locals, NBCEL.verifier.structurals.OperandStack
			 stack)
		{
			this.locals = locals;
			this.stack = stack;
		}

		protected internal virtual object Clone()
		{
			NBCEL.verifier.structurals.Frame f = new NBCEL.verifier.structurals.Frame(locals.
				GetClone(), stack.GetClone());
			return f;
		}

		public virtual NBCEL.verifier.structurals.Frame GetClone()
		{
			return (NBCEL.verifier.structurals.Frame)Clone();
		}

		public virtual NBCEL.verifier.structurals.LocalVariables GetLocals()
		{
			return locals;
		}

		public virtual NBCEL.verifier.structurals.OperandStack GetStack()
		{
			return stack;
		}

		/// <returns>a hash code value for the object.</returns>
		public override int GetHashCode()
		{
			return stack.GetHashCode() ^ locals.GetHashCode();
		}

		public override bool Equals(object o)
		{
			if (!(o is NBCEL.verifier.structurals.Frame))
			{
				return false;
			}
			// implies "null" is non-equal.
			NBCEL.verifier.structurals.Frame f = (NBCEL.verifier.structurals.Frame)o;
			return this.stack.Equals(f.stack) && this.locals.Equals(f.locals);
		}

		/// <summary>Returns a String representation of the Frame instance.</summary>
		public override string ToString()
		{
			string s = "Local Variables:\n";
			s += locals;
			s += "OperandStack:\n";
			s += stack;
			return s;
		}

		/// <returns>the _this</returns>
		/// <since>6.0</since>
		public static NBCEL.verifier.structurals.UninitializedObjectType GetThis()
		{
			return _this;
		}

		/// <param name="_this">the _this to set</param>
		/// <since>6.0</since>
		public static void SetThis(NBCEL.verifier.structurals.UninitializedObjectType _this
			)
		{
			NBCEL.verifier.structurals.Frame._this = _this;
		}
	}
}
