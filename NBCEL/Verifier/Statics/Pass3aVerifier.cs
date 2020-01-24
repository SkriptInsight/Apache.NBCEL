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
using Apache.NBCEL.Generic;
using Apache.NBCEL.Verifier.Exc;
using Type = Apache.NBCEL.Generic.Type;

namespace Apache.NBCEL.Verifier.Statics
{
	/// <summary>
	///     This PassVerifier verifies a class file according to
	///     pass 3, static part as described in The Java Virtual
	///     Machine Specification, 2nd edition.
	/// </summary>
	/// <remarks>
	///     This PassVerifier verifies a class file according to
	///     pass 3, static part as described in The Java Virtual
	///     Machine Specification, 2nd edition.
	///     More detailed information is to be found at the do_verify()
	///     method's documentation.
	/// </remarks>
	/// <seealso cref="Do_verify()" />
	public sealed class Pass3aVerifier : PassVerifier
    {
	    /// <summary>The method number to verify.</summary>
	    /// <remarks>
	    ///     The method number to verify.
	    ///     This is the index in the array returned
	    ///     by JavaClass.getMethods().
	    /// </remarks>
	    private readonly int method_no;

        /// <summary>The Verifier that created this.</summary>
        private readonly Verifier myOwner;

        /// <summary>The one and only Code object used by an instance of this class.</summary>
        /// <remarks>
        ///     The one and only Code object used by an instance of this class.
        ///     It's here for performance reasons by do_verify() and its callees.
        /// </remarks>
        private Code code;

        /// <summary>
        ///     The one and only InstructionList object used by an instance of this class.
        /// </summary>
        /// <remarks>
        ///     The one and only InstructionList object used by an instance of this class.
        ///     It's here for performance reasons by do_verify() and its callees.
        /// </remarks>
        private InstructionList instructionList;

        /// <summary>Should only be instantiated by a Verifier.</summary>
        public Pass3aVerifier(Verifier owner, int method_no)
        {
            myOwner = owner;
            this.method_no = method_no;
        }

