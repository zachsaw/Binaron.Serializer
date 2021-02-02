using Binaron.Serializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinSerializerTest
{
    class Program
    {
        class TestObject
        {
            public int A { get; set; }
            public TestObject()
            {

            }

            public TestObject(int a)
            {
                A = a; 
            }
        }

        class Collector<T> : IEnumerable<T>
        {
            private List<T> mData = new List<T>();

            public T this[int index] => mData[index];
            public int Count => mData.Count;

            public void Add(T element) => mData.Add(element);

            public IEnumerator<T> GetEnumerator() => mData.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => mData.GetEnumerator();
        }

        public static void Main(string[] args)
        {
            Collector<TestObject> c = new Collector<TestObject>() { new TestObject(1), new TestObject(2) };
            using (var ms1 = new MemoryStream())
            {
                BinaronConvert.Serialize(c, ms1);
                using (var ms2 = new MemoryStream(ms1.ToArray()))
                {
                    var c2 = BinaronConvert.Deserialize<Collector<TestObject>>(ms2);
                    ;
                }
            }

            
        }
    }
}
