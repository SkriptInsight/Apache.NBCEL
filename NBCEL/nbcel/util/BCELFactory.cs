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
using System.IO;
using System.Text;
using NBCEL.classfile;
using NBCEL.generic;
using Sharpen;
using EmptyVisitor = NBCEL.generic.EmptyVisitor;
using Type = NBCEL.generic.Type;

namespace NBCEL.util
{
	/// <summary>Factory creates il.append() statements, and sets instruction targets.</summary>
	/// <remarks>
	///     Factory creates il.append() statements, and sets instruction targets.
	///     A helper class for BCELifier.
	/// </remarks>
	/// <seealso cref="BCELifier" />
	internal class BCELFactory : EmptyVisitor
    {
        private static readonly string CONSTANT_PREFIX = typeof(Const).GetSimpleName
                                                             () + ".";

        private readonly ConstantPoolGen _cp;

        private readonly MethodGen _mg;

        private readonly TextWriter _out;

        private readonly IDictionary<Instruction
            , InstructionHandle> branch_map = new Dictionary
            <Instruction, InstructionHandle>();

        private readonly List<BranchInstruction>
            branches = new List<BranchInstruction>
                ();

        internal BCELFactory(MethodGen mg, TextWriter @out)
        {
            _mg = mg;
            _cp = mg.GetConstantPool();
            _out = @out;
        }

        public virtual void Start()
        {
            if (!_mg.IsAbstract() && !_mg.IsNative())
            {
                for (var ih = _mg.GetInstructionList().GetStart();
                    ih
                    != null;
                    ih = ih.GetNext())
                {
                    var i = ih.GetInstruction();
                    if (i is BranchInstruction) Collections.Put(branch_map, i, ih);
                    // memorize container
                    if (ih.HasTargeters())
                    {
                        if (i is BranchInstruction)
                            _out.WriteLine("    InstructionHandle ih_" + ih.GetPosition() + ";");
                        else
                            _out.Write("    InstructionHandle ih_" + ih.GetPosition() + " = ");
                    }
                    else
                    {
                        _out.Write("    ");
                    }

                    if (!VisitInstruction(i)) i.Accept(this);
                }

                UpdateBranchTargets();
                UpdateExceptionHandlers();
            }
        }

        private bool VisitInstruction(Instruction i)
        {
            var opcode = i.GetOpcode();
            if (InstructionConst.GetInstruction(opcode) != null && !(i is BaseConstantPushInstruction
                    ) && !(i is ReturnInstruction))
            {
                // Handled below
                _out.WriteLine("il.append(InstructionConst." + i.GetName().ToUpper() + ");");
                return true;
            }

            return false;
        }

        public override void VisitLocalVariableInstruction(LocalVariableInstruction
            i)
        {
            var opcode = i.GetOpcode();
            var type = i.GetType(_cp);
            if (opcode == Const.IINC)
            {
                _out.WriteLine("il.append(new IINC(" + i.GetIndex() + ", " + ((IINC) i
                               ).GetIncrement() + "));");
            }
            else
            {
                var kind = opcode < Const.ISTORE ? "Load" : "Store";
                _out.WriteLine("il.append(_factory.create" + kind + "(" + BCELifier.PrintType
                                   (type) + ", " + i.GetIndex() + "));");
            }
        }

        public override void VisitArrayInstruction(ArrayInstruction i)
        {
            var opcode = i.GetOpcode();
            var type = i.GetType(_cp);
            var kind = opcode < Const.IASTORE ? "Load" : "Store";
            _out.WriteLine("il.append(_factory.createArray" + kind + "(" + BCELifier
                               .PrintType(type) + "));");
        }

        public override void VisitFieldInstruction(FieldInstruction i)
        {
            var opcode = i.GetOpcode();
            var class_name = i.GetClassName(_cp);
            var field_name = i.GetFieldName(_cp);
            var type = i.GetFieldType(_cp);
            _out.WriteLine("il.append(_factory.createFieldAccess(\"" + class_name + "\", \"" +
                           field_name + "\", " + BCELifier.PrintType(type) + ", " + CONSTANT_PREFIX
                           + Const.GetOpcodeName(opcode).ToUpper() + "));");
        }

        public override void VisitInvokeInstruction(InvokeInstruction i)
        {
            var opcode = i.GetOpcode();
            var class_name = i.GetClassName(_cp);
            var method_name = i.GetMethodName(_cp);
            var type = i.GetReturnType(_cp);
            var arg_types = i.GetArgumentTypes(_cp);
            _out.WriteLine("il.append(_factory.createInvoke(\"" + class_name + "\", \"" + method_name
                           + "\", " + BCELifier.PrintType(type) + ", " + BCELifier.PrintArgumentTypes
                               (arg_types) + ", " + CONSTANT_PREFIX + Const.GetOpcodeName(opcode).ToUpper
                               () + "));");
        }

        public override void VisitAllocationInstruction(AllocationInstruction
            i)
        {
            Type type;
            if (i is CPInstruction)
                type = ((CPInstruction) i).GetType(_cp);
            else
                type = ((NEWARRAY) i).GetType();
            var opcode = ((Instruction) i).GetOpcode();
            var dim = 1;
            switch (opcode)
            {
                case Const.NEW:
                {
                    _out.WriteLine("il.append(_factory.createNew(\"" + ((ObjectType) type)
                                   .GetClassName() + "\"));");
                    break;
                }

                case Const.MULTIANEWARRAY:
                {
                    dim = ((MULTIANEWARRAY) i).GetDimensions();
                    goto case Const.ANEWARRAY;
                }

                case Const.ANEWARRAY:
                case Const.NEWARRAY:
                {
                    //$FALL-THROUGH$
                    if (type is ArrayType) type = ((ArrayType) type).GetBasicType();
                    _out.WriteLine("il.append(_factory.createNewArray(" + BCELifier.PrintType
                                       (type) + ", (short) " + dim + "));");
                    break;
                }

                default:
                {
                    throw new Exception("Oops: " + opcode);
                }
            }
        }

