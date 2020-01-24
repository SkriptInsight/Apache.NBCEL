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
    public class AnnotationElementValueGen : ElementValueGen
    {
        private readonly AnnotationEntryGen a;

        public AnnotationElementValueGen(AnnotationEntryGen a, ConstantPoolGen
            cpool)
            : base(ANNOTATION, cpool)
        {
            // For annotation element values, this is the annotation
            this.a = a;
        }

        public AnnotationElementValueGen(int type, AnnotationEntryGen annotation
            , ConstantPoolGen cpool)
            : base(type, cpool)
        {
            if (type != ANNOTATION)
                throw new Exception(
                    "Only element values of type annotation can be built with this ctor - type specified: "
                    + type);
            a = annotation;
        }

        public AnnotationElementValueGen(AnnotationElementValue value, ConstantPoolGen
            cpool, bool copyPoolEntries)
            : base(ANNOTATION, cpool)
        {
            a = new AnnotationEntryGen(value.GetAnnotationEntry(), cpool, copyPoolEntries
            );
        }

        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream dos)
        {
            dos.WriteByte(base.GetElementValueType());
            // u1 type of value (ANNOTATION == '@')
            a.Dump(dos);
        }

        public override string StringifyValue()
        {
            throw new Exception("Not implemented yet");
        }

        /// <summary>Return immutable variant of this AnnotationElementValueGen</summary>
        public override ElementValue GetElementValue()
        {
            return new AnnotationElementValue(base.GetElementValueType(), a.GetAnnotation
                (), GetConstantPool().GetConstantPool());
        }

        public virtual AnnotationEntryGen GetAnnotation()
        {
            return a;
        }
    }
}