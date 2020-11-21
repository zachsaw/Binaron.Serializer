using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Binaron.Serializer.Enums;
using TypeCode = Binaron.Serializer.Enums.TypeCode;

namespace Binaron.Serializer.Infrastructure
{
    internal static class GenericWriter
    {
        private static readonly ConcurrentDictionary<(Type KeyType, Type ValueType), Action<WriterState, object>> DictionaryAdders = new ConcurrentDictionary<(Type KeyType, Type ValueType), Action<WriterState, object>>();
        private static readonly ConcurrentDictionary<(Type KeyType, Type ValueType), Action<WriterState, object>> ReadOnlyDictionaryAdders = new ConcurrentDictionary<(Type KeyType, Type ValueType), Action<WriterState, object>>();

        private interface IGenericEnumerableWriter
        {
            void Write(WriterState writer, IEnumerable list);
        }

        private static class GetGenericEnumerableWriter<T>
        {
            public static readonly IGenericEnumerableWriter Writer = GenericEnumerableWriters.CreateWriter<T>();
        }

        private static class GenericEnumerableWriters
        {
            public static IGenericEnumerableWriter CreateWriter<T>()
            {
                var elementType = GenericType.GetIEnumerableGenericType<T>.Type;
                if (elementType.Type == null || elementType.Type == typeof(object))
                    return null;

                return (IGenericEnumerableWriter) Activator.CreateInstance(typeof(GenericEnumerableWriter<>).MakeGenericType(elementType.Type));
            }

            private class GenericEnumerableWriter<T> : IGenericEnumerableWriter
            {
                public void Write(WriterState writer, IEnumerable list)
                {
                    foreach (var item in (IEnumerable<T>) list)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Serializer.WriteNonPrimitive(writer, item);
                    }
                    writer.Write((byte) EnumerableType.End);
                }
            }
        }

        public static bool WriteEnumerable<T>(WriterState writer, IEnumerable enumerable)
        {
            var elementType = GenericType.GetIEnumerableGenericType<T>.Type;
            if (elementType.Type == null)
                return false;

            if (elementType.Type.IsEnum != true)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                if (WriteEnumerable(writer, enumerable, elementType.Code))
                    return true;

                var genericWriter = GetGenericEnumerableWriter<T>.Writer;
                if (genericWriter == null)
                    return false;

                // ReSharper disable once PossibleMultipleEnumeration
                genericWriter.Write(writer, enumerable);
                return true;
            }

