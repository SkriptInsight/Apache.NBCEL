using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	/// This attribute exists for local or
	/// anonymous classes and ...
	/// </summary>
	/// <remarks>
	/// This attribute exists for local or
	/// anonymous classes and ... there can be only one.
	/// </remarks>
	/// <since>6.0</since>
	public class EnclosingMethod : NBCEL.classfile.Attribute
	{
		private int classIndex;

		private int methodIndex;

		/// <exception cref="System.IO.IOException"/>
		internal EnclosingMethod(int nameIndex, int len, java.io.DataInput input, NBCEL.classfile.ConstantPool
			 cpool)
			: this(nameIndex, len, input.ReadUnsignedShort(), input.ReadUnsignedShort(), cpool
				)
		{
		}

		private EnclosingMethod(int nameIndex, int len, int classIdx, int methodIdx, NBCEL.classfile.ConstantPool
			 cpool)
			: base(NBCEL.Const.ATTR_ENCLOSING_METHOD, nameIndex, len, cpool)
		{
			// Pointer to the CONSTANT_Class_info structure representing the
			// innermost class that encloses the declaration of the current class.
			// If the current class is not immediately enclosed by a method or
			// constructor, then the value of the method_index item must be zero.
			// Otherwise, the value of the  method_index item must point to a
			// CONSTANT_NameAndType_info structure representing the name and the
			// type of a method in the class referenced by the class we point
			// to in the class_index.  *It is the compiler responsibility* to
			// ensure that the method identified by this index is the closest
			// lexically enclosing method that includes the local/anonymous class.
			// Ctors - and code to read an attribute in.
			classIndex = classIdx;
			methodIndex = methodIdx;
		}

		public override void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitEnclosingMethod(this);
		}

		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool constant_pool
			)
		{
			return (NBCEL.classfile.Attribute)Clone();
		}

		// Accessors
		public int GetEnclosingClassIndex()
		{
			return classIndex;
		}

		public int GetEnclosingMethodIndex()
		{
			return methodIndex;
		}

		public void SetEnclosingClassIndex(int idx)
		{
			classIndex = idx;
		}

		public void SetEnclosingMethodIndex(int idx)
		{
			methodIndex = idx;
		}

		public NBCEL.classfile.ConstantClass GetEnclosingClass()
		{
			NBCEL.classfile.ConstantClass c = (NBCEL.classfile.ConstantClass)base.GetConstantPool
				().GetConstant(classIndex, NBCEL.Const.CONSTANT_Class);
			return c;
		}

		public NBCEL.classfile.ConstantNameAndType GetEnclosingMethod()
		{
			if (methodIndex == 0)
			{
				return null;
			}
			NBCEL.classfile.ConstantNameAndType nat = (NBCEL.classfile.ConstantNameAndType)base
				.GetConstantPool().GetConstant(methodIndex, NBCEL.Const.CONSTANT_NameAndType);
			return nat;
		}

		/// <exception cref="System.IO.IOException"/>
		public sealed override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteShort(classIndex);
			file.WriteShort(methodIndex);
		}
	}
}
