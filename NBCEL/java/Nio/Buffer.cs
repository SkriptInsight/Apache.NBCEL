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

using System;
using ObjectWeb.Misc.Java.Util;

namespace ObjectWeb.Misc.Java.Nio
{
	/// <summary>A container for data of a specific primitive type.</summary>
	/// <remarks>
	///     A container for data of a specific primitive type.
	///     <p>
	///         A buffer is a linear, finite sequence of elements of a specific
	///         primitive type.  Aside from its content, the essential properties of a
	///         buffer are its capacity, limit, and position:
	///     </p>
	///     <blockquote>
	///         <p>
	///             A buffer's <i>capacity</i> is the number of elements it contains.  The
	///             capacity of a buffer is never negative and never changes.
	///         </p>
	///         <p>
	///             A buffer's <i>limit</i> is the index of the first element that should
	///             not be read or written.  A buffer's limit is never negative and is never
	///             greater than its capacity.
	///         </p>
	///         <p>
	///             A buffer's <i>position</i> is the index of the next element to be
	///             read or written.  A buffer's position is never negative and is never
	///             greater than its limit.
	///         </p>
	///     </blockquote>
	///     <p>
	///         There is one subclass of this class for each non-boolean primitive type.
	///         <h2> Transferring data </h2>
	///         <p>
	///             Each subclass of this class defines two categories of <i>get</i> and
	///             <i>put</i> operations:
	///         </p>
	///         <blockquote>
	///             <p>
	///                 <i>Relative</i> operations read or write one or more elements starting
	///                 at the current position and then increment the position by the number of
	///                 elements transferred.  If the requested transfer exceeds the limit then a
	///                 relative <i>get</i> operation throws a
	///                 <see cref="BufferUnderflowException" />
	///                 and a relative <i>put</i> operation throws a
	///                 <see cref="BufferOverflowException" />
	///                 ; in either case, no data is transferred.
	///             </p>
	///             <p>
	///                 <i>Absolute</i> operations take an explicit element index and do not
	///                 affect the position.  Absolute <i>get</i> and <i>put</i> operations throw
	///                 an
	///                 <see cref="IndexOutOfRangeException" />
	///                 if the index argument exceeds the
	///                 limit.
	///             </p>
	///         </blockquote>
	///         <p>
	///             Data may also, of course, be transferred in to or out of a buffer by the
	///             I/O operations of an appropriate channel, which are always relative to the
	///             current position.
	///             <h2> Marking and resetting </h2>
	///             <p>
	///                 A buffer's <i>mark</i> is the index to which its position will be reset
	///                 when the
	///                 <see cref="Reset()">reset</see>
	///                 method is invoked.  The mark is not always
	///                 defined, but when it is defined it is never negative and is never greater
	///                 than the position.  If the mark is defined then it is discarded when the
	///                 position or the limit is adjusted to a value smaller than the mark.  If the
	///                 mark is not defined then invoking the
	///                 <see cref="Reset()">reset</see>
	///                 method causes an
	///                 <see cref="InvalidMarkException" />
	///                 to be thrown.
	///                 <h2> Invariants </h2>
	///                 <p>
	///                     The following invariant holds for the mark, position, limit, and
	///                     capacity values:
	///                     <blockquote>
	///                         <tt>0</tt> <tt>&lt;=</tt>
	///                         <i>mark</i> <tt>&lt;=</tt>
	///                         <i>position</i> <tt>&lt;=</tt>
	///                         <i>limit</i> <tt>&lt;=</tt>
	///                         <i>capacity</i>
	///                     </blockquote>
	///                     <p>
	///                         A newly-created buffer always has a position of zero and a mark that is
	///                         undefined.  The initial limit may be zero, or it may be some other value
	///                         that depends upon the type of the buffer and the manner in which it is
	///                         constructed.  Each element of a newly-allocated buffer is initialized
	///                         to zero.
	///                         <h2> Clearing, flipping, and rewinding </h2>
	///                         <p>
	///                             In addition to methods for accessing the position, limit, and capacity
	///                             values and for marking and resetting, this class also defines the following
	///                             operations upon buffers:
	///                             <ul>
	///                                 <li>
	///                                     <p>
	///                                         <see cref="Clear()" />
	///                                         makes a buffer ready for a new sequence of
	///                                         channel-read or relative <i>put</i> operations: It sets the limit to the
	///                                         capacity and the position to zero.
	///                                     </p>
	///                                 </li>
	///                                 <li>
	///                                     <p>
	///                                         <see cref="Flip()" />
	///                                         makes a buffer ready for a new sequence of
	///                                         channel-write or relative <i>get</i> operations: It sets the limit to the
	///                                         current position and then sets the position to zero.
	///                                     </p>
	///                                 </li>
	///                                 <li>
	///                                     <p>
	///                                         <see cref="Rewind()" />
	///                                         makes a buffer ready for re-reading the data that
	///                                         it already contains: It leaves the limit unchanged and sets the position
	///                                         to zero.
	///                                     </p>
	///                                 </li>
	///                             </ul>
	///                             <h2> Read-only buffers </h2>
	///                             <p>
	///                                 Every buffer is readable, but not every buffer is writable.  The
	///                                 mutation methods of each buffer class are specified as
	///                                 <i>
	///                                     optional
	///                                     operations
	///                                 </i>
	///                                 that will throw a
	///                                 <see cref="ReadOnlyBufferException" />
	///                                 when
	///                                 invoked upon a read-only buffer.  A read-only buffer does not allow its
	///                                 content to be changed, but its mark, position, and limit values are mutable.
	///                                 Whether or not a buffer is read-only may be determined by invoking its
	///                                 <see cref="IsReadOnly()">isReadOnly</see>
	///                                 method.
	///                                 <h2> Thread safety </h2>
	///                                 <p>
	///                                     Buffers are not safe for use by multiple concurrent threads.  If a
	///                                     buffer is to be used by more than one thread then access to the buffer
	///                                     should be controlled by appropriate synchronization.
	///                                     <h2> Invocation chaining </h2>
	///                                     <p>
	///                                         Methods in this class that do not otherwise have a value to return are
	///                                         specified to return the buffer upon which they are invoked.  This allows
	///                                         method invocations to be chained; for example, the sequence of statements
	///                                         <blockquote>
	///                                             <pre>
	///                                                 b.flip();
	///                                                 b.position(23);
	///                                                 b.limit(42);
	///                                             </pre>
	///                                         </blockquote>
	///                                         can be replaced by the single, more compact statement
	///                                         <blockquote>
	///                                             <pre>
	///                                                 b.flip().position(23).limit(42);
	///                                             </pre>
	///                                         </blockquote>
	/// </remarks>
	/// <author>Mark Reinhold</author>
	/// <author>JSR-51 Expert Group</author>
	/// <since>1.4</since>
	public abstract class Buffer
    {
	    /// <summary>
	    ///     The characteristics of Spliterators that traverse and split elements
	    ///     maintained in Buffers.
	    /// </summary>
	    internal const int Spliterator_Characteristics = SpliteratorConstants.Sized | SpliteratorConstants
		                                                     .Subsized | SpliteratorConstants.Ordered;

