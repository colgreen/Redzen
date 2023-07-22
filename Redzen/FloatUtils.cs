using System.Numerics;

namespace Redzen;

/// <summary>
/// Static utility methods for <see cref="IBinaryFloatingPointIeee754{T}"/> types.
/// </summary>
public static class FloatUtils
{
    /// <summary>
    /// Tests if a value has a non-negative real value.
    /// </summary>
    /// <typeparam name="T">The floating point value type.</typeparam>
    /// <param name="x">The value to test.</param>
    /// <returns>true if the value is a real non-negative value; otherwise false.</returns>
    public static bool IsNonNegativeReal<T>(T x)
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        if(typeof(T) == typeof(double))
        {
            ulong bits = BitConverter.DoubleToUInt64Bits(double.CreateChecked(x));

            // If all exponent bits are set then x is one of the special values [PositiveInfinity, NegativeInfinity or NaN).
            // If the sign bit is set then x is negative, or NegativeInfinity.
            return !(((bits & 0x7ff0_0000_0000_0000UL) == 0x7ff0_0000_0000_0000UL) || ((bits & 0x8000_0000_0000_0000UL) == 0x8000_0000_0000_0000UL));
        }
        else if(typeof(T) == typeof(float))
        {
            uint bits = BitConverter.SingleToUInt32Bits(float.CreateChecked(x));

            // If all exponent bits are set then x is one of the special values [PositiveInfinity, NegativeInfinity or NaN).
            // If the sign bit is set then x is negative, or NegativeInfinity.
            return !(((bits & 0x7f80_0000U) == 0x7f80_0000U) || ((bits & 0x8000_0000U) == 0x8000_0000U));
        }
        else if(typeof(T) == typeof(Half))
        {
            ushort bits = BitConverter.HalfToUInt16Bits(Half.CreateChecked(x));

            // If all exponent bits are set then x is one of the special values [PositiveInfinity, NegativeInfinity or NaN).
            // If the sign bit is set then x is negative, or NegativeInfinity.
            return !(((bits & 0x7c00U) == 0x7c00U) || ((bits & 0x8000U) == 0x8000U));
        }

        throw new NotSupportedException("Unsupported type");
    }

    /// <summary>
    /// Tests if a value has a positive real value.
    /// </summary>
    /// <typeparam name="T">The floating point value type.</typeparam>
    /// <param name="x">The value to test.</param>
    /// <returns>true if the value is a non-negative real value; otherwise false.</returns>
    public static bool IsPositiveReal<T>(T x)
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        if(typeof(T) == typeof(double))
        {
            double d = double.CreateChecked(x);
            ulong bits = BitConverter.DoubleToUInt64Bits(d);

            // If all exponent bits are set then d is one of the special values [PositiveInfinity, NegativeInfinity or NaN).
            // If the sign bit is set then d is negative, or NegativeInfinity.
            // If any of the fraction bits are set, then the value cannot be zero.
            return !(((bits & 0x7ff0_0000_0000_0000UL) == 0x7ff0_0000_0000_0000UL) || ((bits & 0x8000_0000_0000_0000UL) == 0x8000_0000_0000_0000UL)) && d != 0.0;
        }
        else if(typeof(T) == typeof(float))
        {
            float f = float.CreateChecked(x);
            uint bits = BitConverter.SingleToUInt32Bits(f);

            // If all exponent bits are set then f is one of the special values [PositiveInfinity, NegativeInfinity or NaN).
            // If the sign bit is set then f is negative, or NegativeInfinity.
            // If any of the fraction bits are set, then the value cannot be zero.
            return !(((bits & 0x7f80_0000U) == 0x7f80_0000U) || ((bits & 0x8000_0000U) == 0x8000_0000U)) && f != 0f;
        }
        else if(typeof(T) == typeof(Half))
        {
            Half h = Half.CreateChecked(x);
            ushort bits = BitConverter.HalfToUInt16Bits(h);

            // If all exponent bits are set then h is one of the special values [PositiveInfinity, NegativeInfinity or NaN).
            // If the sign bit is set then h is negative, or NegativeInfinity.
            // If any of the fraction bits are set, then the value cannot be zero.
            return !(((bits & 0x7c00U) == 0x7c00U) || ((bits & 0x8000U) == 0x8000U)) && h != Half.Zero;
        }

        throw new NotSupportedException("Unsupported type");
    }

    /// <summary>
    /// Tests if all of the values in a span have a non-negative real value.
    /// </summary>
    /// <typeparam name="T">The floating point value type.</typeparam>
    /// <param name="s">The span to test.</param>
    /// <returns>true if all of the span elements are non-negative real values; otherwise false.</returns>
    public static bool AllNonNegativeReal<T>(ReadOnlySpan<T> s)
        where T : struct, IBinaryFloatingPointIeee754<T>
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
    /// <typeparam name="T">The floating point value type.</typeparam>
    /// <param name="s">The span to test.</param>
    /// <returns>true if all of the span elements have a positive real value; otherwise false.</returns>
    public static bool AllPositiveReal<T>(ReadOnlySpan<T> s)
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        for(int i=0; i < s.Length; i++)
        {
            if(!IsPositiveReal(s[i]))
                return false;
        }

        return true;
    }
}
