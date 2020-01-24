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
	/// <summary>
	/// INVOKEINTERFACE - Invoke interface method
	/// <PRE>Stack: ..., objectref, [arg1, [arg2 ...]] -&gt; ...</PRE>
	/// </summary>
	/// <seealso>
	/// * <a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-6.html#jvms-6.5.invokeinterface">
	/// * The invokeinterface instruction in The Java Virtual Machine Specification</a></seealso>
	public sealed class INVOKEINTERFACE : NBCEL.generic.InvokeInstruction
	{
		private int nargs;

		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal INVOKEINTERFACE()
		{
		}

		public INVOKEINTERFACE(int index, int nargs)
			: base(NBCEL.Const.INVOKEINTERFACE, index)
		{
			// Number of arguments on stack (number of stack slots), called "count" in vmspec2
			base.SetLength(5);
			if (nargs < 1)
			{
				throw new NBCEL.generic.ClassGenException("Number of arguments must be > 0 " + nargs
					);
			}
			this.nargs = nargs;
		}

		/// <summary>Dump instruction as byte code to stream out.</summary>
		/// <param name="out">Output stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream @out)
		{
			@out.WriteByte(base.GetOpcode());
			@out.WriteShort(base.GetIndex());
			@out.WriteByte(nargs);
			@out.WriteByte(0);
		}

		/// <summary>
		/// The <B>count</B> argument according to the Java Language Specification,
		/// Second Edition.
		/// </summary>
		public int GetCount()
		{
			return nargs;
		}

		/// <summary>Read needed data (i.e., index) from file.</summary>
		/// <exception cref="System.IO.IOException"/>
		protected internal override void InitFromFile(NBCEL.util.ByteSequence bytes, bool
			 wide)
		{
			base.InitFromFile(bytes, wide);
			base.SetLength(5);
			nargs = bytes.ReadUnsignedByte();
			bytes.ReadByte();
		}

		// Skip 0 byte
		/// <returns>mnemonic for instruction with symbolic references resolved</returns>
		public override string ToString(NBCEL.classfile.ConstantPool cp)
		{
			return base.ToString(cp) + " " + nargs;
		}

		public override int ConsumeStack(NBCEL.generic.ConstantPoolGen cpg)
		{
			// nargs is given in byte-code
			return nargs;
		}

		// nargs includes this reference
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
			v.VisitINVOKEINTERFACE(this);
		}
	}
}
