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

namespace NBCEL.generic
{
	/// <summary>
	/// Instances of this class may be used, e.g., to generate typed
	/// versions of instructions.
	/// </summary>
	/// <remarks>
	/// Instances of this class may be used, e.g., to generate typed
	/// versions of instructions. Its main purpose is to be used as the
	/// byte code generating backend of a compiler. You can subclass it to
	/// add your own create methods.
	/// <p>
	/// Note: The static createXXX methods return singleton instances
	/// from the
	/// <see cref="InstructionConst"/>
	/// class.
	/// </remarks>
	/// <seealso cref="NBCEL.Const"/>
	/// <seealso cref="InstructionConst"/>
	public class InstructionFactory : NBCEL.generic.InstructionConstants
	{
		private static readonly string[] short_names = new string[] { "C", "F", "D", "B", 
			"S", "I", "L" };

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal NBCEL.generic.ClassGen cg;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal NBCEL.generic.ConstantPoolGen cp;

		public InstructionFactory(NBCEL.generic.ClassGen cg, NBCEL.generic.ConstantPoolGen
			 cp)
		{
			// N.N. These must agree with the order of Constants.T_CHAR through T_LONG
			this.cg = cg;
			this.cp = cp;
		}

		/// <summary>Initialize with ClassGen object</summary>
		public InstructionFactory(NBCEL.generic.ClassGen cg)
			: this(cg, cg.GetConstantPool())
		{
		}

		/// <summary>Initialize just with ConstantPoolGen object</summary>
		public InstructionFactory(NBCEL.generic.ConstantPoolGen cp)
			: this(null, cp)
		{
		}

		/// <summary>Create an invoke instruction.</summary>
		/// <remarks>Create an invoke instruction. (Except for invokedynamic.)</remarks>
		/// <param name="class_name">name of the called class</param>
		/// <param name="name">name of the called method</param>
		/// <param name="ret_type">return type of method</param>
		/// <param name="arg_types">argument types of method</param>
		/// <param name="kind">
		/// how to invoke, i.e., INVOKEINTERFACE, INVOKESTATIC, INVOKEVIRTUAL,
		/// or INVOKESPECIAL
		/// </param>
		/// <seealso cref="NBCEL.Const"/>
		public virtual NBCEL.generic.InvokeInstruction CreateInvoke(string class_name, string
			 name, NBCEL.generic.Type ret_type, NBCEL.generic.Type[] arg_types, short kind)
		{
			int index;
			int nargs = 0;
			string signature = NBCEL.generic.Type.GetMethodSignature(ret_type, arg_types);
			foreach (NBCEL.generic.Type arg_type in arg_types)
			{
				nargs += arg_type.GetSize();
			}
			if (kind == NBCEL.Const.INVOKEINTERFACE)
			{
				index = cp.AddInterfaceMethodref(class_name, name, signature);
			}
			else
			{
				index = cp.AddMethodref(class_name, name, signature);
			}
			switch (kind)
			{
				case NBCEL.Const.INVOKESPECIAL:
				{
					return new NBCEL.generic.INVOKESPECIAL(index);
				}

				case NBCEL.Const.INVOKEVIRTUAL:
				{
					return new NBCEL.generic.INVOKEVIRTUAL(index);
				}

				case NBCEL.Const.INVOKESTATIC:
				{
					return new NBCEL.generic.INVOKESTATIC(index);
				}

				case NBCEL.Const.INVOKEINTERFACE:
				{
					return new NBCEL.generic.INVOKEINTERFACE(index, nargs + 1);
				}

				case NBCEL.Const.INVOKEDYNAMIC:
				{
					return new NBCEL.generic.INVOKEDYNAMIC(index);
				}

				default:
				{
					throw new System.Exception("Oops: Unknown invoke kind: " + kind);
				}
			}
		}

