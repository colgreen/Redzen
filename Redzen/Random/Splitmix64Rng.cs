// A C# port of the Splitmix 64 pseudo random number generator (PRNG).
// Original C source code was obtained from:
//    https://github.com/svaarala/duktape/blob/master/misc/splitmix64.c
//
// See original headers below for more info.
//
// -----------------------------------------------------------------------
//
// Written in 2015 by Sebastiano Vigna (vigna@acm.org)
// To the extent possible under law, the author has dedicated all copyright
// and related and neighboring rights to this software to the public domain
// worldwide. This software is distributed without any warranty.
// See <http://creativecommons.org/publicdomain/zero/1.0/>.
//
// This is a fixed-increment version of Java 8's SplittableRandom generator
// See http://dx.doi.org/10.1145/2714064.2660195 and
// http://docs.oracle.com/javase/8/docs/api/java/util/SplittableRandom.html
// It is a very fast generator passing BigCrush, and it can be useful if
// for some reason you absolutely want 64 bits of state; otherwise, we
// rather suggest to use a xoroshiro128+ (for moderately parallel
// computations) or xorshift1024* (for massively parallel computations)
// generator.

namespace Redzen.Random
{
    /// <summary>
    /// Splitmix64 Pseudo Random Number Generator (PRNG).
    /// </summary>
    public static class Splitmix64Rng
    {
        /// <summary>
        /// Splitmix64 PRNG.
        /// </summary>
        /// <param name="x">PRNG state. This can take any value, including zero.</param>
        /// <returns>A new random UInt64.</returns>
        public static ulong Next(ref ulong x)
        {
	        ulong z = (x += 0x9E3779B97F4A7C15UL);
	        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
	        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
	        return z ^ (z >> 31);
        }
    }
}
