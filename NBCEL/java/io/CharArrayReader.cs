/*
* Copyright (c) 1996, 2005, Oracle and/or its affiliates. All rights reserved.
* ORACLE PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
*
*/

using System;
using System.IO;
using Sharpen;

namespace Java.IO
{
	/// <summary>
	///     This class implements a character buffer that can be used as a
	///     character-input stream.
	/// </summary>
	/// <author>Herb Jellinek</author>
	/// <since>JDK1.1</since>
	public class CharArrayReader : TextReader
    {
        /// <summary>The character buffer.</summary>
        protected internal char[] buf;

        /// <summary>The index of the end of this buffer.</summary>
        /// <remarks>
        ///     The index of the end of this buffer.  There is not valid
        ///     data at or beyond this index.
        /// </remarks>
        protected internal int count;

        /// <summary>The position of mark in buffer.</summary>
        protected internal int markedPos;

        /// <summary>The current buffer position.</summary>
        protected internal int pos;

        /// <summary>Creates a CharArrayReader from the specified array of chars.</summary>
        /// <param name="buf">Input buffer (not copied)</param>
        public CharArrayReader(char[] buf)
        {
            this.buf = buf;
            pos = 0;
            count = buf.Length;
        }

        /// <summary>Creates a CharArrayReader from the specified array of chars.</summary>
        /// <remarks>
        ///     Creates a CharArrayReader from the specified array of chars.
        ///     <p>
        ///         The resulting reader will start reading at the given
        ///         <tt>offset</tt>.  The total number of <tt>char</tt> values that can be
        ///         read from this reader will be either <tt>length</tt> or
        ///         <tt>buf.length-offset</tt>, whichever is smaller.
        /// </remarks>
        /// <exception cref="System.ArgumentException">
        ///     If <tt>offset</tt> is negative or greater than
        ///     <tt>buf.length</tt>, or if <tt>length</tt> is negative, or if
        ///     the sum of these two values is negative.
        /// </exception>
        /// <param name="buf">Input buffer (not copied)</param>
        /// <param name="offset">Offset of the first char to read</param>
        /// <param name="length">Number of chars to read</param>
        public CharArrayReader(char[] buf, int offset, int length)
        {
            if (offset < 0 || offset > buf.Length || length < 0 || offset + length <
                0)
                throw new ArgumentException();
            this.buf = buf;
            pos = offset;
            count = Math.Min(offset + length, buf.Length);
            markedPos = offset;
        }

        public object Lock { get; } = new object();

        /// <summary>Checks to make sure that the stream has not been closed</summary>
        /// <exception cref="System.IO.IOException" />
        private void EnsureOpen()
        {
            if (buf == null) throw new IOException("Stream closed");
        }

        /// <summary>Reads a single character.</summary>
        /// <exception>
        ///     IOException
        ///     If an I/O error occurs
        /// </exception>
        /// <exception cref="System.IO.IOException" />
        public override int Read()
        {
            lock (Lock)
            {
                EnsureOpen();
                if (pos >= count)
                    return -1;
                return buf[pos++];
            }
        }

        /// <summary>Reads characters into a portion of an array.</summary>
        /// <param name="b">Destination buffer</param>
        /// <param name="off">Offset at which to start storing characters</param>
        /// <param name="len">Maximum number of characters to read</param>
        /// <returns>
        ///     The actual number of characters read, or -1 if
        ///     the end of the stream has been reached
        /// </returns>
        /// <exception>
        ///     IOException
        ///     If an I/O error occurs
        /// </exception>
        /// <exception cref="System.IO.IOException" />
        public override int Read(char[] b, int off, int len)
        {
            lock (Lock)
            {
                EnsureOpen();
                if (off < 0 || off > b.Length || len < 0 || off + len > b.Length || off
                    + len < 0)
                    throw new IndexOutOfRangeException();
                if (len == 0) return 0;
                if (pos >= count) return -1;
                var avail = count - pos;
                if (len > avail) len = avail;
                if (len <= 0) return 0;
                Array.Copy(buf, pos, b, off, len);
                pos += len;
                return len;
            }
        }

        /// <summary>Skips characters.</summary>
        /// <remarks>
        ///     Skips characters.  Returns the number of characters that were skipped.
        ///     <p>
        ///         The <code>n</code> parameter may be negative, even though the
        ///         <code>skip</code> method of the
        ///         <see cref="Reader" />
        ///         superclass throws
        ///         an exception in this case. If <code>n</code> is negative, then
        ///         this method does nothing and returns <code>0</code>.
        /// </remarks>
        /// <param name="n">The number of characters to skip</param>
        /// <returns>The number of characters actually skipped</returns>
        /// <exception>
        ///     IOException
        ///     If the stream is closed, or an I/O error occurs
        /// </exception>
        /// <exception cref="System.IO.IOException" />
        public long Skip(long n)
        {
            lock (Lock)
            {
                EnsureOpen();
                long avail = count - pos;
                if (n > avail) n = avail;
                if (n < 0) return 0;
                pos += (int) n;
                return n;
            }
        }

        /// <summary>Tells whether this stream is ready to be read.</summary>
        /// <remarks>
        ///     Tells whether this stream is ready to be read.  Character-array readers
        ///     are always ready to be read.
        /// </remarks>
        /// <exception>
        ///     IOException
        ///     If an I/O error occurs
        /// </exception>
        /// <exception cref="System.IO.IOException" />
        public bool Ready()
        {
            lock (Lock)
            {
                EnsureOpen();
                return count - pos > 0;
            }
        }

        /// <summary>Tells whether this stream supports the mark() operation, which it does.</summary>
        public bool MarkSupported()
        {
            return true;
        }

        /// <summary>Marks the present position in the stream.</summary>
        /// <remarks>
        ///     Marks the present position in the stream.  Subsequent calls to reset()
        ///     will reposition the stream to this point.
        /// </remarks>
        /// <param name="readAheadLimit">
        ///     Limit on the number of characters that may be
        ///     read while still preserving the mark.  Because
        ///     the stream's input comes from a character array,
        ///     there is no actual limit; hence this argument is
        ///     ignored.
        /// </param>
        /// <exception>
        ///     IOException
        ///     If an I/O error occurs
        /// </exception>
        /// <exception cref="System.IO.IOException" />
        public void Mark(int readAheadLimit)
        {
            lock (Lock)
            {
                EnsureOpen();
                markedPos = pos;
            }
        }

        /// <summary>
        ///     Resets the stream to the most recent mark, or to the beginning if it has
        ///     never been marked.
        /// </summary>
        /// <exception>
        ///     IOException
        ///     If an I/O error occurs
        /// </exception>
        /// <exception cref="System.IO.IOException" />
        public void Reset()
        {
            lock (Lock)
            {
                EnsureOpen();
                pos = markedPos;
            }
        }

        /// <summary>
        ///     Closes the stream and releases any system resources associated with
        ///     it.
        /// </summary>
        /// <remarks>
        ///     Closes the stream and releases any system resources associated with
        ///     it.  Once the stream has been closed, further read(), ready(),
        ///     mark(), reset(), or skip() invocations will throw an IOException.
        ///     Closing a previously closed stream has no effect.
        /// </remarks>
        public override void Close()
        {
            buf = null;
        }
    }
}