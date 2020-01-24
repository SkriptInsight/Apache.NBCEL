using System.IO;
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.Java.Nio
{
    public static partial class MemoryStreamExtensions
    {
        public static OutputStream ToOutputStream(this MemoryStream stream)
        {
            return new MemoryOutputStream(stream);
        }
    }

    public class MemoryOutputStream : OutputStream
    {
        public MemoryOutputStream(MemoryStream stream)
        {
            Stream = stream;
        }


        public MemoryStream Stream { get; }

        public static implicit operator MemoryOutputStream(MemoryStream stream)
        {
            return new MemoryOutputStream(stream);
        }

        public override void Write(int b)
        {
            Stream.WriteByte((byte) b);
        }
    }
}