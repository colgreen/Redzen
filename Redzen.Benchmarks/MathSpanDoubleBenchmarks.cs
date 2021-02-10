using System;
using BenchmarkDotNet.Attributes;
using Redzen.Numerics.Distributions;
using Redzen.Numerics.Distributions.Double;

namespace Redzen.Benchmarks
{
    public class MathSpanDoubleBenchmarks
    {
        readonly ISampler<double> _sampler = new UniformDistributionSampler(100.0, true, 0);
        readonly double[] _data = new double[1_000_003];

        [GlobalSetup]
        public void GlobalSetup()
        {
            _sampler.Sample(_data);
        }

        [Benchmark]
        public void MinMax()
        {
            for(int startIdx = 0; startIdx < 20; startIdx++) 
            {
                MathSpan.MinMax(_data.AsSpan(startIdx), out double actualMin, out double actualMax);
            }   
        }
    }
}
