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

namespace NBCEL.generic
{
	/// <summary>Super class for the GET/PUTxxx family of instructions.</summary>
	public abstract class FieldInstruction : NBCEL.generic.FieldOrMethod
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal FieldInstruction()
		{
		}

		/// <param name="index">to constant pool</param>
		protected internal FieldInstruction(short opcode, int index)
			: base(opcode, index)
		{
		}

		/// <returns>mnemonic for instruction with symbolic references resolved</returns>
		public override string ToString(NBCEL.classfile.ConstantPool cp)
		{
			return NBCEL.Const.GetOpcodeName(base.GetOpcode()) + " " + cp.ConstantToString(base
				.GetIndex(), NBCEL.Const.CONSTANT_Fieldref);
		}

		/// <returns>size of field (1 or 2)</returns>
		protected internal virtual int GetFieldSize(NBCEL.generic.ConstantPoolGen cpg)
		{
			return NBCEL.generic.Type.Size(NBCEL.generic.Type.GetTypeSize(GetSignature(cpg)));
		}

		/// <returns>return type of referenced field</returns>
		public override NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cpg)
		{
			return GetFieldType(cpg);
		}

		/// <returns>type of field</returns>
		public virtual NBCEL.generic.Type GetFieldType(NBCEL.generic.ConstantPoolGen cpg)
		{
			return NBCEL.generic.Type.GetType(GetSignature(cpg));
		}

		/// <returns>name of referenced field.</returns>
		public virtual string GetFieldName(NBCEL.generic.ConstantPoolGen cpg)
		{
			return GetName(cpg);
		}
	}
}