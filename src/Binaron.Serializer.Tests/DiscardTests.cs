using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Binaron.Serializer.Tests.Extensions;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class DiscardTests
    {
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public async ValueTask DiscardNoSetterPropertyTests<TSource>(TSource source)
        {
            var dest = await Tester.TestRoundTrip(new TestClassNoSetter<TSource>(source));
            Assert.AreEqual(default(TSource), dest.Value);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public async ValueTask DiscardNoGetterPropertyTests<TSource>(TSource source)
        {
            var dest = await Tester.TestRoundTrip(new TestClassNoGetter<TSource>(source));
            Assert.AreEqual(default(TSource), dest.GetValue());
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public async ValueTask DiscardObjectTests<TSource>(TSource source)
        {
            var dest = await Tester.TestRoundTrip<TestEnumerable<KeyValuePair<string, TSource>>>(new TestClass<TSource> {Value = source});
            Assert.AreEqual(0, dest.Count());
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public async ValueTask DiscardListTests<TSource>(TSource source)
        {
            var dest = await Tester.TestRoundTrip<TestEnumerable<TSource>>(new[] {source});
            Assert.AreEqual(0, dest.Count());
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public async ValueTask DiscardEnumerableTests<TSource>(TSource source)
        {
            var dest = await Tester.TestRoundTrip<TestEnumerable<TSource>>(GetEnumerable(new[] {source}));
            Assert.AreEqual(0, dest.Count());
        }

        [Test]
        public async ValueTask DiscardDictionaryTest()
        {
            var dest = await Tester.TestRoundTrip<TestEnumerable<KeyValuePair<string, string>>>(new Dictionary<string, string> {{"key", "value"}});
            Assert.AreEqual(0, dest.Count());
        }

        [Test]
        public async ValueTask DiscardObjectAsIntTest()
        {
            var dest = await Tester.TestRoundTrip<int>(new TestClass<int> {Value = 1});
            Assert.AreEqual(0, dest);
        }

        [Test]
        public async ValueTask DiscardListAsIntTest()
        {
            var dest = await Tester.TestRoundTrip<int>(new[] {1});
            Assert.AreEqual(0, dest);
        }

        [Test]
        public async ValueTask DiscardEnumerableAsIntTest()
        {
            var dest = await Tester.TestRoundTrip<int>(GetEnumerable(new[] {1}));
            Assert.AreEqual(0, dest);
        }

        [Test]
        public async ValueTask DiscardDictionaryAsIntTest()
        {
            var dest = await Tester.TestRoundTrip<int>(new Dictionary<string, string> {{"key", "value"}});
            Assert.AreEqual(0, dest);
        }

        [Test]
        public async ValueTask DiscardListOfListTest()
        {
            var dest = await Tester.TestRoundTrip<int[]>(new[] {new[] {1}});
            Assert.AreEqual(0, dest.Length);
        }

        [Test]
        public async ValueTask DiscardListOfEnumerableTest()
        {
            var dest = await Tester.TestRoundTrip<int[]>(new[] {GetEnumerable(new[] {1})});
            Assert.AreEqual(0, dest.Length);
        }

        [Test]
        public async ValueTask DiscardListOfDictionaryTest()
        {
            var dest = await Tester.TestRoundTrip<int[]>(new[] {new Dictionary<string, string> {{"key", "value"}}});
            Assert.AreEqual(0, dest.Length);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public async ValueTask DiscardCustomEnumerableTests(dynamic value)
        {
            var dest = await Tester.TestRoundTrip<TestEnumerable<object>>(GetEnumerable(new[] {value}));
            Assert.AreEqual(0, dest.Count());
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public async ValueTask DiscardCustomListTests(dynamic value)
        {
            var dest = await Tester.TestRoundTrip<TestEnumerable<object>>(new[] {value});
            Assert.AreEqual(0, dest.Count());
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public async ValueTask DiscardCustomDictionaryTests(dynamic value)
        {
            var dest = await Tester.TestRoundTrip<TestEnumerable<object>>(new Hashtable {{value, value}});
            Assert.AreEqual(0, dest.Count());
        }

        [Test]
        public async ValueTask DiscardCustomEnumerableKvpTest()
        {
            var dest = await Tester.TestRoundTrip<TestEnumerable<object>>(GetEnumerable(new[] {new KeyValuePair<string, string>("key", "value")}));
            Assert.AreEqual(0, dest.Count());
        }

        [Test]
        public async ValueTask DiscardCustomDestEnumerableDictionaryTest()
        {
            var dest = await Tester.TestRoundTrip<TestEnumerable<KeyValuePair<string, string>>>(new Dictionary<string, string> {{"key", "value"}});
            Assert.AreEqual(0, dest.Count());
        }
        
        [Test]
        public async ValueTask DiscardObjectAsDictionaryTest()
        {
            var dest = await Tester.TestRoundTrip<TestEnumerable<KeyValuePair<string, string>>>(new TestClass<int> {Value = 1});
            Assert.AreEqual(0, dest.Count());
        }

        private static IEnumerable<T> GetEnumerable<T>(IEnumerable<T> list)
        {
            foreach (var item in list)
                yield return item;
        }

        private sealed class TestClass<T>
        {
            public T Value { get; set; }
        }

        private sealed class TestClassNoSetter<T>
        {
            public T Value { get; }

            public TestClassNoSetter()
            {
            }

            public TestClassNoSetter(T value)
            {
                Value = value;
            }
        }

        private sealed class TestClassNoGetter<T>
        {
            private T val;

            public T Value
            {
                set => val = value;
            }

            public TestClassNoGetter()
            {
            }

            public TestClassNoGetter(T value)
            {
                val = value;
            }

            public T GetValue() => val;
        }

        private sealed class TestEnumerable<T> : IEnumerable<T>
        {
            public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>) Array.Empty<T>()).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}