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

using ObjectWeb.Misc.Java.Nio;
using Sharpen;

namespace NBCEL.generic
{
	/// <since>6.0</since>
	public class AnnotationEntryGen
	{
		private int typeIndex;

		private System.Collections.Generic.List<NBCEL.generic.ElementValuePairGen> evs;

		private readonly NBCEL.generic.ConstantPoolGen cpool;

		private bool isRuntimeVisible__ = false;

		/// <summary>
		/// Here we are taking a fixed annotation of type Annotation and building a
		/// modifiable AnnotationGen object.
		/// </summary>
		/// <remarks>
		/// Here we are taking a fixed annotation of type Annotation and building a
		/// modifiable AnnotationGen object. If the pool passed in is for a different
		/// class file, then copyPoolEntries should have been passed as true as that
		/// will force us to do a deep copy of the annotation and move the cpool
		/// entries across. We need to copy the type and the element name value pairs
		/// and the visibility.
		/// </remarks>
		public AnnotationEntryGen(NBCEL.classfile.AnnotationEntry a, NBCEL.generic.ConstantPoolGen
			 cpool, bool copyPoolEntries)
		{
			this.cpool = cpool;
			if (copyPoolEntries)
			{
				typeIndex = cpool.AddUtf8(a.GetAnnotationType());
			}
			else
			{
				typeIndex = a.GetAnnotationTypeIndex();
			}
			isRuntimeVisible__ = a.IsRuntimeVisible();
			evs = CopyValues(a.GetElementValuePairs(), cpool, copyPoolEntries);
		}

		private System.Collections.Generic.List<NBCEL.generic.ElementValuePairGen> CopyValues
			(NBCEL.classfile.ElementValuePair[] @in, NBCEL.generic.ConstantPoolGen cpool, bool
			 copyPoolEntries)
		{
			System.Collections.Generic.List<NBCEL.generic.ElementValuePairGen> @out = new System.Collections.Generic.List
				<NBCEL.generic.ElementValuePairGen>();
			foreach (NBCEL.classfile.ElementValuePair nvp in @in)
			{
				@out.Add(new NBCEL.generic.ElementValuePairGen(nvp, cpool, copyPoolEntries));
			}
			return @out;
		}

		private AnnotationEntryGen(NBCEL.generic.ConstantPoolGen cpool)
		{
			this.cpool = cpool;
		}

		/// <summary>Retrieve an immutable version of this AnnotationGen</summary>
		public virtual NBCEL.classfile.AnnotationEntry GetAnnotation()
		{
			NBCEL.classfile.AnnotationEntry a = new NBCEL.classfile.AnnotationEntry(typeIndex
				, cpool.GetConstantPool(), isRuntimeVisible__);
			foreach (NBCEL.generic.ElementValuePairGen element in evs)
			{
				a.AddElementNameValuePair(element.GetElementNameValuePair());
			}
			return a;
		}

		public AnnotationEntryGen(NBCEL.generic.ObjectType type, System.Collections.Generic.List
			<NBCEL.generic.ElementValuePairGen> elements, bool vis, NBCEL.generic.ConstantPoolGen
			 cpool)
		{
			this.cpool = cpool;
			this.typeIndex = cpool.AddUtf8(type.GetSignature());
			evs = elements;
			isRuntimeVisible__ = vis;
		}

