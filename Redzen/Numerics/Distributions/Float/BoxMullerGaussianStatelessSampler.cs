// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float;

/// <summary>
/// A stateless Gaussian distribution sampler based on the Box-Muller transform.
/// </summary>
public class BoxMullerGaussianStatelessSampler : IStatelessSampler<float>
{
    readonly float _mean;
    readonly float _stdDev;
    float? _sample;

    #region Constructors

    /// <summary>
    /// Construct with the default distribution parameters.
    /// </summary>
    public BoxMullerGaussianStatelessSampler()
    {
        _mean = 0f;
        _stdDev = 1f;
    }

    /// <summary>
    /// Construct with the given distribution parameters.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    public BoxMullerGaussianStatelessSampler(float mean, float stdDev)
    {
        _mean = mean;
        _stdDev = stdDev;
    }

    #endregion

    #region IStatelessSampler

    /// <inheritdoc/>
    public void Sample(out float x, IRandomSource rng)
    {
        if(_sample.HasValue)
        {
            x = _sample.Value;
            _sample = null;
            return;
        }

        // Note. The Box-Muller transform generates samples in pairs.
        (float x1, float x2) = BoxMullerGaussian.Sample(rng, _mean, _stdDev);

        // Return the first sample and store the other for future use.
        x = x1;
        _sample = x2;
    }

    /// <inheritdoc/>
    public float Sample(IRandomSource rng)
    {
        if(_sample.HasValue)
        {
            float x = _sample.Value;
            _sample = null;
            return x;
        }

        // Note. The Box-Muller transform generates samples in pairs.
        (float x1, float x2) = BoxMullerGaussian.Sample(rng, _mean, _stdDev);

        // Return the first sample and store the other for future use.
        _sample = x2;
        return x1;
    }

    /// <inheritdoc/>
    public void Sample(Span<float> span, IRandomSource rng)
    {
        BoxMullerGaussian.Sample(rng, _mean, _stdDev, span);
    }

    #endregion
}
