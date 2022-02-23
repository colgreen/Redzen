// A C# port of the xorshiro256+ pseudo random number generator (PRNG).
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
// This is xoshiro256+ 1.0, our best and fastest generator for floating-point
// numbers. We suggest to use its upper bits for floating-point
// generation, as it is slightly faster than xoshiro256**. It passes all
// tests we are aware of except for the lowest three bits, which might
// fail linearity tests (and just those), so if low linear complexity is
// not considered an issue (as it is usually the case) it can be used to
// generate 64-bit outputs, too.
//
// We suggest to use a sign test to extract a random Boolean value, and
// right shifts to extract subsets of bits.
//
// The state must be seeded so that it is not everywhere zero. If you have
// a 64-bit seed, we suggest to seed a splitmix64 generator and use its
// output to fill s.
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Redzen.Random;

/// <summary>
/// Xoshiro256+ (xor, shift, rotate) pseudo-random number generator (PRNG).
/// </summary>
public sealed class Xoshiro256PlusRandom : RandomSourceBase, IRandomSource
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
    public Xoshiro256PlusRandom()
    {
        Reinitialise(RandomDefaults.GetSeed());
    }

    /// <summary>
    /// Initialises a new instance with the provided ulong seed.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    public Xoshiro256PlusRandom(ulong seed)
    {
        Reinitialise(seed);
    }

    #endregion

    #region Public Methods [Re-initialisation]

    /// <summary>
    /// Re-initialises the random number generator state using the provided seed.
    /// </summary>
    /// <param name="seed">Seed value.</param>
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
    /// <param name="span">The byte span to fill with random values.</param>
    public override unsafe void NextBytes(Span<byte> span)
    {
        // For improved performance the below loop operates on these stack allocated copies of the heap variables.
        // Notes. doing this means that these heavily used variables are located near to other local/stack variables,
        // thus they will very likely be cached in the same CPU cache line.
        ulong s0 = _s0;
        ulong s1 = _s1;
        ulong s2 = _s2;
        ulong s3 = _s3;

        // Allocate bytes in groups of 8 (64 bits at a time), for good performance.
        // Keep looping and updating buffer to point to the remaining/unset bytes, until buffer.Length is too small
        // to use this loop.
        while(span.Length >= sizeof(ulong))
        {
            // Get 64 random bits, and assign to buffer (at the slice it is currently pointing to).
            Unsafe.WriteUnaligned(
                ref MemoryMarshal.GetReference(span),
                s0 + s3);

            // Update PRNG state.
            ulong t = s1 << 17;
            s2 ^= s0;
            s3 ^= s1;
            s1 ^= s2;
            s0 ^= s3;
            s2 ^= t;
            s3 = BitOperations.RotateLeft(s3, 45);

            // Set buffer to the a slice over the remaining bytes.
            span = span.Slice(sizeof(ulong));
        }

        // Fill any remaining bytes in buffer (these occur when its length is not a multiple of eight).
        if(!span.IsEmpty)
        {
            // Get 64 random bits.
            ulong next = s0 + s3;
            byte* remainingBytes = (byte*)&next;

            for(int i=0; i < span.Length; i++)
            {
                span[i] = remainingBytes[i];
            }

            // Update PRNG state.
            ulong t = s1 << 17;
            s2 ^= s0;
            s3 ^= s1;
            s1 ^= s2;
            s0 ^= s3;
            s2 ^= t;
            s3 = BitOperations.RotateLeft(s3, 45);
        }

        // Update the state variables on the heap.
        _s0 = s0;
        _s1 = s1;
        _s2 = s2;
        _s3 = s3;
    }

    /// <summary>
    /// Get the next 64 random bits from the underlying PRNG. This method forms the foundation for most of the methods of each
    /// <see cref="IRandomSource"/> implementation, which take these 64 bits and manipulate them to provide random values of various
    /// data types, such as integers, byte arrays, floating point values, etc.
    /// </summary>
    /// <returns>A <see cref="ulong"/> containing random bits from the underlying PRNG algorithm.</returns>
    protected override ulong NextULongInner()
    {
        ulong s0 = _s0;
        ulong s1 = _s1;
        ulong s2 = _s2;
        ulong s3 = _s3;

        // Generate a new random sample.
        ulong result = s0 + s3;

        // Update PRNG state.
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