        private void CreateConstant(object value)
        {
            var embed = value.ToString();
            if (value is string)
            {
                embed = '"' + Utility.ConvertString(embed) + '"';
            }
            else if (value is char)
            {
                embed = "(char)0x" + Runtime.ToHexString((char) value);
            }
            else if (value is float)
            {
                embed += "f";
            }
            else if (value is long)
            {
                embed += "L";
            }
            else if (value is ObjectType)
            {
                var ot = (ObjectType) value;
                embed = "new ObjectType(\"" + ot.GetClassName() + "\")";
            }

            _out.WriteLine("il.append(new PUSH(_cp, " + embed + "));");
        }

        public override void VisitLDC(LDC i)
        {
            CreateConstant(i.GetValue(_cp));
        }

        public override void VisitLDC2_W(LDC2_W i)
        {
            CreateConstant(i.GetValue(_cp));
        }

        public override void VisitConstantPushInstruction<T>(ConstantPushInstruction<T>
            i)
        {
            CreateConstant(i.GetValue());
        }

        public override void VisitINSTANCEOF(INSTANCEOF i)
        {
            var type = i.GetType(_cp);
            _out.WriteLine("il.append(new INSTANCEOF(_cp.addClass(" + BCELifier.PrintType
                               (type) + ")));");
        }

        public override void VisitCHECKCAST(CHECKCAST i)
        {
            var type = i.GetType(_cp);
            _out.WriteLine("il.append(_factory.createCheckCast(" + BCELifier.PrintType
                               (type) + "));");
        }

        public override void VisitReturnInstruction(ReturnInstruction i)
        {
            var type = i.GetType(_cp);
            _out.WriteLine("il.append(_factory.createReturn(" + BCELifier.PrintType(
                               type) + "));");
        }

        // Memorize BranchInstructions that need an update
        public override void VisitBranchInstruction(BranchInstruction bi)
        {
            var bh = (BranchHandle) branch_map.GetOrNull(
                bi);
            var pos = bh.GetPosition();
            var name = bi.GetName() + "_" + pos;
            if (bi is Select)
            {
                var s = (Select) bi;
                branches.Add(bi);
                var args = new StringBuilder("new int[] { ");
                var matchs = s.GetMatchs();
                for (var i = 0; i < matchs.Length; i++)
                {
                    args.Append(matchs[i]);
                    if (i < matchs.Length - 1) args.Append(", ");
                }

                args.Append(" }");
                _out.Write("Select " + name + " = new " + bi.GetName().ToUpper(
                           ) + "(" + args + ", new InstructionHandle[] { ");
                for (var i = 0; i < matchs.Length; i++)
                {
                    _out.Write("null");
                    if (i < matchs.Length - 1) _out.Write(", ");
                }

                _out.WriteLine(" }, null);");
            }
            else
            {
                var t_pos = bh.GetTarget().GetPosition();
                string target;
                if (pos > t_pos)
                {
                    target = "ih_" + t_pos;
                }
                else
                {
                    branches.Add(bi);
                    target = "null";
                }

                _out.WriteLine("    BranchInstruction " + name + " = _factory.createBranchInstruction("
                               + CONSTANT_PREFIX + bi.GetName().ToUpper() + ", " + target
                               + ");");
            }

            if (bh.HasTargeters())
                _out.WriteLine("    ih_" + pos + " = il.append(" + name + ");");
            else
                _out.WriteLine("    il.append(" + name + ");");
        }

        public override void VisitRET(RET i)
        {
            _out.WriteLine("il.append(new RET(" + i.GetIndex() + ")));");
        }

        private void UpdateBranchTargets()
        {
            foreach (var bi in branches)
            {
                var bh = (BranchHandle) branch_map.GetOrNull(
                    bi);
                var pos = bh.GetPosition();
                var name = bi.GetName() + "_" + pos;
                var t_pos = bh.GetTarget().GetPosition();
                _out.WriteLine("    " + name + ".setTarget(ih_" + t_pos + ");");
                if (bi is Select)
                {
                    var ihs = ((Select) bi).GetTargets();
                    for (var j = 0; j < ihs.Length; j++)
                    {
                        t_pos = ihs[j].GetPosition();
                        _out.WriteLine("    " + name + ".setTarget(" + j + ", ih_" + t_pos + ");");
                    }
                }
            }
        }

        private void UpdateExceptionHandlers()
        {
            var handlers = _mg.GetExceptionHandlers();
            foreach (var h in handlers)
            {
                var type = h.GetCatchType() == null
                    ? "null"
                    : BCELifier.PrintType
                        (h.GetCatchType());
                _out.WriteLine("    method.addExceptionHandler(" + "ih_" + h.GetStartPC().GetPosition
                                   () + ", " + "ih_" + h.GetEndPC().GetPosition() + ", " + "ih_" + h.GetHandlerPC()
                                   .GetPosition() + ", " + type + ");");
            }
        }
    }
}