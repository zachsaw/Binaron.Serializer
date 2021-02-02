using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Accessors;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;
using TypeCode = Binaron.Serializer.Enums.TypeCode;

namespace Binaron.Serializer
{
    internal static class Serializer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteValue<T>(WriterState writer, T val)
        {
            if (val == null)
            {
                writer.Write((byte) SerializedType.Null);
                return;
            }

            WriteNonNullValue(writer, val);
        }

        public static void WriteValue(WriterState writer, object val)
        {
            if (val == null)
            {
                writer.Write((byte) SerializedType.Null);
                return;
            }

            WriteNonNullValue(writer, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteNonNullValue<T>(WriterState writer, T val)
        {
            if (WritePrimitive(writer, val))
                return;

            WriteNonPrimitive(writer, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNonNullValue(WriterState writer, object val)
        {
            if (WritePrimitive(writer, val))
                return;

            WriteNonPrimitive(writer, val);
        }

        private interface INonPrimitiveWriter<in T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Write(WriterState writer, T val);
        }

        private static class GetNonPrimitiveWriterGeneric<T>
        {
            public static readonly INonPrimitiveWriter<T> Writer = NonPrimitiveWriters.CreateWriter<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNonPrimitive<T>(WriterState writer, T val) => GetNonPrimitiveWriterGeneric<T>.Writer.Write(writer, val);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteNonPrimitive(WriterState writer, object val)
        {
            switch (val)
            {
                case IDictionary<string, object> dictAsObj: // treat IDictionary<string, object> as Object type
                    WriteDictionaryAsObject(writer, dictAsObj);
                    break;
                case IDictionary _ when GenericWriter.WriteDictionary(writer, val):
                    break;
                case IDictionary dictionary:
                    WriteDictionary(writer, dictionary);
                    break;
                case ICollection _ when GenericWriter.WriteDictionary(writer, val):
                    break;
                case ICollection list:
                    WriteList(writer, list);
                    break;
                case IReadOnlyDictionary<string, object> roDictAsObj: // treat IReadOnlyDictionary<string, object> as Object type
                    WriteReadOnlyDictionaryAsObject(writer, roDictAsObj);
                    break;
                case IEnumerable _ when GenericWriter.WriteReadOnlyDictionary(writer, val):
                    break;
                case IEnumerable enumerable:
                    WriteEnumerable(writer, enumerable);
                    break;
                default:
                    WriteObject(writer, val);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteObject<T>(WriterState writer, T val)
        {
            if (val == null)
            {
                writer.Write((byte)SerializedType.Null);
            }
            else
            {
                var type = val.GetType();
                if (writer.CustomObjectIdentifierProviders == null || !writer.CustomObjectIdentifierProviders.TryGetValue(typeof(T), out var customObjectIdentifierProvider))
                {
                    writer.Write((byte)SerializedType.Object);
                }
                else
                {
                    writer.Write((byte)SerializedType.CustomObject);
                    WriteValue(writer, customObjectIdentifierProvider.GetIdentifier(type));
                }


                var getters = type == typeof(T) ? GetterHandler.GetterHandlers<T>.Getters : GetterHandler.GetGetterHandlers(type);
                foreach (var getter in getters)
                    getter.Handle(writer, val);
                writer.Write((byte)EnumerableType.End);
            }
            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteObject(WriterState writer, object val)
        {
            writer.Write((byte) SerializedType.Object);

            foreach (var getter in GetterHandler.GetGetterHandlers(val.GetType()))
                getter.Handle(writer, val);

            writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteDictionaryAsObject(WriterState writer, IDictionary<string, object> dictionary)
        {
            writer.Write((byte) SerializedType.Object);

            foreach (var (key, value) in dictionary.Keys.Zip(dictionary.Values))
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(key);
                WriteValue(writer, value);
            }

            writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteReadOnlyDictionaryAsObject(WriterState writer, IReadOnlyDictionary<string, object> dictionary)
        {
            writer.Write((byte) SerializedType.Object);

            foreach (var (key, value) in dictionary.Keys.Zip(dictionary.Values))
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(key);
                WriteValue(writer, value);
            }

            writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteDictionary(WriterState writer, IDictionary dictionary)
        {
            writer.Write((byte) SerializedType.Dictionary);
            writer.Write(dictionary.Count);
            foreach (var (key, value) in dictionary.Keys.Zip(dictionary.Values))
            {
                WriteValue(writer, key);
                WriteValue(writer, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteList<T>(WriterState writer, T list) where T : ICollection
        {
            if (GenericWriter.WriteList(writer, list))
                return;

            // fallback to non-generic version
            WriteListFallback(writer, list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteList(WriterState writer, ICollection list)
        {
            if (GenericWriter.WriteList(writer, list))
                return;

            // fallback to non-generic version
            WriteListFallback(writer, list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteListFallback(WriterState writer, ICollection list)
        {
            writer.Write((byte) SerializedType.List);
            writer.Write(list.Count);
            foreach (var item in list)
                WriteValue(writer, item);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteEnumerable<T>(WriterState writer, T enumerable) where T : IEnumerable
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (GenericWriter.WriteEnumerable(writer, enumerable))
                return;

            // fallback to non-generic version (we are not enumerating twice)
            // ReSharper disable once PossibleMultipleEnumeration
            writer.Write((byte) SerializedType.Enumerable);
            WriteEnumerableFallback(writer, enumerable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteEnumerable(WriterState writer, IEnumerable enumerable)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (GenericWriter.WriteEnumerable(writer, enumerable))
                return;

            // fallback to non-generic version (we are not enumerating twice)
            // ReSharper disable once PossibleMultipleEnumeration
            writer.Write((byte) SerializedType.Enumerable);
            WriteEnumerableFallback(writer, enumerable);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteEnumerableFallback(WriterState writer, IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                writer.Write((byte) EnumerableType.HasItem);
                WriteValue(writer, item);
            }

            writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool WritePrimitive(WriterState writer, object value)
        {
            var type = value.GetType();
            var type1 = Nullable.GetUnderlyingType(type);
            if (type1 != null && type1 != type)
                type = type1;

            return WritePrimitive(writer, type.GetTypeCode(), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool WritePrimitive<T>(WriterState writer, T value)
        {
            var type = typeof(T);
            var type1 = Nullable.GetUnderlyingType(type);
            if (type1 != null && type1 != type)
                type = type1;

            return WritePrimitive(writer, type.GetTypeCode(), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool WritePrimitive<T>(WriterState writer, TypeCode typeCode, T value)
        {
            switch (typeCode)
            {
                case TypeCode.String:
                    Writer.Write(writer, (string) (object) value);
                    return true;
                case TypeCode.UInt32:
                    Writer.Write(writer, (uint) (object) value);
                    return true;
                case TypeCode.Int32:
                    Writer.Write(writer, (int) (object) value);
                    return true;
                case TypeCode.Byte:
                    Writer.Write(writer, (byte) (object) value);
                    return true;
                case TypeCode.SByte:
                    Writer.Write(writer, (sbyte) (object) value);
                    return true;
                case TypeCode.UInt16:
                    Writer.Write(writer, (ushort) (object) value);
                    return true;
                case TypeCode.Int16:
                    Writer.Write(writer, (short) (object) value);
                    return true;
                case TypeCode.Int64:
                    Writer.Write(writer, (long) (object) value);
                    return true;
                case TypeCode.UInt64:
                    Writer.Write(writer, (ulong) (object) value);
                    return true;
                case TypeCode.Single:
                    Writer.Write(writer, (float) (object) value);
                    return true;
                case TypeCode.Double:
                    Writer.Write(writer, (double) (object) value);
                    return true;
                case TypeCode.Decimal:
                    Writer.Write(writer, (decimal) (object) value);
                    return true;
                case TypeCode.Boolean:
                    Writer.Write(writer, (bool) (object) value);
                    return true;
                case TypeCode.DateTime:
                    Writer.Write(writer, (DateTime) (object) value);
                    return true;
                case TypeCode.Guid:
                    Writer.Write(writer, (Guid) (object) value);
                    return true;
                case TypeCode.Char:
                    Writer.Write(writer, (char) (object) value);
                    return true;
                default:
                    return false;
            }
        }
        
        private static class NonPrimitiveWriters
        {
            public static INonPrimitiveWriter<T> CreateWriter<T>()
            {
                if (TryCreateIDictionaryWriter(out INonPrimitiveWriter<T> writer1))
                    return writer1;

                if (TryCreateIReadOnlyDictionaryWriter(out INonPrimitiveWriter<T> writer2))
                    return writer2;

                if (typeof(IDictionary).IsAssignableFrom(typeof(T)))
                    return (INonPrimitiveWriter<T>) Activator.CreateInstance(typeof(DictionaryWriter<>).MakeGenericType(typeof(T)));

                if (typeof(ICollection).IsAssignableFrom(typeof(T)))
                    return (INonPrimitiveWriter<T>) Activator.CreateInstance(typeof(ListWriter<>).MakeGenericType(typeof(T)));

                if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                    return (INonPrimitiveWriter<T>) Activator.CreateInstance(typeof(EnumerableWriter<>).MakeGenericType(typeof(T)));

                return new ObjectWriter<T>();
            }

            private static bool TryCreateIDictionaryWriter<T>(out INonPrimitiveWriter<T> writer)
            {
                var (keyType, valueType) = GenericType.GetIDictionaryWriterGenericTypes<T>.Types;
                if (keyType != null)
                {
                    if (keyType == typeof(string) && valueType == typeof(object))
                        writer = new DictionaryAsObjectWriter<T>();
                    else
                        writer = new GenericDictionaryWriter<T>();
                    
                    return true;
                }
                
                writer = null;
                return false;
            }

            private static bool TryCreateIReadOnlyDictionaryWriter<T>(out INonPrimitiveWriter<T> writer)
            {
                var (keyType, valueType) = GenericType.GetIReadOnlyDictionaryWriterGenericTypes<T>.Types;
                if (keyType != null)
                {
                    if (keyType == typeof(string) && valueType == typeof(object))
                        writer = new ReadOnlyDictionaryAsObjectWriter<T>();
                    else
                        writer = new GenericReadOnlyDictionaryWriter<T>();
                    
                    return true;
                }
                
                writer = null;
                return false;
            }

            private class DictionaryAsObjectWriter<T> : INonPrimitiveWriter<T>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => WriteDictionaryAsObject(writer, (IDictionary<string, object>) val);
            }

            private class ReadOnlyDictionaryAsObjectWriter<T> : INonPrimitiveWriter<T>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => WriteReadOnlyDictionaryAsObject(writer, (IReadOnlyDictionary<string, object>) val);
            }

            private class GenericDictionaryWriter<T> : INonPrimitiveWriter<T>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => GenericWriter.WriteDictionary(writer, val);
            }

            private class GenericReadOnlyDictionaryWriter<T> : INonPrimitiveWriter<T>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => GenericWriter.WriteReadOnlyDictionary(writer, val);
            }

            private class DictionaryWriter<T> : INonPrimitiveWriter<T> where T : IDictionary
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => WriteDictionary(writer, val);
            }

            private class ListWriter<T> : INonPrimitiveWriter<T> where T : ICollection
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => WriteList(writer, val);
            }

            private class EnumerableWriter<T> : INonPrimitiveWriter<T> where T : IEnumerable
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => WriteEnumerable(writer, val);
            }

            private class ObjectWriter<T> : INonPrimitiveWriter<T>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(WriterState writer, T val) => WriteObject(writer, val);
            }
        }
    }
}