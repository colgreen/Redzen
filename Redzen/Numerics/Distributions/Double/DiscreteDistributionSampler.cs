/* ***************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015-2022 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double;

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

    /// <summary>
    /// Gets a random sample from the distribution.
    /// </summary>
    /// <param name="x">Reference to a variable to store the new sample value in.</param>
    public void Sample(out int x)
    {
        x = DiscreteDistribution.Sample(_rng, _dist);
    }

    /// <summary>
    /// Returns a random sample from the discrete distribution.
    /// </summary>
    /// <returns>A new random sample.</returns>
    public int Sample()
    {
        return DiscreteDistribution.Sample(_rng, _dist);
    }

    /// <summary>
    /// Fills the provided span with random samples from the discrete distribution.
    /// </summary>
    /// <param name="span">The span to fill with random samples.</param>
    public void Sample(Span<int> span)
    {
        DiscreteDistribution.Sample(_rng, _dist, span);
    }

    #endregion
}
