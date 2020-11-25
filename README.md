# Binaron.Serializer

[Binaron.Serializer NuGet package](https://www.nuget.org/packages/Binaron.Serializer)

**A *really* fast serializer for modern programming languages with an open source [binary object notation format](BinaryObjectNotation.md).**

In this repository, you'll find 2 benchmarks using BenchmarkDotNet comparing Binaron.Serializer to Newtonsoft.JSON. System.Text.Json's JsonSerializer unfortunately failed validation, which is why it is not included in the benchmark.

Note that these benchmarks were run on .net 5.0.

The first one showcases best case scenario vs a JSON serializer, where we serialize an array of double with 64k items. In reality, this could be your typical weights from a small CNN model. More importantly, Binaron.Serializer also serializes doubles in a 64-bit lossless format unlike JSON which requires workarounds such as converting the raw 64-bit into byte array.

## Benchmark 1

In this benchmark, **Binaron.Serializer is *nearly 150x faster* than Newtonsoft.JSON in serialization and *around 137x faster* in deserialization!**

|              Method |         Mean |     Error |    StdDev |
|-------------------- |-------------:|----------:|----------:|
|   Binaron_Serialize |     5.783 ms | 0.0344 ms | 0.0322 ms |
| Binaron_Deserialize |     9.180 ms | 0.0487 ms | 0.0455 ms |
|      Json_Serialize |   864.364 ms | 2.1031 ms | 1.9672 ms |
|    Json_Deserialize | 1,258.305 ms | 3.2696 ms | 3.0584 ms |

### Array of Doubles - e.g. Weights from a CNN Model
![Binaron.Serializer vs Newtonsoft.JSON - CNN Model Weights](https://i.imgur.com/miBN4OG.png)

## Benchmark 2

The second benchmark is your typical DTO where Binaron.Serializer's advantage is not as pronounced, but is **over *3.5x faster* in serialization and *4.3x faster* in deserialization** nonetheless.

|              Method |     Mean |    Error |   StdDev |
|-------------------- |---------:|---------:|---------:|
|   Binaron_Serialize | 14.96 ms | 0.033 ms | 0.027 ms |
| Binaron_Deserialize | 18.12 ms | 0.350 ms | 0.479 ms |
|      Json_Serialize | 54.33 ms | 0.186 ms | 0.165 ms |
|    Json_Deserialize | 78.75 ms | 0.193 ms | 0.171 ms |

### Book Object (Typical DTO)
![Binaron.Serializer vs Newtonsoft.JSON - Book (DTO)](https://i.imgur.com/eOLJlME.png)

## Usage

```C#

var input = new Book();

using var stream = new MemoryStream(); // C# 8.0 syntax
BinaronConvert.Serialize(input, stream);

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

BinaronConvert.Serialize(person, stream, new SerializerOptions {SkipNullValues = true, CustomObjectIdentifierProviders = {new PersonIdentifierProvider()}});

var person = BinaronConvert.Deserialize<IPerson>(stream, new DeserializerOptions {CustomObjectFactories = {new PersonFactory()}});

```

### Object Activator / Dependency Injection (DI) Support

Binaron.Serializer can also be configured to support DI frameworks or any custom object activators.

```C#

public class ServiceProviderActivator : IObjectActivator
{
    private readonly IServiceProvider serviceProvider;

    public ServiceProviderActivator(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public object Create(Type type)
    {
        return ActivatorUtilities.CreateInstance(serviceProvider, type);
    }
}

var person = BinaronConvert.Deserialize<Person>(stream, new DeserializerOptions {ObjectActivator = new ServiceProviderActivator(serviceProvider)});

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

Binaron.Serializer uses and relies heavily on the newly released features of `.net standard 2.1` for maximum performance and thus is only compatible with `.net core app 3.0` and above.

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

### But... Binary serializers are brittle, aren't they?

No. Only the .NET BinaryFormatter is brittle because it serializes the type's full name (including namespace). It encodes exact type names etc, making it useless for archiving data such as in a document format for an application for example.

Binaron.Serializer has the same brittleness as JSON serializers.

### TL;DR

In other words, I wanted a drop-in replacement for [Json.NET](https://www.newtonsoft.com/json) with near zero learning curve that promises vastly superior performance but couldn't find one.

## Detailed benchmark results

### Benchmark 1
```
// * Detailed results *
BinaronVsJsonTrainedWeights.Binaron_Serialize: DefaultJob
Runtime = .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT; GC = Concurrent Workstation
Mean = 5.783 ms, StdErr = 0.008 ms (0.14%), N = 15, StdDev = 0.032 ms
Min = 5.727 ms, Q1 = 5.754 ms, Median = 5.793 ms, Q3 = 5.800 ms, Max = 5.834 ms
IQR = 0.046 ms, LowerFence = 5.686 ms, UpperFence = 5.868 ms
ConfidenceInterval = [5.749 ms; 5.817 ms] (CI 99.9%), Margin = 0.034 ms (0.59% of Mean)
Skewness = -0.23, Kurtosis = 1.79, MValue = 2
-------------------- Histogram --------------------
[5.710 ms ; 5.851 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

BinaronVsJsonTrainedWeights.Binaron_Deserialize: DefaultJob
Runtime = .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT; GC = Concurrent Workstation
Mean = 9.180 ms, StdErr = 0.012 ms (0.13%), N = 15, StdDev = 0.046 ms
Min = 9.094 ms, Q1 = 9.157 ms, Median = 9.168 ms, Q3 = 9.213 ms, Max = 9.277 ms
IQR = 0.056 ms, LowerFence = 9.073 ms, UpperFence = 9.297 ms
ConfidenceInterval = [9.131 ms; 9.229 ms] (CI 99.9%), Margin = 0.049 ms (0.53% of Mean)
Skewness = 0.27, Kurtosis = 2.57, MValue = 2
-------------------- Histogram --------------------
[9.070 ms ; 9.301 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

BinaronVsJsonTrainedWeights.Json_Serialize: DefaultJob
Runtime = .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT; GC = Concurrent Workstation
Mean = 864.364 ms, StdErr = 0.508 ms (0.06%), N = 15, StdDev = 1.967 ms
Min = 860.825 ms, Q1 = 862.755 ms, Median = 864.731 ms, Q3 = 865.711 ms, Max = 867.360 ms
IQR = 2.956 ms, LowerFence = 858.322 ms, UpperFence = 870.144 ms
ConfidenceInterval = [862.260 ms; 866.467 ms] (CI 99.9%), Margin = 2.103 ms (0.24% of Mean)
Skewness = -0.18, Kurtosis = 1.68, MValue = 2
-------------------- Histogram --------------------
[859.778 ms ; 868.407 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

BinaronVsJsonTrainedWeights.Json_Deserialize: DefaultJob
Runtime = .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT; GC = Concurrent Workstation
Mean = 1.258 s, StdErr = 0.001 s (0.06%), N = 15, StdDev = 0.003 s
Min = 1.253 s, Q1 = 1.256 s, Median = 1.259 s, Q3 = 1.260 s, Max = 1.264 s
IQR = 0.004 s, LowerFence = 1.250 s, UpperFence = 1.266 s
ConfidenceInterval = [1.255 s; 1.262 s] (CI 99.9%), Margin = 0.003 s (0.26% of Mean)
Skewness = -0.03, Kurtosis = 1.92, MValue = 2
-------------------- Histogram --------------------
[1.251 s ; 1.266 s) | @@@@@@@@@@@@@@@
---------------------------------------------------

// * Summary *

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1237 (1909/November2018Update/19H2)
Intel Core i7-10850H CPU 2.70GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=5.0.100
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
  DefaultJob : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT


|              Method |         Mean |     Error |    StdDev |
|-------------------- |-------------:|----------:|----------:|
|   Binaron_Serialize |     5.783 ms | 0.0344 ms | 0.0322 ms |
| Binaron_Deserialize |     9.180 ms | 0.0487 ms | 0.0455 ms |
|      Json_Serialize |   864.364 ms | 2.1031 ms | 1.9672 ms |
|    Json_Deserialize | 1,258.305 ms | 3.2696 ms | 3.0584 ms |
```

### Benchmark 2
```
// * Detailed results *
BinaronVsJsonBook.Binaron_Serialize: DefaultJob
Runtime = .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT; GC = Concurrent Workstation
Mean = 14.960 ms, StdErr = 0.008 ms (0.05%), N = 13, StdDev = 0.027 ms
Min = 14.907 ms, Q1 = 14.942 ms, Median = 14.951 ms, Q3 = 14.984 ms, Max = 15.007 ms
IQR = 0.041 ms, LowerFence = 14.880 ms, UpperFence = 15.046 ms
ConfidenceInterval = [14.927 ms; 14.992 ms] (CI 99.9%), Margin = 0.033 ms (0.22% of Mean)
Skewness = -0.05, Kurtosis = 2.13, MValue = 2
-------------------- Histogram --------------------
[14.891 ms ; 15.022 ms) | @@@@@@@@@@@@@
---------------------------------------------------

BinaronVsJsonBook.Binaron_Deserialize: DefaultJob
Runtime = .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT; GC = Concurrent Workstation
Mean = 18.115 ms, StdErr = 0.094 ms (0.52%), N = 26, StdDev = 0.479 ms
Min = 17.425 ms, Q1 = 17.696 ms, Median = 18.158 ms, Q3 = 18.438 ms, Max = 19.234 ms
IQR = 0.742 ms, LowerFence = 16.583 ms, UpperFence = 19.550 ms
ConfidenceInterval = [17.765 ms; 18.465 ms] (CI 99.9%), Margin = 0.350 ms (1.93% of Mean)
Skewness = 0.38, Kurtosis = 2.5, MValue = 2.55
-------------------- Histogram --------------------
[17.362 ms ; 17.787 ms) | @@@@@@@@
[17.787 ms ; 18.188 ms) | @@@@@
[18.188 ms ; 18.613 ms) | @@@@@@@@@@@
[18.613 ms ; 19.359 ms) | @@
---------------------------------------------------

BinaronVsJsonBook.Json_Serialize: DefaultJob
Runtime = .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT; GC = Concurrent Workstation
Mean = 54.332 ms, StdErr = 0.044 ms (0.08%), N = 14, StdDev = 0.165 ms
Min = 54.089 ms, Q1 = 54.210 ms, Median = 54.362 ms, Q3 = 54.467 ms, Max = 54.555 ms
IQR = 0.257 ms, LowerFence = 53.823 ms, UpperFence = 54.853 ms
ConfidenceInterval = [54.146 ms; 54.519 ms] (CI 99.9%), Margin = 0.186 ms (0.34% of Mean)
Skewness = -0.11, Kurtosis = 1.36, MValue = 2
-------------------- Histogram --------------------
[53.999 ms ; 54.645 ms) | @@@@@@@@@@@@@@
---------------------------------------------------

BinaronVsJsonBook.Json_Deserialize: DefaultJob
Runtime = .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT; GC = Concurrent Workstation
Mean = 78.747 ms, StdErr = 0.046 ms (0.06%), N = 14, StdDev = 0.171 ms
Min = 78.566 ms, Q1 = 78.623 ms, Median = 78.685 ms, Q3 = 78.884 ms, Max = 79.105 ms
IQR = 0.260 ms, LowerFence = 78.233 ms, UpperFence = 79.274 ms
ConfidenceInterval = [78.554 ms; 78.939 ms] (CI 99.9%), Margin = 0.193 ms (0.24% of Mean)
Skewness = 0.77, Kurtosis = 2.09, MValue = 2
-------------------- Histogram --------------------
[78.473 ms ; 79.198 ms) | @@@@@@@@@@@@@@
---------------------------------------------------

// * Summary *

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1237 (1909/November2018Update/19H2)
Intel Core i7-10850H CPU 2.70GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=5.0.100
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
  DefaultJob : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT


|              Method |     Mean |    Error |   StdDev |
|-------------------- |---------:|---------:|---------:|
|   Binaron_Serialize | 14.96 ms | 0.033 ms | 0.027 ms |
| Binaron_Deserialize | 18.12 ms | 0.350 ms | 0.479 ms |
|      Json_Serialize | 54.33 ms | 0.186 ms | 0.165 ms |
|    Json_Deserialize | 78.75 ms | 0.193 ms | 0.171 ms |
```