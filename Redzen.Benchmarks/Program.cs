using BenchmarkDotNet.Running;

namespace Redzen
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

            BenchmarkRunner.Run<Numerics.Distributions.Float.Benchmarks.ZigguratGaussianDistributionBenchmarks>();
            //BenchmarkRunner.Run<Numerics.Distributions.Float.Benchmarks.BoxMullerGaussianDistributionBenchmarks>();


            //BenchmarkRunner.Run<MemoryExtensionsSortBenchmarks>();
            //BenchmarkRunner.Run<MemoryExtensionsSortKVBenchmarks>();
            //BenchmarkRunner.Run<IntroSortKVWBenchmarks>();
            //BenchmarkRunner.Run<TimSortBenchmarks>();
            //BenchmarkRunner.Run<TimSortKVBenchmarks>();
            //BenchmarkRunner.Run<TimSortKVWBenchmarks>();


            //BenchmarkRunner.Run<MathUtilsBenchmark>();
            //BenchmarkRunner.Run<MathSpanDoubleBenchmarks>();



            //BenchmarkRunner.Run<Numerics.Distributions.Float.Benchmarks.UniformDistributionBenchmarks>();
        }
    }
}
