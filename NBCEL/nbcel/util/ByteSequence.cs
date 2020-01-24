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

using java.io;
using ObjectWeb.Misc.Java.Nio;
using Sharpen;

namespace NBCEL.util
{
	/// <summary>
	///     Utility class that implements a sequence of bytes which can be read
	///     via the `readByte()' method.
	/// </summary>
	/// <remarks>
	///     Utility class that implements a sequence of bytes which can be read
	///     via the `readByte()' method. This is used to implement a wrapper for the
	///     Java byte code stream to gain some more readability.
	/// </remarks>
	public sealed class ByteSequence : DataInputStream
    {
        private readonly ByteArrayStream byteStream;

        public ByteSequence(byte[] bytes)
            : base(bytes.ToInputStream())
        {
            byteStream = (ByteArrayStream) @in;
        }

        public int GetIndex()
        {
            return (int) byteStream.GetPosition();
        }

        internal void UnreadByte()
        {
            byteStream.UnreadByte();
        }

        private sealed class ByteArrayStream : MemoryInputStream
        {
            internal ByteArrayStream(byte[] bytes)
                : base(bytes)
            {
            }

            internal long GetPosition()
            {
                // pos is protected in ByteArrayInputStream
                return Stream.Position;
            }

            internal void UnreadByte()
            {
                if (Stream.Position > 0) Stream.Position--;
            }
        }
    }
}