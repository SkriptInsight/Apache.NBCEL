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
using System.Text;
using NBCEL.generic;
using NBCEL.verifier.exc;
using Type = NBCEL.generic.Type;

namespace NBCEL.verifier.structurals
{
	/// <summary>
	///     This class implements an array of local variables used for symbolic JVM
	///     simulation.
	/// </summary>
	public class LocalVariables : ICloneable
    {
        /// <summary>The Type[] containing the local variable slots.</summary>
        private readonly Type[] locals;

        /// <summary>Creates a new LocalVariables object.</summary>
        /// <param name="localVariableCount">local variable count.</param>
        public LocalVariables(int localVariableCount)
        {
            locals = new Type[localVariableCount];
            for (var i = 0; i < localVariableCount; i++) locals[i] = Type.UNKNOWN;
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>Returns a deep copy of this object; i.e.</summary>
        /// <remarks>
        ///     Returns a deep copy of this object; i.e. the clone
        ///     operates on a new local variable array.
        ///     However, the Type objects in the array are shared.
        /// </remarks>
        public virtual object Clone()
        {
            var lvs = new LocalVariables
                (locals.Length);
            for (var i = 0; i < locals.Length; i++) lvs.locals[i] = locals[i];
            return lvs;
        }

        /// <summary>Returns the type of the local variable slot index.</summary>
        /// <param name="slotIndex">Slot to look up.</param>
        /// <returns>the type of the local variable slot index.</returns>
        public virtual Type Get(int slotIndex)
        {
            return locals[slotIndex];
        }

        /// <summary>Returns a (correctly typed) clone of this object.</summary>
        /// <remarks>
        ///     Returns a (correctly typed) clone of this object.
        ///     This is equivalent to ((LocalVariables) this.clone()).
        /// </remarks>
        /// <returns>a (correctly typed) clone of this object.</returns>
        public virtual LocalVariables GetClone()
        {
            return (LocalVariables) Clone();
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
        public virtual void Set(int slotIndex, Type type)
        {
            // TODO could be package-protected?
            if (type == Type.BYTE || type == Type.SHORT || type ==
                Type.BOOLEAN || type == Type.CHAR)
                throw new AssertionViolatedException("LocalVariables do not know about '"
                                                     + type + "'. Use Type.INT instead.");
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
            if (!(o is LocalVariables)) return false;
            var lv = (LocalVariables
                ) o;
            if (locals.Length != lv.locals.Length) return false;
            for (var i = 0; i < locals.Length; i++)
                if (!locals[i].Equals(lv.locals[i]))
                    //System.out.println(this.locals[i]+" is not "+lv.locals[i]);
                    return false;
            return true;
        }

        /// <summary>
        ///     Merges two local variables sets as described in the Java Virtual Machine Specification,
        ///     Second Edition, section 4.9.2, page 146.
        /// </summary>
        /// <param name="localVariable">other local variable.</param>
        public virtual void Merge(LocalVariables localVariable
        )
        {
            if (locals.Length != localVariable.locals.Length)
                throw new AssertionViolatedException(
                    "Merging LocalVariables of different size?!? From different methods or what?!?"
                );
            for (var i = 0; i < locals.Length; i++) Merge(localVariable, i);
        }

        /// <summary>Merges a single local variable.</summary>
        /// <seealso cref="Merge(LocalVariables)" />
        private void Merge(LocalVariables lv, int i)
        {
            try
            {
                // We won't accept an unitialized object if we know it was initialized;
                // compare vmspec2, 4.9.4, last paragraph.
                if (!(locals[i] is UninitializedObjectType) && lv.locals
                        [i] is UninitializedObjectType)
                    throw new StructuralCodeConstraintException(
                        "Backwards branch with an uninitialized object in the local variables detected."
                    );
                // Even harder, what about _different_ uninitialized object types?!
                if (!locals[i].Equals(lv.locals[i]) && locals[i] is UninitializedObjectType &&
                    lv.locals[i] is UninitializedObjectType)
                    throw new StructuralCodeConstraintException(
                        "Backwards branch with an uninitialized object in the local variables detected."
                    );
                // If we just didn't know that it was initialized, we have now learned.
                if (locals[i] is UninitializedObjectType)
                    if (!(lv.locals[i] is UninitializedObjectType))
                        locals[i] = ((UninitializedObjectType) locals[i]).GetInitialized
                            ();
                if (locals[i] is ReferenceType && lv.locals[i] is ReferenceType)
                {
                    if (!locals[i].Equals(lv.locals[i]))
                    {
                        // needed in case of two UninitializedObjectType instances
                        Type sup = ((ReferenceType) locals[i]).GetFirstCommonSuperclass
                            ((ReferenceType) lv.locals[i]);
                        if (sup != null)
                            locals[i] = sup;
                        else
                            // We should have checked this in Pass2!
                            throw new AssertionViolatedException("Could not load all the super classes of '"
                                                                 + locals[i] + "' and '" + lv.locals[i] + "'.");
                    }
                }
                else if (!locals[i].Equals(lv.locals[i]))
                {
                    /*TODO
                    if ((locals[i] instanceof org.apache.bcel.generic.ReturnaddressType) &&
                    (lv.locals[i] instanceof org.apache.bcel.generic.ReturnaddressType)) {
                    //System.err.println("merging "+locals[i]+" and "+lv.locals[i]);
                    throw new AssertionViolatedException("Merging different ReturnAddresses: '"+locals[i]+"' and '"+lv.locals[i]+"'.");
                    }
                    */
                    locals[i] = Type.UNKNOWN;
                }
            }
            catch (TypeLoadException e)
            {
                // FIXME: maybe not the best way to handle this
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>Returns a String representation of this object.</summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < locals.Length; i++)
            {
                sb.Append(i.ToString());
                sb.Append(": ");
                sb.Append(locals[i]);
                sb.Append("\n");
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Replaces all occurrences of
        ///     <paramref name="uninitializedObjectType" />
        ///     in this local variables set
        ///     with an "initialized" ObjectType.
        /// </summary>
        /// <param name="uninitializedObjectType">the object to match.</param>
        public virtual void InitializeObject(UninitializedObjectType
            uninitializedObjectType)
        {
            for (var i = 0; i < locals.Length; i++)
                if (locals[i] == uninitializedObjectType)
                    locals[i] = uninitializedObjectType.GetInitialized();
        }
    }
}