namespace Binaron.Serializer.Accessors
{
    public interface IMemberSetterHandler<in T>
    {
        string MemberName { get; }

        void Handle(T state, object target);
    }
}