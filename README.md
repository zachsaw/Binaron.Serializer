# Binaron.Serializer

[Binaron.Serializer NuGet package](https://www.nuget.org/packages/Binaron.Serializer)

**A *really* fast serializer for modern programming languages with an open source [binary object notation format](BinaryObjectNotation.md).**

In this repository, you'll find 2 benchmarks using BenchmarkDotNet comparing Binaron.Serializer to Newtonsoft.JSON.

The first one showcases best case scenario vs a JSON serializer, where we serialize an array of double with 128k items. In reality, this could be your typical weights from a CNN model.

## Benchmark 1

In this benchmark, **Binaron.Serializer is *over 10,000% faster (> 100x)* than Newtonsoft.JSON in serialization and *over 5,000% faster (> 50x)* in deserialization!**

|              Method |       Mean |     Error |    StdDev |
|-------------------: |-----------:|----------:|----------:|
|      Json_Serialize | 389.842 ms | 1.3542 ms | 1.2667 ms |
|   Binaron_Serialize |   3.689 ms | 0.0537 ms | 0.0502 ms |
|    Json_Deserialize | 692.229 ms | 1.0580 ms | 0.9897 ms |
| Binaron_Deserialize |  13.261 ms | 0.1102 ms | 0.0977 ms |

### Array of Doubles - e.g. Weights from a CNN Model
![Binaron.Serializer vs Newtonsoft.JSON - CNN Model Weights](https://imgur.com/download/96nXv9T)

## Benchmark 2

The second benchmark is your typical DTO where Binaron.Serializer's advantage is not as pronounced, but is **over *300% faster* in serialization and *~350% faster* in deserialization** nonetheless.

|              Method |     Mean |     Error |    StdDev |
|-------------------: |---------:|----------:|----------:|
|      Json_Serialize | 353.3 ms | 0.9824 ms | 0.9190 ms |
|   Binaron_Serialize | 115.5 ms | 0.3651 ms | 0.3415 ms |
|    Json_Deserialize | 600.7 ms | 1.2501 ms | 1.1081 ms |
| Binaron_Deserialize | 173.8 ms | 0.6994 ms | 0.5841 ms |

### Book Object (Typical DTO)
![Binaron.Serializer vs Newtonsoft.JSON - Book (DTO)](https://imgur.com/download/5n6OZa0)

## Usage

```C#

var input = new Book();

using var stream = new MemoryStream(); // C# 8.0 syntaxBinaronConvert.Serialize(input, stream);

stream.Position = 0;

var book = BinaronConvert.Deserialize<Book>(stream);

// or

stream.Position = 0;

var dynamicBook = BinaronConvert.Deserialize(stream);

// ...
```

### Polymorphism support

Binaron.Serializer can be configured to support serialization / deserialization of interfaces and abstract types.

```C#

public interface IPerson
{
    string FirstName { get; set; }
    string LastName { get; set; }
    DateTime BirthDate { get; set; }
}

public class Employee : IPerson
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }

    public string Department { get; set; }
    public string JobTitle { get; set; }
}

public class Customer : IPerson
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }

    public string Email { get; set; }
}

public class PersonIdentifierProvider : CustomObjectIdentifierProvider<IPerson>
{
    public override object GetIdentifier(Type objectType) => objectType.Name;
}

public class PersonFactory : CustomObjectFactory<IPerson>
{
    public override object Create(object identifier)
    {
        return (identifier as string) switch
        {
            nameof(Employee) => new Employee(),
            nameof(Customer) => new Customer(),
            _ => null
        };
    }
}

```

If you are using a service provider, you would use it in the `PersonFactory` to construct `Employee` and `Customer` instead.

For serialization / deserialization, you'll need to provide the PersonIdentifierProvider as well as the PersonFactory as follows.

```C#

var identifiers = new ICustomObjectIdentifierProvider[] {new PersonIdentifierProvider()};
BinaronConvert.Serialize(person, stream, new SerializerOptions {SkipNullValues = true, CustomObjectIdentifierProviders = identifiers});

var factories = new ICustomObjectFactory[] {new PersonFactory()};
var person = BinaronConvert.Deserialize<IPerson>(stream, new DeserializerOptions {CustomObjectFactories = factories});

```

### Ignore Attributes

Binaron.Serializer supports the following ignore attributes: `System.NonSerializedAttribute` and `System.Runtime.Serialization.IgnoreDataMemberAttribute`.

```C#

public class Person
{
    [IgnoreDataMember]
    public int Age { get; set; }

    [IgnoreDataMember]
    public int AgeField;

    [field:NonSerialized]
    public DateTime Dob { get; set; }

    [NonSerialized]
    public DateTime DobField;
}

```

## Limitations

Binaron.Serializer also uses and relies heavily on the newly released features of `.net standard 2.1` for maximum performance and thus is only compatible with `.net core app 3.0` and above.

## High unit test coverage

Writing a serializer was easy. Writing a deserializer that deserializes to `ExpandoObject`(dynamic type) was just as easy. However, deserializing to a specific type was a PITA simply because of the need to make Binaron.Serializer fit its serialized data as best it could (to sensible limits set in the Binary Object Notation documentation) to the destination object. For example, an `int32` type should fit `int64` and the deserializer shoud be smart enough to do that transparently. Likewise, an object with properties / fields that's been serialized should be deserializable to a dictionary.

To make sure all these permutations are covered and tested, the unit tests in this repository has a **94% coverage**. Not perfect but most would agree it is high enough and will be improved in the near future.

## Why another serializer?

### Big payloads

In the world of microservices, data payloads tend to be pretty big these days with how the data is now consumed. As network bandwidth becomes cheaper, bigger data becomes the norm as it opens up UI/UX that would otherwise have been impossible (e.g. responsive web apps and mobi/mobile apps). Converting from text to object and vice versa is a very slow process. The bigger the payload, the slower it is, naturally.

JSON was created for consumption of the old web days where Javascript had limited support for binary. Unfortunately for everyone else on first class languages, they've had to dumb down to the common lowest denominator.

### But... JSON is human readable

JSON does have its merits such as human readability. But, does machine really care about human readability? At what cost are we sacrificing performance - thus infra cost, latencies and ultimately user experience? If we really care about human readability, we could simply have the endpoint support two different types of accept-headers - one for JSON, the other Binary. In a normal day to day operation, you would go binary. For debugging purposes, give it a JSON only accept-header and you would get JSON sent back to you. How many microservices are doing this though?

### What about protobuf?

Granted there are myriad of libraries that have tried to do binary serialization. The most popular is arguably protobuf. All of these libraries are fast but they lack the one key feature that JSON serializers offer - the ability to serialize from / de-serialize to any unstructured object.

For example, protobuf requires a schema to be defined for the structure of the object - i.e. a .proto file. This itself is a massive burden on the developers to learn, create, debug and maintain. What do you do if you're storing data in a NoSQL manner where data is simply unstructured?

Unfortunately, all the binary serializers for .NET have one fundamental flaw - they assume your data is structured, including [ZeroFormatter](https://github.com/neuecc/ZeroFormatter).

### TL;DR

In other words, I wanted a drop-in replacement for [Json.NET](https://www.newtonsoft.com/json) with near zero learning curve that promises vastly superior performance but couldn't find one.

## Detailed benchmark results

### Benchmark 1
```
// * Detailed results *
BinaronVsJsonTrainedWeights.Json_Serialize: DefaultJob
Runtime = .NET Core 3.0.1 (CoreCLR 4.700.19.51502, CoreFX 4.700.19.51609), 64bit RyuJIT; GC = Concurrent Workstation
Mean = 389.8421 ms, StdErr = 0.3271 ms (0.08%); N = 15, StdDev = 1.2667 ms
Min = 388.0537 ms, Q1 = 388.7856 ms, Median = 390.2393 ms, Q3 = 390.8508 ms, Max = 392.2435 ms
IQR = 2.0652 ms, LowerFence = 385.6877 ms, UpperFence = 393.9486 ms
ConfidenceInterval = [388.4879 ms; 391.1964 ms] (CI 99.9%), Margin = 1.3542 ms (0.35% of Mean)
Skewness = 0.12, Kurtosis = 1.71, MValue = 2
-------------------- Histogram --------------------
[387.604 ms ; 392.693 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

BinaronVsJsonTrainedWeights.Binaron_Serialize: DefaultJob
Runtime = .NET Core 3.0.1 (CoreCLR 4.700.19.51502, CoreFX 4.700.19.51609), 64bit RyuJIT; GC = Concurrent Workstation
Mean = 3.6890 ms, StdErr = 0.0130 ms (0.35%); N = 15, StdDev = 0.0502 ms
Min = 3.6444 ms, Q1 = 3.6607 ms, Median = 3.6659 ms, Q3 = 3.7227 ms, Max = 3.8062 ms
IQR = 0.0620 ms, LowerFence = 3.5677 ms, UpperFence = 3.8158 ms
ConfidenceInterval = [3.6354 ms; 3.7427 ms] (CI 99.9%), Margin = 0.0537 ms (1.45% of Mean)
Skewness = 1.28, Kurtosis = 3.12, MValue = 2
-------------------- Histogram --------------------
[3.627 ms ; 3.824 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

BinaronVsJsonTrainedWeights.Json_Deserialize: DefaultJob
Runtime = .NET Core 3.0.1 (CoreCLR 4.700.19.51502, CoreFX 4.700.19.51609), 64bit RyuJIT; GC = Concurrent Workstation
Mean = 692.2286 ms, StdErr = 0.2555 ms (0.04%); N = 15, StdDev = 0.9897 ms
Min = 690.6930 ms, Q1 = 691.4408 ms, Median = 692.2797 ms, Q3 = 692.7500 ms, Max = 694.3949 ms
IQR = 1.3092 ms, LowerFence = 689.4769 ms, UpperFence = 694.7138 ms
ConfidenceInterval = [691.1705 ms; 693.2866 ms] (CI 99.9%), Margin = 1.0580 ms (0.15% of Mean)
Skewness = 0.39, Kurtosis = 2.48, MValue = 2
-------------------- Histogram --------------------
[690.342 ms ; 694.746 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

BinaronVsJsonTrainedWeights.Binaron_Deserialize: DefaultJob
Runtime = .NET Core 3.0.1 (CoreCLR 4.700.19.51502, CoreFX 4.700.19.51609), 64bit RyuJIT; GC = Concurrent Workstation
Mean = 13.2613 ms, StdErr = 0.0261 ms (0.20%); N = 14, StdDev = 0.0977 ms
Min = 13.1627 ms, Q1 = 13.1830 ms, Median = 13.2343 ms, Q3 = 13.3258 ms, Max = 13.4512 ms
IQR = 0.1428 ms, LowerFence = 12.9688 ms, UpperFence = 13.5400 ms
ConfidenceInterval = [13.1511 ms; 13.3715 ms] (CI 99.9%), Margin = 0.1102 ms (0.83% of Mean)
Skewness = 0.82, Kurtosis = 2.25, MValue = 2
-------------------- Histogram --------------------
[13.127 ms ; 13.487 ms) | @@@@@@@@@@@@@@
---------------------------------------------------

// * Summary *

BenchmarkDotNet=v0.11.5, OS=macOS High Sierra 10.13.6 (17G9016) [Darwin 17.7.0]
Intel Core i5-3470 CPU 3.20GHz (Max: 3.21GHz) (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.0.101
[Host]     : .NET Core 3.0.1 (CoreCLR 4.700.19.51502, CoreFX 4.700.19.51609), 64bit RyuJIT
DefaultJob : .NET Core 3.0.1 (CoreCLR 4.700.19.51502, CoreFX 4.700.19.51609), 64bit RyuJIT


|              Method |       Mean |     Error |    StdDev |
|-------------------- |-----------:|----------:|----------:|
|      Json_Serialize | 389.842 ms | 1.3542 ms | 1.2667 ms |
|   Binaron_Serialize |   3.689 ms | 0.0537 ms | 0.0502 ms |
|    Json_Deserialize | 692.229 ms | 1.0580 ms | 0.9897 ms |
| Binaron_Deserialize |  13.261 ms | 0.1102 ms | 0.0977 ms |
```

### Benchmark 2
```
// * Detailed results *
BinaronVsJson.Json_Serialize: DefaultJob
Runtime = .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT; GC = Concurrent Workstation
Mean = 353.2906 ms, StdErr = 0.2373 ms (0.07%); N = 15, StdDev = 0.9190 ms
Min = 351.8990 ms, Q1 = 352.4945 ms, Median = 353.1237 ms, Q3 = 353.7853 ms, Max = 355.1822 ms
IQR = 1.2908 ms, LowerFence = 350.5584 ms, UpperFence = 355.7214 ms
ConfidenceInterval = [352.3082 ms; 354.2731 ms] (CI 99.9%), Margin = 0.9824 ms (0.28% of Mean)
Skewness = 0.57, Kurtosis = 2.5, MValue = 2
-------------------- Histogram --------------------
[351.573 ms ; 355.508 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

BinaronVsJson.Binaron_Serialize: DefaultJob
Runtime = .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT; GC = Concurrent Workstation
Mean = 115.4851 ms, StdErr = 0.0882 ms (0.08%); N = 15, StdDev = 0.3415 ms
Min = 114.6920 ms, Q1 = 115.2784 ms, Median = 115.4497 ms, Q3 = 115.6688 ms, Max = 116.0873 ms
IQR = 0.3904 ms, LowerFence = 114.6927 ms, UpperFence = 116.2544 ms
ConfidenceInterval = [115.1200 ms; 115.8502 ms] (CI 99.9%), Margin = 0.3651 ms (0.32% of Mean)
Skewness = -0.34, Kurtosis = 2.92, MValue = 2
-------------------- Histogram --------------------
[114.571 ms ; 116.208 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

BinaronVsJson.Json_Deserialize: DefaultJob
Runtime = .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT; GC = Concurrent Workstation
Mean = 600.7190 ms, StdErr = 0.2962 ms (0.05%); N = 14, StdDev = 1.1081 ms
Min = 599.2788 ms, Q1 = 599.8592 ms, Median = 600.4859 ms, Q3 = 601.9519 ms, Max = 602.6048 ms
IQR = 2.0927 ms, LowerFence = 596.7202 ms, UpperFence = 605.0909 ms
ConfidenceInterval = [599.4690 ms; 601.9691 ms] (CI 99.9%), Margin = 1.2501 ms (0.21% of Mean)
Skewness = 0.37, Kurtosis = 1.61, MValue = 2
-------------------- Histogram --------------------
[598.876 ms ; 603.007 ms) | @@@@@@@@@@@@@@
---------------------------------------------------

BinaronVsJson.Binaron_Deserialize: DefaultJob
Runtime = .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT; GC = Concurrent Workstation
Mean = 173.7795 ms, StdErr = 0.1620 ms (0.09%); N = 13, StdDev = 0.5841 ms
Min = 172.3075 ms, Q1 = 173.6573 ms, Median = 173.9798 ms, Q3 = 174.1402 ms, Max = 174.3649 ms
IQR = 0.4829 ms, LowerFence = 172.9330 ms, UpperFence = 174.8645 ms
ConfidenceInterval = [173.0801 ms; 174.4790 ms] (CI 99.9%), Margin = 0.6994 ms (0.40% of Mean)
Skewness = -1.3, Kurtosis = 3.68, MValue = 2
-------------------- Histogram --------------------
[172.090 ms ; 174.582 ms) | @@@@@@@@@@@@@
---------------------------------------------------

// * Summary *

BenchmarkDotNet=v0.11.5, OS=macOS High Sierra 10.13.6 (17G9016) [Darwin 17.7.0]
Intel Core i5-3470 CPU 3.20GHz (Max: 3.21GHz) (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.0.100
[Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT
DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT


|              Method |     Mean |     Error |    StdDev |
|-------------------- |---------:|----------:|----------:|
|      Json_Serialize | 353.3 ms | 0.9824 ms | 0.9190 ms |
|   Binaron_Serialize | 115.5 ms | 0.3651 ms | 0.3415 ms |
|    Json_Deserialize | 600.7 ms | 1.2501 ms | 1.1081 ms |
| Binaron_Deserialize | 173.8 ms | 0.6994 ms | 0.5841 ms |
```