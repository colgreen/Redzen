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
            : this(0.0, 1.0, RandomSourceFactory.Create())
        {}

        /// <summary>
        /// Construct with the specified ulong random seed.
        /// </summary>
        public BoxMullerGaussianDistribution(ulong seed)
            : this(0.0, 1.0, RandomSourceFactory.Create(seed))
        {}

        /// <summary>
        /// Construct with the provided random source.
        /// </summary>
        public BoxMullerGaussianDistribution(IRandomSource rng)
            : this(0.0, 1.0, rng)
        {
            _rng = rng;
        }

        /// <summary>
        /// Construct with a default random source.
        /// </summary>
        public BoxMullerGaussianDistribution(double mean, double stdDev) 
            : this(mean, stdDev, RandomSourceFactory.Create())
        {}

        /// <summary>
        /// Construct with the specified ulong random seed.
        /// </summary>
        public BoxMullerGaussianDistribution(double mean, double stdDev, ulong seed)
            : this(mean, stdDev, RandomSourceFactory.Create(seed))
        {}

        /// <summary>
        /// Construct with the provided RNG source.
        /// </summary>
        public BoxMullerGaussianDistribution(double mean, double stdDev, IRandomSource rng)
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
        /// Take a sample from the distribution.
        /// </summary>
        public double Sample()
        {
            return _sampleFn();
        }

        /// <summary>
        /// Take a sample from the distribution.
        /// </summary>
        /// <param name="mean">Distribution mean.</param>
        /// <param name="stdDev">Distribution standard deviation.</param>
        /// <returns>A new random sample.</returns>
        public double Sample(double mean, double stdDev)
        {
            return mean + (SampleStandard() * stdDev);
        }

        /// <summary>
        /// Take a sample from the standard gaussian distribution, i.e. with mean of 0 and standard deviation of 1.
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
