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
using Sharpen;

namespace NBCEL.generic
{
	/// <summary>Template class for building up a method.</summary>
	/// <remarks>
	/// Template class for building up a method. This is done by defining exception
	/// handlers, adding thrown exceptions, local variables and attributes, whereas
	/// the `LocalVariableTable' and `LineNumberTable' attributes will be set
	/// automatically for the code. Use stripAttributes() if you don't like this.
	/// While generating code it may be necessary to insert NOP operations. You can
	/// use the `removeNOPs' method to get rid off them.
	/// The resulting method object can be obtained via the `getMethod()' method.
	/// </remarks>
	/// <seealso cref="InstructionList"/>
	/// <seealso cref="NBCEL.classfile.Method"/>
	public class MethodGen : NBCEL.generic.FieldGenOrMethodGen
	{
		private string class_name;

		private NBCEL.generic.Type[] arg_types;

		private string[] arg_names;

		private int max_locals;

		private int max_stack;

		private NBCEL.generic.InstructionList il;

		private bool strip_attributes;

		private NBCEL.classfile.LocalVariableTypeTable local_variable_type_table = null;

		private readonly System.Collections.Generic.List<NBCEL.generic.LocalVariableGen> 
			variable_vec = new System.Collections.Generic.List<NBCEL.generic.LocalVariableGen
			>();

		private readonly System.Collections.Generic.List<NBCEL.generic.LineNumberGen> line_number_vec
			 = new System.Collections.Generic.List<NBCEL.generic.LineNumberGen>();

		private readonly System.Collections.Generic.List<NBCEL.generic.CodeExceptionGen> 
			exception_vec = new System.Collections.Generic.List<NBCEL.generic.CodeExceptionGen
			>();

		private readonly System.Collections.Generic.List<string> throws_vec = new System.Collections.Generic.List
			<string>();

		private readonly System.Collections.Generic.List<NBCEL.classfile.Attribute> code_attrs_vec
			 = new System.Collections.Generic.List<NBCEL.classfile.Attribute>();

		private System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen>[] param_annotations;

		private bool hasParameterAnnotations = false;

		private bool haveUnpackedParameterAnnotations = false;

		private sealed class _BCELComparator_79 : NBCEL.util.BCELComparator
		{
			public _BCELComparator_79()
			{
			}

			// Array of lists containing AnnotationGen objects
			public bool Equals(object o1, object o2)
			{
				NBCEL.generic.MethodGen THIS = (NBCEL.generic.MethodGen)o1;
				NBCEL.generic.MethodGen THAT = (NBCEL.generic.MethodGen)o2;
				return Sharpen.System.Equals(THIS.GetName(), THAT.GetName()) && Sharpen.System
					.Equals(THIS.GetSignature(), THAT.GetSignature());
			}

			public int HashCode(object o)
			{
				NBCEL.generic.MethodGen THIS = (NBCEL.generic.MethodGen)o;
				return THIS.GetSignature().GetHashCode() ^ THIS.GetName().GetHashCode();
			}
		}

		private static NBCEL.util.BCELComparator bcelComparator = new _BCELComparator_79(
			);

		/// <summary>Declare method.</summary>
		/// <remarks>
		/// Declare method. If the method is non-static the constructor
		/// automatically declares a local variable `$this' in slot 0. The
		/// actual code is contained in the `il' parameter, which may further
		/// manipulated by the user. But he must take care not to remove any
		/// instruction (handles) that are still referenced from this object.
		/// For example one may not add a local variable and later remove the
		/// instructions it refers to without causing havoc. It is safe
		/// however if you remove that local variable, too.
		/// </remarks>
		/// <param name="access_flags">access qualifiers</param>
		/// <param name="return_type">method type</param>
		/// <param name="arg_types">argument types</param>
		/// <param name="arg_names">
		/// argument names (if this is null, default names will be provided
		/// for them)
		/// </param>
		/// <param name="method_name">name of method</param>
		/// <param name="class_name">class name containing this method (may be null, if you don't care)
		/// 	</param>
		/// <param name="il">
		/// instruction list associated with this method, may be null only for
		/// abstract or native methods
		/// </param>
		/// <param name="cp">constant pool</param>
		public MethodGen(int access_flags, NBCEL.generic.Type return_type, NBCEL.generic.Type
			[] arg_types, string[] arg_names, string method_name, string class_name, NBCEL.generic.InstructionList
			 il, NBCEL.generic.ConstantPoolGen cp)
			: base(access_flags)
		{
			SetType(return_type);
			SetArgumentTypes(arg_types);
			SetArgumentNames(arg_names);
			SetName(method_name);
			SetClassName(class_name);
			SetInstructionList(il);
			SetConstantPool(cp);
			bool abstract_ = IsAbstract() || IsNative();
			NBCEL.generic.InstructionHandle start = null;
			NBCEL.generic.InstructionHandle end = null;
			if (!abstract_)
			{
				start = il.GetStart();
				// end == null => live to end of method
				/* Add local variables, namely the implicit `this' and the arguments
				*/
				if (!IsStatic() && (class_name != null))
				{
					// Instance method -> `this' is local var 0
					AddLocalVariable("this", NBCEL.generic.ObjectType.GetInstance(class_name), start, 
						end);
				}
			}
			if (arg_types != null)
			{
				int size = arg_types.Length;
				foreach (NBCEL.generic.Type arg_type in arg_types)
				{
					if (NBCEL.generic.Type.VOID == arg_type)
					{
						throw new NBCEL.generic.ClassGenException("'void' is an illegal argument type for a method"
							);
					}
				}
				if (arg_names != null)
				{
					// Names for variables provided?
					if (size != arg_names.Length)
					{
						throw new NBCEL.generic.ClassGenException("Mismatch in argument array lengths: " 
							+ size + " vs. " + arg_names.Length);
					}
				}
				else
				{
					// Give them dummy names
					arg_names = new string[size];
					for (int i = 0; i < size; i++)
					{
						arg_names[i] = "arg" + i;
					}
					SetArgumentNames(arg_names);
				}
				if (!abstract_)
				{
					for (int i = 0; i < size; i++)
					{
						AddLocalVariable(arg_names[i], arg_types[i], start, end);
					}
				}
			}
		}

