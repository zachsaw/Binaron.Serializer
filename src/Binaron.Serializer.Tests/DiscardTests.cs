using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class DiscardTests
    {
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public void DiscardNoSetterPropertyTests<TSource>(TSource source)
        {
            var dest = Tester.TestRoundTrip(new TestClassNoSetter<TSource>(source));
            Assert.AreEqual(default(TSource), dest.Value);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public void DiscardNoGetterPropertyTests<TSource>(TSource source)
        {
            var dest = Tester.TestRoundTrip(new TestClassNoGetter<TSource>(source));
            Assert.AreEqual(default(TSource), dest.GetValue());
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public void DiscardObjectTests<TSource>(TSource source)
        {
            var dest = Tester.TestRoundTrip<TestEnumerable<KeyValuePair<string, TSource>>>(new TestClass<TSource> {Value = source});
            Assert.AreEqual(0, dest.Count());
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public void DiscardListTests<TSource>(TSource source)
        {
            var dest = Tester.TestRoundTrip<TestEnumerable<TSource>>(new[] {source});
            Assert.AreEqual(0, dest.Count());
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public void DiscardEnumerableTests<TSource>(TSource source)
        {
            var dest = Tester.TestRoundTrip<TestEnumerable<TSource>>(GetEnumerable(new[] {source}));
            Assert.AreEqual(0, dest.Count());
        }

        [Test]
        public void DiscardDictionaryTest()
        {
            var dest = Tester.TestRoundTrip<TestEnumerable<KeyValuePair<string, string>>>(new Dictionary<string, string> {{"key", "value"}});
            Assert.AreEqual(0, dest.Count());
        }

        [Test]
        public void DiscardObjectAsIntTest()
        {
            var dest = Tester.TestRoundTrip<int>(new TestClass<int> {Value = 1});
            Assert.AreEqual(0, dest);
        }

        [Test]
        public void DiscardListAsIntTest()
        {
            var dest = Tester.TestRoundTrip<int>(new[] {1});
            Assert.AreEqual(0, dest);
        }

        [Test]
        public void DiscardEnumerableAsIntTest()
        {
            var dest = Tester.TestRoundTrip<int>(GetEnumerable(new[] {1}));
            Assert.AreEqual(0, dest);
        }

        [Test]
        public void DiscardDictionaryAsIntTest()
        {
            var dest = Tester.TestRoundTrip<int>(new Dictionary<string, string> {{"key", "value"}});
            Assert.AreEqual(0, dest);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public void DiscardListOfListTest<T>(T element)
        {
            var dest = Tester.TestRoundTrip<int[]>(new[] {new[] {element}});
            Assert.AreEqual(0, dest.Length);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public void DiscardListOfEnumerableTest<T>(T element)
        {
            var dest = Tester.TestRoundTrip<int[]>(new[] {GetEnumerable(new[] {element})});
            Assert.AreEqual(0, dest.Length);
        }

        [Test]
        public void DiscardListOfListOfClassesTest()
        {
            var dest = Tester.TestRoundTrip<int[]>(new[] {new[] {new TestClass<int>()}});
            Assert.AreEqual(0, dest.Length);
        }

        [Test]
        public void DiscardListOfDictionaryTest()
        {
            var dest = Tester.TestRoundTrip<int[]>(new[] {new Dictionary<string, string> {{"key", "value"}}});
            Assert.AreEqual(0, dest.Length);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public void DiscardCustomEnumerableTests(dynamic value)
        {
            var dest = Tester.TestRoundTrip<TestEnumerable<object>>(GetEnumerable(new[] {value}));
            Assert.AreEqual(0, dest.Count());
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public void DiscardCustomListTests(dynamic value)
        {
            var dest = Tester.TestRoundTrip<TestEnumerable<object>>(new[] {value});
            Assert.AreEqual(0, dest.Count());
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestCaseOfValues))]
        public void DiscardCustomDictionaryTests(dynamic value)
        {
            var dest = Tester.TestRoundTrip<TestEnumerable<object>>(new Hashtable {{value, value}});
            Assert.AreEqual(0, dest.Count());
        }

        [Test]
        public void DiscardCustomEnumerableKvpTest()
        {
            var dest = Tester.TestRoundTrip<TestEnumerable<object>>(GetEnumerable(new[] {new KeyValuePair<string, string>("key", "value")}));
            Assert.AreEqual(0, dest.Count());
        }

        [Test]
        public void DiscardCustomDestEnumerableDictionaryTest()
        {
            var dest = Tester.TestRoundTrip<TestEnumerable<KeyValuePair<string, string>>>(new Dictionary<string, string> {{"key", "value"}});
            Assert.AreEqual(0, dest.Count());
        }
        
        [Test]
        public void DiscardObjectAsDictionaryTest()
        {
            var dest = Tester.TestRoundTrip<TestEnumerable<KeyValuePair<string, string>>>(new TestClass<int> {Value = 1});
            Assert.AreEqual(0, dest.Count());
        }

        private static IEnumerable<T> GetEnumerable<T>(IEnumerable<T> list)
        {
            foreach (var item in list)
                yield return item;
        }

        private class TestClass<T>
        {
            public T Value { get; set; }
        }

        private class TestClassNoSetter<T>
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

        private class TestClassNoGetter<T>
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

        private class TestEnumerable<T> : IEnumerable<T>
        {
            public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>) Array.Empty<T>()).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}