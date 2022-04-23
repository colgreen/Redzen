// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.Random;

/// <summary>
/// A factory of IRandomSource instances.
/// </summary>
public interface IRandomSourceFactory
{
    /// <summary>
    /// Creates a new <see cref="IRandomSource"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="IRandomSource"/>.</returns>
    IRandomSource Create();

    /// <summary>
    /// Creates a new <see cref="IRandomSource"/> with the given PRNG seed.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    /// <returns>A new instance of <see cref="IRandomSource"/>.</returns>
    IRandomSource Create(ulong seed);
}
