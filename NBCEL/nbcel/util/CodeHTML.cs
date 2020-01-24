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

using System.IO;
using NFernflower.Java.Util;
using Sharpen;

namespace NBCEL.util
{
	/// <summary>Convert code into HTML file.</summary>
	internal sealed class CodeHTML
	{
		private readonly string class_name;

		private readonly System.IO.TextWriter file;

		private BitSet goto_set;

		private readonly NBCEL.classfile.ConstantPool constant_pool;

		private readonly NBCEL.util.ConstantHTML constant_html;

		private static bool wide = false;

		/// <exception cref="System.IO.IOException"/>
		internal CodeHTML(string dir, string class_name, NBCEL.classfile.Method[] methods
			, NBCEL.classfile.ConstantPool constant_pool, NBCEL.util.ConstantHTML constant_html
			)
		{
			// name of current class
			//    private Method[] methods; // Methods to print
			// file to write to
			this.class_name = class_name;
			//        this.methods = methods;
			this.constant_pool = constant_pool;
			this.constant_html = constant_html;
			file = new System.IO.StreamWriter(System.IO.File.OpenWrite(dir + class_name + "_code.html"));
			file.WriteLine("<HTML><BODY BGCOLOR=\"#C0C0C0\">");
			for (int i = 0; i < methods.Length; i++)
			{
				WriteMethod(methods[i], i);
			}
			file.WriteLine("</BODY></HTML>");
			file.Close();
		}

