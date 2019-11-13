using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;

namespace Binaron.Serializer
{
    internal static class Deserializer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDictionary<string, object> ReadObject(BinaryReader reader)
        {
            var expandoObject = (IDictionary<string, object>) new ExpandoObject();
            while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
            {
                var key = reader.ReadString();
                expandoObject[key] = ReadValue(reader);
            }

            return expandoObject;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDictionary ReadDictionary(BinaryReader reader)
        {
            var count = reader.Read<int>();
            var result = new Dictionary<object, object>(ListCapacity.Clamp(count));
            for (var i = 0; i < count; i++)
            {
                var key = ReadValue(reader);
                var value = ReadValue(reader);
                result.Add(key, value);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] ReadList(BinaryReader reader)
        {
            var count = reader.Read<int>();
            var list = new List<object>(ListCapacity.Clamp(count));
            for (var i = 0; i < count; i++)
                list.Add(ReadValue(reader));
            var result = list.GetInternalArray();
            return result.Length == list.Count ? result : list.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICollection<object> ReadEnumerable(BinaryReader reader)
        {
            var result = new List<object>();
            while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem) 
                result.Add(ReadValue(reader));
            return result;
        }

        public static object ReadValue(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            switch (valueType)
            {
                case SerializedType.Object:
                    return ReadObject(reader);
                case SerializedType.Dictionary:
                    return ReadDictionary(reader);
                case SerializedType.List:
                    return ReadList(reader);
                case SerializedType.Enumerable:
                    return ReadEnumerable(reader);
                case SerializedType.String:
                    return Reader.ReadString(reader);
                case SerializedType.Byte:
                    return Reader.ReadByte(reader);
                case SerializedType.SByte:
                    return Reader.ReadSByte(reader);
                case SerializedType.UShort:
                    return Reader.ReadUShort(reader);
                case SerializedType.Short:
                    return Reader.ReadShort(reader);
                case SerializedType.UInt:
                    return Reader.ReadUInt(reader);
                case SerializedType.Int:
                    return Reader.ReadInt(reader);
                case SerializedType.ULong:
                    return Reader.ReadULong(reader);
                case SerializedType.Long:
                    return Reader.ReadLong(reader);
                case SerializedType.Float:
                    return Reader.ReadFloat(reader);
                case SerializedType.Double:
                    return Reader.ReadDouble(reader);
                case SerializedType.Bool:
                    return Reader.ReadBool(reader);
                case SerializedType.Decimal:
                    return Reader.ReadDecimal(reader);
                case SerializedType.DateTime:
                    return Reader.ReadDateTime(reader);
                case SerializedType.Char:
                    return Reader.ReadChar(reader);
                case SerializedType.Null:
                    return null;
                default:
                    throw new NotSupportedException($"SerializedType '{valueType}' is not supported");
            }
        }
    }
}