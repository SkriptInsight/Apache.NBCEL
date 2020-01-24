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
using NBCEL.classfile;
using NBCEL.generic;
using NBCEL.verifier.exc;
using Sharpen;
using EmptyVisitor = NBCEL.generic.EmptyVisitor;
using Type = NBCEL.generic.Type;

namespace NBCEL.verifier.structurals
{
	/// <summary>A Visitor class testing for valid preconditions of JVM instructions.</summary>
	/// <remarks>
	///     A Visitor class testing for valid preconditions of JVM instructions.
	///     The instance of this class will throw a StructuralCodeConstraintException
	///     instance if an instruction is visitXXX()ed which has preconditions that are
	///     not satisfied.
	///     TODO: Currently, the JVM's behavior concerning monitors (MONITORENTER,
	///     MONITOREXIT) is not modeled in JustIce.
	/// </remarks>
	/// <seealso cref="NBCEL.verifier.exc.StructuralCodeConstraintException" />
	public class InstConstraintVisitor : EmptyVisitor
    {
        private static readonly ObjectType GENERIC_ARRAY = ObjectType
            .GetInstance(typeof(GenericArray).FullName);

        /// <summary>The ConstantPoolGen we're working on.</summary>
        /// <seealso cref="SetConstantPoolGen(NBCEL.generic.ConstantPoolGen)" />
        private ConstantPoolGen cpg;

        /// <summary>The Execution Frame we're working on.</summary>
        /// <seealso cref="SetFrame(Frame)" />
        /// <seealso cref="Locals()" />
        /// <seealso cref="Stack()" />
        private Frame frame;

        /// <summary>The MethodGen we're working on.</summary>
        /// <seealso cref="SetMethodGen(NBCEL.generic.MethodGen)" />
        private MethodGen mg;

        //CHECKSTYLE:OFF (there are lots of references!)
        //CHECKSTYLE:ON
        /// <summary>The OperandStack we're working on.</summary>
        /// <seealso cref="SetFrame(Frame)" />
        private OperandStack Stack()
        {
            return frame.GetStack();
        }

        /// <summary>The LocalVariables we're working on.</summary>
        /// <seealso cref="SetFrame(Frame)" />
        private LocalVariables Locals()
        {
            return frame.GetLocals();
        }

        /// <summary>
        ///     This method is called by the visitXXX() to notify the acceptor of this InstConstraintVisitor
        ///     that a constraint violation has occured.
        /// </summary>
        /// <remarks>
        ///     This method is called by the visitXXX() to notify the acceptor of this InstConstraintVisitor
        ///     that a constraint violation has occured. This is done by throwing an instance of a
        ///     StructuralCodeConstraintException.
        /// </remarks>
        /// <exception cref="NBCEL.verifier.exc.StructuralCodeConstraintException">always.</exception>
        private void ConstraintViolated(Instruction violator, string description
        )
        {
            var fq_classname = violator.GetType().FullName;
            throw new StructuralCodeConstraintException("Instruction " + Runtime.Substring
                                                            (fq_classname, fq_classname.LastIndexOf('.') + 1) +
                                                        " constraint violated: " + description
            );
        }

        /// <summary>This returns the single instance of the InstConstraintVisitor class.</summary>
        /// <remarks>
        ///     This returns the single instance of the InstConstraintVisitor class.
        ///     To operate correctly, other values must have been set before actually
        ///     using the instance.
        ///     Use this method for performance reasons.
        /// </remarks>
        /// <seealso cref="SetConstantPoolGen(NBCEL.generic.ConstantPoolGen)" />
        /// <seealso cref="SetMethodGen(NBCEL.generic.MethodGen)" />
        public virtual void SetFrame(Frame f)
        {
            // TODO could be package-protected?
            frame = f;
        }

        //if (singleInstance.mg == null || singleInstance.cpg == null)
        // throw new AssertionViolatedException("Forgot to set important values first.");
        /// <summary>
        ///     Sets the ConstantPoolGen instance needed for constraint
        ///     checking prior to execution.
        /// </summary>
        public virtual void SetConstantPoolGen(ConstantPoolGen cpg)
        {
            // TODO could be package-protected?
            this.cpg = cpg;
        }

        /// <summary>
        ///     Sets the MethodGen instance needed for constraint
        ///     checking prior to execution.
        /// </summary>
        public virtual void SetMethodGen(MethodGen mg)
        {
            this.mg = mg;
        }

        /// <summary>Assures index is of type INT.</summary>
        /// <exception cref="NBCEL.verifier.exc.StructuralCodeConstraintException">
        ///     if the above constraint is not satisfied.
        /// </exception>
        private void IndexOfInt(Instruction o, Type index)
        {
            if (!index.Equals(Type.INT))
                ConstraintViolated(o, "The 'index' is not of type int but of type " + index + "."
                );
        }

        /// <summary>Assures the ReferenceType r is initialized (or Type.NULL).</summary>
        /// <remarks>
        ///     Assures the ReferenceType r is initialized (or Type.NULL).
        ///     Formally, this means (!(r instanceof UninitializedObjectType)), because
        ///     there are no uninitialized array types.
        /// </remarks>
        /// <exception cref="NBCEL.verifier.exc.StructuralCodeConstraintException">
        ///     if the above constraint is not satisfied.
        /// </exception>
        private void ReferenceTypeIsInitialized(Instruction o, ReferenceType
            r)
        {
            if (r is UninitializedObjectType) ConstraintViolated(o, "Working on an uninitialized object '" + r + "'.");
        }

        /// <summary>Assures value is of type INT.</summary>
        private void ValueOfInt(Instruction o, Type value)
        {
            if (!value.Equals(Type.INT))
                ConstraintViolated(o, "The 'value' is not of type int but of type " + value + "."
                );
        }

        /// <summary>
        ///     Assures arrayref is of ArrayType or NULL;
        ///     returns true if and only if arrayref is non-NULL.
        /// </summary>
        /// <exception cref="NBCEL.verifier.exc.StructuralCodeConstraintException">
        ///     if the above constraint is violated.
        /// </exception>
        private bool ArrayrefOfArrayType(Instruction o, Type
            arrayref)
        {
            if (!(arrayref is ArrayType || arrayref.Equals(Type
                      .NULL)))
                ConstraintViolated(o, "The 'arrayref' does not refer to an array but is of type "
                                      + arrayref + ".");
            return arrayref is ArrayType;
        }

        /* MISC                                                        */
        /// <summary>
        ///     Ensures the general preconditions of an instruction that accesses the stack.
        /// </summary>
        /// <remarks>
        ///     Ensures the general preconditions of an instruction that accesses the stack.
        ///     This method is here because BCEL has no such superinterface for the stack
        ///     accessing instructions; and there are funny unexpected exceptions in the
        ///     semantices of the superinterfaces and superclasses provided.
        ///     E.g. SWAP is a StackConsumer, but DUP_X1 is not a StackProducer.
        ///     Therefore, this method is called by all StackProducer, StackConsumer,
        ///     and StackInstruction instances via their visitXXX() method.
        ///     Unfortunately, as the superclasses and superinterfaces overlap, some instructions
        ///     cause this method to be called two or three times. [TODO: Fix this.]
        /// </remarks>
        /// <seealso cref="VisitStackConsumer(NBCEL.generic.StackConsumer)" />
        /// <seealso cref="VisitStackProducer(NBCEL.generic.StackProducer)" />
        /// <seealso cref="VisitStackInstruction(NBCEL.generic.StackInstruction)" />
        private void _visitStackAccessor(Instruction o)
        {
            var consume = o.ConsumeStack(cpg);
            // Stack values are always consumed first; then produced.
            if (consume > Stack().SlotsUsed())
                ConstraintViolated(o, "Cannot consume " + consume + " stack slots: only " + Stack
                                          ().SlotsUsed() + " slot(s) left on stack!\nStack:\n" + Stack());
            var produce = o.ProduceStack(cpg) - o.ConsumeStack(cpg);
            // Stack values are always consumed first; then produced.
            if (produce + Stack().SlotsUsed() > Stack().MaxStack())
                ConstraintViolated(o, "Cannot produce " + produce + " stack slots: only " + (Stack
                                                                                                 ().MaxStack() -
                                                                                             Stack().SlotsUsed()) +
                                      " free stack slot(s) left.\nStack:\n" + Stack
                                          ());
        }

        /* "generic"visitXXXX methods where XXXX is an interface       */
        /* therefore, we don't know the order of visiting; but we know */
        /* these methods are called before the visitYYYY methods below */
        /// <summary>Assures the generic preconditions of a LoadClass instance.</summary>
        /// <remarks>
        ///     Assures the generic preconditions of a LoadClass instance.
        ///     The referenced class is loaded and pass2-verified.
        /// </remarks>
        public override void VisitLoadClass(LoadClass o)
        {
            var t = o.GetLoadClassType(cpg);
            if (t != null)
            {
                // null means "no class is loaded"
                var v = VerifierFactory.GetVerifier(t.GetClassName
                    ());
                var vr = v.DoPass2();
                if (vr.GetStatus() != VerificationResult.VERIFIED_OK)
                    ConstraintViolated((Instruction) o, "Class '" + o.GetLoadClassType(cpg
                                                        ).GetClassName() +
                                                        "' is referenced, but cannot be loaded and resolved: '" + vr
                                                        + "'.");
            }
        }

