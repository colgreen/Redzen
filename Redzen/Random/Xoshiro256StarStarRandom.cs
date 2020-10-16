﻿// A C# port of the xorshiro256** pseudo random number generator (PRNG).
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
using System.Numerics;

namespace Redzen.Random
{
    /// <summary>
    /// Xoshiro256** (xor, shift, rotate) pseudo-random number generator (PRNG).
    /// </summary>
    public sealed class Xoshiro256StarStarRandom : RandomSourceBase, IRandomSource
    {
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

        #region Protected Methods

        /// <summary>
        /// Fills the provided byte span with random bytes.
        /// </summary>
        /// <param name="buffer">The byte span to fill with random values.</param>
        public override unsafe void NextBytes(Span<byte> buffer)
        {
            // For improved performance the below loop operates on these stack allocated copies of the heap variables.
            // Note. doing this means that these heavily used variables are located near to other local/stack variables,
            // thus they will very likely be cached in the same CPU cache line.
            ulong s0 = _s0;
            ulong s1 = _s1;
            ulong s2 = _s2;
            ulong s3 = _s3;

            int i = 0;

            // Get a pointer to the start of {buffer}; to do this we must pin {buffer} because it may be on the heap and 
            // therefore could be moved by the GC at any time if not pinned.
            fixed(byte* pBuffer = buffer)
            {
                // A pointer to 64 bit size segments of {buffer}.
                ulong* pULong = (ulong*)pBuffer;

                // Create and store new random bytes in groups of eight.
                for(int bound = buffer.Length / 8; i < bound; i++)
                {
                    // Generate 64 random bits and assign to the segment that pULong is currently pointing to.
                    pULong[i] = BitOperations.RotateLeft(s1 * 5, 7) * 9;

                    // Update PRNG state.
                    ulong t = s1 << 17;
                    s2 ^= s0;
                    s3 ^= s1;
                    s1 ^= s2;
                    s0 ^= s3;
                    s2 ^= t;
                    s3 = BitOperations.RotateLeft(s3, 45);
                }
            }

            // Fill any trailing entries in {buffer} that occur when the its length is not a multiple of eight.
            // Note. We do this using safe C# therefore can unpin {buffer}; i.e. its preferable to hold pins for the 
            // shortest duration possible because they have an impact on the effectiveness of the garbage collector.

            // Convert back to one based indexing instead of groups of eight bytes.
            i *= 8;

            // Fill any remaining bytes in the buffer.
            if(i < buffer.Length)
            {
                // Generate a further 64 random bits.
                ulong result = BitOperations.RotateLeft(s1 * 5, 7) * 9;

                // Update PRNG state.
                ulong t = s1 << 17;
                s2 ^= s0;
                s3 ^= s1;
                s1 ^= s2;
                s0 ^= s3;
                s2 ^= t;
                s3 = BitOperations.RotateLeft(s3, 45);

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

        /// <summary>
        /// Get the next 64 random bits from the underlying PRNG. This method forms the foundation for most of the methods of each
        /// IRandomSource implementation, which take these 64 bits and manipulate them to provide random values of various
        /// data types, such as integers, byte arrays, floating point values, etc.
        /// </summary>
        /// <returns>A <see cref="ulong"/> containing random bits from the underlying PRNG algorithm</returns>
        protected override ulong NextULongInner()
        {
            ulong s0 = _s0;
            ulong s1 = _s1;
            ulong s2 = _s2;
            ulong s3 = _s3;

            ulong result = BitOperations.RotateLeft(s1 * 5, 7) * 9;

            ulong t = s1 << 17;

            s2 ^= s0;
            s3 ^= s1;
            s1 ^= s2;
            s0 ^= s3;

            s2 ^= t;

            s3 = BitOperations.RotateLeft(s3, 45);

            _s0 = s0;
            _s1 = s1;
            _s2 = s2;
            _s3 = s3;

            return result;
        }

        #endregion
    }
}