		/// <summary>
		/// Disassemble a stream of byte codes and return the
		/// string representation.
		/// </summary>
		/// <param name="stream">data input stream</param>
		/// <returns>String representation of byte code</returns>
		/// <exception cref="System.IO.IOException"/>
		private string CodeToHTML(NBCEL.util.ByteSequence bytes, int method_number)
		{
			short opcode = (short)bytes.ReadUnsignedByte();
			string name;
			string signature;
			int default_offset = 0;
			int low;
			int high;
			int index;
			int class_index;
			int vindex;
			int constant;
			int[] jump_table;
			int no_pad_bytes = 0;
			int offset;
			System.Text.StringBuilder buf = new System.Text.StringBuilder(256);
			// CHECKSTYLE IGNORE MagicNumber
			buf.Append("<TT>").Append(NBCEL.Const.GetOpcodeName(opcode)).Append("</TT></TD><TD>"
				);
			/* Special case: Skip (0-3) padding bytes, i.e., the
			* following bytes are 4-byte-aligned
			*/
			if ((opcode == NBCEL.Const.TABLESWITCH) || (opcode == NBCEL.Const.LOOKUPSWITCH))
			{
				int remainder = bytes.GetIndex() % 4;
				no_pad_bytes = (remainder == 0) ? 0 : 4 - remainder;
				for (int i = 0; i < no_pad_bytes; i++)
				{
					bytes.ReadByte();
				}
				// Both cases have a field default_offset in common
				default_offset = bytes.ReadInt();
			}
			switch (opcode)
			{
				case NBCEL.Const.TABLESWITCH:
				{
					low = bytes.ReadInt();
					high = bytes.ReadInt();
					offset = bytes.GetIndex() - 12 - no_pad_bytes - 1;
					default_offset += offset;
					buf.Append("<TABLE BORDER=1><TR>");
					// Print switch indices in first row (and default)
					jump_table = new int[high - low + 1];
					for (int i = 0; i < jump_table.Length; i++)
					{
						jump_table[i] = offset + bytes.ReadInt();
						buf.Append("<TH>").Append(low + i).Append("</TH>");
					}
					buf.Append("<TH>default</TH></TR>\n<TR>");
					// Print target and default indices in second row
					foreach (int element in jump_table)
					{
						buf.Append("<TD><A HREF=\"#code").Append(method_number).Append("@").Append(element
							).Append("\">").Append(element).Append("</A></TD>");
					}
					buf.Append("<TD><A HREF=\"#code").Append(method_number).Append("@").Append(default_offset
						).Append("\">").Append(default_offset).Append("</A></TD></TR>\n</TABLE>\n");
					break;
				}

				case NBCEL.Const.LOOKUPSWITCH:
				{
					/* Lookup switch has variable length arguments.
					*/
					int npairs = bytes.ReadInt();
					offset = bytes.GetIndex() - 8 - no_pad_bytes - 1;
					jump_table = new int[npairs];
					default_offset += offset;
					buf.Append("<TABLE BORDER=1><TR>");
					// Print switch indices in first row (and default)
					for (int i = 0; i < npairs; i++)
					{
						int match = bytes.ReadInt();
						jump_table[i] = offset + bytes.ReadInt();
						buf.Append("<TH>").Append(match).Append("</TH>");
					}
					buf.Append("<TH>default</TH></TR>\n<TR>");
					// Print target and default indices in second row
					for (int i = 0; i < npairs; i++)
					{
						buf.Append("<TD><A HREF=\"#code").Append(method_number).Append("@").Append(jump_table
							[i]).Append("\">").Append(jump_table[i]).Append("</A></TD>");
					}
					buf.Append("<TD><A HREF=\"#code").Append(method_number).Append("@").Append(default_offset
						).Append("\">").Append(default_offset).Append("</A></TD></TR>\n</TABLE>\n");
					break;
				}

				case NBCEL.Const.GOTO:
				case NBCEL.Const.IFEQ:
				case NBCEL.Const.IFGE:
				case NBCEL.Const.IFGT:
				case NBCEL.Const.IFLE:
				case NBCEL.Const.IFLT:
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
				case NBCEL.Const.JSR:
				{
					/* Two address bytes + offset from start of byte stream form the
					* jump target.
					*/
					index = bytes.GetIndex() + bytes.ReadShort() - 1;
					buf.Append("<A HREF=\"#code").Append(method_number).Append("@").Append(index).Append
						("\">").Append(index).Append("</A>");
					break;
				}

				case NBCEL.Const.GOTO_W:
				case NBCEL.Const.JSR_W:
				{
					/* Same for 32-bit wide jumps
					*/
					int windex = bytes.GetIndex() + bytes.ReadInt() - 1;
					buf.Append("<A HREF=\"#code").Append(method_number).Append("@").Append(windex).Append
						("\">").Append(windex).Append("</A>");
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
						vindex = bytes.ReadShort();
						wide = false;
					}
					else
					{
						// Clear flag
						vindex = bytes.ReadUnsignedByte();
					}
					buf.Append("%").Append(vindex);
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
					buf.Append("(wide)");
					break;
				}

				case NBCEL.Const.NEWARRAY:
				{
					/* Array of basic type.
					*/
					buf.Append("<FONT COLOR=\"#00FF00\">").Append(NBCEL.Const.GetTypeName(bytes.ReadByte
						())).Append("</FONT>");
					break;
				}

				case NBCEL.Const.GETFIELD:
				case NBCEL.Const.GETSTATIC:
				case NBCEL.Const.PUTFIELD:
				case NBCEL.Const.PUTSTATIC:
				{
					/* Access object/class fields.
					*/
					index = bytes.ReadShort();
					NBCEL.classfile.ConstantFieldref c1 = (NBCEL.classfile.ConstantFieldref)constant_pool
						.GetConstant(index, NBCEL.Const.CONSTANT_Fieldref);
					class_index = c1.GetClassIndex();
					name = constant_pool.GetConstantString(class_index, NBCEL.Const.CONSTANT_Class);
					name = NBCEL.classfile.Utility.CompactClassName(name, false);
					index = c1.GetNameAndTypeIndex();
					string field_name = constant_pool.ConstantToString(index, NBCEL.Const.CONSTANT_NameAndType
						);
					if (name.Equals(class_name))
					{
						// Local field
						buf.Append("<A HREF=\"").Append(class_name).Append("_methods.html#field").Append(
							field_name).Append("\" TARGET=Methods>").Append(field_name).Append("</A>\n");
					}
					else
					{
						buf.Append(constant_html.ReferenceConstant(class_index)).Append(".").Append(field_name
							);
					}
					break;
				}

				case NBCEL.Const.CHECKCAST:
				case NBCEL.Const.INSTANCEOF:
				case NBCEL.Const.NEW:
				{
					/* Operands are references to classes in constant pool
					*/
					index = bytes.ReadShort();
					buf.Append(constant_html.ReferenceConstant(index));
					break;
				}

				case NBCEL.Const.INVOKESPECIAL:
				case NBCEL.Const.INVOKESTATIC:
				case NBCEL.Const.INVOKEVIRTUAL:
				case NBCEL.Const.INVOKEINTERFACE:
				case NBCEL.Const.INVOKEDYNAMIC:
				{
					/* Operands are references to methods in constant pool
					*/
					int m_index = bytes.ReadShort();
					string str;
					if (opcode == NBCEL.Const.INVOKEINTERFACE)
					{
						// Special treatment needed
						bytes.ReadUnsignedByte();
						// Redundant
						bytes.ReadUnsignedByte();
						// Reserved
						//                    int nargs = bytes.readUnsignedByte(); // Redundant
						//                    int reserved = bytes.readUnsignedByte(); // Reserved
						NBCEL.classfile.ConstantInterfaceMethodref c = (NBCEL.classfile.ConstantInterfaceMethodref
							)constant_pool.GetConstant(m_index, NBCEL.Const.CONSTANT_InterfaceMethodref);
						class_index = c.GetClassIndex();
						index = c.GetNameAndTypeIndex();
						name = NBCEL.util.Class2HTML.ReferenceClass(class_index);
					}
					else if (opcode == NBCEL.Const.INVOKEDYNAMIC)
					{
						// Special treatment needed
						bytes.ReadUnsignedByte();
						// Reserved
						bytes.ReadUnsignedByte();
						// Reserved
						NBCEL.classfile.ConstantInvokeDynamic c = (NBCEL.classfile.ConstantInvokeDynamic)
							constant_pool.GetConstant(m_index, NBCEL.Const.CONSTANT_InvokeDynamic);
						index = c.GetNameAndTypeIndex();
						name = "#" + c.GetBootstrapMethodAttrIndex();
					}
					else
					{
						// UNDONE: Java8 now allows INVOKESPECIAL and INVOKESTATIC to
						// reference EITHER a Methodref OR an InterfaceMethodref.
						// Not sure if that affects this code or not.  (markro)
						NBCEL.classfile.ConstantMethodref c = (NBCEL.classfile.ConstantMethodref)constant_pool
							.GetConstant(m_index, NBCEL.Const.CONSTANT_Methodref);
						class_index = c.GetClassIndex();
						index = c.GetNameAndTypeIndex();
						name = NBCEL.util.Class2HTML.ReferenceClass(class_index);
					}
					str = NBCEL.util.Class2HTML.ToHTML(constant_pool.ConstantToString(constant_pool.GetConstant
						(index, NBCEL.Const.CONSTANT_NameAndType)));
					// Get signature, i.e., types
					NBCEL.classfile.ConstantNameAndType c2 = (NBCEL.classfile.ConstantNameAndType)constant_pool
						.GetConstant(index, NBCEL.Const.CONSTANT_NameAndType);
					signature = constant_pool.ConstantToString(c2.GetSignatureIndex(), NBCEL.Const.CONSTANT_Utf8
						);
					string[] args = NBCEL.classfile.Utility.MethodSignatureArgumentTypes(signature, false
						);
					string type = NBCEL.classfile.Utility.MethodSignatureReturnType(signature, false);
					buf.Append(name).Append(".<A HREF=\"").Append(class_name).Append("_cp.html#cp").Append
						(m_index).Append("\" TARGET=ConstantPool>").Append(str).Append("</A>").Append("("
						);
					// List arguments
					for (int i = 0; i < args.Length; i++)
					{
						buf.Append(NBCEL.util.Class2HTML.ReferenceType(args[i]));
						if (i < args.Length - 1)
						{
							buf.Append(", ");
						}
					}
					// Attach return type
					buf.Append("):").Append(NBCEL.util.Class2HTML.ReferenceType(type));
					break;
				}

				case NBCEL.Const.LDC_W:
				case NBCEL.Const.LDC2_W:
				{
					/* Operands are references to items in constant pool
					*/
					index = bytes.ReadShort();
					buf.Append("<A HREF=\"").Append(class_name).Append("_cp.html#cp").Append(index).Append
						("\" TARGET=\"ConstantPool\">").Append(NBCEL.util.Class2HTML.ToHTML(constant_pool
						.ConstantToString(index, constant_pool.GetConstant(index).GetTag()))).Append("</a>"
						);
					break;
				}

				case NBCEL.Const.LDC:
				{
					index = bytes.ReadUnsignedByte();
					buf.Append("<A HREF=\"").Append(class_name).Append("_cp.html#cp").Append(index).Append
						("\" TARGET=\"ConstantPool\">").Append(NBCEL.util.Class2HTML.ToHTML(constant_pool
						.ConstantToString(index, constant_pool.GetConstant(index).GetTag()))).Append("</a>"
						);
					break;
				}

				case NBCEL.Const.ANEWARRAY:
				{
					/* Array of references.
					*/
					index = bytes.ReadShort();
					buf.Append(constant_html.ReferenceConstant(index));
					break;
				}

				case NBCEL.Const.MULTIANEWARRAY:
				{
					/* Multidimensional array of references.
					*/
					index = bytes.ReadShort();
					int dimensions = bytes.ReadByte();
					buf.Append(constant_html.ReferenceConstant(index)).Append(":").Append(dimensions)
						.Append("-dimensional");
					break;
				}

				case NBCEL.Const.IINC:
				{
					/* Increment local variable.
					*/
					if (wide)
					{
						vindex = bytes.ReadShort();
						constant = bytes.ReadShort();
						wide = false;
					}
					else
					{
						vindex = bytes.ReadUnsignedByte();
						constant = bytes.ReadByte();
					}
					buf.Append("%").Append(vindex).Append(" ").Append(constant);
					break;
				}

				default:
				{
					if (NBCEL.Const.GetNoOfOperands(opcode) > 0)
					{
						for (int i = 0; i < NBCEL.Const.GetOperandTypeCount(opcode); i++)
						{
							switch (NBCEL.Const.GetOperandType(opcode, i))
							{
								case NBCEL.Const.T_BYTE:
								{
									buf.Append(bytes.ReadUnsignedByte());
									break;
								}

								case NBCEL.Const.T_SHORT:
								{
									// Either branch or index
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
									throw new System.InvalidOperationException("Unreachable default case reached! " +
										 NBCEL.Const.GetOperandType(opcode, i));
								}
							}
							buf.Append("&nbsp;");
						}
					}
					break;
				}
			}
			buf.Append("</TD>");
			return buf.ToString();
		}

