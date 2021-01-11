using System;
using Redzen.Random;

namespace Redzen.Benchmarks.Sorting
{
    internal static class SpanSortPerfUtils
    {
        /// <summary>
        /// Initialise an array with random integers.
        /// </summary>
        /// <param name="keys">The array to initialise.</param>
        /// <param name="rng">Random number generator.</param>
        public static void InitRandom(Span<int> keys, IRandomSource rng)
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
        public static void InitRandom(Span<int> keys, Span<int> values, IRandomSource rng)
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
        public static void InitRandom(Span<int> keys, Span<int> values, Span<int> values2, IRandomSource rng)
        {
            for(int i=0; i < keys.Length; i++) 
            {
                keys[i] = rng.Next();
                values[i] = keys[i];
                values2[i] = keys[i];
            }
        }

        /// <summary>
        /// Initialise a span with 'naturally' ordered random integers.
        /// The initialised span will contain sub-spans of sorted integers, in both ascending and descending order.
        /// </summary>
        /// <param name="keys">The span to initialise.</param>
        /// <param name="rng">Random number generator.</param>
        public static void InitNatural(Span<int> keys, IRandomSource rng)
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
                    keys[0..idx].Reverse();
                    keys[idx2..].Reverse();
                }
                else
                {
                    keys[idx..idx2].Reverse();
                }
            }
        }

        /// <summary>
        /// Initialise a span with 'naturally' ordered random integers.
        /// The initialised span will contain sub-spans of sorted integers, in both ascending and descending order.
        /// </summary>
        /// <param name="keys">The span to initialise.</param>
        /// <param name="values">A values span.</param>
        /// <param name="rng">Random number generator.</param>
        public static void InitNatural(Span<int> keys, Span<int> values, IRandomSource rng)
        {
            InitNatural(keys, rng);

            for(int i=0; i < keys.Length; i++) {
                values[i] = keys[i];
            }
        }

        /// <summary>
        /// Initialise a span with 'naturally' ordered random integers.
        /// The initialised span will contain sub-spans of sorted integers, in both ascending and descending order.
        /// </summary>
        /// <param name="keys">The span to initialise.</param>
        /// <param name="values">A values span.</param>
        /// <param name="values2">A secondary values span.</param>
        /// <param name="rng">Random number generator.</param>
        public static void InitNatural(Span<int> keys, Span<int> values, Span<int> values2, IRandomSource rng)
        {
            InitNatural(keys, rng);

            for(int i=0; i < keys.Length; i++) 
            {
                values[i] = keys[i];
                values2[i] = keys[i];
            }
        }
    }
}
