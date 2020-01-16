using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Binaron.Serializer.CustomObject;

namespace Binaron.Serializer.Infrastructure
{
    internal class WriterState : IDisposable
    {
        private readonly BinaryWriter writer;

        public WriterState(Stream stream, SerializerOptions options)
        {
            writer = new BinaryWriter(stream);
            SkipNullValues = options.SkipNullValues;
            var identifierProviders = options.CustomObjectIdentifierProviders?.ToDictionary(handler => handler.BaseType, handler => handler);
            if (identifierProviders?.Any() == true)
                CustomObjectIdentifierProviders = identifierProviders;
        }

        ~WriterState()
        {
            Dispose();
        }
        
        public bool SkipNullValues { get; }
        public Dictionary<Type, ICustomObjectIdentifierProvider> CustomObjectIdentifierProviders { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write<T>(T value) where T : unmanaged => writer.Write(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value) => writer.WriteString(value);

        public void Dispose()
        {
            writer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}