using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using BinaryWriter = Binaron.Serializer.Infrastructure.BinaryWriter;

namespace Binaron.Serializer.Tests
{
    public class BinaryReaderWriterTests
    {
        [Test]
        public async ValueTask UnmanagedTest()
        {
            await using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                await writer.Write(1);
                await writer.Flush();
                writer.Dispose(); // double dispose is OK
            }

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new Infrastructure.BinaryReader(stream);
            Assert.AreEqual(1, reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
            reader.Dispose(); // double dispose is OK
        }

        [Test]
        public async ValueTask StringTest()
        {
            const string str = "Test";
            await using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                await writer.WriteString(str);
                await writer.Flush();
            }

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new Infrastructure.BinaryReader(stream);
            Assert.AreEqual(str, reader.ReadString());
            Assert.Throws<EndOfStreamException>(() => reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [Test]
        public async ValueTask EmptyStringTest()
        {
            var str = string.Empty;
            await using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                await writer.WriteString(str);
                await writer.Flush();
            }

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new Infrastructure.BinaryReader(stream);
            Assert.AreEqual(str, reader.ReadString());
            Assert.Throws<EndOfStreamException>(() => reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [TestCase(1 * 1024 * 1024)]
        [TestCase(2 * 1024 * 1024)]
        public async ValueTask ManyUnmanagedBoundaryTests(int bufferSize)
        {
            await using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                for (var i = 0; i < bufferSize / sizeof(int); i++)
                    await writer.Write(i);
                await writer.Flush();
            }

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new Infrastructure.BinaryReader(stream);
            for (var i = 0; i < bufferSize / sizeof(int); i++)
                Assert.AreEqual(i, reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [TestCase(1 * 1024 * 1024)]
        [TestCase(2 * 1024 * 1024)]
        public async ValueTask ManyStringBoundaryTests(int bufferSize)
        {
            const string str = "Test";
            await using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                for (var i = 0; i < bufferSize / (sizeof(int) + sizeof(char) * str.Length); i++)
                    await writer.WriteString(str);
                await writer.Flush();
            }

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new Infrastructure.BinaryReader(stream);
            for (var i = 0; i < bufferSize / (sizeof(int) + sizeof(char) * str.Length); i++)
                Assert.AreEqual(str, reader.ReadString());
            Assert.Throws<EndOfStreamException>(() => reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [TestCase(1 * 1024 * 1024)]
        [TestCase(2 * 1024 * 1024)]
        public async ValueTask ManyUnmanagedTests(int bufferSize)
        {
            await using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                for (var i = 0; i < 1 + bufferSize / sizeof(int); i++)
                    await writer.Write(i);
                await writer.Flush();
            }

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new Infrastructure.BinaryReader(stream);
            for (var i = 0; i < 1 + bufferSize / sizeof(int); i++)
                Assert.AreEqual(i, reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [TestCase(1 * 1024 * 1024)]
        [TestCase(2 * 1024 * 1024)]
        public async ValueTask ManyStringTests(int bufferSize)
        {
            const string str = "Test";
            await using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                for (var i = 0; i < 1 + bufferSize / (sizeof(int) + sizeof(char) * str.Length); i++)
                    await writer.WriteString(str);
                await writer.Flush();
            }

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new Infrastructure.BinaryReader(stream);
            for (var i = 0; i < 1 + bufferSize / (sizeof(int) + sizeof(char) * str.Length); i++)
                Assert.AreEqual(str, reader.ReadString());
            Assert.Throws<EndOfStreamException>(() => reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }
        
        [TestCase(sizeof(char))]
        [TestCase(2 * 1024 * 1024 + sizeof(char))]
        public async ValueTask EndOfStreamTests(int bufferSize)
        {
            var str = CreateString(bufferSize / sizeof(char));
            await using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                await writer.WriteString(str);
                await writer.Flush();
            }

            await using var newStream = new MemoryStream(stream.ToArray().Take((int) stream.Length - sizeof(char)).ToArray());
            using var reader = new Infrastructure.BinaryReader(newStream);
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [TestCase(1 * 1024 * 1024)]
        [TestCase(2 * 1024 * 1024)]
        public async ValueTask StringThatCanJustFitBufferTests(int bufferSize)
        {
            var str = CreateString(bufferSize / sizeof(char));
            await using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                await writer.WriteString(str);
                await writer.Flush();
            }

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new Infrastructure.BinaryReader(stream);
            Assert.AreEqual(str, reader.ReadString());
            Assert.Throws<EndOfStreamException>(() => reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [TestCase(2 * 1024 * 1024)]
        [TestCase(4 * 1024 * 1024)]
        public async ValueTask StringThatCannotFitBufferTests(int bufferSize)
        {
            var str = CreateString((bufferSize + sizeof(char)) / sizeof(char));
            await using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                await writer.WriteString(str);
                await writer.Flush();
            }

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new Infrastructure.BinaryReader(stream);
            Assert.AreEqual(str, reader.ReadString());
            Assert.Throws<EndOfStreamException>(() => reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        private static string CreateString(int length)
        {
            var chars = new char[length];
            for (var i = 0; i < chars.Length; i++) 
                chars[i] = (char) ('a' + i % 26);
            return new string(chars);
        }
    }
}