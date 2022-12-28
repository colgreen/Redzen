// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.Random;

/// <summary>
/// A factory of XorShiftRandom instances.
/// </summary>
[Obsolete("Superseded by Xoshiro256StarStarRandomFactory (comparable performance, but passes more statistical tests and has a longer period)")]
public sealed class XorShiftRandomFactory : IRandomSourceFactory
{
    readonly IRandomSeedSource _seedSource;

    /// <summary>
    /// Construct with a default seed source.
    /// </summary>
    public XorShiftRandomFactory()
    {
        _seedSource = new DefaultRandomSeedSource();
    }

    /// <summary>
    /// Construct with the given seed source.
    /// </summary>
    /// <param name="seedSource">Random seed source.</param>
    public XorShiftRandomFactory(
        IRandomSeedSource seedSource)
    {
        _seedSource = seedSource;
    }

    /// <inheritdoc/>
    public IRandomSource Create()
    {
        ulong seed = _seedSource.GetSeed();
        return new XorShiftRandom(seed);
    }

    /// <inheritdoc/>
    public IRandomSource Create(ulong seed)
    {
        return new XorShiftRandom(seed);
    }
}
