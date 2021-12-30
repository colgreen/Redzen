using System;
using BenchmarkDotNet.Attributes;
using Redzen.Numerics.Distributions;
using Redzen.Numerics.Distributions.Float;

namespace Redzen.Benchmarks
{
    public class MathSpanSingleBenchmarks
    {
        readonly ISampler<float> _sampler = new UniformDistributionSampler(100f, true, 0);
        readonly float[] _data = new float[1_000_003];

        [GlobalSetup]
        public void GlobalSetup()
        {
            _sampler.Sample(_data);
        }

        [Benchmark]
        public void MinMax()
        {
            for(int startIdx = 0; startIdx < 20; startIdx++)
                MathSpan.MinMax(_data.AsSpan(startIdx), out _, out _);
        }

        [Benchmark]
        public void SumOfSquares()
        {
            for(int startIdx = 0; startIdx < 20; startIdx++)
                MathSpan.SumOfSquares(_data.AsSpan(startIdx));
        }
    }
}
