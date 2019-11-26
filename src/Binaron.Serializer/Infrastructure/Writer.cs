using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Binaron.Serializer.Enums;
using Binaron.Serializer.IeeeDecimal;

namespace Binaron.Serializer.Infrastructure
{
    internal static class Writer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, int val)
        {
            await writer.Write((byte) SerializedType.Int);
            await writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, long val)
        {
            await writer.Write((byte) SerializedType.Long);
            await writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, short val)
        {
            await writer.Write((byte) SerializedType.Short);
            await writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, double val)
        {
            await writer.Write((byte) SerializedType.Double);
            await writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, float val)
        {
            await writer.Write((byte) SerializedType.Float);
            await writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, decimal val)
        {
            // Decimals are stored as IEEE 754-2008 Decimal128 format https://en.wikipedia.org/wiki/Decimal128_floating-point_format
            // The IEEE version has higher precision than .net's decimal implementation and is compatible with other platforms
            await writer.Write((byte) SerializedType.Decimal);
            var d = new Decimal128(val);
            await writer.Write(d.GetIeeeHighBits());
            await writer.Write(d.GetIeeeLowBits());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, bool val)
        {
            await writer.Write((byte) SerializedType.Bool);
            await writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, byte val)
        {
            await writer.Write((byte) SerializedType.Byte);
            await writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, sbyte val)
        {
            await writer.Write((byte) SerializedType.SByte);
            await writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, char val)
        {
            await writer.Write((byte) SerializedType.Char);
            await writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, DateTime val)
        {
            await writer.Write((byte) SerializedType.DateTime);
            await writer.Write(val.ToUniversalTime().Ticks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, ushort val)
        {
            await writer.Write((byte) SerializedType.UShort);
            await writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, uint val)
        {
            await writer.Write((byte) SerializedType.UInt);
            await writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, ulong val)
        {
            await writer.Write((byte) SerializedType.ULong);
            await writer.Write(val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async ValueTask Write(WriterState writer, string val)
        {
            await writer.Write((byte) SerializedType.String);
            await writer.WriteString(val);
        }
    }
}