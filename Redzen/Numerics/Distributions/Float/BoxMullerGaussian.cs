/* ***************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015-2021 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Numerics;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float
{
    /// <summary>
    /// Static methods for taking samples from Gaussian distributions using the Box-Muller transform.
    /// (see: http://en.wikipedia.org/wiki/Box_Muller_transform).
    /// </summary>
    public static class BoxMullerGaussian
    {
        #region Public Static Methods

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

                if(Vector.IsHardwareAccelerated)
                {
                    x = MathF.FusedMultiplyAdd(x, 2f, -1f);
                    y = MathF.FusedMultiplyAdd(y, 2f, -1f);
                }
                else
                {
                    x = (x * 2f) - 1f;
                    y = (y * 2f) - 1f;
                }

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

            if(Vector.IsHardwareAccelerated)
            {
                pair.Item1 = MathF.FusedMultiplyAdd(pair.Item1, stdDev, mean);
                pair.Item2 = MathF.FusedMultiplyAdd(pair.Item2, stdDev, mean);
            }
            else
            {
                pair.Item1 = mean + (pair.Item1 * stdDev);
                pair.Item2 = mean + (pair.Item2 * stdDev);
            }

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

            if(Vector.IsHardwareAccelerated)
            {
                for(; i <= span.Length - 2; i += 2)
                {
                    (float a, float b) = Sample(rng);
                    span[i] = MathF.FusedMultiplyAdd(a, stdDev, mean);
                    span[i + 1] = MathF.FusedMultiplyAdd(b, stdDev, mean);
                }

                if(i < span.Length)
                    span[i] = MathF.FusedMultiplyAdd(Sample(rng).Item1, stdDev, mean);
            }
            else
            {
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

        #endregion
    }
}
