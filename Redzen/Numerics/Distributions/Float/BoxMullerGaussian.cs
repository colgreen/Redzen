﻿// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float;

#pragma warning disable SA1414 // Tuple types in signatures should have element names

/// <summary>
/// Static methods for taking samples from Gaussian distributions using the Box-Muller transform.
/// (see: http://en.wikipedia.org/wiki/Box_Muller_transform).
/// </summary>
public static class BoxMullerGaussian
{
    /// <summary>
    /// Take a sample from the standard Gaussian distribution, i.e. with mean of 0 and standard deviation of 1.
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <returns>A pair of random samples (because the Box-Muller transform generates samples in pairs).</returns>
    public static (float, float) Sample(IRandomSource rng)
    {
        // Generate two new Gaussian values.
        float x, y, sqr;

        // We need a non-zero random point inside the unit circle.
        do
        {
            x = rng.NextFloat();
            y = rng.NextFloat();

            x = (x * 2f) - 1f;
            y = (y * 2f) - 1f;

            sqr = (x * x) + (y * y);
        }
        while(sqr > 1f || sqr == 0f);

        // Make the Box-Muller transformation.
        float fac = MathF.Sqrt((-2f * MathF.Log(sqr)) / sqr);

        // Return two samples.
        return (x * fac, y * fac);
    }

    /// <summary>
    /// Take a sample from the a Gaussian distribution with the specified mean and standard deviation.
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    /// <returns>A pair of random samples (because the Box-Muller transform generates samples in pairs).</returns>
    public static (float, float) Sample(IRandomSource rng, float mean, float stdDev)
    {
        var pair = Sample(rng);

        pair.Item1 = mean + (pair.Item1 * stdDev);
        pair.Item2 = mean + (pair.Item2 * stdDev);

        return pair;
    }

    /// <summary>
    /// Fill a span with samples from the standard Gaussian distribution, i.e. with mean of 0 and standard deviation of 1.
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="span">The span to fill with samples.</param>
    public static void Sample(IRandomSource rng, Span<float> span)
    {
        int i=0;
        for(; i <= span.Length - 2; i += 2)
        {
            (float a, float b) = Sample(rng);
            span[i] = a;
            span[i + 1] = b;
        }

        if(i < span.Length)
            span[i] = Sample(rng).Item1;
    }

    /// <summary>
    /// Fill a span with samples from a Gaussian distribution with the specified mean and standard deviation.
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="mean">Distribution mean.</param>
    /// <param name="stdDev">Distribution standard deviation.</param>
    /// <param name="span">The span to fill with samples.</param>
    public static void Sample(IRandomSource rng, float mean, float stdDev, Span<float> span)
    {
        int i=0;
        for(; i <= span.Length - 2; i += 2)
        {
            (float a, float b) = Sample(rng);
            span[i] = (a * stdDev) + mean;
            span[i + 1] = (b * stdDev) + mean;
        }

        if(i < span.Length)
            span[i] = (Sample(rng).Item1 * stdDev) + mean;
    }
}
