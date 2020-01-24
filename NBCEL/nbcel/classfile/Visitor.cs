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

namespace NBCEL.classfile
{
	/// <summary>Interface to make use of the Visitor pattern programming style.</summary>
	/// <remarks>
	/// Interface to make use of the Visitor pattern programming style. I.e. a class
	/// that implements this interface can traverse the contents of a Java class just
	/// by calling the `accept' method which all classes have.
	/// </remarks>
	public interface Visitor
	{
		void VisitCode(NBCEL.classfile.Code obj);

		void VisitCodeException(NBCEL.classfile.CodeException obj);

		void VisitConstantClass(NBCEL.classfile.ConstantClass obj);

		void VisitConstantDouble(NBCEL.classfile.ConstantDouble obj);

		void VisitConstantFieldref(NBCEL.classfile.ConstantFieldref obj);

		void VisitConstantFloat(NBCEL.classfile.ConstantFloat obj);

		void VisitConstantInteger(NBCEL.classfile.ConstantInteger obj);

		void VisitConstantInterfaceMethodref(NBCEL.classfile.ConstantInterfaceMethodref obj
			);

		void VisitConstantInvokeDynamic(NBCEL.classfile.ConstantInvokeDynamic obj);

		void VisitConstantLong(NBCEL.classfile.ConstantLong obj);

		void VisitConstantMethodref(NBCEL.classfile.ConstantMethodref obj);

		void VisitConstantNameAndType(NBCEL.classfile.ConstantNameAndType obj);

		void VisitConstantPool(NBCEL.classfile.ConstantPool obj);

		void VisitConstantString(NBCEL.classfile.ConstantString obj);

		void VisitConstantUtf8(NBCEL.classfile.ConstantUtf8 obj);

		void VisitConstantValue(NBCEL.classfile.ConstantValue obj);

		void VisitDeprecated(NBCEL.classfile.Deprecated obj);

		void VisitExceptionTable(NBCEL.classfile.ExceptionTable obj);

		void VisitField(NBCEL.classfile.Field obj);

		void VisitInnerClass(NBCEL.classfile.InnerClass obj);

		void VisitInnerClasses(NBCEL.classfile.InnerClasses obj);

		void VisitJavaClass(NBCEL.classfile.JavaClass obj);

		void VisitLineNumber(NBCEL.classfile.LineNumber obj);

		void VisitLineNumberTable(NBCEL.classfile.LineNumberTable obj);

		void VisitLocalVariable(NBCEL.classfile.LocalVariable obj);

		void VisitLocalVariableTable(NBCEL.classfile.LocalVariableTable obj);

		void VisitMethod(NBCEL.classfile.Method obj);

		void VisitSignature(NBCEL.classfile.Signature obj);

		void VisitSourceFile(NBCEL.classfile.SourceFile obj);

		void VisitSynthetic(NBCEL.classfile.Synthetic obj);

		void VisitUnknown(NBCEL.classfile.Unknown obj);

		void VisitStackMap(NBCEL.classfile.StackMap obj);

		void VisitStackMapEntry(NBCEL.classfile.StackMapEntry obj);

		/// <since>6.0</since>
		void VisitAnnotation(NBCEL.classfile.Annotations obj);

		/// <since>6.0</since>
		void VisitParameterAnnotation(NBCEL.classfile.ParameterAnnotations obj);

		/// <since>6.0</since>
		void VisitAnnotationEntry(NBCEL.classfile.AnnotationEntry obj);

		/// <since>6.0</since>
		void VisitAnnotationDefault(NBCEL.classfile.AnnotationDefault obj);

		/// <since>6.0</since>
		void VisitLocalVariableTypeTable(NBCEL.classfile.LocalVariableTypeTable obj);

		/// <since>6.0</since>
		void VisitEnclosingMethod(NBCEL.classfile.EnclosingMethod obj);

		/// <since>6.0</since>
		void VisitBootstrapMethods(NBCEL.classfile.BootstrapMethods obj);

		/// <since>6.0</since>
		void VisitMethodParameters(NBCEL.classfile.MethodParameters obj);

		/// <since>6.4.0</since>
		void VisitMethodParameter(NBCEL.classfile.MethodParameter obj);

		// empty
		/// <since>6.0</since>
		void VisitConstantMethodType(NBCEL.classfile.ConstantMethodType obj);

		/// <since>6.0</since>
		void VisitConstantMethodHandle(NBCEL.classfile.ConstantMethodHandle obj);

		/// <since>6.0</since>
		void VisitParameterAnnotationEntry(NBCEL.classfile.ParameterAnnotationEntry obj);

		/// <since>6.1</since>
		void VisitConstantPackage(NBCEL.classfile.ConstantPackage constantPackage);

		/// <since>6.1</since>
		void VisitConstantModule(NBCEL.classfile.ConstantModule constantModule);

		/// <since>6.3</since>
		void VisitConstantDynamic(NBCEL.classfile.ConstantDynamic constantDynamic);

		// empty
		/// <since>6.4.0</since>
		void VisitModule(NBCEL.classfile.Module constantModule);

		// empty
		/// <since>6.4.0</since>
		void VisitModuleRequires(NBCEL.classfile.ModuleRequires constantModule);

		// empty
		/// <since>6.4.0</since>
		void VisitModuleExports(NBCEL.classfile.ModuleExports constantModule);

		// empty
		/// <since>6.4.0</since>
		void VisitModuleOpens(NBCEL.classfile.ModuleOpens constantModule);

		// empty
		/// <since>6.4.0</since>
		void VisitModuleProvides(NBCEL.classfile.ModuleProvides constantModule);

		// empty
		/// <since>6.4.0</since>
		void VisitModulePackages(NBCEL.classfile.ModulePackages constantModule);

		// empty
		/// <since>6.4.0</since>
		void VisitModuleMainClass(NBCEL.classfile.ModuleMainClass obj);

		// empty
		/// <since>6.4.0</since>
		void VisitNestHost(NBCEL.classfile.NestHost obj);

		// empty
		/// <since>6.4.0</since>
		void VisitNestMembers(NBCEL.classfile.NestMembers obj);
		// empty
	}
}