		/*
		* createInvokeDynamic only needed if instrumention code wants to generate
		* a new invokedynamic instruction.  I don't think we need.  (markro)
		*
		public InvokeInstruction createInvokeDynamic( int bootstrap_index, String name, Type ret_type,
		Type[] arg_types) {
		int index;
		int nargs = 0;
		String signature = Type.getMethodSignature(ret_type, arg_types);
		for (int i = 0; i < arg_types.length; i++) {
		nargs += arg_types[i].getSize();
		}
		// UNDONE - needs to be added to ConstantPoolGen
		//index = cp.addInvokeDynamic(bootstrap_index, name, signature);
		index = 0;
		return new INVOKEDYNAMIC(index);
		}
		*/
		/// <summary>Create a call to the most popular System.out.println() method.</summary>
		/// <param name="s">the string to print</param>
		public virtual NBCEL.generic.InstructionList CreatePrintln(string s)
		{
			NBCEL.generic.InstructionList il = new NBCEL.generic.InstructionList();
			int @out = cp.AddFieldref("java.lang.System", "out", "Ljava/io/PrintStream;");
			int println = cp.AddMethodref("java.io.PrintStream", "println", "(Ljava/lang/String;)V"
				);
			il.Append(new NBCEL.generic.GETSTATIC(@out));
			il.Append(new NBCEL.generic.PUSH(cp, s));
			il.Append(new NBCEL.generic.INVOKEVIRTUAL(println));
			return il;
		}

		
		/// <summary>Uses PUSH to push a constant value onto the stack.</summary>
		/// <param name="value">must be of type Number, Boolean, Character or String</param>
		public virtual NBCEL.generic.Instruction CreateConstant(object value)
		{
			NBCEL.generic.PUSH push;
			if (value.IsNumber())
			{
				push = new NBCEL.generic.PUSH(cp, value, true);
			}
			else if (value is string)
			{
				push = new NBCEL.generic.PUSH(cp, (string)value);
			}
			else if (value is bool)
			{
				push = new NBCEL.generic.PUSH(cp, (bool)value);
			}
			else if (value is char)
			{
				push = new NBCEL.generic.PUSH(cp, (char)value);
			}
			else
			{
				throw new NBCEL.generic.ClassGenException("Illegal type: " + value.GetType());
			}
			return push.GetInstruction();
		}

		private class MethodObject
		{
			internal readonly NBCEL.generic.Type[] arg_types;

			internal readonly NBCEL.generic.Type result_type;

			internal readonly string class_name;

			internal readonly string name;

			internal MethodObject(string c, string n, NBCEL.generic.Type r, NBCEL.generic.Type
				[] a)
			{
				class_name = c;
				name = n;
				result_type = r;
				arg_types = a;
			}
		}

		private NBCEL.generic.InvokeInstruction CreateInvoke(NBCEL.generic.InstructionFactory.MethodObject
			 m, short kind)
		{
			return CreateInvoke(m.class_name, m.name, m.result_type, m.arg_types, kind);
		}

		private static readonly NBCEL.generic.InstructionFactory.MethodObject[] append_mos
			 = new NBCEL.generic.InstructionFactory.MethodObject[] { new NBCEL.generic.InstructionFactory.MethodObject
			("java.lang.StringBuffer", "append", NBCEL.generic.Type.STRINGBUFFER, new NBCEL.generic.Type
			[] { NBCEL.generic.Type.STRING }), new NBCEL.generic.InstructionFactory.MethodObject
			("java.lang.StringBuffer", "append", NBCEL.generic.Type.STRINGBUFFER, new NBCEL.generic.Type
			[] { NBCEL.generic.Type.OBJECT }), null, null, new NBCEL.generic.InstructionFactory.MethodObject
			("java.lang.StringBuffer", "append", NBCEL.generic.Type.STRINGBUFFER, new NBCEL.generic.Type
			[] { NBCEL.generic.Type.BOOLEAN }), new NBCEL.generic.InstructionFactory.MethodObject
			("java.lang.StringBuffer", "append", NBCEL.generic.Type.STRINGBUFFER, new NBCEL.generic.Type
			[] { NBCEL.generic.Type.CHAR }), new NBCEL.generic.InstructionFactory.MethodObject
			("java.lang.StringBuffer", "append", NBCEL.generic.Type.STRINGBUFFER, new NBCEL.generic.Type
			[] { NBCEL.generic.Type.FLOAT }), new NBCEL.generic.InstructionFactory.MethodObject
			("java.lang.StringBuffer", "append", NBCEL.generic.Type.STRINGBUFFER, new NBCEL.generic.Type
			[] { NBCEL.generic.Type.DOUBLE }), new NBCEL.generic.InstructionFactory.MethodObject
			("java.lang.StringBuffer", "append", NBCEL.generic.Type.STRINGBUFFER, new NBCEL.generic.Type
			[] { NBCEL.generic.Type.INT }), new NBCEL.generic.InstructionFactory.MethodObject
			("java.lang.StringBuffer", "append", NBCEL.generic.Type.STRINGBUFFER, new NBCEL.generic.Type
			[] { NBCEL.generic.Type.INT }), new NBCEL.generic.InstructionFactory.MethodObject
			("java.lang.StringBuffer", "append", NBCEL.generic.Type.STRINGBUFFER, new NBCEL.generic.Type
			[] { NBCEL.generic.Type.INT }), new NBCEL.generic.InstructionFactory.MethodObject
			("java.lang.StringBuffer", "append", NBCEL.generic.Type.STRINGBUFFER, new NBCEL.generic.Type
			[] { NBCEL.generic.Type.LONG }) };

