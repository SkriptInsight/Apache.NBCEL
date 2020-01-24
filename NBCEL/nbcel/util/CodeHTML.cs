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
using System.IO;
using System.Text;
using NBCEL.classfile;
using NFernflower.Java.Util;
using Sharpen;

namespace NBCEL.util
{
    /// <summary>Convert code into HTML file.</summary>
    internal sealed class CodeHTML
    {
        private static bool wide;
        private readonly string class_name;

        private readonly ConstantHTML constant_html;

        private readonly ConstantPool constant_pool;

        private readonly TextWriter file;

        private BitSet goto_set;

        /// <exception cref="System.IO.IOException" />
        internal CodeHTML(string dir, string class_name, Method[] methods
            , ConstantPool constant_pool, ConstantHTML constant_html
        )
        {
            // name of current class
            //    private Method[] methods; // Methods to print
            // file to write to
            this.class_name = class_name;
            //        this.methods = methods;
            this.constant_pool = constant_pool;
            this.constant_html = constant_html;
            file = new StreamWriter(File.OpenWrite(dir + class_name + "_code.html"));
            file.WriteLine("<HTML><BODY BGCOLOR=\"#C0C0C0\">");
            for (var i = 0; i < methods.Length; i++) WriteMethod(methods[i], i);
            file.WriteLine("</BODY></HTML>");
            file.Close();
        }

