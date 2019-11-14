using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Accessors;
using Binaron.Serializer.Creators;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Infrastructure;

namespace Binaron.Serializer
{
    internal static class TypedDeserializer
    {
        private static readonly ConcurrentDictionary<Type, ResultObjectCreator.List> ListResultObjectCreators = new ConcurrentDictionary<Type, ResultObjectCreator.List>();
        private static readonly ConcurrentDictionary<Type, ResultObjectCreator.Enumerable> EnumerableResultObjectCreators = new ConcurrentDictionary<Type, ResultObjectCreator.Enumerable>();
        private static readonly ConcurrentDictionary<Type, ResultObjectCreator.Dictionary> DictionaryResultObjectCreators = new ConcurrentDictionary<Type, ResultObjectCreator.Dictionary>();

        public interface IObjectReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            object Read(BinaryReader reader);
        }

        private static class GetObjectReaderGeneric<T>
        {
            public static readonly IObjectReader Reader = ObjectReaders.CreateReader<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadObject<T>(BinaryReader reader) => GetObjectReaderGeneric<T>.Reader.Read(reader);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadObject(BinaryReader reader, Type type)
        {
            if (type == typeof(object))
                return Deserializer.ReadObject(reader);

            var typeInfo = SetterHandler.GetActivatorAndSetterHandlers(type);
            var valueType = typeInfo.IDictionaryValueType;
            if (valueType != null)
                return GenericReader.ReadObjectAsDictionary(reader, type, valueType);
            
            var setterHandlers = typeInfo.Setters;
            var result = typeInfo.Activate();

            if (result is IDictionary d)
            {
                while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                {
                    var key = reader.ReadString();
                    var value = Deserializer.ReadValue(reader);
                    d.Add(key, value);
                }
                return result;
            }

            while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
            {
                var key = reader.ReadString();
                if (!setterHandlers.TryGetValue(key, out var setter))
                {
                    DiscardValue(reader);
                    continue;
                }

                setter.Handle(reader, result);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object Default<T>() => default(T);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object AsEnum<T>(object val)
        {
            try
            {
                return (T) val;
            }
            catch (InvalidCastException)
            {
                return Default<T>();
            }
        }

        public static object ReadValue<T>(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadValue<T>(reader, valueType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadValue<T>(BinaryReader reader, SerializedType valueType)
        {
            switch (valueType)
            {
                case SerializedType.Object:
                    return ReadObject<T>(reader);
                case SerializedType.Dictionary:
                    return ReadDictionary<T>(reader);
                case SerializedType.List:
                    return ReadList<T>(reader);
                case SerializedType.Enumerable:
                    return ReadEnumerable<T>(reader);
                case SerializedType.String:
                {
                    var val = Reader.ReadString(reader);
                    if (typeof(T) == typeof(string))
                        return val;
                    return typeof(T) == typeof(object) ? val : null;
                }
                case SerializedType.Byte:
                {
                    var val = Reader.ReadByte(reader);
                    if (typeof(T).IsEnum)
                        return AsEnum<T>(val);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.SByte:
                {
                    var val = Reader.ReadSByte(reader);
                    if (typeof(T).IsEnum)
                        return AsEnum<T>(val);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.UShort:
                {
                    var val = Reader.ReadUShort(reader);
                    if (typeof(T).IsEnum)
                        return AsEnum<T>(val);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.Short:
                {
                    var val = Reader.ReadShort(reader);
                    if (typeof(T).IsEnum)
                        return AsEnum<T>(val);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.UInt:
                {
                    var val = Reader.ReadUInt(reader);
                    if (typeof(T).IsEnum)
                        return AsEnum<T>(val);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.Int:
                {
                    var val = Reader.ReadInt(reader);
                    if (typeof(T).IsEnum)
                        return AsEnum<T>(val);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.ULong:
                {
                    var val = Reader.ReadULong(reader);
                    if (typeof(T).IsEnum)
                        return AsEnum<T>(val);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.Long:
                {
                    var val = Reader.ReadLong(reader);
                    if (typeof(T).IsEnum)
                        return AsEnum<T>(val);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.Float:
                {
                    var val = Reader.ReadFloat(reader);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.Double:
                {
                    var val = Reader.ReadDouble(reader);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.Bool:
                {
                    var val = Reader.ReadBool(reader);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.Decimal:
                {
                    var val = Reader.ReadDecimal(reader);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.DateTime:
                {
                    var val = Reader.ReadDateTime(reader);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.Char:
                {
                    var val = Reader.ReadChar(reader);
                    if (SelfUpgradingReader.As<T>(val, out var result)) 
                        return result;
                    return typeof(T) == typeof(object) ? val : GetNullableOrDefault<T>(val);
                }
                case SerializedType.Null:
                    return null;
                default:
                    throw new NotSupportedException($"SerializedType '{valueType}' is not supported");
            }
        }

        private static readonly ConcurrentDictionary<(Type, Type), Func<object, object>> Upgraders = new ConcurrentDictionary<(Type, Type), Func<object, object>>();
        private static Func<object, object> GetUpgrader(Type from, Type to) => Upgraders.GetOrAdd((from, to), _ =>
        {
            var method = typeof(Upgrader).GetMethod(nameof(Upgrader.Upgrade))?.MakeGenericMethod(from, to) ?? throw new MissingMethodException();
            return (Func<object, object>) Delegate.CreateDelegate(typeof(Func<object, object>), null, method);
        });

        private static class Upgrader
        {
            public static object Upgrade<TFrom, TTo>(object val)
            {
                switch (Type.GetTypeCode(typeof(TFrom)))
                {
                    case TypeCode.Boolean:
                    {
                        return SelfUpgradingReader.As<TTo>((bool) val, out var result) ? result : null;
                    }
                    case TypeCode.Byte:
                    {
                        return SelfUpgradingReader.As<TTo>((byte) val, out var result) ? result : null;
                    }
                    case TypeCode.Char:
                    {
                        return SelfUpgradingReader.As<TTo>((char) val, out var result) ? result : null;
                    }
                    case TypeCode.DateTime:
                    {
                        return SelfUpgradingReader.As<TTo>((DateTime) val, out var result) ? result : null;
                    }
                    case TypeCode.Decimal:
                    {
                        return SelfUpgradingReader.As<TTo>((decimal) val, out var result) ? result : null;
                    }
                    case TypeCode.Double:
                    {
                        return SelfUpgradingReader.As<TTo>((double) val, out var result) ? result : null;
                    }
                    case TypeCode.Int16:
                    {
                        return SelfUpgradingReader.As<TTo>((short) val, out var result) ? result : null;
                    }
                    case TypeCode.Int32:
                    {
                        return SelfUpgradingReader.As<TTo>((int) val, out var result) ? result : null;
                    }
                    case TypeCode.Int64:
                    {
                        return SelfUpgradingReader.As<TTo>((long) val, out var result) ? result : null;
                    }
                    case TypeCode.SByte:
                    {
                        return SelfUpgradingReader.As<TTo>((sbyte) val, out var result) ? result : null;
                    }
                    case TypeCode.Single:
                    {
                        return SelfUpgradingReader.As<TTo>((float) val, out var result) ? result : null;
                    }
                    case TypeCode.UInt16:
                    {
                        return SelfUpgradingReader.As<TTo>((ushort) val, out var result) ? result : null;
                    }
                    case TypeCode.UInt32:
                    {
                        return SelfUpgradingReader.As<TTo>((uint) val, out var result) ? result : null;
                    }
                    case TypeCode.UInt64:
                    {
                        return SelfUpgradingReader.As<TTo>((ulong) val, out var result) ? result : null;
                    }
                    default:
                        return null;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object GetNullableOrDefault<T>(object val)
        {
            var type = Nullable.GetUnderlyingType(typeof(T));
            if (type == null)
                return Default<T>();

            if (type.IsEnum)
                return Enum.ToObject(type, val);

            return Upgrade(type, val);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object Upgrade(Type targetType, object val)
        {
            var sourceType = val.GetType();
            return sourceType == targetType ? val : GetUpgrader(sourceType, targetType)(val);
        }

        public static void DiscardValue(BinaryReader reader, SerializedType? knownType = null)
        {
            var valueType = knownType ?? (SerializedType) reader.Read<byte>();
            switch (valueType)
            {
                case SerializedType.Object: 
                    while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem) 
                    { 
                        reader.ReadString(); // key 
                        DiscardValue(reader); // value 
                    } 
                    break; 
                case SerializedType.Dictionary: 
                    var kvpCount = reader.Read<int>(); 
                    for (var i = 0; i < kvpCount; i++) 
                    { 
                        DiscardValue(reader); // key 
                        DiscardValue(reader); // value 
                    } 
                    break; 
                case SerializedType.List: 
                    var count = reader.Read<int>(); 
                    for (var i = 0; i < count; i++) 
                        DiscardValue(reader); 
                    break; 
                case SerializedType.Enumerable: 
                    while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem) 
                        DiscardValue(reader); 
                    break;    
                case SerializedType.String:
                    Reader.ReadString(reader);
                    break;
                case SerializedType.Byte:
                    Reader.ReadByte(reader);
                    break;
                case SerializedType.SByte:
                    Reader.ReadSByte(reader);
                    break;
                case SerializedType.UShort:
                    Reader.ReadUShort(reader);
                    break;
                case SerializedType.Short:
                    Reader.ReadShort(reader);
                    break;
                case SerializedType.UInt:
                    Reader.ReadUInt(reader);
                    break;
                case SerializedType.Int:
                    Reader.ReadInt(reader);
                    break;
                case SerializedType.ULong:
                    Reader.ReadULong(reader);
                    break;
                case SerializedType.Long:
                    Reader.ReadLong(reader);
                    break;
                case SerializedType.Float:
                    Reader.ReadFloat(reader);
                    break;
                case SerializedType.Double:
                    Reader.ReadDouble(reader);
                    break;
                case SerializedType.Bool:
                    Reader.ReadBool(reader);
                    break;
                case SerializedType.Decimal:
                    Reader.ReadDecimal(reader);
                    break;
                case SerializedType.DateTime:
                    Reader.ReadDateTime(reader);
                    break;
                case SerializedType.Char:
                    Reader.ReadChar(reader);
                    break;
                case SerializedType.Null:
                    break;
                default:
                    throw new NotSupportedException($"SerializedType '{valueType}' is not supported");
            }
        }

        public static object ReadDictionary<T>(BinaryReader reader) => ReadDictionary(reader, typeof(T), GenericType.GetIDictionaryReaderGenericTypes<T>.Types);

        public static object ReadDictionary(BinaryReader reader, Type type) => ReadDictionary(reader, type, GenericType.GetIDictionaryReader(type));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ReadDictionary(BinaryReader reader, Type type, (Type KeyType, Type ValueType) dictionaryGenericType)
        {
            if (dictionaryGenericType.KeyType != null)
            {
                var count = reader.Read<int>();
                return GenericReader.ReadDictionary(reader, type, dictionaryGenericType, count);
            }

            if (type == typeof(object))
                return Deserializer.ReadDictionary(reader);

            // non-generic type
            {
                var count = reader.Read<int>();
                var result = CreateDictionaryResultObject(type, count);
                switch (result)
                {
                    case IDictionary d:
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var key = Deserializer.ReadValue(reader);
                            var value = Deserializer.ReadValue(reader);
                            d.Add(key, value);
                        }

                        break;
                    }
                    default:
                    {
                        for (var i = 0; i < count; i++)
                        {
                            DiscardValue(reader); // key
                            DiscardValue(reader); // value
                        }

                        break;
                    }
                }

                return result;
            }
        }

        public static object ReadEnumerable<T>(BinaryReader reader) => ReadEnumerable(reader, typeof(T), GenericType.GetIEnumerableGenericType<T>.Type);

        public static object ReadEnumerable(BinaryReader reader, Type type) => ReadEnumerable(reader, type, GenericType.GetIEnumerable(type));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ReadEnumerable(BinaryReader reader, Type type, Type elementType)
        {
            if (elementType != null)
                return GenericReader.ReadEnumerable(reader, type, elementType);

            if (type == typeof(object))
                return Deserializer.ReadEnumerable(reader);

            // non-generic type
            var result = CreateResultObject(type);
            if (result is IList l)
            {
                while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                    l.Add(Deserializer.ReadValue(reader));
            }
            else
            {
                while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                    DiscardValue(reader);
            }

            return result;
        }

        public static object ReadList<T>(BinaryReader reader) => ReadList(reader, typeof(T), GenericType.GetIEnumerableGenericType<T>.Type);

        public static object ReadList(BinaryReader reader, Type type) => ReadList(reader, type, GenericType.GetIEnumerable(type));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ReadList(BinaryReader reader, Type type, Type elementType)
        {
            if (elementType != null)
            {
                var count = reader.Read<int>();
                return GenericReader.ReadList(reader, type, elementType, count);
            }

            if (type == typeof(object))
                return Deserializer.ReadList(reader);

            // non-generic type
            {
                var count = reader.Read<int>();
                var result = CreateResultObject(type, count);
                if (result is IList l)
                {
                    for (var i = 0; i < count; i++)
                        l.Add(Deserializer.ReadValue(reader));
                }
                else
                {
                    for (var i = 0; i < count; i++)
                        DiscardValue(reader);
                }

                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ResultObjectCreator.Enumerable GetEnumerableResultObjectCreator(Type type) => EnumerableResultObjectCreators.GetOrAdd(type, _ => new ResultObjectCreator.Enumerable(type));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object CreateResultObject(Type type) => GetEnumerableResultObjectCreator(type).Create();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ResultObjectCreator.List GetListResultObjectCreator(Type type) => ListResultObjectCreators.GetOrAdd(type, _ => new ResultObjectCreator.List(type));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object CreateResultObject(Type type, int count) => GetListResultObjectCreator(type).Create(ListCapacity.Clamp(count));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ResultObjectCreator.Dictionary GetDictionaryResultObjectCreator(Type type) => DictionaryResultObjectCreators.GetOrAdd(type, _ => new ResultObjectCreator.Dictionary(type));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object CreateDictionaryResultObject(Type type, int count) => GetDictionaryResultObjectCreator(type).Create(ListCapacity.Clamp(count));

        private static class ObjectReaders
        {
            public static IObjectReader CreateReader<T>()
            {
                var type = typeof(T);
                if (type == typeof(object))
                    return new DynamicObjectReader();

                var typeInfo = SetterHandler.GetActivatorAndSetterHandlers(type);
                var valueType = typeInfo.IDictionaryValueType;
                if (valueType != null)
                    return new GenericDictionaryReader(type, valueType);

                if (typeof(IDictionary).IsAssignableFrom(typeInfo.ActualType))
                    return new DictionaryReader(typeInfo.Activate);

                return new ObjectReader(typeInfo.Activate, typeInfo.Setters);
            }

            private class ObjectReader : IObjectReader
            {
                private readonly Func<object> activate;
                private readonly IDictionary<string, IMemberSetterHandler<BinaryReader>> setterHandlers;

                public ObjectReader(Func<object> activate, IDictionary<string, IMemberSetterHandler<BinaryReader>> setterHandlers)
                {
                    this.activate = activate;
                    this.setterHandlers = setterHandlers;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader)
                {
                    var result = activate();
                    while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                    {
                        var key = reader.ReadString();
                        if (!setterHandlers.TryGetValue(key, out var setter))
                        {
                            DiscardValue(reader);
                            continue;
                        }

                        setter.Handle(reader, result);
                    }
                    return result;
                }
            }

            private class DictionaryReader : IObjectReader
            {
                private readonly Func<object> activate;

                public DictionaryReader(Func<object> activate)
                {
                    this.activate = activate;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader)
                {
                    var result = (IDictionary) activate();
                    while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                    {
                        var key = reader.ReadString();
                        var value = Deserializer.ReadValue(reader);
                        result.Add(key, value);
                    }
                    return result;
                }
            }

            private class GenericDictionaryReader : IObjectReader
            {
                private readonly Type type;
                private readonly Type elementType;

                public GenericDictionaryReader(Type type, Type elementType)
                {
                    this.type = type;
                    this.elementType = elementType;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader) => GenericReader.ReadObjectAsDictionary(reader, type, elementType);
            }

            private class DynamicObjectReader : IObjectReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader) => Deserializer.ReadObject(reader);
            }
        }
    }
}