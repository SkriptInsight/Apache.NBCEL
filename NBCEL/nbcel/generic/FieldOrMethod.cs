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
	/// Super class for InvokeInstruction and FieldInstruction, since they have
	/// some methods in common!
	/// </summary>
	public abstract class FieldOrMethod : NBCEL.generic.CPInstruction, NBCEL.generic.LoadClass
	{
		/// <summary>Empty constructor needed for Instruction.readInstruction.</summary>
		/// <remarks>
		/// Empty constructor needed for Instruction.readInstruction.
		/// Not to be used otherwise.
		/// </remarks>
		internal FieldOrMethod()
		{
		}

		/// <param name="index">to constant pool</param>
		protected internal FieldOrMethod(short opcode, int index)
			: base(opcode, index)
		{
		}

		// no init
		/// <returns>signature of referenced method/field.</returns>
		public virtual string GetSignature(NBCEL.generic.ConstantPoolGen cpg)
		{
			NBCEL.classfile.ConstantPool cp = cpg.GetConstantPool();
			NBCEL.classfile.ConstantCP cmr = (NBCEL.classfile.ConstantCP)cp.GetConstant(base.
				GetIndex());
			NBCEL.classfile.ConstantNameAndType cnat = (NBCEL.classfile.ConstantNameAndType)cp
				.GetConstant(cmr.GetNameAndTypeIndex());
			return ((NBCEL.classfile.ConstantUtf8)cp.GetConstant(cnat.GetSignatureIndex())).GetBytes
				();
		}

		/// <returns>name of referenced method/field.</returns>
		public virtual string GetName(NBCEL.generic.ConstantPoolGen cpg)
		{
			NBCEL.classfile.ConstantPool cp = cpg.GetConstantPool();
			NBCEL.classfile.ConstantCP cmr = (NBCEL.classfile.ConstantCP)cp.GetConstant(base.
				GetIndex());
			NBCEL.classfile.ConstantNameAndType cnat = (NBCEL.classfile.ConstantNameAndType)cp
				.GetConstant(cmr.GetNameAndTypeIndex());
			return ((NBCEL.classfile.ConstantUtf8)cp.GetConstant(cnat.GetNameIndex())).GetBytes
				();
		}

		/// <returns>name of the referenced class/interface</returns>
		[System.ObsoleteAttribute(@"If the instruction references an array class, this method will return ""java.lang.Object"". For code generated by Java 1.5, this answer is sometimes wrong (e.g., if the ""clone()"" method is called on an array).  A better idea is to use the GetReferenceType(ConstantPoolGen) method, which correctly distinguishes between class types and array types."
			)]
		public virtual string GetClassName(NBCEL.generic.ConstantPoolGen cpg)
		{
			NBCEL.classfile.ConstantPool cp = cpg.GetConstantPool();
			NBCEL.classfile.ConstantCP cmr = (NBCEL.classfile.ConstantCP)cp.GetConstant(base.
				GetIndex());
			string className = cp.GetConstantString(cmr.GetClassIndex(), NBCEL.Const.CONSTANT_Class
				);
			if (className.StartsWith("["))
			{
				// Turn array classes into java.lang.Object.
				return "java.lang.Object";
			}
			return className.Replace('/', '.');
		}

		/// <returns>type of the referenced class/interface</returns>
		[System.ObsoleteAttribute(@"If the instruction references an array class, the ObjectType returned will be invalid.  Use getReferenceType() instead."
			)]
		public virtual NBCEL.generic.ObjectType GetClassType(NBCEL.generic.ConstantPoolGen
			 cpg)
		{
			return NBCEL.generic.ObjectType.GetInstance(GetClassName(cpg));
		}

		/// <summary>
		/// Gets the reference type representing the class, interface,
		/// or array class referenced by the instruction.
		/// </summary>
		/// <param name="cpg">the ConstantPoolGen used to create the instruction</param>
		/// <returns>
		/// an ObjectType (if the referenced class type is a class
		/// or interface), or an ArrayType (if the referenced class
		/// type is an array class)
		/// </returns>
		public virtual NBCEL.generic.ReferenceType GetReferenceType(NBCEL.generic.ConstantPoolGen
			 cpg)
		{
			NBCEL.classfile.ConstantPool cp = cpg.GetConstantPool();
			NBCEL.classfile.ConstantCP cmr = (NBCEL.classfile.ConstantCP)cp.GetConstant(base.
				GetIndex());
			string className = cp.GetConstantString(cmr.GetClassIndex(), NBCEL.Const.CONSTANT_Class
				);
			if (className.StartsWith("["))
			{
				return (NBCEL.generic.ArrayType)NBCEL.generic.Type.GetType(className);
			}
			className = className.Replace('/', '.');
			return NBCEL.generic.ObjectType.GetInstance(className);
		}

		/// <summary>Gets the ObjectType of the method return or field.</summary>
		/// <returns>type of the referenced class/interface</returns>
		/// <exception cref="ClassGenException">when the field is (or method returns) an array,
		/// 	</exception>
		public virtual NBCEL.generic.ObjectType GetLoadClassType(NBCEL.generic.ConstantPoolGen
			 cpg)
		{
			NBCEL.generic.ReferenceType rt = GetReferenceType(cpg);
			if (rt is NBCEL.generic.ObjectType)
			{
				return (NBCEL.generic.ObjectType)rt;
			}
			throw new NBCEL.generic.ClassGenException(((object)rt).GetType().Name + " " +
				 rt.GetSignature() + " does not represent an ObjectType");
		}
	}
}