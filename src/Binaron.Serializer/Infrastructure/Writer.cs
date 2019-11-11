using System;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Enums;
using Binaron.Serializer.IeeeDecimal;

namespace Binaron.Serializer.Infrastructure
{
    internal static class Writer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, int val)
        {
            writer.Write((byte) SerializedType.Int);
            writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, long val)
        {
            writer.Write((byte) SerializedType.Long);
            writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, short val)
        {
            writer.Write((byte) SerializedType.Short);
            writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, double val)
        {
            writer.Write((byte) SerializedType.Double);
            writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, float val)
        {
            writer.Write((byte) SerializedType.Float);
            writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, decimal val)
        {
            // Decimals are stored as IEEE 754-2008 Decimal128 format https://en.wikipedia.org/wiki/Decimal128_floating-point_format
            // The IEEE version has higher precision than .net's decimal implementation and is compatible with other platforms
            writer.Write((byte) SerializedType.Decimal);
            var d = new Decimal128(val);
            writer.Write(d.GetIeeeHighBits());
            writer.Write(d.GetIeeeLowBits());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, bool val)
        {
            writer.Write((byte) SerializedType.Bool);
            writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, byte val)
        {
            writer.Write((byte) SerializedType.Byte);
            writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, sbyte val)
        {
            writer.Write((byte) SerializedType.SByte);
            writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, char val)
        {
            writer.Write((byte) SerializedType.Char);
            writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, DateTime val)
        {
            writer.Write((byte) SerializedType.DateTime);
            writer.Write(val.ToUniversalTime().Ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, ushort val)
        {
            writer.Write((byte) SerializedType.UShort);
            writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, uint val)
        {
            writer.Write((byte) SerializedType.UInt);
            writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, ulong val)
        {
            writer.Write((byte) SerializedType.ULong);
            writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(BinaryWriter writer, string val)
        {
            writer.Write((byte) SerializedType.String);
            writer.WriteString(val);
        }
    }
}