// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double;

/// <summary>
/// A Gaussian distribution sampler based on the Box-Muller transform.
/// </summary>
public class BoxMullerGaussianSampler : ISampler<double>
{
    #region Instance Fields

    readonly double _mean;
    readonly double _stdDev;
    readonly IRandomSource _rng;
    double? _sample;

    #endregion

    #region Constructors

    /// <summary>
    /// Construct with the default distribution parameters, and a new random source.
    /// </summary>
    public BoxMullerGaussianSampler()
        : this(0.0, 1.0, RandomDefaults.CreateRandomSource())
    {
    }

    /// <summary>
    /// Construct with the given distribution parameters, and a new random source.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    public BoxMullerGaussianSampler(double mean, double stdDev)
        : this(mean, stdDev, RandomDefaults.CreateRandomSource())
    {
    }

    /// <summary>
    /// Construct with the given distribution parameters, and a new random source.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    /// <param name="seed">Random source seed.</param>
    public BoxMullerGaussianSampler(double mean, double stdDev, ulong seed)
        : this(mean, stdDev, RandomDefaults.CreateRandomSource(seed))
    {
    }

    /// <summary>
    /// Construct with the given distribution parameters, and a random source.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    /// <param name="rng">Random source.</param>
    public BoxMullerGaussianSampler(double mean, double stdDev, IRandomSource rng)
    {
        _mean = mean;
        _stdDev = stdDev;
        _rng = rng;
    }

    #endregion

    #region ISampler

    /// <inheritdoc/>
    public void Sample(out double x)
    {
        if(_sample.HasValue)
        {
            x = _sample.Value;
            _sample = null;
            return;
        }

        // Note. The Box-Muller transform generates samples in pairs.
        (double x1, double x2) = BoxMullerGaussian.Sample(_rng, _mean, _stdDev);

        // Return the first sample and store the other for future use.
        x = x1;
        _sample = x2;
        return;
    }

    /// <inheritdoc/>
    public double Sample()
    {
        if(_sample.HasValue)
        {
            double x = _sample.Value;
            _sample = null;
            return x;
        }

        // Note. The Box-Muller transform generates samples in pairs.
        (double x1, double x2) = BoxMullerGaussian.Sample(_rng, _mean, _stdDev);

        // Return the first sample and store the other for future use.
        _sample = x2;
        return x1;
    }

    /// <inheritdoc/>
    public void Sample(Span<double> span)
    {
        BoxMullerGaussian.Sample(_rng, _mean, _stdDev, span);
    }

    #endregion
}
