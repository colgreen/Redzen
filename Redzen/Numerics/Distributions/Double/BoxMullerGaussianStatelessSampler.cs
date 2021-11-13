﻿/* ***************************************************************************
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

namespace Redzen.Numerics.Distributions.Double
{
    /// <summary>
    /// A stateless Gaussian distribution sampler based on the Box-Muller transform.
    /// </summary>
    public class BoxMullerGaussianStatelessSampler : IStatelessSampler<double>
    {
        #region Instance Fields

        readonly double _mean;
        readonly double _stdDev;
        double? _sample = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with the default distribution parameters.
        /// </summary>
        public BoxMullerGaussianStatelessSampler()
        {
            _mean = 0.0;
            _stdDev = 1.0;
        }

        /// <summary>
        /// Construct with the given distribution parameters.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        public BoxMullerGaussianStatelessSampler(double mean, double stdDev)
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
        public void Sample(ref double x, IRandomSource rng)
        {
            if(_sample.HasValue)
            {
                x = _sample.Value;
                _sample = null;
                return;
            }

            // Note. The Box-Muller transform generates samples in pairs.
            (double x1, double x2) = BoxMullerGaussian.Sample(rng, _mean, _stdDev);

            // Return the first sample and store the other for future use.
            x = x1;
            _sample = x2;
        }

        /// <summary>
        /// Take a sample from the distribution, using the provided <see cref="IRandomSource"/> as the source of entropy.
        /// </summary>
        /// <param name="rng">Random source.</param>
        /// <returns>A random sample.</returns>
        public double Sample(IRandomSource rng)
        {
            if(_sample.HasValue)
            {
                double x = _sample.Value;
                _sample = null;
                return x;
            }

            // Note. The Box-Muller transform generates samples in pairs.
            (double x1, double x2) = BoxMullerGaussian.Sample(rng, _mean, _stdDev);

            // Return the first sample and store the other for future use.
            _sample = x2;
            return x1;
        }

        /// <summary>
        /// Fill a span with samples from the distribution, using the provided <see cref="IRandomSource"/> as the source of entropy.
        /// </summary>
        /// <param name="span">The span to fill with samples.</param>
        /// <param name="rng">Random source.</param>
        public void Sample(Span<double> span, IRandomSource rng)
        {
            BoxMullerGaussian.Sample(rng, span);
        }

        #endregion
    }
}
