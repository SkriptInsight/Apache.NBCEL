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
*/
using Sharpen;

namespace NBCEL.generic
{
	/// <summary>
	/// Super class for FieldOrMethod and INVOKEDYNAMIC, since they both have
	/// names and signatures
	/// </summary>
	/// <since>6.0</since>
	public abstract class NameSignatureInstruction : NBCEL.generic.CPInstruction
	{
		public NameSignatureInstruction()
			: base()
		{
		}

		public NameSignatureInstruction(short opcode, int index)
			: base(opcode, index)
		{
		}

		public virtual NBCEL.classfile.ConstantNameAndType GetNameAndType(NBCEL.generic.ConstantPoolGen
			 cpg)
		{
			NBCEL.classfile.ConstantPool cp = cpg.GetConstantPool();
			NBCEL.classfile.ConstantCP cmr = (NBCEL.classfile.ConstantCP)cp.GetConstant(base.
				GetIndex());
			return (NBCEL.classfile.ConstantNameAndType)cp.GetConstant(cmr.GetNameAndTypeIndex
				());
		}

		/// <returns>signature of referenced method/field.</returns>
		public virtual string GetSignature(NBCEL.generic.ConstantPoolGen cpg)
		{
			NBCEL.classfile.ConstantPool cp = cpg.GetConstantPool();
			NBCEL.classfile.ConstantNameAndType cnat = GetNameAndType(cpg);
			return ((NBCEL.classfile.ConstantUtf8)cp.GetConstant(cnat.GetSignatureIndex())).GetBytes
				();
		}

		/// <returns>name of referenced method/field.</returns>
		public virtual string GetName(NBCEL.generic.ConstantPoolGen cpg)
		{
			NBCEL.classfile.ConstantPool cp = cpg.GetConstantPool();
			NBCEL.classfile.ConstantNameAndType cnat = GetNameAndType(cpg);
			return ((NBCEL.classfile.ConstantUtf8)cp.GetConstant(cnat.GetNameIndex())).GetBytes
				();
		}
	}
}
