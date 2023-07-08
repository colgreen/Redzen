// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
using System.Numerics;

namespace Redzen.Random;

/// <summary>
/// A source of random values.
/// </summary>
public interface IRandomSource
{
    /// <summary>
    /// Re-initialises the random number generator state using the provided seed value.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    void Reinitialise(ulong seed);

    #region Public Methods [System.Random equivalent methods]

    /// <summary>
    /// Returns a random integer sampled from the uniform distribution with interval [0, int.MaxValue),
    /// i.e., exclusive of <see cref="Int32.MaxValue"/>.
    /// </summary>
    /// <returns>A new random sample.</returns>
    int Next();

    /// <summary>
    /// Returns a random integer sampled from the uniform distribution with interval [0, maxValue),
    /// i.e., exclusive of <paramref name="maxValue"/>.
    /// </summary>
    /// <param name="maxValue">The maximum value to be sampled (exclusive).</param>
    /// <returns>A new random sample.</returns>
    int Next(int maxValue);

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
    int Next(int minValue, int maxValue);

    /// <summary>
    /// Returns a random <see cref="double"/> sampled from the uniform distribution with interval [0, 1),
    /// i.e., inclusive of 0.0 and exclusive of 1.0.
    /// </summary>
    /// <returns>A new random sample, of type <see cref="double"/>.</returns>
    double NextDouble();

    /// <summary>
    /// Fills the provided span with random byte values, sampled from the uniform distribution with interval [0, 255].
    /// </summary>
    /// <param name="span">The byte span to fill with random samples.</param>
    void NextBytes(Span<byte> span);

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
    int NextInt();

    /// <summary>
    /// Returns a random <see cref="uint"/> sampled from the uniform distribution with interval [0, uint.MaxValue],
    /// i.e., over the full range of possible uint values.
    /// </summary>
    /// <returns>A new random sample.</returns>
    uint NextUInt();

    /// <summary>
    /// Returns a random <see cref="ulong"/> sampled from the uniform distribution with interval [0, ulong.MaxValue],
    /// i.e., over the full range of possible ulong values.
    /// </summary>
    /// <returns>A new random sample.</returns>
    ulong NextULong();

    /// <summary>
    /// Returns a random boolean sampled from the uniform discrete distribution {false, true}, i.e., a fair coin flip.
    /// </summary>
    /// <returns>A new random sample.</returns>
    /// <remarks>
    /// Returns a sample the Bernoulli distribution with p = 0.5; also known as a a fair coin flip.
    /// </remarks>
    bool NextBool();

    /// <summary>
    /// Returns a random byte value sampled from the uniform distribution [0, 255].
    /// </summary>
    /// <returns>A new random sample.</returns>
    byte NextByte();

    /// <summary>
    /// Returns a random <see cref="float"/> sampled from the uniform distribution with interval [0, 1),
    /// i.e., inclusive of 0.0 and exclusive of 1.0.
    /// </summary>
    /// <returns>A new random sample, of type <see cref="float"/>.</returns>
    float NextFloat();

    /// <summary>
    /// Returns a random <see cref="float"/> sampled from the uniform distribution with interval (0, 1],
    /// i.e., exclusive of 0.0, and inclusive of 1.0.
    /// </summary>
    /// <returns>A new random sample, of type <see cref="float"/>.</returns>
    float NextFloatNonZero();

    /// <summary>
    /// Returns a random <see cref="double"/> sampled from the uniform distribution with interval (0, 1],
    /// i.e., exclusive of 0.0, and inclusive of 1.0.
    /// </summary>
    /// <returns>A new random sample, of type <see cref="double"/>.</returns>
    double NextDoubleNonZero();

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
    double NextDoubleHighRes();

    /// <summary>
    /// Returns a random value sampled from the uniform distribution with interval [0, 1),
    /// i.e., inclusive of 0.0 and exclusive of 1.0.
    /// </summary>
    /// <typeparam name="T">The numeric data type.</typeparam>
    /// <returns>A new random sample, of type <typeparamref name="T"/>.</returns>
    T NextUnitInterval<T>()
        where T : struct, IBinaryFloatingPointIeee754<T>;

    /// <summary>
    /// Returns a random value sampled from the uniform distribution with interval (0, 1],
    /// i.e., exclusive of 0.0, and inclusive of 1.0.
    /// </summary>
    /// <typeparam name="T">The numeric data type.</typeparam>
    /// <returns>A new random sample, of type <typeparamref name="T"/>.</returns>
    T NextUnitIntervalNonZero<T>()
        where T : struct, IBinaryFloatingPointIeee754<T>;

    #endregion
}
