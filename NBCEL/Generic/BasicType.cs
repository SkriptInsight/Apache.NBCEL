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

namespace Apache.NBCEL.Generic
{
    /// <summary>Denotes basic type such as int.</summary>
    public sealed class BasicType : Type
    {
	    /// <summary>Constructor for basic types such as int, long, `void'</summary>
	    /// <param name="type">one of T_INT, T_BOOLEAN, ..., T_VOID</param>
	    /// <seealso cref="NBCEL.Const" />
	    internal BasicType(byte type)
            : base(type, Const.GetShortTypeName(type))
        {
            if ((sbyte) type < Const.T_BOOLEAN || type > Const.T_VOID)
                throw new ClassGenException("Invalid type: " + type);
        }

        // @since 6.0 no longer final
        public static BasicType GetType(byte type)
        {
            switch (type)
            {
                case Const.T_VOID:
                {
                    return VOID;
                }

                case Const.T_BOOLEAN:
                {
                    return BOOLEAN;
                }

                case Const.T_BYTE:
                {
                    return BYTE;
                }

                case Const.T_SHORT:
                {
                    return SHORT;
                }

                case Const.T_CHAR:
                {
                    return CHAR;
                }

                case Const.T_INT:
                {
                    return INT;
                }

                case Const.T_LONG:
                {
                    return LONG;
                }

                case Const.T_DOUBLE:
                {
                    return DOUBLE;
                }

                case Const.T_FLOAT:
                {
                    return FLOAT;
                }

                default:
                {
                    throw new ClassGenException("Invalid type: " + type);
                }
            }
        }

        /// <returns>a hash code value for the object.</returns>
        public override int GetHashCode()
        {
            return base.GetType();
        }

        /// <returns>true if both type objects refer to the same type</returns>
        public override bool Equals(object _type)
        {
            return _type is BasicType
                ? ((BasicType) _type).GetType
                      () == GetType()
                : false;
        }
    }
}