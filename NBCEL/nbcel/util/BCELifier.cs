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
	/// <summary>
	/// This class takes a given JavaClass object and converts it to a
	/// Java program that creates that very class using BCEL.
	/// </summary>
	/// <remarks>
	/// This class takes a given JavaClass object and converts it to a
	/// Java program that creates that very class using BCEL. This
	/// gives new users of BCEL a useful example showing how things
	/// are done with BCEL. It does not cover all features of BCEL,
	/// but tries to mimic hand-written code as close as possible.
	/// </remarks>
	public class BCELifier : NBCEL.classfile.EmptyVisitor
	{
		/// <summary>Enum corresponding to flag source.</summary>
		[System.Serializable]
		public sealed class FLAGS : Sharpen.EnumBase
		{
			public static readonly NBCEL.util.BCELifier.FLAGS UNKNOWN = new NBCEL.util.BCELifier.FLAGS
				(0, "UNKNOWN");

			public static readonly NBCEL.util.BCELifier.FLAGS CLASS = new NBCEL.util.BCELifier.FLAGS
				(1, "CLASS");

			public static readonly NBCEL.util.BCELifier.FLAGS METHOD = new NBCEL.util.BCELifier.FLAGS
				(2, "METHOD");

			private FLAGS(int ordinal, string name)
				: base(ordinal, name)
			{
			}

			public static FLAGS[] Values()
			{
				return new FLAGS[] { UNKNOWN, CLASS, METHOD };
			}

			static FLAGS()
			{
				RegisterValues<FLAGS>(Values());
			}
		}

		private static readonly string BASE_PACKAGE = typeof(NBCEL.Const).Namespace;

		private static readonly string CONSTANT_PREFIX = typeof(NBCEL.Const).GetSimpleName
			() + ".";

		private readonly NBCEL.classfile.JavaClass _clazz;

		private readonly System.IO.TextWriter _out;

		private readonly NBCEL.generic.ConstantPoolGen _cp;

		/// <param name="clazz">Java class to "decompile"</param>
		/// <param name="out">where to output Java program</param>
		public BCELifier(NBCEL.classfile.JavaClass clazz, TextWriter @out)
		{
			// The base package name for imports; assumes Const is at the top level
			// N.B we use the class so renames will be detected by the compiler/IDE
			_clazz = clazz;
			_out = @out;
			_cp = new NBCEL.generic.ConstantPoolGen(_clazz.GetConstantPool());
		}

		/// <summary>Start Java code generation</summary>
		public virtual void Start()
		{
			VisitJavaClass(_clazz);
			_out.Flush();
		}

		public override void VisitJavaClass(NBCEL.classfile.JavaClass clazz)
		{
			string class_name = clazz.GetClassName();
			string super_name = clazz.GetSuperclassName();
			string package_name = clazz.GetPackageName();
			string inter = NBCEL.classfile.Utility.PrintArray(clazz.GetInterfaceNames(), false
				, true);
			if (!string.Empty.Equals(package_name))
			{
				class_name = Sharpen.Runtime.Substring(class_name, package_name.Length + 1);
				_out.WriteLine("package " + package_name + ";");
				_out.WriteLine();
			}
			_out.WriteLine("import " + BASE_PACKAGE + ".generic.*;");
			_out.WriteLine("import " + BASE_PACKAGE + ".classfile.*;");
			_out.WriteLine("import " + BASE_PACKAGE + ".*;");
			_out.WriteLine("import java.io.*;");
			_out.WriteLine();
			_out.WriteLine("public class " + class_name + "Creator {");
			_out.WriteLine("  private InstructionFactory _factory;");
			_out.WriteLine("  private ConstantPoolGen    _cp;");
			_out.WriteLine("  private ClassGen           _cg;");
			_out.WriteLine();
			_out.WriteLine("  public " + class_name + "Creator() {");
			_out.WriteLine("    _cg = new ClassGen(\"" + ((string.Empty.Equals(package_name)) ? 
				class_name : package_name + "." + class_name) + "\", \"" + super_name + "\", " +
				 "\"" + clazz.GetSourceFileName() + "\", " + PrintFlags(clazz.GetAccessFlags(), 
				NBCEL.util.BCELifier.FLAGS.CLASS) + ", " + "new String[] { " + inter + " });");
			_out.WriteLine("    _cg.setMajor(" + clazz.GetMajor() + ");");
			_out.WriteLine("    _cg.setMinor(" + clazz.GetMinor() + ");");
			_out.WriteLine();
			_out.WriteLine("    _cp = _cg.getConstantPool();");
			_out.WriteLine("    _factory = new InstructionFactory(_cg, _cp);");
			_out.WriteLine("  }");
			_out.WriteLine();
			PrintCreate();
			NBCEL.classfile.Field[] fields = clazz.GetFields();
			if (fields.Length > 0)
			{
				_out.WriteLine("  private void createFields() {");
				_out.WriteLine("    FieldGen field;");
				foreach (NBCEL.classfile.Field field in fields)
				{
					field.Accept(this);
				}
				_out.WriteLine("  }");
				_out.WriteLine();
			}
			NBCEL.classfile.Method[] methods = clazz.GetMethods();
			for (int i = 0; i < methods.Length; i++)
			{
				_out.WriteLine("  private void createMethod_" + i + "() {");
				methods[i].Accept(this);
				_out.WriteLine("  }");
				_out.WriteLine();
			}
			PrintMain();
			_out.WriteLine("}");
		}

		private void PrintCreate()
		{
			_out.WriteLine("  public void create(OutputStream out) throws IOException {");
			NBCEL.classfile.Field[] fields = _clazz.GetFields();
			if (fields.Length > 0)
			{
				_out.WriteLine("    createFields();");
			}
			NBCEL.classfile.Method[] methods = _clazz.GetMethods();
			for (int i = 0; i < methods.Length; i++)
			{
				_out.WriteLine("    createMethod_" + i + "();");
			}
			_out.WriteLine("    _cg.getJavaClass().dump(out);");
			_out.WriteLine("  }");
			_out.WriteLine();
		}

		private void PrintMain()
		{
			string class_name = _clazz.GetClassName();
			_out.WriteLine("  public static void main(String[] args) throws Exception {");
			_out.WriteLine("    " + class_name + "Creator creator = new " + class_name + "Creator();"
				);
			_out.WriteLine("    creator.create(new FileOutputStream(\"" + class_name + ".class\"));"
				);
			_out.WriteLine("  }");
		}

		public override void VisitField(NBCEL.classfile.Field field)
		{
			_out.WriteLine();
			_out.WriteLine("    field = new FieldGen(" + PrintFlags(field.GetAccessFlags()) + ", "
				 + PrintType(field.GetSignature()) + ", \"" + field.GetName() + "\", _cp);");
			NBCEL.classfile.ConstantValue cv = field.GetConstantValue();
			if (cv != null)
			{
				string value = cv.ToString();
				_out.WriteLine("    field.setInitValue(" + value + ")");
			}
			_out.WriteLine("    _cg.addField(field.getField());");
		}

		public override void VisitMethod(NBCEL.classfile.Method method)
		{
			NBCEL.generic.MethodGen mg = new NBCEL.generic.MethodGen(method, _clazz.GetClassName
				(), _cp);
			_out.WriteLine("    InstructionList il = new InstructionList();");
			_out.WriteLine("    MethodGen method = new MethodGen(" + PrintFlags(method.GetAccessFlags
				(), NBCEL.util.BCELifier.FLAGS.METHOD) + ", " + PrintType(mg.GetReturnType()) + 
				", " + PrintArgumentTypes(mg.GetArgumentTypes()) + ", " + "new String[] { " + NBCEL.classfile.Utility
				.PrintArray(mg.GetArgumentNames(), false, true) + " }, \"" + method.GetName() + 
				"\", \"" + _clazz.GetClassName() + "\", il, _cp);");
			_out.WriteLine();
			NBCEL.util.BCELFactory factory = new NBCEL.util.BCELFactory(mg, _out);
			factory.Start();
			_out.WriteLine("    method.setMaxStack();");
			_out.WriteLine("    method.setMaxLocals();");
			_out.WriteLine("    _cg.addMethod(method.getMethod());");
			_out.WriteLine("    il.dispose();");
		}

		internal static string PrintFlags(int flags)
		{
			return PrintFlags(flags, NBCEL.util.BCELifier.FLAGS.UNKNOWN);
		}

		/// <summary>Return a string with the flag settings</summary>
		/// <param name="flags">the flags field to interpret</param>
		/// <param name="location">the item type</param>
		/// <returns>the formatted string</returns>
		/// <since>6.0 made public</since>
		public static string PrintFlags(int flags, NBCEL.util.BCELifier.FLAGS location)
		{
			if (flags == 0)
			{
				return "0";
			}
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			for (int i = 0, pow = 1; pow <= NBCEL.Const.MAX_ACC_FLAG_I; i++)
			{
				if ((flags & pow) != 0)
				{
					if ((pow == NBCEL.Const.ACC_SYNCHRONIZED) && (location == NBCEL.util.BCELifier.FLAGS
						.CLASS))
					{
						buf.Append(CONSTANT_PREFIX + "ACC_SUPER | ");
					}
					else if ((pow == NBCEL.Const.ACC_VOLATILE) && (location == NBCEL.util.BCELifier.FLAGS
						.METHOD))
					{
						buf.Append(CONSTANT_PREFIX + "ACC_BRIDGE | ");
					}
					else if ((pow == NBCEL.Const.ACC_TRANSIENT) && (location == NBCEL.util.BCELifier.FLAGS
						.METHOD))
					{
						buf.Append(CONSTANT_PREFIX + "ACC_VARARGS | ");
					}
					else if (i < NBCEL.Const.ACCESS_NAMES_LENGTH)
					{
						buf.Append(CONSTANT_PREFIX + "ACC_").Append(NBCEL.Const.GetAccessName(i).ToUpper(
							)).Append(" | ");
					}
					else
					{
						buf.Append(string.Format(CONSTANT_PREFIX + "ACC_BIT %x | ", pow));
					}
				}
				pow <<= 1;
			}
			string str = buf.ToString();
			return Sharpen.Runtime.Substring(str, 0, str.Length - 3);
		}

		internal static string PrintArgumentTypes(NBCEL.generic.Type[] arg_types)
		{
			if (arg_types.Length == 0)
			{
				return "Type.NO_ARGS";
			}
			System.Text.StringBuilder args = new System.Text.StringBuilder();
			for (int i = 0; i < arg_types.Length; i++)
			{
				args.Append(PrintType(arg_types[i]));
				if (i < arg_types.Length - 1)
				{
					args.Append(", ");
				}
			}
			return "new Type[] { " + args.ToString() + " }";
		}

		internal static string PrintType(NBCEL.generic.Type type)
		{
			return PrintType(type.GetSignature());
		}

		internal static string PrintType(string signature)
		{
			NBCEL.generic.Type type = NBCEL.generic.Type.GetType(signature);
			byte t = type.GetType();
			if (((sbyte)t) <= NBCEL.Const.T_VOID)
			{
				return "Type." + NBCEL.Const.GetTypeName(t).ToUpper();
			}
			else if (type.ToString().Equals("java.lang.String"))
			{
				return "Type.STRING";
			}
			else if (type.ToString().Equals("java.lang.Object"))
			{
				return "Type.OBJECT";
			}
			else if (type.ToString().Equals("java.lang.StringBuffer"))
			{
				return "Type.STRINGBUFFER";
			}
			else if (type is NBCEL.generic.ArrayType)
			{
				NBCEL.generic.ArrayType at = (NBCEL.generic.ArrayType)type;
				return "new ArrayType(" + PrintType(at.GetBasicType()) + ", " + at.GetDimensions(
					) + ")";
			}
			else
			{
				return "new ObjectType(\"" + NBCEL.classfile.Utility.SignatureToString(signature, 
					false) + "\")";
			}
		}

		/// <summary>Default main method</summary>
		/// <exception cref="System.Exception"/>
		public static void Main(string[] argv)
		{
			if (argv.Length != 1)
			{
				System.Console.Out.WriteLine("Usage: BCELifier classname");
				System.Console.Out.WriteLine("\tThe class must exist on the classpath");
				return;
			}
			NBCEL.classfile.JavaClass java_class = GetJavaClass(argv[0]);
			NBCEL.util.BCELifier bcelifier = new NBCEL.util.BCELifier(java_class, System.Console.Out
				);
			bcelifier.Start();
		}

		// Needs to be accessible from unit test code
		/// <exception cref="System.TypeLoadException"/>
		/// <exception cref="System.IO.IOException"/>
		internal static NBCEL.classfile.JavaClass GetJavaClass(string name)
		{
			NBCEL.classfile.JavaClass java_class;
			if ((java_class = NBCEL.Repository.LookupClass(name)) == null)
			{
				java_class = new NBCEL.classfile.ClassParser(name).Parse();
			}
			// May throw IOException
			return java_class;
		}
	}
}
