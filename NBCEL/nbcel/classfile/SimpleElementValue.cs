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
using System.Globalization;
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
    /// <since>6.0</since>
    public class SimpleElementValue : ElementValue
    {
        private int index;

        public SimpleElementValue(int type, int index, ConstantPool cpool
        )
            : base(type, cpool)
        {
            this.index = index;
        }

        /// <returns>Value entry index in the cpool</returns>
        public virtual int GetIndex()
        {
            return index;
        }

        public virtual void SetIndex(int index)
        {
            this.index = index;
        }

        public virtual string GetValueString()
        {
            if (GetType() != STRING)
                throw new Exception("Dont call getValueString() on a non STRING ElementValue"
                );
            var c = (ConstantUtf8) GetConstantPool
                ().GetConstant(GetIndex(), Const.CONSTANT_Utf8);
            return c.GetBytes();
        }

        public virtual int GetValueInt()
        {
            if (GetType() != PRIMITIVE_INT)
                throw new Exception("Dont call getValueString() on a non STRING ElementValue"
                );
            var c = (ConstantInteger) GetConstantPool
                ().GetConstant(GetIndex(), Const.CONSTANT_Integer);
            return c.GetBytes();
        }

        public virtual byte GetValueByte()
        {
            if (GetType() != PRIMITIVE_BYTE) throw new Exception("Dont call getValueByte() on a non BYTE ElementValue");
            var c = (ConstantInteger) GetConstantPool
                ().GetConstant(GetIndex(), Const.CONSTANT_Integer);
            return unchecked((byte) c.GetBytes());
        }

        public virtual char GetValueChar()
        {
            if (GetType() != PRIMITIVE_CHAR) throw new Exception("Dont call getValueChar() on a non CHAR ElementValue");
            var c = (ConstantInteger) GetConstantPool
                ().GetConstant(GetIndex(), Const.CONSTANT_Integer);
            return (char) c.GetBytes();
        }

        public virtual long GetValueLong()
        {
            if (GetType() != PRIMITIVE_LONG) throw new Exception("Dont call getValueLong() on a non LONG ElementValue");
            var j = (ConstantLong) GetConstantPool
                ().GetConstant(GetIndex());
            return j.GetBytes();
        }

        public virtual float GetValueFloat()
        {
            if (GetType() != PRIMITIVE_FLOAT)
                throw new Exception("Dont call getValueFloat() on a non FLOAT ElementValue"
                );
            var f = (ConstantFloat) GetConstantPool
                ().GetConstant(GetIndex());
            return f.GetBytes();
        }

        public virtual double GetValueDouble()
        {
            if (GetType() != PRIMITIVE_DOUBLE)
                throw new Exception("Dont call getValueDouble() on a non DOUBLE ElementValue"
                );
            var d = (ConstantDouble) GetConstantPool
                ().GetConstant(GetIndex());
            return d.GetBytes();
        }

        public virtual bool GetValueBoolean()
        {
            if (GetType() != PRIMITIVE_BOOLEAN)
                throw new Exception("Dont call getValueBoolean() on a non BOOLEAN ElementValue"
                );
            var bo = (ConstantInteger) GetConstantPool
                ().GetConstant(GetIndex());
            return bo.GetBytes() != 0;
        }

        public virtual short GetValueShort()
        {
            if (GetType() != PRIMITIVE_SHORT)
                throw new Exception("Dont call getValueShort() on a non SHORT ElementValue"
                );
            var s = (ConstantInteger) GetConstantPool
                ().GetConstant(GetIndex());
            return (short) s.GetBytes();
        }

        public override string ToString()
        {
            return StringifyValue();
        }

        // Whatever kind of value it is, return it as a string
        public override string StringifyValue()
        {
            var cpool = GetConstantPool();
            var _type = GetType();
            switch (_type)
            {
                case PRIMITIVE_INT:
                {
                    var c = (ConstantInteger) cpool.GetConstant
                        (GetIndex(), Const.CONSTANT_Integer);
                    return c.GetBytes().ToString();
                }

                case PRIMITIVE_LONG:
                {
                    var j = (ConstantLong) cpool.GetConstant(
                        GetIndex(), Const.CONSTANT_Long);
                    return Convert.ToString(j.GetBytes());
                }

                case PRIMITIVE_DOUBLE:
                {
                    var d = (ConstantDouble) cpool.GetConstant
                        (GetIndex(), Const.CONSTANT_Double);
                    return Convert.ToString(d.GetBytes(), CultureInfo.InvariantCulture);
                }

                case PRIMITIVE_FLOAT:
                {
                    var f = (ConstantFloat) cpool.GetConstant
                        (GetIndex(), Const.CONSTANT_Float);
                    return Convert.ToString(f.GetBytes(), CultureInfo.InvariantCulture);
                }

                case PRIMITIVE_SHORT:
                {
                    var s = (ConstantInteger) cpool.GetConstant
                        (GetIndex(), Const.CONSTANT_Integer);
                    return Convert.ToString(s.GetBytes());
                }

                case PRIMITIVE_BYTE:
                {
                    var b = (ConstantInteger) cpool.GetConstant
                        (GetIndex(), Const.CONSTANT_Integer);
                    return Convert.ToString(b.GetBytes());
                }

                case PRIMITIVE_CHAR:
                {
                    var ch = (ConstantInteger) cpool.GetConstant
                        (GetIndex(), Const.CONSTANT_Integer);
                    return ((char) ch.GetBytes()).ToString();
                }

                case PRIMITIVE_BOOLEAN:
                {
                    var bo = (ConstantInteger) cpool.GetConstant
                        (GetIndex(), Const.CONSTANT_Integer);
                    if (bo.GetBytes() == 0) return "false";
                    return "true";
                }

                case STRING:
                {
                    var cu8 = (ConstantUtf8) cpool.GetConstant
                        (GetIndex(), Const.CONSTANT_Utf8);
                    return cu8.GetBytes();
                }

                default:
                {
                    throw new Exception("SimpleElementValue class does not know how to stringify type "
                                        + _type);
                }
            }
        }

        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream dos)
        {
            var _type = GetType();
            dos.WriteByte(_type);
            switch (_type)
            {
                case PRIMITIVE_INT:
                case PRIMITIVE_BYTE:
                case PRIMITIVE_CHAR:
                case PRIMITIVE_FLOAT:
                case PRIMITIVE_LONG:
                case PRIMITIVE_BOOLEAN:
                case PRIMITIVE_SHORT:
                case PRIMITIVE_DOUBLE:
                case STRING:
                {
                    // u1 kind of value
                    dos.WriteShort(GetIndex());
                    break;
                }

                default:
                {
                    throw new Exception("SimpleElementValue doesnt know how to write out type "
                                        + _type);
                }
            }
        }
    }
}