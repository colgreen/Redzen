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
    /// A Gaussian distribution sampler based on the Box-Muller transform.
    /// </summary>
    public class BoxMullerGaussianSampler : ISampler<float>
    {
        #region Instance Fields

        readonly float _mean;
        readonly float _stdDev;
        readonly IRandomSource _rng;
        float? _sample = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with the default distribution parameters, and a new random source.
        /// </summary>
        public BoxMullerGaussianSampler()
            : this(0f, 1f, RandomDefaults.CreateRandomSource())
        {}

        /// <summary>
        /// Construct with the given distribution parameters, and a new random source.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        public BoxMullerGaussianSampler(float mean, float stdDev)
            : this(mean, stdDev, RandomDefaults.CreateRandomSource())
        {}

        /// <summary>
        /// Construct with the given distribution parameters, and a new random source.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <param name="seed">Random source seed.</param>
        public BoxMullerGaussianSampler(float mean, float stdDev, ulong seed)
            : this(mean, stdDev, RandomDefaults.CreateRandomSource(seed))
        {}

        /// <summary>
        /// Construct with the given distribution parameters, and a random source.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <param name="rng">Random source.</param>
        public BoxMullerGaussianSampler(float mean, float stdDev, IRandomSource rng)
        {
            _mean = mean;
            _stdDev = stdDev;
            _rng = rng;
        }

        #endregion

        #region ISampler

        /// <summary>
        /// Gets a random sample from the distribution.
        /// </summary>
        /// <param name="x">Reference to a variable to store the new sample value in.</param>
        public void Sample(ref float x)
        {
            if (_sample.HasValue)
            {
                x = _sample.Value;
                _sample = null;
                return;
            }

            // Note. The Box-Muller transform generates samples in pairs.
            (float x1, float x2) = BoxMullerGaussian.Sample(_rng, _mean, _stdDev);

            // Return the first sample and store the other for future use.
            x = x1;
            _sample = x2;
            return;
        }

        /// <summary>
        /// Take a sample from the distribution.
        /// </summary>
        /// <returns>A random sample.</returns>
        public float Sample()
        {
            if(_sample.HasValue)
            {
                float x = _sample.Value;
                _sample = null;
                return x;
            }

            // Note. The Box-Muller transform generates samples in pairs.
            (float x1, float x2) = BoxMullerGaussian.Sample(_rng, _mean, _stdDev);

            // Return the first sample and store the other for future use.
            _sample = x2;
            return x1;
        }

        /// <summary>
        /// Fill a span with samples from the distribution.
        /// </summary>
        /// <param name="span">The span to fill with samples.</param>
        public void Sample(Span<float> span)
        {
            BoxMullerGaussian.Sample(_rng, _mean, _stdDev, span);
        }

        #endregion
    }
}
