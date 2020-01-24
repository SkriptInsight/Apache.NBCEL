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
	/// <summary>Super class for all objects that have modifiers like private, final, ...
	/// 	</summary>
	/// <remarks>Super class for all objects that have modifiers like private, final, ... I.e. classes, fields, and methods.
	/// 	</remarks>
	public abstract class AccessFlags
	{
		[System.ObsoleteAttribute(@"(since 6.0) will be made private; do not access directly, use getter/setter"
			)]
		protected internal int access_flags;

		public AccessFlags()
		{
		}

		/// <param name="a">inital access flags</param>
		public AccessFlags(int a)
		{
			// TODO not used externally at present
			access_flags = a;
		}

		/// <returns>Access flags of the object aka. "modifiers".</returns>
		public int GetAccessFlags()
		{
			return access_flags;
		}

		/// <returns>Access flags of the object aka. "modifiers".</returns>
		public int GetModifiers()
		{
			return access_flags;
		}

		/// <summary>Set access flags aka "modifiers".</summary>
		/// <param name="access_flags">Access flags of the object.</param>
		public void SetAccessFlags(int access_flags)
		{
			this.access_flags = access_flags;
		}

		/// <summary>Set access flags aka "modifiers".</summary>
		/// <param name="access_flags">Access flags of the object.</param>
		public void SetModifiers(int access_flags)
		{
			SetAccessFlags(access_flags);
		}

		private void SetFlag(int flag, bool set)
		{
			if ((access_flags & flag) != 0)
			{
				// Flag is set already
				if (!set)
				{
					access_flags ^= flag;
				}
			}
			else if (set)
			{
				// Flag not set
				access_flags |= flag;
			}
		}

		public void IsPublic(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_PUBLIC, flag);
		}

		public bool IsPublic()
		{
			return (access_flags & NBCEL.Const.ACC_PUBLIC) != 0;
		}

		public void IsPrivate(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_PRIVATE, flag);
		}

		public bool IsPrivate()
		{
			return (access_flags & NBCEL.Const.ACC_PRIVATE) != 0;
		}

		public void IsProtected(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_PROTECTED, flag);
		}

		public bool IsProtected()
		{
			return (access_flags & NBCEL.Const.ACC_PROTECTED) != 0;
		}

		public void IsStatic(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_STATIC, flag);
		}

		public bool IsStatic()
		{
			return (access_flags & NBCEL.Const.ACC_STATIC) != 0;
		}

		public void IsFinal(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_FINAL, flag);
		}

		public bool IsFinal()
		{
			return (access_flags & NBCEL.Const.ACC_FINAL) != 0;
		}

		public void IsSynchronized(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_SYNCHRONIZED, flag);
		}

		public bool IsSynchronized()
		{
			return (access_flags & NBCEL.Const.ACC_SYNCHRONIZED) != 0;
		}

		public void IsVolatile(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_VOLATILE, flag);
		}

		public bool IsVolatile()
		{
			return (access_flags & NBCEL.Const.ACC_VOLATILE) != 0;
		}

		public void IsTransient(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_TRANSIENT, flag);
		}

		public bool IsTransient()
		{
			return (access_flags & NBCEL.Const.ACC_TRANSIENT) != 0;
		}

		public void IsNative(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_NATIVE, flag);
		}

		public bool IsNative()
		{
			return (access_flags & NBCEL.Const.ACC_NATIVE) != 0;
		}

		public void IsInterface(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_INTERFACE, flag);
		}

		public bool IsInterface()
		{
			return (access_flags & NBCEL.Const.ACC_INTERFACE) != 0;
		}

		public void IsAbstract(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_ABSTRACT, flag);
		}

		public bool IsAbstract()
		{
			return (access_flags & NBCEL.Const.ACC_ABSTRACT) != 0;
		}

		public void IsStrictfp(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_STRICT, flag);
		}

		public bool IsStrictfp()
		{
			return (access_flags & NBCEL.Const.ACC_STRICT) != 0;
		}

		public void IsSynthetic(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_SYNTHETIC, flag);
		}

		public bool IsSynthetic()
		{
			return (access_flags & NBCEL.Const.ACC_SYNTHETIC) != 0;
		}

		public void IsAnnotation(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_ANNOTATION, flag);
		}

		public bool IsAnnotation()
		{
			return (access_flags & NBCEL.Const.ACC_ANNOTATION) != 0;
		}

		public void IsEnum(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_ENUM, flag);
		}

		public bool IsEnum()
		{
			return (access_flags & NBCEL.Const.ACC_ENUM) != 0;
		}

		public void IsVarArgs(bool flag)
		{
			SetFlag(NBCEL.Const.ACC_VARARGS, flag);
		}

		public bool IsVarArgs()
		{
			return (access_flags & NBCEL.Const.ACC_VARARGS) != 0;
		}
	}
}
