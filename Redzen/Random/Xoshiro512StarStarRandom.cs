// A C# port of the xorshiro512** pseudo random number generator (PRNG).
// Original C source code was obtained from:
//    http://xoshiro.di.unimi.it/xoshiro512starstar.c
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
// This is xoshiro512** 1.0, an all-purpose, rock-solid generator. It has
// excellent (about 1ns) speed, an increased state (512 bits) that is
// large enough for any parallel application, and it passes all tests we
// are aware of.
//
// For generating just floating-point numbers, xoshiro512+ is even faster.
//
// The state must be seeded so that it is not everywhere zero. If you have
// a 64-bit seed, we suggest to seed a splitmix64 generator and use its
// output to fill s.

using System.Runtime.CompilerServices;
using static Redzen.BitwiseUtils;

namespace Redzen.Random
{
    /// <summary>
    /// Xoshiro512** (xor, shift, rotate) pseudo random number generator (PRNG).
    /// </summary>
    public sealed class Xoshiro512StarStarRandom : RandomSourceBase, IRandomSource
    {
        // RNG state.
        ulong _s0;
        ulong _s1;
        ulong _s2;
        ulong _s3;
        ulong _s4;
        ulong _s5;
        ulong _s6;
        ulong _s7;

        #region Constructors

        /// <summary>
        /// Initialises a new instance with a seed from the default seed source.
        /// </summary>
        public Xoshiro512StarStarRandom()
        {
            Reinitialise(RandomDefaults.GetSeed());
        }

        /// <summary>
        /// Initialises a new instance with the provided seed.
        /// </summary>
        public Xoshiro512StarStarRandom(ulong seed)
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
            _s4 = Splitmix64Rng.Next(ref seed);
            _s5 = Splitmix64Rng.Next(ref seed);
            _s6 = Splitmix64Rng.Next(ref seed);
            _s7 = Splitmix64Rng.Next(ref seed);
        }

        #endregion

        #region Protected Methods

        // ENHANCEMENT: NextBytes(Span<byte>)
        /// <summary>
        /// Fills the provided byte array with random bytes.
        /// </summary>
        /// <param name="buffer">The byte array to fill with random values.</param>
        public override unsafe void NextBytes(byte[] buffer)
        {
            // For improved performance the below loop operates on these stack allocated copies of the heap variables.
            // Note. doing this means that these heavily used variables are located near to other local/stack variables,
            // thus they will very likely be cached in the same CPU cache line.
            ulong s0 = _s0;
            ulong s1 = _s1;
            ulong s2 = _s2;
            ulong s3 = _s3;
            ulong s4 = _s4;
            ulong s5 = _s5;
            ulong s6 = _s6;
            ulong s7 = _s7;

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
                    ulong t = s1 << 11;
                    s2 ^= s0;
                    s5 ^= s1;
                    s1 ^= s2;
                    s7 ^= s3;
                    s3 ^= s4;
                    s4 ^= s5;
                    s0 ^= s6;
                    s6 ^= s7;
                    s6 ^= t;
                    s7 = RotateLeft(s7, 21);
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
                ulong t = s1 << 11;
                s2 ^= s0;
                s5 ^= s1;
                s1 ^= s2;
                s7 ^= s3;
                s3 ^= s4;
                s4 ^= s5;
                s0 ^= s6;
                s6 ^= s7;
                s6 ^= t;
                s7 = RotateLeft(s7, 21);

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
            _s4 = s4;
            _s5 = s5;
            _s6 = s6;
            _s7 = s7;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override ulong NextULongInner()
        {
            ulong result = RotateLeft(_s1 * 5, 7) * 9;

            ulong t = _s1 << 11;

            _s2 ^= _s0;
            _s5 ^= _s1;
            _s1 ^= _s2;
            _s7 ^= _s3;
            _s3 ^= _s4;
            _s4 ^= _s5;
            _s0 ^= _s6;
            _s6 ^= _s7;

            _s6 ^= t;

            _s7 = RotateLeft(_s7, 21);

            return result;
        }

        #endregion
    }
}
