using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Binaron.Serializer.Infrastructure
{
    internal class WriterState : IDisposable
    {
        private readonly BinaryWriter writer;

        public WriterState(Stream stream, SerializerOptions options)
        {
            writer = new BinaryWriter(stream);
            SkipNullValues = options.SkipNullValues;
        }
        
        ~WriterState()
        {
            Dispose();
        }
        
        public bool SkipNullValues { get; }

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