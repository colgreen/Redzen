﻿using BenchmarkDotNet.Attributes;
using Redzen.Numerics;
using Redzen.Numerics.Distributions;
using Redzen.Numerics.Distributions.Double;
using Redzen.Random;

namespace Redzen;

public class NumericsUtilsBenchmarks_Double
{
    readonly ISampler<double> _sampler = new UniformDistributionSampler(100.0, true, 0);
    readonly Memory<double> _memory = new(new double[1_000]);
    readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();

    [GlobalSetup]
    public void GlobalSetup()
    {
        _sampler.Sample(_memory.Span);
    }

    [Benchmark]
    public void StochasticRound()
    {
        var span = _memory.Span;
        
        for(int i=0; i < span.Length; i++)
        {
            NumericsUtils.StochasticRound(span[i], _rng);
        }
    }

    [Benchmark]
    public void StochasticRoundGeneric()
    {
        var span = _memory.Span;

        for(int i=0; i < span.Length; i++)
        {
            NumericsUtils.StochasticRound<double>(span[i], _rng);
        }
    }
}
