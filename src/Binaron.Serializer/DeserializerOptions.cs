using System.Collections.Generic;
using Binaron.Serializer.CustomObject;

namespace Binaron.Serializer
{
    public class DeserializerOptions
    {
        public List<ICustomObjectFactory> CustomObjectFactories { get; set; } = new List<ICustomObjectFactory>();
        public IObjectActivator ObjectActivator { get; set; }
    }
}