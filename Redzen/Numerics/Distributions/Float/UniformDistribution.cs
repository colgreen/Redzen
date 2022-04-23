// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float;

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
    public static float Sample(IRandomSource rng)
    {
        return rng.NextFloat();
    }

    /// <summary>
    /// Take a sample from the uniform distribution with interval (-1, 1).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <returns>A random sample.</returns>
    public static float SampleSigned(IRandomSource rng)
    {
        float sample = rng.NextFloat();

        uint signBit = rng.NextUInt() & 0x8000_0000U;
        SetSignBit(ref sample, ref signBit);

        return sample;
    }

    /// <summary>
    /// Take a sample from the uniform distribution with interval [0, max).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="max">Maximum value (exclusive).</param>
    /// <returns>A random sample.</returns>
    public static float Sample(IRandomSource rng, float max)
    {
        Debug.Assert(max >= 0.0);
        return rng.NextFloat() * max;
    }

    /// <summary>
    /// Take a sample from the uniform distribution with interval (-max, max).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="max">Maximum absolute value (exclusive).</param>
    /// <returns>A random sample.</returns>
    public static float SampleSigned(IRandomSource rng, float max)
    {
        Debug.Assert(max >= 0.0);

        float sample = rng.NextFloat() * max;

        uint signBit = rng.NextUInt() & 0x8000_0000U;
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
    public static float Sample(IRandomSource rng, float min, float max)
    {
        Debug.Assert(max >= min);

        return (rng.NextFloat() * (max - min)) + min;
    }

    /// <summary>
    /// Fill a span with samples from the uniform distribution with interval [0, 1).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="span">The span to fill with samples.</param>
    public static void Sample(IRandomSource rng, Span<float> span)
    {
        for(int i=0; i < span.Length; i++)
            span[i] = rng.NextFloat();
    }

    /// <summary>
    /// Fill a span with samples from the uniform distribution with interval [0, max).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="max">Maximum value (exclusive).</param>
    /// <param name="span">The span to fill with samples.</param>
    public static void Sample(IRandomSource rng, float max, Span<float> span)
    {
        Debug.Assert(max >= 0.0);

        for(int i=0; i < span.Length; i++)
            span[i] = rng.NextFloat() * max;
    }

    /// <summary>
    /// Fill a span with samples from the uniform distribution with interval (-max, max).
    /// </summary>
    /// <param name="rng">Random source.</param>
    /// <param name="max">Maximum absolute value (exclusive).</param>
    /// <param name="span">The span to fill with samples.</param>
    public static void SampleSigned(IRandomSource rng, float max, Span<float> span)
    {
        Debug.Assert(max >= 0.0);

        const int sliceSize = 32;
        uint signBits;
        uint signBit;
        float sample;

        // Generate blocks of 32 samples; this us to generate and use 32 sign bits per block.
        while(span.Length >= sliceSize)
        {
            // Generate 32 sign bits.
            signBits = rng.NextUInt();

            // Produce 32 samples.
            for(int i=0; i < sliceSize; i++)
            {
                sample = rng.NextFloat() * max;

                signBit = signBits & 0x8000_0000U;
                SetSignBit(ref sample, ref signBit);
                signBits <<= 1;

                span[i] = sample;
            }

            // Move the span forward by 32 elements.
            span = span.Slice(sliceSize);
        }

        // Handle tail elements when span length is not a multiple of sliceSize.
        if(span.Length != 0)
        {
            // Generate 32 sign bits.
            signBits = rng.NextUInt();

            for(int i=0; i < span.Length; i++)
            {
                sample = rng.NextFloat() * max;

                signBit = signBits & 0x8000_0000U;
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
    public static void Sample(IRandomSource rng, float min, float max, Span<float> span)
    {
        Debug.Assert(max >= min);

        float delta = max - min;

        for(int i=0; i < span.Length; i++)
            span[i] = (rng.NextFloat() * delta) + min;
    }

    #endregion

    #region Private Static Methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetSignBit(ref float x, ref uint signBit)
    {
        Unsafe.As<float, uint>(ref x) |= signBit;
    }

    #endregion
}
