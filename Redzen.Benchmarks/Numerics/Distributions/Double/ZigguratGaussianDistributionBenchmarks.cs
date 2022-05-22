using BenchmarkDotNet.Attributes;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double.Benchmarks;

public class ZigguratGaussianDistributionBenchmarks
{
    readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();
    readonly double[] _samples = new double[1000];

    [Benchmark]
    public void Sample()
    {
        ZigguratGaussian.Sample(_rng, out double _);
    }

    [Benchmark]
    public void SampleMeanStdDev()
    {
        ZigguratGaussian.Sample(_rng, 1.0, 2.0, out double _);
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
        ZigguratGaussian.Sample(_rng, 1.0, 2.0, samplesSpan);
    }
}
