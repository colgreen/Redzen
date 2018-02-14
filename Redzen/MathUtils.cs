
namespace Redzen
{
    public static class MathUtils
    {
        /// <summary>
        /// Returns the smallest power of two that is greater than or equal to the specified number.
        /// </summary>
        /// <param name="x"></param>
        /// <returns>The smallest integral power of two that is greater than or equal to x.</returns>
        public static int CeilingPowerOfTwo(int x)
        {
            // From: https://stackoverflow.com/questions/466204/rounding-up-to-next-power-of-2
            uint v = unchecked((uint)x); // compute the next highest power of 2 of 32-bit v

            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;

            return (int)v;
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
    }
}
