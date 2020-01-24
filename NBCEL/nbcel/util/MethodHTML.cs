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
	/// <summary>Convert methods and fields into HTML file.</summary>
	internal sealed class MethodHTML
	{
		private readonly string class_name;

		private readonly System.IO.TextWriter file;

		private readonly NBCEL.util.ConstantHTML constant_html;

		private readonly NBCEL.util.AttributeHTML attribute_html;

		/// <exception cref="System.IO.IOException"/>
		internal MethodHTML(string dir, string class_name, NBCEL.classfile.Method[] methods
			, NBCEL.classfile.Field[] fields, NBCEL.util.ConstantHTML constant_html, NBCEL.util.AttributeHTML
			 attribute_html)
		{
			// name of current class
			// file to write to
			this.class_name = class_name;
			this.attribute_html = attribute_html;
			this.constant_html = constant_html;
			file = new System.IO.StreamWriter(System.IO.File.OpenWrite(dir + class_name + "_methods.html"
				));
			file.WriteLine("<HTML><BODY BGCOLOR=\"#C0C0C0\"><TABLE BORDER=0>");
			file.WriteLine("<TR><TH ALIGN=LEFT>Access&nbsp;flags</TH><TH ALIGN=LEFT>Type</TH>" 
				+ "<TH ALIGN=LEFT>Field&nbsp;name</TH></TR>");
			foreach (NBCEL.classfile.Field field in fields)
			{
				WriteField(field);
			}
			file.WriteLine("</TABLE>");
			file.WriteLine("<TABLE BORDER=0><TR><TH ALIGN=LEFT>Access&nbsp;flags</TH>" + "<TH ALIGN=LEFT>Return&nbsp;type</TH><TH ALIGN=LEFT>Method&nbsp;name</TH>"
				 + "<TH ALIGN=LEFT>Arguments</TH></TR>");
			for (int i = 0; i < methods.Length; i++)
			{
				WriteMethod(methods[i], i);
			}
			file.WriteLine("</TABLE></BODY></HTML>");
			file.Close();
		}

		/// <summary>Print field of class.</summary>
		/// <param name="field">field to print</param>
		/// <exception cref="System.IO.IOException"/>
		private void WriteField(NBCEL.classfile.Field field)
		{
			string type = NBCEL.classfile.Utility.SignatureToString(field.GetSignature());
			string name = field.GetName();
			string access = NBCEL.classfile.Utility.AccessToString(field.GetAccessFlags());
			NBCEL.classfile.Attribute[] attributes;
			access = NBCEL.classfile.Utility.Replace(access, " ", "&nbsp;");
			file.Write("<TR><TD><FONT COLOR=\"#FF0000\">" + access + "</FONT></TD>\n<TD>" + NBCEL.util.Class2HTML
				.ReferenceType(type) + "</TD><TD><A NAME=\"field" + name + "\">" + name + "</A></TD>"
				);
			attributes = field.GetAttributes();
			// Write them to the Attributes.html file with anchor "<name>[<i>]"
			for (int i = 0; i < attributes.Length; i++)
			{
				attribute_html.WriteAttribute(attributes[i], name + "@" + i);
			}
			for (int i = 0; i < attributes.Length; i++)
			{
				if (attributes[i].GetTag() == NBCEL.Const.ATTR_CONSTANT_VALUE)
				{
					// Default value
					string str = ((NBCEL.classfile.ConstantValue)attributes[i]).ToString();
					// Reference attribute in _attributes.html
					file.Write("<TD>= <A HREF=\"" + class_name + "_attributes.html#" + name + "@" + i
						 + "\" TARGET=\"Attributes\">" + str + "</TD>\n");
					break;
				}
			}
			file.WriteLine("</TR>");
		}

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
			string html_name;
			// Get method's access flags
			string access = NBCEL.classfile.Utility.AccessToString(method.GetAccessFlags());
			// Get the method's attributes, the Code Attribute in particular
			NBCEL.classfile.Attribute[] attributes = method.GetAttributes();
			/* HTML doesn't like names like <clinit> and spaces are places to break
			* lines. Both we don't want...
			*/
			access = NBCEL.classfile.Utility.Replace(access, " ", "&nbsp;");
			html_name = NBCEL.util.Class2HTML.ToHTML(name);
			file.Write("<TR VALIGN=TOP><TD><FONT COLOR=\"#FF0000\"><A NAME=method" + method_number
				 + ">" + access + "</A></FONT></TD>");
			file.Write("<TD>" + NBCEL.util.Class2HTML.ReferenceType(type) + "</TD><TD>" + "<A HREF="
				 + class_name + "_code.html#method" + method_number + " TARGET=Code>" + html_name
				 + "</A></TD>\n<TD>(");
			for (int i = 0; i < args.Length; i++)
			{
				file.Write(NBCEL.util.Class2HTML.ReferenceType(args[i]));
				if (i < args.Length - 1)
				{
					file.Write(", ");
				}
			}
			file.Write(")</TD></TR>");
			// Check for thrown exceptions
			for (int i = 0; i < attributes.Length; i++)
			{
				attribute_html.WriteAttribute(attributes[i], "method" + method_number + "@" + i, 
					method_number);
				byte tag = attributes[i].GetTag();
				if (tag == NBCEL.Const.ATTR_EXCEPTIONS)
				{
					file.Write("<TR VALIGN=TOP><TD COLSPAN=2></TD><TH ALIGN=LEFT>throws</TH><TD>");
					int[] exceptions = ((NBCEL.classfile.ExceptionTable)attributes[i]).GetExceptionIndexTable
						();
					for (int j = 0; j < exceptions.Length; j++)
					{
						file.Write(constant_html.ReferenceConstant(exceptions[j]));
						if (j < exceptions.Length - 1)
						{
							file.Write(", ");
						}
					}
					file.WriteLine("</TD></TR>");
				}
				else if (tag == NBCEL.Const.ATTR_CODE)
				{
					NBCEL.classfile.Attribute[] c_a = ((NBCEL.classfile.Code)attributes[i]).GetAttributes
						();
					for (int j = 0; j < c_a.Length; j++)
					{
						attribute_html.WriteAttribute(c_a[j], "method" + method_number + "@" + i + "@" + 
							j, method_number);
					}
				}
			}
		}
	}
}
