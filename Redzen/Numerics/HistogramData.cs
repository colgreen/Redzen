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

namespace Redzen.Numerics
{
    // TODO: Check if there is an equivalent class in MathNet.Numerics.
    /// <summary>
    /// Histogram data. Frequency counts arranged into bins..
    /// </summary>
    public sealed class HistogramData
    {
        #region Auto Properties

        /// <summary>
        /// The minimum value in the data series the distribution represents.
        /// </summary>
        public double Min { get; }

        /// <summary>
        /// The maximum value in the data series the distribution represents.
        /// </summary>
        public double Max { get; }

        /// <summary>
        /// The range of a single category bucket.
        /// </summary>
        public double Increment { get; }

        /// <summary>
        /// The array of category frequency counts.
        /// </summary>
        public int[] FrequencyArray { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Construct with the provided frequency distribution data.
        /// </summary>
        /// <param name="min">The minimum value in the data series the distribution represents.</param>
        /// <param name="max">The maximum value in the data series the distribution represents.</param>
        /// <param name="increment">The range of a single category bucket.</param>
        /// <param name="frequencyArr">The array of category frequency counts.</param>
        public HistogramData(double min, double max, double increment, int[] frequencyArr)
        {
            this.Min = min;
            this.Max = max;
            this.Increment = increment;
            this.FrequencyArray = frequencyArr;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the index of the bucket that covers the specified x value. Throws an exception if x is 
        /// outside the range of represented by the distribution buckets.
        /// </summary>
        public int GetBucketIndex(double x)
        {
            if(x < Min || x > Max) {
                throw new ApplicationException("x is outside the range represented by the distribution data.");
            }
            return (int)((x - Min) / Increment);
        }

        #endregion
    }
}
