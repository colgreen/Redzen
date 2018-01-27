``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.192)
Intel Core i7-6700T CPU 2.80GHz (Skylake), 1 CPU, 4 logical cores and 4 physical cores
Frequency=2742186 Hz, Resolution=364.6726 ns, Timer=TSC
.NET Core SDK=2.1.2
  [Host]     : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.3 (Framework 4.6.25815.02), 64bit RyuJIT


```
|                 Method |     Mean |     Error |    StdDev |
|----------------------- |---------:|----------:|----------:|
|                Next10M | 23.61 ms | 0.0010 ms | 0.0009 ms |
|      NextUpperBound10M | 41.34 ms | 0.0096 ms | 0.0080 ms |
| NextLowerUpperBound10M | 44.26 ms | 0.0081 ms | 0.0068 ms |
|          NextDouble10M | 26.55 ms | 0.0017 ms | 0.0015 ms |
|          NextBytes100M | 81.16 ms | 0.0026 ms | 0.0023 ms |
|           NextFloat10M | 29.50 ms | 0.0009 ms | 0.0009 ms |
|            NextUInt10M | 22.72 ms | 0.0016 ms | 0.0013 ms |
|                NextInt | 21.93 ms | 0.0010 ms | 0.0009 ms |
|   NextDoubleNonZero10M | 29.52 ms | 0.0204 ms | 0.0191 ms |
|               NextBool | 21.12 ms | 0.0052 ms | 0.0048 ms |
|               NextByte | 18.52 ms | 0.0217 ms | 0.0181 ms |