        /// <summary>Ensures the general preconditions of a StackConsumer instance.</summary>
        public override void VisitStackConsumer(StackConsumer o)
        {
            _visitStackAccessor((Instruction) o);
        }

        /// <summary>Ensures the general preconditions of a StackProducer instance.</summary>
        public override void VisitStackProducer(StackProducer o)
        {
            _visitStackAccessor((Instruction) o);
        }

        /* "generic" visitYYYY methods where YYYY is a superclass.     */
        /* therefore, we know the order of visiting; we know           */
        /* these methods are called after the visitXXXX methods above. */
        /// <summary>Ensures the general preconditions of a CPInstruction instance.</summary>
        public override void VisitCPInstruction(CPInstruction o)
        {
            var idx = o.GetIndex();
            if (idx < 0 || idx >= cpg.GetSize())
                throw new AssertionViolatedException("Huh?! Constant pool index of instruction '"
                                                     + o + "' illegal? Pass 3a should have checked this!");
        }

        /// <summary>Ensures the general preconditions of a FieldInstruction instance.</summary>
        public override void VisitFieldInstruction(FieldInstruction o)
        {
            // visitLoadClass(o) has been called before: Every FieldOrMethod
            // implements LoadClass.
            // visitCPInstruction(o) has been called before.
            // A FieldInstruction may be: GETFIELD, GETSTATIC, PUTFIELD, PUTSTATIC
            var c = cpg.GetConstant(o.GetIndex());
            if (!(c is ConstantFieldref))
                ConstraintViolated(o,
                    "Index '" + o.GetIndex() + "' should refer to a CONSTANT_Fieldref_info structure, but refers to '"
                    + c + "'.");
            // the o.getClassType(cpg) type has passed pass 2; see visitLoadClass(o).
            var t = o.GetType(cpg);
            if (t is ObjectType)
            {
                var name = ((ObjectType) t).GetClassName();
                var v = VerifierFactory.GetVerifier(name);
                var vr = v.DoPass2();
                if (vr.GetStatus() != VerificationResult.VERIFIED_OK)
                    ConstraintViolated(o, "Class '" + name + "' is referenced, but cannot be loaded and resolved: '"
                                          + vr + "'.");
            }
        }

        /// <summary>Ensures the general preconditions of an InvokeInstruction instance.</summary>
        public override void VisitInvokeInstruction(InvokeInstruction o)
        {
        }

        // visitLoadClass(o) has been called before: Every FieldOrMethod
        // implements LoadClass.
        // visitCPInstruction(o) has been called before.
        //TODO
        /// <summary>Ensures the general preconditions of a StackInstruction instance.</summary>
        public override void VisitStackInstruction(StackInstruction o)
        {
            _visitStackAccessor(o);
        }

        /// <summary>
        ///     Assures the generic preconditions of a LocalVariableInstruction instance.
        /// </summary>
        /// <remarks>
        ///     Assures the generic preconditions of a LocalVariableInstruction instance.
        ///     That is, the index of the local variable must be valid.
        /// </remarks>
        public override void VisitLocalVariableInstruction(LocalVariableInstruction
            o)
        {
            if (Locals().MaxLocals() <= (o.GetType(cpg).GetSize() == 1
                    ? o.GetIndex()
                    : o.GetIndex
                          () + 1))
                ConstraintViolated(o, "The 'index' is not a valid index into the local variable array."
                );
        }

        /// <summary>Assures the generic preconditions of a LoadInstruction instance.</summary>
        public override void VisitLoadInstruction(LoadInstruction o)
        {
            //visitLocalVariableInstruction(o) is called before, because it is more generic.
            // LOAD instructions must not read Type.UNKNOWN
            if (Locals().Get(o.GetIndex()) == Type.UNKNOWN)
                ConstraintViolated(o, "Read-Access on local variable " + o.GetIndex() + " with unknown content."
                );
            // LOAD instructions, two-slot-values at index N must have Type.UNKNOWN
            // as a symbol for the higher halve at index N+1
            // [suppose some instruction put an int at N+1--- our double at N is defective]
            if (o.GetType(cpg).GetSize() == 2)
                if (Locals().Get(o.GetIndex() + 1) != Type.UNKNOWN)
                    ConstraintViolated(o, "Reading a two-locals value from local variables " + o.GetIndex
                                              () + " and " + (o.GetIndex() + 1) +
                                          " where the latter one is destroyed.");
            // LOAD instructions must read the correct type.
            if (!(o is ALOAD))
            {
                if (Locals().Get(o.GetIndex()) != o.GetType(cpg))
                    ConstraintViolated(o, "Local Variable type and LOADing Instruction type mismatch: Local Variable: '"
                                          + Locals().Get(o.GetIndex()) + "'; Instruction type: '" + o.GetType(cpg) +
                                          "'."
                    );
            }
            else if (!(Locals().Get(o.GetIndex()) is ReferenceType))
            {
                // we deal with an ALOAD
                ConstraintViolated(o, "Local Variable type and LOADing Instruction type mismatch: Local Variable: '"
                                      + Locals().Get(o.GetIndex()) + "'; Instruction expects a ReferenceType.");
            }

            // ALOAD __IS ALLOWED__ to put uninitialized objects onto the stack!
            //referenceTypeIsInitialized(o, (ReferenceType) (locals().get(o.getIndex())));
            // LOAD instructions must have enough free stack slots.
            if (Stack().MaxStack() - Stack().SlotsUsed() < o.GetType(cpg).GetSize())
                ConstraintViolated(o, "Not enough free stack slots to load a '" + o.GetType(cpg)
                                                                                + "' onto the OperandStack.");
        }

        /// <summary>Assures the generic preconditions of a StoreInstruction instance.</summary>
        public override void VisitStoreInstruction(StoreInstruction o)
        {
            //visitLocalVariableInstruction(o) is called before, because it is more generic.
            if (Stack().IsEmpty())
                // Don't bother about 1 or 2 stack slots used. This check is implicitly done below while type checking.
                ConstraintViolated(o, "Cannot STORE: Stack to read from is empty.");
            if (!(o is ASTORE))
            {
                if (!(Stack().Peek() == o.GetType(cpg)))
                    // the other xSTORE types are singletons in BCEL.
                    ConstraintViolated(o, "Stack top type and STOREing Instruction type mismatch: Stack top: '"
                                          + Stack().Peek() + "'; Instruction type: '" + o.GetType(cpg) + "'.");
            }
            else
            {
                // we deal with ASTORE
                var stacktop = Stack().Peek();
                if (!(stacktop is ReferenceType) && !(stacktop is ReturnaddressType
                        ))
                    ConstraintViolated(o, "Stack top type and STOREing Instruction type mismatch: Stack top: '"
                                          + Stack().Peek() +
                                          "'; Instruction expects a ReferenceType or a ReturnadressType."
                    );
            }
        }

        //if (stacktop instanceof ReferenceType) {
        //    referenceTypeIsInitialized(o, (ReferenceType) stacktop);
        //}
        /// <summary>Assures the generic preconditions of a ReturnInstruction instance.</summary>
        public override void VisitReturnInstruction(ReturnInstruction o)
        {
            var method_type = mg.GetType();
            if (method_type == Type.BOOLEAN || method_type == Type
                    .BYTE || method_type == Type.SHORT || method_type == Type
                    .CHAR)
                method_type = Type.INT;
            if (o is RETURN)
            {
                if (method_type != Type.VOID)
                    ConstraintViolated(o, "RETURN instruction in non-void method.");
                else
                    return;
            }

            if (o is ARETURN)
            {
                if (method_type == Type.VOID) ConstraintViolated(o, "ARETURN instruction in void method.");
                if (Stack().Peek() == Type.NULL) return;
                if (!(Stack().Peek() is ReferenceType))
                    ConstraintViolated(o, "Reference type expected on top of stack, but is: '" + Stack
                                              ().Peek() + "'.");
                ReferenceTypeIsInitialized(o, (ReferenceType) Stack().Peek());
            }
            else if (!method_type.Equals(Stack().Peek()))
            {
                //ReferenceType objectref = (ReferenceType) (stack().peek());
                // TODO: This can only be checked if using Staerk-et-al's "set of object types" instead of a
                // "wider cast object type" created during verification.
                //if (! (objectref.isAssignmentCompatibleWith(mg.getType())) ) {
                //    constraintViolated(o, "Type on stack top which should be returned is a '"+stack().peek()+
                //    "' which is not assignment compatible with the return type of this method, '"+mg.getType()+"'.");
                //}
                ConstraintViolated(o, "Current method has return type of '" + mg.GetType() + "' expecting a '"
                                      + method_type + "' on top of the stack. But stack top is a '" + Stack().Peek()
                                      + "'.");
            }
        }

