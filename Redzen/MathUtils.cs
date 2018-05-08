using System;

namespace Redzen
{
    public static class MathUtils
    {
        /// <summary>
        /// Returns the smallest power of two that is greater than or equal to the specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns>The smallest integral power of two that is greater than or equal to x.</returns>
        public static int CeilingToPowerOfTwo(int x)
        {
            // Text for max input value. There is one more high bit, but that is the sign bit.
            if (x < 0 || x > 0x4000_0000) {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            // From: https://stackoverflow.com/questions/466204/rounding-up-to-next-power-of-2
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }

        /// <summary>
        /// Returns the smallest power of two that is greater than or equal to the specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns>The smallest integral power of two that is greater than or equal to x.</returns>
        public static long CeilingToPowerOfTwo(long x)
        {
            // Text for max input value. There is one more high bit, but that is the sign bit.
            if (x < 0 || x > 0x4000_0000_0000_0000){
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            // From: https://stackoverflow.com/questions/466204/rounding-up-to-next-power-of-2
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x |= x >> 32;
            return x + 1;
        }

        /// <summary>
        /// Returns true if the input value is an integer power of two.
        /// </summary>
        /// <param name="x">The value to test.</param>
        /// <returns>True if the input value is an integer power of two; otherwise false.</returns>
        public static bool IsPowerOfTwo(int x)
        {
            // From: https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            return (x != 0) && (x & (x - 1)) == 0;
        }

        /// <summary>
        /// Returns true if the input value is an integer power of two.
        /// </summary>
        /// <param name="x">The value to test.</param>
        /// <returns>True if the input value is an integer power of two; otherwise false.</returns>
        public static bool IsPowerOfTwo(long x)
        {
            // From: https://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            return (x != 0) && (x & (x - 1)) == 0;
        }

        /// <summary>
        /// Evaluate the binary logarithm of a non-zero Int32.
        /// </summary>
        /// <remarks>Two-step method using a De Bruijn-like sequence table lookup.</remarks>
        public static int Log2(int x)
        {
            // Method from: https://stackoverflow.com/a/11398748/15703

            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;

            return __log2_32[(uint)(x * 0x07C4ACDDU) >> 27];
        }

        /// <summary>
        /// Evaluate the binary logarithm of a non-zero Int64.
        /// </summary>
        /// <remarks>Two-step method using a De Bruijn-like sequence table lookup.</remarks>
        public static int Log2(long x)
        {
            // Method from: https://stackoverflow.com/a/11398748/15703

            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x |= x >> 32;

            return __log2_64[((ulong)((x - (x >> 1)) * 0x07EDD5E59A4E28C2)) >> 58];
        }


        static readonly int[] __log2_32 = new int[32]
        {
            0,   9,  1, 10, 13, 21, 2, 29,
            11, 14, 16, 18, 22, 25, 3, 30,
            8,  12, 20, 28, 15, 17, 24, 7,
            19, 27, 23,  6, 26,  5,  4, 31
        };

        static readonly int[] __log2_64 = new int[64]
        {
            63,  0, 58,  1, 59, 47, 53,  2,
            60, 39, 48, 27, 54, 33, 42,  3,
            61, 51, 37, 40, 49, 18, 28, 20,
            55, 30, 34, 11, 43, 14, 22,  4,
            62, 57, 46, 52, 38, 26, 32, 41,
            50, 36, 17, 19, 29, 10, 13, 21,
            56, 45, 25, 31, 35, 16,  9, 12,
            44, 24, 15,  8, 23,  7,  6,  5
        };
    }
}
