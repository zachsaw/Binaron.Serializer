using Binaron.Serializer.CustomObject;

namespace Binaron.Serializer
{
    public struct SerializerOptions
    {
        public bool SkipNullValues { get; set; }
        public ICustomObjectIdentifierProvider[] CustomObjectIdentifierProviders { get; set; }
    }
}