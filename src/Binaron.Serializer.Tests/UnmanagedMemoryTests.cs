using System;
using Binaron.Serializer.Infrastructure;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class UnmanagedMemoryTests
    {
        [Test]
        public unsafe void CanReadWriteTest()
        {
            using var memory = new UnmanagedMemory<int>(1);
            memory.Data[0] = int.MinValue;
            Assert.AreEqual(1, memory.Length);
            Assert.AreEqual(int.MinValue, memory.Data[0]);
        }

        [Test]
        public void ZeroLengthTest()
        {
            using var memory = new UnmanagedMemory<int>(0);
            Assert.AreEqual(0, memory.Length);
        }

        [Test]
        public void InvalidLengthTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using var _ = new UnmanagedMemory<int>(-1);
            });
        }

        [Test]
        public unsafe void DoubleDisposeTest()
        {
            using var memory = new UnmanagedMemory<int>(0);
            memory.Dispose();
            memory.Dispose();
            Assert.Throws<NullReferenceException>(() =>
            {
                var _ = *memory.Data;
            });
        }
    }
}