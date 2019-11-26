using System;
using System.IO;
using System.Threading.Tasks;
using Binaron.Serializer.Infrastructure;
using BinaryReader = Binaron.Serializer.Infrastructure.BinaryReader;

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

        public static async ValueTask Serialize(object obj, Stream stream)
        {
            await using var writer = new WriterState(stream, new SerializerOptions());
            await Serializer.WriteValue(writer, obj);
        }
        
        public static async ValueTask Serialize(object obj, Stream stream, SerializerOptions options)
        {
            await using var writer = new WriterState(stream, options);
            await Serializer.WriteValue(writer, obj);
        }

        public static async ValueTask Serialize<T>(T obj, Stream stream)
        {
            await using var writer = new WriterState(stream, new SerializerOptions());
            await Serializer.WriteValue(writer, obj);
        }
        
        public static async ValueTask Serialize<T>(T obj, Stream stream, SerializerOptions options)
        {
            await using var writer = new WriterState(stream, options);
            await Serializer.WriteValue(writer, obj);
        }
    }
}