using System;
using BenchmarkDotNet.Attributes;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double.Benchmarks
{
    public class BoxMullerGaussianDistributionBenchmarks
    {
        readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();
        readonly double[] _samples = new double[1000];

        [Benchmark]
        public void SampleMeanStdDev()
        {
            BoxMullerGaussian.Sample(_rng, 1.0, 2.0);
        }

        [Benchmark]
        public void Sample_Span()
        {
            var samplesSpan = _samples.AsSpan();
            BoxMullerGaussian.Sample(_rng, samplesSpan);
        }

        [Benchmark]
        public void SampleMeanStdDev_Span()
        {
            var samplesSpan = _samples.AsSpan();
            BoxMullerGaussian.Sample(_rng, 1.0, 2.0, samplesSpan);
        }
    }
}
