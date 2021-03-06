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

using Apache.NBCEL.Generic;
using Apache.NBCEL.Verifier.Exc;

namespace Apache.NBCEL.Verifier.Statics
{
	/// <summary>
	///     A utility class holding the information about
	///     the names and the types of the local variables in
	///     a given method.
	/// </summary>
	public class LocalVariablesInfo
    {
        /// <summary>The information about the local variables is stored here.</summary>
        private readonly LocalVariableInfo[] localVariableInfos;

        /// <summary>The constructor.</summary>
        internal LocalVariablesInfo(int max_locals)
        {
            localVariableInfos = new LocalVariableInfo[max_locals];
            for (var i = 0; i < max_locals; i++) localVariableInfos[i] = new LocalVariableInfo();
        }

        /// <summary>Returns the LocalVariableInfo for the given slot.</summary>
        /// <param name="slot">Slot to query.</param>
        /// <returns>The LocalVariableInfo for the given slot.</returns>
        public virtual LocalVariableInfo GetLocalVariableInfo(int
            slot)
        {
            if (slot < 0 || slot >= localVariableInfos.Length)
                throw new AssertionViolatedException("Slot number for local variable information out of range."
                );
            return localVariableInfos[slot];
        }

        /// <summary>Adds information about the local variable in slot 'slot'.</summary>
        /// <remarks>
        ///     Adds information about the local variable in slot 'slot'. Automatically
        ///     adds information for slot+1 if 't' is Type.LONG or Type.DOUBLE.
        /// </remarks>
        /// <param name="name">variable name</param>
        /// <param name="startPc">Range in which the variable is valid.</param>
        /// <param name="length">length of ...</param>
        /// <param name="type">variable type</param>
        /// <exception cref="LocalVariableInfoInconsistentException">
        ///     if the new information conflicts
        ///     with already gathered information.
        /// </exception>
        public virtual void Add(int slot, string name, int startPc, int length, Type
            type)
        {
            // The add operation on LocalVariableInfo may throw the '...Inconsistent...' exception, we don't throw it explicitely here.
            if (slot < 0 || slot >= localVariableInfos.Length)
                throw new AssertionViolatedException("Slot number for local variable information out of range."
                );
            localVariableInfos[slot].Add(name, startPc, length, type);
            if (type == Type.LONG)
                localVariableInfos[slot + 1].Add(name, startPc, length, LONG_Upper
                    .TheInstance());
            if (type == Type.DOUBLE)
                localVariableInfos[slot + 1].Add(name, startPc, length, DOUBLE_Upper
                    .TheInstance());
        }
    }
}