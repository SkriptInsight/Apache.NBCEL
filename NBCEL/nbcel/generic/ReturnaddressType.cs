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

namespace NBCEL.generic
{
	/// <summary>Returnaddress, the type JSR or JSR_W instructions push upon the stack.</summary>
	/// <remarks>
	///     Returnaddress, the type JSR or JSR_W instructions push upon the stack.
	///     see vmspec2 �3.3.3
	/// </remarks>
	public class ReturnaddressType : Type
    {
        public static readonly ReturnaddressType NO_TARGET = new ReturnaddressType
            ();

        private readonly InstructionHandle returnTarget;

        /// <summary>A Returnaddress [that doesn't know where to return to].</summary>
        private ReturnaddressType()
            : base(Const.T_ADDRESS, "<return address>")
        {
        }

        /// <summary>Creates a ReturnaddressType object with a target.</summary>
        public ReturnaddressType(InstructionHandle returnTarget)
            : base(Const.T_ADDRESS, "<return address targeting " + returnTarget + ">")
        {
            this.returnTarget = returnTarget;
        }

        /// <returns>a hash code value for the object.</returns>
        public override int GetHashCode()
        {
            if (returnTarget == null) return 0;
            return returnTarget.GetHashCode();
        }

        /// <summary>Returns if the two Returnaddresses refer to the same target.</summary>
        public override bool Equals(object rat)
        {
            if (!(rat is ReturnaddressType)) return false;
            var that = (ReturnaddressType) rat;
            if (returnTarget == null || that.returnTarget == null) return that.returnTarget == returnTarget;
            return that.returnTarget.Equals(returnTarget);
        }

        /// <returns>the target of this ReturnaddressType</returns>
        public virtual InstructionHandle GetTarget()
        {
            return returnTarget;
        }
    }
}