        /* "special"visitXXXX methods for one type of instruction each */
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitAALOAD(AALOAD o)
        {
            var arrayref = Stack().Peek(1);
            var index = Stack().Peek(0);
            IndexOfInt(o, index);
            if (ArrayrefOfArrayType(o, arrayref))
                if (!(((ArrayType) arrayref).GetElementType() is ReferenceType
                    ))
                    ConstraintViolated(o,
                        "The 'arrayref' does not refer to an array with elements of a ReferenceType but to an array of "
                        + ((ArrayType) arrayref).GetElementType() + ".");
        }

        //referenceTypeIsInitialized(o, (ReferenceType) (((ArrayType) arrayref).getElementType()));
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitAASTORE(AASTORE o)
        {
            var arrayref = Stack().Peek(2);
            var index = Stack().Peek(1);
            var value = Stack().Peek(0);
            IndexOfInt(o, index);
            if (!(value is ReferenceType))
                ConstraintViolated(o, "The 'value' is not of a ReferenceType but of type " + value
                                                                                           + ".");
            //referenceTypeIsInitialized(o, (ReferenceType) value);
            // Don't bother further with "referenceTypeIsInitialized()", there are no arrays
            // of an uninitialized object type.
            if (ArrayrefOfArrayType(o, arrayref))
                if (!(((ArrayType) arrayref).GetElementType() is ReferenceType
                    ))
                    ConstraintViolated(o,
                        "The 'arrayref' does not refer to an array with elements of a ReferenceType but to an array of "
                        + ((ArrayType) arrayref).GetElementType() + ".");
        }

        // No check for array element assignment compatibility. This is done at runtime.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitACONST_NULL(ACONST_NULL o)
        {
        }

        // Nothing needs to be done here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitALOAD(ALOAD o)
        {
        }

        //visitLoadInstruction(LoadInstruction) is called before.
        // Nothing else needs to be done here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitANEWARRAY(ANEWARRAY o)
        {
            if (!Stack().Peek().Equals(Type.INT))
                ConstraintViolated(o, "The 'count' at the stack top is not of type '" + Type
                                          .INT + "' but of type '" + Stack().Peek() + "'.");
        }

        // The runtime constant pool item at that index must be a symbolic reference to a class,
        // array, or interface type. See Pass 3a.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitARETURN(ARETURN o)
        {
            if (!(Stack().Peek() is ReferenceType))
                ConstraintViolated(o, "The 'objectref' at the stack top is not of a ReferenceType but of type '"
                                      + Stack().Peek() + "'.");
            var objectref = (ReferenceType) Stack().Peek
                ();
            ReferenceTypeIsInitialized(o, objectref);
        }

