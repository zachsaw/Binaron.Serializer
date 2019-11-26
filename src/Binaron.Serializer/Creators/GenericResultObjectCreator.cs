using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Creators
{
    internal static class GenericResultObjectCreator
    {
        internal sealed class List
        {
            private readonly Func<int, object> createWithCapacity;
            private readonly Func<object> create;

            public List(Type parentType, Type type)
            {
                if (IsSupportedCollectionInterfaces(parentType, type))
                {
                    createWithCapacity = Activator.GetWithCapacity(typeof(List<>).MakeGenericType(type));
                }
                else
                {
                    if (parentType.IsInterface || parentType.IsAbstract)
                        throw new NotSupportedException($"'{parentType}' is non-concrete");

                    if (parentType.IsArray)
                    {
                        if (parentType.GetArrayRank() != 1)
                            throw new NotSupportedException("Multidimensional array is not supported");

                        parentType = typeof(List<>).MakeGenericType(type);
                    }

                    if (IsBuiltinCollectionConcretes(parentType, type))
                        createWithCapacity = Activator.GetWithCapacity(parentType);
                    else
                        create = Activator.Get(parentType);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public object Create(int capacity = 0) => createWithCapacity != null ? createWithCapacity(capacity) : create();

            private static bool IsBuiltinCollectionConcretes(Type parentType, Type type)
            {
                return parentType == typeof(List<>).MakeGenericType(type);
            }
        }

        private static bool IsSupportedCollectionInterfaces(Type parentType, Type type)
        {
            return parentType.IsAssignableFrom(typeof(List<>).MakeGenericType(type));
        }
        
        internal sealed class Dictionary
        {
            private readonly Func<int, object> createWithCapacity;
            private readonly Func<object> create;

            public Dictionary(Type parentType, (Type KeyType, Type ValueType) type)
            {
                if (IsSupportedDictionaryInterfaces(parentType, type))
                {
                    createWithCapacity = Activator.GetWithCapacity(typeof(Dictionary<,>).MakeGenericType(type.KeyType, type.ValueType));
                }
                else
                {
                    if (parentType.IsInterface || parentType.IsAbstract)
                        throw new NotSupportedException($"'{parentType}' is non-concrete");

                    if (IsBuiltinDictionaryConcretes(parentType, type))
                        createWithCapacity = Activator.GetWithCapacity(parentType);
                    else
                        create = Activator.Get(parentType);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public object Create(int capacity) => createWithCapacity != null ? createWithCapacity(capacity) : create();

            private static bool IsBuiltinDictionaryConcretes(Type parentType, (Type KeyType, Type ValueType) type)
            {
                return parentType == typeof(Dictionary<,>).MakeGenericType(type.KeyType, type.ValueType);
            }

            private static bool IsSupportedDictionaryInterfaces(Type parentType, (Type KeyType, Type ValueType) type)
            {
                return parentType.IsAssignableFrom(typeof(Dictionary<,>).MakeGenericType(type.KeyType, type.ValueType));
            }
        }
    }
}