// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double;

/// <summary>
/// A stateless Gaussian distribution sampler based on the Ziggurat algorithm.
/// </summary>
public class ZigguratGaussianStatelessSampler : IStatelessSampler<double>
{
    #region Instance Fields

    readonly double _mean;
    readonly double _stdDev;

    #endregion

    #region Constructors

    /// <summary>
    /// Construct with the default distribution parameters.
    /// </summary>
    public ZigguratGaussianStatelessSampler()
    {
        _mean = 0.0;
        _stdDev = 1.0;
    }

    /// <summary>
    /// Construct with the given distribution parameters.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    public ZigguratGaussianStatelessSampler(double mean, double stdDev)
    {
        _mean = mean;
        _stdDev = stdDev;
    }

    #endregion

    #region IStatelessSampler

    /// <inheritdoc/>
    public void Sample(out double x, IRandomSource rng)
    {
        ZigguratGaussian.Sample(rng, _mean, _stdDev, out x);
    }

    /// <inheritdoc/>
    public double Sample(IRandomSource rng)
    {
        return ZigguratGaussian.Sample(rng, _mean, _stdDev);
    }

    /// <inheritdoc/>
    public void Sample(Span<double> span, IRandomSource rng)
    {
        ZigguratGaussian.Sample(rng, _mean, _stdDev, span);
    }

    #endregion
}
