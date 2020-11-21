using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Binaron.Serializer.Creators;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Extensions;
using TypeCode = Binaron.Serializer.Enums.TypeCode;

namespace Binaron.Serializer.Infrastructure
{
    internal static class GenericReader
    {
        private static readonly ConcurrentDictionary<(Type KeyType, Type ValueType), Func<ReaderState, object, int, (bool Success, object Result)>> DictionaryAdders = new ConcurrentDictionary<(Type KeyType, Type ValueType), Func<ReaderState, object, int, (bool Success, object Result)>>();
        private static readonly ConcurrentDictionary<Type, Func<ReaderState, object, (bool Success, object Result)>> ObjectAsDictionaryAdders = new ConcurrentDictionary<Type, Func<ReaderState, object, (bool Success, object Result)>>();
        private static readonly ConcurrentDictionary<(Type ParentType, (Type KeyType, Type ValueType)), GenericResultObjectCreator.Dictionary> DictionaryResultObjectCreators = new ConcurrentDictionary<(Type ParentType, (Type KeyType, Type ValueType)), GenericResultObjectCreator.Dictionary>();

        private static void Discard(ReaderState reader)
        {
            while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                TypedDeserializer.DiscardValue(reader);
        }
        
        public static object ReadEnums<T, TElement>(ReaderState reader)
        {
            var result = CreateResultObject<T, TElement>();
            var addAll = EnumerableEnumAdder<TElement>.Adder;
            var (success, addedResult) = addAll(reader, result, typeof(T).IsArray);
            if (success)
                return addedResult;

            Discard(reader);
            return result;
        }

