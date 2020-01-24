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

namespace Apache.NBCEL.ClassFile
{
	/// <summary>
	///     Traverses a JavaClass with another Visitor object 'piggy-backed' that is
	///     applied to all components of a JavaClass object.
	/// </summary>
	/// <remarks>
	///     Traverses a JavaClass with another Visitor object 'piggy-backed' that is
	///     applied to all components of a JavaClass object. I.e. this class supplies the
	///     traversal strategy, other classes can make use of it.
	/// </remarks>
	public class DescendingVisitor : Visitor
    {
        private readonly JavaClass clazz;

        private readonly Stack<object> stack = new Stack<object>();

        private readonly Visitor visitor;

        /// <param name="clazz">Class to traverse</param>
        /// <param name="visitor">visitor object to apply to all components</param>
        public DescendingVisitor(JavaClass clazz, Visitor
            visitor)
        {
            this.clazz = clazz;
            this.visitor = visitor;
        }

        public virtual void VisitJavaClass(JavaClass _clazz)
        {
            stack.Push(_clazz);
            _clazz.Accept(visitor);
            var fields = _clazz.GetFields();
            foreach (var field in fields) field.Accept(this);
            var methods = _clazz.GetMethods();
            foreach (var method in methods) method.Accept(this);
            var attributes = _clazz.GetAttributes();
            foreach (var attribute in attributes) attribute.Accept(this);
            _clazz.GetConstantPool().Accept(this);
            stack.Pop();
        }

        /// <since>6.0</since>
        public virtual void VisitAnnotation(Annotations annotation)
        {
            stack.Push(annotation);
            annotation.Accept(visitor);
            var entries = annotation.GetAnnotationEntries();
            foreach (var entrie in entries) entrie.Accept(this);
            stack.Pop();
        }

