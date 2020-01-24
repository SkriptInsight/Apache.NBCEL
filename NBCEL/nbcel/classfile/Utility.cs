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
*/

using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using java.io;
using Java.IO;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>Utility functions that do not really belong to any class in particular.</summary>
	public abstract class Utility
	{
		// @since 6.0 methods are no longer final
		private static int Unwrap(ThreadLocal<int> tl)
		{
			return tl.Value;
		}

		private static void Wrap(System.Threading.ThreadLocal<int> tl, int value)
		{
			tl.Value = value;
		}

		private sealed class _ThreadLocal_60 : System.Threading.ThreadLocal<int>
		{
			/* How many chars have been consumed
			* during parsing in typeSignatureToString().
			* Read by methodSignatureToString().
			* Set by side effect, but only internally.
			*/
			public _ThreadLocal_60() : base(() => 0)
			{
			}

		}

		private static System.Threading.ThreadLocal<int> consumed_chars = new _ThreadLocal_60();

		private static bool wide = false;

		/* The `WIDE' instruction is used in the
		* byte code to allow 16-bit wide indices
		* for local variables. This opcode
		* precedes an `ILOAD', e.g.. The opcode
		* immediately following takes an extra
		* byte which is combined with the
		* following byte to form a
		* 16-bit value.
		*/
		/// <summary>Convert bit field of flags into string such as `static final'.</summary>
		/// <param name="access_flags">Access flags</param>
		/// <returns>String representation of flags</returns>
		public static string AccessToString(int access_flags)
		{
			return AccessToString(access_flags, false);
		}

		/// <summary>Convert bit field of flags into string such as `static final'.</summary>
		/// <remarks>
		/// Convert bit field of flags into string such as `static final'.
		/// Special case: Classes compiled with new compilers and with the
		/// `ACC_SUPER' flag would be said to be "synchronized". This is
		/// because SUN used the same value for the flags `ACC_SUPER' and
		/// `ACC_SYNCHRONIZED'.
		/// </remarks>
		/// <param name="access_flags">Access flags</param>
		/// <param name="for_class">access flags are for class qualifiers ?</param>
		/// <returns>String representation of flags</returns>
		public static string AccessToString(int access_flags, bool for_class)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			int p = 0;
			for (int i = 0; p < NBCEL.Const.MAX_ACC_FLAG_I; i++)
			{
				// Loop through known flags
				p = Pow2(i);
				if ((access_flags & p) != 0)
				{
					/* Special case: Classes compiled with new compilers and with the
					* `ACC_SUPER' flag would be said to be "synchronized". This is
					* because SUN used the same value for the flags `ACC_SUPER' and
					* `ACC_SYNCHRONIZED'.
					*/
					if (for_class && ((p == NBCEL.Const.ACC_SUPER) || (p == NBCEL.Const.ACC_INTERFACE
						)))
					{
						continue;
					}
					buf.Append(NBCEL.Const.GetAccessName(i)).Append(" ");
				}
			}
			return buf.ToString().Trim();
		}

		/// <param name="access_flags">the class flags</param>
		/// <returns>"class" or "interface", depending on the ACC_INTERFACE flag</returns>
		public static string ClassOrInterface(int access_flags)
		{
			return ((access_flags & NBCEL.Const.ACC_INTERFACE) != 0) ? "interface" : "class";
		}

		/// <summary>
		/// Disassemble a byte array of JVM byte codes starting from code line
		/// `index' and return the disassembled string representation.
		/// </summary>
		/// <remarks>
		/// Disassemble a byte array of JVM byte codes starting from code line
		/// `index' and return the disassembled string representation. Decode only
		/// `num' opcodes (including their operands), use -1 if you want to
		/// decompile everything.
		/// </remarks>
		/// <param name="code">byte code array</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <param name="index">
		/// offset in `code' array
		/// <EM>(number of opcodes, not bytes!)</EM>
		/// </param>
		/// <param name="length">number of opcodes to decompile, -1 for all</param>
		/// <param name="verbose">be verbose, e.g. print constant pool index</param>
		/// <returns>String representation of byte codes</returns>
		public static string CodeToString(byte[] code, NBCEL.classfile.ConstantPool constant_pool
			, int index, int length, bool verbose)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder(code.Length * 20);
			// Should be sufficient // CHECKSTYLE IGNORE MagicNumber
			try
			{
				using (NBCEL.util.ByteSequence stream = new NBCEL.util.ByteSequence(code))
				{
					for (int i = 0; i < index; i++)
					{
						CodeToString(stream, constant_pool, verbose);
					}
					for (int i = 0; stream.Available() > 0; i++)
					{
						if ((length < 0) || (i < length))
						{
							string indices = Fillup(stream.GetIndex() + ":", 6, true, ' ');
							buf.Append(indices).Append(CodeToString(stream, constant_pool, verbose)).Append('\n'
								);
						}
					}
				}
			}
			catch (System.IO.IOException e)
			{
				throw new NBCEL.classfile.ClassFormatException("Byte code error: " + buf.ToString
					(), e);
			}
			return buf.ToString();
		}

		public static string CodeToString(byte[] code, NBCEL.classfile.ConstantPool constant_pool
			, int index, int length)
		{
			return CodeToString(code, constant_pool, index, length, true);
		}

		/// <summary>
		/// Disassemble a stream of byte codes and return the
		/// string representation.
		/// </summary>
		/// <param name="bytes">stream of bytes</param>
		/// <param name="constant_pool">Array of constants</param>
		/// <param name="verbose">be verbose, e.g. print constant pool index</param>
		/// <returns>String representation of byte code</returns>
		/// <exception cref="IOException">if a failure from reading from the bytes argument occurs
		/// 	</exception>
		public static string CodeToString(NBCEL.util.ByteSequence bytes, NBCEL.classfile.ConstantPool
			 constant_pool, bool verbose)
		{
			short opcode = (short)bytes.ReadUnsignedByte();
			int default_offset = 0;
			int low;
			int high;
			int npairs;
			int index;
			int vindex;
			int constant;
			int[] match;
			int[] jump_table;
			int no_pad_bytes = 0;
			int offset;
			System.Text.StringBuilder buf = new System.Text.StringBuilder(NBCEL.Const.GetOpcodeName
				(opcode));
			/* Special case: Skip (0-3) padding bytes, i.e., the
			* following bytes are 4-byte-aligned
			*/
			if ((opcode == NBCEL.Const.TABLESWITCH) || (opcode == NBCEL.Const.LOOKUPSWITCH))
			{
				int remainder = bytes.GetIndex() % 4;
				no_pad_bytes = (remainder == 0) ? 0 : 4 - remainder;
				for (int i = 0; i < no_pad_bytes; i++)
				{
					byte b;
					if ((b = bytes.ReadByte()) != 0)
					{
						System.Console.Error.WriteLine("Warning: Padding byte != 0 in " + NBCEL.Const.GetOpcodeName
							(opcode) + ":" + b);
					}
				}
				// Both cases have a field default_offset in common
				default_offset = bytes.ReadInt();
			}
			switch (opcode)
			{
				case NBCEL.Const.TABLESWITCH:
				{
					/* Table switch has variable length arguments.
					*/
					low = bytes.ReadInt();
					high = bytes.ReadInt();
					offset = bytes.GetIndex() - 12 - no_pad_bytes - 1;
					default_offset += offset;
					buf.Append("\tdefault = ").Append(default_offset).Append(", low = ").Append(low).
						Append(", high = ").Append(high).Append("(");
					jump_table = new int[high - low + 1];
					for (int i = 0; i < jump_table.Length; i++)
					{
						jump_table[i] = offset + bytes.ReadInt();
						buf.Append(jump_table[i]);
						if (i < jump_table.Length - 1)
						{
							buf.Append(", ");
						}
					}
					buf.Append(")");
					break;
				}

				case NBCEL.Const.LOOKUPSWITCH:
				{
					/* Lookup switch has variable length arguments.
					*/
					npairs = bytes.ReadInt();
					offset = bytes.GetIndex() - 8 - no_pad_bytes - 1;
					match = new int[npairs];
					jump_table = new int[npairs];
					default_offset += offset;
					buf.Append("\tdefault = ").Append(default_offset).Append(", npairs = ").Append(npairs
						).Append(" (");
					for (int i = 0; i < npairs; i++)
					{
						match[i] = bytes.ReadInt();
						jump_table[i] = offset + bytes.ReadInt();
						buf.Append("(").Append(match[i]).Append(", ").Append(jump_table[i]).Append(")");
						if (i < npairs - 1)
						{
							buf.Append(", ");
						}
					}
					buf.Append(")");
					break;
				}

				case NBCEL.Const.GOTO:
				case NBCEL.Const.IFEQ:
				case NBCEL.Const.IFGE:
				case NBCEL.Const.IFGT:
				case NBCEL.Const.IFLE:
				case NBCEL.Const.IFLT:
				case NBCEL.Const.JSR:
				case NBCEL.Const.IFNE:
				case NBCEL.Const.IFNONNULL:
				case NBCEL.Const.IFNULL:
				case NBCEL.Const.IF_ACMPEQ:
				case NBCEL.Const.IF_ACMPNE:
				case NBCEL.Const.IF_ICMPEQ:
				case NBCEL.Const.IF_ICMPGE:
				case NBCEL.Const.IF_ICMPGT:
				case NBCEL.Const.IF_ICMPLE:
				case NBCEL.Const.IF_ICMPLT:
				case NBCEL.Const.IF_ICMPNE:
				{
					/* Two address bytes + offset from start of byte stream form the
					* jump target
					*/
					buf.Append("\t\t#").Append((bytes.GetIndex() - 1) + bytes.ReadShort());
					break;
				}

				case NBCEL.Const.GOTO_W:
				case NBCEL.Const.JSR_W:
				{
					/* 32-bit wide jumps
					*/
					buf.Append("\t\t#").Append((bytes.GetIndex() - 1) + bytes.ReadInt());
					break;
				}

				case NBCEL.Const.ALOAD:
				case NBCEL.Const.ASTORE:
				case NBCEL.Const.DLOAD:
				case NBCEL.Const.DSTORE:
				case NBCEL.Const.FLOAD:
				case NBCEL.Const.FSTORE:
				case NBCEL.Const.ILOAD:
				case NBCEL.Const.ISTORE:
				case NBCEL.Const.LLOAD:
				case NBCEL.Const.LSTORE:
				case NBCEL.Const.RET:
				{
					/* Index byte references local variable (register)
					*/
					if (wide)
					{
						vindex = bytes.ReadUnsignedShort();
						wide = false;
					}
					else
					{
						// Clear flag
						vindex = bytes.ReadUnsignedByte();
					}
					buf.Append("\t\t%").Append(vindex);
					break;
				}

				case NBCEL.Const.WIDE:
				{
					/*
					* Remember wide byte which is used to form a 16-bit address in the
					* following instruction. Relies on that the method is called again with
					* the following opcode.
					*/
					wide = true;
					buf.Append("\t(wide)");
					break;
				}

				case NBCEL.Const.NEWARRAY:
				{
					/* Array of basic type.
					*/
					buf.Append("\t\t<").Append(NBCEL.Const.GetTypeName(bytes.ReadByte())).Append(">");
					break;
				}

				case NBCEL.Const.GETFIELD:
				case NBCEL.Const.GETSTATIC:
				case NBCEL.Const.PUTFIELD:
				case NBCEL.Const.PUTSTATIC:
				{
					/* Access object/class fields.
					*/
					index = bytes.ReadUnsignedShort();
					buf.Append("\t\t").Append(constant_pool.ConstantToString(index, NBCEL.Const.CONSTANT_Fieldref
						)).Append(verbose ? " (" + index + ")" : string.Empty);
					break;
				}

				case NBCEL.Const.NEW:
				case NBCEL.Const.CHECKCAST:
				{
					/* Operands are references to classes in constant pool
					*/
					buf.Append("\t");
					goto case NBCEL.Const.INSTANCEOF;
				}

				case NBCEL.Const.INSTANCEOF:
				{
					//$FALL-THROUGH$
					index = bytes.ReadUnsignedShort();
					buf.Append("\t<").Append(constant_pool.ConstantToString(index, NBCEL.Const.CONSTANT_Class
						)).Append(">").Append(verbose ? " (" + index + ")" : string.Empty);
					break;
				}

				case NBCEL.Const.INVOKESPECIAL:
				case NBCEL.Const.INVOKESTATIC:
				{
					/* Operands are references to methods in constant pool
					*/
					index = bytes.ReadUnsignedShort();
					NBCEL.classfile.Constant c = constant_pool.GetConstant(index);
					// With Java8 operand may be either a CONSTANT_Methodref
					// or a CONSTANT_InterfaceMethodref.   (markro)
					buf.Append("\t").Append(constant_pool.ConstantToString(index, c.GetTag())).Append
						(verbose ? " (" + index + ")" : string.Empty);
					break;
				}

				case NBCEL.Const.INVOKEVIRTUAL:
				{
					index = bytes.ReadUnsignedShort();
					buf.Append("\t").Append(constant_pool.ConstantToString(index, NBCEL.Const.CONSTANT_Methodref
						)).Append(verbose ? " (" + index + ")" : string.Empty);
					break;
				}

				case NBCEL.Const.INVOKEINTERFACE:
				{
					index = bytes.ReadUnsignedShort();
					int nargs = bytes.ReadUnsignedByte();
					// historical, redundant
					buf.Append("\t").Append(constant_pool.ConstantToString(index, NBCEL.Const.CONSTANT_InterfaceMethodref
						)).Append(verbose ? " (" + index + ")\t" : string.Empty).Append(nargs).Append("\t"
						).Append(bytes.ReadUnsignedByte());
					// Last byte is a reserved space
					break;
				}

				case NBCEL.Const.INVOKEDYNAMIC:
				{
					index = bytes.ReadUnsignedShort();
					buf.Append("\t").Append(constant_pool.ConstantToString(index, NBCEL.Const.CONSTANT_InvokeDynamic
						)).Append(verbose ? " (" + index + ")\t" : string.Empty).Append(bytes.ReadUnsignedByte
						()).Append(bytes.ReadUnsignedByte());
					// Thrid byte is a reserved space
					// Last byte is a reserved space
					break;
				}

				case NBCEL.Const.LDC_W:
				case NBCEL.Const.LDC2_W:
				{
					/* Operands are references to items in constant pool
					*/
					index = bytes.ReadUnsignedShort();
					buf.Append("\t\t").Append(constant_pool.ConstantToString(index, constant_pool.GetConstant
						(index).GetTag())).Append(verbose ? " (" + index + ")" : string.Empty);
					break;
				}

				case NBCEL.Const.LDC:
				{
					index = bytes.ReadUnsignedByte();
					buf.Append("\t\t").Append(constant_pool.ConstantToString(index, constant_pool.GetConstant
						(index).GetTag())).Append(verbose ? " (" + index + ")" : string.Empty);
					break;
				}

				case NBCEL.Const.ANEWARRAY:
				{
					/* Array of references.
					*/
					index = bytes.ReadUnsignedShort();
					buf.Append("\t\t<").Append(CompactClassName(constant_pool.GetConstantString(index
						, NBCEL.Const.CONSTANT_Class), false)).Append(">").Append(verbose ? " (" + index
						 + ")" : string.Empty);
					break;
				}

				case NBCEL.Const.MULTIANEWARRAY:
				{
					/* Multidimensional array of references.
					*/
					index = bytes.ReadUnsignedShort();
					int dimensions = bytes.ReadUnsignedByte();
					buf.Append("\t<").Append(CompactClassName(constant_pool.GetConstantString(index, 
						NBCEL.Const.CONSTANT_Class), false)).Append(">\t").Append(dimensions).Append(verbose
						 ? " (" + index + ")" : string.Empty);
					break;
				}

				case NBCEL.Const.IINC:
				{
					/* Increment local variable.
					*/
					if (wide)
					{
						vindex = bytes.ReadUnsignedShort();
						constant = bytes.ReadShort();
						wide = false;
					}
					else
					{
						vindex = bytes.ReadUnsignedByte();
						constant = bytes.ReadByte();
					}
					buf.Append("\t\t%").Append(vindex).Append("\t").Append(constant);
					break;
				}

				default:
				{
					if (NBCEL.Const.GetNoOfOperands(opcode) > 0)
					{
						for (int i = 0; i < NBCEL.Const.GetOperandTypeCount(opcode); i++)
						{
							buf.Append("\t\t");
							switch (NBCEL.Const.GetOperandType(opcode, i))
							{
								case NBCEL.Const.T_BYTE:
								{
									buf.Append(bytes.ReadByte());
									break;
								}

								case NBCEL.Const.T_SHORT:
								{
									buf.Append(bytes.ReadShort());
									break;
								}

								case NBCEL.Const.T_INT:
								{
									buf.Append(bytes.ReadInt());
									break;
								}

								default:
								{
									// Never reached
									throw new System.InvalidOperationException("Unreachable default case reached!");
								}
							}
						}
					}
					break;
				}
			}
			return buf.ToString();
		}

		/// <exception cref="IOException"/>
		public static string CodeToString(NBCEL.util.ByteSequence bytes, NBCEL.classfile.ConstantPool
			 constant_pool)
		{
			return CodeToString(bytes, constant_pool, true);
		}

		/// <summary>
		/// Shorten long class names, <em>java/lang/String</em> becomes
		/// <em>String</em>.
		/// </summary>
		/// <param name="str">The long class name</param>
		/// <returns>Compacted class name</returns>
		public static string CompactClassName(string str)
		{
			return CompactClassName(str, true);
		}

		/// <summary>
		/// Shorten long class names, <em>java/lang/String</em> becomes
		/// <em>java.lang.String</em>,
		/// e.g..
		/// </summary>
		/// <remarks>
		/// Shorten long class names, <em>java/lang/String</em> becomes
		/// <em>java.lang.String</em>,
		/// e.g.. If <em>chopit</em> is <em>true</em> the prefix <em>java.lang</em>
		/// is also removed.
		/// </remarks>
		/// <param name="str">The long class name</param>
		/// <param name="chopit">flag that determines whether chopping is executed or not</param>
		/// <returns>Compacted class name</returns>
		public static string CompactClassName(string str, bool chopit)
		{
			return CompactClassName(str, "java.lang.", chopit);
		}

		/// <summary>
		/// Shorten long class name <em>str</em>, i.e., chop off the <em>prefix</em>,
		/// if the
		/// class name starts with this string and the flag <em>chopit</em> is true.
		/// </summary>
		/// <remarks>
		/// Shorten long class name <em>str</em>, i.e., chop off the <em>prefix</em>,
		/// if the
		/// class name starts with this string and the flag <em>chopit</em> is true.
		/// Slashes <em>/</em> are converted to dots <em>.</em>.
		/// </remarks>
		/// <param name="str">The long class name</param>
		/// <param name="prefix">The prefix the get rid off</param>
		/// <param name="chopit">flag that determines whether chopping is executed or not</param>
		/// <returns>Compacted class name</returns>
		public static string CompactClassName(string str, string prefix, bool chopit)
		{
			int len = prefix.Length;
			str = str.Replace('/', '.');
			// Is `/' on all systems, even DOS
			if (chopit)
			{
				// If string starts with `prefix' and contains no further dots
				if (str.StartsWith(prefix) && (Sharpen.Runtime.Substring(str, len).IndexOf('.') ==
					 -1))
				{
					str = Sharpen.Runtime.Substring(str, len);
				}
			}
			return str;
		}

		/// <returns>`flag' with bit `i' set to 1</returns>
		public static int SetBit(int flag, int i)
		{
			return flag | Pow2(i);
		}

		/// <returns>`flag' with bit `i' set to 0</returns>
		public static int ClearBit(int flag, int i)
		{
			int bit = Pow2(i);
			return (flag & bit) == 0 ? flag : flag ^ bit;
		}

		/// <returns>true, if bit `i' in `flag' is set</returns>
		public static bool IsSet(int flag, int i)
		{
			return (flag & Pow2(i)) != 0;
		}

		/// <summary>
		/// Converts string containing the method return and argument types
		/// to a byte code method signature.
		/// </summary>
		/// <param name="ret">Return type of method</param>
		/// <param name="argv">Types of method arguments</param>
		/// <returns>Byte code representation of method signature</returns>
		/// <exception cref="ClassFormatException">if the signature is for Void</exception>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public static string MethodTypeToSignature(string ret, string[] argv)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder("(");
			string str;
			if (argv != null)
			{
				foreach (string element in argv)
				{
					str = GetSignature(element);
					if (str.EndsWith("V"))
					{
						throw new NBCEL.classfile.ClassFormatException("Invalid type: " + element);
					}
					buf.Append(str);
				}
			}
			str = GetSignature(ret);
			buf.Append(")").Append(str);
			return buf.ToString();
		}

		/// <summary>Converts argument list portion of method signature to string with all class names compacted.
		/// 	</summary>
		/// <param name="signature">Method signature</param>
		/// <returns>String Array of argument types</returns>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public static string[] MethodSignatureArgumentTypes(string signature)
		{
			return MethodSignatureArgumentTypes(signature, true);
		}

		/// <summary>Converts argument list portion of method signature to string.</summary>
		/// <param name="signature">Method signature</param>
		/// <param name="chopit">flag that determines whether chopping is executed or not</param>
		/// <returns>String Array of argument types</returns>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public static string[] MethodSignatureArgumentTypes(string signature, bool chopit
			)
		{
			System.Collections.Generic.List<string> vec = new System.Collections.Generic.List
				<string>();
			int index;
			try
			{
				// Skip any type arguments to read argument declarations between `(' and `)'
				index = signature.IndexOf('(') + 1;
				if (index <= 0)
				{
					throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
						);
				}
				while (signature[index] != ')')
				{
					vec.Add(TypeSignatureToString(Sharpen.Runtime.Substring(signature, index), chopit
						));
					//corrected concurrent private static field acess
					index += Unwrap(consumed_chars);
				}
			}
			catch (Exception e)
			{
				// update position
				// Should never occur
				throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
					, e);
			}
			return Sharpen.Collections.ToArray(vec, new string[vec.Count]);
		}

		/// <summary>Converts return type portion of method signature to string with all class names compacted.
		/// 	</summary>
		/// <param name="signature">Method signature</param>
		/// <returns>String representation of method return type</returns>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public static string MethodSignatureReturnType(string signature)
		{
			return MethodSignatureReturnType(signature, true);
		}

		/// <summary>Converts return type portion of method signature to string.</summary>
		/// <param name="signature">Method signature</param>
		/// <param name="chopit">flag that determines whether chopping is executed or not</param>
		/// <returns>String representation of method return type</returns>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public static string MethodSignatureReturnType(string signature, bool chopit)
		{
			int index;
			string type;
			try
			{
				// Read return type after `)'
				index = signature.LastIndexOf(')') + 1;
				if (index <= 0)
				{
					throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
						);
				}
				type = TypeSignatureToString(Sharpen.Runtime.Substring(signature, index), chopit);
			}
			catch (Exception e)
			{
				// Should never occur
				throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
					, e);
			}
			return type;
		}

		/// <summary>Converts method signature to string with all class names compacted.</summary>
		/// <param name="signature">to convert</param>
		/// <param name="name">of method</param>
		/// <param name="access">flags of method</param>
		/// <returns>Human readable signature</returns>
		public static string MethodSignatureToString(string signature, string name, string
			 access)
		{
			return MethodSignatureToString(signature, name, access, true);
		}

		/// <summary>Converts method signature to string.</summary>
		/// <param name="signature">to convert</param>
		/// <param name="name">of method</param>
		/// <param name="access">flags of method</param>
		/// <param name="chopit">flag that determines whether chopping is executed or not</param>
		/// <returns>Human readable signature</returns>
		public static string MethodSignatureToString(string signature, string name, string
			 access, bool chopit)
		{
			return MethodSignatureToString(signature, name, access, chopit, null);
		}

		/// <summary>
		/// This method converts a method signature string into a Java type declaration like
		/// `void main(String[])' and throws a `ClassFormatException' when the parsed
		/// type is invalid.
		/// </summary>
		/// <param name="signature">Method signature</param>
		/// <param name="name">Method name</param>
		/// <param name="access">Method access rights</param>
		/// <param name="chopit">flag that determines whether chopping is executed or not</param>
		/// <param name="vars">the LocalVariableTable for the method</param>
		/// <returns>Java type declaration</returns>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public static string MethodSignatureToString(string signature, string name, string
			 access, bool chopit, NBCEL.classfile.LocalVariableTable vars)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder("(");
			string type;
			int index;
			int var_index = access.Contains("static") ? 0 : 1;
			try
			{
				// Skip any type arguments to read argument declarations between `(' and `)'
				index = signature.IndexOf('(') + 1;
				if (index <= 0)
				{
					throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
						);
				}
				while (signature[index] != ')')
				{
					string param_type = TypeSignatureToString(Sharpen.Runtime.Substring(signature, index
						), chopit);
					buf.Append(param_type);
					if (vars != null)
					{
						NBCEL.classfile.LocalVariable l = vars.GetLocalVariable(var_index, 0);
						if (l != null)
						{
							buf.Append(" ").Append(l.GetName());
						}
					}
					else
					{
						buf.Append(" arg").Append(var_index);
					}
					if ("double".Equals(param_type) || "long".Equals(param_type))
					{
						var_index += 2;
					}
					else
					{
						var_index++;
					}
					buf.Append(", ");
					//corrected concurrent private static field acess
					index += Unwrap(consumed_chars);
				}
				// update position
				index++;
				// update position
				// Read return type after `)'
				type = TypeSignatureToString(Sharpen.Runtime.Substring(signature, index), chopit);
			}
			catch (Exception e)
			{
				// Should never occur
				throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
					, e);
			}
			// ignore any throws information in the signature
			if (buf.Length > 1)
			{
				buf.Length = buf.Length - 2;
			}
			buf.Append(")");
			return access + ((access.Length > 0) ? " " : string.Empty) + type + " " + name + 
				buf.ToString();
		}

		// May be an empty string
		private static int Pow2(int n)
		{
			return 1 << n;
		}

		/// <summary>Replace all occurrences of <em>old</em> in <em>str</em> with <em>new</em>.
		/// 	</summary>
		/// <param name="str">String to permute</param>
		/// <param name="old">String to be replaced</param>
		/// <param name="new_">Replacement string</param>
		/// <returns>new String object</returns>
		public static string Replace(string str, string old, string new_)
		{
			int index;
			int old_index;
			try
			{
				if (str.Contains(old))
				{
					// `old' found in str
					System.Text.StringBuilder buf = new System.Text.StringBuilder();
					old_index = 0;
					// String start offset
					// While we have something to replace
					while ((index = str.IndexOf(old, old_index)) != -1)
					{
						buf.Append(Sharpen.Runtime.Substring(str, old_index, index));
						// append prefix
						buf.Append(new_);
						// append replacement
						old_index = index + old.Length;
					}
					// Skip `old'.length chars
					buf.Append(Sharpen.Runtime.Substring(str, old_index));
					// append rest of string
					str = buf.ToString();
				}
			}
			catch (Exception e)
			{
				// Should not occur
				System.Console.Error.WriteLine(e);
			}
			return str;
		}

		/// <summary>Converts a signature to a string with all class names compacted.</summary>
		/// <remarks>
		/// Converts a signature to a string with all class names compacted.
		/// Class, Method and Type signatures are supported.
		/// Enum and Interface signatures are not supported.
		/// </remarks>
		/// <param name="signature">signature to convert</param>
		/// <returns>String containg human readable signature</returns>
		public static string SignatureToString(string signature)
		{
			return SignatureToString(signature, true);
		}

		/// <summary>Converts a signature to a string.</summary>
		/// <remarks>
		/// Converts a signature to a string.
		/// Class, Method and Type signatures are supported.
		/// Enum and Interface signatures are not supported.
		/// </remarks>
		/// <param name="signature">signature to convert</param>
		/// <param name="chopit">flag that determines whether chopping is executed or not</param>
		/// <returns>String containg human readable signature</returns>
		public static string SignatureToString(string signature, bool chopit)
		{
			string type = string.Empty;
			string typeParams = string.Empty;
			int index = 0;
			if (signature[0] == '<')
			{
				// we have type paramters
				typeParams = TypeParamTypesToString(signature, chopit);
				index += Unwrap(consumed_chars);
			}
			// update position
			if (signature[index] == '(')
			{
				// We have a Method signature.
				// add types of arguments
				type = typeParams + TypeSignaturesToString(Sharpen.Runtime.Substring(signature, index
					), chopit, ')');
				index += Unwrap(consumed_chars);
				// update position
				// add return type
				type = type + TypeSignatureToString(Sharpen.Runtime.Substring(signature, index), 
					chopit);
				index += Unwrap(consumed_chars);
				// update position
				// ignore any throws information in the signature
				return type;
			}
			else
			{
				// Could be Class or Type...
				type = TypeSignatureToString(Sharpen.Runtime.Substring(signature, index), chopit);
				index += Unwrap(consumed_chars);
				// update position
				if ((typeParams.Length == 0) && (index == signature.Length))
				{
					// We have a Type signature.
					return type;
				}
				// We have a Class signature.
				System.Text.StringBuilder typeClass = new System.Text.StringBuilder(typeParams);
				typeClass.Append(" extends ");
				typeClass.Append(type);
				if (index < signature.Length)
				{
					typeClass.Append(" implements ");
					typeClass.Append(TypeSignatureToString(Sharpen.Runtime.Substring(signature, index
						), chopit));
					index += Unwrap(consumed_chars);
				}
				// update position
				while (index < signature.Length)
				{
					typeClass.Append(", ");
					typeClass.Append(TypeSignatureToString(Sharpen.Runtime.Substring(signature, index
						), chopit));
					index += Unwrap(consumed_chars);
				}
				// update position
				return typeClass.ToString();
			}
		}

		/// <summary>Converts a type parameter list signature to a string.</summary>
		/// <param name="signature">signature to convert</param>
		/// <param name="chopit">flag that determines whether chopping is executed or not</param>
		/// <returns>String containg human readable signature</returns>
		private static string TypeParamTypesToString(string signature, bool chopit)
		{
			// The first character is guranteed to be '<'
			System.Text.StringBuilder typeParams = new System.Text.StringBuilder("<");
			int index = 1;
			// skip the '<'
			// get the first TypeParameter
			typeParams.Append(TypeParamTypeToString(Sharpen.Runtime.Substring(signature, index
				), chopit));
			index += Unwrap(consumed_chars);
			// update position
			// are there more TypeParameters?
			while (signature[index] != '>')
			{
				typeParams.Append(", ");
				typeParams.Append(TypeParamTypeToString(Sharpen.Runtime.Substring(signature, index
					), chopit));
				index += Unwrap(consumed_chars);
			}
			// update position
			Wrap(consumed_chars, index + 1);
			// account for the '>' char
			return typeParams.Append(">").ToString();
		}

		/// <summary>Converts a type parameter signature to a string.</summary>
		/// <param name="signature">signature to convert</param>
		/// <param name="chopit">flag that determines whether chopping is executed or not</param>
		/// <returns>String containg human readable signature</returns>
		private static string TypeParamTypeToString(string signature, bool chopit)
		{
			int index = signature.IndexOf(':');
			if (index <= 0)
			{
				throw new NBCEL.classfile.ClassFormatException("Invalid type parameter signature: "
					 + signature);
			}
			// get the TypeParameter identifier
			System.Text.StringBuilder typeParam = new System.Text.StringBuilder(Sharpen.Runtime.Substring
				(signature, 0, index));
			index++;
			// account for the ':'
			if (signature[index] != ':')
			{
				// we have a class bound
				typeParam.Append(" extends ");
				typeParam.Append(TypeSignatureToString(Sharpen.Runtime.Substring(signature, index
					), chopit));
				index += Unwrap(consumed_chars);
			}
			// update position
			// look for interface bounds
			while (signature[index] == ':')
			{
				index++;
				// skip over the ':'
				typeParam.Append(" & ");
				typeParam.Append(TypeSignatureToString(Sharpen.Runtime.Substring(signature, index
					), chopit));
				index += Unwrap(consumed_chars);
			}
			// update position
			Wrap(consumed_chars, index);
			return typeParam.ToString();
		}

		/// <summary>Converts a list of type signatures to a string.</summary>
		/// <param name="signature">signature to convert</param>
		/// <param name="chopit">flag that determines whether chopping is executed or not</param>
		/// <param name="term">character indicating the end of the list</param>
		/// <returns>String containg human readable signature</returns>
		private static string TypeSignaturesToString(string signature, bool chopit, char 
			term)
		{
			// The first character will be an 'open' that matches the 'close' contained in term.
			System.Text.StringBuilder typeList = new System.Text.StringBuilder(Sharpen.Runtime.Substring
				(signature, 0, 1));
			int index = 1;
			// skip the 'open' character
			// get the first Type in the list
			if (signature[index] != term)
			{
				typeList.Append(TypeSignatureToString(Sharpen.Runtime.Substring(signature, index)
					, chopit));
				index += Unwrap(consumed_chars);
			}
			// update position
			// are there more types in the list?
			while (signature[index] != term)
			{
				typeList.Append(", ");
				typeList.Append(TypeSignatureToString(Sharpen.Runtime.Substring(signature, index)
					, chopit));
				index += Unwrap(consumed_chars);
			}
			// update position
			Wrap(consumed_chars, index + 1);
			// account for the term char
			return typeList.Append(term).ToString();
		}

		/// <summary>
		/// This method converts a type signature string into a Java type declaration such as
		/// `String[]' and throws a `ClassFormatException' when the parsed type is invalid.
		/// </summary>
		/// <param name="signature">type signature</param>
		/// <param name="chopit">flag that determines whether chopping is executed or not</param>
		/// <returns>string containing human readable type signature</returns>
		/// <exception cref="ClassFormatException"/>
		/// <since>6.4.0</since>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public static string TypeSignatureToString(string signature, bool chopit)
		{
			//corrected concurrent private static field acess
			Wrap(consumed_chars, 1);
			// This is the default, read just one char like `B'
			try
			{
				switch (signature[0])
				{
					case 'B':
					{
						return "byte";
					}

					case 'C':
					{
						return "char";
					}

					case 'D':
					{
						return "double";
					}

					case 'F':
					{
						return "float";
					}

					case 'I':
					{
						return "int";
					}

					case 'J':
					{
						return "long";
					}

					case 'T':
					{
						// TypeVariableSignature
						int index = signature.IndexOf(';');
						// Look for closing `;'
						if (index < 0)
						{
							throw new NBCEL.classfile.ClassFormatException("Invalid type variable signature: "
								 + signature);
						}
						//corrected concurrent private static field acess
						Wrap(consumed_chars, index + 1);
						// "Tblabla;" `T' and `;' are removed
						return CompactClassName(Sharpen.Runtime.Substring(signature, 1, index), chopit);
					}

					case 'L':
					{
						// Full class name
						// should this be a while loop? can there be more than
						// one generic clause?  (markro)
						int fromIndex = signature.IndexOf('<');
						// generic type?
						if (fromIndex < 0)
						{
							fromIndex = 0;
						}
						else
						{
							fromIndex = signature.IndexOf('>', fromIndex);
							if (fromIndex < 0)
							{
								throw new NBCEL.classfile.ClassFormatException("Invalid signature: " + signature);
							}
						}
						int index = signature.IndexOf(';', fromIndex);
						// Look for closing `;'
						if (index < 0)
						{
							throw new NBCEL.classfile.ClassFormatException("Invalid signature: " + signature);
						}
						// check to see if there are any TypeArguments
						int bracketIndex = Sharpen.Runtime.Substring(signature, 0, index).IndexOf('<');
						if (bracketIndex < 0)
						{
							// just a class identifier
							Wrap(consumed_chars, index + 1);
							// "Lblabla;" `L' and `;' are removed
							return CompactClassName(Sharpen.Runtime.Substring(signature, 1, index), chopit);
						}
						// but make sure we are not looking past the end of the current item
						fromIndex = signature.IndexOf(';');
						if (fromIndex < 0)
						{
							throw new NBCEL.classfile.ClassFormatException("Invalid signature: " + signature);
						}
						if (fromIndex < bracketIndex)
						{
							// just a class identifier
							Wrap(consumed_chars, fromIndex + 1);
							// "Lblabla;" `L' and `;' are removed
							return CompactClassName(Sharpen.Runtime.Substring(signature, 1, fromIndex), chopit
								);
						}
						// we have TypeArguments; build up partial result
						// as we recurse for each TypeArgument
						System.Text.StringBuilder type = new System.Text.StringBuilder(CompactClassName(Sharpen.Runtime.Substring
							(signature, 1, bracketIndex), chopit)).Append("<");
						int inner_consumed_chars = bracketIndex + 1;
						// Shadows global var
						// check for wildcards
						if (signature[inner_consumed_chars] == '+')
						{
							type.Append("? extends ");
							inner_consumed_chars++;
						}
						else if (signature[inner_consumed_chars] == '-')
						{
							type.Append("? super ");
							inner_consumed_chars++;
						}
						// get the first TypeArgument
						if (signature[inner_consumed_chars] == '*')
						{
							type.Append("?");
							inner_consumed_chars++;
						}
						else
						{
							type.Append(TypeSignatureToString(Sharpen.Runtime.Substring(signature, inner_consumed_chars
								), chopit));
							// update our consumed count by the number of characters the for type argument
							inner_consumed_chars = Unwrap(NBCEL.classfile.Utility.consumed_chars) + inner_consumed_chars;
							Wrap(NBCEL.classfile.Utility.consumed_chars, inner_consumed_chars);
						}
						// are there more TypeArguments?
						while (signature[inner_consumed_chars] != '>')
						{
							type.Append(", ");
							// check for wildcards
							if (signature[inner_consumed_chars] == '+')
							{
								type.Append("? extends ");
								inner_consumed_chars++;
							}
							else if (signature[inner_consumed_chars] == '-')
							{
								type.Append("? super ");
								inner_consumed_chars++;
							}
							if (signature[inner_consumed_chars] == '*')
							{
								type.Append("?");
								inner_consumed_chars++;
							}
							else
							{
								type.Append(TypeSignatureToString(Sharpen.Runtime.Substring(signature, inner_consumed_chars
									), chopit));
								// update our consumed count by the number of characters the for type argument
								inner_consumed_chars = Unwrap(NBCEL.classfile.Utility.consumed_chars) + inner_consumed_chars;
								Wrap(NBCEL.classfile.Utility.consumed_chars, inner_consumed_chars);
							}
						}
						// process the closing ">"
						inner_consumed_chars++;
						type.Append(">");
						if (signature[inner_consumed_chars] == '.')
						{
							// we have a ClassTypeSignatureSuffix
							type.Append(".");
							// convert SimpleClassTypeSignature to fake ClassTypeSignature
							// and then recurse to parse it
							type.Append(TypeSignatureToString("L" + Sharpen.Runtime.Substring(signature, inner_consumed_chars
								 + 1), chopit));
							// update our consumed count by the number of characters the for type argument
							// note that this count includes the "L" we added, but that is ok
							// as it accounts for the "." we didn't consume
							inner_consumed_chars = Unwrap(NBCEL.classfile.Utility.consumed_chars) + inner_consumed_chars;
							Wrap(NBCEL.classfile.Utility.consumed_chars, inner_consumed_chars);
							return type.ToString();
						}
						if (signature[inner_consumed_chars] != ';')
						{
							throw new NBCEL.classfile.ClassFormatException("Invalid signature: " + signature);
						}
						Wrap(NBCEL.classfile.Utility.consumed_chars, inner_consumed_chars + 1);
						// remove final ";"
						return type.ToString();
					}

					case 'S':
					{
						return "short";
					}

					case 'Z':
					{
						return "boolean";
					}

					case '[':
					{
						// Array declaration
						int n;
						System.Text.StringBuilder brackets;
						string type;
						int consumed_chars;
						// Shadows global var
						brackets = new System.Text.StringBuilder();
						// Accumulate []'s
						// Count opening brackets and look for optional size argument
						for (n = 0; signature[n] == '['; n++)
						{
							brackets.Append("[]");
						}
						consumed_chars = n;
						// Remember value
						// The rest of the string denotes a `<field_type>'
						type = TypeSignatureToString(Sharpen.Runtime.Substring(signature, n), chopit);
						//corrected concurrent private static field acess
						//Utility.consumed_chars += consumed_chars; is replaced by:
						int _temp = Unwrap(NBCEL.classfile.Utility.consumed_chars) + consumed_chars;
						Wrap(NBCEL.classfile.Utility.consumed_chars, _temp);
						return type + brackets.ToString();
					}

					case 'V':
					{
						return "void";
					}

					default:
					{
						throw new NBCEL.classfile.ClassFormatException("Invalid signature: `" + signature
							 + "'");
					}
				}
			}
			catch (Exception e)
			{
				// Should never occur
				throw new NBCEL.classfile.ClassFormatException("Invalid signature: " + signature, 
					e);
			}
		}

		/// <summary>
		/// Parse Java type such as "char", or "java.lang.String[]" and return the
		/// signature in byte code format, e.g.
		/// </summary>
		/// <remarks>
		/// Parse Java type such as "char", or "java.lang.String[]" and return the
		/// signature in byte code format, e.g. "C" or "[Ljava/lang/String;" respectively.
		/// </remarks>
		/// <param name="type">Java type</param>
		/// <returns>byte code signature</returns>
		public static string GetSignature(string type)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			char[] chars = type.ToCharArray();
			bool char_found = false;
			bool delim = false;
			int index = -1;
			for (int i = 0; i < chars.Length; i++)
			{
				switch (chars[i])
				{
					case ' ':
					case '\t':
					case '\n':
					case '\r':
					case '\f':
					{
						if (char_found)
						{
							delim = true;
						}
						break;
					}

					case '[':
					{
						if (!char_found)
						{
							throw new System.Exception("Illegal type: " + type);
						}
						index = i;
						goto loop_break;
					}

					default:
					{
						char_found = true;
						if (!delim)
						{
							buf.Append(chars[i]);
						}
						break;
					}
				}
loop_continue: ;
			}
