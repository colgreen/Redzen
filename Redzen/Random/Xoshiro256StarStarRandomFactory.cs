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
/// A factory of Xoshiro256StarStarRandom instances.
/// </summary>
public class Xoshiro256StarStarRandomFactory : IRandomSourceFactory
{
    readonly IRandomSeedSource _seedSource;

    #region Constructors

    /// <summary>
    /// Construct with a default seed source.
    /// </summary>
    public Xoshiro256StarStarRandomFactory()
    {
        _seedSource = new DefaultRandomSeedSource();
    }

    /// <summary>
    /// Construct with the given seed source.
    /// </summary>
    /// <param name="seedSource">Random seed source.</param>
    public Xoshiro256StarStarRandomFactory(
        IRandomSeedSource seedSource)
    {
        _seedSource = seedSource;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a new instance of <see cref="Xoshiro256StarStarRandom"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="Xoshiro256StarStarRandom"/>.</returns>
    public IRandomSource Create()
    {
        ulong seed = _seedSource.GetSeed();
        return new Xoshiro256StarStarRandom(seed);
    }

    /// <summary>
    /// Creates a new instance of <see cref="Xoshiro256StarStarRandom"/> with the given PRNG seed.
    /// </summary>
    /// <param name="seed">Seed value.</param>
    /// <returns>A new instance of <see cref="Xoshiro256StarStarRandom"/>.</returns>
    public IRandomSource Create(ulong seed)
    {
        return new Xoshiro256StarStarRandom(seed);
    }

    #endregion
}
