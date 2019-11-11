using System;
using Microsoft.CSharp.RuntimeBinder;

namespace Binaron.Serializer.Tests.Extensions
{
    internal static class Caster
    {
        public static dynamic DynamicCast<T>(T entity, Type to)
        {
            var method = new Method(typeof(Caster), nameof(Cast), to);
            dynamic arg = entity;
            return method.Func(null, new object[]{arg});
        }

        private static T Cast<T>(dynamic entity)
        {
            try
            {
                return entity;
            }
            catch (RuntimeBinderException)
            {
                throw new InvalidCastException();
            }
        }
    }
}