		// indices 2, 3
		// No append(byte)
		// No append(short)
		private static bool IsString(NBCEL.generic.Type type)
		{
			return (type is NBCEL.generic.ObjectType) && ((NBCEL.generic.ObjectType)type).GetClassName
				().Equals("java.lang.String");
		}

		public virtual NBCEL.generic.Instruction CreateAppend(NBCEL.generic.Type type)
		{
			byte t = type.GetType();
			if (IsString(type))
			{
				return CreateInvoke(append_mos[0], NBCEL.Const.INVOKEVIRTUAL);
			}
			switch (t)
			{
				case NBCEL.Const.T_BOOLEAN:
				case NBCEL.Const.T_CHAR:
				case NBCEL.Const.T_FLOAT:
				case NBCEL.Const.T_DOUBLE:
				case NBCEL.Const.T_BYTE:
				case NBCEL.Const.T_SHORT:
				case NBCEL.Const.T_INT:
				case NBCEL.Const.T_LONG:
				{
					return CreateInvoke(append_mos[t], NBCEL.Const.INVOKEVIRTUAL);
				}

				case NBCEL.Const.T_ARRAY:
				case NBCEL.Const.T_OBJECT:
				{
					return CreateInvoke(append_mos[1], NBCEL.Const.INVOKEVIRTUAL);
				}

				default:
				{
					throw new System.Exception("Oops: No append for this type? " + type);
				}
			}
		}

		/// <summary>Create a field instruction.</summary>
		/// <param name="class_name">name of the accessed class</param>
		/// <param name="name">name of the referenced field</param>
		/// <param name="type">type of field</param>
		/// <param name="kind">how to access, i.e., GETFIELD, PUTFIELD, GETSTATIC, PUTSTATIC</param>
		/// <seealso cref="NBCEL.Const"/>
		public virtual NBCEL.generic.FieldInstruction CreateFieldAccess(string class_name
			, string name, NBCEL.generic.Type type, short kind)
		{
			int index;
			string signature = type.GetSignature();
			index = cp.AddFieldref(class_name, name, signature);
			switch (kind)
			{
				case NBCEL.Const.GETFIELD:
				{
					return new NBCEL.generic.GETFIELD(index);
				}

				case NBCEL.Const.PUTFIELD:
				{
					return new NBCEL.generic.PUTFIELD(index);
				}

				case NBCEL.Const.GETSTATIC:
				{
					return new NBCEL.generic.GETSTATIC(index);
				}

				case NBCEL.Const.PUTSTATIC:
				{
					return new NBCEL.generic.PUTSTATIC(index);
				}

				default:
				{
					throw new System.Exception("Oops: Unknown getfield kind:" + kind);
				}
			}
		}

		/// <summary>Create reference to `this'</summary>
		public static NBCEL.generic.Instruction CreateThis()
		{
			return new NBCEL.generic.ALOAD(0);
		}

		/// <summary>Create typed return</summary>
		public static NBCEL.generic.ReturnInstruction CreateReturn(NBCEL.generic.Type type
			)
		{
			switch (type.GetType())
			{
				case NBCEL.Const.T_ARRAY:
				case NBCEL.Const.T_OBJECT:
				{
					return NBCEL.generic.InstructionConst.ARETURN;
				}

				case NBCEL.Const.T_INT:
				case NBCEL.Const.T_SHORT:
				case NBCEL.Const.T_BOOLEAN:
				case NBCEL.Const.T_CHAR:
				case NBCEL.Const.T_BYTE:
				{
					return NBCEL.generic.InstructionConst.IRETURN;
				}

				case NBCEL.Const.T_FLOAT:
				{
					return NBCEL.generic.InstructionConst.FRETURN;
				}

				case NBCEL.Const.T_DOUBLE:
				{
					return NBCEL.generic.InstructionConst.DRETURN;
				}

				case NBCEL.Const.T_LONG:
				{
					return NBCEL.generic.InstructionConst.LRETURN;
				}

				case NBCEL.Const.T_VOID:
				{
					return NBCEL.generic.InstructionConst.RETURN;
				}

				default:
				{
					throw new System.Exception("Invalid type: " + type);
				}
			}
		}

