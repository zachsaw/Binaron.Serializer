using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Binaron.Serializer.Tests.Extensions;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class DictionarySerializationTests
    {
        [TestCaseSource(nameof(AllListCases))]
        public void RootDictionaryTests(Type destType)
        {
            var source = new Dictionary<string, string> {{"a key", "a value"}};
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(source, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = (IEnumerable) Converter.Deserialize(destType, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest2 = (IEnumerable) BinaronConvert.Deserialize(stream);
            Assert.AreEqual(1, dest.Count());
            Assert.AreEqual(1, dest2.Count());
        }

        [TestCaseSource(nameof(AllListCases))]
        public void RootReadOnlyDictionaryTests(Type destType)
        {
            IReadOnlyDictionary<string, string> source = new Dictionary<string, string> {{"a key", "a value"}};
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(source, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest = (IEnumerable) Converter.Deserialize(destType, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var dest2 = (IEnumerable) BinaronConvert.Deserialize(stream);
            Assert.AreEqual(1, dest.Count());
            Assert.AreEqual(1, dest2.Count());
        }

        [Test]
        public void ClassWithDictionaryTest()
        {
            var source = new ClassWithDictionary {Dictionary = new Dictionary<string, object> {{"a key", "a value"}}};
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(source, stream);
            stream.Seek(0, SeekOrigin.Begin);
            dynamic dest = BinaronConvert.Deserialize(stream);
            Assert.AreEqual("a value", ((IDictionary<string, object>) dest.Dictionary)["a key"]);
        }

        [Test]
        public void ClassWithReadOnlyDictionaryTest()
        {
            var source = new ClassWithReadOnlyDictionary {Dictionary = new Dictionary<string, object> {{"a key", "a value"}}};
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(source, stream);
            stream.Seek(0, SeekOrigin.Begin);
            dynamic dest = BinaronConvert.Deserialize(stream);
            Assert.AreEqual("a value", ((IDictionary<string, object>) dest.Dictionary)["a key"]);
        }

        [Test]
        public void IntToNullableIntTest()
        {
            const int val = int.MinValue;
            var dest = Tester.TestRoundTrip<IDictionary<int?, int?>>(new Dictionary<int, int>{{val, val}});
            Assert.AreEqual((val, val), (dest.Single().Key, dest.Single().Value));
            var dest2 = Tester.TestRoundTrip<IReadOnlyDictionary<int?, int?>>(new Dictionary<int, int>{{val, val}});
            Assert.AreEqual((val, val), (dest2.Single().Key, dest2.Single().Value));
        }

        [Test]
        public void EnumToNullableEnumTest()
        {
            const TestByteEnum val = TestByteEnum.Max;
            var dest = Tester.TestRoundTrip<IDictionary<TestByteEnum?, TestByteEnum?>>(new Dictionary<TestByteEnum, TestByteEnum>{{val, val}});
            Assert.AreEqual((val, val), (dest.Single().Key, dest.Single().Value));
            var dest2 = Tester.TestRoundTrip<IReadOnlyDictionary<TestByteEnum?, TestByteEnum?>>(new Dictionary<TestByteEnum, TestByteEnum>{{val, val}});
            Assert.AreEqual((val, val), (dest2.Single().Key, dest2.Single().Value));
        }
        
        [Test]
        public void NullableStructTest()
        {
            var val = new TestStruct {Value = 1, NullableValue = 2};
            var dest = Tester.TestRoundTrip<IDictionary<TestStruct?, TestStruct?>>(new Dictionary<TestStruct, TestStruct>{{val, val}});
            Assert.AreEqual(val.Value, dest.Single().Key?.Value);
            Assert.AreEqual(val.NullableValue, dest.Single().Key?.NullableValue);
            Assert.AreEqual(val.Value, dest.Single().Value?.Value);
            Assert.AreEqual(val.NullableValue, dest.Single().Value?.NullableValue);
            var dest2 = Tester.TestRoundTrip<IReadOnlyDictionary<TestStruct?, TestStruct?>>(new Dictionary<TestStruct, TestStruct>{{val, val}});
            Assert.AreEqual(val.Value, dest2.Single().Key?.Value);
            Assert.AreEqual(val.NullableValue, dest2.Single().Key?.NullableValue);
            Assert.AreEqual(val.Value, dest2.Single().Value?.Value);
            Assert.AreEqual(val.NullableValue, dest2.Single().Value?.NullableValue);
        }
        
        public struct TestStruct
        {
            public int Value { get; set; }
            public int? NullableValue { get; set; }
        }
        
        private static IEnumerable<TestCaseData> AllListCases()
        {
            var listTypes = new[]
            {
                typeof(Dictionary<,>),
                typeof(IDictionary<,>),
                typeof(IDictionary),
                typeof(IReadOnlyDictionary<,>),
                typeof(ICollection<>),
                typeof(ICollection),
                typeof(IEnumerable<>),
                typeof(IEnumerable),
                typeof(TestDictionary<,>)
            };

            var keyType = typeof(string);
            var valueType = typeof(string);
            var kvpElementType = typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);

            foreach (var type in listTypes)
            {
                var actualType = type == typeof(Array) ? kvpElementType.MakeArrayType() : type;
                if (actualType.IsGenericType) 
                    actualType = actualType.MakeGenericType(actualType.GetGenericArguments().Length == 1 ? new[] {kvpElementType} : new[]{keyType, valueType});
                yield return new TestCaseData(actualType);
            }
        }
        
        private class ClassWithReadOnlyDictionary
        {
            public IReadOnlyDictionary<string, object> Dictionary { get; set; }
        }

        private class ClassWithDictionary
        {
            public Dictionary<string, object> Dictionary { get; set; }
        }

        private class TestDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
        {
            private readonly Dictionary<TKey, TValue> backing = new Dictionary<TKey, TValue>();
            
            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return backing.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                backing.Add(item.Key, item.Value);
            }

            public void Clear()
            {
                backing.Clear();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return backing.Contains(item);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                return backing.Remove(item.Key);
            }

            public int Count => backing.Count;
            public bool IsReadOnly => false;
            public void Add(TKey key, TValue value)
            {
                backing.Add(key, value);
            }

            public bool ContainsKey(TKey key)
            {
                return backing.ContainsKey(key);
            }

            public bool Remove(TKey key)
            {
                return backing.Remove(key);
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                return backing.TryGetValue(key, out value);
            }

            public TValue this[TKey key]
            {
                get => backing[key];
                set => backing[key] = value;
            }

            IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

            IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

            public ICollection<TKey> Keys => backing.Keys;
            public ICollection<TValue> Values => backing.Values;
        }
    }
}