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
using System.Collections.Generic;
using System.IO;
using System.Text;
using Apache.NBCEL.ClassFile;

namespace Apache.NBCEL.Util
{
	/// <summary>Read class file(s) and convert them into HTML files.</summary>
	/// <remarks>
	///     Read class file(s) and convert them into HTML files.
	///     Given a JavaClass object "class" that is in package "package" five files
	///     will be created in the specified directory.
	///     <OL>
	///         <LI>
	///             "package"."class".html as the main file which defines the frames for
	///             the following subfiles.
	///             <LI>
	///                 "package"."class"_attributes.html contains all (known) attributes found in the file
	///                 <LI>
	///                     "package"."class"_cp.html contains the constant pool
	///                     <LI>
	///                         "package"."class"_code.html contains the byte code
	///                         <LI>  "package"."class"_methods.html contains references to all methods and fields of the class
	///     </OL>
	///     All subfiles reference each other appropriately, e.g. clicking on a
	///     method in the Method's frame will jump to the appropriate method in
	///     the Code frame.
	/// </remarks>
	public class Class2HTML : Constants
    {
        private static string class_package;

        private static string class_name;

        private static ConstantPool constant_pool;

        private static readonly HashSet<string> basic_types =
            new HashSet<string>();

        private readonly string dir;
        private readonly JavaClass java_class;

        static Class2HTML()
        {
            // current class object
            // name of package, unclean to make it static, but ...
            // name of current class, dito
            basic_types.Add("int");
            basic_types.Add("short");
            basic_types.Add("boolean");
            basic_types.Add("void");
            basic_types.Add("char");
            basic_types.Add("byte");
            basic_types.Add("long");
            basic_types.Add("double");
            basic_types.Add("float");
        }

        /// <summary>Write contents of the given JavaClass into HTML files.</summary>
        /// <param name="java_class">The class to write</param>
        /// <param name="dir">The directory to put the files in</param>
        /// <exception cref="System.IO.IOException" />
        public Class2HTML(JavaClass java_class, string dir)
        {
            var methods = java_class.GetMethods();
            this.java_class = java_class;
            this.dir = dir;
            class_name = java_class.GetClassName();
            // Remember full name
            constant_pool = java_class.GetConstantPool();
            // Get package name by tacking off everything after the last `.'
            var index = class_name.LastIndexOf('.');
            if (index > -1)
                class_package = Runtime.Substring(class_name, 0, index);
            else
                class_package = string.Empty;
            // default package
            var constant_html = new ConstantHTML(dir, class_name
                , class_package, methods, constant_pool);
            /* Attributes can't be written in one step, so we just open a file
            * which will be written consequently.
            */
            var attribute_html = new AttributeHTML(dir, class_name
                , constant_pool, constant_html);
            new MethodHTML(dir, class_name, methods, java_class.GetFields(), constant_html
                , attribute_html);
            // Write main file (with frames, yuk)
            WriteMainHTML(attribute_html);
            new CodeHTML(dir, class_name, methods, constant_pool, constant_html);
            attribute_html.Close();
        }

        /// <exception cref="System.IO.IOException" />
        public static void Main(string[] argv)
        {
            var file_name = new string[argv.Length];
            var files = 0;
            ClassParser parser = null;
            JavaClass java_class = null;
            string zip_file = null;
            var sep = Path.DirectorySeparatorChar;
            var dir = "." + sep;
            // Where to store HTML files
            /* Parse command line arguments.
            */
            for (var i = 0; i < argv.Length; i++)
                if (argv[i][0] == '-')
                {
                    // command line switch
                    if (argv[i].Equals("-d"))
                    {
                        // Specify target directory, default '.'
                        dir = argv[++i];
                        if (!dir.EndsWith(string.Empty + sep)) dir = dir + sep;
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    }
                    else if (argv[i].Equals("-zip"))
                    {
                        zip_file = argv[++i];
                    }
                    else
                    {
                        Console.Out.WriteLine("Unknown option " + argv[i]);
                    }
                }
                else
                {
                    file_name[files++] = argv[i];
                }

            if (files == 0)
                Console.Error.WriteLine("Class2HTML: No input files specified.");
            else
                // Loop through files ...
                for (var i = 0; i < files; i++)
                {
                    Console.Out.Write("Processing " + file_name[i] + "...");
                    if (zip_file == null)
                        parser = new ClassParser(file_name[i]);
                    else
                        // Create parser object from file
                        parser = new ClassParser(zip_file, file_name[i]);
                    // Create parser object from zip file
                    java_class = parser.Parse();
                    new Class2HTML(java_class, dir);
                    Console.Out.WriteLine("Done.");
                }
        }