        internal long address;

        private int capacity__;

        private int limit__;

        private int mark__ = -1;

        private int position__;

        internal Buffer(int mark, int pos, int lim, int cap)
        {
            // Invariants: mark <= position <= limit <= capacity
            // Used only by direct buffers
            // NOTE: hoisted here for speed in JNI GetDirectBufferAddress
            // Creates a new buffer with the given mark, position, limit, and capacity,
            // after checking invariants.
            //
            // package-private
            if (cap < 0) throw new ArgumentException("Negative capacity: " + cap);
            capacity__ = cap;
            Limit(lim);
            Position(pos);
            if (mark >= 0)
            {
                if (mark > pos) throw new ArgumentException("mark > position: (" + mark + " > " + pos + ")");
                mark__ = mark;
            }
        }

        /// <summary>Returns this buffer's capacity.</summary>
        /// <returns>The capacity of this buffer</returns>
        public int Capacity()
        {
            return capacity__;
        }

        /// <summary>Returns this buffer's position.</summary>
        /// <returns>The position of this buffer</returns>
        public int Position()
        {
            return position__;
        }

        /// <summary>Sets this buffer's position.</summary>
        /// <remarks>
        ///     Sets this buffer's position.  If the mark is defined and larger than the
        ///     new position then it is discarded.
        /// </remarks>
        /// <param name="newPosition">
        ///     The new position value; must be non-negative
        ///     and no larger than the current limit
        /// </param>
        /// <returns>This buffer</returns>
        /// <exception cref="ArgumentException">
        ///     If the preconditions on <tt>newPosition</tt> do not hold
        /// </exception>
        public Buffer Position(int newPosition)
        {
            if (newPosition > limit__ || newPosition < 0) throw new ArgumentException();
            position__ = newPosition;
            if (mark__ > position__) mark__ = -1;
            return this;
        }

        /// <summary>Returns this buffer's limit.</summary>
        /// <returns>The limit of this buffer</returns>
        public int Limit()
        {
            return limit__;
        }

