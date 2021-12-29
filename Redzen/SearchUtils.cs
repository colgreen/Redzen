/* ***************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015-2021 Colin Green (colin.green1@gmail.com)
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
        public static int BinarySearch<T, V>(
            ReadOnlySpan<T> span,
            V value,
            Func<T,V,int> compareFn)
        {
            int lo = 0;
            int hi = span.Length - 1;
            while(lo <= hi)
            {
                int mid = lo + ((hi - lo) >> 1);

                int cmp = compareFn(span[mid], value);

                if(cmp == 0)
                    return mid;

                if(cmp < 0)
                    lo = mid + 1;
                else
                    hi = mid - 1;
            }
            return ~lo;
        }

        /// <summary>
        /// Searches an entire one-dimensional sorted list for a specific item, using the provided comparison function.
        /// </summary>
        /// <typeparam name="T">The list item type.</typeparam>
        /// <typeparam name="V">The type of value being searched on.</typeparam>
        /// <param name="list">The list of items to search.</param>
        /// <param name="value">The value being search for.</param>
        /// <param name="compareFn">Comparison function. For comparing list items based on the return value of this function rather than the items themselves.</param>
        /// <returns>The zero-based index of an item in the list, if item is found; otherwise, a negative number that is the bitwise complement of the index of the next
        /// element that is larger than item or, if there is no larger element, the bitwise complement of list.Count.</returns>
        public static int BinarySearch<T, V>(
            IList<T> list,
            V value,
            Func<T,V,int> compareFn)
        {
            // Invoke the faster Span overload if the IList is an array.
            if(list is T[] arr)
                return BinarySearch((ReadOnlySpan<T>)arr, value, compareFn);

            return BinarySearch(list, 0, list.Count, value, compareFn);
        }

        /// <summary>
        /// Searches a sub-range of a one-dimensional sorted list for a specific item, using the provided comparison function.
        /// </summary>
        /// <typeparam name="T">The list item type.</typeparam>
        /// <typeparam name="V">The type of value being searched on.</typeparam>
        /// <param name="list">The list of items to search.</param>
        /// <param name="index">The starting index of the range to search.</param>
        /// <param name="length">The length of the range to search.</param>
        /// <param name="value">The value being search for.</param>
        /// <param name="compareFn">Comparison function. For comparing list items based on the return value of this function rather than the items themselves.</param>
        /// <returns>The zero-based index of an item in the list, if item is found; otherwise, a negative number that is
        /// the bitwise complement of the index of the next element that is larger than item or, if there is no larger element,
        /// the bitwise complement of list.Count.</returns>
        public static int BinarySearch<T,V>(
            IList<T> list,
            int index,
            int length,
            V value,
            Func<T,V,int> compareFn)
        {
            int lo = index;
            int hi = index + length - 1;
            while(lo <= hi)
            {
                int mid = lo + ((hi - lo) >> 1);

                int cmp = compareFn(list[mid], value);
                if(cmp == 0)
                    return mid;

                if(cmp < 0)
                    lo = mid + 1;
                else
                    hi = mid - 1;
            }
            return ~lo;
        }
    }
}
