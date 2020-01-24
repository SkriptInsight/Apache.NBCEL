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
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>
	///     This class represents an entry in the exception table of the <em>Code</em>
	///     attribute and is used only there.
	/// </summary>
	/// <remarks>
	///     This class represents an entry in the exception table of the <em>Code</em>
	///     attribute and is used only there. It contains a range in which a
	///     particular exception handler is active.
	/// </remarks>
	/// <seealso cref="Code" />
	public sealed class CodeException : ICloneable, Node
    {
        private int catch_type;

        private int end_pc;

        private int handler_pc;
        private int start_pc;

        /// <summary>Initialize from another object.</summary>
        public CodeException(CodeException c)
            : this(c.GetStartPC(), c.GetEndPC(), c.GetHandlerPC(), c.GetCatchType())
        {
        }

        /// <summary>Construct object from file stream.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal CodeException(DataInput file)
            : this(file.ReadUnsignedShort(), file.ReadUnsignedShort(), file.ReadUnsignedShort
                (), file.ReadUnsignedShort())
        {
        }

        /// <param name="start_pc">
        ///     Range in the code the exception handler is active,
        ///     start_pc is inclusive while
        /// </param>
        /// <param name="end_pc">is exclusive</param>
        /// <param name="handler_pc">
        ///     Starting address of exception handler, i.e.,
        ///     an offset from start of code.
        /// </param>
        /// <param name="catch_type">
        ///     If zero the handler catches any
        ///     exception, otherwise it points to the exception class which is
        ///     to be caught.
        /// </param>
        public CodeException(int start_pc, int end_pc, int handler_pc, int catch_type)
        {
            // Range in the code the exception handler is
            // active. start_pc is inclusive, end_pc exclusive
            /* Starting address of exception handler, i.e.,
            * an offset from start of code.
            */
            /* If this is zero the handler catches any
            * exception, otherwise it points to the
            * exception class which is to be caught.
            */
            this.start_pc = start_pc;
            this.end_pc = end_pc;
            this.handler_pc = handler_pc;
            this.catch_type = catch_type;
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        ///     Called by objects that are traversing the nodes of the tree implicitely
        ///     defined by the contents of a Java class.
        /// </summary>
        /// <remarks>
        ///     Called by objects that are traversing the nodes of the tree implicitely
        ///     defined by the contents of a Java class. I.e., the hierarchy of methods,
        ///     fields, attributes, etc. spawns a tree of objects.
        /// </remarks>
        /// <param name="v">Visitor object</param>
        public void Accept(Visitor v)
        {
            v.VisitCodeException(this);
        }

        /// <summary>Dump code exception to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public void Dump(DataOutputStream file)
        {
            file.WriteShort(start_pc);
            file.WriteShort(end_pc);
            file.WriteShort(handler_pc);
            file.WriteShort(catch_type);
        }

        /// <returns>
        ///     0, if the handler catches any exception, otherwise it points to
        ///     the exception class which is to be caught.
        /// </returns>
        public int GetCatchType()
        {
            return catch_type;
        }

        /// <returns>Exclusive end index of the region where the handler is active.</returns>
        public int GetEndPC()
        {
            return end_pc;
        }

        /// <returns>Starting address of exception handler, relative to the code.</returns>
        public int GetHandlerPC()
        {
            return handler_pc;
        }

        /// <returns>Inclusive start index of the region where the handler is active.</returns>
        public int GetStartPC()
        {
            return start_pc;
        }

        /// <param name="catch_type">the type of exception that is caught</param>
        public void SetCatchType(int catch_type)
        {
            this.catch_type = catch_type;
        }

        /// <param name="end_pc">end of handled block</param>
        public void SetEndPC(int end_pc)
        {
            this.end_pc = end_pc;
        }

        /// <param name="handler_pc">where the actual code is</param>
        public void SetHandlerPC(int handler_pc)
        {
            // TODO unused
            this.handler_pc = handler_pc;
        }

        /// <param name="start_pc">start of handled block</param>
        public void SetStartPC(int start_pc)
        {
            // TODO unused
            this.start_pc = start_pc;
        }

        /// <returns>String representation.</returns>
        public override string ToString()
        {
            return "CodeException(start_pc = " + start_pc + ", end_pc = " + end_pc + ", handler_pc = "
                   + handler_pc + ", catch_type = " + catch_type + ")";
        }

        /// <returns>String representation.</returns>
        public string ToString(ConstantPool cp, bool verbose)
        {
            string str;
            if (catch_type == 0)
                str = "<Any exception>(0)";
            else
                str = Utility.CompactClassName(cp.GetConstantString(catch_type, Const
                          .CONSTANT_Class), false) + (verbose ? "(" + catch_type + ")" : string.Empty);
            return start_pc + "\t" + end_pc + "\t" + handler_pc + "\t" + str;
        }

        public string ToString(ConstantPool cp)
        {
            return ToString(cp, true);
        }

        /// <returns>deep copy of this object</returns>
        public CodeException Copy()
        {
            return (CodeException) MemberwiseClone();
            // TODO should this throw?
            return null;
        }
    }
}