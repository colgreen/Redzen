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
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float;

/// <summary>
/// A Gaussian distribution sampler based on the Ziggurat algorithm.
/// </summary>
public class ZigguratGaussianSampler : ISampler<float>
{
    readonly float _mean;
    readonly float _stdDev;
    readonly IRandomSource _rng;

    #region Constructors

    /// <summary>
    /// Construct with the default distribution parameters, and a new random source.
    /// </summary>
    public ZigguratGaussianSampler()
        : this(0f, 1f, RandomDefaults.CreateRandomSource())
    {
    }

    /// <summary>
    /// Construct with the given distribution parameters, and a new random source.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    public ZigguratGaussianSampler(float mean, float stdDev)
        : this(mean, stdDev, RandomDefaults.CreateRandomSource())
    {
    }

    /// <summary>
    /// Construct with the given distribution parameters, and a new random source.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    /// <param name="seed">Random source seed.</param>
    public ZigguratGaussianSampler(float mean, float stdDev, ulong seed)
        : this(mean, stdDev, RandomDefaults.CreateRandomSource(seed))
    {
    }

    /// <summary>
    /// Construct with the given distribution parameters, and a random source.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    /// <param name="rng">Random source.</param>
    public ZigguratGaussianSampler(float mean, float stdDev, IRandomSource rng)
    {
        _mean = mean;
        _stdDev = stdDev;
        _rng = rng;
    }

    #endregion

    #region ISampler

    /// <summary>
    /// Returns a random sample from the distribution.
    /// </summary>
    /// <param name="x">Reference to a variable to store the new sample value in.</param>
    public void Sample(out float x)
    {
        ZigguratGaussian.Sample(_rng, _mean, _stdDev, out x);
    }

    /// <summary>
    /// Take a sample from the distribution.
    /// </summary>
    /// <returns>A random sample.</returns>
    public float Sample()
    {
        return ZigguratGaussian.Sample(_rng, _mean, _stdDev);
    }

    /// <summary>
    /// Fill a span with samples from the distribution.
    /// </summary>
    /// <param name="span">The span to fill with samples.</param>
    public void Sample(Span<float> span)
    {
        ZigguratGaussian.Sample(_rng, _mean, _stdDev, span);
    }

    #endregion
}
