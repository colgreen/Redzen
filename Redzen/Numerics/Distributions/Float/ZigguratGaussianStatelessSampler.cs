// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float;

/// <summary>
/// A stateless Gaussian distribution sampler based on the Ziggurat algorithm.
/// </summary>
public class ZigguratGaussianStatelessSampler : IStatelessSampler<float>
{
    #region Instance Fields

    readonly float _mean;
    readonly float _stdDev;

    #endregion

    #region Constructors

    /// <summary>
    /// Construct with the given distribution parameters.
    /// </summary>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    public ZigguratGaussianStatelessSampler(float mean, float stdDev)
    {
        _mean = mean;
        _stdDev = stdDev;
    }

    #endregion

    #region IStatelessSampler

    /// <summary>
    /// Gets a random sample from the distribution.
    /// </summary>
    /// <param name="x">Reference to a variable to store the new sample value in.</param>
    /// <param name="rng">Random source.</param>
    public void Sample(out float x, IRandomSource rng)
    {
        ZigguratGaussian.Sample(rng, _mean, _stdDev, out x);
    }

    /// <summary>
    /// Returns a random sample from the distribution, using the provided <see cref="IRandomSource"/> as the source of entropy.
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <returns>A new random sample.</returns>
    public float Sample(IRandomSource rng)
    {
        return ZigguratGaussian.Sample(rng, _mean, _stdDev);
    }

    /// <summary>
    /// Fills the provided span with random samples from the distribution,
    /// using the provided <see cref="IRandomSource"/> as the source of entropy.
    /// </summary>
    /// <param name="span">The span to fill with samples.</param>
    /// <param name="rng">Random source.</param>
    public void Sample(Span<float> span, IRandomSource rng)
    {
        ZigguratGaussian.Sample(rng, _mean, _stdDev, span);
    }

    #endregion
}
