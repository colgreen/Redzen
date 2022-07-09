// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double;

/// <summary>
/// A stateless Gaussian distribution sampler based on the Box-Muller transform.
/// </summary>
public class BoxMullerGaussianStatelessSampler : IStatelessSampler<double>
{
    readonly double _mean;
    readonly double _stdDev;
    double? _sample;

    #region Constructors

    /// <summary>
    /// Construct with the default distribution parameters.
    /// </summary>
    public BoxMullerGaussianStatelessSampler()
    {
        _mean = 0.0;
        _stdDev = 1.0;
    }

    /// <summary>
    /// Construct with the given distribution parameters.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    public BoxMullerGaussianStatelessSampler(double mean, double stdDev)
    {
        _mean = mean;
        _stdDev = stdDev;
    }

    #endregion

    #region IStatelessSampler

    /// <inheritdoc/>
    public void Sample(out double x, IRandomSource rng)
    {
        if(_sample.HasValue)
        {
            x = _sample.Value;
            _sample = null;
            return;
        }

        // Note. The Box-Muller transform generates samples in pairs.
        (double x1, double x2) = BoxMullerGaussian.Sample(rng, _mean, _stdDev);

        // Return the first sample and store the other for future use.
        x = x1;
        _sample = x2;
    }

    /// <inheritdoc/>
    public double Sample(IRandomSource rng)
    {
        if(_sample.HasValue)
        {
            double x = _sample.Value;
            _sample = null;
            return x;
        }

        // Note. The Box-Muller transform generates samples in pairs.
        (double x1, double x2) = BoxMullerGaussian.Sample(rng, _mean, _stdDev);

        // Return the first sample and store the other for future use.
        _sample = x2;
        return x1;
    }

    /// <inheritdoc/>
    public void Sample(Span<double> span, IRandomSource rng)
    {
        BoxMullerGaussian.Sample(rng, _mean, _stdDev, span);
    }

    #endregion
}
