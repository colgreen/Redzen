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

namespace Redzen.Numerics.Distributions.Double
{
    /// <summary>
    /// A uniform distribution sampler.
    /// </summary>
    public class UniformDistributionSampler : ISampler<double>
    {
        #region Instance Fields

        readonly double _max = 1.0;
        readonly bool _signed = false;
        readonly Func<IRandomSource, double> _sampleFn;
        readonly IRandomSource _rng;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with the unit distribution and a new random source.
        /// </summary>
        public UniformDistributionSampler()
            : this(1.0, false, RandomDefaults.CreateRandomSource())
        {}

        /// <summary>
        /// Construct with the given distribution and a new random source.
        /// </summary>
        /// <param name="max">Maximum absolute value.</param>
        /// <param name="signed">Indicates if the distribution interval includes negative values.</param>
        public UniformDistributionSampler(double max, bool signed)
            : this(max, signed, RandomDefaults.CreateRandomSource())
        {}

        /// <summary>
        /// Construct with the given distribution and a new random source.
        /// </summary>
        /// <param name="max">Maximum absolute value.</param>
        /// <param name="signed">Indicates if the distribution interval includes negative values.</param>
        /// <param name="seed">Random source seed.</param>
        public UniformDistributionSampler(double max, bool signed, ulong seed)
            : this(max, signed, RandomDefaults.CreateRandomSource(seed))
        {}

        /// <summary>
        /// Construct with the given distribution and a random source.
        /// </summary>
        /// <param name="max">Maximum absolute value.</param>
        /// <param name="signed">Indicates if the distribution interval includes negative values.</param>
        /// <param name="rng">Random source.</param>
        public UniformDistributionSampler(double max, bool signed, IRandomSource rng)
        {
            _max = max;
            _signed = signed;
            _rng = rng;

            // Note. We predetermine which of these two function variants to use at construction time,
            // thus avoiding a branch on each invocation of Sample() (i.e. this is a micro-optimization).
            if(signed) {
                _sampleFn = (r) => UniformDistribution.SampleSigned(r, _max);
            }
            else {
                _sampleFn = (r) => UniformDistribution.Sample(r, _max);
            }
        }

        #endregion

        #region ISampler

        /// <summary>
        /// Returns a random sample from the uniform distribution.
        /// </summary>
        /// <returns>A new random sample.</returns>
        public double Sample()
        {
            return _sampleFn(_rng);
        }

        /// <summary>
        /// Fills the provided span with random samples from the uniform distribution.
        /// </summary>
        /// <param name="span">The span to fill with samples.</param>
        public void Sample(Span<double> span)
        {
            if(_signed) {
                UniformDistribution.SampleSigned(_rng, _max, span);
            }
            else {
                UniformDistribution.Sample(_rng, _max, span);
            }
        }

        #endregion
    }
}
