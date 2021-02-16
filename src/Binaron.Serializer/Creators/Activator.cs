using System;
using System.Collections.Concurrent;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Creators
{
    internal static class Activator
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> Activators = new ConcurrentDictionary<Type, Func<object>>();
        private static readonly ConcurrentDictionary<Type, Func<int, object>> WithCapacityActivators = new ConcurrentDictionary<Type, Func<int, object>>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<object> Get(Type type) => Activators.GetOrAdd(type, CreateActivator);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<int, object> GetWithCapacity(Type type) => WithCapacityActivators.GetOrAdd(type, CreateWithCapacityActivator);
        
        private static Func<object> CreateActivator(Type type)
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(object), Type.EmptyTypes);
            var il = method.GetILGenerator();
            if (!type.IsValueType)
            {
                var constructorInfo = type.GetConstructor(Type.EmptyTypes);
                if (constructorInfo == null)
                    return null;

                il.Emit(OpCodes.Newobj, constructorInfo);
                il.Emit(OpCodes.Ret);
            }
            else
            {
                var local = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldloca_S, local);
                il.Emit(OpCodes.Initobj, type);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
                il.Emit(OpCodes.Ret);
            }
            return (Func<object>) method.CreateDelegate(typeof(Func<object>));
        }

        private static readonly Type[] ParamTypes = {typeof(int)};
        
        private static Func<int, object> CreateWithCapacityActivator(Type type)
        {
            var method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(object), ParamTypes);
            var il = method.GetILGenerator();
            if (!type.IsValueType)
            {
                var constructorInfo = type.GetConstructor(ParamTypes);
                if (constructorInfo == null)
                    throw new MissingMethodException(type.FullName, "No constructor with single 'int' argument was found");
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Newobj, constructorInfo);
                il.Emit(OpCodes.Ret);
            }
            else
            {
                throw new InvalidOperationException();
            }
            return (Func<int, object>) method.CreateDelegate(typeof(Func<int, object>));
        }
    }
}