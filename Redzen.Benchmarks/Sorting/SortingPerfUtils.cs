using System;
using Redzen.Random;

namespace Redzen.Benchmarks.Sorting
{
    internal static class SortingPerfUtils
    {
        /// <summary>
        /// Initialise an array with random integers.
        /// </summary>
        /// <param name="keys">The array to initialise.</param>
        /// <param name="rng">Random number generator.</param>
        public static void InitRandom(int[] keys, IRandomSource rng)
        {
            for(int i=0; i < keys.Length; i++) {
                keys[i] = rng.Next();
            }
        }

        /// <summary>
        /// Initialise an array with random integers.
        /// </summary>
        /// <param name="keys">The array to initialise.</param>
        /// <param name="values">A values array.</param>
        /// <param name="rng">Random number generator.</param>
        public static void InitRandom(int[] keys, int[] values, IRandomSource rng)
        {
            for(int i=0; i < keys.Length; i++) 
            {
                keys[i] = rng.Next();
                values[i] = keys[i];
            }
        }

        /// <summary>
        /// Initialise an array with random integers.
        /// </summary>
        /// <param name="keys">The keys array to initialise.</param>
        /// <param name="values">A values array.</param>
        /// <param name="values2">A secondary values array.</param>
        /// <param name="rng">Random number generator.</param>
        public static void InitRandom(int[] keys, int[] values, int[] values2, IRandomSource rng)
        {
            for(int i=0; i < keys.Length; i++) 
            {
                keys[i] = rng.Next();
                values[i] = keys[i];
                values2[i] = keys[i];
            }
        }

        /// <summary>
        /// Initialise an array with 'naturally' random integers.
        /// The array will contain sub-spans of sorted integers, in both ascending and descending order.
        /// </summary>
        /// <param name="keys">The array to initialise.</param>
        /// <param name="rng">Random number generator.</param>
        public static void InitNatural(int[] keys, IRandomSource rng)
        {
            // Init with an incrementing sequence.
            for(int i=0; i < keys.Length; i++) {
                keys[i] = i;
            }

            // Reverse multiple random sub-ranges.
            int reverseCount = (int)(Math.Sqrt(keys.Length) * 2.0);
            int len = keys.Length;

            for(int i=0; i < reverseCount; i++)
            {
                int idx = rng.Next(keys.Length);
                int idx2 = rng.Next(keys.Length);

                if(idx > idx2) {
                    VariableUtils.Swap(ref idx, ref idx2);
                }

                if(rng.NextBool())
                {
                    Array.Reverse(keys, 0, idx + 1);
                    Array.Reverse(keys, idx2, len - idx2);
                }
                else
                {
                    Array.Reverse(keys, idx, idx2 - idx);
                }
            }
        }

        /// <summary>
        /// Initialise an array with 'naturally' random integers.
        /// The array will contain sub-spans of sorted integers, in both ascending and descending order.
        /// </summary>
        /// <param name="keys">The array to initialise.</param>
        /// <param name="values">A values array.</param>
        /// <param name="rng">Random number generator.</param>
        public static void InitNatural(int[] keys, int[] values, IRandomSource rng)
        {
            // Init with an incrementing sequence.
            for(int i=0; i < keys.Length; i++) {
                keys[i] = i;
            }

            // Reverse multiple random sub-ranges.
            int reverseCount = (int)(Math.Sqrt(keys.Length) * 2.0);
            int len = keys.Length;

            for(int i=0; i < reverseCount; i++)
            {
                int idx = rng.Next(keys.Length);
                int idx2 = rng.Next(keys.Length);

                if(idx > idx2) {
                    VariableUtils.Swap(ref idx, ref idx2);
                }

                if(rng.NextBool())
                {
                    Array.Reverse(keys, 0, idx + 1);
                    Array.Reverse(keys, idx2, len - idx2);
                }
                else
                {
                    Array.Reverse(keys, idx, idx2 - idx);
                }
            }

            for(int i=0; i < keys.Length; i++) {
                values[i] = keys[i];
            }
        }

        /// <summary>
        /// Initialise an array with 'naturally' random integers.
        /// The array will contain sub-spans of sorted integers, in both ascending and descending order.
        /// </summary>
        /// <param name="keys">The array to initialise.</param>
        /// <param name="values">A values array.</param>
        /// <param name="values2">A secondary values array.</param>
        /// <param name="rng">Random number generator.</param>
        public static void InitNatural(int[] keys, int[] values, int[] values2, IRandomSource rng)
        {
            // Init with an incrementing sequence.
            for(int i=0; i < keys.Length; i++) {
                keys[i] = i;
            }

            // Reverse multiple random sub-ranges.
            int reverseCount = (int)(Math.Sqrt(keys.Length) * 2.0);
            int len = keys.Length;

            for(int i=0; i < reverseCount; i++)
            {
                int idx = rng.Next(keys.Length);
                int idx2 = rng.Next(keys.Length);

                if(idx > idx2) {
                    VariableUtils.Swap(ref idx, ref idx2);
                }

                if(rng.NextBool())
                {
                    Array.Reverse(keys, 0, idx + 1);
                    Array.Reverse(keys, idx2, len - idx2);
                }
                else
                {
                    Array.Reverse(keys, idx, idx2 - idx);
                }
            }

            for(int i=0; i < keys.Length; i++) 
            {
                values[i] = keys[i];
                values2[i] = keys[i];
            }
        }
    }
}
