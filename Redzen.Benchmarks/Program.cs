using BenchmarkDotNet.Running;

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
            var boxMullerGaussianSummary = BenchmarkRunner.Run<BoxMullerGaussianDistributionBenchmark>();
            
            //ArraySortPerfTest.RunTests(50_000, 1000);
        }
    }
}
