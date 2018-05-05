// A C# port of the xoroshiro128+ pseudo random number generator (PRNG).
// Original C source code was obtained from:
//    http://xoroshiro.di.unimi.it/xoroshiro128plus.c
//
// See original headers below for more info.
//
// -----------------------------------------------------------------------
//
// This is xoroshiro128+ 1.0, our best and fastest small-state generator
// for floating-point numbers. We suggest to use its upper bits for
// floating-point generation, as it is slightly faster than
// xoroshiro128**. It passes all tests we are aware of except for the four
// lower bits, which might fail linearity tests (and just those), so if
// low linear complexity is not considered an issue (as it is usually the
// case) it can be used to generate 64-bit outputs, too; moreover, this
// generator has a very mild Hamming-weight dependency making our test
// (http://prng.di.unimi.it/hwd.php) fail after 8 TB of output; we believe
// this slight bias cannot affect any application. If you are concerned,
// use xoroshiro128** or xoshiro256+.
//
// We suggest to use a sign test to extract a random Boolean value, and
// right shifts to extract subsets of bits.
//
// The state must be seeded so that it is not everywhere zero. If you have
// a 64-bit seed, we suggest to seed a splitmix64 generator and use its
// output to fill s. 
//
// NOTE: the parameters (a=24, b=16, b=37) of this version give slightly
// better results in our test than the 2016 version (a=55, b=14, c=36).

using System;
using System.Runtime.CompilerServices;

namespace Redzen.Random
{
    /// <summary>
    /// Xoroshiro128++ (xor, shift, rotate) pseudo random number generator (PRNG).
    /// </summary>
    public sealed class XoroShiro128PlusRandom : IRandomSource
    {
        // Constants.
        const double REAL_UNIT_UINT = 1.0 / (1UL << 53);
        const float REAL_UNIT_UINT_F = 1f / (1U << 24);

        // RNG state.
        ulong _s0;
        ulong _s1;

        #region Constructors

        /// <summary>
        /// Initialises a new instance using a seed generated from the class's static seed RNG.
        /// </summary>
        public XoroShiro128PlusRandom()
        {
            Reinitialise(RandomSourceFactory.GetNextSeed());
        }

        /// <summary>
        /// Initialises a new instance using an int value as seed.
        /// This constructor signature is provided to maintain compatibility with
        /// System.Random
        /// </summary>
        public XoroShiro128PlusRandom(int seed)
        {
            Reinitialise(seed);
        }

        #endregion

        #region Public Methods [Re-initialisation]

        /// <summary>
        /// Re-initialises using an int value as a seed.
        /// </summary>
        public void Reinitialise(int seed)
        {
            // TODO/FIXME: Confirm if additional checks are required to avoid the possibility that both _s0 and _s1 are zero.

            // Use the splitmix64 RNG to hash the seed.
            // Note. It is required that at least one the state variables (_s0, _s1) be non-zero.
            ulong q = (ulong)seed;

            _s0 = Splitmix64Rng.Next(ref q);
            _s1 = Splitmix64Rng.Next(ref q);
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
	    retry:
            // Handle the special case where the value int.MaxValue is generated; this is outside 
            // the range of permitted return values for this method. 
            ulong rtn = NextInnerULong() & 0x7fffffffUL;
            if(rtn == 0x7FFFFFFF) {
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
        public unsafe void NextBytes(byte[] buffer)
        {
            // For improved performance the below loop operates on these stack allocated copies of the heap variables.
            // Notes. doing this means that these heavily used variables are located near to other local/stack variables,
            // thus they will very likely be cached in the same CPU cache line.
            ulong s0 = _s0, s1 = _s1;

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
	                pULong[i] = s0 + s1;
	                s1 ^= s0;
	                s0 = RotateLeft(s0, 55) ^ s1 ^ (s1 << 14);
	                s1 = RotateLeft(s1, 36);
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
                ulong t = s0 + s1;
	            s1 ^= s0;
	            s0 = RotateLeft(s0, 55) ^ s1 ^ (s1 << 14);
	            s1 = RotateLeft(s1, 36);

                // Allocate one byte at a time until we reach the end of the buffer.
                while(i < buffer.Length)
                {
                    buffer[i++] = (byte)t;   
                    t >>= 8;
                }              
            }

            // Update the state variables on the heap.
            _s0 = s0;
            _s1 = s1;
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
            return (NextInnerUInt() >> 8) * REAL_UNIT_UINT_F;
        }

        /// <summary>
        /// Generates a random UInt32 over the interval [uint.MinValue, uint.MaxValue], i.e. over the full 
        /// range of a UInt32.
        /// </summary>
        public uint NextUInt()
        {
            return NextInnerUInt();
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
            return (int)(NextInnerUInt() & 0x7FFFFFFF);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint NextInnerUInt()
        {
            return (uint)(NextInnerULong() & 0xffffffff);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong NextInnerULong()
        {
	        ulong s0 = _s0;
	        ulong s1 = _s1;
	        ulong result = s0 + s1;

	        s1 ^= s0;

	        _s0 = RotateLeft(s0, 55) ^ s1 ^ (s1 << 14); // a, b
	        _s1 = RotateLeft(s1, 36); // c

            return result;
        }

        #endregion

        #region Private Static Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong RotateLeft(ulong x, int k)
        {
            // Note. Ryujit will compile this to a rotate CPU instruction (as of about 2017).
            // This is a significant factor in the speed of this RNG.
            return (x << k) | (x >> (64 - k));
        }

        #endregion
    }
}
