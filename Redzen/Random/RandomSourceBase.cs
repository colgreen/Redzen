// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace Redzen.Random;

/// <summary>
/// Base class providing much of the shared logic for <see cref="IRandomSource"/> implementations.
/// </summary>
public abstract class RandomSourceBase
{
    // Constants.
    const double INCR_DOUBLE = 1.0 / (1UL << 53);
    const float INCR_FLOAT = 1f / (1U << 24);
    const float INCR_HALF = 1f / (1U << 11);

    #region Public Methods [System.Random equivalent methods]

    /// <summary>
    /// Returns a random integer sampled from the uniform distribution with interval [0, int.MaxValue),
    /// i.e., exclusive of <see cref="Int32.MaxValue"/>.
    /// </summary>
    /// <returns>A new random sample.</returns>
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
            rtn = NextULongInner() >> 33;
        }
        while(rtn == 0x7fff_ffffUL);

        return (int)rtn;
    }

    /// <summary>
    /// Returns a random integer sampled from the uniform distribution with interval [0, maxValue),
    /// i.e., exclusive of <paramref name="maxValue"/>.
    /// </summary>
    /// <param name="maxValue">The maximum value to be sampled (exclusive).</param>
    /// <returns>A new random sample.</returns>
    public int Next(int maxValue)
    {
        if(maxValue < 1)
            throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "maxValue must be > 0");

        return NextInner(maxValue);
    }

    /// <summary>
    /// Returns a random integer sampled from the uniform distribution with interval [minValue, maxValue),
    /// i.e., inclusive of <paramref name="minValue"/> and exclusive of <paramref name="maxValue"/>.
    /// </summary>
    /// <param name="minValue">The minimum value to be sampled (inclusive).</param>
    /// <param name="maxValue">The maximum value to be sampled (exclusive).</param>
    /// <returns>A new random sample.</returns>
    /// <remarks>
    /// maxValue must be greater than minValue. minValue may be negative.
    /// </remarks>
    public int Next(int minValue, int maxValue)
    {
        if(minValue >= maxValue)
            throw new ArgumentOutOfRangeException(nameof(maxValue), maxValue, "maxValue must be > minValue");

        long range = (long)maxValue - minValue;
        if(range <= int.MaxValue)
            return NextInner((int)range) + minValue;

        // Call NextInner(long); i.e. the range is greater than int.MaxValue.
        return (int)(NextInner(range) + minValue);
    }

    /// <summary>
    /// Returns a random <see cref="double"/> sampled from the uniform distribution with interval [0, 1),
    /// i.e., inclusive of 0.0 and exclusive of 1.0.
    /// </summary>
    /// <returns>A new random sample, of type <see cref="double"/>.</returns>
    public double NextDouble()
    {
        return NextDoubleInner();
    }

    /// <summary>
    /// Fills the provided span with random byte values, sampled from the uniform distribution with interval [0, 255].
    /// </summary>
    /// <param name="span">The byte span to fill with random samples.</param>
    public abstract void NextBytes(Span<byte> span);

    #endregion

    #region Public Methods [Methods not present on System.Random]

    /// <summary>
    /// Returns a random integer sampled from the uniform distribution with interval [0, int.MaxValue],
    /// i.e., <b>inclusive</b> of <see cref="Int32.MaxValue"/>.
    /// </summary>
    /// <returns>A new random sample.</returns>
    /// <remarks>
    /// This method differs from <see cref="Next()"/>, in the following way; the uniform distribution that
    /// is sampled from includes the value <see cref="int.MaxValue"/>.
    /// </remarks>
    /// <remarks>
    /// This method differs from <see cref="Next()"/>, in the following way; the uniform distribution that
    /// is sampled from includes the value <see cref="int.MaxValue"/>. This small difference results in faster
    /// execution, because Next() must test for Int32.MaxValue and re-sample the underlying PRNG when that
    /// value occurs.
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
    /// Returns a random <see cref="uint"/> sampled from the uniform distribution with interval [0, uint.MaxValue],
    /// i.e., over the full range of possible uint values.
    /// </summary>
    /// <returns>A new random sample.</returns>
    public uint NextUInt()
    {
        return (uint)NextULongInner();
    }

    /// <summary>
    /// Returns a random <see cref="ulong"/> sampled from the uniform distribution with interval [0, ulong.MaxValue],
    /// i.e., over the full range of possible ulong values.
    /// </summary>
    /// <returns>A new random sample.</returns>
    public ulong NextULong()
    {
        return NextULongInner();
    }

    /// <summary>
    /// Returns a random boolean sampled from the uniform discrete distribution {false, true}, i.e., a fair coin flip.
    /// </summary>
    /// <returns>A new random sample.</returns>
    /// <remarks>
    /// Returns a sample the Bernoulli distribution with p = 0.5; also known as a a fair coin flip.
    /// </remarks>
    public bool NextBool()
    {
        // Use a high bit since the low bits are linear-feedback shift registers (LFSRs) with low degree.
        // This is slower than the approach of generating and caching 64 bits for future calls, but
        // (A) gives good quality randomness, and (B) is still very fast.
        return (NextULongInner() & 0x8000_0000_0000_0000) != 0;
    }

    /// <summary>
    /// Returns a random byte value sampled from the uniform distribution [0, 255].
    /// </summary>
    /// <returns>A new random sample.</returns>
    public byte NextByte()
    {
        // Here we shift right to use the 8 most significant bits because these exhibit higher quality
        // randomness than the lower bits.
        return (byte)(NextULongInner() >> 56);
    }

    /// <summary>
    /// Returns a random <see cref="float"/> sampled from the uniform distribution with interval [0, 1),
    /// i.e., inclusive of 0.0 and exclusive of 1.0.
    /// </summary>
    /// <returns>A new random sample, of type <see cref="float"/>.</returns>
    public float NextFloat()
    {
        // Here we generate a random integer between 0 and 2^24-1 (i.e., 24 binary 1s) and multiply by the fractional
        // unit value 1.0 / 2^24, thus the resulting random value has a min value of 0.0, and a max value of
        // 1.0 - (1.0 / 2^24).
        return (NextULongInner() >> 40) * INCR_FLOAT;
    }

    /// <summary>
    /// Returns a random <see cref="float"/> sampled from the uniform distribution with interval (0, 1],
    /// i.e., exclusive of 0.0, and inclusive of 1.0.
    /// </summary>
    /// <returns>A new random sample, of type <see cref="float"/>.</returns>
    public float NextFloatNonZero()
    {
        // Here we generate a random float in the interval [0, 1 - (1 / 2^24)], and add INCR_FLOAT
        // to produce a value in the interval [1 / 2^24, 1]
        return NextFloat() + INCR_FLOAT;
    }

    /// <summary>
    /// Returns a random <see cref="Half"/> sampled from the uniform distribution with interval (0, 1],
    /// i.e., exclusive of 0.0, and inclusive of 1.0.
    /// </summary>
    /// <returns>A new random sample, of type <see cref="Half"/>.</returns>
    public Half NextHalf()
    {
        // Here we generate a random integer between 0 and (2^11)-1 (i.e., 11 binary 1s) and multiply
        // by the fractional unit value 1.0 / 2^11, thus the result has a max value of
        // 1.0 - (1.0 / 2^11).
        return (Half)((NextULongInner() >> 53) * INCR_HALF);
    }

    /// <summary>
    /// Returns a random <see cref="Half"/> sampled from the uniform distribution with interval (0, 1],
    /// i.e., exclusive of 0.0, and inclusive of 1.0.
    /// </summary>
    /// <returns>A new random sample, of type <see cref="Half"/>.</returns>
    public Half NextHalfNonZero()
    {
        // Here we generate a random float in the interval [0, 1-(1 / 2^24)], and add INCR_FLOAT
        // to produce a value in the interval [(1 / 2^24), 1]
        return (Half)(((NextULongInner() >> 53) * INCR_HALF) + INCR_HALF);
    }

    /// <summary>
    /// Returns a random <see cref="double"/> sampled from the uniform distribution with interval (0, 1],
    /// i.e., exclusive of 0.0, and inclusive of 1.0.
    /// </summary>
    /// <returns>A new random sample, of type <see cref="double"/>.</returns>
    public double NextDoubleNonZero()
    {
        // Here we generate a random double in the interval [0, 1 - (1 / 2^53)], and add INCR_DOUBLE
        // to produce a value in the interval [1 / 2^53, 1]
        return NextDoubleInner() + INCR_DOUBLE;
    }

    /// <summary>
    /// Returns a random <see cref="double"/> sampled from the uniform distribution with interval [0, 1),
    /// i.e., inclusive of 0.0 and exclusive of 1.0, and using an alternative high-resolution sampling method.
    /// </summary>
    /// <returns>A new random sample, of type <see cref="double"/>.</returns>
    /// <remarks>
    /// Uses an alternative sampling method that is capable of generating all possible values in the
    /// interval [0,1] that can be represented by a double precision float. Note however that this method
    /// is significantly slower than NextDouble().
    /// </remarks>
    public double NextDoubleHighRes()
    {
        // Notes.
        // An alternative sampling method from:
        //
        //    2014, Taylor R Campbell
        //
        //    Uniform random floats:  How to generate a double-precision
        //    floating-point number in [0, 1] uniformly at random given a uniform
        //    random source of bits.
        //
        //    https://mumble.net/~campbell/tmp/random_real.c
        //
        // The basic idea is that we generate a string of binary digits and use them to construct a
        // base two number of the form:
        //
        //    0.{digits}
        //
        // The digits are generated in blocks of 64 bits. If all 64 bits in a block are zero then a
        // running exponent value is reduced by 64 and another 64 bits are generated. This process is
        // repeated until a block with non-zero bits is produced, or the exponent value falls below -1074.
        //
        // The final step is to create the IEE754 double precision variable from a 64 bit significand
        // (the most recent and thus significant 64 bits), and the running exponent.
        //
        // This scheme is capable of generating all possible values in the interval [0,1) that can be
        // represented by a double precision float, and without bias. There are a little under 2^62
        // possible discrete values, and this compares to the 2^53 possible values than can be generated by
        // NextDouble(), however the scheme used in this method is much slower, so is likely of interest
        // in specialist scenarios.
        int exponent = -64;
        ulong significand;
        int shift;

        // Read zeros into the exponent until we hit a one; the rest
        // will go into the significand.
        while((significand = NextULongInner()) == 0)
        {
            exponent -= 64;

            // If the exponent falls below -1074 = emin + 1 - p,
            // the exponent of the smallest subnormal, we are
            // guaranteed the result will be rounded to zero.  This
            // case is so unlikely it will happen in realistic
            // terms only if random64 is broken.
            if(exponent < -1074)
                return 0;
        }

        // There is a 1 somewhere in significand, not necessarily in
        // the most significant position.  If there are leading zeros,
        // shift them into the exponent and refill the less-significant
        // bits of the significand.  Can't predict one way or another
        // whether there are leading zeros: there's a fifty-fifty
        // chance, if random64 is uniformly distributed.
        shift = BitOperations.LeadingZeroCount(significand);
        if(shift != 0)
        {
            exponent -= shift;
            significand <<= shift;
            significand |= NextULongInner() >> (64 - shift);
        }

        // Set the sticky bit, since there is almost surely another 1
        // in the bit stream.  Otherwise, we might round what looks
        // like a tie to even when, almost surely, were we to look
        // further in the bit stream, there would be a 1 breaking the
        // tie.
        significand |= 1;

        // Finally, convert to double (rounding) and scale by
        // 2^exponent.
        return (double)significand * Math.Pow(2, exponent);
    }

    /// <summary>
    /// Returns a random value sampled from the uniform distribution with interval [0, 1),
    /// i.e., inclusive of 0.0 and exclusive of 1.0.
    /// </summary>
    /// <typeparam name="T">The numeric data type.</typeparam>
    /// <returns>A new random sample, of type <typeparamref name="T"/>.</returns>
    public T NextUnitInterval<T>()
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        if(typeof(T) == typeof(double))
        {
            return T.CreateChecked(NextDoubleInner());
        }
        else if(typeof(T) == typeof(float))
        {
            return T.CreateChecked(NextFloat());
        }
        else if(typeof(T) == typeof(Half))
        {
            return T.CreateChecked(NextHalf());
        }
        else
        {
            throw new ArgumentException("Unsupported type argument");
        }
    }

    /// <summary>
    /// Returns a random value sampled from the uniform distribution with interval (0, 1],
    /// i.e., exclusive of 0.0, and inclusive of 1.0.
    /// </summary>
    /// <typeparam name="T">The numeric data type.</typeparam>
    /// <returns>A new random sample, of type <typeparamref name="T"/>.</returns>
    public T NextUnitIntervalNonZero<T>()
        where T : struct, IBinaryFloatingPointIeee754<T>
    {
        if(typeof(T) == typeof(double))
        {
            return T.CreateChecked(NextDoubleNonZero());
        }
        else if(typeof(T) == typeof(float))
        {
            return T.CreateChecked(NextFloatNonZero());
        }
        else if(typeof(T) == typeof(Half))
        {
            return T.CreateChecked(NextHalfNonZero());
        }
        else
        {
            throw new ArgumentException("Unsupported type argument");
        }
    }

    #endregion

    #region Private Methods

    private int NextInner(int maxValue)
    {
        // Handle special case of a single sample value.
        if(maxValue == 1)
            return 0;

        // Notes.
        // Here we sample an integer value within the interval [0, maxValue). Rejection sampling is used in
        // order to produce unbiased samples. An alternative approach is:
        //
        //  return (int)(NextDoubleInner() * maxValue);
        //
        // I.e. generate a double precision float in the interval [0,1) and multiply by maxValue. However the
        // use of floating point arithmetic will introduce bias therefore this method is not used.
        //
        // The rejection sampling method used here operates as follows:
        //
        //  1) Calculate N such that  2^(N-1) < maxValue <= 2^N, i.e. N is the minimum number of bits required
        //     to represent maxValue states.
        //  2) Generate an N bit random sample.
        //  3) Reject samples that are >= maxValue, and goto (2) to re-sample.
        //
        // Repeat until a valid sample is generated.

        // Log2Ceiling(numberOfStates) gives the number of bits required to represent maxValue states.
        int bitCount = MathUtils.Log2Ceiling((uint)maxValue);

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
        // Handle special case of a single sample value.
        if(maxValue == 1)
            return 0;

        // See comments on NextInner(int) for details.

        // Log2Ceiling(numberOfStates) gives the number of bits required to represent maxValue states.
        int bitCount = MathUtils.Log2Ceiling((ulong)maxValue);

        // Rejection sampling loop.
        long x;
        do
        {
            x = (long)(NextULongInner() >> (64 - bitCount));
        }
        while(x >= maxValue);

        return x;
    }

    private double NextDoubleInner()
    {
        // Notes.
        // Here we generate a random integer in the interval [0, 2^53-1]  (i.e., the max value is 53 binary 1s),
        // and multiply by the fractional value 1.0 / 2^53, thus the resulting random has a min value of 0.0 and a max
        // value of 1.0 - (1.0 / 2^53).
        //
        // From http://prng.di.unimi.it/:
        // "A standard double (64-bit) floating-point number in IEEE floating point format has 52 bits of significand,
        //  plus an implicit bit at the left of the significand. Thus, the representation can actually store numbers with
        //  53 significant binary digits. Because of this fact, in C99 a 64-bit unsigned integer x should be converted to
        //  a 64-bit double using the expression
        //  (x >> 11) *0x1.0p-53"
        return (NextULongInner() >> 11) * INCR_DOUBLE;
    }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// Get the next 64 random bits from the underlying PRNG. This method forms the foundation for most of the methods of each
    /// <see cref="IRandomSource"/> implementation, which take these 64 bits and manipulate them to provide random values of various
    /// data types, such as integers, byte arrays, floating point values, etc.
    /// </summary>
    /// <returns>A <see cref="ulong"/> containing random bits from the underlying PRNG algorithm.</returns>
    protected abstract ulong NextULongInner();

    #endregion
}
