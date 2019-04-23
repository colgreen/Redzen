/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2019 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using System.Diagnostics;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double
{
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
            if(rng.NextBool()) {
                sample *= -1.0f;
            }
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
            if(rng.NextBool()) {
                sample *= -1.0;
            }
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
            return min + (rng.NextDouble() * (max - min));
        }

        /// <summary>
        /// Fill an array with samples from the uniform distribution with interval [0, 1).
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <param name="buf">The array to fill with samples.</param>
        public static void Sample(IRandomSource rng, double[] buf)
        {
            for(int i=0; i < buf.Length; i++) {
                buf[i] = rng.NextDouble();
            }
        }

        /// <summary>
        /// Fill an array with samples from the uniform distribution with interval [0, max).
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <param name="max">Maximum value (exclusive).</param>
        /// <param name="buf">The array to fill with samples.</param>
        public static void Sample(IRandomSource rng, double max, double[] buf)
        {
            Debug.Assert(max >= 0.0);

            for(int i=0; i < buf.Length; i++) {
                buf[i] = rng.NextDouble() * max;
            }
        }

        /// <summary>
        /// Fill an array with samples from the uniform distribution with interval (-max, max).
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <param name="max">Maximum absolute value (exclusive).</param>
        /// <param name="buf">The array to fill with samples.</param>
        public static void SampleSigned(IRandomSource rng, double max, double[] buf)
        {
            Debug.Assert(max >= 0.0);

            for(int i=0; i < buf.Length; i++) 
            {
                double sample = rng.NextDouble() * max;
                if(rng.NextBool()) {
                    sample *= -1.0;
                }
                buf[i] = sample;
            }
        }

        /// <summary>
        /// Fill an array with samples from the uniform distribution with interval [min, max).
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <param name="min">Minimum value (inclusive).</param>
        /// <param name="max">Maximum value (exclusive).</param>
        /// <param name="buf">The array to fill with samples.</param>
        public static void Sample(IRandomSource rng, double min, double max, double[] buf)
        {
            Debug.Assert(max >= min);

            double delta = max - min;

            for(int i=0; i < buf.Length; i++) {
                buf[i] = min + (rng.NextDouble() * delta);
            }
        }

        #endregion
    }
}
