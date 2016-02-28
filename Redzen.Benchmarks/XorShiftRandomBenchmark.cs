using BenchmarkDotNet.Attributes;
using Redzen.Numerics;

namespace Redzen.Benchmarks
{
    public class XorShiftRandomBenchmark
    {
        const int __loops = 10000000;
        XorShiftRandom _rng = new XorShiftRandom();
        byte[] _buff = new byte[1000000];

        #region Benchmark Methods [System.Random Equivalents]

        [Benchmark]
        public void Next10M()
        {
            for(int i=0; i<__loops; i++) {
                _rng.Next();
            }
        }

        [Benchmark]
        public void NextUpperBound10M()
        {
            for(int i=0; i<__loops; i++) {
                _rng.Next(123);
            }
        }

        [Benchmark]
        public void NextLowerUpperBound10M()
        {
            for(int i=0; i<__loops; i++) {
                _rng.Next(1000, 10000);
            }
        }

        [Benchmark]
        public void NextDouble10M()
        {
            for(int i=0; i<__loops; i++) {
                _rng.NextDouble();
            }
        }

        [Benchmark]
        public void NextBytes100M()
        {
            for(int i=0; i<100; i++) {
                _rng.NextBytes(_buff);
            }
        }

        #endregion

        #region Benchmark Methods [Methods not present on System.Random]

        [Benchmark]
        public void NextFloat10M()
        {
            for(int i=0; i<__loops; i++) {
                _rng.NextFloat();
            }
        }

        [Benchmark]
        public void NextUInt10M()
        {
            for(int i=0; i<__loops; i++) {
                _rng.NextUInt();
            }
        }

        [Benchmark]
        public void NextInt()
        {
            for(int i=0; i<__loops; i++) {
                _rng.NextInt();
            }
        }

        [Benchmark]
        public void NextDoubleNonZero10M()
        {
            for(int i=0; i<__loops; i++) {
                _rng.NextDoubleNonZero();
            }
        }

        [Benchmark]
        public void NextBool()
        {
            for(int i=0; i<__loops; i++) {
                _rng.NextBool();
            }
        }

        [Benchmark]
        public void NextByte()
        {
            for(int i=0; i<__loops; i++) {
                _rng.NextByte();
            }
        }

        //[Benchmark]
        //public void NextBytes8_100M()
        //{
        //    for(int i=0; i<100; i++) {
        //        _rng.NextBytes8(_buff);
        //    }
        //}

        #endregion
    }
}
