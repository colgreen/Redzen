using BenchmarkDotNet.Attributes;
using Redzen.Random;
using System;

namespace Redzen.Benchmarks
{
    public class ZigguratGaussianDistributionBenchmark
    {
        const int __loops = 10_000;
        readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();
        readonly double[] _samples = new double[1000];
        readonly float[] _samplesF = new float[1000];

        [Benchmark]
        public void SampleStandard_Span_Double()
        {
            var samplesSpan = _samples.AsSpan();

            for (int i=0; i < __loops; i++)
            {
                Numerics.Distributions.Double.ZigguratGaussian.Sample(_rng, samplesSpan);
            }
        }

        [Benchmark]
        public void SampleStandard_Span_Float()
        {
            var samplesSpan = _samplesF.AsSpan();

            for (int i=0; i < __loops; i++)
            {
                Numerics.Distributions.Float.ZigguratGaussian.Sample(_rng, samplesSpan);
            }
        }
    }
}
