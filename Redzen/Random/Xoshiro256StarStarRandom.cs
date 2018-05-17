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
        const double INCR_DOUBLE = 1.0 / (1UL << 53);
        const float INCR_FLOAT = 1f / (1U << 24);

        // RNG state.
        ulong _s0;
        ulong _s1;
        ulong _s2;
        ulong _s3;

        #region Constructors

        /// <summary>
        /// Initialises a new instance with a seed from the default seed source.
        /// </summary>
        public Xoshiro256StarStarRandom()
        {
            Reinitialise(RandomDefaults.GetSeed());
        }

        /// <summary>
        /// Initialises a new instance with the provided seed.
        /// </summary>
        public Xoshiro256StarStarRandom(ulong seed)
        {
            Reinitialise(seed);
        }

        #endregion

        #region Public Methods [Re-initialisation]

        /// <summary>
        /// Re-initialises the random number generator state using the provided seed.
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
        /// Fills the provided byte array with random bytes.
        /// </summary>
        /// <param name="buffer">The byte array to fill with random values.</param>
        public unsafe void NextBytes(byte[] buffer)
        {
            // For improved performance the below loop operates on these stack allocated copies of the heap variables.
            // Note. doing this means that these heavily used variables are located near to other local/stack variables,
            // thus they will very likely be cached in the same CPU cache line.
            ulong s0 = _s0;
            ulong s1 = _s1;
            ulong s2 = _s2;
            ulong s3 = _s3;

            int i = 0;

            // Get a pointer to the start of {buffer}; to do this we must pin {buffer} because it is allocated
            // on the heap and therefore could be moved by the GC at any time (if we didn't pin it).
            fixed(byte* pBuffer = buffer)
            {
                // A pointer to 64 bit size segments of {buffer}.
                ulong* pULong = (ulong*)pBuffer;

                // Create and store new random bytes in groups of eight.
                for(int bound = buffer.Length / 8; i < bound; i++)
                {
                    // Generate 64 random bits and assign to the segment that pULong is currently pointing to.
                    pULong[i] = RotateLeft(s1 * 5, 7) * 9;

                    // Update PRNG state.
                    ulong t = s1 << 17;
                    s2 ^= s0;
                    s3 ^= s1;
                    s1 ^= s2;
                    s0 ^= s3;
                    s2 ^= t;
                    s3 = RotateLeft(s3, 45);
                }
            }

            // Fill any trailing entries in {buffer} that occur when the its length is not a multiple of eight.
            // Note. We do this using safe C# therefore can unpin {buffer}; i.e. its preferable to hold pins for the 
            // shortest duration possible because they have an impact on the effectiveness of the garbage collector.

            // Convert back to one based indexing instead of groups of four bytes.
            i = i * 8;

            // Fill any remaining bytes in the buffer.
            if(i < buffer.Length)
            {
                // Generate a further 64 random bits.
                ulong result = RotateLeft(s1 * 5, 7) * 9;

                // Update PRNG state.
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
            // Notes.
            // Here we sample an integer value within the interval [0, maxValue). Rejection sampling is used in 
            // order to produce unbiased samples. An alternative approach is:
            //
            //  return (int)(NextDoubleInner() * maxValue);
            //
            // I.e. generate a double precision float in the interval [0,1) and multiply by maxValue. However the
            // use of floating point arithmetic will introduce bias for odd values of maxValue, therefore this 
            // method is not used.
            //
            // The rejection sampling method used here operates as follows:
            //
            //  1) Calculate N such that  2^(N-1) < maxValue <= 2^N, i.e. N is the minimum number of bits required
            //     to represent maxValue states.
            //  2) Generate an N bit random sample.
            //  3) Reject samples that are >= maxValue, and goto (2) to resample.
            //
            // Repeat until a valid sample is generated.

            // Log2(numberOfStates) gives the number of bits required to represent that many states, however this
            // is integer Log2() so any fractional part in the result is truncated, i.e. the result may be 1 bit 
            // too short. Thus, if 2^bitCount == maxValue, bitCount was correct (which in turn also means that 
            // maxValue is a power of two); otherwise we increment bitCount by one to get the correct bit count.
            int bitCount = MathUtils.Log2(maxValue);
            int range = MathUtils.PowerOfTwo(bitCount);
            if(maxValue != range) {
                bitCount++;
            }

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
            // See comments on NextInner(int).
            int bitCount = MathUtils.Log2(maxValue);
            long range = MathUtils.PowerOfTwo((long)bitCount);
            if(maxValue != range) {
                bitCount++;
            }

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
            // 
            // Note however that the total number of distinct values that can be represented by a double in the interval 
            // [0,1] is a little under 2^62, i.e. considerably more than the 2^53 values in the above described scheme,
            // e.g. that scheme will not generate any of the possible values in the interval (0, 2^-53). However, selecting 
            // from the full set of possible values uniformly will produce a highly biased distribution. 
            //
            // An alternative scheme exists that can produce all 2^62 (or so) values, and that produces a uniform distribution
            // over [0,1]; for an explanation see:
            //
            //    2014, Taylor R Campbell
            //
            //    Uniform random floats:  How to generate a double-precision
            //    floating-point number in [0, 1] uniformly at random given a uniform
            //    random source of bits.
            //
            //    https://mumble.net/~campbell/tmp/random_real.c
            //
            // That scheme is not employed here because its additional complexity will have significantly slower performance
            // compared to the simple shift and multiply performed here, and for most general purpose uses the 1/2^53 
            // resolution is more than sufficient, representing precision to approximately the 16th decimal place.
            return (NextULongInner() >> 11) * INCR_DOUBLE;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong NextULongInner()
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
