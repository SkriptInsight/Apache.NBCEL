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
using System.Collections.Generic;
using Apache.NBCEL.ClassFile;
using Apache.NBCEL.Generic;
using Apache.NBCEL.Verifier.Exc;
using Type = System.Type;

namespace Apache.NBCEL.Verifier.Statics
{
	/// <summary>
	///     This PassVerifier verifies a class file according to
	///     pass 2 as described in The Java Virtual Machine
	///     Specification, 2nd edition.
	/// </summary>
	/// <remarks>
	///     This PassVerifier verifies a class file according to
	///     pass 2 as described in The Java Virtual Machine
	///     Specification, 2nd edition.
	///     More detailed information is to be found at the do_verify()
	///     method's documentation.
	/// </remarks>
	/// <seealso cref="Do_verify()" />
	public sealed class Pass2Verifier : PassVerifier
    {
        /// <summary>The Verifier that created this.</summary>
        private readonly Verifier myOwner;

        /// <summary>The LocalVariableInfo instances used by Pass3bVerifier.</summary>
        /// <remarks>
        ///     The LocalVariableInfo instances used by Pass3bVerifier.
        ///     localVariablesInfos[i] denotes the information for the
        ///     local variables of method number i in the
        ///     JavaClass this verifier operates on.
        /// </remarks>
        private LocalVariablesInfo[] localVariablesInfos;

        /// <summary>Should only be instantiated by a Verifier.</summary>
        /// <seealso cref="Verifier" />
        public Pass2Verifier(Verifier owner)
        {
            myOwner = owner;
        }

        /// <summary>
        ///     Returns a LocalVariablesInfo object containing information
        ///     about the usage of the local variables in the Code attribute
        ///     of the said method or <B>null</B> if the class file this
        ///     Pass2Verifier operates on could not be pass-2-verified correctly.
        /// </summary>
        /// <remarks>
        ///     Returns a LocalVariablesInfo object containing information
        ///     about the usage of the local variables in the Code attribute
        ///     of the said method or <B>null</B> if the class file this
        ///     Pass2Verifier operates on could not be pass-2-verified correctly.
        ///     The method number method_nr is the method you get using
        ///     <B>Repository.lookupClass(myOwner.getClassname()).getMethods()[method_nr];</B>.
        ///     You should not add own information. Leave that to JustIce.
        /// </remarks>
        public LocalVariablesInfo GetLocalVariablesInfo(int method_nr
        )
        {
            if (Verify() != VerificationResult.VR_OK) return null;
            // It's cached, don't worry.
            if (method_nr < 0 || method_nr >= localVariablesInfos.Length)
                throw new AssertionViolatedException("Method number out of range."
                );
            return localVariablesInfos[method_nr];
        }