		private static NBCEL.generic.ArithmeticInstruction CreateBinaryIntOp(char first, 
			string op)
		{
			switch (first)
			{
				case '-':
				{
					return NBCEL.generic.InstructionConst.ISUB;
				}

				case '+':
				{
					return NBCEL.generic.InstructionConst.IADD;
				}

				case '%':
				{
					return NBCEL.generic.InstructionConst.IREM;
				}

				case '*':
				{
					return NBCEL.generic.InstructionConst.IMUL;
				}

				case '/':
				{
					return NBCEL.generic.InstructionConst.IDIV;
				}

				case '&':
				{
					return NBCEL.generic.InstructionConst.IAND;
				}

				case '|':
				{
					return NBCEL.generic.InstructionConst.IOR;
				}

				case '^':
				{
					return NBCEL.generic.InstructionConst.IXOR;
				}

				case '<':
				{
					return NBCEL.generic.InstructionConst.ISHL;
				}

				case '>':
				{
					return op.Equals(">>>") ? NBCEL.generic.InstructionConst.IUSHR : NBCEL.generic.InstructionConst
						.ISHR;
				}

				default:
				{
					throw new System.Exception("Invalid operand " + op);
				}
			}
		}

		private static NBCEL.generic.ArithmeticInstruction CreateBinaryLongOp(char first, 
			string op)
		{
			switch (first)
			{
				case '-':
				{
					return NBCEL.generic.InstructionConst.LSUB;
				}

				case '+':
				{
					return NBCEL.generic.InstructionConst.LADD;
				}

				case '%':
				{
					return NBCEL.generic.InstructionConst.LREM;
				}

				case '*':
				{
					return NBCEL.generic.InstructionConst.LMUL;
				}

				case '/':
				{
					return NBCEL.generic.InstructionConst.LDIV;
				}

				case '&':
				{
					return NBCEL.generic.InstructionConst.LAND;
				}

				case '|':
				{
					return NBCEL.generic.InstructionConst.LOR;
				}

				case '^':
				{
					return NBCEL.generic.InstructionConst.LXOR;
				}

				case '<':
				{
					return NBCEL.generic.InstructionConst.LSHL;
				}

				case '>':
				{
					return op.Equals(">>>") ? NBCEL.generic.InstructionConst.LUSHR : NBCEL.generic.InstructionConst
						.LSHR;
				}

				default:
				{
					throw new System.Exception("Invalid operand " + op);
				}
			}
		}

		private static NBCEL.generic.ArithmeticInstruction CreateBinaryFloatOp(char op)
		{
			switch (op)
			{
				case '-':
				{
					return NBCEL.generic.InstructionConst.FSUB;
				}

				case '+':
				{
					return NBCEL.generic.InstructionConst.FADD;
				}

				case '*':
				{
					return NBCEL.generic.InstructionConst.FMUL;
				}

				case '/':
				{
					return NBCEL.generic.InstructionConst.FDIV;
				}

				case '%':
				{
					return NBCEL.generic.InstructionConst.FREM;
				}

				default:
				{
					throw new System.Exception("Invalid operand " + op);
				}
			}
		}

		private static NBCEL.generic.ArithmeticInstruction CreateBinaryDoubleOp(char op)
		{
			switch (op)
			{
				case '-':
				{
					return NBCEL.generic.InstructionConst.DSUB;
				}

				case '+':
				{
					return NBCEL.generic.InstructionConst.DADD;
				}

				case '*':
				{
					return NBCEL.generic.InstructionConst.DMUL;
				}

				case '/':
				{
					return NBCEL.generic.InstructionConst.DDIV;
				}

				case '%':
				{
					return NBCEL.generic.InstructionConst.DREM;
				}

				default:
				{
					throw new System.Exception("Invalid operand " + op);
				}
			}
		}

