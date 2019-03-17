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

            if(Vector.IsHardwareAccelerated)
            {
                int width = Vector<double>.Count;
                var sumVec = new Vector<double>(0.0);

                // Loop over vector sized segments, calc the squared error for each, and accumulate in sumVec.
                int idx=0;
                for(; idx <= a.Length - width; idx += width)
                {
                    var aVec = new Vector<double>(a, idx);
                    var bVec = new Vector<double>(b, idx);

                    var cVec = aVec - bVec;
                    sumVec += cVec * cVec;
                }

                // Sum the elements of sumVec.
                for(int j=0; j < width; j++){
                    total += sumVec[j];
                }

                // Handle remaining elements (if any).
                for(; idx < a.Length; idx++)
                {
                    double err = a[idx] - b[idx];
                    total += err * err;
                }
            }
            else
            {
                // Calc sum(squared error).
                for(int i=0; i < a.Length; i++)
                {
                    double err = a[i] - b[i];
                    total += err * err;
                }
            }
            return total;
        }

        #endregion
    }
}
