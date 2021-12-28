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
        /// Gets a random sample from the distribution.
        /// </summary>
        /// <param name="x">Reference to a variable to store the new sample value in.</param>
        /// <param name="rng">Random source.</param>
        public void Sample(ref float x, IRandomSource rng)
        {
            ZigguratGaussian.Sample(rng, _mean, _stdDev, ref x);
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
}
