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
	/// Abstract super class for instructions that use an index into the
	/// constant pool such as LDC, INVOKEVIRTUAL, etc.
	/// </summary>
	/// <seealso cref="ConstantPoolGen"/>
	/// <seealso cref="LDC"/>
	/// <seealso cref="INVOKEVIRTUAL"/>
	public abstract class CPInstruction : NBCEL.generic.Instruction, NBCEL.generic.TypedInstruction
		, NBCEL.generic.IndexedInstruction
	{
		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal int index;

		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal CPInstruction()
		{
		}

		/// <param name="index">to constant pool</param>
		protected internal CPInstruction(short opcode, int index)
			: base(opcode, (short)3)
		{
			// index to constant pool
			SetIndex(index);
		}

		/// <summary>Dump instruction as byte code to stream out.</summary>
		/// <param name="out">Output stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream @out)
		{
			@out.WriteByte(base.GetOpcode());
			@out.WriteShort(index);
		}

		/// <summary>
		/// Long output format:
		/// &lt;name of opcode&gt; "["&lt;opcode number&gt;"]"
		/// "("&lt;length of instruction&gt;")" "&lt;"&lt; constant pool index&gt;"&gt;"
		/// </summary>
		/// <param name="verbose">long/short format switch</param>
		/// <returns>mnemonic for instruction</returns>
		public override string ToString(bool verbose)
		{
			return base.ToString(verbose) + " " + index;
		}

		/// <returns>mnemonic for instruction with symbolic references resolved</returns>
		public override string ToString(NBCEL.classfile.ConstantPool cp)
		{
			NBCEL.classfile.Constant c = cp.GetConstant(index);
			string str = cp.ConstantToString(c);
			if (c is NBCEL.classfile.ConstantClass)
			{
				str = str.Replace('.', '/');
			}
			return NBCEL.Const.GetOpcodeName(base.GetOpcode()) + " " + str;
		}

		/// <summary>Read needed data (i.e., index) from file.</summary>
		/// <param name="bytes">input stream</param>
		/// <param name="wide">wide prefix?</param>
		/// <exception cref="System.IO.IOException"/>
		protected internal override void InitFromFile(NBCEL.util.ByteSequence bytes, bool
			 wide)
		{
			SetIndex(bytes.ReadUnsignedShort());
			base.SetLength(3);
		}

		/// <returns>index in constant pool referred by this instruction.</returns>
		public int GetIndex()
		{
			return index;
		}

		/// <summary>Set the index to constant pool.</summary>
		/// <param name="index">in  constant pool.</param>
		public virtual void SetIndex(int index)
		{
			// TODO could be package-protected?
			if (index < 0)
			{
				throw new NBCEL.generic.ClassGenException("Negative index value: " + index);
			}
			this.index = index;
		}

		/// <returns>type related with this instruction.</returns>
		public virtual NBCEL.generic.Type GetType(NBCEL.generic.ConstantPoolGen cpg)
		{
			NBCEL.classfile.ConstantPool cp = cpg.GetConstantPool();
			string name = cp.GetConstantString(index, NBCEL.Const.CONSTANT_Class);
			if (!name.StartsWith("["))
			{
				name = "L" + name + ";";
			}
			return NBCEL.generic.Type.GetType(name);
		}
	}
}
