using System;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Infrastructure;

namespace Binaron.Serializer.Accessors
{
    internal static class MemberGetterHandlers
    {
        internal class BoolHandler : MemberGetterHandlerBase<WriterState, bool>
        {
            public BoolHandler(MemberGetter<bool> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, bool val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class ByteHandler : MemberGetterHandlerBase<WriterState, byte>
        {
            public ByteHandler(MemberGetter<byte> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, byte val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class CharHandler : MemberGetterHandlerBase<WriterState, char>
        {
            public CharHandler(MemberGetter<char> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, char val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class DateTimeHandler : MemberGetterHandlerBase<WriterState, DateTime>
        {
            public DateTimeHandler(MemberGetter<DateTime> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, DateTime val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class GuidHandler : MemberGetterHandlerBase<WriterState, Guid>
        {
            public GuidHandler(MemberGetter<Guid> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, Guid val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class DecimalHandler : MemberGetterHandlerBase<WriterState, decimal>
        {
            public DecimalHandler(MemberGetter<decimal> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, decimal val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class DoubleHandler : MemberGetterHandlerBase<WriterState, double>
        {
            public DoubleHandler(MemberGetter<double> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, double val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class ShortHandler : MemberGetterHandlerBase<WriterState, short>
        {
            public ShortHandler(MemberGetter<short> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, short val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class IntHandler : MemberGetterHandlerBase<WriterState, int>
        {
            public IntHandler(MemberGetter<int> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, int val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class LongHandler : MemberGetterHandlerBase<WriterState, long>
        {
            public LongHandler(MemberGetter<long> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, long val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class SByteHandler : MemberGetterHandlerBase<WriterState, sbyte>
        {
            public SByteHandler(MemberGetter<sbyte> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, sbyte val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class FloatHandler : MemberGetterHandlerBase<WriterState, float>
        {
            public FloatHandler(MemberGetter<float> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, float val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class UShortHandler : MemberGetterHandlerBase<WriterState, ushort>
        {
            public UShortHandler(MemberGetter<ushort> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, ushort val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class UIntHandler : MemberGetterHandlerBase<WriterState, uint>
        {
            public UIntHandler(MemberGetter<uint> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, uint val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class ULongHandler : MemberGetterHandlerBase<WriterState, ulong>
        {
            public ULongHandler(MemberGetter<ulong> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, ulong val)
            {
                WriteMemberName(writer, MemberName);
                Writer.Write(writer, val);
            }
        }

        internal class StringHandler : MemberGetterHandlerBase<WriterState, string>
        {
            public StringHandler(MemberGetter<string> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, string val)
            {
                if (val == null)
                {
                    WriteNull(writer, MemberName);
                }
                else
                {
                    WriteMemberName(writer, MemberName);
                    Writer.Write(writer, val);
                }
            }
        }

        internal class ObjectHandler : MemberGetterHandlerBase<WriterState, object>
        {
            public ObjectHandler(MemberGetter<object> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, object val)
            {
                if (val == null)
                {
                    WriteNull(writer, MemberName);
                }
                else
                {
                    WriteMemberName(writer, MemberName);
                    Serializer.WriteNonNullValue(writer, val);
                }
            }
        }

        internal class TypedObjectHandler<T> : MemberGetterHandlerBase<WriterState, object>
        {
            public TypedObjectHandler(MemberGetter<object> getter) : base(getter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void HandleInternal(WriterState writer, object val)
            {
                if (val == null)
                {
                    WriteNull(writer, MemberName);
                }
                else
                {
                    WriteMemberName(writer, MemberName);
                    Serializer.WriteNonPrimitive(writer, (T) val);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteNull(WriterState writer, string memberName)
        {
            if (writer.SkipNullValues)
                return;

            WriteMemberName(writer, memberName);
            writer.Write((byte) SerializedType.Null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void WriteMemberName(WriterState writer, string memberName)
        {
            writer.Write((byte) EnumerableType.HasItem);
            writer.WriteString(memberName);
        }
    }
}