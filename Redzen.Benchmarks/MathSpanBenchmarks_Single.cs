using BenchmarkDotNet.Attributes;
using Redzen.Numerics.Distributions;
using Redzen.Numerics.Distributions.Float;

namespace Redzen;

public class MathSpanBenchmarks_Single
{
    readonly ISampler<float> _sampler = new UniformDistributionSampler(100f, true, 0);

    readonly Memory<float> _memory = new(new float[2_000_0006]);
    readonly Memory<float> _data;
    readonly Memory<float> _data2;

    public MathSpanBenchmarks_Single()
    {
        _data = _memory.Slice(0, 1_000_0003);
        _data2 = _memory.Slice(1_000_0003, 1_000_0003);
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _sampler.Sample(_memory.Span);
    }

    [Benchmark]
    public void Clip()
    {
        MathSpan.Clip(_data.Span, -1000f, 1000f);
    }

    [Benchmark]
    public void Min()
    {
        _ = MathSpan.Min<float>(_data.Span);
    }

    [Benchmark]
    public void Max()
    {
        _ = MathSpan.Max<float>(_data.Span);
    }

    [Benchmark]
    public void MinMax()
    {
        MathSpan.MinMax<float>(_data.Span, out _, out _);
    }

    [Benchmark]
    public void Multiply()
    {
        MathSpan.Multiply(_data.Span, 1.000f);
    }

    [Benchmark]
    public void Sum()
    {
        _ = MathSpan.Sum<float>(_data.Span);
    }

    [Benchmark]
    public void SumOfSquares()
    {
        _ = MathSpan.SumOfSquares<float>(_data.Span);
    }

    [Benchmark]
    public void SumSquaredDelta()
    {
        MathSpan.SumSquaredDelta<float>(_data.Span, _data2.Span);
    }
}
