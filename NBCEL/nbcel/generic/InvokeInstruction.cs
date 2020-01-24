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

using System.Linq;
using Sharpen;

namespace NBCEL.generic
{
	/// <summary>Super class for the INVOKExxx family of instructions.</summary>
	public abstract class InvokeInstruction : NBCEL.generic.FieldOrMethod, NBCEL.generic.ExceptionThrower
		, NBCEL.generic.StackConsumer, NBCEL.generic.StackProducer
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal InvokeInstruction()
		{
		}

		/// <param name="index">to constant pool</param>
		protected internal InvokeInstruction(short opcode, int index)
			: base(opcode, index)
		{
		}

		/// <returns>mnemonic for instruction with symbolic references resolved</returns>
		public override string ToString(NBCEL.classfile.ConstantPool cp)
		{
			NBCEL.classfile.Constant c = cp.GetConstant(base.GetIndex());
			var tok = cp.ConstantToString(c).Split('\t', '\n', '\r', '\f');
			string opcodeName = NBCEL.Const.GetOpcodeName(base.GetOpcode());
			System.Text.StringBuilder sb = new System.Text.StringBuilder(opcodeName);
			if (tok.ElementAtOrDefault(0) != null)
			{
				sb.Append(" ");
				sb.Append(tok.ElementAtOrDefault(0)?.Replace('.', '/'));
				if (tok.ElementAtOrDefault(1) != null)
				{
					sb.Append(tok.ElementAtOrDefault(1));
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Also works for instructions whose stack effect depends on the
		/// constant pool entry they reference.
		/// </summary>
		/// <returns>Number of words consumed from stack by this instruction</returns>
		public override int ConsumeStack(NBCEL.generic.ConstantPoolGen cpg)
		{
			int sum;
			if ((base.GetOpcode() == NBCEL.Const.INVOKESTATIC) || (base.GetOpcode() == NBCEL.Const
				.INVOKEDYNAMIC))
			{
				sum = 0;
			}
			else
			{
				sum = 1;
			}
			// this reference
			string signature = GetSignature(cpg);
			sum += NBCEL.generic.Type.GetArgumentTypesSize(signature);
			return sum;
		}

		/// <summary>
		/// Also works for instructions whose stack effect depends on the
		/// constant pool entry they reference.
		/// </summary>
		/// <returns>Number of words produced onto stack by this instruction</returns>
		public override int ProduceStack(NBCEL.generic.ConstantPoolGen cpg)
		{
			string signature = GetSignature(cpg);
			return NBCEL.generic.Type.GetReturnTypeSize(signature);
		}

		/// <summary>
		/// This overrides the deprecated version as we know here that the referenced class
		/// may legally be an array.
		/// </summary>
		/// <returns>name of the referenced class/interface</returns>
		/// <exception cref="System.ArgumentException">if the referenced class is an array (this should not happen)
		/// 	</exception>
		public override string GetClassName(NBCEL.generic.ConstantPoolGen cpg)
		{
			NBCEL.classfile.ConstantPool cp = cpg.GetConstantPool();
			NBCEL.classfile.ConstantCP cmr = (NBCEL.classfile.ConstantCP)cp.GetConstant(base.
				GetIndex());
			string className = cp.GetConstantString(cmr.GetClassIndex(), NBCEL.Const.CONSTANT_Class
				);
			return className.Replace('/', '.');
		}

		/// <returns>return type of referenced method.</returns>
		public override NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cpg)
		{
			return GetReturnType(cpg);
		}

		/// <returns>name of referenced method.</returns>
		public virtual string GetMethodName(NBCEL.generic.ConstantPoolGen cpg)
		{
			return GetName(cpg);
		}

		/// <returns>return type of referenced method.</returns>
		public virtual NBCEL.generic.Type GetReturnType(NBCEL.generic.ConstantPoolGen cpg
			)
		{
			return NBCEL.generic.Type.GetReturnType(GetSignature(cpg));
		}

		/// <returns>argument types of referenced method.</returns>
		public virtual NBCEL.generic.Type[] GetArgumentTypes(NBCEL.generic.ConstantPoolGen
			 cpg)
		{
			return NBCEL.generic.Type.GetArgumentTypes(GetSignature(cpg));
		}

		public abstract System.Type[] GetExceptions();
	}
}
