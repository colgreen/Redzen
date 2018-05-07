using BenchmarkDotNet.Running;

namespace Redzen.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var sysRandsummary = BenchmarkRunner.Run<SystemRandomBenchmark>();
            //var xorShiftSummary = BenchmarkRunner.Run<XorShiftRandomBenchmark>();
            //var xoroShiro128PlusSummary = BenchmarkRunner.Run<Xoroshiro128PlusRandomBenchmark>();
            //var xoshiro256StarStarSummary = BenchmarkRunner.Run<Xoshiro256StarStarRandomBenchmark>();
            var xoshiro256PlusSummary = BenchmarkRunner.Run<Xoshiro256PlusRandomBenchmark>();

            //ArraySortPerfTest.RunTests(50_000, 1000);
        }
    }
}
