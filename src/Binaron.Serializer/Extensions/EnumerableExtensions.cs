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
    }
}