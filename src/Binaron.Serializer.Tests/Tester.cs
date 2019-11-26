using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Binaron.Serializer.Tests
{
    public static class Tester
    {
        public static ValueTask<T> TestRoundTrip<T>(object val) => TestRoundTrip<T>(val, new SerializerOptions());

        public static async ValueTask<T> TestRoundTrip<T>(object val, SerializerOptions options)
        {
            await using var stream = new MemoryStream();
            await BinaronConvert.Serialize(val, stream, options);
            stream.Seek(0, SeekOrigin.Begin);
            return BinaronConvert.Deserialize<T>(stream);
        }

        public static ValueTask<(T, object)> TestRoundTrip2<T>(object val) => TestRoundTrip2<T>(val, new SerializerOptions());

        public static async ValueTask<(T, object)> TestRoundTrip2<T>(object val, SerializerOptions options)
        {
            await using var stream = new MemoryStream();
            await BinaronConvert.Serialize(val, stream, options);
            stream.Seek(0, SeekOrigin.Begin);
            var result1 = BinaronConvert.Deserialize<T>(stream);
            stream.Seek(0, SeekOrigin.Begin);
            var result2 = BinaronConvert.Deserialize(stream);
            return (result1, result2);
        }

        public static ValueTask<T> TestRoundTrip<T>(T val) => TestRoundTrip(val, new SerializerOptions());
        public static ValueTask<T> TestRoundTrip<T>(T val, SerializerOptions options) => TestRoundTrip<T>((object) val, options);
        public static ValueTask<(T, object)> TestRoundTrip2<T>(T val) => TestRoundTrip2(val, new SerializerOptions());
        public static ValueTask<(T, object)> TestRoundTrip2<T>(T val, SerializerOptions options) => TestRoundTrip2<T>((object) val, options);

        public static IEnumerable GetEnumerable(params object[] items)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in items)
                yield return item;
        }

        public static IEnumerable<T> GetEnumerable<T>(params T[] items)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in items)
                yield return item;
        }

        public interface ITestBase
        {
            int Value { get; set; }
        }

        public class TestClass : ITestBase
        {
            public int Value { get; set; }
        }

        public struct TestStruct : ITestBase
        {
            public int Value { get; set; }
            public int? NullableValue { get; set; }
        }
    }
}