using System;

namespace Binaron.Serializer.CustomObject
{
    public abstract class CustomObjectIdentifierProvider<T> : ICustomObjectIdentifierProvider
    {
        public Type BaseType => typeof(T);
        public abstract object GetIdentifier(Type objectType);
    }
}