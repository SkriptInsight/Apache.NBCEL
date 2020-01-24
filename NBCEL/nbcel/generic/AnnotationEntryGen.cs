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
using System.Collections.Generic;
using System.IO;
using System.Text;
using java.io;
using NBCEL.classfile;
using ObjectWeb.Misc.Java.Nio;
using Sharpen;
using Attribute = NBCEL.classfile.Attribute;

namespace NBCEL.generic
{
    /// <since>6.0</since>
    public class AnnotationEntryGen
    {
        private readonly ConstantPoolGen cpool;

        private List<ElementValuePairGen> evs;

        private bool isRuntimeVisible__;
        private int typeIndex;

        /// <summary>
        ///     Here we are taking a fixed annotation of type Annotation and building a
        ///     modifiable AnnotationGen object.
        /// </summary>
        /// <remarks>
        ///     Here we are taking a fixed annotation of type Annotation and building a
        ///     modifiable AnnotationGen object. If the pool passed in is for a different
        ///     class file, then copyPoolEntries should have been passed as true as that
        ///     will force us to do a deep copy of the annotation and move the cpool
        ///     entries across. We need to copy the type and the element name value pairs
        ///     and the visibility.
        /// </remarks>
        public AnnotationEntryGen(AnnotationEntry a, ConstantPoolGen
            cpool, bool copyPoolEntries)
        {
            this.cpool = cpool;
            if (copyPoolEntries)
                typeIndex = cpool.AddUtf8(a.GetAnnotationType());
            else
                typeIndex = a.GetAnnotationTypeIndex();
            isRuntimeVisible__ = a.IsRuntimeVisible();
            evs = CopyValues(a.GetElementValuePairs(), cpool, copyPoolEntries);
        }

        private AnnotationEntryGen(ConstantPoolGen cpool)
        {
            this.cpool = cpool;
        }

        public AnnotationEntryGen(ObjectType type, List
            <ElementValuePairGen> elements, bool vis, ConstantPoolGen
            cpool)
        {
            this.cpool = cpool;
            typeIndex = cpool.AddUtf8(type.GetSignature());
            evs = elements;
            isRuntimeVisible__ = vis;
        }

        private List<ElementValuePairGen> CopyValues
        (ElementValuePair[] @in, ConstantPoolGen cpool, bool
            copyPoolEntries)
        {
            var @out = new List
                <ElementValuePairGen>();
            foreach (var nvp in @in) @out.Add(new ElementValuePairGen(nvp, cpool, copyPoolEntries));
            return @out;
        }

        /// <summary>Retrieve an immutable version of this AnnotationGen</summary>
        public virtual AnnotationEntry GetAnnotation()
        {
            var a = new AnnotationEntry(typeIndex
                , cpool.GetConstantPool(), isRuntimeVisible__);
            foreach (var element in evs) a.AddElementNameValuePair(element.GetElementNameValuePair());
            return a;
        }

        /// <exception cref="System.IO.IOException" />
        public static AnnotationEntryGen Read(DataInput dis, ConstantPoolGen
            cpool, bool b)
        {
            var a = new AnnotationEntryGen(cpool) {typeIndex = dis.ReadUnsignedShort()};
            var elemValuePairCount = dis.ReadUnsignedShort();
            for (var i = 0; i < elemValuePairCount; i++)
            {
                var nidx = dis.ReadUnsignedShort();
                a.AddElementNameValuePair(new ElementValuePairGen(nidx, ElementValueGen
                    .ReadElementValue(dis, cpool), cpool));
            }

            a.IsRuntimeVisible(b);
            return a;
        }

        /// <exception cref="System.IO.IOException" />
        public virtual void Dump(DataOutputStream dos)
        {
            dos.WriteShort(typeIndex);
            // u2 index of type name in cpool
            dos.WriteShort(evs.Count);
            // u2 element_value pair count
            foreach (var envp in evs) envp.Dump(dos);
        }

        public virtual void AddElementNameValuePair(ElementValuePairGen evp
        )
        {
            if (evs == null) evs = new List<ElementValuePairGen>();
            evs.Add(evp);
        }

        public virtual int GetTypeIndex()
        {
            return typeIndex;
        }

        public string GetTypeSignature()
        {
            // ConstantClass c = (ConstantClass)cpool.getConstant(typeIndex);
            var utf8 = (ConstantUtf8) cpool.GetConstant
                (typeIndex);
            /* c.getNameIndex() */
            return utf8.GetBytes();
        }

        public string GetTypeName()
        {
            return GetTypeSignature();
        }

        // BCELBUG: Should I use this instead?
        // Utility.signatureToString(getTypeSignature());
        /// <summary>Returns list of ElementNameValuePair objects</summary>
        public virtual List<ElementValuePairGen>
            GetValues()
        {
            return evs;
        }

        public override string ToString()
        {
            var s = new StringBuilder(32);
            // CHECKSTYLE IGNORE MagicNumber
            s.Append("AnnotationGen:[").Append(GetTypeName()).Append(" #").Append(evs.Count).Append(" {");
            for (var i = 0; i < evs.Count; i++)
            {
                s.Append(evs[i]);
                if (i + 1 < evs.Count) s.Append(",");
            }

            s.Append("}]");
            return s.ToString();
        }

        public virtual string ToShortString()
        {
            var s = new StringBuilder();
            s.Append("@").Append(GetTypeName()).Append("(");
            for (var i = 0; i < evs.Count; i++)
            {
                s.Append(evs[i]);
                if (i + 1 < evs.Count) s.Append(",");
            }

            s.Append(")");
            return s.ToString();
        }

        private void IsRuntimeVisible(bool b)
        {
            isRuntimeVisible__ = b;
        }

        public virtual bool IsRuntimeVisible()
        {
            return isRuntimeVisible__;
        }

