using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Infrastructure
{
    internal static class EnumerableWrapperWithAdd
    {
        public static readonly ConcurrentDictionary<Type, object> Adders = new ConcurrentDictionary<Type, object>();
    }
    
    internal readonly struct EnumerableWrapperWithAdd<T>
    {
        private readonly Action<object, T> action;
        private readonly object result;

        public bool HasAddAction => action != null;

        public EnumerableWrapperWithAdd(IEnumerable<T> result) : this()
        {
            action = (Action<object, T>) EnumerableWrapperWithAdd.Adders.GetOrAdd(result.GetType(), CreateAdder);
            this.result = result;
        }

        private static object CreateAdder(Type type)
        {
            var method = type.GetMethod("Add", new[] {typeof(T)});

            if (method == null)
                return null;

            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString(), null, new[] {typeof(object), typeof(T)});

            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, method);
            il.Emit(OpCodes.Ret);

            return dynamicMethod.CreateDelegate(typeof(Action<object, T>));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T value) => action(result, value);
    }
}