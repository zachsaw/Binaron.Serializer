
# Binaron.Serializer  
A fast serializer for modern programming languages with an open source binary object notation format. With the benchmark project included in this repository (Binaron.Serializer.Benchmark), Binaron.Serializer is 3x faster compared to Newtonsoft.JSON.

|  Method |     Mean |     Error |    StdDev |  
|--------:|---------:|----------:|----------:|  
|    Json | 335.3 ms | 0.8092 ms | 0.7569 ms |  
| Binaron | 110.9 ms | 0.8884 ms | 0.8310 ms |  

![Binaron.Serializer vs Newtonsoft.JSON](https://imgur.com/download/ELn1jtg)

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
  [Host]     : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT  
  DefaultJob : .NET Core 3.0.0 (CoreCLR 4.700.19.46205, CoreFX 4.700.19.46214), 64bit RyuJIT  
  
  
|  Method |     Mean |     Error |    StdDev |  
|-------- |---------:|----------:|----------:|  
|    Json | 335.3 ms | 0.8092 ms | 0.7569 ms |  
| Binaron | 110.9 ms | 0.8884 ms | 0.8310 ms |  
  
// * Legends *  
  Mean   : Arithmetic mean of all measurements  
  Error  : Half of 99.9% confidence interval  
  StdDev : Standard deviation of all measurements  
  1 ms   : 1 Millisecond (0.001 sec)  
  
// ***** BenchmarkRunner: End *****  
// ** Remained 0 benchmark(s) to run **  
Run time: 00:00:27 (27.05 sec), executed benchmarks: 2  
  
Global total time: 00:00:30 (30.95 sec), executed benchmarks: 2  
```