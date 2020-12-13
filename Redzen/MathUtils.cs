/* ***************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015-2020 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using System;
using System.Numerics;

namespace Redzen
{
    /// <summary>
    /// Math utility methods.
    /// </summary>
    public static class MathUtils
    {
        #region Public Static Methods

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
        /// Calculates two to the power of the given integer exponent.
        /// </summary>
        /// <param name="exponent">The integer exponent (0 &lt;= exponent &lt; 31)</param>
        public static int PowerOfTwo(int exponent)
        {
            if (exponent < 0 || exponent > 30) {
                throw new ArgumentOutOfRangeException("exponent");
            }
            return 1 << exponent;
        }

        /// <summary>
        /// Calculates two to the power of the given long integer exponent.
        /// </summary>
        /// <param name="exponent">The long integer exponent (0 &lt;= exponent &lt; 63)</param>
        public static long PowerOfTwo(long exponent)
        {
            if (exponent < 0 || exponent > 62) {
                throw new ArgumentOutOfRangeException("exponent");
            }
            return 1L << (int)exponent;
        }

        /// <summary>
        /// Returns the smallest power of two that is greater than or equal to the specified number.
        /// </summary>
        /// <returns>The smallest integral power of two that is greater than or equal to x.</returns>
        public static int CeilingToPowerOfTwo(int x)
        {
            // Test for max input value. There is one more high bit, but that is the sign bit.
            if (x < 0 || x > 0x4000_0000) {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            return x == 1 ? 1 : 1 << (int)(32u - BitOperations.LeadingZeroCount((uint)(x-1)));
        }

        /// <summary>
        /// Returns the smallest power of two that is greater than or equal to the specified number.
        /// </summary>
        /// <returns>The smallest integral power of two that is greater than or equal to x.</returns>
        public static long CeilingToPowerOfTwo(long x)
        {
            // Test for max input value. There is one more high bit, but that is the sign bit.
            if (x < 0 || x > 0x4000_0000_0000_0000){
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            return x == 1L ? 1L : 1L << (int)(64u - BitOperations.LeadingZeroCount((ulong)(x-1)));
        }

        /// <summary>
        /// Evaluate the binary logarithm of a non-zero Int32, with rounding up of fractional results.
        /// I.e. returns the exponent of the smallest power of two that is greater than or equal to the specified number.
        /// </summary>
        /// <returns>The exponent of the smallest integral power of two that is greater than or equal to x.</returns>
        public static int Log2Ceiling(uint x)
        {
            // Log2(x) gives the required power of two, however this is integer Log2() therefore any fractional
            // part in exp is truncated, i.e. the result may be 1 too low. Thus, if 2^exp == x, then x is an exact 
            // power of two and exp is correct, otherwise exp + 1 gives the correct value.
            int exp = BitOperations.Log2(x);

            // Calc x1 = 2^exp
            int x1 = 1 << exp;

            // Return exp + 1 if x is not an exact power of two.
            return (x == x1) ? exp : exp + 1;
        }

        /// <summary>
        /// Evaluate the binary logarithm of a non-zero Int32, with rounding up of fractional results.
        /// I.e. returns the exponent of the smallest power of two that is greater than or equal to the specified number.
        /// </summary>
        /// <returns>The exponent of the smallest integral power of two that is greater than or equal to x.</returns>
        public static int Log2Ceiling(ulong x)
        {
            // Log2(x) gives the required power of two, however this is integer Log2() therefore any fractional
            // part in exp is truncated, i.e. the result may be 1 too low. Thus, if 2^exp == x, then x is an exact 
            // power of two and exp is correct, otherwise exp+1 gives the correct value.
            int exp = BitOperations.Log2(x);

            // Calc x1 = 2^exp
            ulong x1 = 1UL << exp;

            // Return exp + 1 if x is not an exact power of two.
            return (x == x1) ? exp : exp + 1;
        }

        #endregion
    }
}
