using System;
using BenchmarkDotNet.Attributes;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float.Benchmarks
{
    public class BoxMullerGaussianDistributionBenchmark
    {
        readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();
        readonly float[] _samples = new float[1000];

        [Benchmark]
        public void SampleMeanStdDev()
        {
            BoxMullerGaussian.Sample(_rng, 1f, 2f);
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
            BoxMullerGaussian.Sample(_rng, 1f, 2f, samplesSpan);
        }
    }
}
