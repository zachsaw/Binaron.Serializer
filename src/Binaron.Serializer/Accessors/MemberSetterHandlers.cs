using System;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;

namespace Binaron.Serializer.Accessors
{
    internal static class MemberSetterHandlers
    {
        internal class BoolHandler : MemberSetterHandlerBase<ReaderState, bool>
        {
            public BoolHandler(MemberSetter<bool> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override bool HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsBool(reader);
        }

        internal class ByteHandler : MemberSetterHandlerBase<ReaderState, byte>
        {
            public ByteHandler(MemberSetter<byte> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override byte HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsByte(reader);
        }

        internal class CharHandler : MemberSetterHandlerBase<ReaderState, char>
        {
            public CharHandler(MemberSetter<char> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override char HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsChar(reader);
        }

        internal class DateTimeHandler : MemberSetterHandlerBase<ReaderState, DateTime>
        {
            public DateTimeHandler(MemberSetter<DateTime> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override DateTime HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsDateTime(reader);
        }

        internal class DecimalHandler : MemberSetterHandlerBase<ReaderState, decimal>
        {
            public DecimalHandler(MemberSetter<decimal> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override decimal HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsDecimal(reader);
        }

        internal class DoubleHandler : MemberSetterHandlerBase<ReaderState, double>
        {
            public DoubleHandler(MemberSetter<double> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override double HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsDouble(reader);
        }

        internal class ShortHandler : MemberSetterHandlerBase<ReaderState, short>
        {
            public ShortHandler(MemberSetter<short> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override short HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsShort(reader);
        }

        internal class IntHandler : MemberSetterHandlerBase<ReaderState, int>
        {
            public IntHandler(MemberSetter<int> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override int HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsInt(reader);
        }

        internal class LongHandler : MemberSetterHandlerBase<ReaderState, long>
        {
            public LongHandler(MemberSetter<long> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override long HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsLong(reader);
        }

        internal class SByteHandler : MemberSetterHandlerBase<ReaderState, sbyte>
        {
            public SByteHandler(MemberSetter<sbyte> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override sbyte HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsSByte(reader);
        }

        internal class FloatHandler : MemberSetterHandlerBase<ReaderState, float>
        {
            public FloatHandler(MemberSetter<float> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override float HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsFloat(reader);
        }

        internal class UShortHandler : MemberSetterHandlerBase<ReaderState, ushort>
        {
            public UShortHandler(MemberSetter<ushort> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override ushort HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsUShort(reader);
        }

        internal class UIntHandler : MemberSetterHandlerBase<ReaderState, uint>
        {
            public UIntHandler(MemberSetter<uint> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override uint HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsUInt(reader);
        }

        internal class ULongHandler : MemberSetterHandlerBase<ReaderState, ulong>
        {
            public ULongHandler(MemberSetter<ulong> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override ulong HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsULong(reader);
        }

        internal class StringHandler : MemberSetterHandlerBase<ReaderState, string>
        {
            public StringHandler(MemberSetter<string> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override string HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsString(reader);
        }

        internal class StructObjectHandler<T> : MemberSetterHandlerBase<ReaderState, object> where T : struct
        {
            public StructObjectHandler(MemberSetter<object> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override object HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsObject<T>(reader);
        }

        internal class ClassObjectHandler<T> : MemberSetterHandlerBase<ReaderState, object> where T : class
        {
            public ClassObjectHandler(MemberSetter<object> setter) : base(setter)
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override object HandleInternal(ReaderState reader) => SelfUpgradingReader.ReadAsObject<T>(reader);
        }

        internal class ObjectHandler : MemberSetterHandlerBase<ReaderState, object>
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
                object Handle(ReaderState reader);
            }

            private class Handler<T> : IHandler
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Handle(ReaderState reader)
                {
                    var valueType = (SerializedType) reader.Read<byte>();
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
            protected override object HandleInternal(ReaderState reader) => handler.Handle(reader);
        }
    }
}