		/// <summary>Instantiate from existing method.</summary>
		/// <param name="m">method</param>
		/// <param name="class_name">class name containing this method</param>
		/// <param name="cp">constant pool</param>
		public MethodGen(NBCEL.classfile.Method m, string class_name, NBCEL.generic.ConstantPoolGen
			 cp)
			: this(m.GetAccessFlags(), NBCEL.generic.Type.GetReturnType(m.GetSignature()), NBCEL.generic.Type
				.GetArgumentTypes(m.GetSignature()), null, m.GetName(), class_name, ((m.GetAccessFlags
				() & (NBCEL.Const.ACC_ABSTRACT | NBCEL.Const.ACC_NATIVE)) == 0) ? new NBCEL.generic.InstructionList
				(m.GetCode().GetCode()) : null, cp)
		{
			/* may be overridden anyway */
			NBCEL.classfile.Attribute[] attributes = m.GetAttributes();
			foreach (NBCEL.classfile.Attribute attribute in attributes)
			{
				NBCEL.classfile.Attribute a = attribute;
				if (a is NBCEL.classfile.Code)
				{
					NBCEL.classfile.Code c = (NBCEL.classfile.Code)a;
					SetMaxStack(c.GetMaxStack());
					SetMaxLocals(c.GetMaxLocals());
					NBCEL.classfile.CodeException[] ces = c.GetExceptionTable();
					if (ces != null)
					{
						foreach (NBCEL.classfile.CodeException ce in ces)
						{
							int type = ce.GetCatchType();
							NBCEL.generic.ObjectType c_type = null;
							if (type > 0)
							{
								string cen = m.GetConstantPool().GetConstantString(type, NBCEL.Const.CONSTANT_Class
									);
								c_type = NBCEL.generic.ObjectType.GetInstance(cen);
							}
							int end_pc = ce.GetEndPC();
							int length = m.GetCode().GetCode().Length;
							NBCEL.generic.InstructionHandle end;
							if (length == end_pc)
							{
								// May happen, because end_pc is exclusive
								end = il.GetEnd();
							}
							else
							{
								end = il.FindHandle(end_pc);
								end = end.GetPrev();
							}
							// Make it inclusive
							AddExceptionHandler(il.FindHandle(ce.GetStartPC()), end, il.FindHandle(ce.GetHandlerPC
								()), c_type);
						}
					}
					NBCEL.classfile.Attribute[] c_attributes = c.GetAttributes();
					foreach (NBCEL.classfile.Attribute c_attribute in c_attributes)
					{
						a = c_attribute;
						if (a is NBCEL.classfile.LineNumberTable)
						{
							NBCEL.classfile.LineNumber[] ln = ((NBCEL.classfile.LineNumberTable)a).GetLineNumberTable
								();
							foreach (NBCEL.classfile.LineNumber l in ln)
							{
								NBCEL.generic.InstructionHandle ih = il.FindHandle(l.GetStartPC());
								if (ih != null)
								{
									AddLineNumber(ih, l.GetLineNumber());
								}
							}
						}
						else if (a is NBCEL.classfile.LocalVariableTable)
						{
							UpdateLocalVariableTable((NBCEL.classfile.LocalVariableTable)a);
						}
						else if (a is NBCEL.classfile.LocalVariableTypeTable)
						{
							this.local_variable_type_table = (NBCEL.classfile.LocalVariableTypeTable)a.Copy(cp
								.GetConstantPool());
						}
						else
						{
							AddCodeAttribute(a);
						}
					}
				}
				else if (a is NBCEL.classfile.ExceptionTable)
				{
					string[] names = ((NBCEL.classfile.ExceptionTable)a).GetExceptionNames();
					foreach (string name2 in names)
					{
						AddException(name2);
					}
				}
				else if (a is NBCEL.classfile.Annotations)
				{
					NBCEL.classfile.Annotations runtimeAnnotations = (NBCEL.classfile.Annotations)a;
					NBCEL.classfile.AnnotationEntry[] aes = runtimeAnnotations.GetAnnotationEntries();
					foreach (NBCEL.classfile.AnnotationEntry element in aes)
					{
						AddAnnotationEntry(new NBCEL.generic.AnnotationEntryGen(element, cp, false));
					}
				}
				else
				{
					AddAttribute(a);
				}
			}
		}

		/// <summary>Adds a local variable to this method.</summary>
		/// <param name="name">variable name</param>
		/// <param name="type">variable type</param>
		/// <param name="slot">
		/// the index of the local variable, if type is long or double, the next available
		/// index is slot+2
		/// </param>
		/// <param name="start">from where the variable is valid</param>
		/// <param name="end">until where the variable is valid</param>
		/// <param name="orig_index">the index of the local variable prior to any modifications
		/// 	</param>
		/// <returns>new local variable object</returns>
		/// <seealso cref="NBCEL.classfile.LocalVariable"/>
		public virtual NBCEL.generic.LocalVariableGen AddLocalVariable(string name, NBCEL.generic.Type
			 type, int slot, NBCEL.generic.InstructionHandle start, NBCEL.generic.InstructionHandle
			 end, int orig_index)
		{
			byte t = type.GetType();
			if (t != NBCEL.Const.T_ADDRESS)
			{
				int add = type.GetSize();
				if (slot + add > max_locals)
				{
					max_locals = slot + add;
				}
				NBCEL.generic.LocalVariableGen l = new NBCEL.generic.LocalVariableGen(slot, name, 
					type, start, end, orig_index);
				int i;
				if ((i = variable_vec.IndexOf(l)) >= 0)
				{
					variable_vec[i] = l;
				}
				else
				{
					variable_vec.Add(l);
				}
				return l;
			}
			throw new System.ArgumentException("Can not use " + type + " as type for local variable"
				);
		}