		/// <exception cref="System.IO.IOException"/>
		public static NBCEL.generic.AnnotationEntryGen Read(java.io.DataInput dis, NBCEL.generic.ConstantPoolGen
			 cpool, bool b)
		{
			NBCEL.generic.AnnotationEntryGen a = new NBCEL.generic.AnnotationEntryGen(cpool);
			a.typeIndex = dis.ReadUnsignedShort();
			int elemValuePairCount = dis.ReadUnsignedShort();
			for (int i = 0; i < elemValuePairCount; i++)
			{
				int nidx = dis.ReadUnsignedShort();
				a.AddElementNameValuePair(new NBCEL.generic.ElementValuePairGen(nidx, NBCEL.generic.ElementValueGen
					.ReadElementValue(dis, cpool), cpool));
			}
			a.IsRuntimeVisible(b);
			return a;
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void Dump(java.io.DataOutputStream dos)
		{
			dos.WriteShort(typeIndex);
			// u2 index of type name in cpool
			dos.WriteShort(evs.Count);
			// u2 element_value pair count
			foreach (NBCEL.generic.ElementValuePairGen envp in evs)
			{
				envp.Dump(dos);
			}
		}

		public virtual void AddElementNameValuePair(NBCEL.generic.ElementValuePairGen evp
			)
		{
			if (evs == null)
			{
				evs = new System.Collections.Generic.List<NBCEL.generic.ElementValuePairGen>();
			}
			evs.Add(evp);
		}

		public virtual int GetTypeIndex()
		{
			return typeIndex;
		}

		public string GetTypeSignature()
		{
			// ConstantClass c = (ConstantClass)cpool.getConstant(typeIndex);
			NBCEL.classfile.ConstantUtf8 utf8 = (NBCEL.classfile.ConstantUtf8)cpool.GetConstant
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
		public virtual System.Collections.Generic.List<NBCEL.generic.ElementValuePairGen>
			 GetValues()
		{
			return evs;
		}

		public override string ToString()
		{
			System.Text.StringBuilder s = new System.Text.StringBuilder(32);
			// CHECKSTYLE IGNORE MagicNumber
			s.Append("AnnotationGen:[").Append(GetTypeName()).Append(" #").Append(evs.Count).
				Append(" {");
			for (int i = 0; i < evs.Count; i++)
			{
				s.Append(evs[i]);
				if (i + 1 < evs.Count)
				{
					s.Append(",");
				}
			}
			s.Append("}]");
			return s.ToString();
		}

		public virtual string ToShortString()
		{
			System.Text.StringBuilder s = new System.Text.StringBuilder();
			s.Append("@").Append(GetTypeName()).Append("(");
			for (int i = 0; i < evs.Count; i++)
			{
				s.Append(evs[i]);
				if (i + 1 < evs.Count)
				{
					s.Append(",");
				}
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
		/// Converts a list of AnnotationGen objects into a set of attributes
		/// that can be attached to the class file.
		/// </summary>
		/// <param name="cp">The constant pool gen where we can create the necessary name refs
		/// 	</param>
		/// <param name="annotationEntryGens">An array of AnnotationGen objects</param>
		internal static NBCEL.classfile.Attribute[] GetAnnotationAttributes(NBCEL.generic.ConstantPoolGen
			 cp, NBCEL.generic.AnnotationEntryGen[] annotationEntryGens)
		{
			if (annotationEntryGens.Length == 0)
			{
				return new NBCEL.classfile.Attribute[0];
			}
			try
			{
				int countVisible = 0;
				int countInvisible = 0;
				//  put the annotations in the right output stream
				foreach (NBCEL.generic.AnnotationEntryGen a in annotationEntryGens)
				{
					if (a.IsRuntimeVisible())
					{
						countVisible++;
					}
					else
					{
						countInvisible++;
					}
				}
				System.IO.MemoryStream rvaBytes = new System.IO.MemoryStream();
				System.IO.MemoryStream riaBytes = new System.IO.MemoryStream();
				using (java.io.DataOutputStream rvaDos = new java.io.DataOutputStream(rvaBytes.ToOutputStream()))
				{
					using (java.io.DataOutputStream riaDos = new java.io.DataOutputStream(riaBytes.ToOutputStream()))
					{
						rvaDos.WriteShort(countVisible);
						riaDos.WriteShort(countInvisible);
						// put the annotations in the right output stream
						foreach (NBCEL.generic.AnnotationEntryGen a in annotationEntryGens)
						{
							if (a.IsRuntimeVisible())
							{
								a.Dump(rvaDos);
							}
							else
							{
								a.Dump(riaDos);
							}
						}
					}
				}
				byte[] rvaData = rvaBytes.ToArray();
				byte[] riaData = riaBytes.ToArray();
				int rvaIndex = -1;
				int riaIndex = -1;
				if (rvaData.Length > 2)
				{
					rvaIndex = cp.AddUtf8("RuntimeVisibleAnnotations");
				}
				if (riaData.Length > 2)
				{
					riaIndex = cp.AddUtf8("RuntimeInvisibleAnnotations");
				}
				System.Collections.Generic.List<NBCEL.classfile.Attribute> newAttributes = new System.Collections.Generic.List
					<NBCEL.classfile.Attribute>();
				if (rvaData.Length > 2)
				{
					newAttributes.Add(new NBCEL.classfile.RuntimeVisibleAnnotations(rvaIndex, rvaData
						.Length, new java.io.DataInputStream((rvaData).ToInputStream()), 
						cp.GetConstantPool()));
				}
				if (riaData.Length > 2)
				{
					newAttributes.Add(new NBCEL.classfile.RuntimeInvisibleAnnotations(riaIndex, riaData
						.Length, new java.io.DataInputStream((riaData).ToInputStream()), 
						cp.GetConstantPool()));
				}
				return Sharpen.Collections.ToArray(newAttributes, new NBCEL.classfile.Attribute[newAttributes
					.Count]);
			}
			catch (System.IO.IOException e)
			{
				System.Console.Error.WriteLine("IOException whilst processing annotations");
				Sharpen.Runtime.PrintStackTrace(e);
			}
			return null;
		}

		/// <summary>
		/// Annotations against a class are stored in one of four attribute kinds:
		/// - RuntimeVisibleParameterAnnotations
		/// - RuntimeInvisibleParameterAnnotations
		/// </summary>
		internal static NBCEL.classfile.Attribute[] GetParameterAnnotationAttributes(NBCEL.generic.ConstantPoolGen
			 cp, System.Collections.Generic.List<NBCEL.generic.AnnotationEntryGen>[] vec)
		{
			/*Array of lists, array size depends on #params */
			int[] visCount = new int[vec.Length];
			int totalVisCount = 0;
			int[] invisCount = new int[vec.Length];
			int totalInvisCount = 0;
			try
			{
				for (int i = 0; i < vec.Length; i++)
				{
					if (vec[i] != null)
					{
						foreach (NBCEL.generic.AnnotationEntryGen element in vec[i])
						{
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
						}
					}
				}
				// Lets do the visible ones
				System.IO.MemoryStream rvaBytes = new System.IO.MemoryStream();
				using (java.io.DataOutputStream rvaDos = new java.io.DataOutputStream(rvaBytes.ToOutputStream()))
				{
					rvaDos.WriteByte(vec.Length);
					// First goes number of parameters
					for (int i = 0; i < vec.Length; i++)
					{
						rvaDos.WriteShort(visCount[i]);
						if (visCount[i] > 0)
						{
							foreach (NBCEL.generic.AnnotationEntryGen element in vec[i])
							{
								if (element.IsRuntimeVisible())
								{
									element.Dump(rvaDos);
								}
							}
						}
					}
				}
				// Lets do the invisible ones
				System.IO.MemoryStream riaBytes = new System.IO.MemoryStream();
				using (java.io.DataOutputStream riaDos = new java.io.DataOutputStream(riaBytes.ToOutputStream()))
				{
					riaDos.WriteByte(vec.Length);
					// First goes number of parameters
					for (int i = 0; i < vec.Length; i++)
					{
						riaDos.WriteShort(invisCount[i]);
						if (invisCount[i] > 0)
						{
							foreach (NBCEL.generic.AnnotationEntryGen element in vec[i])
							{
								if (!element.IsRuntimeVisible())
								{
									element.Dump(riaDos);
								}
							}
						}
					}
				}
				byte[] rvaData = rvaBytes.ToArray();
				byte[] riaData = riaBytes.ToArray();
				int rvaIndex = -1;
				int riaIndex = -1;
				if (totalVisCount > 0)
				{
					rvaIndex = cp.AddUtf8("RuntimeVisibleParameterAnnotations");
				}
				if (totalInvisCount > 0)
				{
					riaIndex = cp.AddUtf8("RuntimeInvisibleParameterAnnotations");
				}
				System.Collections.Generic.List<NBCEL.classfile.Attribute> newAttributes = new System.Collections.Generic.List
					<NBCEL.classfile.Attribute>();
				if (totalVisCount > 0)
				{
					newAttributes.Add(new NBCEL.classfile.RuntimeVisibleParameterAnnotations(rvaIndex
						, rvaData.Length, new java.io.DataInputStream((rvaData
						).ToInputStream()), cp.GetConstantPool()));
				}
				if (totalInvisCount > 0)
				{
					newAttributes.Add(new NBCEL.classfile.RuntimeInvisibleParameterAnnotations(riaIndex
						, riaData.Length, new java.io.DataInputStream((riaData).ToInputStream()), cp.GetConstantPool()));
				}
				return Sharpen.Collections.ToArray(newAttributes, new NBCEL.classfile.Attribute[newAttributes
					.Count]);
			}
			catch (System.IO.IOException e)
			{
				System.Console.Error.WriteLine("IOException whilst processing parameter annotations"
					);
				Sharpen.Runtime.PrintStackTrace(e);
			}
			return null;
		}
	}
}
