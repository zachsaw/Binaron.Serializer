using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Extensions
{
    internal static class EnumerableExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(object, object)> Zip(this IEnumerable first, IEnumerable second)
        {
            var firstEnumerator = first.GetEnumerator();
            var secondEnumerator = second.GetEnumerator();
            
            while (firstEnumerator.MoveNext() && secondEnumerator.MoveNext())
                yield return (firstEnumerator.Current, secondEnumerator.Current);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<(T1, T2)> Zip<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second)
        {
            using var firstEnumerator = first.GetEnumerator();
            using var secondEnumerator = second.GetEnumerator();
            
            while (firstEnumerator.MoveNext() && secondEnumerator.MoveNext())
                yield return (firstEnumerator.Current, secondEnumerator.Current);
        }
    }
}