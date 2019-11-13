using System.IO;
using System.Linq;
using NUnit.Framework;
using BinaryWriter = Binaron.Serializer.Infrastructure.BinaryWriter;

namespace Binaron.Serializer.Tests
{
    public class BinaryReaderWriterTests
    {
        [Test]
        public void UnmanagedTest()
        {
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(1);
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
        public void StringTest()
        {
            const string str = "Test";
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                writer.WriteString(str);
            }

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new Infrastructure.BinaryReader(stream);
            Assert.AreEqual(str, reader.ReadString());
            Assert.Throws<EndOfStreamException>(() => reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [Test]
        public void EmptyStringTest()
        {
            var str = string.Empty;
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                writer.WriteString(str);
            }

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new Infrastructure.BinaryReader(stream);
            Assert.AreEqual(str, reader.ReadString());
            Assert.Throws<EndOfStreamException>(() => reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [TestCase(1 * 1024 * 1024)]
        [TestCase(2 * 1024 * 1024)]
        public void ManyUnmanagedBoundaryTests(int bufferSize)
        {
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                for (var i = 0; i < bufferSize / sizeof(int); i++)
                    writer.Write(i);
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
        public void ManyStringBoundaryTests(int bufferSize)
        {
            const string str = "Test";
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                for (var i = 0; i < bufferSize / (sizeof(int) + sizeof(char) * str.Length); i++)
                    writer.WriteString(str);
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
        public void ManyUnmanagedTests(int bufferSize)
        {
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                for (var i = 0; i < 1 + bufferSize / sizeof(int); i++)
                    writer.Write(i);
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
        public void ManyStringTests(int bufferSize)
        {
            const string str = "Test";
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                for (var i = 0; i < 1 + bufferSize / (sizeof(int) + sizeof(char) * str.Length); i++)
                    writer.WriteString(str);
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
        public void EndOfStreamTests(int bufferSize)
        {
            var str = CreateString(bufferSize / sizeof(char));
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                writer.WriteString(str);
            }

            using var newStream = new MemoryStream(stream.ToArray().Take((int) stream.Length - sizeof(char)).ToArray());
            using var reader = new Infrastructure.BinaryReader(newStream);
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [TestCase(1 * 1024 * 1024)]
        [TestCase(2 * 1024 * 1024)]
        public void StringThatCanJustFitBufferTests(int bufferSize)
        {
            var str = CreateString(bufferSize / sizeof(char));
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                writer.WriteString(str);
            }

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new Infrastructure.BinaryReader(stream);
            Assert.AreEqual(str, reader.ReadString());
            Assert.Throws<EndOfStreamException>(() => reader.Read<int>());
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [TestCase(2 * 1024 * 1024)]
        [TestCase(4 * 1024 * 1024)]
        public void StringThatCannotFitBufferTests(int bufferSize)
        {
            var str = CreateString((bufferSize + sizeof(char)) / sizeof(char));
            using var stream = new MemoryStream();
            using (var writer = new BinaryWriter(stream))
            {
                writer.WriteString(str);
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