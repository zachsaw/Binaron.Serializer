using System.Reflection;

namespace Binaron.Serializer.Accessors
{
    public interface IMemberSetterHandler<in T>
    {
        MemberInfo MemberInfo { get; }

        void Handle(T state, object target);
    }
}