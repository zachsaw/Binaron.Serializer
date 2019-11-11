using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Binaron.Serializer.Tests.Extensions;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class InvalidNonConcreteTests
    {
        [TestCaseSource(nameof(TestCases))]
        public void CustomInvalidNonConcreteTest<TSource>(TSource source, Type destType)
        {
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(source, stream);
            stream.Seek(0, SeekOrigin.Begin);
            Assert.Throws<NotSupportedException>(() => Converter.Deserialize(destType, stream));
        }

        private static IEnumerable<TestCaseData> TestCases()
        {
            var customDictionary = new CustomDictionary {{"Key", "Value"}};
            yield return new TestCaseData(customDictionary, typeof(CustomBaseDictionary));
            yield return new TestCaseData(customDictionary, typeof(Custom));
            yield return new TestCaseData(customDictionary, typeof(ICustom));

            var customList = new CustomList {"Item"};
            yield return new TestCaseData(customList, typeof(CustomBaseList));
            yield return new TestCaseData(customList, typeof(Custom));
            yield return new TestCaseData(customList, typeof(ICustom));

            var customEnumerable = new CustomEnumerable("Item");
            yield return new TestCaseData(customEnumerable, typeof(CustomBaseEnumerable));
            yield return new TestCaseData(customEnumerable, typeof(Custom));
            yield return new TestCaseData(customEnumerable, typeof(ICustom));
        }

        public interface ICustom
        {
        }

        public abstract class Custom
        {
        }

        public abstract class CustomBaseDictionary : Dictionary<string, string>
        {
        }
        
        public class CustomDictionary : CustomBaseDictionary
        {
        }

        public abstract class CustomBaseList : List<string>
        {
        }

        public class CustomList : CustomBaseList
        {
        }

        public abstract class CustomBaseEnumerable : IEnumerable<string>
        {
            private readonly string[] items;

            protected CustomBaseEnumerable(params string[] items)
            {
                this.items = items;
            }

            public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>) items).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class CustomEnumerable : CustomBaseEnumerable
        {
            public CustomEnumerable(params string[] items) : base(items)
            {
            }
        }
    }
}