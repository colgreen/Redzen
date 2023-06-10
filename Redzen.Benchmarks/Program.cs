using BenchmarkDotNet.Running;

namespace Redzen;

class Program
{
    static void Main()
    {
        BenchmarkRunner.Run(
            new Type[] 
            {
                typeof(Numerics.Distributions.Double.BoxMullerGaussianDistributionBenchmarks),
                typeof(Numerics.Distributions.Double.UniformDistributionBenchmarks),
                typeof(Numerics.Distributions.Double.ZigguratGaussianDistributionBenchmarks),

                typeof(Numerics.Distributions.Float.BoxMullerGaussianDistributionBenchmarks),
                typeof(Numerics.Distributions.Float.UniformDistributionBenchmarks),
                typeof(Numerics.Distributions.Float.ZigguratGaussianDistributionBenchmarks),

                typeof(Random.SystemRandomBenchmarks),
                typeof(Random.XorShiftRandomBenchmarks),
                typeof(Random.Xoshiro256PlusRandomBenchmarks),
                typeof(Random.Xoshiro256StarStarRandomBenchmarks),
                typeof(Random.Xoshiro512StarStarRandomBenchmarks),
                typeof(Random.WyRandomBenchmarks),

                typeof(Sorting.IntroSortKVWBenchmarks),
                typeof(Sorting.MemoryExtensionsSortBenchmarks),
                typeof(Sorting.MemoryExtensionsSortKVBenchmarks),
                typeof(Sorting.SortUtilsBenchmarks),
                typeof(Sorting.TimSortBenchmarks),
                typeof(Sorting.TimSortKVBenchmarks),
                typeof(Sorting.TimSortKVWBenchmarks),

                typeof(MathSpanDoubleBenchmarks),
                typeof(MathSpanSingleBenchmarks),
                typeof(MathUtilsBenchmarks)
            });
    }
}
