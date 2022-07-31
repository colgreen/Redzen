using BenchmarkDotNet.Attributes;
using Redzen.Numerics.Distributions;
using Redzen.Numerics.Distributions.Double;

namespace Redzen.Benchmarks;

public class MathSpanDoubleBenchmarks
{
    readonly ISampler<double> _sampler = new UniformDistributionSampler(100.0, true, 0);

    readonly Memory<double> _memory = new(new double[2_000_0006]);
    readonly Memory<double> _data;
    readonly Memory<double> _data2;

    public MathSpanDoubleBenchmarks()
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
        MathSpan.Clip(_data.Span, -1000.0, 1000.0);
    }

    [Benchmark]
    public void Min()
    {
        _ = MathSpan.Min(_data.Span);
    }

    [Benchmark]
    public void Max()
    {
        _ = MathSpan.Max(_data.Span);
    }

    [Benchmark]
    public void MinMax()
    {
        MathSpan.MinMax(_data.Span, out _, out _);
    }

    [Benchmark]
    public void Multiply()
    {
        MathSpan.Multiply(_data.Span, 1.000);
    }

    [Benchmark]
    public void Sum()
    {
        _ = MathSpan.Sum(_data.Span);
    }

    [Benchmark]
    public void SumOfSquares()
    {
        _ = MathSpan.SumOfSquares(_data.Span);
    }

    [Benchmark]
    public void SumSquaredDelta()
    {
        MathSpan.SumSquaredDelta(_data.Span, _data2.Span);
    }
}
