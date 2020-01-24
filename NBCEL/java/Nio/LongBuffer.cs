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
	/// <summary>A long buffer.</summary>
	/// <remarks>
	///     A long buffer.
	///     <p>
	///         This class defines four categories of operations upon
	///         long buffers:
	///         <ul>
	///             <li>
	///                 <p>
	///                     Absolute and relative
	///                     <see cref="Get()">
	///                         <i>get</i>
	///                     </see>
	///                     and
	///                     <see cref="Put(long)">
	///                         <i>put</i>
	///                     </see>
	///                     methods that read and write
	///                     single longs;
	///                 </p>
	///             </li>
	///             <li>
	///                 <p>
	///                     Relative
	///                     <see cref="Get(long[])">
	///                         <i>bulk get</i>
	///                     </see>
	///                     methods that transfer contiguous sequences of longs from this buffer
	///                     into an array; and
	///                 </p>
	///             </li>
	///             <li>
	///                 <p>
	///                     Relative
	///                     <see cref="Put(long[])">
	///                         <i>bulk put</i>
	///                     </see>
	///                     methods that transfer contiguous sequences of longs from a
	///                     long array or some other long
	///                     buffer into this buffer;&#32;and
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
	///                     a long buffer.
	///                 </p>
	///             </li>
	///         </ul>
	///         <p>
	///             Long buffers can be created either by
	///             <see cref="Allocate(int)">
	///                 <i>allocation</i>
	///             </see>
	///             , which allocates space for the buffer's
	///             content, by
	///             <see cref="Wrap(long[])">
	///                 <i>wrapping</i>
	///             </see>
	///             an existing
	///             long array  into a buffer, or by creating a
	///             <a href="ByteBuffer.html#views">
	///                 <i>view</i>
	///             </a>
	///             of an existing byte buffer.
	///             <p>
	///                 Like a byte buffer, a long buffer is either &lt;a
	///                 href="ByteBuffer.html#direct"&gt;<i>direct</i> or <i>non-direct</i></a>.  A
	///                 long buffer created via the <tt>wrap</tt> methods of this class will
	///                 be non-direct.  A long buffer created as a view of a byte buffer will
	///                 be direct if, and only if, the byte buffer itself is direct.  Whether or not
	///                 a long buffer is direct may be determined by invoking the
	///                 <see cref="IsDirect()">isDirect</see>
	///                 method.
	///             </p>
	///             <p>
	///                 Methods in this class that do not otherwise have a value to return are
	///                 specified to return the buffer upon which they are invoked.  This allows
	///                 method invocations to be chained.
	/// </remarks>
	/// <author>Mark Reinhold</author>
	/// <author>JSR-51 Expert Group</author>
	/// <since>1.4</since>
	public abstract class LongBuffer : Buffer, IComparable<LongBuffer>
    {
        internal readonly long[] hb;

        internal readonly int offset;

        internal bool isReadOnly;

        internal LongBuffer(int mark, int pos, int lim, int cap, long[] hb, int offset)
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

        internal LongBuffer(int mark, int pos, int lim, int cap)
            : this(mark, pos, lim, cap, null, 0)
        {
        }

        /// <summary>Compares this buffer to another.</summary>
        /// <remarks>
        ///     Compares this buffer to another.
        ///     <p>
        ///         Two long buffers are compared by comparing their sequences of
        ///         remaining elements lexicographically, without regard to the starting
        ///         position of each sequence within its corresponding buffer.
        ///         Pairs of
        ///         <c>long</c>
        ///         elements are compared as if by invoking
        ///         <see cref="System.Compare(long,long)" />
        ///         .
        ///         <p> A long buffer is not comparable to any other type of object.
        /// </remarks>
        /// <returns>
        ///     A negative integer, zero, or a positive integer as this buffer
        ///     is less than, equal to, or greater than the given buffer
        /// </returns>
        public virtual int CompareTo(LongBuffer that)
        {
            var n = Position() + Math.Min(Remaining(), that.Remaining());
            for (int i = Position(), j = that.Position(); i < n; i++, j++)
            {
                var cmp = Compare(Get(i), that.Get(j));
                if (cmp != 0) return cmp;
            }

            return Remaining() - that.Remaining();
        }

        // Creates a new buffer with the given mark, position, limit, and capacity
        //
        // package-private
        /// <summary>Allocates a new long buffer.</summary>
        /// <remarks>
        ///     Allocates a new long buffer.
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
        /// <param name="capacity">The new buffer's capacity, in longs</param>
        /// <returns>The new long buffer</returns>
        /// <exception cref="System.ArgumentException">
        ///     If the <tt>capacity</tt> is a negative integer
        /// </exception>
        public static LongBuffer Allocate(int capacity)
        {
            if (capacity < 0) throw new ArgumentException();
            return new HeapLongBuffer(capacity, capacity);
        }

        /// <summary>Wraps a long array into a buffer.</summary>
        /// <remarks>
        ///     Wraps a long array into a buffer.
        ///     <p>
        ///         The new buffer will be backed by the given long array;
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
        /// <returns>The new long buffer</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If the preconditions on the <tt>offset</tt> and <tt>length</tt>
        ///     parameters do not hold
        /// </exception>
        public static LongBuffer Wrap(long[] array, int offset, int length)
        {
            try
            {
                return new HeapLongBuffer(array, offset, length);
            }
            catch (ArgumentException)
            {
                throw new IndexOutOfRangeException();
            }
        }

        /// <summary>Wraps a long array into a buffer.</summary>
        /// <remarks>
        ///     Wraps a long array into a buffer.
        ///     <p>
        ///         The new buffer will be backed by the given long array;
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
        /// <returns>The new long buffer</returns>
        public static LongBuffer Wrap(long[] array)
        {
            return Wrap(array, 0, array.Length);
        }

        /// <summary>
        ///     Creates a new long buffer whose content is a shared subsequence of
        ///     this buffer's content.
        /// </summary>
        /// <remarks>
        ///     Creates a new long buffer whose content is a shared subsequence of
        ///     this buffer's content.
        ///     <p>
        ///         The content of the new buffer will start at this buffer's current
        ///         position.  Changes to this buffer's content will be visible in the new
        ///         buffer, and vice versa; the two buffers' position, limit, and mark
        ///         values will be independent.
        ///         <p>
        ///             The new buffer's position will be zero, its capacity and its limit
        ///             will be the number of longs remaining in this buffer, and its mark
        ///             will be undefined.  The new buffer will be direct if, and only if, this
        ///             buffer is direct, and it will be read-only if, and only if, this buffer
        ///             is read-only.
        ///         </p>
        /// </remarks>
        /// <returns>The new long buffer</returns>
        public abstract LongBuffer Slice();

        /// <summary>Creates a new long buffer that shares this buffer's content.</summary>
        /// <remarks>
        ///     Creates a new long buffer that shares this buffer's content.
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
        /// <returns>The new long buffer</returns>
        public abstract LongBuffer Duplicate();

        /// <summary>
        ///     Creates a new, read-only long buffer that shares this buffer's
        ///     content.
        /// </summary>
        /// <remarks>
        ///     Creates a new, read-only long buffer that shares this buffer's
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
        /// <returns>The new, read-only long buffer</returns>
        public abstract LongBuffer AsReadOnlyBuffer();

        // -- Singleton get/put methods --
        /// <summary>Relative <i>get</i> method.</summary>
        /// <remarks>
        ///     Relative <i>get</i> method.  Reads the long at this buffer's
        ///     current position, and then increments the position.
        /// </remarks>
        /// <returns>The long at the buffer's current position</returns>
        /// <exception cref="BufferUnderflowException">
        ///     If the buffer's current position is not smaller than its limit
        /// </exception>
        public abstract long Get();

        /// <summary>Relative <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.</summary>
        /// <remarks>
        ///     Relative <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes the given long into this buffer at the current
        ///         position, and then increments the position.
        ///     </p>
        /// </remarks>
        /// <param name="l">The long to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferOverflowException">
        ///     If this buffer's current position is not smaller than its limit
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract LongBuffer Put(long l);

        /// <summary>Absolute <i>get</i> method.</summary>
        /// <remarks>
        ///     Absolute <i>get</i> method.  Reads the long at the given
        ///     index.
        /// </remarks>
        /// <param name="index">The index from which the long will be read</param>
        /// <returns>The long at the given index</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit
        /// </exception>
        public abstract long Get(int index);

        /// <summary>Absolute <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.</summary>
        /// <remarks>
        ///     Absolute <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         Writes the given long into this buffer at the given
        ///         index.
        ///     </p>
        /// </remarks>
        /// <param name="index">The index at which the long will be written</param>
        /// <param name="l">The long value to be written</param>
        /// <returns>This buffer</returns>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If <tt>index</tt> is negative
        ///     or not smaller than the buffer's limit
        /// </exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract LongBuffer Put(int index, long l);

        // -- Bulk get operations --
        /// <summary>Relative bulk <i>get</i> method.</summary>
        /// <remarks>
        ///     Relative bulk <i>get</i> method.
        ///     <p>
        ///         This method transfers longs from this buffer into the given
        ///         destination array.  If there are fewer longs remaining in the
        ///         buffer than are required to satisfy the request, that is, if
        ///         <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
        ///         longs are transferred and a
        ///         <see cref="BufferUnderflowException" />
        ///         is
        ///         thrown.
        ///         <p>
        ///             Otherwise, this method copies <tt>length</tt> longs from this
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
        ///                 except that it first checks that there are sufficient longs in
        ///                 this buffer and it is potentially much more efficient.
        /// </remarks>
        /// <param name="dst">The array into which longs are to be written</param>
        /// <param name="offset">
        ///     The offset within the array of the first long to be
        ///     written; must be non-negative and no larger than
        ///     <tt>dst.length</tt>
        /// </param>
        /// <param name="length">
        ///     The maximum number of longs to be written to the given
        ///     array; must be non-negative and no larger than
        ///     <tt>dst.length - offset</tt>
        /// </param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferUnderflowException">
        ///     If there are fewer than <tt>length</tt> longs
        ///     remaining in this buffer
        /// </exception>
        /// <exception cref="System.IndexOutOfRangeException">
        ///     If the preconditions on the <tt>offset</tt> and <tt>length</tt>
        ///     parameters do not hold
        /// </exception>
        public virtual LongBuffer Get(long[] dst, int offset, int length)
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
        ///         This method transfers longs from this buffer into the given
        ///         destination array.  An invocation of this method of the form
        ///         <tt>src.get(a)</tt> behaves in exactly the same way as the invocation
        ///         <pre>
        ///             src.get(a, 0, a.length)
        ///         </pre>
        /// </remarks>
        /// <param name="dst">The destination array</param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferUnderflowException">
        ///     If there are fewer than <tt>length</tt> longs
        ///     remaining in this buffer
        /// </exception>
        public virtual LongBuffer Get(long[] dst)
        {
            return Get(dst, 0, dst.Length);
        }

        // -- Bulk put operations --
        /// <summary>Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.</summary>
        /// <remarks>
        ///     Relative bulk <i>put</i> method&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         This method transfers the longs remaining in the given source
        ///         buffer into this buffer.  If there are more longs remaining in the
        ///         source buffer than in this buffer, that is, if
        ///         <tt>src.remaining()</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>,
        ///         then no longs are transferred and a
        ///         <see cref="BufferOverflowException" />
        ///         is thrown.
        ///         <p>
        ///             Otherwise, this method copies
        ///             <i>n</i>&nbsp;=&nbsp;<tt>src.remaining()</tt> longs from the given
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
        ///     The source buffer from which longs are to be read;
        ///     must not be this buffer
        /// </param>
        /// <returns>This buffer</returns>
        /// <exception cref="BufferOverflowException">
        ///     If there is insufficient space in this buffer
        ///     for the remaining longs in the source buffer
        /// </exception>
        /// <exception cref="System.ArgumentException">If the source buffer is this buffer</exception>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public virtual LongBuffer Put(LongBuffer src)
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
        ///         This method transfers longs into this buffer from the given
        ///         source array.  If there are more longs to be copied from the array
        ///         than remain in this buffer, that is, if
        ///         <tt>length</tt>&nbsp;<tt>&gt;</tt>&nbsp;<tt>remaining()</tt>, then no
        ///         longs are transferred and a
        ///         <see cref="BufferOverflowException" />
        ///         is
        ///         thrown.
        ///         <p>
        ///             Otherwise, this method copies <tt>length</tt> longs from the
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
        /// <param name="src">The array from which longs are to be read</param>
        /// <param name="offset">
        ///     The offset within the array of the first long to be read;
        ///     must be non-negative and no larger than <tt>array.length</tt>
        /// </param>
        /// <param name="length">
        ///     The number of longs to be read from the given array;
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
        public virtual LongBuffer Put(long[] src, int offset, int length)
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
        ///         long array into this buffer.  An invocation of this method of the
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
        public LongBuffer Put(long[] src)
        {
            return Put(src, 0, src.Length);
        }

        // -- Other stuff --
        /// <summary>
        ///     Tells whether or not this buffer is backed by an accessible long
        ///     array.
        /// </summary>
        /// <remarks>
        ///     Tells whether or not this buffer is backed by an accessible long
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
        ///     Returns the long array that backs this
        ///     buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Returns the long array that backs this
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
        ///         The longs between the buffer's current position and its limit,
        ///         if any, are copied to the beginning of the buffer.  That is, the
        ///         long at index <i>p</i>&nbsp;=&nbsp;<tt>position()</tt> is copied
        ///         to index zero, the long at index <i>p</i>&nbsp;+&nbsp;1 is copied
        ///         to index one, and so forth until the long at index
        ///         <tt>limit()</tt>&nbsp;-&nbsp;1 is copied to index
        ///         <i>n</i>&nbsp;=&nbsp;<tt>limit()</tt>&nbsp;-&nbsp;<tt>1</tt>&nbsp;-&nbsp;<i>p</i>.
        ///         The buffer's position is then set to <i>n+1</i> and its limit is set to
        ///         its capacity.  The mark, if defined, is discarded.
        ///         <p>
        ///             The buffer's position is set to the number of longs copied,
        ///             rather than to zero, so that an invocation of this method can be
        ///             followed immediately by an invocation of another relative <i>put</i>
        ///             method.
        ///         </p>
        /// </remarks>
        /// <returns>This buffer</returns>
        /// <exception cref="ReadOnlyBufferException">If this buffer is read-only</exception>
        public abstract LongBuffer Compact();

        /// <summary>Tells whether or not this long buffer is direct.</summary>
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
        ///         The hash code of a long buffer depends only upon its remaining
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
            for (var i = Limit() - 1; i >= p; i--) h = 31 * h + (int) Get(i);
            return h;
        }

        /// <summary>Tells whether or not this buffer is equal to another object.</summary>
        /// <remarks>
        ///     Tells whether or not this buffer is equal to another object.
        ///     <p>
        ///         Two long buffers are equal if, and only if,
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
        ///         <p> A long buffer is not equal to any other type of object.  </p>
        /// </remarks>
        /// <param name="ob">The object to which this buffer is to be compared</param>
        /// <returns>
        ///     <tt>true</tt> if, and only if, this buffer is equal to the
        ///     given object
        /// </returns>
        public override bool Equals(object ob)
        {
            if (this == ob) return true;
            if (!(ob is LongBuffer)) return false;
            var that = (LongBuffer) ob;
            if (Remaining() != that.Remaining()) return false;
            var p = Position();
            for (int i = Limit() - 1, j = that.Limit() - 1; i >= p; i--, j--)
                if (!Equals(Get(i), that.Get(j)))
                    return false;
            return true;
        }

        private static bool Equals(long x, long y)
        {
            return x == y;
        }

        private static int Compare(long x, long y)
        {
            return x.CompareTo(y);
        }

        // -- Other char stuff --
        // -- Other byte stuff: Access to binary data --
        /// <summary>Retrieves this buffer's byte order.</summary>
        /// <remarks>
        ///     Retrieves this buffer's byte order.
        ///     <p>
        ///         The byte order of a long buffer created by allocation or by
        ///         wrapping an existing <tt>long</tt> array is the
        ///         <see cref="ByteOrder.NativeOrder()">native order</see>
        ///         of the underlying
        ///         hardware.  The byte order of a long buffer created as a &lt;a
        ///         href="ByteBuffer.html#views"&gt;view</a> of a byte buffer is that of the
        ///         byte buffer at the moment that the view is created.
        ///     </p>
        /// </remarks>
        /// <returns>This buffer's byte order</returns>
        public abstract ByteOrder Order();
    }
}