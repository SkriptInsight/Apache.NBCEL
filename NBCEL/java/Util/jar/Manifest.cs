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
using System.Collections.Generic;
using System.IO;
using System.Text;
using java.io;
using Sharpen;

namespace Java.Util.Jar
{
	/// <summary>
	/// The Manifest class is used to maintain Manifest entry names and their
	/// associated Attributes.
	/// </summary>
	/// <remarks>
	/// The Manifest class is used to maintain Manifest entry names and their
	/// associated Attributes. There are main Manifest Attributes as well as
	/// per-entry Attributes. For information on the Manifest format, please
	/// see the
	/// <a href="../../../../technotes/guides/jar/jar.html">
	/// Manifest format specification</a>.
	/// </remarks>
	/// <author>David Connelly</author>
	/// <seealso cref="Attributes"/>
	/// <since>1.2</since>
	public class Manifest : ICloneable
	{
		private Attributes attr = new Attributes();

		private IDictionary<string, Attributes> entries = new Dictionary<string, Attributes
			>();

		/// <summary>Constructs a new, empty Manifest.</summary>
		public Manifest()
		{
		}

		/// <summary>Constructs a new Manifest from the specified input stream.</summary>
		/// <param name="is">the input stream containing manifest data</param>
		/// <exception cref="IOException">if an I/O error has occurred</exception>
		public Manifest(InputStream @is)
		{
			// manifest main attributes
			// manifest entries
			Read(@is);
		}

		/// <summary>Constructs a new Manifest that is a copy of the specified Manifest.</summary>
		/// <param name="man">the Manifest to copy</param>
		public Manifest(Manifest man)
		{
			Sharpen.Collections.PutAll(attr, man.GetMainAttributes());
			Sharpen.Collections.PutAll(entries, man.GetEntries());
		}

		/// <summary>Returns the main Attributes for the Manifest.</summary>
		/// <returns>the main Attributes for the Manifest</returns>
		public virtual Attributes GetMainAttributes()
		{
			return attr;
		}

		/// <summary>Returns a Map of the entries contained in this Manifest.</summary>
		/// <remarks>
		/// Returns a Map of the entries contained in this Manifest. Each entry
		/// is represented by a String name (key) and associated Attributes (value).
		/// The Map permits the
		/// <see langword="null"/>
		/// key, but no entry with a null key is
		/// created by
		/// <see cref="Read(InputStream)"/>
		/// , nor is such an entry written by using
		/// <see cref="Write(OutputStream)"/>
		/// .
		/// </remarks>
		/// <returns>a Map of the entries contained in this Manifest</returns>
		public virtual IDictionary<string, Attributes> GetEntries()
		{
			return entries;
		}

		/// <summary>Returns the Attributes for the specified entry name.</summary>
		/// <remarks>
		/// Returns the Attributes for the specified entry name.
		/// This method is defined as:
		/// <pre>
		/// return (Attributes)getEntries().get(name)
		/// </pre>
		/// Though
		/// <see langword="null"/>
		/// is a valid
		/// <paramref name="name"/>
		/// , when
		/// <c>getAttributes(null)</c>
		/// is invoked on a
		/// <c>Manifest</c>
		/// obtained from a jar file,
		/// <see langword="null"/>
		/// will be returned.  While jar
		/// files themselves do not allow
		/// <see langword="null"/>
		/// -named attributes, it is
		/// possible to invoke
		/// <see cref="GetEntries()"/>
		/// on a
		/// <c>Manifest</c>
		/// , and
		/// on that result, invoke
		/// <c>put</c>
		/// with a null key and an
		/// arbitrary value.  Subsequent invocations of
		/// <c>getAttributes(null)</c>
		/// will return the just-
		/// <c>put</c>
		/// value.
		/// <p>
		/// Note that this method does not return the manifest's main attributes;
		/// see
		/// <see cref="GetMainAttributes()"/>
		/// .
		/// </remarks>
		/// <param name="name">entry name</param>
		/// <returns>the Attributes for the specified entry name</returns>
		public virtual Attributes GetAttributes(string name)
		{
			return GetEntries().GetOrNull(name);
		}

		/// <summary>Clears the main Attributes as well as the entries in this Manifest.</summary>
		public virtual void Clear()
		{
			attr.Clear();
			entries.Clear();
		}

