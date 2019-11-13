using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class OptionalNullTests
    {
        [TestCase(new object[]{1, null, 2, null, 3}, false)]
        [TestCase(new object[]{1, null, 2, null, 3}, true)]
        public void PreserveOrSkipNullsEnumerableTest(object[] source, bool skipNulls)
        {
            var dest = Tester.TestRoundTrip<IList<object>>(Tester.GetEnumerable<object>(source), new SerializerOptions {SkipNullValues = skipNulls});
            CollectionAssert.AreEqual(skipNulls ? source.Where(item => item != null) : source, dest);
        }

        [TestCase(new object[]{1, null, 2, null, 3}, false)]
        [TestCase(new object[]{1, null, 2, null, 3}, true)]
        public void PreserveOrSkipNullsListTest(object[] source, bool skipNulls)
        {
            var dest = Tester.TestRoundTrip<IList<object>>(source, new SerializerOptions {SkipNullValues = skipNulls});
            CollectionAssert.AreEqual(skipNulls ? source.Where(item => item != null) : source, dest);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PreserveOrSkipNullsObjectTest(bool skipNulls)
        {
            var dest = Tester.TestRoundTrip<TestClass>(new TestClass {Value = null}, new SerializerOptions {SkipNullValues = skipNulls});
            Assert.AreEqual(skipNulls ? (object) 1 : null, dest.Value);
        }

        public class TestClass
        {
            public object Value { get; set; } = 1;
        }
    }
}