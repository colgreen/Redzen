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

                // Loop over vector sized slices.
                do
                {
                    var vec = new Vector<float>(s);
                    sumVec += vec;
                    s = s.Slice(width);
                }
                while(s.Length >= width);

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
        public static float MedianOfSorted(ReadOnlySpan<float> vals)
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
        /// Clamp (limit) the values in a span to be within some defined interval.
        /// For example, if an interval of [0, 1] is specified, values smaller than 0 become 0, and values larger than 1 become 1.
        /// </summary>
        /// <param name="s">Span containing the elements to clip.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static void Clip(Span<float> s, float min, float max)
        {
            // Run the vectorised code only if the hardware acceleration is available, and there are
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (s.Length >= Vector<float>.Count))
            {
                int width = Vector<float>.Count;
                var minVec = new Vector<float>(min);
                var maxVec = new Vector<float>(max);

                // Loop over vector sized slices.
                do
                {
                    var vec = new Vector<float>(s);
                    vec = Vector.Max(minVec, vec);
                    vec = Vector.Min(maxVec, vec);
                    vec.CopyTo(s);
                    s = s.Slice(width);
                }
                while(s.Length >= width);
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
        /// Determine the minimum value in the provided array.
        /// </summary>
        /// <param name="s">The span.</param>
        /// <returns>The minimum value in the span.</returns>
        public static float Min(ReadOnlySpan<float> s)
        {
            float min;

            // Run the vectorised code only if the hardware acceleration is available, and there are
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (s.Length >= Vector<float>.Count << 1))
            {
                int width = Vector<float>.Count;
                var minVec = new Vector<float>(s);
                s = s.Slice(width);

                // Loop over vector sized slices.
                do
                {
                    var vec = new Vector<float>(s);
                    minVec = Vector.Min(minVec, vec);
                    s = s.Slice(width);
                }
                while(s.Length >= width);

                // Calc min(minVec) and max(maxVec).
                min = minVec[0];
                for(int i=1; i < width; i++)
                {
                    if(minVec[i] < min) min = minVec[i];
                }
            }
            else
            {
                min = s[0];
            }

            // Calc min/max.
            // Note. If the above vector logic block was executed then this handles remaining elements,
            // otherwise it handles all elements.
            for(int i=0; i < s.Length; i++)
            {
                if(s[i] < min) {
                    min = s[i];
                }
            }

            return min;
        }

        /// <summary>
        /// Determine the maximum value in the provided array.
        /// </summary>
        /// <param name="s">The span.</param>
        /// <returns>The minimum value in the span.</returns>
        public static float Max(ReadOnlySpan<float> s)
        {
            float max;

            // Run the vectorised code only if the hardware acceleration is available, and there are
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (s.Length >= Vector<float>.Count << 1))
            {
                int width = Vector<float>.Count;
                var maxVec = new Vector<float>(s);
                s = s.Slice(width);

                // Loop over vector sized slices.
                do
                {
                    var vec = new Vector<float>(s);
                    maxVec = Vector.Max(maxVec, vec);
                    s = s.Slice(width);
                }
                while(s.Length >= width);

                // Calc min(minVec) and max(maxVec).
                max = maxVec[0];
                for(int i=1; i < width; i++)
                {
                    if(maxVec[i] > max) max = maxVec[i];
                }
            }
            else
            {
                max = s[0];
            }

            // Calc min/max.
            // Note. If the above vector logic block was executed then this handles remaining elements,
            // otherwise it handles all elements.
            for(int i=0; i < s.Length; i++)
            {
                if(s[i] > max) {
                    max = s[i];
                }
            }

            return max;
        }

        /// <summary>
        /// Determine the minimum and maximum values in the provided array.
        /// </summary>
        /// <param name="s">The span.</param>
        /// <param name="min">Returns the minimum value in the span.</param>
        /// <param name="max">Returns the maximum value in the span.</param>
        public static void MinMax(ReadOnlySpan<float> s, out float min, out float max)
        {
            // Run the vectorised code only if the hardware acceleration is available, and there are
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (s.Length >= Vector<float>.Count << 1))
            {
                int width = Vector<float>.Count;
                var minVec = new Vector<float>(s);
                var maxVec = new Vector<float>(s);
                s = s.Slice(width);

                // Loop over vector sized slices.
                do
                {
                    var vec = new Vector<float>(s);
                    minVec = Vector.Min(minVec, vec);
                    maxVec = Vector.Max(maxVec, vec);
                    s = s.Slice(width);
                }
                while(s.Length >= width);

                // Calc min(minVec) and max(maxVec).
                min = minVec[0];
                max = maxVec[0];
                for(int i=1; i < width; i++)
                {
                    if(minVec[i] < min) min = minVec[i];
                    if(maxVec[i] > max) max = maxVec[i];
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

            // Run the vectorised code only if the hardware acceleration is available, and there are
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (a.Length >= Vector<float>.Count))
            {
                int width = Vector<float>.Count;
                var sumVec = new Vector<float>(0f);

                // Loop over vector sized slices, calc the squared error for each, and accumulate in sumVec.
                do
                {
                    var av = new Vector<float>(a);
                    var bv = new Vector<float>(b);
                    var cv = av - bv;
                    sumVec += cv * cv;

                    a = a.Slice(width);
                    b = b.Slice(width);
                }
                while(a.Length >= width);

                // Sum the elements of sumVec.
                for(int i=0; i < width; i++) {
                    total += sumVec[i];
                }
            }

            // Calc sum(squared error).
            // Note. If the above vector logic block was executed then this handles remaining elements,
            // otherwise it handles all elements.
            for(int i=0; i < a.Length; i++)
            {
                float err = a[i] - b[i];
                total += err * err;
            }

            return total;
        }
    }
}
