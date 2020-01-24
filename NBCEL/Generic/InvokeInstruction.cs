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
using System.Text;
using Apache.NBCEL.ClassFile;

namespace Apache.NBCEL.Generic
{
    /// <summary>Super class for the INVOKExxx family of instructions.</summary>
    public abstract class InvokeInstruction : FieldOrMethod, ExceptionThrower
        , StackConsumer, StackProducer
    {
	    /// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
	    /// <remarks>
	    ///     Empty constructor needed for Instruction.readInstruction.
	    ///     Not to be used otherwise.
	    /// </remarks>
	    internal InvokeInstruction()
        {
        }

        /// <param name="index">to constant pool</param>
        protected internal InvokeInstruction(short opcode, int index)
            : base(opcode, index)
        {
        }

        public abstract global::System.Type[] GetExceptions();

        /// <summary>
        ///     Also works for instructions whose stack effect depends on the
        ///     constant pool entry they reference.
        /// </summary>
        /// <returns>Number of words consumed from stack by this instruction</returns>
        public override int ConsumeStack(ConstantPoolGen cpg)
        {
            int sum;
            if (base.GetOpcode() == Const.INVOKESTATIC || base.GetOpcode() == Const
                    .INVOKEDYNAMIC)
                sum = 0;
            else
                sum = 1;
            // this reference
            var signature = GetSignature(cpg);
            sum += Type.GetArgumentTypesSize(signature);
            return sum;
        }

        /// <summary>
        ///     Also works for instructions whose stack effect depends on the
        ///     constant pool entry they reference.
        /// </summary>
        /// <returns>Number of words produced onto stack by this instruction</returns>
        public override int ProduceStack(ConstantPoolGen cpg)
        {
            var signature = GetSignature(cpg);
            return Type.GetReturnTypeSize(signature);
        }

        /// <returns>mnemonic for instruction with symbolic references resolved</returns>
        public override string ToString(ConstantPool cp)
        {
            var c = cp.GetConstant(GetIndex());
            var tok = cp.ConstantToString(c).Split('\t', '\n', '\r', '\f');
            var opcodeName = Const.GetOpcodeName(base.GetOpcode());
            var sb = new StringBuilder(opcodeName);
            if (tok.ElementAtOrDefault(0) != null)
            {
                sb.Append(" ");
                sb.Append(tok.ElementAtOrDefault(0)?.Replace('.', '/'));
                if (tok.ElementAtOrDefault(1) != null) sb.Append(tok.ElementAtOrDefault(1));
            }

            return sb.ToString();
        }

        /// <summary>
        ///     This overrides the deprecated version as we know here that the referenced class
        ///     may legally be an array.
        /// </summary>
        /// <returns>name of the referenced class/interface</returns>
        /// <exception cref="System.ArgumentException">
        ///     if the referenced class is an array (this should not happen)
        /// </exception>
        public override string GetClassName(ConstantPoolGen cpg)
        {
            var cp = cpg.GetConstantPool();
            var cmr = (ConstantCP) cp.GetConstant(GetIndex());
            var className = cp.GetConstantString(cmr.GetClassIndex(), Const.CONSTANT_Class
            );
            return className.Replace('/', '.');
        }

        /// <returns>return type of referenced method.</returns>
        public override Type GetType(ConstantPoolGen cpg)
        {
            return GetReturnType(cpg);
        }

        /// <returns>name of referenced method.</returns>
        public virtual string GetMethodName(ConstantPoolGen cpg)
        {
            return GetName(cpg);
        }

        /// <returns>return type of referenced method.</returns>
        public virtual Type GetReturnType(ConstantPoolGen cpg
        )
        {
            return Type.GetReturnType(GetSignature(cpg));
        }

        /// <returns>argument types of referenced method.</returns>
        public virtual Type[] GetArgumentTypes(ConstantPoolGen
            cpg)
        {
            return Type.GetArgumentTypes(GetSignature(cpg));
        }
    }
}