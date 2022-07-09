﻿// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen;

/// <summary>
/// Utility methods related to prime numbers.
/// </summary>
public static class PrimeUtils
{
    static readonly int[] __primes = new int[]
    {
        1,    2,   3,   5,   7,  11,  13,  17,  19,  23,   29,  31,  37,  41,  43,  47,
        53,  59,  61,  67,  71,  73,  79,  83,  89,  97,  101, 103, 107, 109, 113, 127,
        131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211,
        223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293, 307,
        311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397, 401,
        409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499,
        503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599, 601, 607,
        613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691, 701, 709
    };

    /// <summary>
    /// Returns the smallest prime greater than or equal to the given integer value.
    /// </summary>
    /// <param name="x">The input value.</param>
    /// <returns>The integer that is the smallest prime greater than or equal to the given integer value.</returns>
    public static int CeilingPrime(int x)
    {
        if(x < 0) throw new ArgumentException("Must be greater than or equal to zero.", nameof(x));

        if(x < 719)
        {
            for(int i=0; i < __primes.Length; i++)
            {
                if(__primes[i] >= x)
                    return __primes[i];
            }
        }
        else
        {
            // Note. int.MaxValue is prime.
            for(int i = x | 1; i <= int.MaxValue; i += 2)
            {
                if(IsPrime(i))
                    return i;
            }
        }

        // Unreachable code.
        throw new InvalidOperationException();
    }

    /// <summary>
    /// Test for a prime number.
    /// </summary>
    /// <param name="x">The number to test.</param>
    /// <returns>true is <paramref name="x"/> is prime; otherwise false.</returns>
    public static bool IsPrime(int x)
    {
        // Test for odd number.
        if((x & 1) != 0)
        {
            int sqrt = (int)Math.Sqrt((double)x);

            // Loop through all odd numbers, starting at 3.
            for(int i = 3; i <= sqrt; i += 2)
            {
                // Test if divisible by i.
                if(x % i == 0)
                    return false;
            }
            return true;
        }
        return x == 2;
    }
}