        // The check below should already done via visitReturnInstruction(ReturnInstruction), see there.
        // It cannot be done using Staerk-et-al's "set of object types" instead of a
        // "wider cast object type", anyway.
        //if (! objectref.isAssignmentCompatibleWith(mg.getReturnType() )) {
        //    constraintViolated(o, "The 'objectref' type "+objectref+
        // " at the stack top is not assignment compatible with the return type '"+mg.getReturnType()+"' of the method.");
        //}
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitARRAYLENGTH(ARRAYLENGTH o)
        {
            var arrayref = Stack().Peek(0);
            ArrayrefOfArrayType(o, arrayref);
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitASTORE(ASTORE o)
        {
            if (!(Stack().Peek() is ReferenceType || Stack().Peek() is ReturnaddressType))
                ConstraintViolated(o, "The 'objectref' is not of a ReferenceType or of ReturnaddressType but of "
                                      + Stack().Peek() + ".");
        }

        //if (stack().peek() instanceof ReferenceType) {
        //    referenceTypeIsInitialized(o, (ReferenceType) (stack().peek()) );
        //}
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitATHROW(ATHROW o)
        {
            try
            {
                // It's stated that 'objectref' must be of a ReferenceType --- but since Throwable is
                // not derived from an ArrayType, it follows that 'objectref' must be of an ObjectType or Type.NULL.
                if (!(Stack().Peek() is ObjectType || Stack().Peek().Equals(Type
                          .NULL)))
                    ConstraintViolated(o, "The 'objectref' is not of an (initialized) ObjectType but of type "
                                          + Stack().Peek() + ".");
                // NULL is a subclass of every class, so to speak.
                if (Stack().Peek().Equals(Type.NULL)) return;
                var exc = (ObjectType) Stack().Peek();
                var throwable = (ObjectType) Type
                    .GetType("Ljava/lang/Throwable;");
                if (!exc.SubclassOf(throwable) && !exc.Equals(throwable))
                    ConstraintViolated(o,
                        "The 'objectref' is not of class Throwable or of a subclass of Throwable, but of '"
                        + Stack().Peek() + "'.");
            }
            catch (TypeLoadException e)
            {
                // FIXME: maybe not the best way to handle this
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitBALOAD(BALOAD o)
        {
            var arrayref = Stack().Peek(1);
            var index = Stack().Peek(0);
            IndexOfInt(o, index);
            if (ArrayrefOfArrayType(o, arrayref))
                if (!(((ArrayType) arrayref).GetElementType().Equals(Type
                          .BOOLEAN) || ((ArrayType) arrayref).GetElementType().Equals(Type
                          .BYTE)))
                    ConstraintViolated(o,
                        "The 'arrayref' does not refer to an array with elements of a Type.BYTE or Type.BOOLEAN but to an array of '"
                        + ((ArrayType) arrayref).GetElementType() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitBASTORE(BASTORE o)
        {
            var arrayref = Stack().Peek(2);
            var index = Stack().Peek(1);
            var value = Stack().Peek(0);
            IndexOfInt(o, index);
            ValueOfInt(o, value);
            if (ArrayrefOfArrayType(o, arrayref))
                if (!(((ArrayType) arrayref).GetElementType().Equals(Type
                          .BOOLEAN) || ((ArrayType) arrayref).GetElementType().Equals(Type
                          .BYTE)))
                    ConstraintViolated(o,
                        "The 'arrayref' does not refer to an array with elements of a Type.BYTE or Type.BOOLEAN but to an array of '"
                        + ((ArrayType) arrayref).GetElementType() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitBIPUSH(BIPUSH o)
        {
        }

        // Nothing to do...
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitBREAKPOINT(BREAKPOINT o)
        {
            throw new AssertionViolatedException(
                "In this JustIce verification pass there should not occur an illegal instruction such as BREAKPOINT."
            );
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitCALOAD(CALOAD o)
        {
            var arrayref = Stack().Peek(1);
            var index = Stack().Peek(0);
            IndexOfInt(o, index);
            ArrayrefOfArrayType(o, arrayref);
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitCASTORE(CASTORE o)
        {
            var arrayref = Stack().Peek(2);
            var index = Stack().Peek(1);
            var value = Stack().Peek(0);
            IndexOfInt(o, index);
            ValueOfInt(o, value);
            if (ArrayrefOfArrayType(o, arrayref))
                if (!((ArrayType) arrayref).GetElementType().Equals(Type
                    .CHAR))
                    ConstraintViolated(o,
                        "The 'arrayref' does not refer to an array with elements of type char but to an array of type "
                        + ((ArrayType) arrayref).GetElementType() + ".");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitCHECKCAST(CHECKCAST o)
        {
            // The objectref must be of type reference.
            var objectref = Stack().Peek(0);
            if (!(objectref is ReferenceType))
                ConstraintViolated(o, "The 'objectref' is not of a ReferenceType but of type " +
                                      objectref + ".");
            //else{
            //    referenceTypeIsInitialized(o, (ReferenceType) objectref);
            //}
            // The unsigned indexbyte1 and indexbyte2 are used to construct an index into the runtime constant pool of the
            // current class (�3.6), where the value of the index is (indexbyte1 << 8) | indexbyte2. The runtime constant
            // pool item at the index must be a symbolic reference to a class, array, or interface type.
            var c = cpg.GetConstant(o.GetIndex());
            if (!(c is ConstantClass))
                ConstraintViolated(o, "The Constant at 'index' is not a ConstantClass, but '" + c
                                                                                              + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitD2F(D2F o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitD2I(D2I o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitD2L(D2L o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDADD(DADD o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDALOAD(DALOAD o)
        {
            IndexOfInt(o, Stack().Peek());
            if (Stack().Peek(1) == Type.NULL) return;
            if (!(Stack().Peek(1) is ArrayType))
                ConstraintViolated(o, "Stack next-to-top must be of type double[] but is '" + Stack
                                          ().Peek(1) + "'.");
            var t = ((ArrayType) Stack().Peek(1)).GetBasicType(
            );
            if (t != Type.DOUBLE)
                ConstraintViolated(o, "Stack next-to-top must be of type double[] but is '" + Stack
                                          ().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDASTORE(DASTORE o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
            IndexOfInt(o, Stack().Peek(1));
            if (Stack().Peek(2) == Type.NULL) return;
            if (!(Stack().Peek(2) is ArrayType))
                ConstraintViolated(o, "Stack next-to-next-to-top must be of type double[] but is '"
                                      + Stack().Peek(2) + "'.");
            var t = ((ArrayType) Stack().Peek(2)).GetBasicType(
            );
            if (t != Type.DOUBLE)
                ConstraintViolated(o, "Stack next-to-next-to-top must be of type double[] but is '"
                                      + Stack().Peek(2) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDCMPG(DCMPG o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDCMPL(DCMPL o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDCONST(DCONST o)
        {
        }

        // There's nothing to be done here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDDIV(DDIV o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDLOAD(DLOAD o)
        {
        }

        //visitLoadInstruction(LoadInstruction) is called before.
        // Nothing else needs to be done here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDMUL(DMUL o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDNEG(DNEG o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDREM(DREM o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDRETURN(DRETURN o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDSTORE(DSTORE o)
        {
        }

        //visitStoreInstruction(StoreInstruction) is called before.
        // Nothing else needs to be done here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDSUB(DSUB o)
        {
            if (Stack().Peek() != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.DOUBLE)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDUP(DUP o)
        {
            if (Stack().Peek().GetSize() != 1)
                ConstraintViolated(o,
                    "Won't DUP type on stack top '" + Stack().Peek() +
                    "' because it must occupy exactly one slot, not '"
                    + Stack().Peek().GetSize() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDUP_X1(DUP_X1 o)
        {
            if (Stack().Peek().GetSize() != 1)
                ConstraintViolated(o, "Type on stack top '" + Stack().Peek() + "' should occupy exactly one slot, not '"
                                      + Stack().Peek().GetSize() + "'.");
            if (Stack().Peek(1).GetSize() != 1)
                ConstraintViolated(o,
                    "Type on stack next-to-top '" + Stack().Peek(1) + "' should occupy exactly one slot, not '"
                    + Stack().Peek(1).GetSize() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDUP_X2(DUP_X2 o)
        {
            if (Stack().Peek().GetSize() != 1)
                ConstraintViolated(o, "Stack top type must be of size 1, but is '" + Stack().Peek
                                          () + "' of size '" + Stack().Peek().GetSize() + "'.");
            if (Stack().Peek(1).GetSize() == 2) return;
            // Form 2, okay.
            //stack().peek(1).getSize == 1.
            if (Stack().Peek(2).GetSize() != 1)
                ConstraintViolated(o, "If stack top's size is 1 and stack next-to-top's size is 1,"
                                      + " stack next-to-next-to-top's size must also be 1, but is: '" + Stack().Peek(
                                          2) + "' of size '" + Stack().Peek(2).GetSize() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDUP2(DUP2 o)
        {
            if (Stack().Peek().GetSize() == 2) return;
            // Form 2, okay.
            //stack().peek().getSize() == 1.
            if (Stack().Peek(1).GetSize() != 1)
                ConstraintViolated(o,
                    "If stack top's size is 1, then stack next-to-top's size must also be 1. But it is '"
                    + Stack().Peek(1) + "' of size '" + Stack().Peek(1).GetSize() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDUP2_X1(DUP2_X1 o)
        {
            if (Stack().Peek().GetSize() == 2)
            {
                if (Stack().Peek(1).GetSize() != 1)
                    ConstraintViolated(o,
                        "If stack top's size is 2, then stack next-to-top's size must be 1. But it is '"
                        + Stack().Peek(1) + "' of size '" + Stack().Peek(1).GetSize() + "'.");
                else
                    return;
            }
            else
            {
                // Form 2
                // stack top is of size 1
                if (Stack().Peek(1).GetSize() != 1)
                    ConstraintViolated(o,
                        "If stack top's size is 1, then stack next-to-top's size must also be 1. But it is '"
                        + Stack().Peek(1) + "' of size '" + Stack().Peek(1).GetSize() + "'.");
                if (Stack().Peek(2).GetSize() != 1)
                    ConstraintViolated(o,
                        "If stack top's size is 1, then stack next-to-next-to-top's size must also be 1. But it is '"
                        + Stack().Peek(2) + "' of size '" + Stack().Peek(2).GetSize() + "'.");
            }
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitDUP2_X2(DUP2_X2 o)
        {
            if (Stack().Peek(0).GetSize() == 2)
            {
                if (Stack().Peek(1).GetSize() == 2) return;
                // Form 4
                // stack top size is 2, next-to-top's size is 1
                if (Stack().Peek(2).GetSize() != 1)
                    ConstraintViolated(o, "If stack top's size is 2 and stack-next-to-top's size is 1,"
                                          + " then stack next-to-next-to-top's size must also be 1. But it is '" +
                                          Stack(
                                          ).Peek(2) + "' of size '" + Stack().Peek(2).GetSize() + "'.");
                else
                    return;
            }
            else if (Stack().Peek(1).GetSize() == 1)
            {
                // Form 2
                // stack top is of size 1
                if (Stack().Peek(2).GetSize() == 2) return;
                // Form 3
                if (Stack().Peek(3).GetSize() == 1) return;
            }

            // Form 1
            ConstraintViolated(o,
                "The operand sizes on the stack do not match any of the four forms of usage of this instruction."
            );
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitF2D(F2D o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitF2I(F2I o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitF2L(F2L o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFADD(FADD o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFALOAD(FALOAD o)
        {
            IndexOfInt(o, Stack().Peek());
            if (Stack().Peek(1) == Type.NULL) return;
            if (!(Stack().Peek(1) is ArrayType))
                ConstraintViolated(o, "Stack next-to-top must be of type float[] but is '" + Stack
                                          ().Peek(1) + "'.");
            var t = ((ArrayType) Stack().Peek(1)).GetBasicType(
            );
            if (t != Type.FLOAT)
                ConstraintViolated(o, "Stack next-to-top must be of type float[] but is '" + Stack
                                          ().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFASTORE(FASTORE o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
            IndexOfInt(o, Stack().Peek(1));
            if (Stack().Peek(2) == Type.NULL) return;
            if (!(Stack().Peek(2) is ArrayType))
                ConstraintViolated(o, "Stack next-to-next-to-top must be of type float[] but is '"
                                      + Stack().Peek(2) + "'.");
            var t = ((ArrayType) Stack().Peek(2)).GetBasicType(
            );
            if (t != Type.FLOAT)
                ConstraintViolated(o, "Stack next-to-next-to-top must be of type float[] but is '"
                                      + Stack().Peek(2) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFCMPG(FCMPG o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFCMPL(FCMPL o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFCONST(FCONST o)
        {
        }

        // nothing to do here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFDIV(FDIV o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFLOAD(FLOAD o)
        {
        }

        //visitLoadInstruction(LoadInstruction) is called before.
        // Nothing else needs to be done here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFMUL(FMUL o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFNEG(FNEG o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFREM(FREM o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFRETURN(FRETURN o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFSTORE(FSTORE o)
        {
        }

        //visitStoreInstruction(StoreInstruction) is called before.
        // Nothing else needs to be done here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitFSUB(FSUB o)
        {
            if (Stack().Peek() != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.FLOAT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        private ObjectType GetObjectType(FieldInstruction o)
        {
            var rt = o.GetReferenceType(cpg);
            if (rt is ObjectType) return (ObjectType) rt;
            ConstraintViolated(o, "expecting ObjectType but got " + rt);
            return null;
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitGETFIELD(GETFIELD o)
        {
            try
            {
                var objectref = Stack().Peek();
                if (!(objectref is ObjectType || objectref == Type
                          .NULL))
                    ConstraintViolated(o,
                        "Stack top should be an object reference that's not an array reference, but is '"
                        + objectref + "'.");
                var field_name = o.GetFieldName(cpg);
                var jc = Repository.LookupClass(GetObjectType(o).GetClassName
                    ());
                var fields = jc.GetFields();
                Field f = null;
                foreach (var field in fields)
                    if (field.GetName().Equals(field_name))
                    {
                        var f_type = Type.GetType(field.GetSignature());
                        var o_type = o.GetType(cpg);
                        /* TODO: Check if assignment compatibility is sufficient.
                        * What does Sun do?
                        */
                        if (f_type.Equals(o_type))
                        {
                            f = field;
                            break;
                        }
                    }

                if (f == null)
                {
                    var superclasses = jc.GetSuperClasses();
                    foreach (var superclass in superclasses)
                    {
                        fields = superclass.GetFields();
                        foreach (var field in fields)
                            if (field.GetName().Equals(field_name))
                            {
                                var f_type = Type.GetType(field.GetSignature());
                                var o_type = o.GetType(cpg);
                                if (f_type.Equals(o_type))
                                {
                                    f = field;
                                    if ((f.GetAccessFlags() & (Const.ACC_PUBLIC | Const.ACC_PROTECTED)) ==
                                        0)
                                        f = null;
                                    goto outer_break;
                                }
                            }
                    }

                    outer_break: ;
                    if (f == null)
                        throw new AssertionViolatedException("Field '" + field_name +
                                                             "' not found in " + jc.GetClassName());
                }

                if (f.IsProtected())
                {
                    var classtype = GetObjectType(o);
                    var curr = ObjectType.GetInstance(mg.GetClassName
                        ());
                    if (classtype.Equals(curr) || curr.SubclassOf(classtype))
                    {
                        var t = Stack().Peek();
                        if (t == Type.NULL) return;
                        if (!(t is ObjectType))
                            ConstraintViolated(o,
                                "The 'objectref' must refer to an object that's not an array. Found instead: '"
                                + t + "'.");
                        var objreftype = (ObjectType) t;
                        if (!(objreftype.Equals(curr) || objreftype.SubclassOf(curr)))
                        {
                        }
                    }
                }

                //TODO: One day move to Staerk-et-al's "Set of object types" instead of "wider" object types
                //      created during the verification.
                //      "Wider" object types don't allow us to check for things like that below.
                //constraintViolated(o, "The referenced field has the ACC_PROTECTED modifier, "+
                // "and it's a member of the current class or a superclass of the current class."+
                // " However, the referenced object type '"+stack().peek()+
                // "' is not the current class or a subclass of the current class.");
                // TODO: Could go into Pass 3a.
                if (f.IsStatic())
                    ConstraintViolated(o, "Referenced field '" + f + "' is static which it shouldn't be."
                    );
            }
            catch (TypeLoadException e)
            {
                // FIXME: maybe not the best way to handle this
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitGETSTATIC(GETSTATIC o)
        {
        }

        // Field must be static: see Pass 3a.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitGOTO(GOTO o)
        {
        }

        // nothing to do here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitGOTO_W(GOTO_W o)
        {
        }

        // nothing to do here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitI2B(I2B o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitI2C(I2C o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitI2D(I2D o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitI2F(I2F o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitI2L(I2L o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitI2S(I2S o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIADD(IADD o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIALOAD(IALOAD o)
        {
            IndexOfInt(o, Stack().Peek());
            if (Stack().Peek(1) == Type.NULL) return;
            if (!(Stack().Peek(1) is ArrayType))
                ConstraintViolated(o, "Stack next-to-top must be of type int[] but is '" + Stack(
                                      ).Peek(1) + "'.");
            var t = ((ArrayType) Stack().Peek(1)).GetBasicType(
            );
            if (t != Type.INT)
                ConstraintViolated(o, "Stack next-to-top must be of type int[] but is '" + Stack(
                                      ).Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIAND(IAND o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIASTORE(IASTORE o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            IndexOfInt(o, Stack().Peek(1));
            if (Stack().Peek(2) == Type.NULL) return;
            if (!(Stack().Peek(2) is ArrayType))
                ConstraintViolated(o, "Stack next-to-next-to-top must be of type int[] but is '"
                                      + Stack().Peek(2) + "'.");
            var t = ((ArrayType) Stack().Peek(2)).GetBasicType(
            );
            if (t != Type.INT)
                ConstraintViolated(o, "Stack next-to-next-to-top must be of type int[] but is '"
                                      + Stack().Peek(2) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitICONST(ICONST o)
        {
        }

        //nothing to do here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIDIV(IDIV o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIF_ACMPEQ(IF_ACMPEQ o)
        {
            if (!(Stack().Peek() is ReferenceType))
                ConstraintViolated(o, "The value at the stack top is not of a ReferenceType, but of type '"
                                      + Stack().Peek() + "'.");
            //referenceTypeIsInitialized(o, (ReferenceType) (stack().peek()) );
            if (!(Stack().Peek(1) is ReferenceType))
                ConstraintViolated(o, "The value at the stack next-to-top is not of a ReferenceType, but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        //referenceTypeIsInitialized(o, (ReferenceType) (stack().peek(1)) );
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIF_ACMPNE(IF_ACMPNE o)
        {
            if (!(Stack().Peek() is ReferenceType))
                ConstraintViolated(o, "The value at the stack top is not of a ReferenceType, but of type '"
                                      + Stack().Peek() + "'.");
            //referenceTypeIsInitialized(o, (ReferenceType) (stack().peek()) );
            if (!(Stack().Peek(1) is ReferenceType))
                ConstraintViolated(o, "The value at the stack next-to-top is not of a ReferenceType, but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        //referenceTypeIsInitialized(o, (ReferenceType) (stack().peek(1)) );
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIF_ICMPEQ(IF_ICMPEQ o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIF_ICMPGE(IF_ICMPGE o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIF_ICMPGT(IF_ICMPGT o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIF_ICMPLE(IF_ICMPLE o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIF_ICMPLT(IF_ICMPLT o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIF_ICMPNE(IF_ICMPNE o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIFEQ(IFEQ o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIFGE(IFGE o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIFGT(IFGT o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIFLE(IFLE o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIFLT(IFLT o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIFNE(IFNE o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIFNONNULL(IFNONNULL o)
        {
            if (!(Stack().Peek() is ReferenceType))
                ConstraintViolated(o, "The value at the stack top is not of a ReferenceType, but of type '"
                                      + Stack().Peek() + "'.");
            ReferenceTypeIsInitialized(o, (ReferenceType) Stack().Peek());
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIFNULL(IFNULL o)
        {
            if (!(Stack().Peek() is ReferenceType))
                ConstraintViolated(o, "The value at the stack top is not of a ReferenceType, but of type '"
                                      + Stack().Peek() + "'.");
            ReferenceTypeIsInitialized(o, (ReferenceType) Stack().Peek());
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIINC(IINC o)
        {
            // Mhhh. In BCEL, at this time "IINC" is not a LocalVariableInstruction.
            if (Locals().MaxLocals() <= (o.GetType(cpg).GetSize() == 1
                    ? o.GetIndex()
                    : o.GetIndex
                          () + 1))
                ConstraintViolated(o, "The 'index' is not a valid index into the local variable array."
                );
            IndexOfInt(o, Locals().Get(o.GetIndex()));
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitILOAD(ILOAD o)
        {
        }

        // All done by visitLocalVariableInstruction(), visitLoadInstruction()
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIMPDEP1(IMPDEP1 o)
        {
            throw new AssertionViolatedException(
                "In this JustIce verification pass there should not occur an illegal instruction such as IMPDEP1."
            );
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIMPDEP2(IMPDEP2 o)
        {
            throw new AssertionViolatedException(
                "In this JustIce verification pass there should not occur an illegal instruction such as IMPDEP2."
            );
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIMUL(IMUL o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitINEG(INEG o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitINSTANCEOF(INSTANCEOF o)
        {
            // The objectref must be of type reference.
            var objectref = Stack().Peek(0);
            if (!(objectref is ReferenceType))
                ConstraintViolated(o, "The 'objectref' is not of a ReferenceType but of type " +
                                      objectref + ".");
            //else{
            //    referenceTypeIsInitialized(o, (ReferenceType) objectref);
            //}
            // The unsigned indexbyte1 and indexbyte2 are used to construct an index into the runtime constant pool of the
            // current class (�3.6), where the value of the index is (indexbyte1 << 8) | indexbyte2. The runtime constant
            // pool item at the index must be a symbolic reference to a class, array, or interface type.
            var c = cpg.GetConstant(o.GetIndex());
            if (!(c is ConstantClass))
                ConstraintViolated(o, "The Constant at 'index' is not a ConstantClass, but '" + c
                                                                                              + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        /// <since>6.0</since>
        public override void VisitINVOKEDYNAMIC(INVOKEDYNAMIC o)
        {
            throw new Exception("INVOKEDYNAMIC instruction is not supported at this time"
            );
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitINVOKEINTERFACE(INVOKEINTERFACE o)
        {
            // Method is not native, otherwise pass 3 would not happen.
            var count = o.GetCount();
            if (count == 0) ConstraintViolated(o, "The 'count' argument must not be 0.");
            // It is a ConstantInterfaceMethodref, Pass 3a made it sure.
            // TODO: Do we want to do anything with it?
            //ConstantInterfaceMethodref cimr = (ConstantInterfaceMethodref) (cpg.getConstant(o.getIndex()));
            // the o.getClassType(cpg) type has passed pass 2; see visitLoadClass(o).
            var t = o.GetType(cpg);
            if (t is ObjectType)
            {
                var name = ((ObjectType) t).GetClassName();
                var v = VerifierFactory.GetVerifier(name);
                var vr = v.DoPass2();
                if (vr.GetStatus() != VerificationResult.VERIFIED_OK)
                    ConstraintViolated(o, "Class '" + name + "' is referenced, but cannot be loaded and resolved: '"
                                          + vr + "'.");
            }

            var argtypes = o.GetArgumentTypes(cpg);
            var nargs = argtypes.Length;
            for (var i = nargs - 1; i >= 0; i--)
            {
                var fromStack = Stack().Peek(nargs - 1 - i);
                // 0 to nargs-1
                var fromDesc = argtypes[i];
                if (fromDesc == Type.BOOLEAN || fromDesc == Type.BYTE
                                             || fromDesc == Type.CHAR || fromDesc == Type.SHORT)
                    fromDesc = Type.INT;
                if (!fromStack.Equals(fromDesc))
                {
                    if (fromStack is ReferenceType && fromDesc is ReferenceType)
                    {
                        var rFromStack = (ReferenceType) fromStack;
                        //ReferenceType rFromDesc = (ReferenceType) fromDesc;
                        // TODO: This can only be checked when using Staerk-et-al's "set of object types"
                        // instead of a "wider cast object type" created during verification.
                        //if ( ! rFromStack.isAssignmentCompatibleWith(rFromDesc) ) {
                        //    constraintViolated(o, "Expecting a '"+fromDesc+"' but found a '"+fromStack+
                        //    "' on the stack (which is not assignment compatible).");
                        //}
                        ReferenceTypeIsInitialized(o, rFromStack);
                    }
                    else
                    {
                        ConstraintViolated(o, "Expecting a '" + fromDesc + "' but found a '" + fromStack
                                              + "' on the stack.");
                    }
                }
            }

            var objref = Stack().Peek(nargs);
            if (objref == Type.NULL) return;
            if (!(objref is ReferenceType))
                ConstraintViolated(o, "Expecting a reference type as 'objectref' on the stack, not a '"
                                      + objref + "'.");
            ReferenceTypeIsInitialized(o, (ReferenceType) objref);
            if (!(objref is ObjectType))
            {
                if (!(objref is ArrayType))
                    // could be a ReturnaddressType
                    ConstraintViolated(o, "Expecting an ObjectType as 'objectref' on the stack, not a '"
                                          + objref + "'.");
                else
                    objref = GENERIC_ARRAY;
            }

            // String objref_classname = ((ObjectType) objref).getClassName();
            // String theInterface = o.getClassName(cpg);
            // TODO: This can only be checked if we're using Staerk-et-al's "set of object types"
            //       instead of "wider cast object types" generated during verification.
            //if ( ! Repository.implementationOf(objref_classname, theInterface) ) {
            //    constraintViolated(o, "The 'objref' item '"+objref+"' does not implement '"+theInterface+"' as expected.");
            //}
            var counted_count = 1;
            // 1 for the objectref
            for (var i = 0; i < nargs; i++) counted_count += argtypes[i].GetSize();
            if (count != counted_count)
                ConstraintViolated(o, "The 'count' argument should probably read '" + counted_count
                                                                                    + "' but is '" + count + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitINVOKESPECIAL(INVOKESPECIAL o)
        {
            try
            {
                // Don't init an object twice.
                if (o.GetMethodName(cpg).Equals(Const.CONSTRUCTOR_NAME) && !(Stack().Peek
                                (o.GetArgumentTypes(cpg).Length) is UninitializedObjectType
                        ))
                    ConstraintViolated(o,
                        "Possibly initializing object twice." + " A valid instruction sequence must not have an uninitialized object on the operand stack or in a local variable"
                                                              + " during a backwards branch, or in a local variable in code protected by an exception handler."
                                                              + " Please see The Java Virtual Machine Specification, Second Edition, 4.9.4 (pages 147 and 148) for details."
                    );
                // the o.getClassType(cpg) type has passed pass 2; see visitLoadClass(o).
                var t = o.GetType(cpg);
                if (t is ObjectType)
                {
                    var name = ((ObjectType) t).GetClassName();
                    var v = VerifierFactory.GetVerifier(name);
                    var vr = v.DoPass2();
                    if (vr.GetStatus() != VerificationResult.VERIFIED_OK)
                        ConstraintViolated(o, "Class '" + name + "' is referenced, but cannot be loaded and resolved: '"
                                              + vr + "'.");
                }

                var argtypes = o.GetArgumentTypes(cpg);
                var nargs = argtypes.Length;
                for (var i = nargs - 1; i >= 0; i--)
                {
                    var fromStack = Stack().Peek(nargs - 1 - i);
                    // 0 to nargs-1
                    var fromDesc = argtypes[i];
                    if (fromDesc == Type.BOOLEAN || fromDesc == Type.BYTE
                                                 || fromDesc == Type.CHAR || fromDesc == Type.SHORT)
                        fromDesc = Type.INT;
                    if (!fromStack.Equals(fromDesc))
                    {
                        if (fromStack is ReferenceType && fromDesc is ReferenceType)
                        {
                            var rFromStack = (ReferenceType) fromStack;
                            var rFromDesc = (ReferenceType) fromDesc;
                            // TODO: This can only be checked using Staerk-et-al's "set of object types", not
                            // using a "wider cast object type".
                            if (!rFromStack.IsAssignmentCompatibleWith(rFromDesc))
                                ConstraintViolated(o, "Expecting a '" + fromDesc + "' but found a '" + fromStack
                                                      + "' on the stack (which is not assignment compatible).");
                            ReferenceTypeIsInitialized(o, rFromStack);
                        }
                        else
                        {
                            ConstraintViolated(o, "Expecting a '" + fromDesc + "' but found a '" + fromStack
                                                  + "' on the stack.");
                        }
                    }
                }

                var objref = Stack().Peek(nargs);
                if (objref == Type.NULL) return;
                if (!(objref is ReferenceType))
                    ConstraintViolated(o, "Expecting a reference type as 'objectref' on the stack, not a '"
                                          + objref + "'.");
                string objref_classname = null;
                if (!o.GetMethodName(cpg).Equals(Const.CONSTRUCTOR_NAME))
                {
                    ReferenceTypeIsInitialized(o, (ReferenceType) objref);
                    if (!(objref is ObjectType))
                    {
                        if (!(objref is ArrayType))
                            // could be a ReturnaddressType
                            ConstraintViolated(o, "Expecting an ObjectType as 'objectref' on the stack, not a '"
                                                  + objref + "'.");
                        else
                            objref = GENERIC_ARRAY;
                    }

                    objref_classname = ((ObjectType) objref).GetClassName();
                }
                else
                {
                    if (!(objref is UninitializedObjectType))
                        ConstraintViolated(o,
                            "Expecting an UninitializedObjectType as 'objectref' on the stack, not a '"
                            + objref +
                            "'. Otherwise, you couldn't invoke a method since an array has no methods (not to speak of a return address)."
                        );
                    objref_classname = ((UninitializedObjectType) objref).GetInitialized
                        ().GetClassName();
                }

                var theClass = o.GetClassName(cpg);
                if (!Repository.InstanceOf(objref_classname, theClass))
                    ConstraintViolated(o, "The 'objref' item '" + objref + "' does not implement '" +
                                          theClass + "' as expected.");
            }
            catch (TypeLoadException e)
            {
                // FIXME: maybe not the best way to handle this
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitINVOKESTATIC(INVOKESTATIC o)
        {
            try
            {
                // Method is not native, otherwise pass 3 would not happen.
                var t = o.GetType(cpg);
                if (t is ObjectType)
                {
                    var name = ((ObjectType) t).GetClassName();
                    var v = VerifierFactory.GetVerifier(name);
                    var vr = v.DoPass2();
                    if (vr.GetStatus() != VerificationResult.VERIFIED_OK)
                        ConstraintViolated(o, "Class '" + name + "' is referenced, but cannot be loaded and resolved: '"
                                              + vr + "'.");
                }

                var argtypes = o.GetArgumentTypes(cpg);
                var nargs = argtypes.Length;
                for (var i = nargs - 1; i >= 0; i--)
                {
                    var fromStack = Stack().Peek(nargs - 1 - i);
                    // 0 to nargs-1
                    var fromDesc = argtypes[i];
                    if (fromDesc == Type.BOOLEAN || fromDesc == Type.BYTE
                                                 || fromDesc == Type.CHAR || fromDesc == Type.SHORT)
                        fromDesc = Type.INT;
                    if (!fromStack.Equals(fromDesc))
                    {
                        if (fromStack is ReferenceType && fromDesc is ReferenceType)
                        {
                            var rFromStack = (ReferenceType) fromStack;
                            var rFromDesc = (ReferenceType) fromDesc;
                            // TODO: This check can possibly only be done using Staerk-et-al's "set of object types"
                            // instead of a "wider cast object type" created during verification.
                            if (!rFromStack.IsAssignmentCompatibleWith(rFromDesc))
                                ConstraintViolated(o, "Expecting a '" + fromDesc + "' but found a '" + fromStack
                                                      + "' on the stack (which is not assignment compatible).");
                            ReferenceTypeIsInitialized(o, rFromStack);
                        }
                        else
                        {
                            ConstraintViolated(o, "Expecting a '" + fromDesc + "' but found a '" + fromStack
                                                  + "' on the stack.");
                        }
                    }
                }
            }
            catch (TypeLoadException e)
            {
                // FIXME: maybe not the best way to handle this
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitINVOKEVIRTUAL(INVOKEVIRTUAL o)
        {
            try
            {
                // the o.getClassType(cpg) type has passed pass 2; see visitLoadClass(o).
                var t = o.GetType(cpg);
                if (t is ObjectType)
                {
                    var name = ((ObjectType) t).GetClassName();
                    var v = VerifierFactory.GetVerifier(name);
                    var vr = v.DoPass2();
                    if (vr.GetStatus() != VerificationResult.VERIFIED_OK)
                        ConstraintViolated(o, "Class '" + name + "' is referenced, but cannot be loaded and resolved: '"
                                              + vr + "'.");
                }

                var argtypes = o.GetArgumentTypes(cpg);
                var nargs = argtypes.Length;
                for (var i = nargs - 1; i >= 0; i--)
                {
                    var fromStack = Stack().Peek(nargs - 1 - i);
                    // 0 to nargs-1
                    var fromDesc = argtypes[i];
                    if (fromDesc == Type.BOOLEAN || fromDesc == Type.BYTE
                                                 || fromDesc == Type.CHAR || fromDesc == Type.SHORT)
                        fromDesc = Type.INT;
                    if (!fromStack.Equals(fromDesc))
                    {
                        if (fromStack is ReferenceType && fromDesc is ReferenceType)
                        {
                            var rFromStack = (ReferenceType) fromStack;
                            var rFromDesc = (ReferenceType) fromDesc;
                            // TODO: This can possibly only be checked when using Staerk-et-al's "set of object types" instead
                            // of a single "wider cast object type" created during verification.
                            if (!rFromStack.IsAssignmentCompatibleWith(rFromDesc))
                                ConstraintViolated(o, "Expecting a '" + fromDesc + "' but found a '" + fromStack
                                                      + "' on the stack (which is not assignment compatible).");
                            ReferenceTypeIsInitialized(o, rFromStack);
                        }
                        else
                        {
                            ConstraintViolated(o, "Expecting a '" + fromDesc + "' but found a '" + fromStack
                                                  + "' on the stack.");
                        }
                    }
                }

                var objref = Stack().Peek(nargs);
                if (objref == Type.NULL) return;
                if (!(objref is ReferenceType))
                    ConstraintViolated(o, "Expecting a reference type as 'objectref' on the stack, not a '"
                                          + objref + "'.");
                ReferenceTypeIsInitialized(o, (ReferenceType) objref);
                if (!(objref is ObjectType))
                {
                    if (!(objref is ArrayType))
                        // could be a ReturnaddressType
                        ConstraintViolated(o, "Expecting an ObjectType as 'objectref' on the stack, not a '"
                                              + objref + "'.");
                    else
                        objref = GENERIC_ARRAY;
                }

                var objref_classname = ((ObjectType) objref).GetClassName();
                var theClass = o.GetClassName(cpg);
                if (!Repository.InstanceOf(objref_classname, theClass))
                    ConstraintViolated(o, "The 'objref' item '" + objref + "' does not implement '" +
                                          theClass + "' as expected.");
            }
            catch (TypeLoadException e)
            {
                // FIXME: maybe not the best way to handle this
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIOR(IOR o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIREM(IREM o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIRETURN(IRETURN o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitISHL(ISHL o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitISHR(ISHR o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitISTORE(ISTORE o)
        {
        }

        //visitStoreInstruction(StoreInstruction) is called before.
        // Nothing else needs to be done here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitISUB(ISUB o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIUSHR(IUSHR o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitIXOR(IXOR o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.INT)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitJSR(JSR o)
        {
        }

        // nothing to do here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitJSR_W(JSR_W o)
        {
        }

        // nothing to do here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitL2D(L2D o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitL2F(L2F o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitL2I(L2I o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLADD(LADD o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.LONG)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLALOAD(LALOAD o)
        {
            IndexOfInt(o, Stack().Peek());
            if (Stack().Peek(1) == Type.NULL) return;
            if (!(Stack().Peek(1) is ArrayType))
                ConstraintViolated(o, "Stack next-to-top must be of type long[] but is '" + Stack
                                          ().Peek(1) + "'.");
            var t = ((ArrayType) Stack().Peek(1)).GetBasicType(
            );
            if (t != Type.LONG)
                ConstraintViolated(o, "Stack next-to-top must be of type long[] but is '" + Stack
                                          ().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLAND(LAND o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.LONG)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLASTORE(LASTORE o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
            IndexOfInt(o, Stack().Peek(1));
            if (Stack().Peek(2) == Type.NULL) return;
            if (!(Stack().Peek(2) is ArrayType))
                ConstraintViolated(o, "Stack next-to-next-to-top must be of type long[] but is '"
                                      + Stack().Peek(2) + "'.");
            var t = ((ArrayType) Stack().Peek(2)).GetBasicType(
            );
            if (t != Type.LONG)
                ConstraintViolated(o, "Stack next-to-next-to-top must be of type long[] but is '"
                                      + Stack().Peek(2) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLCMP(LCMP o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.LONG)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLCONST(LCONST o)
        {
        }

        // Nothing to do here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLDC(LDC o)
        {
            // visitCPInstruction is called first.
            var c = cpg.GetConstant(o.GetIndex());
            if (!(c is ConstantInteger || c is ConstantFloat || c is ConstantString || c is ConstantClass))
                ConstraintViolated(o,
                    "Referenced constant should be a CONSTANT_Integer, a CONSTANT_Float, a CONSTANT_String or a CONSTANT_Class, but is '"
                    + c + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public virtual void VisitLDC_W(LDC_W o)
        {
            // visitCPInstruction is called first.
            var c = cpg.GetConstant(o.GetIndex());
            if (!(c is ConstantInteger || c is ConstantFloat || c is ConstantString || c is ConstantClass))
                ConstraintViolated(o,
                    "Referenced constant should be a CONSTANT_Integer, a CONSTANT_Float, a CONSTANT_String or a CONSTANT_Class, but is '"
                    + c + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLDC2_W(LDC2_W o)
        {
            // visitCPInstruction is called first.
            var c = cpg.GetConstant(o.GetIndex());
            if (!(c is ConstantLong || c is ConstantDouble))
                ConstraintViolated(o,
                    "Referenced constant should be a CONSTANT_Integer, a CONSTANT_Float or a CONSTANT_String, but is '"
                    + c + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLDIV(LDIV o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.LONG)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLLOAD(LLOAD o)
        {
        }

        //visitLoadInstruction(LoadInstruction) is called before.
        // Nothing else needs to be done here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLMUL(LMUL o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.LONG)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLNEG(LNEG o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLOOKUPSWITCH(LOOKUPSWITCH o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        // See also pass 3a.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLOR(LOR o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.LONG)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLREM(LREM o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.LONG)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLRETURN(LRETURN o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLSHL(LSHL o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.LONG)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLSHR(LSHR o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.LONG)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLSTORE(LSTORE o)
        {
        }

        //visitStoreInstruction(StoreInstruction) is called before.
        // Nothing else needs to be done here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLSUB(LSUB o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.LONG)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLUSHR(LUSHR o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.LONG)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitLXOR(LXOR o)
        {
            if (Stack().Peek() != Type.LONG)
                ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
                                      + Stack().Peek() + "'.");
            if (Stack().Peek(1) != Type.LONG)
                ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
                                      + Stack().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitMONITORENTER(MONITORENTER o)
        {
            if (!(Stack().Peek() is ReferenceType))
                ConstraintViolated(o, "The stack top should be of a ReferenceType, but is '" + Stack
                                          ().Peek() + "'.");
        }

        //referenceTypeIsInitialized(o, (ReferenceType) (stack().peek()) );
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitMONITOREXIT(MONITOREXIT o)
        {
            if (!(Stack().Peek() is ReferenceType))
                ConstraintViolated(o, "The stack top should be of a ReferenceType, but is '" + Stack
                                          ().Peek() + "'.");
        }

        //referenceTypeIsInitialized(o, (ReferenceType) (stack().peek()) );
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitMULTIANEWARRAY(MULTIANEWARRAY o)
        {
            int dimensions = o.GetDimensions();
            // Dimensions argument is okay: see Pass 3a.
            for (var i = 0; i < dimensions; i++)
                if (Stack().Peek(i) != Type.INT)
                    ConstraintViolated(o, "The '" + dimensions + "' upper stack types should be 'int' but aren't."
                    );
        }

        // The runtime constant pool item at that index must be a symbolic reference to a class,
        // array, or interface type. See Pass 3a.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitNEW(NEW o)
        {
            //visitCPInstruction(CPInstruction) has been called before.
            //visitLoadClass(LoadClass) has been called before.
            var t = o.GetType(cpg);
            if (!(t is ReferenceType))
                throw new AssertionViolatedException("NEW.getType() returning a non-reference type?!"
                );
            if (!(t is ObjectType))
                ConstraintViolated(o, "Expecting a class type (ObjectType) to work on. Found: '"
                                      + t + "'.");
            var obj = (ObjectType) t;
            //e.g.: Don't instantiate interfaces
            try
            {
                if (!obj.ReferencesClassExact())
                    ConstraintViolated(o, "Expecting a class type (ObjectType) to work on. Found: '"
                                          + obj + "'.");
            }
            catch (TypeLoadException e)
            {
                ConstraintViolated(o, "Expecting a class type (ObjectType) to work on. Found: '"
                                      + obj + "'." + " which threw " + e);
            }
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitNEWARRAY(NEWARRAY o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitNOP(NOP o)
        {
        }

        // nothing is to be done here.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitPOP(POP o)
        {
            if (Stack().Peek().GetSize() != 1)
                ConstraintViolated(o, "Stack top size should be 1 but stack top is '" + Stack().Peek
                                          () + "' of size '" + Stack().Peek().GetSize() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitPOP2(POP2 o)
        {
            if (Stack().Peek().GetSize() != 2)
                ConstraintViolated(o, "Stack top size should be 2 but stack top is '" + Stack().Peek
                                          () + "' of size '" + Stack().Peek().GetSize() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitPUTFIELD(PUTFIELD o)
        {
            try
            {
                var objectref = Stack().Peek(1);
                if (!(objectref is ObjectType || objectref == Type
                          .NULL))
                    ConstraintViolated(o,
                        "Stack next-to-top should be an object reference that's not an array reference, but is '"
                        + objectref + "'.");
                var field_name = o.GetFieldName(cpg);
                var jc = Repository.LookupClass(GetObjectType(o).GetClassName
                    ());
                var fields = jc.GetFields();
                Field f = null;
                foreach (var field in fields)
                    if (field.GetName().Equals(field_name))
                    {
                        var f_type = Type.GetType(field.GetSignature());
                        var o_type = o.GetType(cpg);
                        /* TODO: Check if assignment compatibility is sufficient.
                        * What does Sun do?
                        */
                        if (f_type.Equals(o_type))
                        {
                            f = field;
                            break;
                        }
                    }

                if (f == null)
                    throw new AssertionViolatedException("Field '" + field_name +
                                                         "' not found in " + jc.GetClassName());
                var value = Stack().Peek();
                var t = Type.GetType(f.GetSignature());
                var shouldbe = t;
                if (shouldbe == Type.BOOLEAN || shouldbe == Type.BYTE
                                             || shouldbe == Type.CHAR || shouldbe == Type.SHORT)
                    shouldbe = Type.INT;
                if (t is ReferenceType)
                {
                    ReferenceType rvalue = null;
                    if (value is ReferenceType)
                    {
                        rvalue = (ReferenceType) value;
                        ReferenceTypeIsInitialized(o, rvalue);
                    }
                    else
                    {
                        ConstraintViolated(o,
                            "The stack top type '" + value + "' is not of a reference type as expected."
                        );
                    }

                    // TODO: This can possibly only be checked using Staerk-et-al's "set-of-object types", not
                    // using "wider cast object types" created during verification.
                    // Comment it out if you encounter problems. See also the analogon at visitPUTSTATIC.
                    if (!rvalue.IsAssignmentCompatibleWith(shouldbe))
                        ConstraintViolated(o, "The stack top type '" + value + "' is not assignment compatible with '"
                                              + shouldbe + "'.");
                }
                else if (shouldbe != value)
                {
                    ConstraintViolated(o, "The stack top type '" + value + "' is not of type '" + shouldbe
                                          + "' as expected.");
                }

                if (f.IsProtected())
                {
                    var classtype = GetObjectType(o);
                    var curr = ObjectType.GetInstance(mg.GetClassName
                        ());
                    if (classtype.Equals(curr) || curr.SubclassOf(classtype))
                    {
                        var tp = Stack().Peek(1);
                        if (tp == Type.NULL) return;
                        if (!(tp is ObjectType))
                            ConstraintViolated(o,
                                "The 'objectref' must refer to an object that's not an array. Found instead: '"
                                + tp + "'.");
                        var objreftype = (ObjectType) tp;
                        if (!(objreftype.Equals(curr) || objreftype.SubclassOf(curr)))
                            ConstraintViolated(o,
                                "The referenced field has the ACC_PROTECTED modifier, and it's a member of the current class or"
                                + " a superclass of the current class. However, the referenced object type '" +
                                Stack().Peek() + "' is not the current class or a subclass of the current class."
                            );
                    }
                }

                // TODO: Could go into Pass 3a.
                if (f.IsStatic())
                    ConstraintViolated(o, "Referenced field '" + f + "' is static which it shouldn't be."
                    );
            }
            catch (TypeLoadException e)
            {
                // FIXME: maybe not the best way to handle this
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitPUTSTATIC(PUTSTATIC o)
        {
            try
            {
                var field_name = o.GetFieldName(cpg);
                var jc = Repository.LookupClass(GetObjectType(o).GetClassName
                    ());
                var fields = jc.GetFields();
                Field f = null;
                foreach (var field in fields)
                    if (field.GetName().Equals(field_name))
                    {
                        var f_type = Type.GetType(field.GetSignature());
                        var o_type = o.GetType(cpg);
                        /* TODO: Check if assignment compatibility is sufficient.
                        * What does Sun do?
                        */
                        if (f_type.Equals(o_type))
                        {
                            f = field;
                            break;
                        }
                    }

                if (f == null)
                    throw new AssertionViolatedException("Field '" + field_name +
                                                         "' not found in " + jc.GetClassName());
                var value = Stack().Peek();
                var t = Type.GetType(f.GetSignature());
                var shouldbe = t;
                if (shouldbe == Type.BOOLEAN || shouldbe == Type.BYTE
                                             || shouldbe == Type.CHAR || shouldbe == Type.SHORT)
                    shouldbe = Type.INT;
                if (t is ReferenceType)
                {
                    ReferenceType rvalue = null;
                    if (value is ReferenceType)
                    {
                        rvalue = (ReferenceType) value;
                        ReferenceTypeIsInitialized(o, rvalue);
                    }
                    else
                    {
                        ConstraintViolated(o,
                            "The stack top type '" + value + "' is not of a reference type as expected."
                        );
                    }

                    // TODO: This can possibly only be checked using Staerk-et-al's "set-of-object types", not
                    // using "wider cast object types" created during verification.
                    // Comment it out if you encounter problems. See also the analogon at visitPUTFIELD.
                    if (!rvalue.IsAssignmentCompatibleWith(shouldbe))
                        ConstraintViolated(o, "The stack top type '" + value + "' is not assignment compatible with '"
                                              + shouldbe + "'.");
                }
                else if (shouldbe != value)
                {
                    ConstraintViolated(o, "The stack top type '" + value + "' is not of type '" + shouldbe
                                          + "' as expected.");
                }
            }
            catch (TypeLoadException e)
            {
                // TODO: Interface fields may be assigned to only once. (Hard to implement in
                //       JustIce's execution model). This may only happen in <clinit>, see Pass 3a.
                // FIXME: maybe not the best way to handle this
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitRET(RET o)
        {
            if (!(Locals().Get(o.GetIndex()) is ReturnaddressType))
                ConstraintViolated(o, "Expecting a ReturnaddressType in local variable " + o.GetIndex
                                          () + ".");
            if (Locals().Get(o.GetIndex()) == ReturnaddressType.NO_TARGET)
                throw new AssertionViolatedException("Oops: RET expecting a target!"
                );
        }

        // Other constraints such as non-allowed overlapping subroutines are enforced
        // while building the Subroutines data structure.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitRETURN(RETURN o)
        {
            if (mg.GetName().Equals(Const.CONSTRUCTOR_NAME))
                // If we leave an <init> method
                if (Frame.GetThis() != null && !mg.GetClassName().Equals(Type.OBJECT.GetClassName()))
                    ConstraintViolated(o, "Leaving a constructor that itself did not call a constructor."
                    );
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitSALOAD(SALOAD o)
        {
            IndexOfInt(o, Stack().Peek());
            if (Stack().Peek(1) == Type.NULL) return;
            if (!(Stack().Peek(1) is ArrayType))
                ConstraintViolated(o, "Stack next-to-top must be of type short[] but is '" + Stack
                                          ().Peek(1) + "'.");
            var t = ((ArrayType) Stack().Peek(1)).GetBasicType(
            );
            if (t != Type.SHORT)
                ConstraintViolated(o, "Stack next-to-top must be of type short[] but is '" + Stack
                                          ().Peek(1) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitSASTORE(SASTORE o)
        {
            if (Stack().Peek() != Type.INT)
                ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
                                      + Stack().Peek() + "'.");
            IndexOfInt(o, Stack().Peek(1));
            if (Stack().Peek(2) == Type.NULL) return;
            if (!(Stack().Peek(2) is ArrayType))
                ConstraintViolated(o, "Stack next-to-next-to-top must be of type short[] but is '"
                                      + Stack().Peek(2) + "'.");
            var t = ((ArrayType) Stack().Peek(2)).GetBasicType(
            );
            if (t != Type.SHORT)
                ConstraintViolated(o, "Stack next-to-next-to-top must be of type short[] but is '"
                                      + Stack().Peek(2) + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitSIPUSH(SIPUSH o)
        {
        }

        // nothing to do here. Generic visitXXX() methods did the trick before.
        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitSWAP(SWAP o)
        {
            if (Stack().Peek().GetSize() != 1)
                ConstraintViolated(o, "The value at the stack top is not of size '1', but of size '"
                                      + Stack().Peek().GetSize() + "'.");
            if (Stack().Peek(1).GetSize() != 1)
                ConstraintViolated(o, "The value at the stack next-to-top is not of size '1', but of size '"
                                      + Stack().Peek(1).GetSize() + "'.");
        }

        /// <summary>Ensures the specific preconditions of the said instruction.</summary>
        public override void VisitTABLESWITCH(TABLESWITCH o)
        {
            IndexOfInt(o, Stack().Peek());
        }

        // See Pass 3a.
    }
}