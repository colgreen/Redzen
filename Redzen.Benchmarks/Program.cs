using BenchmarkDotNet.Running;

namespace Redzen
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Numerics.Distributions.Double.Benchmarks.BoxMullerGaussianDistributionBenchmarks>();
            BenchmarkRunner.Run<Numerics.Distributions.Double.Benchmarks.UniformDistributionBenchmarks>();
            BenchmarkRunner.Run<Numerics.Distributions.Double.Benchmarks.ZigguratGaussianDistributionBenchmarks>();

            BenchmarkRunner.Run<Numerics.Distributions.Float.Benchmarks.BoxMullerGaussianDistributionBenchmarks>();
            BenchmarkRunner.Run<Numerics.Distributions.Float.Benchmarks.UniformDistributionBenchmarks>();
            BenchmarkRunner.Run<Numerics.Distributions.Float.Benchmarks.ZigguratGaussianDistributionBenchmarks>();

            BenchmarkRunner.Run<Random.Benchmarks.SystemRandomBenchmark>();
            BenchmarkRunner.Run<Random.Benchmarks.XorShiftRandomBenchmark>();
            BenchmarkRunner.Run<Random.Benchmarks.Xoshiro256PlusRandomBenchmark>();
            BenchmarkRunner.Run<Random.Benchmarks.Xoshiro256StarStarRandomBenchmark>();
            BenchmarkRunner.Run<Random.Benchmarks.Xoshiro512StarStarRandomBenchmark>();
            BenchmarkRunner.Run<Random.Benchmarks.WyRandomBenchmarks>();

            BenchmarkRunner.Run<Sorting.Benchmarks.IntroSortKVWBenchmarks>();
            BenchmarkRunner.Run<Sorting.Benchmarks.MemoryExtensionsSortBenchmarks>();
            BenchmarkRunner.Run<Sorting.Benchmarks.MemoryExtensionsSortKVBenchmarks>();
            BenchmarkRunner.Run<Sorting.Benchmarks.SortUtilsBenchmarks>();
            BenchmarkRunner.Run<Sorting.Benchmarks.TimSortBenchmarks>();
            BenchmarkRunner.Run<Sorting.Benchmarks.TimSortKVBenchmarks>();
            BenchmarkRunner.Run<Sorting.Benchmarks.TimSortKVWBenchmarks>();

            BenchmarkRunner.Run<Benchmarks.MathSpanDoubleBenchmarks>();
            BenchmarkRunner.Run<Benchmarks.MathSpanSingleBenchmarks>();

            BenchmarkRunner.Run<Benchmarks.MathUtilsBenchmark>();
        }
    }
}
