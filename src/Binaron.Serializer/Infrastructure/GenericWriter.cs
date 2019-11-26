using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Binaron.Serializer.Enums;

namespace Binaron.Serializer.Infrastructure
{
    internal static class GenericWriter
    {
        private static readonly ConcurrentDictionary<(Type KeyType, Type ValueType), Func<WriterState, object, ValueTask>> DictionaryAdders = new ConcurrentDictionary<(Type KeyType, Type ValueType), Func<WriterState, object, ValueTask>>();

        private interface IGenericEnumerableWriter
        {
            ValueTask Write(WriterState writer, IEnumerable list);
        }

        private static class GetGenericEnumerableWriter<T>
        {
            public static readonly IGenericEnumerableWriter Writer = GenericEnumerableWriters.CreateWriter<T>();
        }

        private static class GenericEnumerableWriters
        {
            public static IGenericEnumerableWriter CreateWriter<T>()
            {
                var elementType = GenericType.GetICollectionGenericType<T>.Type;
                if (elementType == null)
                    return null;

                return (IGenericEnumerableWriter) Activator.CreateInstance(typeof(GenericEnumerableWriter<>).MakeGenericType(elementType));
            }

            private sealed class GenericEnumerableWriter<T> : IGenericEnumerableWriter
            {
                public async ValueTask Write(WriterState writer, IEnumerable list)
                {
                    foreach (var item in (IEnumerable<T>) list)
                        await Serializer.WriteNonPrimitive(writer, item);
                }
            }
        }

        public static async ValueTask<bool> WriteEnumerable<T>(WriterState writer, IEnumerable enumerable)
        {
            var elementType = GenericType.GetIEnumerableGenericType<T>.Type;
            if (elementType == null)
                return false;

            if (elementType.IsEnum != true)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                if (await WriteEnumerable(writer, enumerable, elementType))
                    return true;

                var genericWriter = GetGenericEnumerableWriter<T>.Writer;
                if (genericWriter == null)
                    return false;

                // ReSharper disable once PossibleMultipleEnumeration
                await genericWriter.Write(writer, enumerable);
                return true;
            }

            await WriteEnums(writer, enumerable, elementType);
            return true;
        }

        public static async ValueTask<bool> WriteEnumerable(WriterState writer, IEnumerable enumerable)
        {
            var elementType = GenericType.GetIEnumerable(enumerable.GetType());
            if (elementType == null)
                return false;

            if (elementType.IsEnum != true)
                return await WriteEnumerable(writer, enumerable, elementType);

            await WriteEnums(writer, enumerable, elementType);
            return true;
        }