		/// <summary>Adds a local variable to this method.</summary>
		/// <param name="name">variable name</param>
		/// <param name="type">variable type</param>
		/// <param name="slot">
		/// the index of the local variable, if type is long or double, the next available
		/// index is slot+2
		/// </param>
		/// <param name="start">from where the variable is valid</param>
		/// <param name="end">until where the variable is valid</param>
		/// <returns>new local variable object</returns>
		/// <seealso cref="NBCEL.classfile.LocalVariable"/>
		public virtual NBCEL.generic.LocalVariableGen AddLocalVariable(string name, NBCEL.generic.Type
			 type, int slot, NBCEL.generic.InstructionHandle start, NBCEL.generic.InstructionHandle
			 end)
		{
			return AddLocalVariable(name, type, slot, start, end, slot);
		}

		/// <summary>Adds a local variable to this method and assigns an index automatically.
		/// 	</summary>
		/// <param name="name">variable name</param>
		/// <param name="type">variable type</param>
		/// <param name="start">
		/// from where the variable is valid, if this is null,
		/// it is valid from the start
		/// </param>
		/// <param name="end">
		/// until where the variable is valid, if this is null,
		/// it is valid to the end
		/// </param>
		/// <returns>new local variable object</returns>
		/// <seealso cref="NBCEL.classfile.LocalVariable"/>
		public virtual NBCEL.generic.LocalVariableGen AddLocalVariable(string name, NBCEL.generic.Type
			 type, NBCEL.generic.InstructionHandle start, NBCEL.generic.InstructionHandle end
			)
		{
			return AddLocalVariable(name, type, max_locals, start, end);
		}

		/// <summary>
		/// Remove a local variable, its slot will not be reused, if you do not use addLocalVariable
		/// with an explicit index argument.
		/// </summary>
		public virtual void RemoveLocalVariable(NBCEL.generic.LocalVariableGen l)
		{
			l.Dispose();
			variable_vec.Remove(l);
		}

		/// <summary>Remove all local variables.</summary>
		public virtual void RemoveLocalVariables()
		{
			foreach (NBCEL.generic.LocalVariableGen lv in variable_vec)
			{
				lv.Dispose();
			}
			variable_vec.Clear();
		}

		/*
		* If the range of the variable has not been set yet, it will be set to be valid from
		* the start to the end of the instruction list.
		*
		* @return array of declared local variables sorted by index
		*/
		public virtual NBCEL.generic.LocalVariableGen[] GetLocalVariables()
		{
			int size = variable_vec.Count;
			NBCEL.generic.LocalVariableGen[] lg = new NBCEL.generic.LocalVariableGen[size];
			Sharpen.Collections.ToArray(variable_vec, lg);
			for (int i = 0; i < size; i++)
			{
				if ((lg[i].GetStart() == null) && (il != null))
				{
					lg[i].SetStart(il.GetStart());
				}
				if ((lg[i].GetEnd() == null) && (il != null))
				{
					lg[i].SetEnd(il.GetEnd());
				}
			}
			if (size > 1)
			{
				Array.Sort(lg, (NBCEL.generic.LocalVariableGen o1, NBCEL.generic.LocalVariableGen
					 o2) => o1.GetIndex() - o2.GetIndex());
			}
			return lg;
		}

		/// <returns>`LocalVariableTable' attribute of all the local variables of this method.
		/// 	</returns>
		public virtual NBCEL.classfile.LocalVariableTable GetLocalVariableTable(NBCEL.generic.ConstantPoolGen
			 cp)
		{
			NBCEL.generic.LocalVariableGen[] lg = GetLocalVariables();
			int size = lg.Length;
			NBCEL.classfile.LocalVariable[] lv = new NBCEL.classfile.LocalVariable[size];
			for (int i = 0; i < size; i++)
			{
				lv[i] = lg[i].GetLocalVariable(cp);
			}
			return new NBCEL.classfile.LocalVariableTable(cp.AddUtf8("LocalVariableTable"), 2
				 + lv.Length * 10, lv, cp.GetConstantPool());
		}

		/// <returns>`LocalVariableTypeTable' attribute of this method.</returns>
		public virtual NBCEL.classfile.LocalVariableTypeTable GetLocalVariableTypeTable()
		{
			return local_variable_type_table;
		}

		/// <summary>Give an instruction a line number corresponding to the source code line.
		/// 	</summary>
		/// <param name="ih">instruction to tag</param>
		/// <returns>new line number object</returns>
		/// <seealso cref="NBCEL.classfile.LineNumber"/>
		public virtual NBCEL.generic.LineNumberGen AddLineNumber(NBCEL.generic.InstructionHandle
			 ih, int src_line)
		{
			NBCEL.generic.LineNumberGen l = new NBCEL.generic.LineNumberGen(ih, src_line);
			line_number_vec.Add(l);
			return l;
		}

		/// <summary>Remove a line number.</summary>
		public virtual void RemoveLineNumber(NBCEL.generic.LineNumberGen l)
		{
			line_number_vec.Remove(l);
		}

		/// <summary>Remove all line numbers.</summary>
		public virtual void RemoveLineNumbers()
		{
			line_number_vec.Clear();
		}

		/*
		* @return array of line numbers
		*/
		public virtual NBCEL.generic.LineNumberGen[] GetLineNumbers()
		{
			NBCEL.generic.LineNumberGen[] lg = new NBCEL.generic.LineNumberGen[line_number_vec
				.Count];
			Sharpen.Collections.ToArray(line_number_vec, lg);
			return lg;
		}

		/// <returns>`LineNumberTable' attribute of all the local variables of this method.</returns>
		public virtual NBCEL.classfile.LineNumberTable GetLineNumberTable(NBCEL.generic.ConstantPoolGen
			 cp)
		{
			int size = line_number_vec.Count;
			NBCEL.classfile.LineNumber[] ln = new NBCEL.classfile.LineNumber[size];
			for (int i = 0; i < size; i++)
			{
				ln[i] = line_number_vec[i].GetLineNumber();
			}
			return new NBCEL.classfile.LineNumberTable(cp.AddUtf8("LineNumberTable"), 2 + ln.
				Length * 4, ln, cp.GetConstantPool());
		}

