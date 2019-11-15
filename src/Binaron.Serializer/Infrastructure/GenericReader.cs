using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Creators;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;

namespace Binaron.Serializer.Infrastructure
{
    internal static class GenericReader
    {
        private static readonly ConcurrentDictionary<(Type KeyType, Type ValueType), Func<BinaryReader, object, int, (bool Success, object Result)>> DictionaryAdders = new ConcurrentDictionary<(Type KeyType, Type ValueType), Func<BinaryReader, object, int, (bool Success, object Result)>>();
        private static readonly ConcurrentDictionary<Type, Func<BinaryReader, object, (bool Success, object Result)>> ObjectAsDictionaryAdders = new ConcurrentDictionary<Type, Func<BinaryReader, object, (bool Success, object Result)>>();
        private static readonly ConcurrentDictionary<Type, Func<BinaryReader, object, int, bool, (bool Success, object Result)>> ListEnumAdders = new ConcurrentDictionary<Type, Func<BinaryReader, object, int, bool, (bool Success, object Result)>>();
        private static readonly ConcurrentDictionary<Type, Func<BinaryReader, object, bool, (bool Success, object Result)>> EnumerableEnumAdders = new ConcurrentDictionary<Type, Func<BinaryReader, object, bool, (bool Success, object Result)>>();

