using System;
using BenchmarkDotNet.Attributes;
using Redzen.Random;
using Redzen.Sorting;

namespace Redzen.Benchmarks
{
    // FIXME: WARNING. DO NOT USE! This benchmark is awaiting a fix to issue https://github.com/dotnet/BenchmarkDotNet/issues/1636
    // The unrollFactor argument of the InvocationCount attribute defaults to one here, but this is not honoured. The result is that
    // the methods marked with [IterationSetup] are not run for each call to the [Benchmark] methods, thus invalidating the results.
    [InvocationCount(1000)]
    [MinWarmupCount(6, forceAutoWarmup: true)] // when InvocationCount is set, BDN does not run Pilot Stage, so to get the code promoted to Tier 1 before Actual Workload, we enforce more Warmups
    public class IntroSortKVWBenchmarks
    {
        [Params(50_000)]
        public int Length;

        int[] _keys;
        int[] _values;
        int[] _values2;
        IRandomSource _rng = RandomDefaults.CreateRandomSource(123);

        [GlobalSetup]
        public void GlobalSetup()
        {
            _keys = new int[Length];
            _values = new int[Length];
            _values2 = new int[Length];
        }

        [IterationSetup(Targets = new[] { nameof(Sort), nameof(TimSort) })]
        public void IterationSetup()
        {
            for(int i=0; i < _keys.Length; i++)
            {
                _keys[i] = _rng.Next();
                _values[i] = _keys[i];
                _values2[i] = _keys[i];
            }
        }

        [IterationSetup(Targets = new[] { nameof(Sort_NaturallyOrderedKeys), nameof(TimSort_NaturallyOrderedKeys) } )]
        public void IterationSetup_NaturallyOrderedKeys()
        {
            // Init with an incrementing sequence.
            for(int i=0; i < _keys.Length; i++) {
                _keys[i] = i;
            }

            // Reverse multiple random sub-ranges.
            int reverseCount = (int)(Math.Sqrt(_keys.Length) * 2.0);
            int len = _keys.Length;

            for(int i=0; i < reverseCount; i++)
            {
                int idx = _rng.Next(len);
                int idx2 = _rng.Next(len);

                if(idx > idx2) {
                    VariableUtils.Swap(ref idx, ref idx2);
                }

                if(_rng.NextBool())
                {
                    Array.Reverse(_keys, 0, idx + 1);
                    Array.Reverse(_keys, idx2, len - idx2);
                }
                else
                {
                    Array.Reverse(_keys, idx, idx2 - idx);
                }
            }

            // Init value arrays (just copy key value into these).
            for(int i=0; i < _keys.Length; i++) 
            {
                _values[i] = _keys[i];
                _values2[i] = _keys[i];
            }
        }


        [Benchmark]
        public void Sort()
        {
            IntroSort<int,int,int>.Sort(_keys,_values,_values2);
        }

        [Benchmark]
        public void Sort_NaturallyOrderedKeys()
        {
            IntroSort<int,int,int>.Sort(_keys,_values,_values2);
        }

        [Benchmark]
        public void TimSort()
        {
            TimSort<int,int,int>.Sort(_keys,_values,_values2);
        }

        [Benchmark]
        public void TimSort_NaturallyOrderedKeys()
        {
            TimSort<int,int,int>.Sort(_keys,_values,_values2);
        }
    }
}
