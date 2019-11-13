using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Accessors;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;

namespace Binaron.Serializer
{
    internal static class Serializer
    {
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
        public static void WriteNonNullValue(WriterState writer, object val)
        {
            if (WritePrimitive(writer, val))
                return;

            WriteNonPrimitive(writer, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteNonPrimitive(WriterState writer, object val)
        {
            switch (val)
            {
                case IDictionary dictionary when GenericWriter.WriteDictionary(writer, dictionary):
                    break;
                case IDictionary dictionary:
                    WriteDictionary(writer, dictionary);
                    break;
                case ICollection list when GenericWriter.WriteDictionary(writer, list):
                    break;
                case ICollection list when writer.SkipNullValues:
                    WriteEnumerable(writer, list);
                    break;
                case ICollection list:
                    WriteList(writer, list);
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
        private static void WriteObject(WriterState writer, object val)
        {
            writer.Write((byte) SerializedType.Object);
            var type = val.GetType();
            var getters = GetterHandler.GetGetterHandlers(type);

            foreach (var getter in getters)
                getter.Handle(writer, val);

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
        private static void WriteList(WriterState writer, ICollection list)
        {
            writer.Write((byte) SerializedType.List);
            writer.Write(list.Count);
            if (GenericWriter.WriteList(writer, list))
                return;

            // fallback to non-generic version
            foreach (var item in list)
                WriteValue(writer, item);
        }

        private static void WriteEnumerable(WriterState writer, IEnumerable enumerable)
        {
            writer.Write((byte) SerializedType.Enumerable);

            // ReSharper disable once PossibleMultipleEnumeration
            if (GenericWriter.WriteEnumerable(writer, enumerable))
                return;

            // fallback to non-generic version (we are not enumerating twice)
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in enumerable)
            {
                if (item != null)
                {
                    writer.Write((byte) EnumerableType.HasItem);
                    WriteValue(writer, item);
                }
                else if (!writer.SkipNullValues)
                {
                    writer.Write((byte) EnumerableType.HasItem);
                    writer.Write((byte) SerializedType.Null);
                }
            }

            writer.Write((byte) EnumerableType.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool WritePrimitive(WriterState writer, object value)
        {
            var type = value.GetType();

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    Writer.Write(writer, (string) value);
                    return true;
                case TypeCode.UInt32:
                    Writer.Write(writer, (uint) value);
                    return true;
                case TypeCode.Int32:
                    Writer.Write(writer, (int) value);
                    return true;
                case TypeCode.Byte:
                    Writer.Write(writer, (byte) value);
                    return true;
                case TypeCode.SByte:
                    Writer.Write(writer, (sbyte) value);
                    return true;
                case TypeCode.UInt16:
                    Writer.Write(writer, (ushort) value);
                    return true;
                case TypeCode.Int16:
                    Writer.Write(writer, (short) value);
                    return true;
                case TypeCode.Int64:
                    Writer.Write(writer, (long) value);
                    return true;
                case TypeCode.UInt64:
                    Writer.Write(writer, (ulong) value);
                    return true;
                case TypeCode.Single:
                    Writer.Write(writer, (float) value);
                    return true;
                case TypeCode.Double:
                    Writer.Write(writer, (double) value);
                    return true;
                case TypeCode.Decimal:
                    Writer.Write(writer, (decimal) value);
                    return true;
                case TypeCode.Boolean:
                    Writer.Write(writer, (bool) value);
                    return true;
                case TypeCode.DateTime:
                    Writer.Write(writer, (DateTime) value);
                    return true;
                case TypeCode.Char:
                    Writer.Write(writer, (char) value);
                    return true;
                default:
                    return false;
            }
        }
    }
}