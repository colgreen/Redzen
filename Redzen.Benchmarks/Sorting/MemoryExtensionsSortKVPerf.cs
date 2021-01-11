using System;
using Redzen.Benchmarks.Sorting;

namespace Redzen.Benchmarks
{
    /// <summary>
    /// Performance benchmarks for <see cref="MemoryExtensions.Sort{T}(Span{T}, Span{T})"/>.
    /// 
    /// The benchmarks are:
    /// 
    ///    Span<int>.Sort(Span<int>) [Random] - Performance sorting pure random data.
    /// 
    ///    Span<int>.Sort(Span<int>) [Natural] - Performance sorting 'natural' data, i.e., with sub-spans of
    ///    already sorted data, some in the wrong order (ascending vs. descending).
    /// 
    /// </summary>
    public class MemoryExtensionsSortKVPerf
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
            var benchmark = new SpanSortKVPerf(
                SpanSortPerfUtils.InitRandom,
                MemoryExtensions.Sort,
                length,
                loopsPerRun);

            double msPerSort = benchmark.Run();
            Console.WriteLine($"Span<int>.Sort(Span<int>) [Random]:\t{msPerSort} ms / sort");
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
            Console.WriteLine($"Span<int>.Sort(Span<int>) [Natural]:\t{msPerSort} ms / sort");
        }

        #endregion
    }
}
