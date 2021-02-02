using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Infrastructure
{
    internal static class IEnumerableExtension
    {
        private static ConcurrentDictionary<Type, Action<object, object>> gAdders = new ConcurrentDictionary<Type, Action<object, object>>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add<T>(this IEnumerable<T> enumerable, T value)
        {
            if (!gAdders.TryGetValue(enumerable.GetType(), out Action<object, object> action))
            {
                    var method = enumerable.GetType().GetMethod("Add", new Type[] { typeof(T) });
                    if (method == null)
                        action = (o, v) => { };
                    else
                        action = (o, v) => method.Invoke(o, new object[] { v });
                gAdders.TryAdd(enumerable.GetType(), action);
            }
            action(enumerable, value);
        }
    }
}