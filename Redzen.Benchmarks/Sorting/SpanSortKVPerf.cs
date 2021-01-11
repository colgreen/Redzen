using System;
using System.Diagnostics;
using Redzen.Random;

namespace Redzen.Benchmarks.Sorting
{
    internal class SpanSortKVPerf
    {
        internal delegate void InitKVDelegate(Span<int> keys, Span<int> values, IRandomSource rng);
        internal delegate void SortKVDelegate(Span<int> keys, Span<int> values);

        #region Instance Fields
        
        readonly InitKVDelegate _init;
        readonly SortKVDelegate _sort;
        readonly int[] _keys;
        readonly int[] _values;
        readonly int _loopsPerRun;

        #endregion

        #region Construction

        public SpanSortKVPerf(
            InitKVDelegate init,
            SortKVDelegate sort,
            int length,
            int loopsPerRun)
        {
            _init = init;
            _sort = sort;
            _keys = new int[length];
            _values = new int[length];
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
                _init(_keys, _values, rng);
                _sort(_keys, _values);
            }

            // Benchmark phase.
            Stopwatch sw = new Stopwatch();
            for(int i=0; i < _loopsPerRun; i++)
            {
                _init(_keys, _values, rng);
                sw.Start();
                _sort(_keys, _values);
                sw.Stop();
            }

            double msPerSort = (double)sw.ElapsedMilliseconds / _loopsPerRun;
            return msPerSort;
        }

        #endregion
    }
}
