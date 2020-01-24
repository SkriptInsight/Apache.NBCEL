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
using Apache.NBCEL.Java.IO;
using Apache.NBCEL.Util;

namespace Apache.NBCEL.Generic
{
    /// <summary>Abstract super class for all Java byte codes.</summary>
    public abstract class Instruction : ICloneable
    {
        private static InstructionComparator cmp = InstructionComparator
            .DEFAULT;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal short length = 1;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal short opcode = -1;

        /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
        /// <remarks>
        ///     Empty constructor needed for Instruction.readInstruction.
        ///     Not to be used otherwise.
        /// </remarks>
        internal Instruction()
        {
        }

        public Instruction(short opcode, short length)
        {
            // Length of instruction in bytes
            // Opcode number
            this.length = length;
            this.opcode = opcode;
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>Dump instruction as byte code to stream out.</summary>
        /// <param name="out">Output stream</param>
        /// <exception cref="System.IO.IOException" />
        public virtual void Dump(DataOutputStream @out)
        {
            @out.WriteByte(opcode);
        }

        // Common for all instructions
        /// <returns>name of instruction, i.e., opcode name</returns>
        public virtual string GetName()
        {
            return Const.GetOpcodeName(opcode);
        }

        /// <summary>
        ///     Long output format:
        ///     &lt;name of opcode&gt; "["&lt;opcode number&gt;"]"
        ///     "("&lt;length of instruction&gt;")"
        /// </summary>
        /// <param name="verbose">long/short format switch</param>
        /// <returns>mnemonic for instruction</returns>
        public virtual string ToString(bool verbose)
        {
            if (verbose) return GetName() + "[" + opcode + "](" + length + ")";
            return GetName();
        }

        /// <returns>mnemonic for instruction in verbose format</returns>
        public override string ToString()
        {
            return ToString(true);
        }

        /// <returns>mnemonic for instruction with sumbolic references resolved</returns>
        public virtual string ToString(ConstantPool cp)
        {
            return ToString(false);
        }

        /// <summary>
        ///     Use with caution, since `BranchInstruction's have a `target' reference which
        ///     is not copied correctly (only basic types are).
        /// </summary>
        /// <remarks>
        ///     Use with caution, since `BranchInstruction's have a `target' reference which
        ///     is not copied correctly (only basic types are). This also applies for
        ///     `Select' instructions with their multiple branch targets.
        /// </remarks>
        /// <seealso cref="BranchInstruction" />
        /// <returns>(shallow) copy of an instruction</returns>
        public virtual Instruction Copy()
        {
            Instruction i = null;
            // "Constant" instruction, no need to duplicate
            if (InstructionConst.GetInstruction(GetOpcode()) != null)
                i = this;
            else
                i = (Instruction) MemberwiseClone();
            return i;
        }

        /// <summary>Read needed data (e.g.</summary>
        /// <remarks>Read needed data (e.g. index) from file.</remarks>
        /// <param name="bytes">byte sequence to read from</param>
        /// <param name="wide">"wide" instruction flag</param>
        /// <exception cref="System.IO.IOException">
        ///     may be thrown if the implementation needs to read data from the file
        /// </exception>
        protected internal virtual void InitFromFile(ByteSequence bytes, bool
            wide)
        {
        }

        /// <summary>
        ///     Read an instruction from (byte code) input stream and return the
        ///     appropiate object.
        /// </summary>
        /// <remarks>
        ///     Read an instruction from (byte code) input stream and return the
        ///     appropiate object.
        ///     <p>
        ///         If the Instruction is defined in
        ///         <see cref="InstructionConst" />
        ///         , then the
        ///         singleton instance is returned.
        /// </remarks>
        /// <param name="bytes">input stream bytes</param>
        /// <returns>instruction object being read</returns>
        /// <seealso cref="InstructionConst.GetInstruction(int)" />
        /// <exception cref="System.IO.IOException" />
        public static Instruction ReadInstruction(ByteSequence bytes
        )
        {
            // @since 6.0 no longer final
            var wide = false;
            var opcode = (short) bytes.ReadUnsignedByte();
            Instruction obj = null;
            if (opcode == Const.WIDE)
            {
                // Read next opcode after wide byte
                wide = true;
                opcode = (short) bytes.ReadUnsignedByte();
            }

            var instruction = InstructionConst.GetInstruction
                (opcode);
            if (instruction != null) return instruction;
            switch (opcode)
            {
                case Const.BIPUSH:
                {
                    // Used predefined immutable object, if available
                    obj = new BIPUSH();
                    break;
                }

                case Const.SIPUSH:
                {
                    obj = new SIPUSH();
                    break;
                }

                case Const.LDC:
                {
                    obj = new LDC();
                    break;
                }

                case Const.LDC_W:
                {
                    obj = new LDC_W();
                    break;
                }

                case Const.LDC2_W:
                {
                    obj = new LDC2_W();
                    break;
                }

                case Const.ILOAD:
                {
                    obj = new ILOAD();
                    break;
                }

                case Const.LLOAD:
                {
                    obj = new LLOAD();
                    break;
                }

                case Const.FLOAD:
                {
                    obj = new FLOAD();
                    break;
                }

                case Const.DLOAD:
                {
                    obj = new DLOAD();
                    break;
                }

                case Const.ALOAD:
                {
                    obj = new ALOAD();
                    break;
                }

                case Const.ILOAD_0:
                {
                    obj = new ILOAD(0);
                    break;
                }

                case Const.ILOAD_1:
                {
                    obj = new ILOAD(1);
                    break;
                }

                case Const.ILOAD_2:
                {
                    obj = new ILOAD(2);
                    break;
                }

                case Const.ILOAD_3:
                {
                    obj = new ILOAD(3);
                    break;
                }

                case Const.LLOAD_0:
                {
                    obj = new LLOAD(0);
                    break;
                }

                case Const.LLOAD_1:
                {
                    obj = new LLOAD(1);
                    break;
                }

                case Const.LLOAD_2:
                {
                    obj = new LLOAD(2);
                    break;
                }

                case Const.LLOAD_3:
                {
                    obj = new LLOAD(3);
                    break;
                }

                case Const.FLOAD_0:
                {
                    obj = new FLOAD(0);
                    break;
                }

                case Const.FLOAD_1:
                {
                    obj = new FLOAD(1);
                    break;
                }

                case Const.FLOAD_2:
                {
                    obj = new FLOAD(2);
                    break;
                }

                case Const.FLOAD_3:
                {
                    obj = new FLOAD(3);
                    break;
                }

                case Const.DLOAD_0:
                {
                    obj = new DLOAD(0);
                    break;
                }

                case Const.DLOAD_1:
                {
                    obj = new DLOAD(1);
                    break;
                }

                case Const.DLOAD_2:
                {
                    obj = new DLOAD(2);
                    break;
                }

                case Const.DLOAD_3:
                {
                    obj = new DLOAD(3);
                    break;
                }

                case Const.ALOAD_0:
                {
                    obj = new ALOAD(0);
                    break;
                }

                case Const.ALOAD_1:
                {
                    obj = new ALOAD(1);
                    break;
                }

                case Const.ALOAD_2:
                {
                    obj = new ALOAD(2);
                    break;
                }

                case Const.ALOAD_3:
                {
                    obj = new ALOAD(3);
                    break;
                }

                case Const.ISTORE:
                {
                    obj = new ISTORE();
                    break;
                }

                case Const.LSTORE:
                {
                    obj = new LSTORE();
                    break;
                }

                case Const.FSTORE:
                {
                    obj = new FSTORE();
                    break;
                }

                case Const.DSTORE:
                {
                    obj = new DSTORE();
                    break;
                }

                case Const.ASTORE:
                {
                    obj = new ASTORE();
                    break;
                }

                case Const.ISTORE_0:
                {
                    obj = new ISTORE(0);
                    break;
                }

                case Const.ISTORE_1:
                {
                    obj = new ISTORE(1);
                    break;
                }

                case Const.ISTORE_2:
                {
                    obj = new ISTORE(2);
                    break;
                }

                case Const.ISTORE_3:
                {
                    obj = new ISTORE(3);
                    break;
                }

                case Const.LSTORE_0:
                {
                    obj = new LSTORE(0);
                    break;
                }

                case Const.LSTORE_1:
                {
                    obj = new LSTORE(1);
                    break;
                }

                case Const.LSTORE_2:
                {
                    obj = new LSTORE(2);
                    break;
                }

                case Const.LSTORE_3:
                {
                    obj = new LSTORE(3);
                    break;
                }

                case Const.FSTORE_0:
                {
                    obj = new FSTORE(0);
                    break;
                }

                case Const.FSTORE_1:
                {
                    obj = new FSTORE(1);
                    break;
                }

                case Const.FSTORE_2:
                {
                    obj = new FSTORE(2);
                    break;
                }

                case Const.FSTORE_3:
                {
                    obj = new FSTORE(3);
                    break;
                }

                case Const.DSTORE_0:
                {
                    obj = new DSTORE(0);
                    break;
                }

                case Const.DSTORE_1:
                {
                    obj = new DSTORE(1);
                    break;
                }

                case Const.DSTORE_2:
                {
                    obj = new DSTORE(2);
                    break;
                }

                case Const.DSTORE_3:
                {
                    obj = new DSTORE(3);
                    break;
                }

                case Const.ASTORE_0:
                {
                    obj = new ASTORE(0);
                    break;
                }

                case Const.ASTORE_1:
                {
                    obj = new ASTORE(1);
                    break;
                }

                case Const.ASTORE_2:
                {
                    obj = new ASTORE(2);
                    break;
                }

                case Const.ASTORE_3:
                {
                    obj = new ASTORE(3);
                    break;
                }

                case Const.IINC:
                {
                    obj = new IINC();
                    break;
                }

                case Const.IFEQ:
                {
                    obj = new IFEQ();
                    break;
                }

                case Const.IFNE:
                {
                    obj = new IFNE();
                    break;
                }

                case Const.IFLT:
                {
                    obj = new IFLT();
                    break;
                }

                case Const.IFGE:
                {
                    obj = new IFGE();
                    break;
                }

                case Const.IFGT:
                {
                    obj = new IFGT();
                    break;
                }

                case Const.IFLE:
                {
                    obj = new IFLE();
                    break;
                }

                case Const.IF_ICMPEQ:
                {
                    obj = new IF_ICMPEQ();
                    break;
                }

                case Const.IF_ICMPNE:
                {
                    obj = new IF_ICMPNE();
                    break;
                }

                case Const.IF_ICMPLT:
                {
                    obj = new IF_ICMPLT();
                    break;
                }

                case Const.IF_ICMPGE:
                {
                    obj = new IF_ICMPGE();
                    break;
                }

                case Const.IF_ICMPGT:
                {
                    obj = new IF_ICMPGT();
                    break;
                }

                case Const.IF_ICMPLE:
                {
                    obj = new IF_ICMPLE();
                    break;
                }

                case Const.IF_ACMPEQ:
                {
                    obj = new IF_ACMPEQ();
                    break;
                }

                case Const.IF_ACMPNE:
                {
                    obj = new IF_ACMPNE();
                    break;
                }

                case Const.GOTO:
                {
                    obj = new GOTO();
                    break;
                }

                case Const.JSR:
                {
                    obj = new JSR();
                    break;
                }

                case Const.RET:
                {
                    obj = new RET();
                    break;
                }

                case Const.TABLESWITCH:
                {
                    obj = new TABLESWITCH();
                    break;
                }

                case Const.LOOKUPSWITCH:
                {
                    obj = new LOOKUPSWITCH();
                    break;
                }

                case Const.GETSTATIC:
                {
                    obj = new GETSTATIC();
                    break;
                }

                case Const.PUTSTATIC:
                {
                    obj = new PUTSTATIC();
                    break;
                }

                case Const.GETFIELD:
                {
                    obj = new GETFIELD();
                    break;
                }

                case Const.PUTFIELD:
                {
                    obj = new PUTFIELD();
                    break;
                }

                case Const.INVOKEVIRTUAL:
                {
                    obj = new INVOKEVIRTUAL();
                    break;
                }

                case Const.INVOKESPECIAL:
                {
                    obj = new INVOKESPECIAL();
                    break;
                }

                case Const.INVOKESTATIC:
                {
                    obj = new INVOKESTATIC();
                    break;
                }

                case Const.INVOKEINTERFACE:
                {
                    obj = new INVOKEINTERFACE();
                    break;
                }

                case Const.INVOKEDYNAMIC:
                {
                    obj = new INVOKEDYNAMIC();
                    break;
                }

                case Const.NEW:
                {
                    obj = new NEW();
                    break;
                }

                case Const.NEWARRAY:
                {
                    obj = new NEWARRAY();
                    break;
                }

                case Const.ANEWARRAY:
                {
                    obj = new ANEWARRAY();
                    break;
                }

                case Const.CHECKCAST:
                {
                    obj = new CHECKCAST();
                    break;
                }

                case Const.INSTANCEOF:
                {
                    obj = new INSTANCEOF();
                    break;
                }

                case Const.MULTIANEWARRAY:
                {
                    obj = new MULTIANEWARRAY();
                    break;
                }

                case Const.IFNULL:
                {
                    obj = new IFNULL();
                    break;
                }

                case Const.IFNONNULL:
                {
                    obj = new IFNONNULL();
                    break;
                }

                case Const.GOTO_W:
                {
                    obj = new GOTO_W();
                    break;
                }

                case Const.JSR_W:
                {
                    obj = new JSR_W();
                    break;
                }

                case Const.BREAKPOINT:
                {
                    obj = new BREAKPOINT();
                    break;
                }

                case Const.IMPDEP1:
                {
                    obj = new IMPDEP1();
                    break;
                }

                case Const.IMPDEP2:
                {
                    obj = new IMPDEP2();
                    break;
                }

                default:
                {
                    throw new ClassGenException("Illegal opcode detected: " + opcode);
                }
            }

            if (wide && !(obj is LocalVariableInstruction || obj is IINC || obj is RET))
                throw new ClassGenException("Illegal opcode after wide: " + opcode);
            obj.SetOpcode(opcode);
            obj.InitFromFile(bytes, wide);
            // Do further initializations, if any
            return obj;
        }

        /// <summary>
        ///     This method also gives right results for instructions whose
        ///     effect on the stack depends on the constant pool entry they
        ///     reference.
        /// </summary>
        /// <returns>
        ///     Number of words consumed from stack by this instruction,
        ///     or Constants.UNPREDICTABLE, if this can not be computed statically
        /// </returns>
        public virtual int ConsumeStack(ConstantPoolGen cpg)
        {
            return Const.GetConsumeStack(opcode);
        }

        /// <summary>
        ///     This method also gives right results for instructions whose
        ///     effect on the stack depends on the constant pool entry they
        ///     reference.
        /// </summary>
        /// <returns>
        ///     Number of words produced onto stack by this instruction,
        ///     or Constants.UNPREDICTABLE, if this can not be computed statically
        /// </returns>
        public virtual int ProduceStack(ConstantPoolGen cpg)
        {
            return Const.GetProduceStack(opcode);
        }

        /// <returns>this instructions opcode</returns>
        public virtual short GetOpcode()
        {
            return opcode;
        }

        /// <returns>length (in bytes) of instruction</returns>
        public virtual int GetLength()
        {
            return length;
        }

        /// <summary>Needed in readInstruction and subclasses in this package</summary>
        internal void SetOpcode(short opcode)
        {
            this.opcode = opcode;
        }

        /// <summary>Needed in readInstruction and subclasses in this package</summary>
        /// <since>6.0</since>
        internal void SetLength(int length)
        {
            this.length = (short) length;
        }

        // TODO check range?
        /// <summary>Some instructions may be reused, so don't do anything by default.</summary>
        internal virtual void Dispose()
        {
        }

        /// <summary>Call corresponding visitor method(s).</summary>
        /// <remarks>
        ///     Call corresponding visitor method(s). The order is:
        ///     Call visitor methods of implemented interfaces first, then
        ///     call methods according to the class hierarchy in descending order,
        ///     i.e., the most specific visitXXX() call comes last.
        /// </remarks>
        /// <param name="v">Visitor object</param>
        public abstract void Accept(Visitor v);

        /// <summary>
        ///     Get Comparator object used in the equals() method to determine
        ///     equality of instructions.
        /// </summary>
        /// <returns>currently used comparator for equals()</returns>
        [Obsolete(
            @"(6.0) use the built in comparator, or wrap this class in another object that implements these methods"
        )]
        public static InstructionComparator GetComparator()
        {
            return cmp;
        }

        /// <summary>Set comparator to be used for equals().</summary>
        [Obsolete(
            @"(6.0) use the built in comparator, or wrap this class in another object that implements these methods"
        )]
        public static void SetComparator(InstructionComparator c)
        {
            cmp = c;
        }

        /// <summary>Check for equality, delegated to comparator</summary>
        /// <returns>true if that is an Instruction and has the same opcode</returns>
        public override bool Equals(object that)
        {
            return that is Instruction
                ? cmp.Equals(this, (Instruction
                    ) that)
                : false;
        }

        /// <summary>calculate the hashCode of this object</summary>
        /// <returns>the hashCode</returns>
        /// <since>6.0</since>
        public override int GetHashCode()
        {
            return opcode;
        }

        /// <summary>Check if the value can fit in a byte (signed)</summary>
        /// <param name="value">the value to check</param>
        /// <returns>true if the value is in range</returns>
        /// <since>6.0</since>
        public static bool IsValidByte(int value)
        {
            return value >= byte.MinValue && value <= byte.MaxValue;
        }

        /// <summary>Check if the value can fit in a short (signed)</summary>
        /// <param name="value">the value to check</param>
        /// <returns>true if the value is in range</returns>
        /// <since>6.0</since>
        public static bool IsValidShort(int value)
        {
            return value >= short.MinValue && value <= short.MaxValue;
        }
    }
}