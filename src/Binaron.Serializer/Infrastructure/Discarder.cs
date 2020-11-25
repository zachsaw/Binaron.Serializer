using System;
using Binaron.Serializer.Enums;

namespace Binaron.Serializer.Infrastructure
{
    internal static class Discarder
    {
        public static void DiscardValue(ReaderState reader, SerializedType? knownType = null)
        {
            var valueType = knownType ?? Reader.ReadSerializedType(reader);
            switch (valueType)
            {
                case SerializedType.CustomObject:
                    DiscardValue(reader); // custom object ID
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                    { 
                        reader.ReadString(); // key 
                        DiscardValue(reader); // value 
                    } 
                    break; 
                case SerializedType.Object: 
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                    { 
                        reader.ReadString(); // key 
                        DiscardValue(reader); // value 
                    } 
                    break; 
                case SerializedType.Dictionary: 
                    var kvpCount = reader.Read<int>(); 
                    for (var i = 0; i < kvpCount; i++) 
                    { 
                        DiscardValue(reader); // key 
                        DiscardValue(reader); // value 
                    } 
                    break; 
                case SerializedType.List:
                {
                    var count = reader.Read<int>();
                    for (var i = 0; i < count; i++)
                        DiscardValue(reader);
                    break;
                }
                case SerializedType.HList:
                {
                    var count = reader.Read<int>();
                    var elementType = Reader.ReadSerializedType(reader);
                    DiscardValues(reader, count, elementType);
                    break;
                }
                case SerializedType.Enumerable: 
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem) 
                        DiscardValue(reader); 
                    break;
                case SerializedType.HEnumerable:
                {
                    var elementType = Reader.ReadSerializedType(reader);
                    DiscardValues(reader, elementType);
                    break;
                }
                case SerializedType.String:
                    Reader.ReadString(reader);
                    break;
                case SerializedType.Byte:
                    Reader.ReadByte(reader);
                    break;
                case SerializedType.SByte:
                    Reader.ReadSByte(reader);
                    break;
                case SerializedType.UShort:
                    Reader.ReadUShort(reader);
                    break;
                case SerializedType.Short:
                    Reader.ReadShort(reader);
                    break;
                case SerializedType.UInt:
                    Reader.ReadUInt(reader);
                    break;
                case SerializedType.Int:
                    Reader.ReadInt(reader);
                    break;
                case SerializedType.ULong:
                    Reader.ReadULong(reader);
                    break;
                case SerializedType.Long:
                    Reader.ReadLong(reader);
                    break;
                case SerializedType.Float:
                    Reader.ReadFloat(reader);
                    break;
                case SerializedType.Double:
                    Reader.ReadDouble(reader);
                    break;
                case SerializedType.Bool:
                    Reader.ReadBool(reader);
                    break;
                case SerializedType.Decimal:
                    Reader.ReadDecimal(reader);
                    break;
                case SerializedType.DateTime:
                    Reader.ReadDateTime(reader);
                    break;
                case SerializedType.Guid:
                    Reader.ReadGuid(reader);
                    break;
                case SerializedType.Char:
                    Reader.ReadChar(reader);
                    break;
                case SerializedType.Null:
                    break;
                default:
                    throw new NotSupportedException($"SerializedType '{valueType}' is not supported");
            }
        }

        public static void Discard(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                DiscardValue(reader);
        }
        
        public static void Discard(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                DiscardValue(reader);
        }

        public static void DiscardDateTimes(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadDateTime(reader);
        }
        
        public static void DiscardGuids(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadGuid(reader);
        }
        
        public static void DiscardStrings(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadString(reader);
        }
        
        public static void DiscardBools(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadBool(reader);
        }
        
        public static void DiscardChars(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadChar(reader);
        }
        
        public static void DiscardBytes(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadByte(reader);
        }

        public static void DiscardSBytes(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadSByte(reader);
        }

        public static void DiscardShorts(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadShort(reader);
        }

        public static void DiscardUShorts(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadUShort(reader);
        }

        public static void DiscardInts(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadInt(reader);
        }

        public static void DiscardUInts(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadUInt(reader);
        }

        public static void DiscardLongs(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadLong(reader);
        }

        public static void DiscardULongs(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadULong(reader);
        }

