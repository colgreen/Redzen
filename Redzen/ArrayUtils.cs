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
using System.Collections.Generic;

namespace Redzen
{
    /// <summary>
    /// Array static utility methods.
    /// </summary>
    public static class ArrayUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Returns true if the two arrays are equal.
        /// </summary>
        /// <param name="x">First array.</param>
        /// <param name="y">Second array.</param>
        public static bool Equals<T>(T[] x, T[] y)
        {
            // x and y are equal if they are the same reference, or both are null.
            if(x == y) {
                return true;
            }

            // Test if one is null and the other not null.
            // Note. We already tested for both being null (above).
            if(null == x || null == y) {
                return false;
            }

            if(x.Length != y.Length) {
                return false;
            }

            var comp = Comparer<T>.Default;

            for(int i=0; i < x.Length; i++)
            {
                if(comp.Compare(x[i], y[i]) != 0){
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if all of the array elements are equal to the given value.
        /// </summary>
        /// <param name="arr">Array to test..</param>
        /// <param name="v">The test value.</param>
        public static bool Equals<T>(T[] arr, T v)
        {
            var comp = Comparer<T>.Default;

            for(int i=0; i < arr.Length; i++)
            {
                if(comp.Compare(arr[i], v) != 0){
                    return false;
                }
            }
            return true;
        }

        // TODO: Create overload based on Span<T> once it is available.
        /// <summary>
        /// Returns true if the elements of the specified array sub-segment are equal to the given value.
        /// </summary>
        /// <param name="arr">Array to test..</param>
        /// <param name="v">The test value.</param>
        public static bool Equals<T>(
            T[] arr, T v,
            int offset, int length)
        {
            if(offset < 0 || length < 0 || offset + length > arr.Length) {
                throw new ArgumentOutOfRangeException();
            }

            var comp = Comparer<T>.Default;

            for(int i=offset; i < offset + length; i++)
            {
                if(comp.Compare(arr[i], v) != 0){
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
