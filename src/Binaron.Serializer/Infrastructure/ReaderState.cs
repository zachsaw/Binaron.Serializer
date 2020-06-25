using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Binaron.Serializer.CustomObject;

namespace Binaron.Serializer.Infrastructure
{
    internal class ReaderState : IDisposable
    {
        private readonly BinaryReader reader;

        public ReaderState(Stream stream, DeserializerOptions options)
        {
            reader = new BinaryReader(stream);
            var factories = options.CustomObjectFactories?.ToDictionary(handler => handler.BaseType, handler => handler);
            if (factories?.Any() == true)
                CustomObjectFactories = factories;

            ObjectActivator = options.ObjectActivator;
            CultureInfo = options.CultureInfo;
        }

        public Dictionary<Type, ICustomObjectFactory> CustomObjectFactories { get; }
        public IObjectActivator ObjectActivator { get; }
        public IFormatProvider CultureInfo { get; }

        ~ReaderState()
        {
            Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read<T>() where T : unmanaged => reader.Read<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString() => reader.ReadString();

        public void Dispose()
        {
            reader.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}