        private static readonly ConcurrentDictionary<(Type ParentType, (Type KeyType, Type ValueType)), GenericResultObjectCreator.Dictionary> DictionaryResultObjectCreators = new ConcurrentDictionary<(Type ParentType, (Type KeyType, Type ValueType)), GenericResultObjectCreator.Dictionary>();
        private static readonly ConcurrentDictionary<(Type ParentType, Type Type), GenericResultObjectCreator.List> ListResultObjectCreators = new ConcurrentDictionary<(Type ParentType, Type Type), GenericResultObjectCreator.List>();
        private static readonly ConcurrentDictionary<(Type ParentType, Type Type), GenericResultObjectCreator.List> EnumerableResultObjectCreators = new ConcurrentDictionary<(Type ParentType, Type Type), GenericResultObjectCreator.List>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Discard(BinaryReader reader)
        {
            while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                TypedDeserializer.DiscardValue(reader);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadEnums(BinaryReader reader, Type parentType, Type type)
        {
            var result = CreateResultObject(parentType, type);
            var addAll = GetEnumerableEnumAdder(type);
            var (success, addedResult) = addAll(reader, result, parentType.IsArray);
            if (success)
                return addedResult;

            Discard(reader);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadTypedEnumerable<T, TElement>(BinaryReader reader)
        {
            var parentType = typeof(T);
            var type = typeof(TElement);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<TElement> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsObject<TElement>(reader, valueType);
                            l.Add((TElement) v);
                        }

                        return parentType.IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                default:
                {
                    ReadTypedEnumerable(reader, parentType, type, out var result);
                    return result;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadTypedEnumerable(BinaryReader reader, Type parentType, Type type, out object enumerable)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<bool> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsBool(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.Byte:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<byte> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsByte(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.Char:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<char> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsChar(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.DateTime:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<DateTime> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsDateTime(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.Decimal:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<decimal> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsDecimal(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.Double:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<double> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsDouble(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.Int16:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<short> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsShort(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.Int32:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<int> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsInt(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.Int64:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<long> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsLong(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.SByte:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<sbyte> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsSByte(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.Single:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<float> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsFloat(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.String:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<string> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsString(reader, valueType);
                            l.Add(v);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.UInt16:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<ushort> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsUShort(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.UInt32:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<uint> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsUInt(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
                case TypeCode.UInt64:
                {
                    var result = CreateResultObject(parentType, type);
                    if (result is ICollection<ulong> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsULong(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        enumerable = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader);
                    enumerable = result;
                    return true;
                }
            }

            enumerable = null;
            return false;
        }

        private static Func<BinaryReader, object, bool, (bool Success, object Result)> GetEnumerableEnumAdder(Type type) => EnumerableEnumAdders.GetOrAdd(type, _ =>
        {
            var method = typeof(EnumerableAdder).GetMethod(nameof(EnumerableAdder.AddEnums))?.MakeGenericMethod(type) ?? throw new MissingMethodException();
            return (Func<BinaryReader, object, bool, (bool Success, object Result)>) Delegate.CreateDelegate(typeof(Func<BinaryReader, object, bool, (bool Success, object Result)>), null, method);
        });

        private static class EnumerableAdder
        {
            public static (bool Success, object Result) AddEnums<T>(BinaryReader reader, object list, bool convertToArray) where T : struct
            {
                if (!(list is ICollection<T> l)) 
                    return (false, list);

                do
                {
                    try
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var val = ReadEnum<T>(reader);
                            if (val.HasValue)
                                l.Add(val.Value);
                        }
                        break;
                    }
                    catch (InvalidCastException)
                    {
                    }
                    catch (OverflowException)
                    {
                    }
                } while (true);
            
                return (true, convertToArray ? ToArray(l) : l);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static GenericResultObjectCreator.List GetEnumerableResultObjectCreator(Type parentType, Type type) => 
            EnumerableResultObjectCreators.GetOrAdd((parentType, type), _ => new GenericResultObjectCreator.List(parentType, type));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object CreateResultObject(Type parentType, Type type) => GetEnumerableResultObjectCreator(parentType, type).Create();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Discard(BinaryReader reader, int count)
        {
            for (var i = 0; i < count; i++)
                TypedDeserializer.DiscardValue(reader);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadEnums(BinaryReader reader, Type parentType, Type type, int count)
        {
            var result = CreateResultObject(parentType, type, count);
            var addAll = GetListEnumAdder(type);
            var (success, addedResult) = addAll(reader, result, count, parentType.IsArray);
            if (success)
                return addedResult;

            Discard(reader, count);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadTypedList<T, TElement>(BinaryReader reader, int count)
        {
            var parentType = typeof(T);
            var type = typeof(TElement);
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<TElement> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsObject<TElement>(reader, valueType);
                            l.Add((TElement) v);
                        }

                        return parentType.IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                default:
                {
                    ReadTypedList(reader, parentType, type, count, out var result);
                    return result;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ReadTypedList(BinaryReader reader, Type parentType, Type type, int count, out object list)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<bool> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsBool(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.Byte:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<byte> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsByte(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.Char:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<char> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsChar(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.DateTime:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<DateTime> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsDateTime(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.Decimal:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<decimal> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsDecimal(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.Double:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<double> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsDouble(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.Int16:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<short> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsShort(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.Int32:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<int> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsInt(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.Int64:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<long> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsLong(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.SByte:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<sbyte> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsSByte(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.Single:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<float> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsFloat(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.String:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<string> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsString(reader, valueType);
                            l.Add(v);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.UInt16:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<ushort> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsUShort(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.UInt32:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<uint> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsUInt(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
                case TypeCode.UInt64:
                {
                    var result = CreateResultObject(parentType, type, count);
                    if (result is ICollection<ulong> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsULong(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        list = parentType.IsArray ? ToArray(l) : result;
                        return true;
                    }

                    Discard(reader, count);
                    list = result;
                    return true;
                }
            }

            list = null;
            return false;
        }

        private static Func<BinaryReader, object, int, bool, (bool Success, object Result)> GetListEnumAdder(Type type) => ListEnumAdders.GetOrAdd(type, _ =>
        {
            var method = typeof(ListAdder).GetMethod(nameof(ListAdder.AddEnums))?.MakeGenericMethod(type) ?? throw new MissingMethodException();
            return (Func<BinaryReader, object, int, bool, (bool Success, object Result)>) Delegate.CreateDelegate(typeof(Func<BinaryReader, object, int, bool, (bool Success, object Result)>), null, method);
        });

        private static class ListAdder
        {
            public static (bool Success, object Result) AddEnums<T>(BinaryReader reader, object list, int count, bool convertToArray) where T : struct
            {
                if (!(list is ICollection<T> l)) 
                    return (false, list);

                var i = 0;
                do
                {
                    try
                    {
                        for (; i < count; i++)
                        {
                            var val = ReadEnum<T>(reader);
                            if (val.HasValue)
                                l.Add(val.Value);
                        }
                        break;
                    }
                    catch (InvalidCastException)
                    {
                        ++i;
                    }
                    catch (OverflowException)
                    {
                        ++i;
                    }
                } while (true);
            
                return (true, convertToArray ? ToArray(l) : l);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T? ReadEnum<T>(BinaryReader reader) where T : struct
        {
            var valueType = (SerializedType) reader.Read<byte>();
            switch (valueType)
            {
                case SerializedType.Byte:
                    return (T) Enum.ToObject(typeof(T), Reader.ReadByte(reader));
                case SerializedType.SByte:
                    return (T) Enum.ToObject(typeof(T), Reader.ReadSByte(reader));
                case SerializedType.UShort:
                    return (T) Enum.ToObject(typeof(T), Reader.ReadUShort(reader));
                case SerializedType.Short:
                    return (T) Enum.ToObject(typeof(T), Reader.ReadShort(reader));
                case SerializedType.UInt:
                    return (T) Enum.ToObject(typeof(T), Reader.ReadUInt(reader));
                case SerializedType.Int:
                    return (T) Enum.ToObject(typeof(T), Reader.ReadInt(reader));
                case SerializedType.ULong:
                    return (T) Enum.ToObject(typeof(T), Reader.ReadULong(reader));
                case SerializedType.Long:
                    return (T) Enum.ToObject(typeof(T), Reader.ReadLong(reader));
                default:
                    TypedDeserializer.DiscardValue(reader, valueType);
                    return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static GenericResultObjectCreator.List GetListResultObjectCreator(Type parentType, Type type) => 
            ListResultObjectCreators.GetOrAdd((parentType, type), _ => new GenericResultObjectCreator.List(parentType, type));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object CreateResultObject(Type parentType, Type type, int count) => GetListResultObjectCreator(parentType, type).Create(ListCapacity.Clamp(count));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T[] ToArray<T>(ICollection<T> list)
        {
            if (!(list is List<T> l)) 
                return list.ToArray();

            var result = l.GetInternalArray();
            return result.Length == list.Count ? result : l.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadObjectAsDictionary(BinaryReader reader, Type parentType, Type type)
        {
            var result = CreateResultObject(parentType, (typeof(string), type));
            var addAll = GetObjectAsDictionaryAdder(type);
            var (success, addedResult) = addAll(reader, result);
            if (success)
                return addedResult;
            
            while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
            {
                reader.ReadString(); // key
                TypedDeserializer.DiscardValue(reader); // value
            }
            return result;
        }

        private static Func<BinaryReader, object, (bool Success, object Result)> GetObjectAsDictionaryAdder(Type type) => ObjectAsDictionaryAdders.GetOrAdd(type, _ =>
        {
            var method = typeof(ObjectAsDictionaryAdder).GetMethod(nameof(ObjectAsDictionaryAdder.AddAll))?.MakeGenericMethod(type) ?? throw new MissingMethodException();
            return (Func<BinaryReader, object, (bool Success, object Result)>) Delegate.CreateDelegate(typeof(Func<BinaryReader, object, (bool Success, object Result)>), null, method);
        });

        private static class ObjectAsDictionaryAdder
        {
            public static (bool Success, object Result) AddAll<TValue>(BinaryReader reader, object dictionary)
            {
                if (!(dictionary is ICollection<KeyValuePair<string, TValue>> d)) 
                    return (false, dictionary);

                Add(reader, d);

                return (true, d);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void Add<TValue>(BinaryReader reader, ICollection<KeyValuePair<string, TValue>> dictionary)
            {
                do
                {
                    try
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var key = reader.ReadString();
                            var valueType = (SerializedType) reader.Read<byte>();
                            var value = valueType != SerializedType.Null ? (TValue) TypedDeserializer.ReadValue<TValue>(reader, valueType) : default;
                            if (key != null) 
                                dictionary.Add(new KeyValuePair<string, TValue>(key, value));
                        }
                        break;
                    }
                    catch (InvalidCastException)
                    {
                    }
                    catch (OverflowException)
                    {
                    }
                } while (true);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadDictionary(BinaryReader reader, Type parentType, (Type KeyType, Type ValueType) type, int count)
        {
            var result = CreateResultObject(parentType, type, count);
            var addAll = GetDictionaryAdder(type);
            var (success, addedResult) = addAll(reader, result, count);
            if (success)
                return addedResult;
            
            for (var i = 0; i < count; i++)
            {
                TypedDeserializer.DiscardValue(reader); // key
                TypedDeserializer.DiscardValue(reader); // value
            }
            return result;
        }
        
        private static Func<BinaryReader, object, int, (bool Success, object Result)> GetDictionaryAdder((Type KeyType, Type ValueType) type) => DictionaryAdders.GetOrAdd(type, _ =>
        {
            var method = typeof(DictionaryAdder).GetMethod(nameof(DictionaryAdder.AddAll))?.MakeGenericMethod(type.KeyType, type.ValueType) ?? throw new MissingMethodException();
            return (Func<BinaryReader, object, int, (bool Success, object Result)>) Delegate.CreateDelegate(typeof(Func<BinaryReader, object, int, (bool Success, object Result)>), null, method);
        });

        private static class DictionaryAdder
        {
            public static (bool Success, object Result) AddAll<TKey, TValue>(BinaryReader reader, object dictionary, int count)
            {
                if (!(dictionary is ICollection<KeyValuePair<TKey, TValue>> d)) 
                    return (false, dictionary);

                Add(reader, count, d);

                return (true, d);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void Add<TKey, TValue>(BinaryReader reader, int count, ICollection<KeyValuePair<TKey, TValue>> dictionary)
            {
                var i = 0;
                do
                {
                    try
                    {
                        for (; i < count; i++)
                        {
                            var keyType = (SerializedType) reader.Read<byte>();
                            var key = TypedDeserializer.ReadValue<TKey>(reader, keyType);
                            var valueType = (SerializedType) reader.Read<byte>();
                            var value = valueType != SerializedType.Null ? (TValue) TypedDeserializer.ReadValue<TValue>(reader, valueType) : default;
                            if (key != null)
                                dictionary.Add(new KeyValuePair<TKey, TValue>((TKey) key, value));
                        }
                        break;
                    }
                    catch (InvalidCastException)
                    {
                        ++i;
                    }
                    catch (OverflowException)
                    {
                        ++i;
                    }
                } while (true);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static GenericResultObjectCreator.Dictionary GetDictionaryResultObjectCreator(Type parentType, (Type KeyType, Type ValueType) type) => 
            DictionaryResultObjectCreators.GetOrAdd((parentType, type), _ => new GenericResultObjectCreator.Dictionary(parentType, type));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object CreateResultObject(Type parentType, (Type KeyType, Type ValueType) type, int count) => GetDictionaryResultObjectCreator(parentType, type).Create(ListCapacity.Clamp(count));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object CreateResultObject(Type parentType, (Type KeyType, Type ValueType) type) => GetDictionaryResultObjectCreator(parentType, type).Create(0);
    }
}