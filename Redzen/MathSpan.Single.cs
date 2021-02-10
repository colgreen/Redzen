﻿/* ***************************************************************************
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
        public static float Sum(ReadOnlySpan<float> s)
        {
            int width = Vector<float>.Count;
            float sum=0;

            // Run the vectorised code only if hardware acceleration is available, and there are enough array
            // elements to justify its use.
            if(Vector.IsHardwareAccelerated && (s.Length >= width << 1))
            {
                var sumVec = new Vector<float>(s);
                s = s.Slice(width);

                while(s.Length >= width)
                {
                    var vec = new Vector<float>(s);
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
        public static float Mean(ReadOnlySpan<float> s)
        {
            return s.Length != 0 ? Sum(s) / s.Length : 0f;
        }

        /// <summary>
        /// Returns the median value in a span of sorted values.
        /// </summary>
        /// <param name="vals">The values, sorted in ascending order.</param>
        /// <returns>The median of the provided values.</returns>
        public static float Median(ReadOnlySpan<float> vals)
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
                return (vals[idx - 1] + vals[idx]) * 0.5f;
            }

            // Odd number of values. Return the middle value.
            // Note. bit shift right by one bit results in integer division by two with the fraction part truncated, e.g. 3/2 = 1.
            return vals[vals.Length >> 1];
        }

        /// <summary>
        /// Clamp (limit) the values in a span to be within some defined range.
        /// For example, if an interval of [0, 1] is specified, values smaller than 0 become 0, and values larger than 1 become 1.
        /// </summary>
        /// <param name="s">Span containing the elements to clip.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static void Clamp(Span<float> s, float min, float max)
        {
            int idx=0;

            // Run the vectorised code only if the hardware acceleration is available, and there are
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (s.Length >= Vector<float>.Count))
            {
                int width = Vector<float>.Count;
                var minVec = new Vector<float>(min);
                var maxVec = new Vector<float>(max);

                // Loop over vector sized segments.
                for(; idx <= s.Length - width; idx += width)
                {
                    Span<float> slice = s.Slice(idx, width);
                    var xv = new Vector<float>(slice);
                    xv = Vector.Max(minVec, xv);
                    xv = Vector.Min(maxVec, xv);
                    xv.CopyTo(slice);
                }
            }

            // Note. If the above vector logic block was executed then this handles remaining elements,
            // otherwise it handles all elements.
            for(; idx < s.Length; idx++)
            {
                if(s[idx] < min)
                    s[idx] = min;
                else if(s[idx] > max)
                    s[idx] = max;
            }
        }

        /// <summary>
        /// Calculate the minimum and maximum values in the provided array.
        /// </summary>
        /// <param name="s">The span.</param>
        /// <param name="min">Returns the minimum value in the array.</param>
        /// <param name="max">Returns the maximum value in the array.</param>
        public static void MinMax(ReadOnlySpan<float> s, out float min, out float max)
        {
            // Run the vectorised code only if the hardware acceleration is available, and there are
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (s.Length >= Vector<float>.Count))
            {
                int width = Vector<float>.Count;
                var minVec = new Vector<float>(s);
                var maxVec = new Vector<float>(s);
                s = s.Slice(width);

                while(s.Length >= width)
                {
                    var vec = new Vector<float>(s);
                    minVec = Vector.Min(minVec, vec);
                    maxVec = Vector.Max(maxVec, vec);
                    s = s.Slice(width);
                }

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
                float val = s[i];
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
        /// <returns>A float.</returns>
        /// <remarks>Arrays {a} and {b} must be the same length.</remarks>
        public static float MeanSquaredDelta(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
        {
            return SumSquaredDelta(a, b) / a.Length;
        }

        /// <summary>
        /// Calculate the sum of the squared difference of each elements in arrays {a} and {b}.
        /// </summary>
        /// <param name="a">Array {a}.</param>
        /// <param name="b">Array {b}.</param>
        /// <returns>A float.</returns>
        /// <remarks>Arrays {a} and {b} must be the same length.</remarks>
        public static float SumSquaredDelta(ReadOnlySpan<float> a, ReadOnlySpan<float> b)
        {
            if(a.Length != b.Length) throw new ArgumentException("Array lengths are not equal.");

            float total = 0f;
            int idx=0;

            // Run the vectorised code only if the hardware acceleration is available, and there are
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (a.Length >= Vector<float>.Count))
            {
                int width = Vector<float>.Count;
                var sumVec = new Vector<float>(0f);

                // Loop over vector sized segments, calc the squared error for each, and accumulate in sumVec.
                for(; idx <= a.Length - width; idx += width)
                {
                    var av = new Vector<float>(a.Slice(idx, width));
                    var bv = new Vector<float>(b.Slice(idx, width));

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
                float err = a[idx] - b[idx];
                total += err * err;
            }

            return total;
        }
    }
}
