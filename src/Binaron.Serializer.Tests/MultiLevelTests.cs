using System.Collections.Generic;
using System.IO;
using System.Linq;
using Binaron.Serializer.Extensions;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class MultiLevelTests
    {
        [Test]
        public void EnumerableObjectEnumerableTest()
        {
            var multiLevelEnumerables = new InnerMultiLevelEnumerablesTestClass().Yield();

            using var stream = new MemoryStream();
            BinaronConvert.Serialize(multiLevelEnumerables, stream, new SerializerOptions {SkipNullValues = true});
            stream.Position = 0;
            var response = BinaronConvert.Deserialize<IEnumerable<InnerMultiLevelEnumerablesTestClass>>(stream);
            var inner = response.First();
            CollectionAssert.AreEqual(Enumerable.Range(0, 2), inner.MultiLevelEnumerables.First());
            CollectionAssert.AreEqual(Enumerable.Range(3, 4), inner.MultiLevelEnumerables.Last());
        }

        private class InnerMultiLevelEnumerablesTestClass
        {
            public IEnumerable<IEnumerable<int>> MultiLevelEnumerables { get; set; } = CreateMultiLevelEnumerables();

            private static IEnumerable<IEnumerable<int>> CreateMultiLevelEnumerables()
            {
                yield return Enumerable.Range(0, 2);
                yield return Enumerable.Range(3, 4);
            }
        }
    }
}