loop_break: ;
			int brackets = 0;
			if (index > 0)
			{
				brackets = CountBrackets(Sharpen.Runtime.Substring(type, index));
			}
			type = buf.ToString();
			buf.Length = 0;
			for (int i = 0; i < brackets; i++)
			{
				buf.Append('[');
			}
			bool found = false;
			for (int i = NBCEL.Const.T_BOOLEAN; (i <= NBCEL.Const.T_VOID) && !found; i++)
			{
				if (NBCEL.Const.GetTypeName(i).Equals(type))
				{
					found = true;
					buf.Append(NBCEL.Const.GetShortTypeName(i));
				}
			}
			if (!found)
			{
				buf.Append('L').Append(type.Replace('.', '/')).Append(';');
			}
			return buf.ToString();
		}

		private static int CountBrackets(string brackets)
		{
			char[] chars = brackets.ToCharArray();
			int count = 0;
			bool open = false;
			foreach (char c in chars)
			{
				switch (c)
				{
					case '[':
					{
						if (open)
						{
							throw new System.Exception("Illegally nested brackets:" + brackets);
						}
						open = true;
						break;
					}

					case ']':
					{
						if (!open)
						{
							throw new System.Exception("Illegally nested brackets:" + brackets);
						}
						open = false;
						count++;
						break;
					}

					default:
					{
						// Don't care
						break;
					}
				}
			}
			if (open)
			{
				throw new System.Exception("Illegally nested brackets:" + brackets);
			}
			return count;
		}

		/// <summary>Return type of method signature as a byte value as defined in <em>Constants</em>
		/// 	</summary>
		/// <param name="signature">in format described above</param>
		/// <returns>type of method signature</returns>
		/// <seealso cref="NBCEL.Const"/>
		/// <exception cref="ClassFormatException">if signature is not a method signature</exception>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public static byte TypeOfMethodSignature(string signature)
		{
			int index;
			try
			{
				if (signature[0] != '(')
				{
					throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
						);
				}
				index = signature.LastIndexOf(')') + 1;
				return TypeOfSignature(Sharpen.Runtime.Substring(signature, index));
			}
			catch (Exception e)
			{
				throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
					, e);
			}
		}

		/// <summary>Return type of signature as a byte value as defined in <em>Constants</em>
		/// 	</summary>
		/// <param name="signature">in format described above</param>
		/// <returns>type of signature</returns>
		/// <seealso cref="NBCEL.Const"/>
		/// <exception cref="ClassFormatException">if signature isn't a known type</exception>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public static byte TypeOfSignature(string signature)
		{
			try
			{
				switch (signature[0])
				{
					case 'B':
					{
						return NBCEL.Const.T_BYTE;
					}

					case 'C':
					{
						return NBCEL.Const.T_CHAR;
					}

					case 'D':
					{
						return NBCEL.Const.T_DOUBLE;
					}

					case 'F':
					{
						return NBCEL.Const.T_FLOAT;
					}

					case 'I':
					{
						return NBCEL.Const.T_INT;
					}

					case 'J':
					{
						return NBCEL.Const.T_LONG;
					}

					case 'L':
					case 'T':
					{
						return NBCEL.Const.T_REFERENCE;
					}

					case '[':
					{
						return NBCEL.Const.T_ARRAY;
					}

					case 'V':
					{
						return NBCEL.Const.T_VOID;
					}

					case 'Z':
					{
						return NBCEL.Const.T_BOOLEAN;
					}

					case 'S':
					{
						return NBCEL.Const.T_SHORT;
					}

					case '!':
					case '+':
					case '*':
					{
						return TypeOfSignature(Sharpen.Runtime.Substring(signature, 1));
					}

					default:
					{
						throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
							);
					}
				}
			}
			catch (Exception e)
			{
				throw new NBCEL.classfile.ClassFormatException("Invalid method signature: " + signature
					, e);
			}
		}

		/// <summary>Map opcode names to opcode numbers.</summary>
		/// <remarks>Map opcode names to opcode numbers. E.g., return Constants.ALOAD for "aload"
		/// 	</remarks>
		public static short SearchOpcode(string name)
		{
			name = name.ToLower();
			for (short i = 0; i < NBCEL.Const.OPCODE_NAMES_LENGTH; i++)
			{
				if (NBCEL.Const.GetOpcodeName(i).Equals(name))
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Convert (signed) byte to (unsigned) short value, i.e., all negative
		/// values become positive.
		/// </summary>
		private static short ByteToShort(byte b)
		{
			return (((sbyte)b) < 0) ? (short)(256 + b) : (short)b;
		}

		/// <summary>Convert bytes into hexadecimal string</summary>
		/// <param name="bytes">an array of bytes to convert to hexadecimal</param>
		/// <returns>bytes as hexadecimal string, e.g. 00 fa 12 ...</returns>
		public static string ToHexString(byte[] bytes)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				short b = ByteToShort(bytes[i]);
				string hex = Sharpen.Runtime.ToHexString(b);
				if (b < unchecked((int)(0x10)))
				{
					buf.Append('0');
				}
				buf.Append(hex);
				if (i < bytes.Length - 1)
				{
					buf.Append(' ');
				}
			}
			return buf.ToString();
		}

		/// <summary>
		/// Return a string for an integer justified left or right and filled up with
		/// `fill' characters if necessary.
		/// </summary>
		/// <param name="i">integer to format</param>
		/// <param name="length">length of desired string</param>
		/// <param name="left_justify">format left or right</param>
		/// <param name="fill">fill character</param>
		/// <returns>formatted int</returns>
		public static string Format(int i, int length, bool left_justify, char fill)
		{
			return Fillup(i.ToString(), length, left_justify, fill);
		}

		/// <summary>Fillup char with up to length characters with char `fill' and justify it left or right.
		/// 	</summary>
		/// <param name="str">string to format</param>
		/// <param name="length">length of desired string</param>
		/// <param name="left_justify">format left or right</param>
		/// <param name="fill">fill character</param>
		/// <returns>formatted string</returns>
		public static string Fillup(string str, int length, bool left_justify, char fill)
		{
			int len = length - str.Length;
			char[] buf = new char[(len < 0) ? 0 : len];
			for (int j = 0; j < buf.Length; j++)
			{
				buf[j] = fill;
			}
			if (left_justify)
			{
				return str + new string(buf);
			}
			return new string(buf) + str;
		}

		internal static bool Equals(byte[] a, byte[] b)
		{
			int size;
			if ((size = a.Length) != b.Length)
			{
				return false;
			}
			for (int i = 0; i < size; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			return true;
		}

		public static void PrintArray(System.IO.TextWriter @out, object[] obj)
		{
			@out.WriteLine(PrintArray(obj, true));
		}

		public static string PrintArray(object[] obj)
		{
			return PrintArray(obj, true);
		}

		public static string PrintArray(object[] obj, bool braces)
		{
			return PrintArray(obj, braces, false);
		}

		public static string PrintArray(object[] obj, bool braces, bool quote)
		{
			if (obj == null)
			{
				return null;
			}
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			if (braces)
			{
				buf.Append('{');
			}
			for (int i = 0; i < obj.Length; i++)
			{
				if (obj[i] != null)
				{
					buf.Append(quote ? "\"" : string.Empty).Append(obj[i]).Append(quote ? "\"" : string.Empty
						);
				}
				else
				{
					buf.Append("null");
				}
				if (i < obj.Length - 1)
				{
					buf.Append(", ");
				}
			}
			if (braces)
			{
				buf.Append('}');
			}
			return buf.ToString();
		}

		/// <param name="ch">the character to test if it's part of an identifier</param>
		/// <returns>true, if character is one of (a, ... z, A, ... Z, 0, ... 9, _)</returns>
		public static bool IsJavaIdentifierPart(char ch)
		{
			return ((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z')) || ((ch >= '0'
				) && (ch <= '9')) || (ch == '_');
		}

		/// <summary>
		/// Encode byte array it into Java identifier string, i.e., a string
		/// that only contains the following characters: (a, ...
		/// </summary>
		/// <remarks>
		/// Encode byte array it into Java identifier string, i.e., a string
		/// that only contains the following characters: (a, ... z, A, ... Z,
		/// 0, ... 9, _, $).  The encoding algorithm itself is not too
		/// clever: if the current byte's ASCII value already is a valid Java
		/// identifier part, leave it as it is. Otherwise it writes the
		/// escape character($) followed by:
		/// <ul>
		/// <li> the ASCII value as a hexadecimal string, if the value is not in the range 200..247</li>
		/// <li>a Java identifier char not used in a lowercase hexadecimal string, if the value is in the range 200..247</li>
		/// </ul>
		/// <p>This operation inflates the original byte array by roughly 40-50%</p>
		/// </remarks>
		/// <param name="bytes">the byte array to convert</param>
		/// <param name="compress">use gzip to minimize string</param>
		/// <exception cref="IOException">if there's a gzip exception</exception>
		public static string Encode(byte[] bytes, bool compress)
		{
			if (compress)
			{
				using (System.IO.MemoryStream baos = new System.IO.MemoryStream())
				{
					using (GZipStream gos = new GZipStream(baos, CompressionLevel.Optimal))
					{
						gos.Write(bytes, 0, bytes.Length);
						bytes = baos.ToArray();
					}
				}
			}
			java.io.CharArrayWriter caw = new java.io.CharArrayWriter(new char[bytes.Length]);
			using (var jw = new NBCEL.classfile.Utility.JavaWriter(caw))
			{
				foreach (byte b in bytes)
				{
					int @in = b & unchecked((int)(0x000000ff));
					// Normalize to unsigned
					jw.Write(@in);
				}
			}
			return caw.ToString();
		}

		/// <summary>Decode a string back to a byte array.</summary>
		/// <param name="s">the string to convert</param>
		/// <param name="uncompress">use gzip to uncompress the stream of bytes</param>
		/// <exception cref="IOException">if there's a gzip exception</exception>
		public static byte[] Decode(string s, bool uncompress)
		{
			byte[] bytes;
			using (NBCEL.classfile.Utility.JavaReader jr = new NBCEL.classfile.Utility.JavaReader
				(new CharArrayReader(s.ToCharArray())))
			{
				using (System.IO.MemoryStream bos = new System.IO.MemoryStream())
				{
					int ch;
					while ((ch = jr.Read()) >= 0)
					{
						bos.WriteByte((byte) ch);
					}
					bytes = bos.ToArray();
				}
			}
			if (uncompress)
			{
				GZipStream gis = new GZipStream(new MemoryStream(bytes), CompressionLevel.Optimal);
				byte[] tmp = new byte[bytes.Length * 3];
				// Rough estimate
				int count = 0;
				int b;
				while ((b = gis.ReadByte()) >= 0)
				{
					tmp[count++] = unchecked((byte)b);
				}
				bytes = new byte[count];
				System.Array.Copy(tmp, 0, bytes, 0, count);
			}
			return bytes;
		}

		private const int FREE_CHARS = 48;

		private static int[] CHAR_MAP = new int[FREE_CHARS];

		private static int[] MAP_CHAR = new int[256];

		private const char ESCAPE_CHAR = '$';

		static Utility()
		{
			// A-Z, g-z, _, $
			// Reverse map
			int j = 0;
			for (int i = 'A'; i <= 'Z'; i++)
			{
				CHAR_MAP[j] = i;
				MAP_CHAR[i] = j;
				j++;
			}
			for (int i = 'g'; i <= 'z'; i++)
			{
				CHAR_MAP[j] = i;
				MAP_CHAR[i] = j;
				j++;
			}
			CHAR_MAP[j] = '$';
			MAP_CHAR['$'] = j;
			j++;
			CHAR_MAP[j] = '_';
			MAP_CHAR['_'] = j;
		}

		/// <summary>Decode characters into bytes.</summary>
		/// <remarks>
		/// Decode characters into bytes.
		/// Used by <a href="Utility.html#decode(java.lang.String, boolean)">decode()</a>
		/// </remarks>
		private class JavaReader : TextReader
		{
			public CharArrayReader In { get; }

			public JavaReader(CharArrayReader @in)
			{
				In = @in;
			}

			/// <exception cref="IOException"/>
			public override int Read()
			{
				int b = In.Read();
				if (b != ESCAPE_CHAR)
				{
					return b;
				}
				int i = In.Read();
				if (i < 0)
				{
					return -1;
				}
				if (((i >= '0') && (i <= '9')) || ((i >= 'a') && (i <= 'f')))
				{
					// Normal escape
					int j = In.Read();
					if (j < 0)
					{
						return -1;
					}
					char[] tmp = new char[] { (char)i, (char)j };
					int s = System.Convert.ToInt32(new string(tmp), 16);
					return s;
				}
				return MAP_CHAR[i];
			}

			/// <exception cref="IOException"/>
			public override int Read(char[] cbuf, int off, int len)
			{
				for (int i = 0; i < len; i++)
				{
					cbuf[off + i] = (char)Read();
				}
				return len;
			}
		}

		/// <summary>Encode bytes into valid java identifier characters.</summary>
		/// <remarks>
		/// Encode bytes into valid java identifier characters.
		/// Used by <a href="Utility.html#encode(byte[], boolean)">encode()</a>
		/// </remarks>
		private class JavaWriter : TextWriter
		{
			public CharArrayWriter Out { get; }

			public JavaWriter(CharArrayWriter @out)
			{
				Out = @out;
			}

			/// <exception cref="IOException"/>
			public override void Write(int b)
			{
				if (IsJavaIdentifierPart((char)b) && (b != ESCAPE_CHAR))
				{
					Out.Write(b);
				}
				else
				{
					@Out.Write(ESCAPE_CHAR);
					// Escape character
					// Special escape
					if (b >= 0 && b < FREE_CHARS)
					{
						Out.Write(CHAR_MAP[b]);
					}
					else
					{
						// Normal escape
						char[] tmp = Sharpen.Runtime.ToHexString(b).ToCharArray();
						if (tmp.Length == 1)
						{
							Out.Write('0');
							Out.Write(tmp[0]);
						}
						else
						{
							Out.Write(tmp[0]);
							Out.Write(tmp[1]);
						}
					}
				}
			}

			public override Encoding Encoding => Encoding.Unicode;

			/// <exception cref="IOException"/>
			public override void Write(char[] cbuf, int off, int len)
			{
				for (int i = 0; i < len; i++)
				{
					Write(cbuf[off + i]);
				}
			}

			/// <exception cref="IOException"/>
			public void Write(string str, int off, int len)
			{
				Write(str.ToCharArray(), off, len);
			}
		}

		/// <summary>Escape all occurences of newline chars '\n', quotes \", etc.</summary>
		public static string ConvertString(string label)
		{
			char[] ch = label.ToCharArray();
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			foreach (char element in ch)
			{
				switch (element)
				{
					case '\n':
					{
						buf.Append("\\n");
						break;
					}

					case '\r':
					{
						buf.Append("\\r");
						break;
					}

					case '\"':
					{
						buf.Append("\\\"");
						break;
					}

					case '\'':
					{
						buf.Append("\\'");
						break;
					}

					case '\\':
					{
						buf.Append("\\\\");
						break;
					}

					default:
					{
						buf.Append(element);
						break;
					}
				}
			}
			return buf.ToString();
		}
	}
}
