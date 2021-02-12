using System;
using Binaron.Serializer.Accessors;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class MemberGetSetterTests
    {
        [Test]
        public void MemberGetterInvalidFieldTest()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new MemberGetter<int>(typeof(TestClass), "InvalidMemberName");
            });
        }

        [Test]
        public void MemberSetterInvalidFieldTest()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                // ReSharper disable once ObjectCreationAsStatement
                new MemberSetter<int>(typeof(TestClass), "InvalidMemberName");
            });
        }

        [Test]
        public void MemberSetterNoWriteTest()
        {
            var setter = new MemberSetter<int>(typeof(TestClass), nameof(TestClass.NoWrite));
            Assert.AreEqual(false, setter.IsValid);
        }

        [Test]
        public void MemberGetterNoReadTest()
        {
            var getter = new MemberGetter<int>(typeof(TestClass), nameof(TestClass.NoRead));
            Assert.AreEqual(false, getter.IsValid);
        }
        
        private class TestClass
        {
            public int Value { get; }
            public int NoWrite => 0;
            public int NoRead
            {
                set { }
            }
        }
       
    }
}