// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace Redzen;

/// <summary>
/// Math utility methods.
/// </summary>
public static class MathUtils
{
    /// <summary>
    /// Returns true if the input value is an integer power of two.
    /// </summary>
    /// <param name="x">The value to test.</param>
    /// <returns>True if the input value is an integer power of two; otherwise false.</returns>
    public static bool IsPowerOfTwo(int x)
    {
        return x > 0 && BitOperations.PopCount((uint)x) == 1;
    }

    /// <summary>
    /// Returns true if the input value is an integer power of two.
    /// </summary>
    /// <param name="x">The value to test.</param>
    /// <returns>True if the input value is an integer power of two; otherwise false.</returns>
    public static bool IsPowerOfTwo(long x)
    {
        return x > 0L && BitOperations.PopCount((ulong)x) == 1;
    }

    /// <summary>
    /// Calculates two to the power of the given integer exponent.
    /// </summary>
    /// <param name="exponent">The integer exponent (0 &lt;= exponent &lt; 31).</param>
    /// <returns>Two raised to the given power.</returns>
    public static int PowerOfTwo(int exponent)
    {
        if(exponent < 0 || exponent > 30)
            throw new ArgumentOutOfRangeException(nameof(exponent));

        return 1 << exponent;
    }

    /// <summary>
    /// Calculates two to the power of the given long integer exponent.
    /// </summary>
    /// <param name="exponent">The long integer exponent (0 &lt;= exponent &lt; 63).</param>
    /// <returns>Two raised to the given power.</returns>
    public static long PowerOfTwo(long exponent)
    {
        if(exponent < 0 || exponent > 62)
            throw new ArgumentOutOfRangeException(nameof(exponent));

        return 1L << (int)exponent;
    }

    /// <summary>
    /// Returns the smallest power of two that is greater than or equal to the specified number.
    /// </summary>
    /// <param name="x">The input value.</param>
    /// <returns>The smallest integral power of two that is greater than or equal to x.</returns>
    public static int CeilingToPowerOfTwo(int x)
    {
        // Test for max input value. There is one more high bit, but that is the sign bit.
        if(x < 0 || x > 0x4000_0000)
            throw new ArgumentOutOfRangeException(nameof(x));

        return x == 1 ? 1 : 1 << (32 - BitOperations.LeadingZeroCount((uint)(x-1)));
    }

    /// <summary>
    /// Returns the smallest power of two that is greater than or equal to the specified number.
    /// </summary>
    /// <param name="x">The input value.</param>
    /// <returns>The smallest integral power of two that is greater than or equal to x.</returns>
    public static long CeilingToPowerOfTwo(long x)
    {
        // Test for max input value. There is one more high bit, but that is the sign bit.
        if(x < 0 || x > 0x4000_0000_0000_0000)
            throw new ArgumentOutOfRangeException(nameof(x));

        return x == 1L ? 1L : 1L << (64 - BitOperations.LeadingZeroCount((ulong)(x-1)));
    }

    /// <summary>
    /// Evaluate the binary logarithm of a non-zero Int32, with rounding up of fractional results.
    /// I.e. returns the exponent of the smallest power of two that is greater than or equal to the specified number.
    /// </summary>
    /// <param name="x">The input value.</param>
    /// <returns>The exponent of the smallest integral power of two that is greater than or equal to x.</returns>
    public static int Log2Ceiling(uint x)
    {
        // Log2(x) gives the required power of two, however this is integer Log2() therefore any fractional
        // part in the result is truncated, i.e., the result may be 1 too low. To compensate we add 1 if x
        // is not an exact power of two.
        int exp = BitOperations.Log2(x);

        // Return (exp + 1) if x is non-zero, and not an exact power of two.
        if(BitOperations.PopCount(x) > 1)
            exp++;

        return exp;
    }

    /// <summary>
    /// Evaluate the binary logarithm of a non-zero Int32, with rounding up of fractional results.
    /// I.e. returns the exponent of the smallest power of two that is greater than or equal to the specified number.
    /// </summary>
    /// <param name="x">The input value.</param>
    /// <returns>The exponent of the smallest integral power of two that is greater than or equal to x.</returns>
    public static int Log2Ceiling(ulong x)
    {
        // Log2(x) gives the required power of two, however this is integer Log2() therefore any fractional
        // part in the result is truncated, i.e., the result may be 1 too low. To compensate we add 1 if x
        // is not an exact power of two.
        int exp = BitOperations.Log2(x);

        // Return (exp + 1) if x is non-zero, and not an exact power of two.
        if(BitOperations.PopCount(x) > 1)
            exp++;

        return exp;
    }
}
