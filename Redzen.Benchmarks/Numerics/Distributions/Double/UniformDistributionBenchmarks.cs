using System;
using BenchmarkDotNet.Attributes;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double.Benchmarks;

public class UniformDistributionBenchmarks
{
    readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();
    readonly double[] _samples = new double[1000];

    [Benchmark]
    public void SampleSigned()
    {
        UniformDistribution.SampleSigned(_rng);
    }

    [Benchmark]
    public void SampleSignedMax()
    {
        UniformDistribution.SampleSigned(_rng, 100.0);
    }

    [Benchmark]
    public void SampleMinMax()
    {
        UniformDistribution.Sample(_rng, 1.0, 100.0);
    }

    [Benchmark]
    public void SampleMaxSpan()
    {
        var samplesSpan = _samples.AsSpan();
        UniformDistribution.Sample(_rng, 100.0, samplesSpan);
    }

    [Benchmark]
    public void SampleSignedMaxSpan()
    {
        var samplesSpan = _samples.AsSpan();
        UniformDistribution.SampleSigned(_rng, 100.0, samplesSpan);
    }

    [Benchmark]
    public void SampleMinMaxSpan()
    {
        var samplesSpan = _samples.AsSpan();
        UniformDistribution.Sample(_rng, 1.0, 100.0, samplesSpan);
    }
}
