/*
* Copyright (c) 2000, 2013, Oracle and/or its affiliates. All rights reserved.
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
// -- This file was mechanically generated: Do not edit! -- //

using System;
using System.Text;

namespace ObjectWeb.Misc.Java.Nio
{
	/// <summary>A byte buffer.</summary>
	/// <remarks>
	///     A byte buffer.
	///     <p>
	///         This class defines six categories of operations upon
	///         byte buffers:
	///         <ul>
	///             <li>
	///                 <p>
	///                     Absolute and relative
	///                     <see cref="Get()">
	///                         <i>get</i>
	///                     </see>
	///                     and
	///                     <see cref="Put(byte)">
	///                         <i>put</i>
	///                     </see>
	///                     methods that read and write
	///                     single bytes;
	///                 </p>
	///             </li>
	///             <li>
	///                 <p>
	///                     Relative
	///                     <see cref="Get(byte[])">
	///                         <i>bulk get</i>
	///                     </see>
	///                     methods that transfer contiguous sequences of bytes from this buffer
	///                     into an array;
	///                 </p>
	///             </li>
	///             <li>
	///                 <p>
	///                     Relative
	///                     <see cref="Put(byte[])">
	///                         <i>bulk put</i>
	///                     </see>
	///                     methods that transfer contiguous sequences of bytes from a
	///                     byte array or some other byte
	///                     buffer into this buffer;
	///                 </p>
	///             </li>
	///             <li>
	///                 <p>
	///                     Absolute and relative
	///                     <see cref="GetChar()">
	///                         <i>get</i>
	///                     </see>
	///                     and
	///                     <see cref="PutChar(char)">
	///                         <i>put</i>
	///                     </see>
	///                     methods that read and
	///                     write values of other primitive types, translating them to and from
	///                     sequences of bytes in a particular byte order;
	///                 </p>
	///             </li>
	///             <li>
	///                 <p>
	///                     Methods for creating
	///                     <i>
	///                         <a href="#views">view buffers</a>
	///                     </i>
	///                     ,
	///                     which allow a byte buffer to be viewed as a buffer containing values of
	///                     some other primitive type; and
	///                 </p>
	///             </li>
	///             <li>
	///                 <p>
	///                     Methods for
	///                     <see cref="Compact()">compacting</see>
	///                     ,
	///                     <see cref="Duplicate()">duplicating</see>
	///                     , and
	///                     <see cref="Slice()">slicing</see>
	///                     a byte buffer.
	///                 </p>
	///             </li>
	///         </ul>
	///         <p>
	///             Byte buffers can be created either by
	///             <see cref="Allocate(int)">
	///                 <i>allocation</i>
	///             </see>
	///             , which allocates space for the buffer's
	///             content, or by
	///             <see cref="Wrap(byte[])">
	///                 <i>wrapping</i>
	///             </see>
	///             an
	///             existing byte array  into a buffer.
	///             <a name="direct"></a>
	///             <h2> Direct <i>vs.</i> non-direct buffers </h2>
	///             <p>
	///                 A byte buffer is either <i>direct</i> or <i>non-direct</i>.  Given a
	///                 direct byte buffer, the Java virtual machine will make a best effort to
	///                 perform native I/O operations directly upon it.  That is, it will attempt to
	///                 avoid copying the buffer's content to (or from) an intermediate buffer
	///                 before (or after) each invocation of one of the underlying operating
	///                 system's native I/O operations.
	///                 <p>
	///                     A direct byte buffer may be created by invoking the
	///                     <see cref="AllocateDirect(int)">allocateDirect</see>
	///                     factory method of this class.  The
	///                     buffers returned by this method typically have somewhat higher allocation
	///                     and deallocation costs than non-direct buffers.  The contents of direct
	///                     buffers may reside outside of the normal garbage-collected heap, and so
	///                     their impact upon the memory footprint of an application might not be
	///                     obvious.  It is therefore recommended that direct buffers be allocated
	///                     primarily for large, long-lived buffers that are subject to the underlying
	///                     system's native I/O operations.  In general it is best to allocate direct
	///                     buffers only when they yield a measureable gain in program performance.
	///                     <p>
	///                         A direct byte buffer may also be created by
	///                         <see
	///                             cref="Java.Nio.Channels.FileChannel.Map(Java.Nio.Channels.FileChannel.MapMode, long, long)
	/// 	">
	///                             mapping
	///                         </see>
	///                         a region of a file
	///                         directly into memory.  An implementation of the Java platform may optionally
	///                         support the creation of direct byte buffers from native code via JNI.  If an
	///                         instance of one of these kinds of buffers refers to an inaccessible region
	///                         of memory then an attempt to access that region will not change the buffer's
	///                         content and will cause an unspecified exception to be thrown either at the
	///                         time of the access or at some later time.
	///                         <p>
	///                             Whether a byte buffer is direct or non-direct may be determined by
	///                             invoking its
	///                             <see cref="IsDirect()">isDirect</see>
	///                             method.  This method is provided so
	///                             that explicit buffer management can be done in performance-critical code.
	///                             <a name="bin"></a>
	///                             <h2> Access to binary data </h2>
	///                             <p>
	///                                 This class defines methods for reading and writing values of all other
	///                                 primitive types, except <tt>boolean</tt>.  Primitive values are translated
	///                                 to (or from) sequences of bytes according to the buffer's current byte
	///                                 order, which may be retrieved and modified via the
	///                                 <see cref="Order()">order</see>
	///                                 methods.  Specific byte orders are represented by instances of the
	///                                 <see cref="ByteOrder" />
	///                                 class.  The initial order of a byte buffer is always
	///                                 <see cref="ByteOrder.Big_Endian">BIG_ENDIAN</see>
	///                                 .
	///                                 <p>
	///                                     For access to heterogeneous binary data, that is, sequences of values of
	///                                     different types, this class defines a family of absolute and relative
	///                                     <i>get</i> and <i>put</i> methods for each type.  For 32-bit floating-point
	///                                     values, for example, this class defines:
	///                                     <blockquote>
	///                                         <pre>
	///                                             float
	///                                             <see cref="GetFloat()" />
	///                                             float
	///                                             <see cref="GetFloat(int)">getFloat(int index)</see>
	///                                             void
	///                                             <see cref="PutFloat(float)">putFloat(float f)</see>
	///                                             void
	///                                             <see cref="PutFloat(int, float)">putFloat(int index, float f)</see>
	///                                         </pre>
	///                                     </blockquote>
	///                                     <p>
	///                                         Corresponding methods are defined for the types <tt>char</tt>,
	///                                         <tt>short</tt>, <tt>int</tt>, <tt>long</tt>, and <tt>double</tt>.  The index
	///                                         parameters of the absolute <i>get</i> and <i>put</i> methods are in terms of
	///                                         bytes rather than of the type being read or written.
	///                                         <a name="views"></a>
	///                                         <p>
	///                                             For access to homogeneous binary data, that is, sequences of values of
	///                                             the same type, this class defines methods that can create <i>views</i> of a
	///                                             given byte buffer.  A <i>view buffer</i> is simply another buffer whose
	///                                             content is backed by the byte buffer.  Changes to the byte buffer's content
	///                                             will be visible in the view buffer, and vice versa; the two buffers'
	///                                             position, limit, and mark values are independent.  The
	///                                             <see cref="AsFloatBuffer()">asFloatBuffer</see>
	///                                             method, for example, creates an instance of
	///                                             the
	///                                             <see cref="FloatBuffer" />
	///                                             class that is backed by the byte buffer upon which
	///                                             the method is invoked.  Corresponding view-creation methods are defined for
	///                                             the types <tt>char</tt>, <tt>short</tt>, <tt>int</tt>, <tt>long</tt>, and
	///                                             <tt>double</tt>.
	///                                             <p>
	///                                                 View buffers have three important advantages over the families of
	///                                                 type-specific <i>get</i> and <i>put</i> methods described above:
	///                                                 <ul>
	///                                                     <li>
	///                                                         <p>
	///                                                             A view buffer is indexed not in terms of bytes but rather
	///                                                             in terms
	///                                                             of the type-specific size of its values;
	///                                                         </p>
	///                                                     </li>
	///                                                     <li>
	///                                                         <p>
	///                                                             A view buffer provides relative bulk <i>get</i> and
	///                                                             <i>put</i>
	///                                                             methods that can transfer contiguous sequences of values
	///                                                             between a buffer
	///                                                             and an array or some other buffer of the same type; and
	///                                                         </p>
	///                                                     </li>
	///                                                     <li>
	///                                                         <p>
	///                                                             A view buffer is potentially much more efficient because it
	///                                                             will
	///                                                             be direct if, and only if, its backing byte buffer is
	///                                                             direct.
	///                                                         </p>
	///                                                     </li>
	///                                                 </ul>
	///                                                 <p>
	///                                                     The byte order of a view buffer is fixed to be that of its byte
	///                                                     buffer
	///                                                     at the time that the view is created.
	///                                                 </p>
	///                                                 <h2> Invocation chaining </h2>
	///                                                 <p>
	///                                                     Methods in this class that do not otherwise have a value to return
	///                                                     are
	///                                                     specified to return the buffer upon which they are invoked.  This
	///                                                     allows
	///                                                     method invocations to be chained.
	///                                                     The sequence of statements
	///                                                     <blockquote>
	///                                                         <pre>
	///                                                             bb.putInt(0xCAFEBABE);
	///                                                             bb.putShort(3);
	///                                                             bb.putShort(45);
	///                                                         </pre>
	///                                                     </blockquote>
	///                                                     can, for example, be replaced by the single statement
	///                                                     <blockquote>
	///                                                         <pre>
	///                                                             bb.putInt(0xCAFEBABE).putShort(3).putShort(45);
	///                                                         </pre>
	///                                                     </blockquote>
	/// </remarks>
	/// <author>Mark Reinhold</author>
	/// <author>JSR-51 Expert Group</author>
	/// <since>1.4</since>
	public abstract class ByteBuffer : Buffer, IComparable<ByteBuffer>
    {
        internal readonly byte[] hb;

        internal readonly int offset;

        internal bool bigEndian = true;

        internal bool isReadOnly;

        internal bool nativeByteOrder = !BitConverter.IsLittleEndian;

        internal ByteBuffer(int mark, int pos, int lim, int cap, byte[] hb, int offset)
            : base(mark, pos, lim, cap)
        {
            // These fields are declared here rather than in Heap-X-Buffer in order to
            // reduce the number of virtual method invocations needed to access these
            // values, which is especially costly when coding small buffers.
            //
            // Non-null only for heap buffers
            // Valid only for heap buffers
            // Creates a new buffer with the given mark, position, limit, capacity,
            // backing array, and array offset
            //
            // package-private
            this.hb = hb;
            this.offset = offset;
        }

        internal ByteBuffer(int mark, int pos, int lim, int cap)
            : this(mark, pos, lim, cap, null, 0)
        {
        }

        /// <summary>Compares this buffer to another.</summary>
        /// <remarks>
        ///     Compares this buffer to another.
        ///     <p>
        ///         Two byte buffers are compared by comparing their sequences of
        ///         remaining elements lexicographically, without regard to the starting
        ///         position of each sequence within its corresponding buffer.
        ///         Pairs of
        ///         <c>byte</c>
        ///         elements are compared as if by invoking
        ///         <see cref="byte.Compare(byte, byte)" />
        ///         .
        ///         <p> A byte buffer is not comparable to any other type of object.
        /// </remarks>
        /// <returns>
        ///     A negative integer, zero, or a positive integer as this buffer
        ///     is less than, equal to, or greater than the given buffer
        /// </returns>
        public virtual int CompareTo(ByteBuffer that)
        {
            var n = Position() + Math.Min(Remaining(), that.Remaining());
            for (int i = Position(), j = that.Position(); i < n; i++, j++)
            {
                var cmp = Compare(Get(i), that.Get(j));
                if (cmp != 0) return cmp;
            }

            return Remaining() - that.Remaining();
        }

        /// <summary>Allocates a new byte buffer.</summary>
        /// <remarks>
        ///     Allocates a new byte buffer.
        ///     <p>
        ///         The new buffer's position will be zero, its limit will be its
        ///         capacity, its mark will be undefined, and each of its elements will be
        ///         initialized to zero.  It will have a
        ///         <see cref="Array()">backing array</see>
        ///         ,
        ///         and its
        ///         <see cref="ArrayOffset()">array offset</see>
        ///         will be zero.
        /// </remarks>
        /// <param name="capacity">The new buffer's capacity, in bytes</param>
        /// <returns>The new byte buffer</returns>
        /// <exception cref="System.ArgumentException">
        ///     If the <tt>capacity</tt> is a negative integer
        /// </exception>
        public static ByteBuffer Allocate(int capacity)
        {
            if (capacity < 0) throw new ArgumentException();
            throw new NotImplementedException();
        }

        /// <summary>Wraps a byte array into a buffer.</summary>
        /// <remarks>
        ///     Wraps a byte array into a buffer.
        ///     <p>
        ///         The new buffer will be backed by the given byte array;
        ///         that is, modifications to the buffer will cause the array to be modified
        ///         and vice versa.  The new buffer's capacity will be
        ///         <tt>array.length</tt>, its position will be <tt>offset</tt>, its limit
        ///         will be <tt>offset + length</tt>, and its mark will be undefined.  Its
        ///         <see cref="Array()">backing array</see>
        ///         will be the given array, and
        ///         its
        ///         <see cref="ArrayOffset()">array offset</see>
        ///         will be zero.
        ///     </p>
        /// </remarks>
        /// <param name="array">The array that will back the new buffer</param>
        /// <param name="offset">
        ///     The offset of the subarray to be used; must be non-negative and
        ///     no larger than <tt>array.length</tt>.  The new buffer's position
        ///     will be set to this value.
        /// </param>
        /// <param name="length">
        ///     The length of the subarray to be used;
        ///     must be non-negative and no larger than
        ///     <tt>array.length - offset</tt>.
        ///     The new buffer's limit will be set to <tt>offset + length</tt>.
        /// </param>
        /// <returns>The new byte buffer</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If the preconditions on the <tt>offset</tt> and <tt>length</tt>
        ///     parameters do not hold
        /// </exception>
        public static ByteBuffer Wrap(byte[] array, int offset, int length)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (ArgumentException)
            {
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>Wraps a byte array into a buffer.</summary>
        /// <remarks>
        ///     Wraps a byte array into a buffer.
        ///     <p>
        ///         The new buffer will be backed by the given byte array;
        ///         that is, modifications to the buffer will cause the array to be modified
        ///         and vice versa.  The new buffer's capacity and limit will be
        ///         <tt>array.length</tt>, its position will be zero, and its mark will be
        ///         undefined.  Its
        ///         <see cref="Array()">backing array</see>
        ///         will be the
        ///         given array, and its
        ///         <see cref="ArrayOffset()">array offset&gt;</see>
        ///         will
        ///         be zero.
        ///     </p>
        /// </remarks>
        /// <param name="array">The array that will back this buffer</param>
        /// <returns>The new byte buffer</returns>
        [Obsolete("Not Implemented", true)]
        public static ByteBuffer Wrap(byte[] array)
        {
            return Wrap(array, 0, array.Length);
        }

        /// <summary>
        ///     Creates a new byte buffer whose content is a shared subsequence of
        ///     this buffer's content.
        /// </summary>
        /// <remarks>
        ///     Creates a new byte buffer whose content is a shared subsequence of
        ///     this buffer's content.
        ///     <p>
        ///         The content of the new buffer will start at this buffer's current
        ///         position.  Changes to this buffer's content will be visible in the new
        ///         buffer, and vice versa; the two buffers' position, limit, and mark
        ///         values will be independent.
        ///         <p>
        ///             The new buffer's position will be zero, its capacity and its limit
        ///             will be the number of bytes remaining in this buffer, and its mark
        ///             will be undefined.  The new buffer will be direct if, and only if, this
        ///             buffer is direct, and it will be read-only if, and only if, this buffer
        ///             is read-only.
        ///         </p>
        /// </remarks>
        /// <returns>The new byte buffer</returns>
        public abstract ByteBuffer Slice();

        /// <summary>Creates a new byte buffer that shares this buffer's content.</summary>
        /// <remarks>
        ///     Creates a new byte buffer that shares this buffer's content.
        ///     <p>
        ///         The content of the new buffer will be that of this buffer.  Changes
        ///         to this buffer's content will be visible in the new buffer, and vice
        ///         versa; the two buffers' position, limit, and mark values will be
        ///         independent.
        ///         <p>
        ///             The new buffer's capacity, limit, position, and mark values will be
        ///             identical to those of this buffer.  The new buffer will be direct if,
        ///             and only if, this buffer is direct, and it will be read-only if, and
        ///             only if, this buffer is read-only.
        ///         </p>
        /// </remarks>
        /// <returns>The new byte buffer</returns>
        public abstract ByteBuffer Duplicate();

        /// <summary>
        ///     Creates a new, read-only byte buffer that shares this buffer's
        ///     content.
        /// </summary>
        /// <remarks>
        ///     Creates a new, read-only byte buffer that shares this buffer's
        ///     content.
        ///     <p>
        ///         The content of the new buffer will be that of this buffer.  Changes
        ///         to this buffer's content will be visible in the new buffer; the new
        ///         buffer itself, however, will be read-only and will not allow the shared
        ///         content to be modified.  The two buffers' position, limit, and mark
        ///         values will be independent.
        ///         <p>
        ///             The new buffer's capacity, limit, position, and mark values will be
        ///             identical to those of this buffer.
        ///             <p>
        ///                 If this buffer is itself read-only then this method behaves in
        ///                 exactly the same way as the
        ///                 <see cref="Duplicate()">duplicate</see>
        ///                 method.
        ///             </p>
        /// </remarks>
        /// <returns>The new, read-only byte buffer</returns>
        public abstract ByteBuffer AsReadOnlyBuffer();

        // -- Singleton get/put methods --
        /// <summary>Relative <i>get</i> method.</summary>
        /// <remarks>
        ///     Relative <i>get</i> method.  Reads the byte at this buffer's
        ///     current position, and then increments the position.
        /// </remarks>
        /// <returns>The byte at the buffer's current position</returns>
        /// <exception cref="BufferUnderflowException">
        ///     If the buffer's current position is not smaller than its limit
        /// </exception>
        public abstract byte Get();

        /// <summary>Relative <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.</summary>
        /// <remarks>
        ///     Relative <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes the given byte into this buffer at the current
        ///         position, and then increments the position.
        ///     </p>
        /// </remarks>
        /// <param name="b">The byte to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferOverflowException">
        ///     If this buffer's current position is not smaller than its limit
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer Put(byte b);

        /// <summary>Absolute <i>get</i> method.</summary>
        /// <remarks>
        ///     Absolute <i>get</i> method.  Reads the byte at the given
        ///     index.
        /// </remarks>
        /// <param name="index">The index from which the byte will be read</param>
        /// <returns>The byte at the given index</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit
        /// </exception>
        public abstract byte Get(int index);

        /// <summary>Absolute <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.</summary>
        /// <remarks>
        ///     Absolute <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes the given byte into this buffer at the given
        ///         index.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index at which the byte will be written</param>
        /// <param name="b">The byte value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer Put(int index, byte b);

        // -- Bulk get operations --
        /// <summary>Relative bulk <i>get</i> method.</summary>
        /// <remarks>
        ///     Relative bulk <i>get</i> method.
        ///     <p>
        ///         This method transfers bytes from this buffer into the given
        ///         destination array.  If there are fewer bytes remaining in the
        ///         buffer than are required to satisfy the request, that is, if
        ///         <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
        ///         bytes are transferred and a
        ///         <see cref="BufferUnderflowException" />
        ///         is
        ///         thrown.
        ///         <p>
        ///             Otherwise, this method copies <tt>length</tt> bytes from this
        ///             buffer into the given array, starting at the current position of this
        ///             buffer and at the given offset in the array.  The position of this
        ///             buffer is then incremented by <tt>length</tt>.
        ///             <p>
        ///                 In other words, an invocation of this method of the form
        ///                 <tt>src.get(dst,&nbsp;off,&nbsp;len)</tt> has exactly the same effect as
        ///                 the loop
        ///                 <pre>
        ///                     <c>
        ///                         for (int i = off; i &lt; off + len; i++)
        ///                         dst[i] = src.get():
        ///                     </c>
        ///                 </pre>
        ///                 except that it first checks that there are sufficient bytes in
        ///                 this buffer and it is potentially much more efficient.
        /// </remarks>
        /// <param name="dst">The array into which bytes are to be written</param>
        /// <param name="offset">
        ///     The offset within the array of the first byte to be
        ///     written; must be non-negative and no larger than
        ///     <tt>dst.length</tt>
        /// </param>
        /// <param name="length">
        ///     The maximum number of bytes to be written to the given
        ///     array; must be non-negative and no larger than
        ///     <tt>dst.length - offset</tt>
        /// </param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferUnderflowException">
        ///     If there are fewer than <tt>length</tt> bytes
        ///     remaining in this buffer
        /// </exception>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If the preconditions on the <tt>offset</tt> and <tt>length</tt>
        ///     parameters do not hold
        /// </exception>
        public virtual ByteBuffer Get(byte[] dst, int offset, int length)
        {
            CheckBounds(offset, length, dst.Length);
            if (length > Remaining()) throw new BufferUnderflowException();
            var end = offset + length;
            for (var i = offset; i < end; i++) dst[i] = Get();
            return this;
        }

        /// <summary>Relative bulk <i>get</i> method.</summary>
        /// <remarks>
        ///     Relative bulk <i>get</i> method.
        ///     <p>
        ///         This method transfers bytes from this buffer into the given
        ///         destination array.  An invocation of this method of the form
        ///         <tt>src.get(a)</tt> behaves in exactly the same way as the invocation
        ///         <pre>
        ///             src.get(a, 0, a.length)
        ///         </pre>
        /// </remarks>
        /// <param name="dst">The destination array</param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferUnderflowException">
        ///     If there are fewer than <tt>length</tt> bytes
        ///     remaining in this buffer
        /// </exception>
        public virtual ByteBuffer Get(byte[] dst)
        {
            return Get(dst, 0, dst.Length);
        }

        // -- Bulk put operations --
        /// <summary>Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.</summary>
        /// <remarks>
        ///     Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         This method transfers the bytes remaining in the given source
        ///         buffer into this buffer.  If there are more bytes remaining in the
        ///         source buffer than in this buffer, that is, if
        ///         <tt>src.remaining()</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>,
        ///         then no bytes are transferred and a
        ///         <see cref="BufferOverflowException" />
        ///         is thrown.
        ///         <p>
        ///             Otherwise, this method copies
        ///             <i>n</i>&nbsp;=&nbsp;<tt>src.remaining()</tt> bytes from the given
        ///             buffer into this buffer, starting at each buffer's current position.
        ///             The positions of both buffers are then incremented by <i>n</i>.
        ///             <p>
        ///                 In other words, an invocation of this method of the form
        ///                 <tt>dst.put(src)</tt> has exactly the same effect as the loop
        ///                 <pre>
        ///                     while (src.hasRemaining())
        ///                     dst.put(src.get());
        ///                 </pre>
        ///                 except that it first checks that there is sufficient space in this
        ///                 buffer and it is potentially much more efficient.
        /// </remarks>
        /// <param name="src">
        ///     The source buffer from which bytes are to be read;
        ///     must not be this buffer
        /// </param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferOverflowException">
        ///     If there is insufficient space in this buffer
        ///     for the remaining bytes in the source buffer
        /// </exception>
        /// <exception cref="System.ArgumentException">If the source buffer is this buffer</exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public virtual ByteBuffer Put(ByteBuffer src)
        {
            if (src == this) throw new ArgumentException();
            if (IsReadOnly()) throw new ReadOnlyBufferException();
            var n = src.Remaining();
            if (n > Remaining()) throw new BufferOverflowException();
            for (var i = 0; i < n; i++) Put(src.Get());
            return this;
        }

        /// <summary>Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.</summary>
        /// <remarks>
        ///     Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         This method transfers bytes into this buffer from the given
        ///         source array.  If there are more bytes to be copied from the array
        ///         than remain in this buffer, that is, if
        ///         <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
        ///         bytes are transferred and a
        ///         <see cref="BufferOverflowException" />
        ///         is
        ///         thrown.
        ///         <p>
        ///             Otherwise, this method copies <tt>length</tt> bytes from the
        ///             given array into this buffer, starting at the given offset in the array
        ///             and at the current position of this buffer.  The position of this buffer
        ///             is then incremented by <tt>length</tt>.
        ///             <p>
        ///                 In other words, an invocation of this method of the form
        ///                 <tt>dst.put(src,&nbsp;off,&nbsp;len)</tt> has exactly the same effect as
        ///                 the loop
        ///                 <pre>
        ///                     <c>
        ///                         for (int i = off; i &lt; off + len; i++)
        ///                         dst.put(a[i]);
        ///                     </c>
        ///                 </pre>
        ///                 except that it first checks that there is sufficient space in this
        ///                 buffer and it is potentially much more efficient.
        /// </remarks>
        /// <param name="src">The array from which bytes are to be read</param>
        /// <param name="offset">
        ///     The offset within the array of the first byte to be read;
        ///     must be non-negative and no larger than <tt>array.length</tt>
        /// </param>
        /// <param name="length">
        ///     The number of bytes to be read from the given array;
        ///     must be non-negative and no larger than
        ///     <tt>array.length - offset</tt>
        /// </param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferOverflowException">
        ///     If there is insufficient space in this buffer
        /// </exception>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If the preconditions on the <tt>offset</tt> and <tt>length</tt>
        ///     parameters do not hold
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public virtual ByteBuffer Put(byte[] src, int offset, int length)
        {
            CheckBounds(offset, length, src.Length);
            if (length > Remaining()) throw new BufferOverflowException();
            var end = offset + length;
            for (var i = offset; i < end; i++) Put(src[i]);
            return this;
        }

        /// <summary>Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.</summary>
        /// <remarks>
        ///     Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         This method transfers the entire content of the given source
        ///         byte array into this buffer.  An invocation of this method of the
        ///         form <tt>dst.put(a)</tt> behaves in exactly the same way as the
        ///         invocation
        ///         <pre>
        ///             dst.put(a, 0, a.length)
        ///         </pre>
        /// </remarks>
        /// <param name="src">The source array</param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferOverflowException">
        ///     If there is insufficient space in this buffer
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public ByteBuffer Put(byte[] src)
        {
            return Put(src, 0, src.Length);
        }

        // -- Other stuff --
        /// <summary>
        ///     Tells whether or not this buffer is backed by an accessible byte
        ///     array.
        /// </summary>
        /// <remarks>
        ///     Tells whether or not this buffer is backed by an accessible byte
        ///     array.
        ///     <p>
        ///         If this method returns <tt>true</tt> then the
        ///         <see cref="Array()">array</see>
        ///         and
        ///         <see cref="ArrayOffset()">arrayOffset</see>
        ///         methods may safely be invoked.
        ///     </p>
        /// </remarks>
        /// <returns>
        ///     <tt>true</tt> if, and only if, this buffer
        ///     is backed by an array and is not read-only
        /// </returns>
        public sealed override bool HasArray()
        {
            return hb != null && !isReadOnly;
        }

        /// <summary>
        ///     Returns the byte array that backs this
        ///     buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Returns the byte array that backs this
        ///     buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Modifications to this buffer's content will cause the returned
        ///         array's content to be modified, and vice versa.
        ///         <p>
        ///             Invoke the
        ///             <see cref="HasArray()">hasArray</see>
        ///             method before invoking this
        ///             method in order to ensure that this buffer has an accessible backing
        ///             array.
        ///         </p>
        /// </remarks>
        /// <returns>The array that backs this buffer</returns>
        /// <exception cref="ReadOnlyBufferException">
        ///     If this buffer is backed by an array but is read-only
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///     If this buffer is not backed by an accessible array
        /// </exception>
        public sealed override object Array()
        {
            if (hb == null) throw new NotSupportedException();
            if (isReadOnly) throw new ReadOnlyBufferException();
            return hb;
        }

        /// <summary>
        ///     Returns the offset within this buffer's backing array of the first
        ///     element of the buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Returns the offset within this buffer's backing array of the first
        ///     element of the buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         If this buffer is backed by an array then buffer position <i>p</i>
        ///         corresponds to array index <i>p</i>&nbsp;+&nbsp;<tt>arrayOffset()</tt>.
        ///         <p>
        ///             Invoke the
        ///             <see cref="HasArray()">hasArray</see>
        ///             method before invoking this
        ///             method in order to ensure that this buffer has an accessible backing
        ///             array.
        ///         </p>
        /// </remarks>
        /// <returns>
        ///     The offset within this buffer's array
        ///     of the first element of the buffer
        /// </returns>
        /// <exception cref="ReadOnlyBufferException">
        ///     If this buffer is backed by an array but is read-only
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///     If this buffer is not backed by an accessible array
        /// </exception>
        public sealed override int ArrayOffset()
        {
            if (hb == null) throw new NotSupportedException();
            if (isReadOnly) throw new ReadOnlyBufferException();
            return offset;
        }

        /// <summary>Compacts this buffer&nbsp;&nbsp;<i>(optional operation)</i>.</summary>
        /// <remarks>
        ///     Compacts this buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         The bytes between the buffer's current position and its limit,
        ///         if any, are copied to the beginning of the buffer.  That is, the
        ///         byte at index <i>p</i>&nbsp;=&nbsp;<tt>position()</tt> is copied
        ///         to index zero, the byte at index <i>p</i>&nbsp;+&nbsp;1 is copied
        ///         to index one, and so forth until the byte at index
        ///         <tt>limit()</tt>&nbsp;-&nbsp;1 is copied to index
        ///         <i>n</i>&nbsp;=&nbsp;<tt>limit()</tt>&nbsp;-&nbsp;<tt>1</tt>&nbsp;-&nbsp;<i>p</i>.
        ///         The buffer's position is then set to <i>n+1</i> and its limit is set to
        ///         its capacity.  The mark, if defined, is discarded.
        ///         <p>
        ///             The buffer's position is set to the number of bytes copied,
        ///             rather than to zero, so that an invocation of this method can be
        ///             followed immediately by an invocation of another relative <i>put</i>
        ///             method.
        ///         </p>
        ///         <p>
        ///             Invoke this method after writing data from a buffer in case the
        ///             write was incomplete.  The following loop, for example, copies bytes
        ///             from one channel to another via the buffer <tt>buf</tt>:
        ///             <blockquote>
        ///                 <pre>
        ///                     <c>
        ///                         buf.clear();          // Prepare buffer for use
        ///                         while (in.read(buf) &gt;= 0 || buf.position != 0)
        ///                         buf.flip();
        ///                         out.write(buf);
        ///                         buf.compact();    // In case of partial write
        ///                     </c>
        ///                     }
        ///                 </pre>
        ///             </blockquote>
        /// </remarks>
        /// <returns>This buffer</returns>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer Compact();

        /// <summary>Tells whether or not this byte buffer is direct.</summary>
        /// <returns><tt>true</tt> if, and only if, this buffer is direct</returns>
        public abstract override bool IsDirect();

        /// <summary>Returns a string summarizing the state of this buffer.</summary>
        /// <returns>A summary string</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(GetType().FullName);
            sb.Append("[pos=");
            sb.Append(Position());
            sb.Append(" lim=");
            sb.Append(Limit());
            sb.Append(" cap=");
            sb.Append(Capacity());
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>Returns the current hash code of this buffer.</summary>
        /// <remarks>
        ///     Returns the current hash code of this buffer.
        ///     <p>
        ///         The hash code of a byte buffer depends only upon its remaining
        ///         elements; that is, upon the elements from <tt>position()</tt> up to, and
        ///         including, the element at <tt>limit()</tt>&nbsp;-&nbsp;<tt>1</tt>.
        ///         <p>
        ///             Because buffer hash codes are content-dependent, it is inadvisable
        ///             to use buffers as keys in hash maps or similar data structures unless it
        ///             is known that their contents will not change.
        ///         </p>
        /// </remarks>
        /// <returns>The current hash code of this buffer</returns>
        public override int GetHashCode()
        {
            var h = 1;
            var p = Position();
            for (var i = Limit() - 1; i >= p; i--) h = 31 * h + Get(i);
            return h;
        }

        /// <summary>Tells whether or not this buffer is equal to another object.</summary>
        /// <remarks>
        ///     Tells whether or not this buffer is equal to another object.
        ///     <p>
        ///         Two byte buffers are equal if, and only if,
        ///         <ol>
        ///             <li>
        ///                 <p> They have the same element type,  </p>
        ///             </li>
        ///             <li>
        ///                 <p>
        ///                     They have the same number of remaining elements, and
        ///                 </p>
        ///             </li>
        ///             <li>
        ///                 <p>
        ///                     The two sequences of remaining elements, considered
        ///                     independently of their starting positions, are pointwise equal.
        ///                 </p>
        ///             </li>
        ///         </ol>
        ///         <p> A byte buffer is not equal to any other type of object.  </p>
        /// </remarks>
        /// <param name="ob">The object to which this buffer is to be compared</param>
        /// <returns>
        ///     <tt>true</tt> if, and only if, this buffer is equal to the
        ///     given object
        /// </returns>
        public override bool Equals(object ob)
        {
            if (this == ob) return true;
            if (!(ob is ByteBuffer)) return false;
            var that = (ByteBuffer) ob;
            if (Remaining() != that.Remaining()) return false;
            var p = Position();
            for (int i = Limit() - 1, j = that.Limit() - 1; i >= p; i--, j--)
                if (!Equals(Get(i), that.Get(j)))
                    return false;
            return true;
        }

        private static bool Equals(byte x, byte y)
        {
            return x == y;
        }

        private static int Compare(byte x, byte y)
        {
            return x.CompareTo(y);
        }

        // -- Other char stuff --
        // -- Other byte stuff: Access to binary data --
        // package-private
        // package-private
        /// <summary>Retrieves this buffer's byte order.</summary>
        /// <remarks>
        ///     Retrieves this buffer's byte order.
        ///     <p>
        ///         The byte order is used when reading or writing multibyte values, and
        ///         when creating buffers that are views of this byte buffer.  The order of
        ///         a newly-created byte buffer is always
        ///         <see cref="ByteOrder.Big_Endian">BIG_ENDIAN</see>
        ///         .
        ///     </p>
        /// </remarks>
        /// <returns>This buffer's byte order</returns>
        public ByteOrder Order()
        {
            return bigEndian ? ByteOrder.Big_Endian : ByteOrder.Little_Endian;
        }

        /// <summary>Modifies this buffer's byte order.</summary>
        /// <param name="bo">
        ///     The new byte order,
        ///     either
        ///     <see cref="ByteOrder.Big_Endian">BIG_ENDIAN</see>
        ///     or
        ///     <see cref="ByteOrder.Little_Endian">LITTLE_ENDIAN</see>
        /// </param>
        /// <returns>This buffer</returns>
        public ByteBuffer Order(ByteOrder bo)
        {
            bigEndian = bo == ByteOrder.Big_Endian;
            nativeByteOrder = bigEndian == !BitConverter.IsLittleEndian;
            return this;
        }

        // Unchecked accessors, for use by ByteBufferAs-X-Buffer classes
        //
        internal abstract byte _get(int i);

        // package-private
        internal abstract void _put(int i, byte b);

        // package-private
        /// <summary>Relative <i>get</i> method for reading a char value.</summary>
        /// <remarks>
        ///     Relative <i>get</i> method for reading a char value.
        ///     <p>
        ///         Reads the next two bytes at this buffer's current position,
        ///         composing them into a char value according to the current byte order,
        ///         and then increments the position by two.
        ///     </p>
        /// </remarks>
        /// <returns>The char value at the buffer's current position</returns>
        /// <exception cref="BufferUnderflowException">
        ///     If there are fewer than two bytes
        ///     remaining in this buffer
        /// </exception>
        public abstract char GetChar();

        /// <summary>
        ///     Relative <i>put</i> method for writing a char
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Relative <i>put</i> method for writing a char
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes two bytes containing the given char value, in the
        ///         current byte order, into this buffer at the current position, and then
        ///         increments the position by two.
        ///     </p>
        /// </remarks>
        /// <param name="value">The char value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferOverflowException">
        ///     If there are fewer than two bytes
        ///     remaining in this buffer
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer PutChar(char value);

        /// <summary>Absolute <i>get</i> method for reading a char value.</summary>
        /// <remarks>
        ///     Absolute <i>get</i> method for reading a char value.
        ///     <p>
        ///         Reads two bytes at the given index, composing them into a
        ///         char value according to the current byte order.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index from which the bytes will be read</param>
        /// <returns>The char value at the given index</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit,
        ///     minus one
        /// </exception>
        public abstract char GetChar(int index);

        /// <summary>
        ///     Absolute <i>put</i> method for writing a char
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Absolute <i>put</i> method for writing a char
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes two bytes containing the given char value, in the
        ///         current byte order, into this buffer at the given index.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index at which the bytes will be written</param>
        /// <param name="value">The char value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit,
        ///     minus one
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer PutChar(int index, char value);

        /// <summary>Relative <i>get</i> method for reading a short value.</summary>
        /// <remarks>
        ///     Relative <i>get</i> method for reading a short value.
        ///     <p>
        ///         Reads the next two bytes at this buffer's current position,
        ///         composing them into a short value according to the current byte order,
        ///         and then increments the position by two.
        ///     </p>
        /// </remarks>
        /// <returns>The short value at the buffer's current position</returns>
        /// <exception cref="BufferUnderflowException">
        ///     If there are fewer than two bytes
        ///     remaining in this buffer
        /// </exception>
        public abstract short GetShort();

        /// <summary>
        ///     Relative <i>put</i> method for writing a short
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Relative <i>put</i> method for writing a short
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes two bytes containing the given short value, in the
        ///         current byte order, into this buffer at the current position, and then
        ///         increments the position by two.
        ///     </p>
        /// </remarks>
        /// <param name="value">The short value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferOverflowException">
        ///     If there are fewer than two bytes
        ///     remaining in this buffer
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer PutShort(short value);

        /// <summary>Absolute <i>get</i> method for reading a short value.</summary>
        /// <remarks>
        ///     Absolute <i>get</i> method for reading a short value.
        ///     <p>
        ///         Reads two bytes at the given index, composing them into a
        ///         short value according to the current byte order.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index from which the bytes will be read</param>
        /// <returns>The short value at the given index</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit,
        ///     minus one
        /// </exception>
        public abstract short GetShort(int index);

        /// <summary>
        ///     Absolute <i>put</i> method for writing a short
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Absolute <i>put</i> method for writing a short
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes two bytes containing the given short value, in the
        ///         current byte order, into this buffer at the given index.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index at which the bytes will be written</param>
        /// <param name="value">The short value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit,
        ///     minus one
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer PutShort(int index, short value);

        /// <summary>Relative <i>get</i> method for reading an int value.</summary>
        /// <remarks>
        ///     Relative <i>get</i> method for reading an int value.
        ///     <p>
        ///         Reads the next four bytes at this buffer's current position,
        ///         composing them into an int value according to the current byte order,
        ///         and then increments the position by four.
        ///     </p>
        /// </remarks>
        /// <returns>The int value at the buffer's current position</returns>
        /// <exception cref="BufferUnderflowException">
        ///     If there are fewer than four bytes
        ///     remaining in this buffer
        /// </exception>
        public abstract int GetInt();

        /// <summary>
        ///     Relative <i>put</i> method for writing an int
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Relative <i>put</i> method for writing an int
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes four bytes containing the given int value, in the
        ///         current byte order, into this buffer at the current position, and then
        ///         increments the position by four.
        ///     </p>
        /// </remarks>
        /// <param name="value">The int value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferOverflowException">
        ///     If there are fewer than four bytes
        ///     remaining in this buffer
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer PutInt(int value);

        /// <summary>Absolute <i>get</i> method for reading an int value.</summary>
        /// <remarks>
        ///     Absolute <i>get</i> method for reading an int value.
        ///     <p>
        ///         Reads four bytes at the given index, composing them into a
        ///         int value according to the current byte order.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index from which the bytes will be read</param>
        /// <returns>The int value at the given index</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit,
        ///     minus three
        /// </exception>
        public abstract int GetInt(int index);

        /// <summary>
        ///     Absolute <i>put</i> method for writing an int
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Absolute <i>put</i> method for writing an int
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes four bytes containing the given int value, in the
        ///         current byte order, into this buffer at the given index.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index at which the bytes will be written</param>
        /// <param name="value">The int value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit,
        ///     minus three
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer PutInt(int index, int value);

        /// <summary>Relative <i>get</i> method for reading a long value.</summary>
        /// <remarks>
        ///     Relative <i>get</i> method for reading a long value.
        ///     <p>
        ///         Reads the next eight bytes at this buffer's current position,
        ///         composing them into a long value according to the current byte order,
        ///         and then increments the position by eight.
        ///     </p>
        /// </remarks>
        /// <returns>The long value at the buffer's current position</returns>
        /// <exception cref="BufferUnderflowException">
        ///     If there are fewer than eight bytes
        ///     remaining in this buffer
        /// </exception>
        public abstract long GetLong();

        /// <summary>
        ///     Relative <i>put</i> method for writing a long
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Relative <i>put</i> method for writing a long
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes eight bytes containing the given long value, in the
        ///         current byte order, into this buffer at the current position, and then
        ///         increments the position by eight.
        ///     </p>
        /// </remarks>
        /// <param name="value">The long value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferOverflowException">
        ///     If there are fewer than eight bytes
        ///     remaining in this buffer
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer PutLong(long value);

        /// <summary>Absolute <i>get</i> method for reading a long value.</summary>
        /// <remarks>
        ///     Absolute <i>get</i> method for reading a long value.
        ///     <p>
        ///         Reads eight bytes at the given index, composing them into a
        ///         long value according to the current byte order.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index from which the bytes will be read</param>
        /// <returns>The long value at the given index</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit,
        ///     minus seven
        /// </exception>
        public abstract long GetLong(int index);

        /// <summary>
        ///     Absolute <i>put</i> method for writing a long
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Absolute <i>put</i> method for writing a long
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes eight bytes containing the given long value, in the
        ///         current byte order, into this buffer at the given index.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index at which the bytes will be written</param>
        /// <param name="value">The long value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit,
        ///     minus seven
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer PutLong(int index, long value);

        /// <summary>Creates a view of this byte buffer as a long buffer.</summary>
        /// <remarks>
        ///     Creates a view of this byte buffer as a long buffer.
        ///     <p>
        ///         The content of the new buffer will start at this buffer's current
        ///         position.  Changes to this buffer's content will be visible in the new
        ///         buffer, and vice versa; the two buffers' position, limit, and mark
        ///         values will be independent.
        ///         <p>
        ///             The new buffer's position will be zero, its capacity and its limit
        ///             will be the number of bytes remaining in this buffer divided by
        ///             eight, and its mark will be undefined.  The new buffer will be direct
        ///             if, and only if, this buffer is direct, and it will be read-only if, and
        ///             only if, this buffer is read-only.
        ///         </p>
        /// </remarks>
        /// <returns>A new long buffer</returns>
        public abstract LongBuffer AsLongBuffer();

        /// <summary>Relative <i>get</i> method for reading a float value.</summary>
        /// <remarks>
        ///     Relative <i>get</i> method for reading a float value.
        ///     <p>
        ///         Reads the next four bytes at this buffer's current position,
        ///         composing them into a float value according to the current byte order,
        ///         and then increments the position by four.
        ///     </p>
        /// </remarks>
        /// <returns>The float value at the buffer's current position</returns>
        /// <exception cref="BufferUnderflowException">
        ///     If there are fewer than four bytes
        ///     remaining in this buffer
        /// </exception>
        public abstract float GetFloat();

        /// <summary>
        ///     Relative <i>put</i> method for writing a float
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Relative <i>put</i> method for writing a float
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes four bytes containing the given float value, in the
        ///         current byte order, into this buffer at the current position, and then
        ///         increments the position by four.
        ///     </p>
        /// </remarks>
        /// <param name="value">The float value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferOverflowException">
        ///     If there are fewer than four bytes
        ///     remaining in this buffer
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer PutFloat(float value);

        /// <summary>Absolute <i>get</i> method for reading a float value.</summary>
        /// <remarks>
        ///     Absolute <i>get</i> method for reading a float value.
        ///     <p>
        ///         Reads four bytes at the given index, composing them into a
        ///         float value according to the current byte order.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index from which the bytes will be read</param>
        /// <returns>The float value at the given index</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit,
        ///     minus three
        /// </exception>
        public abstract float GetFloat(int index);

        /// <summary>
        ///     Absolute <i>put</i> method for writing a float
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Absolute <i>put</i> method for writing a float
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes four bytes containing the given float value, in the
        ///         current byte order, into this buffer at the given index.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index at which the bytes will be written</param>
        /// <param name="value">The float value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit,
        ///     minus three
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer PutFloat(int index, float value);

        /// <summary>Relative <i>get</i> method for reading a double value.</summary>
        /// <remarks>
        ///     Relative <i>get</i> method for reading a double value.
        ///     <p>
        ///         Reads the next eight bytes at this buffer's current position,
        ///         composing them into a double value according to the current byte order,
        ///         and then increments the position by eight.
        ///     </p>
        /// </remarks>
        /// <returns>The double value at the buffer's current position</returns>
        /// <exception cref="BufferUnderflowException">
        ///     If there are fewer than eight bytes
        ///     remaining in this buffer
        /// </exception>
        public abstract double GetDouble();

        /// <summary>
        ///     Relative <i>put</i> method for writing a double
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Relative <i>put</i> method for writing a double
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes eight bytes containing the given double value, in the
        ///         current byte order, into this buffer at the current position, and then
        ///         increments the position by eight.
        ///     </p>
        /// </remarks>
        /// <param name="value">The double value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferOverflowException">
        ///     If there are fewer than eight bytes
        ///     remaining in this buffer
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer PutDouble(double value);

        /// <summary>Absolute <i>get</i> method for reading a double value.</summary>
        /// <remarks>
        ///     Absolute <i>get</i> method for reading a double value.
        ///     <p>
        ///         Reads eight bytes at the given index, composing them into a
        ///         double value according to the current byte order.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index from which the bytes will be read</param>
        /// <returns>The double value at the given index</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit,
        ///     minus seven
        /// </exception>
        public abstract double GetDouble(int index);

        /// <summary>
        ///     Absolute <i>put</i> method for writing a double
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Absolute <i>put</i> method for writing a double
        ///     value&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes eight bytes containing the given double value, in the
        ///         current byte order, into this buffer at the given index.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index at which the bytes will be written</param>
        /// <param name="value">The double value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit,
        ///     minus seven
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract ByteBuffer PutDouble(int index, double value);
    }

    public class ReadOnlyBufferException : Exception
    {
    }
}