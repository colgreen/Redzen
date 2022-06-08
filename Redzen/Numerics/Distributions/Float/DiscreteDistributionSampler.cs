// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float;

/// <summary>
/// A discrete distribution sampler.
/// </summary>
public class DiscreteDistributionSampler : ISampler<int>
{
    readonly DiscreteDistribution _dist;
    readonly IRandomSource _rng;

    #region Constructors

    /// <summary>
    /// Construct with the given distribution and a new random source.
    /// </summary>
    /// <param name="dist">Discrete distribution.</param>
    public DiscreteDistributionSampler(DiscreteDistribution dist)
    {
        _dist = dist;
        _rng = RandomDefaults.CreateRandomSource();
    }

    /// <summary>
    /// Construct with the given distribution and a new random source.
    /// </summary>
    /// <param name="dist">Discrete distribution.</param>
    /// <param name="seed">Random source seed.</param>
    public DiscreteDistributionSampler(DiscreteDistribution dist, ulong seed)
    {
        _dist = dist;
        _rng = RandomDefaults.CreateRandomSource(seed);
    }

    /// <summary>
    /// Construct with the given distribution and a random source.
    /// </summary>
    /// <param name="dist">Discrete distribution.</param>
    /// <param name="rng">Random source.</param>
    public DiscreteDistributionSampler(DiscreteDistribution dist, IRandomSource rng)
    {
        _dist = dist;
        _rng = rng;
    }

    #endregion

    #region ISampler

    /// <inheritdoc/>
    public void Sample(out int x)
    {
        x = DiscreteDistribution.Sample(_rng, _dist);
    }

    /// <inheritdoc/>
    public int Sample()
    {
        return DiscreteDistribution.Sample(_rng, _dist);
    }

    /// <inheritdoc/>
    public void Sample(Span<int> span)
    {
        DiscreteDistribution.Sample(_rng, _dist, span);
    }

    #endregion
}
