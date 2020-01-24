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
using Sharpen;

namespace NBCEL.util
{
	/// <summary>Convert found attributes into HTML file.</summary>
	internal sealed class AttributeHTML
	{
		private readonly string class_name;

		private readonly TextWriter file;

		private int attr_count = 0;

		private readonly NBCEL.util.ConstantHTML constant_html;

		private readonly NBCEL.classfile.ConstantPool constant_pool;

		/// <exception cref="System.IO.IOException"/>
		internal AttributeHTML(string dir, string class_name, NBCEL.classfile.ConstantPool
			 constant_pool, NBCEL.util.ConstantHTML constant_html)
		{
			// name of current class
			// file to write to
			this.class_name = class_name;
			this.constant_pool = constant_pool;
			this.constant_html = constant_html;
			file = new System.IO.StreamWriter(File.OpenWrite(dir + class_name + "_attributes.html"));
			file.WriteLine("<HTML><BODY BGCOLOR=\"#C0C0C0\"><TABLE BORDER=0>");
		}

		private string CodeLink(int link, int method_number)
		{
			return "<A HREF=\"" + class_name + "_code.html#code" + method_number + "@" + link
				 + "\" TARGET=Code>" + link + "</A>";
		}

		internal void Close()
		{
			file.WriteLine("</TABLE></BODY></HTML>");
			file.Close();
		}

		internal void WriteAttribute(NBCEL.classfile.Attribute attribute, string anchor)
		{
			WriteAttribute(attribute, anchor, 0);
		}

