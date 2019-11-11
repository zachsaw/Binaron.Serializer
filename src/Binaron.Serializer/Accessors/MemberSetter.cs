using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Binaron.Serializer.Extensions;

namespace Binaron.Serializer.Accessors
{
    internal readonly struct MemberSetter<T>
    {
        private const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.Instance;

        private readonly Action<object, T> setDelegate;

        public Type MemberType { get; }
        public string MemberName { get; }
        public bool IsValid { get; }

        public MemberSetter(Type targetType, string memberName)
        {
            MemberName = memberName;
            var memberInfo = targetType.GetProperties(BindingAttr).Cast<MemberInfo>().Concat(targetType.GetFields(BindingAttr)).FirstOrDefault(x => x.Name == memberName);

            if (memberInfo == null)
                throw new ArgumentException($"Property '{memberName}' does not exist for type '${targetType}'.");

            MemberType = memberInfo.GetMemberType();
            
            var canWrite = memberInfo is FieldInfo || ((PropertyInfo) memberInfo).CanWrite;
            if (canWrite)
            {
                setDelegate = GetSetDelegate(targetType, MemberType, memberInfo);
                IsValid = true;
            }
            else
            {
                setDelegate = null;
                IsValid = false;
            }
        }

        public void Set(object target, T value) => setDelegate(target, value);

        private static readonly Type[] SetParamTypes = {typeof(object), typeof(T)};

        private static Action<object, T> GetSetDelegate(Type targetType, Type memberType, MemberInfo memberInfo)
        {
            var owner = targetType.IsAbstract || targetType.IsInterface ? null : targetType;
            var setMethod = owner != null
                ? new DynamicMethod(Guid.NewGuid().ToString(), null, SetParamTypes, owner, true)
                : new DynamicMethod(Guid.NewGuid().ToString(), null, SetParamTypes, true);

            GenerateForSetDelegate(targetType, memberType, memberInfo, setMethod);

            return (Action<object, T>) setMethod.CreateDelegate(Expression.GetActionType(SetParamTypes));
        }

        private static void GenerateForSetDelegate(Type targetType, Type memberType, MemberInfo member, DynamicMethod setMethod)
        {
            var ilGen = setMethod.GetILGenerator();

            var paramType = memberType;
            ilGen.Emit(OpCodes.Ldarg_0); // Load target object  
            ilGen.Emit(targetType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, targetType); // Cast to the source type
            ilGen.Emit(OpCodes.Ldarg_1); // Load value to set 
            if (paramType.IsValueType)
            {
                if (!typeof(T).IsValueType)
                {
                    ilGen.Emit(OpCodes.Unbox, paramType); // Unbox it so we can set to T which is not a value type
                    var load = TypeOpcode.Hash[paramType];
                    if (load != null)
                        ilGen.Emit((OpCode) load);
                    else
                        ilGen.Emit(OpCodes.Ldobj, paramType);
                }
            }
            else
            {
                ilGen.Emit(OpCodes.Castclass, paramType);
            }

            if (member is FieldInfo fieldInfo)
            {
                ilGen.Emit(OpCodes.Stfld, fieldInfo);
            }
            else
            {
                var targetSetMethod = ((PropertyInfo) member).GetSetMethod(false);
                if (targetSetMethod == null)
                    throw new MissingMemberException();

                var opCode = targetType.IsValueType ? OpCodes.Call : OpCodes.Callvirt;
                ilGen.Emit(opCode, targetSetMethod);
            }

            ilGen.Emit(OpCodes.Ret);
        }
    }
}