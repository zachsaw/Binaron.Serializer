using System;
using System.Linq;
using System.Reflection;

namespace Binaron.Serializer.Tests.Extensions
{
    internal sealed class Method
    {
        private readonly MethodInfo method;

        public Method(IReflect type, string methodName, params Type[] types)
        {
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var m = methods.FirstOrDefault(mi => mi.Name == methodName && mi.GetGenericArguments().Length == types.Length);
            method = m?.MakeGenericMethod(types) ?? throw new MissingMethodException();
        }
        
        public void Action(object self, params object[] args) => Func(self, args);

        public object Func(object self, params object[] args)
        {
            try
            {
                return method.Invoke(self, args);
            }
            catch (TargetInvocationException ex)
            {
                ThrowInnerException(ex);
                throw;
            }
        }
        
        private static void ThrowInnerException(TargetInvocationException ex)
        {
            if (ex.InnerException == null) 
                throw new NullReferenceException("TargetInvocationException did not contain an InnerException", ex);

            Exception exception = null;
            try
            {
                // assume typed Exception has "new (String message, Exception innerException)" signature
                exception = (Exception) Activator.CreateInstance(ex.InnerException.GetType(), ex.InnerException.Message, ex.InnerException);
            }
            catch
            {
                // constructor doesn't have the right constructor, eat the error and throw the inner exception below
            }

            if (exception?.InnerException == null || ex.InnerException.Message != exception.Message)
                throw ex.InnerException;
            
            throw exception;
        }
    }
}