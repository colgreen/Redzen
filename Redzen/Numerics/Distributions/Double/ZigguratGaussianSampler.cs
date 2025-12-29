// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double;

/// <summary>
/// A Gaussian distribution sampler based on the Ziggurat algorithm.
/// </summary>
public class ZigguratGaussianSampler : ISampler<double>
{
    readonly double _mean;
    readonly double _stdDev;
    readonly IRandomSource _rng;

    /// <summary>
    /// Construct with the default distribution parameters, and a new random source.
    /// </summary>
    public ZigguratGaussianSampler()
        : this(0.0, 1.0, RandomDefaults.CreateRandomSource())
    {
    }

    /// <summary>
    /// Construct with the given distribution parameters, and a new random source.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    public ZigguratGaussianSampler(double mean, double stdDev)
        : this(mean, stdDev, RandomDefaults.CreateRandomSource())
    {
    }

    /// <summary>
    /// Construct with the given distribution parameters, and a new random source.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    /// <param name="seed">Random source seed.</param>
    public ZigguratGaussianSampler(double mean, double stdDev, ulong seed)
        : this(mean, stdDev, RandomDefaults.CreateRandomSource(seed))
    {
    }

    /// <summary>
    /// Construct with the given distribution parameters, and a random source.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    /// <param name="rng">Random source.</param>
    public ZigguratGaussianSampler(double mean, double stdDev, IRandomSource rng)
    {
        _mean = mean;
        _stdDev = stdDev;
        _rng = rng;
    }

    /// <inheritdoc/>
    public void Sample(out double x)
    {
        ZigguratGaussian.Sample(_rng, _mean, _stdDev, out x);
    }

    /// <inheritdoc/>
    public double Sample()
    {
        return ZigguratGaussian.Sample(_rng, _mean, _stdDev);
    }

    /// <inheritdoc/>
    public void Sample(Span<double> span)
    {
        ZigguratGaussian.Sample(_rng, _mean, _stdDev, span);
    }
}
