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

namespace Redzen
{
    /// <summary>
    /// Helper methods related to binary search.
    /// </summary>
    public static class SearchUtils
    {
        /// <summary>
        /// Searches a sub-range of a one-dimensional sorted list for a specific item, using the provided comparison function.
        /// </summary>
        /// <typeparam name="T">The span element type.</typeparam>
        /// <typeparam name="V">The type of value being searched on.</typeparam>
        /// <param name="span">The span to search.</param>
        /// <param name="value">The value being searched for.</param>
        /// <param name="compareFn">Comparison function. For comparing span elements based on the return value of this function
        /// that allows for comparison between two distinct types.</param>
        /// <returns>
        /// The zero-based index of an element in the span, if item is found; otherwise, a negative number that is the bitwise 
        /// complement of the index of the next element that is larger than item or, if there is no larger element, the bitwise
        /// complement of list.Count.
        /// </returns>
        public static int BinarySearch<T,V>(
            Span<T> span,
            V value,
            Func<T,V,int> compareFn)
        {
			int lo = 0;
			int hi = span.Length - 1;
			while (lo <= hi)
			{
				int mid = lo + ((hi - lo) >> 1);

				int cmp = compareFn(span[mid], value);

				if (cmp == 0) {
					return mid;
				}

				if (cmp < 0) {
					lo = mid + 1;
				}
                else {
					hi = mid - 1;
				}
			}
			return ~lo;
        }
    }
}
