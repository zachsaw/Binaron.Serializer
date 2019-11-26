using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Binaron.Serializer.Infrastructure
{
    internal sealed class WriterState : IAsyncDisposable
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

        public async ValueTask DisposeAsync()
        {
            await writer.Flush();
            Dispose();
        }

        private void Dispose()
        {
            writer.Dispose();
            GC.SuppressFinalize(this);
        }

        public bool SkipNullValues { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask Write<T>(T value) where T : unmanaged => await writer.Write(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async ValueTask WriteString(string value) => await writer.WriteString(value);
    }
}