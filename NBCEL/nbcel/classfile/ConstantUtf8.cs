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
using java.io;
using Sharpen;

namespace NBCEL.classfile
{
	/// <summary>
	///     Extends the abstract
	///     <see cref="Constant" />
	///     to represent a reference to a UTF-8 encoded string.
	///     <p>
	///         The following system properties govern caching this class performs.
	///     </p>
	///     <ul>
	///         <li>
	///             <value>#SYS_PROP_CACHE_MAX_ENTRIES</value>
	///             (since 6.4): The size of the cache, by default 0, meaning caching is disabled.
	///         </li>
	///         <li>
	///             <value>#SYS_PROP_CACHE_MAX_ENTRY_SIZE</value>
	///             (since 6.0): The maximum size of the values to cache, by default 200, 0 disables
	///             caching. Values larger than this are <em>not</em> cached.
	///         </li>
	///         <li>
	///             <value>#SYS_PROP_STATISTICS</value>
	///             (since 6.0): Prints statistics on the console when the JVM exits.
	///         </li>
	///     </ul>
	///     <p>
	///         Here is a sample Maven invocation with caching disabled:
	///     </p>
	///     <pre>
	///         mvn test -Dbcel.statistics=true -Dbcel.maxcached.size=0 -Dbcel.maxcached=0
	///     </pre>
	///     <p>
	///         Here is a sample Maven invocation with caching enabled:
	///     </p>
	///     <pre>
	///         mvn test -Dbcel.statistics=true -Dbcel.maxcached.size=100000 -Dbcel.maxcached=5000000
	///     </pre>
	/// </summary>
	/// <seealso cref="Constant" />
	public sealed class ConstantUtf8 : Constant
    {
        private const string SYS_PROP_CACHE_MAX_ENTRIES = "bcel.maxcached";

        private const string SYS_PROP_CACHE_MAX_ENTRY_SIZE = "bcel.maxcached.size";

        private const string SYS_PROP_STATISTICS = "bcel.statistics";

        private static volatile int considered;

        private static volatile int created;

        private static volatile int hits;

        private static volatile int skipped;

        private readonly string value;

        static ConstantUtf8()
        {
            /*// TODO these should perhaps be AtomicInt?
            if (NBCEL.classfile.ConstantUtf8.Cache.BCEL_STATISTICS)
            {
                java.lang.Runtime.GetRuntime().AddShutdownHook(new _Thread_97());
            }*/
        }

        /// <summary>Initializes from another object.</summary>
        /// <param name="constantUtf8">the value.</param>
        public ConstantUtf8(ConstantUtf8 constantUtf8)
            : this(constantUtf8.GetBytes())
        {
        }

        /// <summary>Initializes instance from file data.</summary>
        /// <param name="dataInput">Input stream</param>
        /// <exception cref="System.IO.IOException" />
        internal ConstantUtf8(DataInput dataInput)
            : base(Const.CONSTANT_Utf8)
        {
            value = dataInput.ReadUTF();
            created++;
        }

