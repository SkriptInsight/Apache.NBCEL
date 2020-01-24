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

namespace NBCEL.verifier.structurals
{
	/// <summary>A Visitor class testing for valid preconditions of JVM instructions.</summary>
	/// <remarks>
	/// A Visitor class testing for valid preconditions of JVM instructions.
	/// The instance of this class will throw a StructuralCodeConstraintException
	/// instance if an instruction is visitXXX()ed which has preconditions that are
	/// not satisfied.
	/// TODO: Currently, the JVM's behavior concerning monitors (MONITORENTER,
	/// MONITOREXIT) is not modeled in JustIce.
	/// </remarks>
	/// <seealso cref="NBCEL.verifier.exc.StructuralCodeConstraintException"/>
	public class InstConstraintVisitor : NBCEL.generic.EmptyVisitor
	{
		private static readonly NBCEL.generic.ObjectType GENERIC_ARRAY = NBCEL.generic.ObjectType
			.GetInstance(typeof(NBCEL.verifier.structurals.GenericArray).FullName);

		/// <summary>The constructor.</summary>
		/// <remarks>The constructor. Constructs a new instance of this class.</remarks>
		public InstConstraintVisitor()
		{
		}

		/// <summary>The Execution Frame we're working on.</summary>
		/// <seealso cref="SetFrame(Frame)"/>
		/// <seealso cref="Locals()"/>
		/// <seealso cref="Stack()"/>
		private NBCEL.verifier.structurals.Frame frame = null;

		/// <summary>The ConstantPoolGen we're working on.</summary>
		/// <seealso cref="SetConstantPoolGen(NBCEL.generic.ConstantPoolGen)"/>
		private NBCEL.generic.ConstantPoolGen cpg = null;

		/// <summary>The MethodGen we're working on.</summary>
		/// <seealso cref="SetMethodGen(NBCEL.generic.MethodGen)"/>
		private NBCEL.generic.MethodGen mg = null;

		//CHECKSTYLE:OFF (there are lots of references!)
		//CHECKSTYLE:ON
		/// <summary>The OperandStack we're working on.</summary>
		/// <seealso cref="SetFrame(Frame)"/>
		private NBCEL.verifier.structurals.OperandStack Stack()
		{
			return frame.GetStack();
		}

		/// <summary>The LocalVariables we're working on.</summary>
		/// <seealso cref="SetFrame(Frame)"/>
		private NBCEL.verifier.structurals.LocalVariables Locals()
		{
			return frame.GetLocals();
		}

		/// <summary>
		/// This method is called by the visitXXX() to notify the acceptor of this InstConstraintVisitor
		/// that a constraint violation has occured.
		/// </summary>
		/// <remarks>
		/// This method is called by the visitXXX() to notify the acceptor of this InstConstraintVisitor
		/// that a constraint violation has occured. This is done by throwing an instance of a
		/// StructuralCodeConstraintException.
		/// </remarks>
		/// <exception cref="NBCEL.verifier.exc.StructuralCodeConstraintException">always.</exception>
		private void ConstraintViolated(NBCEL.generic.Instruction violator, string description
			)
		{
			string fq_classname = violator.GetType().FullName;
			throw new NBCEL.verifier.exc.StructuralCodeConstraintException("Instruction " + Sharpen.Runtime.Substring
				(fq_classname, fq_classname.LastIndexOf('.') + 1) + " constraint violated: " + description
				);
		}

		/// <summary>This returns the single instance of the InstConstraintVisitor class.</summary>
		/// <remarks>
		/// This returns the single instance of the InstConstraintVisitor class.
		/// To operate correctly, other values must have been set before actually
		/// using the instance.
		/// Use this method for performance reasons.
		/// </remarks>
		/// <seealso cref="SetConstantPoolGen(NBCEL.generic.ConstantPoolGen)"/>
		/// <seealso cref="SetMethodGen(NBCEL.generic.MethodGen)"/>
		public virtual void SetFrame(NBCEL.verifier.structurals.Frame f)
		{
			// TODO could be package-protected?
			this.frame = f;
		}

		//if (singleInstance.mg == null || singleInstance.cpg == null)
		// throw new AssertionViolatedException("Forgot to set important values first.");
		/// <summary>
		/// Sets the ConstantPoolGen instance needed for constraint
		/// checking prior to execution.
		/// </summary>
		public virtual void SetConstantPoolGen(NBCEL.generic.ConstantPoolGen cpg)
		{
			// TODO could be package-protected?
			this.cpg = cpg;
		}

		/// <summary>
		/// Sets the MethodGen instance needed for constraint
		/// checking prior to execution.
		/// </summary>
		public virtual void SetMethodGen(NBCEL.generic.MethodGen mg)
		{
			this.mg = mg;
		}

		/// <summary>Assures index is of type INT.</summary>
		/// <exception cref="NBCEL.verifier.exc.StructuralCodeConstraintException">if the above constraint is not satisfied.
		/// 	</exception>
		private void IndexOfInt(NBCEL.generic.Instruction o, NBCEL.generic.Type index)
		{
			if (!index.Equals(NBCEL.generic.Type.INT))
			{
				ConstraintViolated(o, "The 'index' is not of type int but of type " + index + "."
					);
			}
		}

		/// <summary>Assures the ReferenceType r is initialized (or Type.NULL).</summary>
		/// <remarks>
		/// Assures the ReferenceType r is initialized (or Type.NULL).
		/// Formally, this means (!(r instanceof UninitializedObjectType)), because
		/// there are no uninitialized array types.
		/// </remarks>
		/// <exception cref="NBCEL.verifier.exc.StructuralCodeConstraintException">if the above constraint is not satisfied.
		/// 	</exception>
		private void ReferenceTypeIsInitialized(NBCEL.generic.Instruction o, NBCEL.generic.ReferenceType
			 r)
		{
			if (r is NBCEL.verifier.structurals.UninitializedObjectType)
			{
				ConstraintViolated(o, "Working on an uninitialized object '" + r + "'.");
			}
		}

		/// <summary>Assures value is of type INT.</summary>
		private void ValueOfInt(NBCEL.generic.Instruction o, NBCEL.generic.Type value)
		{
			if (!value.Equals(NBCEL.generic.Type.INT))
			{
				ConstraintViolated(o, "The 'value' is not of type int but of type " + value + "."
					);
			}
		}

		/// <summary>
		/// Assures arrayref is of ArrayType or NULL;
		/// returns true if and only if arrayref is non-NULL.
		/// </summary>
		/// <exception cref="NBCEL.verifier.exc.StructuralCodeConstraintException">if the above constraint is violated.
		/// 	</exception>
		private bool ArrayrefOfArrayType(NBCEL.generic.Instruction o, NBCEL.generic.Type 
			arrayref)
		{
			if (!((arrayref is NBCEL.generic.ArrayType) || arrayref.Equals(NBCEL.generic.Type
				.NULL)))
			{
				ConstraintViolated(o, "The 'arrayref' does not refer to an array but is of type "
					 + arrayref + ".");
			}
			return arrayref is NBCEL.generic.ArrayType;
		}

		/* MISC                                                        */
		/// <summary>Ensures the general preconditions of an instruction that accesses the stack.
		/// 	</summary>
		/// <remarks>
		/// Ensures the general preconditions of an instruction that accesses the stack.
		/// This method is here because BCEL has no such superinterface for the stack
		/// accessing instructions; and there are funny unexpected exceptions in the
		/// semantices of the superinterfaces and superclasses provided.
		/// E.g. SWAP is a StackConsumer, but DUP_X1 is not a StackProducer.
		/// Therefore, this method is called by all StackProducer, StackConsumer,
		/// and StackInstruction instances via their visitXXX() method.
		/// Unfortunately, as the superclasses and superinterfaces overlap, some instructions
		/// cause this method to be called two or three times. [TODO: Fix this.]
		/// </remarks>
		/// <seealso cref="VisitStackConsumer(NBCEL.generic.StackConsumer)"/>
		/// <seealso cref="VisitStackProducer(NBCEL.generic.StackProducer)"/>
		/// <seealso cref="VisitStackInstruction(NBCEL.generic.StackInstruction)"/>
		private void _visitStackAccessor(NBCEL.generic.Instruction o)
		{
			int consume = o.ConsumeStack(cpg);
			// Stack values are always consumed first; then produced.
			if (consume > Stack().SlotsUsed())
			{
				ConstraintViolated(o, "Cannot consume " + consume + " stack slots: only " + Stack
					().SlotsUsed() + " slot(s) left on stack!\nStack:\n" + Stack());
			}
			int produce = o.ProduceStack(cpg) - o.ConsumeStack(cpg);
			// Stack values are always consumed first; then produced.
			if (produce + Stack().SlotsUsed() > Stack().MaxStack())
			{
				ConstraintViolated(o, "Cannot produce " + produce + " stack slots: only " + (Stack
					().MaxStack() - Stack().SlotsUsed()) + " free stack slot(s) left.\nStack:\n" + Stack
					());
			}
		}

		/* "generic"visitXXXX methods where XXXX is an interface       */
		/* therefore, we don't know the order of visiting; but we know */
		/* these methods are called before the visitYYYY methods below */
		/// <summary>Assures the generic preconditions of a LoadClass instance.</summary>
		/// <remarks>
		/// Assures the generic preconditions of a LoadClass instance.
		/// The referenced class is loaded and pass2-verified.
		/// </remarks>
		public override void VisitLoadClass(NBCEL.generic.LoadClass o)
		{
			NBCEL.generic.ObjectType t = o.GetLoadClassType(cpg);
			if (t != null)
			{
				// null means "no class is loaded"
				NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(t.GetClassName
					());
				NBCEL.verifier.VerificationResult vr = v.DoPass2();
				if (vr.GetStatus() != NBCEL.verifier.VerificationResult.VERIFIED_OK)
				{
					ConstraintViolated((NBCEL.generic.Instruction)o, "Class '" + o.GetLoadClassType(cpg
						).GetClassName() + "' is referenced, but cannot be loaded and resolved: '" + vr 
						+ "'.");
				}
			}
		}