		/// <summary>
		/// Find all target addresses in code, so that they can be marked
		/// with &lt;A NAME = ...&gt;.
		/// </summary>
		/// <remarks>
		/// Find all target addresses in code, so that they can be marked
		/// with &lt;A NAME = ...&gt;. Target addresses are kept in an BitSet object.
		/// </remarks>
		/// <exception cref="System.IO.IOException"/>
		private void FindGotos(NBCEL.util.ByteSequence bytes, NBCEL.classfile.Code code)
		{
			int index;
			goto_set = new BitSet(bytes.Available());
			int opcode;
			/* First get Code attribute from method and the exceptions handled
			* (try .. catch) in this method. We only need the line number here.
			*/
			if (code != null)
			{
				NBCEL.classfile.CodeException[] ce = code.GetExceptionTable();
				foreach (NBCEL.classfile.CodeException cex in ce)
				{
					goto_set.Set(cex.GetStartPC());
					goto_set.Set(cex.GetEndPC());
					goto_set.Set(cex.GetHandlerPC());
				}
				// Look for local variables and their range
				NBCEL.classfile.Attribute[] attributes = code.GetAttributes();
				foreach (NBCEL.classfile.Attribute attribute in attributes)
				{
					if (attribute.GetTag() == NBCEL.Const.ATTR_LOCAL_VARIABLE_TABLE)
					{
						NBCEL.classfile.LocalVariable[] vars = ((NBCEL.classfile.LocalVariableTable)attribute
							).GetLocalVariableTable();
						foreach (NBCEL.classfile.LocalVariable var in vars)
						{
							int start = var.GetStartPC();
							int end = start + var.GetLength();
							goto_set.Set(start);
							goto_set.Set(end);
						}
						break;
					}
				}
			}
			// Get target addresses from GOTO, JSR, TABLESWITCH, etc.
			for (; bytes.Available() > 0; )
			{
				opcode = bytes.ReadUnsignedByte();
				switch (opcode)
				{
					case NBCEL.Const.TABLESWITCH:
					case NBCEL.Const.LOOKUPSWITCH:
					{
						//System.out.println(getOpcodeName(opcode));
						//bytes.readByte(); // Skip already read byte
						int remainder = bytes.GetIndex() % 4;
						int no_pad_bytes = (remainder == 0) ? 0 : 4 - remainder;
						int default_offset;
						int offset;
						for (int j = 0; j < no_pad_bytes; j++)
						{
							bytes.ReadByte();
						}
						// Both cases have a field default_offset in common
						default_offset = bytes.ReadInt();
						if (opcode == NBCEL.Const.TABLESWITCH)
						{
							int low = bytes.ReadInt();
							int high = bytes.ReadInt();
							offset = bytes.GetIndex() - 12 - no_pad_bytes - 1;
							default_offset += offset;
							goto_set.Set(default_offset);
							for (int j = 0; j < (high - low + 1); j++)
							{
								index = offset + bytes.ReadInt();
								goto_set.Set(index);
							}
						}
						else
						{
							// LOOKUPSWITCH
							int npairs = bytes.ReadInt();
							offset = bytes.GetIndex() - 8 - no_pad_bytes - 1;
							default_offset += offset;
							goto_set.Set(default_offset);
							for (int j = 0; j < npairs; j++)
							{
								//                            int match = bytes.readInt();
								bytes.ReadInt();
								index = offset + bytes.ReadInt();
								goto_set.Set(index);
							}
						}
						break;
					}

					case NBCEL.Const.GOTO:
					case NBCEL.Const.IFEQ:
					case NBCEL.Const.IFGE:
					case NBCEL.Const.IFGT:
					case NBCEL.Const.IFLE:
					case NBCEL.Const.IFLT:
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
					case NBCEL.Const.JSR:
					{
						//bytes.readByte(); // Skip already read byte
						index = bytes.GetIndex() + bytes.ReadShort() - 1;
						goto_set.Set(index);
						break;
					}

					case NBCEL.Const.GOTO_W:
					case NBCEL.Const.JSR_W:
					{
						//bytes.readByte(); // Skip already read byte
						index = bytes.GetIndex() + bytes.ReadInt() - 1;
						goto_set.Set(index);
						break;
					}

					default:
					{
						bytes.UnreadByte();
						CodeToHTML(bytes, 0);
						break;
					}
				}
			}
		}

