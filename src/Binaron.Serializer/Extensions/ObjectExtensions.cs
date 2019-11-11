using System.Collections.Generic;

namespace Binaron.Serializer.Extensions
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> Yield<T>(this T single)
        {
            yield return single;
        }
    }
}