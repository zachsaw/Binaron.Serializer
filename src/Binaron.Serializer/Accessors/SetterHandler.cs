using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using Binaron.Serializer.Infrastructure;
using Activator = Binaron.Serializer.Creators.Activator;

namespace Binaron.Serializer.Accessors
{
    internal static class SetterHandler
    {
        private const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.Instance;

        private static readonly ConcurrentDictionary<Type, (Func<object> Activate, IDictionary<string, IMemberSetterHandler<BinaryReader>> Setters, Type IDictionaryValueType)> ActivatorsAndSetters = 
            new ConcurrentDictionary<Type, (Func<object>, IDictionary<string, IMemberSetterHandler<BinaryReader>>, Type IDictionaryValueType)>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Func<object> Activate, IDictionary<string, IMemberSetterHandler<BinaryReader>> Setters, Type IDictionaryValueType) GetActivatorAndSetterHandlers(Type type) => 
            ActivatorsAndSetters.GetOrAdd(type, _ => CreateActivatorsAndSetters(type));

        private static (Func<object>, IDictionary<string, IMemberSetterHandler<BinaryReader>>, Type IDictionaryValueType) CreateActivatorsAndSetters(Type type)
        {
            var (keyType, valueType) = GenericType.GetIDictionaryReader(type);
            if (keyType == typeof(string))
                return (null, null, valueType);

            var actualType = GetActualType(type);
            return (CreateActivator(actualType), CreateSetters(actualType), null);
        }

        private static Func<object> CreateActivator(Type type) => Activator.Get(type);

        private static IDictionary<string, IMemberSetterHandler<BinaryReader>> CreateSetters(Type type)
        {
            return type.GetProperties(BindingAttr).Cast<MemberInfo>().Concat(type.GetFields(BindingAttr))
                .Select(member => CreateSetterHandler(type, member))
                .Where(handler => handler != null)
                .ToDictionary(handler => handler.MemberName, setter => setter);
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
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, bool>(setter, SelfUpgradingReader.ReadAsBool);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForByte(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<byte>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, byte>(setter, SelfUpgradingReader.ReadAsByte);
        }

        private static IMemberSetterHandler<BinaryReader> CreateHandlerForChar(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<char>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, char>(setter, SelfUpgradingReader.ReadAsChar);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForDateTime(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<DateTime>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, DateTime>(setter, SelfUpgradingReader.ReadAsDateTime);
        }

        private static IMemberSetterHandler<BinaryReader> CreateHandlerForDecimal(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<decimal>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, decimal>(setter, SelfUpgradingReader.ReadAsDecimal);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForDouble(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<double>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, double>(setter, SelfUpgradingReader.ReadAsDouble);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForShort(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<short>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, short>(setter, SelfUpgradingReader.ReadAsShort);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForInt(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<int>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, int>(setter, SelfUpgradingReader.ReadAsInt);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForLong(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<long>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, long>(setter, SelfUpgradingReader.ReadAsLong);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForSByte(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<sbyte>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, sbyte>(setter, SelfUpgradingReader.ReadAsSByte);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForFloat(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<float>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, float>(setter, SelfUpgradingReader.ReadAsFloat);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForUShort(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<ushort>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, ushort>(setter, SelfUpgradingReader.ReadAsUShort);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForUInt(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<uint>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, uint>(setter, SelfUpgradingReader.ReadAsUInt);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForULong(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<ulong>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, ulong>(setter, SelfUpgradingReader.ReadAsULong);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForString(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<string>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            return new MemberSetterHandler<BinaryReader, string>(setter, SelfUpgradingReader.ReadAsString);
        }
        
        private static IMemberSetterHandler<BinaryReader> CreateHandlerForObject(Type type, MemberInfo prop)
        {
            var setter = new MemberSetter<object>(type, prop.Name);
            if (!setter.IsValid)
                return null;

            var memberType = prop.GetMemberType();
            return new MemberSetterHandler<BinaryReader, object>(setter, reader =>
            {
                var valueType = (SerializedType) reader.Read<byte>();
                switch (valueType)
                {
                    case SerializedType.Null:
                        return null;
                    case SerializedType.Object:
                        return TypedDeserializer.ReadObject(reader, memberType);
                    case SerializedType.Dictionary:
                        return TypedDeserializer.ReadDictionary(reader, memberType);
                    case SerializedType.List:
                        return TypedDeserializer.ReadList(reader, memberType);
                    case SerializedType.Enumerable:
                        return TypedDeserializer.ReadEnumerable(reader, memberType);
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
            });
        }
    }
}