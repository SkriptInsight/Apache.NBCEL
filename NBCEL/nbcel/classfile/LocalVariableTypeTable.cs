using Sharpen;

namespace NBCEL.classfile
{
	/// <since>6.0</since>
	public class LocalVariableTypeTable : NBCEL.classfile.Attribute
	{
		private NBCEL.classfile.LocalVariable[] local_variable_type_table;

		public LocalVariableTypeTable(NBCEL.classfile.LocalVariableTypeTable c)
			: this(c.GetNameIndex(), c.GetLength(), c.GetLocalVariableTypeTable(), c.GetConstantPool
				())
		{
		}

		public LocalVariableTypeTable(int name_index, int length, NBCEL.classfile.LocalVariable
			[] local_variable_table, NBCEL.classfile.ConstantPool constant_pool)
			: base(NBCEL.Const.ATTR_LOCAL_VARIABLE_TYPE_TABLE, name_index, length, constant_pool
				)
		{
			// The new table is used when generic types are about...
			//LocalVariableTable_attribute {
			//       u2 attribute_name_index;
			//       u4 attribute_length;
			//       u2 local_variable_table_length;
			//       {  u2 start_pc;
			//          u2 length;
			//          u2 name_index;
			//          u2 descriptor_index;
			//          u2 index;
			//       } local_variable_table[local_variable_table_length];
			//     }
			//LocalVariableTypeTable_attribute {
			//    u2 attribute_name_index;
			//    u4 attribute_length;
			//    u2 local_variable_type_table_length;
			//    {
			//      u2 start_pc;
			//      u2 length;
			//      u2 name_index;
			//      u2 signature_index;
			//      u2 index;
			//    } local_variable_type_table[local_variable_type_table_length];
			//  }
			// J5TODO: Needs some testing !
			// variables
			this.local_variable_type_table = local_variable_table;
		}

		/// <exception cref="System.IO.IOException"/>
		internal LocalVariableTypeTable(int nameIdx, int len, java.io.DataInput input, NBCEL.classfile.ConstantPool
			 cpool)
			: this(nameIdx, len, (NBCEL.classfile.LocalVariable[])null, cpool)
		{
			int local_variable_type_table_length = input.ReadUnsignedShort();
			local_variable_type_table = new NBCEL.classfile.LocalVariable[local_variable_type_table_length
				];
			for (int i = 0; i < local_variable_type_table_length; i++)
			{
				local_variable_type_table[i] = new NBCEL.classfile.LocalVariable(input, cpool);
			}
		}

		public override void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitLocalVariableTypeTable(this);
		}

		/// <exception cref="System.IO.IOException"/>
		public sealed override void Dump(java.io.DataOutputStream file)
		{
			base.Dump(file);
			file.WriteShort(local_variable_type_table.Length);
			foreach (NBCEL.classfile.LocalVariable variable in local_variable_type_table)
			{
				variable.Dump(file);
			}
		}

		public NBCEL.classfile.LocalVariable[] GetLocalVariableTypeTable()
		{
			return local_variable_type_table;
		}

		public NBCEL.classfile.LocalVariable GetLocalVariable(int index)
		{
			foreach (NBCEL.classfile.LocalVariable variable in local_variable_type_table)
			{
				if (variable.GetIndex() == index)
				{
					return variable;
				}
			}
			return null;
		}

		public void SetLocalVariableTable(NBCEL.classfile.LocalVariable[] local_variable_table
			)
		{
			this.local_variable_type_table = local_variable_table;
		}

		/// <returns>String representation.</returns>
		public sealed override string ToString()
		{
			System.Text.StringBuilder buf = new System.Text.StringBuilder();
			for (int i = 0; i < local_variable_type_table.Length; i++)
			{
				buf.Append(local_variable_type_table[i].ToStringShared(true));
				if (i < local_variable_type_table.Length - 1)
				{
					buf.Append('\n');
				}
			}
			return buf.ToString();
		}

		/// <returns>deep copy of this attribute</returns>
		public override NBCEL.classfile.Attribute Copy(NBCEL.classfile.ConstantPool constant_pool
			)
		{
			NBCEL.classfile.LocalVariableTypeTable c = (NBCEL.classfile.LocalVariableTypeTable
				)Clone();
			c.local_variable_type_table = new NBCEL.classfile.LocalVariable[local_variable_type_table
				.Length];
			for (int i = 0; i < local_variable_type_table.Length; i++)
			{
				c.local_variable_type_table[i] = local_variable_type_table[i].Copy();
			}
			c.SetConstantPool(constant_pool);
			return c;
		}

		public int GetTableLength()
		{
			return local_variable_type_table == null ? 0 : local_variable_type_table.Length;
		}
	}
}
