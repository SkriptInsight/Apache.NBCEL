/*
* Copyright (c) 1995, 2008, Oracle and/or its affiliates. All rights reserved.
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
using System.IO;

namespace Apache.NBCEL.Java.IO
{
	/// <summary>
	///     Signals that a malformed string in
	///     <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
	///     format has been read in a data
	///     input stream or by any class that implements the data input
	///     interface.
	/// </summary>
	/// <remarks>
	///     Signals that a malformed string in
	///     <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
	///     format has been read in a data
	///     input stream or by any class that implements the data input
	///     interface.
	///     See the
	///     <a href="DataInput.html#modified-utf-8">
	///         <code>DataInput</code>
	///     </a>
	///     class description for the format in
	///     which modified UTF-8 strings are read and written.
	/// </remarks>
	/// <author>Frank Yellin</author>
	/// <seealso cref="DataInput" />
	/// <seealso cref="DataInputStream.ReadUTF(DataInput)" />
	/// <seealso cref="IOException" />
	/// <since>JDK1.0</since>
	[Serializable]
    public class UTFDataFormatException : IOException
    {
        private const long serialVersionUID = 420743449228280612L;

        /// <summary>
        ///     Constructs a <code>UTFDataFormatException</code> with
        ///     <code>null</code> as its error detail message.
        /// </summary>
        public UTFDataFormatException()
        {
        }

        /// <summary>
        ///     Constructs a <code>UTFDataFormatException</code> with the
        ///     specified detail message.
        /// </summary>
        /// <remarks>
        ///     Constructs a <code>UTFDataFormatException</code> with the
        ///     specified detail message. The string <code>s</code> can be
        ///     retrieved later by the
        ///     <code>
        /// <see cref="Exception.Message" />
        /// </code>
        ///     method of class <code>java.lang.Throwable</code>.
        /// </remarks>
        /// <param name="s">the detail message.</param>
        public UTFDataFormatException(string s)
            : base(s)
        {
        }
    }
}