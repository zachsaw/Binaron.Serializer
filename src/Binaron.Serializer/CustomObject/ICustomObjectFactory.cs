using System;

namespace Binaron.Serializer.CustomObject
{
    public interface ICustomObjectFactory
    {
        Type BaseType { get; }
        object Create(object identifier);
    }
}