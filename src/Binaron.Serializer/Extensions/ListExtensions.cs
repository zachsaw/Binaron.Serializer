using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Extensions
{
    internal static class ListExtensions
    {
        private static class ArrayAccessor<T>
        {
            public static readonly Func<List<T>, T[]> Getter = CreateArrayAccessor();

            private static Func<List<T>, T[]> CreateArrayAccessor()
            {
                var method = new DynamicMethod("get", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, typeof(T[]), new[] {typeof(List<T>)}, typeof(ArrayAccessor<T>), true);
                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0); // Load List<T> argument
                il.Emit(OpCodes.Ldfld, typeof(List<T>).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)); // Replace argument by field
                il.Emit(OpCodes.Ret); // Return field
                return (Func<List<T>, T[]>) method.CreateDelegate(typeof(Func<List<T>, T[]>));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetInternalArray<T>(this List<T> list) => ArrayAccessor<T>.Getter(list);
       
    }
}