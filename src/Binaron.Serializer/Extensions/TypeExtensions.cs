using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeCode = Binaron.Serializer.Enums.TypeCode;

namespace Binaron.Serializer.Extensions
{
    internal static class TypeExtensions
    {
        private const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.Instance;

        public static IEnumerable<MemberInfo> GetMemberInfos(this Type targetType)
        {
            return targetType.GetProperties(BindingAttr).Cast<MemberInfo>().Concat(targetType.GetFields(BindingAttr));
        }

        public static MemberInfo GetMemberInfo(this Type targetType, string memberName)
        {
            return targetType.GetMembers().FirstOrDefault(x => x.Name == memberName) ??
                   throw new ArgumentException($"Property '{memberName}' does not exist for type '${targetType}'.");
        }

        public static MemberInfo TryGetBackingField(this Type targetType, string memberName)
        {
            return targetType.GetFields(BindingFlags.NonPublic | BindingAttr).FirstOrDefault(x => x.Name == $"<{memberName}>k__BackingField");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TypeCode GetTypeCode(this Type type)
        {
            var result = Type.GetTypeCode(type);
            if (result != System.TypeCode.Object)
                return (TypeCode) result;
            
            return type == typeof(Guid) ? TypeCode.Guid : TypeCode.Object;
        }
    }

    internal static class TypeOf<T>
    {
        public static readonly TypeCode TypeCode = typeof(T).GetTypeCode();
    }
}