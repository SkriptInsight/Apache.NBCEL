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

namespace NBCEL.classfile
{
	/// <summary>
	///     Visitor with empty method bodies, can be extended and used in conjunction
	///     with the DescendingVisitor class, e.g.
	/// </summary>
	/// <remarks>
	///     Visitor with empty method bodies, can be extended and used in conjunction
	///     with the DescendingVisitor class, e.g. By courtesy of David Spencer.
	/// </remarks>
	/// <seealso cref="DescendingVisitor" />
	public class EmptyVisitor : Visitor
    {
        protected internal EmptyVisitor()
        {
        }

        /// <since>6.0</since>
        public virtual void VisitAnnotation(Annotations obj)
        {
        }

        /// <since>6.0</since>
        public virtual void VisitParameterAnnotation(ParameterAnnotations
            obj)
        {
        }

        /// <since>6.0</since>
        public virtual void VisitAnnotationEntry(AnnotationEntry obj)
        {
        }

        /// <since>6.0</since>
        public virtual void VisitAnnotationDefault(AnnotationDefault obj)
        {
        }

        public virtual void VisitCode(Code obj)
        {
        }

        public virtual void VisitCodeException(CodeException obj)
        {
        }

        public virtual void VisitConstantClass(ConstantClass obj)
        {
        }

        public virtual void VisitConstantDouble(ConstantDouble obj)
        {
        }

        public virtual void VisitConstantFieldref(ConstantFieldref obj)
        {
        }

        public virtual void VisitConstantFloat(ConstantFloat obj)
        {
        }

        public virtual void VisitConstantInteger(ConstantInteger obj)
        {
        }

        public virtual void VisitConstantInterfaceMethodref(ConstantInterfaceMethodref
            obj)
        {
        }

        public virtual void VisitConstantInvokeDynamic(ConstantInvokeDynamic
            obj)
        {
        }

        public virtual void VisitConstantLong(ConstantLong obj)
        {
        }

        public virtual void VisitConstantMethodref(ConstantMethodref obj)
        {
        }

        public virtual void VisitConstantNameAndType(ConstantNameAndType
            obj)
        {
        }

        public virtual void VisitConstantPool(ConstantPool obj)
        {
        }

        public virtual void VisitConstantString(ConstantString obj)
        {
        }

        public virtual void VisitConstantUtf8(ConstantUtf8 obj)
        {
        }

        public virtual void VisitConstantValue(ConstantValue obj)
        {
        }

        public virtual void VisitDeprecated(Deprecated obj)
        {
        }

        public virtual void VisitExceptionTable(ExceptionTable obj)
        {
        }

        public virtual void VisitField(Field obj)
        {
        }

        public virtual void VisitInnerClass(InnerClass obj)
        {
        }

        public virtual void VisitInnerClasses(InnerClasses obj)
        {
        }

        /// <since>6.0</since>
        public virtual void VisitBootstrapMethods(BootstrapMethods obj)
        {
        }

        public virtual void VisitJavaClass(JavaClass obj)
        {
        }

        public virtual void VisitLineNumber(LineNumber obj)
        {
        }

        public virtual void VisitLineNumberTable(LineNumberTable obj)
        {
        }

        public virtual void VisitLocalVariable(LocalVariable obj)
        {
        }

        public virtual void VisitLocalVariableTable(LocalVariableTable obj
        )
        {
        }

        public virtual void VisitMethod(Method obj)
        {
        }

        public virtual void VisitSignature(Signature obj)
        {
        }

        public virtual void VisitSourceFile(SourceFile obj)
        {
        }

        public virtual void VisitSynthetic(Synthetic obj)
        {
        }

        public virtual void VisitUnknown(Unknown obj)
        {
        }

        public virtual void VisitStackMap(StackMap obj)
        {
        }

        public virtual void VisitStackMapEntry(StackMapEntry obj)
        {
        }

        /// <since>6.0</since>
        public virtual void VisitEnclosingMethod(EnclosingMethod obj)
        {
        }

        /// <since>6.0</since>
        public virtual void VisitLocalVariableTypeTable(LocalVariableTypeTable
            obj)
        {
        }

        /// <since>6.0</since>
        public virtual void VisitMethodParameters(MethodParameters obj)
        {
        }

        /// <since>6.4.0</since>
        public virtual void VisitMethodParameter(MethodParameter obj)
        {
        }

        /// <since>6.0</since>
        public virtual void VisitConstantMethodType(ConstantMethodType obj
        )
        {
        }

        /// <since>6.0</since>
        public virtual void VisitConstantMethodHandle(ConstantMethodHandle
            constantMethodHandle)
        {
        }

        /// <since>6.0</since>
        public virtual void VisitParameterAnnotationEntry(ParameterAnnotationEntry
            parameterAnnotationEntry)
        {
        }

        /// <since>6.1</since>
        public virtual void VisitConstantPackage(ConstantPackage constantPackage
        )
        {
        }

        /// <since>6.1</since>
        public virtual void VisitConstantModule(ConstantModule constantModule
        )
        {
        }

        /// <since>6.3</since>
        public virtual void VisitConstantDynamic(ConstantDynamic obj)
        {
        }

        /// <since>6.4.0</since>
        public virtual void VisitModule(Module obj)
        {
        }

        /// <since>6.4.0</since>
        public virtual void VisitModuleRequires(ModuleRequires obj)
        {
        }

        /// <since>6.4.0</since>
        public virtual void VisitModuleExports(ModuleExports obj)
        {
        }

        /// <since>6.4.0</since>
        public virtual void VisitModuleOpens(ModuleOpens obj)
        {
        }

        /// <since>6.4.0</since>
        public virtual void VisitModuleProvides(ModuleProvides obj)
        {
        }

        /// <since>6.4.0</since>
        public virtual void VisitModulePackages(ModulePackages obj)
        {
        }

        /// <since>6.4.0</since>
        public virtual void VisitModuleMainClass(ModuleMainClass obj)
        {
        }

        /// <since>6.4.0</since>
        public virtual void VisitNestHost(NestHost obj)
        {
        }

        /// <since>6.4.0</since>
        public virtual void VisitNestMembers(NestMembers obj)
        {
        }
    }
}