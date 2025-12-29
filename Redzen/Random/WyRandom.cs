// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
// The underlying algorithm and portions of this source file are from https://github.com/wangyi-fudan/wyhash.
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Redzen.Random;

/// <summary>
/// wyrand pseudo-random number generator (PRNG).
/// </summary>
/// <remarks>
/// Uses the wyrand PRNG defined at https://github.com/wangyi-fudan/wyhash.
/// This PRNG is based on multiplication of two 64 bit numbers to give a 128 bit result, which fast on x86
/// CPUs that have the BMI2 MULX instruction that can do this multiplication in hardware, otherwise the
/// multiplication is emulated but is still very fast.
///
/// This PRNG passes the BigCrush and practrand statistical tests, and is probably the strongest PRNG of those
/// currently provided in the Redzen library. Performance is comparable to xoshiro256** when the BMI2 MULX
/// CPU instruction is available, but perhaps marginally slower overall (on an AMD Ryzen 7 PRO 5750G), but not
/// by enough to really matter. If you require a high-speed non-cryptographic PRNG then this is a good choice
/// and probably preferable to Xoshiro256**.
///
/// Also see https://github.com/lemire/testingRNG/blob/master/README.md.
/// </remarks>
public sealed class WyRandom : RandomSourceBase, IRandomSource
{
    const ulong Prime0 = 0xa0761d6478bd642f;
    const ulong Prime1 = 0xe7037ed1a0b428db;

    // RNG state.
    ulong _s0;

    /// <summary>
    /// Initialises a new instance with a seed from the default seed source.
    /// </summary>
    public WyRandom()
    {
        Reinitialise(RandomDefaults.GetSeed());
    }

    /// <summary>
    /// Initialises a new instance with the provided seed.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    public WyRandom(ulong seed)
    {
        Reinitialise(seed);
    }

    /// <inheritdoc/>
    public void Reinitialise(ulong seed)
    {
        _s0 = seed;
    }

    /// <inheritdoc/>
    public override unsafe void NextBytes(Span<byte> span)
    {
        // For improved performance the below loop operates on this stack allocated copy of the heap variable _s0.
        // Note. doing this means that this heavily used variable is located near to other local/stack variables,
        // thus they will very likely be cached in the same CPU cache line.
        ulong s0 = _s0;
        ulong lo, hi;

        // Allocate bytes in groups of 8 (64 bits at a time), for good performance.
        // Keep looping and updating buffer to point to the remaining/unset bytes, until buffer.Length is too small
        // to use this loop.
        while(span.Length >= sizeof(ulong))
        {
            // Get 64 random bits, and assign to buffer (at the slice it is currently pointing to).
            s0 += Prime0;
            hi = Multiply(s0 ^ Prime1, s0, &lo);

            Unsafe.WriteUnaligned(
                ref MemoryMarshal.GetReference(span),
                hi ^ lo);

            // Set buffer to the a slice over the remaining bytes.
            span = span.Slice(sizeof(ulong));
        }

        // Fill any remaining bytes in buffer (these occur when its length is not a multiple of eight).
        if(!span.IsEmpty)
        {
            // Get 64 random bits.
            s0 += Prime0;
            hi = Multiply(s0 ^ Prime1, s0, &lo);
            lo ^= hi;
            byte* remainingBytes = (byte*)&lo;

            for(int i=0; i < span.Length; i++)
            {
                span[i] = remainingBytes[i];
            }
        }

        // Update the state variable on the heap.
        _s0 = s0;
    }

    /// <inheritdoc/>
    protected unsafe override ulong NextULongInner()
    {
        ulong s0 = _s0;

        s0 += Prime0;

        // The Mix-and-Multiply (MUM) function, manually inlined.
        ulong lo;
        ulong hi = Multiply(s0 ^ Prime1, s0, &lo);

        _s0 = s0;

        // XOR the hi and low 64 bits of the 128 bit result from the multiplication of two 64 bit unsigned integers.
        return lo ^ hi;
    }

    /// <summary>
    /// Multiply two 64 bit unsigned integers, producing a 128 bit result returned as two 64 bit values, one
    /// each for the high and low 64 bits respectively.
    /// </summary>
    /// <param name="x">Operand 1.</param>
    /// <param name="y">Operand 2.</param>
    /// <param name="lo">Address of a variable to store the low 64 bits in.</param>
    /// <returns>The high 64 bits.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe ulong Multiply(ulong x, ulong y, ulong* lo)
    {
        // Use BMI2 intrinsics where available.
        if(Bmi2.X64.IsSupported)
        {
            // Use the MULX CPU instruction to perform the multiplication in hardware.
            return Bmi2.X64.MultiplyNoFlags(x, y, lo);
        }
        else
        {
            // Emulate 64x64 bit multiplication to 128 bit result.
            // Uses the approach defined at https://stackoverflow.com/a/51587262/15703
            *lo = x * y;

            ulong x0 = (uint)x;
            ulong x1 = x >> 32;

            ulong y0 = (uint)y;
            ulong y1 = y >> 32;

            ulong p11 = x1 * y1;
            ulong p01 = x0 * y1;
            ulong p10 = x1 * y0;
            ulong p00 = x0 * y0;

            /*
                This is implementing schoolbook multiplication:

                        x1 x0
                X       y1 y0
                -------------
                           00  LOW PART
                -------------
                        00
                     10 10     MIDDLE PART
                +       01
                -------------
                     01
                + 11 11        HIGH PART
                -------------
            */

            // 64-bit product + two 32-bit values
            ulong middle = p10 + (p00 >> 32) + (uint)p01;

            /*
                Proof that 64-bit products can accumulate two more 32-bit values
                without overflowing:

                Max 32-bit value is 2^32 - 1.
                PSum = (2^32-1) * (2^32-1) + (2^32-1) + (2^32-1)
                     = 2^64 - 2^32 - 2^32 + 1 + 2^32 - 1 + 2^32 - 1
                     = 2^64 - 1
                Therefore it cannot overflow regardless of input.
            */

            // 64-bit product + two 32-bit values
            ulong hi = p11 + (middle >> 32) + (p01 >> 32);

            return hi;
        }
    }
}
