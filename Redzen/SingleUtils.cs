﻿/* ***************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015-2022 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
namespace Redzen;

/// <summary>
/// Static utility methods for <see cref="Single"/>.
/// </summary>
public static class SingleUtils
{
    /// <summary>
    /// Tests if a float has a non-negative real value.
    /// </summary>
    /// <param name="f">The value to test.</param>
    /// <returns>true if the value is a non-negative real value; otherwise false.</returns>
    public static bool IsNonNegativeReal(float f)
    {
        uint bits = BitConverter.SingleToUInt32Bits(f);

        // If all exponent bits are set then f is one of th especial values [PositiveInfinity, NegativeInfinity or NaN).
        // If the sign bit is set then f is negative, or NegativeInfinity.
        return !(((bits & 0x7f80_0000U) == 0x7f80_0000U) || ((bits & 0x8000_0000U) == 0x8000_0000U));
    }

    /// <summary>
    /// Tests if a float has a positive real value.
    /// </summary>
    /// <param name="f">The value to test.</param>
    /// <returns>true if the value is a non-negative real value; otherwise false.</returns>
    public static bool IsPositiveReal(float f)
    {
        uint bits = BitConverter.SingleToUInt32Bits(f);

        // If all exponent bits are set then d is one of th especial values [PositiveInfinity, NegativeInfinity or NaN).
        // If the sign bit is set then d is negative, or NegativeInfinity.
        // If any of the fraction bits are set, then the value cannot be zero.
        return !(((bits & 0x7f80_0000U) == 0x7f80_0000U) || ((bits & 0x8000_0000U) == 0x8000_0000U)) && f != 0f;
    }

    /// <summary>
    /// Tests if all of the values in a span have a non-negative real value.
    /// </summary>
    /// <param name="s">The span to test.</param>
    /// <returns>true if all of the span elements have a real non-negative value; otherwise false.</returns>
    public static bool AllNonNegativeReal(ReadOnlySpan<float> s)
    {
        for(int i=0; i < s.Length; i++)
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
    public static bool AllPositiveReal(ReadOnlySpan<float> s)
    {
        for(int i=0; i < s.Length; i++)
        {
            if(!IsPositiveReal(s[i]))
                return false;
        }

        return true;
    }
}
