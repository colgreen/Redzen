/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2020 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Redzen.Random;
using Redzen.Sorting;

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
        /// 
        /// This is useful if we wish to round values and then sum them without generating a rounding bias.
        /// For monetary rounding this problem is solved with rounding to e.g. the nearest even number which
        /// then causes a bias towards even numbers.
        /// 
        /// This solution is more appropriate for certain types of scientific values.
        /// </summary>
        public static double ProbabilisticRound(double val, IRandomSource rng)
        {
            double integerPart = Math.Floor(val);
            double fractionalPart = val - integerPart;
            return rng.NextDouble() < fractionalPart ? integerPart + 1.0 : integerPart;
        }

        /// <summary>
        /// Calculates the median value in a list of sorted values.
        /// </summary>
        public static double CalculateMedian(IList<double> valueList)
        {
            Debug.Assert(valueList.Count != 0 && SortUtils.IsSortedAscending(valueList), "CalculateMedian() requires non-zero length sorted list of values.");

            if(1 == valueList.Count) {
                return valueList[0];
            }

            if(valueList.Count % 2 == 0)
            {   // Even number of values. The values are already sorted so we simply take the
                // mean of the two central values.
                int idx = valueList.Count / 2;
                return (valueList[idx - 1] + valueList[idx]) / 2.0;
            }

            // Odd number of values. Return the middle value.
            // (Note. integer division truncates fractional part of result).
            return valueList[valueList.Count / 2];
        }

        /// <summary>
        /// Calculate a histogram for the provided span of values.
        /// 1) The minimum and maximum values are found.
        /// 2) The resulting value range is divided into equal sized sub-ranges or bins.
        /// 3) The number of values that fall into each bin is determined.
        /// </summary>
        /// <param name="vals">The values to calculate a histogram for.</param>
        /// <param name="binCount">The number of histogram bins to use.</param>
        public static HistogramData BuildHistogramData(Span<double> vals, int binCount)
        {            
            // Determine min/max.
            MathSpanUtils.MinMax(vals, out double min, out double max);

            // Note. each bin's range has interval [low,high), i.e. samples exactly equal to 'high' will fall
            // into the next highest bin. Except for the last bin which has interval [low, high].
            double range = max - min;

            // Handle special case where the data series contains a single value.
            if(range == 0.0) {
                return new HistogramData(min, max, 0.0, new int[] { vals.Length });
            }

            // Loop values, and for each one increment the relevant category's frequency count.
            double incr = range / binCount;
            int[] frequencyArr = new int[binCount];

            for(int i=0; i < vals.Length; i++) 
            {
                // Determine which bin the value falls within.
                int idx = (int)((vals[i] - min) / incr);

                // Values that equal max, are placed into the last bin.
                if(idx == vals.Length) idx--;

                frequencyArr[idx]++;
            }

            return new HistogramData(min, max, incr, frequencyArr);
        }
    }
}
