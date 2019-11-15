using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Accessors;
using Binaron.Serializer.Creators;
using Binaron.Serializer.Enums;
using Binaron.Serializer.Infrastructure;
using Activator = System.Activator;

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

        public static object ReadValue<T>(BinaryReader reader)
        {
            var valueType = (SerializedType) reader.Read<byte>();
            return ReadValue<T>(reader, valueType);
        }

        private interface IValueReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            object Read(BinaryReader reader, SerializedType valueType);
        }

        private static class GetValueReader<T>
        {
            public static readonly IValueReader Reader = ValueReaders<T>.CreateReader();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ReadValue<T>(BinaryReader reader, SerializedType valueType) => GetValueReader<T>.Reader.Read(reader, valueType);

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

        public interface IDictionaryReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            object Read(BinaryReader reader);
        }

        private static class GetDictionaryReader<T>
        {
            public static readonly IDictionaryReader Reader = DictionaryReaders.CreateReader<T>();
        }

        public static object ReadDictionary<T>(BinaryReader reader) => GetDictionaryReader<T>.Reader.Read(reader);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ReadDictionaryNonGeneric(BinaryReader reader, Type type)
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

        public interface IEnumerableReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            object Read(BinaryReader reader);
        }

        private static class GetEnumerableReader<T>
        {
            public static readonly IEnumerableReader Reader = EnumerableReaders.CreateReader<T>();
        }

        public static object ReadEnumerable<T>(BinaryReader reader) => GetEnumerableReader<T>.Reader.Read(reader);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ReadEnumerableNonGeneric(BinaryReader reader, Type type)
        {
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

        public interface IListReader
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            object Read(BinaryReader reader);
        }

        private static class GetListReader<T>
        {
            public static readonly IListReader Reader = ListReaders.CreateReader<T>();
        }

        public static object ReadList<T>(BinaryReader reader) => GetListReader<T>.Reader.Read(reader);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object ReadListNonGeneric(BinaryReader reader, Type type)
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

        private static class DictionaryReaders
        {
            public static IDictionaryReader CreateReader<T>()
            {
                var type = typeof(T);
                var dictionaryGenericType = GenericType.GetIDictionaryReader(type);
                if (dictionaryGenericType.KeyType != null)
                    return new GenericDictionaryReader(type, dictionaryGenericType);

                if (type == typeof(object))
                    return new DynamicDictionaryReader();

                return new DictionaryReader<T>();
            }

            private class DynamicDictionaryReader : IDictionaryReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader) => Deserializer.ReadDictionary(reader);
            }

            private class DictionaryReader<T> : IDictionaryReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader) => ReadDictionaryNonGeneric(reader, typeof(T));
            }

            private class GenericDictionaryReader : IDictionaryReader
            {
                private readonly Type type;
                private readonly (Type KeyType, Type ValueType) elementType;

                public GenericDictionaryReader(Type type, (Type KeyType, Type ValueType) elementType)
                {
                    this.type = type;
                    this.elementType = elementType;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader)
                {
                    var count = reader.Read<int>();
                    return GenericReader.ReadDictionary(reader, type, elementType, count);
                }
            }
        }

        private static class EnumerableReaders
        {
            public static IEnumerableReader CreateReader<T>()
            {
                var type = typeof(T);
                var elementType = GenericType.GetIEnumerable(type);
                if (elementType != null)
                {
                    if (elementType.IsEnum)
                        return (IEnumerableReader) Activator.CreateInstance(typeof(GenericEnumEnumerableReader<,>).MakeGenericType(type, elementType));

                    return (IEnumerableReader) Activator.CreateInstance(typeof(GenericEnumerableReader<,>).MakeGenericType(type, elementType));
                }

                if (type == typeof(object))
                    return new DynamicEnumerableReader();

                return new EnumerableReader<T>();
            }

            private class DynamicEnumerableReader : IEnumerableReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader) => Deserializer.ReadEnumerable(reader);
            }

            private class GenericEnumEnumerableReader<T, TElement> : IEnumerableReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader) => GenericReader.ReadEnums(reader, typeof(T), typeof(TElement));
            }

            private class EnumerableReader<T> : IEnumerableReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader) => ReadEnumerableNonGeneric(reader, typeof(T));
            }

            private class GenericEnumerableReader<T, TElement> : IEnumerableReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader) => GenericReader.ReadTypedEnumerable<T, TElement>(reader);
            }
        }

        private static class ListReaders
        {
            public static IListReader CreateReader<T>()
            {
                var type = typeof(T);
                var elementType = GenericType.GetIEnumerable(type);
                if (elementType != null)
                {
                    if (elementType.IsEnum)
                        return (IListReader) Activator.CreateInstance(typeof(GenericEnumListReader<,>).MakeGenericType(type, elementType));

                    return (IListReader) Activator.CreateInstance(typeof(GenericListReader<,>).MakeGenericType(type, elementType));
                }

                if (type == typeof(object))
                    return new DynamicListReader();

                return new ListReader<T>();
            }

            private class DynamicListReader : IListReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader) => Deserializer.ReadList(reader);
            }

            private class GenericEnumListReader<T, TElement> : IListReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader)
                {
                    var count = reader.Read<int>();
                    return GenericReader.ReadEnums(reader, typeof(T), typeof(TElement), count);
                }
            }

            private class ListReader<T> : IListReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader) => ReadListNonGeneric(reader, typeof(T));
            }

            private class GenericListReader<T, TElement> : IListReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader)
                {
                    var count = reader.Read<int>();
                    return GenericReader.ReadTypedList<T, TElement>(reader, count);
                }
            }
        }

        private static class ValueReaders<T>
        {
            public static IValueReader CreateReader()
            {
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Boolean:
                        return new BoolReader();
                    case TypeCode.Byte:
                        return new ByteReader();
                    case TypeCode.Char:
                        return new CharReader();
                    case TypeCode.DateTime:
                        return new DateTimeReader();
                    case TypeCode.Decimal:
                        return new DecimalReader();
                    case TypeCode.Double:
                        return new DoubleReader();
                    case TypeCode.Int16:
                        return new ShortReader();
                    case TypeCode.Int32:
                        return new IntReader();
                    case TypeCode.Int64:
                        return new LongReader();
                    case TypeCode.SByte:
                        return new SByteReader();
                    case TypeCode.Single:
                        return new FloatReader();
                    case TypeCode.String:
                        return new StringReader();
                    case TypeCode.UInt16:
                        return new UShortReader();
                    case TypeCode.UInt32:
                        return new UIntReader();
                    case TypeCode.UInt64:
                        return new ULongReader();
                    default:
                        return new ObjectReader();
                }
            }

            private class BoolReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsBool(reader, valueType);
            }

            private class ByteReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsByte(reader, valueType);
            }

            private class CharReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsChar(reader, valueType);
            }

            private class DateTimeReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsDateTime(reader, valueType);
            }

            private class DecimalReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsDecimal(reader, valueType);
            }

            private class DoubleReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsDouble(reader, valueType);
            }

            private class ShortReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsShort(reader, valueType);
            }

            private class IntReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsInt(reader, valueType);
            }

            private class LongReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsLong(reader, valueType);
            }

            private class SByteReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsSByte(reader, valueType);
            }

            private class FloatReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsFloat(reader, valueType);
            }

            private class StringReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsString(reader, valueType);
            }

            private class UShortReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsUShort(reader, valueType);
            }

            private class UIntReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsUInt(reader, valueType);
            }

            private class ULongReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsULong(reader, valueType);
            }

            private class ObjectReader : IValueReader
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public object Read(BinaryReader reader, SerializedType valueType) => SelfUpgradingReader.ReadAsObject<T>(reader, valueType);
            }
        }
    }
}