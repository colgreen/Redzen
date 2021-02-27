using System;
using BenchmarkDotNet.Attributes;
using Redzen.Random;

namespace Redzen.Benchmarks.Sorting
{
    public class MemoryExtensionsSortBenchmarks
    {
        IRandomSource _rng = RandomDefaults.CreateRandomSource(123);

        #region Instance Fields

        [Params(50_000)]
        public int ArraySize;

        [Params(1000)]
        public int ArrayCount;

        int[] _keysRandom;
        int[] _keysNaturalRandom;
        int[][] _arrays;

        #endregion

        #region Public Methods

        [GlobalSetup]
        public void Setup()
        {
            // Alloc arrays.
            _keysRandom = new int[ArraySize];
            _keysNaturalRandom = new int[ArraySize];
            _arrays = new int[ArrayCount][];

            for(int i=0; i < _arrays.Length; i++) {
                _arrays[i] = new int[ArraySize];
            }

            // Fill key arrays with random values.
            SpanSortPerfUtils.InitRandom(_keysRandom, _rng);
            SpanSortPerfUtils.InitNatural(_keysNaturalRandom, _rng);
        }

        [IterationSetup(Target = nameof(SortRandom))]
        public void SetupIteration_Random()
        { 
            // Load a fresh copy of the random values into all test arrays prior to each benchmark iteration
            // (otherwise most iterations will be asked to sort data that is already sorted).
            InitArrays(_arrays, _keysRandom);
        }

        [IterationSetup(Target = nameof(SortNaturalRandom))]
        public void SetupIteration_NaturalRandom()
        { 
            // Load a fresh copy of the random values into all test arrays prior to each benchmark iteration
            // (otherwise most iterations will be asked to sort data that is already sorted).
            InitArrays(_arrays, _keysNaturalRandom);
        }

        [Benchmark]
        public void SortRandom()
        {
            for(int i=0; i < _arrays.Length; i++)
            {
                _arrays[i].AsSpan().Sort();
            }
        }

        [Benchmark]
        public void SortNaturalRandom()
        {
            for(int i=0; i < _arrays.Length; i++)
            {
                _arrays[i].AsSpan().Sort();
            }
        }

        #endregion

        #region Private Static Methods

        private static void InitArrays(int[][] arrays, int[] sourceVals)
        {
            foreach(int[] arr in arrays) {
                Array.Copy(sourceVals, arr, sourceVals.Length);
            }
        }

        #endregion
    }
}
