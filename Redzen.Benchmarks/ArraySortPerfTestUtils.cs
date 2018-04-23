using System;
using Redzen.Random;

namespace Redzen.Benchmarks
{
    internal static class ArraySortPerfTestUtils
    {
        /// <summary>
        /// Initialise an array with random integers.
        /// </summary>
        /// <param name="arr">The array to initialise.</param>
        /// <param name="rng">Random number generator.</param>
        public static void InitRandom(int[] arr, IRandomSource rng)
        {
            for(int i=0; i < arr.Length; i++) {
                arr[i] = rng.Next();
            }
        }

        /// <summary>
        /// Initialise an array with 'naturally' random integers.
        /// The array will contain sub-spans of sorted integers, in both ascending and descending order.
        /// </summary>
        /// <param name="arr">The array to initialise.</param>
        /// <param name="rng">Random number generator.</param>
        public static void InitNatural(int[] arr, IRandomSource rng)
        {
            // Init with an incrementing sequence.
            for(int i=0; i < arr.Length; i++) {
                arr[i] = i;
            }

            // Reverse multiple random sub-ranges.
            int reverseCount = (int)(Math.Sqrt(arr.Length) * 2.0);
            int len = arr.Length;

            for(int i=0; i < reverseCount; i++)
            {
                int idx = rng.Next(arr.Length);
                int idx2 = rng.Next(arr.Length);

                if(idx > idx2) {
                    VariableUtils.Swap(ref idx, ref idx2);
                }

                if(i%2 == 0)
                {
                    Array.Reverse(arr, 0, idx + 1);
                    Array.Reverse(arr, idx2, len - idx2);
                }
                else
                {
                    Array.Reverse(arr, idx, idx2 - idx);
                }
            }
        }
    }
}
