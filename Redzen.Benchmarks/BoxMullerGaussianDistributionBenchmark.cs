using BenchmarkDotNet.Attributes;
using Redzen.Random;

namespace Redzen.Benchmarks
{
    public class BoxMullerGaussianDistributionBenchmark
    {
        const int __loops = 10_000_000;
        readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();

        [Benchmark]
        public void SampleStandard_Double()
        {
            for (int i=0; i < __loops; i++)
            {
                Numerics.Distributions.Double.BoxMullerGaussian.Sample(_rng);
            }
        }

        [Benchmark]
        public void SampleStandard_Float()
        {
            for(int i=0; i < __loops; i++)
            {
                Numerics.Distributions.Float.BoxMullerGaussian.Sample(_rng);
            }
        }

    }
}
