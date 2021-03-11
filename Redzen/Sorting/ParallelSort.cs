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
using System.Threading.Tasks;

namespace Redzen.Sorting
{
    // TODO: Review this class. It's likely not a good choice even for very large arrays.

    /// <summary>
    /// Parallel quicksort algorithm.
    /// </summary>
    /// <typeparam name="T">Sort item type.</typeparam>
    public static class ParallelSort<T>
        where T : IComparable<T>
    {
        #region Public Static Methods

        /// <summary>
        /// Sequential quicksort.
        /// </summary>
        /// <param name="arr">The array to sort.</param>
        public static void QuicksortSequential(T[] arr)
        {
            QuicksortSequential(arr, 0, arr.Length - 1);
        }

        /// <summary>
        /// Parallel quicksort.
        /// </summary>
        /// <param name="arr">The array to sort.</param>
        public static void QuicksortParallel(T[] arr)
        {
            QuicksortParallel(arr, 0, arr.Length - 1);
        }

        #endregion

        #region Private Static Methods

        private static void QuicksortSequential(T[] arr, int left, int right)
        {
            if (right > left)
            {
                int pivot = Partition(arr, left, right);
                QuicksortSequential(arr, left, pivot - 1);
                QuicksortSequential(arr, pivot + 1, right);
            }
        }

        private static void QuicksortParallel(T[] arr, int left, int right)
        {
            const int SequentialThreshold = 2048;
            if (right > left)
            {
                if (right - left < SequentialThreshold)
                {
                    QuicksortSequential(arr, left, right);
                }
                else
                {
                    int pivot = Partition(arr, left, right);
                    Parallel.Invoke(
                        new Action[]
                        {
                            () => QuicksortParallel(arr, left, pivot - 1),
                            () => QuicksortParallel(arr, pivot + 1, right)
                        });
                }
            }
        }

        private static int Partition(T[] arr, int low, int high)
        {
            // Simple partitioning implementation
            int pivotPos = (high + low) / 2;
            T pivot = arr[pivotPos];
            Swap(arr, low, pivotPos);

            int left = low;
            for(int i = low + 1; i <= high; i++)
            {
                if (arr[i].CompareTo(pivot) < 0)
                {
                    left++;
                    Swap(arr, i, left);
                }
            }

            Swap(arr, low, left);
            return left;
        }

        private static void Swap(T[] arr, int i, int j)
        {
            T tmp = arr[i];
            arr[i] = arr[j];
            arr[j] = tmp;
        }

        #endregion
    }
}
