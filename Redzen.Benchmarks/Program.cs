using BenchmarkDotNet.Running;

namespace Redzen;

class Program
{
    static void Main()
    {
        BenchmarkRunner.Run(
            new Type[] {
                typeof(Numerics.Distributions.Double.Benchmarks.BoxMullerGaussianDistributionBenchmarks),
                typeof(Numerics.Distributions.Double.Benchmarks.UniformDistributionBenchmarks),
                typeof(Numerics.Distributions.Double.Benchmarks.ZigguratGaussianDistributionBenchmarks),

                typeof(Numerics.Distributions.Float.Benchmarks.BoxMullerGaussianDistributionBenchmarks),
                typeof(Numerics.Distributions.Float.Benchmarks.UniformDistributionBenchmarks),
                typeof(Numerics.Distributions.Float.Benchmarks.ZigguratGaussianDistributionBenchmarks),

                typeof(Random.Benchmarks.SystemRandomBenchmark),
                typeof(Random.Benchmarks.XorShiftRandomBenchmark),
                typeof(Random.Benchmarks.Xoshiro256PlusRandomBenchmark),
                typeof(Random.Benchmarks.Xoshiro256StarStarRandomBenchmark),
                typeof(Random.Benchmarks.Xoshiro512StarStarRandomBenchmark),
                typeof(Random.Benchmarks.WyRandomBenchmarks),

                typeof(Sorting.Benchmarks.IntroSortKVWBenchmarks),
                typeof(Sorting.Benchmarks.MemoryExtensionsSortBenchmarks),
                typeof(Sorting.Benchmarks.MemoryExtensionsSortKVBenchmarks),
                typeof(Sorting.Benchmarks.SortUtilsBenchmarks),
                typeof(Sorting.Benchmarks.TimSortBenchmarks),
                typeof(Sorting.Benchmarks.TimSortKVBenchmarks),
                typeof(Sorting.Benchmarks.TimSortKVWBenchmarks),

                typeof(Benchmarks.MathSpanDoubleBenchmarks),
                typeof(Benchmarks.MathSpanSingleBenchmarks),
                typeof(Benchmarks.MathUtilsBenchmark)
                });
    }
}
