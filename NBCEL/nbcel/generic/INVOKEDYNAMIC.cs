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
	/// <summary>Class for INVOKEDYNAMIC.</summary>
	/// <remarks>
	/// Class for INVOKEDYNAMIC. Not an instance of InvokeInstruction, since that class
	/// expects to be able to get the class of the method. Ignores the bootstrap
	/// mechanism entirely.
	/// </remarks>
	/// <seealso>
	/// * <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5.invokedynamic">
	/// * The invokedynamic instruction in The Java Virtual Machine Specification</a></seealso>
	/// <since>6.0</since>
	public class INVOKEDYNAMIC : NBCEL.generic.InvokeInstruction
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal INVOKEDYNAMIC()
		{
		}

		public INVOKEDYNAMIC(int index)
			: base(NBCEL.Const.INVOKEDYNAMIC, index)
		{
		}

		/// <summary>Dump instruction as byte code to stream out.</summary>
		/// <param name="out">Output stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream @out)
		{
			@out.WriteByte(base.GetOpcode());
			@out.WriteShort(base.GetIndex());
			@out.WriteByte(0);
			@out.WriteByte(0);
		}

		/// <summary>Read needed data (i.e., index) from file.</summary>
		/// <exception cref="System.IO.IOException"/>
		protected internal override void InitFromFile(NBCEL.util.ByteSequence bytes, bool
			 wide)
		{
			base.InitFromFile(bytes, wide);
			base.SetLength(5);
			bytes.ReadByte();
			// Skip 0 byte
			bytes.ReadByte();
		}

		// Skip 0 byte
		/// <returns>mnemonic for instruction with symbolic references resolved</returns>
		public override string ToString(NBCEL.classfile.ConstantPool cp)
		{
			return base.ToString(cp);
		}

		public override System.Type[] GetExceptions()
		{
			return NBCEL.ExceptionConst.CreateExceptions(NBCEL.ExceptionConst.EXCS.EXCS_INTERFACE_METHOD_RESOLUTION
				, NBCEL.ExceptionConst.UNSATISFIED_LINK_ERROR, NBCEL.ExceptionConst.ABSTRACT_METHOD_ERROR
				, NBCEL.ExceptionConst.ILLEGAL_ACCESS_ERROR, NBCEL.ExceptionConst.INCOMPATIBLE_CLASS_CHANGE_ERROR
				);
		}

		/// <summary>Call corresponding visitor method(s).</summary>
		/// <remarks>
		/// Call corresponding visitor method(s). The order is:
		/// Call visitor methods of implemented interfaces first, then
		/// call methods according to the class hierarchy in descending order,
		/// i.e., the most specific visitXXX() call comes last.
		/// </remarks>
		/// <param name="v">Visitor object</param>
		public override void Accept(NBCEL.generic.Visitor v)
		{
			v.VisitExceptionThrower(this);
			v.VisitTypedInstruction(this);
			v.VisitStackConsumer(this);
			v.VisitStackProducer(this);
			v.VisitLoadClass(this);
			v.VisitCPInstruction(this);
			v.VisitFieldOrMethod(this);
			v.VisitInvokeInstruction(this);
			v.VisitINVOKEDYNAMIC(this);
		}

		/// <summary>Override the parent method because our classname is held elsewhere.</summary>
		public override string GetClassName(NBCEL.generic.ConstantPoolGen cpg)
		{
			NBCEL.classfile.ConstantPool cp = cpg.GetConstantPool();
			NBCEL.classfile.ConstantInvokeDynamic cid = (NBCEL.classfile.ConstantInvokeDynamic
				)cp.GetConstant(base.GetIndex(), NBCEL.Const.CONSTANT_InvokeDynamic);
			return ((NBCEL.classfile.ConstantNameAndType)cp.GetConstant(cid.GetNameAndTypeIndex
				())).GetName(cp);
		}

		/// <summary>
		/// Since InvokeDynamic doesn't refer to a reference type, just return java.lang.Object,
		/// as that is the only type we can say for sure the reference will be.
		/// </summary>
		/// <param name="cpg">the ConstantPoolGen used to create the instruction</param>
		/// <returns>an ObjectType for java.lang.Object</returns>
		/// <since>6.1</since>
		public override NBCEL.generic.ReferenceType GetReferenceType(NBCEL.generic.ConstantPoolGen
			 cpg)
		{
			return new NBCEL.generic.ObjectType(typeof(object).FullName);
		}
	}
}
