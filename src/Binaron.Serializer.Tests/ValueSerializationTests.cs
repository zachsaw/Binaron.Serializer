using System;
using System.IO;
using Binaron.Serializer.Tests.Extensions;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class ValueSerializationTests
    {
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void RootLevelValueTests<TDestination>(object source, TDestination expectation)
        {
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(source, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = BinaronConvert.Deserialize<TDestination>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest2 = BinaronConvert.Deserialize(stream);

            Assert.AreEqual(expectation, dest);
            Assert.AreEqual(GetExpectation(source), dest2);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void NonRootLevelValueInClassTests<TSource, TDestination>(TSource source, TDestination expectation)
        {
            var now = DateTime.UtcNow;
            var sourceClass = new TestClass<TSource> {RootValue = now, Value = source};

            using var stream = new MemoryStream();
            BinaronConvert.Serialize(sourceClass, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var destClass = BinaronConvert.Deserialize<TestClass<TDestination>>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            dynamic dest = BinaronConvert.Deserialize(stream);

            Assert.AreEqual(now, destClass.RootValue);
            Assert.AreEqual(expectation, destClass.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(GetExpectation(source), dest.Value);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void NonRootLevelValueInStructTests<TSource, TDestination>(TSource source, TDestination expectation)
        {
            var now = DateTime.UtcNow;
            var sourceStruct = new TestStruct<TSource> {RootValue = now, Value = source};

            using var stream = new MemoryStream();
            BinaronConvert.Serialize(sourceStruct, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var destStruct = BinaronConvert.Deserialize<TestStruct<TDestination>>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            dynamic dest = BinaronConvert.Deserialize(stream);

            Assert.AreEqual(now, destStruct.RootValue);
            Assert.AreEqual(expectation, destStruct.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(GetExpectation(source), dest.Value);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void RootLevelValueTests(object source, object _)
        {
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(source, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = BinaronConvert.Deserialize<object>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest2 = BinaronConvert.Deserialize(stream);

            Assert.AreEqual(GetExpectation(source), dest);
            Assert.AreEqual(GetExpectation(source), dest2);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void NonRootLevelValueInClassTests<TSource>(TSource source, object _)
        {
            var now = DateTime.UtcNow;
            var sourceClass = new TestClass<TSource> {RootValue = now, Value = source};

            using var stream = new MemoryStream();
            BinaronConvert.Serialize(sourceClass, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var destClass = BinaronConvert.Deserialize<TestClass<object>>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            dynamic dest = BinaronConvert.Deserialize(stream);

            Assert.AreEqual(now, destClass.RootValue);
            Assert.AreEqual(GetExpectation(source), destClass.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(GetExpectation(source), dest.Value);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void NonRootLevelValueInStructTests<TSource>(TSource source, object _)
        {
            var now = DateTime.UtcNow;
            var sourceStruct = new TestStruct<TSource> {RootValue = now, Value = source};

            using var stream = new MemoryStream();
            BinaronConvert.Serialize(sourceStruct, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var destStruct = BinaronConvert.Deserialize<TestStruct<object>>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            dynamic dest = BinaronConvert.Deserialize(stream);

            Assert.AreEqual(now, destStruct.RootValue);
            Assert.AreEqual(GetExpectation(source), destStruct.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(GetExpectation(source), dest.Value);
        }
        
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void RootLevelNullToValueTests<TDestination>(object _, TDestination __)
        {
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(null, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = BinaronConvert.Deserialize<TDestination>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest2 = BinaronConvert.Deserialize(stream);

            Assert.AreEqual(default(TDestination), dest);
            Assert.AreEqual(null, dest2);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void NonRootLevelNullToValueInClassTests<TDestination>(object _, TDestination __)
        {
            var now = DateTime.UtcNow;
            var sourceClass = new TestClass<object> {RootValue = now, Value = null};

            using var stream = new MemoryStream();
            BinaronConvert.Serialize(sourceClass, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var destClass = BinaronConvert.Deserialize<TestClass<TDestination>>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            dynamic dest = BinaronConvert.Deserialize(stream);

            Assert.AreEqual(now, destClass.RootValue);
            Assert.AreEqual(default(TDestination), destClass.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(null, dest.Value);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void NonRootLevelNullToValueInStructTests<TDestination>(object _, TDestination __)
        {
            var now = DateTime.UtcNow;
            var sourceStruct = new TestStruct<object> {RootValue = now, Value = null};

            using var stream = new MemoryStream();
            BinaronConvert.Serialize(sourceStruct, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var destStruct = BinaronConvert.Deserialize<TestStruct<TDestination>>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            dynamic dest = BinaronConvert.Deserialize(stream);

            Assert.AreEqual(now, destStruct.RootValue);
            Assert.AreEqual(default(TDestination), destStruct.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(null, dest.Value);
        }

        [Test]
        public void RootLevelNullToObjectTest()
        {
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(null, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = BinaronConvert.Deserialize<object>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest2 = BinaronConvert.Deserialize(stream);

            Assert.AreEqual(null, dest);
            Assert.AreEqual(null, dest2);
        }

        [Test]
        public void NonRootLevelNullToObjectInClassTest()
        {
            var now = DateTime.UtcNow;
            var sourceClass = new TestClass<object> {RootValue = now, Value = null};

            using var stream = new MemoryStream();
            BinaronConvert.Serialize(sourceClass, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var destClass = BinaronConvert.Deserialize<TestClass<object>>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            dynamic dest = BinaronConvert.Deserialize(stream);

            Assert.AreEqual(now, destClass.RootValue);
            Assert.AreEqual(null, destClass.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(null, dest.Value);
        }

        [Test]
        public void NonRootLevelNullToObjectInStructTest()
        {
            var now = DateTime.UtcNow;
            var sourceStruct = new TestStruct<object> {RootValue = now, Value = null};

            using var stream = new MemoryStream();
            BinaronConvert.Serialize(sourceStruct, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var destStruct = BinaronConvert.Deserialize<TestStruct<object>>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            dynamic dest = BinaronConvert.Deserialize(stream);

            Assert.AreEqual(now, destStruct.RootValue);
            Assert.AreEqual(null, destStruct.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(null, dest.Value);
        }

        [Test]
        public void ClassToStructTest()
        {
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(new TestClass<int> {Value = 1}, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = BinaronConvert.Deserialize<TestStruct<int>>(stream);
            Assert.AreEqual(1, dest.Value);
        }

        [Test]
        public void StructToClassTest()
        {
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(new TestStruct<int> {Value = 1}, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = BinaronConvert.Deserialize<TestClass<int>>(stream);
            Assert.AreEqual(1, dest.Value);
        }
        
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCasesOfValueTypes))]
        public void ToNullableTest<TDestination>(object source, TDestination expectation) where TDestination : struct
        {
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(source, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = BinaronConvert.Deserialize<TDestination?>(stream);
            TDestination? expected;
            if (source.GetType() != typeof(TDestination))
            {
                if (source is char || source.DynamicCast(typeof(TDestination)) == null)
                    expected = null;
                else
                    expected = expectation;
            }
            else
                expected = expectation;
            Assert.AreEqual(expected, dest);
        }
        
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfEnums))]
        public void EnumToNullableEnumTest<T>(T val) where T : struct
        {
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(val, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = BinaronConvert.Deserialize<T?>(stream);
            Assert.AreEqual(val, dest);
        }
        
        [Test]
        public void NullableStructTest()
        {
            var val = new TestStruct<int?> {Value = 1};
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(val, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = BinaronConvert.Deserialize<TestStruct<int?>?>(stream);
            Assert.AreEqual(val.Value, dest?.Value);
        }
        
        [Test]
        public void NullableStructNullValueTest()
        {
            var val = new TestStruct<int?>();
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(val, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = BinaronConvert.Deserialize<TestStruct<int?>?>(stream);
            Assert.AreEqual(null, dest?.Value);
        }

        private static object GetExpectation<TSource>(TSource source) => typeof(TSource).IsEnum ? GetEnumNumeric(source) : source;
        private static object GetExpectation(object source) => source.GetType().IsEnum ? GetEnumNumeric(source) : source;

        private static object GetEnumNumeric(object source)
        {
            switch (Type.GetTypeCode(source.GetType()))
            {
                case TypeCode.SByte:
                    return (sbyte) source;
                case TypeCode.Byte:
                    return (byte) source;
                case TypeCode.Int16:
                    return (short) source;
                case TypeCode.UInt16:
                    return (ushort) source;
                case TypeCode.Int32:
                    return (int) source;
                case TypeCode.UInt32:
                    return (uint) source;
                case TypeCode.Int64:
                    return (long) source;
                case TypeCode.UInt64:
                    return (ulong) source;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source));
            }
        }

        private class TestClass<T>
        {
            public DateTime RootValue { get; set; }
            public T Value { get; set; }
        }

        private struct TestStruct<T>
        {
            public DateTime RootValue { get; set; }
            public T Value { get; set; }
        }
    }
}