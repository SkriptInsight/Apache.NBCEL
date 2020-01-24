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

namespace NBCEL.util
{
	/// <summary>Convert constant pool into HTML file.</summary>
	internal sealed class ConstantHTML
	{
		private readonly string class_name;

		private readonly string class_package;

		private readonly NBCEL.classfile.ConstantPool constant_pool;

		private readonly System.IO.TextWriter file;

		private readonly string[] constant_ref;

		private readonly NBCEL.classfile.Constant[] constants;

		private readonly NBCEL.classfile.Method[] methods;

		/// <exception cref="System.IO.IOException"/>
		internal ConstantHTML(string dir, string class_name, string class_package, NBCEL.classfile.Method
			[] methods, NBCEL.classfile.ConstantPool constant_pool)
		{
			// name of current class
			// name of package
			// reference to constant pool
			// file to write to
			// String to return for cp[i]
			// The constants in the cp
			this.class_name = class_name;
			this.class_package = class_package;
			this.constant_pool = constant_pool;
			this.methods = methods;
			constants = constant_pool.GetConstantPool();
			file = new System.IO.StreamWriter(System.IO.File.OpenWrite(dir + class_name + "_cp.html"
				));
			constant_ref = new string[constants.Length];
			constant_ref[0] = "&lt;unknown&gt;";
			file.WriteLine("<HTML><BODY BGCOLOR=\"#C0C0C0\"><TABLE BORDER=0>");
			// Loop through constants, constants[0] is reserved
			for (int i = 1; i < constants.Length; i++)
			{
				if (i % 2 == 0)
				{
					file.Write("<TR BGCOLOR=\"#C0C0C0\"><TD>");
				}
				else
				{
					file.Write("<TR BGCOLOR=\"#A0A0A0\"><TD>");
				}
				if (constants[i] != null)
				{
					WriteConstant(i);
				}
				file.Write("</TD></TR>\n");
			}
			file.WriteLine("</TABLE></BODY></HTML>");
			file.Close();
		}

		internal string ReferenceConstant(int index)
		{
			return constant_ref[index];
		}