		internal void WriteAttribute(NBCEL.classfile.Attribute attribute, string anchor, 
			int method_number)
		{
			byte tag = attribute.GetTag();
			int index;
			if (tag == NBCEL.Const.ATTR_UNKNOWN)
			{
				return;
			}
			attr_count++;
			// Increment number of attributes found so far
			if (attr_count % 2 == 0)
			{
				file.Write("<TR BGCOLOR=\"#C0C0C0\"><TD>");
			}
			else
			{
				file.Write("<TR BGCOLOR=\"#A0A0A0\"><TD>");
			}
			file.WriteLine("<H4><A NAME=\"" + anchor + "\">" + attr_count + " " + NBCEL.Const.GetAttributeName
				(tag) + "</A></H4>");
			switch (tag)
			{
				case NBCEL.Const.ATTR_CODE:
				{
					/* Handle different attributes
					*/
					NBCEL.classfile.Code c = (NBCEL.classfile.Code)attribute;
					// Some directly printable values
					file.Write("<UL><LI>Maximum stack size = " + c.GetMaxStack() + "</LI>\n<LI>Number of local variables = "
						 + c.GetMaxLocals() + "</LI>\n<LI><A HREF=\"" + class_name + "_code.html#method"
						 + method_number + "\" TARGET=Code>Byte code</A></LI></UL>\n");
					// Get handled exceptions and list them
					NBCEL.classfile.CodeException[] ce = c.GetExceptionTable();
					int len = ce.Length;
					if (len > 0)
					{
						file.Write("<P><B>Exceptions handled</B><UL>");
						foreach (NBCEL.classfile.CodeException cex in ce)
						{
							int catch_type = cex.GetCatchType();
							// Index in constant pool
							file.Write("<LI>");
							if (catch_type != 0)
							{
								file.Write(constant_html.ReferenceConstant(catch_type));
							}
							else
							{
								// Create Link to _cp.html
								file.Write("Any Exception");
							}
							file.Write("<BR>(Ranging from lines " + CodeLink(cex.GetStartPC(), method_number)
								 + " to " + CodeLink(cex.GetEndPC(), method_number) + ", handled at line " + CodeLink
								(cex.GetHandlerPC(), method_number) + ")</LI>");
						}
						file.Write("</UL>");
					}
					break;
				}

				case NBCEL.Const.ATTR_CONSTANT_VALUE:
				{
					index = ((NBCEL.classfile.ConstantValue)attribute).GetConstantValueIndex();
					// Reference _cp.html
					file.Write("<UL><LI><A HREF=\"" + class_name + "_cp.html#cp" + index + "\" TARGET=\"ConstantPool\">Constant value index("
						 + index + ")</A></UL>\n");
					break;
				}

				case NBCEL.Const.ATTR_SOURCE_FILE:
				{
					index = ((NBCEL.classfile.SourceFile)attribute).GetSourceFileIndex();
					// Reference _cp.html
					file.Write("<UL><LI><A HREF=\"" + class_name + "_cp.html#cp" + index + "\" TARGET=\"ConstantPool\">Source file index("
						 + index + ")</A></UL>\n");
					break;
				}

				case NBCEL.Const.ATTR_EXCEPTIONS:
				{
					// List thrown exceptions
					int[] indices = ((NBCEL.classfile.ExceptionTable)attribute).GetExceptionIndexTable
						();
					file.Write("<UL>");
					foreach (int indice in indices)
					{
						file.Write("<LI><A HREF=\"" + class_name + "_cp.html#cp" + indice + "\" TARGET=\"ConstantPool\">Exception class index("
							 + indice + ")</A>\n");
					}
					file.Write("</UL>\n");
					break;
				}

				case NBCEL.Const.ATTR_LINE_NUMBER_TABLE:
				{
					NBCEL.classfile.LineNumber[] line_numbers = ((NBCEL.classfile.LineNumberTable)attribute
						).GetLineNumberTable();
					// List line number pairs
					file.Write("<P>");
					for (int i = 0; i < line_numbers.Length; i++)
					{
						file.Write("(" + line_numbers[i].GetStartPC() + ",&nbsp;" + line_numbers[i].GetLineNumber
							() + ")");
						if (i < line_numbers.Length - 1)
						{
							file.Write(", ");
						}
					}
					// breakable
					break;
				}

				case NBCEL.Const.ATTR_LOCAL_VARIABLE_TABLE:
				{
					NBCEL.classfile.LocalVariable[] vars = ((NBCEL.classfile.LocalVariableTable)attribute
						).GetLocalVariableTable();
					// List name, range and type
					file.Write("<UL>");
					foreach (NBCEL.classfile.LocalVariable var in vars)
					{
						index = var.GetSignatureIndex();
						string signature = ((NBCEL.classfile.ConstantUtf8)constant_pool.GetConstant(index
							, NBCEL.Const.CONSTANT_Utf8)).GetBytes();
						signature = NBCEL.classfile.Utility.SignatureToString(signature, false);
						int start = var.GetStartPC();
						int end = start + var.GetLength();
						file.WriteLine("<LI>" + NBCEL.util.Class2HTML.ReferenceType(signature) + "&nbsp;<B>"
							 + var.GetName() + "</B> in slot %" + var.GetIndex() + "<BR>Valid from lines " +
							 "<A HREF=\"" + class_name + "_code.html#code" + method_number + "@" + start + "\" TARGET=Code>"
							 + start + "</A> to " + "<A HREF=\"" + class_name + "_code.html#code" + method_number
							 + "@" + end + "\" TARGET=Code>" + end + "</A></LI>");
					}
					file.Write("</UL>\n");
					break;
				}

				case NBCEL.Const.ATTR_INNER_CLASSES:
				{
					NBCEL.classfile.InnerClass[] classes = ((NBCEL.classfile.InnerClasses)attribute).
						GetInnerClasses();
					// List inner classes
					file.Write("<UL>");
					foreach (NBCEL.classfile.InnerClass classe in classes)
					{
						string name;
						string access;
						index = classe.GetInnerNameIndex();
						if (index > 0)
						{
							name = ((NBCEL.classfile.ConstantUtf8)constant_pool.GetConstant(index, NBCEL.Const
								.CONSTANT_Utf8)).GetBytes();
						}
						else
						{
							name = "&lt;anonymous&gt;";
						}
						access = NBCEL.classfile.Utility.AccessToString(classe.GetInnerAccessFlags());
						file.Write("<LI><FONT COLOR=\"#FF0000\">" + access + "</FONT> " + constant_html.ReferenceConstant
							(classe.GetInnerClassIndex()) + " in&nbsp;class " + constant_html.ReferenceConstant
							(classe.GetOuterClassIndex()) + " named " + name + "</LI>\n");
					}
					file.Write("</UL>\n");
					break;
				}

				default:
				{
					// Such as Unknown attribute or Deprecated
					file.Write("<P>" + attribute);
					break;
				}
			}
			file.WriteLine("</TD></TR>");
			file.Flush();
		}
	}
}
