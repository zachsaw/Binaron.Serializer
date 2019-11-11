using System;
using System.IO;
using BinaryReader = Binaron.Serializer.Infrastructure.BinaryReader;
using BinaryWriter = Binaron.Serializer.Infrastructure.BinaryWriter;

namespace Binaron.Serializer
{
    public static class BinaronConvert
    {
        public static object Deserialize(Stream stream)
        {
            using var reader = new BinaryReader(stream);
            return Deserializer.ReadValue(reader);
        }

        public static T Deserialize<T>(Stream stream)
        {
            using var reader = new BinaryReader(stream);
            var result = TypedDeserializer.ReadValue<T>(reader);
            try
            {
                return (T) result;
            }
            catch (NullReferenceException) // faster than checking if result is null since most results won't be null
            {
                return default;
            }
        }

        public static void Serialize(object obj, Stream stream)
        {
            using var writer = new BinaryWriter(stream);
            Serializer.WriteValue(writer, obj);
        }
    }
}