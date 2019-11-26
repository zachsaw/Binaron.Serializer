using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Binaron.Serializer.Infrastructure
{
    internal sealed unsafe class StringMemoryManager : MemoryManager<byte>
    {
        private GCHandle gcHandle;

        public StringMemoryManager(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            gcHandle = GCHandle.Alloc(value);
            Data = (byte*) GCHandle.ToIntPtr(gcHandle).ToPointer();
            Length = value.Length * sizeof(char);
            Memory = base.Memory;
        }

        public byte* Data { get; private set; }
        public new Memory<byte> Memory { get; }
        public int Length { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Span<byte> GetSpan() => new Span<byte>(Data, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override MemoryHandle Pin(int elementIndex = 0)
        {
            if (elementIndex < 0 || elementIndex >= Length)
                throw new ArgumentOutOfRangeException(nameof(elementIndex));

            return new MemoryHandle(Data + elementIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpin() { }

        protected override void Dispose(bool disposing)
        {
            if (Data == null)
                return;

            ref var handle = ref gcHandle;
            handle.Free();
            Data = null;
        }
    }

    internal sealed unsafe class UnmanagedMemoryManager<T> : MemoryManager<T> where T : unmanaged
    {
        public UnmanagedMemoryManager(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            Data = Allocate(length);
            Length = length;
            Memory = base.Memory;
        }

        public T* Data { get; private set; }
        public new Memory<T> Memory { get; }
        public int Length { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override Span<T> GetSpan() => new Span<T>(Data, Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override MemoryHandle Pin(int elementIndex = 0)
        {
            if (elementIndex < 0 || elementIndex >= Length)
                throw new ArgumentOutOfRangeException(nameof(elementIndex));

            return new MemoryHandle(Data + elementIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Unpin() { }

        protected override void Dispose(bool disposing)
        {
            if (Data == null)
                return;

            Free(Data);
            Data = null;
        }

        private static void Free(void* mem)
        {
            Marshal.FreeHGlobal(new IntPtr(mem));
        }

        private static T* Allocate(int length)
        {
            var bytes = sizeof(T) * (long) length;
            return (T*) Marshal.AllocHGlobal(new IntPtr(bytes)).ToPointer();
        }
    }
}