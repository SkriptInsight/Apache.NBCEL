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

namespace NBCEL.verifier.statics
{
	/// <summary>
	/// BCEL's Node classes (those from the classfile API that <B>accept()</B> Visitor
	/// instances) have <B>toString()</B> methods that were not designed to be robust,
	/// this gap is closed by this class.
	/// </summary>
	/// <remarks>
	/// BCEL's Node classes (those from the classfile API that <B>accept()</B> Visitor
	/// instances) have <B>toString()</B> methods that were not designed to be robust,
	/// this gap is closed by this class.
	/// When performing class file verification, it may be useful to output which
	/// entity (e.g. a <B>Code</B> instance) is not satisfying the verifier's
	/// constraints, but in this case it could be possible for the <B>toString()</B>
	/// method to throw a RuntimeException.
	/// A (new StringRepresentation(Node n)).toString() never throws any exception.
	/// Note that this class also serves as a placeholder for more sophisticated message
	/// handling in future versions of JustIce.
	/// </remarks>
	public class StringRepresentation : NBCEL.classfile.EmptyVisitor
	{
		/// <summary>The string representation, created by a visitXXX() method, output by toString().
		/// 	</summary>
		private string tostring;

		/// <summary>The node we ask for its string representation.</summary>
		/// <remarks>The node we ask for its string representation. Not really needed; only for debug output.
		/// 	</remarks>
		private readonly NBCEL.classfile.Node n;

		/// <summary>Creates a new StringRepresentation object which is the representation of n.
		/// 	</summary>
		/// <param name="n">The node to represent.</param>
		/// <seealso cref="ToString()"/>
		public StringRepresentation(NBCEL.classfile.Node n)
		{
			this.n = n;
			n.Accept(this);
		}

		// assign a string representation to field 'tostring' if we know n's class.
		/// <summary>Returns the String representation.</summary>
		public override string ToString()
		{
			// The run-time check below is needed because we don't want to omit inheritance
			// of "EmptyVisitor" and provide a thousand empty methods.
			// However, in terms of performance this would be a better idea.
			// If some new "Node" is defined in BCEL (such as some concrete "Attribute"), we
			// want to know that this class has also to be adapted.
			if (tostring == null)
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("Please adapt '" + GetType
					() + "' to deal with objects of class '" + n.GetType() + "'.");
			}
			return tostring;
		}

		/// <summary>
		/// Returns the String representation of the Node object obj;
		/// this is obj.toString() if it does not throw any RuntimeException,
		/// or else it is a string derived only from obj's class name.
		/// </summary>
		private string ToString(NBCEL.classfile.Node obj)
		{
			string ret;
			try
			{
				ret = obj.ToString();
			}
			catch (System.Exception)
			{
				// including ClassFormatException, trying to convert the "signature" of a ReturnaddressType LocalVariable
				// (shouldn't occur, but people do crazy things)
				string s = obj.GetType().FullName;
				s = Sharpen.Runtime.Substring(s, s.LastIndexOf(".") + 1);
				ret = "<<" + s + ">>";
			}
			return ret;
		}

		////////////////////////////////
		// Visitor methods start here //
		////////////////////////////////
		// We don't of course need to call some default implementation:
		// e.g. we could also simply output "Code" instead of a possibly
		// lengthy Code attribute's toString().
		public override void VisitCode(NBCEL.classfile.Code obj)
		{
			//tostring = toString(obj);
			tostring = "<CODE>";
		}

		// We don't need real code outputs.
		/// <since>6.0</since>
		public override void VisitAnnotation(NBCEL.classfile.Annotations obj)
		{
			//this is invoked whenever an annotation is found
			//when verifier is passed over a class
			tostring = ToString(obj);
		}

		/// <since>6.0</since>
		public override void VisitLocalVariableTypeTable(NBCEL.classfile.LocalVariableTypeTable
			 obj)
		{
			//this is invoked whenever a local variable type is found
			//when verifier is passed over a class
			tostring = ToString(obj);
		}

