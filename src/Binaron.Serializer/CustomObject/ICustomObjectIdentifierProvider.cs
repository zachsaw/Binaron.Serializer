using System;

namespace Binaron.Serializer.CustomObject
{
    public interface ICustomObjectIdentifierProvider
    {
        Type BaseType { get; }
        object GetIdentifier(Type objectType);
    }
}