using System;
using System.Collections;
using System.Collections.Generic;

namespace Binaron.Serializer.Tests.Extensions
{
    public static class TypeExtensions
    {
        private const string EnumerableClassSignature = ">d__";

        public static bool TryGetEnumerableType(this Type type, out Type result)
        {
            if (type.Name.Contains(EnumerableClassSignature))
            {
                result = GetEnumerableType(type);
                return true;
            }

            result = null;
            return false;
        }

        public static Type GetEnumerableType(this Type type)
        {
            foreach (var t in type.GetInterfaces())
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    return t;
            }

            return typeof(IEnumerable);
        }
    }
}