        public static object ReadTypedEnumerable<T, TElement>(ReaderState reader)
        {
            switch (TypeOf<TElement>.TypeCode)
            {
                case TypeCode.Object:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<TElement> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsObject<TElement>(reader, valueType);
                            l.Add((TElement) v);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.Boolean:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<bool> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsBool(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.Byte:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<byte> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsByte(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.Char:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<char> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsChar(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.DateTime:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<DateTime> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsDateTime(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.Guid:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<Guid> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsGuid(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.Decimal:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<decimal> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsDecimal(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.Double:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<double> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsDouble(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.Int16:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<short> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsShort(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.Int32:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<int> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsInt(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.Int64:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<long> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsLong(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.SByte:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<sbyte> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsSByte(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.Single:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<float> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsFloat(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.String:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<string> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsString(reader, valueType);
                            l.Add(v);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.UInt16:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<ushort> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsUShort(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.UInt32:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<uint> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsUInt(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
                case TypeCode.UInt64:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<ulong> l)
                    {
                        while ((EnumerableType) reader.Read<byte>() == EnumerableType.HasItem)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsULong(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader);
                    return result;
                }
            }

            return null;
        }

        private static class EnumerableEnumAdder<T>
        {
            public static readonly Func<ReaderState, object, bool, (bool Success, object Result)> Adder = CreateAdder(typeof(T));
        }

        private static Func<ReaderState,object,bool,(bool Success, object Result)> CreateAdder(Type type)
        {
            var method = typeof(EnumerableAdder).GetMethod(nameof(EnumerableAdder.AddEnums))?.MakeGenericMethod(type) ?? throw new MissingMethodException();
            return (Func<ReaderState, object, bool, (bool Success, object Result)>) Delegate.CreateDelegate(typeof(Func<ReaderState, object, bool, (bool Success, object Result)>), null, method);
        }

        private static class EnumerableAdder
        {
            public static (bool Success, object Result) AddEnums<T>(ReaderState reader, object list, bool convertToArray) where T : struct
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

        private static class EnumerableResultObjectCreator<T, TElement>
        {
            public static readonly GenericResultObjectCreator.List Creator = new GenericResultObjectCreator.List(typeof(T), typeof(TElement));
        }

        private static object CreateResultObject<T, TElement>() => EnumerableResultObjectCreator<T, TElement>.Creator.Create();

        private static void Discard(ReaderState reader, int count)
        {
            for (var i = 0; i < count; i++)
                TypedDeserializer.DiscardValue(reader);
        }
        
        public static object ReadEnums<T, TElement>(ReaderState reader, int count)
        {
            var result = CreateResultObject<T, TElement>(count);
            var addAll = ListEnumAdder<TElement>.Adder;
            var (success, addedResult) = addAll(reader, result, count, typeof(T).IsArray);
            if (success)
                return addedResult;

            Discard(reader, count);
            return result;
        }

        public static object ReadTypedList<T, TElement>(ReaderState reader, int count)
        {
            switch (TypeOf<TElement>.TypeCode)
            {
                case TypeCode.Object:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<TElement> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsObject<TElement>(reader, valueType);
                            l.Add((TElement) v);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.Boolean:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<bool> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsBool(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.Byte:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<byte> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsByte(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.Char:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<char> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsChar(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.DateTime:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<DateTime> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsDateTime(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.Guid:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<Guid> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsGuid(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.Decimal:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<decimal> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsDecimal(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.Double:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<double> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsDouble(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.Int16:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<short> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsShort(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.Int32:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<int> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsInt(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.Int64:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<long> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsLong(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.SByte:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<sbyte> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsSByte(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.Single:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<float> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsFloat(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.String:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<string> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsString(reader, valueType);
                            l.Add(v);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.UInt16:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<ushort> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsUShort(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.UInt32:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<uint> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsUInt(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
                case TypeCode.UInt64:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<ulong> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = (SerializedType) reader.Read<byte>();
                            var v = SelfUpgradingReader.ReadAsULong(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discard(reader, count);
                    return result;
                }
            }

            return null;
        }

        private static class ListEnumAdder<T>
        {
            public static Func<ReaderState, object, int, bool, (bool Success, object Result)> Adder = CreateEnumAdder(typeof(T));
        }

        private static Func<ReaderState, object, int, bool, (bool Success, object Result)> CreateEnumAdder(Type type)
        {
            var method = typeof(ListAdder).GetMethod(nameof(ListAdder.AddEnums))?.MakeGenericMethod(type) ?? throw new MissingMethodException();
            return (Func<ReaderState, object, int, bool, (bool Success, object Result)>) Delegate.CreateDelegate(typeof(Func<ReaderState, object, int, bool, (bool Success, object Result)>), null, method);
        }

        private static class ListAdder
        {
            public static (bool Success, object Result) AddEnums<T>(ReaderState reader, object list, int count, bool convertToArray) where T : struct
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
        
        private static T? ReadEnum<T>(ReaderState reader) where T : struct
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
        
        private static class ListResultObjectCreator<T, TElement>
        {
            public static readonly GenericResultObjectCreator.List Creator = new GenericResultObjectCreator.List(typeof(T), typeof(TElement));
        }

        private static object CreateResultObject<T, TElement>(int count) => ListResultObjectCreator<T, TElement>.Creator.Create(ListCapacity.Clamp(count));

        private static T[] ToArray<T>(ICollection<T> list)
        {
            if (!(list is List<T> l)) 
                return list.ToArray();

            var result = l.GetInternalArray();
            return result.Length == list.Count ? result : l.ToArray();
        }

        public static object ReadObjectAsDictionary(ReaderState reader, Type parentType, Type type)
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

        private static Func<ReaderState, object, (bool Success, object Result)> GetObjectAsDictionaryAdder(Type type) => ObjectAsDictionaryAdders.GetOrAdd(type, _ =>
        {
            var method = typeof(ObjectAsDictionaryAdder).GetMethod(nameof(ObjectAsDictionaryAdder.AddAll))?.MakeGenericMethod(type) ?? throw new MissingMethodException();
            return (Func<ReaderState, object, (bool Success, object Result)>) Delegate.CreateDelegate(typeof(Func<ReaderState, object, (bool Success, object Result)>), null, method);
        });

        private static class ObjectAsDictionaryAdder
        {
            public static (bool Success, object Result) AddAll<TValue>(ReaderState reader, object dictionary)
            {
                if (!(dictionary is ICollection<KeyValuePair<string, TValue>> d)) 
                    return (false, dictionary);

                Add(reader, d);

                return (true, d);
            }

            private static void Add<TValue>(ReaderState reader, ICollection<KeyValuePair<string, TValue>> dictionary)
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

        public static object ReadDictionary(ReaderState reader, Type parentType, (Type KeyType, Type ValueType) type, int count)
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
        
        private static Func<ReaderState, object, int, (bool Success, object Result)> GetDictionaryAdder((Type KeyType, Type ValueType) type) => DictionaryAdders.GetOrAdd(type, _ =>
        {
            var method = typeof(DictionaryAdder).GetMethod(nameof(DictionaryAdder.AddAll))?.MakeGenericMethod(type.KeyType, type.ValueType) ?? throw new MissingMethodException();
            return (Func<ReaderState, object, int, (bool Success, object Result)>) Delegate.CreateDelegate(typeof(Func<ReaderState, object, int, (bool Success, object Result)>), null, method);
        });

        private static class DictionaryAdder
        {
            public static (bool Success, object Result) AddAll<TKey, TValue>(ReaderState reader, object dictionary, int count)
            {
                if (!(dictionary is ICollection<KeyValuePair<TKey, TValue>> d)) 
                    return (false, dictionary);

                Add(reader, count, d);

                return (true, d);
            }

            private static void Add<TKey, TValue>(ReaderState reader, int count, ICollection<KeyValuePair<TKey, TValue>> dictionary)
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
        
        private static GenericResultObjectCreator.Dictionary GetDictionaryResultObjectCreator(Type parentType, (Type KeyType, Type ValueType) type) => 
            DictionaryResultObjectCreators.GetOrAdd((parentType, type), _ => new GenericResultObjectCreator.Dictionary(parentType, type));

        private static object CreateResultObject(Type parentType, (Type KeyType, Type ValueType) type, int count) => GetDictionaryResultObjectCreator(parentType, type).Create(ListCapacity.Clamp(count));
        
        private static object CreateResultObject(Type parentType, (Type KeyType, Type ValueType) type) => GetDictionaryResultObjectCreator(parentType, type).Create(0);
    }
}