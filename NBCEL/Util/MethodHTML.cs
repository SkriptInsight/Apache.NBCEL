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
using Apache.NBCEL.ClassFile;

namespace Apache.NBCEL.Util
{
    /// <summary>Convert methods and fields into HTML file.</summary>
    internal sealed class MethodHTML
    {
        private readonly AttributeHTML attribute_html;
        private readonly string class_name;

        private readonly ConstantHTML constant_html;

        private readonly TextWriter file;

        /// <exception cref="System.IO.IOException" />
        internal MethodHTML(string dir, string class_name, Method[] methods
            , Field[] fields, ConstantHTML constant_html, AttributeHTML
                attribute_html)
        {
            // name of current class
            // file to write to
            this.class_name = class_name;
            this.attribute_html = attribute_html;
            this.constant_html = constant_html;
            file = new StreamWriter(File.OpenWrite(dir + class_name + "_methods.html"
            ));
            file.WriteLine("<HTML><BODY BGCOLOR=\"#C0C0C0\"><TABLE BORDER=0>");
            file.WriteLine("<TR><TH ALIGN=LEFT>Access&nbsp;flags</TH><TH ALIGN=LEFT>Type</TH>"
                           + "<TH ALIGN=LEFT>Field&nbsp;name</TH></TR>");
            foreach (var field in fields) WriteField(field);
            file.WriteLine("</TABLE>");
            file.WriteLine(
                "<TABLE BORDER=0><TR><TH ALIGN=LEFT>Access&nbsp;flags</TH>" + "<TH ALIGN=LEFT>Return&nbsp;type</TH><TH ALIGN=LEFT>Method&nbsp;name</TH>"
                                                                            + "<TH ALIGN=LEFT>Arguments</TH></TR>");
            for (var i = 0; i < methods.Length; i++) WriteMethod(methods[i], i);
            file.WriteLine("</TABLE></BODY></HTML>");
            file.Close();
        }

        /// <summary>Print field of class.</summary>
        /// <param name="field">field to print</param>
        /// <exception cref="System.IO.IOException" />
        private void WriteField(Field field)
        {
            var type = Utility.SignatureToString(field.GetSignature());
            var name = field.GetName();
            var access = Utility.AccessToString(field.GetAccessFlags());
            Attribute[] attributes;
            access = Utility.Replace(access, " ", "&nbsp;");
            file.Write("<TR><TD><FONT COLOR=\"#FF0000\">" + access + "</FONT></TD>\n<TD>" + Class2HTML
                           .ReferenceType(type) + "</TD><TD><A NAME=\"field" + name + "\">" + name + "</A></TD>"
            );
            attributes = field.GetAttributes();
            // Write them to the Attributes.html file with anchor "<name>[<i>]"
            for (var i = 0; i < attributes.Length; i++) attribute_html.WriteAttribute(attributes[i], name + "@" + i);
            for (var i = 0; i < attributes.Length; i++)
                if (attributes[i].GetTag() == Const.ATTR_CONSTANT_VALUE)
                {
                    // Default value
                    var str = ((ConstantValue) attributes[i]).ToString();
                    // Reference attribute in _attributes.html
                    file.Write("<TD>= <A HREF=\"" + class_name + "_attributes.html#" + name + "@" + i
                               + "\" TARGET=\"Attributes\">" + str + "</TD>\n");
                    break;
                }

            file.WriteLine("</TR>");
        }

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
            string html_name;
            // Get method's access flags
            var access = Utility.AccessToString(method.GetAccessFlags());
            // Get the method's attributes, the Code Attribute in particular
            var attributes = method.GetAttributes();
            /* HTML doesn't like names like <clinit> and spaces are places to break
            * lines. Both we don't want...
            */
            access = Utility.Replace(access, " ", "&nbsp;");
            html_name = Class2HTML.ToHTML(name);
            file.Write("<TR VALIGN=TOP><TD><FONT COLOR=\"#FF0000\"><A NAME=method" + method_number
                                                                                   + ">" + access + "</A></FONT></TD>");
            file.Write("<TD>" + Class2HTML.ReferenceType(type) + "</TD><TD>" + "<A HREF="
                       + class_name + "_code.html#method" + method_number + " TARGET=Code>" + html_name
                       + "</A></TD>\n<TD>(");
            for (var i = 0; i < args.Length; i++)
            {
                file.Write(Class2HTML.ReferenceType(args[i]));
                if (i < args.Length - 1) file.Write(", ");
            }

            file.Write(")</TD></TR>");
            // Check for thrown exceptions
            for (var i = 0; i < attributes.Length; i++)
            {
                attribute_html.WriteAttribute(attributes[i], "method" + method_number + "@" + i,
                    method_number);
                var tag = attributes[i].GetTag();
                if (tag == Const.ATTR_EXCEPTIONS)
                {
                    file.Write("<TR VALIGN=TOP><TD COLSPAN=2></TD><TH ALIGN=LEFT>throws</TH><TD>");
                    var exceptions = ((ExceptionTable) attributes[i]).GetExceptionIndexTable
                        ();
                    for (var j = 0; j < exceptions.Length; j++)
                    {
                        file.Write(constant_html.ReferenceConstant(exceptions[j]));
                        if (j < exceptions.Length - 1) file.Write(", ");
                    }

                    file.WriteLine("</TD></TR>");
                }
                else if (tag == Const.ATTR_CODE)
                {
                    var c_a = ((Code) attributes[i]).GetAttributes
                        ();
                    for (var j = 0; j < c_a.Length; j++)
                        attribute_html.WriteAttribute(c_a[j], "method" + method_number + "@" + i + "@" +
                                                              j, method_number);
                }
            }
        }
    }
}