		/// <summary>Writes the Manifest to the specified OutputStream.</summary>
		/// <remarks>
		/// Writes the Manifest to the specified OutputStream.
		/// Attributes.Name.MANIFEST_VERSION must be set in
		/// MainAttributes prior to invoking this method.
		/// </remarks>
		/// <param name="out">the output stream</param>
		/// <exception>
		/// IOException
		/// if an I/O error has occurred
		/// </exception>
		/// <seealso cref="GetMainAttributes()"/>
		/// <exception cref="IOException"/>
		public virtual void Write(OutputStream @out)
		{
			DataOutputStream dos = new DataOutputStream(@out);
			// Write out the main attributes for the manifest
			attr.WriteMain(dos);
			// Now write out the pre-entry attributes
			IEnumerator<KeyValuePair<string, Attributes>> it = entries.GetEnumerator();
			while (it.MoveNext())
			{
				KeyValuePair<string, Attributes> e = it.Current;
				StringBuilder buffer = new StringBuilder("Name: ");
				string value = e.Key;
				if (value != null)
				{
					byte[] vb = Sharpen.Runtime.GetBytesForString(value, "UTF8");
					value = Encoding.UTF8.GetString(vb);
				}
				buffer.Append(value);
				buffer.Append("\r\n");
				Make72Safe(buffer);
				dos.WriteBytes(buffer.ToString());
				e.Value.Write(dos);
			}
			dos.Flush();
		}

		/// <summary>Adds line breaks to enforce a maximum 72 bytes per line.</summary>
		internal static void Make72Safe(StringBuilder line)
		{
			int length = line.Length;
			if (length > 72)
			{
				int index = 70;
				while (index < length - 2)
				{
					line.Insert(index, "\r\n ");
					index += 72;
					length += 3;
				}
			}
			return;
		}

		/// <summary>Reads the Manifest from the specified InputStream.</summary>
		/// <remarks>
		/// Reads the Manifest from the specified InputStream. The entry
		/// names and attributes read will be merged in with the current
		/// manifest entries.
		/// </remarks>
		/// <param name="is">the input stream</param>
		/// <exception>
		/// IOException
		/// if an I/O error has occurred
		/// </exception>
		/// <exception cref="IOException"/>
		public virtual void Read(InputStream @is)
		{
			// Buffered input stream for reading manifest data
			Manifest.FastInputStream fis = new Manifest.FastInputStream(@is);
			// Line buffer
			byte[] lbuf = new byte[512];
			// Read the main attributes for the manifest
			attr.Read(fis, lbuf);
			// Total number of entries, attributes read
			int ecount = 0;
			int acount = 0;
			// Average size of entry attributes
			int asize = 2;
			// Now parse the manifest entries
			int len;
			string name = null;
			bool skipEmptyLines = true;
			byte[] lastline = null;
			while ((len = fis.ReadLine(lbuf)) != -1)
			{
				if (lbuf[--len] != '\n')
				{
					throw new IOException("manifest line too long");
				}
				if (len > 0 && lbuf[len - 1] == '\r')
				{
					--len;
				}
				if (len == 0 && skipEmptyLines)
				{
					continue;
				}
				skipEmptyLines = false;
				if (name == null)
				{
					name = ParseName(lbuf, len);
					if (name == null)
					{
						throw new IOException("invalid manifest format");
					}
					if (fis.Peek() == ' ')
					{
						// name is wrapped
						lastline = new byte[len - 6];
						System.Array.Copy(lbuf, 6, lastline, 0, len - 6);
						continue;
					}
				}
				else
				{
					// continuation line
					byte[] buf = new byte[lastline.Length + len - 1];
					System.Array.Copy(lastline, 0, buf, 0, lastline.Length);
					System.Array.Copy(lbuf, 1, buf, lastline.Length, len - 1);
					if (fis.Peek() == ' ')
					{
						// name is wrapped
						lastline = buf;
						continue;
					}
					name = Sharpen.Runtime.GetStringForBytes(buf, "UTF8");
					lastline = null;
				}
				Attributes attr = GetAttributes(name);
				if (attr == null)
				{
					attr = new Attributes(asize);
					Sharpen.Collections.Put(entries, name, attr);
				}
				attr.Read(fis, lbuf);
				ecount++;
				acount += attr.Count;
				//XXX: Fix for when the average is 0. When it is 0,
				// you get an Attributes object with an initial
				// capacity of 0, which tickles a bug in HashMap.
				asize = System.Math.Max(2, acount / ecount);
				name = null;
				skipEmptyLines = true;
			}
		}

		private string ParseName(byte[] lbuf, int len)
		{
			if (ToLower(lbuf[0]) == 'n' && ToLower(lbuf[1]) == 'a' && ToLower(lbuf[2]) == 'm'
				 && ToLower(lbuf[3]) == 'e' && lbuf[4] == ':' && lbuf[5] == ' ')
			{
				try
				{
					return Sharpen.Runtime.GetStringForBytes(lbuf, "UTF8");
				}
				catch (Exception)
				{
				}
			}
			return null;
		}

