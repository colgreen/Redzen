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

using System.Runtime.CompilerServices;

namespace Redzen.Random
{
    /// <summary>
    /// Xoroshiro128+ (xor, shift, rotate) pseudo random number generator (PRNG).
    /// </summary>
    public sealed class XoroShiro128PlusRandom : Random64Base
    {
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
        /// Initialises a new instance using the provided ulong seed.
        /// </summary>
        public XoroShiro128PlusRandom(ulong seed)
        {
            Reinitialise(seed);
        }

        #endregion

        #region Public Methods [Re-initialisation]

        /// <summary>
        /// Re-initialises the random number generator state using the provided seed value.
        /// </summary>
        public override void Reinitialise(ulong seed)
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
        }

        #endregion

        #region Public Methods [System.Random functionally equivalent methods]

        /// <summary>
        /// Fills the provided byte array with random bytes.
        /// </summary>
        /// <param name="buffer">The byte array to fill with random values.</param>
        public override unsafe void NextBytes(byte[] buffer)
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
	                s0 = RotateLeft(s0, 24) ^ s1 ^ (s1 << 16);
	                s1 = RotateLeft(s1, 37);
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
	            s0 = RotateLeft(s0, 24) ^ s1 ^ (s1 << 16);
	            s1 = RotateLeft(s1, 37);

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

        #region Private Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override ulong NextInnerULong()
        {
	        ulong s0 = _s0;
	        ulong s1 = _s1;

	        ulong result = s0 + s1;

	        s1 ^= s0;
	        _s0 = RotateLeft(s0, 24) ^ s1 ^ (s1 << 16); // a, b
	        _s1 = RotateLeft(s1, 37); // c

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
