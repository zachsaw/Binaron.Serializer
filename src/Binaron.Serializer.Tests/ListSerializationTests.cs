using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Binaron.Serializer.Tests.Extensions;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class ListSerializationTests
    {
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.AllListCases))]
        public async ValueTask RootListTest<TSourceItem>(Type sourceType, Type destType, object list, TSourceItem sourceItem, object destItem)
        {
            var source = list.DynamicCast(sourceType);
            if (source == null)
                throw new InvalidCastException($"Bad test case. Can't cast source with type of '{list.GetType()}' to '{sourceType}'");

            // Empty
            {
                await using var stream = new MemoryStream();
                await BinaronConvert.Serialize(source, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest = (IEnumerable) Converter.Deserialize(destType, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest2 = (IEnumerable) BinaronConvert.Deserialize(stream);
                Assert.AreEqual(0, dest.Count());
                Assert.AreEqual(0, dest2.Count());
            }
            // Single
            {
                source = AddItem<TSourceItem>(sourceType, sourceItem, source);
                await using var stream = new MemoryStream();
                await BinaronConvert.Serialize(source, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest = (IEnumerable) Converter.Deserialize(destType, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest2 = (IEnumerable) BinaronConvert.Deserialize(stream);
                Assert.AreEqual(!IsValid(sourceItem, destItem, destType) ? 0 : 1, dest.Count());
                Assert.AreEqual(1, dest2.Count());
            }
            // Multi
            {
                source = AddItem<TSourceItem>(sourceType, sourceItem, source);
                source = AddItem<TSourceItem>(sourceType, sourceItem, source);
                await using var stream = new MemoryStream();
                await BinaronConvert.Serialize(source, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest = (IEnumerable) Converter.Deserialize(destType, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest2 = (IEnumerable) BinaronConvert.Deserialize(stream);
                Assert.AreEqual(!IsValid(sourceItem, destItem, destType) ? 0 : 3, dest.Count());
                Assert.AreEqual(3, dest2.Count());
            }
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestClassAndTestStruct))]
        public async ValueTask ClassesAndStructsInListTest<TType>(IList<TType> _) where TType : Tester.ITestBase, new()
        {
            // Empty
            {
                await using var stream = new MemoryStream();
                await BinaronConvert.Serialize(new TType[0], stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest = BinaronConvert.Deserialize<TType[]>(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest2 = (IEnumerable) BinaronConvert.Deserialize(stream);
                Assert.AreEqual(0, dest.Length);
                Assert.AreEqual(0, dest2.Count());
            }
            // Single
            {
                await using var stream = new MemoryStream();
                await BinaronConvert.Serialize(new[] {new TType{Value=1}}, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest = BinaronConvert.Deserialize<TType[]>(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest2 = ((IEnumerable) BinaronConvert.Deserialize(stream)).ToArray();
                Assert.AreEqual(1, dest.Length);
                Assert.AreEqual(1, dest.Single().Value);
                Assert.AreEqual(1, dest2.Length);
                Assert.AreEqual(1, ((dynamic) dest2.Single()).Value);
            }
            // Multi
            {
                await using var stream = new MemoryStream();
                await BinaronConvert.Serialize(new[] {new TType{Value=1}, new TType{Value=2}, new TType{Value=3}}, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest = BinaronConvert.Deserialize<TType[]>(stream);
                Assert.AreEqual(3, dest.Length);
                stream.Seek(0, SeekOrigin.Begin);
                var dest2 = ((IEnumerable) BinaronConvert.Deserialize(stream)).ToArray();
                Assert.AreEqual(3, dest2.Length);
                {
                    var expected = 0;
                    foreach (var item in dest)
                        Assert.AreEqual(++expected, item.Value);
                }
                {
                    var expected = 0;
                    foreach (dynamic item in dest2)
                        Assert.AreEqual(++expected, item.Value);
                }
            }
        }

        [Test]
        public async ValueTask IntToNullableIntTest()
        {
            const int val = int.MinValue;
            var dest = await Tester.TestRoundTrip<IList<int?>>(new []{val});
            Assert.AreEqual(val, dest.Single());
        }
        
        [Test]
        public async ValueTask EnumToNullableEnumTest()
        {
            const TestByteEnum val = TestByteEnum.Min;
            var dest = await Tester.TestRoundTrip<IList<TestByteEnum?>>(new []{val});
            Assert.AreEqual(val, dest.Single());
        }
        
        [Test]
        public async ValueTask NullableStructTest()
        {
            var val = new Tester.TestStruct {Value = 1, NullableValue = 2};
            var dest = await Tester.TestRoundTrip<IList<Tester.TestStruct?>>(new []{val});
            Assert.AreEqual(val.Value, dest.SingleOrDefault()?.Value);
            Assert.AreEqual(val.NullableValue, dest.SingleOrDefault()?.NullableValue);
        }

        [Test]
        public async ValueTask ObjectToGenericIDictionaryTest() => await TestFromObject<IDictionary<string, int>>();

        [Test]
        public async ValueTask ObjectToGenericIEnumerableTest() => await TestFromObject<IEnumerable<KeyValuePair<string, int>>>();

        [Test]
        public async ValueTask ObjectToGenericICollectionTest() => await TestFromObject<ICollection<KeyValuePair<string, int>>>();

        [Test]
        public async ValueTask ObjectToIDictionaryTest() => await TestFromObject<IDictionary>();

        [Test]
        public async ValueTask ObjectToIEnumerableTest() => await TestFromObject<IEnumerable>();
        
        [Test]
        public async ValueTask ObjectToICollectionTest() => await TestFromObject<ICollection>();

        private static async ValueTask TestFromObject<T>() where T : IEnumerable
        {
            var val = new Tester.TestClass {Value = 1};
            var dest = await Tester.TestRoundTrip<T>(val);
            Assert.AreEqual(1, dest.Count());
        }

        private static bool IsValid(object sourceItem, object destItem, Type destType)
        {
            if (destItem == null)
                return false;

            if (!destType.IsGenericType && !destType.IsArray)
                return true;

            var destElementType = destItem.GetType();
            if (!destElementType.IsEnum)
            {
                if (sourceItem is bool)
                {
                    // bool source cannot be converted to any other types
                    return destElementType == typeof(bool);
                }

                if (sourceItem is char)
                {
                    // char can be upgraded to string
                    return destElementType == typeof(string) || destElementType == typeof(char);
                }

                // otherwise .net default conversions match what we need
                return sourceItem.DynamicCast(destElementType) != null;
            }

            try
            {
                if (sourceItem is char || sourceItem is bool)
                    return false;
                var _ = Enum.ToObject(destElementType, sourceItem);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static dynamic AddItem<TSourceItem>(Type sourceType, TSourceItem sourceItem, object source)
        {
            if (!sourceType.TryGetEnumerableType(out var enumerableType))
                return AddItem(source, sourceItem);

            return enumerableType == typeof(IEnumerable)
                ? AddItem((IEnumerable) source, sourceItem) 
                : AddItem((IEnumerable<TSourceItem>) source, sourceItem);
        }

        private static TList AddItem<TList, TItem>(TList source, TItem item) where TList : class
        {
            if (source.GetType().IsArray)
                return Tester.GetEnumerable(((IEnumerable) source).Concat(item).ToArray()).Cast<TItem>().ToArray() as TList;

            switch (source)
            {
                case ICollection<TItem> list:
                    list.Add(item);
                    return source;
                case IList list:
                    list.Add(item);
                    return source;
                case IEnumerable<TItem> enumerable:
                    return Tester.GetEnumerable(enumerable.Concat(item).ToArray()) as TList;
                case IEnumerable enumerable:
                    return Tester.GetEnumerable(enumerable.Concat(item).ToArray()) as TList;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}