		/// <summary>
		/// Add an exception handler, i.e., specify region where a handler is active and an
		/// instruction where the actual handling is done.
		/// </summary>
		/// <param name="start_pc">Start of region (inclusive)</param>
		/// <param name="end_pc">End of region (inclusive)</param>
		/// <param name="handler_pc">Where handling is done</param>
		/// <param name="catch_type">
		/// class type of handled exception or null if any
		/// exception is handled
		/// </param>
		/// <returns>new exception handler object</returns>
		public virtual NBCEL.generic.CodeExceptionGen AddExceptionHandler(NBCEL.generic.InstructionHandle
			 start_pc, NBCEL.generic.InstructionHandle end_pc, NBCEL.generic.InstructionHandle
			 handler_pc, NBCEL.generic.ObjectType catch_type)
		{
			if ((start_pc == null) || (end_pc == null) || (handler_pc == null))
			{
				throw new NBCEL.generic.ClassGenException("Exception handler target is null instruction"
					);
			}
			NBCEL.generic.CodeExceptionGen c = new NBCEL.generic.CodeExceptionGen(start_pc, end_pc
				, handler_pc, catch_type);
			exception_vec.Add(c);
			return c;
		}

		/// <summary>Remove an exception handler.</summary>
		public virtual void RemoveExceptionHandler(NBCEL.generic.CodeExceptionGen c)
		{
			exception_vec.Remove(c);
		}

		/// <summary>Remove all line numbers.</summary>
		public virtual void RemoveExceptionHandlers()
		{
			exception_vec.Clear();
		}

		/*
		* @return array of declared exception handlers
		*/
		public virtual NBCEL.generic.CodeExceptionGen[] GetExceptionHandlers()
		{
			NBCEL.generic.CodeExceptionGen[] cg = new NBCEL.generic.CodeExceptionGen[exception_vec
				.Count];
			Sharpen.Collections.ToArray(exception_vec, cg);
			return cg;
		}

		/// <returns>code exceptions for `Code' attribute</returns>
		private NBCEL.classfile.CodeException[] GetCodeExceptions()
		{
			int size = exception_vec.Count;
			NBCEL.classfile.CodeException[] c_exc = new NBCEL.classfile.CodeException[size];
			for (int i = 0; i < size; i++)
			{
				NBCEL.generic.CodeExceptionGen c = exception_vec[i];
				c_exc[i] = c.GetCodeException(base.GetConstantPool());
			}
			return c_exc;
		}

		/// <summary>Add an exception possibly thrown by this method.</summary>
		/// <param name="class_name">(fully qualified) name of exception</param>
		public virtual void AddException(string class_name)
		{
			throws_vec.Add(class_name);
		}

		/// <summary>Remove an exception.</summary>
		public virtual void RemoveException(string c)
		{
			throws_vec.Remove(c);
		}

		/// <summary>Remove all exceptions.</summary>
		public virtual void RemoveExceptions()
		{
			throws_vec.Clear();
		}

		/*
		* @return array of thrown exceptions
		*/
		public virtual string[] GetExceptions()
		{
			string[] e = new string[throws_vec.Count];
			Sharpen.Collections.ToArray(throws_vec, e);
			return e;
		}

		/// <returns>`Exceptions' attribute of all the exceptions thrown by this method.</returns>
		private NBCEL.classfile.ExceptionTable GetExceptionTable(NBCEL.generic.ConstantPoolGen
			 cp)
		{
			int size = throws_vec.Count;
			int[] ex = new int[size];
			for (int i = 0; i < size; i++)
			{
				ex[i] = cp.AddClass(throws_vec[i]);
			}
			return new NBCEL.classfile.ExceptionTable(cp.AddUtf8("Exceptions"), 2 + 2 * size, 
				ex, cp.GetConstantPool());
		}

		/// <summary>Add an attribute to the code.</summary>
		/// <remarks>
		/// Add an attribute to the code. Currently, the JVM knows about the
		/// LineNumberTable, LocalVariableTable and StackMap attributes,
		/// where the former two will be generated automatically and the
		/// latter is used for the MIDP only. Other attributes will be
		/// ignored by the JVM but do no harm.
		/// </remarks>
		/// <param name="a">attribute to be added</param>
		public virtual void AddCodeAttribute(NBCEL.classfile.Attribute a)
		{
			code_attrs_vec.Add(a);
		}

		/// <summary>Remove the LocalVariableTypeTable</summary>
		public virtual void RemoveLocalVariableTypeTable()
		{
			local_variable_type_table = null;
		}

		/// <summary>Remove a code attribute.</summary>
		public virtual void RemoveCodeAttribute(NBCEL.classfile.Attribute a)
		{
			code_attrs_vec.Remove(a);
		}

		/// <summary>Remove all code attributes.</summary>
		public virtual void RemoveCodeAttributes()
		{
			local_variable_type_table = null;
			code_attrs_vec.Clear();
		}

		/// <returns>all attributes of this method.</returns>
		public virtual NBCEL.classfile.Attribute[] GetCodeAttributes()
		{
			NBCEL.classfile.Attribute[] attributes = new NBCEL.classfile.Attribute[code_attrs_vec
				.Count];
			Sharpen.Collections.ToArray(code_attrs_vec, attributes);
			return attributes;
		}

		/// <since>6.0</since>
		public virtual void AddAnnotationsAsAttribute(NBCEL.generic.ConstantPoolGen cp)
		{
			NBCEL.classfile.Attribute[] attrs = NBCEL.generic.AnnotationEntryGen.GetAnnotationAttributes
				(cp, base.GetAnnotationEntries());
			foreach (NBCEL.classfile.Attribute attr in attrs)
			{
				AddAttribute(attr);
			}
		}

		/// <since>6.0</since>
		public virtual void AddParameterAnnotationsAsAttribute(NBCEL.generic.ConstantPoolGen
			 cp)
		{
			if (!hasParameterAnnotations)
			{
				return;
			}
			NBCEL.classfile.Attribute[] attrs = NBCEL.generic.AnnotationEntryGen.GetParameterAnnotationAttributes
				(cp, param_annotations);
			if (attrs != null)
			{
				foreach (NBCEL.classfile.Attribute attr in attrs)
				{
					AddAttribute(attr);
				}
			}
		}

