/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2020 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float
{
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
        /// Take a sample from the distribution, using the provided <see cref="IRandomSource"/> as the source of entropy.
        /// </summary>
        /// <returns>A random sample.</returns>
        public float Sample(IRandomSource rng)
        {
            return ZigguratGaussian.Sample(rng, _mean, _stdDev);
        }

        /// <summary>
        /// Fill an array with samples from the distribution, using the provided <see cref="IRandomSource"/> as the source of entropy.
        /// </summary>
        /// <param name="buf">The array to fill with samples.</param>
        /// <param name="rng">Random source.</param>
        public void Sample(float[] buf, IRandomSource rng)
        {
            for(int i=0; i < buf.Length; i++) {
                buf[i] = ZigguratGaussian.Sample(rng, _mean, _stdDev);
            }
        }

        #endregion
    }
}
