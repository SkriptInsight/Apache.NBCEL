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
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	///     This class represents a (PC offset, line number) pair, i.e., a line number in
	///     the source that corresponds to a relative address in the byte code.
	/// </summary>
	/// <remarks>
	///     This class represents a (PC offset, line number) pair, i.e., a line number in
	///     the source that corresponds to a relative address in the byte code. This
	///     is used for debugging purposes.
	/// </remarks>
	/// <seealso cref="LineNumberTable" />
	public sealed class LineNumber : ICloneable, Node
    {
        /// <summary>number in source file</summary>
        private short line_number;

        /// <summary>Program Counter (PC) corresponds to line</summary>
        private short start_pc;

        /// <summary>Initialize from another object.</summary>
        /// <param name="c">the object to copy</param>
        public LineNumber(LineNumber c)
            : this(c.GetStartPC(), c.GetLineNumber())
        {
        }

        /// <summary>Construct object from file stream.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException">
        ///     if an I/O Exception occurs in readUnsignedShort
        /// </exception>
        internal LineNumber(DataInput file)
            : this(file.ReadUnsignedShort(), file.ReadUnsignedShort())
        {
        }

        /// <param name="start_pc">Program Counter (PC) corresponds to</param>
        /// <param name="line_number">line number in source file</param>
        public LineNumber(int start_pc, int line_number)
        {
            this.start_pc = (short) start_pc;
            this.line_number = (short) line_number;
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
            v.VisitLineNumber(this);
        }

        /// <summary>Dump line number/pc pair to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException">if an I/O Exception occurs in writeShort</exception>
        public void Dump(DataOutputStream file)
        {
            file.WriteShort(start_pc);
            file.WriteShort(line_number);
        }

        /// <returns>Corresponding source line</returns>
        public int GetLineNumber()
        {
            return 0xffff & line_number;
        }

        /// <returns>PC in code</returns>
        public int GetStartPC()
        {
            return 0xffff & start_pc;
        }

        /// <param name="line_number">the source line number</param>
        public void SetLineNumber(int line_number)
        {
            this.line_number = (short) line_number;
        }

        /// <param name="start_pc">the pc for this line number</param>
        public void SetStartPC(int start_pc)
        {
            this.start_pc = (short) start_pc;
        }

        /// <returns>String representation</returns>
        public override string ToString()
        {
            return "LineNumber(" + start_pc + ", " + line_number + ")";
        }

        /// <returns>deep copy of this object</returns>
        public LineNumber Copy()
        {
            return (LineNumber) MemberwiseClone();
            // TODO should this throw?
            return null;
        }
    }
}