using BenchmarkDotNet.Attributes;
using Redzen.Numerics.Distributions.Double;
using Redzen.Random;

namespace Redzen.Benchmarks
{
    public class ZigguratGaussianDistributionBenchmark
    {
        const int __loops = 10_000_000;
        readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();

        #region Benchmark Methods [System.Random Equivalents]

        [Benchmark]
        public void SampleStandard()
        {
            for(int i=0; i<__loops; i++) {
                ZigguratGaussian.Sample(_rng);
            }
        }

        #endregion
    }
}
