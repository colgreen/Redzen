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
using System;
using Redzen.Random;

namespace Redzen.Numerics.Distributions.Float
{
    /// <summary>
    /// A stateless uniform distribution sampler.
    /// </summary>
    public class UniformDistributionStatelessSampler : IStatelessSampler<float>
    {
        #region Instance Fields

        readonly float _max = 1.0f;
        readonly bool _signed = false;
        readonly Func<IRandomSource, float> _sampleFn;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with the given distribution and a new random source.
        /// </summary>
        /// <param name="max">Uniform distribution max value.</param>
        /// <param name="signed">If true then the distribution interval is (-max, max), otherwise it is [0, max).</param>
        public UniformDistributionStatelessSampler(float max, bool signed)
        {
            _max = max;
            _signed = signed;

            // Note. We predetermine which of these two function variants to use at construction time,
            // thus avoiding a branch on each invocation of Sample() (i.e. this is a micro-optimization).
            if(signed) {
                _sampleFn = (rng) => UniformDistribution.SampleSigned(rng, _max);
            }
            else {
                _sampleFn = (rng) => UniformDistribution.Sample(rng, _max);
            }
        }

        #endregion

        #region IStatelessSampler

        /// <summary>
        /// Take a sample from the distribution, using the provided <see cref="IRandomSource"/> as the source of entropy.
        /// </summary>
        /// <returns>A random sample.</returns>
        public float Sample(IRandomSource rng)
        {
            return _sampleFn(rng);
        }

        /// <summary>
        /// Fill an array with samples from the distribution, using the provided <see cref="IRandomSource"/> as the source of entropy.
        /// </summary>
        /// <returns>A random sample.</returns>
        public void Sample(IRandomSource rng, float[] buf)
        {
            if(_signed) {
                UniformDistribution.SampleSigned(rng, _max, buf);
            }
            else {
                UniformDistribution.Sample(rng, _max, buf);
            }
        }

        #endregion
    }
}
