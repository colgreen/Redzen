// A C# port of the xorshiro256** pseudo random number generator (PRNG).
// Original C source code was obtained from:
//    http://xoshiro.di.unimi.it/xoshiro256starstar.c
//
// See original headers below for more info.
//
// ===========================================================================
//
// Written in 2018 by David Blackman and Sebastiano Vigna (vigna@acm.org)
//
// To the extent possible under law, the author has dedicated all copyright
// and related and neighboring rights to this software to the public domain
// worldwide. This software is distributed without any warranty.

// See <http://creativecommons.org/publicdomain/zero/1.0/>. */
//
// --------
//
// This is xoshiro256** 1.0, our all-purpose, rock-solid generator. It has
// excellent (sub-ns) speed, a state space (256 bits) that is large enough
// for any parallel application, and it passes all tests we are aware of.
//
// For generating just floating-point numbers, xoshiro256+ is even faster.
//
// The state must be seeded so that it is not everywhere zero. If you have
// a 64-bit seed, we suggest to seed a splitmix64 generator and use its
// output to fill s. 

using System;
using System.Runtime.CompilerServices;

namespace Redzen.Random
{
    /// <summary>
    /// Xoshiro256** (xor, shift, rotate) pseudo random number generator (PRNG).
    /// </summary>
    public sealed class Xoshiro256StarStarRandom : IRandomSource
    {
        // Constants.
        const double REAL_UNIT_UINT = 1.0 / (1UL << 53);
        const float REAL_UNIT_UINT_F = 1f / (1U << 24);

        // RNG state.
        ulong _s0;
        ulong _s1;
        ulong _s2;
        ulong _s3;

        #region Constructors

        /// <summary>
        /// Initialises a new instance using a seed generated from the class's static seed RNG.
        /// </summary>
        public Xoshiro256StarStarRandom()
        {
            Reinitialise(RandomSourceFactory.GetNextSeed());
        }

        /// <summary>
        /// Initialises a new instance using the provided ulong seed.
        /// </summary>
        public Xoshiro256StarStarRandom(ulong seed)
        {
            Reinitialise(seed);
        }

        #endregion

        #region Public Methods [Re-initialisation]

        /// <summary>
        /// Re-initialises the random number generator state using the provided seed value.
        /// </summary>
        public void Reinitialise(ulong seed)
        {
            // Notes.
            // The first random sample will be very strongly correlated to the value we give to the 
            // state variables here; such a correlation is undesirable, therefore we significantly 
            // weaken it by hashing the seed's bits using the splitmix64 PRNG.
            //
            // It is required that at least one of the state variables be non-zero;
            // use of splitmix64 satisfies this requirement because it is an equidistributed generator,
            // thus if it outputs a zero it will next produce a zero after a further 2^64 outputs.

            // Use the splitmix64 RNG to hash the seed.
            _s0 = Splitmix64Rng.Next(ref seed);
            _s1 = Splitmix64Rng.Next(ref seed);
            _s2 = Splitmix64Rng.Next(ref seed);
            _s3 = Splitmix64Rng.Next(ref seed);
        }

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
            // Perform rejection sampling to handle the special case where the value int.MaxValue is generated,
            // this is outside the range of permitted return values for this method. 
        retry:
            ulong rtn = NextInnerULong() & 0x7fff_ffffUL;
            if (rtn == 0x7fff_ffffUL) {
                goto retry;
            }
            return (int)rtn;
        }

        /// <summary>
        /// Generates a random Int32 over the interval [range 0 to upperBound), i.e. excluding upperBound.
        /// </summary>
        public int Next(int upperBound)
        {
            if (upperBound < 0) {
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
            if (lowerBound > upperBound) {
                throw new ArgumentOutOfRangeException("upperBound", upperBound, "upperBound must be >= lowerBound");
            }

            // Test if range will fit into an Int32.
            int range = upperBound - lowerBound;
            if (range >= 0) {
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
        public unsafe void NextBytes(byte[] buffer)
        {
            // For improved performance the below loop operates on these stack allocated copies of the heap variables.
            // Notes. doing this means that these heavily used variables are located near to other local/stack variables,
            // thus they will very likely be cached in the same CPU cache line.
            ulong s0 = _s0;
            ulong s1 = _s1;
            ulong s2 = _s2;
            ulong s3 = _s3;

            int i = 0;

            // Get a pointer to the start of [buffer]; to do this we must pin [buffer] because it is allocated
            // on the heap and therefore could be moved by the GC at any time (if we didn't pin it).
            fixed(byte* pBuffer = buffer)
            {
                // A pointer to 64 bit size segments of [buffer].
                ulong* pULong = (ulong*)pBuffer;

                // Create and store new random bytes in groups of eight.
                for(int bound = buffer.Length / 8; i < bound; i++)
                {
                    // Generate 64 random bits and assign to the segment that pULong is currently pointing to.
	                pULong[i] = RotateLeft(s1 * 5, 7) * 9;

                    ulong t = s1 << 17;

                    s2 ^= s0;
                    s3 ^= s1;
                    s1 ^= s2;
                    s0 ^= s3;

                    s2 ^= t;

                    s3 = RotateLeft(s3, 45);
                }
            }

            // Fill any trailing entries in [buffer] that occur when the its length is not a multiple of eight.
            // Note. We do this using safe C# therefore can unpin [buffer]; i.e. its preferable to hold pins for the 
            // shortest duration possible because they have an impact on the effectiveness of the garbage collector.

            // Convert back to one based indexing instead of groups of four bytes.
            i = i * 8;

            // Fill any remaining bytes in the buffer.
            if(i < buffer.Length)
            {
                // Generate a further 64 random bits.
                ulong result = RotateLeft(s1 * 5, 7) * 9;

                ulong t = s1 << 17;

                s2 ^= s0;
                s3 ^= s1;
                s1 ^= s2;
                s0 ^= s3;

                s2 ^= t;

                s3 = RotateLeft(s3, 45);

                // Allocate one byte at a time until we reach the end of the buffer.
                while (i < buffer.Length)
                {
                    buffer[i++] = (byte)result;
                    result >>= 8;
                }              
            }

            // Update the state variables on the heap.
            _s0 = s0;
            _s1 = s1;
            _s2 = s2;
            _s3 = s3;
        }

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
        /// between these two methods. As a consequence this method will typically be slightly faster because 
        /// Next () must test for Int32.MaxValue and resample the underlying RNG when that value occurs.
        /// </remarks>
        public int NextInt()
        {
            // Generate 64 random bits and shift right to leave the most significant 31 bits.
            // Bit 32 is the sign bit so must be zero to avoid negative results.
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
            // This is slower than the approach of generating and caching 64 bits for future calls, but 
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong NextInnerULong()
        {
            ulong s0 = _s0;
            ulong s1 = _s1;
            ulong s2 = _s2;
            ulong s3 = _s3;

            ulong result = RotateLeft(s1 * 5, 7) * 9;

            ulong t = s1 << 17;

            s2 ^= s0;
            s3 ^= s1;
            s1 ^= s2;
            s0 ^= s3;

            s2 ^= t;

            s3 = RotateLeft(s3, 45);

            _s0 = s0;
            _s1 = s1;
            _s2 = s2;
            _s3 = s3;

            return result;
        }

        #endregion

        #region Private Static Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong RotateLeft(ulong x, int k)
        {
            // Note. RyuJIT will compile this to a single rotate CPU instruction (as of about .NET 4.6.1 and dotnet core 2.0).
            return (x << k) | (x >> (64 - k));
        }

        #endregion
    }
}
