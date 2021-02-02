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
        private List<T> mList = new List<T>();

        public T this[int index] => mList[index];

        public int Count => mList.Count;

        public void Add(T value) => mList.Add(value);


        public IEnumerator<T> GetEnumerator() => mList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => mList.GetEnumerator();

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
