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
	/// This PassVerifier verifies a class file according to
	/// pass 2 as described in The Java Virtual Machine
	/// Specification, 2nd edition.
	/// </summary>
	/// <remarks>
	/// This PassVerifier verifies a class file according to
	/// pass 2 as described in The Java Virtual Machine
	/// Specification, 2nd edition.
	/// More detailed information is to be found at the do_verify()
	/// method's documentation.
	/// </remarks>
	/// <seealso cref="Do_verify()"/>
	public sealed class Pass2Verifier : NBCEL.verifier.PassVerifier
	{
		/// <summary>The LocalVariableInfo instances used by Pass3bVerifier.</summary>
		/// <remarks>
		/// The LocalVariableInfo instances used by Pass3bVerifier.
		/// localVariablesInfos[i] denotes the information for the
		/// local variables of method number i in the
		/// JavaClass this verifier operates on.
		/// </remarks>
		private NBCEL.verifier.statics.LocalVariablesInfo[] localVariablesInfos;

		/// <summary>The Verifier that created this.</summary>
		private readonly NBCEL.verifier.Verifier myOwner;

		/// <summary>Should only be instantiated by a Verifier.</summary>
		/// <seealso cref="NBCEL.verifier.Verifier"/>
		public Pass2Verifier(NBCEL.verifier.Verifier owner)
		{
			myOwner = owner;
		}

		/// <summary>
		/// Returns a LocalVariablesInfo object containing information
		/// about the usage of the local variables in the Code attribute
		/// of the said method or <B>null</B> if the class file this
		/// Pass2Verifier operates on could not be pass-2-verified correctly.
		/// </summary>
		/// <remarks>
		/// Returns a LocalVariablesInfo object containing information
		/// about the usage of the local variables in the Code attribute
		/// of the said method or <B>null</B> if the class file this
		/// Pass2Verifier operates on could not be pass-2-verified correctly.
		/// The method number method_nr is the method you get using
		/// <B>Repository.lookupClass(myOwner.getClassname()).getMethods()[method_nr];</B>.
		/// You should not add own information. Leave that to JustIce.
		/// </remarks>
		public NBCEL.verifier.statics.LocalVariablesInfo GetLocalVariablesInfo(int method_nr
			)
		{
			if (this.Verify() != NBCEL.verifier.VerificationResult.VR_OK)
			{
				return null;
			}
			// It's cached, don't worry.
			if (method_nr < 0 || method_nr >= localVariablesInfos.Length)
			{
				throw new NBCEL.verifier.exc.AssertionViolatedException("Method number out of range."
					);
			}
			return localVariablesInfos[method_nr];
		}

		/// <summary>
		/// Pass 2 is the pass where static properties of the
		/// class file are checked without looking into "Code"
		/// arrays of methods.
		/// </summary>
		/// <remarks>
		/// Pass 2 is the pass where static properties of the
		/// class file are checked without looking into "Code"
		/// arrays of methods.
		/// This verification pass is usually invoked when
		/// a class is resolved; and it may be possible that
		/// this verification pass has to load in other classes
		/// such as superclasses or implemented interfaces.
		/// Therefore, Pass 1 is run on them.<BR>
		/// Note that most referenced classes are <B>not</B> loaded
		/// in for verification or for an existance check by this
		/// pass; only the syntactical correctness of their names
		/// and descriptors (a.k.a. signatures) is checked.<BR>
		/// Very few checks that conceptually belong here
		/// are delayed until pass 3a in JustIce. JustIce does
		/// not only check for syntactical correctness but also
		/// for semantical sanity - therefore it needs access to
		/// the "Code" array of methods in a few cases. Please
		/// see the pass 3a documentation, too.
		/// </remarks>
		/// <seealso cref="Pass3aVerifier"/>
		public override NBCEL.verifier.VerificationResult Do_verify()
		{
			try
			{
				NBCEL.verifier.VerificationResult vr1 = myOwner.DoPass1();
				if (vr1.Equals(NBCEL.verifier.VerificationResult.VR_OK))
				{
					// For every method, we could have information about the local variables out of LocalVariableTable attributes of
					// the Code attributes.
					localVariablesInfos = new NBCEL.verifier.statics.LocalVariablesInfo[NBCEL.Repository
						.LookupClass(myOwner.GetClassName()).GetMethods().Length];
					NBCEL.verifier.VerificationResult vr = NBCEL.verifier.VerificationResult.VR_OK;
					// default.
					try
					{
						Constant_pool_entries_satisfy_static_constraints();
						Field_and_method_refs_are_valid();
						Every_class_has_an_accessible_superclass();
						Final_methods_are_not_overridden();
					}
					catch (NBCEL.verifier.exc.ClassConstraintException cce)
					{
						vr = new NBCEL.verifier.VerificationResult(NBCEL.verifier.VerificationResult.VERIFIED_REJECTED
							, cce.Message);
					}
					return vr;
				}
				return NBCEL.verifier.VerificationResult.VR_NOTYET;
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: this might not be the best way to handle missing classes.
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>
		/// Ensures that every class has a super class and that
		/// <B>final</B> classes are not subclassed.
		/// </summary>
		/// <remarks>
		/// Ensures that every class has a super class and that
		/// <B>final</B> classes are not subclassed.
		/// This means, the class this Pass2Verifier operates
		/// on has proper super classes (transitively) up to
		/// java.lang.Object.
		/// The reason for really loading (and Pass1-verifying)
		/// all of those classes here is that we need them in
		/// Pass2 anyway to verify no final methods are overridden
		/// (that could be declared anywhere in the ancestor hierarchy).
		/// </remarks>
		/// <exception cref="NBCEL.verifier.exc.ClassConstraintException">otherwise.</exception>
		private void Every_class_has_an_accessible_superclass()
		{
			try
			{
				System.Collections.Generic.HashSet<string> hs = new System.Collections.Generic.HashSet
					<string>();
				// save class names to detect circular inheritance
				NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(myOwner.GetClassName(
					));
				int supidx = -1;
				while (supidx != 0)
				{
					supidx = jc.GetSuperclassNameIndex();
					if (supidx == 0)
					{
						if (jc != NBCEL.Repository.LookupClass(NBCEL.generic.Type.OBJECT.GetClassName()))
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Superclass of '" + jc.GetClassName
								() + "' missing but not " + NBCEL.generic.Type.OBJECT.GetClassName() + " itself!"
								);
						}
					}
					else
					{
						string supername = jc.GetSuperclassName();
						if (!hs.Add(supername))
						{
							// If supername already is in the list
							throw new NBCEL.verifier.exc.ClassConstraintException("Circular superclass hierarchy detected."
								);
						}
						NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(supername);
						NBCEL.verifier.VerificationResult vr = v.DoPass1();
						if (vr != NBCEL.verifier.VerificationResult.VR_OK)
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Could not load in ancestor class '"
								 + supername + "'.");
						}
						jc = NBCEL.Repository.LookupClass(supername);
						if (jc.IsFinal())
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Ancestor class '" + supername
								 + "' has the FINAL access modifier and must therefore not be subclassed.");
						}
					}
				}
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: this might not be the best way to handle missing classes.
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>Ensures that <B>final</B> methods are not overridden.</summary>
		/// <remarks>
		/// Ensures that <B>final</B> methods are not overridden.
		/// <B>Precondition to run this method:
		/// constant_pool_entries_satisfy_static_constraints() and
		/// every_class_has_an_accessible_superclass() have to be invoked before
		/// (in that order).</B>
		/// </remarks>
		/// <exception cref="NBCEL.verifier.exc.ClassConstraintException">otherwise.</exception>
		/// <seealso cref="Constant_pool_entries_satisfy_static_constraints()"/>
		/// <seealso cref="Every_class_has_an_accessible_superclass()"/>
		private void Final_methods_are_not_overridden()
		{
			try
			{
				System.Collections.Generic.IDictionary<string, string> hashmap = new System.Collections.Generic.Dictionary
					<string, string>();
				NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(myOwner.GetClassName(
					));
				int supidx = -1;
				while (supidx != 0)
				{
					supidx = jc.GetSuperclassNameIndex();
					NBCEL.classfile.Method[] methods = jc.GetMethods();
					foreach (NBCEL.classfile.Method method in methods)
					{
						string nameAndSig = method.GetName() + method.GetSignature();
						if (hashmap.ContainsKey(nameAndSig))
						{
							if (method.IsFinal())
							{
								if (!(method.IsPrivate()))
								{
									throw new NBCEL.verifier.exc.ClassConstraintException("Method '" + nameAndSig + "' in class '"
										 + hashmap.GetOrNull(nameAndSig) + "' overrides the final (not-overridable) definition in class '"
										 + jc.GetClassName() + "'.");
								}
								AddMessage("Method '" + nameAndSig + "' in class '" + hashmap.GetOrNull(nameAndSig
									) + "' overrides the final (not-overridable) definition in class '" + jc.GetClassName
									() + "'. This is okay, as the original definition was private; however this constraint leverage"
									 + " was introduced by JLS 8.4.6 (not vmspec2) and the behavior of the Sun verifiers."
									);
							}
							else if (!method.IsStatic())
							{
								// static methods don't inherit
								Sharpen.Collections.Put(hashmap, nameAndSig, jc.GetClassName());
							}
						}
						else if (!method.IsStatic())
						{
							// static methods don't inherit
							Sharpen.Collections.Put(hashmap, nameAndSig, jc.GetClassName());
						}
					}
					jc = NBCEL.Repository.LookupClass(jc.GetSuperclassName());
				}
			}
			catch (System.TypeLoadException e)
			{
				// Well, for OBJECT this returns OBJECT so it works (could return anything but must not throw an Exception).
				// FIXME: this might not be the best way to handle missing classes.
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>
		/// Ensures that the constant pool entries satisfy the static constraints
		/// as described in The Java Virtual Machine Specification, 2nd Edition.
		/// </summary>
		/// <exception cref="NBCEL.verifier.exc.ClassConstraintException">otherwise.</exception>
		private void Constant_pool_entries_satisfy_static_constraints()
		{
			try
			{
				// Most of the consistency is handled internally by BCEL; here
				// we only have to verify if the indices of the constants point
				// to constants of the appropriate type and such.
				NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(myOwner.GetClassName(
					));
				new NBCEL.verifier.statics.Pass2Verifier.CPESSC_Visitor(this, jc);
			}
			catch (System.TypeLoadException e)
			{
				// constructor implicitly traverses jc
				// FIXME: this might not be the best way to handle missing classes.
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>
		/// A Visitor class that ensures the constant pool satisfies the static
		/// constraints.
		/// </summary>
		/// <remarks>
		/// A Visitor class that ensures the constant pool satisfies the static
		/// constraints.
		/// The visitXXX() methods throw ClassConstraintException instances otherwise.
		/// </remarks>
		/// <seealso cref="Pass2Verifier.Constant_pool_entries_satisfy_static_constraints()"/
		/// 	>
		private sealed class CPESSC_Visitor : NBCEL.classfile.EmptyVisitor
		{
			private readonly System.Type CONST_Class;

			private readonly System.Type CONST_String;

			private readonly System.Type CONST_Integer;

			private readonly System.Type CONST_Float;

			private readonly System.Type CONST_Long;

			private readonly System.Type CONST_Double;

			private readonly System.Type CONST_NameAndType;

			private readonly System.Type CONST_Utf8;

			private readonly NBCEL.classfile.JavaClass jc;

			private readonly NBCEL.classfile.ConstantPool cp;

			private readonly int cplen;

			private readonly NBCEL.classfile.DescendingVisitor carrier;

			private readonly System.Collections.Generic.HashSet<string> field_names = new System.Collections.Generic.HashSet
				<string>();

			private readonly System.Collections.Generic.HashSet<string> field_names_and_desc = 
				new System.Collections.Generic.HashSet<string>();

			private readonly System.Collections.Generic.HashSet<string> method_names_and_desc
				 = new System.Collections.Generic.HashSet<string>();

			internal CPESSC_Visitor(Pass2Verifier _enclosing, NBCEL.classfile.JavaClass _jc)
			{
				this._enclosing = _enclosing;
				/*
				private Class<?> CONST_Fieldref;
				private Class<?> CONST_Methodref;
				private Class<?> CONST_InterfaceMethodref;
				*/
				// ==jc.getConstantPool() -- only here to save typing work and computing power.
				// == cp.getLength() -- to save computing power.
				this.jc = _jc;
				this.cp = _jc.GetConstantPool();
				this.cplen = this.cp.GetLength();
				this.CONST_Class = typeof(NBCEL.classfile.ConstantClass);
				/*
				CONST_Fieldref = ConstantFieldref.class;
				CONST_Methodref = ConstantMethodref.class;
				CONST_InterfaceMethodref = ConstantInterfaceMethodref.class;
				*/
				this.CONST_String = typeof(NBCEL.classfile.ConstantString);
				this.CONST_Integer = typeof(NBCEL.classfile.ConstantInteger);
				this.CONST_Float = typeof(NBCEL.classfile.ConstantFloat);
				this.CONST_Long = typeof(NBCEL.classfile.ConstantLong);
				this.CONST_Double = typeof(NBCEL.classfile.ConstantDouble);
				this.CONST_NameAndType = typeof(NBCEL.classfile.ConstantNameAndType);
				this.CONST_Utf8 = typeof(NBCEL.classfile.ConstantUtf8);
				this.carrier = new NBCEL.classfile.DescendingVisitor(_jc, this);
				this.carrier.Visit();
			}

			private void CheckIndex(NBCEL.classfile.Node referrer, int index, System.Type shouldbe
				)
			{
				if ((index < 0) || (index >= this.cplen))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Invalid index '" + index +
						 "' used by '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(referrer) + "'.");
				}
				NBCEL.classfile.Constant c = this.cp.GetConstant(index);
				if (!shouldbe.IsInstanceOfType(c))
				{
					/* String isnot = shouldbe.toString().substring(shouldbe.toString().lastIndexOf(".")+1); //Cut all before last "." */
					throw new System.InvalidCastException("Illegal constant '" + NBCEL.verifier.statics.Pass2Verifier
						.Tostring(c) + "' at index '" + index + "'. '" + NBCEL.verifier.statics.Pass2Verifier
						.Tostring(referrer) + "' expects a '" + shouldbe + "'.");
				}
			}

			///////////////////////////////////////
			// ClassFile structure (vmspec2 4.1) //
			///////////////////////////////////////
			public override void VisitJavaClass(NBCEL.classfile.JavaClass obj)
			{
				NBCEL.classfile.Attribute[] atts = obj.GetAttributes();
				bool foundSourceFile = false;
				bool foundInnerClasses = false;
				// Is there an InnerClass referenced?
				// This is a costly check; existing verifiers don't do it!
				bool hasInnerClass = new NBCEL.verifier.statics.Pass2Verifier.InnerClassDetector(
					this.jc).InnerClassReferenced();
				foreach (NBCEL.classfile.Attribute att in atts)
				{
					if ((!(att is NBCEL.classfile.SourceFile)) && (!(att is NBCEL.classfile.Deprecated
						)) && (!(att is NBCEL.classfile.InnerClasses)) && (!(att is NBCEL.classfile.Synthetic
						)))
					{
						this._enclosing.AddMessage("Attribute '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
							(att) + "' as an attribute of the ClassFile structure '" + NBCEL.verifier.statics.Pass2Verifier
							.Tostring(obj) + "' is unknown and will therefore be ignored.");
					}
					if (att is NBCEL.classfile.SourceFile)
					{
						if (!foundSourceFile)
						{
							foundSourceFile = true;
						}
						else
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("A ClassFile structure (like '"
								 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "') may have no more than one SourceFile attribute."
								);
						}
					}
					//vmspec2 4.7.7
					if (att is NBCEL.classfile.InnerClasses)
					{
						if (!foundInnerClasses)
						{
							foundInnerClasses = true;
						}
						else if (hasInnerClass)
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("A Classfile structure (like '"
								 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "') must have exactly one InnerClasses attribute"
								 + " if at least one Inner Class is referenced (which is the case)." + " More than one InnerClasses attribute was found."
								);
						}
						if (!hasInnerClass)
						{
							this._enclosing.AddMessage("No referenced Inner Class found, but InnerClasses attribute '"
								 + NBCEL.verifier.statics.Pass2Verifier.Tostring(att) + "' found. Strongly suggest removal of that attribute."
								);
						}
					}
				}
				if (hasInnerClass && !foundInnerClasses)
				{
					//throw new ClassConstraintException("A Classfile structure (like '"+tostring(obj)+
					// "') must have exactly one InnerClasses attribute if at least one Inner Class is referenced (which is the case)."+
					// " No InnerClasses attribute was found.");
					//vmspec2, page 125 says it would be a constraint: but existing verifiers
					//don't check it and javac doesn't satisfy it when it comes to anonymous
					//inner classes
					this._enclosing.AddMessage("A Classfile structure (like '" + NBCEL.verifier.statics.Pass2Verifier
						.Tostring(obj) + "') must have exactly one InnerClasses attribute if at least one Inner Class is referenced (which is the case)."
						 + " No InnerClasses attribute was found.");
				}
			}

			/////////////////////////////
			// CONSTANTS (vmspec2 4.4) //
			/////////////////////////////
			public override void VisitConstantClass(NBCEL.classfile.ConstantClass obj)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_Class)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Wrong constant tag in '" +
						 NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
				this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
			}

			public override void VisitConstantFieldref(NBCEL.classfile.ConstantFieldref obj)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_Fieldref)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Wrong constant tag in '" +
						 NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
				this.CheckIndex(obj, obj.GetClassIndex(), this.CONST_Class);
				this.CheckIndex(obj, obj.GetNameAndTypeIndex(), this.CONST_NameAndType);
			}

			public override void VisitConstantMethodref(NBCEL.classfile.ConstantMethodref obj
				)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_Methodref)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Wrong constant tag in '" +
						 NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
				this.CheckIndex(obj, obj.GetClassIndex(), this.CONST_Class);
				this.CheckIndex(obj, obj.GetNameAndTypeIndex(), this.CONST_NameAndType);
			}

			public override void VisitConstantInterfaceMethodref(NBCEL.classfile.ConstantInterfaceMethodref
				 obj)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_InterfaceMethodref)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Wrong constant tag in '" +
						 NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
				this.CheckIndex(obj, obj.GetClassIndex(), this.CONST_Class);
				this.CheckIndex(obj, obj.GetNameAndTypeIndex(), this.CONST_NameAndType);
			}

			public override void VisitConstantString(NBCEL.classfile.ConstantString obj)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_String)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Wrong constant tag in '" +
						 NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
				this.CheckIndex(obj, obj.GetStringIndex(), this.CONST_Utf8);
			}

			public override void VisitConstantInteger(NBCEL.classfile.ConstantInteger obj)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_Integer)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Wrong constant tag in '" +
						 NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
			}

			// no indices to check
			public override void VisitConstantFloat(NBCEL.classfile.ConstantFloat obj)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_Float)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Wrong constant tag in '" +
						 NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
			}

			//no indices to check
			public override void VisitConstantLong(NBCEL.classfile.ConstantLong obj)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_Long)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Wrong constant tag in '" +
						 NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
			}

			//no indices to check
			public override void VisitConstantDouble(NBCEL.classfile.ConstantDouble obj)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_Double)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Wrong constant tag in '" +
						 NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
			}

			//no indices to check
			public override void VisitConstantNameAndType(NBCEL.classfile.ConstantNameAndType
				 obj)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_NameAndType)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Wrong constant tag in '" +
						 NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
				this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
				//checkIndex(obj, obj.getDescriptorIndex(), CONST_Utf8); //inconsistently named in BCEL, see below.
				this.CheckIndex(obj, obj.GetSignatureIndex(), this.CONST_Utf8);
			}

			public override void VisitConstantUtf8(NBCEL.classfile.ConstantUtf8 obj)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_Utf8)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Wrong constant tag in '" +
						 NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
			}

			//no indices to check
			//////////////////////////
			// FIELDS (vmspec2 4.5) //
			//////////////////////////
			public override void VisitField(NBCEL.classfile.Field obj)
			{
				if (this.jc.IsClass())
				{
					int maxone = 0;
					if (obj.IsPrivate())
					{
						maxone++;
					}
					if (obj.IsProtected())
					{
						maxone++;
					}
					if (obj.IsPublic())
					{
						maxone++;
					}
					if (maxone > 1)
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("Field '" + NBCEL.verifier.statics.Pass2Verifier
							.Tostring(obj) + "' must only have at most one of its ACC_PRIVATE, ACC_PROTECTED, ACC_PUBLIC modifiers set."
							);
					}
					if (obj.IsFinal() && obj.IsVolatile())
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("Field '" + NBCEL.verifier.statics.Pass2Verifier
							.Tostring(obj) + "' must only have at most one of its ACC_FINAL, ACC_VOLATILE modifiers set."
							);
					}
				}
				else
				{
					// isInterface!
					if (!obj.IsPublic())
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("Interface field '" + NBCEL.verifier.statics.Pass2Verifier
							.Tostring(obj) + "' must have the ACC_PUBLIC modifier set but hasn't!");
					}
					if (!obj.IsStatic())
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("Interface field '" + NBCEL.verifier.statics.Pass2Verifier
							.Tostring(obj) + "' must have the ACC_STATIC modifier set but hasn't!");
					}
					if (!obj.IsFinal())
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("Interface field '" + NBCEL.verifier.statics.Pass2Verifier
							.Tostring(obj) + "' must have the ACC_FINAL modifier set but hasn't!");
					}
				}
				if ((obj.GetAccessFlags() & ~(NBCEL.Const.ACC_PUBLIC | NBCEL.Const.ACC_PRIVATE | 
					NBCEL.Const.ACC_PROTECTED | NBCEL.Const.ACC_STATIC | NBCEL.Const.ACC_FINAL | NBCEL.Const
					.ACC_VOLATILE | NBCEL.Const.ACC_TRANSIENT)) > 0)
				{
					this._enclosing.AddMessage("Field '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
						(obj) + "' has access flag(s) other than ACC_PUBLIC, ACC_PRIVATE, ACC_PROTECTED,"
						 + " ACC_STATIC, ACC_FINAL, ACC_VOLATILE, ACC_TRANSIENT set (ignored).");
				}
				this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
				string name = obj.GetName();
				if (!NBCEL.verifier.statics.Pass2Verifier.ValidFieldName(name))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Field '" + NBCEL.verifier.statics.Pass2Verifier
						.Tostring(obj) + "' has illegal name '" + obj.GetName() + "'.");
				}
				// A descriptor is often named signature in BCEL
				this.CheckIndex(obj, obj.GetSignatureIndex(), this.CONST_Utf8);
				string sig = ((NBCEL.classfile.ConstantUtf8)(this.cp.GetConstant(obj.GetSignatureIndex
					()))).GetBytes();
				// Field or Method sig.(=descriptor)
				try
				{
					NBCEL.generic.Type.GetType(sig);
				}
				catch (NBCEL.classfile.ClassFormatException cfe)
				{
					/* Don't need the return value */
					throw new NBCEL.verifier.exc.ClassConstraintException("Illegal descriptor (==signature) '"
						 + sig + "' used by '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'."
						, cfe);
				}
				string nameanddesc = name + sig;
				if (this.field_names_and_desc.Contains(nameanddesc))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("No two fields (like '" + NBCEL.verifier.statics.Pass2Verifier
						.Tostring(obj) + "') are allowed have same names and descriptors!");
				}
				if (this.field_names.Contains(name))
				{
					this._enclosing.AddMessage("More than one field of name '" + name + "' detected (but with different type descriptors). This is very unusual."
						);
				}
				this.field_names_and_desc.Add(nameanddesc);
				this.field_names.Add(name);
				NBCEL.classfile.Attribute[] atts = obj.GetAttributes();
				foreach (NBCEL.classfile.Attribute att in atts)
				{
					if ((!(att is NBCEL.classfile.ConstantValue)) && (!(att is NBCEL.classfile.Synthetic
						)) && (!(att is NBCEL.classfile.Deprecated)))
					{
						this._enclosing.AddMessage("Attribute '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
							(att) + "' as an attribute of Field '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
							(obj) + "' is unknown and will therefore be ignored.");
					}
					if (!(att is NBCEL.classfile.ConstantValue))
					{
						this._enclosing.AddMessage("Attribute '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
							(att) + "' as an attribute of Field '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
							(obj) + "' is not a ConstantValue and is therefore only of use for debuggers and such."
							);
					}
				}
			}

			///////////////////////////
			// METHODS (vmspec2 4.6) //
			///////////////////////////
			public override void VisitMethod(NBCEL.classfile.Method obj)
			{
				this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
				string name = obj.GetName();
				if (!NBCEL.verifier.statics.Pass2Verifier.ValidMethodName(name, true))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Method '" + NBCEL.verifier.statics.Pass2Verifier
						.Tostring(obj) + "' has illegal name '" + name + "'.");
				}
				// A descriptor is often named signature in BCEL
				this.CheckIndex(obj, obj.GetSignatureIndex(), this.CONST_Utf8);
				string sig = ((NBCEL.classfile.ConstantUtf8)(this.cp.GetConstant(obj.GetSignatureIndex
					()))).GetBytes();
				// Method's signature(=descriptor)
				NBCEL.generic.Type t;
				NBCEL.generic.Type[] ts;
				// needed below the try block.
				try
				{
					t = NBCEL.generic.Type.GetReturnType(sig);
					ts = NBCEL.generic.Type.GetArgumentTypes(sig);
				}
				catch (NBCEL.classfile.ClassFormatException cfe)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Illegal descriptor (==signature) '"
						 + sig + "' used by Method '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj
						) + "'.", cfe);
				}
				// Check if referenced objects exist.
				NBCEL.generic.Type act = t;
				if (act is NBCEL.generic.ArrayType)
				{
					act = ((NBCEL.generic.ArrayType)act).GetBasicType();
				}
				if (act is NBCEL.generic.ObjectType)
				{
					NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(((NBCEL.generic.ObjectType
						)act).GetClassName());
					NBCEL.verifier.VerificationResult vr = v.DoPass1();
					if (vr != NBCEL.verifier.VerificationResult.VR_OK)
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("Method '" + NBCEL.verifier.statics.Pass2Verifier
							.Tostring(obj) + "' has a return type that does not pass verification pass 1: '"
							 + vr + "'.");
					}
				}
				foreach (NBCEL.generic.Type element in ts)
				{
					act = element;
					if (act is NBCEL.generic.ArrayType)
					{
						act = ((NBCEL.generic.ArrayType)act).GetBasicType();
					}
					if (act is NBCEL.generic.ObjectType)
					{
						NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(((NBCEL.generic.ObjectType
							)act).GetClassName());
						NBCEL.verifier.VerificationResult vr = v.DoPass1();
						if (vr != NBCEL.verifier.VerificationResult.VR_OK)
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Method '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' has an argument type that does not pass verification pass 1: '"
								 + vr + "'.");
						}
					}
				}
				// Nearly forgot this! Funny return values are allowed, but a non-empty arguments list makes a different method out of it!
				if (name.Equals(NBCEL.Const.STATIC_INITIALIZER_NAME) && (ts.Length != 0))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Method '" + NBCEL.verifier.statics.Pass2Verifier
						.Tostring(obj) + "' has illegal name '" + name + "'." + " Its name resembles the class or interface initialization method"
						 + " which it isn't because of its arguments (==descriptor).");
				}
				if (this.jc.IsClass())
				{
					int maxone = 0;
					if (obj.IsPrivate())
					{
						maxone++;
					}
					if (obj.IsProtected())
					{
						maxone++;
					}
					if (obj.IsPublic())
					{
						maxone++;
					}
					if (maxone > 1)
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("Method '" + NBCEL.verifier.statics.Pass2Verifier
							.Tostring(obj) + "' must only have at most one of its ACC_PRIVATE, ACC_PROTECTED, ACC_PUBLIC modifiers set."
							);
					}
					if (obj.IsAbstract())
					{
						if (obj.IsFinal())
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Abstract method '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' must not have the ACC_FINAL modifier set.");
						}
						if (obj.IsNative())
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Abstract method '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' must not have the ACC_NATIVE modifier set.");
						}
						if (obj.IsPrivate())
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Abstract method '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' must not have the ACC_PRIVATE modifier set.");
						}
						if (obj.IsStatic())
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Abstract method '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' must not have the ACC_STATIC modifier set.");
						}
						if (obj.IsStrictfp())
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Abstract method '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' must not have the ACC_STRICT modifier set.");
						}
						if (obj.IsSynchronized())
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Abstract method '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' must not have the ACC_SYNCHRONIZED modifier set.");
						}
					}
					// A specific instance initialization method... (vmspec2,Page 116).
					if (name.Equals(NBCEL.Const.CONSTRUCTOR_NAME))
					{
						//..may have at most one of ACC_PRIVATE, ACC_PROTECTED, ACC_PUBLIC set: is checked above.
						//..may also have ACC_STRICT set, but none of the other flags in table 4.5 (vmspec2, page 115)
						if (obj.IsStatic() || obj.IsFinal() || obj.IsSynchronized() || obj.IsNative() || 
							obj.IsAbstract())
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Instance initialization method '"
								 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' must not have" + " any of the ACC_STATIC, ACC_FINAL, ACC_SYNCHRONIZED, ACC_NATIVE, ACC_ABSTRACT modifiers set."
								);
						}
					}
				}
				else if (!name.Equals(NBCEL.Const.STATIC_INITIALIZER_NAME))
				{
					// isInterface!
					//vmspec2, p.116, 2nd paragraph
					if (this.jc.GetMajor() >= NBCEL.Const.MAJOR_1_8)
					{
						if (!(obj.IsPublic() ^ obj.IsPrivate()))
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Interface method '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' must have" + " exactly one of its ACC_PUBLIC and ACC_PRIVATE modifiers set."
								);
						}
						if (obj.IsProtected() || obj.IsFinal() || obj.IsSynchronized() || obj.IsNative())
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Interface method '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' must not have" + " any of the ACC_PROTECTED, ACC_FINAL, ACC_SYNCHRONIZED, or ACC_NATIVE modifiers set."
								);
						}
					}
					else
					{
						if (!obj.IsPublic())
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Interface method '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' must have the ACC_PUBLIC modifier set but hasn't!");
						}
						if (!obj.IsAbstract())
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Interface method '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' must have the ACC_ABSTRACT modifier set but hasn't!");
						}
						if (obj.IsPrivate() || obj.IsProtected() || obj.IsStatic() || obj.IsFinal() || obj
							.IsSynchronized() || obj.IsNative() || obj.IsStrictfp())
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Interface method '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' must not have" + " any of the ACC_PRIVATE, ACC_PROTECTED, ACC_STATIC, ACC_FINAL, ACC_SYNCHRONIZED,"
								 + " ACC_NATIVE, ACC_ABSTRACT, ACC_STRICT modifiers set.");
						}
					}
				}
				if ((obj.GetAccessFlags() & ~(NBCEL.Const.ACC_PUBLIC | NBCEL.Const.ACC_PRIVATE | 
					NBCEL.Const.ACC_PROTECTED | NBCEL.Const.ACC_STATIC | NBCEL.Const.ACC_FINAL | NBCEL.Const
					.ACC_SYNCHRONIZED | NBCEL.Const.ACC_NATIVE | NBCEL.Const.ACC_ABSTRACT | NBCEL.Const
					.ACC_STRICT)) > 0)
				{
					this._enclosing.AddMessage("Method '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
						(obj) + "' has access flag(s) other than" + " ACC_PUBLIC, ACC_PRIVATE, ACC_PROTECTED, ACC_STATIC, ACC_FINAL,"
						 + " ACC_SYNCHRONIZED, ACC_NATIVE, ACC_ABSTRACT, ACC_STRICT set (ignored).");
				}
				string nameanddesc = name + sig;
				if (this.method_names_and_desc.Contains(nameanddesc))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("No two methods (like '" + 
						NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "') are allowed have same names and desciptors!"
						);
				}
				this.method_names_and_desc.Add(nameanddesc);
				NBCEL.classfile.Attribute[] atts = obj.GetAttributes();
				int num_code_atts = 0;
				foreach (NBCEL.classfile.Attribute att in atts)
				{
					if ((!(att is NBCEL.classfile.Code)) && (!(att is NBCEL.classfile.ExceptionTable)
						) && (!(att is NBCEL.classfile.Synthetic)) && (!(att is NBCEL.classfile.Deprecated
						)))
					{
						this._enclosing.AddMessage("Attribute '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
							(att) + "' as an attribute of Method '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
							(obj) + "' is unknown and will therefore be ignored.");
					}
					if ((!(att is NBCEL.classfile.Code)) && (!(att is NBCEL.classfile.ExceptionTable)
						))
					{
						this._enclosing.AddMessage("Attribute '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
							(att) + "' as an attribute of Method '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
							(obj) + "' is neither Code nor Exceptions and is therefore only of use for debuggers and such."
							);
					}
					if ((att is NBCEL.classfile.Code) && (obj.IsNative() || obj.IsAbstract()))
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("Native or abstract methods like '"
							 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' must not have a Code attribute like '"
							 + NBCEL.verifier.statics.Pass2Verifier.Tostring(att) + "'.");
					}
					//vmspec2 page120, 4.7.3
					if (att is NBCEL.classfile.Code)
					{
						num_code_atts++;
					}
				}
				if (!obj.IsNative() && !obj.IsAbstract() && num_code_atts != 1)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Non-native, non-abstract methods like '"
						 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' must have exactly one Code attribute (found: "
						 + num_code_atts + ").");
				}
			}

			///////////////////////////////////////////////////////
			// ClassFile-structure-ATTRIBUTES (vmspec2 4.1, 4.7) //
			///////////////////////////////////////////////////////
			public override void VisitSourceFile(NBCEL.classfile.SourceFile obj)
			{
				//vmspec2 4.7.7
				// zero or one SourceFile attr per ClassFile: see visitJavaClass()
				this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
				string name = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(obj.GetNameIndex
					())).GetBytes();
				if (!name.Equals("SourceFile"))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("The SourceFile attribute '"
						 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' is not correctly named 'SourceFile' but '"
						 + name + "'.");
				}
				this.CheckIndex(obj, obj.GetSourceFileIndex(), this.CONST_Utf8);
				string sourceFileName = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(obj.GetSourceFileIndex
					())).GetBytes();
				//==obj.getSourceFileName() ?
				string sourceFileNameLc = sourceFileName.ToLower();
				if ((sourceFileName.IndexOf('/') != -1) || (sourceFileName.IndexOf('\\') != -1) ||
					 (sourceFileName.IndexOf(':') != -1) || (sourceFileNameLc.LastIndexOf(".java") ==
					 -1))
				{
					this._enclosing.AddMessage("SourceFile attribute '" + NBCEL.verifier.statics.Pass2Verifier
						.Tostring(obj) + "' has a funny name: remember not to confuse certain parsers working on javap's output. Also, this name ('"
						 + sourceFileName + "') is considered an unqualified (simple) file name only.");
				}
			}

			public override void VisitDeprecated(NBCEL.classfile.Deprecated obj)
			{
				//vmspec2 4.7.10
				this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
				string name = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(obj.GetNameIndex
					())).GetBytes();
				if (!name.Equals("Deprecated"))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("The Deprecated attribute '"
						 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' is not correctly named 'Deprecated' but '"
						 + name + "'.");
				}
			}

			public override void VisitSynthetic(NBCEL.classfile.Synthetic obj)
			{
				//vmspec2 4.7.6
				this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
				string name = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(obj.GetNameIndex
					())).GetBytes();
				if (!name.Equals("Synthetic"))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("The Synthetic attribute '"
						 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' is not correctly named 'Synthetic' but '"
						 + name + "'.");
				}
			}

			public override void VisitInnerClasses(NBCEL.classfile.InnerClasses obj)
			{
				//vmspec2 4.7.5
				// exactly one InnerClasses attr per ClassFile if some inner class is refernced: see visitJavaClass()
				this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
				string name = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(obj.GetNameIndex
					())).GetBytes();
				if (!name.Equals("InnerClasses"))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("The InnerClasses attribute '"
						 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' is not correctly named 'InnerClasses' but '"
						 + name + "'.");
				}
				NBCEL.classfile.InnerClass[] ics = obj.GetInnerClasses();
				foreach (NBCEL.classfile.InnerClass ic in ics)
				{
					this.CheckIndex(obj, ic.GetInnerClassIndex(), this.CONST_Class);
					int outer_idx = ic.GetOuterClassIndex();
					if (outer_idx != 0)
					{
						this.CheckIndex(obj, outer_idx, this.CONST_Class);
					}
					int innername_idx = ic.GetInnerNameIndex();
					if (innername_idx != 0)
					{
						this.CheckIndex(obj, innername_idx, this.CONST_Utf8);
					}
					int acc = ic.GetInnerAccessFlags();
					acc = acc & (~(NBCEL.Const.ACC_PUBLIC | NBCEL.Const.ACC_PRIVATE | NBCEL.Const.ACC_PROTECTED
						 | NBCEL.Const.ACC_STATIC | NBCEL.Const.ACC_FINAL | NBCEL.Const.ACC_INTERFACE | 
						NBCEL.Const.ACC_ABSTRACT));
					if (acc != 0)
					{
						this._enclosing.AddMessage("Unknown access flag for inner class '" + NBCEL.verifier.statics.Pass2Verifier
							.Tostring(ic) + "' set (InnerClasses attribute '" + NBCEL.verifier.statics.Pass2Verifier
							.Tostring(obj) + "').");
					}
				}
			}

			// Semantical consistency is not yet checked by Sun, see vmspec2 4.7.5.
			// [marked TODO in JustIce]
			////////////////////////////////////////////////////////
			// field_info-structure-ATTRIBUTES (vmspec2 4.5, 4.7) //
			////////////////////////////////////////////////////////
			public override void VisitConstantValue(NBCEL.classfile.ConstantValue obj)
			{
				//vmspec2 4.7.2
				// Despite its name, this really is an Attribute,
				// not a constant!
				this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
				string name = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(obj.GetNameIndex
					())).GetBytes();
				if (!name.Equals("ConstantValue"))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("The ConstantValue attribute '"
						 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' is not correctly named 'ConstantValue' but '"
						 + name + "'.");
				}
				object pred = this.carrier.Predecessor();
				if (pred is NBCEL.classfile.Field)
				{
					//ConstantValue attributes are quite senseless if the predecessor is not a field.
					NBCEL.classfile.Field f = (NBCEL.classfile.Field)pred;
					// Field constraints have been checked before -- so we are safe using their type information.
					NBCEL.generic.Type field_type = NBCEL.generic.Type.GetType(((NBCEL.classfile.ConstantUtf8
						)(this.cp.GetConstant(f.GetSignatureIndex()))).GetBytes());
					int index = obj.GetConstantValueIndex();
					if ((index < 0) || (index >= this.cplen))
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("Invalid index '" + index +
							 "' used by '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
					}
					NBCEL.classfile.Constant c = this.cp.GetConstant(index);
					if (this.CONST_Long.IsInstanceOfType(c) && field_type.Equals(NBCEL.generic.Type.LONG
						))
					{
						return;
					}
					if (this.CONST_Float.IsInstanceOfType(c) && field_type.Equals(NBCEL.generic.Type.
						FLOAT))
					{
						return;
					}
					if (this.CONST_Double.IsInstanceOfType(c) && field_type.Equals(NBCEL.generic.Type
						.DOUBLE))
					{
						return;
					}
					if (this.CONST_Integer.IsInstanceOfType(c) && (field_type.Equals(NBCEL.generic.Type
						.INT) || field_type.Equals(NBCEL.generic.Type.SHORT) || field_type.Equals(NBCEL.generic.Type
						.CHAR) || field_type.Equals(NBCEL.generic.Type.BYTE) || field_type.Equals(NBCEL.generic.Type
						.BOOLEAN)))
					{
						return;
					}
					if (this.CONST_String.IsInstanceOfType(c) && field_type.Equals(NBCEL.generic.Type
						.STRING))
					{
						return;
					}
					throw new NBCEL.verifier.exc.ClassConstraintException("Illegal type of ConstantValue '"
						 + obj + "' embedding Constant '" + c + "'. It is referenced by field '" + NBCEL.verifier.statics.Pass2Verifier
						.Tostring(f) + "' expecting a different type: '" + field_type + "'.");
				}
			}

			// SYNTHETIC: see above
			// DEPRECATED: see above
			/////////////////////////////////////////////////////////
			// method_info-structure-ATTRIBUTES (vmspec2 4.6, 4.7) //
			/////////////////////////////////////////////////////////
			public override void VisitCode(NBCEL.classfile.Code obj)
			{
				//vmspec2 4.7.3
				try
				{
					// No code attribute allowed for native or abstract methods: see visitMethod(Method).
					// Code array constraints are checked in Pass3 (3a and 3b).
					this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
					string name = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(obj.GetNameIndex
						())).GetBytes();
					if (!name.Equals("Code"))
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("The Code attribute '" + NBCEL.verifier.statics.Pass2Verifier
							.Tostring(obj) + "' is not correctly named 'Code' but '" + name + "'.");
					}
					NBCEL.classfile.Method m = null;
					// satisfy compiler
					if (!(this.carrier.Predecessor() is NBCEL.classfile.Method))
					{
						this._enclosing.AddMessage("Code attribute '" + NBCEL.verifier.statics.Pass2Verifier
							.Tostring(obj) + "' is not declared in a method_info structure but in '" + this.
							carrier.Predecessor() + "'. Ignored.");
						return;
					}
					m = (NBCEL.classfile.Method)this.carrier.Predecessor();
					// we can assume this method was visited before;
					// i.e. the data consistency was verified.
					if (obj.GetCode().Length == 0)
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("Code array of Code attribute '"
							 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' (method '" + m + "') must not be empty."
							);
					}
					//In JustIce, the check for correct offsets into the code array is delayed to Pass 3a.
					NBCEL.classfile.CodeException[] exc_table = obj.GetExceptionTable();
					foreach (NBCEL.classfile.CodeException element in exc_table)
					{
						int exc_index = element.GetCatchType();
						if (exc_index != 0)
						{
							// if 0, it catches all Throwables
							this.CheckIndex(obj, exc_index, this.CONST_Class);
							NBCEL.classfile.ConstantClass cc = (NBCEL.classfile.ConstantClass)(this.cp.GetConstant
								(exc_index));
							// cannot be sure this ConstantClass has already been visited (checked)!
							this.CheckIndex(cc, cc.GetNameIndex(), this.CONST_Utf8);
							string cname = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(cc.GetNameIndex
								())).GetBytes().Replace('/', '.');
							NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(cname);
							NBCEL.verifier.VerificationResult vr = v.DoPass1();
							if (vr != NBCEL.verifier.VerificationResult.VR_OK)
							{
								throw new NBCEL.verifier.exc.ClassConstraintException("Code attribute '" + NBCEL.verifier.statics.Pass2Verifier
									.Tostring(obj) + "' (method '" + m + "') has an exception_table entry '" + NBCEL.verifier.statics.Pass2Verifier
									.Tostring(element) + "' that references '" + cname + "' as an Exception but it does not pass verification pass 1: "
									 + vr);
							}
							// We cannot safely trust any other "instanceof" mechanism. We need to transitively verify
							// the ancestor hierarchy.
							NBCEL.classfile.JavaClass e = NBCEL.Repository.LookupClass(cname);
							NBCEL.classfile.JavaClass t = NBCEL.Repository.LookupClass(NBCEL.generic.Type.THROWABLE
								.GetClassName());
							NBCEL.classfile.JavaClass o = NBCEL.Repository.LookupClass(NBCEL.generic.Type.OBJECT
								.GetClassName());
							while (e != o)
							{
								if (e == t)
								{
									break;
								}
								// It's a subclass of Throwable, OKAY, leave.
								v = NBCEL.verifier.VerifierFactory.GetVerifier(e.GetSuperclassName());
								vr = v.DoPass1();
								if (vr != NBCEL.verifier.VerificationResult.VR_OK)
								{
									throw new NBCEL.verifier.exc.ClassConstraintException("Code attribute '" + NBCEL.verifier.statics.Pass2Verifier
										.Tostring(obj) + "' (method '" + m + "') has an exception_table entry '" + NBCEL.verifier.statics.Pass2Verifier
										.Tostring(element) + "' that references '" + cname + "' as an Exception but '" +
										 e.GetSuperclassName() + "' in the ancestor hierachy does not pass verification pass 1: "
										 + vr);
								}
								e = NBCEL.Repository.LookupClass(e.GetSuperclassName());
							}
							if (e != t)
							{
								throw new NBCEL.verifier.exc.ClassConstraintException("Code attribute '" + NBCEL.verifier.statics.Pass2Verifier
									.Tostring(obj) + "' (method '" + m + "') has an exception_table entry '" + NBCEL.verifier.statics.Pass2Verifier
									.Tostring(element) + "' that references '" + cname + "' as an Exception but it is not a subclass of '"
									 + t.GetClassName() + "'.");
							}
						}
					}
					// Create object for local variables information
					// This is highly unelegant due to usage of the Visitor pattern.
					// TODO: rework it.
					int method_number = -1;
					NBCEL.classfile.Method[] ms = NBCEL.Repository.LookupClass(this._enclosing.myOwner
						.GetClassName()).GetMethods();
					for (int mn = 0; mn < ms.Length; mn++)
					{
						if (m == ms[mn])
						{
							method_number = mn;
							break;
						}
					}
					if (method_number < 0)
					{
						// Mmmmh. Can we be sure BCEL does not sometimes instantiate new objects?
						throw new NBCEL.verifier.exc.AssertionViolatedException("Could not find a known BCEL Method object in the corresponding BCEL JavaClass object."
							);
					}
					this._enclosing.localVariablesInfos[method_number] = new NBCEL.verifier.statics.LocalVariablesInfo
						(obj.GetMaxLocals());
					int num_of_lvt_attribs = 0;
					// Now iterate through the attributes the Code attribute has.
					NBCEL.classfile.Attribute[] atts = obj.GetAttributes();
					for (int a = 0; a < atts.Length; a++)
					{
						if ((!(atts[a] is NBCEL.classfile.LineNumberTable)) && (!(atts[a] is NBCEL.classfile.LocalVariableTable
							)))
						{
							this._enclosing.AddMessage("Attribute '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
								(atts[a]) + "' as an attribute of Code attribute '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' (method '" + m + "') is unknown and will therefore be ignored."
								);
						}
						else
						{
							// LineNumberTable or LocalVariableTable
							this._enclosing.AddMessage("Attribute '" + NBCEL.verifier.statics.Pass2Verifier.Tostring
								(atts[a]) + "' as an attribute of Code attribute '" + NBCEL.verifier.statics.Pass2Verifier
								.Tostring(obj) + "' (method '" + m + "') will effectively be ignored and is only useful for debuggers and such."
								);
						}
						//LocalVariableTable check (partially delayed to Pass3a).
						//Here because its easier to collect the information of the
						//(possibly more than one) LocalVariableTables belonging to
						//one certain Code attribute.
						if (atts[a] is NBCEL.classfile.LocalVariableTable)
						{
							// checks conforming to vmspec2 4.7.9
							NBCEL.classfile.LocalVariableTable lvt = (NBCEL.classfile.LocalVariableTable)atts
								[a];
							this.CheckIndex(lvt, lvt.GetNameIndex(), this.CONST_Utf8);
							string lvtname = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(lvt.GetNameIndex
								())).GetBytes();
							if (!lvtname.Equals("LocalVariableTable"))
							{
								throw new NBCEL.verifier.exc.ClassConstraintException("The LocalVariableTable attribute '"
									 + NBCEL.verifier.statics.Pass2Verifier.Tostring(lvt) + "' is not correctly named 'LocalVariableTable' but '"
									 + lvtname + "'.");
							}
							NBCEL.classfile.Code code = obj;
							//In JustIce, the check for correct offsets into the code array is delayed to Pass 3a.
							NBCEL.classfile.LocalVariable[] localvariables = lvt.GetLocalVariableTable();
							foreach (NBCEL.classfile.LocalVariable localvariable in localvariables)
							{
								this.CheckIndex(lvt, localvariable.GetNameIndex(), this.CONST_Utf8);
								string localname = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(localvariable
									.GetNameIndex())).GetBytes();
								if (!NBCEL.verifier.statics.Pass2Verifier.ValidJavaIdentifier(localname))
								{
									throw new NBCEL.verifier.exc.ClassConstraintException("LocalVariableTable '" + NBCEL.verifier.statics.Pass2Verifier
										.Tostring(lvt) + "' references a local variable by the name '" + localname + "' which is not a legal Java simple name."
										);
								}
								this.CheckIndex(lvt, localvariable.GetSignatureIndex(), this.CONST_Utf8);
								string localsig = ((NBCEL.classfile.ConstantUtf8)(this.cp.GetConstant(localvariable
									.GetSignatureIndex()))).GetBytes();
								// Local sig.(=descriptor)
								NBCEL.generic.Type t;
								try
								{
									t = NBCEL.generic.Type.GetType(localsig);
								}
								catch (NBCEL.classfile.ClassFormatException cfe)
								{
									throw new NBCEL.verifier.exc.ClassConstraintException("Illegal descriptor (==signature) '"
										 + localsig + "' used by LocalVariable '" + NBCEL.verifier.statics.Pass2Verifier
										.Tostring(localvariable) + "' referenced by '" + NBCEL.verifier.statics.Pass2Verifier
										.Tostring(lvt) + "'.", cfe);
								}
								int localindex = localvariable.GetIndex();
								if (((t == NBCEL.generic.Type.LONG || t == NBCEL.generic.Type.DOUBLE) ? localindex
									 + 1 : localindex) >= code.GetMaxLocals())
								{
									throw new NBCEL.verifier.exc.ClassConstraintException("LocalVariableTable attribute '"
										 + NBCEL.verifier.statics.Pass2Verifier.Tostring(lvt) + "' references a LocalVariable '"
										 + NBCEL.verifier.statics.Pass2Verifier.Tostring(localvariable) + "' with an index that exceeds the surrounding Code attribute's max_locals value of '"
										 + code.GetMaxLocals() + "'.");
								}
								try
								{
									this._enclosing.localVariablesInfos[method_number].Add(localindex, localname, localvariable
										.GetStartPC(), localvariable.GetLength(), t);
								}
								catch (NBCEL.verifier.exc.LocalVariableInfoInconsistentException lviie)
								{
									throw new NBCEL.verifier.exc.ClassConstraintException("Conflicting information in LocalVariableTable '"
										 + NBCEL.verifier.statics.Pass2Verifier.Tostring(lvt) + "' found in Code attribute '"
										 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' (method '" + NBCEL.verifier.statics.Pass2Verifier
										.Tostring(m) + "'). " + lviie.Message, lviie);
								}
							}
							// for all local variables localvariables[i] in the LocalVariableTable attribute atts[a] END
							num_of_lvt_attribs++;
							if (!m.IsStatic() && num_of_lvt_attribs > obj.GetMaxLocals())
							{
								throw new NBCEL.verifier.exc.ClassConstraintException("Number of LocalVariableTable attributes of Code attribute '"
									 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' (method '" + NBCEL.verifier.statics.Pass2Verifier
									.Tostring(m) + "') exceeds number of local variable slots '" + obj.GetMaxLocals(
									) + "' ('There may be at most one LocalVariableTable attribute per local variable in the Code attribute.')."
									);
							}
						}
					}
				}
				catch (System.TypeLoadException e)
				{
					// if atts[a] instanceof LocalVariableTable END
					// for all attributes atts[a] END
					// FIXME: this might not be the best way to handle missing classes.
					throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
				}
			}

			// visitCode(Code) END
			public override void VisitExceptionTable(NBCEL.classfile.ExceptionTable obj)
			{
				//vmspec2 4.7.4
				try
				{
					// incorrectly named, it's the Exceptions attribute (vmspec2 4.7.4)
					this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
					string name = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(obj.GetNameIndex
						())).GetBytes();
					if (!name.Equals("Exceptions"))
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("The Exceptions attribute '"
							 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' is not correctly named 'Exceptions' but '"
							 + name + "'.");
					}
					int[] exc_indices = obj.GetExceptionIndexTable();
					foreach (int exc_indice in exc_indices)
					{
						this.CheckIndex(obj, exc_indice, this.CONST_Class);
						NBCEL.classfile.ConstantClass cc = (NBCEL.classfile.ConstantClass)(this.cp.GetConstant
							(exc_indice));
						this.CheckIndex(cc, cc.GetNameIndex(), this.CONST_Utf8);
						// can't be sure this ConstantClass has already been visited (checked)!
						//convert internal notation on-the-fly to external notation:
						string cname = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(cc.GetNameIndex
							())).GetBytes().Replace('/', '.');
						NBCEL.verifier.Verifier v = NBCEL.verifier.VerifierFactory.GetVerifier(cname);
						NBCEL.verifier.VerificationResult vr = v.DoPass1();
						if (vr != NBCEL.verifier.VerificationResult.VR_OK)
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Exceptions attribute '" + 
								NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' references '" + cname + 
								"' as an Exception but it does not pass verification pass 1: " + vr);
						}
						// We cannot safely trust any other "instanceof" mechanism. We need to transitively verify
						// the ancestor hierarchy.
						NBCEL.classfile.JavaClass e = NBCEL.Repository.LookupClass(cname);
						NBCEL.classfile.JavaClass t = NBCEL.Repository.LookupClass(NBCEL.generic.Type.THROWABLE
							.GetClassName());
						NBCEL.classfile.JavaClass o = NBCEL.Repository.LookupClass(NBCEL.generic.Type.OBJECT
							.GetClassName());
						while (e != o)
						{
							if (e == t)
							{
								break;
							}
							// It's a subclass of Throwable, OKAY, leave.
							v = NBCEL.verifier.VerifierFactory.GetVerifier(e.GetSuperclassName());
							vr = v.DoPass1();
							if (vr != NBCEL.verifier.VerificationResult.VR_OK)
							{
								throw new NBCEL.verifier.exc.ClassConstraintException("Exceptions attribute '" + 
									NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' references '" + cname + 
									"' as an Exception but '" + e.GetSuperclassName() + "' in the ancestor hierachy does not pass verification pass 1: "
									 + vr);
							}
							e = NBCEL.Repository.LookupClass(e.GetSuperclassName());
						}
						if (e != t)
						{
							throw new NBCEL.verifier.exc.ClassConstraintException("Exceptions attribute '" + 
								NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' references '" + cname + 
								"' as an Exception but it is not a subclass of '" + t.GetClassName() + "'.");
						}
					}
				}
				catch (System.TypeLoadException e)
				{
					// FIXME: this might not be the best way to handle missing classes.
					throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
				}
			}

			// SYNTHETIC: see above
			// DEPRECATED: see above
			//////////////////////////////////////////////////////////////
			// code_attribute-structure-ATTRIBUTES (vmspec2 4.7.3, 4.7) //
			//////////////////////////////////////////////////////////////
			public override void VisitLineNumberTable(NBCEL.classfile.LineNumberTable obj)
			{
				//vmspec2 4.7.8
				this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
				string name = ((NBCEL.classfile.ConstantUtf8)this.cp.GetConstant(obj.GetNameIndex
					())).GetBytes();
				if (!name.Equals("LineNumberTable"))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("The LineNumberTable attribute '"
						 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' is not correctly named 'LineNumberTable' but '"
						 + name + "'.");
				}
			}

			//In JustIce,this check is delayed to Pass 3a.
			//LineNumber[] linenumbers = obj.getLineNumberTable();
			// ...validity check...
			public override void VisitLocalVariableTable(NBCEL.classfile.LocalVariableTable obj
				)
			{
			}

			//vmspec2 4.7.9
			//In JustIce,this check is partially delayed to Pass 3a.
			//The other part can be found in the visitCode(Code) method.
			////////////////////////////////////////////////////
			// MISC-structure-ATTRIBUTES (vmspec2 4.7.1, 4.7) //
			////////////////////////////////////////////////////
			public override void VisitUnknown(NBCEL.classfile.Unknown obj)
			{
				//vmspec2 4.7.1
				// Represents an unknown attribute.
				this.CheckIndex(obj, obj.GetNameIndex(), this.CONST_Utf8);
				// Maybe only misnamed? Give a (warning) message.
				this._enclosing.AddMessage("Unknown attribute '" + NBCEL.verifier.statics.Pass2Verifier
					.Tostring(obj) + "'. This attribute is not known in any context!");
			}

			//////////
			// BCEL //
			//////////
			public override void VisitLocalVariable(NBCEL.classfile.LocalVariable obj)
			{
			}

			// This does not represent an Attribute but is only
			// related to internal BCEL data representation.
			// see visitLocalVariableTable(LocalVariableTable)
			public override void VisitCodeException(NBCEL.classfile.CodeException obj)
			{
			}

			// Code constraints are checked in Pass3 (3a and 3b).
			// This does not represent an Attribute but is only
			// related to internal BCEL data representation.
			// see visitCode(Code)
			public override void VisitConstantPool(NBCEL.classfile.ConstantPool obj)
			{
			}

			// No need to. We're piggybacked by the DescendingVisitor.
			// This does not represent an Attribute but is only
			// related to internal BCEL data representation.
			public override void VisitInnerClass(NBCEL.classfile.InnerClass obj)
			{
			}

			// This does not represent an Attribute but is only
			// related to internal BCEL data representation.
			public override void VisitLineNumber(NBCEL.classfile.LineNumber obj)
			{
			}

			private readonly Pass2Verifier _enclosing;
			// This does not represent an Attribute but is only
			// related to internal BCEL data representation.
			// see visitLineNumberTable(LineNumberTable)
		}

		/// <summary>
		/// Ensures that the ConstantCP-subclassed entries of the constant
		/// pool are valid.
		/// </summary>
		/// <remarks>
		/// Ensures that the ConstantCP-subclassed entries of the constant
		/// pool are valid. According to "Yellin: Low Level Security in Java",
		/// this method does not verify the existence of referenced entities
		/// (such as classes) but only the formal correctness (such as well-formed
		/// signatures).
		/// The visitXXX() methods throw ClassConstraintException instances otherwise.
		/// <B>Precondition: index-style cross referencing in the constant
		/// pool must be valid. Simply invoke constant_pool_entries_satisfy_static_constraints()
		/// before.</B>
		/// </remarks>
		/// <exception cref="NBCEL.verifier.exc.ClassConstraintException">otherwise.</exception>
		/// <seealso cref="Constant_pool_entries_satisfy_static_constraints()"/>
		private void Field_and_method_refs_are_valid()
		{
			try
			{
				NBCEL.classfile.JavaClass jc = NBCEL.Repository.LookupClass(myOwner.GetClassName(
					));
				NBCEL.classfile.DescendingVisitor v = new NBCEL.classfile.DescendingVisitor(jc, new 
					NBCEL.verifier.statics.Pass2Verifier.FAMRAV_Visitor(this, jc));
				v.Visit();
			}
			catch (System.TypeLoadException e)
			{
				// FIXME: this might not be the best way to handle missing classes.
				throw new NBCEL.verifier.exc.AssertionViolatedException("Missing class: " + e, e);
			}
		}

		/// <summary>
		/// A Visitor class that ensures the ConstantCP-subclassed entries
		/// of the constant pool are valid.
		/// </summary>
		/// <remarks>
		/// A Visitor class that ensures the ConstantCP-subclassed entries
		/// of the constant pool are valid.
		/// <B>Precondition: index-style cross referencing in the constant
		/// pool must be valid.</B>
		/// </remarks>
		/// <seealso cref="Pass2Verifier.Constant_pool_entries_satisfy_static_constraints()"/
		/// 	>
		/// <seealso cref="NBCEL.classfile.ConstantCP"/>
		private sealed class FAMRAV_Visitor : NBCEL.classfile.EmptyVisitor
		{
			private readonly NBCEL.classfile.ConstantPool cp;

			internal FAMRAV_Visitor(Pass2Verifier _enclosing, NBCEL.classfile.JavaClass _jc)
			{
				this._enclosing = _enclosing;
				// ==jc.getConstantPool() -- only here to save typing work.
				this.cp = _jc.GetConstantPool();
			}

			public override void VisitConstantFieldref(NBCEL.classfile.ConstantFieldref obj)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_Fieldref)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("ConstantFieldref '" + NBCEL.verifier.statics.Pass2Verifier
						.Tostring(obj) + "' has wrong tag!");
				}
				int name_and_type_index = obj.GetNameAndTypeIndex();
				NBCEL.classfile.ConstantNameAndType cnat = (NBCEL.classfile.ConstantNameAndType)(
					this.cp.GetConstant(name_and_type_index));
				string name = ((NBCEL.classfile.ConstantUtf8)(this.cp.GetConstant(cnat.GetNameIndex
					()))).GetBytes();
				// Field or Method name
				if (!NBCEL.verifier.statics.Pass2Verifier.ValidFieldName(name))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Invalid field name '" + name
						 + "' referenced by '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'."
						);
				}
				int class_index = obj.GetClassIndex();
				NBCEL.classfile.ConstantClass cc = (NBCEL.classfile.ConstantClass)(this.cp.GetConstant
					(class_index));
				string className = ((NBCEL.classfile.ConstantUtf8)(this.cp.GetConstant(cc.GetNameIndex
					()))).GetBytes();
				// Class Name in internal form
				if (!NBCEL.verifier.statics.Pass2Verifier.ValidClassName(className))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Illegal class name '" + className
						 + "' used by '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
				string sig = ((NBCEL.classfile.ConstantUtf8)(this.cp.GetConstant(cnat.GetSignatureIndex
					()))).GetBytes();
				// Field or Method sig.(=descriptor)
				try
				{
					NBCEL.generic.Type.GetType(sig);
				}
				catch (NBCEL.classfile.ClassFormatException cfe)
				{
					/* Don't need the return value */
					throw new NBCEL.verifier.exc.ClassConstraintException("Illegal descriptor (==signature) '"
						 + sig + "' used by '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'."
						, cfe);
				}
			}

			public override void VisitConstantMethodref(NBCEL.classfile.ConstantMethodref obj
				)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_Methodref)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("ConstantMethodref '" + NBCEL.verifier.statics.Pass2Verifier
						.Tostring(obj) + "' has wrong tag!");
				}
				int name_and_type_index = obj.GetNameAndTypeIndex();
				NBCEL.classfile.ConstantNameAndType cnat = (NBCEL.classfile.ConstantNameAndType)(
					this.cp.GetConstant(name_and_type_index));
				string name = ((NBCEL.classfile.ConstantUtf8)(this.cp.GetConstant(cnat.GetNameIndex
					()))).GetBytes();
				// Field or Method name
				if (!NBCEL.verifier.statics.Pass2Verifier.ValidClassMethodName(name))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Invalid (non-interface) method name '"
						 + name + "' referenced by '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj
						) + "'.");
				}
				int class_index = obj.GetClassIndex();
				NBCEL.classfile.ConstantClass cc = (NBCEL.classfile.ConstantClass)(this.cp.GetConstant
					(class_index));
				string className = ((NBCEL.classfile.ConstantUtf8)(this.cp.GetConstant(cc.GetNameIndex
					()))).GetBytes();
				// Class Name in internal form
				if (!NBCEL.verifier.statics.Pass2Verifier.ValidClassName(className))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Illegal class name '" + className
						 + "' used by '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
				string sig = ((NBCEL.classfile.ConstantUtf8)(this.cp.GetConstant(cnat.GetSignatureIndex
					()))).GetBytes();
				// Field or Method sig.(=descriptor)
				try
				{
					NBCEL.generic.Type t = NBCEL.generic.Type.GetReturnType(sig);
					if (name.Equals(NBCEL.Const.CONSTRUCTOR_NAME) && (t != NBCEL.generic.Type.VOID))
					{
						throw new NBCEL.verifier.exc.ClassConstraintException("Instance initialization method must have VOID return type."
							);
					}
				}
				catch (NBCEL.classfile.ClassFormatException cfe)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Illegal descriptor (==signature) '"
						 + sig + "' used by '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'."
						, cfe);
				}
			}

			public override void VisitConstantInterfaceMethodref(NBCEL.classfile.ConstantInterfaceMethodref
				 obj)
			{
				if (obj.GetTag() != NBCEL.Const.CONSTANT_InterfaceMethodref)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("ConstantInterfaceMethodref '"
						 + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "' has wrong tag!");
				}
				int name_and_type_index = obj.GetNameAndTypeIndex();
				NBCEL.classfile.ConstantNameAndType cnat = (NBCEL.classfile.ConstantNameAndType)(
					this.cp.GetConstant(name_and_type_index));
				string name = ((NBCEL.classfile.ConstantUtf8)(this.cp.GetConstant(cnat.GetNameIndex
					()))).GetBytes();
				// Field or Method name
				if (!NBCEL.verifier.statics.Pass2Verifier.ValidInterfaceMethodName(name))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Invalid (interface) method name '"
						 + name + "' referenced by '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj
						) + "'.");
				}
				int class_index = obj.GetClassIndex();
				NBCEL.classfile.ConstantClass cc = (NBCEL.classfile.ConstantClass)(this.cp.GetConstant
					(class_index));
				string className = ((NBCEL.classfile.ConstantUtf8)(this.cp.GetConstant(cc.GetNameIndex
					()))).GetBytes();
				// Class Name in internal form
				if (!NBCEL.verifier.statics.Pass2Verifier.ValidClassName(className))
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Illegal class name '" + className
						 + "' used by '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'.");
				}
				string sig = ((NBCEL.classfile.ConstantUtf8)(this.cp.GetConstant(cnat.GetSignatureIndex
					()))).GetBytes();
				// Field or Method sig.(=descriptor)
				try
				{
					NBCEL.generic.Type t = NBCEL.generic.Type.GetReturnType(sig);
					if (name.Equals(NBCEL.Const.STATIC_INITIALIZER_NAME) && (t != NBCEL.generic.Type.
						VOID))
					{
						this._enclosing.AddMessage("Class or interface initialization method '" + NBCEL.Const
							.STATIC_INITIALIZER_NAME + "' usually has VOID return type instead of '" + t + "'. Note this is really not a requirement of The Java Virtual Machine Specification, Second Edition."
							);
					}
				}
				catch (NBCEL.classfile.ClassFormatException cfe)
				{
					throw new NBCEL.verifier.exc.ClassConstraintException("Illegal descriptor (==signature) '"
						 + sig + "' used by '" + NBCEL.verifier.statics.Pass2Verifier.Tostring(obj) + "'."
						, cfe);
				}
			}

			private readonly Pass2Verifier _enclosing;
		}

		/// <summary>
		/// This method returns true if and only if the supplied String
		/// represents a valid Java class name.
		/// </summary>
		private static bool ValidClassName(string name)
		{
			/*
			* TODO: implement.
			* Are there any restrictions?
			*/
			return true;
		}

		/// <summary>
		/// This method returns true if and only if the supplied String
		/// represents a valid method name.
		/// </summary>
		/// <remarks>
		/// This method returns true if and only if the supplied String
		/// represents a valid method name.
		/// This is basically the same as a valid identifier name in the
		/// Java programming language, but the special name for
		/// the instance initialization method is allowed and the special name
		/// for the class/interface initialization method may be allowed.
		/// </remarks>
		private static bool ValidMethodName(string name, bool allowStaticInit)
		{
			if (ValidJavaLangMethodName(name))
			{
				return true;
			}
			if (allowStaticInit)
			{
				return name.Equals(NBCEL.Const.CONSTRUCTOR_NAME) || name.Equals(NBCEL.Const.STATIC_INITIALIZER_NAME
					);
			}
			return name.Equals(NBCEL.Const.CONSTRUCTOR_NAME);
		}

		/// <summary>
		/// This method returns true if and only if the supplied String
		/// represents a valid method name that may be referenced by
		/// ConstantMethodref objects.
		/// </summary>
		private static bool ValidClassMethodName(string name)
		{
			return ValidMethodName(name, false);
		}

		/// <summary>
		/// This method returns true if and only if the supplied String
		/// represents a valid Java programming language method name stored as a simple
		/// (non-qualified) name.
		/// </summary>
		/// <remarks>
		/// This method returns true if and only if the supplied String
		/// represents a valid Java programming language method name stored as a simple
		/// (non-qualified) name.
		/// Conforming to: The Java Virtual Machine Specification, Second Edition, �2.7, �2.7.1, �2.2.
		/// </remarks>
		private static bool ValidJavaLangMethodName(string name)
		{
			/*
			if (!char.IsJavaIdentifierStart(name[0]))
			{
				return false;
			}
			*/
			for (int i = 1; i < name.Length; i++)
			{
				if (!Runtime.IsJavaIdentifierPart(name[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// This method returns true if and only if the supplied String
		/// represents a valid Java interface method name that may be
		/// referenced by ConstantInterfaceMethodref objects.
		/// </summary>
		private static bool ValidInterfaceMethodName(string name)
		{
			// I guess we should assume special names forbidden here.
			if (name.StartsWith("<"))
			{
				return false;
			}
			return ValidJavaLangMethodName(name);
		}

		/// <summary>
		/// This method returns true if and only if the supplied String
		/// represents a valid Java identifier (so-called simple name).
		/// </summary>
		private static bool ValidJavaIdentifier(string name)
		{
			if (name.Length == 0)
			{
				return false;
			}
			// must not be empty, reported by <francis.andre@easynet.fr>, thanks!
			// vmspec2 2.7, vmspec2 2.2
			/*
			if (!char.IsJavaIdentifierStart(name[0]))
			{
				return false;
			}
			*/
			for (int i = 1; i < name.Length; i++)
			{
				if (!Runtime.IsJavaIdentifierPart(name[i]))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// This method returns true if and only if the supplied String
		/// represents a valid Java field name.
		/// </summary>
		private static bool ValidFieldName(string name)
		{
			// vmspec2 2.7, vmspec2 2.2
			return ValidJavaIdentifier(name);
		}

		/// <summary>
		/// This class serves for finding out if a given JavaClass' ConstantPool
		/// references an Inner Class.
		/// </summary>
		/// <remarks>
		/// This class serves for finding out if a given JavaClass' ConstantPool
		/// references an Inner Class.
		/// The Java Virtual Machine Specification, Second Edition is not very precise
		/// about when an "InnerClasses" attribute has to appear. However, it states that
		/// there has to be exactly one InnerClasses attribute in the ClassFile structure
		/// if the constant pool of a class or interface refers to any class or interface
		/// "that is not a member of a package". Sun does not mean "member of the default
		/// package". In "Inner Classes Specification" they point out how a "bytecode name"
		/// is derived so one has to deduce what a class name of a class "that is not a
		/// member of a package" looks like: there is at least one character in the byte-
		/// code name that cannot be part of a legal Java Language Class name (and not equal
		/// to '/'). This assumption is wrong as the delimiter is '$' for which
		/// Character.isJavaIdentifierPart() == true.
		/// Hence, you really run into trouble if you have a toplevel class called
		/// "A$XXX" and another toplevel class called "A" with in inner class called "XXX".
		/// JustIce cannot repair this; please note that existing verifiers at this
		/// time even fail to detect missing InnerClasses attributes in pass 2.
		/// </remarks>
		private class InnerClassDetector : NBCEL.classfile.EmptyVisitor
		{
			private bool hasInnerClass = false;

			private readonly NBCEL.classfile.JavaClass jc;

			private readonly NBCEL.classfile.ConstantPool cp;

			/// <summary>Constructs an InnerClassDetector working on the JavaClass _jc.</summary>
			public InnerClassDetector(NBCEL.classfile.JavaClass _jc)
			{
				jc = _jc;
				cp = jc.GetConstantPool();
				(new NBCEL.classfile.DescendingVisitor(jc, this)).Visit();
			}

			/// <summary>
			/// Returns if the JavaClass this InnerClassDetector is working on
			/// has an Inner Class reference in its constant pool.
			/// </summary>
			/// <returns>Whether this InnerClassDetector is working on has an Inner Class reference in its constant pool.
			/// 	</returns>
			public virtual bool InnerClassReferenced()
			{
				return hasInnerClass;
			}

			/// <summary>This method casually visits ConstantClass references.</summary>
			public override void VisitConstantClass(NBCEL.classfile.ConstantClass obj)
			{
				NBCEL.classfile.Constant c = cp.GetConstant(obj.GetNameIndex());
				if (c is NBCEL.classfile.ConstantUtf8)
				{
					//Ignore the case where it's not a ConstantUtf8 here, we'll find out later.
					string classname = ((NBCEL.classfile.ConstantUtf8)c).GetBytes();
					if (classname.StartsWith(jc.GetClassName().Replace('.', '/') + "$"))
					{
						hasInnerClass = true;
					}
				}
			}
		}

		/// <summary>This method is here to save typing work and improve code readability.</summary>
		private static string Tostring(NBCEL.classfile.Node n)
		{
			return new NBCEL.verifier.statics.StringRepresentation(n).ToString();
		}
	}
}
