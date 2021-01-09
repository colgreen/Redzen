using System;
using System.Diagnostics;
using BenchmarkDotNet.Running;
using Redzen.Random;
using Redzen.Sorting;

namespace Redzen.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var sysRandsummary = BenchmarkRunner.Run<SystemRandomBenchmark>();
            //var xorShiftSummary = BenchmarkRunner.Run<XorShiftRandomBenchmark>();
            //var xoshiro256PlusSummary = BenchmarkRunner.Run<Xoshiro256PlusRandomBenchmark>();
            //var xoshiro256StarStarSummary = BenchmarkRunner.Run<Xoshiro256StarStarRandomBenchmark>();
            //var xoshiro512StarStarSummary = BenchmarkRunner.Run<Xoshiro512StarStarRandomBenchmark>();

            //var zigguratGaussianSummary = BenchmarkRunner.Run<ZigguratGaussianDistributionBenchmark>();
            //var boxMullerGaussianSummary = BenchmarkRunner.Run<BoxMullerGaussianDistributionBenchmark>();

            var introSortSummary = BenchmarkRunner.Run<IntroSortKVWBenchmarks>();
        }
    }
}
