using BenchmarkDotNet.Attributes;
using Redzen.Random;
using Redzen.Sorting;

namespace Redzen.Benchmarks
{
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

            for(int i=0; i < _keys.Length; i++)
            {
                _keys[i] = _rng.Next();
                _values[i] = _keys[i];
                _values2[i] = _keys[i];
            }
        }

        [IterationSetup]
        public void IterationSetup()
        {
            SortUtils.Shuffle<int>(_keys, _rng);
        }

        [Benchmark]
        public void Sort()
        {
            IntroSort<int,int,int>.Sort(_keys, _values, _values2);
        }
    }
}
