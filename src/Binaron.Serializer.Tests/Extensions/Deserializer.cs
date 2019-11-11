using System;
using System.IO;

namespace Binaron.Serializer.Tests.Extensions
{
    public static class Converter
    {
        public static object Deserialize(Type destType, Stream stream)
        {
            var type = destType;
            if (type.TryGetEnumerableType(out var enumerableType))
                type = enumerableType;
            var method = new Method(typeof(BinaronConvert), nameof(BinaronConvert.Deserialize), type);
            return method.Func(null, stream);
        }
    }
}