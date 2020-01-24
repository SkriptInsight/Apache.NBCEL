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

namespace ObjectWeb.Misc.Java.Nio
{
	/// <summary>A typesafe enumeration for byte orders.</summary>
	/// <author>Mark Reinhold</author>
	/// <author>JSR-51 Expert Group</author>
	/// <since>1.4</since>
	public sealed class ByteOrder
    {
	    /// <summary>Constant denoting big-endian byte order.</summary>
	    /// <remarks>
	    ///     Constant denoting big-endian byte order.  In this order, the bytes of a
	    ///     multibyte value are ordered from most significant to least significant.
	    /// </remarks>
	    public static readonly ByteOrder Big_Endian = new ByteOrder("BIG_ENDIAN");

	    /// <summary>Constant denoting little-endian byte order.</summary>
	    /// <remarks>
	    ///     Constant denoting little-endian byte order.  In this order, the bytes of
	    ///     a multibyte value are ordered from least significant to most
	    ///     significant.
	    /// </remarks>
	    public static readonly ByteOrder Little_Endian = new ByteOrder("LITTLE_ENDIAN");

        private readonly string name;

        private ByteOrder(string name)
        {
            this.name = name;
        }

        /// <summary>Retrieves the native byte order of the underlying platform.</summary>
        /// <remarks>
        ///     Retrieves the native byte order of the underlying platform.
        ///     <p>
        ///         This method is defined so that performance-sensitive Java code can
        ///         allocate direct buffers with the same byte order as the hardware.
        ///         Native code libraries are often more efficient when such buffers are
        ///         used.
        ///     </p>
        /// </remarks>
        /// <returns>
        ///     The native byte order of the hardware upon which this Java
        ///     virtual machine is running
        /// </returns>
        public static ByteOrder NativeOrder()
        {
            return BitConverter.IsLittleEndian ? Little_Endian : Big_Endian;
        }

        /// <summary>Constructs a string describing this object.</summary>
        /// <remarks>
        ///     Constructs a string describing this object.
        ///     <p>
        ///         This method returns the string <tt>"BIG_ENDIAN"</tt> for
        ///         <see cref="Big_Endian" />
        ///         and <tt>"LITTLE_ENDIAN"</tt> for
        ///         <see cref="Little_Endian" />
        ///         .
        ///     </p>
        /// </remarks>
        /// <returns>The specified string</returns>
        public override string ToString()
        {
            return name;
        }
    }
}