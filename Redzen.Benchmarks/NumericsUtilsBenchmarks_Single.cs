using BenchmarkDotNet.Attributes;
using Redzen.Numerics;
using Redzen.Numerics.Distributions.Float;
using Redzen.Random;

namespace Redzen;

public class NumericsUtilsBenchmarks_Single
{
    readonly UniformDistributionSampler _sampler = new(100f, true, 0);
    readonly Memory<float> _memory = new(new float[1_000]);
    readonly IRandomSource _rng = RandomDefaults.CreateRandomSource();

    [GlobalSetup]
    public void GlobalSetup()
    {
        _sampler.Sample(_memory.Span);
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
