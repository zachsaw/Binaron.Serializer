using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Binaron.Serializer.Tests
{
    class ReadOnlySerializationTest
    {

        [Test]
        public void ReadOnlyStructTest()
        {
            ReadOnlyStruct rsu = new ReadOnlyStruct(1, "text");
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(rsu, stream);

            stream.Seek(0, SeekOrigin.Begin);
            dynamic rsu1 = BinaronConvert.Deserialize(stream);
            Assert.AreEqual(rsu.IntValue, rsu1.IntValue);
            Assert.AreEqual(rsu.StringValue, rsu1.StringValue);

            stream.Seek(0, SeekOrigin.Begin);
            var rsu2 = BinaronConvert.Deserialize<ReadOnlyStruct>(stream);

            //reproduce issue https://github.com/zachsaw/Binaron.Serializer/issues/32
            Assert.AreEqual(rsu.IntValue, rsu1.IntValue);
            Assert.AreEqual(rsu.StringValue, rsu1.StringValue);

        }

        [Test]
        public void ReadOnlyClassTest_OnlyPrimitives()
        {
            ReadOnlyClass rsu = new ReadOnlyClass(1, "text");
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(rsu, stream);

            stream.Seek(0, SeekOrigin.Begin);
            dynamic rsu1 = BinaronConvert.Deserialize(stream);
            Assert.AreEqual(rsu.IntValue, rsu1.IntValue);
            Assert.AreEqual(rsu.StringValue, rsu1.StringValue);

            stream.Seek(0, SeekOrigin.Begin);
            var rsu2 = BinaronConvert.Deserialize<ReadOnlyClass>(stream);

            //reproduce issue https://github.com/zachsaw/Binaron.Serializer/issues/32
            Assert.AreEqual(rsu.IntValue, rsu2.IntValue);
            Assert.AreEqual(rsu.StringValue, rsu2.StringValue);
            Assert.IsNull(rsu2.Aggregate);
        }

        [Test]
        public void ReadOnlyClassTest_ClassProperty()
        {
            ReadOnlyClass rsu = new ReadOnlyClass(1, "text", new ReadOnlyStruct(2, "subtext"));
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(rsu, stream);

            stream.Seek(0, SeekOrigin.Begin);
            dynamic rsu1 = BinaronConvert.Deserialize(stream);
            Assert.AreEqual(rsu.IntValue, rsu1.IntValue);
            Assert.AreEqual(rsu.StringValue, rsu1.StringValue);
            Assert.AreEqual(rsu.Aggregate.Value.IntValue, rsu1.Aggregate.IntValue);
            Assert.AreEqual(rsu.Aggregate.Value.StringValue, rsu1.Aggregate.StringValue);

            stream.Seek(0, SeekOrigin.Begin);
            var rsu2 = BinaronConvert.Deserialize<ReadOnlyClass>(stream);

            //reproduce issue https://github.com/zachsaw/Binaron.Serializer/issues/32
            Assert.AreEqual(rsu.IntValue, rsu2.IntValue);
            Assert.AreEqual(rsu.StringValue, rsu2.StringValue);
            Assert.AreEqual(rsu.Aggregate.Value.IntValue, rsu2.Aggregate.Value.IntValue);
            Assert.AreEqual(rsu.Aggregate.Value.StringValue, rsu2.Aggregate.Value.StringValue);

        }

        [Test]
        public void ReadOnlyClassTest_TypeConversionToLessProperties()
        {
            ReadOnlyClass rsu = new ReadOnlyClass(1, "text", new ReadOnlyStruct(2, "subtext"));
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(rsu, stream);

            stream.Seek(0, SeekOrigin.Begin);
            var rsu2 = BinaronConvert.Deserialize<ReadOnlyClass1>(stream);

            Assert.AreEqual(rsu.IntValue, rsu2.IntValue);
            Assert.AreEqual(rsu.Aggregate.Value.IntValue, rsu2.Aggregate.IntValue);
            Assert.AreEqual(rsu.Aggregate.Value.StringValue, rsu2.Aggregate.StringValue);
            Assert.AreEqual(null, rsu2.Aggregate.Aggregate);
        }
        
        [Test]
        public void ReadOnlyClassTest_TypeConversionToMoreProperties()
        {
            ReadOnlyClass1 rsu = new ReadOnlyClass1(1, new ReadOnlyClass(2, "subtext"));
            using var stream = new MemoryStream();
            BinaronConvert.Serialize(rsu, stream);

            stream.Seek(0, SeekOrigin.Begin);
            var rsu2 = BinaronConvert.Deserialize<ReadOnlyClass>(stream);

            Assert.AreEqual(rsu.IntValue, rsu2.IntValue);
            Assert.IsNull(rsu2.StringValue);

            Assert.IsNotNull(rsu2.Aggregate);
            Assert.AreEqual(rsu.Aggregate.IntValue, rsu2.Aggregate.Value.IntValue);
            Assert.AreEqual(rsu.Aggregate.StringValue, rsu2.Aggregate.Value.StringValue);
        }

        [Test]
        public void ListOfStruct()
        {
            List<ReadOnlyStruct> list = new List<ReadOnlyStruct>()
            {
                new ReadOnlyStruct(1, "text1"),
                new ReadOnlyStruct(2, "text2"),
                new ReadOnlyStruct(3, "text3"),
            };

            using var stream = new MemoryStream();
            BinaronConvert.Serialize(list, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var list1 = BinaronConvert.Deserialize<List<ReadOnlyStruct>>(stream);
            Assert.IsNotNull(list1);
            Assert.AreEqual(3, list1.Count);

            Assert.AreEqual(1, list1[0].IntValue);
            Assert.AreEqual("text1", list1[0].StringValue);
            Assert.AreEqual(2, list1[1].IntValue);
            Assert.AreEqual("text2", list1[1].StringValue);
            Assert.AreEqual(3, list1[2].IntValue);
            Assert.AreEqual("text3", list1[2].StringValue);
        }

        [Test]
        public void ArrayfClasses()
        {
            ReadOnlyClass[] list = new ReadOnlyClass[]
            {
                new ReadOnlyClass(1, "text1"),
                new ReadOnlyClass(2, "text2"),
                new ReadOnlyClass(3, "text3"),
            };

            using var stream = new MemoryStream();
            BinaronConvert.Serialize(list, stream);
            stream.Seek(0, SeekOrigin.Begin);
            var list1 = BinaronConvert.Deserialize<ReadOnlyClass[]>(stream);
            Assert.IsNotNull(list1);
            Assert.AreEqual(3, list1.Length);

            Assert.AreEqual(1, list1[0].IntValue);
            Assert.AreEqual("text1", list1[0].StringValue);
            Assert.AreEqual(2, list1[1].IntValue);
            Assert.AreEqual("text2", list1[1].StringValue);
            Assert.AreEqual(3, list1[2].IntValue);
            Assert.AreEqual("text3", list1[2].StringValue);
        }

        private readonly struct ReadOnlyStruct
        {
            public int IntValue { get; }
            public string StringValue { get; }

            public ReadOnlyStruct(int intValue, string stringValue)
            {
                IntValue = intValue;
                StringValue = stringValue;
            }
        }

        private class ReadOnlyClass
        {
            public int IntValue { get; }
            public string StringValue { get; }

            public ReadOnlyStruct? Aggregate { get; }

            public ReadOnlyClass(int intValue, string stringValue) : this(intValue, stringValue, null)
            {
            }

            [JsonConstructor]
            public ReadOnlyClass(int intValue, string stringValue, ReadOnlyStruct? aggregate)
            {
                IntValue = intValue;
                StringValue = stringValue;
                Aggregate = aggregate;
            }
        }

        private class ReadOnlyClass1
        {
            public int IntValue { get; }

            public ReadOnlyClass Aggregate { get; }

            [BinaronConstructor]
            public ReadOnlyClass1(int intValue, ReadOnlyClass aggregate)
            {
                IntValue = intValue;
                Aggregate = aggregate;
            }
        }

    }
}
