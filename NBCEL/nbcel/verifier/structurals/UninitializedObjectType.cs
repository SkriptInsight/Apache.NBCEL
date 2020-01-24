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

using NBCEL.generic;

namespace NBCEL.verifier.structurals
{
	/// <summary>
	///     This class represents an uninitialized object type; see The Java
	///     Virtual Machine Specification, Second Edition, page 147: 4.9.4 for
	///     more details.
	/// </summary>
	public class UninitializedObjectType : ReferenceType
    {
        /// <summary>The "initialized" version.</summary>
        private readonly ObjectType initialized;

        /// <summary>Creates a new instance.</summary>
        public UninitializedObjectType(ObjectType t)
            : base(Const.T_UNKNOWN, "<UNINITIALIZED OBJECT OF TYPE '" + t.GetClassName(
                                    ) + "'>")
        {
            initialized = t;
        }

        /// <summary>
        ///     Returns the ObjectType of the same class as the one of the uninitialized object
        ///     represented by this UninitializedObjectType instance.
        /// </summary>
        public virtual ObjectType GetInitialized()
        {
            return initialized;
        }

        /// <returns>a hash code value for the object.</returns>
        public override int GetHashCode()
        {
            return initialized.GetHashCode();
        }

        /// <summary>Returns true on equality of this and o.</summary>
        /// <remarks>
        ///     Returns true on equality of this and o.
        ///     Equality means the ObjectType instances of "initialized"
        ///     equal one another in this and the o instance.
        /// </remarks>
        public override bool Equals(object o)
        {
            if (!(o is UninitializedObjectType)) return false;
            return initialized.Equals(((UninitializedObjectType) o)
                .initialized);
        }
    }
}