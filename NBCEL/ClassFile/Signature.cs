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
using System.Text;
using Apache.NBCEL.Java.IO;
using Apache.NBCEL.Java.Nio;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>
	///     This class is derived from <em>Attribute</em> and represents a reference
	///     to a GJ attribute.
	/// </summary>
	/// <seealso cref="Attribute" />
	public sealed class Signature : Attribute
    {
        private int signature_index;

        /// <summary>Initialize from another object.</summary>
        /// <remarks>
        ///     Initialize from another object. Note that both objects use the same
        ///     references (shallow copy). Use clone() for a physical copy.
        /// </remarks>
        public Signature(Signature c)
            : this(c.GetNameIndex(), c.GetLength(), c.GetSignatureIndex(), c.GetConstantPool(
            ))
        {
        }

        /// <summary>Construct object from file stream.</summary>
        /// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="input">Input stream</param>
        /// <param name="constant_pool">Array of constants</param>
        /// <exception cref="System.IO.IOException" />
        internal Signature(int name_index, int length, DataInput input, ConstantPool
            constant_pool)
            : this(name_index, length, input.ReadUnsignedShort(), constant_pool)
        {
        }

        /// <param name="name_index">Index in constant pool to CONSTANT_Utf8</param>
        /// <param name="length">Content length in bytes</param>
        /// <param name="signature_index">Index in constant pool to CONSTANT_Utf8</param>
        /// <param name="constant_pool">Array of constants</param>
        public Signature(int name_index, int length, int signature_index, ConstantPool
            constant_pool)
            : base(Const.ATTR_SIGNATURE, name_index, length, constant_pool)
        {
            this.signature_index = signature_index;
        }

        /// <summary>
        ///     Called by objects that are traversing the nodes of the tree implicitely
        ///     defined by the contents of a Java class.
        /// </summary>
        /// <remarks>
        ///     Called by objects that are traversing the nodes of the tree implicitely
        ///     defined by the contents of a Java class. I.e., the hierarchy of methods,
        ///     fields, attributes, etc. spawns a tree of objects.
        /// </remarks>
        /// <param name="v">Visitor object</param>
        public override void Accept(Visitor v)
        {
            //System.err.println("Visiting non-standard Signature object");
            v.VisitSignature(this);
        }

        /// <summary>Dump source file attribute to file stream in binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            base.Dump(file);
            file.WriteShort(signature_index);
        }

        /// <returns>Index in constant pool of source file name.</returns>
        public int GetSignatureIndex()
        {
            return signature_index;
        }

        /// <param name="signature_index">the index info the constant pool of this signature</param>
        public void SetSignatureIndex(int signature_index)
        {
            this.signature_index = signature_index;
        }

        /// <returns>GJ signature.</returns>
        public string GetSignature()
        {
            var c = (ConstantUtf8) GetConstantPool
                ().GetConstant(signature_index, Const.CONSTANT_Utf8);
            return c.GetBytes();
        }

        private static bool IdentStart(int ch)
        {
            return ch == 'T' || ch == 'L';
        }

        private static void MatchIdent(MyByteArrayInputStream @in
            , StringBuilder buf)
        {
            int ch;
            if ((ch = @in.Read()) == -1)
                throw new Exception("Illegal signature: " + @in.GetData() + " no ident, reaching EOF"
                );
            //System.out.println("return from ident:" + (char)ch);
            if (!IdentStart(ch))
            {
                var buf2 = new StringBuilder();
                var count = 1;
                while (Runtime.IsJavaIdentifierPart((char) ch))
                {
                    buf2.Append((char) ch);
                    count++;
                    ch = @in.Read();
                }

                if (ch == ':')
                {
                    // Ok, formal parameter
                    @in.Skip("Ljava/lang/Object".Length);
                    buf.Append(buf2);
                    ch = @in.Read();
                    @in.Unread();
                }
                else
                {
                    //System.out.println("so far:" + buf2 + ":next:" +(char)ch);
                    for (var i = 0; i < count; i++) @in.Unread();
                }

                return;
            }

            var buf2_1 = new StringBuilder();
            ch = @in.Read();
            do
            {
                buf2_1.Append((char) ch);
                ch = @in.Read();
            } while (ch != -1 && (Runtime.IsJavaIdentifierPart((char) ch) || ch == '/'));

            //System.out.println("within ident:"+ (char)ch);
            buf.Append(buf2_1.ToString().Replace('/', '.'));
            //System.out.println("regular return ident:"+ (char)ch + ":" + buf2);
            if (ch != -1) @in.Unread();
        }

        private static void MatchGJIdent(MyByteArrayInputStream
            @in, StringBuilder buf)
        {
            int ch;
            MatchIdent(@in, buf);
            ch = @in.Read();
            if (ch == '<' || ch == '(')
            {
                // Parameterized or method
                //System.out.println("Enter <");
                buf.Append((char) ch);
                MatchGJIdent(@in, buf);
                while ((ch = @in.Read()) != '>' && ch != ')')
                {
                    // List of parameters
                    if (ch == -1)
                        throw new Exception("Illegal signature: " + @in.GetData() + " reaching EOF"
                        );
                    //System.out.println("Still no >");
                    buf.Append(", ");
                    @in.Unread();
                    MatchGJIdent(@in, buf);
                }

                // Recursive call
                //System.out.println("Exit >");
                buf.Append((char) ch);
            }
            else
            {
                @in.Unread();
            }

            ch = @in.Read();
            if (IdentStart(ch))
            {
                @in.Unread();
                MatchGJIdent(@in, buf);
            }
            else if (ch == ')')
            {
                @in.Unread();
            }
            else if (ch != ';')
            {
                throw new Exception("Illegal signature: " + @in.GetData() + " read " + (char
                                    ) ch);
            }
        }

        public static string Translate(string s)
        {
            //System.out.println("Sig:" + s);
            var buf = new StringBuilder();
            MatchGJIdent(new MyByteArrayInputStream(s), buf);
            return buf.ToString();
        }

        // @since 6.0 is no longer final
        public static bool IsFormalParameterList(string s)
        {
            return s.StartsWith("<") && s.IndexOf(':') > 0;
        }

        // @since 6.0 is no longer final
        public static bool IsActualParameterList(string s)
        {
            return s.StartsWith("L") && s.EndsWith(">;");
        }

        /// <returns>String representation</returns>
        public override string ToString()
        {
            var s = GetSignature();
            return "Signature: " + s;
        }

        /// <returns>deep copy of this attribute</returns>
        public override Attribute Copy(ConstantPool _constant_pool
        )
        {
            return (Attribute) Clone();
        }

        /// <summary>Extends ByteArrayInputStream to make 'unreading' chars possible.</summary>
        private sealed class MyByteArrayInputStream : MemoryInputStream
        {
            internal MyByteArrayInputStream(string data)
                : base(Runtime.GetBytesForString(data))
            {
            }

            internal string GetData()
            {
                return Runtime.GetStringForBytes(Stream.ToArray());
            }

            internal void Unread()
            {
                if (Stream.Position > 0) Stream.Position--;
            }
        }
    }
}