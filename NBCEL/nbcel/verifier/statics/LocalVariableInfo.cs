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

using System.Collections.Generic;
using NBCEL.generic;
using NBCEL.verifier.exc;
using Sharpen;

namespace NBCEL.verifier.statics
{
	/// <summary>
	///     A utility class holding the information about
	///     the name and the type of a local variable in
	///     a given slot (== index).
	/// </summary>
	/// <remarks>
	///     A utility class holding the information about
	///     the name and the type of a local variable in
	///     a given slot (== index). This information
	///     often changes in course of byte code offsets.
	/// </remarks>
	public class LocalVariableInfo
    {
	    /// <summary>The names database.</summary>
	    /// <remarks>The names database. KEY: String representing the offset integer.</remarks>
	    private readonly Dictionary<string, string> names = new Dictionary
            <string, string>();

	    /// <summary>The types database.</summary>
	    /// <remarks>The types database. KEY: String representing the offset integer.</remarks>
	    private readonly Dictionary<string, Type> types = new Dictionary
            <string, Type>();

	    /// <summary>
	    ///     Adds a name of a local variable and a certain slot to our 'names'
	    ///     (Hashtable) database.
	    /// </summary>
	    private void SetName(int offset, string name)
        {
            names[offset.ToString()] = name;
        }

	    /// <summary>
	    ///     Adds a type of a local variable and a certain slot to our 'types'
	    ///     (Hashtable) database.
	    /// </summary>
	    private void SetType(int offset, Type t)
        {
            types[offset.ToString()] = t;
        }

	    /// <summary>
	    ///     Returns the type of the local variable that uses this local variable slot at the given bytecode offset.
	    /// </summary>
	    /// <remarks>
	    ///     Returns the type of the local variable that uses this local variable slot at the given bytecode offset. Care for
	    ///     legal bytecode offsets yourself, otherwise the return value might be wrong. May return 'null' if nothing is known
	    ///     about the type of this local variable slot at the given bytecode offset.
	    /// </remarks>
	    /// <param name="offset">bytecode offset.</param>
	    /// <returns>
	    ///     the type of the local variable that uses this local variable slot at the given bytecode offset.
	    /// </returns>
	    public virtual Type GetType(int offset)
        {
            return types.GetOrNull(offset.ToString());
        }

	    /// <summary>
	    ///     Returns the name of the local variable that uses this local variable slot at the given bytecode offset.
	    /// </summary>
	    /// <remarks>
	    ///     Returns the name of the local variable that uses this local variable slot at the given bytecode offset. Care for
	    ///     legal bytecode offsets yourself, otherwise the return value might be wrong. May return 'null' if nothing is known
	    ///     about the type of this local variable slot at the given bytecode offset.
	    /// </remarks>
	    /// <param name="offset">bytecode offset.</param>
	    /// <returns>
	    ///     the name of the local variable that uses this local variable slot at the given bytecode offset.
	    /// </returns>
	    public virtual string GetName(int offset)
        {
            return names.GetOrNull(offset.ToString());
        }

	    /// <summary>Adds some information about this local variable (slot).</summary>
	    /// <param name="name">variable name</param>
	    /// <param name="startPc">Range in which the variable is valid.</param>
	    /// <param name="length">length of ...</param>
	    /// <param name="type">variable type</param>
	    /// <exception cref="NBCEL.verifier.exc.LocalVariableInfoInconsistentException">
	    ///     if the new information conflicts
	    ///     with already gathered information.
	    /// </exception>
	    public virtual void Add(string name, int startPc, int length, Type
            type)
        {
            for (var i = startPc; i <= startPc + length; i++)
                // incl/incl-notation!
                Add(i, name, type);
        }

	    /// <summary>Adds information about name and type for a given offset.</summary>
	    /// <exception cref="NBCEL.verifier.exc.LocalVariableInfoInconsistentException">
	    ///     if the new information conflicts
	    ///     with already gathered information.
	    /// </exception>
	    private void Add(int offset, string name, Type t)
        {
            if (GetName(offset) != null)
                if (!GetName(offset).Equals(name))
                    throw new LocalVariableInfoInconsistentException("At bytecode offset '"
                                                                     + offset +
                                                                     "' a local variable has two different names: '" +
                                                                     GetName(offset) +
                                                                     "' and '" + name + "'.");
            if (GetType(offset) != null)
                if (!GetType(offset).Equals(t))
                    throw new LocalVariableInfoInconsistentException("At bytecode offset '"
                                                                     + offset +
                                                                     "' a local variable has two different types: '" +
                                                                     GetType(offset) +
                                                                     "' and '" + t + "'.");
            SetName(offset, name);
            SetType(offset, t);
        }
    }
}