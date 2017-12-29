using System;
using System.Collections.Generic;
using Redzen.Random;

namespace Redzen.Linq
{
    /// <summary>
    /// Utility methods related to LINQ and IEnumerable.
    /// </summary>
    public class EnumerableUtils
    {
        /// <summary>Generates a sequence of integral numbers within a specified range and in random order.</summary>
        /// <param name="start">The value of the first integer in the sequence.</param>
        /// <param name="count">The number of sequential integers to generate.</param>
        /// <returns>A new IEnumerable{int}.</returns>
        public static IEnumerable<int> RangeRandomOrder(int start, int count, IRandomSource rng)
        {
	        long numl = (long)start + (long)count - 1L;
	        if (count < 0 || numl > 2147483647L) {
		        throw new ArgumentOutOfRangeException("count");
	        }

            // Create an array of all indexes to be yielded.
            int[] arr = new int[count];
            for(int i=0; i < count; i++) {
                arr[i] = start + i;
            }

            // Yield all values in turn, applying a Fisher–Yates shuffle as we go in order to randomize the yield order.
            // See https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
	        for (int i=count-1; i > 0; i--)
	        {
                int swapIdx = rng.Next(i + 1);
                int tmp = arr[swapIdx];
                arr[swapIdx] = arr[i];
                arr[i] = tmp;

		        yield return arr[i];
	        }

            // Yield final value.
            yield return arr[0];	        
        }
    }
}
