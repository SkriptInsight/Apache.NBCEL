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

namespace NBCEL.generic
{
	/// <summary>Abstract super class for all Java byte codes.</summary>
	public abstract class Instruction : System.ICloneable
	{
		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal short length = 1;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal short opcode = -1;

		private static NBCEL.generic.InstructionComparator cmp = NBCEL.generic.InstructionComparator
			.DEFAULT;

		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
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

		/// <summary>Dump instruction as byte code to stream out.</summary>
		/// <param name="out">Output stream</param>
		/// <exception cref="System.IO.IOException"/>
		public virtual void Dump(java.io.DataOutputStream @out)
		{
			@out.WriteByte(opcode);
		}

		// Common for all instructions
		/// <returns>name of instruction, i.e., opcode name</returns>
		public virtual string GetName()
		{
			return NBCEL.Const.GetOpcodeName(opcode);
		}

		/// <summary>
		/// Long output format:
		/// &lt;name of opcode&gt; "["&lt;opcode number&gt;"]"
		/// "("&lt;length of instruction&gt;")"
		/// </summary>
		/// <param name="verbose">long/short format switch</param>
		/// <returns>mnemonic for instruction</returns>
		public virtual string ToString(bool verbose)
		{
			if (verbose)
			{
				return GetName() + "[" + opcode + "](" + length + ")";
			}
			return GetName();
		}

		/// <returns>mnemonic for instruction in verbose format</returns>
		public override string ToString()
		{
			return ToString(true);
		}

		/// <returns>mnemonic for instruction with sumbolic references resolved</returns>
		public virtual string ToString(NBCEL.classfile.ConstantPool cp)
		{
			return ToString(false);
		}

		/// <summary>
		/// Use with caution, since `BranchInstruction's have a `target' reference which
		/// is not copied correctly (only basic types are).
		/// </summary>
		/// <remarks>
		/// Use with caution, since `BranchInstruction's have a `target' reference which
		/// is not copied correctly (only basic types are). This also applies for
		/// `Select' instructions with their multiple branch targets.
		/// </remarks>
		/// <seealso cref="BranchInstruction"/>
		/// <returns>(shallow) copy of an instruction</returns>
		public virtual NBCEL.generic.Instruction Copy()
		{
			NBCEL.generic.Instruction i = null;
			// "Constant" instruction, no need to duplicate
			if (NBCEL.generic.InstructionConst.GetInstruction(this.GetOpcode()) != null)
			{
				i = this;
			}
			else
			{
				i = (NBCEL.generic.Instruction)MemberwiseClone();
			}
			return i;
		}

		/// <summary>Read needed data (e.g.</summary>
		/// <remarks>Read needed data (e.g. index) from file.</remarks>
		/// <param name="bytes">byte sequence to read from</param>
		/// <param name="wide">"wide" instruction flag</param>
		/// <exception cref="System.IO.IOException">may be thrown if the implementation needs to read data from the file
		/// 	</exception>
		protected internal virtual void InitFromFile(NBCEL.util.ByteSequence bytes, bool 
			wide)
		{
		}

