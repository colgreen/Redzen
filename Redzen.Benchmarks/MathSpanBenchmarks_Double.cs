using BenchmarkDotNet.Attributes;
using Redzen.Numerics.Distributions.Double;

namespace Redzen;

public class MathSpanBenchmarks_Double
{
    readonly UniformDistributionSampler _sampler = new(100.0, true, 0);

    readonly Memory<double> _memory = new(new double[2_000_006]);
    readonly Memory<double> _data;
    readonly Memory<double> _data2;

    public MathSpanBenchmarks_Double()
    {
        _data = _memory.Slice(0, 1_000_003);
        _data2 = _memory.Slice(1_000_003, 1_000_003);
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        _sampler.Sample(_memory.Span);
    }

    [Benchmark]
    public void Clip()
    {
        MathSpan.Clip(_data.Span, -1000.0, 1000.0);
    }

    [Benchmark]
    public void Min()
    {
        _ = MathSpan.Min<double>(_data.Span);
    }

    [Benchmark]
    public void Max()
    {
        _ = MathSpan.Max<double>(_data.Span);
    }

    [Benchmark]
    public void MinMax()
    {
        MathSpan.MinMax<double>(_data.Span, out _, out _);
    }

    [Benchmark]
    public void Multiply()
    {
        MathSpan.Multiply(_data.Span, 1.000);
    }

    [Benchmark]
    public void Sum()
    {
        _ = MathSpan.Sum<double>(_data.Span);
    }

    [Benchmark]
    public void SumOfSquares()
    {
        _ = MathSpan.SumOfSquares<double>(_data.Span);
    }

    [Benchmark]
    public void SumSquaredDelta()
    {
        MathSpan.SumSquaredDelta<double>(_data.Span, _data2.Span);
    }
}