		private int ToLower(int c)
		{
			return (c >= 'A' && c <= 'Z') ? 'a' + (c - 'A') : c;
		}

		/// <summary>
		/// Returns true if the specified Object is also a Manifest and has
		/// the same main Attributes and entries.
		/// </summary>
		/// <param name="o">the object to be compared</param>
		/// <returns>
		/// true if the specified Object is also a Manifest and has
		/// the same main Attributes and entries
		/// </returns>
		public override bool Equals(object o)
		{
			if (o is Manifest)
			{
				Manifest m = (Manifest)o;
				return attr.Equals(m.GetMainAttributes()) && entries.Equals(m.GetEntries());
			}
			else
			{
				return false;
			}
		}

		/// <summary>Returns the hash code for this Manifest.</summary>
		public override int GetHashCode()
		{
			return attr.GetHashCode() + entries.GetHashCode();
		}

		/// <summary>Returns a shallow copy of this Manifest.</summary>
		/// <remarks>
		/// Returns a shallow copy of this Manifest.  The shallow copy is
		/// implemented as follows:
		/// <pre>
		/// public Object clone() { return new Manifest(this); }
		/// </pre>
		/// </remarks>
		/// <returns>a shallow copy of this Manifest</returns>
		public virtual object Clone()
		{
			return new Manifest(this);
		}

		internal class FastInputStream : FilterInputStream
		{
			private byte[] buf;

			private int count = 0;

			private int pos = 0;

			internal FastInputStream(InputStream @in)
				: this(@in, 8192)
			{
			}

			internal FastInputStream(InputStream @in, int size)
				: base(@in)
			{
				/*
				* A fast buffered input stream for parsing manifest files.
				*/
				buf = new byte[size];
			}

			/// <exception cref="IOException"/>
			public override int Read()
			{
				if (pos >= count)
				{
					Fill();
					if (pos >= count)
					{
						return -1;
					}
				}
				return (int) BitConverter.ToUInt32(buf, pos++);
			}

			/// <exception cref="IOException"/>
			public override int Read(byte[] b, int off, int len)
			{
				int avail = count - pos;
				if (avail <= 0)
				{
					if (len >= buf.Length)
					{
						return @in.Read(b, off, len);
					}
					Fill();
					avail = count - pos;
					if (avail <= 0)
					{
						return -1;
					}
				}
				if (len > avail)
				{
					len = avail;
				}
				System.Array.Copy(buf, pos, b, off, len);
				pos += len;
				return len;
			}

			/*
			* Reads 'len' bytes from the input stream, or until an end-of-line
			* is reached. Returns the number of bytes read.
			*/
			/// <exception cref="IOException"/>
			public virtual int ReadLine(byte[] b, int off, int len)
			{
				byte[] tbuf = this.buf;
				int total = 0;
				while (total < len)
				{
					int avail = count - pos;
					if (avail <= 0)
					{
						Fill();
						avail = count - pos;
						if (avail <= 0)
						{
							return -1;
						}
					}
					int n = len - total;
					if (n > avail)
					{
						n = avail;
					}
					int tpos = pos;
					int maxpos = tpos + n;
					while (tpos < maxpos && tbuf[tpos++] != '\n')
					{
					}
					n = tpos - pos;
					System.Array.Copy(tbuf, pos, b, off, n);
					off += n;
					total += n;
					pos = tpos;
					if (tbuf[tpos - 1] == '\n')
					{
						break;
					}
				}
				return total;
			}

			/// <exception cref="IOException"/>
			public virtual byte Peek()
			{
				if (pos == count)
				{
					Fill();
				}
				if (pos == count)
				{
					return unchecked((byte) -1);
				}
				// nothing left in buffer
				return buf[pos];
			}

			/// <exception cref="IOException"/>
			public virtual int ReadLine(byte[] b)
			{
				return ReadLine(b, 0, b.Length);
			}

			/// <exception cref="IOException"/>
			public override long Skip(long n)
			{
				if (n <= 0)
				{
					return 0;
				}
				long avail = count - pos;
				if (avail <= 0)
				{
					return @in.Skip(n);
				}
				if (n > avail)
				{
					n = avail;
				}
				pos += (int)n;
				return n;
			}

			/// <exception cref="IOException"/>
			public override int Available()
			{
				return (count - pos) + @in.Available();
			}

			/// <exception cref="IOException"/>
			public override void Close()
			{
				if (@in != null)
				{
					@in.Close();
					@in = null;
					buf = null;
				}
			}

			/// <exception cref="IOException"/>
			private void Fill()
			{
				count = pos = 0;
				int n = @in.Read(buf, 0, buf.Length);
				if (n > 0)
				{
					count = n;
				}
			}
		}
	}
}
