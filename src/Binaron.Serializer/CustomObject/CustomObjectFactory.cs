using System;

namespace Binaron.Serializer.CustomObject
{
    public abstract class CustomObjectFactory<T> : ICustomObjectFactory
    {
        public Type BaseType => typeof(T);
        public abstract object Create(object identifier);
    }
}