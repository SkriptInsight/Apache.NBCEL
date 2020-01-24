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
using Sharpen;

namespace NBCEL
{
	/// <summary>Exception constants.</summary>
	/// <since>6.0 (intended to replace the InstructionConstant interface)</since>
	public sealed class ExceptionConst
	{
		/// <summary>The mother of all exceptions</summary>
		public static readonly System.Type THROWABLE = typeof(System.Exception);

		/// <summary>Super class of any run-time exception</summary>
		public static readonly System.Type RUNTIME_EXCEPTION = typeof(System.Exception);

		public static readonly System.Type INCOMPATIBLE_CLASS_CHANGE_ERROR = typeof(System.Exception
			);

		public static readonly System.Type ABSTRACT_METHOD_ERROR = typeof(System.Exception
			);

		public static readonly System.Type ILLEGAL_ACCESS_ERROR = typeof(System.Exception
			);

		public static readonly System.Type INSTANTIATION_ERROR = typeof(System.Exception
			);

		public static readonly System.Type NO_SUCH_FIELD_ERROR = typeof(System.Exception
			);

		public static readonly System.Type NO_SUCH_METHOD_ERROR = typeof(System.MissingMethodException
			);

		public static readonly System.Type NO_CLASS_DEF_FOUND_ERROR = typeof(System.Exception
			);

		public static readonly System.Type UNSATISFIED_LINK_ERROR = typeof(System.Exception
			);

		public static readonly System.Type VERIFY_ERROR = typeof(System.Exception);

		/// <summary>Run-Time Exceptions</summary>
		public static readonly System.Type NULL_POINTER_EXCEPTION = typeof(System.ArgumentNullException
			);

		public static readonly System.Type ARRAY_INDEX_OUT_OF_BOUNDS_EXCEPTION = typeof(System.IndexOutOfRangeException
			);

		public static readonly System.Type ARITHMETIC_EXCEPTION = typeof(System.Exception
			);

		public static readonly System.Type NEGATIVE_ARRAY_SIZE_EXCEPTION = typeof(System.Exception
			);

		public static readonly System.Type CLASS_CAST_EXCEPTION = typeof(System.InvalidCastException
			);

		public static readonly System.Type ILLEGAL_MONITOR_STATE = typeof(System.Exception
			);

		/// <summary>
		/// Pre-defined exception arrays according to chapters 5.1-5.4 of the Java Virtual
		/// Machine Specification
		/// </summary>
		private static readonly System.Type[] EXCS_CLASS_AND_INTERFACE_RESOLUTION = new System.Type
			[] { typeof(System.Exception), typeof(System.Exception), 
			typeof(System.Exception), typeof(System.Exception), typeof(System.Exception
			), typeof(System.Exception) };

		private static readonly System.Type[] EXCS_FIELD_AND_METHOD_RESOLUTION = new System.Type
			[] { typeof(System.Exception), typeof(System.Exception), typeof(
			System.MissingMethodException) };

		private static readonly System.Type[] EXCS_INTERFACE_METHOD_RESOLUTION = new System.Type
			[0];

		private static readonly System.Type[] EXCS_STRING_RESOLUTION = new System.Type[0]
			;

		private static readonly System.Type[] EXCS_ARRAY_EXCEPTION = new System.Type[] { 
			typeof(System.ArgumentNullException), typeof(System.IndexOutOfRangeException) };

		/// <summary>
		/// Enum corresponding to the various Exception Class arrays,
		/// used by
		/// <see cref="ExceptionConst.CreateExceptions(EXCS, System.Type{T}[])"/>
		/// </summary>
		[System.Serializable]
		public sealed class EXCS : Sharpen.EnumBase
		{
			public static readonly NBCEL.ExceptionConst.EXCS EXCS_CLASS_AND_INTERFACE_RESOLUTION
				 = new NBCEL.ExceptionConst.EXCS(0, "EXCS_CLASS_AND_INTERFACE_RESOLUTION");

			public static readonly NBCEL.ExceptionConst.EXCS EXCS_FIELD_AND_METHOD_RESOLUTION
				 = new NBCEL.ExceptionConst.EXCS(1, "EXCS_FIELD_AND_METHOD_RESOLUTION");

			public static readonly NBCEL.ExceptionConst.EXCS EXCS_INTERFACE_METHOD_RESOLUTION
				 = new NBCEL.ExceptionConst.EXCS(2, "EXCS_INTERFACE_METHOD_RESOLUTION");

			public static readonly NBCEL.ExceptionConst.EXCS EXCS_STRING_RESOLUTION = new NBCEL.ExceptionConst.EXCS
				(3, "EXCS_STRING_RESOLUTION");

			public static readonly NBCEL.ExceptionConst.EXCS EXCS_ARRAY_EXCEPTION = new NBCEL.ExceptionConst.EXCS
				(4, "EXCS_ARRAY_EXCEPTION");

			private EXCS(int ordinal, string name)
				: base(ordinal, name)
			{
			}

			public static EXCS[] Values()
			{
				return new EXCS[] { EXCS_CLASS_AND_INTERFACE_RESOLUTION, EXCS_FIELD_AND_METHOD_RESOLUTION
					, EXCS_INTERFACE_METHOD_RESOLUTION, EXCS_STRING_RESOLUTION, EXCS_ARRAY_EXCEPTION
					 };
			}

			static EXCS()
			{
				RegisterValues<EXCS>(Values());
			}
			/* UnsupportedClassVersionError is new in JDK 1.2 */
			//    public static final Class UnsupportedClassVersionError = UnsupportedClassVersionError.class;
			// Chapter 5.1
			// Chapter 5.2
			// Chapter 5.3 (as below)
			// Chapter 5.4 (no errors but the ones that _always_ could happen! How stupid.)
		}

		// helper method to merge exception class arrays
		private static System.Type[] MergeExceptions(System.Type[] input, params System.Type
			[] extraClasses)
		{
			int extraLen = extraClasses == null ? 0 : extraClasses.Length;
			var excs = new System.Type[input.Length + extraLen];
			System.Array.Copy(input, 0, excs, 0, input.Length);
			if (extraLen > 0)
			{
				System.Array.Copy(extraClasses, 0, excs, input.Length, extraLen);
			}
			return excs;
		}

		/// <summary>Creates a copy of the specified Exception Class array combined with any additional Exception classes.
		/// 	</summary>
		/// <param name="type">the basic array type</param>
		/// <param name="extraClasses">additional classes, if any</param>
		/// <returns>the merged array</returns>
		public static System.Type[] CreateExceptions(NBCEL.ExceptionConst.EXCS type, params 
			System.Type[] extraClasses)
		{
			switch (type.ordinal())
			{
				case 0:
				{
					return MergeExceptions(EXCS_CLASS_AND_INTERFACE_RESOLUTION, extraClasses);
				}

				case 4:
				{
					return MergeExceptions(EXCS_ARRAY_EXCEPTION, extraClasses);
				}

				case 1:
				{
					return MergeExceptions(EXCS_FIELD_AND_METHOD_RESOLUTION, extraClasses);
				}

				case 2:
				{
					return MergeExceptions(EXCS_INTERFACE_METHOD_RESOLUTION, extraClasses);
				}

				case 3:
				{
					return MergeExceptions(EXCS_STRING_RESOLUTION, extraClasses);
				}

				default:
				{
					throw new Exception("Cannot happen; unexpected enum value: " + type
						);
				}
			}
		}
	}
}
