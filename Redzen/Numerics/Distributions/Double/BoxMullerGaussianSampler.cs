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
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Double
{
    /// <summary>
    /// A Gaussian distribution sampler based on the Box-Muller transform.
    /// </summary>
    public class BoxMullerGaussianSampler : ISampler<double>
    {
        #region Instance Fields

        readonly double _mean;
        readonly double _stdDev;
        readonly IRandomSource _rng;
        double? _sample = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with the default distribution parameters, and a new random source.
        /// </summary>
        public BoxMullerGaussianSampler()
            : this(0.0, 1.0, RandomDefaults.CreateRandomSource())
        {}

        /// <summary>
        /// Construct with the given distribution parameters, and a new random source.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        public BoxMullerGaussianSampler(double mean, double stdDev)
            : this(mean, stdDev, RandomDefaults.CreateRandomSource())
        {}

        /// <summary>
        /// Construct with the given distribution parameters, and a new random source.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <param name="seed">Random source seed.</param>
        public BoxMullerGaussianSampler(double mean, double stdDev, ulong seed)
            : this(mean, stdDev, RandomDefaults.CreateRandomSource(seed))
        {}

        /// <summary>
        /// Construct with the given distribution parameters, and a random source.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <param name="rng">Random source.</param>
        public BoxMullerGaussianSampler(double mean, double stdDev, IRandomSource rng)
        {
            _mean = mean;
            _stdDev = stdDev;
            _rng = rng;
        }

        #endregion

        #region ISampler

        /// <summary>
        /// Take a sample from the distribution.
        /// </summary>
        /// <returns>A random sample.</returns>
        public double Sample()
        {
            if(null != _sample)
            {
                double x = _sample.Value;
                _sample = null;
                return x;
            }

            // Note. The Box-Muller transform generates samples in pairs.
            (double x1, double x2) = BoxMullerGaussian.Sample(_rng, _mean, _stdDev);

            // Return the first sample and store the other for future use.
            _sample = x2;
            return x1;
        }

        /// <summary>
        /// Fill an array with samples from the distribution.
        /// </summary>
        /// <param name="buf">The array to fill with samples.</param>
        public void Sample(double[] buf)
        {
            BoxMullerGaussian.Sample(_rng, buf);
        }

        #endregion
    }
}
