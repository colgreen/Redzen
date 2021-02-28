using BenchmarkDotNet.Running;
using Redzen.Benchmarks.Sorting;

namespace Redzen.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<SystemRandomBenchmark>();
            //BenchmarkRunner.Run<XorShiftRandomBenchmark>();
            //BenchmarkRunner.Run<Xoshiro256PlusRandomBenchmark>();
            //BenchmarkRunner.Run<Xoshiro256StarStarRandomBenchmark>();
            //BenchmarkRunner.Run<Xoshiro512StarStarRandomBenchmark>();

            // BenchmarkRunner.Run<ZigguratGaussianDistributionBenchmark>();
            // BenchmarkRunner.Run<BoxMullerGaussianDistributionBenchmark>();


            //BenchmarkRunner.Run<MemoryExtensionsSortBenchmarks>();
            //BenchmarkRunner.Run<MemoryExtensionsSortKVBenchmarks>();
            //BenchmarkRunner.Run<IntroSortKVWBenchmarks>();
            BenchmarkRunner.Run<TimSortBenchmarks>();
            

            //BenchmarkRunner.Run<MathUtilsBenchmark>();
            //BenchmarkRunner.Run<MathSpanSingleBenchmarks>();
        }
    }
}