        public static void DiscardFloats(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadFloat(reader);
        }

        public static void DiscardDoubles(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadDouble(reader);
        }

        public static void DiscardDecimals(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                Reader.ReadDecimal(reader);
        }
        
        public static void DiscardValues(ReaderState reader, int count, SerializedType elementType)
        {
            switch (elementType)
            {
                case SerializedType.String:
                    DiscardStrings(reader, count);
                    break;
                case SerializedType.Char:
                    DiscardChars(reader, count);
                    break;
                case SerializedType.Byte:
                    DiscardBytes(reader, count);
                    break;
                case SerializedType.SByte:
                    DiscardSBytes(reader, count);
                    break;
                case SerializedType.UShort:
                    DiscardUShorts(reader, count);
                    break;
                case SerializedType.Short:
                    DiscardShorts(reader, count);
                    break;
                case SerializedType.UInt:
                    DiscardUInts(reader, count);
                    break;
                case SerializedType.Int:
                    DiscardInts(reader, count);
                    break;
                case SerializedType.ULong:
                    DiscardULongs(reader, count);
                    break;
                case SerializedType.Long:
                    DiscardLongs(reader, count);
                    break;
                case SerializedType.Float:
                    DiscardFloats(reader, count);
                    break;
                case SerializedType.Double:
                    DiscardDoubles(reader, count);
                    break;
                case SerializedType.Decimal:
                    DiscardDecimals(reader, count);
                    break;
                case SerializedType.Bool:
                    DiscardBools(reader, count);
                    break;
                case SerializedType.DateTime:
                    DiscardDateTimes(reader, count);
                    break;
                case SerializedType.Guid:
                    DiscardGuids(reader, count);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(elementType), elementType, null);
            }
        }

        private static void DiscardDateTimes(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadDateTime(reader);
        }

        private static void DiscardGuids(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadGuid(reader);
        }

        private static void DiscardStrings(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadString(reader);
        }

        private static void DiscardBools(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadBool(reader);
        }

        private static void DiscardChars(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadChar(reader);
        }

        private static void DiscardBytes(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadByte(reader);
        }

        private static void DiscardSBytes(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadSByte(reader);
        }

        private static void DiscardShorts(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadShort(reader);
        }

        private static void DiscardUShorts(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadUShort(reader);
        }

        private static void DiscardInts(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadInt(reader);
        }

        private static void DiscardUInts(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadUInt(reader);
        }

        private static void DiscardLongs(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadLong(reader);
        }

        private static void DiscardULongs(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadULong(reader);
        }

        private static void DiscardFloats(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadFloat(reader);
        }

        private static void DiscardDoubles(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadDouble(reader);
        }

        private static void DiscardDecimals(ReaderState reader)
        {
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                Reader.ReadDecimal(reader);
        }

        public static void DiscardValues(ReaderState reader, SerializedType elementType)
        {
            switch (elementType)
            {
                case SerializedType.String:
                    DiscardStrings(reader);
                    break;
                case SerializedType.Char:
                    DiscardChars(reader);
                    break;
                case SerializedType.Byte:
                    DiscardBytes(reader);
                    break;
                case SerializedType.SByte:
                    DiscardSBytes(reader);
                    break;
                case SerializedType.UShort:
                    DiscardUShorts(reader);
                    break;
                case SerializedType.Short:
                    DiscardShorts(reader);
                    break;
                case SerializedType.UInt:
                    DiscardUInts(reader);
                    break;
                case SerializedType.Int:
                    DiscardInts(reader);
                    break;
                case SerializedType.ULong:
                    DiscardULongs(reader);
                    break;
                case SerializedType.Long:
                    DiscardLongs(reader);
                    break;
                case SerializedType.Float:
                    DiscardFloats(reader);
                    break;
                case SerializedType.Double:
                    DiscardDoubles(reader);
                    break;
                case SerializedType.Decimal:
                    DiscardDecimals(reader);
                    break;
                case SerializedType.Bool:
                    DiscardBools(reader);
                    break;
                case SerializedType.DateTime:
                    DiscardDateTimes(reader);
                    break;
                case SerializedType.Guid:
                    DiscardGuids(reader);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(elementType), elementType, null);
            }
        }
    }
}