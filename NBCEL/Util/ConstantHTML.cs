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
using System.Text;
using Apache.NBCEL.ClassFile;

namespace Apache.NBCEL.Util
{
    /// <summary>Convert constant pool into HTML file.</summary>
    internal sealed class ConstantHTML
    {
        private readonly string class_name;

        private readonly string class_package;

        private readonly ConstantPool constant_pool;

        private readonly string[] constant_ref;

        private readonly Constant[] constants;

        private readonly TextWriter file;

        private readonly Method[] methods;

        /// <exception cref="System.IO.IOException" />
        internal ConstantHTML(string dir, string class_name, string class_package, Method
            [] methods, ConstantPool constant_pool)
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
            file = new StreamWriter(File.OpenWrite(dir + class_name + "_cp.html"
            ));
            constant_ref = new string[constants.Length];
            constant_ref[0] = "&lt;unknown&gt;";
            file.WriteLine("<HTML><BODY BGCOLOR=\"#C0C0C0\"><TABLE BORDER=0>");
            // Loop through constants, constants[0] is reserved
            for (var i = 1; i < constants.Length; i++)
            {
                if (i % 2 == 0)
                    file.Write("<TR BGCOLOR=\"#C0C0C0\"><TD>");
                else
                    file.Write("<TR BGCOLOR=\"#A0A0A0\"><TD>");
                if (constants[i] != null) WriteConstant(i);
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
            var tag = constants[index].GetTag();
            int class_index;
            int name_index;
            string @ref;
            // The header is always the same
            file.WriteLine("<H4> <A NAME=cp" + index + ">" + index + "</A> " + Const.GetConstantName
                               (tag) + "</H4>");
            switch (tag)
            {
                case Const.CONSTANT_InterfaceMethodref:
                case Const.CONSTANT_Methodref:
                {
                    /* For every constant type get the needed parameters and print them appropiately
                    */
                    // Get class_index and name_and_type_index, depending on type
                    if (tag == Const.CONSTANT_Methodref)
                    {
                        var c = (ConstantMethodref) constant_pool
                            .GetConstant(index, Const.CONSTANT_Methodref);
                        class_index = c.GetClassIndex();
                        name_index = c.GetNameAndTypeIndex();
                    }
                    else
                    {
                        var c1 = (ConstantInterfaceMethodref
                            ) constant_pool.GetConstant(index, Const.CONSTANT_InterfaceMethodref);
                        class_index = c1.GetClassIndex();
                        name_index = c1.GetNameAndTypeIndex();
                    }

                    // Get method name and its class
                    var method_name = constant_pool.ConstantToString(name_index, Const.CONSTANT_NameAndType
                    );
                    var html_method_name = Class2HTML.ToHTML(method_name);
                    // Partially compacted class name, i.e., / -> .
                    var method_class = constant_pool.ConstantToString(class_index, Const.CONSTANT_Class
                    );
                    var short_method_class = Utility.CompactClassName(method_class
                    );
                    // I.e., remove java.lang.
                    short_method_class = Utility.CompactClassName(short_method_class,
                        class_package + ".", true);
                    // Remove class package prefix
                    // Get method signature
                    var c2 = (ConstantNameAndType) constant_pool
                        .GetConstant(name_index, Const.CONSTANT_NameAndType);
                    var signature = constant_pool.ConstantToString(c2.GetSignatureIndex(), Const
                        .CONSTANT_Utf8);
                    // Get array of strings containing the argument types
                    var args = Utility.MethodSignatureArgumentTypes(signature, false
                    );
                    // Get return type string
                    var type = Utility.MethodSignatureReturnType(signature, false);
                    var ret_type = Class2HTML.ReferenceType(type);
                    var buf = new StringBuilder("(");
                    for (var i = 0; i < args.Length; i++)
                    {
                        buf.Append(Class2HTML.ReferenceType(args[i]));
                        if (i < args.Length - 1) buf.Append(",&nbsp;");
                    }

                    buf.Append(")");
                    var arg_types = buf.ToString();
                    if (method_class.Equals(class_name))
                        @ref = "<A HREF=\"" + class_name + "_code.html#method" + GetMethodNumber(method_name
                                                                                                 + signature) +
                               "\" TARGET=Code>" + html_method_name + "</A>";
                    else
                        @ref = "<A HREF=\"" + method_class + ".html" + "\" TARGET=_top>" + short_method_class
                               + "</A>." + html_method_name;
                    constant_ref[index] = ret_type + "&nbsp;<A HREF=\"" + class_name + "_cp.html#cp"
                                          + class_index + "\" TARGET=Constants>" + short_method_class +
                                          "</A>.<A HREF=\""
                                          + class_name + "_cp.html#cp" + index + "\" TARGET=ConstantPool>" +
                                          html_method_name
                                          + "</A>&nbsp;" + arg_types;
                    file.WriteLine("<P><TT>" + ret_type + "&nbsp;" + @ref + arg_types + "&nbsp;</TT>\n<UL>"
                                   + "<LI><A HREF=\"#cp" + class_index + "\">Class index(" + class_index + ")</A>\n"
                                   + "<LI><A HREF=\"#cp" + name_index + "\">NameAndType index(" + name_index +
                                   ")</A></UL>"
                    );
                    break;
                }

                case Const.CONSTANT_Fieldref:
                {
                    // Get class_index and name_and_type_index
                    var c3 = (ConstantFieldref) constant_pool
                        .GetConstant(index, Const.CONSTANT_Fieldref);
                    class_index = c3.GetClassIndex();
                    name_index = c3.GetNameAndTypeIndex();
                    // Get method name and its class (compacted)
                    var field_class = constant_pool.ConstantToString(class_index, Const.CONSTANT_Class
                    );
                    var short_field_class = Utility.CompactClassName(field_class);
                    // I.e., remove java.lang.
                    short_field_class = Utility.CompactClassName(short_field_class, class_package
                                                                                    + ".", true);
                    // Remove class package prefix
                    var field_name = constant_pool.ConstantToString(name_index, Const.CONSTANT_NameAndType
                    );
                    if (field_class.Equals(class_name))
                        @ref = "<A HREF=\"" + field_class + "_methods.html#field" + field_name + "\" TARGET=Methods>"
                               + field_name + "</A>";
                    else
                        @ref = "<A HREF=\"" + field_class + ".html\" TARGET=_top>" + short_field_class +
                               "</A>." + field_name + "\n";
                    constant_ref[index] = "<A HREF=\"" + class_name + "_cp.html#cp" + class_index +
                                          "\" TARGET=Constants>"
                                          + short_field_class + "</A>.<A HREF=\"" + class_name + "_cp.html#cp" + index +
                                          "\" TARGET=ConstantPool>" + field_name + "</A>";
                    file.WriteLine("<P><TT>" + @ref + "</TT><BR>\n" + "<UL>" + "<LI><A HREF=\"#cp" + class_index
                                   + "\">Class(" + class_index + ")</A><BR>\n" + "<LI><A HREF=\"#cp" + name_index
                                   + "\">NameAndType(" + name_index + ")</A></UL>");
                    break;
                }

                case Const.CONSTANT_Class:
                {
                    var c4 = (ConstantClass) constant_pool.GetConstant
                        (index, Const.CONSTANT_Class);
                    name_index = c4.GetNameIndex();
                    var class_name2 = constant_pool.ConstantToString(index, tag);
                    // / -> .
                    var short_class_name = Utility.CompactClassName(class_name2);
                    // I.e., remove java.lang.
                    short_class_name = Utility.CompactClassName(short_class_name, class_package
                                                                                  + ".", true);
                    // Remove class package prefix
                    @ref = "<A HREF=\"" + class_name2 + ".html\" TARGET=_top>" + short_class_name + "</A>";
                    constant_ref[index] = "<A HREF=\"" + class_name + "_cp.html#cp" + index + "\" TARGET=ConstantPool>"
                                          + short_class_name + "</A>";
                    file.WriteLine("<P><TT>" + @ref + "</TT><UL>" + "<LI><A HREF=\"#cp" + name_index +
                                   "\">Name index(" + name_index + ")</A></UL>\n");
                    break;
                }

                case Const.CONSTANT_String:
                {
                    var c5 = (ConstantString) constant_pool
                        .GetConstant(index, Const.CONSTANT_String);
                    name_index = c5.GetStringIndex();
                    var str = Class2HTML.ToHTML(constant_pool.ConstantToString(index, tag
                    ));
                    file.WriteLine("<P><TT>" + str + "</TT><UL>" + "<LI><A HREF=\"#cp" + name_index + "\">Name index("
                                   + name_index + ")</A></UL>\n");
                    break;
                }

                case Const.CONSTANT_NameAndType:
                {
                    var c6 = (ConstantNameAndType) constant_pool
                        .GetConstant(index, Const.CONSTANT_NameAndType);
                    name_index = c6.GetNameIndex();
                    var signature_index = c6.GetSignatureIndex();
                    file.WriteLine("<P><TT>" + Class2HTML.ToHTML(constant_pool.ConstantToString
                                       (index, tag)) + "</TT><UL>" + "<LI><A HREF=\"#cp" + name_index + "\">Name index("
                                   + name_index + ")</A>\n" + "<LI><A HREF=\"#cp" + signature_index +
                                   "\">Signature index("
                                   + signature_index + ")</A></UL>\n");
                    break;
                }

                default:
                {
                    file.WriteLine("<P><TT>" + Class2HTML.ToHTML(constant_pool.ConstantToString
                                       (index, tag)) + "</TT>\n");
                    break;
                }
            }
        }

        // switch
        private int GetMethodNumber(string str)
        {
            for (var i = 0; i < methods.Length; i++)
            {
                var cmp = methods[i].GetName() + methods[i].GetSignature();
                if (cmp.Equals(str)) return i;
            }

            return -1;
        }
    }
}