        private static async ValueTask<bool> WriteEnumerable(WriterState writer, IEnumerable enumerable, Type elementType)
        {
            switch (Type.GetTypeCode(elementType))
            {
                case TypeCode.String:
                    foreach (var item in (IEnumerable<string>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        if (item != null)
                            await Writer.Write(writer, item);
                        else
                            await writer.Write((byte) SerializedType.Null);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Boolean:
                    foreach (var item in (IEnumerable<bool>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Byte:
                    foreach (var item in (IEnumerable<byte>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Char:
                    foreach (var item in (IEnumerable<char>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.DateTime:
                    foreach (var item in (IEnumerable<DateTime>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Decimal:
                    foreach (var item in (IEnumerable<decimal>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Double:
                    foreach (var item in (IEnumerable<double>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Int16:
                    foreach (var item in (IEnumerable<short>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Int32:
                    foreach (var item in (IEnumerable<int>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Int64:
                    foreach (var item in (IEnumerable<long>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.SByte:
                    foreach (var item in (IEnumerable<sbyte>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.Single:
                    foreach (var item in (IEnumerable<float>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.UInt16:
                    foreach (var item in (IEnumerable<ushort>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.UInt32:
                    foreach (var item in (IEnumerable<uint>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                case TypeCode.UInt64:
                    foreach (var item in (IEnumerable<ulong>) enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, item);
                    }

                    await writer.Write((byte) EnumerableType.End);
                    return true;
                default:
                    return false;
            }
        }

        private interface IGenericListWriter
        {
            ValueTask Write(WriterState writer, ICollection list);
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
                if (elementType == null)
                    return null;

                return (IGenericListWriter) (typeof(T).IsArray
                    ? Activator.CreateInstance(typeof(GenericArrayWriter<>).MakeGenericType(elementType)) 
                    : Activator.CreateInstance(typeof(GenericListWriter<>).MakeGenericType(elementType)));
            }

            private sealed class GenericArrayWriter<T> : IGenericListWriter
            {
                public async ValueTask Write(WriterState writer, ICollection list)
                {
                    foreach (var item in (T[]) list)
                        await Serializer.WriteNonPrimitive(writer, item);
                }
            }

            private sealed class GenericListWriter<T> : IGenericListWriter
            {
                public async ValueTask Write(WriterState writer, ICollection list)
                {
                    foreach (var item in (ICollection<T>) list)
                        await Serializer.WriteNonPrimitive(writer, item);
                }
            }
        }

        public static async ValueTask<bool> WriteList<T>(WriterState writer, ICollection list)
        {
            var listType = typeof(T);
            var elementType = GenericType.GetICollectionGenericType<T>.Type;
            if (elementType == null)
                return false;

            if (elementType.IsEnum != true)
            {
                if (listType.IsArray ? await WriteArray(writer, list, elementType) : await WriteList(writer, list, elementType))
                    return true;

                var genericWriter = GetGenericListWriter<T>.Writer;
                if (genericWriter == null)
                    return false;

                await genericWriter.Write(writer, list);
                return true;
            }

            await WriteEnums(writer, list, elementType);
            return true;
        }

        public static async ValueTask<bool> WriteList(WriterState writer, ICollection list)
        {
            var listType = list.GetType();
            var elementType = GenericType.GetICollection(listType);
            if (elementType == null)
                return false;

            if (elementType.IsEnum != true)
                return listType.IsArray ? await WriteArray(writer, list, elementType) : await WriteList(writer, list, elementType);

            await WriteEnums(writer, list, elementType);
            return true;
        }

        private static async ValueTask<bool> WriteArray(WriterState writer, ICollection list, Type elementType)
        {
            switch (Type.GetTypeCode(elementType))
            {
                case TypeCode.String:
                    foreach (var item in (string[]) list)
                    {
                        if (item != null)
                            await Writer.Write(writer, item);
                        else
                            await writer.Write((byte) SerializedType.Null);
                    }
                    return true;
                case TypeCode.Boolean:
                    foreach (var item in (bool[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Byte:
                    foreach (var item in (byte[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Char:
                    foreach (var item in (char[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.DateTime:
                    foreach (var item in (DateTime[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Decimal:
                    foreach (var item in (decimal[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Double:
                    foreach (var item in (double[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Int16:
                    foreach (var item in (short[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Int32:
                    foreach (var item in (int[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Int64:
                    foreach (var item in (long[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.SByte:
                    foreach (var item in (sbyte[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Single:
                    foreach (var item in (float[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.UInt16:
                    foreach (var item in (ushort[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.UInt32:
                    foreach (var item in (uint[]) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.UInt64:
                    foreach (var item in (ulong[]) list)
                        await Writer.Write(writer, item);
                    return true;
                default:
                    return false;
            }
        }

        private static async ValueTask<bool> WriteList(WriterState writer, ICollection list, Type elementType)
        {
            switch (Type.GetTypeCode(elementType))
            {
                case TypeCode.String:
                    foreach (var item in (ICollection<string>) list)
                    {
                        if (item != null)
                            await Writer.Write(writer, item);
                        else
                            await writer.Write((byte) SerializedType.Null);
                    }
                    return true;
                case TypeCode.Boolean:
                    foreach (var item in (ICollection<bool>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Byte:
                    foreach (var item in (ICollection<byte>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Char:
                    foreach (var item in (ICollection<char>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.DateTime:
                    foreach (var item in (ICollection<DateTime>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Decimal:
                    foreach (var item in (ICollection<decimal>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Double:
                    foreach (var item in (ICollection<double>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Int16:
                    foreach (var item in (ICollection<short>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Int32:
                    foreach (var item in (ICollection<int>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Int64:
                    foreach (var item in (ICollection<long>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.SByte:
                    foreach (var item in (ICollection<sbyte>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.Single:
                    foreach (var item in (ICollection<float>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.UInt16:
                    foreach (var item in (ICollection<ushort>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.UInt32:
                    foreach (var item in (ICollection<uint>) list)
                        await Writer.Write(writer, item);
                    return true;
                case TypeCode.UInt64:
                    foreach (var item in (ICollection<ulong>) list)
                        await Writer.Write(writer, item);
                    return true;
                default:
                    return false;
            }
        }

        public static async ValueTask WriteDictionary<T>(WriterState writer, T dictionary) => await WriteDictionary(writer, dictionary, GenericType.GetIDictionaryWriterGenericTypes<T>.Types);

        public static async ValueTask<bool> WriteDictionary(WriterState writer, object dictionary)
        {
            var types = GenericType.GetIDictionaryWriter(dictionary.GetType());
            return await WriteDictionary(writer, dictionary, types);
        }

        private static async ValueTask<bool> WriteDictionary(WriterState writer, object dictionary, (Type KeyType, Type ValueType) types)
        {
            if (types.KeyType == null) 
                return false;
            
            var writeAll = GetDictionaryWriter(types);
            await writeAll(writer, dictionary);
            return true;

        }

        private static Func<WriterState, object, ValueTask> GetDictionaryWriter((Type KeyType, Type ValueType) types) => DictionaryAdders.GetOrAdd(types, _ =>
        {
            var method = typeof(DictionaryWriter).GetMethod(nameof(DictionaryWriter.WriteAll))?.MakeGenericMethod(types.KeyType, types.ValueType) ?? throw new MissingMethodException();
            return (Func<WriterState, object, ValueTask>) Delegate.CreateDelegate(typeof(Func<WriterState, object, ValueTask>), null, method);
        });

        private static class DictionaryWriter
        {
            public static async ValueTask WriteAll<TKey, TValue>(WriterState writer, object obj)
            {
                var dictionary = (IDictionary<TKey, TValue>) obj;
                await writer.Write((byte) SerializedType.Dictionary);
                await writer.Write(dictionary.Count);
                foreach (var (key, value) in dictionary)
                {
                    await Serializer.WriteValue(writer, key);
                    await Serializer.WriteValue(writer, value);
                }
            }
        }

        private static async ValueTask WriteEnums(WriterState writer, ICollection list, Type elementType)
        {
            switch (Type.GetTypeCode(elementType))
            {
                case TypeCode.Byte:
                    foreach (var item in list)
                        await Writer.Write(writer, (byte) item);
                    break;
                case TypeCode.Int16:
                    foreach (var item in list)
                        await Writer.Write(writer, (short) item);
                    break;
                case TypeCode.Int32:
                    foreach (var item in list)
                        await Writer.Write(writer, (int) item);
                    break;
                case TypeCode.Int64:
                    foreach (var item in list)
                        await Writer.Write(writer, (long) item);
                    break;
                case TypeCode.SByte:
                    foreach (var item in list)
                        await Writer.Write(writer, (sbyte) item);
                    break;
                case TypeCode.UInt16:
                    foreach (var item in list)
                        await Writer.Write(writer, (ushort) item);
                    break;
                case TypeCode.UInt32:
                    foreach (var item in list)
                        await Writer.Write(writer, (uint) item);
                    break;
                case TypeCode.UInt64:
                    foreach (var item in list)
                        await Writer.Write(writer, (ulong) item);
                    break;
                default:
                    throw new InvalidProgramException();
            }
        }

        private static async ValueTask WriteEnums(WriterState writer, IEnumerable enumerable, Type elementType)
        {
            switch (Type.GetTypeCode(elementType))
            {
                case TypeCode.Byte:
                    foreach (var item in enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, (byte) item);
                    }
                    break;
                case TypeCode.Int16:
                    foreach (var item in enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, (short) item);
                    }
                    break;
                case TypeCode.Int32:
                    foreach (var item in enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, (int) item);
                    }
                    break;
                case TypeCode.Int64:
                    foreach (var item in enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, (long) item);
                    }
                    break;
                case TypeCode.SByte:
                    foreach (var item in enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, (sbyte) item);
                    }
                    break;
                case TypeCode.UInt16:
                    foreach (var item in enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, (ushort) item);
                    }
                    break;
                case TypeCode.UInt32:
                    foreach (var item in enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, (uint) item);
                    }
                    break;
                case TypeCode.UInt64:
                    foreach (var item in enumerable)
                    {
                        await writer.Write((byte) EnumerableType.HasItem);
                        await Writer.Write(writer, (ulong) item);
                    }
                    break;
                default:
                    throw new InvalidProgramException();
            }
            await writer.Write((byte) EnumerableType.End);
        }
    }
}