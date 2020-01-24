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

using System.Collections.Generic;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	/// Extends the abstract
	/// <see cref="Constant"/>
	/// to represent a reference to a UTF-8 encoded string.
	/// <p>
	/// The following system properties govern caching this class performs.
	/// </p>
	/// <ul>
	/// <li>
	/// <value>#SYS_PROP_CACHE_MAX_ENTRIES</value>
	/// (since 6.4): The size of the cache, by default 0, meaning caching is disabled.</li>
	/// <li>
	/// <value>#SYS_PROP_CACHE_MAX_ENTRY_SIZE</value>
	/// (since 6.0): The maximum size of the values to cache, by default 200, 0 disables
	/// caching. Values larger than this are <em>not</em> cached.</li>
	/// <li>
	/// <value>#SYS_PROP_STATISTICS</value>
	/// (since 6.0): Prints statistics on the console when the JVM exits.</li>
	/// </ul>
	/// <p>
	/// Here is a sample Maven invocation with caching disabled:
	/// </p>
	/// <pre>
	/// mvn test -Dbcel.statistics=true -Dbcel.maxcached.size=0 -Dbcel.maxcached=0
	/// </pre>
	/// <p>
	/// Here is a sample Maven invocation with caching enabled:
	/// </p>
	/// <pre>
	/// mvn test -Dbcel.statistics=true -Dbcel.maxcached.size=100000 -Dbcel.maxcached=5000000
	/// </pre>
	/// </summary>
	/// <seealso cref="Constant"/>
	public sealed class ConstantUtf8 : NBCEL.classfile.Constant
	{
		private class Cache
		{
			private static readonly bool BCEL_STATISTICS = Sharpen.System.GetBoolean(SYS_PROP_STATISTICS
				);

			internal static readonly int MAX_ENTRIES = Sharpen.System.GetInteger(SYS_PROP_CACHE_MAX_ENTRIES
				, 0);

			private static readonly int INITIAL_CAPACITY = (int)(MAX_ENTRIES / 0.75);

			internal static System.Collections.Generic.Dictionary<string, NBCEL.classfile.ConstantUtf8
				> CACHE;

			internal static readonly int MAX_ENTRY_SIZE = Sharpen.System.GetInteger(SYS_PROP_CACHE_MAX_ENTRY_SIZE
				, 200);

			// Set the size to 0 or below to skip caching entirely
			internal static bool IsEnabled()
			{
				return NBCEL.classfile.ConstantUtf8.Cache.MAX_ENTRIES > 0 && MAX_ENTRY_SIZE > 0;
			}

			public Cache()
			{
				CACHE = new Dictionary<string, ConstantUtf8>();
			}
		}

		private static volatile int considered = 0;

		private static volatile int created = 0;

		private static volatile int hits = 0;

		private static volatile int skipped = 0;

		private const string SYS_PROP_CACHE_MAX_ENTRIES = "bcel.maxcached";

		private const string SYS_PROP_CACHE_MAX_ENTRY_SIZE = "bcel.maxcached.size";

		private const string SYS_PROP_STATISTICS = "bcel.statistics";

		static ConstantUtf8()
		{
			/*// TODO these should perhaps be AtomicInt?
			if (NBCEL.classfile.ConstantUtf8.Cache.BCEL_STATISTICS)
			{
				java.lang.Runtime.GetRuntime().AddShutdownHook(new _Thread_97());
			}*/
		}

		/*private sealed class _Thread_97 : System.Threading.Thread
		{
			public _Thread_97()
			{
			}

			public override void Run()
			{
				NBCEL.classfile.ConstantUtf8.PrintStats();
			}
		}*/

		/// <summary>Clears the cache.</summary>
		/// <since>6.4.0</since>
		public static void ClearCache()
		{
			lock (typeof(ConstantUtf8))
			{
				NBCEL.classfile.ConstantUtf8.Cache.CACHE.Clear();
			}
		}

		// for accesss by test code
		internal static void ClearStats()
		{
			lock (typeof(ConstantUtf8))
			{
				hits = considered = skipped = created = 0;
			}
		}

		/// <summary>Gets a new or cached instance of the given value.</summary>
		/// <remarks>
		/// Gets a new or cached instance of the given value.
		/// <p>
		/// See
		/// <see cref="ConstantUtf8"/>
		/// class Javadoc for details.
		/// </p>
		/// </remarks>
		/// <param name="value">the value.</param>
		/// <returns>a new or cached instance of the given value.</returns>
		/// <since>6.0</since>
		public static NBCEL.classfile.ConstantUtf8 GetCachedInstance(string value)
		{
			if (value.Length > NBCEL.classfile.ConstantUtf8.Cache.MAX_ENTRY_SIZE)
			{
				skipped++;
				return new NBCEL.classfile.ConstantUtf8(value);
			}
			considered++;
			lock (typeof(NBCEL.classfile.ConstantUtf8))
			{
				// might be better with a specific lock object
				NBCEL.classfile.ConstantUtf8 result = NBCEL.classfile.ConstantUtf8.Cache.CACHE.GetOrNull
					(value);
				if (result != null)
				{
					hits++;
					return result;
				}
				result = new NBCEL.classfile.ConstantUtf8(value);
				Sharpen.Collections.Put(NBCEL.classfile.ConstantUtf8.Cache.CACHE, value, result);
				return result;
			}
		}

		/// <summary>Gets a new or cached instance of the given value.</summary>
		/// <remarks>
		/// Gets a new or cached instance of the given value.
		/// <p>
		/// See
		/// <see cref="ConstantUtf8"/>
		/// class Javadoc for details.
		/// </p>
		/// </remarks>
		/// <param name="dataInput">the value.</param>
		/// <returns>a new or cached instance of the given value.</returns>
		/// <exception cref="System.IO.IOException">if an I/O error occurs.</exception>
		/// <since>6.0</since>
		public static NBCEL.classfile.ConstantUtf8 GetInstance(java.io.DataInput dataInput
			)
		{
			return GetInstance(dataInput.ReadUTF());
		}

		/// <summary>Gets a new or cached instance of the given value.</summary>
		/// <remarks>
		/// Gets a new or cached instance of the given value.
		/// <p>
		/// See
		/// <see cref="ConstantUtf8"/>
		/// class Javadoc for details.
		/// </p>
		/// </remarks>
		/// <param name="value">the value.</param>
		/// <returns>a new or cached instance of the given value.</returns>
		/// <since>6.0</since>
		public static NBCEL.classfile.ConstantUtf8 GetInstance(string value)
		{
			return NBCEL.classfile.ConstantUtf8.Cache.IsEnabled() ? GetCachedInstance(value) : 
				new NBCEL.classfile.ConstantUtf8(value);
		}

		// for accesss by test code
		internal static void PrintStats()
		{
			string prefix = "[Apache Commons BCEL]";
			System.Console.Error.WriteLine("{0} Cache hit {1}/{2}, {3} skipped.", prefix, hits, 
				considered, skipped);
			System.Console.Error.WriteLine("{0} Total of {1} ConstantUtf8 objects created.", prefix
				, created);
			System.Console.Error.WriteLine("{0} Configuration: {1}={2}, {3}={4}.", prefix, SYS_PROP_CACHE_MAX_ENTRIES
				, NBCEL.classfile.ConstantUtf8.Cache.MAX_ENTRIES, SYS_PROP_CACHE_MAX_ENTRY_SIZE, 
				NBCEL.classfile.ConstantUtf8.Cache.MAX_ENTRY_SIZE);
		}

		private readonly string value;

		/// <summary>Initializes from another object.</summary>
		/// <param name="constantUtf8">the value.</param>
		public ConstantUtf8(NBCEL.classfile.ConstantUtf8 constantUtf8)
			: this(constantUtf8.GetBytes())
		{
		}

		/// <summary>Initializes instance from file data.</summary>
		/// <param name="dataInput">Input stream</param>
		/// <exception cref="System.IO.IOException"/>
		internal ConstantUtf8(java.io.DataInput dataInput)
			: base(NBCEL.Const.CONSTANT_Utf8)
		{
			value = dataInput.ReadUTF();
			created++;
		}

		/// <param name="value">Data</param>
		public ConstantUtf8(string value)
			: base(NBCEL.Const.CONSTANT_Utf8)
		{
			if (value == null)
			{
				throw new System.ArgumentException("Value must not be null.");
			}
			this.value = value;
			created++;
		}

		/// <summary>Called by objects that are traversing the nodes of the tree implicitely defined by the contents of a Java class.
		/// 	</summary>
		/// <remarks>
		/// Called by objects that are traversing the nodes of the tree implicitely defined by the contents of a Java class.
		/// I.e., the hierarchy of methods, fields, attributes, etc. spawns a tree of objects.
		/// </remarks>
		/// <param name="v">Visitor object</param>
		public override void Accept(NBCEL.classfile.Visitor v)
		{
			v.VisitConstantUtf8(this);
		}

		/// <summary>Dumps String in Utf8 format to file stream.</summary>
		/// <param name="file">Output file stream</param>
		/// <exception cref="System.IO.IOException"/>
		public override void Dump(java.io.DataOutputStream file)
		{
			file.WriteByte(base.GetTag());
			file.WriteUTF(value);
		}

		/// <returns>Data converted to string.</returns>
		public string GetBytes()
		{
			return value;
		}

		/// <param name="bytes">the raw bytes of this UTF-8</param>
		[System.ObsoleteAttribute(@"(since 6.0)")]
		public void SetBytes(string bytes)
		{
			throw new System.NotSupportedException();
		}

		/// <returns>String representation</returns>
		public override string ToString()
		{
			return base.ToString() + "(\"" + NBCEL.classfile.Utility.Replace(value, "\n", "\\n"
				) + "\")";
		}
	}
}
