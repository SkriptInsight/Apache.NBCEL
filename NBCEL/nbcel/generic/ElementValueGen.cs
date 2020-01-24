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
using java.io;
using NBCEL.classfile;
using Sharpen;

namespace NBCEL.generic
{
    /// <since>6.0</since>
    public abstract class ElementValueGen
    {
        public const int STRING = 's';

        public const int ENUM_CONSTANT = 'e';

        public const int CLASS = 'c';

        public const int ANNOTATION = '@';

        public const int ARRAY = '[';

        public const int PRIMITIVE_INT = 'I';

        public const int PRIMITIVE_BYTE = 'B';

        public const int PRIMITIVE_CHAR = 'C';

        public const int PRIMITIVE_DOUBLE = 'D';

        public const int PRIMITIVE_FLOAT = 'F';

        public const int PRIMITIVE_LONG = 'J';

        public const int PRIMITIVE_SHORT = 'S';

        public const int PRIMITIVE_BOOLEAN = 'Z';

        [Obsolete(@"(since 6.0) will be made private and final; do not access directly, use getter"
        )]
        protected internal ConstantPoolGen cpGen;

        [Obsolete(@"(since 6.0) will be made private and final; do not access directly, use getter"
        )]
        protected internal int type;

        protected internal ElementValueGen(int type, ConstantPoolGen cpGen)
        {
            this.type = type;
            this.cpGen = cpGen;
        }

        /// <summary>Subtypes return an immutable variant of the ElementValueGen</summary>
        public abstract ElementValue GetElementValue();

        public virtual int GetElementValueType()
        {
            return type;
        }

        public abstract string StringifyValue();

        /// <exception cref="System.IO.IOException" />
        public abstract void Dump(DataOutputStream dos);

        /// <exception cref="System.IO.IOException" />
        public static ElementValueGen ReadElementValue(DataInput dis
            , ConstantPoolGen cpGen)
        {
            var type = dis.ReadUnsignedByte();
            switch (type)
            {
                case 'B':
                {
                    // byte
                    return new SimpleElementValueGen(PRIMITIVE_BYTE, dis.ReadUnsignedShort
                        (), cpGen);
                }

                case 'C':
                {
                    // char
                    return new SimpleElementValueGen(PRIMITIVE_CHAR, dis.ReadUnsignedShort
                        (), cpGen);
                }

                case 'D':
                {
                    // double
                    return new SimpleElementValueGen(PRIMITIVE_DOUBLE, dis.ReadUnsignedShort
                        (), cpGen);
                }

                case 'F':
                {
                    // float
                    return new SimpleElementValueGen(PRIMITIVE_FLOAT, dis.ReadUnsignedShort
                        (), cpGen);
                }

                case 'I':
                {
                    // int
                    return new SimpleElementValueGen(PRIMITIVE_INT, dis.ReadUnsignedShort
                        (), cpGen);
                }

                case 'J':
                {
                    // long
                    return new SimpleElementValueGen(PRIMITIVE_LONG, dis.ReadUnsignedShort
                        (), cpGen);
                }

                case 'S':
                {
                    // short
                    return new SimpleElementValueGen(PRIMITIVE_SHORT, dis.ReadUnsignedShort
                        (), cpGen);
                }

                case 'Z':
                {
                    // boolean
                    return new SimpleElementValueGen(PRIMITIVE_BOOLEAN, dis.ReadUnsignedShort
                        (), cpGen);
                }

                case 's':
                {
                    // String
                    return new SimpleElementValueGen(STRING, dis.ReadUnsignedShort(), cpGen
                    );
                }

                case 'e':
                {
                    // Enum constant
                    return new EnumElementValueGen(dis.ReadUnsignedShort(), dis.ReadUnsignedShort
                        (), cpGen);
                }

                case 'c':
                {
                    // Class
                    return new ClassElementValueGen(dis.ReadUnsignedShort(), cpGen);
                }

                case '@':
                {
                    // Annotation
                    // TODO: isRuntimeVisible ??????????
                    // FIXME
                    return new AnnotationElementValueGen(ANNOTATION, new AnnotationEntryGen
                    (AnnotationEntry.Read(dis, cpGen.GetConstantPool(), true), cpGen
                        , false), cpGen);
                }

                case '[':
                {
                    // Array
                    var numArrayVals = dis.ReadUnsignedShort();
                    var evalues = new ElementValue[numArrayVals
                    ];
                    for (var j = 0; j < numArrayVals; j++)
                        evalues[j] = ElementValue.ReadElementValue(dis, cpGen.GetConstantPool
                            ());
                    return new ArrayElementValueGen(ARRAY, evalues, cpGen);
                }

                default:
                {
                    throw new Exception("Unexpected element value kind in annotation: " + type
                    );
                }
            }
        }

        protected internal virtual ConstantPoolGen GetConstantPool()
        {
            return cpGen;
        }

        /// <summary>
        ///     Creates an (modifiable) ElementValueGen copy of an (immutable)
        ///     ElementValue - constant pool is assumed correct.
        /// </summary>
        public static ElementValueGen Copy(ElementValue value
            , ConstantPoolGen cpool, bool copyPoolEntries)
        {
            switch (value.GetElementValueType())
            {
                case 'B':
                case 'C':
                case 'D':
                case 'F':
                case 'I':
                case 'J':
                case 'S':
                case 'Z':
                case 's':
                {
                    // byte
                    // char
                    // double
                    // float
                    // int
                    // long
                    // short
                    // boolean
                    // String
                    return new SimpleElementValueGen((SimpleElementValue
                        ) value, cpool, copyPoolEntries);
                }

                case 'e':
                {
                    // Enum constant
                    return new EnumElementValueGen((EnumElementValue) value
                        , cpool, copyPoolEntries);
                }

                case '@':
                {
                    // Annotation
                    return new AnnotationElementValueGen((AnnotationElementValue
                        ) value, cpool, copyPoolEntries);
                }

                case '[':
                {
                    // Array
                    return new ArrayElementValueGen((ArrayElementValue)
                        value, cpool, copyPoolEntries);
                }

                case 'c':
                {
                    // Class
                    return new ClassElementValueGen((ClassElementValue)
                        value, cpool, copyPoolEntries);
                }

                default:
                {
                    throw new Exception("Not implemented yet! (" + value.GetElementValueType()
                                                                 + ")");
                }
            }
        }
    }
}