using System;
using Redzen.Benchmarks.Sorting;
using Redzen.Sorting;

namespace Redzen.Benchmarks
{
    // TODO: Enable benchmarks once then TimSort API accepts a Span<T> to sort.

    /// <summary>
    /// Performance benchmarks for <see cref="TimSort.Sort"/>.
    /// 
    /// The benchmarks are:
    /// 
    ///    TimSort.Sort() [Random] - Performance sorting pure random data.
    /// 
    ///    TimSort.Sort() [Natural] - Performance sorting 'natural' data, i.e., with sub-spans of
    ///    already sorted data, some in the wrong order (ascending vs. descending).
    /// 
    /// </summary>
    public class TimSortPerf
    {
        #region Public Static Methods

        public static void RunBenchmarks(
            int length, int loopsPerRun)
        {
            RunBenchmark_Random(length, loopsPerRun);
            RunBenchmark_Natural(length, loopsPerRun);
        }

        #endregion

        #region Private Static Methods [Benchmarks]

        private static void RunBenchmark_Random(
            int length, int loopsPerRun)
        {
            var benchmark = new SpanSortPerf(
                SortingPerfUtils.InitRandom,
                TimSort<int>.Sort,
                length,
                loopsPerRun);

            double msPerSort = benchmark.Run();
            Console.WriteLine($"TimSort<int>.Sort() [Random]:\t{msPerSort} ms / sort");
        }

        private static void RunBenchmark_Natural(
            int length, int loopsPerRun)
        {
            var benchmark = new SpanSortPerf(
                SortingPerfUtils.InitNatural,
                TimSort<int>.Sort,
                length,
                loopsPerRun);

            double msPerSort = benchmark.Run();
            Console.WriteLine($"TimSort<int>.Sort() [Natural]:\t{msPerSort} ms / sort");
        }

        #endregion
    }
}
