/*
* Copyright (c) 1997, 2013, Oracle and/or its affiliates. All rights reserved.
* ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using java.io;
using Sharpen;

namespace Java.Util.Jar
{
	/// <summary>
	///     The Attributes class maps Manifest attribute names to associated string
	///     values.
	/// </summary>
	/// <remarks>
	///     The Attributes class maps Manifest attribute names to associated string
	///     values. Valid attribute names are case-insensitive, are restricted to
	///     the ASCII characters in the set [0-9a-zA-Z_-], and cannot exceed 70
	///     characters in length. Attribute values can contain any characters and
	///     will be UTF8-encoded when written to the output stream.  See the
	///     <a href="../../../../technotes/guides/jar/jar.html">JAR File Specification</a>
	///     for more information about valid attribute names and values.
	/// </remarks>
	/// <author>David Connelly</author>
	/// <seealso cref="Manifest" />
	/// <since>1.2</since>
	public class Attributes : IDictionary<Attributes.Name, string>, ICloneable
    {
        /// <summary>The attribute name-value mappings.</summary>
        protected internal Dictionary<Name, string> map;

        /// <summary>Constructs a new, empty Attributes object with default size.</summary>
        public Attributes()
            : this(11)
        {
        }

        /// <summary>
        ///     Constructs a new, empty Attributes object with the specified
        ///     initial size.
        /// </summary>
        /// <param name="size">the initial number of attributes</param>
        public Attributes(int size)
        {
            map = new Dictionary<Name, string>(size);
        }

        /// <summary>
        ///     Constructs a new Attributes object with the same attribute name-value
        ///     mappings as in the specified Attributes.
        /// </summary>
        /// <param name="attr">the specified Attributes</param>
        public Attributes(Attributes attr)
        {
            map = new Dictionary<Name, string>(attr);
        }

        /// <summary>
        ///     Returns a copy of the Attributes, implemented as follows:
        ///     <pre>
        ///         public Object clone() { return new Attributes(this); }
        ///     </pre>
        ///     Since the attribute names and values are themselves immutable,
        ///     the Attributes returned can be safely modified without affecting
        ///     the original.
        /// </summary>
        public virtual object Clone()
        {
            return new Attributes(this);
        }

        /// <summary>Removes the attribute with the specified name (key) from this Map.</summary>
        /// <remarks>
        ///     Removes the attribute with the specified name (key) from this Map.
        ///     Returns the previous attribute value, or null if none.
        /// </remarks>
        /// <param name="name">attribute name</param>
        /// <returns>the previous value of the attribute, or null if none</returns>
        public virtual bool Remove(Name name)
        {
            return Collections.Remove(map, name) != default;
        }

        public bool TryGetValue(Name key, out string value)
        {
            return map.TryGetValue(key, out value);
        }

        public string this[Name key]
        {
            get => map[key];
            set => map[key] = value;
        }

        public void Add(Name key, string value)
        {
            map.Add(key, value);
        }

        /// <summary>Returns true if this Map contains the specified attribute name (key).</summary>
        /// <param name="name">the attribute name</param>
        /// <returns>true if this Map contains the specified attribute name</returns>
        public virtual bool ContainsKey(Name name)
        {
            return map.ContainsKey(name);
        }

        public void Add(KeyValuePair<Name, string> item)
        {
            map.Add(item.Key, item.Value);
        }

        /// <summary>Removes all attributes from this Map.</summary>
        public virtual void Clear()
        {
            map.Clear();
        }

        public bool Contains(KeyValuePair<Name, string> item)
        {
            return map.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<Name, string>[] array, int arrayIndex)
        {
        }

        public bool Remove(KeyValuePair<Name, string> item)
        {
            return map.Remove(item.Key);
        }

        /// <summary>Returns the number of attributes in this Map.</summary>
        public virtual int Count => map.Count;

        public bool IsReadOnly => false;

        /// <summary>Returns a Set view of the attribute names (keys) contained in this Map.</summary>
        public virtual ICollection<Name> Keys => map.Keys;

        /// <summary>
        ///     Returns a Collection view of the attribute values contained in this Map.
        /// </summary>
        public virtual ICollection<string> Values => map.Values;

        public IEnumerator<KeyValuePair<Name, string>> GetEnumerator()
        {
            return map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) map).GetEnumerator();
        }

        /// <summary>
        ///     Returns the value of the specified attribute name, or null if the
        ///     attribute name was not found.
        /// </summary>
        /// <param name="name">the attribute name</param>
        /// <returns>
        ///     the value of the specified attribute name, or null if
        ///     not found.
        /// </returns>
        public virtual string Get(Name name)
        {
            return map.GetOrNull(name);
        }

        /// <summary>
        ///     Returns the value of the specified attribute name, specified as
        ///     a string, or null if the attribute was not found.
        /// </summary>
        /// <remarks>
        ///     Returns the value of the specified attribute name, specified as
        ///     a string, or null if the attribute was not found. The attribute
        ///     name is case-insensitive.
        ///     <p>
        ///         This method is defined as:
        ///         <pre>
        ///             return (String)get(new Attributes.Name((String)name));
        ///         </pre>
        /// </remarks>
        /// <param name="name">the attribute name as a string</param>
        /// <returns>
        ///     the String value of the specified attribute name, or null if
        ///     not found.
        /// </returns>
        /// <exception cref="ArgumentException">if the attribute name is invalid</exception>
        public virtual string GetValue(string name)
        {
            return this.GetOrNull(new Name(name));
        }

        /// <summary>
        ///     Returns the value of the specified Attributes.Name, or null if the
        ///     attribute was not found.
        /// </summary>
        /// <remarks>
        ///     Returns the value of the specified Attributes.Name, or null if the
        ///     attribute was not found.
        ///     <p>
        ///         This method is defined as:
        ///         <pre>
        ///             return (String)get(name);
        ///         </pre>
        /// </remarks>
        /// <param name="name">the Attributes.Name object</param>
        /// <returns>
        ///     the String value of the specified Attribute.Name, or null if
        ///     not found.
        /// </returns>
        public virtual string GetValue(Name name)
        {
            return this.GetOrNull(name);
        }

        /// <summary>
        ///     Associates the specified value with the specified attribute name
        ///     (key) in this Map.
        /// </summary>
        /// <remarks>
        ///     Associates the specified value with the specified attribute name
        ///     (key) in this Map. If the Map previously contained a mapping for
        ///     the attribute name, the old value is replaced.
        /// </remarks>
        /// <param name="name">the attribute name</param>
        /// <param name="value">the attribute value</param>
        /// <returns>the previous value of the attribute, or null if none</returns>
        /// <exception>
        ///     ClassCastException
        ///     if the name is not a Attributes.Name
        ///     or the value is not a String
        /// </exception>
        public virtual object Put(Name name, string value)
        {
            map.Add(name, value);
            return true;
        }

        /// <summary>
        ///     Associates the specified value with the specified attribute name,
        ///     specified as a String.
        /// </summary>
        /// <remarks>
        ///     Associates the specified value with the specified attribute name,
        ///     specified as a String. The attributes name is case-insensitive.
        ///     If the Map previously contained a mapping for the attribute name,
        ///     the old value is replaced.
        ///     <p>
        ///         This method is defined as:
        ///         <pre>
        ///             return (String)put(new Attributes.Name(name), value);
        ///         </pre>
        /// </remarks>
        /// <param name="name">the attribute name as a string</param>
        /// <param name="value">the attribute value</param>
        /// <returns>the previous value of the attribute, or null if none</returns>
        /// <exception>
        ///     IllegalArgumentException
        ///     if the attribute name is invalid
        /// </exception>
        public virtual string PutValue(string name, string value)
        {
            return Collections.Put(this, new Name(name), value);
        }

        /// <summary>
        ///     Returns true if this Map maps one or more attribute names (keys)
        ///     to the specified value.
        /// </summary>
        /// <param name="value">the attribute value</param>
        /// <returns>
        ///     true if this Map maps one or more attribute names to
        ///     the specified value
        /// </returns>
        public virtual bool ContainsValue(string value)
        {
            return map.ContainsValue(value);
        }

        /// <summary>
        ///     Copies all of the attribute name-value mappings from the specified
        ///     Attributes to this Map.
        /// </summary>
        /// <remarks>
        ///     Copies all of the attribute name-value mappings from the specified
        ///     Attributes to this Map. Duplicate mappings will be replaced.
        /// </remarks>
        /// <param name="attr">the Attributes to be stored in this map</param>
        /// <exception>
        ///     ClassCastException
        ///     if attr is not an Attributes
        /// </exception>
        public virtual void PutAll(IDictionary<Name, string> attr)
        {
            // ## javac bug?
            if (!typeof(Attributes).IsInstanceOfType(attr)) throw new InvalidCastException();
            foreach (var me in attr) Put(me.Key, me.Value);
        }

        /// <summary>Returns true if this Map contains no attributes.</summary>
        public virtual bool IsEmpty()
        {
            return map.Count == 0;
        }

        /// <summary>
        ///     Returns a Collection view of the attribute name-value mappings
        ///     contained in this Map.
        /// </summary>
        public virtual Dictionary<Name, string> EntrySet()
        {
            return map;
        }

        /// <summary>Compares the specified Attributes object with this Map for equality.</summary>
        /// <remarks>
        ///     Compares the specified Attributes object with this Map for equality.
        ///     Returns true if the given object is also an instance of Attributes
        ///     and the two Attributes objects represent the same mappings.
        /// </remarks>
        /// <param name="o">the Object to be compared</param>
        /// <returns>true if the specified Object is equal to this Map</returns>
        public override bool Equals(object o)
        {
            return map.Equals(o);
        }

        /// <summary>Returns the hash code value for this Map.</summary>
        public override int GetHashCode()
        {
            return map.GetHashCode();
        }

        /*
        * Writes the current attributes to the specified data output stream.
        * XXX Need to handle UTF8 values and break up lines longer than 72 bytes
        */
        /// <exception cref="IOException" />
        internal virtual void Write(DataOutputStream os)
        {
            foreach (var e in this)
            {
                var buffer = new StringBuilder(e.Key.ToString());
                buffer.Append(": ");
                var value = e.Value;
                if (value != null)
                {
                    var vb = Encoding.UTF8.GetBytes(value);
                    value = Encoding.UTF8.GetString(vb);
                }

                buffer.Append(value);
                buffer.Append("\r\n");
                Manifest.Make72Safe(buffer);
                os.WriteBytes(buffer.ToString());
            }

            os.WriteBytes("\r\n");
        }

        /*
        * Writes the current attributes to the specified data output stream,
        * make sure to write out the MANIFEST_VERSION or SIGNATURE_VERSION
        * attributes first.
        *
        * XXX Need to handle UTF8 values and break up lines longer than 72 bytes
        */
        /// <exception cref="IOException" />
        internal virtual void WriteMain(DataOutputStream @out)
        {
            // write out the *-Version header first, if it exists
            var vername = Name.Manifest_Version.ToString();
            var version = GetValue(vername);
            if (version == null)
            {
                vername = Name.Signature_Version.ToString();
                version = GetValue(vername);
            }

            if (version != null) @out.WriteBytes(vername + ": " + version + "\r\n");
            // write out all attributes except for the version
            // we wrote out earlier
            foreach (var e in this)
            {
                var name = e.Key.ToString();
                if (version != null && !Runtime.EqualsIgnoreCase(name, vername))
                {
                    var buffer = new StringBuilder(name);
                    buffer.Append(": ");
                    var value = e.Value;
                    if (value != null)
                    {
                        var vb = Runtime.GetBytesForString(value, "UTF8");
                        value = Encoding.UTF8.GetString(vb);
                    }

                    buffer.Append(value);
                    buffer.Append("\r\n");
                    Manifest.Make72Safe(buffer);
                    @out.WriteBytes(buffer.ToString());
                }
            }

            @out.WriteBytes("\r\n");
        }

        /*
        * Reads attributes from the specified input stream.
        * XXX Need to handle UTF8 values.
        */
        /// <exception cref="IOException" />
        internal virtual void Read(Manifest.FastInputStream @is, byte[] lbuf)
        {
            string name = null;
            string value = null;
            byte[] lastline = null;
            int len;
            while ((len = @is.ReadLine(lbuf)) != -1)
            {
                var lineContinued = false;
                if (lbuf[--len] != '\n') throw new IOException("line too long");
                if (len > 0 && lbuf[len - 1] == '\r') --len;
                if (len == 0) break;
                var i = 0;
                if (lbuf[0] == ' ')
                {
                    // continuation of previous line
                    if (name == null) throw new IOException("misplaced continuation line");
                    lineContinued = true;
                    var buf = new byte[lastline.Length + len - 1];
                    Array.Copy(lastline, 0, buf, 0, lastline.Length);
                    Array.Copy(lbuf, 1, buf, lastline.Length, len - 1);
                    if (@is.Peek() == ' ')
                    {
                        lastline = buf;
                        continue;
                    }

                    value = Runtime.GetStringForBytes(buf);
                    lastline = null;
                }
                else
                {
                    while (lbuf[i++] != ':')
                        if (i >= len)
                            throw new IOException("invalid header field");
                    if (lbuf[i++] != ' ') throw new IOException("invalid header field");
                    name = Encoding.UTF8.GetString(lbuf);
                    if (@is.Peek() == ' ')
                    {
                        lastline = new byte[len - i];
                        Array.Copy(lbuf, i, lastline, 0, len - i);
                        continue;
                    }

                    value = Runtime.GetStringForBytes(lbuf);
                }

                try
                {
                    if (PutValue(name, value) != null && !lineContinued)
                        Console.WriteLine("Warning>> Duplicate name in Manifest: "
                                          + name + ".\n" + "Ensure that the manifest does not " +
                                          "have duplicate entries, and\n"
                                          + "that blank lines separate " + "individual sections in both your\n" +
                                          "manifest and in the META-INF/MANIFEST.MF "
                                          + "entry in the jar file.");
                }
                catch (ArgumentException)
                {
                    throw new IOException("invalid header field name: " + name);
                }
            }
        }

        /// <summary>
        ///     The Attributes.Name class represents an attribute name stored in
        ///     this Map.
        /// </summary>
        /// <remarks>
        ///     The Attributes.Name class represents an attribute name stored in
        ///     this Map. Valid attribute names are case-insensitive, are restricted
        ///     to the ASCII characters in the set [0-9a-zA-Z_-], and cannot exceed
        ///     70 characters in length. Attribute values can contain any characters
        ///     and will be UTF8-encoded when written to the output stream.  See the
        ///     <a href="../../../../technotes/guides/jar/jar.html">JAR File Specification</a>
        ///     for more information about valid attribute names and values.
        /// </remarks>
        public class Name
        {
	        /// <summary>
	        ///     <code>Name</code> object for <code>Manifest-Version</code>
	        ///     manifest attribute.
	        /// </summary>
	        /// <remarks>
	        ///     <code>Name</code> object for <code>Manifest-Version</code>
	        ///     manifest attribute. This attribute indicates the version number
	        ///     of the manifest standard to which a JAR file's manifest conforms.
	        /// </remarks>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/jar/jar.html#JAR_Manifest">
	        ///         *      Manifest and Signature Specification
	        ///     </a>
	        /// </seealso>
	        public static readonly Name Manifest_Version = new Name("Manifest-Version"
            );

	        /// <summary>
	        ///     <code>Name</code> object for <code>Signature-Version</code>
	        ///     manifest attribute used when signing JAR files.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/jar/jar.html#JAR_Manifest">
	        ///         *      Manifest and Signature Specification
	        ///     </a>
	        /// </seealso>
	        public static readonly Name Signature_Version = new Name("Signature-Version"
            );

	        /// <summary>
	        ///     <code>Name</code> object for <code>Content-Type</code>
	        ///     manifest attribute.
	        /// </summary>
	        public static readonly Name Content_Type = new Name("Content-Type"
            );

	        /// <summary>
	        ///     <code>Name</code> object for <code>Class-Path</code>
	        ///     manifest attribute.
	        /// </summary>
	        /// <remarks>
	        ///     <code>Name</code> object for <code>Class-Path</code>
	        ///     manifest attribute. Bundled extensions can use this attribute
	        ///     to find other JAR files containing needed classes.
	        /// </remarks>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/jar/jar.html#classpath">
	        ///         *      JAR file specification
	        ///     </a>
	        /// </seealso>
	        public static readonly Name Class_Path = new Name("Class-Path"
            );

	        /// <summary>
	        ///     <code>Name</code> object for <code>Main-Class</code> manifest
	        ///     attribute used for launching applications packaged in JAR files.
	        /// </summary>
	        /// <remarks>
	        ///     <code>Name</code> object for <code>Main-Class</code> manifest
	        ///     attribute used for launching applications packaged in JAR files.
	        ///     The <code>Main-Class</code> attribute is used in conjunction
	        ///     with the <code>-jar</code> command-line option of the
	        ///     <tt>java</tt> application launcher.
	        /// </remarks>
	        public static readonly Name Main_Class = new Name("Main-Class"
            );

	        /// <summary>
	        ///     <code>Name</code> object for <code>Sealed</code> manifest attribute
	        ///     used for sealing.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/jar/jar.html#sealing">
	        ///         *      Package Sealing
	        ///     </a>
	        /// </seealso>
	        public static readonly Name Sealed = new Name("Sealed");

	        /// <summary>
	        ///     <code>Name</code> object for <code>Extension-List</code> manifest attribute
	        ///     used for declaring dependencies on installed extensions.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/extensions/spec.html#dependency">
	        ///         *      Installed extension dependency
	        ///     </a>
	        /// </seealso>
	        public static readonly Name Extension_List = new Name("Extension-List"
            );

	        /// <summary>
	        ///     <code>Name</code> object for <code>Extension-Name</code> manifest attribute
	        ///     used for declaring dependencies on installed extensions.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/extensions/spec.html#dependency">
	        ///         *      Installed extension dependency
	        ///     </a>
	        /// </seealso>
	        public static readonly Name Extension_Name = new Name("Extension-Name"
            );

	        /// <summary>
	        ///     <code>Name</code> object for <code>Extension-Name</code> manifest attribute
	        ///     used for declaring dependencies on installed extensions.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/extensions/spec.html#dependency">
	        ///         *      Installed extension dependency
	        ///     </a>
	        /// </seealso>
	        [ObsoleteAttribute(@"Extension mechanism will be removed in a future release. Use class path instead."
            )]
            public static readonly Name Extension_Installation = new Name
                ("Extension-Installation");

	        /// <summary>
	        ///     <code>Name</code> object for <code>Implementation-Title</code>
	        ///     manifest attribute used for package versioning.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
	        ///         *      Java Product Versioning Specification
	        ///     </a>
	        /// </seealso>
	        public static readonly Name Implementation_Title = new Name
                ("Implementation-Title");

	        /// <summary>
	        ///     <code>Name</code> object for <code>Implementation-Version</code>
	        ///     manifest attribute used for package versioning.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
	        ///         *      Java Product Versioning Specification
	        ///     </a>
	        /// </seealso>
	        public static readonly Name Implementation_Version = new Name
                ("Implementation-Version");

	        /// <summary>
	        ///     <code>Name</code> object for <code>Implementation-Vendor</code>
	        ///     manifest attribute used for package versioning.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
	        ///         *      Java Product Versioning Specification
	        ///     </a>
	        /// </seealso>
	        public static readonly Name Implementation_Vendor = new Name
                ("Implementation-Vendor");

	        /// <summary>
	        ///     <code>Name</code> object for <code>Implementation-Vendor-Id</code>
	        ///     manifest attribute used for package versioning.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/extensions/versioning.html#applet">
	        ///         *      Optional Package Versioning
	        ///     </a>
	        /// </seealso>
	        [ObsoleteAttribute(@"Extension mechanism will be removed in a future release. Use class path instead."
            )]
            public static readonly Name Implementation_Vendor_Id = new Name
                ("Implementation-Vendor-Id");

	        /// <summary>
	        ///     <code>Name</code> object for <code>Implementation-URL</code>
	        ///     manifest attribute used for package versioning.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/extensions/versioning.html#applet">
	        ///         *      Optional Package Versioning
	        ///     </a>
	        /// </seealso>
	        [ObsoleteAttribute(@"Extension mechanism will be removed in a future release. Use class path instead."
            )]
            public static readonly Name Implementation_Url = new Name("Implementation-URL"
            );

	        /// <summary>
	        ///     <code>Name</code> object for <code>Specification-Title</code>
	        ///     manifest attribute used for package versioning.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
	        ///         *      Java Product Versioning Specification
	        ///     </a>
	        /// </seealso>
	        public static readonly Name Specification_Title = new Name(
                "Specification-Title");

	        /// <summary>
	        ///     <code>Name</code> object for <code>Specification-Version</code>
	        ///     manifest attribute used for package versioning.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
	        ///         *      Java Product Versioning Specification
	        ///     </a>
	        /// </seealso>
	        public static readonly Name Specification_Version = new Name
                ("Specification-Version");

	        /// <summary>
	        ///     <code>Name</code> object for <code>Specification-Vendor</code>
	        ///     manifest attribute used for package versioning.
	        /// </summary>
	        /// <seealso>
	        ///     <a href="../../../../technotes/guides/versioning/spec/versioning2.html#wp90779">
	        ///         *      Java Product Versioning Specification
	        ///     </a>
	        /// </seealso>
	        public static readonly Name Specification_Vendor = new Name
                ("Specification-Vendor");

            private int hashCode = -1;
            private readonly string name;

            /// <summary>Constructs a new attribute name using the given string name.</summary>
            /// <param name="name">the attribute string name</param>
            /// <exception>
            ///     IllegalArgumentException
            ///     if the attribute name was
            ///     invalid
            /// </exception>
            /// <exception>
            ///     NullPointerException
            ///     if the attribute name was null
            /// </exception>
            public Name(string name)
            {
                if (name == null) throw new ArgumentNullException("name");
                if (!IsValid(name)) throw new ArgumentException(name);
                this.name = string.Intern(name);
            }

            private static bool IsValid(string name)
            {
                var len = name.Length;
                if (len > 70 || len == 0) return false;
                for (var i = 0; i < len; i++)
                    if (!IsValid(name[i]))
                        return false;
                return true;
            }

            private static bool IsValid(char c)
            {
                return IsAlpha(c) || IsDigit(c) || c == '_' || c == '-';
            }

            private static bool IsAlpha(char c)
            {
                return c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z';
            }

            private static bool IsDigit(char c)
            {
                return c >= '0' && c <= '9';
            }

            /// <summary>Compares this attribute name to another for equality.</summary>
            /// <param name="o">the object to compare</param>
            /// <returns>
            ///     true if this attribute name is equal to the
            ///     specified attribute object
            /// </returns>
            public override bool Equals(object o)
            {
                if (o is Name)
                    return name.Equals(((Name) o).name, StringComparison.OrdinalIgnoreCase);
                return false;
            }

            /// <summary>Computes the hash value for this attribute name.</summary>
            public override int GetHashCode()
            {
                if (hashCode == -1) hashCode = name.ToLower().GetHashCode();
                return hashCode;
            }

            /// <summary>Returns the attribute name as a String.</summary>
            public override string ToString()
            {
                return name;
            }
        }
    }
}