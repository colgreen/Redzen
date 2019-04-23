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
using System.Runtime.CompilerServices;

namespace Redzen.Random
{
    public abstract class RandomSourceBase
    {
        // Constants.
        const double INCR_DOUBLE = 1.0 / (1UL << 53);
        const float INCR_FLOAT = 1f / (1U << 24);

        #region Public Methods [System.Random functionally equivalent methods]

        /// <summary>
        /// Generate a random Int32 over the interval [0, Int32.MaxValue), i.e. exclusive of Int32.MaxValue.
        /// </summary>
        /// <remarks>
        /// Int32.MaxValue is excluded in order to be functionally equivalent with System.Random.Next().
        /// 
        /// For slightly improved performance consider these alternatives:
        /// 
        ///  * NextInt() returns an Int32 over the interval [0 to Int32.MaxValue], i.e. inclusive of Int32.MaxValue.
        /// 
        ///  * NextUInt(). Cast the result to an Int32 to generate an value over the full range of an Int32,
        ///    including negative values.
        /// </remarks>
        public int Next()
        {
            // Perform rejection sampling to handle the special case where the value int.MaxValue is generated;
            // this value is outside the range of permitted values for this method. 
            // Rejection sampling ensures we produce an unbiased sample.
            ulong rtn;
            do 
            {
                rtn = NextULongInner() & 0x7fff_ffffUL;
            }
            while(rtn == 0x7fff_ffffUL);

            return (int)rtn;
        }

        /// <summary>
        /// Generate a random Int32 over the interval [0 to maxValue), i.e. excluding maxValue.
        /// </summary>
        public int Next(int maxValue)
        {
            if (maxValue < 1) {
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "maxValue must be > 0");
            }

            if(1 == maxValue) {
                return 0;
            }

            return NextInner(maxValue);
        }

        /// <summary>
        /// Generate a random Int32 over the interval [minValue, maxValue), i.e. excluding maxValue.
        /// maxValue must be >= minValue. minValue may be negative.
        /// </summary>
        public int Next(int minValue, int maxValue)
        {
            if (minValue >= maxValue) {
                throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "maxValue must be > minValue");
            }

            long range = (long)maxValue - minValue;
            if (range <= int.MaxValue) {
                return NextInner((int)range) + minValue;
            }