		private void WriteConstant(int index)
		{
			byte tag = constants[index].GetTag();
			int class_index;
			int name_index;
			string @ref;
			// The header is always the same
			file.WriteLine("<H4> <A NAME=cp" + index + ">" + index + "</A> " + NBCEL.Const.GetConstantName
				(tag) + "</H4>");
			switch (tag)
			{
				case NBCEL.Const.CONSTANT_InterfaceMethodref:
				case NBCEL.Const.CONSTANT_Methodref:
				{
					/* For every constant type get the needed parameters and print them appropiately
					*/
					// Get class_index and name_and_type_index, depending on type
					if (tag == NBCEL.Const.CONSTANT_Methodref)
					{
						NBCEL.classfile.ConstantMethodref c = (NBCEL.classfile.ConstantMethodref)constant_pool
							.GetConstant(index, NBCEL.Const.CONSTANT_Methodref);
						class_index = c.GetClassIndex();
						name_index = c.GetNameAndTypeIndex();
					}
					else
					{
						NBCEL.classfile.ConstantInterfaceMethodref c1 = (NBCEL.classfile.ConstantInterfaceMethodref
							)constant_pool.GetConstant(index, NBCEL.Const.CONSTANT_InterfaceMethodref);
						class_index = c1.GetClassIndex();
						name_index = c1.GetNameAndTypeIndex();
					}
					// Get method name and its class
					string method_name = constant_pool.ConstantToString(name_index, NBCEL.Const.CONSTANT_NameAndType
						);
					string html_method_name = NBCEL.util.Class2HTML.ToHTML(method_name);
					// Partially compacted class name, i.e., / -> .
					string method_class = constant_pool.ConstantToString(class_index, NBCEL.Const.CONSTANT_Class
						);
					string short_method_class = NBCEL.classfile.Utility.CompactClassName(method_class
						);
					// I.e., remove java.lang.
					short_method_class = NBCEL.classfile.Utility.CompactClassName(short_method_class, 
						class_package + ".", true);
					// Remove class package prefix
					// Get method signature
					NBCEL.classfile.ConstantNameAndType c2 = (NBCEL.classfile.ConstantNameAndType)constant_pool
						.GetConstant(name_index, NBCEL.Const.CONSTANT_NameAndType);
					string signature = constant_pool.ConstantToString(c2.GetSignatureIndex(), NBCEL.Const
						.CONSTANT_Utf8);
					// Get array of strings containing the argument types
					string[] args = NBCEL.classfile.Utility.MethodSignatureArgumentTypes(signature, false
						);
					// Get return type string
					string type = NBCEL.classfile.Utility.MethodSignatureReturnType(signature, false);
					string ret_type = NBCEL.util.Class2HTML.ReferenceType(type);
					System.Text.StringBuilder buf = new System.Text.StringBuilder("(");
					for (int i = 0; i < args.Length; i++)
					{
						buf.Append(NBCEL.util.Class2HTML.ReferenceType(args[i]));
						if (i < args.Length - 1)
						{
							buf.Append(",&nbsp;");
						}
					}
					buf.Append(")");
					string arg_types = buf.ToString();
					if (method_class.Equals(class_name))
					{
						@ref = "<A HREF=\"" + class_name + "_code.html#method" + GetMethodNumber(method_name
							 + signature) + "\" TARGET=Code>" + html_method_name + "</A>";
					}
					else
					{
						@ref = "<A HREF=\"" + method_class + ".html" + "\" TARGET=_top>" + short_method_class
							 + "</A>." + html_method_name;
					}
					constant_ref[index] = ret_type + "&nbsp;<A HREF=\"" + class_name + "_cp.html#cp" 
						+ class_index + "\" TARGET=Constants>" + short_method_class + "</A>.<A HREF=\"" 
						+ class_name + "_cp.html#cp" + index + "\" TARGET=ConstantPool>" + html_method_name
						 + "</A>&nbsp;" + arg_types;
					file.WriteLine("<P><TT>" + ret_type + "&nbsp;" + @ref + arg_types + "&nbsp;</TT>\n<UL>"
						 + "<LI><A HREF=\"#cp" + class_index + "\">Class index(" + class_index + ")</A>\n"
						 + "<LI><A HREF=\"#cp" + name_index + "\">NameAndType index(" + name_index + ")</A></UL>"
						);
					break;
				}

				case NBCEL.Const.CONSTANT_Fieldref:
				{
					// Get class_index and name_and_type_index
					NBCEL.classfile.ConstantFieldref c3 = (NBCEL.classfile.ConstantFieldref)constant_pool
						.GetConstant(index, NBCEL.Const.CONSTANT_Fieldref);
					class_index = c3.GetClassIndex();
					name_index = c3.GetNameAndTypeIndex();
					// Get method name and its class (compacted)
					string field_class = constant_pool.ConstantToString(class_index, NBCEL.Const.CONSTANT_Class
						);
					string short_field_class = NBCEL.classfile.Utility.CompactClassName(field_class);
					// I.e., remove java.lang.
					short_field_class = NBCEL.classfile.Utility.CompactClassName(short_field_class, class_package
						 + ".", true);
					// Remove class package prefix
					string field_name = constant_pool.ConstantToString(name_index, NBCEL.Const.CONSTANT_NameAndType
						);
					if (field_class.Equals(class_name))
					{
						@ref = "<A HREF=\"" + field_class + "_methods.html#field" + field_name + "\" TARGET=Methods>"
							 + field_name + "</A>";
					}
					else
					{
						@ref = "<A HREF=\"" + field_class + ".html\" TARGET=_top>" + short_field_class + 
							"</A>." + field_name + "\n";
					}
					constant_ref[index] = "<A HREF=\"" + class_name + "_cp.html#cp" + class_index + "\" TARGET=Constants>"
						 + short_field_class + "</A>.<A HREF=\"" + class_name + "_cp.html#cp" + index + 
						"\" TARGET=ConstantPool>" + field_name + "</A>";
					file.WriteLine("<P><TT>" + @ref + "</TT><BR>\n" + "<UL>" + "<LI><A HREF=\"#cp" + class_index
						 + "\">Class(" + class_index + ")</A><BR>\n" + "<LI><A HREF=\"#cp" + name_index 
						+ "\">NameAndType(" + name_index + ")</A></UL>");
					break;
				}

				case NBCEL.Const.CONSTANT_Class:
				{
					NBCEL.classfile.ConstantClass c4 = (NBCEL.classfile.ConstantClass)constant_pool.GetConstant
						(index, NBCEL.Const.CONSTANT_Class);
					name_index = c4.GetNameIndex();
					string class_name2 = constant_pool.ConstantToString(index, tag);
					// / -> .
					string short_class_name = NBCEL.classfile.Utility.CompactClassName(class_name2);
					// I.e., remove java.lang.
					short_class_name = NBCEL.classfile.Utility.CompactClassName(short_class_name, class_package
						 + ".", true);
					// Remove class package prefix
					@ref = "<A HREF=\"" + class_name2 + ".html\" TARGET=_top>" + short_class_name + "</A>";
					constant_ref[index] = "<A HREF=\"" + class_name + "_cp.html#cp" + index + "\" TARGET=ConstantPool>"
						 + short_class_name + "</A>";
					file.WriteLine("<P><TT>" + @ref + "</TT><UL>" + "<LI><A HREF=\"#cp" + name_index + 
						"\">Name index(" + name_index + ")</A></UL>\n");
					break;
				}

				case NBCEL.Const.CONSTANT_String:
				{
					NBCEL.classfile.ConstantString c5 = (NBCEL.classfile.ConstantString)constant_pool
						.GetConstant(index, NBCEL.Const.CONSTANT_String);
					name_index = c5.GetStringIndex();
					string str = NBCEL.util.Class2HTML.ToHTML(constant_pool.ConstantToString(index, tag
						));
					file.WriteLine("<P><TT>" + str + "</TT><UL>" + "<LI><A HREF=\"#cp" + name_index + "\">Name index("
						 + name_index + ")</A></UL>\n");
					break;
				}

				case NBCEL.Const.CONSTANT_NameAndType:
				{
					NBCEL.classfile.ConstantNameAndType c6 = (NBCEL.classfile.ConstantNameAndType)constant_pool
						.GetConstant(index, NBCEL.Const.CONSTANT_NameAndType);
					name_index = c6.GetNameIndex();
					int signature_index = c6.GetSignatureIndex();
					file.WriteLine("<P><TT>" + NBCEL.util.Class2HTML.ToHTML(constant_pool.ConstantToString
						(index, tag)) + "</TT><UL>" + "<LI><A HREF=\"#cp" + name_index + "\">Name index("
						 + name_index + ")</A>\n" + "<LI><A HREF=\"#cp" + signature_index + "\">Signature index("
						 + signature_index + ")</A></UL>\n");
					break;
				}

				default:
				{
					file.WriteLine("<P><TT>" + NBCEL.util.Class2HTML.ToHTML(constant_pool.ConstantToString
						(index, tag)) + "</TT>\n");
					break;
				}
			}
		}

		// switch
		private int GetMethodNumber(string str)
		{
			for (int i = 0; i < methods.Length; i++)
			{
				string cmp = methods[i].GetName() + methods[i].GetSignature();
				if (cmp.Equals(str))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
