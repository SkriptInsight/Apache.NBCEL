using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
	/// <summary>
	///     This attribute exists for local or
	///     anonymous classes and ...
	/// </summary>
	/// <remarks>
	///     This attribute exists for local or
	///     anonymous classes and ... there can be only one.
	/// </remarks>
	/// <since>6.0</since>
	public class EnclosingMethod : Attribute
    {
        private int classIndex;

        private int methodIndex;

        /// <exception cref="System.IO.IOException" />
        internal EnclosingMethod(int nameIndex, int len, DataInput input, ConstantPool
            cpool)
            : this(nameIndex, len, input.ReadUnsignedShort(), input.ReadUnsignedShort(), cpool
            )
        {
        }

        private EnclosingMethod(int nameIndex, int len, int classIdx, int methodIdx, ConstantPool
            cpool)
            : base(Const.ATTR_ENCLOSING_METHOD, nameIndex, len, cpool)
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

        public override void Accept(Visitor v)
        {
            v.VisitEnclosingMethod(this);
        }

        public override Attribute Copy(ConstantPool constant_pool
        )
        {
            return (Attribute) Clone();
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

        public ConstantClass GetEnclosingClass()
        {
            var c = (ConstantClass) GetConstantPool
                ().GetConstant(classIndex, Const.CONSTANT_Class);
            return c;
        }

        public ConstantNameAndType GetEnclosingMethod()
        {
            if (methodIndex == 0) return null;
            var nat = (ConstantNameAndType) GetConstantPool().GetConstant(methodIndex, Const.CONSTANT_NameAndType);
            return nat;
        }

        /// <exception cref="System.IO.IOException" />
        public sealed override void Dump(DataOutputStream file)
        {
            base.Dump(file);
            file.WriteShort(classIndex);
            file.WriteShort(methodIndex);
        }
    }
}