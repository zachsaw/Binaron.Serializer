using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Binaron.Serializer.Tests.Extensions
{
    public static class Extensions
    {
        public static dynamic DynamicCast(this object v, Type type)
        {
            try
            {
                return Caster.DynamicCast(v, type);
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }
        
        public static object GetDefault(this Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;

        public static int Count(this IEnumerable enumerable) => Enumerable.Count(enumerable.Cast<object>());

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, T item)
        {
            foreach (var e in enumerable)
                yield return e;

            yield return item;
        }

        public static IEnumerable Concat(this IEnumerable enumerable, object item)
        {
            foreach (var e in enumerable)
                yield return e;

            yield return item;
        }

        public static object[] ToArray(this IEnumerable enumerable)
        {
            var list = new ArrayList();
            foreach (var e in enumerable)
                list.Add(e);

            return list.ToArray();
        }
    }
}