        /// <summary>
        ///     Utility method that converts a class reference in the constant pool,
        ///     i.e., an index to a string.
        /// </summary>
        internal static string ReferenceClass(int index)
        {
            var str = constant_pool.GetConstantString(index, Const.CONSTANT_Class);
            str = Utility.CompactClassName(str);
            str = Utility.CompactClassName(str, class_package + ".", true);
            return "<A HREF=\"" + class_name + "_cp.html#cp" + index + "\" TARGET=ConstantPool>"
                   + str + "</A>";
        }

        internal static string ReferenceType(string type)
        {
            var short_type = Utility.CompactClassName(type);
            short_type = Utility.CompactClassName(short_type, class_package +
                                                              ".", true);
            var index = type.IndexOf('[');
            // Type is an array?
            var base_type = type;
            if (index > -1) base_type = Runtime.Substring(type, 0, index);
            // Tack of the `['
            // test for basic type
            if (basic_types.Contains(base_type)) return "<FONT COLOR=\"#00FF00\">" + type + "</FONT>";
            return "<A HREF=\"" + base_type + ".html\" TARGET=_top>" + short_type + "</A>";
        }

        internal static string ToHTML(string str)
        {
            var buf = new StringBuilder();
            for (var i = 0; i < str.Length; i++)
            {
                char ch;
                switch (ch = str[i])
                {
                    case '<':
                    {
                        buf.Append("&lt;");
                        break;
                    }

                    case '>':
                    {
                        buf.Append("&gt;");
                        break;
                    }

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

                    default:
                    {
                        buf.Append(ch);
                        break;
                    }
                }
            }

            return buf.ToString();
        }

        /// <exception cref="System.IO.IOException" />
        private void WriteMainHTML(AttributeHTML attribute_html)
        {
            using (TextWriter file = new StreamWriter(File.OpenWrite(dir + class_name + ".html")))
            {
                file.WriteLine("<HTML>\n" + "<HEAD><TITLE>Documentation for " + class_name + "</TITLE>"
                               + "</HEAD>\n" + "<FRAMESET BORDER=1 cols=\"30%,*\">\n" +
                               "<FRAMESET BORDER=1 rows=\"80%,*\">\n"
                               + "<FRAME NAME=\"ConstantPool\" SRC=\"" + class_name + "_cp.html" +
                               "\"\n MARGINWIDTH=\"0\" "
                               + "MARGINHEIGHT=\"0\" FRAMEBORDER=\"1\" SCROLLING=\"AUTO\">\n" +
                               "<FRAME NAME=\"Attributes\" SRC=\""
                               + class_name + "_attributes.html" + "\"\n MARGINWIDTH=\"0\" " +
                               "MARGINHEIGHT=\"0\" FRAMEBORDER=\"1\" SCROLLING=\"AUTO\">\n"
                               + "</FRAMESET>\n" + "<FRAMESET BORDER=1 rows=\"80%,*\">\n" +
                               "<FRAME NAME=\"Code\" SRC=\""
                               + class_name + "_code.html\"\n MARGINWIDTH=0 " +
                               "MARGINHEIGHT=0 FRAMEBORDER=1 SCROLLING=\"AUTO\">\n"
                               + "<FRAME NAME=\"Methods\" SRC=\"" + class_name + "_methods.html\"\n MARGINWIDTH=0 "
                               + "MARGINHEIGHT=0 FRAMEBORDER=1 SCROLLING=\"AUTO\">\n" + "</FRAMESET></FRAMESET></HTML>"
                );
            }

            var attributes = java_class.GetAttributes();
            for (var i = 0; i < attributes.Length; i++) attribute_html.WriteAttribute(attributes[i], "class" + i);
        }
    }
}