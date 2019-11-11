using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Accessors;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;
using BinaryWriter = Binaron.Serializer.Infrastructure.BinaryWriter;

namespace Binaron.Serializer
{
    internal static class Serializer
    {
        private static void WriteObject(BinaryWriter writer, object value)
        {
            var type = value.GetType();
            var getters = GetterHandler.GetGetterHandlers(type);

            foreach (var getter in getters) 
                getter.Handle(writer, value);

            writer.Write((byte) EnumerableType.End);
        }

        public static void WriteValue(BinaryWriter writer, object val)
        {
            if (val == null)
            {
                writer.Write((byte) SerializedType.Null);
                return;
            }
            
            if (WritePrimitive(writer, val))
                return;

            WriteNonPrimitive(writer, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteNonPrimitive(BinaryWriter writer, object val)
        {
            switch (val)
            {
                case IDictionary dictionary:
                    if (GenericWriter.WriteDictionary(writer, dictionary))
                        break;
                    
                    writer.Write((byte) SerializedType.Dictionary);
                    writer.Write(dictionary.Count);
                    foreach (var (key, value) in dictionary.Keys.Zip(dictionary.Values))
                    {
                        WriteValue(writer, key);
                        WriteValue(writer, value);
                    }
                    break;
                case ICollection list:
                    if (GenericWriter.WriteDictionary(writer, list))
                        break;
                    
                    writer.Write((byte) SerializedType.List);
                    writer.Write(list.Count);
                    if (!GenericWriter.WriteList(writer, list))
                    {
                        // fallback to non-generic version
                        foreach (var item in list)
                            WriteValue(writer, item);
                    }
                    break;
                case IEnumerable enumerable:
                    writer.Write((byte) SerializedType.Enumerable);

                    // ReSharper disable once PossibleMultipleEnumeration
                    if (GenericWriter.WriteEnumerable(writer, enumerable))
                        break;

                    // fallback to non-generic version (we are not enumerating twice)
                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var item in enumerable)
                    {
                        writer.Write((byte) EnumerableType.HasItem);
                        WriteValue(writer, item);
                    }

                    writer.Write((byte) EnumerableType.End);
                    break;
                default:
                    writer.Write((byte) SerializedType.Object);
                    WriteObject(writer, val);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool WritePrimitive(BinaryWriter writer, object value)
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