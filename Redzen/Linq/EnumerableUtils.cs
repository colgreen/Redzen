﻿// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Buffers;
using Redzen.Random;

namespace Redzen.Linq;

/// <summary>
/// Utility methods related to LINQ and IEnumerable.
/// </summary>
public static class EnumerableUtils
{
    /// <summary>Generates a sequence of integral numbers within a specified range and in random order.</summary>
    /// <param name="start">The value of the first integer in the sequence.</param>
    /// <param name="count">The number of sequential integers to generate.</param>
    /// <param name="rng">Random source.</param>
    /// <returns>A new IEnumerable{int}.</returns>
    public static IEnumerable<int> RangeRandomOrder(int start, int count, IRandomSource rng)
    {
        if(count < 0 || (((long)start + count) - 1L) > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(count));

        // Initialise an array of all indexes to be yielded.
        int[] arr = ArrayPool<int>.Shared.Rent(count);

        try
        {
            for(int i=0; i < count; i++)
                arr[i] = start + i;

            // Yield all values in turn, applying a Fisher–Yates shuffle as we go in order to randomize the yield order.
            // See https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
            for(int i = count-1; i > 0; i--)
            {
                // Select at random from the remaining available slots.
                int selectIdx = rng.Next(i + 1);

                // Store the yield value.
                int tmp = arr[selectIdx];

                // Replace the selected slot value with a value that has not yet been selected.
                // This is half of the Fisher–Yates swap, but since we don't need the final
                // shuffled array we can omit moving the yielded value into its new slot.
                arr[selectIdx] = arr[i];

                // Yield the value from the randomly selected slot.
                yield return tmp;
            }

            // Yield final value.
            yield return arr[0];
        }
        finally
        {
            ArrayPool<int>.Shared.Return(arr);
        }
    }
}
