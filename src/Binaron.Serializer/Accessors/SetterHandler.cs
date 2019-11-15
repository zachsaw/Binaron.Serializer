using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;
using Activator = Binaron.Serializer.Creators.Activator;

namespace Binaron.Serializer.Accessors
{
    internal static class SetterHandler
    {
        private const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.Instance;

        private static readonly ConcurrentDictionary<Type, (Func<object> Activate, IDictionary<string, IMemberSetterHandler<BinaryReader>> Setters, Type IDictionaryValueType, Type ActualType)> ActivatorsAndSetters = 
            new ConcurrentDictionary<Type, (Func<object>, IDictionary<string, IMemberSetterHandler<BinaryReader>>, Type IDictionaryValueType, Type ActualType)>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Func<object> Activate, IDictionary<string, IMemberSetterHandler<BinaryReader>> Setters, Type IDictionaryValueType, Type ActualType) GetActivatorAndSetterHandlers(Type type) => 
            ActivatorsAndSetters.GetOrAdd(type, _ => CreateActivatorsAndSetters(type));

        private static (Func<object>, IDictionary<string, IMemberSetterHandler<BinaryReader>>, Type IDictionaryValueType, Type ActualType) CreateActivatorsAndSetters(Type type)
        {
            var (keyType, valueType) = GenericType.GetIDictionaryReader(type);
            if (keyType == typeof(string))
                return (null, null, valueType, type);

            var actualType = GetActualType(type);
            return (CreateActivator(actualType), CreateSetters(actualType), null, actualType);
        }

        private static Func<object> CreateActivator(Type type) => Activator.Get(type);

        private static IDictionary<string, IMemberSetterHandler<BinaryReader>> CreateSetters(Type type)
        {
            return type.GetProperties(BindingAttr).Cast<MemberInfo>().Concat(type.GetFields(BindingAttr))
                .Select(member => CreateSetterHandler(type, member))
                .Where(handler => handler != null)
                .ToDictionary(handler => handler.MemberInfo.Name, setter => setter);
        }

        private static Type GetActualType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null) 
                type = underlyingType;

            if (!type.IsInterface)
                return type.IsAbstract ? throw new NotSupportedException($"Type '{type}' is abstract") : type;

            if (type == typeof(IDictionary) || type == typeof(ICollection) || type == typeof(IEnumerable))
                return typeof(Hashtable);

            throw new NotSupportedException($"Interface '{type}' is not supported");
        }

        private static IMemberSetterHandler<BinaryReader> CreateSetterHandler(Type type, MemberInfo member)
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

        private static IMemberSetterHandler<BinaryReader> CreateHandlerForBool(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<bool>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.BoolHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForByte(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<byte>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.ByteHandler(setter);
        }

        private static IMemberSetterHandler<BinaryReader> CreateHandlerForChar(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<char>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.CharHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForDateTime(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<DateTime>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.DateTimeHandler(setter);
        }

        private static IMemberSetterHandler<BinaryReader> CreateHandlerForDecimal(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<decimal>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.DecimalHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForDouble(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<double>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.DoubleHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForShort(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<short>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.ShortHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForInt(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<int>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.IntHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForLong(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<long>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.LongHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForSByte(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<sbyte>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.SByteHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForFloat(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<float>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.FloatHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForUShort(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<ushort>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.UShortHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForUInt(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<uint>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.UIntHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForULong(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<ulong>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.ULongHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForString(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<string>(type, prop.Name);
            return !setter.IsValid ? null : new MemberSetterHandlers.StringHandler(setter);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForObject(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<object>(type, prop.Name);
            if (!setter.IsValid)
                return null;
            
            var memberType = setter.MemberInfo.GetMemberType();
            if (memberType == typeof(object) || Nullable.GetUnderlyingType(memberType) != null)
                return new MemberSetterHandlers.ObjectHandler(setter);

            return !memberType.IsValueType
                ? (IMemberSetterHandler<BinaryReader>) System.Activator.CreateInstance(typeof(MemberSetterHandlers.ClassObjectHandler<>).MakeGenericType(memberType), setter)
                : (IMemberSetterHandler<BinaryReader>) System.Activator.CreateInstance(typeof(MemberSetterHandlers.StructObjectHandler<>).MakeGenericType(memberType), setter);
        }
    }
}