using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        public async Task Flush()
        {
            await stream.WriteAsync(buffer.Memory.Slice(bufferOffset));
            bufferOffset = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task Write<T>(T value) where T : unmanaged
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
        public async Task WriteString(string value)
        {
            var strLength = value.Length;
            await Write(strLength);
            
            var length = strLength * sizeof(char);
            if (bufferOffset + length > buffer.Length)
            {
                await Flush();
                await stream.WriteAsync(value.AsMemory().Cast<char, byte>());
            }
            else
            {
                CopyToBuffer(value, length);
                bufferOffset += length;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void CopyToBuffer(string value, int length) => value.AsSpan().CopyTo(new Span<char>(buffer.Data + bufferOffset, length));
    }

    internal static class MemoryExtensions
    {
        public static Memory<TTo> Cast<TFrom, TTo>(this ReadOnlyMemory<TFrom> from)
            where TFrom : unmanaged
            where TTo : unmanaged
        {
            // avoid the extra allocation/indirection, at the cost of a gen-0 box
            if (typeof(TFrom) == typeof(TTo)) return (Memory<TTo>)(object)from;

            return new CastMemoryManager<TFrom, TTo>(from).Memory;
        }

        private sealed class CastMemoryManager<TFrom, TTo> : MemoryManager<TTo>
            where TFrom : unmanaged
            where TTo : unmanaged
        {
            private readonly ReadOnlyMemory<TFrom> _from;

            public CastMemoryManager(ReadOnlyMemory<TFrom> from) => _from = from;

            public override Span<TTo> GetSpan() => MemoryMarshal.Cast<TFrom, TTo>(_from.Span);

            protected override void Dispose(bool disposing)
            {
            }

            public override MemoryHandle Pin(int elementIndex = 0) => throw new NotSupportedException();
            public override void Unpin() => throw new NotSupportedException();
        }
    }
}