        /// <summary>
        ///     Converts a list of AnnotationGen objects into a set of attributes
        ///     that can be attached to the class file.
        /// </summary>
        /// <param name="cp">
        ///     The constant pool gen where we can create the necessary name refs
        /// </param>
        /// <param name="annotationEntryGens">An array of AnnotationGen objects</param>
        internal static Attribute[] GetAnnotationAttributes(ConstantPoolGen
            cp, AnnotationEntryGen[] annotationEntryGens)
        {
            if (annotationEntryGens.Length == 0) return new Attribute[0];
            try
            {
                var countVisible = 0;
                var countInvisible = 0;
                //  put the annotations in the right output stream
                foreach (var a in annotationEntryGens)
                    if (a.IsRuntimeVisible())
                        countVisible++;
                    else
                        countInvisible++;
                var rvaBytes = new MemoryStream();
                var riaBytes = new MemoryStream();
                using (var rvaDos = new DataOutputStream(rvaBytes.ToOutputStream()))
                {
                    using (var riaDos = new DataOutputStream(riaBytes.ToOutputStream()))
                    {
                        rvaDos.WriteShort(countVisible);
                        riaDos.WriteShort(countInvisible);
                        // put the annotations in the right output stream
                        foreach (var a in annotationEntryGens)
                            if (a.IsRuntimeVisible())
                                a.Dump(rvaDos);
                            else
                                a.Dump(riaDos);
                    }
                }

                var rvaData = rvaBytes.ToArray();
                var riaData = riaBytes.ToArray();
                var rvaIndex = -1;
                var riaIndex = -1;
                if (rvaData.Length > 2) rvaIndex = cp.AddUtf8("RuntimeVisibleAnnotations");
                if (riaData.Length > 2) riaIndex = cp.AddUtf8("RuntimeInvisibleAnnotations");
                var newAttributes = new List
                    <Attribute>();
                if (rvaData.Length > 2)
                    newAttributes.Add(new RuntimeVisibleAnnotations(rvaIndex, rvaData
                            .Length, new DataInputStream(rvaData.ToInputStream()),
                        cp.GetConstantPool()));
                if (riaData.Length > 2)
                    newAttributes.Add(new RuntimeInvisibleAnnotations(riaIndex, riaData
                            .Length, new DataInputStream(riaData.ToInputStream()),
                        cp.GetConstantPool()));
                return Collections.ToArray(newAttributes, new Attribute[newAttributes
                    .Count]);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("IOException whilst processing annotations");
                Runtime.PrintStackTrace(e);
            }

            return null;
        }

        /// <summary>
        ///     Annotations against a class are stored in one of four attribute kinds:
        ///     - RuntimeVisibleParameterAnnotations
        ///     - RuntimeInvisibleParameterAnnotations
        /// </summary>
        internal static Attribute[] GetParameterAnnotationAttributes(ConstantPoolGen
            cp, List<AnnotationEntryGen>[] vec)
        {
            /*Array of lists, array size depends on #params */
            var visCount = new int[vec.Length];
            var totalVisCount = 0;
            var invisCount = new int[vec.Length];
            var totalInvisCount = 0;
            try
            {
                for (var i = 0; i < vec.Length; i++)
                    if (vec[i] != null)
                        foreach (var element in vec[i])
                            if (element.IsRuntimeVisible())
                            {
                                visCount[i]++;
                                totalVisCount++;
                            }
                            else
                            {
                                invisCount[i]++;
                                totalInvisCount++;
                            }

                // Lets do the visible ones
                var rvaBytes = new MemoryStream();
                using (var rvaDos = new DataOutputStream(rvaBytes.ToOutputStream()))
                {
                    rvaDos.WriteByte(vec.Length);
                    // First goes number of parameters
                    for (var i = 0; i < vec.Length; i++)
                    {
                        rvaDos.WriteShort(visCount[i]);
                        if (visCount[i] > 0)
                            foreach (var element in vec[i])
                                if (element.IsRuntimeVisible())
                                    element.Dump(rvaDos);
                    }
                }

                // Lets do the invisible ones
                var riaBytes = new MemoryStream();
                using (var riaDos = new DataOutputStream(riaBytes.ToOutputStream()))
                {
                    riaDos.WriteByte(vec.Length);
                    // First goes number of parameters
                    for (var i = 0; i < vec.Length; i++)
                    {
                        riaDos.WriteShort(invisCount[i]);
                        if (invisCount[i] > 0)
                            foreach (var element in vec[i])
                                if (!element.IsRuntimeVisible())
                                    element.Dump(riaDos);
                    }
                }

                var rvaData = rvaBytes.ToArray();
                var riaData = riaBytes.ToArray();
                var rvaIndex = -1;
                var riaIndex = -1;
                if (totalVisCount > 0) rvaIndex = cp.AddUtf8("RuntimeVisibleParameterAnnotations");
                if (totalInvisCount > 0) riaIndex = cp.AddUtf8("RuntimeInvisibleParameterAnnotations");
                var newAttributes = new List
                    <Attribute>();
                if (totalVisCount > 0)
                    newAttributes.Add(new RuntimeVisibleParameterAnnotations(rvaIndex
                        , rvaData.Length, new DataInputStream(rvaData.ToInputStream()), cp.GetConstantPool()));
                if (totalInvisCount > 0)
                    newAttributes.Add(new RuntimeInvisibleParameterAnnotations(riaIndex
                        , riaData.Length, new DataInputStream(riaData.ToInputStream()), cp.GetConstantPool()));
                return Collections.ToArray(newAttributes, new Attribute[newAttributes
                    .Count]);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("IOException whilst processing parameter annotations"
                );
                Runtime.PrintStackTrace(e);
            }

            return null;
        }
    }
}