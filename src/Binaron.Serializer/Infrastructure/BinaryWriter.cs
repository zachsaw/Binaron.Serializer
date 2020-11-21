using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Infrastructure
{
    internal class BinaryWriter : IDisposable
    {
        private const int BufferSize = 64 * 1024; // 64KB
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (bufferOffset < 0)
                return;

            Flush();
            buffer.Dispose();
            bufferOffset = -1;
            GC.SuppressFinalize(this);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Flush()
        {
            stream.Write(new ReadOnlySpan<byte>(buffer.Data, bufferOffset));
            bufferOffset = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Span<byte> Reserve(int length)
        {
            if (bufferOffset + length > buffer.Length)
                Flush();

            var result = buffer.Data + bufferOffset;
            bufferOffset += length;
            return new Span<byte>(result, length);
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

                if (length > buffer.Length)
                {
                    fixed (char* ptr = value)
                    {
                        stream.Write(new ReadOnlySpan<byte>(ptr, length));
                    }
                    return;
                }
            }

            value.AsSpan().CopyTo(new Span<char>(buffer.Data + bufferOffset, strLength));
            bufferOffset += length;
        }
    }
}