		// Ignore output
		/// <summary>Write a single method with the byte code associated with it.</summary>
		/// <exception cref="System.IO.IOException"/>
		private void WriteMethod(NBCEL.classfile.Method method, int method_number)
		{
			// Get raw signature
			string signature = method.GetSignature();
			// Get array of strings containing the argument types
			string[] args = NBCEL.classfile.Utility.MethodSignatureArgumentTypes(signature, false
				);
			// Get return type string
			string type = NBCEL.classfile.Utility.MethodSignatureReturnType(signature, false);
			// Get method name
			string name = method.GetName();
			string html_name = NBCEL.util.Class2HTML.ToHTML(name);
			// Get method's access flags
			string access = NBCEL.classfile.Utility.AccessToString(method.GetAccessFlags());
			access = NBCEL.classfile.Utility.Replace(access, " ", "&nbsp;");
			// Get the method's attributes, the Code Attribute in particular
			NBCEL.classfile.Attribute[] attributes = method.GetAttributes();
			file.Write("<P><B><FONT COLOR=\"#FF0000\">" + access + "</FONT>&nbsp;" + "<A NAME=method"
				 + method_number + ">" + NBCEL.util.Class2HTML.ReferenceType(type) + "</A>&nbsp<A HREF=\""
				 + class_name + "_methods.html#method" + method_number + "\" TARGET=Methods>" + 
				html_name + "</A>(");
			for (int i = 0; i < args.Length; i++)
			{
				file.Write(NBCEL.util.Class2HTML.ReferenceType(args[i]));
				if (i < args.Length - 1)
				{
					file.Write(",&nbsp;");
				}
			}
			file.WriteLine(")</B></P>");
			NBCEL.classfile.Code c = null;
			byte[] code = null;
			if (attributes.Length > 0)
			{
				file.Write("<H4>Attributes</H4><UL>\n");
				for (int i = 0; i < attributes.Length; i++)
				{
					byte tag = attributes[i].GetTag();
					if (tag != NBCEL.Const.ATTR_UNKNOWN)
					{
						file.Write("<LI><A HREF=\"" + class_name + "_attributes.html#method" + method_number
							 + "@" + i + "\" TARGET=Attributes>" + NBCEL.Const.GetAttributeName(tag) + "</A></LI>\n"
							);
					}
					else
					{
						file.Write("<LI>" + attributes[i] + "</LI>");
					}
					if (tag == NBCEL.Const.ATTR_CODE)
					{
						c = (NBCEL.classfile.Code)attributes[i];
						NBCEL.classfile.Attribute[] attributes2 = c.GetAttributes();
						code = c.GetCode();
						file.Write("<UL>");
						for (int j = 0; j < attributes2.Length; j++)
						{
							tag = attributes2[j].GetTag();
							file.Write("<LI><A HREF=\"" + class_name + "_attributes.html#" + "method" + method_number
								 + "@" + i + "@" + j + "\" TARGET=Attributes>" + NBCEL.Const.GetAttributeName(tag
								) + "</A></LI>\n");
						}
						file.Write("</UL>");
					}
				}
				file.WriteLine("</UL>");
			}
			if (code != null)
			{
				// No code, an abstract method, e.g.
				//System.out.println(name + "\n" + Utility.codeToString(code, constant_pool, 0, -1));
				// Print the byte code
				using (NBCEL.util.ByteSequence stream = new NBCEL.util.ByteSequence(code))
				{
					stream.Mark(stream.Available());
					FindGotos(stream, c);
					stream.Reset();
					file.WriteLine("<TABLE BORDER=0><TR><TH ALIGN=LEFT>Byte<BR>offset</TH>" + "<TH ALIGN=LEFT>Instruction</TH><TH ALIGN=LEFT>Argument</TH>"
						);
					for (; stream.Available() > 0; )
					{
						int offset = stream.GetIndex();
						string str = CodeToHTML(stream, method_number);
						string anchor = string.Empty;
						/*
						* Set an anchor mark if this line is targetted by a goto, jsr, etc. Defining an anchor for every
						* line is very inefficient!
						*/
						if (goto_set.Get(offset))
						{
							anchor = "<A NAME=code" + method_number + "@" + offset + "></A>";
						}
						string anchor2;
						if (stream.GetIndex() == code.Length)
						{
							anchor2 = "<A NAME=code" + method_number + "@" + code.Length + ">" + offset + "</A>";
						}
						else
						{
							anchor2 = string.Empty + offset;
						}
						file.WriteLine("<TR VALIGN=TOP><TD>" + anchor2 + "</TD><TD>" + anchor + str + "</TR>"
							);
					}
				}
				// Mark last line, may be targetted from Attributes window
				file.WriteLine("<TR><TD> </A></TD></TR>");
				file.WriteLine("</TABLE>");
			}
		}
	}
}