        /// <summary>
        ///     Pass 3a is the verification of static constraints of
        ///     JVM code (such as legal targets of branch instructions).
        /// </summary>
        /// <remarks>
        ///     Pass 3a is the verification of static constraints of
        ///     JVM code (such as legal targets of branch instructions).
        ///     This is the part of pass 3 where you do not need data
        ///     flow analysis.
        ///     JustIce also delays the checks for a correct exception
        ///     table of a Code attribute and correct line number entries
        ///     in a LineNumberTable attribute of a Code attribute (which
        ///     conceptually belong to pass 2) to this pass. Also, most
        ///     of the check for valid local variable entries in a
        ///     LocalVariableTable attribute of a Code attribute is
        ///     delayed until this pass.
        ///     All these checks need access to the code array of the
        ///     Code attribute.
        /// </remarks>
        /// <exception cref="InvalidMethodException">
        ///     if the method to verify does not exist.
        /// </exception>
        public override VerificationResult Do_verify()
        {
            try
            {
                if (myOwner.DoPass2().Equals(VerificationResult.VR_OK))
                {
                    // Okay, class file was loaded correctly by Pass 1
                    // and satisfies static constraints of Pass 2.
                    var jc = Repository.LookupClass(myOwner.GetClassName(
                    ));
                    var methods = jc.GetMethods();
                    if (method_no >= methods.Length) throw new InvalidMethodException("METHOD DOES NOT EXIST!");
                    var method = methods[method_no];
                    code = method.GetCode();
                    // No Code? Nothing to verify!
                    if (method.IsAbstract() || method.IsNative())
                        // IF mg HAS NO CODE (static constraint of Pass 2)
                        return VerificationResult.VR_OK;
                    // TODO:
                    // We want a very sophisticated code examination here with good explanations
                    // on where to look for an illegal instruction or such.
                    // Only after that we should try to build an InstructionList and throw an
                    // AssertionViolatedException if after our examination InstructionList building
                    // still fails.
                    // That examination should be implemented in a byte-oriented way, i.e. look for
                    // an instruction, make sure its validity, count its length, find the next
                    // instruction and so on.
                    try
                    {
                        instructionList = new InstructionList(method.GetCode().GetCode());
                    }
                    catch (Exception)
                    {
                        return new VerificationResult(VerificationResult.VERIFIED_REJECTED
                            , "Bad bytecode in the code array of the Code attribute of method '" + method +
                              "'.");
                    }

                    instructionList.SetPositions(true);
                    // Start verification.
                    var vr = VerificationResult.VR_OK;
                    //default
                    try
                    {
                        DelayedPass2Checks();
                    }
                    catch (ClassConstraintException cce)
                    {
                        vr = new VerificationResult(VerificationResult.VERIFIED_REJECTED
                            , cce.Message);
                        return vr;
                    }

                    try
                    {
                        Pass3StaticInstructionChecks();
                        Pass3StaticInstructionOperandsChecks();
                    }
                    catch (StaticCodeConstraintException scce)
                    {
                        vr = new VerificationResult(VerificationResult.VERIFIED_REJECTED
                            , scce.Message);
                    }
                    catch (InvalidCastException cce)
                    {
                        vr = new VerificationResult(VerificationResult.VERIFIED_REJECTED
                            , "Class Cast Exception: " + cce.Message);
                    }

                    return vr;
                }

                //did not pass Pass 2.
                return VerificationResult.VR_NOTYET;
            }
            catch (TypeLoadException e)
            {
                // FIXME: maybe not the best way to handle this
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>
        ///     These are the checks that could be done in pass 2 but are delayed to pass 3
        ///     for performance reasons.
        /// </summary>
        /// <remarks>
        ///     These are the checks that could be done in pass 2 but are delayed to pass 3
        ///     for performance reasons. Also, these checks need access to the code array
        ///     of the Code attribute of a Method so it's okay to perform them here.
        ///     Also see the description of the do_verify() method.
        /// </remarks>
        /// <exception cref="ClassConstraintException">
        ///     if the verification fails.
        /// </exception>
        /// <seealso cref="Do_verify()" />
        private void DelayedPass2Checks()
        {
            var instructionPositions = instructionList.GetInstructionPositions();
            var codeLength = code.GetCode().Length;
            /////////////////////
            // LineNumberTable //
            /////////////////////
            var lnt = code.GetLineNumberTable();
            if (lnt != null)
            {
                var lineNumbers = lnt.GetLineNumberTable();
                var offsets = new IntList();

                lineNumber_loop_continue:
                foreach (var lineNumber in lineNumbers)
                {
                    // may appear in any order.
                    foreach (var instructionPosition in instructionPositions)
                    {
                        // TODO: Make this a binary search! The instructionPositions array is naturally ordered!
                        var offset = lineNumber.GetStartPC();
                        if (instructionPosition == offset)
                        {
                            if (offsets.Contains(offset))
                                AddMessage("LineNumberTable attribute '" + code.GetLineNumberTable() +
                                           "' refers to the same code offset ('"
                                           + offset + "') more than once" +
                                           " which is violating the semantics [but is sometimes produced by IBM's 'jikes' compiler]."
                                );
                            else
                                offsets.Add(offset);
                            goto lineNumber_loop_continue;
                        }
                    }

                    throw new ClassConstraintException("Code attribute '" + code +
                                                       "' has a LineNumberTable attribute '" +
                                                       code.GetLineNumberTable() + "' referring to a code offset ('"
                                                       + lineNumber.GetStartPC() + "') that does not exist.");
                }
            }

            ///////////////////////////
            // LocalVariableTable(s) //
            ///////////////////////////
            /* We cannot use code.getLocalVariableTable() because there could be more
            than only one. This is a bug in BCEL. */
            var atts = code.GetAttributes();
            foreach (var att in atts)
                if (att is LocalVariableTable)
                {
                    var lvt = (LocalVariableTable) att;
                    var localVariables = lvt.GetLocalVariableTable();
                    foreach (var localVariable in localVariables)
                    {
                        var startpc = localVariable.GetStartPC();
                        var length = localVariable.GetLength();
                        if (!Contains(instructionPositions, startpc))
                            throw new ClassConstraintException("Code attribute '" + code +
                                                               "' has a LocalVariableTable attribute '" +
                                                               code.GetLocalVariableTable() +
                                                               "' referring to a code offset ('"
                                                               + startpc + "') that does not exist.");
                        if (!Contains(instructionPositions, startpc + length) && startpc + length != codeLength)
                            throw new ClassConstraintException("Code attribute '" + code +
                                                               "' has a LocalVariableTable attribute '" +
                                                               code.GetLocalVariableTable() +
                                                               "' referring to a code offset start_pc+length ('"
                                                               + (startpc + length) + "') that does not exist.");
                    }
                }

            ////////////////////
            // ExceptionTable //
            ////////////////////
            // In BCEL's "classfile" API, the startPC/endPC-notation is
            // inclusive/exclusive as in the Java Virtual Machine Specification.
            // WARNING: This is not true for BCEL's "generic" API.
            var exceptionTable = code.GetExceptionTable();
            foreach (var element in exceptionTable)
            {
                var startpc = element.GetStartPC();
                var endpc = element.GetEndPC();
                var handlerpc = element.GetHandlerPC();
                if (startpc >= endpc)
                    throw new ClassConstraintException("Code attribute '" + code +
                                                       "' has an exception_table entry '" + element +
                                                       "' that has its start_pc ('" + startpc
                                                       + "') not smaller than its end_pc ('" + endpc + "').");
                if (!Contains(instructionPositions, startpc))
                    throw new ClassConstraintException("Code attribute '" + code +
                                                       "' has an exception_table entry '" + element +
                                                       "' that has a non-existant bytecode offset as its start_pc ('"
                                                       + startpc + "').");
                if (!Contains(instructionPositions, endpc) && endpc != codeLength)
                    throw new ClassConstraintException("Code attribute '" + code +
                                                       "' has an exception_table entry '" + element +
                                                       "' that has a non-existant bytecode offset as its end_pc ('"
                                                       + startpc + "') [that is also not equal to code_length ('" +
                                                       codeLength + "')]."
                    );
                if (!Contains(instructionPositions, handlerpc))
                    throw new ClassConstraintException("Code attribute '" + code +
                                                       "' has an exception_table entry '" + element +
                                                       "' that has a non-existant bytecode offset as its handler_pc ('"
                                                       + handlerpc + "').");
            }
        }

        /// <summary>
        ///     These are the checks if constraints are satisfied which are described in the
        ///     Java Virtual Machine Specification, Second Edition as Static Constraints on
        ///     the instructions of Java Virtual Machine Code (chapter 4.8.1).
        /// </summary>
        /// <exception cref="StaticCodeConstraintException">
        ///     if the verification fails.
        /// </exception>
        private void Pass3StaticInstructionChecks()
        {
            // Code array must not be empty:
            // Enforced in pass 2 (also stated in the static constraints of the Code
            // array in vmspec2), together with pass 1 (reading code_length bytes and
            // interpreting them as code[]). So this must not be checked again here.
            if (code.GetCode().Length >= Const.MAX_CODE_SIZE)
                // length must be LESS than the max
                throw new StaticCodeInstructionConstraintException("Code array in code attribute '"
                                                                   + code + "' too big: must be smaller than " +
                                                                   Const.MAX_CODE_SIZE + "65536 bytes."
                );
            // First opcode at offset 0: okay, that's clear. Nothing to do.
            // Only instances of the instructions documented in Section 6.4 may appear in
            // the code array.
            // For BCEL's sake, we cannot handle WIDE stuff, but hopefully BCEL does its job right :)
            // The last byte of the last instruction in the code array must be the byte at index
            // code_length-1 : See the do_verify() comments. We actually don't iterate through the
            // byte array, but use an InstructionList so we cannot check for this. But BCEL does
            // things right, so it's implicitly okay.
            // TODO: Check how BCEL handles (and will handle) instructions like IMPDEP1, IMPDEP2,
            //       BREAKPOINT... that BCEL knows about but which are illegal anyway.
            //       We currently go the safe way here.
            var ih = instructionList.GetStart();
            while (ih != null)
            {
                var i = ih.GetInstruction();
                if (i is IMPDEP1)
                    throw new StaticCodeInstructionConstraintException(
                        "IMPDEP1 must not be in the code, it is an illegal instruction for _internal_ JVM use!"
                    );
                if (i is IMPDEP2)
                    throw new StaticCodeInstructionConstraintException(
                        "IMPDEP2 must not be in the code, it is an illegal instruction for _internal_ JVM use!"
                    );
                if (i is BREAKPOINT)
                    throw new StaticCodeInstructionConstraintException(
                        "BREAKPOINT must not be in the code, it is an illegal instruction for _internal_ JVM use!"
                    );
                ih = ih.GetNext();
            }

            // The original verifier seems to do this check here, too.
            // An unreachable last instruction may also not fall through the
            // end of the code, which is stupid -- but with the original
            // verifier's subroutine semantics one cannot predict reachability.
            var last = instructionList.GetEnd().GetInstruction();
            if (!(last is ReturnInstruction || last is RET ||
                  last is GotoInstruction || last is ATHROW))
                throw new StaticCodeInstructionConstraintException(
                    "Execution must not fall off the bottom of the code array."
                    + " This constraint is enforced statically as some existing verifiers do" +
                    " - so it may be a false alarm if the last instruction is not reachable."
                );
        }

        /// <summary>
        ///     These are the checks for the satisfaction of constraints which are described in the
        ///     Java Virtual Machine Specification, Second Edition as Static Constraints on
        ///     the operands of instructions of Java Virtual Machine Code (chapter 4.8.1).
        /// </summary>
        /// <remarks>
        ///     These are the checks for the satisfaction of constraints which are described in the
        ///     Java Virtual Machine Specification, Second Edition as Static Constraints on
        ///     the operands of instructions of Java Virtual Machine Code (chapter 4.8.1).
        ///     BCEL parses the code array to create an InstructionList and therefore has to check
        ///     some of these constraints. Additional checks are also implemented here.
        /// </remarks>
        /// <exception cref="StaticCodeConstraintException">
        ///     if the verification fails.
        /// </exception>
        private void Pass3StaticInstructionOperandsChecks()
        {
            try
            {
                // When building up the InstructionList, BCEL has already done all those checks
                // mentioned in The Java Virtual Machine Specification, Second Edition, as
                // "static constraints on the operands of instructions in the code array".
                // TODO: see the do_verify() comments. Maybe we should really work on the
                //       byte array first to give more comprehensive messages.
                // TODO: Review Exception API, possibly build in some "offending instruction" thing
                //       when we're ready to insulate the offending instruction by doing the
                //       above thing.
                // TODO: Implement as much as possible here. BCEL does _not_ check everything.
                var cpg = new ConstantPoolGen(Repository
                    .LookupClass(myOwner.GetClassName()).GetConstantPool());
                var v = new InstOperandConstraintVisitor
                    (this, cpg);
                // Checks for the things BCEL does _not_ handle itself.
                var ih = instructionList.GetStart();
                while (ih != null)
                {
                    var i = ih.GetInstruction();
                    // An "own" constraint, due to JustIce's new definition of what "subroutine" means.
                    if (i is JsrInstruction)
                    {
                        var target = ((JsrInstruction) i).GetTarget
                            ();
                        if (target == instructionList.GetStart())
                            throw new StaticCodeInstructionOperandConstraintException(
                                "Due to JustIce's clear definition of subroutines, no JSR or JSR_W may have a top-level instruction"
                                + " (such as the very first instruction, which is targeted by instruction '" +
                                ih + "' as its target.");
                        if (!(target.GetInstruction() is ASTORE))
                            throw new StaticCodeInstructionOperandConstraintException(
                                "Due to JustIce's clear definition of subroutines, no JSR or JSR_W may target anything else"
                                + " than an ASTORE instruction. Instruction '" + ih + "' targets '" + target +
                                "'.");
                    }

                    // vmspec2, page 134-137
                    ih.Accept(v);
                    ih = ih.GetNext();
                }
            }
            catch (TypeLoadException e)
            {
                // FIXME: maybe not the best way to handle this
                throw new AssertionViolatedException("Missing class: " + e, e);
            }
        }

        /// <summary>
        ///     A small utility method returning if a given int i is in the given int[] ints.
        /// </summary>
        private static bool Contains(int[] ints, int i)
        {
            foreach (var k in ints)
                if (k == i)
                    return true;
            return false;
        }

        /// <summary>Returns the method number as supplied when instantiating.</summary>
        public int GetMethodNo()
        {
            return method_no;
        }

        /// <summary>
        ///     This visitor class does the actual checking for the instruction
        ///     operand's constraints.
        /// </summary>
        private class InstOperandConstraintVisitor : Generic.EmptyVisitor
        {
            private readonly Pass3aVerifier _enclosing;

            /// <summary>The ConstantPoolGen instance this Visitor operates on.</summary>
            private readonly ConstantPoolGen constantPoolGen;

            /// <summary>The only Constructor.</summary>
            internal InstOperandConstraintVisitor(Pass3aVerifier _enclosing, ConstantPoolGen
                constantPoolGen)
            {
                this._enclosing = _enclosing;
                this.constantPoolGen = constantPoolGen;
            }

            /// <summary>
            ///     Utility method to return the max_locals value of the method verified
            ///     by the surrounding Pass3aVerifier instance.
            /// </summary>
            private int Max_locals()
            {
                try
                {
                    return Repository.LookupClass(_enclosing.myOwner.GetClassName()).GetMethods
                        ()[_enclosing.method_no].GetCode().GetMaxLocals();
                }
                catch (TypeLoadException e)
                {
                    // FIXME: maybe not the best way to handle this
                    throw new AssertionViolatedException("Missing class: " + e, e);
                }
            }

            /// <summary>A utility method to always raise an exeption.</summary>
            private void ConstraintViolated(Instruction i, string message)
            {
                throw new StaticCodeInstructionOperandConstraintException("Instruction "
                                                                          + i + " constraint violated: " + message);
            }

            /// <summary>
            ///     A utility method to raise an exception if the index is not
            ///     a valid constant pool index.
            /// </summary>
            private void IndexValid(Instruction i, int idx)
            {
                if (idx < 0 || idx >= constantPoolGen.GetSize())
                    ConstraintViolated(i, "Illegal constant pool index '" + idx + "'.");
            }

            ///////////////////////////////////////////////////////////
            // The Java Virtual Machine Specification, pages 134-137 //
            ///////////////////////////////////////////////////////////
            /// <summary>Assures the generic preconditions of a LoadClass instance.</summary>
            /// <remarks>
            ///     Assures the generic preconditions of a LoadClass instance.
            ///     The referenced class is loaded and pass2-verified.
            /// </remarks>
            public override void VisitLoadClass(LoadClass loadClass)
            {
                var t = loadClass.GetLoadClassType(constantPoolGen);
                if (t != null)
                {
                    // null means "no class is loaded"
                    var v = VerifierFactory.GetVerifier(t.GetClassName
                        ());
                    var vr = v.DoPass1();
                    if (vr.GetStatus() != VerificationResult.VERIFIED_OK)
                        ConstraintViolated((Instruction) loadClass, "Class '" + loadClass
                                                                        .GetLoadClassType(constantPoolGen)
                                                                        .GetClassName() +
                                                                    "' is referenced, but cannot be loaded: '"
                                                                    + vr + "'.");
                }
            }

            // The target of each jump and branch instruction [...] must be the opcode [...]
            // BCEL _DOES_ handle this.
            // tableswitch: BCEL will do it, supposedly.
            // lookupswitch: BCEL will do it, supposedly.
            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitLDC(LDC ldc)
            {
                // LDC and LDC_W (LDC_W is a subclass of LDC in BCEL's model)
                IndexValid(ldc, ldc.GetIndex());
                var c = constantPoolGen.GetConstant(ldc.GetIndex());
                if (c is ConstantClass)
                    _enclosing.AddMessage("Operand of LDC or LDC_W is CONSTANT_Class '" + c +
                                          "' - this is only supported in JDK 1.5 and higher."
                    );
                else if (!(c is ConstantInteger || c is ConstantFloat || c is ConstantString))
                    ConstraintViolated(ldc,
                        "Operand of LDC or LDC_W must be one of CONSTANT_Integer, CONSTANT_Float or CONSTANT_String, but is '"
                        + c + "'.");
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitLDC2_W(LDC2_W o)
            {
                // LDC2_W
                IndexValid(o, o.GetIndex());
                var c = constantPoolGen.GetConstant(o.GetIndex());
                if (!(c is ConstantLong || c is ConstantDouble))
                    ConstraintViolated(o, "Operand of LDC2_W must be CONSTANT_Long or CONSTANT_Double, but is '"
                                          + c + "'.");
                try
                {
                    IndexValid(o, o.GetIndex() + 1);
                }
                catch (StaticCodeInstructionOperandConstraintException e)
                {
                    throw new AssertionViolatedException(
                        "OOPS: Does not BCEL handle that? LDC2_W operand has a problem."
                        , e);
                }
            }

            private ObjectType GetObjectType(FieldInstruction o)
            {
                var rt = o.GetReferenceType(constantPoolGen);
                if (rt is ObjectType) return (ObjectType) rt;
                ConstraintViolated(o, "expecting ObjectType but got " + rt);
                return null;
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitFieldInstruction(FieldInstruction o)
            {
                //getfield, putfield, getstatic, putstatic
                try
                {
                    IndexValid(o, o.GetIndex());
                    var c = constantPoolGen.GetConstant(o.GetIndex());
                    if (!(c is ConstantFieldref))
                        ConstraintViolated(o, "Indexing a constant that's not a CONSTANT_Fieldref but a '"
                                              + c + "'.");
                    var field_name = o.GetFieldName(constantPoolGen);
                    var jc = Repository.LookupClass(GetObjectType(o)
                        .GetClassName());
                    var fields = jc.GetFields();
                    Field f = null;
                    foreach (var field in fields)
                        if (field.GetName().Equals(field_name))
                        {
                            var f_type = Type.GetType(field.GetSignature());
                            var o_type = o.GetType(constantPoolGen);
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
                                    var o_type = o.GetType(constantPoolGen);
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
                            ConstraintViolated(o, "Referenced field '" + field_name + "' does not exist in class '"
                                                  + jc.GetClassName() + "'.");
                    }
                    else
                    {
                        /* TODO: Check if assignment compatibility is sufficient.
                        What does Sun do? */
                        Type.GetType(f.GetSignature());
                        o.GetType(constantPoolGen);
                    }
                }
                catch (TypeLoadException e)
                {
                    //                Type f_type = Type.getType(f.getSignature());
                    //                Type o_type = o.getType(cpg);
                    // Argh. Sun's implementation allows us to have multiple fields of
                    // the same name but with a different signature.
                    //if (! f_type.equals(o_type)) {
                    //    constraintViolated(o,
                    //        "Referenced field '"+field_name+"' has type '"+f_type+"' instead of '"+o_type+"' as expected.");
                    //}
                    /* TODO: Check for access modifiers here. */
                    // FIXME: maybe not the best way to handle this
                    throw new AssertionViolatedException("Missing class: " + e, e);
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitInvokeInstruction(InvokeInstruction o)
            {
                IndexValid(o, o.GetIndex());
                if (o is INVOKEVIRTUAL || o is INVOKESPECIAL || o is INVOKESTATIC)
                {
                    var c = constantPoolGen.GetConstant(o.GetIndex());
                    if (!(c is ConstantMethodref))
                    {
                        ConstraintViolated(o, "Indexing a constant that's not a CONSTANT_Methodref but a '"
                                              + c + "'.");
                    }
                    else
                    {
                        // Constants are okay due to pass2.
                        var cnat = (ConstantNameAndType) constantPoolGen.GetConstant(
                            ((ConstantMethodref) c).GetNameAndTypeIndex
                                ());
                        var cutf8 = (ConstantUtf8) constantPoolGen
                            .GetConstant(cnat.GetNameIndex());
                        if (cutf8.GetBytes().Equals(Const.CONSTRUCTOR_NAME) && !(o is INVOKESPECIAL
                                ))
                            ConstraintViolated(o,
                                "Only INVOKESPECIAL is allowed to invoke instance initialization methods."
                            );
                        if (!cutf8.GetBytes().Equals(Const.CONSTRUCTOR_NAME) && cutf8.GetBytes
                                ().StartsWith("<"))
                            ConstraintViolated(o,
                                "No method with a name beginning with '<' other than the instance initialization methods"
                                + " may be called by the method invocation instructions.");
                    }
                }
                else
                {
                    //if (o instanceof INVOKEINTERFACE) {
                    var c = constantPoolGen.GetConstant(o.GetIndex());
                    if (!(c is ConstantInterfaceMethodref))
                        ConstraintViolated(o, "Indexing a constant that's not a CONSTANT_InterfaceMethodref but a '"
                                              + c + "'.");
                    // TODO: From time to time check if BCEL allows to detect if the
                    // 'count' operand is consistent with the information in the
                    // CONSTANT_InterfaceMethodref and if the last operand is zero.
                    // By now, BCEL hides those two operands because they're superfluous.
                    // Invoked method must not be <init> or <clinit>
                    var cnat = (ConstantNameAndType) constantPoolGen.GetConstant(((ConstantInterfaceMethodref) c)
                        .GetNameAndTypeIndex());
                    var name = ((ConstantUtf8) constantPoolGen.GetConstant(cnat
                        .GetNameIndex())).GetBytes();
                    if (name.Equals(Const.CONSTRUCTOR_NAME))
                        ConstraintViolated(o, "Method to invoke must not be '" + Const.CONSTRUCTOR_NAME
                                                                               + "'.");
                    if (name.Equals(Const.STATIC_INITIALIZER_NAME))
                        ConstraintViolated(o, "Method to invoke must not be '" + Const.STATIC_INITIALIZER_NAME
                                                                               + "'.");
                }

                // The LoadClassType is the method-declaring class, so we have to check the other types.
                var t = o.GetReturnType(constantPoolGen);
                if (t is ArrayType) t = ((ArrayType) t).GetBasicType();
                if (t is ObjectType)
                {
                    var v = VerifierFactory.GetVerifier(((ObjectType
                        ) t).GetClassName());
                    var vr = v.DoPass2();
                    if (vr.GetStatus() != VerificationResult.VERIFIED_OK)
                        ConstraintViolated(o, "Return type class/interface could not be verified successfully: '"
                                              + vr.GetMessage() + "'.");
                }

                var ts = o.GetArgumentTypes(constantPoolGen);
                foreach (var element in ts)
                {
                    t = element;
                    if (t is ArrayType) t = ((ArrayType) t).GetBasicType();
                    if (t is ObjectType)
                    {
                        var v = VerifierFactory.GetVerifier(((ObjectType
                            ) t).GetClassName());
                        var vr = v.DoPass2();
                        if (vr.GetStatus() != VerificationResult.VERIFIED_OK)
                            ConstraintViolated(o, "Argument type class/interface could not be verified successfully: '"
                                                  + vr.GetMessage() + "'.");
                    }
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitINSTANCEOF(INSTANCEOF o)
            {
                IndexValid(o, o.GetIndex());
                var c = constantPoolGen.GetConstant(o.GetIndex());
                if (!(c is ConstantClass))
                    ConstraintViolated(o, "Expecting a CONSTANT_Class operand, but found a '" +
                                          c + "'.");
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitCHECKCAST(CHECKCAST o)
            {
                IndexValid(o, o.GetIndex());
                var c = constantPoolGen.GetConstant(o.GetIndex());
                if (!(c is ConstantClass))
                    ConstraintViolated(o, "Expecting a CONSTANT_Class operand, but found a '" +
                                          c + "'.");
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitNEW(NEW o)
            {
                IndexValid(o, o.GetIndex());
                var c = constantPoolGen.GetConstant(o.GetIndex());
                if (!(c is ConstantClass))
                {
                    ConstraintViolated(o, "Expecting a CONSTANT_Class operand, but found a '" +
                                          c + "'.");
                }
                else
                {
                    var cutf8 = (ConstantUtf8) constantPoolGen
                        .GetConstant(((ConstantClass) c).GetNameIndex());
                    var t = Type.GetType("L" + cutf8.GetBytes() + ";");
                    if (t is ArrayType) ConstraintViolated(o, "NEW must not be used to create an array.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitMULTIANEWARRAY(MULTIANEWARRAY o)
            {
                IndexValid(o, o.GetIndex());
                var c = constantPoolGen.GetConstant(o.GetIndex());
                if (!(c is ConstantClass))
                    ConstraintViolated(o, "Expecting a CONSTANT_Class operand, but found a '" +
                                          c + "'.");
                int dimensions2create = o.GetDimensions();
                if (dimensions2create < 1)
                    ConstraintViolated(o, "Number of dimensions to create must be greater than zero."
                    );
                var t = o.GetType(constantPoolGen);
                if (t is ArrayType)
                {
                    var dimensions = ((ArrayType) t).GetDimensions();
                    if (dimensions < dimensions2create)
                        ConstraintViolated(o, "Not allowed to create array with more dimensions ('"
                                              + dimensions2create +
                                              "') than the one referenced by the CONSTANT_Class '" + t +
                                              "'.");
                }
                else
                {
                    ConstraintViolated(o, "Expecting a CONSTANT_Class referencing an array type."
                                          + " [Constraint not found in The Java Virtual Machine Specification, Second Edition, 4.8.1]"
                    );
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitANEWARRAY(ANEWARRAY o)
            {
                IndexValid(o, o.GetIndex());
                var c = constantPoolGen.GetConstant(o.GetIndex());
                if (!(c is ConstantClass))
                    ConstraintViolated(o, "Expecting a CONSTANT_Class operand, but found a '" +
                                          c + "'.");
                var t = o.GetType(constantPoolGen);
                if (t is ArrayType)
                {
                    var dimensions = ((ArrayType) t).GetDimensions();
                    if (dimensions > Const.MAX_ARRAY_DIMENSIONS)
                        ConstraintViolated(o, "Not allowed to create an array with more than " + Const
                                                  .MAX_ARRAY_DIMENSIONS + " dimensions;" + " actual: " + dimensions);
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitNEWARRAY(NEWARRAY o)
            {
                var t = o.GetTypecode();
                if (!(t == Const.T_BOOLEAN || t == Const.T_CHAR || t == Const
                          .T_FLOAT || t == Const.T_DOUBLE || t == Const.T_BYTE || t == Const
                          .T_SHORT || t == Const.T_INT || t == Const.T_LONG))
                    ConstraintViolated(o, "Illegal type code '+t+' for 'atype' operand.");
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitILOAD(ILOAD o)
            {
                var idx = o.GetIndex();
                if (idx < 0)
                {
                    ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
                }
                else
                {
                    var maxminus1 = Max_locals() - 1;
                    if (idx > maxminus1)
                        ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
                                              + maxminus1 + "'.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitFLOAD(FLOAD o)
            {
                var idx = o.GetIndex();
                if (idx < 0)
                {
                    ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
                }
                else
                {
                    var maxminus1 = Max_locals() - 1;
                    if (idx > maxminus1)
                        ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
                                              + maxminus1 + "'.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitALOAD(ALOAD o)
            {
                var idx = o.GetIndex();
                if (idx < 0)
                {
                    ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
                }
                else
                {
                    var maxminus1 = Max_locals() - 1;
                    if (idx > maxminus1)
                        ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
                                              + maxminus1 + "'.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitISTORE(ISTORE o)
            {
                var idx = o.GetIndex();
                if (idx < 0)
                {
                    ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
                }
                else
                {
                    var maxminus1 = Max_locals() - 1;
                    if (idx > maxminus1)
                        ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
                                              + maxminus1 + "'.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitFSTORE(FSTORE o)
            {
                var idx = o.GetIndex();
                if (idx < 0)
                {
                    ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
                }
                else
                {
                    var maxminus1 = Max_locals() - 1;
                    if (idx > maxminus1)
                        ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
                                              + maxminus1 + "'.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitASTORE(ASTORE o)
            {
                var idx = o.GetIndex();
                if (idx < 0)
                {
                    ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
                }
                else
                {
                    var maxminus1 = Max_locals() - 1;
                    if (idx > maxminus1)
                        ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
                                              + maxminus1 + "'.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitIINC(IINC o)
            {
                var idx = o.GetIndex();
                if (idx < 0)
                {
                    ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
                }
                else
                {
                    var maxminus1 = Max_locals() - 1;
                    if (idx > maxminus1)
                        ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
                                              + maxminus1 + "'.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitRET(RET o)
            {
                var idx = o.GetIndex();
                if (idx < 0)
                {
                    ConstraintViolated(o, "Index '" + idx + "' must be non-negative.");
                }
                else
                {
                    var maxminus1 = Max_locals() - 1;
                    if (idx > maxminus1)
                        ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-1 '"
                                              + maxminus1 + "'.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitLLOAD(LLOAD o)
            {
                var idx = o.GetIndex();
                if (idx < 0)
                {
                    ConstraintViolated(o,
                        "Index '" + idx + "' must be non-negative." +
                        " [Constraint by JustIce as an analogon to the single-slot xLOAD/xSTORE instructions; may not happen anyway.]"
                    );
                }
                else
                {
                    var maxminus2 = Max_locals() - 2;
                    if (idx > maxminus2)
                        ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-2 '"
                                              + maxminus2 + "'.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitDLOAD(DLOAD o)
            {
                var idx = o.GetIndex();
                if (idx < 0)
                {
                    ConstraintViolated(o,
                        "Index '" + idx + "' must be non-negative." +
                        " [Constraint by JustIce as an analogon to the single-slot xLOAD/xSTORE instructions; may not happen anyway.]"
                    );
                }
                else
                {
                    var maxminus2 = Max_locals() - 2;
                    if (idx > maxminus2)
                        ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-2 '"
                                              + maxminus2 + "'.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitLSTORE(LSTORE o)
            {
                var idx = o.GetIndex();
                if (idx < 0)
                {
                    ConstraintViolated(o,
                        "Index '" + idx + "' must be non-negative." +
                        " [Constraint by JustIce as an analogon to the single-slot xLOAD/xSTORE instructions; may not happen anyway.]"
                    );
                }
                else
                {
                    var maxminus2 = Max_locals() - 2;
                    if (idx > maxminus2)
                        ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-2 '"
                                              + maxminus2 + "'.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitDSTORE(DSTORE o)
            {
                var idx = o.GetIndex();
                if (idx < 0)
                {
                    ConstraintViolated(o,
                        "Index '" + idx + "' must be non-negative." +
                        " [Constraint by JustIce as an analogon to the single-slot xLOAD/xSTORE instructions; may not happen anyway.]"
                    );
                }
                else
                {
                    var maxminus2 = Max_locals() - 2;
                    if (idx > maxminus2)
                        ConstraintViolated(o, "Index '" + idx + "' must not be greater than max_locals-2 '"
                                              + maxminus2 + "'.");
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitLOOKUPSWITCH(LOOKUPSWITCH o)
            {
                var matchs = o.GetMatchs();
                var max = int.MinValue;
                for (var i = 0; i < matchs.Length; i++)
                {
                    if (matchs[i] == max && i != 0)
                        ConstraintViolated(o, "Match '" + matchs[i] + "' occurs more than once.");
                    if (matchs[i] < max)
                        ConstraintViolated(o, "Lookup table must be sorted but isn't.");
                    else
                        max = matchs[i];
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitTABLESWITCH(TABLESWITCH o)
            {
            }

            // "high" must be >= "low". We cannot check this, as BCEL hides
            // it from us.
            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitPUTSTATIC(PUTSTATIC o)
            {
                try
                {
                    var field_name = o.GetFieldName(constantPoolGen);
                    var jc = Repository.LookupClass(GetObjectType(o)
                        .GetClassName());
                    var fields = jc.GetFields();
                    Field f = null;
                    foreach (var field in fields)
                        if (field.GetName().Equals(field_name))
                        {
                            f = field;
                            break;
                        }

                    if (f == null)
                        throw new AssertionViolatedException("Field '" + field_name +
                                                             "' not found in " + jc.GetClassName());
                    if (f.IsFinal())
                        if (!_enclosing.myOwner.GetClassName().Equals(GetObjectType(o).GetClassName
                            ()))
                            ConstraintViolated(o,
                                "Referenced field '" + f +
                                "' is final and must therefore be declared in the current class '"
                                + _enclosing.myOwner.GetClassName() + "' which is not the case: it is declared in '"
                                + o.GetReferenceType(constantPoolGen) + "'.");
                    if (!f.IsStatic())
                        ConstraintViolated(o, "Referenced field '" + f + "' is not static which it should be."
                        );
                    var meth_name = Repository.LookupClass(_enclosing.myOwner.GetClassName
                        ()).GetMethods()[_enclosing.method_no].GetName();
                    // If it's an interface, it can be set only in <clinit>.
                    if (!jc.IsClass() && !meth_name.Equals(Const.STATIC_INITIALIZER_NAME))
                        ConstraintViolated(o, "Interface field '" + f + "' must be set in a '" + Const
                                                  .STATIC_INITIALIZER_NAME + "' method.");
                }
                catch (TypeLoadException e)
                {
                    // FIXME: maybe not the best way to handle this
                    throw new AssertionViolatedException("Missing class: " + e, e);
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitGETSTATIC(GETSTATIC o)
            {
                try
                {
                    var field_name = o.GetFieldName(constantPoolGen);
                    var jc = Repository.LookupClass(GetObjectType(o)
                        .GetClassName());
                    var fields = jc.GetFields();
                    Field f = null;
                    foreach (var field in fields)
                        if (field.GetName().Equals(field_name))
                        {
                            f = field;
                            break;
                        }

                    if (f == null)
                        throw new AssertionViolatedException("Field '" + field_name +
                                                             "' not found in " + jc.GetClassName());
                    if (!f.IsStatic())
                        ConstraintViolated(o, "Referenced field '" + f + "' is not static which it should be."
                        );
                }
                catch (TypeLoadException e)
                {
                    // FIXME: maybe not the best way to handle this
                    throw new AssertionViolatedException("Missing class: " + e, e);
                }
            }

            /* Checks if the constraints of operands of the said instruction(s) are satisfied. */
            //public void visitPUTFIELD(PUTFIELD o) {
            // for performance reasons done in Pass 3b
            //}
            /* Checks if the constraints of operands of the said instruction(s) are satisfied. */
            //public void visitGETFIELD(GETFIELD o) {
            // for performance reasons done in Pass 3b
            //}
            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitINVOKEDYNAMIC(INVOKEDYNAMIC o)
            {
                throw new Exception("INVOKEDYNAMIC instruction is not supported at this time"
                );
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitINVOKEINTERFACE(INVOKEINTERFACE o)
            {
                try
                {
                    // INVOKEINTERFACE is a LoadClass; the Class where the referenced method is declared in,
                    // is therefore resolved/verified.
                    // INVOKEINTERFACE is an InvokeInstruction, the argument and return types are resolved/verified,
                    // too. So are the allowed method names.
                    var classname = o.GetClassName(constantPoolGen);
                    var jc = Repository.LookupClass(classname);
                    var m = GetMethodRecursive(jc, o);
                    if (m == null)
                        ConstraintViolated(o, "Referenced method '" + o.GetMethodName(constantPoolGen
                                              ) + "' with expected signature '" + o.GetSignature(constantPoolGen) +
                                              "' not found in class '"
                                              + jc.GetClassName() + "'.");
                    if (jc.IsClass())
                        ConstraintViolated(o,
                            "Referenced class '" + jc.GetClassName() + "' is a class, but not an interface as expected."
                        );
                }
                catch (TypeLoadException e)
                {
                    // FIXME: maybe not the best way to handle this
                    throw new AssertionViolatedException("Missing class: " + e, e);
                }
            }

            /// <summary>
            ///     Looks for the method referenced by the given invoke instruction in the given class
            ///     or its super classes and super interfaces.
            /// </summary>
            /// <param name="jc">the class that defines the referenced method</param>
            /// <param name="invoke">the instruction that references the method</param>
            /// <returns>the referenced method or null if not found.</returns>
            /// <exception cref="System.TypeLoadException" />
            private Method GetMethodRecursive(JavaClass jc, InvokeInstruction
                invoke)
            {
                Method m;
                //look in the given class
                m = GetMethod(jc, invoke);
                if (m != null)
                    //method found in given class
                    return m;
                //method not found, look in super classes
                foreach (var superclass in jc.GetSuperClasses())
                {
                    m = GetMethod(superclass, invoke);
                    if (m != null)
                        //method found in super class
                        return m;
                }

                //method not found, look in super interfaces
                foreach (var superclass in jc.GetInterfaces())
                {
                    m = GetMethod(superclass, invoke);
                    if (m != null)
                        //method found in super interface
                        return m;
                }

                //method not found in the hierarchy
                return null;
            }

            /// <summary>
            ///     Looks for the method referenced by the given invoke instruction in the given class.
            /// </summary>
            /// <param name="jc">the class that defines the referenced method</param>
            /// <param name="invoke">the instruction that references the method</param>
            /// <returns>the referenced method or null if not found.</returns>
            private Method GetMethod(JavaClass jc, InvokeInstruction
                invoke)
            {
                var ms = jc.GetMethods();
                foreach (var element in ms)
                    if (element.GetName().Equals(invoke.GetMethodName(constantPoolGen)) && Type
                            .GetReturnType(element.GetSignature()).Equals(invoke.GetReturnType(constantPoolGen
                            )) && Objarrayequals(Type.GetArgumentTypes(element.GetSignature
                            ()), invoke.GetArgumentTypes(constantPoolGen)))
                        return element;
                return null;
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitINVOKESPECIAL(INVOKESPECIAL o)
            {
                try
                {
                    // INVOKESPECIAL is a LoadClass; the Class where the referenced method is declared in,
                    // is therefore resolved/verified.
                    // INVOKESPECIAL is an InvokeInstruction, the argument and return types are resolved/verified,
                    // too. So are the allowed method names.
                    var classname = o.GetClassName(constantPoolGen);
                    var jc = Repository.LookupClass(classname);
                    var m = GetMethodRecursive(jc, o);
                    if (m == null)
                        ConstraintViolated(o, "Referenced method '" + o.GetMethodName(constantPoolGen
                                              ) + "' with expected signature '" + o.GetSignature(constantPoolGen) +
                                              "' not found in class '"
                                              + jc.GetClassName() + "'.");
                    var current = Repository.LookupClass(_enclosing.myOwner.GetClassName());
                    if (current.IsSuper())
                        if (Repository.InstanceOf(current, jc) && !current.Equals(jc))
                            if (!o.GetMethodName(constantPoolGen).Equals(Const.CONSTRUCTOR_NAME))
                            {
                                // Special lookup procedure for ACC_SUPER classes.
                                var supidx = -1;
                                Method meth = null;
                                while (supidx != 0)
                                {
                                    supidx = current.GetSuperclassNameIndex();
                                    current = Repository.LookupClass(current.GetSuperclassName());
                                    var meths = current.GetMethods();
                                    foreach (var meth2 in meths)
                                        if (meth2.GetName().Equals(o.GetMethodName(constantPoolGen)) && Type
                                                .GetReturnType(meth2.GetSignature()).Equals(o.GetReturnType(
                                                    constantPoolGen
                                                )) && Objarrayequals(Type.GetArgumentTypes(meth2.GetSignature
                                                ()), o.GetArgumentTypes(constantPoolGen)))
                                        {
                                            meth = meth2;
                                            break;
                                        }

                                    if (meth != null) break;
                                }

                                if (meth == null)
                                    ConstraintViolated(o, "ACC_SUPER special lookup procedure not successful: method '"
                                                          + o.GetMethodName(constantPoolGen) +
                                                          "' with proper signature not declared in superclass hierarchy."
                                    );
                            }
                }
                catch (TypeLoadException e)
                {
                    // FIXME: maybe not the best way to handle this
                    throw new AssertionViolatedException("Missing class: " + e, e);
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitINVOKESTATIC(INVOKESTATIC o)
            {
                try
                {
                    // INVOKESTATIC is a LoadClass; the Class where the referenced method is declared in,
                    // is therefore resolved/verified.
                    // INVOKESTATIC is an InvokeInstruction, the argument and return types are resolved/verified,
                    // too. So are the allowed method names.
                    var classname = o.GetClassName(constantPoolGen);
                    var jc = Repository.LookupClass(classname);
                    var m = GetMethodRecursive(jc, o);
                    if (m == null)
                        ConstraintViolated(o, "Referenced method '" + o.GetMethodName(constantPoolGen
                                              ) + "' with expected signature '" + o.GetSignature(constantPoolGen) +
                                              "' not found in class '"
                                              + jc.GetClassName() + "'.");
                    else if (!m.IsStatic())
                        // implies it's not abstract, verified in pass 2.
                        ConstraintViolated(o, "Referenced method '" + o.GetMethodName(constantPoolGen
                                              ) + "' has ACC_STATIC unset.");
                }
                catch (TypeLoadException e)
                {
                    // FIXME: maybe not the best way to handle this
                    throw new AssertionViolatedException("Missing class: " + e, e);
                }
            }

            /// <summary>
            ///     Checks if the constraints of operands of the said instruction(s) are satisfied.
            /// </summary>
            public override void VisitINVOKEVIRTUAL(INVOKEVIRTUAL o)
            {
                try
                {
                    // INVOKEVIRTUAL is a LoadClass; the Class where the referenced method is declared in,
                    // is therefore resolved/verified.
                    // INVOKEVIRTUAL is an InvokeInstruction, the argument and return types are resolved/verified,
                    // too. So are the allowed method names.
                    var classname = o.GetClassName(constantPoolGen);
                    var jc = Repository.LookupClass(classname);
                    var m = GetMethodRecursive(jc, o);
                    if (m == null)
                        ConstraintViolated(o, "Referenced method '" + o.GetMethodName(constantPoolGen
                                              ) + "' with expected signature '" + o.GetSignature(constantPoolGen) +
                                              "' not found in class '"
                                              + jc.GetClassName() + "'.");
                    if (!jc.IsClass())
                        ConstraintViolated(o,
                            "Referenced class '" + jc.GetClassName() + "' is an interface, but not a class as expected."
                        );
                }
                catch (TypeLoadException e)
                {
                    // FIXME: maybe not the best way to handle this
                    throw new AssertionViolatedException("Missing class: " + e, e);
                }
            }

            // WIDE stuff is BCEL-internal and cannot be checked here.
            /// <summary>A utility method like equals(Object) for arrays.</summary>
            /// <remarks>
            ///     A utility method like equals(Object) for arrays.
            ///     The equality of the elements is based on their equals(Object)
            ///     method instead of their object identity.
            /// </remarks>
            private bool Objarrayequals(object[] o, object[] p)
            {
                if (o.Length != p.Length) return false;
                for (var i = 0; i < o.Length; i++)
                    if (!o[i].Equals(p[i]))
                        return false;
                return true;
            }
        }
    }
}