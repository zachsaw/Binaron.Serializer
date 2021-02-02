using System;
using System.Collections.Generic;
using System.IO;
using Binaron.Serializer.CustomObject;
using Binaron.Serializer.Tests.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class ValueSerializationTests
    {
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void RootLevelValueTests<TDestination>(object source, TDestination expectation)
        {
            var (dest, dest2) = Tester.TestRoundTrip2<TDestination>(source);
        
            Assert.AreEqual(expectation, dest);
            Assert.AreEqual(GetExpectation(source), dest2);
        }
        
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void NonRootLevelValueInClassTests<TSource, TDestination>(TSource source, TDestination expectation)
        {
            var now = DateTime.UtcNow;
            var sourceClass = new TestClass<TSource> {RootValue = now, Value = source};
            (var destClass, dynamic dest) = Tester.TestRoundTrip2<TestClass<TDestination>>(sourceClass);
        
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
            (var destStruct, dynamic dest) = Tester.TestRoundTrip2<TestStruct<TDestination>>(sourceStruct);
        
            Assert.AreEqual(now, destStruct.RootValue);
            Assert.AreEqual(expectation, destStruct.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(GetExpectation(source), dest.Value);
        }
        
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void RootLevelValueTests(object source, object _)
        {
            var (dest, dest2) = Tester.TestRoundTrip2<object>(source);
        
            Assert.AreEqual(GetExpectation(source), dest);
            Assert.AreEqual(GetExpectation(source), dest2);
        }
        
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void NonRootLevelValueInClassTests<TSource>(TSource source, object _)
        {
            var now = DateTime.UtcNow;
            var sourceClass = new TestClass<TSource> {RootValue = now, Value = source};
            (var destClass, dynamic dest) = Tester.TestRoundTrip2<TestClass<object>>(sourceClass);
        
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
            (var destStruct, dynamic dest) = Tester.TestRoundTrip2<TestStruct<object>>(sourceStruct);
        
            Assert.AreEqual(now, destStruct.RootValue);
            Assert.AreEqual(GetExpectation(source), destStruct.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(GetExpectation(source), dest.Value);
        }
        
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void RootLevelNullToValueTests<TDestination>(object _, TDestination __)
        {
            var (dest, dest2) = Tester.TestRoundTrip2<TDestination>(null);
        
            Assert.AreEqual(default(TDestination), dest);
            Assert.AreEqual(null, dest2);
        }
        
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCases))]
        public void NonRootLevelNullToValueInClassTests<TDestination>(object _, TDestination __)
        {
            var now = DateTime.UtcNow;
            var sourceClass = new TestClass<object> {RootValue = now, Value = null};
            (var destClass, dynamic dest) = Tester.TestRoundTrip2<TestClass<TDestination>>(sourceClass);
        
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
            (var destStruct, dynamic dest) = Tester.TestRoundTrip2<TestStruct<TDestination>>(sourceStruct);
        
            Assert.AreEqual(now, destStruct.RootValue);
            Assert.AreEqual(default(TDestination), destStruct.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(null, dest.Value);
        }
        
        [Test]
        public void NestedStructInClassAsObjectTest()
        {
            var sourceClass = new TestClass<object> {Value = new TestStruct<object> {Value = 1}};
            var dest = Tester.TestRoundTrip<TestClass<TestStruct<object>>>(sourceClass);
        
            Assert.AreEqual(1, dest.Value.Value);
        }
        
        [Test]
        public void NestedStructInClassAsTypedObjectTest()
        {
            var sourceClass = new TestClass<TestStruct<object>> {Value = new TestStruct<object> {Value = 1}};
            var dest = Tester.TestRoundTrip(sourceClass);
        
            Assert.AreEqual(1, dest.Value.Value);
        }
        
        [Test]
        public void RootLevelNullToObjectTest()
        {
            var (dest, dest2) = Tester.TestRoundTrip2<object>(null);
        
            Assert.AreEqual(null, dest);
            Assert.AreEqual(null, dest2);
        }
        
        [Test]
        public void NonRootLevelNullToObjectInClassTest()
        {
            var now = DateTime.UtcNow;
            var sourceClass = new TestClass<object> {RootValue = now, Value = null};
            (var destClass, dynamic dest) = Tester.TestRoundTrip2(sourceClass);
        
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
            (var destStruct, dynamic dest) = Tester.TestRoundTrip2(sourceStruct);
        
            Assert.AreEqual(now, destStruct.RootValue);
            Assert.AreEqual(null, destStruct.Value);
            Assert.AreEqual(now, dest.RootValue);
            Assert.AreEqual(null, dest.Value);
        }
        
        [Test]
        public void ClassToStructTest()
        {
            var val = new TestClass<int> {Value = 1};
            var dest = Tester.TestRoundTrip<TestStruct<int>>(val);
            Assert.AreEqual(1, dest.Value);
        }
        
        [Test]
        public void StructToClassTest()
        {
            var val = new TestStruct<int> {Value = 1};
            var dest = Tester.TestRoundTrip<TestClass<int>>(val);
            Assert.AreEqual(1, dest.Value);
        }
        
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCasesOfValueTypes))]
        public void ToNullableTest<TDestination>(object source, TDestination expectation) where TDestination : struct
        {
            var dest = Tester.TestRoundTrip<TDestination?>(source);
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
            var dest = Tester.TestRoundTrip<T?>(val);
            Assert.AreEqual(val, dest);
        }
        
        [Test]
        public void NullableStructTest()
        {
            var val = new TestStruct<int?> {Value = 1};
            var dest = Tester.TestRoundTrip<TestStruct<int?>?>(val);
            Assert.AreEqual(val.Value, dest?.Value);
        }
        
        [Test]
        public void NullableStructNullValueTest()
        {
            var val = new TestStruct<int?>();
            var dest = Tester.TestRoundTrip<TestStruct<int?>?>(val);
            Assert.AreEqual(null, dest?.Value);
        }
        
        [Test]
        public void ObjectActivatorTest()
        {
            const int testVal = 100;
        
            var val = new TestClass<int?>();
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(val, stream, new SerializerOptions {SkipNullValues = true});
            stream.Seek(0, SeekOrigin.Begin);
            var dest = BinaronConvert.Deserialize<TestClass<int?>>(stream, new DeserializerOptions {ObjectActivator = new MyObjectActivator(testVal)});
            Assert.AreEqual(testVal, dest.Value);
        }

        [Test]
        public void DeserializeToDateTimeLocal()
        {
            var dt = DateTime.Now;
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(dt, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = BinaronConvert.Deserialize<DateTime>(stream, new DeserializerOptions {TimeZoneInfo = TimeZoneInfo.Local});
            Assert.AreEqual(dt, dest);
        }

        public class BaseConfig
        {
            public string Name { get; set; } = "base name";
        }

        public class TestConfig
        {
            public TestConfig(BaseConfig config)
            {
                Name = config.Name;
            }

            public string Name { get; set; }
            public double Score { get; set; }
            public int[] Numbers { get; set; }
            public Dictionary<string, string> Dict { get; set; }
            public HashSet<string> Ids { get; set; }
            public SubConfig SubConfig { get; set; }
        }

        public class SubConfig
        {
            public SubConfig(BaseConfig config)
            {
                Name = $"sub {config.Name}";
            }

            public string Name { get; set; }
            public double Score { get; set; }
            public int[] Numbers { get; set; }
            public Dictionary<string, string> Dict { get; set; }
            public HashSet<string> Ids { get; set; }
        }

        [Test]
        public void ServiceProviderObjectActivatorTest()
        {
            var services = new ServiceCollection();
            services.AddSingleton(new BaseConfig());

            var obj = new
            {
                Score = 1.0d,
                Numbers = new[] {3, 2, 1},
                Dict = new
                {
                    A = "a",
                    B = "b"
                },
                Ids = new[] {"Id1", "Id2"},
                SubConfig = new
                {
                    Numbers = new[] {3, 2, 1},
                    Dict = new
                    {
                        A = "a",
                        B = "b"
                    },
                    Ids = new[] {"Id1", "Id2"},
                    Extra = "extra"
                }
            };

            var serviceProvider = services.BuildServiceProvider();
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(obj, stream, new SerializerOptions {SkipNullValues = true});
            stream.Position = 0;
            var result = BinaronConvert.Deserialize<TestConfig>(stream, new DeserializerOptions {ObjectActivator = new ObjectActivator(serviceProvider)});
            Assert.AreEqual(obj.SubConfig.Ids, result.SubConfig.Ids);
        }

        private class ObjectActivator : IObjectActivator
        {
            private readonly IServiceProvider serviceProvider;

            public ObjectActivator(IServiceProvider serviceProvider)
            {
                this.serviceProvider = serviceProvider;
            }

            public object Create(Type type)
            {
                return ActivatorUtilities.CreateInstance(serviceProvider, type);
            }
        }

        [Test]
        public void PopulateObjectTest()
        {
            const int testVal = 100;

            var val = new TestClass<int?> {RootValue = DateTime.MinValue};
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(val, stream, new SerializerOptions {SkipNullValues = true});
            stream.Seek(0, SeekOrigin.Begin);
            var dest = new TestClass<int?> {Value = 100};
            BinaronConvert.Populate(dest, stream);
            Assert.AreEqual(testVal, dest.Value);
            Assert.AreEqual(DateTime.MinValue, dest.RootValue);
        }

        private class MyObjectActivator : IObjectActivator
        {
            private readonly int value;

            public MyObjectActivator(int value)
            {
                this.value = value;
            }

            public object Create(Type type)
            {
                var result = (TestClass<int?>) Activator.CreateInstance(type);
                result.Value = value;
                return result;
            }
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

        [TestCase(1)]
        [TestCase(null)]
        public void NullableTest1(int? value)
        {
            using (var ms1 = new MemoryStream())
            {
                BinaronConvert.Serialize<int?>(value, ms1);
                using (var ms2 = new MemoryStream(ms1.ToArray()))
                {
                    Assert.AreEqual(value, BinaronConvert.Deserialize<int?>(ms2));
                    
                }
            }
        }

        [TestCase(1, "a")]
        [TestCase(null, null)]
        [TestCase(1, null)]
        [TestCase(null, "abcd")]

        public void MemberSetterNullableType1(int? v1, string v2)
        {
            TestClass1 tc1 = new TestClass1() { IntValue = v1, StringValue = v2 };
            using (MemoryStream ms = new MemoryStream())
            {
                BinaronConvert.Serialize(tc1, ms);
                using (MemoryStream ms1 = new MemoryStream(ms.ToArray()))
                {
                    var tc2 = BinaronConvert.Deserialize<TestClass1>(ms1);
                    Assert.AreEqual(v1, tc2.IntValue);
                    Assert.AreEqual(v2, tc2.StringValue);
                }
            }
        }

        [TestCase(1, "a")]
        [TestCase(null, null)]
        [TestCase(1, null)]
        [TestCase(null, "abcd")]

        public void MemberSetterNullableType2(int? v1, string v2)
        {
            List<TestClass1> tc1 = new List<TestClass1>() { new TestClass1() { IntValue = v1, StringValue = v2 } };
            using (MemoryStream ms = new MemoryStream())
            {
                BinaronConvert.Serialize(tc1, ms);
                using (MemoryStream ms1 = new MemoryStream(ms.ToArray()))
                {
                    var tc2 = BinaronConvert.Deserialize<List<TestClass1>>(ms1);
                    Assert.AreEqual(1, tc2.Count);
                    Assert.AreEqual(v1, tc2[0].IntValue);
                    Assert.AreEqual(v2, tc2[0].StringValue);
                }
            }
        }

        private class TestClass1
        {
            public int? IntValue { get; set; }
            public string StringValue { get; set; }
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