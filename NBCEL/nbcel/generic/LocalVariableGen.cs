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
using NBCEL.classfile;

namespace NBCEL.generic
{
	/// <summary>Represents a local variable within a method.</summary>
	/// <remarks>
	///     Represents a local variable within a method. It contains its
	///     scope, name and type. The generated LocalVariable object can be obtained
	///     with getLocalVariable which needs the instruction list and the constant
	///     pool as parameters.
	/// </remarks>
	/// <seealso cref="NBCEL.classfile.LocalVariable" />
	/// <seealso cref="MethodGen" />
	public class LocalVariableGen : InstructionTargeter, NamedAndTyped
        , ICloneable
    {
        private InstructionHandle end;
        private int index;

        private bool live_to_end;

        private string name;

        private readonly int orig_index;

        private InstructionHandle start;

        private Type type;

        /// <summary>Generate a local variable that with index `index'.</summary>
        /// <remarks>
        ///     Generate a local variable that with index `index'. Note that double and long
        ///     variables need two indexs. Index indices have to be provided by the user.
        /// </remarks>
        /// <param name="index">index of local variable</param>
        /// <param name="name">its name</param>
        /// <param name="type">its type</param>
        /// <param name="start">
        ///     from where the instruction is valid (null means from the start)
        /// </param>
        /// <param name="end">until where the instruction is valid (null means to the end)</param>
        public LocalVariableGen(int index, string name, Type type, InstructionHandle
            start, InstructionHandle end)
        {
            // never changes; used to match up with LocalVariableTypeTable entries
            if (index < 0 || index > Const.MAX_SHORT) throw new ClassGenException("Invalid index index: " + index);
            this.name = name;
            this.type = type;
            this.index = index;
            SetStart(start);
            SetEnd(end);
            orig_index = index;
            live_to_end = end == null;
        }

        /// <summary>Generates a local variable that with index `index'.</summary>
        /// <remarks>
        ///     Generates a local variable that with index `index'. Note that double and long
        ///     variables need two indexs. Index indices have to be provided by the user.
        /// </remarks>
        /// <param name="index">index of local variable</param>
        /// <param name="name">its name</param>
        /// <param name="type">its type</param>
        /// <param name="start">
        ///     from where the instruction is valid (null means from the start)
        /// </param>
        /// <param name="end">until where the instruction is valid (null means to the end)</param>
        /// <param name="orig_index">index of local variable prior to any changes to index</param>
        public LocalVariableGen(int index, string name, Type type, InstructionHandle
            start, InstructionHandle end, int orig_index)
            : this(index, name, type, start, end)
        {
            this.orig_index = orig_index;
        }

        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }

        /// <param name="old_ih">old target, either start or end</param>
        /// <param name="new_ih">new target</param>
        public virtual void UpdateTarget(InstructionHandle old_ih, InstructionHandle
            new_ih)
        {
            var targeted = false;
            if (start == old_ih)
            {
                targeted = true;
                SetStart(new_ih);
            }

            if (end == old_ih)
            {
                targeted = true;
                SetEnd(new_ih);
            }

            if (!targeted)
                throw new ClassGenException("Not targeting " + old_ih + ", but {" +
                                            start + ", " + end + "}");
        }

        /// <returns>true, if ih is target of this variable</returns>
        public virtual bool ContainsTarget(InstructionHandle ih)
        {
            return start == ih || end == ih;
        }

        public virtual void SetName(string name)
        {
            this.name = name;
        }

        public virtual string GetName()
        {
            return name;
        }

        public virtual void SetType(Type type)
        {
            this.type = type;
        }

        public virtual Type GetType()
        {
            return type;
        }

        /// <summary>Gets LocalVariable object.</summary>
        /// <remarks>
        ///     Gets LocalVariable object.
        ///     This relies on that the instruction list has already been dumped to byte code or
        ///     or that the `setPositions' methods has been called for the instruction list.
        ///     Note that due to the conversion from byte code offset to InstructionHandle,
        ///     it is impossible to tell the difference between a live range that ends BEFORE
        ///     the last insturction of the method or a live range that ends AFTER the last
        ///     instruction of the method.  Hence the live_to_end flag to differentiate
        ///     between these two cases.
        /// </remarks>
        /// <param name="cp">constant pool</param>
        public virtual LocalVariable GetLocalVariable(ConstantPoolGen
            cp)
        {
            var start_pc = 0;
            var length = 0;
            if (start != null && end != null)
            {
                start_pc = start.GetPosition();
                length = end.GetPosition() - start_pc;
                if (end.GetNext() == null && live_to_end) length += end.GetInstruction().GetLength();
            }

            var name_index = cp.AddUtf8(name);
            var signature_index = cp.AddUtf8(type.GetSignature());
            return new LocalVariable(start_pc, length, name_index, signature_index
                , index, cp.GetConstantPool(), orig_index);
        }

        public virtual void SetIndex(int index)
        {
            this.index = index;
        }

        public virtual int GetIndex()
        {
            return index;
        }

        public virtual int GetOrigIndex()
        {
            return orig_index;
        }

        public virtual void SetLiveToEnd(bool live_to_end)
        {
            this.live_to_end = live_to_end;
        }

        public virtual bool GetLiveToEnd()
        {
            return live_to_end;
        }

        public virtual InstructionHandle GetStart()
        {
            return start;
        }

        public virtual InstructionHandle GetEnd()
        {
            return end;
        }

        public virtual void SetStart(InstructionHandle start)
        {
            // TODO could be package-protected?
            BranchInstruction.NotifyTarget(this.start, start, this);
            this.start = start;
        }

        public virtual void SetEnd(InstructionHandle end)
        {
            // TODO could be package-protected?
            BranchInstruction.NotifyTarget(this.end, end, this);
            this.end = end;
        }

        /// <summary>Clear the references from and to this variable when it's removed.</summary>
        internal virtual void Dispose()
        {
            SetStart(null);
            SetEnd(null);
        }

        public override int GetHashCode()
        {
            // If the user changes the name or type, problems with the targeter hashmap will occur.
            // Note: index cannot be part of hash as it may be changed by the user.
            return name.GetHashCode() ^ type.GetHashCode();
        }

        /// <summary>
        ///     We consider to local variables to be equal, if the use the same index and
        ///     are valid in the same range.
        /// </summary>
        public override bool Equals(object o)
        {
            if (!(o is LocalVariableGen)) return false;
            var l = (LocalVariableGen) o;
            return l.index == index && l.start == start && l.end == end;
        }

        public override string ToString()
        {
            return "LocalVariableGen(" + name + ", " + type + ", " + start + ", " + end + ")";
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        // never happens
    }
}