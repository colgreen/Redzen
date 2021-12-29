using BenchmarkDotNet.Attributes;

namespace Redzen.Benchmarks
{
    public class SystemRandomBenchmark
    {
        const int __loops = 10_000_000;
        readonly System.Random _rng = new();
        readonly byte[] _buff = new byte[1_000_000];

        [Benchmark]
        public void Next10M()
        {
            for(int i=0; i < __loops; i++)
                _rng.Next();
        }

        [Benchmark]
        public void NextUpperBound10M()
        {
            for(int i=0; i < __loops; i++)
                _rng.Next(123);
        }

        [Benchmark]
        public void NextLowerUpperBound10M()
        {
            for(int i=0; i < __loops; i++)
                _rng.Next(1000, 10_000);
        }

        [Benchmark]
        public void NextDouble10M()
        {
            for(int i=0; i < __loops; i++)
                _rng.NextDouble();
        }

        [Benchmark]
        public void NextBytes100M()
        {
            for(int i=0; i < 100; i++)
                _rng.NextBytes(_buff);
        }
    }
}