		/// <summary>Ensures the general preconditions of a StackConsumer instance.</summary>
		public override void VisitStackConsumer(NBCEL.generic.StackConsumer o)
		{
			_visitStackAccessor((NBCEL.generic.Instruction)o);
		}

		/// <summary>Ensures the general preconditions of a StackProducer instance.</summary>
		public override void VisitStackProducer(NBCEL.generic.StackProducer o)
		{
			_visitStackAccessor((NBCEL.generic.Instruction)o);
		}

		/* "generic" visitYYYY methods where YYYY is a superclass.     */
		/* therefore, we know the order of visiting; we know           */
		/* these methods are called after the visitXXXX methods above. */
		/// <summary>Ensures the general preconditions of a CPInstruction instance.</summary>
		public override void VisitCPInstruction(NBCEL.generic.CPInstruction o)
		{
			int idx = o.GetIndex();
			if ((idx < 0) || (idx >= cpg.GetSize()))
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("Huh?! Constant pool index of instruction '"
					 + o + "' illegal? Pass 3a should have checked this!");
			}
		}

		/// <summary>Ensures the general preconditions of a FieldInstruction instance.</summary>
		public override void VisitFieldInstruction(NBCEL.generic.FieldInstruction o)
		{
			// visitLoadClass(o) has been called before: Every FieldOrMethod
			// implements LoadClass.
			// visitCPInstruction(o) has been called before.
			// A FieldInstruction may be: GETFIELD, GETSTATIC, PUTFIELD, PUTSTATIC
			NBCEL.classfile.Constant c = cpg.GetConstant(o.GetIndex());
			if (!(c is NBCEL.classfile.ConstantFieldref))
			{
				ConstraintViolated(o, "Index '" + o.GetIndex() + "' should refer to a CONSTANT_Fieldref_info structure, but refers to '"
					 + c + "'.");
			}
			// the o.getClassType(cpg) type has passed pass 2; see visitLoadClass(o).
			NBCEL.generic.Type t = o.GetType(cpg);
			if (t is NBCEL.generic.ObjectType)
			{
				string name = ((NBCEL.generic.ObjectType)t).GetClassName();
				NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(name);
				NBCEL.verifier.VerificationResult vr = v.DoPass2();
				if (vr.GetStatus() != NBCEL.verifier.VerificationResult.VERIFIED_OK)
				{
					ConstraintViolated(o, "Class '" + name + "' is referenced, but cannot be loaded and resolved: '"
						 + vr + "'.");
				}
			}
		}

		/// <summary>Ensures the general preconditions of an InvokeInstruction instance.</summary>
		public override void VisitInvokeInstruction(NBCEL.generic.InvokeInstruction o)
		{
		}

		// visitLoadClass(o) has been called before: Every FieldOrMethod
		// implements LoadClass.
		// visitCPInstruction(o) has been called before.
		//TODO
		/// <summary>Ensures the general preconditions of a StackInstruction instance.</summary>
		public override void VisitStackInstruction(NBCEL.generic.StackInstruction o)
		{
			_visitStackAccessor(o);
		}

		/// <summary>Assures the generic preconditions of a LocalVariableInstruction instance.
		/// 	</summary>
		/// <remarks>
		/// Assures the generic preconditions of a LocalVariableInstruction instance.
		/// That is, the index of the local variable must be valid.
		/// </remarks>
		public override void VisitLocalVariableInstruction(NBCEL.generic.LocalVariableInstruction
			 o)
		{
			if (Locals().MaxLocals() <= (o.GetType(cpg).GetSize() == 1 ? o.GetIndex() : o.GetIndex
				() + 1))
			{
				ConstraintViolated(o, "The 'index' is not a valid index into the local variable array."
					);
			}
		}

		/// <summary>Assures the generic preconditions of a LoadInstruction instance.</summary>
		public override void VisitLoadInstruction(NBCEL.generic.LoadInstruction o)
		{
			//visitLocalVariableInstruction(o) is called before, because it is more generic.
			// LOAD instructions must not read Type.UNKNOWN
			if (Locals().Get(o.GetIndex()) == NBCEL.generic.Type.UNKNOWN)
			{
				ConstraintViolated(o, "Read-Access on local variable " + o.GetIndex() + " with unknown content."
					);
			}
			// LOAD instructions, two-slot-values at index N must have Type.UNKNOWN
			// as a symbol for the higher halve at index N+1
			// [suppose some instruction put an int at N+1--- our double at N is defective]
			if (o.GetType(cpg).GetSize() == 2)
			{
				if (Locals().Get(o.GetIndex() + 1) != NBCEL.generic.Type.UNKNOWN)
				{
					ConstraintViolated(o, "Reading a two-locals value from local variables " + o.GetIndex
						() + " and " + (o.GetIndex() + 1) + " where the latter one is destroyed.");
				}
			}
			// LOAD instructions must read the correct type.
			if (!(o is NBCEL.generic.ALOAD))
			{
				if (Locals().Get(o.GetIndex()) != o.GetType(cpg))
				{
					ConstraintViolated(o, "Local Variable type and LOADing Instruction type mismatch: Local Variable: '"
						 + Locals().Get(o.GetIndex()) + "'; Instruction type: '" + o.GetType(cpg) + "'."
						);
				}
			}
			else if (!(Locals().Get(o.GetIndex()) is NBCEL.generic.ReferenceType))
			{
				// we deal with an ALOAD
				ConstraintViolated(o, "Local Variable type and LOADing Instruction type mismatch: Local Variable: '"
					 + Locals().Get(o.GetIndex()) + "'; Instruction expects a ReferenceType.");
			}
			// ALOAD __IS ALLOWED__ to put uninitialized objects onto the stack!
			//referenceTypeIsInitialized(o, (ReferenceType) (locals().get(o.getIndex())));
			// LOAD instructions must have enough free stack slots.
			if ((Stack().MaxStack() - Stack().SlotsUsed()) < o.GetType(cpg).GetSize())
			{
				ConstraintViolated(o, "Not enough free stack slots to load a '" + o.GetType(cpg) 
					+ "' onto the OperandStack.");
			}
		}

		/// <summary>Assures the generic preconditions of a StoreInstruction instance.</summary>
		public override void VisitStoreInstruction(NBCEL.generic.StoreInstruction o)
		{
			//visitLocalVariableInstruction(o) is called before, because it is more generic.
			if (Stack().IsEmpty())
			{
				// Don't bother about 1 or 2 stack slots used. This check is implicitly done below while type checking.
				ConstraintViolated(o, "Cannot STORE: Stack to read from is empty.");
			}
			if (!(o is NBCEL.generic.ASTORE))
			{
				if (!(Stack().Peek() == o.GetType(cpg)))
				{
					// the other xSTORE types are singletons in BCEL.
					ConstraintViolated(o, "Stack top type and STOREing Instruction type mismatch: Stack top: '"
						 + Stack().Peek() + "'; Instruction type: '" + o.GetType(cpg) + "'.");
				}
			}
			else
			{
				// we deal with ASTORE
				NBCEL.generic.Type stacktop = Stack().Peek();
				if ((!(stacktop is NBCEL.generic.ReferenceType)) && (!(stacktop is NBCEL.generic.ReturnaddressType
					)))
				{
					ConstraintViolated(o, "Stack top type and STOREing Instruction type mismatch: Stack top: '"
						 + Stack().Peek() + "'; Instruction expects a ReferenceType or a ReturnadressType."
						);
				}
			}
		}

