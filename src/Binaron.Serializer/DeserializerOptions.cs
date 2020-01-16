using Binaron.Serializer.CustomObject;

namespace Binaron.Serializer
{
    public struct DeserializerOptions
    {
        public ICustomObjectFactory[] CustomObjectFactories { get; set; }
    }
}