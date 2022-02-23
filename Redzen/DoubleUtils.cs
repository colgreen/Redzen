using System;

namespace Redzen;

/// <summary>
/// Static utility methods for <see cref="System.Double"/>.
/// </summary>
public static class DoubleUtils
{
    /// <summary>
    /// Tests if a double has a non-negative real value.
    /// </summary>
    /// <param name="d">The value to test.</param>
    /// <returns>true if the value is a real non-negative value; otherwise false.</returns>
    public static bool IsNonNegativeReal(double d)
    {
        ulong bits = BitConverter.DoubleToUInt64Bits(d);

        // If all exponent bits are set then d is one of the special values [PositiveInfinity, NegativeInfinity or NaN).
        // If the sign bit is set then d is negative, or NegativeInfinity.
        return !(((bits & 0x7ff0_0000_0000_0000UL) == 0x7ff0_0000_0000_0000UL) || ((bits & 0x8000_0000_0000_0000UL) == 0x8000_0000_0000_0000UL));
    }

    /// <summary>
    /// Tests if a double has a positive real value.
    /// </summary>
    /// <param name="d">The value to test.</param>
    /// <returns>true if the value is a real non-negative value; otherwise false.</returns>
    public static bool IsPositiveReal(double d)
    {
        ulong bits = BitConverter.DoubleToUInt64Bits(d);

        // If all exponent bits are set then d is one of the special values [PositiveInfinity, NegativeInfinity or NaN).
        // If the sign bit is set then d is negative, or NegativeInfinity.
        // If any of the fraction bits are set, then the value cannot be zero.
        return !(((bits & 0x7ff0_0000_0000_0000UL) == 0x7ff0_0000_0000_0000UL) || ((bits & 0x8000_0000_0000_0000UL) == 0x8000_0000_0000_0000UL)) && d != 0.0;
    }

    /// <summary>
    /// Tests if all of the values in a span have a non-negative real value.
    /// </summary>
    /// <param name="s">The span to test.</param>
    /// <returns>true if all of the span elements have a real non-negative value; otherwise false.</returns>
    public static bool AllNonNegativeReal(ReadOnlySpan<double> s)
    {
        for(int i = 0; i < s.Length; i++)
        {
            if(!IsNonNegativeReal(s[i]))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Tests if all of the values in a span have a positive real value.
    /// </summary>
    /// <param name="s">The span to test.</param>
    /// <returns>true if all of the span elements have a positive real value; otherwise false.</returns>
    public static bool AllPositiveReal(ReadOnlySpan<double> s)
    {
        for(int i = 0; i < s.Length; i++)
        {
            if(!IsPositiveReal(s[i]))
                return false;
        }

        return true;
    }
}
