using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Infrastructure;

namespace Binaron.Serializer.Accessors
{
    internal static class MemberGetterHandlers
    {
        internal sealed class BoolHandler : MemberGetterHandlerBase<WriterState, bool>
        {
            public BoolHandler(MemberGetter<bool> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, bool val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class ByteHandler : MemberGetterHandlerBase<WriterState, byte>
        {
            public ByteHandler(MemberGetter<byte> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, byte val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class CharHandler : MemberGetterHandlerBase<WriterState, char>
        {
            public CharHandler(MemberGetter<char> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, char val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class DateTimeHandler : MemberGetterHandlerBase<WriterState, DateTime>
        {
            public DateTimeHandler(MemberGetter<DateTime> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, DateTime val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class DecimalHandler : MemberGetterHandlerBase<WriterState, decimal>
        {
            public DecimalHandler(MemberGetter<decimal> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, decimal val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class DoubleHandler : MemberGetterHandlerBase<WriterState, double>
        {
            public DoubleHandler(MemberGetter<double> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, double val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class ShortHandler : MemberGetterHandlerBase<WriterState, short>
        {
            public ShortHandler(MemberGetter<short> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, short val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class IntHandler : MemberGetterHandlerBase<WriterState, int>
        {
            public IntHandler(MemberGetter<int> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, int val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class LongHandler : MemberGetterHandlerBase<WriterState, long>
        {
            public LongHandler(MemberGetter<long> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, long val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class SByteHandler : MemberGetterHandlerBase<WriterState, sbyte>
        {
            public SByteHandler(MemberGetter<sbyte> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, sbyte val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class FloatHandler : MemberGetterHandlerBase<WriterState, float>
        {
            public FloatHandler(MemberGetter<float> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, float val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class UShortHandler : MemberGetterHandlerBase<WriterState, ushort>
        {
            public UShortHandler(MemberGetter<ushort> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, ushort val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class UIntHandler : MemberGetterHandlerBase<WriterState, uint>
        {
            public UIntHandler(MemberGetter<uint> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, uint val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class ULongHandler : MemberGetterHandlerBase<WriterState, ulong>
        {
            public ULongHandler(MemberGetter<ulong> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, ulong val)
            {
                await WriteMemberName(writer, MemberName);
                await Writer.Write(writer, val);
            }
        }

        internal sealed class StringHandler : MemberGetterHandlerBase<WriterState, string>
        {
            public StringHandler(MemberGetter<string> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, string val)
            {
                if (val == null)
                {
                    await WriteNull(writer, MemberName);
                }
                else
                {
                    await WriteMemberName(writer, MemberName);
                    await Writer.Write(writer, val);
                }
            }
        }

        internal sealed class ObjectHandler : MemberGetterHandlerBase<WriterState, object>
        {
            public ObjectHandler(MemberGetter<object> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, object val)
            {
                if (val == null)
                {
                    await WriteNull(writer, MemberName);
                }
                else
                {
                    await WriteMemberName(writer, MemberName);
                    await Serializer.WriteNonNullValue(writer, val);
                }
            }
        }

        internal sealed class TypedObjectHandler<T> : MemberGetterHandlerBase<WriterState, object>
        {
            public TypedObjectHandler(MemberGetter<object> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override async ValueTask HandleInternal(WriterState writer, object val)
            {
                if (val == null)
                {
                    await WriteNull(writer, MemberName);
                }
                else
                {
                    await WriteMemberName(writer, MemberName);
                    await Serializer.WriteNonPrimitive(writer, (T) val);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteNull(WriterState writer, string memberName)
        {
            if (writer.SkipNullValues)
                return;

            await WriteMemberName(writer, memberName);
            await writer.Write((byte) SerializedType.Null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static async ValueTask WriteMemberName(WriterState writer, string memberName)
        {
            await writer.Write((byte) EnumerableType.HasItem);
            await writer.WriteString(memberName);
        }
    }
}