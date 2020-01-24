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
	/// <summary>Denotes array type, such as int[][]</summary>
	public sealed class ArrayType : NBCEL.generic.ReferenceType
	{
		private int dimensions;

		private NBCEL.generic.Type basic_type;

		/// <summary>Convenience constructor for array type, e.g.</summary>
		/// <remarks>Convenience constructor for array type, e.g. int[]</remarks>
		/// <param name="type">array type, e.g. T_INT</param>
		public ArrayType(byte type, int dimensions)
			: this(NBCEL.generic.BasicType.GetType(type), dimensions)
		{
		}

		/// <summary>Convenience constructor for reference array type, e.g.</summary>
		/// <remarks>Convenience constructor for reference array type, e.g. Object[]</remarks>
		/// <param name="class_name">complete name of class (java.lang.String, e.g.)</param>
		public ArrayType(string class_name, int dimensions)
			: this(NBCEL.generic.ObjectType.GetInstance(class_name), dimensions)
		{
		}

		/// <summary>Constructor for array of given type</summary>
		/// <param name="type">type of array (may be an array itself)</param>
		public ArrayType(NBCEL.generic.Type type, int dimensions)
			: base(NBCEL.Const.T_ARRAY, "<dummy>")
		{
			if ((dimensions < 1) || (dimensions > NBCEL.Const.MAX_BYTE))
			{
				throw new NBCEL.generic.ClassGenException("Invalid number of dimensions: " + dimensions
					);
			}
			switch (type.GetType())
			{
				case NBCEL.Const.T_ARRAY:
				{
					NBCEL.generic.ArrayType array = (NBCEL.generic.ArrayType)type;
					this.dimensions = dimensions + array.dimensions;
					basic_type = array.basic_type;
					break;
				}

				case NBCEL.Const.T_VOID:
				{
					throw new NBCEL.generic.ClassGenException("Invalid type: void[]");
				}

				default:
				{
					// Basic type or reference
					this.dimensions = dimensions;
					basic_type = type;
					break;
				}
			}
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			for (int i = 0; i < this.dimensions; i++)
			{
				buf.Append('[');
			}
			buf.Append(basic_type.GetSignature());
			base.SetSignature(buf.ToString());
		}

		/// <returns>basic type of array, i.e., for int[][][] the basic type is int</returns>
		public NBCEL.generic.Type GetBasicType()
		{
			return basic_type;
		}

		/// <returns>element type of array, i.e., for int[][][] the element type is int[][]</returns>
		public NBCEL.generic.Type GetElementType()
		{
			if (dimensions == 1)
			{
				return basic_type;
			}
			return new NBCEL.generic.ArrayType(basic_type, dimensions - 1);
		}

		/// <returns>number of dimensions of array</returns>
		public int GetDimensions()
		{
			return dimensions;
		}

		/// <returns>a hash code value for the object.</returns>
		public override int GetHashCode()
		{
			return basic_type.GetHashCode() ^ dimensions;
		}

		/// <returns>true if both type objects refer to the same array type.</returns>
		public override bool Equals(object _type)
		{
			if (_type is NBCEL.generic.ArrayType)
			{
				NBCEL.generic.ArrayType array = (NBCEL.generic.ArrayType)_type;
				return (array.dimensions == dimensions) && array.basic_type.Equals(basic_type);
			}
			return false;
		}
	}
}
