using System;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Infrastructure
{
    internal static class ListCapacity
    {
        private const int MaxCapacityHint = 64 * 1024; // 64K items

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int count) => Math.Min(MaxCapacityHint, count);
    }
}