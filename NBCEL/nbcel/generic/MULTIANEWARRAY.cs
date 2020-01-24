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
	/// MULTIANEWARRAY - Create new mutidimensional array of references
	/// <PRE>Stack: ..., count1, [count2, ...] -&gt; ..., arrayref</PRE>
	/// </summary>
	public class MULTIANEWARRAY : NBCEL.generic.CPInstruction, NBCEL.generic.LoadClass
		, NBCEL.generic.AllocationInstruction, NBCEL.generic.ExceptionThrower
	{
		private short dimensions;

		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal MULTIANEWARRAY()
		{
		}

		public MULTIANEWARRAY(int index, short dimensions)
			: base(NBCEL.Const.MULTIANEWARRAY, index)
		{
			if (dimensions < 1)
			{
				throw new NBCEL.generic.ClassGenException("Invalid dimensions value: " + dimensions
					);
			}
			this.dimensions = dimensions;
			base.SetLength(4);
		}

		/// <summary>Dump instruction as byte code to stream out.</summary>
		/// <param name="out">Output stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream @out)
		{
			@out.WriteByte(base.GetOpcode());
			@out.WriteShort(base.GetIndex());
			@out.WriteByte(dimensions);
		}

		/// <summary>Read needed data (i.e., no.</summary>
		/// <remarks>Read needed data (i.e., no. dimension) from file.</remarks>
		/// <exception cref="System.IO.IOException"/>
		protected internal override void InitFromFile(NBCEL.util.ByteSequence bytes, bool
			 wide)
		{
			base.InitFromFile(bytes, wide);
			dimensions = bytes.ReadByte();
			base.SetLength(4);
		}

		/// <returns>number of dimensions to be created</returns>
		public short GetDimensions()
		{
			return dimensions;
		}

		/// <returns>mnemonic for instruction</returns>
		public override string ToString(bool verbose)
		{
			return base.ToString(verbose) + " " + base.GetIndex() + " " + dimensions;
		}

		/// <returns>mnemonic for instruction with symbolic references resolved</returns>
		public override string ToString(NBCEL.classfile.ConstantPool cp)
		{
			return base.ToString(cp) + " " + dimensions;
		}

		/// <summary>
		/// Also works for instructions whose stack effect depends on the
		/// constant pool entry they reference.
		/// </summary>
		/// <returns>Number of words consumed from stack by this instruction</returns>
		public override int ConsumeStack(NBCEL.generic.ConstantPoolGen cpg)
		{
			return dimensions;
		}

		public virtual System.Type[] GetExceptions()
		{
			return NBCEL.ExceptionConst.CreateExceptions(NBCEL.ExceptionConst.EXCS.EXCS_CLASS_AND_INTERFACE_RESOLUTION
				, NBCEL.ExceptionConst.ILLEGAL_ACCESS_ERROR, NBCEL.ExceptionConst.NEGATIVE_ARRAY_SIZE_EXCEPTION
				);
		}

		public virtual NBCEL.generic.ObjectType GetLoadClassType(NBCEL.generic.ConstantPoolGen
			 cpg)
		{
			NBCEL.generic.Type t = GetType(cpg);
			if (t is NBCEL.generic.ArrayType)
			{
				t = ((NBCEL.generic.ArrayType)t).GetBasicType();
			}
			return (t is NBCEL.generic.ObjectType) ? (NBCEL.generic.ObjectType)t : null;
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
			v.VisitLoadClass(this);
			v.VisitAllocationInstruction(this);
			v.VisitExceptionThrower(this);
			v.VisitTypedInstruction(this);
			v.VisitCPInstruction(this);
			v.VisitMULTIANEWARRAY(this);
		}
	}
}
