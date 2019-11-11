using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Binaron.Serializer.Tests.Extensions;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public enum TestByteEnum : byte {Min, Max}
    public enum TestSByteEnum : sbyte {Min, Max}
    public enum TestShortEnum : short {Min, Max}
    public enum TestUShortEnum : ushort {Min, Max}
    public enum TestIntEnum {Min, Max}
    public enum TestUIntEnum : uint {Min, Max}
    public enum TestLongEnum : long {Min, Max}
    public enum TestULongEnum : ulong {Min, Max}

    public static class AllTestCases
    {
        private static dynamic[] TestValues => new dynamic[]
        {
            decimal.MinValue,
            byte.MinValue,
            sbyte.MinValue,
            short.MinValue,
            ushort.MinValue,
            int.MinValue,
            uint.MinValue,
            long.MinValue,
            ulong.MinValue,
            float.MinValue,
            double.MinValue,
            false
        };

        private static dynamic[] EnumValues => new dynamic[]
        {
            TestByteEnum.Max,
            TestSByteEnum.Max,
            TestShortEnum.Max, 
            TestUShortEnum.Max,
            TestIntEnum.Max,
            TestUIntEnum.Max,
            TestLongEnum.Max,
            TestULongEnum.Max
        };

        private static readonly DateTime Now = DateTime.UtcNow;
        private const char C = '®';
        private const string Str = "A string with unicode surrogate character: 🌉";

        private static dynamic[] MiscValues => new dynamic[] {Now, C, Str};

        public static IEnumerable<TestCaseData> TestCaseOfEnums() => EnumValues.Select(v => new TestCaseData(v));

        public static IEnumerable<TestCaseData> TestCaseOfValues() => TestValues.Concat(EnumValues).Concat(MiscValues).Select(v => new TestCaseData(v));

        public static IEnumerable<TestCaseData> TestCasesOfValueTypes()
        {
            foreach (var testCase in GetTestCases(TestValues)) 
                yield return testCase;

            var now = DateTime.UtcNow;
            yield return new TestCaseData(now, now);
            yield return new TestCaseData(now, 0);

            yield return new TestCaseData(TestByteEnum.Max, TestByteEnum.Max);
            yield return new TestCaseData("Invalid", TestByteEnum.Min);

            yield return new TestCaseData(TestSByteEnum.Max, TestSByteEnum.Max);
            yield return new TestCaseData("Invalid", TestSByteEnum.Min);

            yield return new TestCaseData(TestShortEnum.Max, TestShortEnum.Max);
            yield return new TestCaseData("Invalid", TestShortEnum.Min);

            yield return new TestCaseData(TestUShortEnum.Max, TestUShortEnum.Max);
            yield return new TestCaseData("Invalid", TestUShortEnum.Min);

            yield return new TestCaseData(TestIntEnum.Max, TestIntEnum.Max);
            yield return new TestCaseData("Invalid", TestIntEnum.Min);

            yield return new TestCaseData(TestUIntEnum.Max, TestUIntEnum.Max);
            yield return new TestCaseData("Invalid", TestUIntEnum.Min);

            yield return new TestCaseData(TestLongEnum.Max, TestLongEnum.Max);
            yield return new TestCaseData("Invalid", TestLongEnum.Min);

            yield return new TestCaseData(TestULongEnum.Max, TestULongEnum.Max);
            yield return new TestCaseData("Invalid", TestULongEnum.Min);

            const char c = C;
            yield return new TestCaseData(c, c);
            yield return new TestCaseData(c, 0);
        }

        public static IEnumerable<TestCaseData> TestCases()
        {
            foreach (var testCase in TestCasesOfValueTypes()) 
                yield return testCase;

            const string text = Str;
            yield return new TestCaseData(text, text);
            yield return new TestCaseData(text, 0);

            const char c = C;
            yield return new TestCaseData(c, $"{c}");
        }

        public static IEnumerable<TestCaseData> AllListCases()
        {
            var elements = TestCases().Concat(new TestCaseData(new TestClass {Value = 1}, new TestClass {Value = 1}))
                .Select(c => (Source: c.Arguments[0], Dest: c.Arguments[1])).ToList();

            foreach (var sourceType in ListTypes)
            foreach (var destType in ListTypes)
            foreach (var element in elements)
            {
                var elementSourceType = element.Source.GetType();
                var elementDestType = element.Dest.GetType();
                var actualSourceType = sourceType == typeof(Array) ? elementSourceType.MakeArrayType() : sourceType;
                actualSourceType = actualSourceType.IsGenericType ? actualSourceType.MakeGenericType(elementSourceType) : actualSourceType;
                var actualDestType = destType == typeof(Array) ? elementDestType.MakeArrayType() : destType;
                actualDestType = actualDestType.IsGenericType ? actualDestType.MakeGenericType(elementDestType) : actualDestType;
                var source = actualSourceType.IsArray ? Array.CreateInstance(elementSourceType, 0) : CreateInstance(actualSourceType, elementSourceType);
                yield return new TestCaseData(actualSourceType, actualDestType, source, element.Source, element.Dest);
            }

            object CreateInstance(Type type, Type elementType)
            {
                if (type == typeof(IEnumerable))
                    return GetEnumerable();

                if (type == typeof(IEnumerable<>).MakeGenericType(elementType))
                {
                    var method = new Method(typeof(AllTestCases), nameof(GetEnumerable), elementType);
                    var result = method.Func(null, Array.CreateInstance(elementType, 0));
                    if (!type.IsInstanceOfType(result))
                        throw new InvalidProgramException("Method call has gone wrong");
                    return result;
                }

                return type.IsInterface ? Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType)) : Activator.CreateInstance(type);
            }
        }

        private static IEnumerable<Type> ListTypes =>
            new[]
            {
                typeof(List<>),
                typeof(IList<>),
                typeof(IList),
                typeof(ICollection<>),
                typeof(ICollection),
                typeof(IEnumerable<>),
                typeof(IEnumerable),
                typeof(ArrayList),
                typeof(Array)
            };

        public static IEnumerable<TestCaseData> TestClassAndTestStruct()
        {
            yield return new TestCaseData(new List<TestClass>());
            yield return new TestCaseData(new List<TestStruct>());
        }

        private static IEnumerable<TestCaseData> GetTestCases(object[] values)
        {
            foreach (var source in values)
            foreach (var (dest, type) in values.Select(v => (source.DynamicCast(v.GetType()), v.GetType())))
            {
                var val = dest;
                if (val == null)
                    val = type.GetDefault();
                
                yield return new TestCaseData(source, val);
            }
        }

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