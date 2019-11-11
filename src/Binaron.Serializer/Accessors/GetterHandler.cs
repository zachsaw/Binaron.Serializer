using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;
using BinaryWriter = Binaron.Serializer.Infrastructure.BinaryWriter;

namespace Binaron.Serializer.Accessors
{
    internal static class GetterHandler
    {
        private const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.Instance;

        private static readonly ConcurrentDictionary<Type, IEnumerable<IMemberGetterHandler<BinaryWriter>>> MemberGetters = new ConcurrentDictionary<Type, IEnumerable<IMemberGetterHandler<BinaryWriter>>>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<IMemberGetterHandler<BinaryWriter>> GetGetterHandlers(Type type) => MemberGetters.GetOrAdd(type, _ => CreateGetters(type));
        
        private static IEnumerable<IMemberGetterHandler<BinaryWriter>> CreateGetters(Type type) =>
            type.GetProperties(BindingAttr).Cast<MemberInfo>().Concat(type.GetFields(BindingAttr))
                .Select(member => CreateGetterHandler(type, member))
                .Where(handler => handler != null)
                .ToList(); // Enumerate _now_ so we can cache MemberGetters

        private static IMemberGetterHandler<BinaryWriter> CreateGetterHandler(Type type, MemberInfo member)
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

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForBool(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<bool>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, bool>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForByte(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<byte>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, byte>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForChar(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<char>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, char>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForDateTime(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<DateTime>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, DateTime>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForDecimal(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<decimal>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, decimal>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForDouble(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<double>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, double>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForShort(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<short>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, short>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForInt(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<int>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, int>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForLong(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<long>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, long>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForSByte(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<sbyte>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, sbyte>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }
        
        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForFloat(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<float>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, float>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForUShort(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<ushort>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, ushort>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForUInt(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<uint>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, uint>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForULong(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<ulong>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, ulong>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForString(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<string>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, string>(getter, (writer, val) =>
            {
                if (val == null)
                    return;

                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Writer.Write(writer, val);
            });
        }

        private static IMemberGetterHandler<BinaryWriter> CreateHandlerForObject(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<object>(type, prop.Name);
            if (!getter.IsValid)
                return null;

            var memberName = getter.MemberName;
            return new MemberGetterHandler<BinaryWriter, object>(getter, (writer, val) =>
            {
                writer.Write((byte) EnumerableType.HasItem);
                writer.WriteString(memberName);
                Serializer.WriteValue(writer, val);
            });
        }
    }
}