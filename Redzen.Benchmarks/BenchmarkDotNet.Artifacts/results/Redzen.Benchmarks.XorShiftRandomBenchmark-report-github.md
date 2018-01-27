``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.192)
Intel Core i7-6700T CPU 2.80GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2742186 Hz, Resolution=364.6726 ns, Timer=TSC
.NET Core SDK=2.1.4
  [Host]     : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.5 (Framework 4.6.26020.03), 64bit RyuJIT


```
|                 Method |     Mean |     Error |    StdDev |
|----------------------- |---------:|----------:|----------:|
|                Next10M | 23.60 ms | 0.0013 ms | 0.0012 ms |
|      NextUpperBound10M | 41.30 ms | 0.0014 ms | 0.0013 ms |
| NextLowerUpperBound10M | 44.25 ms | 0.0011 ms | 0.0010 ms |
|          NextDouble10M | 26.55 ms | 0.0008 ms | 0.0007 ms |
|          NextBytes100M | 36.89 ms | 0.0012 ms | 0.0012 ms |
|           NextFloat10M | 29.50 ms | 0.0005 ms | 0.0005 ms |
|            NextUInt10M | 21.81 ms | 0.0009 ms | 0.0008 ms |
|                NextInt | 21.93 ms | 0.0004 ms | 0.0004 ms |
|   NextDoubleNonZero10M | 29.50 ms | 0.0004 ms | 0.0004 ms |
|               NextBool | 25.72 ms | 0.0034 ms | 0.0030 ms |
|               NextByte | 23.60 ms | 0.0006 ms | 0.0006 ms |
