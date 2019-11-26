using System;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;

namespace Binaron.Serializer.Accessors
{
    internal static class MemberSetterHandlers
    {
        internal sealed class BoolHandler : MemberSetterHandlerBase<BinaryReader, bool>
        {
            public BoolHandler(MemberSetter<bool> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override bool HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsBool(reader);
        }

        internal sealed class ByteHandler : MemberSetterHandlerBase<BinaryReader, byte>
        {
            public ByteHandler(MemberSetter<byte> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override byte HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsByte(reader);
        }

        internal sealed class CharHandler : MemberSetterHandlerBase<BinaryReader, char>
        {
            public CharHandler(MemberSetter<char> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override char HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsChar(reader);
        }

        internal sealed class DateTimeHandler : MemberSetterHandlerBase<BinaryReader, DateTime>
        {
            public DateTimeHandler(MemberSetter<DateTime> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override DateTime HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsDateTime(reader);
        }

        internal sealed class DecimalHandler : MemberSetterHandlerBase<BinaryReader, decimal>
        {
            public DecimalHandler(MemberSetter<decimal> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override decimal HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsDecimal(reader);
        }

        internal sealed class DoubleHandler : MemberSetterHandlerBase<BinaryReader, double>
        {
            public DoubleHandler(MemberSetter<double> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override double HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsDouble(reader);
        }

        internal sealed class ShortHandler : MemberSetterHandlerBase<BinaryReader, short>
        {
            public ShortHandler(MemberSetter<short> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override short HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsShort(reader);
        }

        internal sealed class IntHandler : MemberSetterHandlerBase<BinaryReader, int>
        {
            public IntHandler(MemberSetter<int> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override int HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsInt(reader);
        }

        internal sealed class LongHandler : MemberSetterHandlerBase<BinaryReader, long>
        {
            public LongHandler(MemberSetter<long> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override long HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsLong(reader);
        }

        internal sealed class SByteHandler : MemberSetterHandlerBase<BinaryReader, sbyte>
        {
            public SByteHandler(MemberSetter<sbyte> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override sbyte HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsSByte(reader);
        }

        internal sealed class FloatHandler : MemberSetterHandlerBase<BinaryReader, float>
        {
            public FloatHandler(MemberSetter<float> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override float HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsFloat(reader);
        }

        internal sealed class UShortHandler : MemberSetterHandlerBase<BinaryReader, ushort>
        {
            public UShortHandler(MemberSetter<ushort> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override ushort HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsUShort(reader);
        }

        internal sealed class UIntHandler : MemberSetterHandlerBase<BinaryReader, uint>
        {
            public UIntHandler(MemberSetter<uint> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override uint HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsUInt(reader);
        }

        internal sealed class ULongHandler : MemberSetterHandlerBase<BinaryReader, ulong>
        {
            public ULongHandler(MemberSetter<ulong> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override ulong HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsULong(reader);
        }

        internal sealed class StringHandler : MemberSetterHandlerBase<BinaryReader, string>
        {
            public StringHandler(MemberSetter<string> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override string HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsString(reader);
        }

        internal sealed class StructObjectHandler<T> : MemberSetterHandlerBase<BinaryReader, object> where T : struct
        {
            public StructObjectHandler(MemberSetter<object> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override object HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsObject<T>(reader);
        }

        internal sealed class ClassObjectHandler<T> : MemberSetterHandlerBase<BinaryReader, object> where T : class
        {
            public ClassObjectHandler(MemberSetter<object> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override object HandleInternal(BinaryReader reader) => SelfUpgradingReader.ReadAsObject<T>(reader);
        }

        internal sealed class ObjectHandler : MemberSetterHandlerBase<BinaryReader, object>
        {
            private readonly IHandler handler;

            public ObjectHandler(MemberSetter<object> setter) : base(setter)
            {
                var memberType = setter.MemberInfo.GetMemberType();
                handler = (IHandler) Activator.CreateInstance(typeof(Handler<>).MakeGenericType(memberType));
            }

            private interface IHandler
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                object Handle(BinaryReader reader);
            }

            private sealed class Handler<T> : IHandler
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Handle(BinaryReader reader)
                {
                    var valueType = (SerializedType) reader.Read<byte>();
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
                        case SerializedType.String:
                            return Reader.ReadString(reader);
                        case SerializedType.Char:
                            return Reader.ReadChar(reader);
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
                        case SerializedType.Decimal:
                            return Reader.ReadDecimal(reader);
                        case SerializedType.Bool:
                            return Reader.ReadBool(reader);
                        case SerializedType.DateTime:
                            return Reader.ReadDateTime(reader);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override object HandleInternal(BinaryReader reader) => handler.Handle(reader);
        }
    }
}