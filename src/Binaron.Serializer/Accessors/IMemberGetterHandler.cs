namespace Binaron.Serializer.Accessors
{
    public interface IMemberGetterHandler<in T>
    {
        void Handle(T state, object target);
    }
}