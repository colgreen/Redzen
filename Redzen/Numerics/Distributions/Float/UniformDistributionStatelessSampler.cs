// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float;

/// <summary>
/// A stateless uniform distribution sampler.
/// </summary>
public class UniformDistributionStatelessSampler : IStatelessSampler<float>
{
    readonly float _max = 1.0f;
    readonly bool _signed = false;
    readonly Func<IRandomSource, float> _sampleFn;

    #region Constructors

    /// <summary>
    /// Construct with the given distribution and a new random source.
    /// </summary>
    /// <param name="max">Uniform distribution max value.</param>
    /// <param name="signed">If true then the distribution interval is (-max, max), otherwise it is [0, max).</param>
    public UniformDistributionStatelessSampler(float max, bool signed)
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

    #endregion

    #region IStatelessSampler

    /// <summary>
    /// Gets a random sample from the distribution.
    /// </summary>
    /// <param name="x">Reference to a variable to store the new sample value in.</param>
    /// <param name="rng">Random source.</param>
    public void Sample(out float x, IRandomSource rng)
    {
        x = _sampleFn(rng);
    }

    /// <summary>
    /// Returns a random sample from the distribution,
    /// using the provided <see cref="IRandomSource"/> as the source of entropy.
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <returns>A new random sample.</returns>
    public float Sample(IRandomSource rng)
    {
        return _sampleFn(rng);
    }

    /// <summary>
    /// Fills the provided span with random samples from the distribution,
    /// using the provided <see cref="IRandomSource"/> as the source of entropy.
    /// </summary>
    /// <param name="span">The span to fill with samples.</param>
    /// <param name="rng">Random source.</param>
    public void Sample(Span<float> span, IRandomSource rng)
    {
        if(_signed)
            UniformDistribution.SampleSigned(rng, _max, span);
        else
            UniformDistribution.Sample(rng, _max, span);
    }

    #endregion
}
