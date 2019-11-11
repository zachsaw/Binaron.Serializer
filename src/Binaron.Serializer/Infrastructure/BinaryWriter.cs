using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Infrastructure
{
    public class BinaryWriter : IDisposable
    {
        private const int BufferSize = 1 * 1024 * 1024; // 1MB
        private readonly UnmanagedMemory<byte> buffer = new UnmanagedMemory<byte>(BufferSize);
        private readonly Stream stream;
        private int bufferOffset; 

        public BinaryWriter(Stream stream)
        {
            if (!BitConverter.IsLittleEndian)
                throw new NotSupportedException("Only little endian platforms are supported");

            this.stream = stream;
        }

        ~BinaryWriter()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (bufferOffset < 0)
                return;

            Flush();
            buffer.Dispose();
            bufferOffset = -1;
            GC.SuppressFinalize(this);
        }
        
        public unsafe void Flush()
        {
            stream.Write(new ReadOnlySpan<byte>(buffer.Data, bufferOffset));
            bufferOffset = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Write<T>(T value) where T : unmanaged
        {
            if (bufferOffset + sizeof(T) > buffer.Length)
                Flush();

            *(T*) (buffer.Data + bufferOffset) = value;
            bufferOffset += sizeof(T);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteString(string value)
        {
            var strLength = value.Length;
            Write(strLength);
            
            var length = strLength * sizeof(char);
            if (bufferOffset + length > buffer.Length)
            {
                Flush();

                fixed (char* ptr = value)
                {
                    stream.Write(new ReadOnlySpan<byte>(ptr, length));
                }
            }
            else
            {
                value.AsSpan().CopyTo(new Span<char>(buffer.Data + bufferOffset, length));
                bufferOffset += length;
            }
        }
    }
}