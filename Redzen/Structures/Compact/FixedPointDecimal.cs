/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2019 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */
using System;

namespace Redzen.Structures.Compact
{
    /// <summary>
    /// A fixed point decimal data type that has the following qualities:
    /// 
    /// 1) Uses a Int32 to hold the value (4 bytes versus the native decimal's 16 bytes).
    /// 2) Defines a fixed number of six digits after the decimal place. 
    /// 3) Allows a null value to be represented, requiring a single bit to allocated for this.
    /// 
    /// The range of FixedPointDecimal is -1,073.741823 to 1,073.741823. This can therefore 
    /// represent all possible values in the SQL data type decimal(9,6).
    ///
    /// The range takes into account the null bit and the fixed four digits after the decimal
    /// place.
    ///
    /// Fixed point maths also has the benefit of allowing for far simpler/faster comparison.
    /// </summary>
    public readonly struct FixedPointDecimal
    {
        #region Static Fields

        const string __RangeScaleExceptionMsg = "decimal is outside the range and/or scale of a FixedPointDecimal.";

        /// <summary>
        /// Public static instance of a null FixedPointDecimal.
        /// </summary>
        public static FixedPointDecimal Null = new FixedPointDecimal(null);

        #endregion

        #region Instance Fields

        /// <summary>
        /// Bit 31; 'Has value' flag.
        /// Bit 30; Sign bit (0 = positive)
        /// Bits 29 to 0; Significand with range 0 to 2^30-1.
        /// </summary>
        readonly UInt32 _data;

        #endregion

        #region Constructor

        /// <summary>
        /// Construct FixedPointDecimal from the provided nullable decimal.
        /// </summary>
        /// <param name="val">The value to convert.</param>
        public FixedPointDecimal(decimal? val)
        {
            // Test for null.
            if(!val.HasValue)
            {   // Unset hasvalue bit.
                _data = 0;
                return;
            }

            // Get underlying bits
            int[] bits = decimal.GetBits(val.Value);

            // Check high significand bytes .
            if(0 != bits[1] || 0 != bits[2]) {
                throw new Exception(__RangeScaleExceptionMsg);
            }

            // Read significand.
            uint significand = (uint)bits[0];

            // Read exponent and sign bit.
            uint tmp = (uint)bits[3];

            // Get exponent.
            uint exponent = (tmp & 0x00FF0000u) >> 16;

            // Check the value is within range and precision of a FixedDecimal;
            // Rescale the significand based on the value having 4 decimal places.
            switch(exponent)
            {
                case 0:
                    if(significand > 1073u) {
                        throw new Exception(__RangeScaleExceptionMsg);
                    }
                    significand *= 1000000u;
                    break;
                case 1:
                    if(significand > 10737u) {
                        throw new Exception(__RangeScaleExceptionMsg);
                    }
                    significand *= 100000u;
                    break;
                case 2:
                    if(significand > 107374u) {
                        throw new Exception(__RangeScaleExceptionMsg);
                    }
                    significand *= 10000u;
                    break;
                case 3:
                    if(significand > 1073741u) {
                        throw new Exception(__RangeScaleExceptionMsg);
                    }
                    significand *= 1000u;
                    break;
                case 4:
                    if(significand > 10737418u) {
                        throw new Exception(__RangeScaleExceptionMsg);
                    }
                    significand *= 100u;
                    break;
                case 5:
                    if(significand > 107374182u) {
                        throw new Exception(__RangeScaleExceptionMsg);
                    }
                    significand *= 10u;
                    break;
                case 6:
                    if(significand > 1073741823u) {
                        throw new Exception(__RangeScaleExceptionMsg);
                    }
                    break;
                default:
                    throw new Exception(__RangeScaleExceptionMsg);
            }
            
            // Store significand and HasValue bit.
            _data = 0x80000000 | significand;

            // Store sign bit (we explicitly convert -0 to +0, although decimal already does this).
            if((tmp & 0x80000000) != 0 && significand !=0) {
                _data |= 0x40000000;
            }
        }

        private FixedPointDecimal(uint significand, bool isNegative)
        {
            if(significand > 1073741823u) {
                throw new Exception(__RangeScaleExceptionMsg);
            }

            // Store significand and HasValue bit.
            _data = 0x80000000 | significand;

            // Store sign bit (we explicitly convert -0 to +0).
            if(isNegative && significand !=0) {
                _data |= 0x40000000;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value as a native decimal data type.
        /// </summary>
        public decimal? Value
        {
            get
            {
                // Test HasValue bit.
                if((_data & 0x80000000u) == 0) {
                    return null;
                }

                // Extract significand.
                uint significand = _data & 0x3FFFFFFFu;

                // Extract sign bit.
                bool isNegative = (_data & 0x40000000u) != 0u;

                // Reconstruct internal representation bits for System.Decimal.
                Decimal val = new Decimal((int)significand, 0, 0, isNegative, 6);
                return val;
            }
        }

        /// <summary>
        /// Gets a boolean that indicates if the FixedPointDecimal contains a value.
        /// </summary>
        public bool HasValue
        {
            get 
            {   // Test HasValue bit.
                return (_data & 0x80000000u) != 0; 
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the hash code for the object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() 
        {
            return (int)_data;
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the current Object.
        /// </summary>
        /// <param name="value">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object value) 
        {
            if(!(value is FixedPointDecimal)) {
                return false;
            }

            return Equals((FixedPointDecimal)value, this);
        }

        #endregion

        #region Operators
        
        /// <summary>
        /// Determines if two <see cref="FixedPointDecimal"/> objects are not equal.
        /// </summary>
        /// <param name="d1">The first <see cref="FixedPointDecimal"/> to compare.</param>
        /// <param name="d2">The second <see cref="FixedPointDecimal"/> to compare.</param>
        /// <returns><c>true</c> if the objects are not equal, <c>false</c> otherwise.</returns>
        public static bool operator != (in FixedPointDecimal d1, in FixedPointDecimal d2) 
        {
          return !Equals (d1, d2);
        }

        /// <summary>
        /// Determines if two <see cref="FixedPointDecimal"/> objects are equal.
        /// </summary>
        /// <param name="d1">The first <see cref="FixedPointDecimal"/> to compare.</param>
        /// <param name="d2">The second <see cref="FixedPointDecimal"/> to compare.</param>
        /// <returns><c>true</c> if the objects are equal, <c>false</c> otherwise.</returns>
        public static bool operator == (in FixedPointDecimal d1, in FixedPointDecimal d2) 
        {
          return Equals (d1, d2);
        }

        /// <summary>
        /// Determines if the value of one <see cref="FixedPointDecimal"/> object is greater than the value of another.
        /// </summary>
        /// <param name="d1">The first <see cref="FixedPointDecimal"/> to compare.</param>
        /// <param name="d2">The second <see cref="FixedPointDecimal"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="d1"/> is greater than the value of <paramref name="d2"/>, <c>false</c> otherwise.</returns>
        public static bool operator > (in FixedPointDecimal d1, in FixedPointDecimal d2) 
        {
          return Compare (d1, d2) > 0;
        }

        /// <summary>
        /// Determines if the value of one <see cref="FixedPointDecimal"/> object is greater than or equal to the value of another.
        /// </summary>
        /// <param name="d1">The first <see cref="FixedPointDecimal"/> to compare.</param>
        /// <param name="d2">The second <see cref="FixedPointDecimal"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="d1"/> is greater than or equal to the value of <paramref name="d2"/>, <c>false</c> otherwise.</returns>
        public static bool operator >= (in FixedPointDecimal d1, in FixedPointDecimal d2) 
        {
          return Compare (d1, d2) >= 0;
        }

        /// <summary>
        /// Determines if the value of one <see cref="FixedPointDecimal"/> object is less than the value of another.
        /// </summary>
        /// <param name="d1">The first <see cref="FixedPointDecimal"/> to compare.</param>
        /// <param name="d2">The second <see cref="FixedPointDecimal"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="d1"/> is less than the value of <paramref name="d2"/>, <c>false</c> otherwise.</returns>
        public static bool operator < (in FixedPointDecimal d1, in FixedPointDecimal d2) 
        {
          return Compare (d1, d2) < 0;
        }

        /// <summary>
        /// Determines if the value of one <see cref="FixedPointDecimal"/> object is less than or equal to the value of another.
        /// </summary>
        /// <param name="d1">The first <see cref="FixedPointDecimal"/> to compare.</param>
        /// <param name="d2">The second <see cref="FixedPointDecimal"/> to compare.</param>
        /// <returns><c>true</c> if the value of <paramref name="d1"/> is less than or equal to the value of <paramref name="d2"/>, <c>false</c> otherwise.</returns>
        public static bool operator <= (in FixedPointDecimal d1, in FixedPointDecimal d2) 
        {
          return Compare (d1, d2) <= 0;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Static factory method to create a FixedDecimal.
        /// </summary>
        /// <param name="d">The value to create the object from.</param>
        /// <returns>The newly created <see cref="FixedPointDecimal"/> instance.</returns>
        public static FixedPointDecimal Create(decimal? d)
        {
            return new FixedPointDecimal(d);
        }

        /// <summary>
        /// Converts the string representation of a number to its FixedPointDecimal equivalent.
        /// A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">The value to parse.</param>
        /// <param name="result">Upon success contains the <see cref="FixedPointDecimal"/> equivalent of the value of <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> could be parsed into a <see cref="FixedPointDecimal"/> instance.</returns>
        public static bool TryParse(string s, out FixedPointDecimal result)
        {
            return InternalTryParse(s, out result, false);
        }

        /// <summary>
        /// Converts the string representation of a number to its FixedPointDecimal equivalent.
        /// A return value indicates whether the conversion succeeded or failed.
        /// Values outside the range of a FixedPointDecimal are truncated to the min or max values
        /// for FixedPointDecimal as appropriate. Input values with more than four decimal places
        /// have their precision truncated to four decimal places.
        /// </summary>
        /// <param name="s">The value to parse.</param>
        /// <param name="result">Upon success contains the <see cref="FixedPointDecimal"/> equivalent of the value of <paramref name="s"/>.</param>
        /// <returns><c>true</c> if <paramref name="s"/> could be parsed into a <see cref="FixedPointDecimal"/> instance.</returns>
        public static bool TryParseTruncate(string s, out FixedPointDecimal result)
        {
            return InternalTryParse(s, out result, true);
        }

        private static bool InternalTryParse(string s, out FixedPointDecimal result, bool truncateRange)
        {
            // Clean up string. Do quick null/empty test.
            if(null != s) {
                s = s.Trim();
            }

            if(string.IsNullOrEmpty(s))
            {
                result = FixedPointDecimal.Null;
                return false;
            }

            // Split string on optional decimal point.
            string[] parts = s.Split('.');
            if(parts.Length > 2)
            {
                result = FixedPointDecimal.Null;
                return false;
            }

            // Test for negative sign.
            bool isNegative;
            if('-' == parts[0][0])
            {
                isNegative = true;
                parts[0] = parts[0].Substring(1);
                if(string.IsNullOrEmpty(parts[0]))
                {
                    result = FixedPointDecimal.Null;
                    return false;
                }
            }
            else
            {
                isNegative = false;
            }

            // Check fractional part is no longer than 6 digits.
            if(parts.Length == 2 && parts[1].Length > 6)
            { 
                if(truncateRange)
                {
                    parts[1] = parts[1].Substring(0, 6);
                }
                else
                {
                    result = FixedPointDecimal.Null;
                    return false;
                }
            }

            // Join integer and fractional parts; padding fractional part to 6 digits if necessary.
            // Parse resulting significand string as integer.
            string significandStr = parts[0] + (parts.Length == 2 ? parts[1].PadRight(6, '0') : "000000");
            
            if(!uint.TryParse(significandStr, out uint significand))
            {
                result = FixedPointDecimal.Null;
                return false;
            }

            // Test significand is within range of FixedPointDecimal.
            if(significand > 1073741823u)
            {
                if(truncateRange)
                {
                    significand = 1073741823u;
                }
                else
                {
                    result = FixedPointDecimal.Null;
                    return false;
                }
            }

            result = new FixedPointDecimal(significand, isNegative);
            return true;
        }

        /// <summary>
        /// Determines whether the specified FixedPointDecimal is equal to the current FixedPointDecimal.
        /// </summary>
        /// <param name="d1">The first <see cref="FixedPointDecimal"/> to compare.</param>
        /// <param name="d2">The second <see cref="FixedPointDecimal"/> to compare.</param>
        /// <returns><c>true</c> if the objects are equal, <c>false</c> otherwise.</returns>
        public static bool Equals(in FixedPointDecimal d1, in FixedPointDecimal d2) 
        {
            // We can calculate value equality by testing bitwise equality. This is possible because we
            // are using a fixed point representation *and* we convert -0 to +0 (the only other possible ambiguity).
            return d1._data == d2._data;
        }

        /// <summary>
        /// Compares two specified FixedPointDecimal values.
        /// </summary>
        /// <param name="d1">The first <see cref="FixedPointDecimal"/> to compare.</param>
        /// <param name="d2">The second <see cref="FixedPointDecimal"/> to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="d1"/> and <paramref name="d2"/>.</returns>
        public static int Compare(in FixedPointDecimal d1, in FixedPointDecimal d2)
        {
            // Test for null values.
            if((d1._data & 0x80000000) == 0 && (d2._data & 0x80000000) == 0)
            {   // Both values are null;
                return 0;
            }

            if((d1._data & 0x80000000) == 0)
            {   // d1 is null.
                return -1;
            }

            if((d2._data & 0x80000000) == 0)
            {   // d2 is null.
                return 1;
            }

            // Both values are non null. Extract significands to signed int (and apply sign bit).
            int s1 = (int)(d1._data & 0x3FFFFFFFu | ((d1._data & 0x40000000) << 1));
            int s2 = (int)(d2._data & 0x3FFFFFFFu | ((d2._data & 0x40000000) << 1));

            // Compare signed significands.
            if(s1 > s2) {
                return 1;
            }
            else if(s1 < s2) {
                return -1;
            }
            return 0;
        }

        #endregion
    }
}
