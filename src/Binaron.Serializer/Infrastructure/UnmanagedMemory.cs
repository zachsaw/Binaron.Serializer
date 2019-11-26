using System;
using System.Runtime.InteropServices;

namespace Binaron.Serializer.Infrastructure
{
    internal unsafe class UnmanagedMemory<T> : IDisposable where T : unmanaged
    {
        private void* data;

        public UnmanagedMemory(int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            data = Allocate(length);
            Length = length;
        }

        ~UnmanagedMemory()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (data == null)
                return;

            Free(data);
            data = null;

            GC.SuppressFinalize(this);
        }

        public int Length { get; }

        public T* Data => (T*) data;

        private static void Free(void* mem)
        {
            Marshal.FreeHGlobal(new IntPtr(mem));
        }

        private static void* Allocate(int length)
        {
            var bytes = sizeof(T) * (long) length;
            return Marshal.AllocHGlobal(new IntPtr(bytes)).ToPointer();
        }
    }
}