		/// <summary>Get method object.</summary>
		/// <remarks>
		/// Get method object. Never forget to call setMaxStack() or setMaxStack(max), respectively,
		/// before calling this method (the same applies for max locals).
		/// </remarks>
		/// <returns>method object</returns>
		public virtual NBCEL.classfile.Method GetMethod()
		{
			string signature = GetSignature();
			NBCEL.generic.ConstantPoolGen _cp = base.GetConstantPool();
			int name_index = _cp.AddUtf8(base.GetName());
			int signature_index = _cp.AddUtf8(signature);
			/* Also updates positions of instructions, i.e., their indices
			*/
			byte[] byte_code = null;
			if (il != null)
			{
				byte_code = il.GetByteCode();
			}
			NBCEL.classfile.LineNumberTable lnt = null;
			NBCEL.classfile.LocalVariableTable lvt = null;
			/* Create LocalVariableTable and LineNumberTable attributes (for debuggers, e.g.)
			*/
			if ((variable_vec.Count > 0) && !strip_attributes)
			{
				UpdateLocalVariableTable(GetLocalVariableTable(_cp));
				AddCodeAttribute(lvt = GetLocalVariableTable(_cp));
			}
			if (local_variable_type_table != null)
			{
				// LocalVariable length in LocalVariableTypeTable is not updated automatically. It's a difference with LocalVariableTable.
				if (lvt != null)
				{
					AdjustLocalVariableTypeTable(lvt);
				}
				AddCodeAttribute(local_variable_type_table);
			}
			if ((line_number_vec.Count > 0) && !strip_attributes)
			{
				AddCodeAttribute(lnt = GetLineNumberTable(_cp));
			}
			NBCEL.classfile.Attribute[] code_attrs = GetCodeAttributes();
			/* Each attribute causes 6 additional header bytes
			*/
			int attrs_len = 0;
			foreach (NBCEL.classfile.Attribute code_attr in code_attrs)
			{
				attrs_len += code_attr.GetLength() + 6;
			}
			NBCEL.classfile.CodeException[] c_exc = GetCodeExceptions();
			int exc_len = c_exc.Length * 8;
			// Every entry takes 8 bytes
			NBCEL.classfile.Code code = null;
			if ((il != null) && !IsAbstract() && !IsNative())
			{
				// Remove any stale code attribute
				NBCEL.classfile.Attribute[] attributes = GetAttributes();
				foreach (NBCEL.classfile.Attribute a in attributes)
				{
					if (a is NBCEL.classfile.Code)
					{
						RemoveAttribute(a);
					}
				}
				code = new NBCEL.classfile.Code(_cp.AddUtf8("Code"), 8 + byte_code.Length + 2 + exc_len
					 + 2 + attrs_len, max_stack, max_locals, byte_code, c_exc, code_attrs, _cp.GetConstantPool
					());
				// prologue byte code
				// exceptions
				// attributes
				AddAttribute(code);
			}
			AddAnnotationsAsAttribute(_cp);
			AddParameterAnnotationsAsAttribute(_cp);
			NBCEL.classfile.ExceptionTable et = null;
			if (throws_vec.Count > 0)
			{
				AddAttribute(et = GetExceptionTable(_cp));
			}
			// Add `Exceptions' if there are "throws" clauses
			NBCEL.classfile.Method m = new NBCEL.classfile.Method(base.GetAccessFlags(), name_index
				, signature_index, GetAttributes(), _cp.GetConstantPool());
			// Undo effects of adding attributes
			if (lvt != null)
			{
				RemoveCodeAttribute(lvt);
			}
			if (local_variable_type_table != null)
			{
				RemoveCodeAttribute(local_variable_type_table);
			}
			if (lnt != null)
			{
				RemoveCodeAttribute(lnt);
			}
			if (code != null)
			{
				RemoveAttribute(code);
			}
			if (et != null)
			{
				RemoveAttribute(et);
			}
			return m;
		}

		private void UpdateLocalVariableTable(NBCEL.classfile.LocalVariableTable a)
		{
			NBCEL.classfile.LocalVariable[] lv = a.GetLocalVariableTable();
			RemoveLocalVariables();
			foreach (NBCEL.classfile.LocalVariable l in lv)
			{
				NBCEL.generic.InstructionHandle start = il.FindHandle(l.GetStartPC());
				NBCEL.generic.InstructionHandle end = il.FindHandle(l.GetStartPC() + l.GetLength(
					));
				// Repair malformed handles
				if (null == start)
				{
					start = il.GetStart();
				}
				// end == null => live to end of method
				// Since we are recreating the LocalVaraible, we must
				// propagate the orig_index to new copy.
				AddLocalVariable(l.GetName(), NBCEL.generic.Type.GetType(l.GetSignature()), l.GetIndex
					(), start, end, l.GetOrigIndex());
			}
		}

		private void AdjustLocalVariableTypeTable(NBCEL.classfile.LocalVariableTable lvt)
		{
			NBCEL.classfile.LocalVariable[] lv = lvt.GetLocalVariableTable();
			NBCEL.classfile.LocalVariable[] lvg = local_variable_type_table.GetLocalVariableTypeTable
				();
			foreach (NBCEL.classfile.LocalVariable element in lvg)
			{
				foreach (NBCEL.classfile.LocalVariable l in lv)
				{
					if (element.GetName().Equals(l.GetName()) && element.GetIndex() == l.GetOrigIndex
						())
					{
						element.SetLength(l.GetLength());
						element.SetStartPC(l.GetStartPC());
						element.SetIndex(l.GetIndex());
						break;
					}
				}
			}
		}

