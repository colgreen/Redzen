// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.Random;

/// <summary>
/// A factory of Xoshiro512StarStarRandom instances.
/// </summary>
public class Xoshiro512StarStarRandomFactory : IRandomSourceFactory
{
    readonly IRandomSeedSource _seedSource;

    #region Constructors

    /// <summary>
    /// Construct with a default seed source.
    /// </summary>
    public Xoshiro512StarStarRandomFactory()
    {
        _seedSource = new DefaultRandomSeedSource();
    }

    /// <summary>
    /// Construct with the given seed source.
    /// </summary>
    /// <param name="seedSource">Random seed source.</param>
    public Xoshiro512StarStarRandomFactory(
        IRandomSeedSource seedSource)
    {
        _seedSource = seedSource;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a new instance of <see cref="Xoshiro512StarStarRandom"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="Xoshiro512StarStarRandom"/>.</returns>
    public IRandomSource Create()
    {
        ulong seed = _seedSource.GetSeed();
        return new Xoshiro512StarStarRandom(seed);
    }

    /// <summary>
    /// Creates a new instance of <see cref="Xoshiro512StarStarRandom"/> with the given PRNG seed.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    /// <returns>A new instance of <see cref="Xoshiro512StarStarRandom"/>.</returns>
    public IRandomSource Create(ulong seed)
    {
        return new Xoshiro512StarStarRandom(seed);
    }

    #endregion
}
