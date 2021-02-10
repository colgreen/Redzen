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
using System.Diagnostics;
using System.Numerics;
using Redzen.Sorting;

namespace Redzen
{
    /// <summary>
    /// Math utility methods for working with spans.
    /// </summary>
    public static partial class MathSpan
    {
        /// <summary>
        /// Calculate the sum of the span elements.
        /// </summary>
        /// <param name="s">The span.</param>
        /// <returns>The sum of the elements.</returns>
        public static double Sum(ReadOnlySpan<double> s)
        {
            int width = Vector<double>.Count;
            double sum=0;

            // Run the vectorised code only if hardware acceleration is available, and there are enough array
            // elements to justify its use.
            if(Vector.IsHardwareAccelerated && (s.Length >= width << 1))
            {
                var sumVec = new Vector<double>(s);
                s = s.Slice(width);

                // Loop over vector sized slices.
                while(s.Length >= width)
                {
                    var vec = new Vector<double>(s);
                    sumVec += vec;
                    s = s.Slice(width);
                }

                for(int i=0; i < width; i++) {
                    sum += sumVec[i];
                }
            }

            // Sum remaining elements not summed by the vectorized code path.
            for(int i=0; i < s.Length; i++) {
                sum += s[i];
            }

            return sum;
        }

        /// <summary>
        /// Calculate the arithmetic mean of the span elements.
        /// </summary>
        /// <param name="s">The span.</param>
        /// <returns>The sum of the elements.</returns>
        public static double Mean(ReadOnlySpan<double> s)
        {
            return s.Length != 0 ? Sum(s) / s.Length : 0.0;
        }

        /// <summary>
        /// Returns the median value in a span of sorted values.
        /// </summary>
        /// <param name="vals">The values, sorted in ascending order.</param>
        /// <returns>The median of the provided values.</returns>
        public static double MedianOfSorted(ReadOnlySpan<double> vals)
        {
            if(vals.Length == 0) throw new ArgumentException("Empty span. Span must have one or elements.", nameof(vals));

            Debug.Assert(SortUtils.IsSortedAscending(vals), "Span elements are not sorted.");

            if(vals.Length == 1) {
                return vals[0];
            }

            if(vals.Length % 2 == 0)
            {
                // There are an even number of values. The values are already sorted so we
                // simply take the mean of the two central values.
                int idx = vals.Length >> 1;
                return (vals[idx - 1] + vals[idx]) * 0.5;
            }

            // Odd number of values. Return the middle value.
            // Note. bit shift right by one bit results in integer division by two with the fraction part truncated, e.g. 3/2 = 1.
            return vals[vals.Length >> 1];
        }

        /// <summary>
        /// Clip (limit) the values in a span to be within some defined interval.
        /// For example, if an interval of [0, 1] is specified, values smaller than 0 become 0, and values larger than 1 become 1.
        /// </summary>
        /// <param name="s">Span containing the elements to clip.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static void Clip(Span<double> s, double min, double max)
        {
            // Run the vectorised code only if the hardware acceleration is available, and there are
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (s.Length >= Vector<double>.Count))
            {
                int width = Vector<double>.Count;
                var minVec = new Vector<double>(min);
                var maxVec = new Vector<double>(max);

                // Loop over vector sized slices.
                while(s.Length >= width)
                {
                    var vec = new Vector<double>(s);
                    vec = Vector.Max(minVec, vec);
                    vec = Vector.Min(maxVec, vec);
                    vec.CopyTo(s);
                    s = s.Slice(width);
                }
            }

            // Note. If the above vector logic block was executed then this handles remaining elements,
            // otherwise it handles all elements.
            for(int i=0; i < s.Length; i++)
            {
                if(s[i] < min)
                    s[i] = min;
                else if(s[i] > max)
                    s[i] = max;
            }
        }

        /// <summary>
        /// Calculate the minimum and maximum values in the provided array.
        /// </summary>
        /// <param name="s">The span.</param>
        /// <param name="min">Returns the minimum value in the array.</param>
        /// <param name="max">Returns the maximum value in the array.</param>
        public static void MinMax(ReadOnlySpan<double> s, out double min, out double max)
        {
            // Run the vectorised code only if the hardware acceleration is available, and there are
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (s.Length >= Vector<double>.Count << 1))
            {
                int width = Vector<double>.Count;
                var minVec = new Vector<double>(s);
                var maxVec = new Vector<double>(s);
                s = s.Slice(width);

                // Loop over vector sized slices.
                do
                {
                    var vec = new Vector<double>(s);
                    minVec = Vector.Min(minVec, vec);
                    maxVec = Vector.Max(maxVec, vec);
                    s = s.Slice(width);
                }
                while(s.Length >= width);

                // Calc min(minVec) and max(maxVec).
                min = minVec[0];
                max = maxVec[0];
                for(int j = 1; j < width; j++)
                {
                    if(minVec[j] < min) min = minVec[j];
                    if(maxVec[j] > max) max = maxVec[j];
                }
            }
            else
            {
                min = max = s[0];
            }

            // Calc min/max.
            // Note. If the above vector logic block was executed then this handles remaining elements,
            // otherwise it handles all elements.
            for(int i=0; i < s.Length; i++)
            {
                double val = s[i];
                if(val < min) {
                    min = val;
                }
                else if(val > max) {
                    max = val;
                }
            }
        }

        /// <summary>
        /// Calculate the mean squared difference of the elements in arrays {a} and {b}.
        /// </summary>
        /// <param name="a">Span {a}.</param>
        /// <param name="b">Span {b}.</param>
        /// <returns>A double.</returns>
        /// <remarks>Arrays {a} and {b} must be the same length.</remarks>
        public static double MeanSquaredDelta(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            return SumSquaredDelta(a, b) / a.Length;
        }

        /// <summary>
        /// Calculate the sum of the squared difference of each elements in arrays {a} and {b}.
        /// </summary>
        /// <param name="a">Array {a}.</param>
        /// <param name="b">Array {b}.</param>
        /// <returns>A double.</returns>
        /// <remarks>Arrays {a} and {b} must be the same length.</remarks>
        public static double SumSquaredDelta(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            if(a.Length != b.Length) throw new ArgumentException("Array lengths are not equal.");

            double total = 0.0;
            int idx=0;

            // Run the vectorised code only if the hardware acceleration is available, and there are
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (a.Length >= Vector<double>.Count))
            {
                int width = Vector<double>.Count;
                var sumVec = new Vector<double>(0.0);

                // Loop over vector sized slices, calc the squared error for each, and accumulate in sumVec.
                for(; idx <= a.Length - width; idx += width)
                {
                    var av = new Vector<double>(a.Slice(idx, width));
                    var bv = new Vector<double>(b.Slice(idx, width));

                    var cv = av - bv;
                    sumVec += cv * cv;
                }

                // Sum the elements of sumVec.
                for(int j=0; j < width; j++) {
                    total += sumVec[j];
                }
            }

            // Calc sum(squared error).
            // Note. If the above vector logic block was executed then this handles remaining elements,
            // otherwise it handles all elements.
            for(; idx < a.Length; idx++)
            {
                double err = a[idx] - b[idx];
                total += err * err;
            }

            return total;
        }
    }
}