		/// <summary>
		/// Remove all NOPs from the instruction list (if possible) and update every
		/// object referring to them, i.e., branch instructions, local variables and
		/// exception handlers.
		/// </summary>
		public virtual void RemoveNOPs()
		{
			if (il != null)
			{
				NBCEL.generic.InstructionHandle next;
				/* Check branch instructions.
				*/
				for (NBCEL.generic.InstructionHandle ih = il.GetStart(); ih != null; ih = next)
				{
					next = ih.GetNext();
					if ((next != null) && (ih.GetInstruction() is NBCEL.generic.NOP))
					{
						try
						{
							il.Delete(ih);
						}
						catch (NBCEL.generic.TargetLostException e)
						{
							foreach (NBCEL.generic.InstructionHandle target in e.GetTargets())
							{
								foreach (NBCEL.generic.InstructionTargeter targeter in target.GetTargeters())
								{
									targeter.UpdateTarget(target, next);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>Set maximum number of local variables.</summary>
		public virtual void SetMaxLocals(int m)
		{
			max_locals = m;
		}

		public virtual int GetMaxLocals()
		{
			return max_locals;
		}

		/// <summary>Set maximum stack size for this method.</summary>
		public virtual void SetMaxStack(int m)
		{
			// TODO could be package-protected?
			max_stack = m;
		}

		public virtual int GetMaxStack()
		{
			return max_stack;
		}

		/// <returns>class that contains this method</returns>
		public virtual string GetClassName()
		{
			return class_name;
		}

		public virtual void SetClassName(string class_name)
		{
			// TODO could be package-protected?
			this.class_name = class_name;
		}

		public virtual void SetReturnType(NBCEL.generic.Type return_type)
		{
			SetType(return_type);
		}

		public virtual NBCEL.generic.Type GetReturnType()
		{
			return GetType();
		}

		public virtual void SetArgumentTypes(NBCEL.generic.Type[] arg_types)
		{
			this.arg_types = arg_types;
		}

		public virtual NBCEL.generic.Type[] GetArgumentTypes()
		{
			return (Type[]) arg_types.Clone();
		}

		public virtual void SetArgumentType(int i, NBCEL.generic.Type type)
		{
			arg_types[i] = type;
		}

		public virtual NBCEL.generic.Type GetArgumentType(int i)
		{
			return arg_types[i];
		}

		public virtual void SetArgumentNames(string[] arg_names)
		{
			this.arg_names = arg_names;
		}

		public virtual string[] GetArgumentNames()
		{
			return (string[]) arg_names.Clone();
		}

		public virtual void SetArgumentName(int i, string name)
		{
			arg_names[i] = name;
		}

		public virtual string GetArgumentName(int i)
		{
			return arg_names[i];
		}

		public virtual NBCEL.generic.InstructionList GetInstructionList()
		{
			return il;
		}

		public virtual void SetInstructionList(NBCEL.generic.InstructionList il)
		{
			// TODO could be package-protected?
			this.il = il;
		}

		public override string GetSignature()
		{
			return NBCEL.generic.Type.GetMethodSignature(base.GetType(), arg_types);
		}

		/// <summary>Computes max.</summary>
		/// <remarks>Computes max. stack size by performing control flow analysis.</remarks>
		public virtual void SetMaxStack()
		{
			// TODO could be package-protected? (some tests would need repackaging)
			if (il != null)
			{
				max_stack = GetMaxStack(base.GetConstantPool(), il, GetExceptionHandlers());
			}
			else
			{
				max_stack = 0;
			}
		}

		/// <summary>Compute maximum number of local variables.</summary>
		public virtual void SetMaxLocals()
		{
			// TODO could be package-protected? (some tests would need repackaging)
			if (il != null)
			{
				int max = IsStatic() ? 0 : 1;
				if (arg_types != null)
				{
					foreach (NBCEL.generic.Type arg_type in arg_types)
					{
						max += arg_type.GetSize();
					}
				}
				for (NBCEL.generic.InstructionHandle ih = il.GetStart(); ih != null; ih = ih.GetNext
					())
				{
					NBCEL.generic.Instruction ins = ih.GetInstruction();
					if ((ins is NBCEL.generic.LocalVariableInstruction) || (ins is NBCEL.generic.RET)
						 || (ins is NBCEL.generic.IINC))
					{
						int index = ((NBCEL.generic.IndexedInstruction)ins).GetIndex() + ((NBCEL.generic.TypedInstruction
							)ins).GetType(base.GetConstantPool()).GetSize();
						if (index > max)
						{
							max = index;
						}
					}
				}
				max_locals = max;
			}
			else
			{
				max_locals = 0;
			}
		}

		/// <summary>
		/// Do not/Do produce attributes code attributesLineNumberTable and
		/// LocalVariableTable, like javac -O
		/// </summary>
		public virtual void StripAttributes(bool flag)
		{
			strip_attributes = flag;
		}

		internal sealed class BranchTarget
		{
			internal readonly NBCEL.generic.InstructionHandle target;

			internal readonly int stackDepth;

			internal BranchTarget(NBCEL.generic.InstructionHandle target, int stackDepth)
			{
				this.target = target;
				this.stackDepth = stackDepth;
			}
		}

		internal sealed class BranchStack
		{
			private readonly Stack<NBCEL.generic.MethodGen.BranchTarget> branchTargets
				 = new System.Collections.Generic.Stack<NBCEL.generic.MethodGen.BranchTarget>();

			private readonly Dictionary<NBCEL.generic.InstructionHandle, NBCEL.generic.MethodGen.BranchTarget
				> visitedTargets = new Dictionary<NBCEL.generic.InstructionHandle, NBCEL.generic.MethodGen.BranchTarget
				>();

			public void Push(NBCEL.generic.InstructionHandle target, int stackDepth)
			{
				if (Visited(target))
				{
					return;
				}
				branchTargets.Push(Visit(target, stackDepth));
			}

			public NBCEL.generic.MethodGen.BranchTarget Pop()
			{
				if (branchTargets.Count > 0)
				{
					NBCEL.generic.MethodGen.BranchTarget bt = branchTargets.Pop();
					return bt;
				}
				return null;
			}

			private NBCEL.generic.MethodGen.BranchTarget Visit(NBCEL.generic.InstructionHandle
				 target, int stackDepth)
			{
				NBCEL.generic.MethodGen.BranchTarget bt = new NBCEL.generic.MethodGen.BranchTarget
					(target, stackDepth);
				visitedTargets[target] = bt;
				return bt;
			}

			private bool Visited(NBCEL.generic.InstructionHandle target)
			{
				return visitedTargets.GetOrNull(target) != null;
			}
		}

		/// <summary>Computes stack usage of an instruction list by performing control flow analysis.
		/// 	</summary>
		/// <returns>maximum stack depth used by method</returns>
		public static int GetMaxStack(NBCEL.generic.ConstantPoolGen cp, NBCEL.generic.InstructionList
			 il, NBCEL.generic.CodeExceptionGen[] et)
		{
			NBCEL.generic.MethodGen.BranchStack branchTargets = new NBCEL.generic.MethodGen.BranchStack
				();
			/* Initially, populate the branch stack with the exception
			* handlers, because these aren't (necessarily) branched to
			* explicitly. in each case, the stack will have depth 1,
			* containing the exception object.
			*/
			foreach (NBCEL.generic.CodeExceptionGen element in et)
			{
				NBCEL.generic.InstructionHandle handler_pc = element.GetHandlerPC();
				if (handler_pc != null)
				{
					branchTargets.Push(handler_pc, 1);
				}
			}
			int stackDepth = 0;
			int maxStackDepth = 0;
			NBCEL.generic.InstructionHandle ih = il.GetStart();
			while (ih != null)
			{
				NBCEL.generic.Instruction instruction = ih.GetInstruction();
				short opcode = instruction.GetOpcode();
				int delta = instruction.ProduceStack(cp) - instruction.ConsumeStack(cp);
				stackDepth += delta;
				if (stackDepth > maxStackDepth)
				{
					maxStackDepth = stackDepth;
				}
				// choose the next instruction based on whether current is a branch.
				if (instruction is NBCEL.generic.BranchInstruction)
				{
					NBCEL.generic.BranchInstruction branch = (NBCEL.generic.BranchInstruction)instruction;
					if (instruction is NBCEL.generic.Select)
					{
						// explore all of the select's targets. the default target is handled below.
						NBCEL.generic.Select select = (NBCEL.generic.Select)branch;
						NBCEL.generic.InstructionHandle[] targets = select.GetTargets();
						foreach (NBCEL.generic.InstructionHandle target in targets)
						{
							branchTargets.Push(target, stackDepth);
						}
						// nothing to fall through to.
						ih = null;
					}
					else if (!(branch is NBCEL.generic.IfInstruction))
					{
						// if an instruction that comes back to following PC,
						// push next instruction, with stack depth reduced by 1.
						if (opcode == NBCEL.Const.JSR || opcode == NBCEL.Const.JSR_W)
						{
							branchTargets.Push(ih.GetNext(), stackDepth - 1);
						}
						ih = null;
					}
					// for all branches, the target of the branch is pushed on the branch stack.
					// conditional branches have a fall through case, selects don't, and
					// jsr/jsr_w return to the next instruction.
					branchTargets.Push(branch.GetTarget(), stackDepth);
				}
				else if (opcode == NBCEL.Const.ATHROW || opcode == NBCEL.Const.RET || (opcode >= 
					NBCEL.Const.IRETURN && opcode <= NBCEL.Const.RETURN))
				{
					// check for instructions that terminate the method.
					ih = null;
				}
				// normal case, go to the next instruction.
				if (ih != null)
				{
					ih = ih.GetNext();
				}
				// if we have no more instructions, see if there are any deferred branches to explore.
				if (ih == null)
				{
					NBCEL.generic.MethodGen.BranchTarget bt = branchTargets.Pop();
					if (bt != null)
					{
						ih = bt.target;
						stackDepth = bt.stackDepth;
					}
				}
			}
			return maxStackDepth;
		}

		private System.Collections.Generic.List<NBCEL.generic.MethodObserver> observers;

		/// <summary>Add observer for this object.</summary>
		public virtual void AddObserver(NBCEL.generic.MethodObserver o)
		{
			if (observers == null)
			{
				observers = new System.Collections.Generic.List<NBCEL.generic.MethodObserver>();
			}
			observers.Add(o);
		}

		/// <summary>Remove observer for this object.</summary>
		public virtual void RemoveObserver(NBCEL.generic.MethodObserver o)
		{
			if (observers != null)
			{
				observers.Remove(o);
			}
		}

		/// <summary>Call notify() method on all observers.</summary>
		/// <remarks>
		/// Call notify() method on all observers. This method is not called
		/// automatically whenever the state has changed, but has to be
		/// called by the user after he has finished editing the object.
		/// </remarks>
		public virtual void Update()
		{
			if (observers != null)
			{
				foreach (NBCEL.generic.MethodObserver observer in observers)
				{
					observer.Notify(this);
				}
			}
		}

		/// <summary>
		/// Return string representation close to declaration format,
		/// `public static void main(String[]) throws IOException', e.g.
		/// </summary>
		/// <returns>String representation of the method.</returns>
		public sealed override string ToString()
		{
			string access = NBCEL.classfile.Utility.AccessToString(base.GetAccessFlags());
			string signature = NBCEL.generic.Type.GetMethodSignature(base.GetType(), arg_types
				);
			signature = NBCEL.classfile.Utility.MethodSignatureToString(signature, base.GetName
				(), access, true, GetLocalVariableTable(base.GetConstantPool()));
			System.Text.StringBuilder buf = new System.Text.StringBuilder(signature);
			foreach (NBCEL.classfile.Attribute a in GetAttributes())
			{
				if (!((a is NBCEL.classfile.Code) || (a is NBCEL.classfile.ExceptionTable)))
				{
					buf.Append(" [").Append(a).Append("]");
				}
			}
			if (throws_vec.Count > 0)
			{
				foreach (string throwsDescriptor in throws_vec)
				{
					buf.Append("\n\t\tthrows ").Append(throwsDescriptor);
				}
			}
			return buf.ToString();
		}

		/// <returns>deep copy of this method</returns>
		public virtual NBCEL.generic.MethodGen Copy(string class_name, NBCEL.generic.ConstantPoolGen
			 cp)
		{
			NBCEL.classfile.Method m = ((NBCEL.generic.MethodGen)Clone()).GetMethod();
			NBCEL.generic.MethodGen mg = new NBCEL.generic.MethodGen(m, class_name, base.GetConstantPool
				());
			if (base.GetConstantPool() != cp)
			{
				mg.SetConstantPool(cp);
				mg.GetInstructionList().ReplaceConstantPool(base.GetConstantPool(), cp);
			}
			return mg;
		}

		//J5TODO: Should param_annotations be an array of arrays? Rather than an array of lists, this
		// is more likely to suggest to the caller it is readonly (which a List does not).
		/// <summary>Return a list of AnnotationGen objects representing parameter annotations
		/// 	</summary>
		/// <since>6.0</since>
		public virtual System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen> 
			GetAnnotationsOnParameter(int i)
		{
			EnsureExistingParameterAnnotationsUnpacked();
			if (!hasParameterAnnotations || i > arg_types.Length)
			{
				return null;
			}
			return param_annotations[i];
		}

		/// <summary>
		/// Goes through the attributes on the method and identifies any that are
		/// RuntimeParameterAnnotations, extracting their contents and storing them
		/// as parameter annotations.
		/// </summary>
		/// <remarks>
		/// Goes through the attributes on the method and identifies any that are
		/// RuntimeParameterAnnotations, extracting their contents and storing them
		/// as parameter annotations. There are two kinds of parameter annotation -
		/// visible and invisible. Once they have been unpacked, these attributes are
		/// deleted. (The annotations will be rebuilt as attributes when someone
		/// builds a Method object out of this MethodGen object).
		/// </remarks>
		private void EnsureExistingParameterAnnotationsUnpacked()
		{
			if (haveUnpackedParameterAnnotations)
			{
				return;
			}
			// Find attributes that contain parameter annotation data
			NBCEL.classfile.Attribute[] attrs = GetAttributes();
			NBCEL.classfile.ParameterAnnotations paramAnnVisAttr = null;
			NBCEL.classfile.ParameterAnnotations paramAnnInvisAttr = null;
			foreach (NBCEL.classfile.Attribute attribute in attrs)
			{
				if (attribute is NBCEL.classfile.ParameterAnnotations)
				{
					// Initialize param_annotations
					if (!hasParameterAnnotations)
					{
						System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen>[] parmList = new 
							System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen>[arg_types.Length];
						// OK
						param_annotations = parmList;
						for (int j = 0; j < arg_types.Length; j++)
						{
							param_annotations[j] = new System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen
								>();
						}
					}
					hasParameterAnnotations = true;
					NBCEL.classfile.ParameterAnnotations rpa = (NBCEL.classfile.ParameterAnnotations)
						attribute;
					if (rpa is NBCEL.classfile.RuntimeVisibleParameterAnnotations)
					{
						paramAnnVisAttr = rpa;
					}
					else
					{
						paramAnnInvisAttr = rpa;
					}
					NBCEL.classfile.ParameterAnnotationEntry[] parameterAnnotationEntries = rpa.GetParameterAnnotationEntries
						();
					for (int j = 0; j < parameterAnnotationEntries.Length; j++)
					{
						// This returns Annotation[] ...
						NBCEL.classfile.ParameterAnnotationEntry immutableArray = rpa.GetParameterAnnotationEntries
							()[j];
						// ... which needs transforming into an AnnotationGen[] ...
						System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen> mutable = MakeMutableVersion
							(immutableArray.GetAnnotationEntries());
						// ... then add these to any we already know about
						Sharpen.Collections.AddAll(param_annotations[j], mutable);
					}
				}
			}
			if (paramAnnVisAttr != null)
			{
				RemoveAttribute(paramAnnVisAttr);
			}
			if (paramAnnInvisAttr != null)
			{
				RemoveAttribute(paramAnnInvisAttr);
			}
			haveUnpackedParameterAnnotations = true;
		}

		private System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen> MakeMutableVersion
			(NBCEL.classfile.AnnotationEntry[] mutableArray)
		{
			System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen> result = new System.Collections.Generic.List
				<NBCEL.generic.AnnotationEntryGen>();
			foreach (NBCEL.classfile.AnnotationEntry element in mutableArray)
			{
				result.Add(new NBCEL.generic.AnnotationEntryGen(element, GetConstantPool(), false
					));
			}
			return result;
		}

		public virtual void AddParameterAnnotation(int parameterIndex, NBCEL.generic.AnnotationEntryGen
			 annotation)
		{
			EnsureExistingParameterAnnotationsUnpacked();
			if (!hasParameterAnnotations)
			{
				System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen>[] parmList = new 
					System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen>[arg_types.Length];
				// OK
				param_annotations = parmList;
				hasParameterAnnotations = true;
			}
			System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen> existingAnnotations
				 = param_annotations[parameterIndex];
			if (existingAnnotations != null)
			{
				existingAnnotations.Add(annotation);
			}
			else
			{
				System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen> l = new System.Collections.Generic.List
					<NBCEL.generic.AnnotationEntryGen>();
				l.Add(annotation);
				param_annotations[parameterIndex] = l;
			}
		}

		/// <returns>Comparison strategy object</returns>
		public static NBCEL.util.BCELComparator GetComparator()
		{
			return bcelComparator;
		}

		/// <param name="comparator">Comparison strategy object</param>
		public static void SetComparator(NBCEL.util.BCELComparator comparator)
		{
			bcelComparator = comparator;
		}

		/// <summary>Return value as defined by given BCELComparator strategy.</summary>
		/// <remarks>
		/// Return value as defined by given BCELComparator strategy.
		/// By default two MethodGen objects are said to be equal when
		/// their names and signatures are equal.
		/// </remarks>
		/// <seealso cref="object.Equals(object)"/>
		public override bool Equals(object obj)
		{
			return bcelComparator.Equals(this, obj);
		}

		/// <summary>Return value as defined by given BCELComparator strategy.</summary>
		/// <remarks>
		/// Return value as defined by given BCELComparator strategy.
		/// By default return the hashcode of the method's name XOR signature.
		/// </remarks>
		/// <seealso cref="object.GetHashCode()"/>
		public override int GetHashCode()
		{
			return bcelComparator.HashCode(this);
		}
	}
}
