using BenchmarkDotNet.Attributes;
using Redzen.Numerics.Distributions;
using Redzen.Numerics.Distributions.Float;

namespace Redzen.Sorting.Benchmarks;

public class SortUtilsBenchmarks
{
    readonly ISampler<float> _sampler = new UniformDistributionSampler(100f, true, 0);
    readonly float[] _valsSorted = new float[10_000];
    readonly float[] _valsRandom = new float[10_000];
    readonly float[] _valsUnsortedAtEnd = new float[10_000];

    [GlobalSetup]
    public void Setup()
    {
        _sampler.Sample(_valsSorted);
        _valsSorted.AsSpan().Sort();

        _sampler.Sample(_valsRandom);

        _sampler.Sample(_valsUnsortedAtEnd);
        _valsUnsortedAtEnd.AsSpan().Sort();
        _valsUnsortedAtEnd.AsSpan().Slice(_valsUnsortedAtEnd.Length - 6, 6).Reverse();
    }

    [Benchmark]
    public void IsSortedAscending_Sorted()
    {
        SortUtils.IsSortedAscending<float>(_valsSorted);
    }

    [Benchmark]
    public void IsSortedAscending_Random()
    {
        SortUtils.IsSortedAscending<float>(_valsRandom);
    }

    [Benchmark]
    public void IsSortedAscending_UnsortedAtEnd()
    {
        SortUtils.IsSortedAscending<float>(_valsUnsortedAtEnd);
    }
}
