using BenchmarkDotNet.Attributes;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float.Benchmarks;

public class UniformDistributionBenchmarks
{
    readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();
    readonly float[] _samples = new float[1000];

    [Benchmark]
    public void SampleSigned()
    {
        UniformDistribution.SampleSigned(_rng);
    }

    [Benchmark]
    public void SampleSignedMax()
    {
        UniformDistribution.SampleSigned(_rng, 100f);
    }

    [Benchmark]
    public void SampleMinMax()
    {
        UniformDistribution.Sample(_rng, 1f, 100f);
    }

    [Benchmark]
    public void SampleMaxSpan()
    {
        var samplesSpan = _samples.AsSpan();
        UniformDistribution.Sample(_rng, 100f, samplesSpan);
    }

    [Benchmark]
    public void SampleSignedMaxSpan()
    {
        var samplesSpan = _samples.AsSpan();
        UniformDistribution.SampleSigned(_rng, 100f, samplesSpan);
    }

    [Benchmark]
    public void SampleMinMaxSpan()
    {
        var samplesSpan = _samples.AsSpan();
        UniformDistribution.Sample(_rng, 1f, 100f, samplesSpan);
    }
}
