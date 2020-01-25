using System.IO;
using Apache.NBCEL.Java.IO;

namespace Apache.NBCEL.Java.Nio
{
    public static partial class MemoryStreamExtensions
    {
        public static InputStream ToInputStream(this MemoryStream stream)
        {
            return new MemoryInputStream(stream);
        }
    }

    public class MemoryDataInputStream : DataInputStream
    {
        public MemoryDataInputStream(byte[] bytes) : base(bytes.ToInputStream())
        {
        }
    }

    public class MemoryInputStream : InputStream
    {
        public MemoryInputStream(MemoryStream stream)
        {
            Stream = stream;
        }

        public MemoryInputStream(byte[] bytes)
        {
            Stream = new MemoryStream(bytes);
        }


        public MemoryStream Stream { get; }

        public static implicit operator MemoryInputStream(MemoryStream stream)
        {
            return new MemoryInputStream(stream);
        }

        public override void Close()
        {
            Stream.Dispose();
            base.Close();
        }

        public override int Read()
        {
            return Stream.ReadByte();
        }
    }
}