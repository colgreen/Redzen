using System;
using System.Diagnostics;
using Redzen.Random;
using Redzen.Sorting;

namespace Redzen.Benchmarks
{
    /// <summary>
    /// Sort algorithm performance tests.
    /// </summary>
    /// <remarks>
    /// Comparison between:
    ///     Array.Sort() vs. TimSort.
    /// and
    ///     Pure random data vs. 'Natural' random data.
    ///
    /// Much data in the real world is not purely random, rather it is already partially sorted in sub-segments
    /// often as a mix of ascending and descending sub-ranges.
    ///
    /// Timsort is designed to take advantage of such naturally random data, and also performs a stable sort
    /// whereas Array.Sort() does not.
    /// </remarks>
    public class ArraySortPerfTest
    {
        #region Instance Fields

        readonly Action<int[],IRandomSource> _init; // Array initialisation action.
        readonly Action<int[]> _sort;               // Array sort action.
        readonly int[] _arr;
        readonly int _loopsPerRun;

        #endregion

        #region Constructor

        public ArraySortPerfTest(
            Action<int[],IRandomSource> init,
            Action<int[]> sort,
            int length,
            int loopsPerRun)
        {
            _init = init;
            _sort = sort;
            _arr = new int[length];
            _loopsPerRun = loopsPerRun;
        }

        #endregion

        #region Public Methods

        public double Run()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);

            // Warm-up phase.
            for(int i=0; i < 10; i++)
            {
                _init(_arr, rng);
                _sort(_arr);
            }

            // Benchmark phase.
            Stopwatch sw = new Stopwatch();
            for(int i=0; i < _loopsPerRun; i++)
            {
                _init(_arr, rng);
                sw.Start();
                _sort(_arr);
                sw.Stop();
            }

            double msPerSort = (double)sw.ElapsedMilliseconds / _loopsPerRun;
            return msPerSort;
        }

        #endregion

        #region Public Static Methods

        public static void RunTests(
            int length, int loopsPerRun)
        {
            // Array.Sort() tests.
            RunTest_ArraySort_Random(length, loopsPerRun);
            RunTest_ArraySort_Natural(length, loopsPerRun);

            // TimSort tests.
            RunTest_TimSort_Random(length, loopsPerRun);
            RunTest_TimSort_Natural(length, loopsPerRun);
        }

        #endregion

        #region Private Static Methods

        private static void RunTest_ArraySort_Random(
            int length, int loopsPerRun)
        {
            var benchmark = new ArraySortPerfTest(
                ArraySortPerfTestUtils.InitRandom,
                Array.Sort,
                length,
                loopsPerRun);

            double msPerSort = benchmark.Run();
            Console.WriteLine($"Array.Sort() [Random]:\t{msPerSort} ms / sort");
        }

        private static void RunTest_ArraySort_Natural(
            int length, int loopsPerRun)
        {
            var benchmark = new ArraySortPerfTest(
                ArraySortPerfTestUtils.InitNatural,
                Array.Sort,
                length,
                loopsPerRun);

            double msPerSort = benchmark.Run();
            Console.WriteLine($"Array.Sort() [Natural]:\t{msPerSort} ms / sort");
        }

        private static void RunTest_TimSort_Random(
            int length, int loopsPerRun)
        {
            // Alloc a re-usable working array for timsort.
            int[] workArr = new int[50000];

            // Create wrapper for timsort that provides the working array.
            void timsort(int[] arr)
            {
                TimSort<int>.Sort(arr, 0, arr.Length, workArr);
            }

            var benchmark = new ArraySortPerfTest(
                ArraySortPerfTestUtils.InitRandom,
                timsort,
                length,
                loopsPerRun);

            double msPerSort = benchmark.Run();
            Console.WriteLine($"TimSort<int>.Sort [Random]:\t{msPerSort} ms / sort");
        }

        private static void RunTest_TimSort_Natural(
            int length, int loopsPerRun)
        {
            // Alloc a re-usable working array for timsort.
            int[] workArr = new int[50000];

            // Create wrapper for timsort that provides the working array.
            void timsort(int[] arr)
            {
                TimSort<int>.Sort(arr, 0, arr.Length, workArr);
            }

            var benchmark = new ArraySortPerfTest(
                ArraySortPerfTestUtils.InitNatural,
                timsort,
                length,
                loopsPerRun);

            double msPerSort = benchmark.Run();
            Console.WriteLine($"TimSort<int>.Sort [Natural]:\t{msPerSort} ms / sort");
        }

        #endregion
    }
}
