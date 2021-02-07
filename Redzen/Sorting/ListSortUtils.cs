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
using System.Collections.Generic;
using Redzen.Random;

namespace Redzen.Sorting
{
    /// <summary>
    /// Helper methods related to sorting elements of an <see cref="IList{T}"/>.
    /// </summary>
    /// <remarks>
    /// These methods have been moved from SortUtils into their own class, to prevent pollution of the API surface
    /// on SortUtils. E.g. if IsSortedAscending() has overloads for IList{T} and Span{T}, then an array can be
    /// passed to either of those, and the caller will need to cast to a Span to use the faster code inside the
    /// Span overload. The extra case results in messy code. Ideally we would delete this class, but these methods
    /// may still be useful in some scenarios, so they have been kept, but moved to one side in their own helper
    /// class.
    /// </remarks>
    public static class ListSortUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Indicates if the items of a list are sorted in ascending order.
        /// </summary>
        /// <param name="list">The list to test.</param>
        /// <typeparam name="T">The span element type.</typeparam>
        /// <returns>True if the list elements are sorted in ascending order; otherwise false.</returns>
        /// <remarks>
        /// This method requires that all of the list items are non-null. To perform the IsSorted test on a list
        /// containing null elements use the overload of IsSortedAscending() that accepts an <see cref="IComparer{T}"/>.
        /// </remarks>
        public static bool IsSortedAscending<T>(IList<T> list)
            where T : IComparable<T>
        {
            // Invoke the faster Span overload if the IList is an array.
            if (list is T[] arr) {
                return SortUtils.IsSortedAscending<T>(arr);
            }

            if (list.Count < 2) {
                return true;
            }

            // TODO: Performance tune based on comments here: https://news.ycombinator.com/item?id=16842045
            for (int i=0; i < list.Count - 1; i++)
            {
                if(list[i].CompareTo(list[i+1]) > 0) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Indicates if the items of a list are sorted in ascending order, based on the sort order defined by a
        /// provided <see cref="IComparer{T}"/>.
        /// </summary>
        /// <param name="list">The list to test.</param>
        /// <param name="comparer">The comparer to use for comparing list elements.</param>
        /// <typeparam name="T">The list element type.</typeparam>
        /// <returns>True if the list elements are sorted in ascending order; otherwise false.</returns>
        public static bool IsSortedAscending<T>(
            IList<T> list,
            IComparer<T> comparer)
        {
            // Invoke the faster Span overload if the IList is an array.
            if (list is T[] arr) {
                return SortUtils.IsSortedAscending(arr, comparer);
            }

            if (list.Count < 2) {
                return true;
            }

            for (int i=0; i < list.Count - 1; i++)
            {
                if(comparer.Compare(list[i], list[i+1]) > 0) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Randomly shuffles the items of a list.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="rng">Random number generator.</param>
        /// <typeparam name="T">The list element type.</typeparam>
        public static void Shuffle<T>(IList<T> list, IRandomSource rng)
        {
            // Invoke the faster Span overload if the IList is an array.
            if (list is T[] arr)
            {
                SortUtils.Shuffle<T>(arr, rng);
                return;
            }

            // Fisher–Yates shuffle.
            // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

            for (int i = list.Count-1; i > 0; i--)
            {
                int swapIdx = rng.Next(i + 1);
                T tmp = list[swapIdx];
                list[swapIdx] = list[i];
                list[i] = tmp;
            }
        }

        /// <summary>
        /// Randomly shuffles a sub-span of items within a list.
        /// </summary>
        /// <param name="list">The list to shuffle.</param>
        /// <param name="rng">Random number generator.</param>
        /// <param name="startIdx">The index of the first item in the segment.</param>
        /// <param name="endIdx">The index of the last item in the segment, i.e. endIdx is inclusive; the item at endIdx will participate in the shuffle.</param>
        /// <typeparam name="T">The list element type.</typeparam>
        public static void Shuffle<T>(IList<T> list, IRandomSource rng, int startIdx, int endIdx)
        {
            // Invoke the faster Span overload if the IList is an array.
            if (list is T[] arr)
            {
                SortUtils.Shuffle(arr.AsSpan().Slice(startIdx, (endIdx - startIdx) + 1), rng);
                return;
            }

            // Fisher–Yates shuffle.
            // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
            for (int i = endIdx; i > startIdx; i--)
            {
                int swapIdx = startIdx + rng.Next((i - startIdx) + 1);
                T tmp = list[swapIdx];
                list[swapIdx] = list[i];
                list[i] = tmp;
            }
        }

        // TODO: Implementation based on IList<T>; this is not currently done because there is no method available for sorting items of an IList<T>.
        // TODO: Implementation of SortUnstable based on IComparable span items.

        /// <summary>
        /// Sort the items in the provided list. In addition, ensure that items defined as equal by the IComparer
        /// are arranged randomly.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">The list of items to sort.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> to use when comparing items.</param>
        /// <param name="rng">Random number generator.</param>
        public static void SortUnstable<T>(
            List<T> list,
            IComparer<T> comparer,
            IRandomSource rng)
        {
            // Notes.
            // The naive approach is to shuffle the list items and then call Sort(). Regardless of whether the sort is stable or not,
            // the equal items would be arranged randomly within their sorted sub-segments.
            // However, typically lists are already partially sorted and that fact improves the performance of the sort. To try and
            // keep some of that benefit we call sort first, and then call shuffle on sub-segments of items identified as equal.

            // TODO: Unit test.

            // Sort the list.
            list.Sort(comparer);

            // Scan for segments of items that are equal.
            int startIdx = 0;
            int count = list.Count;

            while(TryFindSegment(list, comparer, ref startIdx, out int endIdx))
            {
                // Shuffle the segment of equal items.
                Shuffle(list, rng, startIdx, endIdx);

                // Test for the end of the list.
                // N.B. If endIdx points to one of the last two items then there can be no more segments (segments are made of at least two items).
                if(endIdx > count-3) {
                    break;
                }

                // Set the startIdx of the next candidate segment.
                startIdx = endIdx + 1;
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Search for a contiguous segment of two or more equal elements.
        /// </summary>
        /// <typeparam name="T">List item type.</typeparam>
        /// <param name="list">The list to search.</param>
        /// <param name="comparer">A list item comparer.</param>
        /// <param name="startIdx">The index to start the search at; returns the start index of the first contiguous segment.</param>
        /// <param name="endIdx">Returns the last index of the contiguous segment.</param>
        /// <returns>True if a contiguous segment of two or more elements was found; otherwise false.</returns>
        private static bool TryFindSegment<T>(List<T> list, IComparer<T> comparer, ref int startIdx, out int endIdx)
        {
            // Scan for a matching contiguous pair of elements.
            int count = list.Count;
            for (; startIdx < count - 1; startIdx++)
            {
                // Test if the current element is equal to the next one.
                if (comparer.Compare(list[startIdx], list[startIdx + 1]) == 0)
                {
                    // Scan for the end of the contiguous segment.
                    T startItem = list[startIdx];
                    for (endIdx = startIdx + 2; endIdx < count && comparer.Compare(startItem, list[endIdx]) == 0; endIdx++) ;

                    // endIdx points to the item after the segment's end, so we decrement.
                    endIdx--;
                    return true;
                }
            }

            // No contiguous segment found.
            endIdx = 0;
            return false;
        }

        #endregion
    }
}
