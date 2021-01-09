using System;
using System.Diagnostics;
using Redzen.Random;
using Redzen.Sorting;

namespace Redzen.Benchmarks.Sorting
{
    internal sealed class IntroSortKVWPerf
    {
        #region Instance Fields

        readonly Action<int[], int[], int[], IRandomSource> _init; // Initialisation action.
        readonly Action<int[], int[], int[]> _sort;                // Sort action.
        readonly int[] _keys;
        readonly int[] _values;
        readonly int[] _values2;
        readonly int _loopsPerRun;

        #endregion

        #region Construction

        public IntroSortKVWPerf(
            Action<int[], int[], int[], IRandomSource> init,
            Action<int[], int[], int[]> sort,
            int length,
            int loopsPerRun)
        {
            _init = init;
            _sort = sort;
            _keys = new int[length];
            _values = new int[length];
            _values2 = new int[length];
            _loopsPerRun = loopsPerRun;
        }

        #endregion

        #region Public Methods

        public double Run()
        {
            IRandomSource rng = RandomDefaults.CreateRandomSource(0);

            // Warm-up phase.
            for(int i=0; i < 100; i++)
            {
                _init(_keys, _values, _values2, rng);
                _sort(_keys, _values, _values2);
            }

            // Benchmark phase.
            Stopwatch sw = new Stopwatch();
            for(int i=0; i < _loopsPerRun; i++)
            {
                _init(_keys, _values, _values2, rng);
                sw.Start();
                _sort(_keys, _values, _values2);
                sw.Stop();
            }

            double msPerSort = (double)sw.ElapsedMilliseconds / _loopsPerRun;
            return msPerSort;
        }

        #endregion

        #region Public Static Methods

        public static void RunBenchmarkss(
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
            var benchmark = new IntroSortKVWPerf(
                SortingPerfUtils.InitRandom,
                IntoSort,
                length,
                loopsPerRun);

            double msPerSort = benchmark.Run();
            Console.WriteLine($"IntroSort<int,int,int>.Sort() [Random]:\t\t{msPerSort} ms / sort");
        }

        private static void RunBenchmark_Natural(
            int length, int loopsPerRun)
        {
            var benchmark = new IntroSortKVWPerf(
                SortingPerfUtils.InitNatural,
                IntoSort,
                length,
                loopsPerRun);

            double msPerSort = benchmark.Run();
            Console.WriteLine($"IntroSort<int,int,int>.Sort() [Natural]:\t{msPerSort} ms / sort");
        }

        #endregion

        #region Private Static Methods

        private static void IntoSort(int[] keys, int[] values, int[] values2)
        {
            IntroSort<int,int,int>.Sort(keys, values, values2);
        }

        #endregion
    }
}
