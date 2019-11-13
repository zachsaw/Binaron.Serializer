using System;
using System.Runtime.CompilerServices;
using Binaron.Serializer.IeeeDecimal;

namespace Binaron.Serializer.Infrastructure
{
    internal static class Reader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ReadDecimal(BinaryReader reader)
        {
            // Decimals are stored as IEEE 754-2008 Decimal128 format https://en.wikipedia.org/wiki/Decimal128_floating-point_format
            // The IEEE version has higher precision than .net's decimal implementation and is compatible with other platforms
            // assumes little-endian order
            var highBits = reader.Read<ulong>();
            var lowBits = reader.Read<ulong>();
            return (decimal) Decimal128.FromIeeeBits(highBits, lowBits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char ReadChar(BinaryReader reader)
        {
            return reader.Read<char>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime ReadDateTime(BinaryReader reader)
        {
            var ticks = reader.Read<long>();
            return new DateTime(ticks, DateTimeKind.Utc);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReadString(BinaryReader reader)
        {
            return reader.ReadString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ReadByte(BinaryReader reader)
        {
            return reader.Read<byte>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadSByte(BinaryReader reader)
        {
            return reader.Read<sbyte>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort ReadUShort(BinaryReader reader)
        {
            return reader.Read<ushort>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short ReadShort(BinaryReader reader)
        {
            return reader.Read<short>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadUInt(BinaryReader reader)
        {
            return reader.Read<uint>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadInt(BinaryReader reader)
        {
            return reader.Read<int>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReadULong(BinaryReader reader)
        {
            return reader.Read<ulong>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ReadLong(BinaryReader reader)
        {
            return reader.Read<long>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ReadFloat(BinaryReader reader)
        {
            return reader.Read<float>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ReadDouble(BinaryReader reader)
        {
            return reader.Read<double>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadBool(BinaryReader reader)
        {
            return reader.Read<bool>();
        }
    }
}