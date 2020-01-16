using System.Collections.Generic;
using Binaron.Serializer.CustomObject;

namespace Binaron.Serializer
{
    public class SerializerOptions
    {
        public bool SkipNullValues { get; set; }
        public List<ICustomObjectIdentifierProvider> CustomObjectIdentifierProviders { get; set; } = new List<ICustomObjectIdentifierProvider>();
    }
}