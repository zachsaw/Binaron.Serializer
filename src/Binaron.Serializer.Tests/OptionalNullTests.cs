using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class OptionalNullTests
    {
        [TestCase(false)]
        [TestCase(true)]
        public void PreserveOrSkipNullsObjectTest(bool skipNulls)
        {
            var dest = Tester.TestRoundTrip(new TestClass {Value = null}, new SerializerOptions {SkipNullValues = skipNulls});
            Assert.AreEqual(skipNulls ? (object) 1 : null, dest.Value);
        }

        private class TestClass
        {
            public object Value { get; set; } = 1;
        }
    }
}