        /// <param name="value">Data</param>
        public ConstantUtf8(string value)
            : base(Const.CONSTANT_Utf8)
        {
            if (value == null) throw new ArgumentException("Value must not be null.");
            this.value = value;
            created++;
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
                Cache.CACHE.Clear();
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
        ///     Gets a new or cached instance of the given value.
        ///     <p>
        ///         See
        ///         <see cref="ConstantUtf8" />
        ///         class Javadoc for details.
        ///     </p>
        /// </remarks>
        /// <param name="value">the value.</param>
        /// <returns>a new or cached instance of the given value.</returns>
        /// <since>6.0</since>
        public static ConstantUtf8 GetCachedInstance(string value)
        {
            if (value.Length > Cache.MAX_ENTRY_SIZE)
            {
                skipped++;
                return new ConstantUtf8(value);
            }

            considered++;
            lock (typeof(ConstantUtf8))
            {
                // might be better with a specific lock object
                var result = Cache.CACHE.GetOrNull
                    (value);
                if (result != null)
                {
                    hits++;
                    return result;
                }

                result = new ConstantUtf8(value);
                Collections.Put(Cache.CACHE, value, result);
                return result;
            }
        }

        /// <summary>Gets a new or cached instance of the given value.</summary>
        /// <remarks>
        ///     Gets a new or cached instance of the given value.
        ///     <p>
        ///         See
        ///         <see cref="ConstantUtf8" />
        ///         class Javadoc for details.
        ///     </p>
        /// </remarks>
        /// <param name="dataInput">the value.</param>
        /// <returns>a new or cached instance of the given value.</returns>
        /// <exception cref="System.IO.IOException">if an I/O error occurs.</exception>
        /// <since>6.0</since>
        public static ConstantUtf8 GetInstance(DataInput dataInput
        )
        {
            return GetInstance(dataInput.ReadUTF());
        }

        /// <summary>Gets a new or cached instance of the given value.</summary>
        /// <remarks>
        ///     Gets a new or cached instance of the given value.
        ///     <p>
        ///         See
        ///         <see cref="ConstantUtf8" />
        ///         class Javadoc for details.
        ///     </p>
        /// </remarks>
        /// <param name="value">the value.</param>
        /// <returns>a new or cached instance of the given value.</returns>
        /// <since>6.0</since>
        public static ConstantUtf8 GetInstance(string value)
        {
            return Cache.IsEnabled() ? GetCachedInstance(value) : new ConstantUtf8(value);
        }

        // for accesss by test code
        internal static void PrintStats()
        {
            var prefix = "[Apache Commons BCEL]";
            Console.Error.WriteLine("{0} Cache hit {1}/{2}, {3} skipped.", prefix, hits,
                considered, skipped);
            Console.Error.WriteLine("{0} Total of {1} ConstantUtf8 objects created.", prefix
                , created);
            Console.Error.WriteLine("{0} Configuration: {1}={2}, {3}={4}.", prefix, SYS_PROP_CACHE_MAX_ENTRIES
                , Cache.MAX_ENTRIES, SYS_PROP_CACHE_MAX_ENTRY_SIZE,
                Cache.MAX_ENTRY_SIZE);
        }

        /// <summary>
        ///     Called by objects that are traversing the nodes of the tree implicitely defined by the contents of a Java class.
        /// </summary>
        /// <remarks>
        ///     Called by objects that are traversing the nodes of the tree implicitely defined by the contents of a Java class.
        ///     I.e., the hierarchy of methods, fields, attributes, etc. spawns a tree of objects.
        /// </remarks>
        /// <param name="v">Visitor object</param>
        public override void Accept(Visitor v)
        {
            v.VisitConstantUtf8(this);
        }

        /// <summary>Dumps String in Utf8 format to file stream.</summary>
        /// <param name="file">Output file stream</param>
        /// <exception cref="System.IO.IOException" />
        public override void Dump(DataOutputStream file)
        {
            file.WriteByte(GetTag());
            file.WriteUTF(value);
        }

        /// <returns>Data converted to string.</returns>
        public string GetBytes()
        {
            return value;
        }

        /// <param name="bytes">the raw bytes of this UTF-8</param>
        [Obsolete(@"(since 6.0)")]
        public void SetBytes(string bytes)
        {
            throw new NotSupportedException();
        }

        /// <returns>String representation</returns>
        public override string ToString()
        {
            return base.ToString() + "(\"" + Utility.Replace(value, "\n", "\\n"
                   ) + "\")";
        }

        private class Cache
        {
            private static readonly bool BCEL_STATISTICS = Sharpen.System.GetBoolean(SYS_PROP_STATISTICS
            );

            internal static readonly int MAX_ENTRIES = Sharpen.System.GetInteger(SYS_PROP_CACHE_MAX_ENTRIES
                , 0);

            private static readonly int INITIAL_CAPACITY = (int) (MAX_ENTRIES / 0.75);

            internal static Dictionary<string, ConstantUtf8
            > CACHE;

            internal static readonly int MAX_ENTRY_SIZE = Sharpen.System.GetInteger(SYS_PROP_CACHE_MAX_ENTRY_SIZE
                , 200);

            public Cache()
            {
                CACHE = new Dictionary<string, ConstantUtf8>();
            }

            // Set the size to 0 or below to skip caching entirely
            internal static bool IsEnabled()
            {
                return MAX_ENTRIES > 0 && MAX_ENTRY_SIZE > 0;
            }
        }
    }
}