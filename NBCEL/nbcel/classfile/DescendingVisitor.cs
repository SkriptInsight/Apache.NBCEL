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

using System.Collections.Generic;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	/// Traverses a JavaClass with another Visitor object 'piggy-backed' that is
	/// applied to all components of a JavaClass object.
	/// </summary>
	/// <remarks>
	/// Traverses a JavaClass with another Visitor object 'piggy-backed' that is
	/// applied to all components of a JavaClass object. I.e. this class supplies the
	/// traversal strategy, other classes can make use of it.
	/// </remarks>
	public class DescendingVisitor : NBCEL.classfile.Visitor
	{
		private readonly NBCEL.classfile.JavaClass clazz;

		private readonly NBCEL.classfile.Visitor visitor;

		private readonly Stack<object> stack = new Stack<object>();

		/// <returns>container of current entitity, i.e., predecessor during traversal</returns>
		public virtual object Predecessor()
		{
			return Predecessor(0);
		}

		/// <param name="level">nesting level, i.e., 0 returns the direct predecessor</param>
		/// <returns>container of current entitity, i.e., predecessor during traversal</returns>
		public virtual object Predecessor(int level)
		{
			int size = stack.Count;
			if ((size < 2) || (level < 0))
			{
				return null;
			}
			return stack.ToArray()[size - (level + 2)];
		}

		// size - 1 == current
		/// <returns>current object</returns>
		public virtual object Current()
		{
			return stack.Peek();
		}

		/// <param name="clazz">Class to traverse</param>
		/// <param name="visitor">visitor object to apply to all components</param>
		public DescendingVisitor(NBCEL.classfile.JavaClass clazz, NBCEL.classfile.Visitor
			 visitor)
		{
			this.clazz = clazz;
			this.visitor = visitor;
		}

		/// <summary>Start traversal.</summary>
		public virtual void Visit()
		{
			clazz.Accept(this);
		}

		public virtual void VisitJavaClass(NBCEL.classfile.JavaClass _clazz)
		{
			stack.Push(_clazz);
			_clazz.Accept(visitor);
			NBCEL.classfile.Field[] fields = _clazz.GetFields();
			foreach (NBCEL.classfile.Field field in fields)
			{
				field.Accept(this);
			}
			NBCEL.classfile.Method[] methods = _clazz.GetMethods();
			foreach (NBCEL.classfile.Method method in methods)
			{
				method.Accept(this);
			}
			NBCEL.classfile.Attribute[] attributes = _clazz.GetAttributes();
			foreach (NBCEL.classfile.Attribute attribute in attributes)
			{
				attribute.Accept(this);
			}
			_clazz.GetConstantPool().Accept(this);
			stack.Pop();
		}

		/// <since>6.0</since>
		public virtual void VisitAnnotation(NBCEL.classfile.Annotations annotation)
		{
			stack.Push(annotation);
			annotation.Accept(visitor);
			NBCEL.classfile.AnnotationEntry[] entries = annotation.GetAnnotationEntries();
			foreach (NBCEL.classfile.AnnotationEntry entrie in entries)
			{
				entrie.Accept(this);
			}
			stack.Pop();
		}

		/// <since>6.0</since>
		public virtual void VisitAnnotationEntry(NBCEL.classfile.AnnotationEntry annotationEntry
			)
		{
			stack.Push(annotationEntry);
			annotationEntry.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitField(NBCEL.classfile.Field field)
		{
			stack.Push(field);
			field.Accept(visitor);
			NBCEL.classfile.Attribute[] attributes = field.GetAttributes();
			foreach (NBCEL.classfile.Attribute attribute in attributes)
			{
				attribute.Accept(this);
			}
			stack.Pop();
		}

		public virtual void VisitConstantValue(NBCEL.classfile.ConstantValue cv)
		{
			stack.Push(cv);
			cv.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitMethod(NBCEL.classfile.Method method)
		{
			stack.Push(method);
			method.Accept(visitor);
			NBCEL.classfile.Attribute[] attributes = method.GetAttributes();
			foreach (NBCEL.classfile.Attribute attribute in attributes)
			{
				attribute.Accept(this);
			}
			stack.Pop();
		}

		public virtual void VisitExceptionTable(NBCEL.classfile.ExceptionTable table)
		{
			stack.Push(table);
			table.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitCode(NBCEL.classfile.Code code)
		{
			stack.Push(code);
			code.Accept(visitor);
			NBCEL.classfile.CodeException[] table = code.GetExceptionTable();
			foreach (NBCEL.classfile.CodeException element in table)
			{
				element.Accept(this);
			}
			NBCEL.classfile.Attribute[] attributes = code.GetAttributes();
			foreach (NBCEL.classfile.Attribute attribute in attributes)
			{
				attribute.Accept(this);
			}
			stack.Pop();
		}

		public virtual void VisitCodeException(NBCEL.classfile.CodeException ce)
		{
			stack.Push(ce);
			ce.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitLineNumberTable(NBCEL.classfile.LineNumberTable table)
		{
			stack.Push(table);
			table.Accept(visitor);
			NBCEL.classfile.LineNumber[] numbers = table.GetLineNumberTable();
			foreach (NBCEL.classfile.LineNumber number in numbers)
			{
				number.Accept(this);
			}
			stack.Pop();
		}

		public virtual void VisitLineNumber(NBCEL.classfile.LineNumber number)
		{
			stack.Push(number);
			number.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitLocalVariableTable(NBCEL.classfile.LocalVariableTable table
			)
		{
			stack.Push(table);
			table.Accept(visitor);
			NBCEL.classfile.LocalVariable[] vars = table.GetLocalVariableTable();
			foreach (NBCEL.classfile.LocalVariable var in vars)
			{
				var.Accept(this);
			}
			stack.Pop();
		}

		public virtual void VisitStackMap(NBCEL.classfile.StackMap table)
		{
			stack.Push(table);
			table.Accept(visitor);
			NBCEL.classfile.StackMapEntry[] vars = table.GetStackMap();
			foreach (NBCEL.classfile.StackMapEntry var in vars)
			{
				var.Accept(this);
			}
			stack.Pop();
		}

		public virtual void VisitStackMapEntry(NBCEL.classfile.StackMapEntry var)
		{
			stack.Push(var);
			var.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitLocalVariable(NBCEL.classfile.LocalVariable var)
		{
			stack.Push(var);
			var.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitConstantPool(NBCEL.classfile.ConstantPool cp)
		{
			stack.Push(cp);
			cp.Accept(visitor);
			NBCEL.classfile.Constant[] constants = cp.GetConstantPool();
			for (int i = 1; i < constants.Length; i++)
			{
				if (constants[i] != null)
				{
					constants[i].Accept(this);
				}
			}
			stack.Pop();
		}

		public virtual void VisitConstantClass(NBCEL.classfile.ConstantClass constant)
		{
			stack.Push(constant);
			constant.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitConstantDouble(NBCEL.classfile.ConstantDouble constant)
		{
			stack.Push(constant);
			constant.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitConstantFieldref(NBCEL.classfile.ConstantFieldref constant
			)
		{
			stack.Push(constant);
			constant.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitConstantFloat(NBCEL.classfile.ConstantFloat constant)
		{
			stack.Push(constant);
			constant.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitConstantInteger(NBCEL.classfile.ConstantInteger constant
			)
		{
			stack.Push(constant);
			constant.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitConstantInterfaceMethodref(NBCEL.classfile.ConstantInterfaceMethodref
			 constant)
		{
			stack.Push(constant);
			constant.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.0</since>
		public virtual void VisitConstantInvokeDynamic(NBCEL.classfile.ConstantInvokeDynamic
			 constant)
		{
			stack.Push(constant);
			constant.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitConstantLong(NBCEL.classfile.ConstantLong constant)
		{
			stack.Push(constant);
			constant.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitConstantMethodref(NBCEL.classfile.ConstantMethodref constant
			)
		{
			stack.Push(constant);
			constant.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitConstantNameAndType(NBCEL.classfile.ConstantNameAndType 
			constant)
		{
			stack.Push(constant);
			constant.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitConstantString(NBCEL.classfile.ConstantString constant)
		{
			stack.Push(constant);
			constant.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitConstantUtf8(NBCEL.classfile.ConstantUtf8 constant)
		{
			stack.Push(constant);
			constant.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitInnerClasses(NBCEL.classfile.InnerClasses ic)
		{
			stack.Push(ic);
			ic.Accept(visitor);
			NBCEL.classfile.InnerClass[] ics = ic.GetInnerClasses();
			foreach (NBCEL.classfile.InnerClass ic2 in ics)
			{
				ic2.Accept(this);
			}
			stack.Pop();
		}

		public virtual void VisitInnerClass(NBCEL.classfile.InnerClass inner)
		{
			stack.Push(inner);
			inner.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.0</since>
		public virtual void VisitBootstrapMethods(NBCEL.classfile.BootstrapMethods bm)
		{
			stack.Push(bm);
			bm.Accept(visitor);
			// BootstrapMethod[] bms = bm.getBootstrapMethods();
			// for (int i = 0; i < bms.length; i++)
			// {
			//     bms[i].accept(this);
			// }
			stack.Pop();
		}

		public virtual void VisitDeprecated(NBCEL.classfile.Deprecated attribute)
		{
			stack.Push(attribute);
			attribute.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitSignature(NBCEL.classfile.Signature attribute)
		{
			stack.Push(attribute);
			attribute.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitSourceFile(NBCEL.classfile.SourceFile attribute)
		{
			stack.Push(attribute);
			attribute.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitSynthetic(NBCEL.classfile.Synthetic attribute)
		{
			stack.Push(attribute);
			attribute.Accept(visitor);
			stack.Pop();
		}

		public virtual void VisitUnknown(NBCEL.classfile.Unknown attribute)
		{
			stack.Push(attribute);
			attribute.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.0</since>
		public virtual void VisitAnnotationDefault(NBCEL.classfile.AnnotationDefault obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.0</since>
		public virtual void VisitEnclosingMethod(NBCEL.classfile.EnclosingMethod obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.0</since>
		public virtual void VisitLocalVariableTypeTable(NBCEL.classfile.LocalVariableTypeTable
			 obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.0</since>
		public virtual void VisitParameterAnnotation(NBCEL.classfile.ParameterAnnotations
			 obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.0</since>
		public virtual void VisitMethodParameters(NBCEL.classfile.MethodParameters obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			NBCEL.classfile.MethodParameter[] table = obj.GetParameters();
			foreach (NBCEL.classfile.MethodParameter element in table)
			{
				element.Accept(this);
			}
			stack.Pop();
		}

		/// <since>6.4.0</since>
		public virtual void VisitMethodParameter(NBCEL.classfile.MethodParameter obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.0</since>
		public virtual void VisitConstantMethodType(NBCEL.classfile.ConstantMethodType obj
			)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.0</since>
		public virtual void VisitConstantMethodHandle(NBCEL.classfile.ConstantMethodHandle
			 obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.0</since>
		public virtual void VisitParameterAnnotationEntry(NBCEL.classfile.ParameterAnnotationEntry
			 obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.1</since>
		public virtual void VisitConstantPackage(NBCEL.classfile.ConstantPackage obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.1</since>
		public virtual void VisitConstantModule(NBCEL.classfile.ConstantModule obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.3</since>
		public virtual void VisitConstantDynamic(NBCEL.classfile.ConstantDynamic obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.4.0</since>
		public virtual void VisitModule(NBCEL.classfile.Module obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			NBCEL.classfile.ModuleRequires[] rtable = obj.GetRequiresTable();
			foreach (NBCEL.classfile.ModuleRequires element in rtable)
			{
				element.Accept(this);
			}
			NBCEL.classfile.ModuleExports[] etable = obj.GetExportsTable();
			foreach (NBCEL.classfile.ModuleExports element in etable)
			{
				element.Accept(this);
			}
			NBCEL.classfile.ModuleOpens[] otable = obj.GetOpensTable();
			foreach (NBCEL.classfile.ModuleOpens element in otable)
			{
				element.Accept(this);
			}
			NBCEL.classfile.ModuleProvides[] ptable = obj.GetProvidesTable();
			foreach (NBCEL.classfile.ModuleProvides element in ptable)
			{
				element.Accept(this);
			}
			stack.Pop();
		}

		/// <since>6.4.0</since>
		public virtual void VisitModuleRequires(NBCEL.classfile.ModuleRequires obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.4.0</since>
		public virtual void VisitModuleExports(NBCEL.classfile.ModuleExports obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.4.0</since>
		public virtual void VisitModuleOpens(NBCEL.classfile.ModuleOpens obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.4.0</since>
		public virtual void VisitModuleProvides(NBCEL.classfile.ModuleProvides obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.4.0</since>
		public virtual void VisitModulePackages(NBCEL.classfile.ModulePackages obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.4.0</since>
		public virtual void VisitModuleMainClass(NBCEL.classfile.ModuleMainClass obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.4.0</since>
		public virtual void VisitNestHost(NBCEL.classfile.NestHost obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}

		/// <since>6.4.0</since>
		public virtual void VisitNestMembers(NBCEL.classfile.NestMembers obj)
		{
			stack.Push(obj);
			obj.Accept(visitor);
			stack.Pop();
		}
	}
}