        /// <summary>Sets this buffer's limit.</summary>
        /// <remarks>
        ///     Sets this buffer's limit.  If the position is larger than the new limit
        ///     then it is set to the new limit.  If the mark is defined and larger than
        ///     the new limit then it is discarded.
        /// </remarks>
        /// <param name="newLimit">
        ///     The new limit value; must be non-negative
        ///     and no larger than this buffer's capacity
        /// </param>
        /// <returns>This buffer</returns>
        /// <exception cref="ArgumentException">
        ///     If the preconditions on <tt>newLimit</tt> do not hold
        /// </exception>
        public Buffer Limit(int newLimit)
        {
            if (newLimit > capacity__ || newLimit < 0) throw new ArgumentException();
            limit__ = newLimit;
            if (position__ > limit__) position__ = limit__;
            if (mark__ > limit__) mark__ = -1;
            return this;
        }

        /// <summary>Sets this buffer's mark at its position.</summary>
        /// <returns>This buffer</returns>
        public Buffer Mark()
        {
            mark__ = position__;
            return this;
        }

        /// <summary>Resets this buffer's position to the previously-marked position.</summary>
        /// <remarks>
        ///     Resets this buffer's position to the previously-marked position.
        ///     <p>
        ///         Invoking this method neither changes nor discards the mark's
        ///         value.
        ///     </p>
        /// </remarks>
        /// <returns>This buffer</returns>
        /// <exception cref="InvalidMarkException">If the mark has not been set</exception>
        public Buffer Reset()
        {
            var m = mark__;
            if (m < 0) throw new InvalidMarkException();
            position__ = m;
            return this;
        }

        /// <summary>Clears this buffer.</summary>
        /// <remarks>
        ///     Clears this buffer.  The position is set to zero, the limit is set to
        ///     the capacity, and the mark is discarded.
        ///     <p>
        ///         Invoke this method before using a sequence of channel-read or
        ///         <i>put</i> operations to fill this buffer.  For example:
        ///         <blockquote>
        ///             <pre>
        ///                 buf.clear();     // Prepare buffer for reading
        ///                 in.read(buf);    // Read data
        ///             </pre>
        ///         </blockquote>
        ///         <p>
        ///             This method does not actually erase the data in the buffer, but it
        ///             is named as if it did because it will most often be used in situations
        ///             in which that might as well be the case.
        ///         </p>
        /// </remarks>
        /// <returns>This buffer</returns>
        public Buffer Clear()
        {
            position__ = 0;
            limit__ = capacity__;
            mark__ = -1;
            return this;
        }

        /// <summary>Flips this buffer.</summary>
        /// <remarks>
        ///     Flips this buffer.  The limit is set to the current position and then
        ///     the position is set to zero.  If the mark is defined then it is
        ///     discarded.
        ///     <p>
        ///         After a sequence of channel-read or <i>put</i> operations, invoke
        ///         this method to prepare for a sequence of channel-write or relative
        ///         <i>get</i> operations.  For example:
        ///         <blockquote>
        ///             <pre>
        ///                 buf.put(magic);    // Prepend header
        ///                 in.read(buf);      // Read data into rest of buffer
        ///                 buf.flip();        // Flip buffer
        ///                 out.write(buf);    // Write header + data to channel
        ///             </pre>
        ///         </blockquote>
        ///         <p>
        ///             This method is often used in conjunction with the
        ///             <see cref="ByteBuffer.Compact()">compact</see>
        ///             method when transferring data from
        ///             one place to another.
        ///         </p>
        /// </remarks>
        /// <returns>This buffer</returns>
        public Buffer Flip()
        {
            limit__ = position__;
            position__ = 0;
            mark__ = -1;
            return this;
        }

        /// <summary>Rewinds this buffer.</summary>
        /// <remarks>
        ///     Rewinds this buffer.  The position is set to zero and the mark is
        ///     discarded.
        ///     <p>
        ///         Invoke this method before a sequence of channel-write or <i>get</i>
        ///         operations, assuming that the limit has already been set
        ///         appropriately.  For example:
        ///         <blockquote>
        ///             <pre>
        ///                 out.write(buf);    // Write remaining data
        ///                 buf.rewind();      // Rewind buffer
        ///                 buf.get(array);    // Copy data into array
        ///             </pre>
        ///         </blockquote>
        /// </remarks>
        /// <returns>This buffer</returns>
        public Buffer Rewind()
        {
            position__ = 0;
            mark__ = -1;
            return this;
        }

        /// <summary>
        ///     Returns the number of elements between the current position and the
        ///     limit.
        /// </summary>
        /// <returns>The number of elements remaining in this buffer</returns>
        public int Remaining()
        {
            return limit__ - position__;
        }

        /// <summary>
        ///     Tells whether there are any elements between the current position and
        ///     the limit.
        /// </summary>
        /// <returns>
        ///     <tt>true</tt> if, and only if, there is at least one element
        ///     remaining in this buffer
        /// </returns>
        public bool HasRemaining()
        {
            return position__ < limit__;
        }

