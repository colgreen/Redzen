using System;
using System.Diagnostics;
using Redzen.Random;

namespace Redzen.Benchmarks.Sorting
{
    internal class SpanSortKVWPerf
    {
        internal delegate void InitKVWDelegate(Span<int> keys, Span<int> values, Span<int> values2, IRandomSource rng);
        internal delegate void SortKVWDelegate(Span<int> keys, Span<int> values, Span<int> values2);

        #region Instance Fields
        
        readonly InitKVWDelegate _init;
        readonly SortKVWDelegate _sort;
        readonly int[] _keys;
        readonly int[] _values;
        readonly int[] _values2;
        readonly int _loopsPerRun;

        #endregion

        #region Construction

        public SpanSortKVWPerf(
            InitKVWDelegate init,
            SortKVWDelegate sort,
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
    }
}
