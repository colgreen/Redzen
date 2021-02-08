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
using System.Numerics;

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
        public static float Sum(Span<float> s)
        {
            // TODO: Vectorize.
            float sum = 0f;
            for(int i=0; i < s.Length; i++)
            {
                sum += s[i];
            }
            return sum;
        }


        /// <summary>
        /// Calculate the arithmetic mean of the span elements.
        /// </summary>
        /// <param name="s">The span.</param>
        /// <returns>The sum of the elements.</returns>
        public static float Mean(Span<float> s)
        {
            return s.Length != 0 ? Sum(s) / s.Length : 0f;
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
        public static void MinMax(Span<float> s, out float min, out float max)
        {
            int idx=0;

            // Run the vectorised code only if the hardware acceleration is available, and there are
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (s.Length >= Vector<float>.Count))
            {
                int width = Vector<float>.Count;
                var minVec = new Vector<float>(s[0]);
                var maxVec = new Vector<float>(s[0]);

                // Loop over vector sized segments.
                for(; idx <= s.Length - width; idx += width)
                {
                    var xv = new Vector<float>(s.Slice(idx, width));
                    minVec = Vector.Min(minVec, xv);
                    maxVec = Vector.Max(maxVec, xv);
                }

                // Calc min(minVec) and max(maxVec).
                min = max = s[0];
                for(int j=0; j < width; j++)
                {
                    if(minVec[j] < min) min = minVec[j];
                    if(maxVec[j] > max) max = maxVec[j];
                }
            }
            else
            {
                min = max = s[0];
                idx = 1;
            }

            // Calc min/max.
            // Note. If the above vector logic block was executed then this handles remaining elements,
            // otherwise it handles all elements.
            for(; idx < s.Length; idx++)
            {
                float val = s[idx];
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
        public static float MeanSquaredDelta(Span<float> a, Span<float> b)
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
        public static float SumSquaredDelta(Span<float> a, Span<float> b)
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