		/// <summary>Create binary operation for simple basic types, such as int and float.</summary>
		/// <param name="op">operation, such as "+", "*", "&lt;&lt;", etc.</param>
		public static NBCEL.generic.ArithmeticInstruction CreateBinaryOperation(string op
			, NBCEL.generic.Type type)
		{
			char first = op[0];
			switch (type.GetType())
			{
				case NBCEL.Const.T_BYTE:
				case NBCEL.Const.T_SHORT:
				case NBCEL.Const.T_INT:
				case NBCEL.Const.T_CHAR:
				{
					return CreateBinaryIntOp(first, op);
				}

				case NBCEL.Const.T_LONG:
				{
					return CreateBinaryLongOp(first, op);
				}

				case NBCEL.Const.T_FLOAT:
				{
					return CreateBinaryFloatOp(first);
				}

				case NBCEL.Const.T_DOUBLE:
				{
					return CreateBinaryDoubleOp(first);
				}

				default:
				{
					throw new System.Exception("Invalid type " + type);
				}
			}
		}

		/// <param name="size">size of operand, either 1 (int, e.g.) or 2 (double)</param>
		public static NBCEL.generic.StackInstruction CreatePop(int size)
		{
			return (size == 2) ? NBCEL.generic.InstructionConst.POP2 : NBCEL.generic.InstructionConst
				.POP;
		}

		/// <param name="size">size of operand, either 1 (int, e.g.) or 2 (double)</param>
		public static NBCEL.generic.StackInstruction CreateDup(int size)
		{
			return (size == 2) ? NBCEL.generic.InstructionConst.DUP2 : NBCEL.generic.InstructionConst
				.DUP;
		}

		/// <param name="size">size of operand, either 1 (int, e.g.) or 2 (double)</param>
		public static NBCEL.generic.StackInstruction CreateDup_2(int size)
		{
			return (size == 2) ? NBCEL.generic.InstructionConst.DUP2_X2 : NBCEL.generic.InstructionConst
				.DUP_X2;
		}

		/// <param name="size">size of operand, either 1 (int, e.g.) or 2 (double)</param>
		public static NBCEL.generic.StackInstruction CreateDup_1(int size)
		{
			return (size == 2) ? NBCEL.generic.InstructionConst.DUP2_X1 : NBCEL.generic.InstructionConst
				.DUP_X1;
		}

		/// <param name="index">index of local variable</param>
		public static NBCEL.generic.LocalVariableInstruction CreateStore(NBCEL.generic.Type
			 type, int index)
		{
			switch (type.GetType())
			{
				case NBCEL.Const.T_BOOLEAN:
				case NBCEL.Const.T_CHAR:
				case NBCEL.Const.T_BYTE:
				case NBCEL.Const.T_SHORT:
				case NBCEL.Const.T_INT:
				{
					return new NBCEL.generic.ISTORE(index);
				}

				case NBCEL.Const.T_FLOAT:
				{
					return new NBCEL.generic.FSTORE(index);
				}

				case NBCEL.Const.T_DOUBLE:
				{
					return new NBCEL.generic.DSTORE(index);
				}

				case NBCEL.Const.T_LONG:
				{
					return new NBCEL.generic.LSTORE(index);
				}

				case NBCEL.Const.T_ARRAY:
				case NBCEL.Const.T_OBJECT:
				{
					return new NBCEL.generic.ASTORE(index);
				}

				default:
				{
					throw new System.Exception("Invalid type " + type);
				}
			}
		}

		/// <param name="index">index of local variable</param>
		public static NBCEL.generic.LocalVariableInstruction CreateLoad(NBCEL.generic.Type
			 type, int index)
		{
			switch (type.GetType())
			{
				case NBCEL.Const.T_BOOLEAN:
				case NBCEL.Const.T_CHAR:
				case NBCEL.Const.T_BYTE:
				case NBCEL.Const.T_SHORT:
				case NBCEL.Const.T_INT:
				{
					return new NBCEL.generic.ILOAD(index);
				}

				case NBCEL.Const.T_FLOAT:
				{
					return new NBCEL.generic.FLOAD(index);
				}

				case NBCEL.Const.T_DOUBLE:
				{
					return new NBCEL.generic.DLOAD(index);
				}

				case NBCEL.Const.T_LONG:
				{
					return new NBCEL.generic.LLOAD(index);
				}

				case NBCEL.Const.T_ARRAY:
				case NBCEL.Const.T_OBJECT:
				{
					return new NBCEL.generic.ALOAD(index);
				}

				default:
				{
					throw new System.Exception("Invalid type " + type);
				}
			}
		}

