using System;

namespace Binaron.Serializer.CustomObject
{
    public interface IObjectActivator
    {
        object Create(Type type);
    }
}