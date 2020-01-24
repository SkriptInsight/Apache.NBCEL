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

using NBCEL.classfile;

namespace NBCEL.generic
{
	/// <summary>
	///     Super class for FieldOrMethod and INVOKEDYNAMIC, since they both have
	///     names and signatures
	/// </summary>
	/// <since>6.0</since>
	public abstract class NameSignatureInstruction : CPInstruction
    {
        public NameSignatureInstruction()
        {
        }

        public NameSignatureInstruction(short opcode, int index)
            : base(opcode, index)
        {
        }

        public virtual ConstantNameAndType GetNameAndType(ConstantPoolGen
            cpg)
        {
            var cp = cpg.GetConstantPool();
            var cmr = (ConstantCP) cp.GetConstant(GetIndex());
            return (ConstantNameAndType) cp.GetConstant(cmr.GetNameAndTypeIndex
                ());
        }

        /// <returns>signature of referenced method/field.</returns>
        public virtual string GetSignature(ConstantPoolGen cpg)
        {
            var cp = cpg.GetConstantPool();
            var cnat = GetNameAndType(cpg);
            return ((ConstantUtf8) cp.GetConstant(cnat.GetSignatureIndex())).GetBytes
                ();
        }

        /// <returns>name of referenced method/field.</returns>
        public virtual string GetName(ConstantPoolGen cpg)
        {
            var cp = cpg.GetConstantPool();
            var cnat = GetNameAndType(cpg);
            return ((ConstantUtf8) cp.GetConstant(cnat.GetNameIndex())).GetBytes
                ();
        }
    }
}