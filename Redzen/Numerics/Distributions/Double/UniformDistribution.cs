// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double;

/// <summary>
/// Static methods for taking samples from uniform distributions.
/// </summary>
public static class UniformDistribution
{
    #region Public Static Methods

    /// <summary>
    /// Take a sample from the uniform distribution with interval [0, 1).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <returns>A random sample.</returns>
    public static double Sample(IRandomSource rng)
    {
        return rng.NextDouble();
    }

    /// <summary>
    /// Take a sample from the uniform distribution with interval (-1, 1).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <returns>A random sample.</returns>
    public static double SampleSigned(IRandomSource rng)
    {
        double sample = rng.NextDouble();

        ulong signBit = rng.NextULong() & 0x8000_0000_0000_0000UL;
        SetSignBit(ref sample, ref signBit);

        return sample;
    }

    /// <summary>
    /// Take a sample from the uniform distribution with interval [0, max).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="max">Maximum value (exclusive).</param>
    /// <returns>A random sample.</returns>
    public static double Sample(IRandomSource rng, double max)
    {
        Debug.Assert(max >= 0.0);
        return rng.NextDouble() * max;
    }

    /// <summary>
    /// Take a sample from the uniform distribution with interval (-max, max).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="max">Maximum absolute value (exclusive).</param>
    /// <returns>A random sample.</returns>
    public static double SampleSigned(IRandomSource rng, double max)
    {
        Debug.Assert(max >= 0.0);

        double sample = rng.NextDouble() * max;

        ulong signBit = rng.NextULong() & 0x8000_0000_0000_0000UL;
        SetSignBit(ref sample, ref signBit);

        return sample;
    }

    /// <summary>
    /// Take a sample from the uniform distribution with interval [min, max).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="min">Minimum value (inclusive).</param>
    /// <param name="max">Maximum value (exclusive).</param>
    /// <returns>A random sample.</returns>
    public static double Sample(IRandomSource rng, double min, double max)
    {
        Debug.Assert(max >= min);

        return (rng.NextDouble() * (max - min)) + min;
    }

    /// <summary>
    /// Fill a span with samples from the uniform distribution with interval [0, 1).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="span">The span to fill with samples.</param>
    public static void Sample(IRandomSource rng, Span<double> span)
    {
        for(int i=0; i < span.Length; i++)
            span[i] = rng.NextDouble();
    }

    /// <summary>
    /// Fill a span with samples from the uniform distribution with interval [0, max).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="max">Maximum value (exclusive).</param>
    /// <param name="span">The span to fill with samples.</param>
    public static void Sample(IRandomSource rng, double max, Span<double> span)
    {
        Debug.Assert(max >= 0.0);

        for(int i=0; i < span.Length; i++)
            span[i] = rng.NextDouble() * max;
    }

    /// <summary>
    /// Fill a span with samples from the uniform distribution with interval (-max, max).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="max">Maximum absolute value (exclusive).</param>
    /// <param name="span">The span to fill with samples.</param>
    public static void SampleSigned(IRandomSource rng, double max, Span<double> span)
    {
        Debug.Assert(max >= 0.0);

        const int sliceSize = 64;
        ulong signBits;
        ulong signBit;
        double sample;

        // Generate blocks of 64 samples; this us to generate and use 64 sign bits per block.
        while(span.Length >= sliceSize)
        {
            // Generate 64 sign bits.
            signBits = rng.NextULong();

            // Produce 64 samples.
            for(int i=0; i < sliceSize; i++)
            {
                sample = rng.NextDouble() * max;

                signBit = signBits & 0x8000_0000_0000_0000UL;
                SetSignBit(ref sample, ref signBit);
                signBits <<= 1;

                span[i] = sample;
            }

            // Move the span forward by 64 elements.
            span = span.Slice(sliceSize);
        }

        // Handle tail elements when span length is not a multiple of sliceSize.
        if(span.Length != 0)
        {
            // Generate 64 sign bits.
            signBits = rng.NextULong();

            for(int i=0; i < span.Length; i++)
            {
                sample = rng.NextDouble() * max;

                signBit = signBits & 0x8000_0000_0000_0000UL;
                SetSignBit(ref sample, ref signBit);
                signBits <<= 1;

                span[i] = sample;
            }
        }
    }

    /// <summary>
    /// Fill a span with samples from the uniform distribution with interval [min, max).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="min">Minimum value (inclusive).</param>
    /// <param name="max">Maximum value (exclusive).</param>
    /// <param name="span">The span to fill with samples.</param>
    public static void Sample(IRandomSource rng, double min, double max, Span<double> span)
    {
        Debug.Assert(max >= min);

        double delta = max - min;

        for(int i=0; i < span.Length; i++)
            span[i] = (rng.NextDouble() * delta) + min;
    }

    #endregion

    #region Private Static Methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetSignBit(ref double x, ref ulong signBit)
    {
        Unsafe.As<double, ulong>(ref x) |= signBit;
    }

    #endregion
}
