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

using System.IO;
using Sharpen;

namespace NBCEL.util
{
	/// <summary>Factory creates il.append() statements, and sets instruction targets.</summary>
	/// <remarks>
	/// Factory creates il.append() statements, and sets instruction targets.
	/// A helper class for BCELifier.
	/// </remarks>
	/// <seealso cref="BCELifier"/>
	internal class BCELFactory : NBCEL.generic.EmptyVisitor
	{
		private static readonly string CONSTANT_PREFIX = typeof(NBCEL.Const).GetSimpleName
			() + ".";

		private readonly NBCEL.generic.MethodGen _mg;

		private readonly TextWriter _out;

		private readonly NBCEL.generic.ConstantPoolGen _cp;

		internal BCELFactory(NBCEL.generic.MethodGen mg, TextWriter @out)
		{
			_mg = mg;
			_cp = mg.GetConstantPool();
			_out = @out;
		}

		private readonly System.Collections.Generic.IDictionary<NBCEL.generic.Instruction
			, NBCEL.generic.InstructionHandle> branch_map = new System.Collections.Generic.Dictionary
			<NBCEL.generic.Instruction, NBCEL.generic.InstructionHandle>();

		public virtual void Start()
		{
			if (!_mg.IsAbstract() && !_mg.IsNative())
			{
				for (NBCEL.generic.InstructionHandle ih = _mg.GetInstructionList().GetStart(); ih
					 != null; ih = ih.GetNext())
				{
					NBCEL.generic.Instruction i = ih.GetInstruction();
					if (i is NBCEL.generic.BranchInstruction)
					{
						Sharpen.Collections.Put(branch_map, i, ih);
					}
					// memorize container
					if (ih.HasTargeters())
					{
						if (i is NBCEL.generic.BranchInstruction)
						{
							_out.WriteLine("    InstructionHandle ih_" + ih.GetPosition() + ";");
						}
						else
						{
							_out.Write("    InstructionHandle ih_" + ih.GetPosition() + " = ");
						}
					}
					else
					{
						_out.Write("    ");
					}
					if (!VisitInstruction(i))
					{
						i.Accept(this);
					}
				}
				UpdateBranchTargets();
				UpdateExceptionHandlers();
			}
		}

		private bool VisitInstruction(NBCEL.generic.Instruction i)
		{
			short opcode = i.GetOpcode();
			if ((NBCEL.generic.InstructionConst.GetInstruction(opcode) != null) && !(i is NBCEL.generic.BaseConstantPushInstruction
				) && !(i is NBCEL.generic.ReturnInstruction))
			{
				// Handled below
				_out.WriteLine("il.append(InstructionConst." + i.GetName().ToUpper() + ");");
				return true;
			}
			return false;
		}

		public override void VisitLocalVariableInstruction(NBCEL.generic.LocalVariableInstruction
			 i)
		{
			short opcode = i.GetOpcode();
			NBCEL.generic.Type type = i.GetType(_cp);
			if (opcode == NBCEL.Const.IINC)
			{
				_out.WriteLine("il.append(new IINC(" + i.GetIndex() + ", " + ((NBCEL.generic.IINC)i
					).GetIncrement() + "));");
			}
			else
			{
				string kind = (opcode < NBCEL.Const.ISTORE) ? "Load" : "Store";
				_out.WriteLine("il.append(_factory.create" + kind + "(" + NBCEL.util.BCELifier.PrintType
					(type) + ", " + i.GetIndex() + "));");
			}
		}

		public override void VisitArrayInstruction(NBCEL.generic.ArrayInstruction i)
		{
			short opcode = i.GetOpcode();
			NBCEL.generic.Type type = i.GetType(_cp);
			string kind = (opcode < NBCEL.Const.IASTORE) ? "Load" : "Store";
			_out.WriteLine("il.append(_factory.createArray" + kind + "(" + NBCEL.util.BCELifier
				.PrintType(type) + "));");
		}

