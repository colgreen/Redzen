// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double;

/// <summary>
/// A uniform distribution sampler.
/// </summary>
public class UniformDistributionSampler : ISampler<double>
{
    readonly double _max;
    readonly bool _signed;
    readonly Func<IRandomSource, double> _sampleFn;
    readonly IRandomSource _rng;

    #region Constructors

    /// <summary>
    /// Construct with the unit distribution and a new random source.
    /// </summary>
    public UniformDistributionSampler()
        : this(1.0, false, RandomDefaults.CreateRandomSource())
    {
    }

    /// <summary>
    /// Construct with the given distribution and a new random source.
    /// </summary>
    /// <param name="max">Maximum absolute value.</param>
    /// <param name="signed">Indicates if the distribution interval includes negative values.</param>
    public UniformDistributionSampler(double max, bool signed)
        : this(max, signed, RandomDefaults.CreateRandomSource())
    {
    }

    /// <summary>
    /// Construct with the given distribution and a new random source.
    /// </summary>
    /// <param name="max">Maximum absolute value.</param>
    /// <param name="signed">Indicates if the distribution interval includes negative values.</param>
    /// <param name="seed">Random source seed.</param>
    public UniformDistributionSampler(double max, bool signed, ulong seed)
        : this(max, signed, RandomDefaults.CreateRandomSource(seed))
    {
    }

    /// <summary>
    /// Construct with the given distribution and a random source.
    /// </summary>
    /// <param name="max">Maximum absolute value.</param>
    /// <param name="signed">Indicates if the distribution interval includes negative values.</param>
    /// <param name="rng">Random source.</param>
    public UniformDistributionSampler(double max, bool signed, IRandomSource rng)
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

    /// <inheritdoc/>
    public void Sample(out double x)
    {
        x = _sampleFn(_rng);
    }

    /// <inheritdoc/>
    public double Sample()
    {
        return _sampleFn(_rng);
    }

    /// <inheritdoc/>
    public void Sample(Span<double> span)
    {
        if(_signed)
            UniformDistribution.SampleSigned(_rng, _max, span);
        else
            UniformDistribution.Sample(_rng, _max, span);
    }

    #endregion
}
