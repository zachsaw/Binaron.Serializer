using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Binaron.Serializer.Infrastructure
{
    internal sealed class WriterState : IDisposable
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
        public async Task Write<T>(T value) where T : unmanaged => await writer.Write(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task WriteString(string value) => await writer.WriteString(value);

        public void Dispose()
        {
            writer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}