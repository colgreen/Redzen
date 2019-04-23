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

namespace Redzen.Numerics.Distributions.Float
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
            if(rng.NextBool()) {
                sample *= -1.0f;
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
        public static float Sample(IRandomSource rng, float min, float max)
        {
            Debug.Assert(max >= min);
            return min + (rng.NextFloat() * (max - min));
        }

        /// <summary>
        /// Fill an array with samples from the uniform distribution with interval [0, 1).
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <param name="buf">The array to fill with samples.</param>
        public static void Sample(IRandomSource rng, float[] buf)
        {
            for(int i=0; i < buf.Length; i++) {
                buf[i] = rng.NextFloat();
            }
        }

        /// <summary>
        /// Fill an array with samples from the uniform distribution with interval [0, max).
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <param name="max">Maximum value (exclusive).</param>
        /// <param name="buf">The array to fill with samples.</param>
        public static void Sample(IRandomSource rng, float max, float[] buf)
        {
            Debug.Assert(max >= 0.0);

            for(int i=0; i < buf.Length; i++) {
                buf[i] = rng.NextFloat() * max;
            }
        }

        /// <summary>
        /// Fill an array with samples from the uniform distribution with interval (-max, max).
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <param name="max">Maximum absolute value (exclusive).</param>
        /// <param name="buf">The array to fill with samples.</param>
        public static void SampleSigned(IRandomSource rng, float max, float[] buf)
        {
            Debug.Assert(max >= 0.0);

            for(int i=0; i < buf.Length; i++) 
            {
                float sample = rng.NextFloat() * max;
                if(rng.NextBool()) {
                    sample *= -1.0f;
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
        public static void Sample(IRandomSource rng, float min, float max, float[] buf)
        {
            Debug.Assert(max >= min);

            float delta = max - min;

            for(int i=0; i < buf.Length; i++) {
                buf[i] = min + (rng.NextFloat() * delta);
            }
        }

        #endregion
    }
}
