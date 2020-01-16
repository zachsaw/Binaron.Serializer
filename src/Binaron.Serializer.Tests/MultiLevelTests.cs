using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Binaron.Serializer.CustomObject;
using Binaron.Serializer.Extensions;
using NUnit.Framework;

namespace Binaron.Serializer.Tests
{
    public class MultiLevelTests
    {
        [Test]
        public void EnumerableObjectEnumerableTest()
        {
            var multiLevelEnumerables = new InnerMultiLevelEnumerablesTestClass().Yield();

            using var stream = new MemoryStream();
            BinaronConvert.Serialize(multiLevelEnumerables, stream, new SerializerOptions {SkipNullValues = true});
            stream.Position = 0;
            var response = BinaronConvert.Deserialize<IEnumerable<InnerMultiLevelEnumerablesTestClass>>(stream);
            var inner = response.First();
            CollectionAssert.AreEqual(Enumerable.Range(0, 2), inner.MultiLevelEnumerables.First());
            CollectionAssert.AreEqual(Enumerable.Range(3, 4), inner.MultiLevelEnumerables.Last());
        }

        [Test]
        public async Task MultiLevelWithCustomFactoryTest()
        {
            var obj = CreatedNestedStructure();

            await using var stream = new MemoryStream();
            var objectIdentifierProviders = new ICustomObjectIdentifierProvider[] {new NodeObjectIdentifierProvider()};
            BinaronConvert.Serialize(obj, stream, new SerializerOptions {SkipNullValues = true, CustomObjectIdentifierProviders = objectIdentifierProviders});

            stream.Position = 0;
            var customObjectFactories = new ICustomObjectFactory[] {new NodeObjectFactory()};
            var result = BinaronConvert.Deserialize<Nested>(stream, new DeserializerOptions {CustomObjectFactories = customObjectFactories});

            Assert.IsNull(result.Annotation1);
            Assert.IsNull(result.Annotation2);

            var derived1 = (DerivedNode) result.Nodes["One"];
            var derived2 = (DerivedNode) result.Nodes["Two"];

            Assert.AreEqual("some attributes", derived1.UserAttributes["attribute1"].SingleOrDefault());
            CollectionAssert.AreEqual(new [] {"Child1", "Child2", "Child3"}, derived1.Values[0].SelectedChildren.Select(element => element.Name));
            CollectionAssert.AreEqual(new [] {"Child4", "Child5"}, derived1.Values[1].SelectedChildren.Select(element => element.Name));

            Assert.AreEqual(true, derived2.IsSelected);
            var value = derived2.Values.SingleOrDefault();
            Assert.NotNull(value);
            Assert.AreEqual(100, value.Count);
            var innerNestedNode = value.InnerNested.Nodes["InnerNode1"];
            Assert.AreEqual("InnerDerivedNode1", innerNestedNode.Name);
            Assert.AreEqual(true, ((DerivedNode) innerNestedNode).IsSelected);
        }

        private static Nested CreatedNestedStructure()
        {
            return new Nested
            {
                Annotation1 = "Ignore1",
                Annotation2 = "Ignore2",
                Nodes = new Dictionary<string, Node>
                {
                    {
                        "One", new DerivedNode
                        {
                            Name = "Node1", IsSelected = false, UserAttributes = new UserAttributes {{"attribute1", new List<object> {"some attributes"}}},
                            Values = new List<Value>
                            {
                                new Value
                                {
                                    SelectedChildren = new List<NamedElement>
                                    {
                                        new NamedElement {Name = "Child1"},
                                        new NamedElement {Name = "Child2"},
                                        new NamedElement {Name = "Child3"}
                                    }
                                },
                                new Value
                                {
                                    SelectedChildren = new List<NamedElement>
                                    {
                                        new NamedElement {Name = "Child4"},
                                        new NamedElement {Name = "Child5"}
                                    }
                                }
                            }
                        }
                    },
                    {
                        "Two", new DerivedNode
                        {
                            Name = "Node2", IsSelected = true,
                            Values = new List<Value>
                            {
                                new Value
                                {
                                    Count = 100,
                                    InnerNested = new Nested
                                    {
                                        Nodes = new Dictionary<string, Node>
                                        {
                                            {
                                                "InnerNode1", new DerivedNode
                                                {
                                                    IsSelected = true,
                                                    Name = "InnerDerivedNode1"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        public class NodeObjectFactory : CustomObjectFactory<Node>
        {
            public override object Create(object identifier)
            {
                return (identifier as string) switch
                {
                    nameof(DerivedNode) => new DerivedNode(),
                    _ => null
                };
            }
        }

        public class NodeObjectIdentifierProvider : CustomObjectIdentifierProvider<Node>
        {
            public override object GetIdentifier(Type objectType) => objectType.Name;
        }

        public class Nested
        {
            [field:NonSerialized]
            public string Annotation1 { get; set; }
            [IgnoreDataMember]
            public string Annotation2 { get; set; }
            public IDictionary<string, Node> Nodes { get; set; }
        }

        public abstract class Node
        {
            public string Name { get; set; }
            [field:NonSerialized]
            public string Type { get; }

            protected Node() => Type = GetType().Name;
        }

        public class DerivedNode : Node
        {
            public bool IsSelected { get; set; }
            public IList<Value> Values { get; set; }
            public UserAttributes UserAttributes { get; set; }
        }

        public class UserAttributes : Dictionary<string, List<object>>
        {
        }

        public struct NamedElement
        {
            public string Name { get; set; }
        }
        
        public class Value
        {
            public long? Count { get; set; }
            public Nested InnerNested { get; set; } = new Nested();
            public IList<NamedElement> SelectedChildren { get; set; }
        }

        private class InnerMultiLevelEnumerablesTestClass
        {
            public IEnumerable<IEnumerable<int>> MultiLevelEnumerables { get; set; } = CreateMultiLevelEnumerables();

            private static IEnumerable<IEnumerable<int>> CreateMultiLevelEnumerables()
            {
                yield return Enumerable.Range(0, 2);
                yield return Enumerable.Range(3, 4);
            }
        }
    }
}