        /// <since>6.0</since>
        public virtual void VisitAnnotationEntry(AnnotationEntry annotationEntry
        )
        {
            stack.Push(annotationEntry);
            annotationEntry.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitField(Field field)
        {
            stack.Push(field);
            field.Accept(visitor);
            var attributes = field.GetAttributes();
            foreach (var attribute in attributes) attribute.Accept(this);
            stack.Pop();
        }

        public virtual void VisitConstantValue(ConstantValue cv)
        {
            stack.Push(cv);
            cv.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitMethod(Method method)
        {
            stack.Push(method);
            method.Accept(visitor);
            var attributes = method.GetAttributes();
            foreach (var attribute in attributes) attribute.Accept(this);
            stack.Pop();
        }

        public virtual void VisitExceptionTable(ExceptionTable table)
        {
            stack.Push(table);
            table.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitCode(Code code)
        {
            stack.Push(code);
            code.Accept(visitor);
            var table = code.GetExceptionTable();
            foreach (var element in table) element.Accept(this);
            var attributes = code.GetAttributes();
            foreach (var attribute in attributes) attribute.Accept(this);
            stack.Pop();
        }

        public virtual void VisitCodeException(CodeException ce)
        {
            stack.Push(ce);
            ce.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitLineNumberTable(LineNumberTable table)
        {
            stack.Push(table);
            table.Accept(visitor);
            var numbers = table.GetLineNumberTable();
            foreach (var number in numbers) number.Accept(this);
            stack.Pop();
        }

        public virtual void VisitLineNumber(LineNumber number)
        {
            stack.Push(number);
            number.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitLocalVariableTable(LocalVariableTable table
        )
        {
            stack.Push(table);
            table.Accept(visitor);
            var vars = table.GetLocalVariableTable();
            foreach (var var in vars) var.Accept(this);
            stack.Pop();
        }

        public virtual void VisitStackMap(StackMap table)
        {
            stack.Push(table);
            table.Accept(visitor);
            var vars = table.GetStackMap();
            foreach (var var in vars) var.Accept(this);
            stack.Pop();
        }

        public virtual void VisitStackMapEntry(StackMapEntry var)
        {
            stack.Push(var);
            var.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitLocalVariable(LocalVariable var)
        {
            stack.Push(var);
            var.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitConstantPool(ConstantPool cp)
        {
            stack.Push(cp);
            cp.Accept(visitor);
            var constants = cp.GetConstantPool();
            for (var i = 1; i < constants.Length; i++)
                if (constants[i] != null)
                    constants[i].Accept(this);
            stack.Pop();
        }

        public virtual void VisitConstantClass(ConstantClass constant)
        {
            stack.Push(constant);
            constant.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitConstantDouble(ConstantDouble constant)
        {
            stack.Push(constant);
            constant.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitConstantFieldref(ConstantFieldref constant
        )
        {
            stack.Push(constant);
            constant.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitConstantFloat(ConstantFloat constant)
        {
            stack.Push(constant);
            constant.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitConstantInteger(ConstantInteger constant
        )
        {
            stack.Push(constant);
            constant.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitConstantInterfaceMethodref(ConstantInterfaceMethodref
            constant)
        {
            stack.Push(constant);
            constant.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.0</since>
        public virtual void VisitConstantInvokeDynamic(ConstantInvokeDynamic
            constant)
        {
            stack.Push(constant);
            constant.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitConstantLong(ConstantLong constant)
        {
            stack.Push(constant);
            constant.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitConstantMethodref(ConstantMethodref constant
        )
        {
            stack.Push(constant);
            constant.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitConstantNameAndType(ConstantNameAndType
            constant)
        {
            stack.Push(constant);
            constant.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitConstantString(ConstantString constant)
        {
            stack.Push(constant);
            constant.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitConstantUtf8(ConstantUtf8 constant)
        {
            stack.Push(constant);
            constant.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitInnerClasses(InnerClasses ic)
        {
            stack.Push(ic);
            ic.Accept(visitor);
            var ics = ic.GetInnerClasses();
            foreach (var ic2 in ics) ic2.Accept(this);
            stack.Pop();
        }

        public virtual void VisitInnerClass(InnerClass inner)
        {
            stack.Push(inner);
            inner.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.0</since>
        public virtual void VisitBootstrapMethods(BootstrapMethods bm)
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

        public virtual void VisitDeprecated(Deprecated attribute)
        {
            stack.Push(attribute);
            attribute.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitSignature(Signature attribute)
        {
            stack.Push(attribute);
            attribute.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitSourceFile(SourceFile attribute)
        {
            stack.Push(attribute);
            attribute.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitSynthetic(Synthetic attribute)
        {
            stack.Push(attribute);
            attribute.Accept(visitor);
            stack.Pop();
        }

        public virtual void VisitUnknown(Unknown attribute)
        {
            stack.Push(attribute);
            attribute.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.0</since>
        public virtual void VisitAnnotationDefault(AnnotationDefault obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.0</since>
        public virtual void VisitEnclosingMethod(EnclosingMethod obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.0</since>
        public virtual void VisitLocalVariableTypeTable(LocalVariableTypeTable
            obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.0</since>
        public virtual void VisitParameterAnnotation(ParameterAnnotations
            obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.0</since>
        public virtual void VisitMethodParameters(MethodParameters obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            var table = obj.GetParameters();
            foreach (var element in table) element.Accept(this);
            stack.Pop();
        }

        /// <since>6.4.0</since>
        public virtual void VisitMethodParameter(MethodParameter obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.0</since>
        public virtual void VisitConstantMethodType(ConstantMethodType obj
        )
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.0</since>
        public virtual void VisitConstantMethodHandle(ConstantMethodHandle
            obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.0</since>
        public virtual void VisitParameterAnnotationEntry(ParameterAnnotationEntry
            obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.1</since>
        public virtual void VisitConstantPackage(ConstantPackage obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.1</since>
        public virtual void VisitConstantModule(ConstantModule obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.3</since>
        public virtual void VisitConstantDynamic(ConstantDynamic obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.4.0</since>
        public virtual void VisitModule(Module obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            var rtable = obj.GetRequiresTable();
            foreach (var element in rtable) element.Accept(this);
            var etable = obj.GetExportsTable();
            foreach (var element in etable) element.Accept(this);
            var otable = obj.GetOpensTable();
            foreach (var element in otable) element.Accept(this);
            var ptable = obj.GetProvidesTable();
            foreach (var element in ptable) element.Accept(this);
            stack.Pop();
        }

        /// <since>6.4.0</since>
        public virtual void VisitModuleRequires(ModuleRequires obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.4.0</since>
        public virtual void VisitModuleExports(ModuleExports obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.4.0</since>
        public virtual void VisitModuleOpens(ModuleOpens obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.4.0</since>
        public virtual void VisitModuleProvides(ModuleProvides obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.4.0</since>
        public virtual void VisitModulePackages(ModulePackages obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.4.0</since>
        public virtual void VisitModuleMainClass(ModuleMainClass obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.4.0</since>
        public virtual void VisitNestHost(NestHost obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <since>6.4.0</since>
        public virtual void VisitNestMembers(NestMembers obj)
        {
            stack.Push(obj);
            obj.Accept(visitor);
            stack.Pop();
        }

        /// <returns>container of current entitity, i.e., predecessor during traversal</returns>
        public virtual object Predecessor()
        {
            return Predecessor(0);
        }

        /// <param name="level">nesting level, i.e., 0 returns the direct predecessor</param>
        /// <returns>container of current entitity, i.e., predecessor during traversal</returns>
        public virtual object Predecessor(int level)
        {
            var size = stack.Count;
            if (size < 2 || level < 0) return null;
            return stack.ToArray()[size - (level + 2)];
        }

        // size - 1 == current
        /// <returns>current object</returns>
        public virtual object Current()
        {
            return stack.Peek();
        }

        /// <summary>Start traversal.</summary>
        public virtual void Visit()
        {
            clazz.Accept(this);
        }
    }
}