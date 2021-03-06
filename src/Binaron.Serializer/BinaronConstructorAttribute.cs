using System;

namespace Binaron.Serializer
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class BinaronConstructorAttribute : Attribute
    {
        public BinaronConstructorAttribute()
        {

        }
    }
}