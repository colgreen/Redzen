using BenchmarkDotNet.Attributes;
using Redzen.Random.Double;

namespace Redzen.Benchmarks
{
    public class BoxMullerGaussianDistributionBenchmark
    {
        const int __loops = 10_000_000;
        readonly BoxMullerGaussianDistribution _dist = new BoxMullerGaussianDistribution();

        #region Benchmark Methods [System.Random Equivalents]

        [Benchmark]
        public void SampleStandard()
        {
            for(int i=0; i<__loops; i++) {
                _dist.SampleStandard();
            }
        }

        #endregion
    }
}
