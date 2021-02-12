using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Binaron.Serializer.Tests
{
    public class CustomTestCollection<T> : IEnumerable<T>
    {
        private List<T> List = new List<T>();

        public T this[int index] => List[index];

        public int Count => List.Count;

        public void Add(T value) => List.Add(value);


        public IEnumerator<T> GetEnumerator() => List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => List.GetEnumerator();

        public class TestCollectionObject
        {
            public T A { get; set; }

            public TestCollectionObject()
            {
            }

            public TestCollectionObject(T a)
            {
                A = a;
            }
        }
    }
}
