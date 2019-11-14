using System.Reflection;

namespace Binaron.Serializer.Accessors
{
    public interface IMemberGetterHandler<in T>
    {
        MemberInfo MemberInfo { get; }

        void Handle(T state, object target);
    }
}