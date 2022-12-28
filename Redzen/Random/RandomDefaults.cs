// This file is part of the Redzen code library; Copyright Colin D. Green.
// See LICENSE.txt for details.
namespace Redzen.Random;

/// <summary>
/// Provides a means of creating default implementations of <see cref="IRandomSource"/>, and also a standard way of generating seed values for PRNGs generally.
/// </summary>
public static class RandomDefaults
{
    /// <summary>
    /// A static default IRandomSeedSource instance for use anywhere within the current process.
    /// </summary>
    public static readonly IRandomSeedSource DefaultRandomSeedSource;

    /// <summary>
    /// A static default instance for use anywhere within the current process.
    /// </summary>
    public static readonly IRandomSourceFactory DefaultRandomSourceFactory;

    static RandomDefaults()
    {
        DefaultRandomSeedSource = new DefaultRandomSeedSource();
        DefaultRandomSourceFactory = new Xoshiro256StarStarRandomFactory(DefaultRandomSeedSource);
    }

    /// <summary>
    /// Get a new seed value.
    /// </summary>
    /// <returns>A random <see cref="ulong"/> suitable for seeding a PRNG.</returns>
    public static ulong GetSeed()
    {
        return DefaultRandomSeedSource.GetSeed();
    }

    /// <summary>
    /// Creates a new <see cref="IRandomSource"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="IRandomSource"/>.</returns>
    public static IRandomSource CreateRandomSource()
    {
        return DefaultRandomSourceFactory.Create();
    }

    /// <summary>
    /// Creates a new <see cref="IRandomSource"/> with the given PRNG seed.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    /// <returns>A new instance of <see cref="IRandomSource"/>.</returns>
    public static IRandomSource CreateRandomSource(ulong seed)
    {
        return DefaultRandomSourceFactory.Create(seed);
    }
}
