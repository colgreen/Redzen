// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float;

/// <summary>
/// A stateless Gaussian distribution sampler based on the Ziggurat algorithm.
/// </summary>
public class ZigguratGaussianStatelessSampler : IStatelessSampler<float>
{
    readonly float _mean;
    readonly float _stdDev;

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

    /// <inheritdoc/>
    public void Sample(out float x, IRandomSource rng)
    {
        ZigguratGaussian.Sample(rng, _mean, _stdDev, out x);
    }

    /// <inheritdoc/>
    public float Sample(IRandomSource rng)
    {
        return ZigguratGaussian.Sample(rng, _mean, _stdDev);
    }

    /// <inheritdoc/>
    public void Sample(Span<float> span, IRandomSource rng)
    {
        ZigguratGaussian.Sample(rng, _mean, _stdDev, span);
    }
}
