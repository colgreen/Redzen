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
    /// Helper methods related to sorting.
    /// </summary>
    public static class SortUtils
    {
        #region Public Static Methods

        /// <summary>
        /// Indicates if the items of a span are sorted in ascending order.
        /// </summary>
        /// <param name="span">The span to test.</param>
        /// <typeparam name="T">The span element type.</typeparam>
        /// <returns>True if the span elements are sorted in ascending order; otherwise false.</returns>
        /// <remarks>
        /// This method requires that all of the span items are non-null. To perform the IsSorted test on a span
        /// containing null elements use the overload of IsSortedAscending() that accepts an <see cref="IComparer{T}"/>.
        /// </remarks>
        public static bool IsSortedAscending<T>(Span<T> span)
            where T : IComparable<T>
        {
            if (span.Length < 2) {
                return true;
            }

            // TODO: Performance tune based on comments here: https://news.ycombinator.com/item?id=16842045
            for (int i=0; i < span.Length - 1; i++)
            {
                if(span[i].CompareTo(span[i+1]) > 0) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Indicates if the items of a span are sorted in ascending order, based on the sort order defined by a
        /// provided <see cref="IComparer{T}"/>.
        /// </summary>
        /// <param name="span">The span to test.</param>
        /// <param name="comparer">The comparer to use for comparing span elements.</param>
        /// <typeparam name="T">The span element type.</typeparam>
        /// <returns>True if the span elements are sorted in ascending order; otherwise false.</returns>
        public static bool IsSortedAscending<T>(
            Span<T> span,
            IComparer<T> comparer)
        {
            if (span.Length < 2) {
                return true;
            }

            for (int i=0; i < span.Length - 1; i++)
            {
                if(comparer.Compare(span[i], span[i+1]) > 0) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Randomly shuffles the items of a span.
        /// </summary>
        /// <param name="span">The span to shuffle.</param>
        /// <param name="rng">Random number generator.</param>
        /// <typeparam name="T">The span element type.</typeparam>
        public static void Shuffle<T>(Span<T> span, IRandomSource rng)
        {
            // Fisher–Yates shuffle.
            // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

            for(int i = span.Length-1; i > 0; i--)
            {
                int swapIdx = rng.Next(i + 1);
                T tmp = span[swapIdx];
                span[swapIdx] = span[i];
                span[i] = tmp;
            }
        }

        // TODO: Implementation of SortUnstable based on IComparable span items.

        /// <summary>
        /// Sort the items in the provided span. In addition we ensure that items defined as equal by the IComparer
        /// are arranged randomly.
        /// </summary>
        /// <typeparam name="T">The type of items in the span.</typeparam>
        /// <param name="span">The span of items to sort.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> to use when comparing items.</param>
        /// <param name="rng">Random number generator.</param>
        public static void SortUnstable<T>(
            Span<T> span,
            IComparer<T> comparer,
            IRandomSource rng)
        {
            // Notes.
            // The naive approach is to shuffle the list items and then call Sort(). Regardless of whether the sort is stable or not,
            // the equal items would be arranged randomly within their sorted sub-segments.
            // However, typically lists are already partially sorted and that fact improves the performance of the sort. To try and
            // keep some of that benefit we call sort first, and then call shuffle on sub-segments of items identified as equal.

            // TODO: Unit test.

            // Sort the span.
            span.Sort(comparer);

            // Scan for segments of items that are equal.
            int startIdx = 0;

            while(TryFindSegment(span, comparer, ref startIdx, out int length))
            {
                // Shuffle the segment of equal items.
                Shuffle(span.Slice(startIdx, length), rng);

                // Set startIdx to point at the first item of the next candidate segment.
                startIdx += length;

                // Test for the end of the span.
                // Note. If there are one or fewer items remaining in the span, then there can be no more contiguous segments to find,
                // and therefore we exit.
                if(startIdx > span.Length-2) {
                    break;
                }
            }
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Search for a contiguous segment of two or more equal elements.
        /// </summary>
        /// <typeparam name="T">Span element type.</typeparam>
        /// <param name="span">The span to search.</param>
        /// <param name="comparer">A comparer.</param>
        /// <param name="startIdx">The index to start the search at; returns the start index of the first contiguous segment.</param>
        /// <param name="length">Returns the length of the contiguous segment.</param>
        /// <returns>True if a contiguous segment of two or more elements was found; otherwise false.</returns>
        private static bool TryFindSegment<T>(
            Span<T> span,
            IComparer<T> comparer,
            ref int startIdx,
            out int length)
        {
            // Scan for a matching contiguous pair of elements.
            for(; startIdx < span.Length-1; startIdx++)
            {
                // Test if the current element is equal to the next one.
                if(comparer.Compare(span[startIdx], span[startIdx+1]) == 0)
                {
                    // Scan for the end of the contiguous segment.
                    int endIdx = startIdx+2;
                    T startElem = span[startIdx];
                    for(; endIdx < span.Length && comparer.Compare(startElem, span[endIdx]) == 0; endIdx++);

                    // Calc length of the segment, and return.
                    length = endIdx - startIdx;
                    return true;
                }
            }

            // No contiguous segment found.
            length = 0;
            return false;
        }

        #endregion
    }
}