        /// <summary>
        ///     Disassemble a stream of byte codes and return the
        ///     string representation.
        /// </summary>
        /// <param name="stream">data input stream</param>
        /// <returns>String representation of byte code</returns>
        /// <exception cref="System.IO.IOException" />
        private string CodeToHTML(ByteSequence bytes, int method_number)
        {
            var opcode = (short) bytes.ReadUnsignedByte();
            string name;
            string signature;
            var default_offset = 0;
            int low;
            int high;
            int index;
            int class_index;
            int vindex;
            int constant;
            int[] jump_table;
            var no_pad_bytes = 0;
            int offset;
            var buf = new StringBuilder(256);
            // CHECKSTYLE IGNORE MagicNumber
            buf.Append("<TT>").Append(Const.GetOpcodeName(opcode)).Append("</TT></TD><TD>"
            );
            /* Special case: Skip (0-3) padding bytes, i.e., the
            * following bytes are 4-byte-aligned
            */
            if (opcode == Const.TABLESWITCH || opcode == Const.LOOKUPSWITCH)
            {
                var remainder = bytes.GetIndex() % 4;
                no_pad_bytes = remainder == 0 ? 0 : 4 - remainder;
                for (var i = 0; i < no_pad_bytes; i++) bytes.ReadByte();
                // Both cases have a field default_offset in common
                default_offset = bytes.ReadInt();
            }

            switch (opcode)
            {
                case Const.TABLESWITCH:
                {
                    low = bytes.ReadInt();
                    high = bytes.ReadInt();
                    offset = bytes.GetIndex() - 12 - no_pad_bytes - 1;
                    default_offset += offset;
                    buf.Append("<TABLE BORDER=1><TR>");
                    // Print switch indices in first row (and default)
                    jump_table = new int[high - low + 1];
                    for (var i = 0; i < jump_table.Length; i++)
                    {
                        jump_table[i] = offset + bytes.ReadInt();
                        buf.Append("<TH>").Append(low + i).Append("</TH>");
                    }

                    buf.Append("<TH>default</TH></TR>\n<TR>");
                    // Print target and default indices in second row
                    foreach (var element in jump_table)
                        buf.Append("<TD><A HREF=\"#code").Append(method_number).Append("@").Append(element
                        ).Append("\">").Append(element).Append("</A></TD>");
                    buf.Append("<TD><A HREF=\"#code").Append(method_number).Append("@").Append(default_offset
                    ).Append("\">").Append(default_offset).Append("</A></TD></TR>\n</TABLE>\n");
                    break;
                }

                case Const.LOOKUPSWITCH:
                {
                    /* Lookup switch has variable length arguments.
                    */
                    var npairs = bytes.ReadInt();
                    offset = bytes.GetIndex() - 8 - no_pad_bytes - 1;
                    jump_table = new int[npairs];
                    default_offset += offset;
                    buf.Append("<TABLE BORDER=1><TR>");
                    // Print switch indices in first row (and default)
                    for (var i = 0; i < npairs; i++)
                    {
                        var match = bytes.ReadInt();
                        jump_table[i] = offset + bytes.ReadInt();
                        buf.Append("<TH>").Append(match).Append("</TH>");
                    }

                    buf.Append("<TH>default</TH></TR>\n<TR>");
                    // Print target and default indices in second row
                    for (var i = 0; i < npairs; i++)
                        buf.Append("<TD><A HREF=\"#code").Append(method_number).Append("@").Append(jump_table
                            [i]).Append("\">").Append(jump_table[i]).Append("</A></TD>");
                    buf.Append("<TD><A HREF=\"#code").Append(method_number).Append("@").Append(default_offset
                    ).Append("\">").Append(default_offset).Append("</A></TD></TR>\n</TABLE>\n");
                    break;
                }

                case Const.GOTO:
                case Const.IFEQ:
                case Const.IFGE:
                case Const.IFGT:
                case Const.IFLE:
                case Const.IFLT:
                case Const.IFNE:
                case Const.IFNONNULL:
                case Const.IFNULL:
                case Const.IF_ACMPEQ:
                case Const.IF_ACMPNE:
                case Const.IF_ICMPEQ:
                case Const.IF_ICMPGE:
                case Const.IF_ICMPGT:
                case Const.IF_ICMPLE:
                case Const.IF_ICMPLT:
                case Const.IF_ICMPNE:
                case Const.JSR:
                {
                    /* Two address bytes + offset from start of byte stream form the
                    * jump target.
                    */
                    index = bytes.GetIndex() + bytes.ReadShort() - 1;
                    buf.Append("<A HREF=\"#code").Append(method_number).Append("@").Append(index).Append
                        ("\">").Append(index).Append("</A>");
                    break;
                }

                case Const.GOTO_W:
                case Const.JSR_W:
                {
                    /* Same for 32-bit wide jumps
                    */
                    var windex = bytes.GetIndex() + bytes.ReadInt() - 1;
                    buf.Append("<A HREF=\"#code").Append(method_number).Append("@").Append(windex).Append
                        ("\">").Append(windex).Append("</A>");
                    break;
                }

                case Const.ALOAD:
                case Const.ASTORE:
                case Const.DLOAD:
                case Const.DSTORE:
                case Const.FLOAD:
                case Const.FSTORE:
                case Const.ILOAD:
                case Const.ISTORE:
                case Const.LLOAD:
                case Const.LSTORE:
                case Const.RET:
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

                case Const.WIDE:
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

                case Const.NEWARRAY:
                {
                    /* Array of basic type.
                    */
                    buf.Append("<FONT COLOR=\"#00FF00\">").Append(Const.GetTypeName(bytes.ReadByte
                        ())).Append("</FONT>");
                    break;
                }

                case Const.GETFIELD:
                case Const.GETSTATIC:
                case Const.PUTFIELD:
                case Const.PUTSTATIC:
                {
                    /* Access object/class fields.
                    */
                    index = bytes.ReadShort();
                    var c1 = (ConstantFieldref) constant_pool
                        .GetConstant(index, Const.CONSTANT_Fieldref);
                    class_index = c1.GetClassIndex();
                    name = constant_pool.GetConstantString(class_index, Const.CONSTANT_Class);
                    name = Utility.CompactClassName(name, false);
                    index = c1.GetNameAndTypeIndex();
                    var field_name = constant_pool.ConstantToString(index, Const.CONSTANT_NameAndType
                    );
                    if (name.Equals(class_name))
                        // Local field
                        buf.Append("<A HREF=\"").Append(class_name).Append("_methods.html#field").Append(
                            field_name).Append("\" TARGET=Methods>").Append(field_name).Append("</A>\n");
                    else
                        buf.Append(constant_html.ReferenceConstant(class_index)).Append(".").Append(field_name
                        );
                    break;
                }

                case Const.CHECKCAST:
                case Const.INSTANCEOF:
                case Const.NEW:
                {
                    /* Operands are references to classes in constant pool
                    */
                    index = bytes.ReadShort();
                    buf.Append(constant_html.ReferenceConstant(index));
                    break;
                }

                case Const.INVOKESPECIAL:
                case Const.INVOKESTATIC:
                case Const.INVOKEVIRTUAL:
                case Const.INVOKEINTERFACE:
                case Const.INVOKEDYNAMIC:
                {
                    /* Operands are references to methods in constant pool
                    */
                    int m_index = bytes.ReadShort();
                    string str;
                    if (opcode == Const.INVOKEINTERFACE)
                    {
                        // Special treatment needed
                        bytes.ReadUnsignedByte();
                        // Redundant
                        bytes.ReadUnsignedByte();
                        // Reserved
                        //                    int nargs = bytes.readUnsignedByte(); // Redundant
                        //                    int reserved = bytes.readUnsignedByte(); // Reserved
                        var c = (ConstantInterfaceMethodref
                            ) constant_pool.GetConstant(m_index, Const.CONSTANT_InterfaceMethodref);
                        class_index = c.GetClassIndex();
                        index = c.GetNameAndTypeIndex();
                        name = Class2HTML.ReferenceClass(class_index);
                    }
                    else if (opcode == Const.INVOKEDYNAMIC)
                    {
                        // Special treatment needed
                        bytes.ReadUnsignedByte();
                        // Reserved
                        bytes.ReadUnsignedByte();
                        // Reserved
                        var c = (ConstantInvokeDynamic)
                            constant_pool.GetConstant(m_index, Const.CONSTANT_InvokeDynamic);
                        index = c.GetNameAndTypeIndex();
                        name = "#" + c.GetBootstrapMethodAttrIndex();
                    }
                    else
                    {
                        // UNDONE: Java8 now allows INVOKESPECIAL and INVOKESTATIC to
                        // reference EITHER a Methodref OR an InterfaceMethodref.
                        // Not sure if that affects this code or not.  (markro)
                        var c = (ConstantMethodref) constant_pool
                            .GetConstant(m_index, Const.CONSTANT_Methodref);
                        class_index = c.GetClassIndex();
                        index = c.GetNameAndTypeIndex();
                        name = Class2HTML.ReferenceClass(class_index);
                    }

                    str = Class2HTML.ToHTML(constant_pool.ConstantToString(constant_pool.GetConstant
                        (index, Const.CONSTANT_NameAndType)));
                    // Get signature, i.e., types
                    var c2 = (ConstantNameAndType) constant_pool
                        .GetConstant(index, Const.CONSTANT_NameAndType);
                    signature = constant_pool.ConstantToString(c2.GetSignatureIndex(), Const.CONSTANT_Utf8
                    );
                    var args = Utility.MethodSignatureArgumentTypes(signature, false
                    );
                    var type = Utility.MethodSignatureReturnType(signature, false);
                    buf.Append(name).Append(".<A HREF=\"").Append(class_name).Append("_cp.html#cp").Append
                        (m_index).Append("\" TARGET=ConstantPool>").Append(str).Append("</A>").Append("("
                    );
                    // List arguments
                    for (var i = 0; i < args.Length; i++)
                    {
                        buf.Append(Class2HTML.ReferenceType(args[i]));
                        if (i < args.Length - 1) buf.Append(", ");
                    }

                    // Attach return type
                    buf.Append("):").Append(Class2HTML.ReferenceType(type));
                    break;
                }

                case Const.LDC_W:
                case Const.LDC2_W:
                {
                    /* Operands are references to items in constant pool
                    */
                    index = bytes.ReadShort();
                    buf.Append("<A HREF=\"").Append(class_name).Append("_cp.html#cp").Append(index).Append
                        ("\" TARGET=\"ConstantPool\">").Append(Class2HTML.ToHTML(constant_pool
                        .ConstantToString(index, constant_pool.GetConstant(index).GetTag()))).Append("</a>"
                    );
                    break;
                }

                case Const.LDC:
                {
                    index = bytes.ReadUnsignedByte();
                    buf.Append("<A HREF=\"").Append(class_name).Append("_cp.html#cp").Append(index).Append
                        ("\" TARGET=\"ConstantPool\">").Append(Class2HTML.ToHTML(constant_pool
                        .ConstantToString(index, constant_pool.GetConstant(index).GetTag()))).Append("</a>"
                    );
                    break;
                }

                case Const.ANEWARRAY:
                {
                    /* Array of references.
                    */
                    index = bytes.ReadShort();
                    buf.Append(constant_html.ReferenceConstant(index));
                    break;
                }

                case Const.MULTIANEWARRAY:
                {
                    /* Multidimensional array of references.
                    */
                    index = bytes.ReadShort();
                    int dimensions = bytes.ReadByte();
                    buf.Append(constant_html.ReferenceConstant(index)).Append(":").Append(dimensions)
                        .Append("-dimensional");
                    break;
                }

                case Const.IINC:
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
                    if (Const.GetNoOfOperands(opcode) > 0)
                        for (var i = 0; i < Const.GetOperandTypeCount(opcode); i++)
                        {
                            switch (Const.GetOperandType(opcode, i))
                            {
                                case Const.T_BYTE:
                                {
                                    buf.Append(bytes.ReadUnsignedByte());
                                    break;
                                }

                                case Const.T_SHORT:
                                {
                                    // Either branch or index
                                    buf.Append(bytes.ReadShort());
                                    break;
                                }

                                case Const.T_INT:
                                {
                                    buf.Append(bytes.ReadInt());
                                    break;
                                }

                                default:
                                {
                                    // Never reached
                                    throw new InvalidOperationException("Unreachable default case reached! " +
                                                                        Const.GetOperandType(opcode, i));
                                }
                            }

                            buf.Append("&nbsp;");
                        }

                    break;
                }
            }

