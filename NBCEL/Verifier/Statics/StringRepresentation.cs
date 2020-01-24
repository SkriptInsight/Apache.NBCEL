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
using Apache.NBCEL.ClassFile;
using Apache.NBCEL.Verifier.Exc;

namespace Apache.NBCEL.Verifier.Statics
{
	/// <summary>
	///     BCEL's Node classes (those from the classfile API that <B>accept()</B> Visitor
	///     instances) have <B>toString()</B> methods that were not designed to be robust,
	///     this gap is closed by this class.
	/// </summary>
	/// <remarks>
	///     BCEL's Node classes (those from the classfile API that <B>accept()</B> Visitor
	///     instances) have <B>toString()</B> methods that were not designed to be robust,
	///     this gap is closed by this class.
	///     When performing class file verification, it may be useful to output which
	///     entity (e.g. a <B>Code</B> instance) is not satisfying the verifier's
	///     constraints, but in this case it could be possible for the <B>toString()</B>
	///     method to throw a RuntimeException.
	///     A (new StringRepresentation(Node n)).toString() never throws any exception.
	///     Note that this class also serves as a placeholder for more sophisticated message
	///     handling in future versions of JustIce.
	/// </remarks>
	public class StringRepresentation : EmptyVisitor
    {
	    /// <summary>The node we ask for its string representation.</summary>
	    /// <remarks>
	    ///     The node we ask for its string representation. Not really needed; only for debug output.
	    /// </remarks>
	    private readonly Node n;

	    /// <summary>
	    ///     The string representation, created by a visitXXX() method, output by toString().
	    /// </summary>
	    private string tostring;

	    /// <summary>
	    ///     Creates a new StringRepresentation object which is the representation of n.
	    /// </summary>
	    /// <param name="n">The node to represent.</param>
	    /// <seealso cref="ToString()" />
	    public StringRepresentation(Node n)
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
                throw new AssertionViolatedException("Please adapt '" + GetType
                                                         () + "' to deal with objects of class '" + n.GetType() + "'.");
            return tostring;
        }

        /// <summary>
        ///     Returns the String representation of the Node object obj;
        ///     this is obj.toString() if it does not throw any RuntimeException,
        ///     or else it is a string derived only from obj's class name.
        /// </summary>
        private string ToString(Node obj)
        {
            string ret;
            try
            {
                ret = obj.ToString();
            }
            catch (Exception)
            {
                // including ClassFormatException, trying to convert the "signature" of a ReturnaddressType LocalVariable
                // (shouldn't occur, but people do crazy things)
                var s = obj.GetType().FullName;
                s = Runtime.Substring(s, s.LastIndexOf(".") + 1);
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
        public override void VisitCode(Code obj)
        {
            //tostring = toString(obj);
            tostring = "<CODE>";
        }

        // We don't need real code outputs.
        /// <since>6.0</since>
        public override void VisitAnnotation(Annotations obj)
        {
            //this is invoked whenever an annotation is found
            //when verifier is passed over a class
            tostring = ToString(obj);
        }

        /// <since>6.0</since>
        public override void VisitLocalVariableTypeTable(LocalVariableTypeTable
            obj)
        {
            //this is invoked whenever a local variable type is found
            //when verifier is passed over a class
            tostring = ToString(obj);
        }

        public override void VisitCodeException(CodeException obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantClass(ConstantClass obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantDouble(ConstantDouble obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantFieldref(ConstantFieldref obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantFloat(ConstantFloat obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantInteger(ConstantInteger obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantInterfaceMethodref(ConstantInterfaceMethodref
            obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantLong(ConstantLong obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantMethodref(ConstantMethodref obj
        )
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantNameAndType(ConstantNameAndType
            obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantPool(ConstantPool obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantString(ConstantString obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantUtf8(ConstantUtf8 obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitConstantValue(ConstantValue obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitDeprecated(Deprecated obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitExceptionTable(ExceptionTable obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitField(Field obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitInnerClass(InnerClass obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitInnerClasses(InnerClasses obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitJavaClass(JavaClass obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitLineNumber(LineNumber obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitLineNumberTable(LineNumberTable obj)
        {
            tostring = "<LineNumberTable: " + ToString(obj) + ">";
        }

        public override void VisitLocalVariable(LocalVariable obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitLocalVariableTable(LocalVariableTable obj
        )
        {
            tostring = "<LocalVariableTable: " + ToString(obj) + ">";
        }

        public override void VisitMethod(Method obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitSignature(Signature obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitSourceFile(SourceFile obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitStackMap(StackMap obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitSynthetic(Synthetic obj)
        {
            tostring = ToString(obj);
        }

        public override void VisitUnknown(Unknown obj)
        {
            tostring = ToString(obj);
        }

        /// <since>6.0</since>
        public override void VisitEnclosingMethod(EnclosingMethod obj)
        {
            tostring = ToString(obj);
        }

        /// <since>6.0</since>
        public override void VisitBootstrapMethods(BootstrapMethods obj)
        {
            tostring = ToString(obj);
        }

        /// <since>6.0</since>
        public override void VisitMethodParameters(MethodParameters obj)
        {
            tostring = ToString(obj);
        }

        /// <since>6.0</since>
        public override void VisitConstantInvokeDynamic(ConstantInvokeDynamic
            obj)
        {
            tostring = ToString(obj);
        }

        /// <since>6.0</since>
        public override void VisitStackMapEntry(StackMapEntry obj)
        {
            tostring = ToString(obj);
        }

        /// <since>6.0</since>
        public override void VisitParameterAnnotation(ParameterAnnotations
            obj)
        {
            tostring = ToString(obj);
        }

        /// <since>6.0</since>
        public override void VisitAnnotationEntry(AnnotationEntry obj)
        {
            tostring = ToString(obj);
        }

        /// <since>6.0</since>
        public override void VisitAnnotationDefault(AnnotationDefault obj
        )
        {
            tostring = ToString(obj);
        }

        /// <since>6.0</since>
        public override void VisitConstantMethodType(ConstantMethodType obj
        )
        {
            tostring = ToString(obj);
        }

        /// <since>6.0</since>
        public override void VisitConstantMethodHandle(ConstantMethodHandle
            obj)
        {
            tostring = ToString(obj);
        }

        /// <since>6.0</since>
        public override void VisitParameterAnnotationEntry(ParameterAnnotationEntry
            obj)
        {
            tostring = ToString(obj);
        }

        /// <since>6.4.0</since>
        public override void VisitNestMembers(NestMembers obj)
        {
            tostring = ToString(obj);
        }
    }
}