            // Call NextInner(long); i.e. the range is greater than int.MaxValue.
            return (int)(NextInner(range) + minValue);
        }

        /// <summary>
        /// Generate a random double over the interval [0, 1), i.e. inclusive of 0.0 and exclusive of 1.0.
        /// </summary>
        public double NextDouble()
        {
            return NextDoubleInner();
        }

        /// <summary>
        /// Generate a random double over the interval [0, 1), i.e. inclusive of 0.0 and exclusive of 1.0.
        /// </summary>
        /// <remarks>
        /// Uses an alternative sampling method that is capable of generating all possible values in the
        /// interval [0,1) that can be represented by a double precision float. Note however that this method 
        /// is significantly slower than NextDouble().
        /// </remarks>
        public double NextDoubleHighRes()
        {   
            // Notes.
            // An alternative sampling method from:
            // 
            //    2014, Taylor R Campbell
            //
            //    Uniform random floats:  How to generate a double-precision
            //    floating-point number in [0, 1] uniformly at random given a uniform
            //    random source of bits.
            //
            //    https://mumble.net/~campbell/tmp/random_real.c
            //
            // The basic idea is that we generate a string of binary digits and use them to construct a 
            // base two number of the form:
            //
            //    0.{digits}
            //
            // The digits are generated in blocks of 64 bits. If all 64 bits in a block are zero then a 
            // running exponent value is reduced by 64 and another 64 bits are generated. This process is
            // repeated until a block with non-zero bits is produced, or the exponent value falls below -1074.
            //
            // The final step is to create the IEE754 double precision variable from a 64 bit significand
            // (the most recent and thus significant 64 bits), and the running exponent.
            //
            // This scheme is capable of generating all possible values in the interval [0,1) that can be 
            // represented by a double precision float, and without bias. There are a little under 2^62
            // possible discrete values, and this compares to the 2^53 possible values than can be generated by 
            // NextDouble(), however the scheme used in this method is much slower, so is likely of interest
            // in specialist scenarios.

            int exponent = -64;
            ulong significand;
            int shift;

            // Read zeros into the exponent until we hit a one; the rest
            // will go into the significand.
            while ((significand = NextULongInner()) == 0)
            {
                exponent -= 64;

                // If the exponent falls below -1074 = emin + 1 - p,
                // the exponent of the smallest subnormal, we are
                // guaranteed the result will be rounded to zero.  This
                // case is so unlikely it will happen in realistic
                // terms only if random64 is broken.
                if (exponent < -1074)
                    return 0;
            }

            // There is a 1 somewhere in significand, not necessarily in
            // the most significant position.  If there are leading zeros,
            // shift them into the exponent and refill the less-significant
            // bits of the significand.  Can't predict one way or another
            // whether there are leading zeros: there's a fifty-fifty
            // chance, if random64 is uniformly distributed.
            shift = MathUtils.LeadingZeroCount(significand);
            if (shift != 0)
            {
                exponent -= shift;
                significand <<= shift;
                significand |= (NextULongInner() >> (64 - shift));
            }

	        // Set the sticky bit, since there is almost surely another 1
	        // in the bit stream.  Otherwise, we might round what looks
	        // like a tie to even when, almost surely, were we to look
	        // further in the bit stream, there would be a 1 breaking the
	        // tie.
            significand |= 1;

	        // Finally, convert to double (rounding) and scale by
	        // 2^exponent.
            return (double)significand * Math.Pow(2, exponent);
        }

        // ENHANCEMENT: NextBytes(Span<byte>)
        /// <summary>
        /// Fills the provided byte array with random bytes.
        /// </summary>
        /// <param name="buffer">The byte array to fill with random values.</param>
        public abstract void NextBytes(byte[] buffer);

        #endregion

        #region Public Methods [Methods not present on System.Random]

        /// <summary>
        /// Generate a random float over the interval [0, 1), i.e. inclusive of 0.0 and exclusive of 1.0.
        /// </summary>
        public float NextFloat()
        {
            // Note. Here we generate a random integer between 0 and 2^24-1 (i.e. 24 binary 1s) and multiply
            // by the fractional unit value 1.0 / 2^24, thus the result has a max value of
            // 1.0 - (1.0 / 2^24). Or 0.99999994 in decimal.
            return (NextULongInner() >> 40) * INCR_FLOAT;
        }

        /// <summary>
        /// Generate a random UInt32 over the interval [0, 2^32-1], i.e. over the full range of a UInt32.
        /// </summary>
        public uint NextUInt()
        {
            return (uint)NextULongInner();
        }

        /// <summary>
        /// Generate a random Int32 over interval [0 to 2^31-1], i.e. inclusive of Int32.MaxValue and therefore 
        /// over the full range of non-negative Int32(s).
        /// </summary>
        /// <remarks>
        /// This method can generate Int32.MaxValue, whereas Next() does not; this is the only difference
        /// between these two methods. As a consequence this method will typically be slightly faster because 
        /// Next() must test for Int32.MaxValue and resample the underlying RNG when that value occurs.
        /// </remarks>
        public int NextInt()
        {
            // Generate 64 random bits and shift right to leave the most significant 31 bits.
            // Bit 32 is the sign bit so must be zero to avoid negative results.
            // Note. Shift right is used instead of a mask because the high significant bits 
            // exhibit higher quality randomness compared to the lower bits.
            return (int)(NextULongInner() >> 33);
        }

        /// <summary>
        /// Generate a random UInt64 over the interval [0, 2^64-1], i.e. over the full range of a UInt64.
        /// </summary>
        public ulong NextULong()
        {
            return NextULongInner();
        }

        /// <summary>
        /// Generate a random double over the interval (0, 1), i.e. exclusive of both 0.0 and 1.0
        /// </summary>
        public double NextDoubleNonZero()
        {
            // Here we generate a random value in the interval [0, 0x1f_ffff_ffff_fffe], and add one
            // to generate a random value in the interval [1, 0x1f_ffff_ffff_ffff].
            //
            // We then multiply by the fractional unit 1.0 / 2^53 to obtain a floating point value 
            // in the interval [ 1/(2^53-1) , 1.0].
            //
            // Note. the bit shift right here may appear redundant, however, the high significance
            // bits have better randomness than the low bits, thus this approach is preferred.
            return ((NextULongInner() >> 11) & 0x1f_ffff_ffff_fffe) * INCR_DOUBLE;
        }

        /// <summary>
        /// Generate a single random bit.
        /// </summary>
        public bool NextBool()
        {
            // Use a high bit since the low bits are linear-feedback shift registers (LFSRs) with low degree.
            // This is slower than the approach of generating and caching 64 bits for future calls, but 
            // (A) gives good quality randomness, and (B) is still very fast.
            return (NextULongInner() & 0x8000_0000_0000_0000) == 0;
        }

        /// <summary>
        /// Generate a single random byte over the interval [0,255].
        /// </summary>
        public byte NextByte()
        {
            // Note. Here we shift right to use the 8 most significant bits because these exhibit higher quality
            // randomness than the lower bits.
            return (byte)(NextULongInner() >> 56);
        }

        #endregion

        #region Private Methods

        private int NextInner(int maxValue)
        {
            if(1 == maxValue) {
                return 0;
            }

            // Notes.
            // Here we sample an integer value within the interval [0, maxValue). Rejection sampling is used in 
            // order to produce unbiased samples. An alternative approach is:
            //
            //  return (int)(NextDoubleInner() * maxValue);
            //
            // I.e. generate a double precision float in the interval [0,1) and multiply by maxValue. However the
            // use of floating point arithmetic will introduce bias therefore this method is not used.
            //
            // The rejection sampling method used here operates as follows:
            //
            //  1) Calculate N such that  2^(N-1) < maxValue <= 2^N, i.e. N is the minimum number of bits required
            //     to represent maxValue states.
            //  2) Generate an N bit random sample.
            //  3) Reject samples that are >= maxValue, and goto (2) to resample.
            //
            // Repeat until a valid sample is generated.

            // Log2Ceiling(numberOfStates) gives the number of bits required to represent maxValue states.
            int bitCount = MathUtils.Log2Ceiling((uint)maxValue);

            // Rejection sampling loop.
            // Note. The expected number of samples per generated value is approx. 1.3862,
            // i.e. the number of loops, on average, assuming a random and uniformly distributed maxValue.
            int x;
            do
            { 
                x = (int)(NextULongInner() >> (64 - bitCount));
            }
            while(x >= maxValue);

            return x;
        }

        private long NextInner(long maxValue)
        {
            if(1 == maxValue) {
                return 0;
            }

            // See comments on NextInner(int).

            // Log2Ceiling(numberOfStates) gives the number of bits required to represent maxValue states.
            int bitCount = MathUtils.Log2Ceiling((ulong)maxValue);

            // Rejection sampling loop.
            long x;
            do
            { 
                x = (long)(NextULongInner() >> (64 - bitCount));
            }
            while(x >= maxValue);

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double NextDoubleInner()
        {
            // Notes. 
            // Here we generate a random integer in the interval [0, 2^53-1]  (i.e. the max value is 53 binary 1s),
            // and multiply by the fractional value 1.0 / 2^53, thus the result has a min value of 0.0 and a max value of 
            // 1.0 - (1.0 / 2^53), or 0.99999999999999989 in decimal.
            //
            // I.e. we break the interval [0,1) into 2^53 uniformly distributed discrete values, and thus the interval between
            // two adjacent values is 1.0 / 2^53. This increment is chosen because it is the smallest value at which each 
            // distinct value in the full range (from 0.0 to 1.0 exclusive) can be represented directly by a double precision
            // float, and thus no rounding occurs in the representation of these values, which in turn ensures no bias in the 
            // random samples.
            return (NextULongInner() >> 11) * INCR_DOUBLE;
        }

        #endregion

        #region Abstract Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract ulong NextULongInner();

        #endregion
    }
}
