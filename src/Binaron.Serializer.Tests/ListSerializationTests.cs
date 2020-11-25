using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Binaron.Serializer.Tests.Extensions;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class ListSerializationTests
    {
        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.AllListCases))]
        public void RootListTest<TSourceItem>(Type sourceType, Type destType, object list, TSourceItem sourceItem, object destItem)
        {
            var source = list.DynamicCast(sourceType);
            if (source == null)
                throw new InvalidCastException($"Bad test case. Can't cast source with type of '{list.GetType()}' to '{sourceType}'");

            // Empty
            {
                using var stream = new MemoryStream();
                BinaronConvert.Serialize(source, stream);
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
                using var stream = new MemoryStream();
                BinaronConvert.Serialize(source, stream);
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
                using var stream = new MemoryStream();
                BinaronConvert.Serialize(source, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest = (IEnumerable) Converter.Deserialize(destType, stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest2 = (IEnumerable) BinaronConvert.Deserialize(stream);
                Assert.AreEqual(!IsValid(sourceItem, destItem, destType) ? 0 : 3, dest.Count());
                Assert.AreEqual(3, dest2.Count());
            }
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.TestClassAndTestStruct))]
        public void ClassesAndStructsInListTest<TType>(IList<TType> _) where TType : Tester.ITestBase, new()
        {
            // Empty
            {
                using var stream = new MemoryStream();
                BinaronConvert.Serialize(new TType[0], stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest = BinaronConvert.Deserialize<TType[]>(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var dest2 = (IEnumerable) BinaronConvert.Deserialize(stream);
                Assert.AreEqual(0, dest.Length);
                Assert.AreEqual(0, dest2.Count());
            }
            // Single
            {
                using var stream = new MemoryStream();
                BinaronConvert.Serialize(new[] {new TType {Value = 1}}, stream);
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
                using var stream = new MemoryStream();
                BinaronConvert.Serialize(new[] {new TType {Value = 1}, new TType {Value = 2}, new TType {Value = 3}}, stream);
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

        private static IEnumerable<T> EnumerateRangeSigned<T>() => Enumerable.Range(0, 2).Select(i => (T) ((sbyte) i).DynamicCast(typeof(T)));
        private static IEnumerable<T> EnumerateRangeUnsigned<T>() => Enumerable.Range(0, 2).Select(i => (T) ((byte) i).DynamicCast(typeof(T)));

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.EnumSignedTypeValues))]
        public void SignedToEnumArrayTest<T>(T _)
        {
            var dest = Tester.TestRoundTrip<TestByteEnum[]>(EnumerateRangeSigned<T>().ToArray());
            Assert.AreEqual(new[] {TestByteEnum.Min, TestByteEnum.Max}, dest);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.EnumUnsignedTypeValues))]
        public void UnsignedToEnumArrayTest<T>(T _)
        {
            var dest = Tester.TestRoundTrip<TestByteEnum[]>(EnumerateRangeUnsigned<T>().ToArray());
            Assert.AreEqual(new[] {TestByteEnum.Min, TestByteEnum.Max}, dest);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.EnumSignedTypeValues))]
        public void SignedToEnumEnumerableTest<T>(T _)
        {
            var dest = Tester.TestRoundTrip<IEnumerable<TestByteEnum>>(EnumerateRangeSigned<T>());
            Assert.AreEqual(new[] {TestByteEnum.Min, TestByteEnum.Max}, dest);
        }

        [TestCaseSource(typeof(AllTestCases), nameof(AllTestCases.EnumUnsignedTypeValues))]
        public void UnsignedToEnumEnumerableTest<T>(T _)
        {
            var dest = Tester.TestRoundTrip<IEnumerable<TestByteEnum>>(EnumerateRangeUnsigned<T>());
            Assert.AreEqual(new[] {TestByteEnum.Min, TestByteEnum.Max}, dest);
        }

        [Test]
        public void MixedNullsInStringArrayTest()
        {
            var val = new[] {"val1", null, "val2", null, null};
            var dest = Tester.TestRoundTrip(val);
            Assert.AreEqual(val, dest);
        }

        [Test]
        public void IntToNullableIntTest()
        {
            const int val = int.MinValue;
            var dest = Tester.TestRoundTrip<IList<int?>>(new[] {val});
            Assert.AreEqual(val, dest.Single());
        }

        [Test]
        public void EnumToNullableEnumTest()
        {
            const TestByteEnum val = TestByteEnum.Min;
            var dest = Tester.TestRoundTrip<IList<TestByteEnum?>>(new[] {val});
            Assert.AreEqual(val, dest.Single());
        }

        [Test]
        public void NullableStructTest()
        {
            var val = new Tester.TestStruct {Value = 1, NullableValue = 2};
            var dest = Tester.TestRoundTrip<IList<Tester.TestStruct?>>(new[] {val});
            Assert.AreEqual(val.Value, dest.SingleOrDefault()?.Value);
            Assert.AreEqual(val.NullableValue, dest.SingleOrDefault()?.NullableValue);
        }

        [Test]
        public void ObjectToGenericIDictionaryTest() => TestFromObject<IDictionary<string, int>>();

        [Test]
        public void ObjectToGenericIEnumerableTest() => TestFromObject<IEnumerable<KeyValuePair<string, int>>>();

        [Test]
        public void ObjectToGenericICollectionTest() => TestFromObject<ICollection<KeyValuePair<string, int>>>();

        [Test]
        public void ObjectToIDictionaryTest() => TestFromObject<IDictionary>();

        [Test]
        public void ObjectToIEnumerableTest() => TestFromObject<IEnumerable>();

        [Test]
        public void ObjectToICollectionTest() => TestFromObject<ICollection>();

        private static void TestFromObject<T>() where T : IEnumerable
        {
            var val = new Tester.TestClass {Value = 1};
            var dest = Tester.TestRoundTrip<T>(val);
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