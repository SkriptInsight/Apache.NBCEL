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

namespace Apache.NBCEL.ClassFile
{
	/// <summary>Interface to make use of the Visitor pattern programming style.</summary>
	/// <remarks>
	///     Interface to make use of the Visitor pattern programming style. I.e. a class
	///     that implements this interface can traverse the contents of a Java class just
	///     by calling the `accept' method which all classes have.
	/// </remarks>
	public interface Visitor
    {
        void VisitCode(Code obj);

        void VisitCodeException(CodeException obj);

        void VisitConstantClass(ConstantClass obj);

        void VisitConstantDouble(ConstantDouble obj);

        void VisitConstantFieldref(ConstantFieldref obj);

        void VisitConstantFloat(ConstantFloat obj);

        void VisitConstantInteger(ConstantInteger obj);

        void VisitConstantInterfaceMethodref(ConstantInterfaceMethodref obj
        );

        void VisitConstantInvokeDynamic(ConstantInvokeDynamic obj);

        void VisitConstantLong(ConstantLong obj);

        void VisitConstantMethodref(ConstantMethodref obj);

        void VisitConstantNameAndType(ConstantNameAndType obj);

        void VisitConstantPool(ConstantPool obj);

        void VisitConstantString(ConstantString obj);

        void VisitConstantUtf8(ConstantUtf8 obj);

        void VisitConstantValue(ConstantValue obj);

        void VisitDeprecated(Deprecated obj);

        void VisitExceptionTable(ExceptionTable obj);

        void VisitField(Field obj);

        void VisitInnerClass(InnerClass obj);

        void VisitInnerClasses(InnerClasses obj);

        void VisitJavaClass(JavaClass obj);

        void VisitLineNumber(LineNumber obj);

        void VisitLineNumberTable(LineNumberTable obj);

        void VisitLocalVariable(LocalVariable obj);

        void VisitLocalVariableTable(LocalVariableTable obj);

        void VisitMethod(Method obj);

        void VisitSignature(Signature obj);

        void VisitSourceFile(SourceFile obj);

        void VisitSynthetic(Synthetic obj);

        void VisitUnknown(Unknown obj);

        void VisitStackMap(StackMap obj);

        void VisitStackMapEntry(StackMapEntry obj);

        /// <since>6.0</since>
        void VisitAnnotation(Annotations obj);

        /// <since>6.0</since>
        void VisitParameterAnnotation(ParameterAnnotations obj);

        /// <since>6.0</since>
        void VisitAnnotationEntry(AnnotationEntry obj);

        /// <since>6.0</since>
        void VisitAnnotationDefault(AnnotationDefault obj);

        /// <since>6.0</since>
        void VisitLocalVariableTypeTable(LocalVariableTypeTable obj);

        /// <since>6.0</since>
        void VisitEnclosingMethod(EnclosingMethod obj);

        /// <since>6.0</since>
        void VisitBootstrapMethods(BootstrapMethods obj);

        /// <since>6.0</since>
        void VisitMethodParameters(MethodParameters obj);

        /// <since>6.4.0</since>
        void VisitMethodParameter(MethodParameter obj);

        // empty
        /// <since>6.0</since>
        void VisitConstantMethodType(ConstantMethodType obj);

        /// <since>6.0</since>
        void VisitConstantMethodHandle(ConstantMethodHandle obj);

        /// <since>6.0</since>
        void VisitParameterAnnotationEntry(ParameterAnnotationEntry obj);

        /// <since>6.1</since>
        void VisitConstantPackage(ConstantPackage constantPackage);

        /// <since>6.1</since>
        void VisitConstantModule(ConstantModule constantModule);

        /// <since>6.3</since>
        void VisitConstantDynamic(ConstantDynamic constantDynamic);

        // empty
        /// <since>6.4.0</since>
        void VisitModule(Module constantModule);

        // empty
        /// <since>6.4.0</since>
        void VisitModuleRequires(ModuleRequires constantModule);

        // empty
        /// <since>6.4.0</since>
        void VisitModuleExports(ModuleExports constantModule);

        // empty
        /// <since>6.4.0</since>
        void VisitModuleOpens(ModuleOpens constantModule);

        // empty
        /// <since>6.4.0</since>
        void VisitModuleProvides(ModuleProvides constantModule);

        // empty
        /// <since>6.4.0</since>
        void VisitModulePackages(ModulePackages constantModule);

        // empty
        /// <since>6.4.0</since>
        void VisitModuleMainClass(ModuleMainClass obj);

        // empty
        /// <since>6.4.0</since>
        void VisitNestHost(NestHost obj);

        // empty
        /// <since>6.4.0</since>
        void VisitNestMembers(NestMembers obj);

        // empty
    }
}