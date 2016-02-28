using BenchmarkDotNet.Running;

namespace Redzen.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var sysRandsummary = BenchmarkRunner.Run<SystemRandomBenchmark>();
            var xorShifySummary = BenchmarkRunner.Run<XorShiftRandomBenchmark>();
            
        }
    }
}
