using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Binaron.Serializer.Infrastructure
{
    internal unsafe class UnmanagedMemory<T> : IDisposable where T : unmanaged
    {
        private void* memory;
        private void* data;

        public UnmanagedMemory(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            memory = Allocate(length);
            SetLength(memory, length);
            data = (int*) memory + 1;
        }

        ~UnmanagedMemory()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (memory == null)
                return;

            Free(memory);
            memory = null;
            data = null;

            GC.SuppressFinalize(this);
        }

        public int Length => GetLength(memory);
        public T* Data => (T*) data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLength(void* mem) => *(int*) mem;
        private static void SetLength(void* mem, int len) => *(int*) mem = len;

        private static void Free(void* mem)
        {
            Marshal.FreeHGlobal(new IntPtr(mem));
        }

        private static void* Allocate(int length)
        {
            var bytes = sizeof(int) + sizeof(T) * (long) length;
            return Marshal.AllocHGlobal(new IntPtr(bytes)).ToPointer();
        }
    }
}