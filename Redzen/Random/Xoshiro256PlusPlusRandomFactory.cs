// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.Random;

/// <summary>
/// A factory of Xoshiro256PlusPlusFactory instances.
/// </summary>
public class Xoshiro256PlusPlusRandomFactory : IRandomSourceFactory
{
    readonly IRandomSeedSource _seedSource;

    #region Constructors

    /// <summary>
    /// Construct with a default seed source.
    /// </summary>
    public Xoshiro256PlusPlusRandomFactory()
    {
        _seedSource = new DefaultRandomSeedSource();
    }

    /// <summary>
    /// Construct with the given seed source.
    /// </summary>
    /// <param name="seedSource">Random seed source.</param>
    public Xoshiro256PlusPlusRandomFactory(
        IRandomSeedSource seedSource)
    {
        _seedSource = seedSource;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a new instance of <see cref="Xoshiro256PlusPlusRandom"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="Xoshiro256PlusPlusRandom"/>.</returns>
    public IRandomSource Create()
    {
        ulong seed = _seedSource.GetSeed();
        return new Xoshiro256PlusPlusRandom(seed);
    }

    /// <summary>
    /// Creates a new instance of <see cref="Xoshiro256PlusPlusRandom"/> with the given PRNG seed.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    /// <returns>A new instance of <see cref="Xoshiro256PlusPlusRandom"/>.</returns>
    public IRandomSource Create(ulong seed)
    {
        return new Xoshiro256PlusPlusRandom(seed);
    }

    #endregion
}
