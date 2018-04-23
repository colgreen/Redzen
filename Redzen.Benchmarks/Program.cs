using BenchmarkDotNet.Running;

namespace Redzen.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var sysRandsummary = BenchmarkRunner.Run<SystemRandomBenchmark>();
            //var xorShiftSummary = BenchmarkRunner.Run<XorShiftRandomBenchmark>();

            ArraySortPerfTest.RunTests(50_000, 1000);
        }
    }
}