		public override void VisitFieldInstruction(NBCEL.generic.FieldInstruction i)
		{
			short opcode = i.GetOpcode();
			string class_name = i.GetClassName(_cp);
			string field_name = i.GetFieldName(_cp);
			NBCEL.generic.Type type = i.GetFieldType(_cp);
			_out.WriteLine("il.append(_factory.createFieldAccess(\"" + class_name + "\", \"" + 
				field_name + "\", " + NBCEL.util.BCELifier.PrintType(type) + ", " + CONSTANT_PREFIX
				 + NBCEL.Const.GetOpcodeName(opcode).ToUpper() + "));");
		}

		public override void VisitInvokeInstruction(NBCEL.generic.InvokeInstruction i)
		{
			short opcode = i.GetOpcode();
			string class_name = i.GetClassName(_cp);
			string method_name = i.GetMethodName(_cp);
			NBCEL.generic.Type type = i.GetReturnType(_cp);
			NBCEL.generic.Type[] arg_types = i.GetArgumentTypes(_cp);
			_out.WriteLine("il.append(_factory.createInvoke(\"" + class_name + "\", \"" + method_name
				 + "\", " + NBCEL.util.BCELifier.PrintType(type) + ", " + NBCEL.util.BCELifier.PrintArgumentTypes
				(arg_types) + ", " + CONSTANT_PREFIX + NBCEL.Const.GetOpcodeName(opcode).ToUpper
				() + "));");
		}

		public override void VisitAllocationInstruction(NBCEL.generic.AllocationInstruction
			 i)
		{
			NBCEL.generic.Type type;
			if (i is NBCEL.generic.CPInstruction)
			{
				type = ((NBCEL.generic.CPInstruction)i).GetType(_cp);
			}
			else
			{
				type = ((NBCEL.generic.NEWARRAY)i).GetType();
			}
			short opcode = ((NBCEL.generic.Instruction)i).GetOpcode();
			int dim = 1;
			switch (opcode)
			{
				case NBCEL.Const.NEW:
				{
					_out.WriteLine("il.append(_factory.createNew(\"" + ((NBCEL.generic.ObjectType)type)
						.GetClassName() + "\"));");
					break;
				}

				case NBCEL.Const.MULTIANEWARRAY:
				{
					dim = ((NBCEL.generic.MULTIANEWARRAY)i).GetDimensions();
					goto case NBCEL.Const.ANEWARRAY;
				}

				case NBCEL.Const.ANEWARRAY:
				case NBCEL.Const.NEWARRAY:
				{
					//$FALL-THROUGH$
					if (type is NBCEL.generic.ArrayType)
					{
						type = ((NBCEL.generic.ArrayType)type).GetBasicType();
					}
					_out.WriteLine("il.append(_factory.createNewArray(" + NBCEL.util.BCELifier.PrintType
						(type) + ", (short) " + dim + "));");
					break;
				}

				default:
				{
					throw new System.Exception("Oops: " + opcode);
				}
			}
		}

		private void CreateConstant(object value)
		{
			string embed = value.ToString();
			if (value is string)
			{
				embed = '"' + NBCEL.classfile.Utility.ConvertString(embed) + '"';
			}
			else if (value is char)
			{
				embed = "(char)0x" + Sharpen.Runtime.ToHexString(((char)value));
			}
			else if (value is float)
			{
				embed += "f";
			}
			else if (value is long)
			{
				embed += "L";
			}
			else if (value is NBCEL.generic.ObjectType)
			{
				NBCEL.generic.ObjectType ot = (NBCEL.generic.ObjectType)value;
				embed = "new ObjectType(\"" + ot.GetClassName() + "\")";
			}
			_out.WriteLine("il.append(new PUSH(_cp, " + embed + "));");
		}

		public override void VisitLDC(NBCEL.generic.LDC i)
		{
			CreateConstant(i.GetValue(_cp));
		}

		public override void VisitLDC2_W(NBCEL.generic.LDC2_W i)
		{
			CreateConstant(i.GetValue(_cp));
		}

		public override void VisitConstantPushInstruction<T>(NBCEL.generic.ConstantPushInstruction<T>
			 i)
		{
			CreateConstant(i.GetValue());
		}

		public override void VisitINSTANCEOF(NBCEL.generic.INSTANCEOF i)
		{
			NBCEL.generic.Type type = i.GetType(_cp);
			_out.WriteLine("il.append(new INSTANCEOF(_cp.addClass(" + NBCEL.util.BCELifier.PrintType
				(type) + ")));");
		}

