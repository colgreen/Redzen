using System;
using System.Runtime.CompilerServices;

namespace Redzen.Random
{
    /// <summary>
    /// Base class for IRandomSource implementations based on an underlying 64 bit PRNG.
    /// </summary>
    public abstract class Random64Base : IRandomSource
    {
        // Constants.
        const double REAL_UNIT_UINT = 1.0 / (1UL << 53);
        const float REAL_UNIT_UINT_F = 1f / (1U << 24);

        #region Public Methods [Re-initialisation]

        /// <summary>
        /// Re-initialises the random number generator state using the provided seed value.
        /// </summary>
        public abstract void Reinitialise(ulong seed);

        #endregion

        #region Public Methods [System.Random functionally equivalent methods]

        /// <summary>
        /// Generates a random Int32 over the interval [0, int.MaxValue), i.e. exclusive of Int32.MaxValue.
        /// </summary>
        /// <remarks>
        /// MaxValue is excluded in order to remain functionally equivalent to System.Random.Next().
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
	    retry:
            // Handle the special case where the value int.MaxValue is generated; this is outside 
            // the range of permitted return values for this method. 
            ulong rtn = NextInnerULong() & 0x7fff_ffffUL;
            if(rtn == 0x7fff_ffffUL) {
                goto retry;
            }
	        return (int)rtn;
        }

        /// <summary>
        /// Generates a random Int32 over the interval [range 0 to upperBound), i.e. excluding upperBound.
        /// </summary>
        public int Next(int upperBound)
        {
            if(upperBound < 0) {
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >=0");
            }

            // ENHANCEMENT: Can we do this without converting to a double and back again?
            return (int)(NextDoubleInner() * upperBound);
        }

        /// <summary>
        /// Generates a random Int32 over the interval [lowerBound, upperBound), i.e. excluding upperBound.
        /// upperBound must be >= lowerBound. lowerBound may be negative.
        /// </summary>
        public int Next(int lowerBound, int upperBound)
        {
            if(lowerBound > upperBound) {
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >= lowerBound");
            }

            // Test if range will fit into an Int32.
            int range = upperBound - lowerBound;
            if(range >= 0) {
                return lowerBound + (int)(NextDoubleInner() * range);
            }

            // When range is less than 0 then an overflow has occurred and therefore we must resort to using long integer arithmetic (which is slower).
            return lowerBound + (int)(NextDoubleInner() * ((long)upperBound - (long)lowerBound));
        }

        /// <summary>
        /// Generates a random double over the interval [0, 1), i.e. inclusive of 0.0 and exclusive of 1.0.
        /// </summary>
        public double NextDouble()
        {
            return NextDoubleInner();
        }

        /// <summary>
        /// Fills the provided byte array with random bytes.
        /// </summary>
        /// <param name="buffer">The byte array to fill with random values.</param>
        public abstract void NextBytes(byte[] buffer);

        #endregion

        #region Public Methods [Methods not present on System.Random]

        /// <summary>
        /// Generates a random float over the interval [0, 1), i.e. inclusive of 0.0 and exclusive of 1.0.
        /// </summary>
        public float NextFloat()
        {
            // Note. Here we generate a random integer between 0 and 2^24-1 (i.e. 24 binary 1s) and multiply
            // by the fractional unit value 1.0 / 2^24, thus the result has a max value of
            // 1.0 - (1.0 / 2^24). Or 0.99999994 in decimal.
            return (NextInnerULong() >> 40) * REAL_UNIT_UINT_F;
        }

        /// <summary>
        /// Generates a random UInt32 over the interval [uint.MinValue, uint.MaxValue], i.e. over the full 
        /// range of a UInt32.
        /// </summary>
        public uint NextUInt()
        {
            return (uint)NextInnerULong();
        }

        /// <summary>
        /// Generates a random Int32 over interval [0 to Int32.MaxValue], i.e. inclusive of Int32.MaxValue.
        /// </summary>
        /// <remarks>
        /// This method can generate Int32.MaxValue, whereas Next() does not; this is the only difference
        /// between these two methods. As a sonsequenc ethis method wil ltypically be slightly faster because 
        /// Next () must test for Int32.MaxValue and resample the underlying RNG when that value occurs.
        /// </remarks>
        public int NextInt()
        {
            // Generate 64 random bits and shift right to leave the most signifcant 31 bits.
            // Bit 32 is the sign bit so muct be zero to avoid negative results.
            // Note. Shift right is used instead of a mask because the high significant bits 
            // exhibit high quality randomness compared to the lower bits (for xoroshiro128+).
            return (int)(NextInnerULong() >> 33);
        }

        /// <summary>
        /// Generates a random double over the interval (0, 1), i.e. exclusive of both 0.0 and 1.0
        /// </summary>
        public double NextDoubleNonZero()
        {
            // Here we generate a random value from 0 to 0x1f_ffff_ffff_fffe, and add one
            // to generate a random value from 1 to 0x1f_ffff_ffff_ffff.
            // We then multiple by the fractional unit 1.0 / 2^53.
            // Note. the bit shift right here may appear redundant, but the high significant bits 
            // have better randomness than the low bits, thus this approach is preferred.
            // Specifically, the low bits are linear-feedback shift registers (LFSRs) with low degree.
            return ((NextInnerULong() >> 11) & 0x1f_ffff_ffff_fffe) * REAL_UNIT_UINT;
        }

        /// <summary>
        /// Generates a single random bit.
        /// This method's performance is improved by generating 32 bits in one operation and storing them
        /// ready for future calls.
        /// </summary>
        public bool NextBool()
        {
            // Use a high bit since the low bits are linear-feedback shift registers (LFSRs) with low degree.
            // This is slower than the aproach of generating and caching 64 bits for future calls, but 
            // (A) gives good quality randomness, and (B) is still very fast.
            return (NextInnerULong() & 0x8000_0000_0000_0000) == 0;
        }

        /// <summary>
        /// Generates a single random byte over the interval [0,255].
        /// </summary>
        public byte NextByte()
        {
            // Note. Explicitly masking with 0xff is unnecessary, this is achieved by the cast.
            return (byte)NextInnerULong();
        }

        #endregion

        #region Private Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double NextDoubleInner()
        {
            // Note. Here we generate a random integer between 0 and 2^53-1 (i.e. 53 binary 1s) and multiply
            // by the fractional unit value 1.0 / 2^53, thus the result has a max value of
            // 1.0 - (1.0 / 2^53), or 0.99999999999999989 in decimal.
            return (NextInnerULong() >> 11) * REAL_UNIT_UINT;
        }

        protected abstract ulong NextInnerULong();

        #endregion
    }
}
