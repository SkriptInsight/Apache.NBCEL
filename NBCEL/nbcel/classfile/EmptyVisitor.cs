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
	/// <summary>
	/// Visitor with empty method bodies, can be extended and used in conjunction
	/// with the DescendingVisitor class, e.g.
	/// </summary>
	/// <remarks>
	/// Visitor with empty method bodies, can be extended and used in conjunction
	/// with the DescendingVisitor class, e.g. By courtesy of David Spencer.
	/// </remarks>
	/// <seealso cref="DescendingVisitor"/>
	public class EmptyVisitor : NBCEL.classfile.Visitor
	{
		protected internal EmptyVisitor()
		{
		}

		/// <since>6.0</since>
		public virtual void VisitAnnotation(NBCEL.classfile.Annotations obj)
		{
		}

		/// <since>6.0</since>
		public virtual void VisitParameterAnnotation(NBCEL.classfile.ParameterAnnotations
			 obj)
		{
		}

		/// <since>6.0</since>
		public virtual void VisitAnnotationEntry(NBCEL.classfile.AnnotationEntry obj)
		{
		}

		/// <since>6.0</since>
		public virtual void VisitAnnotationDefault(NBCEL.classfile.AnnotationDefault obj)
		{
		}

		public virtual void VisitCode(NBCEL.classfile.Code obj)
		{
		}

		public virtual void VisitCodeException(NBCEL.classfile.CodeException obj)
		{
		}

		public virtual void VisitConstantClass(NBCEL.classfile.ConstantClass obj)
		{
		}

		public virtual void VisitConstantDouble(NBCEL.classfile.ConstantDouble obj)
		{
		}

		public virtual void VisitConstantFieldref(NBCEL.classfile.ConstantFieldref obj)
		{
		}

		public virtual void VisitConstantFloat(NBCEL.classfile.ConstantFloat obj)
		{
		}

		public virtual void VisitConstantInteger(NBCEL.classfile.ConstantInteger obj)
		{
		}

		public virtual void VisitConstantInterfaceMethodref(NBCEL.classfile.ConstantInterfaceMethodref
			 obj)
		{
		}

		public virtual void VisitConstantInvokeDynamic(NBCEL.classfile.ConstantInvokeDynamic
			 obj)
		{
		}

		public virtual void VisitConstantLong(NBCEL.classfile.ConstantLong obj)
		{
		}

		public virtual void VisitConstantMethodref(NBCEL.classfile.ConstantMethodref obj)
		{
		}

		public virtual void VisitConstantNameAndType(NBCEL.classfile.ConstantNameAndType 
			obj)
		{
		}

		public virtual void VisitConstantPool(NBCEL.classfile.ConstantPool obj)
		{
		}

		public virtual void VisitConstantString(NBCEL.classfile.ConstantString obj)
		{
		}

		public virtual void VisitConstantUtf8(NBCEL.classfile.ConstantUtf8 obj)
		{
		}

		public virtual void VisitConstantValue(NBCEL.classfile.ConstantValue obj)
		{
		}

		public virtual void VisitDeprecated(NBCEL.classfile.Deprecated obj)
		{
		}

		public virtual void VisitExceptionTable(NBCEL.classfile.ExceptionTable obj)
		{
		}

		public virtual void VisitField(NBCEL.classfile.Field obj)
		{
		}

		public virtual void VisitInnerClass(NBCEL.classfile.InnerClass obj)
		{
		}

		public virtual void VisitInnerClasses(NBCEL.classfile.InnerClasses obj)
		{
		}

		/// <since>6.0</since>
		public virtual void VisitBootstrapMethods(NBCEL.classfile.BootstrapMethods obj)
		{
		}

		public virtual void VisitJavaClass(NBCEL.classfile.JavaClass obj)
		{
		}

		public virtual void VisitLineNumber(NBCEL.classfile.LineNumber obj)
		{
		}

		public virtual void VisitLineNumberTable(NBCEL.classfile.LineNumberTable obj)
		{
		}

		public virtual void VisitLocalVariable(NBCEL.classfile.LocalVariable obj)
		{
		}

		public virtual void VisitLocalVariableTable(NBCEL.classfile.LocalVariableTable obj
			)
		{
		}

		public virtual void VisitMethod(NBCEL.classfile.Method obj)
		{
		}

		public virtual void VisitSignature(NBCEL.classfile.Signature obj)
		{
		}

		public virtual void VisitSourceFile(NBCEL.classfile.SourceFile obj)
		{
		}

		public virtual void VisitSynthetic(NBCEL.classfile.Synthetic obj)
		{
		}

		public virtual void VisitUnknown(NBCEL.classfile.Unknown obj)
		{
		}

		public virtual void VisitStackMap(NBCEL.classfile.StackMap obj)
		{
		}

		public virtual void VisitStackMapEntry(NBCEL.classfile.StackMapEntry obj)
		{
		}

		/// <since>6.0</since>
		public virtual void VisitEnclosingMethod(NBCEL.classfile.EnclosingMethod obj)
		{
		}

		/// <since>6.0</since>
		public virtual void VisitLocalVariableTypeTable(NBCEL.classfile.LocalVariableTypeTable
			 obj)
		{
		}

		/// <since>6.0</since>
		public virtual void VisitMethodParameters(NBCEL.classfile.MethodParameters obj)
		{
		}

		/// <since>6.4.0</since>
		public virtual void VisitMethodParameter(NBCEL.classfile.MethodParameter obj)
		{
		}

		/// <since>6.0</since>
		public virtual void VisitConstantMethodType(NBCEL.classfile.ConstantMethodType obj
			)
		{
		}

		/// <since>6.0</since>
		public virtual void VisitConstantMethodHandle(NBCEL.classfile.ConstantMethodHandle
			 constantMethodHandle)
		{
		}

		/// <since>6.0</since>
		public virtual void VisitParameterAnnotationEntry(NBCEL.classfile.ParameterAnnotationEntry
			 parameterAnnotationEntry)
		{
		}

		/// <since>6.1</since>
		public virtual void VisitConstantPackage(NBCEL.classfile.ConstantPackage constantPackage
			)
		{
		}

		/// <since>6.1</since>
		public virtual void VisitConstantModule(NBCEL.classfile.ConstantModule constantModule
			)
		{
		}

		/// <since>6.3</since>
		public virtual void VisitConstantDynamic(NBCEL.classfile.ConstantDynamic obj)
		{
		}

		/// <since>6.4.0</since>
		public virtual void VisitModule(NBCEL.classfile.Module obj)
		{
		}

		/// <since>6.4.0</since>
		public virtual void VisitModuleRequires(NBCEL.classfile.ModuleRequires obj)
		{
		}

		/// <since>6.4.0</since>
		public virtual void VisitModuleExports(NBCEL.classfile.ModuleExports obj)
		{
		}

		/// <since>6.4.0</since>
		public virtual void VisitModuleOpens(NBCEL.classfile.ModuleOpens obj)
		{
		}

		/// <since>6.4.0</since>
		public virtual void VisitModuleProvides(NBCEL.classfile.ModuleProvides obj)
		{
		}

		/// <since>6.4.0</since>
		public virtual void VisitModulePackages(NBCEL.classfile.ModulePackages obj)
		{
		}

		/// <since>6.4.0</since>
		public virtual void VisitModuleMainClass(NBCEL.classfile.ModuleMainClass obj)
		{
		}

		/// <since>6.4.0</since>
		public virtual void VisitNestHost(NBCEL.classfile.NestHost obj)
		{
		}

		/// <since>6.4.0</since>
		public virtual void VisitNestMembers(NBCEL.classfile.NestMembers obj)
		{
		}
	}
}
