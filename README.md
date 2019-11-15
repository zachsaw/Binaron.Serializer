# Binaron.Serializer

[Binaron.Serializer NuGet package](https://www.nuget.org/packages/Binaron.Serializer)

**A fast serializer for modern programming languages with an open source [binary object notation format](BinaryObjectNotation.md).**

With the roundtrip serialization-deserialization benchmark project included in this repository (Binaron.Serializer.Benchmark), **Binaron.Serializer is *faster* than Newtonsoft.JSON by *over 300%*!**

|              Method |     Mean |     Error |    StdDev |
|-------------------: |---------:|----------:|----------:|
|      Json_Serialize | 353.3 ms | 0.9824 ms | 0.9190 ms |
|   Binaron_Serialize | 115.5 ms | 0.3651 ms | 0.3415 ms |
|    Json_Deserialize | 600.7 ms | 1.2501 ms | 1.1081 ms |
| Binaron_Deserialize | 173.8 ms | 0.6994 ms | 0.5841 ms |

![Binaron.Serializer vs Newtonsoft.JSON](https://imgur.com/download/5n6OZa0)

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

## Limitations
This is the first cut of Binary.Serializer. It currently only supports null value skipping in its serializer options. It does not support property ignore attributes yet. It also does not support deserialization of interfaces / abstract classes. These features will be implemented in the future.

Binary.Serializer also uses and relies heavily on the newly released features of `.net standard 2.1` for maximum performance and thus is only compatible with `.net core app 3.0` and above.

## High unit test coverage
Writing a serializer was easy. Writing a deserializer that deserializes to `ExpandoObject`(dynamic type) was just as easy. However, deserializing to a specific type was a PITA simply because of the need to make Binaron.Serializer fit its serialized data as best it could (to sensible limits set in the Binary Object Notation documentation) to the destination object. For example, an `int32` type should fit `int64` and the deserializer shoud be smart enough to do that transparently. Likewise, an object with properties / fields that's been serialized should be deserializable to a dictionary.

To make sure all these permutations are covered and tested, the unit tests in this repository has a **92% coverage**. Not perfect but most would agree it is high enough and will be improved in the near future.

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
