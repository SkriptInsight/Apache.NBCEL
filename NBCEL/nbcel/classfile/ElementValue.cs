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
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
    /// <since>6.0</since>
    public abstract class ElementValue
    {
        public const byte STRING = (byte) 's';

        public const byte ENUM_CONSTANT = (byte) 'e';

        public const byte CLASS = (byte) 'c';

        public const byte ANNOTATION = (byte) '@';

        public const byte ARRAY = (byte) '[';

        public const byte PRIMITIVE_INT = (byte) 'I';

        public const byte PRIMITIVE_BYTE = (byte) 'B';

        public const byte PRIMITIVE_CHAR = (byte) 'C';

        public const byte PRIMITIVE_DOUBLE = (byte) 'D';

        public const byte PRIMITIVE_FLOAT = (byte) 'F';

        public const byte PRIMITIVE_LONG = (byte) 'J';

        public const byte PRIMITIVE_SHORT = (byte) 'S';

        public const byte PRIMITIVE_BOOLEAN = (byte) 'Z';

        [Obsolete(@"(since 6.0) will be made private and final; do not access directly, use getter"
        )]
        protected internal ConstantPool cpool;

        [Obsolete(@"(since 6.0) will be made private and final; do not access directly, use getter"
        )]
        protected internal int type;

        protected internal ElementValue(int type, ConstantPool cpool)
        {
            this.type = type;
            this.cpool = cpool;
        }

        // TODO should be final
        // TODO should be final
        public override string ToString()
        {
            return StringifyValue();
        }

        public virtual int GetElementValueType()
        {
            return type;
        }

        public abstract string StringifyValue();

        /// <exception cref="System.IO.IOException" />
        public abstract void Dump(DataOutputStream dos);

        /// <exception cref="System.IO.IOException" />
        public static ElementValue ReadElementValue(DataInput input
            , ConstantPool cpool)
        {
            var type = input.ReadByte();
            switch (type)
            {
                case PRIMITIVE_BYTE:
                case PRIMITIVE_CHAR:
                case PRIMITIVE_DOUBLE:
                case PRIMITIVE_FLOAT:
                case PRIMITIVE_INT:
                case PRIMITIVE_LONG:
                case PRIMITIVE_SHORT:
                case PRIMITIVE_BOOLEAN:
                case STRING:
                {
                    return new SimpleElementValue(type, input.ReadUnsignedShort(), cpool
                    );
                }

                case ENUM_CONSTANT:
                {
                    return new EnumElementValue(ENUM_CONSTANT, input.ReadUnsignedShort
                        (), input.ReadUnsignedShort(), cpool);
                }

                case CLASS:
                {
                    return new ClassElementValue(CLASS, input.ReadUnsignedShort(), cpool
                    );
                }

                case ANNOTATION:
                {
                    // TODO isRuntimeVisible
                    return new AnnotationElementValue(ANNOTATION, AnnotationEntry
                        .Read(input, cpool, false), cpool);
                }

                case ARRAY:
                {
                    var numArrayVals = input.ReadUnsignedShort();
                    var evalues = new ElementValue[numArrayVals
                    ];
                    for (var j = 0; j < numArrayVals; j++) evalues[j] = ReadElementValue(input, cpool);
                    return new ArrayElementValue(ARRAY, evalues, cpool);
                }

                default:
                {
                    throw new Exception("Unexpected element value kind in annotation: " + type
                    );
                }
            }
        }

        /// <since>6.0</since>
        internal ConstantPool GetConstantPool()
        {
            return cpool;
        }

        /// <since>6.0</since>
        internal int GetType()
        {
            return type;
        }

        public virtual string ToShortString()
        {
            return StringifyValue();
        }
    }
}