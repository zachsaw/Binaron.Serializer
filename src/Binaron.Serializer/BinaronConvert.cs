using System;
using System.IO;
using Binaron.Serializer.Infrastructure;

namespace Binaron.Serializer
{
    public static class BinaronConvert
    {
        public static object Deserialize(Stream stream)
        {
            using var reader = new ReaderState(stream, new DeserializerOptions());
            return Deserializer.ReadValue(reader);
        }

        public static T Deserialize<T>(Stream stream)
        {
            using var reader = new ReaderState(stream, new DeserializerOptions());
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

        public static T Deserialize<T>(Stream stream, DeserializerOptions options)
        {
            using var reader = new ReaderState(stream, options);
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

        public static void Populate<T>(T obj, Stream stream)
        {
            using var reader = new ReaderState(stream, new DeserializerOptions());
            TypedDeserializer.Populate(obj, reader);
        }

        public static void Populate<T>(T obj, Stream stream, DeserializerOptions options)
        {
            using var reader = new ReaderState(stream, options);
            TypedDeserializer.Populate(obj, reader);
        }

        public static void Serialize(object obj, Stream stream)
        {
            using var writer = new WriterState(stream, new SerializerOptions());
            Serializer.WriteValue(writer, obj);
        }
        
        public static void Serialize(object obj, Stream stream, SerializerOptions options)
        {
            using var writer = new WriterState(stream, options);
            Serializer.WriteValue(writer, obj);
        }

        public static void Serialize<T>(T obj, Stream stream)
        {
            using var writer = new WriterState(stream, new SerializerOptions());
            Serializer.WriteValue(writer, obj);
        }
        
        public static void Serialize<T>(T obj, Stream stream, SerializerOptions options)
        {
            using var writer = new WriterState(stream, options);
            Serializer.WriteValue(writer, obj);
        }
    }
}