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
	/// <summary>Denotes basic type such as int.</summary>
	public sealed class BasicType : NBCEL.generic.Type
	{
		/// <summary>Constructor for basic types such as int, long, `void'</summary>
		/// <param name="type">one of T_INT, T_BOOLEAN, ..., T_VOID</param>
		/// <seealso cref="NBCEL.Const"/>
		internal BasicType(byte type)
			: base(type, NBCEL.Const.GetShortTypeName(type))
		{
			if ((((sbyte)type) < NBCEL.Const.T_BOOLEAN) || (type > NBCEL.Const.T_VOID))
			{
				throw new NBCEL.generic.ClassGenException("Invalid type: " + type);
			}
		}

		// @since 6.0 no longer final
		public static NBCEL.generic.BasicType GetType(byte type)
		{
			switch (type)
			{
				case NBCEL.Const.T_VOID:
				{
					return VOID;
				}

				case NBCEL.Const.T_BOOLEAN:
				{
					return BOOLEAN;
				}

				case NBCEL.Const.T_BYTE:
				{
					return BYTE;
				}

				case NBCEL.Const.T_SHORT:
				{
					return SHORT;
				}

				case NBCEL.Const.T_CHAR:
				{
					return CHAR;
				}

				case NBCEL.Const.T_INT:
				{
					return INT;
				}

				case NBCEL.Const.T_LONG:
				{
					return LONG;
				}

				case NBCEL.Const.T_DOUBLE:
				{
					return DOUBLE;
				}

				case NBCEL.Const.T_FLOAT:
				{
					return FLOAT;
				}

				default:
				{
					throw new NBCEL.generic.ClassGenException("Invalid type: " + type);
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
			return (_type is NBCEL.generic.BasicType) ? ((NBCEL.generic.BasicType)_type).GetType
				() == this.GetType() : false;
		}
	}
}
