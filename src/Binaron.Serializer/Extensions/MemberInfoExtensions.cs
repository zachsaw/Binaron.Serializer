using System;
using System.Reflection;

namespace Binaron.Serializer.Extensions
{
    internal static class MemberInfoExtensions
    {
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            if (memberInfo == null)
                throw new ArgumentNullException(nameof(memberInfo));

            switch (memberInfo)
            {
                case FieldInfo info:
                    return info.FieldType;
                case PropertyInfo info:
                    return info.PropertyType;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}