using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Enums;

namespace Binaron.Serializer.Infrastructure
{
    internal static class SelfUpgradingReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadAsObject<T>(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsObject<T>(reader, valueType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadAsObject<T>(BinaryReader reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.Null:
                    return null;
                case SerializedType.Object:
                    return TypedDeserializer.ReadObject<T>(reader);
                case SerializedType.Dictionary:
                    return TypedDeserializer.ReadDictionary<T>(reader);
                case SerializedType.List:
                    return TypedDeserializer.ReadList<T>(reader);
                case SerializedType.Enumerable:
                    return TypedDeserializer.ReadEnumerable<T>(reader);
                default:
                    return ReadAsMiscObject<T>(reader, valueType);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ReadAsMiscObject<T>(BinaryReader reader, SerializedType valueType)
        {
            var result = Deserializer.ReadValue(reader, valueType);
            return typeof(T) == typeof(object) ? result : GetNullableOrDefault<T>(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object AsEnum(Type type, object val)
        {
            try
            {
                return Enum.ToObject(type, val);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object GetNullableOrDefault<T>(object val)
        {
            var type = Nullable.GetUnderlyingType(typeof(T));
            if (type == null)
                return null;

            if (type.IsEnum)
                return AsEnum(type, val);

            return Upgrade(type, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object Upgrade(Type targetType, object val)
        {
            var sourceType = val.GetType();
            return sourceType == targetType ? val : GetUpgrader(sourceType, targetType)(val);
        }

        private static readonly ConcurrentDictionary<(Type, Type), Func<object, object>> Upgraders = new ConcurrentDictionary<(Type, Type), Func<object, object>>();
        private static Func<object, object> GetUpgrader(Type from, Type to) => Upgraders.GetOrAdd((from, to), _ =>
        {
            var method = typeof(Upgrader).GetMethod(nameof(Upgrader.Upgrade))?.MakeGenericMethod(from, to) ?? throw new MissingMethodException();
            return (Func<object, object>) Delegate.CreateDelegate(typeof(Func<object, object>), null, method);
        });

        private static class Upgrader
        {
            public static object Upgrade<TFrom, TTo>(object val)
            {
                switch (Type.GetTypeCode(typeof(TFrom)))
                {
                    case TypeCode.Boolean:
                    {
                        return As<TTo>((bool) val, out var result) ? result : null;
                    }
                    case TypeCode.Byte:
                    {
                        return As<TTo>((byte) val, out var result) ? result : null;
                    }
                    case TypeCode.Char:
                    {
                        return As<TTo>((char) val, out var result) ? result : null;
                    }
                    case TypeCode.DateTime:
                    {
                        return As<TTo>((DateTime) val, out var result) ? result : null;
                    }
                    case TypeCode.Decimal:
                    {
                        return As<TTo>((decimal) val, out var result) ? result : null;
                    }
                    case TypeCode.Double:
                    {
                        return As<TTo>((double) val, out var result) ? result : null;
                    }
                    case TypeCode.Int16:
                    {
                        return As<TTo>((short) val, out var result) ? result : null;
                    }
                    case TypeCode.Int32:
                    {
                        return As<TTo>((int) val, out var result) ? result : null;
                    }
                    case TypeCode.Int64:
                    {
                        return As<TTo>((long) val, out var result) ? result : null;
                    }
                    case TypeCode.SByte:
                    {
                        return As<TTo>((sbyte) val, out var result) ? result : null;
                    }
                    case TypeCode.Single:
                    {
                        return As<TTo>((float) val, out var result) ? result : null;
                    }
                    case TypeCode.UInt16:
                    {
                        return As<TTo>((ushort) val, out var result) ? result : null;
                    }
                    case TypeCode.UInt32:
                    {
                        return As<TTo>((uint) val, out var result) ? result : null;
                    }
                    case TypeCode.UInt64:
                    {
                        return As<TTo>((ulong) val, out var result) ? result : null;
                    }
                    default:
                        return null;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadAsBool(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsBool(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool? ReadAsBool(BinaryReader reader, SerializedType valueType)
        {
            if (valueType == SerializedType.Bool)
                return Reader.ReadBool(reader);

            TypedDeserializer.DiscardValue(reader, valueType);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadAsByte(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsByte(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte? ReadAsByte(BinaryReader reader, SerializedType valueType)
        {
            if (valueType == SerializedType.Byte)
                return Reader.ReadByte(reader);

            TypedDeserializer.DiscardValue(reader, valueType);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadAsChar(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsChar(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char? ReadAsChar(BinaryReader reader, SerializedType valueType)
        {
            if (valueType == SerializedType.Char)
                return Reader.ReadChar(reader);

            TypedDeserializer.DiscardValue(reader, valueType);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ReadAsDateTime(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsDateTime(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime? ReadAsDateTime(BinaryReader reader, SerializedType valueType)
        {
            if (valueType == SerializedType.DateTime)
                return Reader.ReadDateTime(reader);

            TypedDeserializer.DiscardValue(reader, valueType);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadAsDecimal(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsDecimal(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal? ReadAsDecimal(BinaryReader reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.ULong:
                    return Reader.ReadULong(reader);
                case SerializedType.UInt:
                    return Reader.ReadUInt(reader);
                case SerializedType.UShort:
                    return Reader.ReadUShort(reader);
                case SerializedType.Byte:
                    return Reader.ReadByte(reader);
                case SerializedType.Long:
                    return Reader.ReadLong(reader);
                case SerializedType.Int:
                    return Reader.ReadInt(reader);
                case SerializedType.Short:
                    return Reader.ReadShort(reader);
                case SerializedType.SByte:
                    return Reader.ReadSByte(reader);
                case SerializedType.Decimal:
                    return Reader.ReadDecimal(reader);
                default:
                    TypedDeserializer.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadAsDouble(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsDouble(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double? ReadAsDouble(BinaryReader reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.Double:
                    return Reader.ReadDouble(reader);
                case SerializedType.Float:
                    return Reader.ReadFloat(reader);
                case SerializedType.ULong:
                    return Reader.ReadULong(reader);
                case SerializedType.UInt:
                    return Reader.ReadUInt(reader);
                case SerializedType.UShort:
                    return Reader.ReadUShort(reader);
                case SerializedType.Byte:
                    return Reader.ReadByte(reader);
                case SerializedType.Long:
                    return Reader.ReadLong(reader);
                case SerializedType.Int:
                    return Reader.ReadInt(reader);
                case SerializedType.Short:
                    return Reader.ReadShort(reader);
                case SerializedType.SByte:
                    return Reader.ReadSByte(reader);
                default:
                    TypedDeserializer.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadAsShort(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsShort(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short? ReadAsShort(BinaryReader reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.Byte:
                    return Reader.ReadByte(reader);
                case SerializedType.Short:
                    return Reader.ReadShort(reader);
                case SerializedType.SByte:
                    return Reader.ReadSByte(reader);
                default:
                    TypedDeserializer.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadAsInt(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsInt(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int? ReadAsInt(BinaryReader reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.UShort:
                    return Reader.ReadUShort(reader);
                case SerializedType.Byte:
                    return Reader.ReadByte(reader);
                case SerializedType.Int:
                    return Reader.ReadInt(reader);
                case SerializedType.Short:
                    return Reader.ReadShort(reader);
                case SerializedType.SByte:
                    return Reader.ReadSByte(reader);
                default:
                    TypedDeserializer.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadAsLong(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsLong(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long? ReadAsLong(BinaryReader reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.UInt:
                    return Reader.ReadUInt(reader);
                case SerializedType.UShort:
                    return Reader.ReadUShort(reader);
                case SerializedType.Byte:
                    return Reader.ReadByte(reader);
                case SerializedType.Long:
                    return Reader.ReadLong(reader);
                case SerializedType.Int:
                    return Reader.ReadInt(reader);
                case SerializedType.Short:
                    return Reader.ReadShort(reader);
                case SerializedType.SByte:
                    return Reader.ReadSByte(reader);
                default:
                    TypedDeserializer.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadAsSByte(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsSByte(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte? ReadAsSByte(BinaryReader reader, SerializedType valueType)
        {
            if (valueType == SerializedType.SByte)
                return Reader.ReadSByte(reader);

            TypedDeserializer.DiscardValue(reader, valueType);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadAsFloat(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsFloat(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float? ReadAsFloat(BinaryReader reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.Float:
                    return Reader.ReadFloat(reader);
                case SerializedType.ULong:
                    return Reader.ReadULong(reader);
                case SerializedType.UInt:
                    return Reader.ReadUInt(reader);
                case SerializedType.UShort:
                    return Reader.ReadUShort(reader);
                case SerializedType.Byte:
                    return Reader.ReadByte(reader);
                case SerializedType.Long:
                    return Reader.ReadLong(reader);
                case SerializedType.Int:
                    return Reader.ReadInt(reader);
                case SerializedType.Short:
                    return Reader.ReadShort(reader);
                case SerializedType.SByte:
                    return Reader.ReadSByte(reader);
                default:
                    TypedDeserializer.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadAsUShort(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsUShort(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort? ReadAsUShort(BinaryReader reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.UShort:
                    return Reader.ReadUShort(reader);
                case SerializedType.Byte:
                    return Reader.ReadByte(reader);
                default:
                    TypedDeserializer.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadAsUInt(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsUInt(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint? ReadAsUInt(BinaryReader reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.UInt:
                    return Reader.ReadUInt(reader);
                case SerializedType.UShort:
                    return Reader.ReadUShort(reader);
                case SerializedType.Byte:
                    return Reader.ReadByte(reader);
                default:
                    TypedDeserializer.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadAsULong(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsULong(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong? ReadAsULong(BinaryReader reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.ULong:
                    return Reader.ReadULong(reader);
                case SerializedType.UInt:
                    return Reader.ReadUInt(reader);
                case SerializedType.UShort:
                    return Reader.ReadUShort(reader);
                case SerializedType.Byte:
                    return Reader.ReadByte(reader);
                default:
                    TypedDeserializer.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadAsString(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadAsString(reader, valueType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadAsString(BinaryReader reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.String:
                    return Reader.ReadString(reader);
                case SerializedType.Char:
                    return new string(Reader.ReadChar(reader), 1);
                default:
                    TypedDeserializer.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(byte val, out object result)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Byte:
                {
                    result = val;
                    return true;
                }
                case TypeCode.Decimal:
                {
                    result = (decimal) val;
                    return true;
                }
                case TypeCode.Double:
                {
                    result = (double) val;
                    return true;
                }
                case TypeCode.Int16:
                {
                    result = (short) val;
                    return true;
                }
                case TypeCode.Int32:
                {
                    result = (int) val;
                    return true;
                }
                case TypeCode.Int64:
                {
                    result = (long) val;
                    return true;
                }
                case TypeCode.Single:
                {
                    result = (float) val;
                    return true;
                }
                case TypeCode.UInt16:
                {
                    result = (ushort) val;
                    return true;
                }
                case TypeCode.UInt32:
                {
                    result = (uint) val;
                    return true;
                }
                case TypeCode.UInt64:
                {
                    result = (ulong) val;
                    return true;
                }
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(sbyte val, out object result)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.SByte:
                    result = val;
                    return true;
                case TypeCode.Decimal:
                    result = (decimal) val;
                    return true;
                case TypeCode.Double:
                    result = (double) val;
                    return true;
                case TypeCode.Int16:
                    result = (short) val;
                    return true;
                case TypeCode.Int32:
                    result = (int) val;
                    return true;
                case TypeCode.Int64:
                    result = (long) val;
                    return true;
                case TypeCode.Single:
                    result = (float) val;
                    return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(ushort val, out object result)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.UInt16:
                    result = val;
                    return true;
                case TypeCode.Decimal:
                    result = (decimal) val;
                    return true;
                case TypeCode.Double:
                    result = (double) val;
                    return true;
                case TypeCode.Int32:
                    result = (int) val;
                    return true;
                case TypeCode.Int64:
                    result = (long) val;
                    return true;
                case TypeCode.Single:
                    result = (float) val;
                    return true;
                case TypeCode.UInt32:
                    result = (uint) val;
                    return true;
                case TypeCode.UInt64:
                    result = (ulong) val;
                    return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(short val, out object result)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int16:
                    result = val;
                    return true;
                case TypeCode.Decimal:
                    result = (decimal) val;
                    return true;
                case TypeCode.Double:
                    result = (double) val;
                    return true;
                case TypeCode.Int32:
                    result = (int) val;
                    return true;
                case TypeCode.Int64:
                    result = (long) val;
                    return true;
                case TypeCode.Single:
                    result = (float) val;
                    return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(uint val, out object result)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.UInt32:
                    result = val;
                    return true;
                case TypeCode.Decimal:
                    result = (decimal) val;
                    return true;
                case TypeCode.Double:
                    result = (double) val;
                    return true;
                case TypeCode.Int64:
                    result = (long) val;
                    return true;
                case TypeCode.Single:
                    result = (float) val;
                    return true;
                case TypeCode.UInt64:
                    result = (ulong) val;
                    return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(char val, out object result)
        {
            if (typeof(T) == typeof(char))
            {
                result = val;
                return true;
            }

            if (typeof(T) == typeof(string))
            {
                result = new string(val, 1);
                return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(DateTime val, out object result)
        {
            if (typeof(T) == typeof(DateTime))
            {
                result = val;
                return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(decimal val, out object result)
        {
            if (typeof(T) == typeof(decimal))
            {
                result = val;
                return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(bool val, out object result)
        {
            if (typeof(T) == typeof(bool))
            {
                result = val;
                return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(double val, out object result)
        {
            if (typeof(T) == typeof(double))
            {
                result = val;
                return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(float val, out object result)
        {
            if (typeof(T) == typeof(float))
            {
                result = val;
                return true;
            }

            if (typeof(T) == typeof(double))
            {
                result = (double) val;
                return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(long val, out object result)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int64:
                    result = val;
                    return true;
                case TypeCode.Decimal:
                    result = (decimal) val;
                    return true;
                case TypeCode.Double:
                    result = (double) val;
                    return true;
                case TypeCode.Single:
                    result = (float) val;
                    return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(ulong val, out object result)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.UInt64:
                    result = val;
                    return true;
                case TypeCode.Decimal:
                    result = (decimal) val;
                    return true;
                case TypeCode.Double:
                    result = (double) val;
                    return true;
                case TypeCode.Single:
                    result = (float) val;
                    return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool As<T>(int val, out object result)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Int32:
                    result = val;
                    return true;
                case TypeCode.Decimal:
                    result = (decimal) val;
                    return true;
                case TypeCode.Double:
                    result = (double) val;
                    return true;
                case TypeCode.Int64:
                    result = (long) val;
                    return true;
                case TypeCode.Single:
                    result = (float) val;
                    return true;
            }

            result = null;
            return false;
        }
    }
}