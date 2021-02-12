using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Infrastructure
{
    internal class EnumerableWrapperWithAdd<T>
    {
        private readonly Action<object, T> Action;
        private readonly object Result;
        public bool HasAddAction { get; private set; } = false;

        public EnumerableWrapperWithAdd(IEnumerable<T> result)
        {
            Result = result;
            var method = result.GetType().GetMethod("Add", new Type[] { typeof(T) });

            if (method == null)
                Action = (o, v) => { };
            else
            {
                HasAddAction = true;
                DynamicMethod action = new DynamicMethod(
                            "Add",
                            null,
                            new Type[] { typeof(object), typeof(T) },
                            this.GetType().Module);

                var il = action.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, method);
                il.Emit(OpCodes.Ret);

                Action = (Action<object, T>)action.CreateDelegate(typeof(Action<object, T>));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T value)
        {
            Action(Result, value);
        }
    }
}