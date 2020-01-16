using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Binaron.Serializer.Extensions;

namespace Binaron.Serializer.Accessors
{
    internal readonly struct MemberGetter<T>
    {
        private readonly Func<object, T> getDelegate;
        
        public MemberInfo MemberInfo { get; }
        public string MemberName { get; }
        public bool IsValid { get; }
    
        public MemberGetter(Type targetType, string memberName)
        {
            MemberName = memberName;
            var memberInfo = MemberInfo = targetType.GetMemberInfo(memberName);

            var canRead = GetCanRead(targetType, memberName, memberInfo);
            if (canRead)
            {
                getDelegate = GetGetDelegate(targetType, memberInfo.GetMemberType(), memberInfo);
                IsValid = true;
            }
            else
            {
                getDelegate = null;
                IsValid = false;
            }
        }

        private static bool GetCanRead(Type targetType, string memberName, MemberInfo memberInfo)
        {
            var canRead = memberInfo is FieldInfo || ((PropertyInfo) memberInfo).CanRead;

            if (!canRead)
                return false;

            if (memberInfo.GetCustomAttributes(typeof(IgnoreDataMemberAttribute), false).Length != 0)
                return false;

            if (memberInfo is PropertyInfo)
                memberInfo = targetType.TryGetBackingField(memberName) ?? memberInfo;

            if (memberInfo is FieldInfo fi && fi.GetCustomAttributes(typeof(NonSerializedAttribute), false).Length != 0)
                return false;

            return true;
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(object target) => getDelegate(target);

        // ReSharper disable once StaticMemberInGenericType
        private static readonly Type GetParamType = typeof(object);

        // ReSharper disable once StaticMemberInGenericType
        private static readonly Type[] GetParamTypes = {GetParamType};

        private static readonly Type GetReturnType = typeof(T);

        private static Func<object, T> GetGetDelegate(Type targetType, Type memberType, MemberInfo memberInfo)
        {
            var owner = targetType.IsAbstract || targetType.IsInterface ? null : targetType;
            var getMethod = owner != null
                ? new DynamicMethod(Guid.NewGuid().ToString(), GetReturnType, GetParamTypes, owner, true)
                : new DynamicMethod(Guid.NewGuid().ToString(), GetReturnType, GetParamTypes, true);

            GenerateForGetDelegate(targetType, memberType, memberInfo, getMethod);

            return (Func<object, T>) getMethod.CreateDelegate(Expression.GetFuncType(GetParamType, GetReturnType));
        }

        private static void GenerateForGetDelegate(Type targetType, Type memberType, MemberInfo memberInfo, DynamicMethod getMethod)
        {
            var ilGen = getMethod.GetILGenerator();

            ilGen.Emit(OpCodes.Ldarg_0);

            ilGen.Emit(targetType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, targetType);

            Type returnType;
            if (memberInfo is FieldInfo fieldInfo)
            {
                ilGen.Emit(OpCodes.Ldfld, fieldInfo);
                returnType = memberType;
            }
            else
            {
                var targetGetMethod = ((PropertyInfo) memberInfo).GetGetMethod();
                var opCode = targetType.IsValueType ? OpCodes.Call : OpCodes.Callvirt;
                ilGen.Emit(opCode, targetGetMethod);
                returnType = targetGetMethod.ReturnType;
            }

            if (!typeof(T).IsValueType && returnType.IsValueType)
                ilGen.Emit(OpCodes.Box, returnType);

            ilGen.Emit(OpCodes.Ret);
        }
    }
}