		//if (stacktop instanceof ReferenceType) {
		//    referenceTypeIsInitialized(o, (ReferenceType) stacktop);
		//}
		/// <summary>Assures the generic preconditions of a ReturnInstruction instance.</summary>
		public override void VisitReturnInstruction(NBCEL.generic.ReturnInstruction o)
		{
			NBCEL.generic.Type method_type = mg.GetType();
			if (method_type == NBCEL.generic.Type.BOOLEAN || method_type == NBCEL.generic.Type
				.BYTE || method_type == NBCEL.generic.Type.SHORT || method_type == NBCEL.generic.Type
				.CHAR)
			{
				method_type = NBCEL.generic.Type.INT;
			}
			if (o is NBCEL.generic.RETURN)
			{
				if (method_type != NBCEL.generic.Type.VOID)
				{
					ConstraintViolated(o, "RETURN instruction in non-void method.");
				}
				else
				{
					return;
				}
			}
			if (o is NBCEL.generic.ARETURN)
			{
				if (method_type == NBCEL.generic.Type.VOID)
				{
					ConstraintViolated(o, "ARETURN instruction in void method.");
				}
				if (Stack().Peek() == NBCEL.generic.Type.NULL)
				{
					return;
				}
				if (!(Stack().Peek() is NBCEL.generic.ReferenceType))
				{
					ConstraintViolated(o, "Reference type expected on top of stack, but is: '" + Stack
						().Peek() + "'.");
				}
				ReferenceTypeIsInitialized(o, (NBCEL.generic.ReferenceType)(Stack().Peek()));
			}
			else if (!(method_type.Equals(Stack().Peek())))
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
		public override void VisitAALOAD(NBCEL.generic.AALOAD o)
		{
			NBCEL.generic.Type arrayref = Stack().Peek(1);
			NBCEL.generic.Type index = Stack().Peek(0);
			IndexOfInt(o, index);
			if (ArrayrefOfArrayType(o, arrayref))
			{
				if (!(((NBCEL.generic.ArrayType)arrayref).GetElementType() is NBCEL.generic.ReferenceType
					))
				{
					ConstraintViolated(o, "The 'arrayref' does not refer to an array with elements of a ReferenceType but to an array of "
						 + ((NBCEL.generic.ArrayType)arrayref).GetElementType() + ".");
				}
			}
		}

		//referenceTypeIsInitialized(o, (ReferenceType) (((ArrayType) arrayref).getElementType()));
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitAASTORE(NBCEL.generic.AASTORE o)
		{
			NBCEL.generic.Type arrayref = Stack().Peek(2);
			NBCEL.generic.Type index = Stack().Peek(1);
			NBCEL.generic.Type value = Stack().Peek(0);
			IndexOfInt(o, index);
			if (!(value is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "The 'value' is not of a ReferenceType but of type " + value
					 + ".");
			}
			//referenceTypeIsInitialized(o, (ReferenceType) value);
			// Don't bother further with "referenceTypeIsInitialized()", there are no arrays
			// of an uninitialized object type.
			if (ArrayrefOfArrayType(o, arrayref))
			{
				if (!(((NBCEL.generic.ArrayType)arrayref).GetElementType() is NBCEL.generic.ReferenceType
					))
				{
					ConstraintViolated(o, "The 'arrayref' does not refer to an array with elements of a ReferenceType but to an array of "
						 + ((NBCEL.generic.ArrayType)arrayref).GetElementType() + ".");
				}
			}
		}

		// No check for array element assignment compatibility. This is done at runtime.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitACONST_NULL(NBCEL.generic.ACONST_NULL o)
		{
		}

		// Nothing needs to be done here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitALOAD(NBCEL.generic.ALOAD o)
		{
		}

		//visitLoadInstruction(LoadInstruction) is called before.
		// Nothing else needs to be done here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitANEWARRAY(NBCEL.generic.ANEWARRAY o)
		{
			if (!Stack().Peek().Equals(NBCEL.generic.Type.INT))
			{
				ConstraintViolated(o, "The 'count' at the stack top is not of type '" + NBCEL.generic.Type
					.INT + "' but of type '" + Stack().Peek() + "'.");
			}
		}

		// The runtime constant pool item at that index must be a symbolic reference to a class,
		// array, or interface type. See Pass 3a.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitARETURN(NBCEL.generic.ARETURN o)
		{
			if (!(Stack().Peek() is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "The 'objectref' at the stack top is not of a ReferenceType but of type '"
					 + Stack().Peek() + "'.");
			}
			NBCEL.generic.ReferenceType objectref = (NBCEL.generic.ReferenceType)(Stack().Peek
				());
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
		public override void VisitARRAYLENGTH(NBCEL.generic.ARRAYLENGTH o)
		{
			NBCEL.generic.Type arrayref = Stack().Peek(0);
			ArrayrefOfArrayType(o, arrayref);
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitASTORE(NBCEL.generic.ASTORE o)
		{
			if (!((Stack().Peek() is NBCEL.generic.ReferenceType) || (Stack().Peek() is NBCEL.generic.ReturnaddressType
				)))
			{
				ConstraintViolated(o, "The 'objectref' is not of a ReferenceType or of ReturnaddressType but of "
					 + Stack().Peek() + ".");
			}
		}

		//if (stack().peek() instanceof ReferenceType) {
		//    referenceTypeIsInitialized(o, (ReferenceType) (stack().peek()) );
		//}
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitATHROW(NBCEL.generic.ATHROW o)
		{
			try
			{
				// It's stated that 'objectref' must be of a ReferenceType --- but since Throwable is
				// not derived from an ArrayType, it follows that 'objectref' must be of an ObjectType or Type.NULL.
				if (!((Stack().Peek() is NBCEL.generic.ObjectType) || (Stack().Peek().Equals(NBCEL.generic.Type
					.NULL))))
				{
					ConstraintViolated(o, "The 'objectref' is not of an (initialized) ObjectType but of type "
						 + Stack().Peek() + ".");
				}
				// NULL is a subclass of every class, so to speak.
				if (Stack().Peek().Equals(NBCEL.generic.Type.NULL))
				{
					return;
				}
				NBCEL.generic.ObjectType exc = (NBCEL.generic.ObjectType)(Stack().Peek());
				NBCEL.generic.ObjectType throwable = (NBCEL.generic.ObjectType)(NBCEL.generic.Type
					.GetType("Ljava/lang/Throwable;"));
				if ((!(exc.SubclassOf(throwable))) && (!(exc.Equals(throwable))))
				{
					ConstraintViolated(o, "The 'objectref' is not of class Throwable or of a subclass of Throwable, but of '"
						 + Stack().Peek() + "'.");
				}
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: maybe not the best way to handle this
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitBALOAD(NBCEL.generic.BALOAD o)
		{
			NBCEL.generic.Type arrayref = Stack().Peek(1);
			NBCEL.generic.Type index = Stack().Peek(0);
			IndexOfInt(o, index);
			if (ArrayrefOfArrayType(o, arrayref))
			{
				if (!((((NBCEL.generic.ArrayType)arrayref).GetElementType().Equals(NBCEL.generic.Type
					.BOOLEAN)) || (((NBCEL.generic.ArrayType)arrayref).GetElementType().Equals(NBCEL.generic.Type
					.BYTE))))
				{
					ConstraintViolated(o, "The 'arrayref' does not refer to an array with elements of a Type.BYTE or Type.BOOLEAN but to an array of '"
						 + ((NBCEL.generic.ArrayType)arrayref).GetElementType() + "'.");
				}
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitBASTORE(NBCEL.generic.BASTORE o)
		{
			NBCEL.generic.Type arrayref = Stack().Peek(2);
			NBCEL.generic.Type index = Stack().Peek(1);
			NBCEL.generic.Type value = Stack().Peek(0);
			IndexOfInt(o, index);
			ValueOfInt(o, value);
			if (ArrayrefOfArrayType(o, arrayref))
			{
				if (!((((NBCEL.generic.ArrayType)arrayref).GetElementType().Equals(NBCEL.generic.Type
					.BOOLEAN)) || (((NBCEL.generic.ArrayType)arrayref).GetElementType().Equals(NBCEL.generic.Type
					.BYTE))))
				{
					ConstraintViolated(o, "The 'arrayref' does not refer to an array with elements of a Type.BYTE or Type.BOOLEAN but to an array of '"
						 + ((NBCEL.generic.ArrayType)arrayref).GetElementType() + "'.");
				}
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitBIPUSH(NBCEL.generic.BIPUSH o)
		{
		}

		// Nothing to do...
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitBREAKPOINT(NBCEL.generic.BREAKPOINT o)
		{
			throw new NBCEL.verifier.exc.AssertionViolatedException("In this JustIce verification pass there should not occur an illegal instruction such as BREAKPOINT."
				);
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitCALOAD(NBCEL.generic.CALOAD o)
		{
			NBCEL.generic.Type arrayref = Stack().Peek(1);
			NBCEL.generic.Type index = Stack().Peek(0);
			IndexOfInt(o, index);
			ArrayrefOfArrayType(o, arrayref);
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitCASTORE(NBCEL.generic.CASTORE o)
		{
			NBCEL.generic.Type arrayref = Stack().Peek(2);
			NBCEL.generic.Type index = Stack().Peek(1);
			NBCEL.generic.Type value = Stack().Peek(0);
			IndexOfInt(o, index);
			ValueOfInt(o, value);
			if (ArrayrefOfArrayType(o, arrayref))
			{
				if (!((NBCEL.generic.ArrayType)arrayref).GetElementType().Equals(NBCEL.generic.Type
					.CHAR))
				{
					ConstraintViolated(o, "The 'arrayref' does not refer to an array with elements of type char but to an array of type "
						 + ((NBCEL.generic.ArrayType)arrayref).GetElementType() + ".");
				}
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitCHECKCAST(NBCEL.generic.CHECKCAST o)
		{
			// The objectref must be of type reference.
			NBCEL.generic.Type objectref = Stack().Peek(0);
			if (!(objectref is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "The 'objectref' is not of a ReferenceType but of type " + 
					objectref + ".");
			}
			//else{
			//    referenceTypeIsInitialized(o, (ReferenceType) objectref);
			//}
			// The unsigned indexbyte1 and indexbyte2 are used to construct an index into the runtime constant pool of the
			// current class (�3.6), where the value of the index is (indexbyte1 << 8) | indexbyte2. The runtime constant
			// pool item at the index must be a symbolic reference to a class, array, or interface type.
			NBCEL.classfile.Constant c = cpg.GetConstant(o.GetIndex());
			if (!(c is NBCEL.classfile.ConstantClass))
			{
				ConstraintViolated(o, "The Constant at 'index' is not a ConstantClass, but '" + c
					 + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitD2F(NBCEL.generic.D2F o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitD2I(NBCEL.generic.D2I o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitD2L(NBCEL.generic.D2L o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDADD(NBCEL.generic.DADD o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDALOAD(NBCEL.generic.DALOAD o)
		{
			IndexOfInt(o, Stack().Peek());
			if (Stack().Peek(1) == NBCEL.generic.Type.NULL)
			{
				return;
			}
			if (!(Stack().Peek(1) is NBCEL.generic.ArrayType))
			{
				ConstraintViolated(o, "Stack next-to-top must be of type double[] but is '" + Stack
					().Peek(1) + "'.");
			}
			NBCEL.generic.Type t = ((NBCEL.generic.ArrayType)(Stack().Peek(1))).GetBasicType(
				);
			if (t != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "Stack next-to-top must be of type double[] but is '" + Stack
					().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDASTORE(NBCEL.generic.DASTORE o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
			IndexOfInt(o, Stack().Peek(1));
			if (Stack().Peek(2) == NBCEL.generic.Type.NULL)
			{
				return;
			}
			if (!(Stack().Peek(2) is NBCEL.generic.ArrayType))
			{
				ConstraintViolated(o, "Stack next-to-next-to-top must be of type double[] but is '"
					 + Stack().Peek(2) + "'.");
			}
			NBCEL.generic.Type t = ((NBCEL.generic.ArrayType)(Stack().Peek(2))).GetBasicType(
				);
			if (t != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "Stack next-to-next-to-top must be of type double[] but is '"
					 + Stack().Peek(2) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDCMPG(NBCEL.generic.DCMPG o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDCMPL(NBCEL.generic.DCMPL o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDCONST(NBCEL.generic.DCONST o)
		{
		}

		// There's nothing to be done here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDDIV(NBCEL.generic.DDIV o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDLOAD(NBCEL.generic.DLOAD o)
		{
		}

		//visitLoadInstruction(LoadInstruction) is called before.
		// Nothing else needs to be done here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDMUL(NBCEL.generic.DMUL o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDNEG(NBCEL.generic.DNEG o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDREM(NBCEL.generic.DREM o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDRETURN(NBCEL.generic.DRETURN o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDSTORE(NBCEL.generic.DSTORE o)
		{
		}

		//visitStoreInstruction(StoreInstruction) is called before.
		// Nothing else needs to be done here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDSUB(NBCEL.generic.DSUB o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'double', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.DOUBLE)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'double', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDUP(NBCEL.generic.DUP o)
		{
			if (Stack().Peek().GetSize() != 1)
			{
				ConstraintViolated(o, "Won't DUP type on stack top '" + Stack().Peek() + "' because it must occupy exactly one slot, not '"
					 + Stack().Peek().GetSize() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDUP_X1(NBCEL.generic.DUP_X1 o)
		{
			if (Stack().Peek().GetSize() != 1)
			{
				ConstraintViolated(o, "Type on stack top '" + Stack().Peek() + "' should occupy exactly one slot, not '"
					 + Stack().Peek().GetSize() + "'.");
			}
			if (Stack().Peek(1).GetSize() != 1)
			{
				ConstraintViolated(o, "Type on stack next-to-top '" + Stack().Peek(1) + "' should occupy exactly one slot, not '"
					 + Stack().Peek(1).GetSize() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDUP_X2(NBCEL.generic.DUP_X2 o)
		{
			if (Stack().Peek().GetSize() != 1)
			{
				ConstraintViolated(o, "Stack top type must be of size 1, but is '" + Stack().Peek
					() + "' of size '" + Stack().Peek().GetSize() + "'.");
			}
			if (Stack().Peek(1).GetSize() == 2)
			{
				return;
			}
			// Form 2, okay.
			//stack().peek(1).getSize == 1.
			if (Stack().Peek(2).GetSize() != 1)
			{
				ConstraintViolated(o, "If stack top's size is 1 and stack next-to-top's size is 1,"
					 + " stack next-to-next-to-top's size must also be 1, but is: '" + Stack().Peek(
					2) + "' of size '" + Stack().Peek(2).GetSize() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDUP2(NBCEL.generic.DUP2 o)
		{
			if (Stack().Peek().GetSize() == 2)
			{
				return;
			}
			// Form 2, okay.
			//stack().peek().getSize() == 1.
			if (Stack().Peek(1).GetSize() != 1)
			{
				ConstraintViolated(o, "If stack top's size is 1, then stack next-to-top's size must also be 1. But it is '"
					 + Stack().Peek(1) + "' of size '" + Stack().Peek(1).GetSize() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDUP2_X1(NBCEL.generic.DUP2_X1 o)
		{
			if (Stack().Peek().GetSize() == 2)
			{
				if (Stack().Peek(1).GetSize() != 1)
				{
					ConstraintViolated(o, "If stack top's size is 2, then stack next-to-top's size must be 1. But it is '"
						 + Stack().Peek(1) + "' of size '" + Stack().Peek(1).GetSize() + "'.");
				}
				else
				{
					return;
				}
			}
			else
			{
				// Form 2
				// stack top is of size 1
				if (Stack().Peek(1).GetSize() != 1)
				{
					ConstraintViolated(o, "If stack top's size is 1, then stack next-to-top's size must also be 1. But it is '"
						 + Stack().Peek(1) + "' of size '" + Stack().Peek(1).GetSize() + "'.");
				}
				if (Stack().Peek(2).GetSize() != 1)
				{
					ConstraintViolated(o, "If stack top's size is 1, then stack next-to-next-to-top's size must also be 1. But it is '"
						 + Stack().Peek(2) + "' of size '" + Stack().Peek(2).GetSize() + "'.");
				}
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitDUP2_X2(NBCEL.generic.DUP2_X2 o)
		{
			if (Stack().Peek(0).GetSize() == 2)
			{
				if (Stack().Peek(1).GetSize() == 2)
				{
					return;
				}
				// Form 4
				// stack top size is 2, next-to-top's size is 1
				if (Stack().Peek(2).GetSize() != 1)
				{
					ConstraintViolated(o, "If stack top's size is 2 and stack-next-to-top's size is 1,"
						 + " then stack next-to-next-to-top's size must also be 1. But it is '" + Stack(
						).Peek(2) + "' of size '" + Stack().Peek(2).GetSize() + "'.");
				}
				else
				{
					return;
				}
			}
			else if (Stack().Peek(1).GetSize() == 1)
			{
				// Form 2
				// stack top is of size 1
				if (Stack().Peek(2).GetSize() == 2)
				{
					return;
				}
				// Form 3
				if (Stack().Peek(3).GetSize() == 1)
				{
					return;
				}
			}
			// Form 1
			ConstraintViolated(o, "The operand sizes on the stack do not match any of the four forms of usage of this instruction."
				);
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitF2D(NBCEL.generic.F2D o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitF2I(NBCEL.generic.F2I o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitF2L(NBCEL.generic.F2L o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFADD(NBCEL.generic.FADD o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFALOAD(NBCEL.generic.FALOAD o)
		{
			IndexOfInt(o, Stack().Peek());
			if (Stack().Peek(1) == NBCEL.generic.Type.NULL)
			{
				return;
			}
			if (!(Stack().Peek(1) is NBCEL.generic.ArrayType))
			{
				ConstraintViolated(o, "Stack next-to-top must be of type float[] but is '" + Stack
					().Peek(1) + "'.");
			}
			NBCEL.generic.Type t = ((NBCEL.generic.ArrayType)(Stack().Peek(1))).GetBasicType(
				);
			if (t != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "Stack next-to-top must be of type float[] but is '" + Stack
					().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFASTORE(NBCEL.generic.FASTORE o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
			IndexOfInt(o, Stack().Peek(1));
			if (Stack().Peek(2) == NBCEL.generic.Type.NULL)
			{
				return;
			}
			if (!(Stack().Peek(2) is NBCEL.generic.ArrayType))
			{
				ConstraintViolated(o, "Stack next-to-next-to-top must be of type float[] but is '"
					 + Stack().Peek(2) + "'.");
			}
			NBCEL.generic.Type t = ((NBCEL.generic.ArrayType)(Stack().Peek(2))).GetBasicType(
				);
			if (t != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "Stack next-to-next-to-top must be of type float[] but is '"
					 + Stack().Peek(2) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFCMPG(NBCEL.generic.FCMPG o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFCMPL(NBCEL.generic.FCMPL o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFCONST(NBCEL.generic.FCONST o)
		{
		}

		// nothing to do here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFDIV(NBCEL.generic.FDIV o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFLOAD(NBCEL.generic.FLOAD o)
		{
		}

		//visitLoadInstruction(LoadInstruction) is called before.
		// Nothing else needs to be done here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFMUL(NBCEL.generic.FMUL o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFNEG(NBCEL.generic.FNEG o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFREM(NBCEL.generic.FREM o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFRETURN(NBCEL.generic.FRETURN o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFSTORE(NBCEL.generic.FSTORE o)
		{
		}

		//visitStoreInstruction(StoreInstruction) is called before.
		// Nothing else needs to be done here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitFSUB(NBCEL.generic.FSUB o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'float', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.FLOAT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'float', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		private NBCEL.generic.ObjectType GetObjectType(NBCEL.generic.FieldInstruction o)
		{
			NBCEL.generic.ReferenceType rt = o.GetReferenceType(cpg);
			if (rt is NBCEL.generic.ObjectType)
			{
				return (NBCEL.generic.ObjectType)rt;
			}
			ConstraintViolated(o, "expecting ObjectType but got " + rt);
			return null;
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitGETFIELD(NBCEL.generic.GETFIELD o)
		{
			try
			{
				NBCEL.generic.Type objectref = Stack().Peek();
				if (!((objectref is NBCEL.generic.ObjectType) || (objectref == NBCEL.generic.Type
					.NULL)))
				{
					ConstraintViolated(o, "Stack top should be an object reference that's not an array reference, but is '"
						 + objectref + "'.");
				}
				string field_name = o.GetFieldName(cpg);
				NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(GetObjectType(o).GetClassName
					());
				NBCEL.classfile.Field[] fields = jc.GetFields();
				NBCEL.classfile.Field f = null;
				foreach (NBCEL.classfile.Field field in fields)
				{
					if (field.GetName().Equals(field_name))
					{
						NBCEL.generic.Type f_type = NBCEL.generic.Type.GetType(field.GetSignature());
						NBCEL.generic.Type o_type = o.GetType(cpg);
						/* TODO: Check if assignment compatibility is sufficient.
						* What does Sun do?
						*/
						if (f_type.Equals(o_type))
						{
							f = field;
							break;
						}
					}
				}
				if (f == null)
				{
					NBCEL.classfile.JavaClass[] superclasses = jc.GetSuperClasses();
					foreach (NBCEL.classfile.JavaClass superclass in superclasses)
					{
						fields = superclass.GetFields();
						foreach (NBCEL.classfile.Field field in fields)
						{
							if (field.GetName().Equals(field_name))
							{
								NBCEL.generic.Type f_type = NBCEL.generic.Type.GetType(field.GetSignature());
								NBCEL.generic.Type o_type = o.GetType(cpg);
								if (f_type.Equals(o_type))
								{
									f = field;
									if ((f.GetAccessFlags() & (NBCEL.Const.ACC_PUBLIC | NBCEL.Const.ACC_PROTECTED)) ==
										 0)
									{
										f = null;
									}
									goto outer_break;
								}
							}
						}
					}
outer_break: ;
					if (f == null)
					{
						throw new NBCEL.verifier.exc.AssertionViolatedException("Field '" + field_name + 
							"' not found in " + jc.GetClassName());
					}
				}
				if (f.IsProtected())
				{
					NBCEL.generic.ObjectType classtype = GetObjectType(o);
					NBCEL.generic.ObjectType curr = NBCEL.generic.ObjectType.GetInstance(mg.GetClassName
						());
					if (classtype.Equals(curr) || curr.SubclassOf(classtype))
					{
						NBCEL.generic.Type t = Stack().Peek();
						if (t == NBCEL.generic.Type.NULL)
						{
							return;
						}
						if (!(t is NBCEL.generic.ObjectType))
						{
							ConstraintViolated(o, "The 'objectref' must refer to an object that's not an array. Found instead: '"
								 + t + "'.");
						}
						NBCEL.generic.ObjectType objreftype = (NBCEL.generic.ObjectType)t;
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
				{
					ConstraintViolated(o, "Referenced field '" + f + "' is static which it shouldn't be."
						);
				}
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: maybe not the best way to handle this
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitGETSTATIC(NBCEL.generic.GETSTATIC o)
		{
		}

		// Field must be static: see Pass 3a.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitGOTO(NBCEL.generic.GOTO o)
		{
		}

		// nothing to do here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitGOTO_W(NBCEL.generic.GOTO_W o)
		{
		}

		// nothing to do here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitI2B(NBCEL.generic.I2B o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitI2C(NBCEL.generic.I2C o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitI2D(NBCEL.generic.I2D o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitI2F(NBCEL.generic.I2F o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitI2L(NBCEL.generic.I2L o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitI2S(NBCEL.generic.I2S o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIADD(NBCEL.generic.IADD o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIALOAD(NBCEL.generic.IALOAD o)
		{
			IndexOfInt(o, Stack().Peek());
			if (Stack().Peek(1) == NBCEL.generic.Type.NULL)
			{
				return;
			}
			if (!(Stack().Peek(1) is NBCEL.generic.ArrayType))
			{
				ConstraintViolated(o, "Stack next-to-top must be of type int[] but is '" + Stack(
					).Peek(1) + "'.");
			}
			NBCEL.generic.Type t = ((NBCEL.generic.ArrayType)(Stack().Peek(1))).GetBasicType(
				);
			if (t != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "Stack next-to-top must be of type int[] but is '" + Stack(
					).Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIAND(NBCEL.generic.IAND o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIASTORE(NBCEL.generic.IASTORE o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			IndexOfInt(o, Stack().Peek(1));
			if (Stack().Peek(2) == NBCEL.generic.Type.NULL)
			{
				return;
			}
			if (!(Stack().Peek(2) is NBCEL.generic.ArrayType))
			{
				ConstraintViolated(o, "Stack next-to-next-to-top must be of type int[] but is '" 
					+ Stack().Peek(2) + "'.");
			}
			NBCEL.generic.Type t = ((NBCEL.generic.ArrayType)(Stack().Peek(2))).GetBasicType(
				);
			if (t != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "Stack next-to-next-to-top must be of type int[] but is '" 
					+ Stack().Peek(2) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitICONST(NBCEL.generic.ICONST o)
		{
		}

		//nothing to do here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIDIV(NBCEL.generic.IDIV o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIF_ACMPEQ(NBCEL.generic.IF_ACMPEQ o)
		{
			if (!(Stack().Peek() is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "The value at the stack top is not of a ReferenceType, but of type '"
					 + Stack().Peek() + "'.");
			}
			//referenceTypeIsInitialized(o, (ReferenceType) (stack().peek()) );
			if (!(Stack().Peek(1) is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of a ReferenceType, but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		//referenceTypeIsInitialized(o, (ReferenceType) (stack().peek(1)) );
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIF_ACMPNE(NBCEL.generic.IF_ACMPNE o)
		{
			if (!(Stack().Peek() is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "The value at the stack top is not of a ReferenceType, but of type '"
					 + Stack().Peek() + "'.");
			}
			//referenceTypeIsInitialized(o, (ReferenceType) (stack().peek()) );
			if (!(Stack().Peek(1) is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of a ReferenceType, but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		//referenceTypeIsInitialized(o, (ReferenceType) (stack().peek(1)) );
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIF_ICMPEQ(NBCEL.generic.IF_ICMPEQ o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIF_ICMPGE(NBCEL.generic.IF_ICMPGE o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIF_ICMPGT(NBCEL.generic.IF_ICMPGT o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIF_ICMPLE(NBCEL.generic.IF_ICMPLE o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIF_ICMPLT(NBCEL.generic.IF_ICMPLT o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIF_ICMPNE(NBCEL.generic.IF_ICMPNE o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIFEQ(NBCEL.generic.IFEQ o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIFGE(NBCEL.generic.IFGE o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIFGT(NBCEL.generic.IFGT o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIFLE(NBCEL.generic.IFLE o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIFLT(NBCEL.generic.IFLT o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIFNE(NBCEL.generic.IFNE o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIFNONNULL(NBCEL.generic.IFNONNULL o)
		{
			if (!(Stack().Peek() is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "The value at the stack top is not of a ReferenceType, but of type '"
					 + Stack().Peek() + "'.");
			}
			ReferenceTypeIsInitialized(o, (NBCEL.generic.ReferenceType)(Stack().Peek()));
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIFNULL(NBCEL.generic.IFNULL o)
		{
			if (!(Stack().Peek() is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "The value at the stack top is not of a ReferenceType, but of type '"
					 + Stack().Peek() + "'.");
			}
			ReferenceTypeIsInitialized(o, (NBCEL.generic.ReferenceType)(Stack().Peek()));
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIINC(NBCEL.generic.IINC o)
		{
			// Mhhh. In BCEL, at this time "IINC" is not a LocalVariableInstruction.
			if (Locals().MaxLocals() <= (o.GetType(cpg).GetSize() == 1 ? o.GetIndex() : o.GetIndex
				() + 1))
			{
				ConstraintViolated(o, "The 'index' is not a valid index into the local variable array."
					);
			}
			IndexOfInt(o, Locals().Get(o.GetIndex()));
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitILOAD(NBCEL.generic.ILOAD o)
		{
		}

		// All done by visitLocalVariableInstruction(), visitLoadInstruction()
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIMPDEP1(NBCEL.generic.IMPDEP1 o)
		{
			throw new NBCEL.verifier.exc.AssertionViolatedException("In this JustIce verification pass there should not occur an illegal instruction such as IMPDEP1."
				);
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIMPDEP2(NBCEL.generic.IMPDEP2 o)
		{
			throw new NBCEL.verifier.exc.AssertionViolatedException("In this JustIce verification pass there should not occur an illegal instruction such as IMPDEP2."
				);
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIMUL(NBCEL.generic.IMUL o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitINEG(NBCEL.generic.INEG o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitINSTANCEOF(NBCEL.generic.INSTANCEOF o)
		{
			// The objectref must be of type reference.
			NBCEL.generic.Type objectref = Stack().Peek(0);
			if (!(objectref is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "The 'objectref' is not of a ReferenceType but of type " + 
					objectref + ".");
			}
			//else{
			//    referenceTypeIsInitialized(o, (ReferenceType) objectref);
			//}
			// The unsigned indexbyte1 and indexbyte2 are used to construct an index into the runtime constant pool of the
			// current class (�3.6), where the value of the index is (indexbyte1 << 8) | indexbyte2. The runtime constant
			// pool item at the index must be a symbolic reference to a class, array, or interface type.
			NBCEL.classfile.Constant c = cpg.GetConstant(o.GetIndex());
			if (!(c is NBCEL.classfile.ConstantClass))
			{
				ConstraintViolated(o, "The Constant at 'index' is not a ConstantClass, but '" + c
					 + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		/// <since>6.0</since>
		public override void VisitINVOKEDYNAMIC(NBCEL.generic.INVOKEDYNAMIC o)
		{
			throw new System.Exception("INVOKEDYNAMIC instruction is not supported at this time"
				);
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitINVOKEINTERFACE(NBCEL.generic.INVOKEINTERFACE o)
		{
			// Method is not native, otherwise pass 3 would not happen.
			int count = o.GetCount();
			if (count == 0)
			{
				ConstraintViolated(o, "The 'count' argument must not be 0.");
			}
			// It is a ConstantInterfaceMethodref, Pass 3a made it sure.
			// TODO: Do we want to do anything with it?
			//ConstantInterfaceMethodref cimr = (ConstantInterfaceMethodref) (cpg.getConstant(o.getIndex()));
			// the o.getClassType(cpg) type has passed pass 2; see visitLoadClass(o).
			NBCEL.generic.Type t = o.GetType(cpg);
			if (t is NBCEL.generic.ObjectType)
			{
				string name = ((NBCEL.generic.ObjectType)t).GetClassName();
				NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(name);
				NBCEL.verifier.VerificationResult vr = v.DoPass2();
				if (vr.GetStatus() != NBCEL.verifier.VerificationResult.VERIFIED_OK)
				{
					ConstraintViolated(o, "Class '" + name + "' is referenced, but cannot be loaded and resolved: '"
						 + vr + "'.");
				}
			}
			NBCEL.generic.Type[] argtypes = o.GetArgumentTypes(cpg);
			int nargs = argtypes.Length;
			for (int i = nargs - 1; i >= 0; i--)
			{
				NBCEL.generic.Type fromStack = Stack().Peek((nargs - 1) - i);
				// 0 to nargs-1
				NBCEL.generic.Type fromDesc = argtypes[i];
				if (fromDesc == NBCEL.generic.Type.BOOLEAN || fromDesc == NBCEL.generic.Type.BYTE
					 || fromDesc == NBCEL.generic.Type.CHAR || fromDesc == NBCEL.generic.Type.SHORT)
				{
					fromDesc = NBCEL.generic.Type.INT;
				}
				if (!fromStack.Equals(fromDesc))
				{
					if (fromStack is NBCEL.generic.ReferenceType && fromDesc is NBCEL.generic.ReferenceType)
					{
						NBCEL.generic.ReferenceType rFromStack = (NBCEL.generic.ReferenceType)fromStack;
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
			NBCEL.generic.Type objref = Stack().Peek(nargs);
			if (objref == NBCEL.generic.Type.NULL)
			{
				return;
			}
			if (!(objref is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "Expecting a reference type as 'objectref' on the stack, not a '"
					 + objref + "'.");
			}
			ReferenceTypeIsInitialized(o, (NBCEL.generic.ReferenceType)objref);
			if (!(objref is NBCEL.generic.ObjectType))
			{
				if (!(objref is NBCEL.generic.ArrayType))
				{
					// could be a ReturnaddressType
					ConstraintViolated(o, "Expecting an ObjectType as 'objectref' on the stack, not a '"
						 + objref + "'.");
				}
				else
				{
					objref = GENERIC_ARRAY;
				}
			}
			// String objref_classname = ((ObjectType) objref).getClassName();
			// String theInterface = o.getClassName(cpg);
			// TODO: This can only be checked if we're using Staerk-et-al's "set of object types"
			//       instead of "wider cast object types" generated during verification.
			//if ( ! Repository.implementationOf(objref_classname, theInterface) ) {
			//    constraintViolated(o, "The 'objref' item '"+objref+"' does not implement '"+theInterface+"' as expected.");
			//}
			int counted_count = 1;
			// 1 for the objectref
			for (int i = 0; i < nargs; i++)
			{
				counted_count += argtypes[i].GetSize();
			}
			if (count != counted_count)
			{
				ConstraintViolated(o, "The 'count' argument should probably read '" + counted_count
					 + "' but is '" + count + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitINVOKESPECIAL(NBCEL.generic.INVOKESPECIAL o)
		{
			try
			{
				// Don't init an object twice.
				if ((o.GetMethodName(cpg).Equals(NBCEL.Const.CONSTRUCTOR_NAME)) && (!(Stack().Peek
					(o.GetArgumentTypes(cpg).Length) is NBCEL.verifier.structurals.UninitializedObjectType
					)))
				{
					ConstraintViolated(o, "Possibly initializing object twice." + " A valid instruction sequence must not have an uninitialized object on the operand stack or in a local variable"
						 + " during a backwards branch, or in a local variable in code protected by an exception handler."
						 + " Please see The Java Virtual Machine Specification, Second Edition, 4.9.4 (pages 147 and 148) for details."
						);
				}
				// the o.getClassType(cpg) type has passed pass 2; see visitLoadClass(o).
				NBCEL.generic.Type t = o.GetType(cpg);
				if (t is NBCEL.generic.ObjectType)
				{
					string name = ((NBCEL.generic.ObjectType)t).GetClassName();
					NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(name);
					NBCEL.verifier.VerificationResult vr = v.DoPass2();
					if (vr.GetStatus() != NBCEL.verifier.VerificationResult.VERIFIED_OK)
					{
						ConstraintViolated(o, "Class '" + name + "' is referenced, but cannot be loaded and resolved: '"
							 + vr + "'.");
					}
				}
				NBCEL.generic.Type[] argtypes = o.GetArgumentTypes(cpg);
				int nargs = argtypes.Length;
				for (int i = nargs - 1; i >= 0; i--)
				{
					NBCEL.generic.Type fromStack = Stack().Peek((nargs - 1) - i);
					// 0 to nargs-1
					NBCEL.generic.Type fromDesc = argtypes[i];
					if (fromDesc == NBCEL.generic.Type.BOOLEAN || fromDesc == NBCEL.generic.Type.BYTE
						 || fromDesc == NBCEL.generic.Type.CHAR || fromDesc == NBCEL.generic.Type.SHORT)
					{
						fromDesc = NBCEL.generic.Type.INT;
					}
					if (!fromStack.Equals(fromDesc))
					{
						if (fromStack is NBCEL.generic.ReferenceType && fromDesc is NBCEL.generic.ReferenceType)
						{
							NBCEL.generic.ReferenceType rFromStack = (NBCEL.generic.ReferenceType)fromStack;
							NBCEL.generic.ReferenceType rFromDesc = (NBCEL.generic.ReferenceType)fromDesc;
							// TODO: This can only be checked using Staerk-et-al's "set of object types", not
							// using a "wider cast object type".
							if (!rFromStack.IsAssignmentCompatibleWith(rFromDesc))
							{
								ConstraintViolated(o, "Expecting a '" + fromDesc + "' but found a '" + fromStack 
									+ "' on the stack (which is not assignment compatible).");
							}
							ReferenceTypeIsInitialized(o, rFromStack);
						}
						else
						{
							ConstraintViolated(o, "Expecting a '" + fromDesc + "' but found a '" + fromStack 
								+ "' on the stack.");
						}
					}
				}
				NBCEL.generic.Type objref = Stack().Peek(nargs);
				if (objref == NBCEL.generic.Type.NULL)
				{
					return;
				}
				if (!(objref is NBCEL.generic.ReferenceType))
				{
					ConstraintViolated(o, "Expecting a reference type as 'objectref' on the stack, not a '"
						 + objref + "'.");
				}
				string objref_classname = null;
				if (!(o.GetMethodName(cpg).Equals(NBCEL.Const.CONSTRUCTOR_NAME)))
				{
					ReferenceTypeIsInitialized(o, (NBCEL.generic.ReferenceType)objref);
					if (!(objref is NBCEL.generic.ObjectType))
					{
						if (!(objref is NBCEL.generic.ArrayType))
						{
							// could be a ReturnaddressType
							ConstraintViolated(o, "Expecting an ObjectType as 'objectref' on the stack, not a '"
								 + objref + "'.");
						}
						else
						{
							objref = GENERIC_ARRAY;
						}
					}
					objref_classname = ((NBCEL.generic.ObjectType)objref).GetClassName();
				}
				else
				{
					if (!(objref is NBCEL.verifier.structurals.UninitializedObjectType))
					{
						ConstraintViolated(o, "Expecting an UninitializedObjectType as 'objectref' on the stack, not a '"
							 + objref + "'. Otherwise, you couldn't invoke a method since an array has no methods (not to speak of a return address)."
							);
					}
					objref_classname = ((NBCEL.verifier.structurals.UninitializedObjectType)objref).GetInitialized
						().GetClassName();
				}
				string theClass = o.GetClassName(cpg);
				if (!NBCEL.Repository.InstanceOf(objref_classname, theClass))
				{
					ConstraintViolated(o, "The 'objref' item '" + objref + "' does not implement '" +
						 theClass + "' as expected.");
				}
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: maybe not the best way to handle this
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitINVOKESTATIC(NBCEL.generic.INVOKESTATIC o)
		{
			try
			{
				// Method is not native, otherwise pass 3 would not happen.
				NBCEL.generic.Type t = o.GetType(cpg);
				if (t is NBCEL.generic.ObjectType)
				{
					string name = ((NBCEL.generic.ObjectType)t).GetClassName();
					NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(name);
					NBCEL.verifier.VerificationResult vr = v.DoPass2();
					if (vr.GetStatus() != NBCEL.verifier.VerificationResult.VERIFIED_OK)
					{
						ConstraintViolated(o, "Class '" + name + "' is referenced, but cannot be loaded and resolved: '"
							 + vr + "'.");
					}
				}
				NBCEL.generic.Type[] argtypes = o.GetArgumentTypes(cpg);
				int nargs = argtypes.Length;
				for (int i = nargs - 1; i >= 0; i--)
				{
					NBCEL.generic.Type fromStack = Stack().Peek((nargs - 1) - i);
					// 0 to nargs-1
					NBCEL.generic.Type fromDesc = argtypes[i];
					if (fromDesc == NBCEL.generic.Type.BOOLEAN || fromDesc == NBCEL.generic.Type.BYTE
						 || fromDesc == NBCEL.generic.Type.CHAR || fromDesc == NBCEL.generic.Type.SHORT)
					{
						fromDesc = NBCEL.generic.Type.INT;
					}
					if (!fromStack.Equals(fromDesc))
					{
						if (fromStack is NBCEL.generic.ReferenceType && fromDesc is NBCEL.generic.ReferenceType)
						{
							NBCEL.generic.ReferenceType rFromStack = (NBCEL.generic.ReferenceType)fromStack;
							NBCEL.generic.ReferenceType rFromDesc = (NBCEL.generic.ReferenceType)fromDesc;
							// TODO: This check can possibly only be done using Staerk-et-al's "set of object types"
							// instead of a "wider cast object type" created during verification.
							if (!rFromStack.IsAssignmentCompatibleWith(rFromDesc))
							{
								ConstraintViolated(o, "Expecting a '" + fromDesc + "' but found a '" + fromStack 
									+ "' on the stack (which is not assignment compatible).");
							}
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
			catch (System.TypeLoadException e)
			{
				// FIXME: maybe not the best way to handle this
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitINVOKEVIRTUAL(NBCEL.generic.INVOKEVIRTUAL o)
		{
			try
			{
				// the o.getClassType(cpg) type has passed pass 2; see visitLoadClass(o).
				NBCEL.generic.Type t = o.GetType(cpg);
				if (t is NBCEL.generic.ObjectType)
				{
					string name = ((NBCEL.generic.ObjectType)t).GetClassName();
					NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(name);
					NBCEL.verifier.VerificationResult vr = v.DoPass2();
					if (vr.GetStatus() != NBCEL.verifier.VerificationResult.VERIFIED_OK)
					{
						ConstraintViolated(o, "Class '" + name + "' is referenced, but cannot be loaded and resolved: '"
							 + vr + "'.");
					}
				}
				NBCEL.generic.Type[] argtypes = o.GetArgumentTypes(cpg);
				int nargs = argtypes.Length;
				for (int i = nargs - 1; i >= 0; i--)
				{
					NBCEL.generic.Type fromStack = Stack().Peek((nargs - 1) - i);
					// 0 to nargs-1
					NBCEL.generic.Type fromDesc = argtypes[i];
					if (fromDesc == NBCEL.generic.Type.BOOLEAN || fromDesc == NBCEL.generic.Type.BYTE
						 || fromDesc == NBCEL.generic.Type.CHAR || fromDesc == NBCEL.generic.Type.SHORT)
					{
						fromDesc = NBCEL.generic.Type.INT;
					}
					if (!fromStack.Equals(fromDesc))
					{
						if (fromStack is NBCEL.generic.ReferenceType && fromDesc is NBCEL.generic.ReferenceType)
						{
							NBCEL.generic.ReferenceType rFromStack = (NBCEL.generic.ReferenceType)fromStack;
							NBCEL.generic.ReferenceType rFromDesc = (NBCEL.generic.ReferenceType)fromDesc;
							// TODO: This can possibly only be checked when using Staerk-et-al's "set of object types" instead
							// of a single "wider cast object type" created during verification.
							if (!rFromStack.IsAssignmentCompatibleWith(rFromDesc))
							{
								ConstraintViolated(o, "Expecting a '" + fromDesc + "' but found a '" + fromStack 
									+ "' on the stack (which is not assignment compatible).");
							}
							ReferenceTypeIsInitialized(o, rFromStack);
						}
						else
						{
							ConstraintViolated(o, "Expecting a '" + fromDesc + "' but found a '" + fromStack 
								+ "' on the stack.");
						}
					}
				}
				NBCEL.generic.Type objref = Stack().Peek(nargs);
				if (objref == NBCEL.generic.Type.NULL)
				{
					return;
				}
				if (!(objref is NBCEL.generic.ReferenceType))
				{
					ConstraintViolated(o, "Expecting a reference type as 'objectref' on the stack, not a '"
						 + objref + "'.");
				}
				ReferenceTypeIsInitialized(o, (NBCEL.generic.ReferenceType)objref);
				if (!(objref is NBCEL.generic.ObjectType))
				{
					if (!(objref is NBCEL.generic.ArrayType))
					{
						// could be a ReturnaddressType
						ConstraintViolated(o, "Expecting an ObjectType as 'objectref' on the stack, not a '"
							 + objref + "'.");
					}
					else
					{
						objref = GENERIC_ARRAY;
					}
				}
				string objref_classname = ((NBCEL.generic.ObjectType)objref).GetClassName();
				string theClass = o.GetClassName(cpg);
				if (!NBCEL.Repository.InstanceOf(objref_classname, theClass))
				{
					ConstraintViolated(o, "The 'objref' item '" + objref + "' does not implement '" +
						 theClass + "' as expected.");
				}
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: maybe not the best way to handle this
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIOR(NBCEL.generic.IOR o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIREM(NBCEL.generic.IREM o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIRETURN(NBCEL.generic.IRETURN o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitISHL(NBCEL.generic.ISHL o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitISHR(NBCEL.generic.ISHR o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitISTORE(NBCEL.generic.ISTORE o)
		{
		}

		//visitStoreInstruction(StoreInstruction) is called before.
		// Nothing else needs to be done here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitISUB(NBCEL.generic.ISUB o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIUSHR(NBCEL.generic.IUSHR o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitIXOR(NBCEL.generic.IXOR o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'int', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitJSR(NBCEL.generic.JSR o)
		{
		}

		// nothing to do here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitJSR_W(NBCEL.generic.JSR_W o)
		{
		}

		// nothing to do here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitL2D(NBCEL.generic.L2D o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitL2F(NBCEL.generic.L2F o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitL2I(NBCEL.generic.L2I o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLADD(NBCEL.generic.LADD o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLALOAD(NBCEL.generic.LALOAD o)
		{
			IndexOfInt(o, Stack().Peek());
			if (Stack().Peek(1) == NBCEL.generic.Type.NULL)
			{
				return;
			}
			if (!(Stack().Peek(1) is NBCEL.generic.ArrayType))
			{
				ConstraintViolated(o, "Stack next-to-top must be of type long[] but is '" + Stack
					().Peek(1) + "'.");
			}
			NBCEL.generic.Type t = ((NBCEL.generic.ArrayType)(Stack().Peek(1))).GetBasicType(
				);
			if (t != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "Stack next-to-top must be of type long[] but is '" + Stack
					().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLAND(NBCEL.generic.LAND o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLASTORE(NBCEL.generic.LASTORE o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
			IndexOfInt(o, Stack().Peek(1));
			if (Stack().Peek(2) == NBCEL.generic.Type.NULL)
			{
				return;
			}
			if (!(Stack().Peek(2) is NBCEL.generic.ArrayType))
			{
				ConstraintViolated(o, "Stack next-to-next-to-top must be of type long[] but is '"
					 + Stack().Peek(2) + "'.");
			}
			NBCEL.generic.Type t = ((NBCEL.generic.ArrayType)(Stack().Peek(2))).GetBasicType(
				);
			if (t != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "Stack next-to-next-to-top must be of type long[] but is '"
					 + Stack().Peek(2) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLCMP(NBCEL.generic.LCMP o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLCONST(NBCEL.generic.LCONST o)
		{
		}

		// Nothing to do here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLDC(NBCEL.generic.LDC o)
		{
			// visitCPInstruction is called first.
			NBCEL.classfile.Constant c = cpg.GetConstant(o.GetIndex());
			if (!((c is NBCEL.classfile.ConstantInteger) || (c is NBCEL.classfile.ConstantFloat
				) || (c is NBCEL.classfile.ConstantString) || (c is NBCEL.classfile.ConstantClass
				)))
			{
				ConstraintViolated(o, "Referenced constant should be a CONSTANT_Integer, a CONSTANT_Float, a CONSTANT_String or a CONSTANT_Class, but is '"
					 + c + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public virtual void VisitLDC_W(NBCEL.generic.LDC_W o)
		{
			// visitCPInstruction is called first.
			NBCEL.classfile.Constant c = cpg.GetConstant(o.GetIndex());
			if (!((c is NBCEL.classfile.ConstantInteger) || (c is NBCEL.classfile.ConstantFloat
				) || (c is NBCEL.classfile.ConstantString) || (c is NBCEL.classfile.ConstantClass
				)))
			{
				ConstraintViolated(o, "Referenced constant should be a CONSTANT_Integer, a CONSTANT_Float, a CONSTANT_String or a CONSTANT_Class, but is '"
					 + c + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLDC2_W(NBCEL.generic.LDC2_W o)
		{
			// visitCPInstruction is called first.
			NBCEL.classfile.Constant c = cpg.GetConstant(o.GetIndex());
			if (!((c is NBCEL.classfile.ConstantLong) || (c is NBCEL.classfile.ConstantDouble
				)))
			{
				ConstraintViolated(o, "Referenced constant should be a CONSTANT_Integer, a CONSTANT_Float or a CONSTANT_String, but is '"
					 + c + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLDIV(NBCEL.generic.LDIV o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLLOAD(NBCEL.generic.LLOAD o)
		{
		}

		//visitLoadInstruction(LoadInstruction) is called before.
		// Nothing else needs to be done here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLMUL(NBCEL.generic.LMUL o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLNEG(NBCEL.generic.LNEG o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLOOKUPSWITCH(NBCEL.generic.LOOKUPSWITCH o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		// See also pass 3a.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLOR(NBCEL.generic.LOR o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLREM(NBCEL.generic.LREM o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLRETURN(NBCEL.generic.LRETURN o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLSHL(NBCEL.generic.LSHL o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLSHR(NBCEL.generic.LSHR o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLSTORE(NBCEL.generic.LSTORE o)
		{
		}

		//visitStoreInstruction(StoreInstruction) is called before.
		// Nothing else needs to be done here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLSUB(NBCEL.generic.LSUB o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLUSHR(NBCEL.generic.LUSHR o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitLXOR(NBCEL.generic.LXOR o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'long', but of type '"
					 + Stack().Peek() + "'.");
			}
			if (Stack().Peek(1) != NBCEL.generic.Type.LONG)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of type 'long', but of type '"
					 + Stack().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitMONITORENTER(NBCEL.generic.MONITORENTER o)
		{
			if (!((Stack().Peek()) is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "The stack top should be of a ReferenceType, but is '" + Stack
					().Peek() + "'.");
			}
		}

		//referenceTypeIsInitialized(o, (ReferenceType) (stack().peek()) );
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitMONITOREXIT(NBCEL.generic.MONITOREXIT o)
		{
			if (!((Stack().Peek()) is NBCEL.generic.ReferenceType))
			{
				ConstraintViolated(o, "The stack top should be of a ReferenceType, but is '" + Stack
					().Peek() + "'.");
			}
		}

		//referenceTypeIsInitialized(o, (ReferenceType) (stack().peek()) );
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitMULTIANEWARRAY(NBCEL.generic.MULTIANEWARRAY o)
		{
			int dimensions = o.GetDimensions();
			// Dimensions argument is okay: see Pass 3a.
			for (int i = 0; i < dimensions; i++)
			{
				if (Stack().Peek(i) != NBCEL.generic.Type.INT)
				{
					ConstraintViolated(o, "The '" + dimensions + "' upper stack types should be 'int' but aren't."
						);
				}
			}
		}

		// The runtime constant pool item at that index must be a symbolic reference to a class,
		// array, or interface type. See Pass 3a.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitNEW(NBCEL.generic.NEW o)
		{
			//visitCPInstruction(CPInstruction) has been called before.
			//visitLoadClass(LoadClass) has been called before.
			NBCEL.generic.Type t = o.GetType(cpg);
			if (!(t is NBCEL.generic.ReferenceType))
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("NEW.getType() returning a non-reference type?!"
					);
			}
			if (!(t is NBCEL.generic.ObjectType))
			{
				ConstraintViolated(o, "Expecting a class type (ObjectType) to work on. Found: '" 
					+ t + "'.");
			}
			NBCEL.generic.ObjectType obj = (NBCEL.generic.ObjectType)t;
			//e.g.: Don't instantiate interfaces
			try
			{
				if (!obj.ReferencesClassExact())
				{
					ConstraintViolated(o, "Expecting a class type (ObjectType) to work on. Found: '" 
						+ obj + "'.");
				}
			}
			catch (System.TypeLoadException e)
			{
				ConstraintViolated(o, "Expecting a class type (ObjectType) to work on. Found: '" 
					+ obj + "'." + " which threw " + e);
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitNEWARRAY(NBCEL.generic.NEWARRAY o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitNOP(NBCEL.generic.NOP o)
		{
		}

		// nothing is to be done here.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitPOP(NBCEL.generic.POP o)
		{
			if (Stack().Peek().GetSize() != 1)
			{
				ConstraintViolated(o, "Stack top size should be 1 but stack top is '" + Stack().Peek
					() + "' of size '" + Stack().Peek().GetSize() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitPOP2(NBCEL.generic.POP2 o)
		{
			if (Stack().Peek().GetSize() != 2)
			{
				ConstraintViolated(o, "Stack top size should be 2 but stack top is '" + Stack().Peek
					() + "' of size '" + Stack().Peek().GetSize() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitPUTFIELD(NBCEL.generic.PUTFIELD o)
		{
			try
			{
				NBCEL.generic.Type objectref = Stack().Peek(1);
				if (!((objectref is NBCEL.generic.ObjectType) || (objectref == NBCEL.generic.Type
					.NULL)))
				{
					ConstraintViolated(o, "Stack next-to-top should be an object reference that's not an array reference, but is '"
						 + objectref + "'.");
				}
				string field_name = o.GetFieldName(cpg);
				NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(GetObjectType(o).GetClassName
					());
				NBCEL.classfile.Field[] fields = jc.GetFields();
				NBCEL.classfile.Field f = null;
				foreach (NBCEL.classfile.Field field in fields)
				{
					if (field.GetName().Equals(field_name))
					{
						NBCEL.generic.Type f_type = NBCEL.generic.Type.GetType(field.GetSignature());
						NBCEL.generic.Type o_type = o.GetType(cpg);
						/* TODO: Check if assignment compatibility is sufficient.
						* What does Sun do?
						*/
						if (f_type.Equals(o_type))
						{
							f = field;
							break;
						}
					}
				}
				if (f == null)
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("Field '" + field_name + 
						"' not found in " + jc.GetClassName());
				}
				NBCEL.generic.Type value = Stack().Peek();
				NBCEL.generic.Type t = NBCEL.generic.Type.GetType(f.GetSignature());
				NBCEL.generic.Type shouldbe = t;
				if (shouldbe == NBCEL.generic.Type.BOOLEAN || shouldbe == NBCEL.generic.Type.BYTE
					 || shouldbe == NBCEL.generic.Type.CHAR || shouldbe == NBCEL.generic.Type.SHORT)
				{
					shouldbe = NBCEL.generic.Type.INT;
				}
				if (t is NBCEL.generic.ReferenceType)
				{
					NBCEL.generic.ReferenceType rvalue = null;
					if (value is NBCEL.generic.ReferenceType)
					{
						rvalue = (NBCEL.generic.ReferenceType)value;
						ReferenceTypeIsInitialized(o, rvalue);
					}
					else
					{
						ConstraintViolated(o, "The stack top type '" + value + "' is not of a reference type as expected."
							);
					}
					// TODO: This can possibly only be checked using Staerk-et-al's "set-of-object types", not
					// using "wider cast object types" created during verification.
					// Comment it out if you encounter problems. See also the analogon at visitPUTSTATIC.
					if (!(rvalue.IsAssignmentCompatibleWith(shouldbe)))
					{
						ConstraintViolated(o, "The stack top type '" + value + "' is not assignment compatible with '"
							 + shouldbe + "'.");
					}
				}
				else if (shouldbe != value)
				{
					ConstraintViolated(o, "The stack top type '" + value + "' is not of type '" + shouldbe
						 + "' as expected.");
				}
				if (f.IsProtected())
				{
					NBCEL.generic.ObjectType classtype = GetObjectType(o);
					NBCEL.generic.ObjectType curr = NBCEL.generic.ObjectType.GetInstance(mg.GetClassName
						());
					if (classtype.Equals(curr) || curr.SubclassOf(classtype))
					{
						NBCEL.generic.Type tp = Stack().Peek(1);
						if (tp == NBCEL.generic.Type.NULL)
						{
							return;
						}
						if (!(tp is NBCEL.generic.ObjectType))
						{
							ConstraintViolated(o, "The 'objectref' must refer to an object that's not an array. Found instead: '"
								 + tp + "'.");
						}
						NBCEL.generic.ObjectType objreftype = (NBCEL.generic.ObjectType)tp;
						if (!(objreftype.Equals(curr) || objreftype.SubclassOf(curr)))
						{
							ConstraintViolated(o, "The referenced field has the ACC_PROTECTED modifier, and it's a member of the current class or"
								 + " a superclass of the current class. However, the referenced object type '" +
								 Stack().Peek() + "' is not the current class or a subclass of the current class."
								);
						}
					}
				}
				// TODO: Could go into Pass 3a.
				if (f.IsStatic())
				{
					ConstraintViolated(o, "Referenced field '" + f + "' is static which it shouldn't be."
						);
				}
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: maybe not the best way to handle this
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitPUTSTATIC(NBCEL.generic.PUTSTATIC o)
		{
			try
			{
				string field_name = o.GetFieldName(cpg);
				NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(GetObjectType(o).GetClassName
					());
				NBCEL.classfile.Field[] fields = jc.GetFields();
				NBCEL.classfile.Field f = null;
				foreach (NBCEL.classfile.Field field in fields)
				{
					if (field.GetName().Equals(field_name))
					{
						NBCEL.generic.Type f_type = NBCEL.generic.Type.GetType(field.GetSignature());
						NBCEL.generic.Type o_type = o.GetType(cpg);
						/* TODO: Check if assignment compatibility is sufficient.
						* What does Sun do?
						*/
						if (f_type.Equals(o_type))
						{
							f = field;
							break;
						}
					}
				}
				if (f == null)
				{
					throw new NBCEL.verifier.exc.AssertionViolatedException("Field '" + field_name + 
						"' not found in " + jc.GetClassName());
				}
				NBCEL.generic.Type value = Stack().Peek();
				NBCEL.generic.Type t = NBCEL.generic.Type.GetType(f.GetSignature());
				NBCEL.generic.Type shouldbe = t;
				if (shouldbe == NBCEL.generic.Type.BOOLEAN || shouldbe == NBCEL.generic.Type.BYTE
					 || shouldbe == NBCEL.generic.Type.CHAR || shouldbe == NBCEL.generic.Type.SHORT)
				{
					shouldbe = NBCEL.generic.Type.INT;
				}
				if (t is NBCEL.generic.ReferenceType)
				{
					NBCEL.generic.ReferenceType rvalue = null;
					if (value is NBCEL.generic.ReferenceType)
					{
						rvalue = (NBCEL.generic.ReferenceType)value;
						ReferenceTypeIsInitialized(o, rvalue);
					}
					else
					{
						ConstraintViolated(o, "The stack top type '" + value + "' is not of a reference type as expected."
							);
					}
					// TODO: This can possibly only be checked using Staerk-et-al's "set-of-object types", not
					// using "wider cast object types" created during verification.
					// Comment it out if you encounter problems. See also the analogon at visitPUTFIELD.
					if (!(rvalue.IsAssignmentCompatibleWith(shouldbe)))
					{
						ConstraintViolated(o, "The stack top type '" + value + "' is not assignment compatible with '"
							 + shouldbe + "'.");
					}
				}
				else if (shouldbe != value)
				{
					ConstraintViolated(o, "The stack top type '" + value + "' is not of type '" + shouldbe
						 + "' as expected.");
				}
			}
			catch (System.TypeLoadException e)
			{
				// TODO: Interface fields may be assigned to only once. (Hard to implement in
				//       JustIce's execution model). This may only happen in <clinit>, see Pass 3a.
				// FIXME: maybe not the best way to handle this
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitRET(NBCEL.generic.RET o)
		{
			if (!(Locals().Get(o.GetIndex()) is NBCEL.generic.ReturnaddressType))
			{
				ConstraintViolated(o, "Expecting a ReturnaddressType in local variable " + o.GetIndex
					() + ".");
			}
			if (Locals().Get(o.GetIndex()) == NBCEL.generic.ReturnaddressType.NO_TARGET)
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("Oops: RET expecting a target!"
					);
			}
		}

		// Other constraints such as non-allowed overlapping subroutines are enforced
		// while building the Subroutines data structure.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitRETURN(NBCEL.generic.RETURN o)
		{
			if (mg.GetName().Equals(NBCEL.Const.CONSTRUCTOR_NAME))
			{
				// If we leave an <init> method
				if ((NBCEL.verifier.structurals.Frame.GetThis() != null) && (!(mg.GetClassName().
					Equals(NBCEL.generic.Type.OBJECT.GetClassName()))))
				{
					ConstraintViolated(o, "Leaving a constructor that itself did not call a constructor."
						);
				}
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitSALOAD(NBCEL.generic.SALOAD o)
		{
			IndexOfInt(o, Stack().Peek());
			if (Stack().Peek(1) == NBCEL.generic.Type.NULL)
			{
				return;
			}
			if (!(Stack().Peek(1) is NBCEL.generic.ArrayType))
			{
				ConstraintViolated(o, "Stack next-to-top must be of type short[] but is '" + Stack
					().Peek(1) + "'.");
			}
			NBCEL.generic.Type t = ((NBCEL.generic.ArrayType)(Stack().Peek(1))).GetBasicType(
				);
			if (t != NBCEL.generic.Type.SHORT)
			{
				ConstraintViolated(o, "Stack next-to-top must be of type short[] but is '" + Stack
					().Peek(1) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitSASTORE(NBCEL.generic.SASTORE o)
		{
			if (Stack().Peek() != NBCEL.generic.Type.INT)
			{
				ConstraintViolated(o, "The value at the stack top is not of type 'int', but of type '"
					 + Stack().Peek() + "'.");
			}
			IndexOfInt(o, Stack().Peek(1));
			if (Stack().Peek(2) == NBCEL.generic.Type.NULL)
			{
				return;
			}
			if (!(Stack().Peek(2) is NBCEL.generic.ArrayType))
			{
				ConstraintViolated(o, "Stack next-to-next-to-top must be of type short[] but is '"
					 + Stack().Peek(2) + "'.");
			}
			NBCEL.generic.Type t = ((NBCEL.generic.ArrayType)(Stack().Peek(2))).GetBasicType(
				);
			if (t != NBCEL.generic.Type.SHORT)
			{
				ConstraintViolated(o, "Stack next-to-next-to-top must be of type short[] but is '"
					 + Stack().Peek(2) + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitSIPUSH(NBCEL.generic.SIPUSH o)
		{
		}

		// nothing to do here. Generic visitXXX() methods did the trick before.
		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitSWAP(NBCEL.generic.SWAP o)
		{
			if (Stack().Peek().GetSize() != 1)
			{
				ConstraintViolated(o, "The value at the stack top is not of size '1', but of size '"
					 + Stack().Peek().GetSize() + "'.");
			}
			if (Stack().Peek(1).GetSize() != 1)
			{
				ConstraintViolated(o, "The value at the stack next-to-top is not of size '1', but of size '"
					 + Stack().Peek(1).GetSize() + "'.");
			}
		}

		/// <summary>Ensures the specific preconditions of the said instruction.</summary>
		public override void VisitTABLESWITCH(NBCEL.generic.TABLESWITCH o)
		{
			IndexOfInt(o, Stack().Peek());
		}
		// See Pass 3a.
	}
}
