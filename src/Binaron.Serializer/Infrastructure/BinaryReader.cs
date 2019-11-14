using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Binaron.Serializer.Infrastructure
{
    internal class BinaryReader : IDisposable
    {
        private const int BufferSize = 256 * 1024; // 256KB
        private readonly UnmanagedMemory<byte> buffer = new UnmanagedMemory<byte>(BufferSize);
        private readonly Stream stream;
        private int bufferOffset;
        private int bufferLength;

        public BinaryReader(Stream stream)
        {
            if (!BitConverter.IsLittleEndian)
                throw new NotSupportedException("Only little endian platforms are supported");

            this.stream = stream;
        }

        ~BinaryReader()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (bufferOffset < 0)
                return;

            buffer.Dispose();
            bufferOffset = -1;
            GC.SuppressFinalize(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe T Read<T>() where T : unmanaged
        {
            if (bufferOffset + sizeof(T) > bufferLength)
            {
                Fill();
                if (bufferOffset + sizeof(T) > bufferLength)
                    throw new EndOfStreamException();
            }

            var val = *(T*) (buffer.Data + bufferOffset);
            bufferOffset += sizeof(T);
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe string ReadString()
        {
            var len = Read<int>() * sizeof(char);
            if (len == 0)
                return string.Empty;

            if (bufferOffset + len > bufferLength)
            {
                Fill();

                // will it fit in buffer?
                if (len > buffer.Length) // no - fallback to slow version
                    return ReadStringSlow(len);

                // yes
                if (bufferOffset + len > bufferLength)
                    throw new EndOfStreamException();
            }

            var val = new string((char*) (buffer.Data + bufferOffset), 0, len / sizeof(char));
            bufferOffset += len;
            return val;
        }

        private unsafe string ReadStringSlow(int len)
        {
            var strLen = len / sizeof(char);
            var sb = new StringBuilder();
            var remainder = bufferLength / sizeof(char);
            sb.Append(new ReadOnlySpan<char>(buffer.Data, remainder));
            bufferLength = 0;
            strLen -= remainder;

            while (strLen > 0)
            {
                var span = new Span<byte>(buffer.Data, Math.Min(strLen * sizeof(char), buffer.Length));
                if (stream.Read(span) != span.Length)
                    throw new EndOfStreamException();
                var readCharLength = span.Length / sizeof(char);
                strLen -= readCharLength;
                sb.Append(MemoryMarshal.Cast<byte, char>(span));
            }

            return sb.ToString();
        }

        private unsafe void Fill()
        {
            var span = new Span<byte>(buffer.Data, buffer.Length);
            var remainder = bufferLength - bufferOffset;
            span.Slice(bufferOffset, remainder).CopyTo(span);
            bufferLength = stream.Read(span.Slice(remainder)) + remainder;
            bufferOffset = 0;
        }
    }
}