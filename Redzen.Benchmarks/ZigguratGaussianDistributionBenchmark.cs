using BenchmarkDotNet.Attributes;
using Redzen.Random.Double;

namespace Redzen.Benchmarks
{
    public class ZigguratGaussianDistributionBenchmark
    {
        const int __loops = 10_000_000;
        readonly ZigguratGaussianDistribution _dist = new ZigguratGaussianDistribution();

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
