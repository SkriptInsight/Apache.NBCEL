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

namespace NBCEL.classfile
{
	/// <summary>Entry of the parameters table.</summary>
	/// <seealso><a href="http://docs.oracle.com/javase/specs/jvms/se8/html/jvms-4.html#jvms-4.7.24">
	/// * The class File Format : The MethodParameters Attribute</a></seealso>
	/// <since>6.0</since>
	public class MethodParameter : System.ICloneable
	{
		/// <summary>Index of the CONSTANT_Utf8_info structure in the constant_pool table representing the name of the parameter
		/// 	</summary>
		private int name_index;

		/// <summary>The access flags</summary>
		private int access_flags;

		public MethodParameter()
		{
		}

		/// <summary>Construct object from input stream.</summary>
		/// <param name="input">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		internal MethodParameter(java.io.DataInput input)
		{
			name_index = input.ReadUnsignedShort();
			access_flags = input.ReadUnsignedShort();
		}

		public virtual int GetNameIndex()
		{
			return name_index;
		}

		public virtual void SetNameIndex(int name_index)
		{
			this.name_index = name_index;
		}

		/// <summary>Returns the name of the parameter.</summary>
		public virtual string GetParameterName(NBCEL.classfile.ConstantPool constant_pool
			)
		{
			if (name_index == 0)
			{
				return null;
			}
			return ((NBCEL.classfile.ConstantUtf8)constant_pool.GetConstant(name_index, NBCEL.Const
				.CONSTANT_Utf8)).GetBytes();
		}

		public virtual int GetAccessFlags()
		{
			return access_flags;
		}

		public virtual void SetAccessFlags(int access_flags)
		{
			this.access_flags = access_flags;
		}

		public virtual bool IsFinal()
		{
			return (access_flags & NBCEL.Const.ACC_FINAL) != 0;
		}

		public virtual bool IsSynthetic()
		{
			return (access_flags & NBCEL.Const.ACC_SYNTHETIC) != 0;
		}

		public virtual bool IsMandated()
		{
			return (access_flags & NBCEL.Const.ACC_MANDATED) != 0;
		}

		public virtual void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitMethodParameter(this);
		}

		/// <summary>Dump object to file stream on binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public void Dump(java.io.DataOutputStream file)
		{
			file.WriteShort(name_index);
			file.WriteShort(access_flags);
		}

		/// <returns>deep copy of this object</returns>
		public virtual NBCEL.classfile.MethodParameter Copy()
		{
			return (NBCEL.classfile.MethodParameter)MemberwiseClone();
			// TODO should this throw?
			return null;
		}

		object System.ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