		/// <summary>
		/// Read an instruction from (byte code) input stream and return the
		/// appropiate object.
		/// </summary>
		/// <remarks>
		/// Read an instruction from (byte code) input stream and return the
		/// appropiate object.
		/// <p>
		/// If the Instruction is defined in
		/// <see cref="InstructionConst"/>
		/// , then the
		/// singleton instance is returned.
		/// </remarks>
		/// <param name="bytes">input stream bytes</param>
		/// <returns>instruction object being read</returns>
		/// <seealso cref="InstructionConst.GetInstruction(int)"/>
		/// <exception cref="System.IO.IOException"/>
		public static NBCEL.generic.Instruction ReadInstruction(NBCEL.util.ByteSequence bytes
			)
		{
			// @since 6.0 no longer final
			bool wide = false;
			short opcode = (short)bytes.ReadUnsignedByte();
			NBCEL.generic.Instruction obj = null;
			if (opcode == NBCEL.Const.WIDE)
			{
				// Read next opcode after wide byte
				wide = true;
				opcode = (short)bytes.ReadUnsignedByte();
			}
			NBCEL.generic.Instruction instruction = NBCEL.generic.InstructionConst.GetInstruction
				(opcode);
			if (instruction != null)
			{
				return instruction;
			}
			switch (opcode)
			{
				case NBCEL.Const.BIPUSH:
				{
					// Used predefined immutable object, if available
					obj = new NBCEL.generic.BIPUSH();
					break;
				}

				case NBCEL.Const.SIPUSH:
				{
					obj = new NBCEL.generic.SIPUSH();
					break;
				}

				case NBCEL.Const.LDC:
				{
					obj = new NBCEL.generic.LDC();
					break;
				}

				case NBCEL.Const.LDC_W:
				{
					obj = new NBCEL.generic.LDC_W();
					break;
				}

				case NBCEL.Const.LDC2_W:
				{
					obj = new NBCEL.generic.LDC2_W();
					break;
				}

				case NBCEL.Const.ILOAD:
				{
					obj = new NBCEL.generic.ILOAD();
					break;
				}

				case NBCEL.Const.LLOAD:
				{
					obj = new NBCEL.generic.LLOAD();
					break;
				}

				case NBCEL.Const.FLOAD:
				{
					obj = new NBCEL.generic.FLOAD();
					break;
				}

				case NBCEL.Const.DLOAD:
				{
					obj = new NBCEL.generic.DLOAD();
					break;
				}

				case NBCEL.Const.ALOAD:
				{
					obj = new NBCEL.generic.ALOAD();
					break;
				}

				case NBCEL.Const.ILOAD_0:
				{
					obj = new NBCEL.generic.ILOAD(0);
					break;
				}

				case NBCEL.Const.ILOAD_1:
				{
					obj = new NBCEL.generic.ILOAD(1);
					break;
				}

				case NBCEL.Const.ILOAD_2:
				{
					obj = new NBCEL.generic.ILOAD(2);
					break;
				}

				case NBCEL.Const.ILOAD_3:
				{
					obj = new NBCEL.generic.ILOAD(3);
					break;
				}

				case NBCEL.Const.LLOAD_0:
				{
					obj = new NBCEL.generic.LLOAD(0);
					break;
				}

				case NBCEL.Const.LLOAD_1:
				{
					obj = new NBCEL.generic.LLOAD(1);
					break;
				}

				case NBCEL.Const.LLOAD_2:
				{
					obj = new NBCEL.generic.LLOAD(2);
					break;
				}

				case NBCEL.Const.LLOAD_3:
				{
					obj = new NBCEL.generic.LLOAD(3);
					break;
				}

				case NBCEL.Const.FLOAD_0:
				{
					obj = new NBCEL.generic.FLOAD(0);
					break;
				}

				case NBCEL.Const.FLOAD_1:
				{
					obj = new NBCEL.generic.FLOAD(1);
					break;
				}

				case NBCEL.Const.FLOAD_2:
				{
					obj = new NBCEL.generic.FLOAD(2);
					break;
				}

				case NBCEL.Const.FLOAD_3:
				{
					obj = new NBCEL.generic.FLOAD(3);
					break;
				}

				case NBCEL.Const.DLOAD_0:
				{
					obj = new NBCEL.generic.DLOAD(0);
					break;
				}

				case NBCEL.Const.DLOAD_1:
				{
					obj = new NBCEL.generic.DLOAD(1);
					break;
				}

				case NBCEL.Const.DLOAD_2:
				{
					obj = new NBCEL.generic.DLOAD(2);
					break;
				}

				case NBCEL.Const.DLOAD_3:
				{
					obj = new NBCEL.generic.DLOAD(3);
					break;
				}

				case NBCEL.Const.ALOAD_0:
				{
					obj = new NBCEL.generic.ALOAD(0);
					break;
				}

				case NBCEL.Const.ALOAD_1:
				{
					obj = new NBCEL.generic.ALOAD(1);
					break;
				}

				case NBCEL.Const.ALOAD_2:
				{
					obj = new NBCEL.generic.ALOAD(2);
					break;
				}

				case NBCEL.Const.ALOAD_3:
				{
					obj = new NBCEL.generic.ALOAD(3);
					break;
				}

				case NBCEL.Const.ISTORE:
				{
					obj = new NBCEL.generic.ISTORE();
					break;
				}

				case NBCEL.Const.LSTORE:
				{
					obj = new NBCEL.generic.LSTORE();
					break;
				}

				case NBCEL.Const.FSTORE:
				{
					obj = new NBCEL.generic.FSTORE();
					break;
				}

				case NBCEL.Const.DSTORE:
				{
					obj = new NBCEL.generic.DSTORE();
					break;
				}

				case NBCEL.Const.ASTORE:
				{
					obj = new NBCEL.generic.ASTORE();
					break;
				}

				case NBCEL.Const.ISTORE_0:
				{
					obj = new NBCEL.generic.ISTORE(0);
					break;
				}

				case NBCEL.Const.ISTORE_1:
				{
					obj = new NBCEL.generic.ISTORE(1);
					break;
				}

				case NBCEL.Const.ISTORE_2:
				{
					obj = new NBCEL.generic.ISTORE(2);
					break;
				}

				case NBCEL.Const.ISTORE_3:
				{
					obj = new NBCEL.generic.ISTORE(3);
					break;
				}

				case NBCEL.Const.LSTORE_0:
				{
					obj = new NBCEL.generic.LSTORE(0);
					break;
				}

				case NBCEL.Const.LSTORE_1:
				{
					obj = new NBCEL.generic.LSTORE(1);
					break;
				}

				case NBCEL.Const.LSTORE_2:
				{
					obj = new NBCEL.generic.LSTORE(2);
					break;
				}

				case NBCEL.Const.LSTORE_3:
				{
					obj = new NBCEL.generic.LSTORE(3);
					break;
				}

				case NBCEL.Const.FSTORE_0:
				{
					obj = new NBCEL.generic.FSTORE(0);
					break;
				}

				case NBCEL.Const.FSTORE_1:
				{
					obj = new NBCEL.generic.FSTORE(1);
					break;
				}

				case NBCEL.Const.FSTORE_2:
				{
					obj = new NBCEL.generic.FSTORE(2);
					break;
				}

				case NBCEL.Const.FSTORE_3:
				{
					obj = new NBCEL.generic.FSTORE(3);
					break;
				}

				case NBCEL.Const.DSTORE_0:
				{
					obj = new NBCEL.generic.DSTORE(0);
					break;
				}

				case NBCEL.Const.DSTORE_1:
				{
					obj = new NBCEL.generic.DSTORE(1);
					break;
				}

				case NBCEL.Const.DSTORE_2:
				{
					obj = new NBCEL.generic.DSTORE(2);
					break;
				}

				case NBCEL.Const.DSTORE_3:
				{
					obj = new NBCEL.generic.DSTORE(3);
					break;
				}

				case NBCEL.Const.ASTORE_0:
				{
					obj = new NBCEL.generic.ASTORE(0);
					break;
				}

				case NBCEL.Const.ASTORE_1:
				{
					obj = new NBCEL.generic.ASTORE(1);
					break;
				}

				case NBCEL.Const.ASTORE_2:
				{
					obj = new NBCEL.generic.ASTORE(2);
					break;
				}

				case NBCEL.Const.ASTORE_3:
				{
					obj = new NBCEL.generic.ASTORE(3);
					break;
				}

				case NBCEL.Const.IINC:
				{
					obj = new NBCEL.generic.IINC();
					break;
				}

				case NBCEL.Const.IFEQ:
				{
					obj = new NBCEL.generic.IFEQ();
					break;
				}

				case NBCEL.Const.IFNE:
				{
					obj = new NBCEL.generic.IFNE();
					break;
				}

				case NBCEL.Const.IFLT:
				{
					obj = new NBCEL.generic.IFLT();
					break;
				}

				case NBCEL.Const.IFGE:
				{
					obj = new NBCEL.generic.IFGE();
					break;
				}

				case NBCEL.Const.IFGT:
				{
					obj = new NBCEL.generic.IFGT();
					break;
				}

				case NBCEL.Const.IFLE:
				{
					obj = new NBCEL.generic.IFLE();
					break;
				}

				case NBCEL.Const.IF_ICMPEQ:
				{
					obj = new NBCEL.generic.IF_ICMPEQ();
					break;
				}

				case NBCEL.Const.IF_ICMPNE:
				{
					obj = new NBCEL.generic.IF_ICMPNE();
					break;
				}

				case NBCEL.Const.IF_ICMPLT:
				{
					obj = new NBCEL.generic.IF_ICMPLT();
					break;
				}

				case NBCEL.Const.IF_ICMPGE:
				{
					obj = new NBCEL.generic.IF_ICMPGE();
					break;
				}

				case NBCEL.Const.IF_ICMPGT:
				{
					obj = new NBCEL.generic.IF_ICMPGT();
					break;
				}

				case NBCEL.Const.IF_ICMPLE:
				{
					obj = new NBCEL.generic.IF_ICMPLE();
					break;
				}

				case NBCEL.Const.IF_ACMPEQ:
				{
					obj = new NBCEL.generic.IF_ACMPEQ();
					break;
				}

				case NBCEL.Const.IF_ACMPNE:
				{
					obj = new NBCEL.generic.IF_ACMPNE();
					break;
				}

				case NBCEL.Const.GOTO:
				{
					obj = new NBCEL.generic.GOTO();
					break;
				}

				case NBCEL.Const.JSR:
				{
					obj = new NBCEL.generic.JSR();
					break;
				}

				case NBCEL.Const.RET:
				{
					obj = new NBCEL.generic.RET();
					break;
				}

				case NBCEL.Const.TABLESWITCH:
				{
					obj = new NBCEL.generic.TABLESWITCH();
					break;
				}

				case NBCEL.Const.LOOKUPSWITCH:
				{
					obj = new NBCEL.generic.LOOKUPSWITCH();
					break;
				}

				case NBCEL.Const.GETSTATIC:
				{
					obj = new NBCEL.generic.GETSTATIC();
					break;
				}

				case NBCEL.Const.PUTSTATIC:
				{
					obj = new NBCEL.generic.PUTSTATIC();
					break;
				}

				case NBCEL.Const.GETFIELD:
				{
					obj = new NBCEL.generic.GETFIELD();
					break;
				}

				case NBCEL.Const.PUTFIELD:
				{
					obj = new NBCEL.generic.PUTFIELD();
					break;
				}

				case NBCEL.Const.INVOKEVIRTUAL:
				{
					obj = new NBCEL.generic.INVOKEVIRTUAL();
					break;
				}

				case NBCEL.Const.INVOKESPECIAL:
				{
					obj = new NBCEL.generic.INVOKESPECIAL();
					break;
				}

				case NBCEL.Const.INVOKESTATIC:
				{
					obj = new NBCEL.generic.INVOKESTATIC();
					break;
				}

				case NBCEL.Const.INVOKEINTERFACE:
				{
					obj = new NBCEL.generic.INVOKEINTERFACE();
					break;
				}

				case NBCEL.Const.INVOKEDYNAMIC:
				{
					obj = new NBCEL.generic.INVOKEDYNAMIC();
					break;
				}

				case NBCEL.Const.NEW:
				{
					obj = new NBCEL.generic.NEW();
					break;
				}

				case NBCEL.Const.NEWARRAY:
				{
					obj = new NBCEL.generic.NEWARRAY();
					break;
				}

				case NBCEL.Const.ANEWARRAY:
				{
					obj = new NBCEL.generic.ANEWARRAY();
					break;
				}

				case NBCEL.Const.CHECKCAST:
				{
					obj = new NBCEL.generic.CHECKCAST();
					break;
				}

				case NBCEL.Const.INSTANCEOF:
				{
					obj = new NBCEL.generic.INSTANCEOF();
					break;
				}

				case NBCEL.Const.MULTIANEWARRAY:
				{
					obj = new NBCEL.generic.MULTIANEWARRAY();
					break;
				}

				case NBCEL.Const.IFNULL:
				{
					obj = new NBCEL.generic.IFNULL();
					break;
				}

				case NBCEL.Const.IFNONNULL:
				{
					obj = new NBCEL.generic.IFNONNULL();
					break;
				}

				case NBCEL.Const.GOTO_W:
				{
					obj = new NBCEL.generic.GOTO_W();
					break;
				}

				case NBCEL.Const.JSR_W:
				{
					obj = new NBCEL.generic.JSR_W();
					break;
				}

				case NBCEL.Const.BREAKPOINT:
				{
					obj = new NBCEL.generic.BREAKPOINT();
					break;
				}

				case NBCEL.Const.IMPDEP1:
				{
					obj = new NBCEL.generic.IMPDEP1();
					break;
				}

				case NBCEL.Const.IMPDEP2:
				{
					obj = new NBCEL.generic.IMPDEP2();
					break;
				}

				default:
				{
					throw new NBCEL.generic.ClassGenException("Illegal opcode detected: " + opcode);
				}
			}
			if (wide && !((obj is NBCEL.generic.LocalVariableInstruction) || (obj is NBCEL.generic.IINC
				) || (obj is NBCEL.generic.RET)))
			{
				throw new NBCEL.generic.ClassGenException("Illegal opcode after wide: " + opcode);
			}
			obj.SetOpcode(opcode);
			obj.InitFromFile(bytes, wide);
			// Do further initializations, if any
			return obj;
		}

