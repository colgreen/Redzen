using System;
using Redzen.Benchmarks.Sorting;

namespace Redzen.Benchmarks
{
    /// <summary>
    /// Performance benchmarks for <see cref="MemoryExtensions.Sort{T}(Span{T})"/>.
    /// 
    /// The benchmarks are:
    /// 
    ///    Span<int>.Sort() [Random] - Performance sorting pure random data.
    /// 
    ///    Span<int>.Sort() [Natural] - Performance sorting 'natural' data, i.e., with sub-spans of
    ///    already sorted data, some in the wrong order (ascending vs. descending).
    /// 
    /// </summary>
    public class MemoryExtensionsSortPerf
    {
        #region Public Static Methods

        public static void RunBenchmarks(
            int length,int loopsPerRun)
        {
            RunBenchmark_Random(length,loopsPerRun);
            RunBenchmark_Natural(length,loopsPerRun);
        }

        #endregion

        #region Private Static Methods

        private static void RunBenchmark_Random(
            int length, int loopsPerRun)
        {
            var benchmark = new SpanSortPerf(
                SpanSortPerfUtils.InitRandom,
                MemoryExtensions.Sort,
                length,
                loopsPerRun);

            double msPerSort = benchmark.Run();
            Console.WriteLine($"Span<int>.Sort() [Random]:\t{msPerSort} ms / sort");
        }

        private static void RunBenchmark_Natural(
            int length, int loopsPerRun)
        {
            var benchmark = new SpanSortPerf(
                SpanSortPerfUtils.InitNatural,
                MemoryExtensions.Sort,
                length,
                loopsPerRun);

            double msPerSort = benchmark.Run();
            Console.WriteLine($"Span<int>.Sort() [Natural]:\t{msPerSort} ms / sort");
        }

        #endregion
    }
}
