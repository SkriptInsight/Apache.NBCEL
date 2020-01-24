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

using System;
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.ClassFile
{
    /// <summary>Abstract super class for fields and methods.</summary>
    public abstract class FieldOrMethod : AccessFlags, ICloneable
        , Node
    {
        private AnnotationEntry[] annotationEntries;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal Attribute[] attributes;

        [Obsolete(@"(since 6.0) will be removed (not needed)")]
        protected internal int attributes_count;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal ConstantPool constant_pool;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal int name_index;

        private bool searchedForSignatureAttribute;

        [Obsolete(@"(since 6.0) will be made private; do not access directly, use getter/setter"
        )]
        protected internal int signature_index;

        private string signatureAttributeString;

        internal FieldOrMethod()
        {
        }

        /// <summary>Initialize from another object.</summary>
        /// <remarks>
        ///     Initialize from another object. Note that both objects use the same
        ///     references (shallow copy). Use clone() for a physical copy.
        /// </remarks>
        protected internal FieldOrMethod(FieldOrMethod c)
            : this(c.GetAccessFlags(), c.GetNameIndex(), c.GetSignatureIndex(), c.GetAttributes
                (), c.GetConstantPool())
        {
        }

        /// <summary>Construct object from file stream.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="ClassFormatException" />
        /// <exception cref="NBCEL.classfile.ClassFormatException" />
        [Obsolete(@"(6.0) Use FieldOrMethod(java.io.DataInput, ConstantPool) instead."
        )]
        protected internal FieldOrMethod(DataInputStream file, ConstantPool
            constant_pool)
            : this((DataInput) file, constant_pool)
        {
        }

        /// <summary>Construct object from file stream.</summary>
        /// <param name="file">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        /// <exception cref="ClassFormatException" />
        /// <exception cref="NBCEL.classfile.ClassFormatException" />
        protected internal FieldOrMethod(DataInput file, ConstantPool
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
            var attributes_count = file.ReadUnsignedShort();
            attributes = new Attribute[attributes_count];
            for (var i = 0; i < attributes_count; i++) attributes[i] = Attribute.ReadAttribute(file, constant_pool);
            this.attributes_count = attributes_count;
        }

        /// <param name="access_flags">Access rights of method</param>
        /// <param name="name_index">Points to field name in constant pool</param>
        /// <param name="signature_index">Points to encoded signature</param>
        /// <param name="attributes">Collection of attributes</param>
        /// <param name="constant_pool">Array of constants</param>
        protected internal FieldOrMethod(int access_flags, int name_index, int signature_index
            , Attribute[] attributes, ConstantPool constant_pool
        )
            : base(access_flags)
        {
            // init deprecated field
            this.name_index = name_index;
            this.signature_index = signature_index;
            this.constant_pool = constant_pool;
            SetAttributes(attributes);
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        public abstract void Accept(Visitor arg1);

        /// <summary>Dump object to file stream on binary format.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public void Dump(DataOutputStream file)
        {
            file.WriteShort(GetAccessFlags());
            file.WriteShort(name_index);
            file.WriteShort(signature_index);
            file.WriteShort(attributes_count);
            if (attributes != null)
                foreach (var attribute in attributes)
                    attribute.Dump(file);
        }

        /// <returns>Collection of object attributes.</returns>
        public Attribute[] GetAttributes()
        {
            return attributes;
        }

        /// <param name="attributes">Collection of object attributes.</param>
        public void SetAttributes(Attribute[] attributes)
        {
            this.attributes = attributes;
            attributes_count = attributes != null ? attributes.Length : 0;
        }

        // init deprecated field
        /// <returns>Constant pool used by this object.</returns>
        public ConstantPool GetConstantPool()
        {
            return constant_pool;
        }

        /// <param name="constant_pool">Constant pool to be used for this object.</param>
        public void SetConstantPool(ConstantPool constant_pool)
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
            ConstantUtf8 c;
            c = (ConstantUtf8) constant_pool.GetConstant(name_index, Const
                .CONSTANT_Utf8);
            return c.GetBytes();
        }

        /// <returns>String representation of object's type signature (java style)</returns>
        public string GetSignature()
        {
            ConstantUtf8 c;
            c = (ConstantUtf8) constant_pool.GetConstant(signature_index, Const
                .CONSTANT_Utf8);
            return c.GetBytes();
        }

        /// <returns>deep copy of this field</returns>
        protected internal virtual FieldOrMethod Copy_(ConstantPool
            _constant_pool)
        {
            FieldOrMethod c = null;
            c = (FieldOrMethod) MemberwiseClone();
            // ignored, but will cause NPE ...
            c.constant_pool = constant_pool;
            c.attributes = new Attribute[attributes.Length];
            c.attributes_count = attributes_count;
            // init deprecated field
            for (var i = 0; i < attributes.Length; i++) c.attributes[i] = attributes[i].Copy(constant_pool);
            return c;
        }

        /// <returns>Annotations on the field or method</returns>
        /// <since>6.0</since>
        public virtual AnnotationEntry[] GetAnnotationEntries()
        {
            if (annotationEntries == null)
                annotationEntries = AnnotationEntry.CreateAnnotationEntries(GetAttributes
                    ());
            return annotationEntries;
        }

        /// <summary>Hunts for a signature attribute on the member and returns its contents.</summary>
        /// <remarks>
        ///     Hunts for a signature attribute on the member and returns its contents.  So where the 'regular' signature
        ///     may be (Ljava/util/Vector;)V the signature attribute may in fact say 'Ljava/lang/Vector&lt;Ljava/lang/String&gt;;'
        ///     Coded for performance - searches for the attribute only when requested - only searches for it once.
        /// </remarks>
        /// <since>6.0</since>
        public string GetGenericSignature()
        {
            if (!searchedForSignatureAttribute)
            {
                var found = false;
                for (var i = 0; !found && i < attributes.Length; i++)
                    if (attributes[i] is Signature)
                    {
                        signatureAttributeString = ((Signature) attributes[i]).GetSignature
                            ();
                        found = true;
                    }

                searchedForSignatureAttribute = true;
            }

            return signatureAttributeString;
        }
    }
}