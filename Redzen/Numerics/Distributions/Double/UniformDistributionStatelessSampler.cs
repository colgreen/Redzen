// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double;

/// <summary>
/// A stateless uniform distribution sampler.
/// </summary>
public class UniformDistributionStatelessSampler : IStatelessSampler<double>
{
    readonly double _max;
    readonly bool _signed;
    readonly Func<IRandomSource, double> _sampleFn;

    /// <summary>
    /// Construct with the given distribution and a new random source.
    /// </summary>
    /// <param name="max">Uniform distribution max value.</param>
    /// <param name="signed">If true then the distribution interval is (-max, max), otherwise it is [0, max).</param>
    public UniformDistributionStatelessSampler(double max, bool signed)
    {
        _max = max;
        _signed = signed;

        // Note. We predetermine which of these two function variants to use at construction time,
        // thus avoiding a branch on each invocation of Sample() (i.e. this is a micro-optimization).
        if(signed)
            _sampleFn = (rng) => UniformDistribution.SampleSigned(rng, _max);
        else
            _sampleFn = (rng) => UniformDistribution.Sample(rng, _max);
    }

    /// <inheritdoc/>
    public void Sample(out double x, IRandomSource rng)
    {
        x = _sampleFn(rng);
    }

    /// <inheritdoc/>
    public double Sample(IRandomSource rng)
    {
        return _sampleFn(rng);
    }

    /// <inheritdoc/>
    public void Sample(Span<double> span, IRandomSource rng)
    {
        if(_signed)
            UniformDistribution.SampleSigned(rng, _max, span);
        else
            UniformDistribution.Sample(rng, _max, span);
    }
}
