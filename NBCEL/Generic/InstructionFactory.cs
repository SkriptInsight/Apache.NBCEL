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

namespace Apache.NBCEL.Generic
{
	/// <summary>
	///     Instances of this class may be used, e.g., to generate typed
	///     versions of instructions.
	/// </summary>
	/// <remarks>
	///     Instances of this class may be used, e.g., to generate typed
	///     versions of instructions. Its main purpose is to be used as the
	///     byte code generating backend of a compiler. You can subclass it to
	///     add your own create methods.
	///     <p>
	///         Note: The static createXXX methods return singleton instances
	///         from the
	///         <see cref="InstructionConst" />
	///         class.
	/// </remarks>
	/// <seealso cref="NBCEL.Const" />
	/// <seealso cref="InstructionConst" />
	public class InstructionFactory : InstructionConstants
    {
        private static readonly string[] short_names =
        {
            "C", "F", "D", "B",
            "S", "I", "L"
        };

        private static readonly MethodObject[] append_mos
            =
            {
                new MethodObject
                ("java.lang.StringBuffer", "append", Type.STRINGBUFFER, new Type
                    [] {Type.STRING}),
                new MethodObject
                ("java.lang.StringBuffer", "append", Type.STRINGBUFFER, new Type
                    [] {Type.OBJECT}),
                null, null, new MethodObject
                ("java.lang.StringBuffer", "append", Type.STRINGBUFFER, new Type
                    [] {Type.BOOLEAN}),
                new MethodObject
                ("java.lang.StringBuffer", "append", Type.STRINGBUFFER, new Type
                    [] {Type.CHAR}),
                new MethodObject
                ("java.lang.StringBuffer", "append", Type.STRINGBUFFER, new Type
                    [] {Type.FLOAT}),
                new MethodObject
                ("java.lang.StringBuffer", "append", Type.STRINGBUFFER, new Type
                    [] {Type.DOUBLE}),
                new MethodObject
                ("java.lang.StringBuffer", "append", Type.STRINGBUFFER, new Type
                    [] {Type.INT}),
                new MethodObject
                ("java.lang.StringBuffer", "append", Type.STRINGBUFFER, new Type
                    [] {Type.INT}),
                new MethodObject
                ("java.lang.StringBuffer", "append", Type.STRINGBUFFER, new Type
                    [] {Type.INT}),
                new MethodObject
                ("java.lang.StringBuffer", "append", Type.STRINGBUFFER, new Type
                    [] {Type.LONG})
            };

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal ClassGen cg;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal ConstantPoolGen cp;

        public InstructionFactory(ClassGen cg, ConstantPoolGen
            cp)
        {
            // N.N. These must agree with the order of Constants.T_CHAR through T_LONG
            this.cg = cg;
            this.cp = cp;
        }

        /// <summary>Initialize with ClassGen object</summary>
        public InstructionFactory(ClassGen cg)
            : this(cg, cg.GetConstantPool())
        {
        }