		public override void VisitCodeException(NBCEL.classfile.CodeException obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantClass(NBCEL.classfile.ConstantClass obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantDouble(NBCEL.classfile.ConstantDouble obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantFieldref(NBCEL.classfile.ConstantFieldref obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantFloat(NBCEL.classfile.ConstantFloat obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantInteger(NBCEL.classfile.ConstantInteger obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantInterfaceMethodref(NBCEL.classfile.ConstantInterfaceMethodref
			 obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantLong(NBCEL.classfile.ConstantLong obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantMethodref(NBCEL.classfile.ConstantMethodref obj
			)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantNameAndType(NBCEL.classfile.ConstantNameAndType
			 obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantPool(NBCEL.classfile.ConstantPool obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantString(NBCEL.classfile.ConstantString obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantUtf8(NBCEL.classfile.ConstantUtf8 obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitConstantValue(NBCEL.classfile.ConstantValue obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitDeprecated(NBCEL.classfile.Deprecated obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitExceptionTable(NBCEL.classfile.ExceptionTable obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitField(NBCEL.classfile.Field obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitInnerClass(NBCEL.classfile.InnerClass obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitInnerClasses(NBCEL.classfile.InnerClasses obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitJavaClass(NBCEL.classfile.JavaClass obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitLineNumber(NBCEL.classfile.LineNumber obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitLineNumberTable(NBCEL.classfile.LineNumberTable obj)
		{
			tostring = "<LineNumberTable: " + ToString(obj) + ">";
		}

		public override void VisitLocalVariable(NBCEL.classfile.LocalVariable obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitLocalVariableTable(NBCEL.classfile.LocalVariableTable obj
			)
		{
			tostring = "<LocalVariableTable: " + ToString(obj) + ">";
		}

		public override void VisitMethod(NBCEL.classfile.Method obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitSignature(NBCEL.classfile.Signature obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitSourceFile(NBCEL.classfile.SourceFile obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitStackMap(NBCEL.classfile.StackMap obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitSynthetic(NBCEL.classfile.Synthetic obj)
		{
			tostring = ToString(obj);
		}

		public override void VisitUnknown(NBCEL.classfile.Unknown obj)
		{
			tostring = ToString(obj);
		}

		/// <since>6.0</since>
		public override void VisitEnclosingMethod(NBCEL.classfile.EnclosingMethod obj)
		{
			tostring = ToString(obj);
		}

		/// <since>6.0</since>
		public override void VisitBootstrapMethods(NBCEL.classfile.BootstrapMethods obj)
		{
			tostring = ToString(obj);
		}

		/// <since>6.0</since>
		public override void VisitMethodParameters(NBCEL.classfile.MethodParameters obj)
		{
			tostring = ToString(obj);
		}

		/// <since>6.0</since>
		public override void VisitConstantInvokeDynamic(NBCEL.classfile.ConstantInvokeDynamic
			 obj)
		{
			tostring = ToString(obj);
		}

		/// <since>6.0</since>
		public override void VisitStackMapEntry(NBCEL.classfile.StackMapEntry obj)
		{
			tostring = ToString(obj);
		}

		/// <since>6.0</since>
		public override void VisitParameterAnnotation(NBCEL.classfile.ParameterAnnotations
			 obj)
		{
			tostring = ToString(obj);
		}

		/// <since>6.0</since>
		public override void VisitAnnotationEntry(NBCEL.classfile.AnnotationEntry obj)
		{
			tostring = ToString(obj);
		}

		/// <since>6.0</since>
		public override void VisitAnnotationDefault(NBCEL.classfile.AnnotationDefault obj
			)
		{
			tostring = ToString(obj);
		}

		/// <since>6.0</since>
		public override void VisitConstantMethodType(NBCEL.classfile.ConstantMethodType obj
			)
		{
			tostring = ToString(obj);
		}

		/// <since>6.0</since>
		public override void VisitConstantMethodHandle(NBCEL.classfile.ConstantMethodHandle
			 obj)
		{
			tostring = ToString(obj);
		}

		/// <since>6.0</since>
		public override void VisitParameterAnnotationEntry(NBCEL.classfile.ParameterAnnotationEntry
			 obj)
		{
			tostring = ToString(obj);
		}

		/// <since>6.4.0</since>
		public override void VisitNestMembers(NBCEL.classfile.NestMembers obj)
		{
			tostring = ToString(obj);
		}
	}
}