		/// <param name="type">type of elements of array, i.e., array.getElementType()</param>
		public static NBCEL.generic.ArrayInstruction CreateArrayLoad(NBCEL.generic.Type type
			)
		{
			switch (type.GetType())
			{
				case NBCEL.Const.T_BOOLEAN:
				case NBCEL.Const.T_BYTE:
				{
					return NBCEL.generic.InstructionConst.BALOAD;
				}

				case NBCEL.Const.T_CHAR:
				{
					return NBCEL.generic.InstructionConst.CALOAD;
				}

				case NBCEL.Const.T_SHORT:
				{
					return NBCEL.generic.InstructionConst.SALOAD;
				}

				case NBCEL.Const.T_INT:
				{
					return NBCEL.generic.InstructionConst.IALOAD;
				}

				case NBCEL.Const.T_FLOAT:
				{
					return NBCEL.generic.InstructionConst.FALOAD;
				}

				case NBCEL.Const.T_DOUBLE:
				{
					return NBCEL.generic.InstructionConst.DALOAD;
				}

				case NBCEL.Const.T_LONG:
				{
					return NBCEL.generic.InstructionConst.LALOAD;
				}

				case NBCEL.Const.T_ARRAY:
				case NBCEL.Const.T_OBJECT:
				{
					return NBCEL.generic.InstructionConst.AALOAD;
				}

				default:
				{
					throw new System.Exception("Invalid type " + type);
				}
			}
		}

		/// <param name="type">type of elements of array, i.e., array.getElementType()</param>
		public static NBCEL.generic.ArrayInstruction CreateArrayStore(NBCEL.generic.Type 
			type)
		{
			switch (type.GetType())
			{
				case NBCEL.Const.T_BOOLEAN:
				case NBCEL.Const.T_BYTE:
				{
					return NBCEL.generic.InstructionConst.BASTORE;
				}

				case NBCEL.Const.T_CHAR:
				{
					return NBCEL.generic.InstructionConst.CASTORE;
				}

				case NBCEL.Const.T_SHORT:
				{
					return NBCEL.generic.InstructionConst.SASTORE;
				}

				case NBCEL.Const.T_INT:
				{
					return NBCEL.generic.InstructionConst.IASTORE;
				}

				case NBCEL.Const.T_FLOAT:
				{
					return NBCEL.generic.InstructionConst.FASTORE;
				}

				case NBCEL.Const.T_DOUBLE:
				{
					return NBCEL.generic.InstructionConst.DASTORE;
				}

				case NBCEL.Const.T_LONG:
				{
					return NBCEL.generic.InstructionConst.LASTORE;
				}

				case NBCEL.Const.T_ARRAY:
				case NBCEL.Const.T_OBJECT:
				{
					return NBCEL.generic.InstructionConst.AASTORE;
				}

				default:
				{
					throw new System.Exception("Invalid type " + type);
				}
			}
		}

		/// <summary>
		/// Create conversion operation for two stack operands, this may be an I2C, instruction, e.g.,
		/// if the operands are basic types and CHECKCAST if they are reference types.
		/// </summary>
		public virtual NBCEL.generic.Instruction CreateCast(NBCEL.generic.Type src_type, 
			NBCEL.generic.Type dest_type)
		{
			if ((src_type is NBCEL.generic.BasicType) && (dest_type is NBCEL.generic.BasicType
				))
			{
				byte dest = dest_type.GetType();
				byte src = src_type.GetType();
				if (dest == NBCEL.Const.T_LONG && (src == NBCEL.Const.T_CHAR || src == NBCEL.Const
					.T_BYTE || src == NBCEL.Const.T_SHORT))
				{
					src = NBCEL.Const.T_INT;
				}
				string name = "org.apache.bcel.generic." + short_names[src - NBCEL.Const.T_CHAR] 
					+ "2" + short_names[dest - NBCEL.Const.T_CHAR];
				NBCEL.generic.Instruction i = null;
				try
				{
					i = (NBCEL.generic.Instruction)System.Activator.CreateInstance(Sharpen.Runtime.GetJavaType
						(name));
				}
				catch (System.Exception e)
				{
					throw new System.Exception("Could not find instruction: " + name, e);
				}
				return i;
			}
			else if ((src_type is NBCEL.generic.ReferenceType) && (dest_type is NBCEL.generic.ReferenceType
				))
			{
				if (dest_type is NBCEL.generic.ArrayType)
				{
					return new NBCEL.generic.CHECKCAST(cp.AddArrayClass((NBCEL.generic.ArrayType)dest_type
						));
				}
				return new NBCEL.generic.CHECKCAST(cp.AddClass(((NBCEL.generic.ObjectType)dest_type
					).GetClassName()));
			}
			else
			{
				throw new System.Exception("Can not cast " + src_type + " to " + dest_type);
			}
		}