        /// <summary>
        ///     Pass 2 is the pass where static properties of the
        ///     class file are checked without looking into "Code"
        ///     arrays of methods.
        /// </summary>
        /// <remarks>
        ///     Pass 2 is the pass where static properties of the
        ///     class file are checked without looking into "Code"
        ///     arrays of methods.
        ///     This verification pass is usually invoked when
        ///     a class is resolved; and it may be possible that
        ///     this verification pass has to load in other classes
        ///     such as superclasses or implemented interfaces.
        ///     Therefore, Pass 1 is run on them.
        ///     <BR>
        ///         Note that most referenced classes are <B>not</B> loaded
        ///         in for verification or for an existance check by this
        ///         pass; only the syntactical correctness of their names
        ///         and descriptors (a.k.a. signatures) is checked.
        ///         <BR>
        ///             Very few checks that conceptually belong here
        ///             are delayed until pass 3a in JustIce. JustIce does
        ///             not only check for syntactical correctness but also
        ///             for semantical sanity - therefore it needs access to
        ///             the "Code" array of methods in a few cases. Please
        ///             see the pass 3a documentation, too.
        /// </remarks>
        /// <seealso cref="Pass3aVerifier" />
        public override VerificationResult Do_verify()
        {
            try
            {
                var vr1 = myOwner.DoPass1();
                if (vr1.Equals(VerificationResult.VR_OK))
                {
                    // For every method, we could have information about the local variables out of LocalVariableTable attributes of
                    // the Code attributes.
                    localVariablesInfos = new LocalVariablesInfo[Repository
                        .LookupClass(myOwner.GetClassName()).GetMethods().Length];
                    var vr = VerificationResult.VR_OK;
                    // default.
                    try
                    {
                        Constant_pool_entries_satisfy_static_constraints();
                        Field_and_method_refs_are_valid();
                        Every_class_has_an_accessible_superclass();
                        Final_methods_are_not_overridden();
                    }
                    catch (ClassConstraintException cce)
                    {
                        vr = new VerificationResult(VerificationResult.VERIFIED_REJECTED
                            , cce.Message);
                    }

                    return vr;
                }

                return VerificationResult.VR_NOTYET;
            }
            catch (TypeLoadException e)
            {
                // FIXME: this might not be the best way to handle missing classes.
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>
        ///     Ensures that every class has a super class and that
        ///     <B>final</B> classes are not subclassed.
        /// </summary>
        /// <remarks>
        ///     Ensures that every class has a super class and that
        ///     <B>final</B> classes are not subclassed.
        ///     This means, the class this Pass2Verifier operates
        ///     on has proper super classes (transitively) up to
        ///     java.lang.Object.
        ///     The reason for really loading (and Pass1-verifying)
        ///     all of those classes here is that we need them in
        ///     Pass2 anyway to verify no final methods are overridden
        ///     (that could be declared anywhere in the ancestor hierarchy).
        /// </remarks>
        /// <exception cref="ClassConstraintException">otherwise.</exception>
        private void Every_class_has_an_accessible_superclass()
        {
            try
            {
                var hs = new HashSet
                    <string>();
                // save class names to detect circular inheritance
                var jc = Repository.LookupClass(myOwner.GetClassName(
                ));
                var supidx = -1;
                while (supidx != 0)
                {
                    supidx = jc.GetSuperclassNameIndex();
                    if (supidx == 0)
                    {
                        if (jc != Repository.LookupClass(Generic.Type.OBJECT.GetClassName()))
                            throw new ClassConstraintException("Superclass of '" + jc.GetClassName
                                                                   () + "' missing but not " +
                                                               Generic.Type.OBJECT.GetClassName() + " itself!"
                            );
                    }
                    else
                    {
                        var supername = jc.GetSuperclassName();
                        if (!hs.Add(supername))
                            // If supername already is in the list
                            throw new ClassConstraintException("Circular superclass hierarchy detected."
                            );
                        var v = VerifierFactory.GetVerifier(supername);
                        var vr = v.DoPass1();
                        if (vr != VerificationResult.VR_OK)
                            throw new ClassConstraintException("Could not load in ancestor class '"
                                                               + supername + "'.");
                        jc = Repository.LookupClass(supername);
                        if (jc.IsFinal())
                            throw new ClassConstraintException("Ancestor class '" + supername
                                                                                  + "' has the FINAL access modifier and must therefore not be subclassed.");
                    }
                }
            }
            catch (TypeLoadException e)
            {
                // FIXME: this might not be the best way to handle missing classes.
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>Ensures that <B>final</B> methods are not overridden.</summary>
        /// <remarks>
        ///     Ensures that <B>final</B> methods are not overridden.
        ///     <B>
        ///         Precondition to run this method:
        ///         constant_pool_entries_satisfy_static_constraints() and
        ///         every_class_has_an_accessible_superclass() have to be invoked before
        ///         (in that order).
        ///     </B>
        /// </remarks>
        /// <exception cref="ClassConstraintException">otherwise.</exception>
        /// <seealso cref="Constant_pool_entries_satisfy_static_constraints()" />
        /// <seealso cref="Every_class_has_an_accessible_superclass()" />
        private void Final_methods_are_not_overridden()
        {
            try
            {
                IDictionary<string, string> hashmap = new Dictionary
                    <string, string>();
                var jc = Repository.LookupClass(myOwner.GetClassName(
                ));
                var supidx = -1;
                while (supidx != 0)
                {
                    supidx = jc.GetSuperclassNameIndex();
                    var methods = jc.GetMethods();
                    foreach (var method in methods)
                    {
                        var nameAndSig = method.GetName() + method.GetSignature();
                        if (hashmap.ContainsKey(nameAndSig))
                        {
                            if (method.IsFinal())
                            {
                                if (!method.IsPrivate())
                                    throw new ClassConstraintException("Method '" + nameAndSig + "' in class '"
                                                                       + hashmap.GetOrNull(nameAndSig) +
                                                                       "' overrides the final (not-overridable) definition in class '"
                                                                       + jc.GetClassName() + "'.");
                                AddMessage("Method '" + nameAndSig + "' in class '" + hashmap.GetOrNull(nameAndSig
                                           ) + "' overrides the final (not-overridable) definition in class '" +
                                           jc.GetClassName
                                               () +
                                           "'. This is okay, as the original definition was private; however this constraint leverage"
                                           + " was introduced by JLS 8.4.6 (not vmspec2) and the behavior of the Sun verifiers."
                                );
                            }
                            else if (!method.IsStatic())
                            {
                                // static methods don't inherit
                                Collections.Put(hashmap, nameAndSig, jc.GetClassName());
                            }
                        }
                        else if (!method.IsStatic())
                        {
                            // static methods don't inherit
                            Collections.Put(hashmap, nameAndSig, jc.GetClassName());
                        }
                    }

                    jc = Repository.LookupClass(jc.GetSuperclassName());
                }
            }
            catch (TypeLoadException e)
            {
                // Well, for OBJECT this returns OBJECT so it works (could return anything but must not throw an Exception).
                // FIXME: this might not be the best way to handle missing classes.
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>
        ///     Ensures that the constant pool entries satisfy the static constraints
        ///     as described in The Java Virtual Machine Specification, 2nd Edition.
        /// </summary>
        /// <exception cref="ClassConstraintException">otherwise.</exception>
        private void Constant_pool_entries_satisfy_static_constraints()
        {
            try
            {
                // Most of the consistency is handled internally by BCEL; here
                // we only have to verify if the indices of the constants point
                // to constants of the appropriate type and such.
                var jc = Repository.LookupClass(myOwner.GetClassName(
                ));
                new CPESSC_Visitor(this, jc);
            }
            catch (TypeLoadException e)
            {
                // constructor implicitly traverses jc
                // FIXME: this might not be the best way to handle missing classes.
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>
        ///     Ensures that the ConstantCP-subclassed entries of the constant
        ///     pool are valid.
        /// </summary>
        /// <remarks>
        ///     Ensures that the ConstantCP-subclassed entries of the constant
        ///     pool are valid. According to "Yellin: Low Level Security in Java",
        ///     this method does not verify the existence of referenced entities
        ///     (such as classes) but only the formal correctness (such as well-formed
        ///     signatures).
        ///     The visitXXX() methods throw ClassConstraintException instances otherwise.
        ///     <B>
        ///         Precondition: index-style cross referencing in the constant
        ///         pool must be valid. Simply invoke constant_pool_entries_satisfy_static_constraints()
        ///         before.
        ///     </B>
        /// </remarks>
        /// <exception cref="ClassConstraintException">otherwise.</exception>
        /// <seealso cref="Constant_pool_entries_satisfy_static_constraints()" />
        private void Field_and_method_refs_are_valid()
        {
            try
            {
                var jc = Repository.LookupClass(myOwner.GetClassName(
                ));
                var v = new DescendingVisitor(jc, new
                    FAMRAV_Visitor(this, jc));
                v.Visit();
            }
            catch (TypeLoadException e)
            {
                // FIXME: this might not be the best way to handle missing classes.
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>
        ///     This method returns true if and only if the supplied String
        ///     represents a valid Java class name.
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
        ///     This method returns true if and only if the supplied String
        ///     represents a valid method name.
        /// </summary>
        /// <remarks>
        ///     This method returns true if and only if the supplied String
        ///     represents a valid method name.
        ///     This is basically the same as a valid identifier name in the
        ///     Java programming language, but the special name for
        ///     the instance initialization method is allowed and the special name
        ///     for the class/interface initialization method may be allowed.
        /// </remarks>
        private static bool ValidMethodName(string name, bool allowStaticInit)
        {
            if (ValidJavaLangMethodName(name)) return true;
            if (allowStaticInit)
                return name.Equals(Const.CONSTRUCTOR_NAME) || name.Equals(Const.STATIC_INITIALIZER_NAME
                       );
            return name.Equals(Const.CONSTRUCTOR_NAME);
        }

        /// <summary>
        ///     This method returns true if and only if the supplied String
        ///     represents a valid method name that may be referenced by
        ///     ConstantMethodref objects.
        /// </summary>
        private static bool ValidClassMethodName(string name)
        {
            return ValidMethodName(name, false);
        }

        /// <summary>
        ///     This method returns true if and only if the supplied String
        ///     represents a valid Java programming language method name stored as a simple
        ///     (non-qualified) name.
        /// </summary>
        /// <remarks>
        ///     This method returns true if and only if the supplied String
        ///     represents a valid Java programming language method name stored as a simple
        ///     (non-qualified) name.
        ///     Conforming to: The Java Virtual Machine Specification, Second Edition, �2.7, �2.7.1, �2.2.
        /// </remarks>
        private static bool ValidJavaLangMethodName(string name)
        {
            /*
            if (!char.IsJavaIdentifierStart(name[0]))
            {
                return false;
            }
            */
            for (var i = 1; i < name.Length; i++)
                if (!Runtime.IsJavaIdentifierPart(name[i]))
                    return false;
            return true;
        }

        /// <summary>
        ///     This method returns true if and only if the supplied String
        ///     represents a valid Java interface method name that may be
        ///     referenced by ConstantInterfaceMethodref objects.
        /// </summary>
        private static bool ValidInterfaceMethodName(string name)
        {
            // I guess we should assume special names forbidden here.
            if (name.StartsWith("<")) return false;
            return ValidJavaLangMethodName(name);
        }

        /// <summary>
        ///     This method returns true if and only if the supplied String
        ///     represents a valid Java identifier (so-called simple name).
        /// </summary>
        private static bool ValidJavaIdentifier(string name)
        {
            if (name.Length == 0) return false;
            // must not be empty, reported by <francis.andre@easynet.fr>, thanks!
            // vmspec2 2.7, vmspec2 2.2
            /*
            if (!char.IsJavaIdentifierStart(name[0]))
            {
                return false;
            }
            */
            for (var i = 1; i < name.Length; i++)
                if (!Runtime.IsJavaIdentifierPart(name[i]))
                    return false;
            return true;
        }

        /// <summary>
        ///     This method returns true if and only if the supplied String
        ///     represents a valid Java field name.
        /// </summary>
        private static bool ValidFieldName(string name)
        {
            // vmspec2 2.7, vmspec2 2.2
            return ValidJavaIdentifier(name);
        }

        /// <summary>This method is here to save typing work and improve code readability.</summary>
        private static string Tostring(Node n)
        {
            return new StringRepresentation(n).ToString();
        }

        /// <summary>
        ///     A Visitor class that ensures the constant pool satisfies the static
        ///     constraints.
        /// </summary>
        /// <remarks>
        ///     A Visitor class that ensures the constant pool satisfies the static
        ///     constraints.
        ///     The visitXXX() methods throw ClassConstraintException instances otherwise.
        /// </remarks>
        /// <seealso cref="Pass2Verifier.Constant_pool_entries_satisfy_static_constraints()"/>
        private sealed class CPESSC_Visitor : ClassFile.EmptyVisitor
        {
            private readonly Pass2Verifier _enclosing;

            private readonly DescendingVisitor carrier;
            private readonly Type CONST_Class;

            private readonly Type CONST_Double;

            private readonly Type CONST_Float;

            private readonly Type CONST_Integer;

            private readonly Type CONST_Long;

            private readonly Type CONST_NameAndType;

            private readonly Type CONST_String;

            private readonly Type CONST_Utf8;

            private readonly ConstantPool cp;

            private readonly int cplen;

            private readonly HashSet<string> field_names = new HashSet
                <string>();

            private readonly HashSet<string> field_names_and_desc =
                new HashSet<string>();

            private readonly JavaClass jc;

            private readonly HashSet<string> method_names_and_desc
                = new HashSet<string>();

            internal CPESSC_Visitor(Pass2Verifier _enclosing, JavaClass _jc)
            {
                this._enclosing = _enclosing;
                /*
                private Class<?> CONST_Fieldref;
                private Class<?> CONST_Methodref;
                private Class<?> CONST_InterfaceMethodref;
                */
                // ==jc.getConstantPool() -- only here to save typing work and computing power.
                // == cp.getLength() -- to save computing power.
                jc = _jc;
                cp = _jc.GetConstantPool();
                cplen = cp.GetLength();
                CONST_Class = typeof(ConstantClass);
                /*
                CONST_Fieldref = ConstantFieldref.class;
                CONST_Methodref = ConstantMethodref.class;
                CONST_InterfaceMethodref = ConstantInterfaceMethodref.class;
                */
                CONST_String = typeof(ConstantString);
                CONST_Integer = typeof(ConstantInteger);
                CONST_Float = typeof(ConstantFloat);
                CONST_Long = typeof(ConstantLong);
                CONST_Double = typeof(ConstantDouble);
                CONST_NameAndType = typeof(ConstantNameAndType);
                CONST_Utf8 = typeof(ConstantUtf8);
                carrier = new DescendingVisitor(_jc, this);
                carrier.Visit();
            }

            private void CheckIndex(Node referrer, int index, Type shouldbe
            )
            {
                if (index < 0 || index >= cplen)
                    throw new ClassConstraintException("Invalid index '" + index +
                                                       "' used by '" + Tostring(referrer) + "'.");
                var c = cp.GetConstant(index);
                if (!shouldbe.IsInstanceOfType(c))
                    /* String isnot = shouldbe.toString().substring(shouldbe.toString().lastIndexOf(".")+1); //Cut all before last "." */
                    throw new InvalidCastException("Illegal constant '" + Tostring(c) + "' at index '" + index +
                                                   "'. '" + Tostring(referrer) + "' expects a '" + shouldbe + "'.");
            }

            ///////////////////////////////////////
            // ClassFile structure (vmspec2 4.1) //
            ///////////////////////////////////////
            public override void VisitJavaClass(JavaClass obj)
            {
                var atts = obj.GetAttributes();
                var foundSourceFile = false;
                var foundInnerClasses = false;
                // Is there an InnerClass referenced?
                // This is a costly check; existing verifiers don't do it!
                var hasInnerClass = new InnerClassDetector(
                    jc).InnerClassReferenced();
                foreach (var att in atts)
                {
                    if (!(att is SourceFile) && !(att is Deprecated
                            ) && !(att is InnerClasses) && !(att is Synthetic
                            ))
                        _enclosing.AddMessage("Attribute '" + Tostring
                                                  (att) + "' as an attribute of the ClassFile structure '" +
                                              Tostring(obj) + "' is unknown and will therefore be ignored.");
                    if (att is SourceFile)
                    {
                        if (!foundSourceFile)
                            foundSourceFile = true;
                        else
                            throw new ClassConstraintException("A ClassFile structure (like '"
                                                               + Tostring(obj) +
                                                               "') may have no more than one SourceFile attribute."
                            );
                    }

                    //vmspec2 4.7.7
                    if (att is InnerClasses)
                    {
                        if (!foundInnerClasses)
                            foundInnerClasses = true;
                        else if (hasInnerClass)
                            throw new ClassConstraintException("A Classfile structure (like '"
                                                               + Tostring(obj) +
                                                               "') must have exactly one InnerClasses attribute"
                                                               + " if at least one Inner Class is referenced (which is the case)." +
                                                               " More than one InnerClasses attribute was found."
                            );
                        if (!hasInnerClass)
                            _enclosing.AddMessage("No referenced Inner Class found, but InnerClasses attribute '"
                                                  + Tostring(att) +
                                                  "' found. Strongly suggest removal of that attribute."
                            );
                    }
                }

                if (hasInnerClass && !foundInnerClasses)
                    //throw new ClassConstraintException("A Classfile structure (like '"+tostring(obj)+
                    // "') must have exactly one InnerClasses attribute if at least one Inner Class is referenced (which is the case)."+
                    // " No InnerClasses attribute was found.");
                    //vmspec2, page 125 says it would be a constraint: but existing verifiers
                    //don't check it and javac doesn't satisfy it when it comes to anonymous
                    //inner classes
                    _enclosing.AddMessage("A Classfile structure (like '" + Tostring(obj) +
                                          "') must have exactly one InnerClasses attribute if at least one Inner Class is referenced (which is the case)."
                                          + " No InnerClasses attribute was found.");
            }

            /////////////////////////////
            // CONSTANTS (vmspec2 4.4) //
            /////////////////////////////
            public override void VisitConstantClass(ConstantClass obj)
            {
                if (obj.GetTag() != Const.CONSTANT_Class)
                    throw new ClassConstraintException("Wrong constant tag in '" +
                                                       Tostring(obj) + "'.");
                CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
            }

            public override void VisitConstantFieldref(ConstantFieldref obj)
            {
                if (obj.GetTag() != Const.CONSTANT_Fieldref)
                    throw new ClassConstraintException("Wrong constant tag in '" +
                                                       Tostring(obj) + "'.");
                CheckIndex(obj, obj.GetClassIndex(), CONST_Class);
                CheckIndex(obj, obj.GetNameAndTypeIndex(), CONST_NameAndType);
            }

            public override void VisitConstantMethodref(ConstantMethodref obj
            )
            {
                if (obj.GetTag() != Const.CONSTANT_Methodref)
                    throw new ClassConstraintException("Wrong constant tag in '" +
                                                       Tostring(obj) + "'.");
                CheckIndex(obj, obj.GetClassIndex(), CONST_Class);
                CheckIndex(obj, obj.GetNameAndTypeIndex(), CONST_NameAndType);
            }

            public override void VisitConstantInterfaceMethodref(ConstantInterfaceMethodref
                obj)
            {
                if (obj.GetTag() != Const.CONSTANT_InterfaceMethodref)
                    throw new ClassConstraintException("Wrong constant tag in '" +
                                                       Tostring(obj) + "'.");
                CheckIndex(obj, obj.GetClassIndex(), CONST_Class);
                CheckIndex(obj, obj.GetNameAndTypeIndex(), CONST_NameAndType);
            }

            public override void VisitConstantString(ConstantString obj)
            {
                if (obj.GetTag() != Const.CONSTANT_String)
                    throw new ClassConstraintException("Wrong constant tag in '" +
                                                       Tostring(obj) + "'.");
                CheckIndex(obj, obj.GetStringIndex(), CONST_Utf8);
            }

            public override void VisitConstantInteger(ConstantInteger obj)
            {
                if (obj.GetTag() != Const.CONSTANT_Integer)
                    throw new ClassConstraintException("Wrong constant tag in '" +
                                                       Tostring(obj) + "'.");
            }

            // no indices to check
            public override void VisitConstantFloat(ConstantFloat obj)
            {
                if (obj.GetTag() != Const.CONSTANT_Float)
                    throw new ClassConstraintException("Wrong constant tag in '" +
                                                       Tostring(obj) + "'.");
            }

            //no indices to check
            public override void VisitConstantLong(ConstantLong obj)
            {
                if (obj.GetTag() != Const.CONSTANT_Long)
                    throw new ClassConstraintException("Wrong constant tag in '" +
                                                       Tostring(obj) + "'.");
            }

            //no indices to check
            public override void VisitConstantDouble(ConstantDouble obj)
            {
                if (obj.GetTag() != Const.CONSTANT_Double)
                    throw new ClassConstraintException("Wrong constant tag in '" +
                                                       Tostring(obj) + "'.");
            }

            //no indices to check
            public override void VisitConstantNameAndType(ConstantNameAndType
                obj)
            {
                if (obj.GetTag() != Const.CONSTANT_NameAndType)
                    throw new ClassConstraintException("Wrong constant tag in '" +
                                                       Tostring(obj) + "'.");
                CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
                //checkIndex(obj, obj.getDescriptorIndex(), CONST_Utf8); //inconsistently named in BCEL, see below.
                CheckIndex(obj, obj.GetSignatureIndex(), CONST_Utf8);
            }

            public override void VisitConstantUtf8(ConstantUtf8 obj)
            {
                if (obj.GetTag() != Const.CONSTANT_Utf8)
                    throw new ClassConstraintException("Wrong constant tag in '" +
                                                       Tostring(obj) + "'.");
            }

            //no indices to check
            //////////////////////////
            // FIELDS (vmspec2 4.5) //
            //////////////////////////
            public override void VisitField(Field obj)
            {
                if (jc.IsClass())
                {
                    var maxone = 0;
                    if (obj.IsPrivate()) maxone++;
                    if (obj.IsProtected()) maxone++;
                    if (obj.IsPublic()) maxone++;
                    if (maxone > 1)
                        throw new ClassConstraintException(
                            "Field '" + Tostring(obj) +
                            "' must only have at most one of its ACC_PRIVATE, ACC_PROTECTED, ACC_PUBLIC modifiers set."
                        );
                    if (obj.IsFinal() && obj.IsVolatile())
                        throw new ClassConstraintException(
                            "Field '" + Tostring(obj) +
                            "' must only have at most one of its ACC_FINAL, ACC_VOLATILE modifiers set."
                        );
                }
                else
                {
                    // isInterface!
                    if (!obj.IsPublic())
                        throw new ClassConstraintException("Interface field '" + Tostring(obj) +
                                                           "' must have the ACC_PUBLIC modifier set but hasn't!");
                    if (!obj.IsStatic())
                        throw new ClassConstraintException("Interface field '" + Tostring(obj) +
                                                           "' must have the ACC_STATIC modifier set but hasn't!");
                    if (!obj.IsFinal())
                        throw new ClassConstraintException("Interface field '" + Tostring(obj) +
                                                           "' must have the ACC_FINAL modifier set but hasn't!");
                }

                if ((obj.GetAccessFlags() & ~(Const.ACC_PUBLIC | Const.ACC_PRIVATE |
                                              Const.ACC_PROTECTED | Const.ACC_STATIC | Const.ACC_FINAL | Const
                                                  .ACC_VOLATILE | Const.ACC_TRANSIENT)) > 0)
                    _enclosing.AddMessage("Field '" + Tostring
                                              (obj) +
                                          "' has access flag(s) other than ACC_PUBLIC, ACC_PRIVATE, ACC_PROTECTED,"
                                          + " ACC_STATIC, ACC_FINAL, ACC_VOLATILE, ACC_TRANSIENT set (ignored).");
                CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
                var name = obj.GetName();
                if (!ValidFieldName(name))
                    throw new ClassConstraintException("Field '" + Tostring(obj) + "' has illegal name '" +
                                                       obj.GetName() + "'.");
                // A descriptor is often named signature in BCEL
                CheckIndex(obj, obj.GetSignatureIndex(), CONST_Utf8);
                var sig = ((ConstantUtf8) cp.GetConstant(obj.GetSignatureIndex
                    ())).GetBytes();
                // Field or Method sig.(=descriptor)
                try
                {
                    Generic.Type.GetType(sig);
                }
                catch (ClassFormatException cfe)
                {
                    /* Don't need the return value */
                    throw new ClassConstraintException("Illegal descriptor (==signature) '"
                                                       + sig + "' used by '" + Tostring(obj) + "'."
                        , cfe);
                }

                var nameanddesc = name + sig;
                if (field_names_and_desc.Contains(nameanddesc))
                    throw new ClassConstraintException("No two fields (like '" + Tostring(obj) +
                                                       "') are allowed have same names and descriptors!");
                if (field_names.Contains(name))
                    _enclosing.AddMessage("More than one field of name '" + name +
                                          "' detected (but with different type descriptors). This is very unusual."
                    );
                field_names_and_desc.Add(nameanddesc);
                field_names.Add(name);
                var atts = obj.GetAttributes();
                foreach (var att in atts)
                {
                    if (!(att is ConstantValue) && !(att is Synthetic
                            ) && !(att is Deprecated))
                        _enclosing.AddMessage("Attribute '" + Tostring
                                                  (att) + "' as an attribute of Field '" + Tostring
                                                  (obj) + "' is unknown and will therefore be ignored.");
                    if (!(att is ConstantValue))
                        _enclosing.AddMessage("Attribute '" + Tostring
                                                  (att) + "' as an attribute of Field '" + Tostring
                                                  (obj) +
                                              "' is not a ConstantValue and is therefore only of use for debuggers and such."
                        );
                }
            }

            ///////////////////////////
            // METHODS (vmspec2 4.6) //
            ///////////////////////////
            public override void VisitMethod(Method obj)
            {
                CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
                var name = obj.GetName();
                if (!ValidMethodName(name, true))
                    throw new ClassConstraintException(
                        "Method '" + Tostring(obj) + "' has illegal name '" + name + "'.");
                // A descriptor is often named signature in BCEL
                CheckIndex(obj, obj.GetSignatureIndex(), CONST_Utf8);
                var sig = ((ConstantUtf8) cp.GetConstant(obj.GetSignatureIndex
                    ())).GetBytes();
                // Method's signature(=descriptor)
                Generic.Type t;
                Generic.Type[] ts;
                // needed below the try block.
                try
                {
                    t = Generic.Type.GetReturnType(sig);
                    ts = Generic.Type.GetArgumentTypes(sig);
                }
                catch (ClassFormatException cfe)
                {
                    throw new ClassConstraintException("Illegal descriptor (==signature) '"
                                                       + sig + "' used by Method '" + Tostring(obj
                                                       ) + "'.", cfe);
                }

                // Check if referenced objects exist.
                var act = t;
                if (act is ArrayType) act = ((ArrayType) act).GetBasicType();
                if (act is ObjectType)
                {
                    var v = VerifierFactory.GetVerifier(((ObjectType
                        ) act).GetClassName());
                    var vr = v.DoPass1();
                    if (vr != VerificationResult.VR_OK)
                        throw new ClassConstraintException("Method '" + Tostring(obj) +
                                                           "' has a return type that does not pass verification pass 1: '"
                                                           + vr + "'.");
                }

                foreach (var element in ts)
                {
                    act = element;
                    if (act is ArrayType) act = ((ArrayType) act).GetBasicType();
                    if (act is ObjectType)
                    {
                        var v = VerifierFactory.GetVerifier(((ObjectType
                            ) act).GetClassName());
                        var vr = v.DoPass1();
                        if (vr != VerificationResult.VR_OK)
                            throw new ClassConstraintException("Method '" + Tostring(obj) +
                                                               "' has an argument type that does not pass verification pass 1: '"
                                                               + vr + "'.");
                    }
                }

                // Nearly forgot this! Funny return values are allowed, but a non-empty arguments list makes a different method out of it!
                if (name.Equals(Const.STATIC_INITIALIZER_NAME) && ts.Length != 0)
                    throw new ClassConstraintException("Method '" + Tostring(obj) + "' has illegal name '" + name +
                                                       "'." +
                                                       " Its name resembles the class or interface initialization method"
                                                       + " which it isn't because of its arguments (==descriptor).");
                if (jc.IsClass())
                {
                    var maxone = 0;
                    if (obj.IsPrivate()) maxone++;
                    if (obj.IsProtected()) maxone++;
                    if (obj.IsPublic()) maxone++;
                    if (maxone > 1)
                        throw new ClassConstraintException(
                            "Method '" + Tostring(obj) +
                            "' must only have at most one of its ACC_PRIVATE, ACC_PROTECTED, ACC_PUBLIC modifiers set."
                        );
                    if (obj.IsAbstract())
                    {
                        if (obj.IsFinal())
                            throw new ClassConstraintException(
                                "Abstract method '" + Tostring(obj) + "' must not have the ACC_FINAL modifier set.");
                        if (obj.IsNative())
                            throw new ClassConstraintException(
                                "Abstract method '" + Tostring(obj) + "' must not have the ACC_NATIVE modifier set.");
                        if (obj.IsPrivate())
                            throw new ClassConstraintException(
                                "Abstract method '" + Tostring(obj) + "' must not have the ACC_PRIVATE modifier set.");
                        if (obj.IsStatic())
                            throw new ClassConstraintException(
                                "Abstract method '" + Tostring(obj) + "' must not have the ACC_STATIC modifier set.");
                        if (obj.IsStrictfp())
                            throw new ClassConstraintException(
                                "Abstract method '" + Tostring(obj) + "' must not have the ACC_STRICT modifier set.");
                        if (obj.IsSynchronized())
                            throw new ClassConstraintException(
                                "Abstract method '" + Tostring(obj) +
                                "' must not have the ACC_SYNCHRONIZED modifier set.");
                    }

                    // A specific instance initialization method... (vmspec2,Page 116).
                    if (name.Equals(Const.CONSTRUCTOR_NAME))
                        //..may have at most one of ACC_PRIVATE, ACC_PROTECTED, ACC_PUBLIC set: is checked above.
                        //..may also have ACC_STRICT set, but none of the other flags in table 4.5 (vmspec2, page 115)
                        if (obj.IsStatic() || obj.IsFinal() || obj.IsSynchronized() || obj.IsNative() ||
                            obj.IsAbstract())
                            throw new ClassConstraintException("Instance initialization method '"
                                                               + Tostring(obj) + "' must not have" +
                                                               " any of the ACC_STATIC, ACC_FINAL, ACC_SYNCHRONIZED, ACC_NATIVE, ACC_ABSTRACT modifiers set."
                            );
                }
                else if (!name.Equals(Const.STATIC_INITIALIZER_NAME))
                {
                    // isInterface!
                    //vmspec2, p.116, 2nd paragraph
                    if (jc.GetMajor() >= Const.MAJOR_1_8)
                    {
                        if (!(obj.IsPublic() ^ obj.IsPrivate()))
                            throw new ClassConstraintException(
                                "Interface method '" + Tostring(obj) + "' must have" +
                                " exactly one of its ACC_PUBLIC and ACC_PRIVATE modifiers set."
                            );
                        if (obj.IsProtected() || obj.IsFinal() || obj.IsSynchronized() || obj.IsNative())
                            throw new ClassConstraintException(
                                "Interface method '" + Tostring(obj) + "' must not have" +
                                " any of the ACC_PROTECTED, ACC_FINAL, ACC_SYNCHRONIZED, or ACC_NATIVE modifiers set."
                            );
                    }
                    else
                    {
                        if (!obj.IsPublic())
                            throw new ClassConstraintException(
                                "Interface method '" + Tostring(obj) +
                                "' must have the ACC_PUBLIC modifier set but hasn't!");
                        if (!obj.IsAbstract())
                            throw new ClassConstraintException(
                                "Interface method '" + Tostring(obj) +
                                "' must have the ACC_ABSTRACT modifier set but hasn't!");
                        if (obj.IsPrivate() || obj.IsProtected() || obj.IsStatic() || obj.IsFinal() || obj
                                .IsSynchronized() || obj.IsNative() || obj.IsStrictfp())
                            throw new ClassConstraintException("Interface method '" + Tostring(obj) +
                                                               "' must not have" +
                                                               " any of the ACC_PRIVATE, ACC_PROTECTED, ACC_STATIC, ACC_FINAL, ACC_SYNCHRONIZED,"
                                                               + " ACC_NATIVE, ACC_ABSTRACT, ACC_STRICT modifiers set.");
                    }
                }

                if ((obj.GetAccessFlags() & ~(Const.ACC_PUBLIC | Const.ACC_PRIVATE |
                                              Const.ACC_PROTECTED | Const.ACC_STATIC | Const.ACC_FINAL | Const
                                                  .ACC_SYNCHRONIZED | Const.ACC_NATIVE | Const.ACC_ABSTRACT | Const
                                                  .ACC_STRICT)) > 0)
                    _enclosing.AddMessage("Method '" + Tostring
                                              (obj) + "' has access flag(s) other than" +
                                          " ACC_PUBLIC, ACC_PRIVATE, ACC_PROTECTED, ACC_STATIC, ACC_FINAL,"
                                          + " ACC_SYNCHRONIZED, ACC_NATIVE, ACC_ABSTRACT, ACC_STRICT set (ignored).");
                var nameanddesc = name + sig;
                if (method_names_and_desc.Contains(nameanddesc))
                    throw new ClassConstraintException("No two methods (like '" +
                                                       Tostring(obj) + "') are allowed have same names and desciptors!"
                    );
                method_names_and_desc.Add(nameanddesc);
                var atts = obj.GetAttributes();
                var num_code_atts = 0;
                foreach (var att in atts)
                {
                    if (!(att is Code) && !(att is ExceptionTable) && !(att is Synthetic) && !(att is Deprecated
                            ))
                        _enclosing.AddMessage("Attribute '" + Tostring
                                                  (att) + "' as an attribute of Method '" + Tostring
                                                  (obj) + "' is unknown and will therefore be ignored.");
                    if (!(att is Code) && !(att is ExceptionTable))
                        _enclosing.AddMessage("Attribute '" + Tostring
                                                  (att) + "' as an attribute of Method '" + Tostring
                                                  (obj) +
                                              "' is neither Code nor Exceptions and is therefore only of use for debuggers and such."
                        );
                    if (att is Code && (obj.IsNative() || obj.IsAbstract()))
                        throw new ClassConstraintException("Native or abstract methods like '"
                                                           + Tostring(obj) + "' must not have a Code attribute like '"
                                                           + Tostring(att) + "'.");
                    //vmspec2 page120, 4.7.3
                    if (att is Code) num_code_atts++;
                }

                if (!obj.IsNative() && !obj.IsAbstract() && num_code_atts != 1)
                    throw new ClassConstraintException("Non-native, non-abstract methods like '"
                                                       + Tostring(obj) +
                                                       "' must have exactly one Code attribute (found: "
                                                       + num_code_atts + ").");
            }

            ///////////////////////////////////////////////////////
            // ClassFile-structure-ATTRIBUTES (vmspec2 4.1, 4.7) //
            ///////////////////////////////////////////////////////
            public override void VisitSourceFile(SourceFile obj)
            {
                //vmspec2 4.7.7
                // zero or one SourceFile attr per ClassFile: see visitJavaClass()
                CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
                var name = ((ConstantUtf8) cp.GetConstant(obj.GetNameIndex
                    ())).GetBytes();
                if (!name.Equals("SourceFile"))
                    throw new ClassConstraintException("The SourceFile attribute '"
                                                       + Tostring(obj) + "' is not correctly named 'SourceFile' but '"
                                                       + name + "'.");
                CheckIndex(obj, obj.GetSourceFileIndex(), CONST_Utf8);
                var sourceFileName = ((ConstantUtf8) cp.GetConstant(obj.GetSourceFileIndex
                    ())).GetBytes();
                //==obj.getSourceFileName() ?
                var sourceFileNameLc = sourceFileName.ToLower();
                if (sourceFileName.IndexOf('/') != -1 || sourceFileName.IndexOf('\\') != -1 ||
                    sourceFileName.IndexOf(':') != -1 || sourceFileNameLc.LastIndexOf(".java") ==
                    -1)
                    _enclosing.AddMessage("SourceFile attribute '" + Tostring(obj) +
                                          "' has a funny name: remember not to confuse certain parsers working on javap's output. Also, this name ('"
                                          + sourceFileName +
                                          "') is considered an unqualified (simple) file name only.");
            }

            public override void VisitDeprecated(Deprecated obj)
            {
                //vmspec2 4.7.10
                CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
                var name = ((ConstantUtf8) cp.GetConstant(obj.GetNameIndex
                    ())).GetBytes();
                if (!name.Equals("Deprecated"))
                    throw new ClassConstraintException("The Deprecated attribute '"
                                                       + Tostring(obj) + "' is not correctly named 'Deprecated' but '"
                                                       + name + "'.");
            }

            public override void VisitSynthetic(Synthetic obj)
            {
                //vmspec2 4.7.6
                CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
                var name = ((ConstantUtf8) cp.GetConstant(obj.GetNameIndex
                    ())).GetBytes();
                if (!name.Equals("Synthetic"))
                    throw new ClassConstraintException("The Synthetic attribute '"
                                                       + Tostring(obj) + "' is not correctly named 'Synthetic' but '"
                                                       + name + "'.");
            }

            public override void VisitInnerClasses(InnerClasses obj)
            {
                //vmspec2 4.7.5
                // exactly one InnerClasses attr per ClassFile if some inner class is refernced: see visitJavaClass()
                CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
                var name = ((ConstantUtf8) cp.GetConstant(obj.GetNameIndex
                    ())).GetBytes();
                if (!name.Equals("InnerClasses"))
                    throw new ClassConstraintException("The InnerClasses attribute '"
                                                       + Tostring(obj) + "' is not correctly named 'InnerClasses' but '"
                                                       + name + "'.");
                var ics = obj.GetInnerClasses();
                foreach (var ic in ics)
                {
                    CheckIndex(obj, ic.GetInnerClassIndex(), CONST_Class);
                    var outer_idx = ic.GetOuterClassIndex();
                    if (outer_idx != 0) CheckIndex(obj, outer_idx, CONST_Class);
                    var innername_idx = ic.GetInnerNameIndex();
                    if (innername_idx != 0) CheckIndex(obj, innername_idx, CONST_Utf8);
                    var acc = ic.GetInnerAccessFlags();
                    acc = acc & ~(Const.ACC_PUBLIC | Const.ACC_PRIVATE | Const.ACC_PROTECTED
                                  | Const.ACC_STATIC | Const.ACC_FINAL | Const.ACC_INTERFACE |
                                  Const.ACC_ABSTRACT);
                    if (acc != 0)
                        _enclosing.AddMessage("Unknown access flag for inner class '" + Tostring(ic) +
                                              "' set (InnerClasses attribute '" + Tostring(obj) + "').");
                }
            }

            // Semantical consistency is not yet checked by Sun, see vmspec2 4.7.5.
            // [marked TODO in JustIce]
            ////////////////////////////////////////////////////////
            // field_info-structure-ATTRIBUTES (vmspec2 4.5, 4.7) //
            ////////////////////////////////////////////////////////
            public override void VisitConstantValue(ConstantValue obj)
            {
                //vmspec2 4.7.2
                // Despite its name, this really is an Attribute,
                // not a constant!
                CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
                var name = ((ConstantUtf8) cp.GetConstant(obj.GetNameIndex
                    ())).GetBytes();
                if (!name.Equals("ConstantValue"))
                    throw new ClassConstraintException("The ConstantValue attribute '"
                                                       + Tostring(obj) +
                                                       "' is not correctly named 'ConstantValue' but '"
                                                       + name + "'.");
                var pred = carrier.Predecessor();
                if (pred is Field)
                {
                    //ConstantValue attributes are quite senseless if the predecessor is not a field.
                    var f = (Field) pred;
                    // Field constraints have been checked before -- so we are safe using their type information.
                    var field_type = Generic.Type.GetType(((ConstantUtf8
                        ) cp.GetConstant(f.GetSignatureIndex())).GetBytes());
                    var index = obj.GetConstantValueIndex();
                    if (index < 0 || index >= cplen)
                        throw new ClassConstraintException("Invalid index '" + index +
                                                           "' used by '" + Tostring(obj) + "'.");
                    var c = cp.GetConstant(index);
                    if (CONST_Long.IsInstanceOfType(c) && field_type.Equals(Generic.Type.LONG
                        ))
                        return;
                    if (CONST_Float.IsInstanceOfType(c) && field_type.Equals(Generic.Type.FLOAT))
                        return;
                    if (CONST_Double.IsInstanceOfType(c) && field_type.Equals(Generic.Type
                            .DOUBLE))
                        return;
                    if (CONST_Integer.IsInstanceOfType(c) && (field_type.Equals(Generic.Type
                                                                  .INT) || field_type.Equals(Generic.Type.SHORT) ||
                                                              field_type.Equals(Generic.Type
                                                                  .CHAR) || field_type.Equals(Generic.Type.BYTE) ||
                                                              field_type.Equals(Generic.Type
                                                                  .BOOLEAN)))
                        return;
                    if (CONST_String.IsInstanceOfType(c) && field_type.Equals(Generic.Type
                            .STRING))
                        return;
                    throw new ClassConstraintException("Illegal type of ConstantValue '"
                                                       + obj + "' embedding Constant '" + c +
                                                       "'. It is referenced by field '" + Tostring(f) +
                                                       "' expecting a different type: '" + field_type + "'.");
                }
            }

            // SYNTHETIC: see above
            // DEPRECATED: see above
            /////////////////////////////////////////////////////////
            // method_info-structure-ATTRIBUTES (vmspec2 4.6, 4.7) //
            /////////////////////////////////////////////////////////
            public override void VisitCode(Code obj)
            {
                //vmspec2 4.7.3
                try
                {
                    // No code attribute allowed for native or abstract methods: see visitMethod(Method).
                    // Code array constraints are checked in Pass3 (3a and 3b).
                    CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
                    var name = ((ConstantUtf8) cp.GetConstant(obj.GetNameIndex
                        ())).GetBytes();
                    if (!name.Equals("Code"))
                        throw new ClassConstraintException("The Code attribute '" + Tostring(obj) +
                                                           "' is not correctly named 'Code' but '" + name + "'.");
                    Method m = null;
                    // satisfy compiler
                    if (!(carrier.Predecessor() is Method))
                    {
                        _enclosing.AddMessage("Code attribute '" + Tostring(obj) +
                                              "' is not declared in a method_info structure but in '" +
                                              carrier.Predecessor() + "'. Ignored.");
                        return;
                    }

                    m = (Method) carrier.Predecessor();
                    // we can assume this method was visited before;
                    // i.e. the data consistency was verified.
                    if (obj.GetCode().Length == 0)
                        throw new ClassConstraintException("Code array of Code attribute '"
                                                           + Tostring(obj) + "' (method '" + m + "') must not be empty."
                        );
                    //In JustIce, the check for correct offsets into the code array is delayed to Pass 3a.
                    var exc_table = obj.GetExceptionTable();
                    foreach (var element in exc_table)
                    {
                        var exc_index = element.GetCatchType();
                        if (exc_index != 0)
                        {
                            // if 0, it catches all Throwables
                            CheckIndex(obj, exc_index, CONST_Class);
                            var cc = (ConstantClass) cp.GetConstant
                                (exc_index);
                            // cannot be sure this ConstantClass has already been visited (checked)!
                            CheckIndex(cc, cc.GetNameIndex(), CONST_Utf8);
                            var cname = ((ConstantUtf8) cp.GetConstant(cc.GetNameIndex
                                ())).GetBytes().Replace('/', '.');
                            var v = VerifierFactory.GetVerifier(cname);
                            var vr = v.DoPass1();
                            if (vr != VerificationResult.VR_OK)
                                throw new ClassConstraintException("Code attribute '" + Tostring(obj) + "' (method '" +
                                                                   m + "') has an exception_table entry '" +
                                                                   Tostring(element) + "' that references '" + cname +
                                                                   "' as an Exception but it does not pass verification pass 1: "
                                                                   + vr);
                            // We cannot safely trust any other "instanceof" mechanism. We need to transitively verify
                            // the ancestor hierarchy.
                            var e = Repository.LookupClass(cname);
                            var t = Repository.LookupClass(Generic.Type.THROWABLE
                                .GetClassName());
                            var o = Repository.LookupClass(Generic.Type.OBJECT
                                .GetClassName());
                            while (e != o)
                            {
                                if (e == t) break;
                                // It's a subclass of Throwable, OKAY, leave.
                                v = VerifierFactory.GetVerifier(e.GetSuperclassName());
                                vr = v.DoPass1();
                                if (vr != VerificationResult.VR_OK)
                                    throw new ClassConstraintException("Code attribute '" + Tostring(obj) +
                                                                       "' (method '" + m +
                                                                       "') has an exception_table entry '" +
                                                                       Tostring(element) + "' that references '" +
                                                                       cname + "' as an Exception but '" +
                                                                       e.GetSuperclassName() +
                                                                       "' in the ancestor hierachy does not pass verification pass 1: "
                                                                       + vr);
                                e = Repository.LookupClass(e.GetSuperclassName());
                            }

                            if (e != t)
                                throw new ClassConstraintException("Code attribute '" + Tostring(obj) + "' (method '" +
                                                                   m + "') has an exception_table entry '" +
                                                                   Tostring(element) + "' that references '" + cname +
                                                                   "' as an Exception but it is not a subclass of '"
                                                                   + t.GetClassName() + "'.");
                        }
                    }

                    // Create object for local variables information
                    // This is highly unelegant due to usage of the Visitor pattern.
                    // TODO: rework it.
                    var method_number = -1;
                    var ms = Repository.LookupClass(_enclosing.myOwner
                        .GetClassName()).GetMethods();
                    for (var mn = 0; mn < ms.Length; mn++)
                        if (m == ms[mn])
                        {
                            method_number = mn;
                            break;
                        }

                    if (method_number < 0)
                        // Mmmmh. Can we be sure BCEL does not sometimes instantiate new objects?
                        throw new AssertionViolatedException(
                            "Could not find a known BCEL Method object in the corresponding BCEL JavaClass object."
                        );
                    _enclosing.localVariablesInfos[method_number] = new LocalVariablesInfo
                        (obj.GetMaxLocals());
                    var num_of_lvt_attribs = 0;
                    // Now iterate through the attributes the Code attribute has.
                    var atts = obj.GetAttributes();
                    for (var a = 0; a < atts.Length; a++)
                    {
                        if (!(atts[a] is LineNumberTable) && !(atts[a] is LocalVariableTable
                                ))
                            _enclosing.AddMessage("Attribute '" + Tostring
                                                      (atts[a]) + "' as an attribute of Code attribute '" +
                                                  Tostring(obj) + "' (method '" + m +
                                                  "') is unknown and will therefore be ignored."
                            );
                        else
                            // LineNumberTable or LocalVariableTable
                            _enclosing.AddMessage("Attribute '" + Tostring
                                                      (atts[a]) + "' as an attribute of Code attribute '" +
                                                  Tostring(obj) + "' (method '" + m +
                                                  "') will effectively be ignored and is only useful for debuggers and such."
                            );
                        //LocalVariableTable check (partially delayed to Pass3a).
                        //Here because its easier to collect the information of the
                        //(possibly more than one) LocalVariableTables belonging to
                        //one certain Code attribute.
                        if (atts[a] is LocalVariableTable)
                        {
                            // checks conforming to vmspec2 4.7.9
                            var lvt = (LocalVariableTable) atts
                                [a];
                            CheckIndex(lvt, lvt.GetNameIndex(), CONST_Utf8);
                            var lvtname = ((ConstantUtf8) cp.GetConstant(lvt.GetNameIndex
                                ())).GetBytes();
                            if (!lvtname.Equals("LocalVariableTable"))
                                throw new ClassConstraintException("The LocalVariableTable attribute '"
                                                                   + Tostring(lvt) +
                                                                   "' is not correctly named 'LocalVariableTable' but '"
                                                                   + lvtname + "'.");
                            var code = obj;
                            //In JustIce, the check for correct offsets into the code array is delayed to Pass 3a.
                            var localvariables = lvt.GetLocalVariableTable();
                            foreach (var localvariable in localvariables)
                            {
                                CheckIndex(lvt, localvariable.GetNameIndex(), CONST_Utf8);
                                var localname = ((ConstantUtf8) cp.GetConstant(localvariable
                                    .GetNameIndex())).GetBytes();
                                if (!ValidJavaIdentifier(localname))
                                    throw new ClassConstraintException(
                                        "LocalVariableTable '" + Tostring(lvt) +
                                        "' references a local variable by the name '" + localname +
                                        "' which is not a legal Java simple name."
                                    );
                                CheckIndex(lvt, localvariable.GetSignatureIndex(), CONST_Utf8);
                                var localsig = ((ConstantUtf8) cp.GetConstant(localvariable
                                    .GetSignatureIndex())).GetBytes();
                                // Local sig.(=descriptor)
                                Generic.Type t;
                                try
                                {
                                    t = Generic.Type.GetType(localsig);
                                }
                                catch (ClassFormatException cfe)
                                {
                                    throw new ClassConstraintException("Illegal descriptor (==signature) '"
                                                                       + localsig + "' used by LocalVariable '" +
                                                                       Tostring(localvariable) + "' referenced by '" +
                                                                       Tostring(lvt) + "'.", cfe);
                                }

                                var localindex = localvariable.GetIndex();
                                if ((t == Generic.Type.LONG || t == Generic.Type.DOUBLE
                                        ? localindex
                                          + 1
                                        : localindex) >= code.GetMaxLocals())
                                    throw new ClassConstraintException("LocalVariableTable attribute '"
                                                                       + Tostring(lvt) +
                                                                       "' references a LocalVariable '"
                                                                       + Tostring(localvariable) +
                                                                       "' with an index that exceeds the surrounding Code attribute's max_locals value of '"
                                                                       + code.GetMaxLocals() + "'.");
                                try
                                {
                                    _enclosing.localVariablesInfos[method_number].Add(localindex, localname,
                                        localvariable
                                            .GetStartPC(), localvariable.GetLength(), t);
                                }
                                catch (LocalVariableInfoInconsistentException lviie)
                                {
                                    throw new ClassConstraintException("Conflicting information in LocalVariableTable '"
                                                                       + Tostring(lvt) + "' found in Code attribute '"
                                                                       + Tostring(obj) + "' (method '" + Tostring(m) +
                                                                       "'). " + lviie.Message, lviie);
                                }
                            }

                            // for all local variables localvariables[i] in the LocalVariableTable attribute atts[a] END
                            num_of_lvt_attribs++;
                            if (!m.IsStatic() && num_of_lvt_attribs > obj.GetMaxLocals())
                                throw new ClassConstraintException(
                                    "Number of LocalVariableTable attributes of Code attribute '"
                                    + Tostring(obj) + "' (method '" + Tostring(m) +
                                    "') exceeds number of local variable slots '" + obj.GetMaxLocals(
                                    ) +
                                    "' ('There may be at most one LocalVariableTable attribute per local variable in the Code attribute.')."
                                );
                        }
                    }
                }
                catch (TypeLoadException e)
                {
                    // if atts[a] instanceof LocalVariableTable END
                    // for all attributes atts[a] END
                    // FIXME: this might not be the best way to handle missing classes.
                    throw new AssertionViolatedException("Missing class: " + e, e);
                }
            }

            // visitCode(Code) END
            public override void VisitExceptionTable(ExceptionTable obj)
            {
                //vmspec2 4.7.4
                try
                {
                    // incorrectly named, it's the Exceptions attribute (vmspec2 4.7.4)
                    CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
                    var name = ((ConstantUtf8) cp.GetConstant(obj.GetNameIndex
                        ())).GetBytes();
                    if (!name.Equals("Exceptions"))
                        throw new ClassConstraintException("The Exceptions attribute '"
                                                           + Tostring(obj) +
                                                           "' is not correctly named 'Exceptions' but '"
                                                           + name + "'.");
                    var exc_indices = obj.GetExceptionIndexTable();
                    foreach (var exc_indice in exc_indices)
                    {
                        CheckIndex(obj, exc_indice, CONST_Class);
                        var cc = (ConstantClass) cp.GetConstant
                            (exc_indice);
                        CheckIndex(cc, cc.GetNameIndex(), CONST_Utf8);
                        // can't be sure this ConstantClass has already been visited (checked)!
                        //convert internal notation on-the-fly to external notation:
                        var cname = ((ConstantUtf8) cp.GetConstant(cc.GetNameIndex
                            ())).GetBytes().Replace('/', '.');
                        var v = VerifierFactory.GetVerifier(cname);
                        var vr = v.DoPass1();
                        if (vr != VerificationResult.VR_OK)
                            throw new ClassConstraintException("Exceptions attribute '" +
                                                               Tostring(obj) + "' references '" + cname +
                                                               "' as an Exception but it does not pass verification pass 1: " +
                                                               vr);
                        // We cannot safely trust any other "instanceof" mechanism. We need to transitively verify
                        // the ancestor hierarchy.
                        var e = Repository.LookupClass(cname);
                        var t = Repository.LookupClass(Generic.Type.THROWABLE
                            .GetClassName());
                        var o = Repository.LookupClass(Generic.Type.OBJECT
                            .GetClassName());
                        while (e != o)
                        {
                            if (e == t) break;
                            // It's a subclass of Throwable, OKAY, leave.
                            v = VerifierFactory.GetVerifier(e.GetSuperclassName());
                            vr = v.DoPass1();
                            if (vr != VerificationResult.VR_OK)
                                throw new ClassConstraintException("Exceptions attribute '" +
                                                                   Tostring(obj) + "' references '" + cname +
                                                                   "' as an Exception but '" + e.GetSuperclassName() +
                                                                   "' in the ancestor hierachy does not pass verification pass 1: "
                                                                   + vr);
                            e = Repository.LookupClass(e.GetSuperclassName());
                        }

                        if (e != t)
                            throw new ClassConstraintException("Exceptions attribute '" +
                                                               Tostring(obj) + "' references '" + cname +
                                                               "' as an Exception but it is not a subclass of '" +
                                                               t.GetClassName() + "'.");
                    }
                }
                catch (TypeLoadException e)
                {
                    // FIXME: this might not be the best way to handle missing classes.
                    throw new AssertionViolatedException("Missing class: " + e, e);
                }
            }

            // SYNTHETIC: see above
            // DEPRECATED: see above
            //////////////////////////////////////////////////////////////
            // code_attribute-structure-ATTRIBUTES (vmspec2 4.7.3, 4.7) //
            //////////////////////////////////////////////////////////////
            public override void VisitLineNumberTable(LineNumberTable obj)
            {
                //vmspec2 4.7.8
                CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
                var name = ((ConstantUtf8) cp.GetConstant(obj.GetNameIndex
                    ())).GetBytes();
                if (!name.Equals("LineNumberTable"))
                    throw new ClassConstraintException("The LineNumberTable attribute '"
                                                       + Tostring(obj) +
                                                       "' is not correctly named 'LineNumberTable' but '"
                                                       + name + "'.");
            }

            //In JustIce,this check is delayed to Pass 3a.
            //LineNumber[] linenumbers = obj.getLineNumberTable();
            // ...validity check...
            public override void VisitLocalVariableTable(LocalVariableTable obj
            )
            {
            }

            //vmspec2 4.7.9
            //In JustIce,this check is partially delayed to Pass 3a.
            //The other part can be found in the visitCode(Code) method.
            ////////////////////////////////////////////////////
            // MISC-structure-ATTRIBUTES (vmspec2 4.7.1, 4.7) //
            ////////////////////////////////////////////////////
            public override void VisitUnknown(Unknown obj)
            {
                //vmspec2 4.7.1
                // Represents an unknown attribute.
                CheckIndex(obj, obj.GetNameIndex(), CONST_Utf8);
                // Maybe only misnamed? Give a (warning) message.
                _enclosing.AddMessage("Unknown attribute '" + Tostring(obj) +
                                      "'. This attribute is not known in any context!");
            }

            //////////
            // BCEL //
            //////////
            public override void VisitLocalVariable(LocalVariable obj)
            {
            }

            // This does not represent an Attribute but is only
            // related to internal BCEL data representation.
            // see visitLocalVariableTable(LocalVariableTable)
            public override void VisitCodeException(CodeException obj)
            {
            }

            // Code constraints are checked in Pass3 (3a and 3b).
            // This does not represent an Attribute but is only
            // related to internal BCEL data representation.
            // see visitCode(Code)
            public override void VisitConstantPool(ConstantPool obj)
            {
            }

            // No need to. We're piggybacked by the DescendingVisitor.
            // This does not represent an Attribute but is only
            // related to internal BCEL data representation.
            public override void VisitInnerClass(InnerClass obj)
            {
            }

            // This does not represent an Attribute but is only
            // related to internal BCEL data representation.
            public override void VisitLineNumber(LineNumber obj)
            {
            }

            // This does not represent an Attribute but is only
            // related to internal BCEL data representation.
            // see visitLineNumberTable(LineNumberTable)
        }

        /// <summary>
        ///     A Visitor class that ensures the ConstantCP-subclassed entries
        ///     of the constant pool are valid.
        /// </summary>
        /// <remarks>
        ///     A Visitor class that ensures the ConstantCP-subclassed entries
        ///     of the constant pool are valid.
        ///     <B>
        ///         Precondition: index-style cross referencing in the constant
        ///         pool must be valid.
        ///     </B>
        /// </remarks>
        /// <seealso cref="Pass2Verifier.Constant_pool_entries_satisfy_static_constraints()"/>
        ///     <seealso cref="ConstantCP" />
        private sealed class FAMRAV_Visitor : ClassFile.EmptyVisitor
        {
            private readonly Pass2Verifier _enclosing;
            private readonly ConstantPool cp;

            internal FAMRAV_Visitor(Pass2Verifier _enclosing, JavaClass _jc)
            {
                this._enclosing = _enclosing;
                // ==jc.getConstantPool() -- only here to save typing work.
                cp = _jc.GetConstantPool();
            }

            public override void VisitConstantFieldref(ConstantFieldref obj)
            {
                if (obj.GetTag() != Const.CONSTANT_Fieldref)
                    throw new ClassConstraintException("ConstantFieldref '" + Tostring(obj) + "' has wrong tag!");
                var name_and_type_index = obj.GetNameAndTypeIndex();
                var cnat = (ConstantNameAndType) cp.GetConstant(name_and_type_index);
                var name = ((ConstantUtf8) cp.GetConstant(cnat.GetNameIndex
                    ())).GetBytes();
                // Field or Method name
                if (!ValidFieldName(name))
                    throw new ClassConstraintException("Invalid field name '" + name
                                                                              + "' referenced by '" + Tostring(obj) +
                                                                              "'."
                    );
                var class_index = obj.GetClassIndex();
                var cc = (ConstantClass) cp.GetConstant
                    (class_index);
                var className = ((ConstantUtf8) cp.GetConstant(cc.GetNameIndex
                    ())).GetBytes();
                // Class Name in internal form
                if (!ValidClassName(className))
                    throw new ClassConstraintException("Illegal class name '" + className
                                                                              + "' used by '" + Tostring(obj) + "'.");
                var sig = ((ConstantUtf8) cp.GetConstant(cnat.GetSignatureIndex
                    ())).GetBytes();
                // Field or Method sig.(=descriptor)
                try
                {
                    Generic.Type.GetType(sig);
                }
                catch (ClassFormatException cfe)
                {
                    /* Don't need the return value */
                    throw new ClassConstraintException("Illegal descriptor (==signature) '"
                                                       + sig + "' used by '" + Tostring(obj) + "'."
                        , cfe);
                }
            }

            public override void VisitConstantMethodref(ConstantMethodref obj
            )
            {
                if (obj.GetTag() != Const.CONSTANT_Methodref)
                    throw new ClassConstraintException("ConstantMethodref '" + Tostring(obj) + "' has wrong tag!");
                var name_and_type_index = obj.GetNameAndTypeIndex();
                var cnat = (ConstantNameAndType) cp.GetConstant(name_and_type_index);
                var name = ((ConstantUtf8) cp.GetConstant(cnat.GetNameIndex
                    ())).GetBytes();
                // Field or Method name
                if (!ValidClassMethodName(name))
                    throw new ClassConstraintException("Invalid (non-interface) method name '"
                                                       + name + "' referenced by '" + Tostring(obj
                                                       ) + "'.");
                var class_index = obj.GetClassIndex();
                var cc = (ConstantClass) cp.GetConstant
                    (class_index);
                var className = ((ConstantUtf8) cp.GetConstant(cc.GetNameIndex
                    ())).GetBytes();
                // Class Name in internal form
                if (!ValidClassName(className))
                    throw new ClassConstraintException("Illegal class name '" + className
                                                                              + "' used by '" + Tostring(obj) + "'.");
                var sig = ((ConstantUtf8) cp.GetConstant(cnat.GetSignatureIndex
                    ())).GetBytes();
                // Field or Method sig.(=descriptor)
                try
                {
                    var t = Generic.Type.GetReturnType(sig);
                    if (name.Equals(Const.CONSTRUCTOR_NAME) && t != Generic.Type.VOID)
                        throw new ClassConstraintException("Instance initialization method must have VOID return type."
                        );
                }
                catch (ClassFormatException cfe)
                {
                    throw new ClassConstraintException("Illegal descriptor (==signature) '"
                                                       + sig + "' used by '" + Tostring(obj) + "'."
                        , cfe);
                }
            }

            public override void VisitConstantInterfaceMethodref(ConstantInterfaceMethodref
                obj)
            {
                if (obj.GetTag() != Const.CONSTANT_InterfaceMethodref)
                    throw new ClassConstraintException("ConstantInterfaceMethodref '"
                                                       + Tostring(obj) + "' has wrong tag!");
                var name_and_type_index = obj.GetNameAndTypeIndex();
                var cnat = (ConstantNameAndType) cp.GetConstant(name_and_type_index);
                var name = ((ConstantUtf8) cp.GetConstant(cnat.GetNameIndex
                    ())).GetBytes();
                // Field or Method name
                if (!ValidInterfaceMethodName(name))
                    throw new ClassConstraintException("Invalid (interface) method name '"
                                                       + name + "' referenced by '" + Tostring(obj
                                                       ) + "'.");
                var class_index = obj.GetClassIndex();
                var cc = (ConstantClass) cp.GetConstant
                    (class_index);
                var className = ((ConstantUtf8) cp.GetConstant(cc.GetNameIndex
                    ())).GetBytes();
                // Class Name in internal form
                if (!ValidClassName(className))
                    throw new ClassConstraintException("Illegal class name '" + className
                                                                              + "' used by '" + Tostring(obj) + "'.");
                var sig = ((ConstantUtf8) cp.GetConstant(cnat.GetSignatureIndex
                    ())).GetBytes();
                // Field or Method sig.(=descriptor)
                try
                {
                    var t = Generic.Type.GetReturnType(sig);
                    if (name.Equals(Const.STATIC_INITIALIZER_NAME) && t != Generic.Type.VOID)
                        _enclosing.AddMessage("Class or interface initialization method '" + Const
                                                  .STATIC_INITIALIZER_NAME +
                                              "' usually has VOID return type instead of '" + t +
                                              "'. Note this is really not a requirement of The Java Virtual Machine Specification, Second Edition."
                        );
                }
                catch (ClassFormatException cfe)
                {
                    throw new ClassConstraintException("Illegal descriptor (==signature) '"
                                                       + sig + "' used by '" + Tostring(obj) + "'."
                        , cfe);
                }
            }
        }

        /// <summary>
        ///     This class serves for finding out if a given JavaClass' ConstantPool
        ///     references an Inner Class.
        /// </summary>
        /// <remarks>
        ///     This class serves for finding out if a given JavaClass' ConstantPool
        ///     references an Inner Class.
        ///     The Java Virtual Machine Specification, Second Edition is not very precise
        ///     about when an "InnerClasses" attribute has to appear. However, it states that
        ///     there has to be exactly one InnerClasses attribute in the ClassFile structure
        ///     if the constant pool of a class or interface refers to any class or interface
        ///     "that is not a member of a package". Sun does not mean "member of the default
        ///     package". In "Inner Classes Specification" they point out how a "bytecode name"
        ///     is derived so one has to deduce what a class name of a class "that is not a
        ///     member of a package" looks like: there is at least one character in the byte-
        ///     code name that cannot be part of a legal Java Language Class name (and not equal
        ///     to '/'). This assumption is wrong as the delimiter is '$' for which
        ///     Character.isJavaIdentifierPart() == true.
        ///     Hence, you really run into trouble if you have a toplevel class called
        ///     "A$XXX" and another toplevel class called "A" with in inner class called "XXX".
        ///     JustIce cannot repair this; please note that existing verifiers at this
        ///     time even fail to detect missing InnerClasses attributes in pass 2.
        /// </remarks>
        private class InnerClassDetector : ClassFile.EmptyVisitor
        {
            private readonly ConstantPool cp;

            private readonly JavaClass jc;
            private bool hasInnerClass;

            /// <summary>Constructs an InnerClassDetector working on the JavaClass _jc.</summary>
            public InnerClassDetector(JavaClass _jc)
            {
                jc = _jc;
                cp = jc.GetConstantPool();
                new DescendingVisitor(jc, this).Visit();
            }

            /// <summary>
            ///     Returns if the JavaClass this InnerClassDetector is working on
            ///     has an Inner Class reference in its constant pool.
            /// </summary>
            /// <returns>
            ///     Whether this InnerClassDetector is working on has an Inner Class reference in its constant pool.
            /// </returns>
            public virtual bool InnerClassReferenced()
            {
                return hasInnerClass;
            }

            /// <summary>This method casually visits ConstantClass references.</summary>
            public override void VisitConstantClass(ConstantClass obj)
            {
                var c = cp.GetConstant(obj.GetNameIndex());
                if (c is ConstantUtf8)
                {
                    //Ignore the case where it's not a ConstantUtf8 here, we'll find out later.
                    var classname = ((ConstantUtf8) c).GetBytes();
                    if (classname.StartsWith(jc.GetClassName().Replace('.', '/') + "$")) hasInnerClass = true;
                }
            }
        }
    }
}