        /// <summary>Initialize just with ConstantPoolGen object</summary>
        public InstructionFactory(ConstantPoolGen cp)
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
        ///     how to invoke, i.e., INVOKEINTERFACE, INVOKESTATIC, INVOKEVIRTUAL,
        ///     or INVOKESPECIAL
        /// </param>
        /// <seealso cref="NBCEL.Const" />
        public virtual InvokeInstruction CreateInvoke(string class_name, string
            name, Type ret_type, Type[] arg_types, short kind)
        {
            int index;
            var nargs = 0;
            var signature = Type.GetMethodSignature(ret_type, arg_types);
            foreach (var arg_type in arg_types) nargs += arg_type.GetSize();
            if (kind == Const.INVOKEINTERFACE)
                index = cp.AddInterfaceMethodref(class_name, name, signature);
            else
                index = cp.AddMethodref(class_name, name, signature);
            switch (kind)
            {
                case Const.INVOKESPECIAL:
                {
                    return new INVOKESPECIAL(index);
                }

                case Const.INVOKEVIRTUAL:
                {
                    return new INVOKEVIRTUAL(index);
                }

                case Const.INVOKESTATIC:
                {
                    return new INVOKESTATIC(index);
                }

                case Const.INVOKEINTERFACE:
                {
                    return new INVOKEINTERFACE(index, nargs + 1);
                }

                case Const.INVOKEDYNAMIC:
                {
                    return new INVOKEDYNAMIC(index);
                }

                default:
                {
                    throw new Exception("Oops: Unknown invoke kind: " + kind);
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
        public virtual InstructionList CreatePrintln(string s)
        {
            var il = new InstructionList();
            var @out = cp.AddFieldref("java.lang.System", "out", "Ljava/io/PrintStream;");
            var println = cp.AddMethodref("java.io.PrintStream", "println", "(Ljava/lang/String;)V"
            );
            il.Append(new GETSTATIC(@out));
            il.Append(new PUSH(cp, s));
            il.Append(new INVOKEVIRTUAL(println));
            return il;
        }


        /// <summary>Uses PUSH to push a constant value onto the stack.</summary>
        /// <param name="value">must be of type Number, Boolean, Character or String</param>
        public virtual Instruction CreateConstant(object value)
        {
            PUSH push;
            if (value.IsNumber())
                push = new PUSH(cp, value, true);
            else if (value is string)
                push = new PUSH(cp, (string) value);
            else if (value is bool)
                push = new PUSH(cp, (bool) value);
            else if (value is char)
                push = new PUSH(cp, (char) value);
            else
                throw new ClassGenException("Illegal type: " + value.GetType());
            return push.GetInstruction();
        }

        private InvokeInstruction CreateInvoke(MethodObject
            m, short kind)
        {
            return CreateInvoke(m.class_name, m.name, m.result_type, m.arg_types, kind);
        }

        // indices 2, 3
        // No append(byte)
        // No append(short)
        private static bool IsString(Type type)
        {
            return type is ObjectType && ((ObjectType) type).GetClassName
                       ().Equals("java.lang.String");
        }

        public virtual Instruction CreateAppend(Type type)
        {
            var t = type.GetType();
            if (IsString(type)) return CreateInvoke(append_mos[0], Const.INVOKEVIRTUAL);
            switch (t)
            {
                case Const.T_BOOLEAN:
                case Const.T_CHAR:
                case Const.T_FLOAT:
                case Const.T_DOUBLE:
                case Const.T_BYTE:
                case Const.T_SHORT:
                case Const.T_INT:
                case Const.T_LONG:
                {
                    return CreateInvoke(append_mos[t], Const.INVOKEVIRTUAL);
                }

                case Const.T_ARRAY:
                case Const.T_OBJECT:
                {
                    return CreateInvoke(append_mos[1], Const.INVOKEVIRTUAL);
                }

                default:
                {
                    throw new Exception("Oops: No append for this type? " + type);
                }
            }
        }

        /// <summary>Create a field instruction.</summary>
        /// <param name="class_name">name of the accessed class</param>
        /// <param name="name">name of the referenced field</param>
        /// <param name="type">type of field</param>
        /// <param name="kind">how to access, i.e., GETFIELD, PUTFIELD, GETSTATIC, PUTSTATIC</param>
        /// <seealso cref="NBCEL.Const" />
        public virtual FieldInstruction CreateFieldAccess(string class_name
            , string name, Type type, short kind)
        {
            int index;
            var signature = type.GetSignature();
            index = cp.AddFieldref(class_name, name, signature);
            switch (kind)
            {
                case Const.GETFIELD:
                {
                    return new GETFIELD(index);
                }

                case Const.PUTFIELD:
                {
                    return new PUTFIELD(index);
                }

                case Const.GETSTATIC:
                {
                    return new GETSTATIC(index);
                }

                case Const.PUTSTATIC:
                {
                    return new PUTSTATIC(index);
                }

                default:
                {
                    throw new Exception("Oops: Unknown getfield kind:" + kind);
                }
            }
        }

        /// <summary>Create reference to `this'</summary>
        public static Instruction CreateThis()
        {
            return new ALOAD(0);
        }

        /// <summary>Create typed return</summary>
        public static ReturnInstruction CreateReturn(Type type
        )
        {
            switch (type.GetType())
            {
                case Const.T_ARRAY:
                case Const.T_OBJECT:
                {
                    return InstructionConst.ARETURN;
                }

                case Const.T_INT:
                case Const.T_SHORT:
                case Const.T_BOOLEAN:
                case Const.T_CHAR:
                case Const.T_BYTE:
                {
                    return InstructionConst.IRETURN;
                }

                case Const.T_FLOAT:
                {
                    return InstructionConst.FRETURN;
                }

                case Const.T_DOUBLE:
                {
                    return InstructionConst.DRETURN;
                }

                case Const.T_LONG:
                {
                    return InstructionConst.LRETURN;
                }

                case Const.T_VOID:
                {
                    return InstructionConst.RETURN;
                }

                default:
                {
                    throw new Exception("Invalid type: " + type);
                }
            }
        }

        private static ArithmeticInstruction CreateBinaryIntOp(char first,
            string op)
        {
            switch (first)
            {
                case '-':
                {
                    return InstructionConst.ISUB;
                }

                case '+':
                {
                    return InstructionConst.IADD;
                }

                case '%':
                {
                    return InstructionConst.IREM;
                }

                case '*':
                {
                    return InstructionConst.IMUL;
                }

                case '/':
                {
                    return InstructionConst.IDIV;
                }

                case '&':
                {
                    return InstructionConst.IAND;
                }

                case '|':
                {
                    return InstructionConst.IOR;
                }

                case '^':
                {
                    return InstructionConst.IXOR;
                }

                case '<':
                {
                    return InstructionConst.ISHL;
                }

                case '>':
                {
                    return op.Equals(">>>")
                        ? InstructionConst.IUSHR
                        : InstructionConst
                            .ISHR;
                }

                default:
                {
                    throw new Exception("Invalid operand " + op);
                }
            }
        }

        private static ArithmeticInstruction CreateBinaryLongOp(char first,
            string op)
        {
            switch (first)
            {
                case '-':
                {
                    return InstructionConst.LSUB;
                }

                case '+':
                {
                    return InstructionConst.LADD;
                }

                case '%':
                {
                    return InstructionConst.LREM;
                }

                case '*':
                {
                    return InstructionConst.LMUL;
                }

                case '/':
                {
                    return InstructionConst.LDIV;
                }

                case '&':
                {
                    return InstructionConst.LAND;
                }

                case '|':
                {
                    return InstructionConst.LOR;
                }

                case '^':
                {
                    return InstructionConst.LXOR;
                }

                case '<':
                {
                    return InstructionConst.LSHL;
                }

                case '>':
                {
                    return op.Equals(">>>")
                        ? InstructionConst.LUSHR
                        : InstructionConst
                            .LSHR;
                }

                default:
                {
                    throw new Exception("Invalid operand " + op);
                }
            }
        }

        private static ArithmeticInstruction CreateBinaryFloatOp(char op)
        {
            switch (op)
            {
                case '-':
                {
                    return InstructionConst.FSUB;
                }

                case '+':
                {
                    return InstructionConst.FADD;
                }

                case '*':
                {
                    return InstructionConst.FMUL;
                }

                case '/':
                {
                    return InstructionConst.FDIV;
                }

                case '%':
                {
                    return InstructionConst.FREM;
                }

                default:
                {
                    throw new Exception("Invalid operand " + op);
                }
            }
        }

        private static ArithmeticInstruction CreateBinaryDoubleOp(char op)
        {
            switch (op)
            {
                case '-':
                {
                    return InstructionConst.DSUB;
                }

                case '+':
                {
                    return InstructionConst.DADD;
                }

                case '*':
                {
                    return InstructionConst.DMUL;
                }

                case '/':
                {
                    return InstructionConst.DDIV;
                }

                case '%':
                {
                    return InstructionConst.DREM;
                }

                default:
                {
                    throw new Exception("Invalid operand " + op);
                }
            }
        }

        /// <summary>Create binary operation for simple basic types, such as int and float.</summary>
        /// <param name="op">operation, such as "+", "*", "&lt;&lt;", etc.</param>
        public static ArithmeticInstruction CreateBinaryOperation(string op
            , Type type)
        {
            var first = op[0];
            switch (type.GetType())
            {
                case Const.T_BYTE:
                case Const.T_SHORT:
                case Const.T_INT:
                case Const.T_CHAR:
                {
                    return CreateBinaryIntOp(first, op);
                }

                case Const.T_LONG:
                {
                    return CreateBinaryLongOp(first, op);
                }

                case Const.T_FLOAT:
                {
                    return CreateBinaryFloatOp(first);
                }

                case Const.T_DOUBLE:
                {
                    return CreateBinaryDoubleOp(first);
                }

                default:
                {
                    throw new Exception("Invalid type " + type);
                }
            }
        }

        /// <param name="size">size of operand, either 1 (int, e.g.) or 2 (double)</param>
        public static StackInstruction CreatePop(int size)
        {
            return size == 2
                ? InstructionConst.POP2
                : InstructionConst
                    .POP;
        }

        /// <param name="size">size of operand, either 1 (int, e.g.) or 2 (double)</param>
        public static StackInstruction CreateDup(int size)
        {
            return size == 2
                ? InstructionConst.DUP2
                : InstructionConst
                    .DUP;
        }

        /// <param name="size">size of operand, either 1 (int, e.g.) or 2 (double)</param>
        public static StackInstruction CreateDup_2(int size)
        {
            return size == 2
                ? InstructionConst.DUP2_X2
                : InstructionConst
                    .DUP_X2;
        }

        /// <param name="size">size of operand, either 1 (int, e.g.) or 2 (double)</param>
        public static StackInstruction CreateDup_1(int size)
        {
            return size == 2
                ? InstructionConst.DUP2_X1
                : InstructionConst
                    .DUP_X1;
        }

        /// <param name="index">index of local variable</param>
        public static LocalVariableInstruction CreateStore(Type
            type, int index)
        {
            switch (type.GetType())
            {
                case Const.T_BOOLEAN:
                case Const.T_CHAR:
                case Const.T_BYTE:
                case Const.T_SHORT:
                case Const.T_INT:
                {
                    return new ISTORE(index);
                }

                case Const.T_FLOAT:
                {
                    return new FSTORE(index);
                }

                case Const.T_DOUBLE:
                {
                    return new DSTORE(index);
                }

                case Const.T_LONG:
                {
                    return new LSTORE(index);
                }

                case Const.T_ARRAY:
                case Const.T_OBJECT:
                {
                    return new ASTORE(index);
                }

                default:
                {
                    throw new Exception("Invalid type " + type);
                }
            }
        }

        /// <param name="index">index of local variable</param>
        public static LocalVariableInstruction CreateLoad(Type
            type, int index)
        {
            switch (type.GetType())
            {
                case Const.T_BOOLEAN:
                case Const.T_CHAR:
                case Const.T_BYTE:
                case Const.T_SHORT:
                case Const.T_INT:
                {
                    return new ILOAD(index);
                }

                case Const.T_FLOAT:
                {
                    return new FLOAD(index);
                }

                case Const.T_DOUBLE:
                {
                    return new DLOAD(index);
                }

                case Const.T_LONG:
                {
                    return new LLOAD(index);
                }

                case Const.T_ARRAY:
                case Const.T_OBJECT:
                {
                    return new ALOAD(index);
                }

                default:
                {
                    throw new Exception("Invalid type " + type);
                }
            }
        }

        /// <param name="type">type of elements of array, i.e., array.getElementType()</param>
        public static ArrayInstruction CreateArrayLoad(Type type
        )
        {
            switch (type.GetType())
            {
                case Const.T_BOOLEAN:
                case Const.T_BYTE:
                {
                    return InstructionConst.BALOAD;
                }

                case Const.T_CHAR:
                {
                    return InstructionConst.CALOAD;
                }

                case Const.T_SHORT:
                {
                    return InstructionConst.SALOAD;
                }

                case Const.T_INT:
                {
                    return InstructionConst.IALOAD;
                }

                case Const.T_FLOAT:
                {
                    return InstructionConst.FALOAD;
                }

                case Const.T_DOUBLE:
                {
                    return InstructionConst.DALOAD;
                }

                case Const.T_LONG:
                {
                    return InstructionConst.LALOAD;
                }

                case Const.T_ARRAY:
                case Const.T_OBJECT:
                {
                    return InstructionConst.AALOAD;
                }

                default:
                {
                    throw new Exception("Invalid type " + type);
                }
            }
        }

        /// <param name="type">type of elements of array, i.e., array.getElementType()</param>
        public static ArrayInstruction CreateArrayStore(Type
            type)
        {
            switch (type.GetType())
            {
                case Const.T_BOOLEAN:
                case Const.T_BYTE:
                {
                    return InstructionConst.BASTORE;
                }

                case Const.T_CHAR:
                {
                    return InstructionConst.CASTORE;
                }

                case Const.T_SHORT:
                {
                    return InstructionConst.SASTORE;
                }

                case Const.T_INT:
                {
                    return InstructionConst.IASTORE;
                }

                case Const.T_FLOAT:
                {
                    return InstructionConst.FASTORE;
                }

                case Const.T_DOUBLE:
                {
                    return InstructionConst.DASTORE;
                }

                case Const.T_LONG:
                {
                    return InstructionConst.LASTORE;
                }

                case Const.T_ARRAY:
                case Const.T_OBJECT:
                {
                    return InstructionConst.AASTORE;
                }

                default:
                {
                    throw new Exception("Invalid type " + type);
                }
            }
        }

        /// <summary>
        ///     Create conversion operation for two stack operands, this may be an I2C, instruction, e.g.,
        ///     if the operands are basic types and CHECKCAST if they are reference types.
        /// </summary>
        public virtual Instruction CreateCast(Type src_type,
            Type dest_type)
        {
            if (src_type is BasicType && dest_type is BasicType)
            {
                var dest = dest_type.GetType();
                var src = src_type.GetType();
                if (dest == Const.T_LONG && (src == Const.T_CHAR || src == Const
                                                 .T_BYTE || src == Const.T_SHORT))
                    src = Const.T_INT;
                var name = "org.apache.bcel.generic." + short_names[src - Const.T_CHAR]
                                                      + "2" + short_names[dest - Const.T_CHAR];
                Instruction i = null;
                try
                {
                    i = (Instruction) Activator.CreateInstance(Runtime.GetJavaType
                        (name));
                }
                catch (Exception e)
                {
                    throw new Exception("Could not find instruction: " + name, e);
                }

                return i;
            }

            if (src_type is ReferenceType && dest_type is ReferenceType)
            {
                if (dest_type is ArrayType)
                    return new CHECKCAST(cp.AddArrayClass((ArrayType) dest_type
                    ));
                return new CHECKCAST(cp.AddClass(((ObjectType) dest_type
                    ).GetClassName()));
            }

            throw new Exception("Can not cast " + src_type + " to " + dest_type);
        }

        public virtual GETFIELD CreateGetField(string class_name, string name
            , Type t)
        {
            return new GETFIELD(cp.AddFieldref(class_name, name, t.GetSignature
                ()));
        }

        public virtual GETSTATIC CreateGetStatic(string class_name, string
            name, Type t)
        {
            return new GETSTATIC(cp.AddFieldref(class_name, name, t.GetSignature
                ()));
        }

        public virtual PUTFIELD CreatePutField(string class_name, string name
            , Type t)
        {
            return new PUTFIELD(cp.AddFieldref(class_name, name, t.GetSignature
                ()));
        }

        public virtual PUTSTATIC CreatePutStatic(string class_name, string
            name, Type t)
        {
            return new PUTSTATIC(cp.AddFieldref(class_name, name, t.GetSignature
                ()));
        }

        public virtual CHECKCAST CreateCheckCast(ReferenceType
            t)
        {
            if (t is ArrayType) return new CHECKCAST(cp.AddArrayClass((ArrayType) t));
            return new CHECKCAST(cp.AddClass((ObjectType) t));
        }

        public virtual INSTANCEOF CreateInstanceOf(ReferenceType
            t)
        {
            if (t is ArrayType) return new INSTANCEOF(cp.AddArrayClass((ArrayType) t));
            return new INSTANCEOF(cp.AddClass((ObjectType) t));
        }

        public virtual NEW CreateNew(ObjectType t)
        {
            return new NEW(cp.AddClass(t));
        }

        public virtual NEW CreateNew(string s)
        {
            return CreateNew(ObjectType.GetInstance(s));
        }

        /// <summary>Create new array of given size and type.</summary>
        /// <returns>
        ///     an instruction that creates the corresponding array at runtime, i.e. is an AllocationInstruction
        /// </returns>
        public virtual Instruction CreateNewArray(Type t, short
            dim)
        {
            if (dim == 1)
            {
                if (t is ObjectType)
                    return new ANEWARRAY(cp.AddClass((ObjectType) t));
                if (t is ArrayType)
                    return new ANEWARRAY(cp.AddArrayClass((ArrayType) t));
                return new NEWARRAY(t.GetType());
            }

            ArrayType at;
            if (t is ArrayType)
                at = (ArrayType) t;
            else
                at = new ArrayType(t, dim);
            return new MULTIANEWARRAY(cp.AddArrayClass(at), dim);
        }

        /// <summary>Create "null" value for reference types, 0 for basic types like int</summary>
        public static Instruction CreateNull(Type type)
        {
            switch (type.GetType())
            {
                case Const.T_ARRAY:
                case Const.T_OBJECT:
                {
                    return InstructionConst.ACONST_NULL;
                }

                case Const.T_INT:
                case Const.T_SHORT:
                case Const.T_BOOLEAN:
                case Const.T_CHAR:
                case Const.T_BYTE:
                {
                    return InstructionConst.ICONST_0;
                }

                case Const.T_FLOAT:
                {
                    return InstructionConst.FCONST_0;
                }

                case Const.T_DOUBLE:
                {
                    return InstructionConst.DCONST_0;
                }

                case Const.T_LONG:
                {
                    return InstructionConst.LCONST_0;
                }

                case Const.T_VOID:
                {
                    return InstructionConst.NOP;
                }

                default:
                {
                    throw new Exception("Invalid type: " + type);
                }
            }
        }

        /// <summary>
        ///     Create branch instruction by given opcode, except LOOKUPSWITCH and TABLESWITCH.
        /// </summary>
        /// <remarks>
        ///     Create branch instruction by given opcode, except LOOKUPSWITCH and TABLESWITCH.
        ///     For those you should use the SWITCH compound instruction.
        /// </remarks>
        public static BranchInstruction CreateBranchInstruction(short opcode
            , InstructionHandle target)
        {
            switch (opcode)
            {
                case Const.IFEQ:
                {
                    return new IFEQ(target);
                }

                case Const.IFNE:
                {
                    return new IFNE(target);
                }

                case Const.IFLT:
                {
                    return new IFLT(target);
                }

                case Const.IFGE:
                {
                    return new IFGE(target);
                }

                case Const.IFGT:
                {
                    return new IFGT(target);
                }

                case Const.IFLE:
                {
                    return new IFLE(target);
                }

                case Const.IF_ICMPEQ:
                {
                    return new IF_ICMPEQ(target);
                }

                case Const.IF_ICMPNE:
                {
                    return new IF_ICMPNE(target);
                }

                case Const.IF_ICMPLT:
                {
                    return new IF_ICMPLT(target);
                }

                case Const.IF_ICMPGE:
                {
                    return new IF_ICMPGE(target);
                }

                case Const.IF_ICMPGT:
                {
                    return new IF_ICMPGT(target);
                }

                case Const.IF_ICMPLE:
                {
                    return new IF_ICMPLE(target);
                }

                case Const.IF_ACMPEQ:
                {
                    return new IF_ACMPEQ(target);
                }

                case Const.IF_ACMPNE:
                {
                    return new IF_ACMPNE(target);
                }

                case Const.GOTO:
                {
                    return new GOTO(target);
                }

                case Const.JSR:
                {
                    return new JSR(target);
                }

                case Const.IFNULL:
                {
                    return new IFNULL(target);
                }

                case Const.IFNONNULL:
                {
                    return new IFNONNULL(target);
                }

                case Const.GOTO_W:
                {
                    return new GOTO_W(target);
                }

                case Const.JSR_W:
                {
                    return new JSR_W(target);
                }

                default:
                {
                    throw new Exception("Invalid opcode: " + opcode);
                }
            }
        }

        public virtual void SetClassGen(ClassGen c)
        {
            cg = c;
        }

        public virtual ClassGen GetClassGen()
        {
            return cg;
        }

        public virtual void SetConstantPool(ConstantPoolGen c)
        {
            cp = c;
        }

        public virtual ConstantPoolGen GetConstantPool()
        {
            return cp;
        }

        private class MethodObject
        {
            internal readonly Type[] arg_types;

            internal readonly string class_name;

            internal readonly string name;

            internal readonly Type result_type;

            internal MethodObject(string c, string n, Type r, Type
                [] a)
            {
                class_name = c;
                name = n;
                result_type = r;
                arg_types = a;
            }
        }
    }
}