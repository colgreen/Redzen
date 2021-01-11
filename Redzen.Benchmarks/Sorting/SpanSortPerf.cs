using System;
using System.Diagnostics;
using Redzen.Random;

namespace Redzen.Benchmarks.Sorting
{
    internal class SpanSortPerf
    {
        internal delegate void InitDelegate(Span<int> span, IRandomSource rng);
        internal delegate void SortDelegate(Span<int> span);

        #region Instance Fields
        
        readonly InitDelegate _init;
        readonly SortDelegate _sort;
        readonly int[] _keys;
        readonly int _loopsPerRun;

        #endregion

        #region Construction

        public SpanSortPerf(
            InitDelegate init,
            SortDelegate sort,
            int length,
            int loopsPerRun)
        {
            _init = init;
            _sort = sort;
            _keys = new int[length];
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
                _init(_keys, rng);
                _sort(_keys);
            }

            // Benchmark phase.
            Stopwatch sw = new Stopwatch();
            for(int i=0; i < _loopsPerRun; i++)
            {
                _init(_keys, rng);
                sw.Start();
                _sort(_keys);
                sw.Stop();
            }

            double msPerSort = (double)sw.ElapsedMilliseconds / _loopsPerRun;
            return msPerSort;
        }

        #endregion
    }
}
