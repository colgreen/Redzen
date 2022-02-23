/* ***************************************************************************
 * This file is part of the Redzen code library.
 *
 * Copyright 2015-2022 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */

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

    #region Static Initialiser

    static RandomDefaults()
    {
        DefaultRandomSeedSource = new DefaultRandomSeedSource();
        DefaultRandomSourceFactory = new Xoshiro256StarStarRandomFactory(DefaultRandomSeedSource);
    }

    #endregion

    #region Public Static Methods

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

    #endregion
}
