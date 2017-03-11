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

namespace Redzen.Numerics
{
    /// <summary>
    /// Source of random values sample from a Gaussian distribution. Uses the polar form of the Box-Muller method.
    /// http://en.wikipedia.org/wiki/Box_Muller_transform
    /// </summary>
    public class BoxMullerGaussianSampler : IContinuousDistributionSampler
    {
        XorShiftRandom _rng = new XorShiftRandom();
        double? _spareValue = null;

        /// <summary>
        /// Get the next sample point from the gaussian distribution.
        /// </summary>
        public double NextDouble()
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

        /// <summary>
        /// Get the next sample point from the gaussian distribution.
        /// </summary>
        public double NextDouble(double mu, double sigma)
        {
            return mu + (NextDouble() * sigma);
        }
    }
}