        /// <summary>Tells whether or not this buffer is read-only.</summary>
        /// <returns><tt>true</tt> if, and only if, this buffer is read-only</returns>
        public abstract bool IsReadOnly();

        /// <summary>
        ///     Tells whether or not this buffer is backed by an accessible
        ///     array.
        /// </summary>
        /// <remarks>
        ///     Tells whether or not this buffer is backed by an accessible
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
        /// <since>1.6</since>
        public abstract bool HasArray();

        /// <summary>
        ///     Returns the array that backs this
        ///     buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        /// </summary>
        /// <remarks>
        ///     Returns the array that backs this
        ///     buffer&nbsp;&nbsp;<i>(optional operation)</i>.
        ///     <p>
        ///         This method is intended to allow array-backed buffers to be
        ///         passed to native code more efficiently. Concrete subclasses
        ///         provide more strongly-typed return values for this method.
        ///         <p>
        ///             Modifications to this buffer's content will cause the returned
        ///             array's content to be modified, and vice versa.
        ///             <p>
        ///                 Invoke the
        ///                 <see cref="HasArray()">hasArray</see>
        ///                 method before invoking this
        ///                 method in order to ensure that this buffer has an accessible backing
        ///                 array.
        ///             </p>
        /// </remarks>
        /// <returns>The array that backs this buffer</returns>
        /// <exception cref="ReadOnlyBufferException">
        ///     If this buffer is backed by an array but is read-only
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     If this buffer is not backed by an accessible array
        /// </exception>
        /// <since>1.6</since>
        public abstract object Array();

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
        /// <exception cref="NotSupportedException">
        ///     If this buffer is not backed by an accessible array
        /// </exception>
        /// <since>1.6</since>
        public abstract int ArrayOffset();

        /// <summary>
        ///     Tells whether or not this buffer is
        ///     <a href="ByteBuffer.html#direct">
        ///         <i>direct</i>
        ///     </a>
        ///     .
        /// </summary>
        /// <returns><tt>true</tt> if, and only if, this buffer is direct</returns>
        /// <since>1.6</since>
        public abstract bool IsDirect();

        // -- Package-private methods for bounds checking, etc. --
        /// <summary>
        ///     Checks the current position against the limit, throwing a
        ///     <see cref="BufferUnderflowException" />
        ///     if it is not smaller than the limit, and then
        ///     increments the position.
        /// </summary>
        /// <returns>The current position value, before it is incremented</returns>
        internal int NextGetIndex()
        {
            // package-private
            if (position__ >= limit__) throw new BufferUnderflowException();
            return position__++;
        }

        internal int NextGetIndex(int nb)
        {
            // package-private
            if (limit__ - position__ < nb) throw new BufferUnderflowException();
            var p = position__;
            position__ += nb;
            return p;
        }

        /// <summary>
        ///     Checks the current position against the limit, throwing a
        ///     <see cref="BufferOverflowException" />
        ///     if it is not smaller than the limit, and then
        ///     increments the position.
        /// </summary>
        /// <returns>The current position value, before it is incremented</returns>
        internal int NextPutIndex()
        {
            // package-private
            if (position__ >= limit__) throw new BufferOverflowException();
            return position__++;
        }

        internal int NextPutIndex(int nb)
        {
            // package-private
            if (limit__ - position__ < nb) throw new BufferOverflowException();
            var p = position__;
            position__ += nb;
            return p;
        }

        /// <summary>
        ///     Checks the given index against the limit, throwing an
        ///     <see cref="IndexOutOfRangeException" />
        ///     if it is not smaller than the limit
        ///     or is smaller than zero.
        /// </summary>
        internal int CheckIndex(int i)
        {
            // package-private
            if (i < 0 || i >= limit__) throw new IndexOutOfRangeException();
            return i;
        }

        internal int CheckIndex(int i, int nb)
        {
            // package-private
            if (i < 0 || nb > limit__ - i) throw new IndexOutOfRangeException();
            return i;
        }

        internal int MarkValue()
        {
            // package-private
            return mark__;
        }

        internal void Truncate()
        {
            // package-private
            mark__ = -1;
            position__ = 0;
            limit__ = 0;
            capacity__ = 0;
        }

        internal void DiscardMark()
        {
            // package-private
            mark__ = -1;
        }

        internal static void CheckBounds(int off, int len, int size)
        {
            // package-private
            if ((off | len | (off + len) | (size - (off + len))) < 0) throw new IndexOutOfRangeException();
        }
    }

    internal class BufferOverflowException : Exception
    {
    }

    internal class BufferUnderflowException : Exception
    {
    }

    public class InvalidMarkException : Exception
    {
    }
}