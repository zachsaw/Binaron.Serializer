using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;

namespace Binaron.Serializer.Accessors
{
    internal static class GetterHandler
    {
        private static readonly ConcurrentDictionary<Type, IMemberGetterHandler<WriterState>[]> MemberGetters = new ConcurrentDictionary<Type, IMemberGetterHandler<WriterState>[]>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IMemberGetterHandler<WriterState>[] GetGetterHandlers(Type type) => MemberGetters.GetOrAdd(type, _ => CreateGetters(type));

        public static class GetterHandlers<T>
        {
            public static readonly IMemberGetterHandler<WriterState>[] Getters = GetGetterHandlers(typeof(T));
        }
        
        private static IMemberGetterHandler<WriterState>[] CreateGetters(Type type) =>
            type.GetMemberInfos()
                .Select(member => CreateGetterHandler(type, member))
                .Where(handler => handler != null)
                .OrderBy(GetHandlerOrder)
                .ToArray(); // Enumerate _now_ so we can cache MemberGetters

        public static int GetFieldOffset(this FieldInfo fi) => GetFieldOffset(fi.FieldHandle);

        public static int GetFieldOffset(RuntimeFieldHandle h) => Marshal.ReadInt32(h.Value + (4 + IntPtr.Size)) & 0xFFFFFF;

        private static int GetHandlerOrder(IMemberGetterHandler<WriterState> handler)
        {
            var memberType = handler.MemberInfo.GetMemberType();
            if (handler.MemberInfo is FieldInfo fi)
            {
                switch (Type.GetTypeCode(memberType))
                {
                    case TypeCode.Object:
                        return int.MaxValue;
                    default:
                        return int.MinValue + GetFieldOffset(fi);
                }
            }

            switch (Type.GetTypeCode(memberType))
            {
                case TypeCode.Object:
                    return int.MaxValue;
                default:
                    return 0;
            }
        }

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
            return !getter.IsValid ? null : new MemberGetterHandlers.BoolHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForByte(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<byte>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.ByteHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForChar(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<char>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.CharHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForDateTime(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<DateTime>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.DateTimeHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForDecimal(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<decimal>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.DecimalHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForDouble(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<double>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.DoubleHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForShort(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<short>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.ShortHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForInt(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<int>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.IntHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForLong(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<long>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.LongHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForSByte(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<sbyte>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.SByteHandler(getter);
        }
        
        private static IMemberGetterHandler<WriterState> CreateHandlerForFloat(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<float>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.FloatHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForUShort(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<ushort>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.UShortHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForUInt(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<uint>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.UIntHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForULong(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<ulong>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.ULongHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForString(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<string>(type, prop.Name);
            return !getter.IsValid ? null : new MemberGetterHandlers.StringHandler(getter);
        }

        private static IMemberGetterHandler<WriterState> CreateHandlerForObject(Type type, MemberInfo prop)
        {
            var getter = new MemberGetter<object>(type, prop.Name);
            return (IMemberGetterHandler<WriterState>)
                (!getter.IsValid
                    ? null
                    : Activator.CreateInstance(typeof(MemberGetterHandlers.ObjectHandler<>).MakeGenericType(getter.MemberInfo.GetMemberType()), getter));
        }
    }
}