		public override void VisitCHECKCAST(NBCEL.generic.CHECKCAST i)
		{
			NBCEL.generic.Type type = i.GetType(_cp);
			_out.WriteLine("il.append(_factory.createCheckCast(" + NBCEL.util.BCELifier.PrintType
				(type) + "));");
		}

		public override void VisitReturnInstruction(NBCEL.generic.ReturnInstruction i)
		{
			NBCEL.generic.Type type = i.GetType(_cp);
			_out.WriteLine("il.append(_factory.createReturn(" + NBCEL.util.BCELifier.PrintType(
				type) + "));");
		}

		private readonly System.Collections.Generic.List<NBCEL.generic.BranchInstruction>
			 branches = new System.Collections.Generic.List<NBCEL.generic.BranchInstruction>
			();

		// Memorize BranchInstructions that need an update
		public override void VisitBranchInstruction(NBCEL.generic.BranchInstruction bi)
		{
			NBCEL.generic.BranchHandle bh = (NBCEL.generic.BranchHandle)branch_map.GetOrNull(
				bi);
			int pos = bh.GetPosition();
			string name = bi.GetName() + "_" + pos;
			if (bi is NBCEL.generic.Select)
			{
				NBCEL.generic.Select s = (NBCEL.generic.Select)bi;
				branches.Add(bi);
				System.Text.StringBuilder args = new System.Text.StringBuilder("new int[] { ");
				int[] matchs = s.GetMatchs();
				for (int i = 0; i < matchs.Length; i++)
				{
					args.Append(matchs[i]);
					if (i < matchs.Length - 1)
					{
						args.Append(", ");
					}
				}
				args.Append(" }");
				_out.Write("Select " + name + " = new " + bi.GetName().ToUpper(
					) + "(" + args + ", new InstructionHandle[] { ");
				for (int i = 0; i < matchs.Length; i++)
				{
					_out.Write("null");
					if (i < matchs.Length - 1)
					{
						_out.Write(", ");
					}
				}
				_out.WriteLine(" }, null);");
			}
			else
			{
				int t_pos = bh.GetTarget().GetPosition();
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
			{
				_out.WriteLine("    ih_" + pos + " = il.append(" + name + ");");
			}
			else
			{
				_out.WriteLine("    il.append(" + name + ");");
			}
		}

		public override void VisitRET(NBCEL.generic.RET i)
		{
			_out.WriteLine("il.append(new RET(" + i.GetIndex() + ")));");
		}

		private void UpdateBranchTargets()
		{
			foreach (NBCEL.generic.BranchInstruction bi in branches)
			{
				NBCEL.generic.BranchHandle bh = (NBCEL.generic.BranchHandle)branch_map.GetOrNull(
					bi);
				int pos = bh.GetPosition();
				string name = bi.GetName() + "_" + pos;
				int t_pos = bh.GetTarget().GetPosition();
				_out.WriteLine("    " + name + ".setTarget(ih_" + t_pos + ");");
				if (bi is NBCEL.generic.Select)
				{
					NBCEL.generic.InstructionHandle[] ihs = ((NBCEL.generic.Select)bi).GetTargets();
					for (int j = 0; j < ihs.Length; j++)
					{
						t_pos = ihs[j].GetPosition();
						_out.WriteLine("    " + name + ".setTarget(" + j + ", ih_" + t_pos + ");");
					}
				}
			}
		}

		private void UpdateExceptionHandlers()
		{
			NBCEL.generic.CodeExceptionGen[] handlers = _mg.GetExceptionHandlers();
			foreach (NBCEL.generic.CodeExceptionGen h in handlers)
			{
				string type = (h.GetCatchType() == null) ? "null" : NBCEL.util.BCELifier.PrintType
					(h.GetCatchType());
				_out.WriteLine("    method.addExceptionHandler(" + "ih_" + h.GetStartPC().GetPosition
					() + ", " + "ih_" + h.GetEndPC().GetPosition() + ", " + "ih_" + h.GetHandlerPC()
					.GetPosition() + ", " + type + ");");
			}
		}
	}
}
