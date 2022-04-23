// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float;

/// <summary>
/// A uniform distribution sampler.
/// </summary>
public class UniformDistributionSampler : ISampler<float>
{
    readonly float _max = 1.0f;
    readonly bool _signed = false;
    readonly Func<IRandomSource, float> _sampleFn;
    readonly IRandomSource _rng;

    #region Constructors

    /// <summary>
    /// Construct with the unit distribution and a new random source.
    /// </summary>
    public UniformDistributionSampler()
        : this(1f, false, RandomDefaults.CreateRandomSource())
    {
    }

    /// <summary>
    /// Construct with the given distribution and a new random source.
    /// </summary>
    /// <param name="max">Maximum absolute value.</param>
    /// <param name="signed">Indicates if the distribution interval includes negative values.</param>
    public UniformDistributionSampler(float max, bool signed)
        : this(max, signed, RandomDefaults.CreateRandomSource())
    {
    }

    /// <summary>
    /// Construct with the given distribution and a new random source.
    /// </summary>
    /// <param name="max">Maximum absolute value.</param>
    /// <param name="signed">Indicates if the distribution interval includes negative values.</param>
    /// <param name="seed">Random source seed.</param>
    public UniformDistributionSampler(float max, bool signed, ulong seed)
        : this(max, signed, RandomDefaults.CreateRandomSource(seed))
    {
    }

    /// <summary>
    /// Construct with the given distribution and a random source.
    /// </summary>
    /// <param name="max">Maximum absolute value.</param>
    /// <param name="signed">Indicates if the distribution interval includes negative values.</param>
    /// <param name="rng">Random source.</param>
    public UniformDistributionSampler(float max, bool signed, IRandomSource rng)
    {
        _max = max;
        _signed = signed;
        _rng = rng;

        // Note. We predetermine which of these two function variants to use at construction time,
        // thus avoiding a branch on each invocation of Sample() (i.e. this is a micro-optimization).
        if(signed)
            _sampleFn = (r) => UniformDistribution.SampleSigned(r, _max);
        else
            _sampleFn = (r) => UniformDistribution.Sample(r, _max);
    }

    #endregion

    #region ISampler

    /// <summary>
    /// Gets a random sample from the distribution.
    /// </summary>
    /// <param name="x">Reference to a variable to store the new sample value in.</param>
    public void Sample(out float x)
    {
        x = _sampleFn(_rng);
    }

    /// <summary>
    /// Returns a random sample from the distribution.
    /// </summary>
    /// <returns>A new random sample.</returns>
    public float Sample()
    {
        return _sampleFn(_rng);
    }

    /// <summary>
    /// Fills the provided span with random samples from the distribution.
    /// </summary>
    /// <param name="span">The span to fill with samples.</param>
    public void Sample(Span<float> span)
    {
        if(_signed)
            UniformDistribution.SampleSigned(_rng, _max, span);
        else
            UniformDistribution.Sample(_rng, _max, span);
    }

    #endregion
}
