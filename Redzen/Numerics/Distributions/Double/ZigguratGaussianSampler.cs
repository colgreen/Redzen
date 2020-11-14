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
using System;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double
{
    /// <summary>
    /// A Gaussian distribution sampler based on the Ziggurat algorithm.
    /// </summary>
    public class ZigguratGaussianSampler : ISampler<double>
    {
        #region Instance Fields

        readonly double _mean;
        readonly double _stdDev;
        readonly IRandomSource _rng;

        #endregion

        #region Constructors

        /// <summary>
        ///Construct with the default distribution parameters, and a new random source.
        /// </summary>
        public ZigguratGaussianSampler()
            : this(0.0, 1.0, RandomDefaults.CreateRandomSource())
        {}

        /// <summary>
        /// Construct with the given distribution parameters, and a new random source.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        public ZigguratGaussianSampler(double mean, double stdDev)
            : this(mean, stdDev, RandomDefaults.CreateRandomSource())
        {}

        /// <summary>
        /// Construct with the given distribution parameters, and a new random source.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <param name="seed">Random source seed.</param>
        public ZigguratGaussianSampler(double mean, double stdDev, ulong seed)
            : this(mean, stdDev, RandomDefaults.CreateRandomSource(seed))
        {}

        /// <summary>
        /// Construct with the given distribution parameters, and a random source.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <param name="rng">Random source.</param>
        public ZigguratGaussianSampler(double mean, double stdDev, IRandomSource rng)
        {
            _mean = mean;
            _stdDev = stdDev;
            _rng = rng;
        }

        #endregion

        #region IDistributionSampler

        /// <summary>
        /// Take a sample from the distribution.
        /// </summary>
        /// <returns>A random sample.</returns>
        public double Sample()
        {
            return ZigguratGaussian.Sample(_rng, _mean, _stdDev);
        }

        /// <summary>
        /// Fill a span with samples from the distribution.
        /// </summary>
        /// <param name="span">The span to fill with samples.</param>
        public void Sample(Span<double> span)
        {
            ZigguratGaussian.Sample(_rng, span);
        }

        #endregion
    }
}
