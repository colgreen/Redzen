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
using System.Collections.Generic;
using System.Diagnostics;
using Redzen.Sorting;

namespace Redzen.Numerics
{
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
        public static double ProbabilisticRound(double val, XorShiftRandom rng)
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
            Debug.Assert(valueList.Count != 0 && SortUtils.IsSorted(valueList), "CalculateMedian() requires non-zero length sorted list of values.");

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
        /// Calculate a frequency distribution for the provided array of values.
        /// 1) The minimum and maximum values are found.
        /// 2) The resulting value range is divided into equal sized sub-ranges (categoryCount).
        /// 3) The number of values that fall into each category is determined.
        /// </summary>
        public static HistogramData BuildHistogramData(double[] valArr, int categoryCount)
        {
            // Determine min/max.
            double min = valArr[0];
            double max = min;

            for(int i=1; i < valArr.Length; i++)
            {
                double val = valArr[i];
                if(val < min) {
                    min = val;
                }
                else if(val > max) {
                    max = val;
                }
            }

            double range = max - min;

            // Handle special case where the data series contains a single value.
            if(0.0 == range) {
                return new HistogramData(min, max, 0.0, new int[] { valArr.Length });
            }

            // Loop values and for each one increment the relevant category's frequency count.
            double incr = range / (categoryCount - 1);
            int[] frequencyArr = new int[categoryCount];
            for(int i=0; i < valArr.Length; i++) {
                frequencyArr[(int)((valArr[i] - min) / incr)]++;
            }
            return new HistogramData(min, max, incr, frequencyArr);
        }
    }
}
