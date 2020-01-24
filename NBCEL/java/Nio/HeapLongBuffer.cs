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

namespace ObjectWeb.Misc.Java.Nio
{
    /// <summary>A read/write HeapLongBuffer.</summary>
    internal class HeapLongBuffer : LongBuffer
    {
        internal HeapLongBuffer(int cap, int lim)
            : base(-1, 0, lim, cap, new long[cap], 0)
        {
        }

        internal HeapLongBuffer(long[] buf, int off, int len)
            : base(-1, off, off + len, buf.Length, buf, 0)
        {
        }

        protected internal HeapLongBuffer(long[] buf, int mark, int pos, int lim, int cap
            , int off)
            : base(mark, pos, lim, cap, buf, off)
        {
        }

        // For speed these fields are actually declared in X-Buffer;
        // these declarations are here as documentation
        /*
        
        protected final long[] hb;
        protected final int offset;
        
        */
        // package-private
        /*
        hb = new long[cap];
        offset = 0;
        */
        // package-private
        /*
        hb = buf;
        offset = 0;
        */
        /*
        hb = buf;
        offset = off;
        */
        public override LongBuffer Slice()
        {
            return new HeapLongBuffer(hb, -1, 0, Remaining(), Remaining(), Position
                                                                               () + offset);
        }

        public override LongBuffer Duplicate()
        {
            return new HeapLongBuffer(hb, MarkValue(), Position(), Limit(), Capacity(), offset);
        }

        public override LongBuffer AsReadOnlyBuffer()
        {
            return new HeapLongBufferR(hb, MarkValue(), Position(), Limit(), Capacity(), offset);
        }

        protected internal virtual int Ix(int i)
        {
            return i + offset;
        }

        public override long Get()
        {
            return hb[Ix(NextGetIndex())];
        }

        public override long Get(int i)
        {
            return hb[Ix(CheckIndex(i))];
        }

        public override LongBuffer Get(long[] dst, int offset, int length)
        {
            CheckBounds(offset, length, dst.Length);
            if (length > Remaining()) throw new BufferUnderflowException();
            global::System.Array.Copy(hb, Ix(Position()), dst, offset, length);
            Position(Position() + length);
            return this;
        }

        public override bool IsDirect()
        {
            return false;
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public override LongBuffer Put(long x)
        {
            hb[Ix(NextPutIndex())] = x;
            return this;
        }

        public override LongBuffer Put(int i, long x)
        {
            hb[Ix(CheckIndex(i))] = x;
            return this;
        }

        public override LongBuffer Put(long[] src, int offset, int length)
        {
            CheckBounds(offset, length, src.Length);
            if (length > Remaining()) throw new BufferOverflowException();
            global::System.Array.Copy(src, offset, hb, Ix(Position()), length);
            Position(Position() + length);
            return this;
        }

        public override LongBuffer Put(LongBuffer src)
        {
            if (src is HeapLongBuffer)
            {
                if (src == this) throw new ArgumentException();
                var sb = (HeapLongBuffer) src;
                var n = sb.Remaining();
                if (n > Remaining()) throw new BufferOverflowException();
                global::System.Array.Copy(sb.hb, sb.Ix(sb.Position()), hb, Ix(Position()), n);
                sb.Position(sb.Position() + n);
                Position(Position() + n);
            }
            else if (src.IsDirect())
            {
                var n = src.Remaining();
                if (n > Remaining()) throw new BufferOverflowException();
                src.Get(hb, Ix(Position()), n);
                Position(Position() + n);
            }
            else
            {
                base.Put(src);
            }

            return this;
        }

        public override LongBuffer Compact()
        {
            global::System.Array.Copy(hb, Ix(Position()), hb, Ix(0), Remaining());
            Position(Remaining());
            Limit(Capacity());
            DiscardMark();
            return this;
        }

        public override ByteOrder Order()
        {
            return ByteOrder.NativeOrder();
        }
    }
}