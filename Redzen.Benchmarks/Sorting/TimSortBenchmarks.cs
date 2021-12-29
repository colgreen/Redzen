using System;
using BenchmarkDotNet.Attributes;
using Redzen.Random;
using Redzen.Sorting;

namespace Redzen.Benchmarks.Sorting
{
    public class TimSortBenchmarks
    {
        #region Instance Fields

        [Params(50_000)]
        public int ArrayLength;

        [Params(100)]
        public int ArrayCount;

        int[] _keysRandom;
        int[] _keysNaturalRandom;
        int[] _work;
        int[][] _arrays;

        #endregion

        #region Public Methods

        [GlobalSetup]
        public void Setup()
        {
            // Alloc arrays.
            _keysRandom = new int[ArrayLength];
            _keysNaturalRandom = new int[ArrayLength];
            _work = new int[ArrayCount];
            _arrays = new int[ArrayCount][];

            for(int i=0; i < _arrays.Length; i++)
                _arrays[i] = new int[ArrayLength];

            // Fill key arrays with random values.
            IRandomSource rng = RandomDefaults.CreateRandomSource(123);
            SortBenchmarkUtils.InitRandom(_keysRandom, rng);
            SortBenchmarkUtils.InitNatural(_keysNaturalRandom, rng);
        }

        [IterationSetup(Target = nameof(SortRandom))]
        public void IterationSetup_Random()
        {
            // Load a fresh copy of the random values into all test arrays prior to each benchmark iteration
            // (otherwise most iterations will be asked to sort data that is already sorted).
            InitArrays(_arrays, _keysRandom);
        }

        [IterationSetup(Target = nameof(SortNaturalRandom))]
        public void IterationSetup_NaturalRandom()
        {
            // Load a fresh copy of the random values into all test arrays prior to each benchmark iteration
            // (otherwise most iterations will be asked to sort data that is already sorted).
            InitArrays(_arrays, _keysNaturalRandom);
        }

        [Benchmark]
        public void SortRandom()
        {
            for(int i=0; i < _arrays.Length; i++)
                TimSort<int>.Sort(_arrays[i], ref _work);
        }

        [Benchmark]
        public void SortNaturalRandom()
        {
            for(int i=0; i < _arrays.Length; i++)
                TimSort<int>.Sort(_arrays[i], ref _work);
        }

        #endregion

        #region Private Static Methods

        private static void InitArrays(int[][] arrays, int[] sourceVals)
        {
            foreach(int[] arr in arrays)
                Array.Copy(sourceVals, arr, sourceVals.Length);
        }

        #endregion
    }
}
