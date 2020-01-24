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
using System.IO.Compression;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>Wrapper class that parses a given Java .class file.</summary>
	/// <remarks>
	/// Wrapper class that parses a given Java .class file. The method &lt;A
	/// href ="#parse"&gt;parse</A> returns a <A href ="JavaClass.html">
	/// JavaClass</A> object on success. When an I/O error or an
	/// inconsistency occurs an appropiate exception is propagated back to
	/// the caller.
	/// The structure and the names comply, except for a few conveniences,
	/// exactly with the <A href="http://docs.oracle.com/javase/specs/">
	/// JVM specification 1.0</a>. See this paper for
	/// further details about the structure of a bytecode file.
	/// </remarks>
	public sealed class ClassParser
	{
		private java.io.DataInputStream dataInputStream;

		private readonly bool fileOwned;

		private readonly string file_name;

		private string zip_file;

		private int class_name_index;

		private int superclass_name_index;

		private int major;

		private int minor;

		private int access_flags;

		private int[] interfaces;

		private NBCEL.classfile.ConstantPool constant_pool;

		private NBCEL.classfile.Field[] fields;

		private NBCEL.classfile.Method[] methods;

		private NBCEL.classfile.Attribute[] attributes;

		private readonly bool is_zip;

		private const int BUFSIZE = 8192;

		/// <summary>Parses class from the given stream.</summary>
		/// <param name="inputStream">Input stream</param>
		/// <param name="file_name">File name</param>
		public ClassParser(java.io.InputStream inputStream, string file_name)
		{
			// Compiler version
			// Compiler version
			// Access rights of parsed class
			// Names of implemented interfaces
			// collection of constants
			// class fields, i.e., its variables
			// methods defined in the class
			// attributes defined in the class
			// Loaded from zip file
			this.file_name = file_name;
			fileOwned = false;
			string clazz = inputStream.GetType().FullName;
			// Not a very clean solution ...
			is_zip = clazz.StartsWith("java.util.zip.") || clazz.StartsWith("java.util.jar.");
			if (inputStream is java.io.DataInputStream)
			{
				this.dataInputStream = (java.io.DataInputStream)inputStream;
			}/*
			else
			{
				this.dataInputStream = new java.io.DataInputStream(new java.io.BufferedInputStream
					(inputStream, BUFSIZE));
			}*/
		}

		/// <summary>Parses class from given .class file.</summary>
		/// <param name="file_name">file name</param>
		public ClassParser(string file_name)
		{
			is_zip = false;
			this.file_name = file_name;
			fileOwned = true;
		}

		/// <summary>Parses class from given .class file in a ZIP-archive</summary>
		/// <param name="zip_file">zip file name</param>
		/// <param name="file_name">file name</param>
		public ClassParser(string zip_file, string file_name)
		{
			is_zip = true;
			fileOwned = true;
			this.zip_file = zip_file;
			this.file_name = file_name;
		}

		/// <summary>
		/// Parses the given Java class file and return an object that represents
		/// the contained data, i.e., constants, methods, fields and commands.
		/// </summary>
		/// <remarks>
		/// Parses the given Java class file and return an object that represents
		/// the contained data, i.e., constants, methods, fields and commands.
		/// A <em>ClassFormatException</em> is raised, if the file is not a valid
		/// .class file. (This does not include verification of the byte code as it
		/// is performed by the java interpreter).
		/// </remarks>
		/// <returns>Class object representing the parsed class file</returns>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		public NBCEL.classfile.JavaClass Parse()
		{
			ZipArchive zip = null;
			try
			{
				if (fileOwned)
				{
					if (is_zip)
					{
						zip = ZipFile.OpenRead(zip_file);
						ZipArchiveEntry entry = zip.GetEntry(file_name);
						if (entry == null)
						{
							throw new System.IO.IOException("File " + file_name + " not found");
						}
						dataInputStream = new java.io.DataInputStream(entry.Open().ReadFully().ToInputStream());
					}
					else
					{
						dataInputStream = new java.io.DataInputStream(File.ReadAllBytes(file_name).ToInputStream());
					}
				}
				// Check magic tag of class file
				ReadID();
				// Get compiler version
				ReadVersion();
				// Read constant pool entries
				ReadConstantPool();
				// Get class information
				ReadClassInfo();
				// Get interface information, i.e., implemented interfaces
				ReadInterfaces();
				// Read class fields, i.e., the variables of the class
				ReadFields();
				// Read class methods, i.e., the functions in the class
				ReadMethods();
				// Read class attributes
				ReadAttributes();
			}
			finally
			{
				// Check for unknown variables
				//Unknown[] u = Unknown.getUnknownAttributes();
				//for (int i=0; i < u.length; i++)
				//  System.err.println("WARNING: " + u[i]);
				// Everything should have been read now
				//      if(file.available() > 0) {
				//        int bytes = file.available();
				//        byte[] buf = new byte[bytes];
				//        file.read(buf);
				//        if(!(is_zip && (buf.length == 1))) {
				//      System.err.println("WARNING: Trailing garbage at end of " + file_name);
				//      System.err.println(bytes + " extra bytes: " + Utility.toHexString(buf));
				//        }
				//      }
				// Read everything of interest, so close the file
				if (fileOwned)
				{
					try
					{
						if (dataInputStream != null)
						{
							dataInputStream.Close();
						}
					}
					catch (System.IO.IOException)
					{
					}
				}
				//ignore close exceptions
				try
				{
					if (zip != null)
					{
						zip.Dispose();
					}
				}
				catch (System.IO.IOException)
				{
				}
			}
			//ignore close exceptions
			// Return the information we have gathered in a new object
			return new NBCEL.classfile.JavaClass(class_name_index, superclass_name_index, file_name
				, major, minor, access_flags, constant_pool, interfaces, fields, methods, attributes
				, is_zip ? NBCEL.classfile.JavaClass.ZIP : NBCEL.classfile.JavaClass.FILE);
		}

		/// <summary>Reads information about the attributes of the class.</summary>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		private void ReadAttributes()
		{
			int attributes_count = dataInputStream.ReadUnsignedShort();
			attributes = new NBCEL.classfile.Attribute[attributes_count];
			for (int i = 0; i < attributes_count; i++)
			{
				attributes[i] = NBCEL.classfile.Attribute.ReadAttribute(dataInputStream, constant_pool
					);
			}
		}

		/// <summary>Reads information about the class and its super class.</summary>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		private void ReadClassInfo()
		{
			access_flags = dataInputStream.ReadUnsignedShort();
			/* Interfaces are implicitely abstract, the flag should be set
			* according to the JVM specification.
			*/
			if ((access_flags & NBCEL.Const.ACC_INTERFACE) != 0)
			{
				access_flags |= NBCEL.Const.ACC_ABSTRACT;
			}
			if (((access_flags & NBCEL.Const.ACC_ABSTRACT) != 0) && ((access_flags & NBCEL.Const
				.ACC_FINAL) != 0))
			{
				throw new NBCEL.classfile.ClassFormatException("Class " + file_name + " can't be both final and abstract"
					);
			}
			class_name_index = dataInputStream.ReadUnsignedShort();
			superclass_name_index = dataInputStream.ReadUnsignedShort();
		}

		/// <summary>Reads constant pool entries.</summary>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		private void ReadConstantPool()
		{
			constant_pool = new NBCEL.classfile.ConstantPool(dataInputStream);
		}

		/// <summary>Reads information about the fields of the class, i.e., its variables.</summary>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		private void ReadFields()
		{
			int fields_count = dataInputStream.ReadUnsignedShort();
			fields = new NBCEL.classfile.Field[fields_count];
			for (int i = 0; i < fields_count; i++)
			{
				fields[i] = new NBCEL.classfile.Field(dataInputStream, constant_pool);
			}
		}

		/// <summary>Checks whether the header of the file is ok.</summary>
		/// <remarks>
		/// Checks whether the header of the file is ok.
		/// Of course, this has to be the first action on successive file reads.
		/// </remarks>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		private void ReadID()
		{
			if (dataInputStream.ReadInt() != NBCEL.Const.JVM_CLASSFILE_MAGIC)
			{
				throw new NBCEL.classfile.ClassFormatException(file_name + " is not a Java .class file"
					);
			}
		}

		/// <summary>Reads information about the interfaces implemented by this class.</summary>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		private void ReadInterfaces()
		{
			int interfaces_count = dataInputStream.ReadUnsignedShort();
			interfaces = new int[interfaces_count];
			for (int i = 0; i < interfaces_count; i++)
			{
				interfaces[i] = dataInputStream.ReadUnsignedShort();
			}
		}

		/// <summary>Reads information about the methods of the class.</summary>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		private void ReadMethods()
		{
			int methods_count = dataInputStream.ReadUnsignedShort();
			methods = new NBCEL.classfile.Method[methods_count];
			for (int i = 0; i < methods_count; i++)
			{
				methods[i] = new NBCEL.classfile.Method(dataInputStream, constant_pool);
			}
		}

		/// <summary>Reads major and minor version of compiler which created the file.</summary>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		private void ReadVersion()
		{
			minor = dataInputStream.ReadUnsignedShort();
			major = dataInputStream.ReadUnsignedShort();
		}
	}
}