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
*/
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>This class represents an entry in the exports table of the Module attribute.
	/// 	</summary>
	/// <remarks>
	/// This class represents an entry in the exports table of the Module attribute.
	/// Each entry describes a package which may open the parent module.
	/// </remarks>
	/// <seealso cref="Module"/>
	/// <since>6.4.0</since>
	public sealed class ModuleExports : System.ICloneable, NBCEL.classfile.Node
	{
		private readonly int exports_index;

		private readonly int exports_flags;

		private readonly int exports_to_count;

		private readonly int[] exports_to_index;

		/// <summary>Construct object from file stream.</summary>
		/// <param name="file">Input stream</param>
		/// <exception cref="System.IO.IOException">if an I/O Exception occurs in readUnsignedShort
		/// 	</exception>
		internal ModuleExports(java.io.DataInput file)
		{
			// points to CONSTANT_Package_info
			// points to CONSTANT_Module_info
			exports_index = file.ReadUnsignedShort();
			exports_flags = file.ReadUnsignedShort();
			exports_to_count = file.ReadUnsignedShort();
			exports_to_index = new int[exports_to_count];
			for (int i = 0; i < exports_to_count; i++)
			{
				exports_to_index[i] = file.ReadUnsignedShort();
			}
		}

		/// <summary>
		/// Called by objects that are traversing the nodes of the tree implicitely
		/// defined by the contents of a Java class.
		/// </summary>
		/// <remarks>
		/// Called by objects that are traversing the nodes of the tree implicitely
		/// defined by the contents of a Java class. I.e., the hierarchy of methods,
		/// fields, attributes, etc. spawns a tree of objects.
		/// </remarks>
		/// <param name="v">Visitor object</param>
		public void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitModuleExports(this);
		}

		// TODO add more getters and setters?
		/// <summary>Dump table entry to file stream in binary format.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException">if an I/O Exception occurs in writeShort</exception>
		public void Dump(java.io.DataOutputStream file)
		{
			file.WriteShort(exports_index);
			file.WriteShort(exports_flags);
			file.WriteShort(exports_to_count);
			foreach (int entry in exports_to_index)
			{
				file.WriteShort(entry);
			}
		}

		/// <returns>String representation</returns>
		public override string ToString()
		{
			return "exports(" + exports_index + ", " + exports_flags + ", " + exports_to_count
				 + ", ...)";
		}

		/// <returns>Resolved string representation</returns>
		public string ToString(NBCEL.classfile.ConstantPool constant_pool)
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			string package_name = constant_pool.ConstantToString(exports_index, NBCEL.Const.CONSTANT_Package
				);
			buf.Append(NBCEL.classfile.Utility.CompactClassName(package_name, false));
			buf.Append(", ").Append(string.Format("%04x", exports_flags));
			buf.Append(", to(").Append(exports_to_count).Append("):\n");
			foreach (int index in exports_to_index)
			{
				string module_name = constant_pool.GetConstantString(index, NBCEL.Const.CONSTANT_Module
					);
				buf.Append("      ").Append(NBCEL.classfile.Utility.CompactClassName(module_name, 
					false)).Append("\n");
			}
			return buf.Substring(0, buf.Length - 1);
		}

		// remove the last newline
		/// <returns>deep copy of this object</returns>
		public NBCEL.classfile.ModuleExports Copy()
		{
			return (NBCEL.classfile.ModuleExports)MemberwiseClone();
			// TODO should this throw?
			return null;
		}

		object System.ICloneable.Clone()
		{
			return MemberwiseClone();
		}
	}
}