		/// <summary>
		/// This method also gives right results for instructions whose
		/// effect on the stack depends on the constant pool entry they
		/// reference.
		/// </summary>
		/// <returns>
		/// Number of words consumed from stack by this instruction,
		/// or Constants.UNPREDICTABLE, if this can not be computed statically
		/// </returns>
		public virtual int ConsumeStack(NBCEL.generic.ConstantPoolGen cpg)
		{
			return NBCEL.Const.GetConsumeStack(opcode);
		}

		/// <summary>
		/// This method also gives right results for instructions whose
		/// effect on the stack depends on the constant pool entry they
		/// reference.
		/// </summary>
		/// <returns>
		/// Number of words produced onto stack by this instruction,
		/// or Constants.UNPREDICTABLE, if this can not be computed statically
		/// </returns>
		public virtual int ProduceStack(NBCEL.generic.ConstantPoolGen cpg)
		{
			return NBCEL.Const.GetProduceStack(opcode);
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
			this.length = (short)length;
		}

		// TODO check range?
		/// <summary>Some instructions may be reused, so don't do anything by default.</summary>
		internal virtual void Dispose()
		{
		}

		/// <summary>Call corresponding visitor method(s).</summary>
		/// <remarks>
		/// Call corresponding visitor method(s). The order is:
		/// Call visitor methods of implemented interfaces first, then
		/// call methods according to the class hierarchy in descending order,
		/// i.e., the most specific visitXXX() call comes last.
		/// </remarks>
		/// <param name="v">Visitor object</param>
		public abstract void Accept(NBCEL.generic.Visitor v);

		/// <summary>
		/// Get Comparator object used in the equals() method to determine
		/// equality of instructions.
		/// </summary>
		/// <returns>currently used comparator for equals()</returns>
		[System.ObsoleteAttribute(@"(6.0) use the built in comparator, or wrap this class in another object that implements these methods"
			)]
		public static NBCEL.generic.InstructionComparator GetComparator()
		{
			return cmp;
		}

		/// <summary>Set comparator to be used for equals().</summary>
		[System.ObsoleteAttribute(@"(6.0) use the built in comparator, or wrap this class in another object that implements these methods"
			)]
		public static void SetComparator(NBCEL.generic.InstructionComparator c)
		{
			cmp = c;
		}

		/// <summary>Check for equality, delegated to comparator</summary>
		/// <returns>true if that is an Instruction and has the same opcode</returns>
		public override bool Equals(object that)
		{
			return (that is NBCEL.generic.Instruction) ? cmp.Equals(this, (NBCEL.generic.Instruction
				)that) : false;
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

		object System.ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
