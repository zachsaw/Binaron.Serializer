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

        public static object ReadEnums<T, TElement>(ReaderState reader)
        {
            var result = CreateResultObject<T, TElement>();
            var addAll = EnumerableEnumAdder<TElement>.Adder;
            var (success, addedResult) = addAll(reader, result, typeof(T).IsArray);
            if (success)
                return addedResult;

            Discarder.Discard(reader);
            return result;
        }

        public static object ReadHEnums<T, TElement>(ReaderState reader)
        {
            var result = CreateResultObject<T, TElement>();
            var valueType = Reader.ReadSerializedType(reader);
            var addAll = HEnumerableEnumAdder<TElement>.Adder;
            var (success, addedResult) = addAll(reader, result, valueType, typeof(T).IsArray);
            if (success)
                return addedResult;

            Discarder.Discard(reader);
            return result;
        }

        public static object ReadHEnumerable<T, TElement>(ReaderState reader)
        {
            switch (TypeOf<TElement>.TypeCode)
            {
                case TypeCode.Object:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<TElement> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var v = SelfUpgradingReader.ReadAsObject<TElement>(reader, valueType);
                                l.Add((TElement)v);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<TElement> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<TElement>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var v = SelfUpgradingReader.ReadAsObject<TElement>(reader, valueType);
                                l1.Add((TElement)v);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Boolean:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<bool> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Bool)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadBool(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsBool(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<bool> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<bool>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Bool)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadBool(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsBool(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Byte:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<byte> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Byte)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadByte(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsByte(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<byte> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<byte>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Byte)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadByte(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsByte(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }
                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Char:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<char> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Char)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadChar(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsChar(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<char> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<char>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Char)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadChar(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsChar(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.DateTime:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<DateTime> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.DateTime)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadDateTime(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsDateTime(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<DateTime> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<DateTime>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.DateTime)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadDateTime(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsDateTime(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Guid:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<Guid> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Guid)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadGuid(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsGuid(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<Guid> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<Guid>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Guid)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadGuid(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsGuid(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Decimal:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<decimal> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Decimal)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadDecimal(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsDecimal(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<decimal> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<decimal>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Decimal)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadDecimal(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsDecimal(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Double:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<double> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Double)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadDouble(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsDouble(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<double> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<double>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Double)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadDouble(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsDouble(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Int16:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<short> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Short)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadShort(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsShort(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<short> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<short>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Short)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadShort(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsShort(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Int32:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<int> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Int)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadInt(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsInt(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<int> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<int>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Int)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadInt(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsInt(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Int64:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<long> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Long)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadLong(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsLong(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<long> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<long>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Long)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadLong(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsLong(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.SByte:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<sbyte> l)
                    {
                        var valueType = Reader.ReadSerializedType(reader);
                        if (valueType == SerializedType.SByte)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                l.Add(Reader.ReadSByte(reader));
                        }
                        else
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var v = SelfUpgradingReader.ReadAsSByte(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }
                    else if (result is IEnumerable<sbyte> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<sbyte>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.SByte)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadSByte(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsSByte(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                    return result;
                }
                case TypeCode.Single:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<float> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Float)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadFloat(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsFloat(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<float> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<float>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.Float)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadFloat(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsFloat(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.String:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<string> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.String)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadString(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsString(reader, valueType);
                                    l.Add(v);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<string> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<string>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.String)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadString(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsString(reader, valueType);
                                    l1.Add(v);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.UInt16:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<ushort> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.UShort)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadUShort(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsUShort(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<ushort> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<ushort>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.UShort)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadUShort(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsUShort(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.UInt32:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<uint> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.UInt)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadUInt(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsUInt(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<uint> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<uint>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.UInt)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadUInt(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsUInt(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.UInt64:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<ulong> l)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.ULong)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l.Add(Reader.ReadULong(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsULong(reader, valueType);
                                    if (v.HasValue)
                                        l.Add(v.Value);
                                }
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;

                        }
                        else if (result is IEnumerable<ulong> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<ulong>(e1);
                            var valueType = Reader.ReadSerializedType(reader);
                            if (valueType == SerializedType.ULong)
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                    l1.Add(Reader.ReadULong(reader));
                            }
                            else
                            {
                                while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                                {
                                    var v = SelfUpgradingReader.ReadAsULong(reader, valueType);
                                    if (v.HasValue)
                                        l1.Add(v.Value);
                                }
                            }

                            return result;
                        }


                        Discarder.Discard(reader);
                        return result;
                    }
            }

            return null;
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
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsObject<TElement>(reader, valueType);
                                l.Add((TElement)v);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<TElement> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<TElement>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsObject<TElement>(reader, valueType);
                                l1.Add((TElement)v);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Boolean:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<bool> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsBool(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<bool> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<bool>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsBool(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Byte:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<byte> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsByte(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<byte> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<byte>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsByte(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Char:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<char> l)
                    {
                        while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsChar(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }
                    else if (result is IEnumerable<char> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<char>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsChar(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                    return result;
                }
                case TypeCode.DateTime:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<DateTime> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsDateTime(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<DateTime> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<DateTime>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsDateTime(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Guid:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<Guid> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsGuid(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<Guid> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<Guid>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsGuid(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Decimal:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<decimal> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsDecimal(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<decimal> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<decimal>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsDecimal(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Double:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<double> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsDouble(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<double> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<double>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsDouble(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Int16:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<short> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsShort(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<short> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<short>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsShort(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Int32:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<int> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsInt(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<int> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<int>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsInt(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Int64:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<long> l)
                    {
                        while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsLong(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }
                    else if (result is IEnumerable<long> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<long>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsLong(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                    return result;
                }
                case TypeCode.SByte:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<sbyte> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsSByte(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<sbyte> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<sbyte>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsSByte(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.Single:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<float> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsFloat(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<float> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<float>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsFloat(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.String:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<string> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsString(reader, valueType);
                                l.Add(v);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<string> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<string>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsString(reader, valueType);
                                l1.Add(v);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.UInt16:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<ushort> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsUShort(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<ushort> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<ushort>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsUShort(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
                case TypeCode.UInt32:
                {
                    var result = CreateResultObject<T, TElement>();
                    if (result is ICollection<uint> l)
                    {
                        while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsUInt(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }
                    else if (result is IEnumerable<uint> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<uint>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsUInt(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                    return result;
                }
                case TypeCode.UInt64:
                    {
                        var result = CreateResultObject<T, TElement>();
                        if (result is ICollection<ulong> l)
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsULong(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }

                            return typeof(T).IsArray ? ToArray(l) : result;
                        }
                        else if (result is IEnumerable<ulong> e1)
                        {
                            var l1 = new EnumerableWrapperWithAdd<ulong>(e1);
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var valueType = Reader.ReadSerializedType(reader);
                                var v = SelfUpgradingReader.ReadAsULong(reader, valueType);
                                if (v.HasValue)
                                    l1.Add(v.Value);
                            }

                            return result;
                        }

                        Discarder.Discard(reader);
                        return result;
                    }
            }

            return null;
        }

        private static class HEnumerableEnumAdder<T>
        {
            public static readonly Func<ReaderState, object, SerializedType, bool, (bool Success, object Result)> Adder = CreateHAdder(typeof(T));
        }

        private static Func<ReaderState, object, SerializedType, bool, (bool Success, object Result)> CreateHAdder(Type type)
        {
            var method = typeof(HEnumerableAdder).GetMethod(nameof(HEnumerableAdder.AddEnums))?.MakeGenericMethod(type) ?? throw new MissingMethodException();
            return (Func<ReaderState, object, SerializedType, bool, (bool Success, object Result)>) Delegate.CreateDelegate(typeof(Func<ReaderState, object, SerializedType, bool, (bool Success, object Result)>), null, method);
        }

        private static class HEnumerableAdder
        {
            public static (bool Success, object Result) AddEnums<T>(ReaderState reader, object list, SerializedType elementType, bool convertToArray) where T : struct
            {
                if (!(list is ICollection<T> l)) 
                    return (false, list);

                do
                {
                    try
                    {
                        ReadEnumsIntoList(reader, elementType, l);
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

        private static void ReadEnumsIntoList<T>(ReaderState reader, SerializedType valueType, ICollection<T> l) where T : struct
        {
            switch (valueType)
            {
                case SerializedType.Byte:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadByte(reader)));
                    break;
                case SerializedType.SByte:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadSByte(reader)));
                    break;
                case SerializedType.UShort:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadUShort(reader)));
                    break;
                case SerializedType.Short:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadShort(reader)));
                    break;
                case SerializedType.UInt:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadUInt(reader)));
                    break;
                case SerializedType.Int:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadInt(reader)));
                    break;
                case SerializedType.ULong:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadULong(reader)));
                    break;
                case SerializedType.Long:
                    while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadLong(reader)));
                    break;
                default:
                    Discarder.DiscardValues(reader, valueType);
                    break;
            }
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
                {
                    if (!(list is IEnumerable<T> e))
                        return (false, list);

                    EnumerableWrapperWithAdd<T> adder = new EnumerableWrapperWithAdd<T>(e);
                    if (!adder.HasAddAction)
                        return (false, list);

                    do
                    {
                        try
                        {
                            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                            {
                                var val = ReadEnum<T>(reader);
                                if (val.HasValue)
                                    adder.Add(val.Value);
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
                    return (true, list);
                }

                do
                {
                    try
                    {
                        while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
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

        public static object ReadHEnums<T, TElement>(ReaderState reader, int count, SerializedType elementType)
        {
            var result = CreateResultObject<T, TElement>(count);
            var addAll = HListEnumAdder<TElement>.Adder;
            var (success, addedResult) = addAll(reader, result, count, elementType, typeof(T).IsArray);
            if (success)
                return addedResult;

            Discarder.Discard(reader, count);
            return result;
        }
        
        public static object ReadEnums<T, TElement>(ReaderState reader, int count)
        {
            var result = CreateResultObject<T, TElement>(count);
            var addAll = ListEnumAdder<TElement>.Adder;
            var (success, addedResult) = addAll(reader, result, count, typeof(T).IsArray);
            if (success)
                return addedResult;

            Discarder.Discard(reader, count);
            return result;
        }

        public static object ReadHList<T, TElement>(ReaderState reader, int count)
        {
            var result = CreateResultObject<T, TElement>(count);
            var valueType = Reader.ReadSerializedType(reader);
            switch (TypeOf<TElement>.TypeCode)
            {
                case TypeCode.Object:
                {
                    if (result is ICollection<TElement> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var v = SelfUpgradingReader.ReadAsObject<TElement>(reader, valueType);
                            l.Add((TElement) v);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.Boolean:
                {
                    if (result is ICollection<bool> l)
                    {
                        if (valueType == SerializedType.Bool)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadBool(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsBool(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardBools(reader, count);
                    return result;
                }
                case TypeCode.Char:
                {
                    if (result is ICollection<char> l)
                    {
                        if (valueType == SerializedType.Char)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadChar(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsChar(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardChars(reader, count);
                    return result;
                }
                case TypeCode.SByte:
                {
                    if (result is ICollection<sbyte> l)
                    {
                        if (valueType == SerializedType.SByte)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadSByte(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsSByte(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardSBytes(reader, count);
                    return result;
                }
                case TypeCode.Byte:
                {
                    if (result is ICollection<byte> l)
                    {
                        if (valueType == SerializedType.Byte)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadByte(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsByte(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardBytes(reader, count);
                    return result;
                }
                case TypeCode.Int16:
                {
                    if (result is ICollection<short> l)
                    {
                        if (valueType == SerializedType.Short)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadShort(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsShort(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardShorts(reader, count);
                    return result;
                }
                case TypeCode.UInt16:
                {
                    if (result is ICollection<ushort> l)
                    {
                        if (valueType == SerializedType.UShort)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadUShort(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsUShort(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardUShorts(reader, count);
                    return result;
                }
                case TypeCode.Int32:
                {
                    if (result is ICollection<int> l)
                    {
                        if (valueType == SerializedType.Int)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadInt(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsInt(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardInts(reader, count);
                    return result;
                }
                case TypeCode.UInt32:
                {
                    if (result is ICollection<uint> l)
                    {
                        if (valueType == SerializedType.UInt)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadUInt(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsUInt(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardUInts(reader, count);
                    return result;
                }
                case TypeCode.Int64:
                {
                    if (result is ICollection<long> l)
                    {
                        if (valueType == SerializedType.Long)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadLong(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsLong(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardLongs(reader, count);
                    return result;
                }
                case TypeCode.UInt64:
                {
                    if (result is ICollection<ulong> l)
                    {
                        if (valueType == SerializedType.ULong)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadULong(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsULong(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardULongs(reader, count);
                    return result;
                }
                case TypeCode.Single:
                {
                    if (result is ICollection<float> l)
                    {
                        if (valueType == SerializedType.Float)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadFloat(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsFloat(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardFloats(reader, count);
                    return result;
                }
                case TypeCode.Double:
                {
                    if (result is ICollection<double> l)
                    {
                        if (valueType == SerializedType.Double)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadDouble(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsDouble(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardDoubles(reader, count);
                    return result;
                }
                case TypeCode.Decimal:
                {
                    if (result is ICollection<decimal> l)
                    {
                        if (valueType == SerializedType.Decimal)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadDecimal(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsDecimal(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardDecimals(reader, count);
                    return result;
                }
                case TypeCode.DateTime:
                {
                    if (result is ICollection<DateTime> l)
                    {
                        if (valueType == SerializedType.DateTime)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadDateTime(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsDateTime(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardDateTimes(reader, count);
                    return result;
                }
                case TypeCode.Guid:
                {
                    if (result is ICollection<Guid> l)
                    {
                        if (valueType == SerializedType.Guid)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadGuid(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsGuid(reader, valueType);
                                if (v.HasValue)
                                    l.Add(v.Value);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardGuids(reader, count);
                    return result;
                }
                case TypeCode.String:
                {
                    if (result is ICollection<string> l)
                    {
                        if (valueType == SerializedType.Double)
                        {
                            for (var i = 0; i < count; i++)
                                l.Add(Reader.ReadString(reader));
                        }
                        else
                        {
                            for (var i = 0; i < count; i++)
                            {
                                var v = SelfUpgradingReader.ReadAsString(reader, valueType);
                                l.Add(v);
                            }
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.DiscardStrings(reader, count);
                    return result;
                }
            }

            return null;
        }

        public static object ReadList<T, TElement>(ReaderState reader, int count)
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
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsObject<TElement>(reader, valueType);
                            l.Add((TElement) v);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.Boolean:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<bool> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsBool(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.Byte:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<byte> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsByte(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.Char:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<char> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsChar(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.DateTime:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<DateTime> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsDateTime(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.Guid:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<Guid> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsGuid(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.Decimal:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<decimal> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsDecimal(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.Double:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<double> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsDouble(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.Int16:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<short> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsShort(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.Int32:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<int> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsInt(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.Int64:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<long> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsLong(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.SByte:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<sbyte> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsSByte(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.Single:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<float> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsFloat(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.String:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<string> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsString(reader, valueType);
                            l.Add(v);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.UInt16:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<ushort> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsUShort(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.UInt32:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<uint> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsUInt(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
                case TypeCode.UInt64:
                {
                    var result = CreateResultObject<T, TElement>(count);
                    if (result is ICollection<ulong> l)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            var valueType = Reader.ReadSerializedType(reader);
                            var v = SelfUpgradingReader.ReadAsULong(reader, valueType);
                            if (v.HasValue)
                                l.Add(v.Value);
                        }

                        return typeof(T).IsArray ? ToArray(l) : result;
                    }

                    Discarder.Discard(reader, count);
                    return result;
                }
            }

            return null;
        }

        private static class HListEnumAdder<T>
        {
            public static readonly Func<ReaderState, object, int, SerializedType, bool, (bool Success, object Result)> Adder = CreateHEnumAdder(typeof(T));
        }

        private static Func<ReaderState, object, int, SerializedType, bool, (bool Success, object Result)> CreateHEnumAdder(Type type)
        {
            var method = typeof(HListAdder).GetMethod(nameof(HListAdder.AddEnums))?.MakeGenericMethod(type) ?? throw new MissingMethodException();
            return (Func<ReaderState, object, int, SerializedType, bool, (bool Success, object Result)>) Delegate.CreateDelegate(typeof(Func<ReaderState, object, int, SerializedType, bool, (bool Success, object Result)>), null, method);
        }
        
        private static class ListEnumAdder<T>
        {
            public static readonly Func<ReaderState, object, int, bool, (bool Success, object Result)> Adder = CreateEnumAdder(typeof(T));
        }

        private static Func<ReaderState, object, int, bool, (bool Success, object Result)> CreateEnumAdder(Type type)
        {
            var method = typeof(ListAdder).GetMethod(nameof(ListAdder.AddEnums))?.MakeGenericMethod(type) ?? throw new MissingMethodException();
            return (Func<ReaderState, object, int, bool, (bool Success, object Result)>) Delegate.CreateDelegate(typeof(Func<ReaderState, object, int, bool, (bool Success, object Result)>), null, method);
        }

        private static class HListAdder
        {
            public static (bool Success, object Result) AddEnums<T>(ReaderState reader, object list, int count, SerializedType elementType, bool convertToArray) where T : struct
            {
                if (!(list is ICollection<T> l)) 
                    return (false, list);

                var i = 0;
                do
                {
                    try
                    {
                        ReadEnumsIntoList(reader, elementType, i, count, l);
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
        
        private static void ReadEnumsIntoList<T>(ReaderState reader, SerializedType valueType, int startIndex, int count, ICollection<T> l) where T : struct
        {
            switch (valueType)
            {
                case SerializedType.Byte:
                    for (var i = startIndex; i < count; i++)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadByte(reader)));
                    break;
                case SerializedType.SByte:
                    for (var i = startIndex; i < count; i++)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadSByte(reader)));
                    break;
                case SerializedType.UShort:
                    for (var i = startIndex; i < count; i++)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadUShort(reader)));
                    break;
                case SerializedType.Short:
                    for (var i = startIndex; i < count; i++)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadShort(reader)));
                    break;
                case SerializedType.UInt:
                    for (var i = startIndex; i < count; i++)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadUInt(reader)));
                    break;
                case SerializedType.Int:
                    for (var i = startIndex; i < count; i++)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadInt(reader)));
                    break;
                case SerializedType.ULong:
                    for (var i = startIndex; i < count; i++)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadULong(reader)));
                    break;
                case SerializedType.Long:
                    for (var i = startIndex; i < count; i++)
                        l.Add((T) Enum.ToObject(typeof(T), Reader.ReadLong(reader)));
                    break;
                default:
                    Discarder.DiscardValues(reader, count - startIndex, valueType);
                    break;
            }
        }
        
        private static T? ReadEnum<T>(ReaderState reader) where T : struct
        {
            var valueType = Reader.ReadSerializedType(reader);
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
                    Discarder.DiscardValue(reader, valueType);
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
            return result.Length == l.Count ? result : l.ToArray();
        }

        public static object ReadObjectAsDictionary(ReaderState reader, Type parentType, Type type)
        {
            var result = CreateResultObject(parentType, (typeof(string), type));
            var addAll = GetObjectAsDictionaryAdder(type);
            var (success, addedResult) = addAll(reader, result);
            if (success)
                return addedResult;
            
            while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
            {
                reader.ReadString(); // key
                Discarder.DiscardValue(reader); // value
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
                        while (Reader.ReadEnumerableType(reader) == EnumerableType.HasItem)
                        {
                            var key = reader.ReadString();
                            var valueType = Reader.ReadSerializedType(reader);
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
                Discarder.DiscardValue(reader); // key
                Discarder.DiscardValue(reader); // value
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
                            var keyType = Reader.ReadSerializedType(reader);
                            var key = TypedDeserializer.ReadValue<TKey>(reader, keyType);
                            var valueType = Reader.ReadSerializedType(reader);
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