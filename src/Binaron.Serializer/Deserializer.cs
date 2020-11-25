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
        public static IDictionary<string, object> ReadObject(ReaderState reader)
        {
            var expandoObject = (IDictionary<string, object>) new ExpandoObject();
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
            {
                var key = reader.ReadString();
                expandoObject[key] = ReadValue(reader);
            }

            return expandoObject;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDictionary ReadDictionary(ReaderState reader)
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
        public static object[] ReadList(ReaderState reader)
        {
            var count = reader.Read<int>();
            var list = new List<object>(ListCapacity.Clamp(count));
            for (var i = 0; i < count; i++)
                list.Add(ReadValue(reader));
            var result = list.GetInternalArray();
            return result.Length == list.Count ? result : list.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] ReadHList(ReaderState reader)
        {
            var count = reader.Read<int>();
            var elementType = Reader.ReadSerializedType(reader);
            var list = new List<object>(ListCapacity.Clamp(count));
            ReadValuesIntoList(reader, count, elementType, list);
            var result = list.GetInternalArray();
            return result.Length == list.Count ? result : list.ToArray();
        }

        public static void ReadValuesIntoList<T>(ReaderState reader, int count, SerializedType elementType, T list) where T : IList
        {
            switch (elementType)
            {
                case SerializedType.String:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadString(reader));
                    break;
                case SerializedType.Char:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadChar(reader));
                    break;
                case SerializedType.Byte:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadByte(reader));
                    break;
                case SerializedType.SByte:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadSByte(reader));
                    break;
                case SerializedType.UShort:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadUShort(reader));
                    break;
                case SerializedType.Short:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadShort(reader));
                    break;
                case SerializedType.UInt:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadUInt(reader));
                    break;
                case SerializedType.Int:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadInt(reader));
                    break;
                case SerializedType.ULong:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadULong(reader));
                    break;
                case SerializedType.Long:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadLong(reader));
                    break;
                case SerializedType.Float:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadFloat(reader));
                    break;
                case SerializedType.Double:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadDouble(reader));
                    break;
                case SerializedType.Decimal:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadDecimal(reader));
                    break;
                case SerializedType.Bool:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadBool(reader));
                    break;
                case SerializedType.DateTime:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadDateTime(reader));
                    break;
                case SerializedType.Guid:
                    for (var i = 0; i < count; i++)
                        list.Add(Reader.ReadGuid(reader));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICollection<object> ReadEnumerable(ReaderState reader)
        {
            var result = new List<object>();
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                result.Add(ReadValue(reader));
            return result;
        }

        public static object ReadValue(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadValue(reader, valueType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadValue(ReaderState reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.CustomObject:
                    ReadValue(reader); // discard custom object ID
                    return ReadObject(reader);
                case SerializedType.Object:
                    return ReadObject(reader);
                case SerializedType.Dictionary:
                    return ReadDictionary(reader);
                case SerializedType.List:
                    return ReadList(reader);
                case SerializedType.HList:
                    return ReadHList(reader);
                case SerializedType.Enumerable:
                    return ReadEnumerable(reader);
                case SerializedType.HEnumerable:
                    return ReadHEnumerable(reader);
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
                case SerializedType.Guid:
                    return Reader.ReadGuid(reader);
                case SerializedType.Char:
                    return Reader.ReadChar(reader);
                case SerializedType.Null:
                    return null;
                default:
                    throw new NotSupportedException($"SerializedType '{valueType}' is not supported");
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICollection<object> ReadHEnumerable(ReaderState reader)
        {
            var elementType = Reader.ReadSerializedType(reader);
            var result = new List<object>();
            ReadValuesIntoList(reader, elementType, result);
            return result;
        }
        
        public static void ReadValuesIntoList<T>(ReaderState reader, SerializedType elementType, T list) where T : IList
        {
            switch (elementType)
            {
                case SerializedType.String:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadString(reader));
                    break;
                case SerializedType.Char:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadChar(reader));
                    break;
                case SerializedType.Byte:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadByte(reader));
                    break;
                case SerializedType.SByte:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadSByte(reader));
                    break;
                case SerializedType.UShort:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadUShort(reader));
                    break;
                case SerializedType.Short:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadShort(reader));
                    break;
                case SerializedType.UInt:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadUInt(reader));
                    break;
                case SerializedType.Int:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadInt(reader));
                    break;
                case SerializedType.ULong:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadULong(reader));
                    break;
                case SerializedType.Long:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadLong(reader));
                    break;
                case SerializedType.Float:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadFloat(reader));
                    break;
                case SerializedType.Double:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadDouble(reader));
                    break;
                case SerializedType.Decimal:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadDecimal(reader));
                    break;
                case SerializedType.Bool:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadBool(reader));
                    break;
                case SerializedType.DateTime:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadDateTime(reader));
                    break;
                case SerializedType.Guid:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        list.Add(Reader.ReadGuid(reader));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}