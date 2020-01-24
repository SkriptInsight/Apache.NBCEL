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
	/// This class implements an array of local variables used for symbolic JVM
	/// simulation.
	/// </summary>
	public class LocalVariables : System.ICloneable
	{
		/// <summary>The Type[] containing the local variable slots.</summary>
		private readonly NBCEL.generic.Type[] locals;

		/// <summary>Creates a new LocalVariables object.</summary>
		/// <param name="localVariableCount">local variable count.</param>
		public LocalVariables(int localVariableCount)
		{
			locals = new NBCEL.generic.Type[localVariableCount];
			for (int i = 0; i < localVariableCount; i++)
			{
				locals[i] = NBCEL.generic.Type.UNKNOWN;
			}
		}

		/// <summary>Returns a deep copy of this object; i.e.</summary>
		/// <remarks>
		/// Returns a deep copy of this object; i.e. the clone
		/// operates on a new local variable array.
		/// However, the Type objects in the array are shared.
		/// </remarks>
		public virtual object Clone()
		{
			NBCEL.verifier.structurals.LocalVariables lvs = new NBCEL.verifier.structurals.LocalVariables
				(locals.Length);
			for (int i = 0; i < locals.Length; i++)
			{
				lvs.locals[i] = this.locals[i];
			}
			return lvs;
		}

		/// <summary>Returns the type of the local variable slot index.</summary>
		/// <param name="slotIndex">Slot to look up.</param>
		/// <returns>the type of the local variable slot index.</returns>
		public virtual NBCEL.generic.Type Get(int slotIndex)
		{
			return locals[slotIndex];
		}

		/// <summary>Returns a (correctly typed) clone of this object.</summary>
		/// <remarks>
		/// Returns a (correctly typed) clone of this object.
		/// This is equivalent to ((LocalVariables) this.clone()).
		/// </remarks>
		/// <returns>a (correctly typed) clone of this object.</returns>
		public virtual NBCEL.verifier.structurals.LocalVariables GetClone()
		{
			return (NBCEL.verifier.structurals.LocalVariables)this.Clone();
		}

		/// <summary>Returns the number of local variable slots.</summary>
		/// <returns>the number of local variable slots.</returns>
		public virtual int MaxLocals()
		{
			return locals.Length;
		}

		/// <summary>Sets a new Type for the given local variable slot.</summary>
		/// <param name="slotIndex">Target slot index.</param>
		/// <param name="type">Type to save at the given slot index.</param>
		public virtual void Set(int slotIndex, NBCEL.generic.Type type)
		{
			// TODO could be package-protected?
			if (type == NBCEL.generic.Type.BYTE || type == NBCEL.generic.Type.SHORT || type ==
				 NBCEL.generic.Type.BOOLEAN || type == NBCEL.generic.Type.CHAR)
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("LocalVariables do not know about '"
					 + type + "'. Use Type.INT instead.");
			}
			locals[slotIndex] = type;
		}

		/// <returns>a hash code value for the object.</returns>
		public override int GetHashCode()
		{
			return locals.Length;
		}

		/*
		* Fulfills the general contract of Object.equals().
		*/
		public override bool Equals(object o)
		{
			if (!(o is NBCEL.verifier.structurals.LocalVariables))
			{
				return false;
			}
			NBCEL.verifier.structurals.LocalVariables lv = (NBCEL.verifier.structurals.LocalVariables
				)o;
			if (this.locals.Length != lv.locals.Length)
			{
				return false;
			}
			for (int i = 0; i < this.locals.Length; i++)
			{
				if (!this.locals[i].Equals(lv.locals[i]))
				{
					//System.out.println(this.locals[i]+" is not "+lv.locals[i]);
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Merges two local variables sets as described in the Java Virtual Machine Specification,
		/// Second Edition, section 4.9.2, page 146.
		/// </summary>
		/// <param name="localVariable">other local variable.</param>
		public virtual void Merge(NBCEL.verifier.structurals.LocalVariables localVariable
			)
		{
			if (this.locals.Length != localVariable.locals.Length)
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("Merging LocalVariables of different size?!? From different methods or what?!?"
					);
			}
			for (int i = 0; i < locals.Length; i++)
			{
				Merge(localVariable, i);
			}
		}

		/// <summary>Merges a single local variable.</summary>
		/// <seealso cref="Merge(LocalVariables)"/>
		private void Merge(NBCEL.verifier.structurals.LocalVariables lv, int i)
		{
			try
			{
				// We won't accept an unitialized object if we know it was initialized;
				// compare vmspec2, 4.9.4, last paragraph.
				if ((!(locals[i] is NBCEL.verifier.structurals.UninitializedObjectType)) && (lv.locals
					[i] is NBCEL.verifier.structurals.UninitializedObjectType))
				{
					throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Backwards branch with an uninitialized object in the local variables detected."
						);
				}
				// Even harder, what about _different_ uninitialized object types?!
				if ((!(locals[i].Equals(lv.locals[i]))) && (locals[i] is NBCEL.verifier.structurals.UninitializedObjectType
					) && (lv.locals[i] is NBCEL.verifier.structurals.UninitializedObjectType))
				{
					throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Backwards branch with an uninitialized object in the local variables detected."
						);
				}
				// If we just didn't know that it was initialized, we have now learned.
				if (locals[i] is NBCEL.verifier.structurals.UninitializedObjectType)
				{
					if (!(lv.locals[i] is NBCEL.verifier.structurals.UninitializedObjectType))
					{
						locals[i] = ((NBCEL.verifier.structurals.UninitializedObjectType)locals[i]).GetInitialized
							();
					}
				}
				if ((locals[i] is NBCEL.generic.ReferenceType) && (lv.locals[i] is NBCEL.generic.ReferenceType
					))
				{
					if (!locals[i].Equals(lv.locals[i]))
					{
						// needed in case of two UninitializedObjectType instances
						NBCEL.generic.Type sup = ((NBCEL.generic.ReferenceType)locals[i]).GetFirstCommonSuperclass
							((NBCEL.generic.ReferenceType)(lv.locals[i]));
						if (sup != null)
						{
							locals[i] = sup;
						}
						else
						{
							// We should have checked this in Pass2!
							throw new NBCEL.verifier.exc.AssertionViolatedException("Could not load all the super classes of '"
								 + locals[i] + "' and '" + lv.locals[i] + "'.");
						}
					}
				}
				else if (!(locals[i].Equals(lv.locals[i])))
				{
					/*TODO
					if ((locals[i] instanceof org.apache.bcel.generic.ReturnaddressType) &&
					(lv.locals[i] instanceof org.apache.bcel.generic.ReturnaddressType)) {
					//System.err.println("merging "+locals[i]+" and "+lv.locals[i]);
					throw new AssertionViolatedException("Merging different ReturnAddresses: '"+locals[i]+"' and '"+lv.locals[i]+"'.");
					}
					*/
					locals[i] = NBCEL.generic.Type.UNKNOWN;
				}
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: maybe not the best way to handle this
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>Returns a String representation of this object.</summary>
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < locals.Length; i++)
			{
				sb.Append(i.ToString());
				sb.Append(": ");
				sb.Append(locals[i]);
				sb.Append("\n");
			}
			return sb.ToString();
		}

		/// <summary>
		/// Replaces all occurrences of
		/// <paramref name="uninitializedObjectType"/>
		/// in this local variables set
		/// with an "initialized" ObjectType.
		/// </summary>
		/// <param name="uninitializedObjectType">the object to match.</param>
		public virtual void InitializeObject(NBCEL.verifier.structurals.UninitializedObjectType
			 uninitializedObjectType)
		{
			for (int i = 0; i < locals.Length; i++)
			{
				if (locals[i] == uninitializedObjectType)
				{
					locals[i] = uninitializedObjectType.GetInitialized();
				}
			}
		}

		object System.ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
