/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2017 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */

using System;

namespace Redzen.Random.Double
{
    /// <summary>
    /// Source of random values sample from a Gaussian distribution. Uses the polar form of the Box-Muller method.
    /// http://en.wikipedia.org/wiki/Box_Muller_transform
    /// </summary>
    public class BoxMullerGaussianDistribution : IGaussianDistribution<double>
    {
        #region Instance Fields

        readonly IRandomSource _rng;
        readonly double _mean;
        readonly double _stdDev;
        readonly Func<double> _sampleFn;

        double? _spareValue = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Construct with a default RNG source.
        /// </summary>
        public BoxMullerGaussianDistribution() 
            : this(new XorShiftRandom(), 0.0, 1.0)
        { }

        /// <summary>
        /// Construct with the specified RNG seed.
        /// </summary>
        public BoxMullerGaussianDistribution(int seed)
            : this(new XorShiftRandom(seed), 0.0, 1.0)
        { }

        /// <summary>
        /// Construct with the provided RNG source.
        /// </summary>
        public BoxMullerGaussianDistribution(IRandomSource rng)
            : this(rng, 0.0, 1.0)
        {
            _rng = rng;
        }

        /// <summary>
        /// Construct with a default RNG source.
        /// </summary>
        public BoxMullerGaussianDistribution(double mean, double stdDev) 
            : this(new XorShiftRandom())
        { }

        /// <summary>
        /// Construct with the specified RNG seed.
        /// </summary>
        public BoxMullerGaussianDistribution(int seed, double mean, double stdDev)
            : this(new XorShiftRandom(seed))
        { }

        /// <summary>
        /// Construct with the provided RNG source.
        /// </summary>
        public BoxMullerGaussianDistribution(IRandomSource rng, double mean, double stdDev)
        {
            _rng = rng;
            _mean = mean;
            _stdDev = stdDev;

            // Note. We predetermine which of these four function variants to use at construction time,
            // thus avoiding the two condition tests on each invocation of Sample(). 
            // I.e. this is a micro-optimization.
            if(0.0 == mean)
            {
                if(1.0 == stdDev) {
                    _sampleFn = () => { return SampleStandard(); };
                }
                else {
                    _sampleFn = () => { return SampleStandard() * stdDev; };
                }
            }
            else
            {
                if(1.0 == stdDev) {
                    _sampleFn = () => { return _mean + SampleStandard(); };
                }
                else {
                    _sampleFn = () => { return _mean + (SampleStandard() * stdDev); };
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get a sample from the distribution.
        /// </summary>
        public double Sample()
        {
            return _sampleFn();
        }

        /// <summary>
        /// Get a sample value from the gaussian distribution.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <returns>A new random sample.</returns>
        public double Sample(double mean, double stdDev)
        {
            return mean + (SampleStandard() * stdDev);
        }

        /// <summary>
        /// Get a sample from the standard gaussian distribution, i.e. with mean - 0.0 and standard deviation of 1.0.
        /// </summary>
        public double SampleStandard()
        {
            if(null != _spareValue)
            {
                double tmp = _spareValue.Value;
                _spareValue = null;
                return tmp;
            }

            // Generate two new gaussian values.
            double x, y, sqr;

            // We need a non-zero random point inside the unit circle.
            do
            {
                x = 2.0 * _rng.NextDouble() - 1.0;
                y = 2.0 * _rng.NextDouble() - 1.0;
                sqr = x * x + y * y;
            }
            while(sqr > 1.0 || sqr == 0);

            // Make the Box-Muller transformation.
            double fac = Math.Sqrt(-2.0 * Math.Log(sqr) / sqr);

            _spareValue = x * fac;
            return y * fac;
        }

        #endregion
    }
}
