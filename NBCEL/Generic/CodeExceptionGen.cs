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
using Apache.NBCEL.ClassFile;

namespace Apache.NBCEL.Generic
{
	/// <summary>
	///     This class represents an exception handler, i.e., specifies the  region where
	///     a handler is active and an instruction where the actual handling is done.
	/// </summary>
	/// <remarks>
	///     This class represents an exception handler, i.e., specifies the  region where
	///     a handler is active and an instruction where the actual handling is done.
	///     pool as parameters. Opposed to the JVM specification the end of the handled
	///     region is set to be inclusive, i.e. all instructions between start and end
	///     are protected including the start and end instructions (handles) themselves.
	///     The end of the region is automatically mapped to be exclusive when calling
	///     getCodeException(), i.e., there is no difference semantically.
	/// </remarks>
	/// <seealso cref="MethodGen" />
	/// <seealso cref="CodeException" />
	/// <seealso cref="InstructionHandle" />
	public sealed class CodeExceptionGen : InstructionTargeter, ICloneable
    {
        private ObjectType catch_type;

        private InstructionHandle end_pc;

        private InstructionHandle handler_pc;
        private InstructionHandle start_pc;

        /// <summary>
        ///     Add an exception handler, i.e., specify region where a handler is active and an
        ///     instruction where the actual handling is done.
        /// </summary>
        /// <param name="start_pc">Start of handled region (inclusive)</param>
        /// <param name="end_pc">End of handled region (inclusive)</param>
        /// <param name="handler_pc">Where handling is done</param>
        /// <param name="catch_type">which exception is handled, null for ANY</param>
        public CodeExceptionGen(InstructionHandle start_pc, InstructionHandle
                end_pc, InstructionHandle handler_pc, ObjectType catch_type
        )
        {
            SetStartPC(start_pc);
            SetEndPC(end_pc);
            SetHandlerPC(handler_pc);
            this.catch_type = catch_type;
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        /// <param name="old_ih">old target, either start or end</param>
        /// <param name="new_ih">new target</param>
        public void UpdateTarget(InstructionHandle old_ih, InstructionHandle
            new_ih)
        {
            var targeted = false;
            if (start_pc == old_ih)
            {
                targeted = true;
                SetStartPC(new_ih);
            }

            if (end_pc == old_ih)
            {
                targeted = true;
                SetEndPC(new_ih);
            }

            if (handler_pc == old_ih)
            {
                targeted = true;
                SetHandlerPC(new_ih);
            }

            if (!targeted)
                throw new ClassGenException("Not targeting " + old_ih + ", but {" +
                                            start_pc + ", " + end_pc + ", " + handler_pc + "}");
        }

        /// <returns>true, if ih is target of this handler</returns>
        public bool ContainsTarget(InstructionHandle ih)
        {
            return start_pc == ih || end_pc == ih || handler_pc == ih;
        }

        /// <summary>
        ///     Get CodeException object.
        ///     <BR>
        ///         This relies on that the instruction list has already been dumped
        ///         to byte code or or that the `setPositions' methods has been
        ///         called for the instruction list.
        /// </summary>
        /// <param name="cp">constant pool</param>
        public CodeException GetCodeException(ConstantPoolGen
            cp)
        {
            return new CodeException(start_pc.GetPosition(), end_pc.GetPosition
                                                                 () + end_pc.GetInstruction().GetLength(),
                handler_pc.GetPosition(), catch_type
                                          == null
                    ? 0
                    : cp.AddClass(catch_type));
        }

        /* Set start of handler
        * @param start_pc Start of handled region (inclusive)
        */
        public void SetStartPC(InstructionHandle start_pc)
        {
            // TODO could be package-protected?
            BranchInstruction.NotifyTarget(this.start_pc, start_pc, this);
            this.start_pc = start_pc;
        }

        /* Set end of handler
        * @param end_pc End of handled region (inclusive)
        */
        public void SetEndPC(InstructionHandle end_pc)
        {
            // TODO could be package-protected?
            BranchInstruction.NotifyTarget(this.end_pc, end_pc, this);
            this.end_pc = end_pc;
        }

        /* Set handler code
        * @param handler_pc Start of handler
        */
        public void SetHandlerPC(InstructionHandle handler_pc)
        {
            // TODO could be package-protected?
            BranchInstruction.NotifyTarget(this.handler_pc, handler_pc, this);
            this.handler_pc = handler_pc;
        }

        /// <summary>Sets the type of the Exception to catch.</summary>
        /// <remarks>Sets the type of the Exception to catch. Set 'null' for ANY.</remarks>
        public void SetCatchType(ObjectType catch_type)
        {
            this.catch_type = catch_type;
        }

        /// <summary>Gets the type of the Exception to catch, 'null' for ANY.</summary>
        public ObjectType GetCatchType()
        {
            return catch_type;
        }

        /// <returns>start of handled region (inclusive)</returns>
        public InstructionHandle GetStartPC()
        {
            return start_pc;
        }

        /// <returns>end of handled region (inclusive)</returns>
        public InstructionHandle GetEndPC()
        {
            return end_pc;
        }

        /// <returns>start of handler</returns>
        public InstructionHandle GetHandlerPC()
        {
            return handler_pc;
        }

        public override string ToString()
        {
            return "CodeExceptionGen(" + start_pc + ", " + end_pc + ", " + handler_pc + ")";
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        // never happens
    }
}