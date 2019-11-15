using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Binaron.Serializer.Extensions;

namespace Binaron.Serializer.Infrastructure
{
    internal static class GenericType
    {
        private static readonly ConcurrentDictionary<Type, Type> CollectionGenericTypeLookup = new ConcurrentDictionary<Type, Type>();
        private static readonly ConcurrentDictionary<Type, Type> EnumerableGenericTypeLookup = new ConcurrentDictionary<Type, Type>();
        private static readonly ConcurrentDictionary<Type, (Type KeyType, Type ValueType)> ReaderDictionaryGenericTypeLookup = new ConcurrentDictionary<Type, (Type KeyType, Type ValueType)>();
        private static readonly ConcurrentDictionary<Type, (Type KeyType, Type ValueType)> WriterDictionaryGenericTypeLookup = new ConcurrentDictionary<Type, (Type KeyType, Type ValueType)>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetICollection(Type listType) => CollectionGenericTypeLookup.GetOrAdd(listType,
            _ =>
            {
                return listType.GetInterfaces().Concat(listType.Yield().Where(t => t.IsInterface))
                    .FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))?
                    .GenericTypeArguments[0];
            });

        public static class GetICollectionGenericType<T>
        {
            public static readonly Type Type = GetICollection(typeof(T));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type GetIEnumerable(Type enumerableType) => EnumerableGenericTypeLookup.GetOrAdd(enumerableType,
            _ =>
            {
                return enumerableType.GetInterfaces().Concat(enumerableType.Yield().Where(t => t.IsInterface))
                    .FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))?
                    .GenericTypeArguments[0];
            });

        public static class GetIEnumerableGenericType<T>
        {
            public static readonly Type Type = GetIEnumerable(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Type KeyType, Type ValueType) GetIDictionaryWriter(Type dictionaryType) => WriterDictionaryGenericTypeLookup.GetOrAdd(dictionaryType, _ => GetIDictionary(dictionaryType));

        public static class GetIDictionaryWriterGenericTypes<T>
        {
            public static readonly (Type KeyType, Type ValueType) Types = GetIDictionaryWriter(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Type KeyType, Type ValueType) GetIDictionaryReader(Type dictionaryType) => ReaderDictionaryGenericTypeLookup.GetOrAdd(dictionaryType, _ => GetIEnumerableKvp(dictionaryType));

        private static (Type KeyType, Type ValueType) GetIDictionary(Type dictionaryType)
        {
            var results = dictionaryType.GetInterfaces().Concat(dictionaryType.Yield().Where(t => t.IsInterface))
                .FirstOrDefault(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))?
                .GenericTypeArguments;

            return (results?[0], results?[1]);
        }

        private static (Type KeyType, Type ValueType) GetIEnumerableKvp(Type dictionaryType)
        {
            var results = dictionaryType.GetInterfaces().Concat(dictionaryType.Yield().Where(t => t.IsInterface))
                .Select(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) && GetKvpTypeArgs(type, out var kvpType) ? kvpType : null)
                .FirstOrDefault(i => i != null)?
                .GenericTypeArguments;

            return (results?[0], results?[1]);
        }

        private static bool GetKvpTypeArgs(Type type, out Type kvpType)
        {
            var typeArg = type.GenericTypeArguments.Single();
            if (typeArg.IsGenericType && typeArg.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                kvpType = typeArg;
                return true;
            }

            kvpType = null;
            return false;
        }
    }
}