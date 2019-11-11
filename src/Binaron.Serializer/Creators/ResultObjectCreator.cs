using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Creators
{
    internal static class ResultObjectCreator
    {
        internal class List
        {
            private readonly Func<int, object> createWithCapacity;
            private readonly Func<object> create;

            public List(Type type)
            {
                if (IsSupportedCollectionInterfaces(type))
                {
                    createWithCapacity = Activator.GetWithCapacity(typeof(ArrayList));
                }
                else
                {
                    if (type.IsInterface || type.IsAbstract)
                        throw new NotSupportedException($"'{type}' is non-concrete");

                    if (IsBuiltinCollectionConcretes(type))
                        createWithCapacity = Activator.GetWithCapacity(type);
                    else
                        create = Activator.Get(type);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public object Create(int capacity) => createWithCapacity != null ? createWithCapacity(capacity) : create();

            private static bool IsBuiltinCollectionConcretes(Type type)
            {
                return type == typeof(ArrayList);
            }
        }

        internal class Enumerable
        {
            private readonly Func<object> create;

            public Enumerable(Type type)
            {
                if (IsSupportedCollectionInterfaces(type))
                {
                    create = Activator.Get(typeof(ArrayList));
                }
                else
                {
                    if (type.IsInterface || type.IsAbstract)
                        throw new NotSupportedException($"'{type}' is non-concrete");

                    create = Activator.Get(type);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public object Create() => create();
        }
        
        private static bool IsSupportedCollectionInterfaces(Type type)
        {
            return type.IsAssignableFrom(typeof(ArrayList));
        }
        
        internal class Dictionary
        {
            private readonly Func<int, object> createWithCapacity;
            private readonly Func<object> create;

            public Dictionary(Type type)
            {
                if (IsSupportedDictionaryInterfaces(type))
                {
                    createWithCapacity = Activator.GetWithCapacity(typeof(Hashtable));
                }
                else
                {
                    if (type.IsInterface || type.IsAbstract)
                        throw new NotSupportedException($"'{type}' is non-concrete");

                    if (IsBuiltinCollectionConcretes(type))
                        createWithCapacity = Activator.GetWithCapacity(type);
                    else
                        create = Activator.Get(type);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public object Create(int capacity) => createWithCapacity != null ? createWithCapacity(capacity) : create();
            
            private static bool IsBuiltinCollectionConcretes(Type type)
            {
                return type == typeof(Hashtable);
            }
            
            private static bool IsSupportedDictionaryInterfaces(Type type)
            {
                return type.IsAssignableFrom(typeof(Hashtable));
            }
        }
    }
}