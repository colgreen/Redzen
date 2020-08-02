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
        public double SampleStandard()
        {
            double sum = 0f;

            for(int i=0; i < __loops; i++) {
                sum += ZigguratGaussian.Sample(_rng);
            }

            return sum;
        }

        #endregion
    }
}