		public virtual NBCEL.generic.GETFIELD CreateGetField(string class_name, string name
			, NBCEL.generic.Type t)
		{
			return new NBCEL.generic.GETFIELD(cp.AddFieldref(class_name, name, t.GetSignature
				()));
		}

		public virtual NBCEL.generic.GETSTATIC CreateGetStatic(string class_name, string 
			name, NBCEL.generic.Type t)
		{
			return new NBCEL.generic.GETSTATIC(cp.AddFieldref(class_name, name, t.GetSignature
				()));
		}

		public virtual NBCEL.generic.PUTFIELD CreatePutField(string class_name, string name
			, NBCEL.generic.Type t)
		{
			return new NBCEL.generic.PUTFIELD(cp.AddFieldref(class_name, name, t.GetSignature
				()));
		}

		public virtual NBCEL.generic.PUTSTATIC CreatePutStatic(string class_name, string 
			name, NBCEL.generic.Type t)
		{
			return new NBCEL.generic.PUTSTATIC(cp.AddFieldref(class_name, name, t.GetSignature
				()));
		}

		public virtual NBCEL.generic.CHECKCAST CreateCheckCast(NBCEL.generic.ReferenceType
			 t)
		{
			if (t is NBCEL.generic.ArrayType)
			{
				return new NBCEL.generic.CHECKCAST(cp.AddArrayClass((NBCEL.generic.ArrayType)t));
			}
			return new NBCEL.generic.CHECKCAST(cp.AddClass((NBCEL.generic.ObjectType)t));
		}

		public virtual NBCEL.generic.INSTANCEOF CreateInstanceOf(NBCEL.generic.ReferenceType
			 t)
		{
			if (t is NBCEL.generic.ArrayType)
			{
				return new NBCEL.generic.INSTANCEOF(cp.AddArrayClass((NBCEL.generic.ArrayType)t));
			}
			return new NBCEL.generic.INSTANCEOF(cp.AddClass((NBCEL.generic.ObjectType)t));
		}

		public virtual NBCEL.generic.NEW CreateNew(NBCEL.generic.ObjectType t)
		{
			return new NBCEL.generic.NEW(cp.AddClass(t));
		}

		public virtual NBCEL.generic.NEW CreateNew(string s)
		{
			return CreateNew(NBCEL.generic.ObjectType.GetInstance(s));
		}

		/// <summary>Create new array of given size and type.</summary>
		/// <returns>an instruction that creates the corresponding array at runtime, i.e. is an AllocationInstruction
		/// 	</returns>
		public virtual NBCEL.generic.Instruction CreateNewArray(NBCEL.generic.Type t, short
			 dim)
		{
			if (dim == 1)
			{
				if (t is NBCEL.generic.ObjectType)
				{
					return new NBCEL.generic.ANEWARRAY(cp.AddClass((NBCEL.generic.ObjectType)t));
				}
				else if (t is NBCEL.generic.ArrayType)
				{
					return new NBCEL.generic.ANEWARRAY(cp.AddArrayClass((NBCEL.generic.ArrayType)t));
				}
				else
				{
					return new NBCEL.generic.NEWARRAY(t.GetType());
				}
			}
			NBCEL.generic.ArrayType at;
			if (t is NBCEL.generic.ArrayType)
			{
				at = (NBCEL.generic.ArrayType)t;
			}
			else
			{
				at = new NBCEL.generic.ArrayType(t, dim);
			}
			return new NBCEL.generic.MULTIANEWARRAY(cp.AddArrayClass(at), dim);
		}

