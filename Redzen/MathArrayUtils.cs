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
using System.Numerics;

namespace Redzen
{
    /// <summary>
    /// Math utility methods for working with arrays.
    /// </summary>
    public static class MathArrayUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Clip (limit) the values in an array.
        /// For example, if an interval of [0, 1] is specified, values smaller than 0 become 0, and values larger than 1 become 1.
        /// </summary>
        /// <param name="x">Array containing the elements to clip.</param>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public static void Clip(double[] x, double min, double max)
        {
            int idx=0;

            // Run the vectorised code only if the hardware acceleration is available, and there are 
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (x.Length > Vector<double>.Count))
            {
                int width = Vector<double>.Count;
                var minVec = new Vector<double>(min);
                var maxVec = new Vector<double>(max);

                // Loop over vector sized segments.
                for(; idx <= x.Length - width; idx += width)
                {
                    var xv = new Vector<double>(x, idx);
                    xv = Vector.Max(minVec, xv);
                    xv = Vector.Min(maxVec, xv);
                    xv.CopyTo(x, idx);
                }
            }

            // Note. If the above vector logic block was executed then this handles remaining elements,
            // otherwise it handles all elements.
            for(; idx < x.Length; idx++)
            {
                if(x[idx] < min)
                    x[idx] = min;
                else if(x[idx] > max)
                    x[idx] = max;
            }
        }

        /// <summary>
        /// Calculate the mean squared difference of the elements in arrays {a} and {b}.
        /// </summary>
        /// <param name="a">Array {a}.</param>
        /// <param name="b">Array {b}.</param>
        /// <returns>A double.</returns>
        /// <remarks>Arrays {a} and {b} must be the same length.</remarks>
        public static double MeanSquaredDelta(double[] a, double[] b)
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
        public static double SumSquaredDelta(double[] a, double[] b)
        {
            if(a.Length != b.Length) throw new ArgumentException("Array lengths are not equal.");

            double total = 0.0;
            int idx=0;

            // Run the vectorised code only if the hardware acceleration is available, and there are 
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (a.Length > Vector<double>.Count))
            {
                int width = Vector<double>.Count;
                var sumVec = new Vector<double>(0.0);

                // Loop over vector sized segments, calc the squared error for each, and accumulate in sumVec.
                for(; idx <= a.Length - width; idx += width)
                {
                    var av = new Vector<double>(a, idx);
                    var bv = new Vector<double>(b, idx);

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

        /// <summary>
        /// Calculate the minimum and maximum values in the provided array.
        /// </summary>
        /// <param name="a">The array.</param>
        /// <param name="min">Returns the minimum value in the array.</param>
        /// <param name="max">Returns the maximum value in the array.</param>
        public static void MinMax(double[] a, out double min, out double max)
        {
            int idx=0;

            // Run the vectorised code only if the hardware acceleration is available, and there are 
            // enough array elements to utilise it.
            if(Vector.IsHardwareAccelerated && (a.Length > Vector<double>.Count))
            {
                int width = Vector<double>.Count;
                var minVec = new Vector<double>(a[0]);
                var maxVec = new Vector<double>(a[0]);

                // Loop over vector sized segments.
                for(; idx <= a.Length - width; idx += width)
                {
                    var xv = new Vector<double>(a, idx);
                    minVec = Vector.Min(minVec, xv);
                    maxVec = Vector.Max(maxVec, xv);
                }

                // Calc min(minVec) and max(maxVec).
                min = max = a[0];
                for(int j=0; j < width; j++)
                {
                    if(minVec[j] < min) min = minVec[j];
                    if(maxVec[j] > max) max = maxVec[j];
                }
            }
            else
            {
                min = max = a[0];
                idx = 1;
            }

            // Calc min/max.
            // Note. If the above vector logic block was executed then this handles remaining elements,
            // otherwise it handles all elements.
            for(; idx < a.Length; idx++)
            {
                double val = a[idx];
                if(val < min) {
                    min = val;
                }
                else if(val > max) {
                    max = val;
                }
            }
        }

        #endregion
    }
}
