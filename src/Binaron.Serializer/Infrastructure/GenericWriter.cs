using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Enums;

namespace Binaron.Serializer.Infrastructure
{
    internal static class GenericWriter
    {
        private static readonly ConcurrentDictionary<(Type KeyType, Type ValueType), Action<WriterState, object>> DictionaryAdders = new ConcurrentDictionary<(Type KeyType, Type ValueType), Action<WriterState, object>>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WriteEnumerable(WriterState writer, IEnumerable enumerable)
        {
            var elementType = GenericType.GetIEnumerable(enumerable.GetType());
            if (elementType?.IsEnum != true)
            {
                switch (Type.GetTypeCode(elementType))
                {
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

            WriteEnums(writer, enumerable, elementType);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WriteList(WriterState writer, ICollection list)
        {
            var elementType = GenericType.GetICollection(list.GetType());
            if (elementType?.IsEnum != true)
            {
                switch (Type.GetTypeCode(elementType))
                {
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

            WriteEnums(writer, list, elementType);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WriteDictionary(WriterState writer, object dictionary)
        {
            var type = GenericType.GetIDictionaryWriter(dictionary.GetType());
            if (type.KeyType != null)
            {
                var writeAll = GetDictionaryWriter(type);
                writeAll(writer, dictionary);
                return true;
            }
            return false;
        }

        private static Action<WriterState, object> GetDictionaryWriter((Type KeyType, Type ValueType) type) => DictionaryAdders.GetOrAdd(type, _ =>
        {
            var method = typeof(DictionaryWriter).GetMethod(nameof(DictionaryWriter.WriteAll))?.MakeGenericMethod(type.KeyType, type.ValueType) ?? throw new MissingMethodException();
            return (Action<WriterState, object>) Delegate.CreateDelegate(typeof(Action<WriterState, object>), null, method);
        });

        private static class DictionaryWriter
        {
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteEnums(WriterState writer, ICollection list, Type elementType)
        {
            switch (Type.GetTypeCode(elementType))
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

        private static void WriteEnums(WriterState writer, IEnumerable enumerable, Type elementType)
        {
            switch (Type.GetTypeCode(elementType))
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