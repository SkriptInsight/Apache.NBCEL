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

namespace NBCEL.classfile
{
	/// <summary>Abstract super class for fields and methods.</summary>
	public abstract class FieldOrMethod : NBCEL.classfile.AccessFlags, System.ICloneable
		, NBCEL.classfile.Node
	{
		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal int name_index;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal int signature_index;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal NBCEL.classfile.Attribute[] attributes;

		[System.ObsoleteAttribute(@"(since 6.0) will be removed (not needed)")]
		protected internal int attributes_count;

		private NBCEL.classfile.AnnotationEntry[] annotationEntries;

		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal NBCEL.classfile.ConstantPool constant_pool;

		private string signatureAttributeString = null;

		private bool searchedForSignatureAttribute = false;

		internal FieldOrMethod()
		{
		}

		/// <summary>Initialize from another object.</summary>
		/// <remarks>
		/// Initialize from another object. Note that both objects use the same
		/// references (shallow copy). Use clone() for a physical copy.
		/// </remarks>
		protected internal FieldOrMethod(NBCEL.classfile.FieldOrMethod c)
			: this(c.GetAccessFlags(), c.GetNameIndex(), c.GetSignatureIndex(), c.GetAttributes
				(), c.GetConstantPool())
		{
		}

		/// <summary>Construct object from file stream.</summary>
		/// <param name="file">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		[System.ObsoleteAttribute(@"(6.0) Use FieldOrMethod(java.io.DataInput, ConstantPool) instead."
			)]
		protected internal FieldOrMethod(java.io.DataInputStream file, NBCEL.classfile.ConstantPool
			 constant_pool)
			: this((java.io.DataInput)file, constant_pool)
		{
		}

		/// <summary>Construct object from file stream.</summary>
		/// <param name="file">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ClassFormatException"/>
		/// <exception cref="NBCEL.classfile.ClassFormatException"/>
		protected internal FieldOrMethod(java.io.DataInput file, NBCEL.classfile.ConstantPool
			 constant_pool)
			: this(file.ReadUnsignedShort(), file.ReadUnsignedShort(), file.ReadUnsignedShort
				(), null, constant_pool)
		{
			// Points to field name in constant pool
			// Points to encoded signature
			// Collection of attributes
			// No. of attributes
			// @since 6.0
			// annotations defined on the field or method
			int attributes_count = file.ReadUnsignedShort();
			attributes = new NBCEL.classfile.Attribute[attributes_count];
			for (int i = 0; i < attributes_count; i++)
			{
				attributes[i] = NBCEL.classfile.Attribute.ReadAttribute(file, constant_pool);
			}
			this.attributes_count = attributes_count;
		}

		/// <param name="access_flags">Access rights of method</param>
		/// <param name="name_index">Points to field name in constant pool</param>
		/// <param name="signature_index">Points to encoded signature</param>
		/// <param name="attributes">Collection of attributes</param>
		/// <param name="constant_pool">Array of constants</param>
		protected internal FieldOrMethod(int access_flags, int name_index, int signature_index
			, NBCEL.classfile.Attribute[] attributes, NBCEL.classfile.ConstantPool constant_pool
			)
			: base(access_flags)
		{
			// init deprecated field
			this.name_index = name_index;
			this.signature_index = signature_index;
			this.constant_pool = constant_pool;
			SetAttributes(attributes);
		}

		/// <summary>Dump object to file stream on binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public void Dump(java.io.DataOutputStream file)
		{
			file.WriteShort(base.GetAccessFlags());
			file.WriteShort(name_index);
			file.WriteShort(signature_index);
			file.WriteShort(attributes_count);
			if (attributes != null)
			{
				foreach (NBCEL.classfile.Attribute attribute in attributes)
				{
					attribute.Dump(file);
				}
			}
		}

		/// <returns>Collection of object attributes.</returns>
		public NBCEL.classfile.Attribute[] GetAttributes()
		{
			return attributes;
		}

		/// <param name="attributes">Collection of object attributes.</param>
		public void SetAttributes(NBCEL.classfile.Attribute[] attributes)
		{
			this.attributes = attributes;
			this.attributes_count = attributes != null ? attributes.Length : 0;
		}

		// init deprecated field
		/// <returns>Constant pool used by this object.</returns>
		public NBCEL.classfile.ConstantPool GetConstantPool()
		{
			return constant_pool;
		}

		/// <param name="constant_pool">Constant pool to be used for this object.</param>
		public void SetConstantPool(NBCEL.classfile.ConstantPool constant_pool)
		{
			this.constant_pool = constant_pool;
		}

		/// <returns>Index in constant pool of object's name.</returns>
		public int GetNameIndex()
		{
			return name_index;
		}

		/// <param name="name_index">Index in constant pool of object's name.</param>
		public void SetNameIndex(int name_index)
		{
			this.name_index = name_index;
		}

		/// <returns>Index in constant pool of field signature.</returns>
		public int GetSignatureIndex()
		{
			return signature_index;
		}

		/// <param name="signature_index">Index in constant pool of field signature.</param>
		public void SetSignatureIndex(int signature_index)
		{
			this.signature_index = signature_index;
		}

		/// <returns>Name of object, i.e., method name or field name</returns>
		public string GetName()
		{
			NBCEL.classfile.ConstantUtf8 c;
			c = (NBCEL.classfile.ConstantUtf8)constant_pool.GetConstant(name_index, NBCEL.Const
				.CONSTANT_Utf8);
			return c.GetBytes();
		}

		/// <returns>String representation of object's type signature (java style)</returns>
		public string GetSignature()
		{
			NBCEL.classfile.ConstantUtf8 c;
			c = (NBCEL.classfile.ConstantUtf8)constant_pool.GetConstant(signature_index, NBCEL.Const
				.CONSTANT_Utf8);
			return c.GetBytes();
		}

		/// <returns>deep copy of this field</returns>
		protected internal virtual NBCEL.classfile.FieldOrMethod Copy_(NBCEL.classfile.ConstantPool
			 _constant_pool)
		{
			NBCEL.classfile.FieldOrMethod c = null;
			c = (NBCEL.classfile.FieldOrMethod)MemberwiseClone();
			// ignored, but will cause NPE ...
			c.constant_pool = constant_pool;
			c.attributes = new NBCEL.classfile.Attribute[attributes.Length];
			c.attributes_count = attributes_count;
			// init deprecated field
			for (int i = 0; i < attributes.Length; i++)
			{
				c.attributes[i] = attributes[i].Copy(constant_pool);
			}
			return c;
		}

		/// <returns>Annotations on the field or method</returns>
		/// <since>6.0</since>
		public virtual NBCEL.classfile.AnnotationEntry[] GetAnnotationEntries()
		{
			if (annotationEntries == null)
			{
				annotationEntries = NBCEL.classfile.AnnotationEntry.CreateAnnotationEntries(GetAttributes
					());
			}
			return annotationEntries;
		}

		/// <summary>Hunts for a signature attribute on the member and returns its contents.</summary>
		/// <remarks>
		/// Hunts for a signature attribute on the member and returns its contents.  So where the 'regular' signature
		/// may be (Ljava/util/Vector;)V the signature attribute may in fact say 'Ljava/lang/Vector&lt;Ljava/lang/String&gt;;'
		/// Coded for performance - searches for the attribute only when requested - only searches for it once.
		/// </remarks>
		/// <since>6.0</since>
		public string GetGenericSignature()
		{
			if (!searchedForSignatureAttribute)
			{
				bool found = false;
				for (int i = 0; !found && i < attributes.Length; i++)
				{
					if (attributes[i] is NBCEL.classfile.Signature)
					{
						signatureAttributeString = ((NBCEL.classfile.Signature)attributes[i]).GetSignature
							();
						found = true;
					}
				}
				searchedForSignatureAttribute = true;
			}
			return signatureAttributeString;
		}

		public abstract void Accept(NBCEL.classfile.Visitor arg1);

		object System.ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
