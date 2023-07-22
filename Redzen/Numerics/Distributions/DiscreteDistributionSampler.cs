// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;
using Redzen.Random;

namespace Redzen.Numerics.Distributions;

/// <summary>
/// A discrete distribution sampler.
/// </summary>
/// <typeparam name="T">Discrete probabilities floating point data type.</typeparam>
public class DiscreteDistributionSampler<T> : ISampler<int>
    where T : struct, IBinaryFloatingPointIeee754<T>
{
    readonly DiscreteDistribution<T> _dist;
    readonly IRandomSource _rng;

    #region Constructors

    /// <summary>
    /// Construct with the given distribution and a new random source.
    /// </summary>
    /// <param name="dist">Discrete distribution.</param>
    public DiscreteDistributionSampler(
        DiscreteDistribution<T> dist)
    {
        _dist = dist;
        _rng = RandomDefaults.CreateRandomSource();
    }

    /// <summary>
    /// Construct with the given distribution and a new random source.
    /// </summary>
    /// <param name="dist">Discrete distribution.</param>
    /// <param name="seed">Random source seed.</param>
    public DiscreteDistributionSampler(
        DiscreteDistribution<T> dist,
        ulong seed)
    {
        _dist = dist;
        _rng = RandomDefaults.CreateRandomSource(seed);
    }

    /// <summary>
    /// Construct with the given distribution and a random source.
    /// </summary>
    /// <param name="dist">Discrete distribution.</param>
    /// <param name="rng">Random source.</param>
    public DiscreteDistributionSampler(
        DiscreteDistribution<T> dist,
        IRandomSource rng)
    {
        _dist = dist;
        _rng = rng;
    }

    #endregion

    #region ISampler

    /// <inheritdoc/>
    public void Sample(out int x)
    {
        x = _dist.Sample(_rng);
    }

    /// <inheritdoc/>
    public int Sample()
    {
        return _dist.Sample(_rng);
    }

    /// <inheritdoc/>
    public void Sample(Span<int> span)
    {
        _dist.Sample(span, _rng);
    }

    #endregion
}
