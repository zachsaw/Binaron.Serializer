using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;

namespace Binaron.Serializer.Accessors
{
    internal static class GetterHandler
    {
        private const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.Instance;

        private static readonly ConcurrentDictionary<Type, IEnumerable<IMemberGetterHandler<WriterState>>> MemberGetters = new ConcurrentDictionary<Type, IEnumerable<IMemberGetterHandler<WriterState>>>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<IMemberGetterHandler<WriterState>> GetGetterHandlers(Type type) => MemberGetters.GetOrAdd(type, _ => CreateGetters(type));
        
        private static IEnumerable<IMemberGetterHandler<WriterState>> CreateGetters(Type type) =>
            type.GetProperties(BindingAttr).Cast<MemberInfo>().Concat(type.GetFields(BindingAttr))
                .Select(member => CreateGetterHandler(type, member))
                .Where(handler => handler != null)
                .ToList(); // Enumerate _now_ so we can cache MemberGetters

        private static IMemberGetterHandler<WriterState> CreateGetterHandler(Type type, MemberInfo member)
        {
            var memberType = member.GetMemberType();

            switch (Type.GetTypeCode(memberType))
            {
                case TypeCode.Boolean:
                    return CreateHandlerForBool(type, member);
                case TypeCode.Byte:
                    return CreateHandlerForByte(type, member);
                case TypeCode.Char:
                    return CreateHandlerForChar(type, member);
                case TypeCode.DateTime:
                    return CreateHandlerForDateTime(type, member);
                case TypeCode.Decimal:
                    return CreateHandlerForDecimal(type, member);
                case TypeCode.Double:
                    return CreateHandlerForDouble(type, member);
                case TypeCode.Int16:
                    return CreateHandlerForShort(type, member);
                case TypeCode.Int32:
                    return CreateHandlerForInt(type, member);
                case TypeCode.Int64:
                    return CreateHandlerForLong(type, member);
                case TypeCode.SByte:
                    return CreateHandlerForSByte(type, member);
                case TypeCode.Single:
                    return CreateHandlerForFloat(type, member);
                case TypeCode.String:
                    return CreateHandlerForString(type, member);
                case TypeCode.UInt16:
                    return CreateHandlerForUShort(type, member);
                case TypeCode.UInt32:
                    return CreateHandlerForUInt(type, member);
                case TypeCode.UInt64:
                    return CreateHandlerForULong(type, member);
                default:
                    return CreateHandlerForObject(type, member);
            }
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForBool(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<bool>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, bool>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForByte(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<byte>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, byte>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForChar(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<char>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, char>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForDateTime(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<DateTime>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, DateTime>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForDecimal(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<decimal>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, decimal>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForDouble(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<double>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, double>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForShort(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<short>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, short>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForInt(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<int>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, int>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForLong(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<long>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, long>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForSByte(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<sbyte>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, sbyte>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }
        
        private static IMemberGetterHandler<WriterState> CreateHandlerForFloat(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<float>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, float>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForUShort(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<ushort>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, ushort>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForUInt(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<uint>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, uint>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForULong(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<ulong>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, ulong>(getter, (writer, val) =>
            {
                WriteMemberName(writer, memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForString(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<string>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, string>(getter, (writer, val) =>
            {
                if (val == null)
                {
                    WriteNull(writer, memberName);
                }
                else
                {
                    WriteMemberName(writer, memberName);
                    Writer.Write(writer, val);
                }
            });
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForObject(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<object>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<WriterState, object>(getter, (writer, val) =>
            {
                if (val == null)
                {
                    WriteNull(writer, memberName);
                }
                else
                {
                    WriteMemberName(writer, memberName);
                    Serializer.WriteNonNullValue(writer, val);
                }
            });
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