using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using TypeCode = Binaron.Serializer.Enums.TypeCode;

namespace Binaron.Serializer.Infrastructure
{
    internal static class SelfUpgradingReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadAsObject<T>(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsObject<T>(reader, valueType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadAsObject<T>(ReaderState reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.Null:
                    return null;
                case SerializedType.CustomObject:
                    var identifier = Deserializer.ReadValue(reader);
                    return TypedDeserializer.ReadObject<T>(reader, identifier);
                case SerializedType.Object:
                    return TypedDeserializer.ReadObject<T>(reader);
                case SerializedType.Dictionary:
                    return TypedDeserializer.ReadDictionary<T>(reader);
                case SerializedType.List:
                    return TypedDeserializer.ReadList<T>(reader);
                case SerializedType.HList:
                    return TypedDeserializer.ReadHList<T>(reader);
                case SerializedType.Enumerable:
                    return TypedDeserializer.ReadEnumerable<T>(reader);
                case SerializedType.HEnumerable:
                    return TypedDeserializer.ReadHEnumerable<T>(reader);
                default:
                    return ReadAsMiscObject<T>(reader, valueType);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ReadAsMiscObject<T>(ReaderState reader, SerializedType valueType)
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
                return TypeOf<TFrom>.TypeCode switch
                {
                    TypeCode.Boolean => As<TTo>((bool) val, out var result) ? result : null,
                    TypeCode.Byte => As<TTo>((byte) val, out var result) ? result : null,
                    TypeCode.Char => As<TTo>((char) val, out var result) ? result : null,
                    TypeCode.DateTime => As<TTo>((DateTime) val, out var result) ? result : null,
                    TypeCode.Guid => As<TTo>((Guid) val, out var result) ? result : null,
                    TypeCode.Decimal => As<TTo>((decimal) val, out var result) ? result : null,
                    TypeCode.Double => As<TTo>((double) val, out var result) ? result : null,
                    TypeCode.Int16 => As<TTo>((short) val, out var result) ? result : null,
                    TypeCode.Int32 => As<TTo>((int) val, out var result) ? result : null,
                    TypeCode.Int64 => As<TTo>((long) val, out var result) ? result : null,
                    TypeCode.SByte => As<TTo>((sbyte) val, out var result) ? result : null,
                    TypeCode.Single => As<TTo>((float) val, out var result) ? result : null,
                    TypeCode.UInt16 => As<TTo>((ushort) val, out var result) ? result : null,
                    TypeCode.UInt32 => As<TTo>((uint) val, out var result) ? result : null,
                    TypeCode.UInt64 => As<TTo>((ulong) val, out var result) ? result : null,
                    _ => null
                };
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadAsBool(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsBool(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool? ReadAsBool(ReaderState reader, SerializedType valueType)
        {
            if (valueType == SerializedType.Bool)
                return Reader.ReadBool(reader);

            Discarder.DiscardValue(reader, valueType);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadAsByte(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsByte(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte? ReadAsByte(ReaderState reader, SerializedType valueType)
        {
            if (valueType == SerializedType.Byte)
                return Reader.ReadByte(reader);

            Discarder.DiscardValue(reader, valueType);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadAsChar(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsChar(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char? ReadAsChar(ReaderState reader, SerializedType valueType)
        {
            if (valueType == SerializedType.Char)
                return Reader.ReadChar(reader);

            Discarder.DiscardValue(reader, valueType);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ReadAsDateTime(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsDateTime(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid ReadAsGuid(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsGuid(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime? ReadAsDateTime(ReaderState reader, SerializedType valueType)
        {
            if (valueType == SerializedType.DateTime)
                return Reader.ReadDateTime(reader);

            Discarder.DiscardValue(reader, valueType);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Guid? ReadAsGuid(ReaderState reader, SerializedType valueType)
        {
            if (valueType == SerializedType.Guid)
                return Reader.ReadGuid(reader);

            Discarder.DiscardValue(reader, valueType);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadAsDecimal(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsDecimal(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal? ReadAsDecimal(ReaderState reader, SerializedType valueType)
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
                    Discarder.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadAsDouble(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsDouble(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double? ReadAsDouble(ReaderState reader, SerializedType valueType)
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
                    Discarder.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadAsShort(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsShort(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short? ReadAsShort(ReaderState reader, SerializedType valueType)
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
                    Discarder.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadAsInt(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsInt(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int? ReadAsInt(ReaderState reader, SerializedType valueType)
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
                    Discarder.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadAsLong(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsLong(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long? ReadAsLong(ReaderState reader, SerializedType valueType)
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
                    Discarder.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadAsSByte(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsSByte(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte? ReadAsSByte(ReaderState reader, SerializedType valueType)
        {
            if (valueType == SerializedType.SByte)
                return Reader.ReadSByte(reader);

            Discarder.DiscardValue(reader, valueType);
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadAsFloat(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsFloat(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float? ReadAsFloat(ReaderState reader, SerializedType valueType)
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
                    Discarder.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadAsUShort(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsUShort(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort? ReadAsUShort(ReaderState reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.UShort:
                    return Reader.ReadUShort(reader);
                case SerializedType.Byte:
                    return Reader.ReadByte(reader);
                default:
                    Discarder.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadAsUInt(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsUInt(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint? ReadAsUInt(ReaderState reader, SerializedType valueType)
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
                    Discarder.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadAsULong(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsULong(reader, valueType) ?? default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong? ReadAsULong(ReaderState reader, SerializedType valueType)
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
                    Discarder.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadAsString(ReaderState reader)
        {
            var valueType = Reader.ReadSerializedType(reader);
            return ReadAsString(reader, valueType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadAsString(ReaderState reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.String:
                    return Reader.ReadString(reader);
                case SerializedType.Char:
                    return new string(Reader.ReadChar(reader), 1);
                case SerializedType.ULong:
                    return Reader.ReadULong(reader).ToString(reader.CultureInfo);
                case SerializedType.UInt:
                    return Reader.ReadUInt(reader).ToString(reader.CultureInfo);
                case SerializedType.UShort:
                    return Reader.ReadUShort(reader).ToString(reader.CultureInfo);
                case SerializedType.Byte:
                    return Reader.ReadByte(reader).ToString(reader.CultureInfo);
                case SerializedType.Long:
                    return Reader.ReadLong(reader).ToString(reader.CultureInfo);
                case SerializedType.Int:
                    return Reader.ReadInt(reader).ToString(reader.CultureInfo);
                case SerializedType.Short:
                    return Reader.ReadShort(reader).ToString(reader.CultureInfo);
                case SerializedType.SByte:
                    return Reader.ReadSByte(reader).ToString(reader.CultureInfo);
                case SerializedType.Decimal:
                    return Reader.ReadDecimal(reader).ToString(reader.CultureInfo);
                case SerializedType.Bool:
                    return Reader.ReadBool(reader).ToString(reader.CultureInfo);
                case SerializedType.Float:
                    return Reader.ReadFloat(reader).ToString(reader.CultureInfo);
                case SerializedType.Double:
                    return Reader.ReadDouble(reader).ToString(reader.CultureInfo);
                case SerializedType.DateTime:
                    return Reader.ReadDateTime(reader).ToString(reader.CultureInfo);
                case SerializedType.Guid:
                    return Reader.ReadGuid(reader).ToString();
                default:
                    Discarder.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool As<T>(byte val, out object result)
        {
            switch (TypeOf<T>.TypeCode)
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
        private static bool As<T>(sbyte val, out object result)
        {
            switch (TypeOf<T>.TypeCode)
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
        private static bool As<T>(ushort val, out object result)
        {
            switch (TypeOf<T>.TypeCode)
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
        private static bool As<T>(short val, out object result)
        {
            switch (TypeOf<T>.TypeCode)
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
        private static bool As<T>(uint val, out object result)
        {
            switch (TypeOf<T>.TypeCode)
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
        private static bool As<T>(char val, out object result)
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
        private static bool As<T>(DateTime val, out object result)
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
        private static bool As<T>(Guid val, out object result)
        {
            if (typeof(T) == typeof(Guid))
            {
                result = val;
                return true;
            }

            result = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool As<T>(decimal val, out object result)
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
        private static bool As<T>(bool val, out object result)
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
        private static bool As<T>(double val, out object result)
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
        private static bool As<T>(float val, out object result)
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
        private static bool As<T>(long val, out object result)
        {
            switch (TypeOf<T>.TypeCode)
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
        private static bool As<T>(ulong val, out object result)
        {
            switch (TypeOf<T>.TypeCode)
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
        private static bool As<T>(int val, out object result)
        {
            switch (TypeOf<T>.TypeCode)
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