		/// <summary>Create "null" value for reference types, 0 for basic types like int</summary>
		public static NBCEL.generic.Instruction CreateNull(NBCEL.generic.Type type)
		{
			switch (type.GetType())
			{
				case NBCEL.Const.T_ARRAY:
				case NBCEL.Const.T_OBJECT:
				{
					return NBCEL.generic.InstructionConst.ACONST_NULL;
				}

				case NBCEL.Const.T_INT:
				case NBCEL.Const.T_SHORT:
				case NBCEL.Const.T_BOOLEAN:
				case NBCEL.Const.T_CHAR:
				case NBCEL.Const.T_BYTE:
				{
					return NBCEL.generic.InstructionConst.ICONST_0;
				}

				case NBCEL.Const.T_FLOAT:
				{
					return NBCEL.generic.InstructionConst.FCONST_0;
				}

				case NBCEL.Const.T_DOUBLE:
				{
					return NBCEL.generic.InstructionConst.DCONST_0;
				}

				case NBCEL.Const.T_LONG:
				{
					return NBCEL.generic.InstructionConst.LCONST_0;
				}

				case NBCEL.Const.T_VOID:
				{
					return NBCEL.generic.InstructionConst.NOP;
				}

				default:
				{
					throw new System.Exception("Invalid type: " + type);
				}
			}
		}

		/// <summary>Create branch instruction by given opcode, except LOOKUPSWITCH and TABLESWITCH.
		/// 	</summary>
		/// <remarks>
		/// Create branch instruction by given opcode, except LOOKUPSWITCH and TABLESWITCH.
		/// For those you should use the SWITCH compound instruction.
		/// </remarks>
		public static NBCEL.generic.BranchInstruction CreateBranchInstruction(short opcode
			, NBCEL.generic.InstructionHandle target)
		{
			switch (opcode)
			{
				case NBCEL.Const.IFEQ:
				{
					return new NBCEL.generic.IFEQ(target);
				}

				case NBCEL.Const.IFNE:
				{
					return new NBCEL.generic.IFNE(target);
				}

				case NBCEL.Const.IFLT:
				{
					return new NBCEL.generic.IFLT(target);
				}

				case NBCEL.Const.IFGE:
				{
					return new NBCEL.generic.IFGE(target);
				}

				case NBCEL.Const.IFGT:
				{
					return new NBCEL.generic.IFGT(target);
				}

				case NBCEL.Const.IFLE:
				{
					return new NBCEL.generic.IFLE(target);
				}

				case NBCEL.Const.IF_ICMPEQ:
				{
					return new NBCEL.generic.IF_ICMPEQ(target);
				}

				case NBCEL.Const.IF_ICMPNE:
				{
					return new NBCEL.generic.IF_ICMPNE(target);
				}

				case NBCEL.Const.IF_ICMPLT:
				{
					return new NBCEL.generic.IF_ICMPLT(target);
				}

				case NBCEL.Const.IF_ICMPGE:
				{
					return new NBCEL.generic.IF_ICMPGE(target);
				}

				case NBCEL.Const.IF_ICMPGT:
				{
					return new NBCEL.generic.IF_ICMPGT(target);
				}

				case NBCEL.Const.IF_ICMPLE:
				{
					return new NBCEL.generic.IF_ICMPLE(target);
				}

				case NBCEL.Const.IF_ACMPEQ:
				{
					return new NBCEL.generic.IF_ACMPEQ(target);
				}

				case NBCEL.Const.IF_ACMPNE:
				{
					return new NBCEL.generic.IF_ACMPNE(target);
				}

				case NBCEL.Const.GOTO:
				{
					return new NBCEL.generic.GOTO(target);
				}

				case NBCEL.Const.JSR:
				{
					return new NBCEL.generic.JSR(target);
				}

				case NBCEL.Const.IFNULL:
				{
					return new NBCEL.generic.IFNULL(target);
				}

				case NBCEL.Const.IFNONNULL:
				{
					return new NBCEL.generic.IFNONNULL(target);
				}

				case NBCEL.Const.GOTO_W:
				{
					return new NBCEL.generic.GOTO_W(target);
				}

				case NBCEL.Const.JSR_W:
				{
					return new NBCEL.generic.JSR_W(target);
				}

				default:
				{
					throw new System.Exception("Invalid opcode: " + opcode);
				}
			}
		}

		public virtual void SetClassGen(NBCEL.generic.ClassGen c)
		{
			cg = c;
		}

		public virtual NBCEL.generic.ClassGen GetClassGen()
		{
			return cg;
		}

		public virtual void SetConstantPool(NBCEL.generic.ConstantPoolGen c)
		{
			cp = c;
		}

		public virtual NBCEL.generic.ConstantPoolGen GetConstantPool()
		{
			return cp;
		}
	}
}
