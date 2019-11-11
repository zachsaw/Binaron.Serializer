using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Binaron.Serializer.Extensions;

namespace Binaron.Serializer.Accessors
{
    internal readonly struct MemberGetter<T>
    {
        private const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.Instance;

        private readonly Func<object, T> getDelegate;
        
        public Type MemberType { get; }

        public string MemberName { get; }
        public bool IsValid { get; }
    
        public MemberGetter(Type targetType, string memberName)
        {
            MemberName = memberName;
            var memberInfo = targetType.GetProperties(BindingAttr).Cast<MemberInfo>().Concat(targetType.GetFields(BindingAttr)).FirstOrDefault(x => x.Name == memberName);

            if (memberInfo == null)
                throw new ArgumentException($"Property '{memberName}' does not exist for type '${targetType}'.");

            MemberType = memberInfo.GetMemberType();
            
            var canRead = memberInfo is FieldInfo || ((PropertyInfo) memberInfo).CanRead;
            if (canRead)
            {
                getDelegate = GetGetDelegate(targetType, MemberType, memberInfo);
                IsValid = true;
            }
            else
            {
                getDelegate = null;
                IsValid = false;
            }
        }

        [Pure]
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