            buf.Append("</TD>");
            return buf.ToString();
        }

        /// <summary>
        ///     Find all target addresses in code, so that they can be marked
        ///     with &lt;A NAME = ...&gt;.
        /// </summary>
        /// <remarks>
        ///     Find all target addresses in code, so that they can be marked
        ///     with &lt;A NAME = ...&gt;. Target addresses are kept in an BitSet object.
        /// </remarks>
        /// <exception cref="System.IO.IOException" />
        private void FindGotos(ByteSequence bytes, Code code)
        {
            int index;
            goto_set = new BitSet(bytes.Available());
            int opcode;
            /* First get Code attribute from method and the exceptions handled
            * (try .. catch) in this method. We only need the line number here.
            */
            if (code != null)
            {
                var ce = code.GetExceptionTable();
                foreach (var cex in ce)
                {
                    goto_set.Set(cex.GetStartPC());
                    goto_set.Set(cex.GetEndPC());
                    goto_set.Set(cex.GetHandlerPC());
                }

                // Look for local variables and their range
                var attributes = code.GetAttributes();
                foreach (var attribute in attributes)
                    if (attribute.GetTag() == Const.ATTR_LOCAL_VARIABLE_TABLE)
                    {
                        var vars = ((LocalVariableTable) attribute
                            ).GetLocalVariableTable();
                        foreach (var var in vars)
                        {
                            var start = var.GetStartPC();
                            var end = start + var.GetLength();
                            goto_set.Set(start);
                            goto_set.Set(end);
                        }

                        break;
                    }
            }

            // Get target addresses from GOTO, JSR, TABLESWITCH, etc.
            for (; bytes.Available() > 0;)
            {
                opcode = bytes.ReadUnsignedByte();
                switch (opcode)
                {
                    case Const.TABLESWITCH:
                    case Const.LOOKUPSWITCH:
                    {
                        //System.out.println(getOpcodeName(opcode));
                        //bytes.readByte(); // Skip already read byte
                        var remainder = bytes.GetIndex() % 4;
                        var no_pad_bytes = remainder == 0 ? 0 : 4 - remainder;
                        int default_offset;
                        int offset;
                        for (var j = 0; j < no_pad_bytes; j++) bytes.ReadByte();
                        // Both cases have a field default_offset in common
                        default_offset = bytes.ReadInt();
                        if (opcode == Const.TABLESWITCH)
                        {
                            var low = bytes.ReadInt();
                            var high = bytes.ReadInt();
                            offset = bytes.GetIndex() - 12 - no_pad_bytes - 1;
                            default_offset += offset;
                            goto_set.Set(default_offset);
                            for (var j = 0; j < high - low + 1; j++)
                            {
                                index = offset + bytes.ReadInt();
                                goto_set.Set(index);
                            }
                        }
                        else
                        {
                            // LOOKUPSWITCH
                            var npairs = bytes.ReadInt();
                            offset = bytes.GetIndex() - 8 - no_pad_bytes - 1;
                            default_offset += offset;
                            goto_set.Set(default_offset);
                            for (var j = 0; j < npairs; j++)
                            {
                                //                            int match = bytes.readInt();
                                bytes.ReadInt();
                                index = offset + bytes.ReadInt();
                                goto_set.Set(index);
                            }
                        }

                        break;
                    }

                    case Const.GOTO:
                    case Const.IFEQ:
                    case Const.IFGE:
                    case Const.IFGT:
                    case Const.IFLE:
                    case Const.IFLT:
                    case Const.IFNE:
                    case Const.IFNONNULL:
                    case Const.IFNULL:
                    case Const.IF_ACMPEQ:
                    case Const.IF_ACMPNE:
                    case Const.IF_ICMPEQ:
                    case Const.IF_ICMPGE:
                    case Const.IF_ICMPGT:
                    case Const.IF_ICMPLE:
                    case Const.IF_ICMPLT:
                    case Const.IF_ICMPNE:
                    case Const.JSR:
                    {
                        //bytes.readByte(); // Skip already read byte
                        index = bytes.GetIndex() + bytes.ReadShort() - 1;
                        goto_set.Set(index);
                        break;
                    }

                    case Const.GOTO_W:
                    case Const.JSR_W:
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
        /// <exception cref="System.IO.IOException" />
        private void WriteMethod(Method method, int method_number)
        {
            // Get raw signature
            var signature = method.GetSignature();
            // Get array of strings containing the argument types
            var args = Utility.MethodSignatureArgumentTypes(signature, false
            );
            // Get return type string
            var type = Utility.MethodSignatureReturnType(signature, false);
            // Get method name
            var name = method.GetName();
            var html_name = Class2HTML.ToHTML(name);
            // Get method's access flags
            var access = Utility.AccessToString(method.GetAccessFlags());
            access = Utility.Replace(access, " ", "&nbsp;");
            // Get the method's attributes, the Code Attribute in particular
            var attributes = method.GetAttributes();
            file.Write("<P><B><FONT COLOR=\"#FF0000\">" + access + "</FONT>&nbsp;" + "<A NAME=method"
                       + method_number + ">" + Class2HTML.ReferenceType(type) + "</A>&nbsp<A HREF=\""
                       + class_name + "_methods.html#method" + method_number + "\" TARGET=Methods>" +
                       html_name + "</A>(");
            for (var i = 0; i < args.Length; i++)
            {
                file.Write(Class2HTML.ReferenceType(args[i]));
                if (i < args.Length - 1) file.Write(",&nbsp;");
            }

            file.WriteLine(")</B></P>");
            Code c = null;
            byte[] code = null;
            if (attributes.Length > 0)
            {
                file.Write("<H4>Attributes</H4><UL>\n");
                for (var i = 0; i < attributes.Length; i++)
                {
                    var tag = attributes[i].GetTag();
                    if (tag != Const.ATTR_UNKNOWN)
                        file.Write("<LI><A HREF=\"" + class_name + "_attributes.html#method" + method_number
                                   + "@" + i + "\" TARGET=Attributes>" + Const.GetAttributeName(tag) + "</A></LI>\n"
                        );
                    else
                        file.Write("<LI>" + attributes[i] + "</LI>");
                    if (tag == Const.ATTR_CODE)
                    {
                        c = (Code) attributes[i];
                        var attributes2 = c.GetAttributes();
                        code = c.GetCode();
                        file.Write("<UL>");
                        for (var j = 0; j < attributes2.Length; j++)
                        {
                            tag = attributes2[j].GetTag();
                            file.Write("<LI><A HREF=\"" + class_name + "_attributes.html#" + "method" + method_number
                                       + "@" + i + "@" + j + "\" TARGET=Attributes>" + Const.GetAttributeName(tag
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
                using (var stream = new ByteSequence(code))
                {
                    stream.Mark(stream.Available());
                    FindGotos(stream, c);
                    stream.Reset();
                    file.WriteLine("<TABLE BORDER=0><TR><TH ALIGN=LEFT>Byte<BR>offset</TH>" +
                                   "<TH ALIGN=LEFT>Instruction</TH><TH ALIGN=LEFT>Argument</TH>"
                    );
                    for (; stream.Available() > 0;)
                    {
                        var offset = stream.GetIndex();
                        var str = CodeToHTML(stream, method_number);
                        var anchor = string.Empty;
                        /*
                        * Set an anchor mark if this line is targetted by a goto, jsr, etc. Defining an anchor for every
                        * line is very inefficient!
                        */
                        if (goto_set.Get(offset)) anchor = "<A NAME=code" + method_number + "@" + offset + "></A>";
                        string anchor2;
                        if (stream.GetIndex() == code.Length)
                            anchor2 = "<A NAME=code" + method_number + "@" + code.Length + ">" + offset + "</A>";
                        else
                            anchor2 = string.Empty + offset;
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