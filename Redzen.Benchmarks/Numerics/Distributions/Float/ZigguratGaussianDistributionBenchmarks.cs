using BenchmarkDotNet.Attributes;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float.Benchmarks;

public class ZigguratGaussianDistributionBenchmarks
{
    readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();
    readonly float[] _samples = new float[1000];

    [Benchmark]
    public void Sample()
    {
        ZigguratGaussian.Sample(_rng, out float _);
    }

    [Benchmark]
    public void SampleMeanStdDev()
    {
        ZigguratGaussian.Sample(_rng, 1f, 2f, out float _);
    }

    [Benchmark]
    public void Sample_Span()
    {
        var samplesSpan = _samples.AsSpan();
        ZigguratGaussian.Sample(_rng, samplesSpan);
    }

    [Benchmark]
    public void SampleMeanStdDev_Span()
    {
        var samplesSpan = _samples.AsSpan();
        ZigguratGaussian.Sample(_rng, 1f, 2f, samplesSpan);
    }
}
