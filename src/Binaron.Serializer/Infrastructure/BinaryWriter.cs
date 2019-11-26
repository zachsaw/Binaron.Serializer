using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Binaron.Serializer.Infrastructure
{
    internal sealed class BinaryWriter : IDisposable
    {
        private const int BufferSize = 64 * 1024; // 64KB
        private readonly UnmanagedMemoryManager<byte> buffer = new UnmanagedMemoryManager<byte>(BufferSize);
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

            IDisposable disposable = buffer;
            disposable.Dispose();
            bufferOffset = -1;
            GC.SuppressFinalize(this);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask Flush()
        {
            await stream.WriteAsync(buffer.Memory.Slice(0, bufferOffset));
            bufferOffset = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask Write<T>(T value) where T : unmanaged
        {
            if (bufferOffset + SizeOf<T>() > buffer.Length)
                await Flush();

            Write(value, bufferOffset);
            bufferOffset += SizeOf<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe int SizeOf<T>() where T : unmanaged => sizeof(T);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void Write<T>(T value, int offset) where T : unmanaged => *(T*) (buffer.Data + offset) = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask WriteString(string value)
        {
            var strLength = value.Length;
            await Write(strLength);
            
            var length = strLength * sizeof(char);
            if (bufferOffset + length > buffer.Length)
            {
                await Flush();
                if (length > buffer.Length)
                {
                    using var memoryMgr = new StringMemoryManager(value);
                    await stream.WriteAsync(memoryMgr.Memory);
                    return;
                }
            }

            CopyToBuffer(value, length);
            bufferOffset += length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void CopyToBuffer(string value, int length) => value.AsSpan().CopyTo(new Span<char>(buffer.Data + bufferOffset, length));
    }
}