# Binaron.Serializer

[Binaron.Serializer NuGet package](https://www.nuget.org/packages/Binaron.Serializer)

**A fast serializer for modern programming languages with an open source [binary object notation format](BinaryObjectNotation.md).**

With the roundtrip serialization-deserialization benchmark project included in this repository (Binaron.Serializer.Benchmark), **Binaron.Serializer is *faster* than Newtonsoft.JSON by *over 300%*!**

|Method | Mean | Error |StdDev |
|--------:|---------:|----------:|----------:|
|Json | 335.3 ms | 0.8092 ms | 0.7569 ms |
| Binaron | 110.9 ms | 0.8884 ms | 0.8310 ms |

![Binaron.Serializer vs Newtonsoft.JSON](https://imgur.com/download/ELn1jtg)

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
Writing a serializer is easy. Writing a deserializer that deserializes to `ExpandoObject`(dynamic type) is just as easy. A deserializer however is a PITA simply because the need to make Binaron.Serializer fit its serialized data as best it could (to sensible limits set in the Binary Object Notation documentation) to the destination object. For example, an `int32` type should fit `int64` and the deserializer shoud be smart enough to do that transparently. Likewise, an object with properties / fields that's been serialized should be deserializable to a dictionary.

To make sure all these permutations are covered and tested, the unit tests in this repository has a **96% coverage**. Not perfect but most would agree it is high enough.

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
BinaronVsJson.Json: DefaultJob
Runtime = .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT; GC = Concurrent Workstation
Mean = 335.3007 ms, StdErr = 0.1954 ms (0.06%); N = 15, StdDev = 0.7569 ms
Min = 334.1482 ms, Q1 = 334.7016 ms, Median = 335.3867 ms, Q3 = 335.7157 ms, Max = 336.8240 ms
IQR = 1.0141 ms, LowerFence = 333.1804 ms, UpperFence = 337.2369 ms
ConfidenceInterval = [334.4915 ms; 336.1099 ms] (CI 99.9%), Margin = 0.8092 ms (0.24% of Mean)
Skewness = 0.02, Kurtosis = 2.19, MValue = 2
-------------------- Histogram --------------------
[333.880 ms ; 337.093 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

BinaronVsJson.Binaron: DefaultJob
Runtime = .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT; GC = Concurrent Workstation
Mean = 110.8839 ms, StdErr = 0.2146 ms (0.19%); N = 15, StdDev = 0.8310 ms
Min = 109.7809 ms, Q1 = 109.8832 ms, Median = 111.0556 ms, Q3 = 111.4201 ms, Max = 112.1343 ms
IQR = 1.5369 ms, LowerFence = 107.5779 ms, UpperFence = 113.7254 ms
ConfidenceInterval = [109.9955 ms; 111.7723 ms] (CI 99.9%), Margin = 0.8884 ms (0.80% of Mean)
Skewness = -0.01, Kurtosis = 1.51, MValue = 2
-------------------- Histogram --------------------
[109.486 ms ; 112.313 ms) | @@@@@@@@@@@@@@@
---------------------------------------------------

// * Summary *

BenchmarkDotNet=v0.11.5, OS=macOS High Sierra 10.13.6 (17G9016) [Darwin 17.7.0]
Intel Core i5-3470 CPU 3.20GHz (Max: 3.21GHz) (Ivy Bridge), 1 CPU, 4 logical and 4 physical cores
.NET Core SDK=3.0.100
[Host] : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT
DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT


|Method | Mean | Error |StdDev |
|-------- |---------:|----------:|----------:|
|Json | 335.3 ms | 0.8092 ms | 0.7569 ms |
| Binaron | 110.9 ms | 0.8884 ms | 0.8310 ms |

// * Legends *
Mean : Arithmetic mean of all measurements
Error: Half of 99.9% confidence interval
StdDev : Standard deviation of all measurements
1 ms : 1 Millisecond (0.001 sec)

// ***** BenchmarkRunner: End *****
// ** Remained 0 benchmark(s) to run **
Run time: 00:00:27 (27.05 sec), executed benchmarks: 2

Global total time: 00:00:30 (30.95 sec), executed benchmarks: 2
```
