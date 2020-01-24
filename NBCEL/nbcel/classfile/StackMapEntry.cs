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

using System;
using System.Text;
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	///     This class represents a stack map entry recording the types of
	///     local variables and the the of stack items at a given byte code offset.
	/// </summary>
	/// <remarks>
	///     This class represents a stack map entry recording the types of
	///     local variables and the the of stack items at a given byte code offset.
	///     See CLDC specification 5.3.1.2
	/// </remarks>
	/// <seealso cref="StackMap" />
	/// <seealso cref="StackMapType" />
	public sealed class StackMapEntry : Node, ICloneable
    {
        private int byte_code_offset;

        private ConstantPool constant_pool;
        private int frame_type;

        private StackMapType[] types_of_locals;

        private StackMapType[] types_of_stack_items;

        /// <summary>Construct object from input stream.</summary>
        /// <param name="input">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal StackMapEntry(DataInput input, ConstantPool constantPool
        )
            : this(input.ReadByte() & 0xFF, -1, null, null, constantPool)
        {
            if (frame_type >= Const.SAME_FRAME && frame_type <= Const.SAME_FRAME_MAX)
            {
                byte_code_offset = frame_type - Const.SAME_FRAME;
            }
            else if (frame_type >= Const.SAME_LOCALS_1_STACK_ITEM_FRAME && frame_type <=
                     Const.SAME_LOCALS_1_STACK_ITEM_FRAME_MAX)
            {
                byte_code_offset = frame_type - Const.SAME_LOCALS_1_STACK_ITEM_FRAME;
                types_of_stack_items = new StackMapType[1];
                types_of_stack_items[0] = new StackMapType(input, constantPool);
            }
            else if (frame_type == Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED)
            {
                byte_code_offset = input.ReadShort();
                types_of_stack_items = new StackMapType[1];
                types_of_stack_items[0] = new StackMapType(input, constantPool);
            }
            else if (frame_type >= Const.CHOP_FRAME && frame_type <= Const.CHOP_FRAME_MAX)
            {
                byte_code_offset = input.ReadShort();
            }
            else if (frame_type == Const.SAME_FRAME_EXTENDED)
            {
                byte_code_offset = input.ReadShort();
            }
            else if (frame_type >= Const.APPEND_FRAME && frame_type <= Const.APPEND_FRAME_MAX)
            {
                byte_code_offset = input.ReadShort();
                var number_of_locals = frame_type - 251;
                types_of_locals = new StackMapType[number_of_locals];
                for (var i = 0; i < number_of_locals; i++) types_of_locals[i] = new StackMapType(input, constantPool);
            }
            else if (frame_type == Const.FULL_FRAME)
            {
                byte_code_offset = input.ReadShort();
                int number_of_locals = input.ReadShort();
                types_of_locals = new StackMapType[number_of_locals];
                for (var i = 0; i < number_of_locals; i++) types_of_locals[i] = new StackMapType(input, constantPool);
                int number_of_stack_items = input.ReadShort();
                types_of_stack_items = new StackMapType[number_of_stack_items];
                for (var i = 0; i < number_of_stack_items; i++)
                    types_of_stack_items[i] = new StackMapType(input, constantPool);
            }
            else
            {
                /* Can't happen */
                throw new ClassFormatException("Invalid frame type found while parsing stack map table: "
                                               + frame_type);
            }
        }

        /// <summary>DO NOT USE</summary>
        /// <param name="byteCodeOffset" />
        /// <param name="numberOfLocals">NOT USED</param>
        /// <param name="typesOfLocals">
        ///     array of
        ///     <see cref="StackMapType" />
        ///     s of locals
        /// </param>
        /// <param name="numberOfStackItems">NOT USED</param>
        /// <param name="typesOfStackItems">
        ///     array ot
        ///     <see cref="StackMapType" />
        ///     s of stack items
        /// </param>
        /// <param name="constantPool">the constant pool</param>
        [Obsolete(@"Since 6.0, use StackMapEntry(int, int, StackMapType[], StackMapType[], ConstantPool) instead"
        )]
        public StackMapEntry(int byteCodeOffset, int numberOfLocals, StackMapType
                [] typesOfLocals, int numberOfStackItems, StackMapType[] typesOfStackItems
            , ConstantPool constantPool)
        {
            byte_code_offset = byteCodeOffset;
            types_of_locals = typesOfLocals != null
                ? typesOfLocals
                : new StackMapType
                    [0];
            types_of_stack_items = typesOfStackItems != null
                ? typesOfStackItems
                : new StackMapType
                    [0];
            constant_pool = constantPool;
        }

        /// <summary>Create an instance</summary>
        /// <param name="tag">the frame_type to use</param>
        /// <param name="byteCodeOffset" />
        /// <param name="typesOfLocals">
        ///     array of
        ///     <see cref="StackMapType" />
        ///     s of locals
        /// </param>
        /// <param name="typesOfStackItems">
        ///     array ot
        ///     <see cref="StackMapType" />
        ///     s of stack items
        /// </param>
        /// <param name="constantPool">the constant pool</param>
        public StackMapEntry(int tag, int byteCodeOffset, StackMapType[]
            typesOfLocals, StackMapType[] typesOfStackItems, ConstantPool
            constantPool)
        {
            frame_type = tag;
            byte_code_offset = byteCodeOffset;
            types_of_locals = typesOfLocals != null
                ? typesOfLocals
                : new StackMapType
                    [0];
            types_of_stack_items = typesOfStackItems != null
                ? typesOfStackItems
                : new StackMapType
                    [0];
            constant_pool = constantPool;
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        ///     Called by objects that are traversing the nodes of the tree implicitely
        ///     defined by the contents of a Java class.
        /// </summary>
        /// <remarks>
        ///     Called by objects that are traversing the nodes of the tree implicitely
        ///     defined by the contents of a Java class. I.e., the hierarchy of methods,
        ///     fields, attributes, etc. spawns a tree of objects.
        /// </remarks>
        /// <param name="v">Visitor object</param>
        public void Accept(Visitor v)
        {
            v.VisitStackMapEntry(this);
        }

        /// <summary>Dump stack map entry</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public void Dump(DataOutputStream file)
        {
            file.Write(frame_type);
            if (frame_type >= Const.SAME_FRAME && frame_type <= Const.SAME_FRAME_MAX)
            {
            }
            else if (frame_type >= Const.SAME_LOCALS_1_STACK_ITEM_FRAME && frame_type <=
                     Const.SAME_LOCALS_1_STACK_ITEM_FRAME_MAX)
            {
                // nothing to be done
                types_of_stack_items[0].Dump(file);
            }
            else if (frame_type == Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED)
            {
                file.WriteShort(byte_code_offset);
                types_of_stack_items[0].Dump(file);
            }
            else if (frame_type >= Const.CHOP_FRAME && frame_type <= Const.CHOP_FRAME_MAX)
            {
                file.WriteShort(byte_code_offset);
            }
            else if (frame_type == Const.SAME_FRAME_EXTENDED)
            {
                file.WriteShort(byte_code_offset);
            }
            else if (frame_type >= Const.APPEND_FRAME && frame_type <= Const.APPEND_FRAME_MAX)
            {
                file.WriteShort(byte_code_offset);
                foreach (var type in types_of_locals) type.Dump(file);
            }
            else if (frame_type == Const.FULL_FRAME)
            {
                file.WriteShort(byte_code_offset);
                file.WriteShort(types_of_locals.Length);
                foreach (var type in types_of_locals) type.Dump(file);
                file.WriteShort(types_of_stack_items.Length);
                foreach (var type in types_of_stack_items) type.Dump(file);
            }
            else
            {
                /* Can't happen */
                throw new ClassFormatException("Invalid Stack map table tag: " +
                                               frame_type);
            }
        }

        /// <returns>String representation.</returns>
        public override string ToString()
        {
            var buf = new StringBuilder(64);
            buf.Append("(");
            if (frame_type >= Const.SAME_FRAME && frame_type <= Const.SAME_FRAME_MAX)
                buf.Append("SAME");
            else if (frame_type >= Const.SAME_LOCALS_1_STACK_ITEM_FRAME && frame_type <=
                     Const.SAME_LOCALS_1_STACK_ITEM_FRAME_MAX)
                buf.Append("SAME_LOCALS_1_STACK");
            else if (frame_type == Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED)
                buf.Append("SAME_LOCALS_1_STACK_EXTENDED");
            else if (frame_type >= Const.CHOP_FRAME && frame_type <= Const.CHOP_FRAME_MAX)
                buf.Append("CHOP ").Append((251 - frame_type).ToString());
            else if (frame_type == Const.SAME_FRAME_EXTENDED)
                buf.Append("SAME_EXTENDED");
            else if (frame_type >= Const.APPEND_FRAME && frame_type <= Const.APPEND_FRAME_MAX)
                buf.Append("APPEND ").Append((frame_type - 251).ToString());
            else if (frame_type == Const.FULL_FRAME)
                buf.Append("FULL");
            else
                buf.Append("UNKNOWN (").Append(frame_type).Append(")");
            buf.Append(", offset delta=").Append(byte_code_offset);
            if (types_of_locals.Length > 0)
            {
                buf.Append(", locals={");
                for (var i = 0; i < types_of_locals.Length; i++)
                {
                    buf.Append(types_of_locals[i]);
                    if (i < types_of_locals.Length - 1) buf.Append(", ");
                }

                buf.Append("}");
            }

            if (types_of_stack_items.Length > 0)
            {
                buf.Append(", stack items={");
                for (var i = 0; i < types_of_stack_items.Length; i++)
                {
                    buf.Append(types_of_stack_items[i]);
                    if (i < types_of_stack_items.Length - 1) buf.Append(", ");
                }

                buf.Append("}");
            }

            buf.Append(")");
            return buf.ToString();
        }

        /// <summary>Calculate stack map entry size</summary>
        internal int GetMapEntrySize()
        {
            if (frame_type >= Const.SAME_FRAME && frame_type <= Const.SAME_FRAME_MAX) return 1;

            if (frame_type >= Const.SAME_LOCALS_1_STACK_ITEM_FRAME && frame_type <=
                Const.SAME_LOCALS_1_STACK_ITEM_FRAME_MAX)
                return 1 + (types_of_stack_items[0].HasIndex() ? 3 : 1);

            if (frame_type == Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED)
                return 3 + (types_of_stack_items[0].HasIndex() ? 3 : 1);

            if (frame_type >= Const.CHOP_FRAME && frame_type <= Const.CHOP_FRAME_MAX) return 3;

            if (frame_type == Const.SAME_FRAME_EXTENDED) return 3;

            if (frame_type >= Const.APPEND_FRAME && frame_type <= Const.APPEND_FRAME_MAX)
            {
                var len = 3;
                foreach (var types_of_local in types_of_locals) len += types_of_local.HasIndex() ? 3 : 1;
                return len;
            }

            if (frame_type == Const.FULL_FRAME)
            {
                var len = 7;
                foreach (var types_of_local in types_of_locals) len += types_of_local.HasIndex() ? 3 : 1;
                foreach (var types_of_stack_item in types_of_stack_items) len += types_of_stack_item.HasIndex() ? 3 : 1;
                return len;
            }

            throw new Exception("Invalid StackMap frame_type: " + frame_type);
        }

        public void SetFrameType(int f)
        {
            if (f >= Const.SAME_FRAME && f <= Const.SAME_FRAME_MAX)
            {
                byte_code_offset = f - Const.SAME_FRAME;
            }
            else if (f >= Const.SAME_LOCALS_1_STACK_ITEM_FRAME && f <= Const.SAME_LOCALS_1_STACK_ITEM_FRAME_MAX)
            {
                byte_code_offset = f - Const.SAME_LOCALS_1_STACK_ITEM_FRAME;
            }
            else if (f == Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED)
            {
            }
            else if (f >= Const.CHOP_FRAME && f <= Const.CHOP_FRAME_MAX)
            {
            }
            else if (f == Const.SAME_FRAME_EXTENDED)
            {
            }
            else if (f >= Const.APPEND_FRAME && f <= Const.APPEND_FRAME_MAX)
            {
            }
            else if (f == Const.FULL_FRAME)
            {
            }
            else
            {
                // CHECKSTYLE IGNORE EmptyBlock
                // CHECKSTYLE IGNORE EmptyBlock
                // CHECKSTYLE IGNORE EmptyBlock
                // CHECKSTYLE IGNORE EmptyBlock
                // CHECKSTYLE IGNORE EmptyBlock
                throw new Exception("Invalid StackMap frame_type");
            }

            frame_type = f;
        }

        public int GetFrameType()
        {
            return frame_type;
        }

        public void SetByteCodeOffset(int new_offset)
        {
            if (new_offset < 0 || new_offset > 32767) throw new Exception("Invalid StackMap offset: " + new_offset);
            if (frame_type >= Const.SAME_FRAME && frame_type <= Const.SAME_FRAME_MAX)
            {
                if (new_offset > Const.SAME_FRAME_MAX)
                    frame_type = Const.SAME_FRAME_EXTENDED;
                else
                    frame_type = new_offset;
            }
            else if (frame_type >= Const.SAME_LOCALS_1_STACK_ITEM_FRAME && frame_type <=
                     Const.SAME_LOCALS_1_STACK_ITEM_FRAME_MAX)
            {
                if (new_offset > Const.SAME_FRAME_MAX)
                    frame_type = Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED;
                else
                    frame_type = Const.SAME_LOCALS_1_STACK_ITEM_FRAME + new_offset;
            }
            else if (frame_type == Const.SAME_LOCALS_1_STACK_ITEM_FRAME_EXTENDED)
            {
            }
            else if (frame_type >= Const.CHOP_FRAME && frame_type <= Const.CHOP_FRAME_MAX)
            {
            }
            else if (frame_type == Const.SAME_FRAME_EXTENDED)
            {
            }
            else if (frame_type >= Const.APPEND_FRAME && frame_type <= Const.APPEND_FRAME_MAX)
            {
            }
            else if (frame_type == Const.FULL_FRAME)
            {
            }
            else
            {
                // CHECKSTYLE IGNORE EmptyBlock
                // CHECKSTYLE IGNORE EmptyBlock
                // CHECKSTYLE IGNORE EmptyBlock
                // CHECKSTYLE IGNORE EmptyBlock
                // CHECKSTYLE IGNORE EmptyBlock
                throw new Exception("Invalid StackMap frame_type: " + frame_type);
            }

            byte_code_offset = new_offset;
        }

        /// <summary>
        ///     Update the distance (as an offset delta) from this StackMap
        ///     entry to the next.
        /// </summary>
        /// <remarks>
        ///     Update the distance (as an offset delta) from this StackMap
        ///     entry to the next.  Note that this might cause the the
        ///     frame type to change.  Note also that delta may be negative.
        /// </remarks>
        /// <param name="delta">offset delta</param>
        public void UpdateByteCodeOffset(int delta)
        {
            SetByteCodeOffset(byte_code_offset + delta);
        }

        public int GetByteCodeOffset()
        {
            return byte_code_offset;
        }

        [Obsolete(@"since 6.0")]
        public void SetNumberOfLocals(int n)
        {
        }

        // TODO unused
        public int GetNumberOfLocals()
        {
            return types_of_locals.Length;
        }

        public void SetTypesOfLocals(StackMapType[] types)
        {
            types_of_locals = types != null ? types : new StackMapType[0];
        }

        public StackMapType[] GetTypesOfLocals()
        {
            return types_of_locals;
        }

        [Obsolete(@"since 6.0")]
        public void SetNumberOfStackItems(int n)
        {
        }

        // TODO unused
        public int GetNumberOfStackItems()
        {
            return types_of_stack_items.Length;
        }

        public void SetTypesOfStackItems(StackMapType[] types)
        {
            types_of_stack_items = types != null
                ? types
                : new StackMapType[0
                ];
        }

        public StackMapType[] GetTypesOfStackItems()
        {
            return types_of_stack_items;
        }

        /// <returns>deep copy of this object</returns>
        public StackMapEntry Copy()
        {
            StackMapEntry e;
            e = (StackMapEntry) MemberwiseClone();
            e.types_of_locals = new StackMapType[types_of_locals.Length];
            for (var i = 0; i < types_of_locals.Length; i++) e.types_of_locals[i] = types_of_locals[i].Copy();
            e.types_of_stack_items = new StackMapType[types_of_stack_items.Length
            ];
            for (var i = 0; i < types_of_stack_items.Length; i++)
                e.types_of_stack_items[i] = types_of_stack_items[i].Copy();
            return e;
        }

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
    }
}