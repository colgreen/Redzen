/* ***************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015-2022 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using Redzen.Random;

namespace Redzen.Numerics
{
    /// <summary>
    /// General purpose numeric helper methods.
    /// </summary>
    public static class NumericsUtils
    {
        /// <summary>
        /// Rounds up or down to a whole number by using the fractional part of the input value
        /// as the probability that the value will be rounded up.
        /// </summary>
        /// <remarks>
        /// This is useful if we wish to round values and then sum them without generating a rounding bias.
        /// For monetary rounding this problem is solved with rounding to e.g. the nearest even number,
        /// which then causes a bias towards even numbers. As such, this solution is more appropriate for
        /// certain types of scientific calculations.
        /// </remarks>
        /// <param name="val">The value to round.</param>
        /// <param name="rng">Random source.</param>
        /// <returns>The rounded value.</returns>
        public static double StochasticRound(double val, IRandomSource rng)
        {
            double integerPart = Math.Floor(val);
            double fractionalPart = val - integerPart;
            return rng.NextDouble() < fractionalPart ? integerPart + 1.0 : integerPart;
        }

        /// <summary>
        /// Calculate a histogram for the provided span of values.
        /// 1) The minimum and maximum values are found.
        /// 2) The resulting value range is divided into equal sized sub-ranges or bins.
        /// 3) The number of values that fall into each bin is determined.
        /// </summary>
        /// <param name="vals">The values to calculate a histogram for.</param>
        /// <param name="binCount">The number of histogram bins to use.</param>
        /// <returns>A new instance of <see cref="HistogramData"/>.</returns>
        public static HistogramData BuildHistogramData(Span<double> vals, int binCount)
        {
            // Determine min/max.
            MathSpan.MinMax(vals, out double min, out double max);

            // Note. each bin's range has interval [low,high), i.e. samples exactly equal to 'high' will fall
            // into the next highest bin. Except for the last bin which has interval [low, high].
            double range = max - min;

            // Handle special case where the data series contains a single value.
            if(range == 0.0)
                return new HistogramData(min, max, 0.0, new int[] { vals.Length });

            // Loop values, and for each one increment the relevant category's frequency count.
            double incr = range / binCount;
            int[] frequencyArr = new int[binCount];

            for(int i=0; i < vals.Length; i++)
            {
                // Determine which bin the value falls within.
                int binIdx = (int)((vals[i] - min) / incr);

                // Values that equal max, are placed into the last bin.
                if(binIdx == binCount) binIdx--;

                frequencyArr[binIdx]++;
            }

            return new HistogramData(min, max, incr, frequencyArr);
        }
    }
}