            WriteEnums(writer, enumerable, elementType.Code);
            return true;
        }

        public static bool WriteEnumerable(WriterState writer, IEnumerable enumerable)
        {
            var elementType = GenericType.GetIEnumerable(enumerable.GetType());
            if (elementType.Type == null)
                return false;

            if (elementType.Type.IsEnum != true)
                return WriteEnumerable(writer, enumerable, elementType.Code);

            WriteEnums(writer, enumerable, elementType.Code);
            return true;
        }

        private static bool WriteEnumerable(WriterState writer, IEnumerable enumerable, TypeCode elementTypeCode)
        {
            switch (elementTypeCode)
            {
                case TypeCode.String:
                    foreach (var item in (IEnumerable<string>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        if (item != null)
                            Writer.Write(writer, item);
                        else
                            writer.Write((byte) SerializedType.Null);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Boolean:
                    foreach (var item in (IEnumerable<bool>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Byte:
                    foreach (var item in (IEnumerable<byte>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Char:
                    foreach (var item in (IEnumerable<char>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.DateTime:
                    foreach (var item in (IEnumerable<DateTime>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Guid:
                    foreach (var item in (IEnumerable<Guid>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Decimal:
                    foreach (var item in (IEnumerable<decimal>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Double:
                    foreach (var item in (IEnumerable<double>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Int16:
                    foreach (var item in (IEnumerable<short>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Int32:
                    foreach (var item in (IEnumerable<int>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Int64:
                    foreach (var item in (IEnumerable<long>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.SByte:
                    foreach (var item in (IEnumerable<sbyte>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Single:
                    foreach (var item in (IEnumerable<float>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.UInt16:
                    foreach (var item in (IEnumerable<ushort>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.UInt32:
                    foreach (var item in (IEnumerable<uint>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.UInt64:
                    foreach (var item in (IEnumerable<ulong>) enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    return true;
                default:
                    return false;
            }
        }

        private interface IGenericListWriter
        {
            void Write(WriterState writer, ICollection list);
        }

        private static class GetGenericListWriter<T>
        {
            public static readonly IGenericListWriter Writer = GenericListWriters.CreateWriter<T>();
        }

        private static class GenericListWriters
        {
            public static IGenericListWriter CreateWriter<T>()
            {
                var elementType = GenericType.GetICollectionGenericType<T>.Type;
                if (elementType.Type == null || elementType.Type == typeof(object))
                    return null;

                return (IGenericListWriter) (typeof(T).IsArray
                    ? Activator.CreateInstance(typeof(GenericArrayWriter<>).MakeGenericType(elementType.Type)) 
                    : Activator.CreateInstance(typeof(GenericListWriter<>).MakeGenericType(elementType.Type)));
            }

            private class GenericArrayWriter<T> : IGenericListWriter
            {
                public void Write(WriterState writer, ICollection list)
                {
                    foreach (var item in (T[]) list)
                        Serializer.WriteNonPrimitive(writer, item);
                }
            }

            private class GenericListWriter<T> : IGenericListWriter
            {
                public void Write(WriterState writer, ICollection list)
                {
                    foreach (var item in (ICollection<T>) list)
                        Serializer.WriteNonPrimitive(writer, item);
                }
            }
        }

        public static bool WriteList<T>(WriterState writer, ICollection list)
        {
            var listType = typeof(T);

            var elementType = GenericType.GetICollectionGenericType<T>.Type;
            if (elementType.Type == null)
                return false;

            if (elementType.Type.IsEnum != true)
            {
                if (listType.IsArray ? WriteArray(writer, list, elementType.Code) : WriteList(writer, list, elementType.Code))
                    return true;

                var genericWriter = GetGenericListWriter<T>.Writer;
                if (genericWriter == null)
                    return false;

                genericWriter.Write(writer, list);
                return true;
            }

            WriteEnums(writer, list, elementType.Code);
            return true;
        }

        public static bool WriteList(WriterState writer, ICollection list)
        {
            var listType = list.GetType();
            var elementType = GenericType.GetICollection(listType);
            if (elementType.Type == null)
                return false;

            if (elementType.Type.IsEnum != true)
                return listType.IsArray ? WriteArray(writer, list, elementType.Code) : WriteList(writer, list, elementType.Code);

            WriteEnums(writer, list, elementType.Code);
            return true;
        }

        private static bool WriteArray(WriterState writer, ICollection list, TypeCode elementTypeCode)
        {
            switch (elementTypeCode)
            {
                case TypeCode.String:
                    foreach (var item in (string[]) list)
                    {
                        if (item != null)
                            Writer.Write(writer, item);
                        else
                            writer.Write((byte) SerializedType.Null);
                    }
                    return true;
                case TypeCode.Boolean:
                    foreach (var item in (bool[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Byte:
                    foreach (var item in (byte[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Char:
                    foreach (var item in (char[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.DateTime:
                    foreach (var item in (DateTime[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Guid:
                    foreach (var item in (Guid[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Decimal:
                    foreach (var item in (decimal[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Double:
                    foreach (var item in (double[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Int16:
                    foreach (var item in (short[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Int32:
                    foreach (var item in (int[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Int64:
                    foreach (var item in (long[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.SByte:
                    foreach (var item in (sbyte[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Single:
                    foreach (var item in (float[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.UInt16:
                    foreach (var item in (ushort[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.UInt32:
                    foreach (var item in (uint[]) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.UInt64:
                    foreach (var item in (ulong[]) list)
                        Writer.Write(writer, item);
                    return true;
                default:
                    return false;
            }
        }

        private static bool WriteList(WriterState writer, ICollection list, TypeCode elementTypeCode)
        {
            switch (elementTypeCode)
            {
                case TypeCode.String:
                    foreach (var item in (ICollection<string>) list)
                    {
                        if (item != null)
                            Writer.Write(writer, item);
                        else
                            writer.Write((byte) SerializedType.Null);
                    }
                    return true;
                case TypeCode.Boolean:
                    foreach (var item in (ICollection<bool>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Byte:
                    foreach (var item in (ICollection<byte>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Char:
                    foreach (var item in (ICollection<char>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.DateTime:
                    foreach (var item in (ICollection<DateTime>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Guid:
                    foreach (var item in (ICollection<Guid>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Decimal:
                    foreach (var item in (ICollection<decimal>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Double:
                    foreach (var item in (ICollection<double>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Int16:
                    foreach (var item in (ICollection<short>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Int32:
                    foreach (var item in (ICollection<int>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Int64:
                    foreach (var item in (ICollection<long>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.SByte:
                    foreach (var item in (ICollection<sbyte>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.Single:
                    foreach (var item in (ICollection<float>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.UInt16:
                    foreach (var item in (ICollection<ushort>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.UInt32:
                    foreach (var item in (ICollection<uint>) list)
                        Writer.Write(writer, item);
                    return true;
                case TypeCode.UInt64:
                    foreach (var item in (ICollection<ulong>) list)
                        Writer.Write(writer, item);
                    return true;
                default:
                    return false;
            }
        }

        public static void WriteReadOnlyDictionary<T>(WriterState writer, T dictionary) => WriteReadOnlyDictionary(writer, dictionary, GenericType.GetIReadOnlyDictionaryWriterGenericTypes<T>.Types);

        public static void WriteDictionary<T>(WriterState writer, T dictionary) => WriteDictionary(writer, dictionary, GenericType.GetIDictionaryWriterGenericTypes<T>.Types);

        public static bool WriteReadOnlyDictionary(WriterState writer, object dictionary) => WriteReadOnlyDictionary(writer, dictionary, GenericType.GetIReadOnlyDictionaryWriter(dictionary.GetType()));

        public static bool WriteDictionary(WriterState writer, object dictionary) => WriteDictionary(writer, dictionary, GenericType.GetIDictionaryWriter(dictionary.GetType()));

        private static bool WriteReadOnlyDictionary(WriterState writer, object dictionary, (Type KeyType, Type ValueType) types)
        {
            if (types.KeyType == null) 
                return false;
            
            var writeAll = GetReadOnlyDictionaryWriter(types);
            writeAll(writer, dictionary);
            return true;
        }

        private static Action<WriterState, object> GetReadOnlyDictionaryWriter((Type KeyType, Type ValueType) types) => ReadOnlyDictionaryAdders.GetOrAdd(types, _ =>
        {
            var method = typeof(DictionaryWriter).GetMethod(nameof(DictionaryWriter.WriteAllReadOnly))?.MakeGenericMethod(types.KeyType, types.ValueType) ?? throw new MissingMethodException();
            return (Action<WriterState, object>) Delegate.CreateDelegate(typeof(Action<WriterState, object>), null, method);
        });
        
        private static bool WriteDictionary(WriterState writer, object dictionary, (Type KeyType, Type ValueType) types)
        {
            if (types.KeyType == null) 
                return false;
            
            var writeAll = GetDictionaryWriter(types);
            writeAll(writer, dictionary);
            return true;
        }

        private static Action<WriterState, object> GetDictionaryWriter((Type KeyType, Type ValueType) types) => DictionaryAdders.GetOrAdd(types, _ =>
        {
            var method = typeof(DictionaryWriter).GetMethod(nameof(DictionaryWriter.WriteAll))?.MakeGenericMethod(types.KeyType, types.ValueType) ?? throw new MissingMethodException();
            return (Action<WriterState, object>) Delegate.CreateDelegate(typeof(Action<WriterState, object>), null, method);
        });

        private static class DictionaryWriter
        {
            public static void WriteAllReadOnly<TKey, TValue>(WriterState writer, object obj)
            {
                var dictionary = (IReadOnlyDictionary<TKey, TValue>) obj;
                writer.Write((byte) SerializedType.Dictionary);
                writer.Write(dictionary.Count);
                foreach (var (key, value) in dictionary)
                {
                    Serializer.WriteValue(writer, key);
                    Serializer.WriteValue(writer, value);
                }
            }
            
            public static void WriteAll<TKey, TValue>(WriterState writer, object obj)
            {
                var dictionary = (IDictionary<TKey, TValue>) obj;
                writer.Write((byte) SerializedType.Dictionary);
                writer.Write(dictionary.Count);
                foreach (var (key, value) in dictionary)
                {
                    Serializer.WriteValue(writer, key);
                    Serializer.WriteValue(writer, value);
                }
            }
        }

        private static void WriteEnums(WriterState writer, ICollection list, TypeCode elementTypeCode)
        {
            switch (elementTypeCode)
            {
                case TypeCode.Byte:
                    foreach (var item in list)
                        Writer.Write(writer, (byte) item);
                    break;
                case TypeCode.Int16:
                    foreach (var item in list)
                        Writer.Write(writer, (short) item);
                    break;
                case TypeCode.Int32:
                    foreach (var item in list)
                        Writer.Write(writer, (int) item);
                    break;
                case TypeCode.Int64:
                    foreach (var item in list)
                        Writer.Write(writer, (long) item);
                    break;
                case TypeCode.SByte:
                    foreach (var item in list)
                        Writer.Write(writer, (sbyte) item);
                    break;
                case TypeCode.UInt16:
                    foreach (var item in list)
                        Writer.Write(writer, (ushort) item);
                    break;
                case TypeCode.UInt32:
                    foreach (var item in list)
                        Writer.Write(writer, (uint) item);
                    break;
                case TypeCode.UInt64:
                    foreach (var item in list)
                        Writer.Write(writer, (ulong) item);
                    break;
                default:
                    throw new InvalidProgramException();
            }
        }

        private static void WriteEnums(WriterState writer, IEnumerable enumerable, TypeCode elementTypeCode)
        {
            switch (elementTypeCode)
            {
                case TypeCode.Byte:
                    foreach (var item in enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, (byte) item);
                    }
                    break;
                case TypeCode.Int16:
                    foreach (var item in enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, (short) item);
                    }
                    break;
                case TypeCode.Int32:
                    foreach (var item in enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, (int) item);
                    }
                    break;
                case TypeCode.Int64:
                    foreach (var item in enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, (long) item);
                    }
                    break;
                case TypeCode.SByte:
                    foreach (var item in enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, (sbyte) item);
                    }
                    break;
                case TypeCode.UInt16:
                    foreach (var item in enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, (ushort) item);
                    }
                    break;
                case TypeCode.UInt32:
                    foreach (var item in enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, (uint) item);
                    }
                    break;
                case TypeCode.UInt64:
                    foreach (var item in enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        Writer.Write(writer, (ulong) item);
                    }
                    break;
                default:
                    throw new InvalidProgramException();
            }
            writer.Write((byte) EnumerableType.End);
        }
    }
}