using System;
using System.IO;

namespace java.io
{
    class CharArrayWriter
    {
        protected char[] buffer;
        protected int count;

        public CharArrayWriter(char[] buffer)
        {
            this.buffer = buffer;
        }

        public CharArrayWriter(TextReader reader, int length)
        {
            buffer = new char[length];

            for (int left = length; left > 0;)
            {
                int read = reader.Read(buffer, count, left);

                if (read == -1)
                {
                    if (left > 0)
                    {
                        reader.Close();

                        throw new EndOfStreamException();
                    }

                    break;
                }

                left -= read;
                count += read;
            }
        }

        public void Write(int c)
        {
            if (count == buffer.Length)
            {
                EnsureSize(count + 1);
            }

            buffer[count++] = (char) c;
        }

        void EnsureSize(int size)
        {
            if (size <= buffer.Length)
            {
                return;
            }

            int newSize = buffer.Length;

            while (newSize < size)
            {
                newSize *= 2;
            }

            char[] newBuffer = new char[newSize];

            Array.Copy(buffer, 0, newBuffer, 0, count);

            buffer = newBuffer;
        }

        public void Write(String str, int off, int len)
        {
            EnsureSize(count + len);
            str.CopyTo(off, buffer, count, len);

            count += len;
        }

        public void Reset()
        {
            count = 0;
        }

        public void Reset(char[] buffer)
        {
            count = 0;
            this.buffer = buffer;
        }

        public char[] ToCharArray()
        {
            char[] newBuffer = new char[count];

            Array.Copy(buffer, 0, newBuffer, 0, count);

            return (char[]) newBuffer;
        }

        public int Size()
        {
            return count;
        }

        /**
         * Converts input data to a string.
         * @return the string.
         */
        public override string ToString()
        {
            return new string(buffer, 0, count);
        }
    }
}