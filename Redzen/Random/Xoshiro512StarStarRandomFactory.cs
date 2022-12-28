// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.Random;

/// <summary>
/// A factory of Xoshiro512StarStarRandom instances.
/// </summary>
public sealed class Xoshiro512StarStarRandomFactory : IRandomSourceFactory
{
    readonly IRandomSeedSource _seedSource;

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

    /// <inheritdoc/>
    public IRandomSource Create()
    {
        ulong seed = _seedSource.GetSeed();
        return new Xoshiro512StarStarRandom(seed);
    }

    /// <inheritdoc/>
    public IRandomSource Create(ulong seed)
    {
        return new Xoshiro512StarStarRandom(seed);
    }
}
