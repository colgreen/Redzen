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
        /// Searches an entire one-dimensional sorted list for a specific item, using the provided comparison function.
        /// </summary>
        /// <typeparam name="T">The list item type.</typeparam>
        /// <typeparam name="V">The type of value being searched on.</typeparam>
        /// <param name="list">The list of items to search.</param>
        /// <param name="value">The value being search for.</param>
        /// <param name="compareFn">Comparison function. For comparing list items based on the return value of this function rather than the items themselves.</param>
        /// <returns>The zero-based index of item in the list, if item is found; otherwise, a negative number that is the bitwise complement of the index of the next 
        /// element that is larger than item or, if there is no larger element, the bitwise complement of list.Count.</returns>
        public static int BinarySearch<T,V>(IList<T> list, V value, Func<T,V,int> compareFn)
        {
			int lower = 0;
			int upper = list.Count - 1;
			while (lower <= upper)
			{
				int mid = lower + (upper - lower >> 1);

				int cmp = compareFn(list[mid], value);
				if (cmp == 0) {
					return mid;
				}

				if (cmp < 0) {
					lower = mid + 1;
				} else {
					upper = mid - 1;
				}
			}
			return ~lower;
        }
    }
}
